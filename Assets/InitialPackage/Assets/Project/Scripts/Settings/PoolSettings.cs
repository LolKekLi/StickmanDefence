using System;
using System.Linq;
using UnityEngine;

namespace Project
{
    [CreateAssetMenu(fileName = "PoolSettings", menuName = "Scriptable/PoolSettings", order = 0)]
    public class PoolSettings : ScriptableObject
    {
        [field: SerializeField]
        public Bullet Bullet
        {
            get;
            private set;
        }
    }
}