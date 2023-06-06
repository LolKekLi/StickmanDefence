using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project
{
    [CreateAssetMenu(fileName = "EnemySettings", menuName = "MySettings/Enemy Settings", order = 0)]
    public class EnemySettings : ScriptableObject
    {
        [Serializable]
        public class EnemyPreset
        {
            [field: SerializeField]
            public EnemyType Type
            {
                get;
                private set;
            }

            [field: SerializeField]
            public Enemy EnemyPrefab
            {
                get;
                private set;
            }

            [field: SerializeField]
            public float MoveSpeed
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
            public int Damage
            {
                get;
                private set;
            }

            [field: SerializeField]
            public int Cost
            {
                get;
                private set;
            }
        }
       
        [SerializeField]
        private EnemyPreset[] _enemyPresets = null;

        public EnemyPreset GetEnemyPreset(EnemyType type)
        {
            var enemyPreset = _enemyPresets.FirstOrDefault(x => x.Type == type);

            if (enemyPreset != null)
            {
                return enemyPreset;
            }
            
            Debug.LogError($"{(typeof(EnemySettings))}Нет пресета EnemyPreset под тип {type}");

            return null;
        }
    }
}