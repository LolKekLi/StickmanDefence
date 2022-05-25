using System;
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
            public TowerType Type
            {
                get;
                private set;
            }

            [field: SerializeField]
            public ToggleObject Prefab
            {
                get;
                private set;
            }
        }

        [SerializeField]
        private TowerPreset[] _towerPresets = null;



    }
}