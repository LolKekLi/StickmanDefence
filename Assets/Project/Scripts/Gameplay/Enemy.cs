﻿using System.Collections;
using PathCreation;
using UnityEngine;

namespace Project
{
    public class Enemy : MonoBehaviour
    {
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
                transform.position += Vector3.up;

                yield return null;
            }
        }
    }
}