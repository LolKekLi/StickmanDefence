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

        [SerializeField]
        private float _hp = 0;
        private float _speed = 0;
        private int _damage = 0;

        private Action _onFreeAction = null;

        private Coroutine _followPathCor = null;
        private Action<int> _onReachFinishPosition;

        public bool IsDied
        {
            get;
            private set;
        } = false;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out BulletBase bullet))
            {
                TakeDamage(bullet);
            }
        }

        public void Setup(EnemySettings.EnemyPreset enemyPreset, Action onDiedAction, Action<int> onReachFinishPosition)
        {
            _onReachFinishPosition = onReachFinishPosition;
            IsDied = false;

            _hp = enemyPreset.HP;
            _speed = enemyPreset.MoveSpeed;

            _damage = enemyPreset.Damage;

            _onFreeAction = onDiedAction;
        }

        private void OnEnable()
        {
            UltimateController.KillAll += Die;
        }
        
        public void StartFollowPath(PathCreator path, EndOfPathInstruction endOfPathInstruction)
        {
            _followPathCor = StartCoroutine(FollowPathCor(path, endOfPathInstruction));
        }

        protected override void BeforeReturnToPool()
        {
            UltimateController.KillAll -= Die;
            _onFreeAction?.Invoke();

            base.BeforeReturnToPool();
        }

        private void TakeDamage(BulletBase bullet)
        {
            var damage = bullet.GetDamage();
            
            var bulletDamageType = bullet.DamageType;

            _hp -= damage;
            
            if (_hp <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            Died(_spawnEnemyType);

            IsDied = true;
            Free();
        }

        private IEnumerator FollowPathCor(PathCreator path, EndOfPathInstruction endOfPathInstruction)
        {
            var distanceTravelled = 0f;
            var enemyTransform = transform;
            var enemyTransformPosition = enemyTransform.position;
            var oldTransformPosition = enemyTransformPosition.ChangeX(enemyTransformPosition.x - 1);

            while (true)
            {
                distanceTravelled += Time.deltaTime * _speed;
                enemyTransform.position = path.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
                enemyTransform.rotation = path.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);

                if (oldTransformPosition == enemyTransform.position)
                {
                    break;
                }

                oldTransformPosition = enemyTransform.position;

                yield return null;
            }

            OnReachFinish();
        }

        private void OnReachFinish()
        {
            _onReachFinishPosition?.Invoke(_damage);
            Free();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position,transform.position+ transform.forward);
        }
#endif
    }
}