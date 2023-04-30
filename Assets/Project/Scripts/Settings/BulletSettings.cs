using System;
using System.Linq;
using UnityEngine;

namespace Project
{
    [CreateAssetMenu(fileName = "BulletSettings", menuName = "MySettings/Bullet Settings", order = 0)]
    public class BulletSettings : ScriptableObject
    {
        [field: SerializeField]
        public DamageType DamageType
        {
            get;
            private set;
        }

        [field: SerializeField]
        public float LifeTime
        {
            get;
            private set;
        }

        [field: SerializeField]
        public float Speed
        {
            get;
            private set;
        }

        [field: SerializeField]
        public Mesh Mesh
        {
            get;
            private set;
        }

        [field: SerializeField]
        public int BaseDamage
        {
            get;
            private set;
        } = 1;
    }
}