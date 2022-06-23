using System;
using System.Collections;
using PathCreation;
using UnityEngine;

namespace Project
{
    public class Enemy : PooledBehaviour
    {
        private Action _onFreeAction = null;

        private Coroutine _followPathCor = null;

        public void StartFollowPath(PathCreator path, EndOfPathInstruction endOfPathInstruction)
        {
            _followPathCor = StartCoroutine(FollowPathCor(path, endOfPathInstruction));
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

            Damaged();
            Free();
        }

        protected override void BeforeReturnToPool()
        {
            _onFreeAction?.Invoke();
            
            base.BeforeReturnToPool();
        }

        private void Damaged()
        {
            
        }

        public void Setup(Action action)
        {
            _onFreeAction = action;
        }
    }
}