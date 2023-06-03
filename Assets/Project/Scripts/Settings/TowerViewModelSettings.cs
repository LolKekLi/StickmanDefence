using System;
using System.Linq;
using UnityEngine;

namespace Project
{
    [CreateAssetMenu(menuName = "MySettings/TowerViewModelSettings", fileName = "TowerViewModelSettings", order = 0)]
    public class TowerViewModelSettings : ScriptableObject
    {
        [Serializable]
        public class TowerViewModelPreset
        {
            [field: SerializeField]
            public TowerViewModelType TowerViewModelType
            {
                get;
                private set;
            }

            [field: SerializeField]
            public TowerViewModel TowerViewModelPrefab
            {
                get;
                private set;
            }
        }

        [SerializeField]
        private TowerViewModelPreset[] _towerViewModelPresets;


        public TowerViewModelPreset GetPreset(TowerViewModelType towerViewModelType)
        {
            var preset = _towerViewModelPresets.FirstOrDefault(x => x.TowerViewModelType == towerViewModelType);

            if (preset == null)
            {
                Debug.LogError($"Нет преста вью башни под тип {towerViewModelType}");
            }

            return preset;
        }
    }
}