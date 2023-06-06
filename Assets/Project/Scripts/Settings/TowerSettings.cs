using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Project
{
    [Serializable]
    public class FirePreset
    {
        [field: SerializeField]
        public BulletSettings BulletSettings
        {
            get;
            private set;
        }

        [field: SerializeField]
        public float FireRate
        {
            get;
            private set;
        }

        [field: SerializeField]
        public FireType FireType
        {
            get;
            private set;
        }

        [field: SerializeField, ShowIf("FireType", FireType.Multi)]
        public uint BulletCount
        {
            get;
            private set;
        }

        [field: SerializeField, ShowIf("FireType", FireType.Multi)]
        public uint DelayBetweenBullet
        {
            get;
            private set;
        }

        [field: SerializeField]
        public AttackRadiusType AttackRadiusType
        {
            get;
            private set;
        }

        [field: SerializeField, ShowIf("AttackRadiusType", AttackRadiusType.Circle)]
        public float AttackRadius
        {
            get;
            private set;
        }

        [field: SerializeField, ShowIf("AttackRadiusType", AttackRadiusType.Box)]
        public Vector2 AttackBoxSize
        {
            get;
            private set;
        }
    }

    [CreateAssetMenu(fileName = "TowerSettings", menuName = "MySettings/Tower Settings", order = 0)]
    public class TowerSettings : ScriptableObject
    {
        [Serializable]
        public class TowerPreset
        {
            [field: SerializeField]
            public BaseTower TowerPrefab
            {
                get;
                private set;
            }
            
            [field: SerializeField]
            public TowerViewModelType BaseViewModelType
            {
                get;
                set;
            }

            [field: SerializeField]
            public int Cost
            {
                get;
                private set;
            }

            [field: SerializeField]
            public FirePreset FirePreset
            {
                get;
                private set;
            }
        }

        [field: SerializeField]
        public float SmoothTime
        {
            get;
            private set;
        } = 0.3f;


        [field: FormerlySerializedAs("_towerPresets")]
        [field: SerializeField, TableList]
        public TowerPreset[] TowerPresets
        {
            get;
            private set;
        }

        [SerializeField]
        private TowerUpgradeSettings[] _towerUpgradeSettings = null;

        public TowerPreset GetTowerPresetByType(TowerType type)
        {
            var towerPreset = TowerPresets.FirstOrDefault(x => x.TowerPrefab.TowerType == type);

            if (towerPreset != null)
            {
                return towerPreset;
            }
            else
            {
                Debug.LogError($"{typeof(TowerSettings)} Нет пресета TowerPreset под тип {type}");

                return null;
            }
        }

        public TowerUpgradeSettings GetTowerUpdateSettings(TowerType type)
        {
            var towerPreset = _towerUpgradeSettings.FirstOrDefault(x => x.TowerType == type);

            if (towerPreset != null)
            {
                return towerPreset;
            }
            else
            {
                Debug.LogError($"{typeof(TowerSettings)} Нет пресета TowerPreset под тип {type}");

                return null;
            }
        }
    }
}