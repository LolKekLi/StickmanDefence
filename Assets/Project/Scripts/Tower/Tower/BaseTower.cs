using System;
using System.Collections;
using UnityEngine;
using Zenject;

namespace Project
{
    public abstract class BaseTower : PooledBehaviour, IUpgradeable
    {
        private static readonly string BaseColorID = "_BaseColor";
        private static readonly string OutLineWidthID = "_OutlineWidth";
        private static readonly string OutLineColorID = "_OutlineColor";
        private static readonly string RenderModID = "_RenderingMode";

        [SerializeField]
        private AttackRadius _attackRadius = null;
        [SerializeField]
        private LayerMask _towerLayerMask = default;
        [SerializeField]
        private LayerMask _xrayLayerMask = default;

        [SerializeField]
        private ParticleSystem _onChangeVisualFx;

        private bool _isTriggered = false;
        private int _towerLayer;
        private int _xRayLayer;

        private Vector3 _velocity = Vector3.zero;
        private Action _onBuildAction = null;
        private Quaternion _startRotation = default;
        private Action _onCellAction;

        private TowerSettings _towerSettings = null;
        private TowerSettings.TowerPreset _towerPreset = null;
        private PoolManager _poolManager = null;
        private Coroutine _attackCor = null;
        private Coroutine _lookAtEnemyCor = null;
        private Coroutine _rotateToOrigineCor = null;

        private Collider _collider;
        private UpdateInfo _updateInfo = null;

        private BaseAttackController _attackController;
        private TowerViewModel _towerViewModel;
        private TowerHighLightSettings _highLightSettings;

        [field: SerializeField]
        public CantSpawnZone CantSpawnZone
        {
            get;
            private set;
        }

        public abstract TowerType TowerType
        {
            get;
        }
        
        public TowerViewModelType TowerViewModelType
        {
            get =>
                _towerViewModel.ModelType;
        }

        public bool IsCanSpawn
        {
            get;
            private set;
        } = true;

        public bool IsSpawned
        {
            get;
            private set;
        }

        public float SqrAttackRadius
        {
            get
            {
                return _attackRadius.SqrAttackRadius;
                return _towerPreset.FirePreset.AttackRadius * _towerPreset.FirePreset.AttackRadius;
            }
        }

        public Enemy Target
        {
            get =>
                _attackController.Target;
        }

        public bool IsAttack
        {
            get =>
                _attackCor != null;
        }

        public bool IsLostTarget
        {
            get =>
                (transform.position - Target.transform.position).sqrMagnitude >=
                SqrAttackRadius;
        }

        public abstract bool IsAssTower
        {
            get;
        }


        [Inject]
        private void Construct(TowerHighLightSettings highLightSettings)
        {
            _highLightSettings = highLightSettings;
        }

        public override void Prepare(PooledObjectType pooledType)
        {
            base.Prepare(pooledType);

            _collider = GetComponent<Collider>();
        }

        public override void SpawnFromPool()
        {
            base.SpawnFromPool();

            _collider.enabled = false;
        }

        public void OnGetTowerFromPool(TowerSettings towerSettings, PoolManager poolManager, Action onBuildAction,
            Action onCellAction, TowerViewModel towerViewModel)
        {
            SetupLayers();

            _startRotation = transform.rotation;
            _poolManager = poolManager;
            _towerSettings = towerSettings;

            _towerPreset = _towerSettings.GetTowerPresetByType(TowerType);

            _updateInfo = new UpdateInfo();

            _attackRadius.Setup(_towerPreset.FirePreset.AttackRadius);

            IsSpawned = false;

            RefreshTowerViewModel(towerViewModel);

            ChangeInteractionState(TowerInteractionState.Spawned);

            _onBuildAction = onBuildAction;
            _onCellAction = onCellAction;
        }

        public void ChangeViewModel(TowerViewModel towerViewModel, BaseAttackController attackController,
            FirePreset firePreset)
        {
            _onChangeVisualFx?.Play();
            
            _towerViewModel.Free();

            RefreshTowerViewModel(towerViewModel);

            _towerViewModel.OnBuild();

            SetupAttackController(attackController, firePreset);
        }

        private void RefreshTowerViewModel(TowerViewModel towerViewModel)
        {
            _towerViewModel = towerViewModel;
            _towerViewModel.transform.localPosition = Vector3.zero;
            _towerViewModel.transform.localRotation = Quaternion.identity;
            _towerViewModel.OnGetTowerFromPool(_towerLayer, _xRayLayer, _highLightSettings, _attackRadius);
        }

        private void SetupLayers()
        {
            _towerLayer = _towerLayerMask.GetFirstLayerNumber();

            if (_towerLayer < 0)
            {
                Debug.LogError("В _towerLayerMask  не указан ни один слой");
            }

            _xRayLayer = _xrayLayerMask.GetFirstLayerNumber();

            if (_xRayLayer < 0)
            {
                Debug.LogError("В _xRayLayer  не указан ни один слой");
            }
        }

        public virtual void OnBuild(BaseAttackController attackController)
        {
            _collider.enabled = true;

            IsSpawned = true;

            _towerViewModel.OnBuild();

            ChangeInteractionState(TowerInteractionState.UnSelected);

            SetupAttackController(attackController, _towerPreset.FirePreset);

            _onBuildAction?.Invoke();
        }

