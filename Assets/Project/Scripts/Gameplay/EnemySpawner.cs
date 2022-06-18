using System;
using System.Collections.Generic;
using PathCreation;
using UnityEngine;
using UnityEngine.Serialization;

namespace Project
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField]
        private EndOfPathInstruction _endOfPathInstruction = default;
       
        [SerializeField]
        private List<PathCreator> _paths = null;

        private void Start()
        {
            var enemy = GameObject.CreatePrimitive(PrimitiveType.Cube).AddComponent<Enemy>();
            
            enemy.StartFollowPath(_paths[0], _endOfPathInstruction);
        }
    }
}