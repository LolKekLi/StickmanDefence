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
            public TowerType TowerType
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

        [field: SerializeField]
        public AssUIPreset[] AssUIPresets
        {
            get;
            private set;
        }

        
        public AssUIPreset GetUIPreset(TowerType type)
        {
            var preset = AssUIPresets.FirstOrDefault(x=>x.TowerType == type);

            if (preset == null)
            {
                Debug.LogError($"Нет пресета башни АССА под тип {type}");
            }

            return preset;
        }

    }
}