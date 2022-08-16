using System;
using System.Linq;
using UnityEngine;

namespace Project
{
    [CreateAssetMenu(fileName = "BulletSettings", menuName = "MySettings/Bullet Settings", order = 0)]
    public class BulletSettings : ScriptableObject
    {
        [Serializable]
        public class BulletPreset
        {
            [field: SerializeField]
            public DamageType DamageType
            {
                get;
                private set;
            }

            [field: SerializeField]
            public float LifeTime
            {
                get;
                private set;
            }

            [field: SerializeField]
            public float Speed
            {
                get;
                private set;
            }

            [field: SerializeField]
            public Mesh Mesh
            {
                get;
                private set;
            }
        }

        [SerializeField]
        private BulletPreset[] _bulletPresets = null;
        
        public BulletPreset GetPresetByType(DamageType type)
        {
            var bulletPreset = _bulletPresets.FirstOrDefault(x => x.DamageType == type);

            if (bulletPreset == null)
            {
                Debug.LogError($"{typeof(BulletSettings)} не найден пресет пули под тип дамаг {type}");
            }

            return bulletPreset;
        }
    }
}