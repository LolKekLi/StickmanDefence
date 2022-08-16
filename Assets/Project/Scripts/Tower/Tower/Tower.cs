using System;
using System.Collections;
using UnityEngine;
using Zenject;

namespace Project
{
    public abstract class Tower : PooledBehaviour
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

        private TowerSettings _towerSettings = null;
        private TowerSettings.TowerPreset _towerPreset = null;
        private MaterialPropertyBlock _materialPropertyBlock;
        private BulletSettings _bulletSettings = null;
        private BulletSettings.BulletPreset _bulletPreset = null;
        private PoolManager _poolManager = null;

        private Coroutine _attackCor = null;
        private Coroutine _lookAtEnemyCor = null;

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

        public bool IsAttacked
        {
            get =>
                _attackCor != null;
        }

        public Enemy Target
        {
            get;
            private set;
        }

        [Inject]
        private void Construct(BulletSettings bulletSettings)
        {
            _bulletSettings = bulletSettings;
        }

        public void Spawn(TowerSettings towerSettings, PoolManager poolManager, Action action)
        {
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

        public void Attack(Enemy enemy)
        {
            Target = enemy;

            _attackCor = StartCoroutine(AttackCor(enemy, _bulletPreset));
            _lookAtEnemyCor = StartCoroutine(LookAtEnemyCor(enemy));
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

        public void StopAttack()
        {
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
        }

        private IEnumerator AttackCor(Enemy enemy, BulletSettings.BulletPreset bulletPreset)
        {
            var waiter = new WaitForSeconds(1 / _towerPreset.FireRate);

            while (!enemy.IsFree)
            {
                var bullet = _poolManager.Get<Bullet>(_poolManager.PoolSettings.Bullet, _firePosition.position,
                    Quaternion.identity);

                bullet.Setup(_bulletPreset);
                bullet.Shoot(enemy.transform.position - transform.position);

                yield return waiter;
            }

            _attackCor = null;
            Target = null;
        }

        private IEnumerator LookAtEnemyCor(Enemy enemy)
        {
            while (!enemy.IsFree)
            {
                transform.LookAt(enemy.transform);

                yield return null;
            }

            _lookAtEnemyCor = null;
        }

        private void OnDrawGizmos()
        {
            if (_towerPreset != null)
            {
                Gizmos.DrawWireSphere(transform.position, _towerPreset.AttackRadius);
            }
        }
    }
}