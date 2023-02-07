using System;
using System.Collections;
using UnityEngine;
using Zenject;

namespace Project
{
    public abstract class BaseTower : PooledBehaviour
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

        private bool _isTriggered = false;

        private Vector3 _velocity = Vector3.zero;
        private Color _startColor = default;
        private Color _spawnColor = default;
        private Action _onSpawnAction = null;
        private Quaternion _startRotation = default;

        private TowerSettings _towerSettings = null;
        private TowerSettings.TowerPreset _towerPreset = null;
        private MaterialPropertyBlock _materialPropertyBlock;
        private BulletSettings _bulletSettings = null;
        private BulletSettings.BulletPreset _bulletPreset = null;
        private PoolManager _poolManager = null;

        private Coroutine _attackCor = null;
        private Coroutine _lookAtEnemyCor = null;
        private Coroutine _rotateToOrigineCor = null;

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

        [Inject]
        private void Construct(BulletSettings bulletSettings)
        {
            _bulletSettings = bulletSettings;
        }

        public void Spawn(TowerSettings towerSettings, PoolManager poolManager, Action action)
        {
            _startRotation = transform.rotation;

            _poolManager = poolManager;
            _towerSettings = towerSettings;
            _towerPreset = _towerSettings.GetPresetByType(Type);

            _bulletPreset = _bulletSettings.GetPresetByType(_towerPreset.DamageType);

            _attackRadius.Setup(_towerPreset.AttackRadius);
            _attackRadius.Show();

            IsSpawned = false;

            _materialPropertyBlock = new MaterialPropertyBlock();

            _startColor = _skinnedMeshRenderer.material.color;
            _spawnColor = new Color(_startColor.r, _startColor.g, _startColor.b, _startColor.a / 2);

            _materialPropertyBlock.SetColor(BaseColorID, _spawnColor);
            _materialPropertyBlock.SetColor(OutLineColorID, _towerSettings.SpawnOutLIneColor);
            _materialPropertyBlock.SetFloat(OutLineWidthID, 0.5f);

            _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);

            _onSpawnAction = action;
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

        public void Spawn()
        {
            IsSpawned = true;
            _attackRadius.gameObject.SetActive(false);

            _materialPropertyBlock.SetColor(BaseColorID, _startColor);
            _materialPropertyBlock.SetColor(OutLineColorID, _towerSettings.BaseOutLineColor);
            
            _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);

            _onSpawnAction?.Invoke();
        }

        public void ToggleSpawnAbility(bool isCanSpawn)
        {
            if (IsSpawned)
            {
                return;
            }

            IsCanSpawn = isCanSpawn;
            _attackRadius.ChangeColor(isCanSpawn);

            ChangeOutLineColor(isCanSpawn);
        }

        private void ChangeOutLineColor(bool isCanSpawn)
        {
            _materialPropertyBlock.SetColor(OutLineColorID,
                isCanSpawn ? _towerSettings.SpawnOutLIneColor : _towerSettings.CantSpawnOutLIneColor);

            _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
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
            StopAttack();
            Free();
        }

        private IEnumerator AttackCor(BulletSettings.BulletPreset bulletPreset)
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

        public void ToggleHighlight(bool isNeedHighLight)
        {
            _materialPropertyBlock.SetColor(BaseColorID, isNeedHighLight ? _spawnColor : _startColor);
            _materialPropertyBlock.SetColor(OutLineColorID,
                isNeedHighLight ? _towerSettings.SpawnOutLIneColor : _towerSettings.BaseOutLineColor);
            
            _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
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