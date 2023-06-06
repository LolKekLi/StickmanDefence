using System;
using System.Linq;
using UnityEngine;

namespace Project
{
    [CreateAssetMenu(fileName = "LevelSettings", menuName = "Scriptable/LevelSettings", order = 0)]
    public class LevelSettings : ScriptableObject
    {
        [Serializable]
        public class DifficultMultiplierPreset
        {
            [field: SerializeField]
            public LevelDifficultType Type
            {
                get;
                private set;
            }

            [field: SerializeField, Min(1)]
            public float Multiplier
            {
                get;
                private set;
            }
        }

        [Serializable]
        public class LevelPreset
        {
            [field: SerializeField]
            public string SceneName
            {
                get;
                private set;
            }

            [field: SerializeField]
            public string LevelName
            {
                get;
                private set;
            }

            [field: SerializeField]
            public string NightSceneName
            {
                get;
                private set;
            }

            [field: SerializeField]
            public Sprite Icon
            {
                get;
                private set;
            }

            [field: SerializeField]
            public Sprite NightIcon
            {
                get;
                private set;
            }

            [field: SerializeField]
            public int Prise
            {
                get;
                private set;
            }

            [field: SerializeField]
            public EnemyWaveSettings EnemyWaveSettings
            {
                get;
                private set;
            }

            [field: SerializeField]
            public int HP
            {
                get;
                private set;
            }

            [field: SerializeField]
            public int StartCoinCount
            {
                get;
                private set;
            }
        }

        [field: SerializeField]
        public string HubSceneName
        {
            get;
            private set;
        }

        [field: SerializeField]
        public LevelPreset[] Levels
        {
            get;
            private set;
        }

        [field: SerializeField]
        public DifficultMultiplierPreset[] MultiplierPresets
        {
            get;
            private set;
        }

        [field: SerializeField]
        public float ResultDelay
        {
            get;
            set;
        }
    }
}