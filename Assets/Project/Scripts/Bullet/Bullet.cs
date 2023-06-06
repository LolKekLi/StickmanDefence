using System.Collections;
using UnityEngine;

namespace Project
{
    public class Bullet : BulletBase
    {
        [SerializeField]
        private TrailRenderer _trailRenderer;
        
        private Coroutine _shootCor = null;


        public override void Shoot(Vector3 direction)
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

        public override float GetDamage()
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