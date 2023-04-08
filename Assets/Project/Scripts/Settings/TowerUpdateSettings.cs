using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project
{
    public enum UpdateType
    {
        Damage,
        Speed,
        UpdateWeapon,
        FireRadius,
    }
    
    [CreateAssetMenu(fileName = "TowerUpdateSettings", menuName = "MySettings/TowerUpdateSettings", order = 0)]
    public class TowerUpdateSettings : ScriptableObject
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
            
            [field: SerializeField]
            public uint Cost
            {
                get;
                private set;
            }

            [field: SerializeField, HideIf("UpdateType", UpdateType.UpdateWeapon)]
            public float Value
            {
                get;
                private set;
            }
            
            [field: SerializeField, ShowIf("UpdateType", UpdateType.UpdateWeapon)]
            public TowerType UpgradeTowerType
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
    }
}