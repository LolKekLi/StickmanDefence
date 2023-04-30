using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project
{
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

            [field: SerializeField, PreviewField(100), HideLabel]
            public Sprite UIIcon
            {
                get;
                private set;
            }

            [field: SerializeField]
            public TowerMeshType BaseMesh
            {
                get;
                private set;
            }

            [field: SerializeField, TextArea]
            public string TowerLabel
            {
                get;
                private set;
            }

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

        [field: SerializeField]
        public float SmoothTime
        {
            get;
            private set;
        } = 0.3f;


        [SerializeField, TableList]
        private TowerPreset[] _towerPresets = null;

        [SerializeField]
        private TowerUpgradeSettings[] _towerUpgradeSettings = null;

        public TowerPreset GetTowerPresetByType(TowerType type)
        {
            var towerPreset = _towerPresets.FirstOrDefault(x => x.TowerPrefab.Type == type);

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
            var towerPreset = _towerUpgradeSettings.FirstOrDefault(x => x.TowerType == type );

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