using System;
using System.Linq;
using UnityEngine;

namespace Project
{
    [CreateAssetMenu(menuName = "MySettings/AssUISetting", fileName = "AssUISetting", order = 0)]
    public class AssUISetting : ScriptableObject
    {
        [Serializable]
        public class AssUIPreset
        {
            [field: SerializeField]
            public AssType TowerType
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

            [field: SerializeField, TextArea]
            public string Description
            {
                get;
                private set;
            }

            [field: SerializeField]
            public string Lable
            {
                get;
                private set;
            }
        }

        [SerializeField]
        private AssUIPreset[] _assUIPresets;

        
        public AssUIPreset GetUIPreset(AssType type)
        {
            var preset = _assUIPresets.FirstOrDefault(x=>x.TowerType == type);

            if (preset == null)
            {
                Debug.LogError($"Нет пресета башни АССА под тип {type}");
            }

            return preset;
        }

    }
}