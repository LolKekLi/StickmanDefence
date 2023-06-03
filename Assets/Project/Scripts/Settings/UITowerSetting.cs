using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project
{
    [CreateAssetMenu(menuName = "MySettings/TowerUISetting", fileName = "TowerUiSetting", order = 0)]
    public class UITowerSetting : ScriptableObject
    {
        [Serializable]
        public class UITowerPreset
        {
            [field: SerializeField]
            public TowerViewModelType TowerViewModelType
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

            [field: SerializeField, TextArea]
            public string TowerLabel
            {
                get;
                private set;
            }
            
        }

        [SerializeField, TableList]
        private UITowerPreset[] _uiTowerPresets;

        public UITowerPreset GetUITowerPreset(TowerViewModelType type)
        {
            var preset = _uiTowerPresets.FirstOrDefault(x => x.TowerViewModelType == type);

            if (preset == null)
            {
                Debug.LogError($"Нет UI пареста для типа башни {type}");
            }

            return preset;
        }
    }
}