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
            public BaseTower Tower
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

            [field: SerializeField]
            public DamageType DamageType
            {
                get;
                private set;
            }
        }
        
        [Serializable]
        public class HighlightPreset
        {
            [field: SerializeField]
            public TowerInteractionState TowerInteractionState
            {
                get;
                private set;
            }

            [field: SerializeField]
            public Color Color
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
        public Color UnslectedOutLineColor
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
        public Color SelectedOutLineColor
        {
            get;
            private set;
        }

        [SerializeField]
        private TowerPreset[] _towerPresets = null;

        [SerializeField]
        private HighlightPreset[] _highlightPresets = null;

        public TowerPreset GetTowerPresetByType(TowerType type)
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

        public HighlightPreset GetHighlightPresetByType(TowerInteractionState towerInteractionState)
        {
            var highlightPreset = _highlightPresets.FirstOrDefault(x=>x.TowerInteractionState == towerInteractionState);

            if (highlightPreset == null)
            {
                Debug.LogError($"{typeof(TowerSettings)} Нет HighlightPreset TowerPreset под тип {towerInteractionState}");
                
                return null;
            }

            return highlightPreset;
        }
    }
}