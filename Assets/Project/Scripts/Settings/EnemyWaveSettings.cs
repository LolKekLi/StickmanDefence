using System;
using Project;
using UnityEngine;

namespace Project
{
    [CreateAssetMenu(menuName = "MySettings/EnemyWaveSettings", fileName = "EnemyWaveSettings", order = 0)]
    public class EnemyWaveSettings : ScriptableObject
    {
        [Serializable]
        public class SequencePreset
        {
            [field: SerializeField]
            public EnemyType Type
            {
                get;
                private set;
            }

            [field: SerializeField, Min(0)]
            public int EnemyCount
            {
                get;
                private set;
            }

            [field: SerializeField]
            public float SpawnDelay
            {
                get;
                private set;
            }

            [field: SerializeField]
            public float DelayAfterSpawn
            {
                get;
                private set;
            }
        }

        [Serializable]
        public class WavePreset
        {
            [field: SerializeField]
            public SequencePreset[] SequencePresets
            {
                get;
                private set;
            }

            [field: SerializeField]
            public float DelayAfterSpawn
            {
                get;
                private set;
            }
        }

        [field: SerializeField]
        public WavePreset[] WavePresets
        {
            get;
            private set;
        }
    }
}