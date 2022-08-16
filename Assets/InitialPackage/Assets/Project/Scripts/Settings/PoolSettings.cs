using System;
using System.Linq;
using UnityEngine;

namespace Project
{
    [CreateAssetMenu(fileName = "PoolSettings", menuName = "Scriptable/PoolSettings", order = 0)]
    public class PoolSettings : ScriptableObject
    {
        [Serializable]
        private class PooledObjectPreset
        {
            [field: SerializeField]
            public PooledBehaviour PooledObject
            {
                get;
                private set;
            }

            [field: SerializeField]
            public int SpawnCount
            {
                get;
                private set;
            }
        }

        [SerializeField]
        private PooledObjectPreset[] _pooledObjectPreset = null;

        // public T Get<T>()
        // {
        //     return (T)_pooledObjectPreset.FirstOrDefault(x => x.PooledObject.GetType() == typeof(T));
        // }
    }
}