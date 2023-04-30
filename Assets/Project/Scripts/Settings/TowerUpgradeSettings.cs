using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project
{
    public enum UpdateType
    {
        Damage,
        FireSpeed,
        FireRadius,
    }

    [CreateAssetMenu(fileName = "TowerUpgradeSettings", menuName = "MySettings/TowerUpgradeSettings", order = 0)]
    public class TowerUpgradeSettings : ScriptableObject
    {
        [Serializable]
        public class UpdatePreset
        {
            [field: SerializeField]
            public UpdateType UpdateType
            {
                get;
                private set;
            }

            [field: SerializeField, PreviewField(100)]
            public Sprite UIIcon
            {
                get;
                private set;
            }

            [field: SerializeField]
            public uint Cost
            {
                get;
                private set;
            }

            [field: SerializeField]
            public float Value
            {
                get;
                private set;
            }

            [field: SerializeField]
            public bool NeedChangeVisual
            {
                get;
                private set;
            }

            [field: SerializeField, ShowIf("UpdateType", true)]
            public TowerMeshType TowerMeshType
            {
                get;
                private set;
            }
            
            [field: SerializeField, TextArea]
            public string UpdateDescription
            {
                get;
                private set;
            }
        }
        
        [field: SerializeField]
        public TowerType TowerType
        {
            get;
            private set;
        }

        [SerializeField, ListDrawerSettings(ShowIndexLabels = true), TableList]
        private UpdatePreset[] _firstLineUpdatePresets = null;
        [SerializeField, ListDrawerSettings(ShowIndexLabels = true), TableList]
        private UpdatePreset[] _secondLineUpdatePresets = null;
        [SerializeField, ListDrawerSettings(ShowIndexLabels = true), TableList]
        private UpdatePreset[] _thirdLineUpdatePresets = null;

        public UpdatePreset[] GetPresetByLineType(UpgradeLinePerkType perkType)
        {
            return perkType switch
            {
                UpgradeLinePerkType.FirstLine => _firstLineUpdatePresets,
                UpgradeLinePerkType.SecondLine => _secondLineUpdatePresets,
                UpgradeLinePerkType.ThirdLine => _thirdLineUpdatePresets,
                _=> null,

            };
        }
    }
}