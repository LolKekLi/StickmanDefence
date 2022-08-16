using System;
using System.Collections;
using PathCreation;
using UnityEngine;

namespace Project
{
    public class Enemy : PooledBehaviour
    {
        public static event Action<EnemyType> Died = delegate { };

        [SerializeField]
        private EnemyType _spawnEnemyType = default;

        private int _hp = 0;

        private Action _onFreeAction = null;

        private Coroutine _followPathCor = null;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Bullet bullet))
            {
                TakeDamage(bullet);
            }
        }

        public void Setup(Action action)
        {
            _onFreeAction = action;
        }

        public void StartFollowPath(PathCreator path, EndOfPathInstruction endOfPathInstruction)
        {
            _followPathCor = StartCoroutine(FollowPathCor(path, endOfPathInstruction));
        }

        protected override void BeforeReturnToPool()
        {
            _onFreeAction?.Invoke();

            base.BeforeReturnToPool();
        }

        private void TakeDamage(Bullet bullet)
        {
            var bulletDamageType = bullet.DamageType;

           _hp -= bullet.GetDamage();

           if (_hp <= 0)
           {
               Die();
           }
        }
        
        private void Die()
        {
            Died(_spawnEnemyType);
            
            Free();
        }

        private IEnumerator FollowPathCor(PathCreator path, EndOfPathInstruction endOfPathInstruction)
        {
            float distanceTravelled = 0;

            while (path != null)
            {
                distanceTravelled += Time.deltaTime * 10;
                transform.position = path.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
                transform.rotation = path.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);

                yield return null;
            }
            
            Free();
        }
    }
}