        private void SetupAttackController(BaseAttackController attackController, FirePreset firePreset)
        {
            Enemy tempTarget = null;

            if (_attackController != null)
            {
                tempTarget = _attackController.Target;
            }

            _attackController = attackController;
            _attackController.Setup(firePreset.BulletSettings, _towerViewModel.FirePosition, transform, firePreset,
                _poolManager);

            _attackController.Target = tempTarget;
        }

        public void Move(Vector3 targetPosition)
        {
            transform.position =
                Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, _towerSettings.SmoothTime);
        }

        public void Attack()
        {
            if (_rotateToOrigineCor != null)
            {
                StopCoroutine(_rotateToOrigineCor);
                _rotateToOrigineCor = null;
            }

            _lookAtEnemyCor = StartCoroutine(LookAtTargetCor());
            _attackController.RefreshAttackDelay();
            _attackCor = StartCoroutine(AttackCor());

            _towerViewModel.OnFire();
        }

        public void ToggleSpawnAbility(bool isCanSpawn)
        {
            if (IsSpawned)
            {
                return;
            }

            IsCanSpawn = isCanSpawn;
            ChangeInteractionState(isCanSpawn ? TowerInteractionState.Selected : TowerInteractionState.CantSpawn);
        }

        public void ChangeTarget(Enemy enemy)
        {
            _attackController.Target = enemy;
        }

        public void StopAttack()
        {
            _towerViewModel.OnFireEnded();
            _attackController.Target = null;

            if (_attackCor != null)
            {
                StopCoroutine(_attackCor);
                _attackCor = null;
            }

            if (_lookAtEnemyCor != null)
            {
                StopCoroutine(_lookAtEnemyCor);
                _lookAtEnemyCor = null;
            }

            if (_rotateToOrigineCor == null)
            {
                _rotateToOrigineCor = StartCoroutine(RotateToOrigin());
            }
        }

        public virtual void Sell()
        {
            _onCellAction?.Invoke();
            StopAttack();
            Free();
        }

        private IEnumerator AttackCor()
        {
            while (!Target.IsDied)
            {
                yield return _attackController.AttackDelay;

                if (Target.IsDied)
                {
                    break;
                }

                _attackController.Fire();
            }

            Debug.Log("Attack cor stop attack");

            StopAttack();
        }

        private IEnumerator LookAtTargetCor()
        {
            while (!Target.IsDied)
            {
                transform.LookAt(Target.transform.position);

                yield return null;
            }

            _lookAtEnemyCor = null;
        }

        private IEnumerator RotateToOrigin()
        {
            float time = 0;
            float timeCor = 1;
            float progress = 0;

            while (time <= timeCor)
            {
                time += Time.deltaTime;
                progress = time / timeCor;

                transform.rotation = Quaternion.Lerp(transform.rotation, _startRotation, progress);

                yield return null;
            }

            _rotateToOrigineCor = null;
        }

        void IUpgradeable.UnSelected()
        {
            ChangeInteractionState(TowerInteractionState.UnSelected);
        }

        void IUpgradeable.Selected()
        {
            ChangeInteractionState(TowerInteractionState.Selected);
        }

        string IUpgradeable.GetTransformName()
        {
            return transform.name;
        }

        int IUpgradeable.GetUpgradeLevel(UpgradeLinePerkType perkLineType)
        {
            return _updateInfo.LevelInfo[perkLineType];
        }

        void IUpgradeable.Upgrade(UpgradeLinePerkType upgradeLinePerkType,
            TowerUpgradeSettings.UpdatePreset[] presetByLineType,
            Action<TowerViewModelType, FirePreset> changeViewModelCallback)
        {
            var i = _updateInfo.LevelInfo[upgradeLinePerkType];
            var updatePreset = presetByLineType[i];
            var updateType = updatePreset.UpdateType;
            
            switch (updateType)
            {
                case UpdateType.Damage:
                    UpdateDamage(updatePreset.Value);
                    break;
                case UpdateType.FireSpeed:
                    UpdateFireSpeed(updatePreset.Value);
                    break;
                case UpdateType.FireRadius:
                    UpdateFireRadius(updatePreset.Value);
                    break;
            }

            if (updatePreset.NeedChangeVisual)
            {
                if (updatePreset.FirePreset != null)
                    changeViewModelCallback.Invoke(updatePreset.TowerMeshType, updatePreset.FirePreset);
            }

            _updateInfo.Update(upgradeLinePerkType);
        }


        private void UpdateFireRadius(float updatePresetValue)
        {
            _attackRadius.UpdateFireRadius(updatePresetValue);
        }

        private void UpdateFireSpeed(float updatePresetValue)
        {
            _attackController.UpdateFireSpeed(updatePresetValue);
            _attackController.RefreshAttackDelay();
        }

        private void UpdateDamage(float updatePresetValue)
        {
            _attackController.UpdateDamage(updatePresetValue);
        }

        UpdateInfo IUpgradeable.GetUpgradeInfo()
        {
            return _updateInfo;
        }

        private void ChangeInteractionState(TowerInteractionState interactionState)
        {
            _towerViewModel.ChangeInteractionState(interactionState);
        }

        public bool SeeTarget(Vector3 targetPos)
        {
            return (transform.position.ChangeY(targetPos.y) -
                    targetPos)
                .sqrMagnitude <= SqrAttackRadius;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_towerPreset != null)
            {
                Gizmos.DrawWireSphere(transform.position,_attackRadius.Radius);
            }
        }
#endif
    }
}