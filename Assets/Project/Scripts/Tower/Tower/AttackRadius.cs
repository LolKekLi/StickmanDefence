using System;
using UnityEngine;

namespace Project
{
    [RequireComponent(typeof(MeshRenderer))]
    public class AttackRadius : MonoBehaviour
    {
        private readonly string OutlineColorID = "_BoundColor";
        private readonly string ColorID = "_BgColor";

        [Serializable]
        private struct ColorPreset
        {
            [field: SerializeField]
            public Color OutLineColor
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

        [SerializeField]
        private ColorPreset _baseColor = default;

        [SerializeField]
        private ColorPreset _cantSpawnColor = default;

        private Transform _transform = null;
        private MeshRenderer _meshRenderer = null;
        private MaterialPropertyBlock _basePropertyBlock = null;
        private float _radius;

        public float SqrAttackRadius
        {
            get =>
                _radius * _radius;
        }

        public float Radius
        {
            get =>
                _radius;
        }

        public void Setup(float radius)
        {
            _transform = GetComponent<Transform>();
            _meshRenderer = GetComponent<MeshRenderer>();

            _basePropertyBlock = new MaterialPropertyBlock();

            ChangeColor(true);

            gameObject.SetActive(false);

            _radius = radius;
            _transform.localScale = Vector3.one * (_radius * 0.2f);
        }

        public void Show()
        {
            ToggleActive(true);
        }

        public void ChangeColor(bool isCanSpawn)
        {
            _basePropertyBlock.SetColor(ColorID, isCanSpawn ? _baseColor.Color : _cantSpawnColor.Color);
            _basePropertyBlock.SetColor(OutlineColorID,
                isCanSpawn ? _baseColor.OutLineColor : _cantSpawnColor.OutLineColor);

            _meshRenderer.SetPropertyBlock(_basePropertyBlock);
        }

        public void ToggleActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        public void UpdateFireRadius(float updatePresetValue)
        {
            _radius += (_radius * (updatePresetValue / 100));
            _transform.localScale = Vector3.one * (_radius * 0.2f);
        }
    }
}