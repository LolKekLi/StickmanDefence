using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Project
{
    [CreateAssetMenu(menuName = "MySettings/UltimateSettings", fileName = "UltimateSettings", order = 0)]
    public class UltimateSettings : ScriptableObject
    {
        [Serializable]
        public class UltimatePreset
        {
            [field: SerializeField]
            public UltimateType Type
            {
                get;
                private set;
            }

            [field: SerializeField]
            public Sprite Sprite
            {
                get;
                private set;
            }

            [field: SerializeField]
            public float ReloadTime
            {
                get;
                private set;
            }
        }

        [SerializeField]
        public UltimatePreset[] _ultimatePresets;

        public UltimatePreset GetPreset(UltimateType type)
        {
            var preset = _ultimatePresets.FirstOrDefault(x=>x.Type == type);

            if (preset == null)
            {
                Debug.LogError($"Нет преста под тип {type}");
            }

            return preset;
        }
    }
}