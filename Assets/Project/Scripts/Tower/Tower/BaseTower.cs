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
        private Transform _firePosition = null;

        [SerializeField]
        private AttackRadius _attackRadius = null;

        [SerializeField]
        private SkinnedMeshRenderer _skinnedMeshRenderer = null;

        [SerializeField]
        private LayerMask _towerLayerMask = default;

        [SerializeField]
        private LayerMask _xrayLayerMask = default;

        private bool _isTriggered = false;
        private int _towerLayer;
        private int _xRayLayer;

        private Vector3 _velocity = Vector3.zero;
        private Color _startColor = default;
        private Color _spawnColor = default;
        private Action _onBuildAction = null;
        private Quaternion _startRotation = default;

        private TowerSettings _towerSettings = null;
        private TowerSettings.TowerPreset _towerPreset = null;
        private MaterialPropertyBlock _materialPropertyBlock;
        
        private BulletSettings _bulletPreset = null;
        private PoolManager _poolManager = null;

        private Coroutine _attackCor = null;
        private Coroutine _lookAtEnemyCor = null;
        private Coroutine _rotateToOrigineCor = null;
        private Action _onCellAction;
        private Collider _collider;
        private TowerHighLightSettings _highLightSettings;

        [field: SerializeField]
        public CantSpawnZone CantSpawnZone
        {
            get;
            private set;
        }

        public abstract TowerType Type
        {
            get;
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
            get =>
                _towerPreset.AttackRadius * _towerPreset.AttackRadius;
        }

        public Enemy Target
        {
            get;
            private set;
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

        [Inject]
        private void Construct(TowerHighLightSettings highLightSettings)
        {
            _highLightSettings = highLightSettings; 
        }

        public override void Prepare(PooledObjectType pooledType)
        {
            base.Prepare(pooledType);

            _collider = GetComponent<Collider>();
            _materialPropertyBlock = new MaterialPropertyBlock();

            _startColor = _skinnedMeshRenderer.material.color;
            _spawnColor = new Color(_startColor.r, _startColor.g, _startColor.b, _startColor.a / 2);
        }

        public override void SpawnFromPool()
        {
            base.SpawnFromPool();

            _collider.enabled = false;
            _skinnedMeshRenderer.GetPropertyBlock(_materialPropertyBlock);
            _materialPropertyBlock.SetFloat(OutLineWidthID, 0.5f);
            _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
        }

        public void OnGetTowerFromPool(TowerSettings towerSettings, PoolManager poolManager, Action onBuildAction,
            Action onCellAction)
        {
            SetupLayers();

            _skinnedMeshRenderer.gameObject.layer = _towerLayer;
            _startRotation = transform.rotation;
            _poolManager = poolManager;
            _towerSettings = towerSettings;

            _towerPreset = _towerSettings.GetTowerPresetByType(Type);
            _bulletPreset = _towerPreset.BulletSettings;

            _attackRadius.Setup(_towerPreset.AttackRadius);

            IsSpawned = false;

            _skinnedMeshRenderer.GetPropertyBlock(_materialPropertyBlock);
            _materialPropertyBlock.SetColor(BaseColorID, _spawnColor);

            ChangeInteractionState(TowerInteractionState.Spawned);

            _onBuildAction = onBuildAction;
            _onCellAction = onCellAction;
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

        public void OnBuild()
        {
            _collider.enabled = true;
            _skinnedMeshRenderer.GetPropertyBlock(_materialPropertyBlock);
            _materialPropertyBlock.SetColor(BaseColorID, _startColor);

            IsSpawned = true;

            _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);

            ChangeInteractionState(TowerInteractionState.UnSelected);

            _onBuildAction?.Invoke();
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
            _attackCor = StartCoroutine(AttackCor(_bulletPreset));
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
            Target = enemy;
        }

        public void StopAttack()
        {
            Target = null;

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

        public void Cell()
        {
            _onCellAction?.Invoke();
            StopAttack();
            Free();
        }

        private IEnumerator AttackCor(BulletSettings bulletPreset)
        {
            var waiter = new WaitForSeconds(1 / _towerPreset.FireRate);
        
            while (!Target.IsDied)
            {
                yield return waiter;
        
                if (Target.IsDied)
                {
                    break;
                }
        
                var bullet = _poolManager.Get<Bullet>(_poolManager.PoolSettings.Bullet, _firePosition.position,
                    Quaternion.identity);
        
                bullet.Setup(_bulletPreset);
                bullet.Shoot((Target.transform.position - transform.position).normalized);
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

        private void ChangeInteractionState(TowerInteractionState interactionState)
        {
            _skinnedMeshRenderer.GetPropertyBlock(_materialPropertyBlock);

            _skinnedMeshRenderer.gameObject.layer =
                interactionState == TowerInteractionState.CantSpawn ? _xRayLayer : _towerLayer;

            switch (interactionState)
            {
                case TowerInteractionState.Spawned:
                    _materialPropertyBlock.SetColor(BaseColorID, _spawnColor);
                    _materialPropertyBlock.SetColor(OutLineColorID,
                        _highLightSettings.GetHighlightPresetByType(interactionState).Color);
                    _attackRadius.ToggleActive(true);
                    _attackRadius.ChangeColor(true);
                    break;

                case TowerInteractionState.Selected:
                    _materialPropertyBlock.SetColor(OutLineColorID,
                        _highLightSettings.GetHighlightPresetByType(interactionState).Color);
                    _attackRadius.ToggleActive(true);
                    _attackRadius.ChangeColor(true);

                    break;
                case TowerInteractionState.UnSelected:
                    _materialPropertyBlock.SetColor(BaseColorID, _startColor);
                    _materialPropertyBlock.SetColor(OutLineColorID,
                        _highLightSettings.GetHighlightPresetByType(interactionState).Color);
                    _attackRadius.ToggleActive(false);
                    break;
                case TowerInteractionState.CantSpawn:
                    _materialPropertyBlock.SetColor(OutLineColorID,
                        _highLightSettings.GetHighlightPresetByType(interactionState).Color);
                    _attackRadius.ChangeColor(false);
                    break;
            }

            _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
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
                Gizmos.DrawWireSphere(transform.position, _towerPreset.AttackRadius);
            }

            if (Target != null && !Target.IsDied)
            {
                Gizmos.DrawLine(transform.position, Target.transform.position);
            }
        }
#endif
    }
}