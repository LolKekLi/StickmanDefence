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
            public Tower Tower
            {
                get;
                private set;
            }

            [field: SerializeField, PreviewField]
            public Sprite UIIcon
            {
                get;
                private set;
            }

            [field: SerializeField]
            public float AttackRadius
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
        }

        [field: SerializeField]
        public float SmoothTime
        {
            get;
            private set;
        } = 0.3f;


        [field: SerializeField]
        public Color BaseOutLineColor
        {
            get;
            private set;
        }

        [field: SerializeField]
        public Color CantSpawnOutLIneColor
        {
            get;
            private set;
        }

        [field: SerializeField]
        public Color SpawnOutLIneColor
        {
            get;
            private set;
        }

        [SerializeField]
        private TowerPreset[] _towerPresets = null;

        public TowerPreset GetPresetByType(TowerType type)
        {
            var towerPreset = _towerPresets.FirstOrDefault(x => x.Tower.Type == type);

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