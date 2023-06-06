using System.Collections;
using UnityEngine;

namespace Project
{
    public class Granat : BulletBase
    {
        [SerializeField]
        private AnimationCurve _curve;

        private Coroutine _shootCor = null;

        public override void Shoot(Vector3 direction)
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

        private IEnumerator ShootCor(Vector3 direction)
        {
            var time = 0f;

            while (time <= _lifeTime)
            {
                time += Time.deltaTime;

                //direction = direction.ChangeY(_curve.Evaluate(direction.y));
                
                var deltaTime = direction * (Time.deltaTime * _speed);

                transform.Translate(deltaTime);

                yield return null;
            }

            _shootCor = null;

            Free();
        }

        public override float GetDamage()
        {
            return 0;
        }
    }
}