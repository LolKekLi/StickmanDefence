using System.Collections;
using UnityEngine;

namespace Project
{
    public abstract class Bullet : PooledBehaviour
    {
        [SerializeField]
        private MeshRenderer _meshRenderer = null;
        
        private int _damageAbility = 0;
        private int _damage = 0;

        private Coroutine _shootCor = null;

        public abstract DamageType DamageType
        {
            get;
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
            var lifeTime = 1f;

            while (time <= lifeTime)
            {
                time += Time.deltaTime;

                transform.Translate(direction * Time.deltaTime);

                yield return null;
            }

            _shootCor = null;
            Free();
        }
    }
}