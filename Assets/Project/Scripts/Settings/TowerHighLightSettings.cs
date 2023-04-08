using System;
using System.Linq;
using UnityEngine;

namespace Project
{
    [CreateAssetMenu(fileName = "TowerHighLightSettings", menuName = "MySettings/TowerHighLightSettings", order = 0)]
    public class TowerHighLightSettings : ScriptableObject
    {
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
        
        [SerializeField, Space]
        private HighlightPreset[] _highlightPresets = null;
        
        public HighlightPreset GetHighlightPresetByType(TowerInteractionState towerInteractionState)
        {
            var highlightPreset =
                _highlightPresets.FirstOrDefault(x => x.TowerInteractionState == towerInteractionState);

            if (highlightPreset == null)
            {
                Debug.LogError(
                    $"{typeof(TowerSettings)} Нет HighlightPreset TowerPreset под тип {towerInteractionState}");

                return null;
            }

            return highlightPreset;
        }
    }
}