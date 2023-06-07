using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Project
{
    public class Granat : BulletBase
    {
        [SerializeField]
        private ParticleSystem _particleSystem;

        [SerializeField] 
        private MeshRenderer _meshRenderer;
        
        private Coroutine _shootCor = null;

        public override void Shoot(Vector3 direction)
        {
            _shootCor = StartCoroutine(ShootCor(direction));
        }

        protected override void BeforeReturnToPool()
        {
            base.BeforeReturnToPool();
            
            _meshRenderer.enabled = true;
            
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

                var deltaTime = direction * (Time.deltaTime * _speed);

                transform.Translate(deltaTime);

                yield return null;
            }

            _shootCor = null;

            Free();
        }

        public override float GetDamage()
        {
            GranatFree();

            return 2;
        }

        private async void GranatFree()
        {
            _particleSystem.Play();
            
            if (_shootCor != null)
            {
                StopCoroutine(_shootCor);
                _shootCor = null;
            }
            
            await UniTask.Delay(TimeSpan.FromSeconds(1f));
            
            Free();
        }
    }
}