using System.Collections;
using UnityEngine;

namespace Project
{
    public class Bullet : PooledBehaviour
    {
        [SerializeField]
        private MeshFilter _meshRenderer = null;

        [SerializeField]
        private TrailRenderer _trailRenderer;

        private int _damageAbility = 0;
        private float _damage = 1;

        private float _lifeTime = 0f;
        private float _speed = 0f;

        private Coroutine _shootCor = null;
        private float _damageUpdateProcent;

        public DamageType DamageType
        {
            get;
            private set;
        }

        public void Setup(BulletSettings bulletSettings, float damageUpdateProcent)
        {
            _damageUpdateProcent = damageUpdateProcent;
            _damage = bulletSettings.BaseDamage;
            _lifeTime = bulletSettings.LifeTime;
            _speed = bulletSettings.Speed;
            _meshRenderer.mesh = bulletSettings.Mesh;
            DamageType = bulletSettings.DamageType;
        }

        public void Shoot(Vector3 direction)
        {
            _shootCor = StartCoroutine(ShootCor(direction));
        }

        protected override void BeforeReturnToPool()
        {
            base.BeforeReturnToPool();

            if (_trailRenderer)
            {
                _trailRenderer.Clear();
            }
            
            if (_shootCor != null)
            {
                StopCoroutine(_shootCor);
                _shootCor = null;
            }
        }

        public float GetDamage()
        {
            _damageAbility--;

            if (_damageAbility <= 0)
            {
                Free();
            }

            return _damage + (_damage * (_damageUpdateProcent / 100));
        }

        private IEnumerator ShootCor(Vector3 direction)
        {
            var time = 0f;

            while (time <= _lifeTime)
            {
                time += Time.deltaTime;

                transform.Translate(direction * (Time.deltaTime * _speed));

                yield return null;
            }

            _shootCor = null;

            Free();
        }
    }
}