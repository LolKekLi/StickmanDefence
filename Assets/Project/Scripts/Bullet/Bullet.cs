using System.Collections;
using UnityEngine;

namespace Project
{
    public  class Bullet : PooledBehaviour
    {
        [SerializeField]
        private MeshFilter _meshRenderer = null;
        
        private int _damageAbility = 0;
        private int _damage = 0;
        
        private float _lifeTime = 0f;
        private float _speed = 0f;

        private Coroutine _shootCor = null;

        public DamageType DamageType
        {
            get;
            private set;
        }

        public void Setup(BulletSettings.BulletPreset bulletPreset)
        {
            Debug.Log("Setup");
                
            _lifeTime = bulletPreset.LifeTime;
            _speed = bulletPreset.Speed;
            _meshRenderer.mesh = bulletPreset.Mesh;
            DamageType = bulletPreset.DamageType;
        }

        public void Shoot(Vector3 direction)
        {
            _shootCor = StartCoroutine(ShootCor(direction));
        }

        protected override void BeforeReturnToPool()
        {
            base.BeforeReturnToPool();

            if (_shootCor != null)
            {
                StopCoroutine(_shootCor);
                _shootCor = null;
            }
        }

        public int GetDamage()
        {
            _damageAbility--;

            if (_damageAbility <= 0)
            {
                Free();
            }

            return _damage;
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