using System;
using UnityEngine;

namespace Project
{
    [CreateAssetMenu(fileName = "TowerViewModelSettings", menuName = "MySettings/TowerViewModelSettings", order = 0)]
    public class TowerViewModelSettings : ScriptableObject
    {
        [Serializable]
        public class TowerViewModelPreset
        {
            [field: SerializeField]
            public TowerMeshType TowerMeshType
            {
                get;
                private set;
            }

            [field: SerializeField]
            public TowerViewModel TowerViewModel
            {
                get;
                private set;
            }
        }
        
        [field: SerializeField]
        public TowerViewModelPreset[] TowerViewModelPresets
        {
            get;
            private set;
        }
    }
}