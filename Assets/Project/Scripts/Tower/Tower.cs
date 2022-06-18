using UnityEngine;
using Zenject;


namespace Project
{
    public abstract class Tower : PooledBehaviour
    {
        private static readonly string BaseColorID = "_BaseColor";

        [SerializeField]
        private Transform _attackRadius = null;

        [SerializeField]
        private MeshRenderer _meshRenderer = null;

        private Vector3 _velocity = Vector3.zero;
        private Color _startColor = default;

        private TowerSettings _towerSettings = null;
        private MaterialPropertyBlock _materialPropertyBlock;

        public abstract TowerType Type
        {
            get;
        }

        public bool IsCanSpawn
        {
            get;
            private set;
        } = true;

        [Inject]
        private void Construct(TowerSettings towerSettings)
        {
            _towerSettings = towerSettings;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Tower tower))
            {
                IsCanSpawn = false;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Tower tower))
            {
                IsCanSpawn = true;
            }
        }

        public override void SpawnFromPool()
        {
            base.SpawnFromPool();

            _attackRadius.gameObject.SetActive(true);
            _attackRadius.localScale = Vector3.one * _towerSettings.GetPresetByType(Type).AttackRadius;

            _materialPropertyBlock = new MaterialPropertyBlock();

            var materialColor = _meshRenderer.material.color;
            _startColor = materialColor;

            materialColor = new Color(materialColor.r, materialColor.g, materialColor.b, materialColor.a / 2);

            _materialPropertyBlock.SetColor(BaseColorID, materialColor);
            _meshRenderer.SetPropertyBlock(_materialPropertyBlock);
        }

        public void Move(Vector3 targetPosition)
        {
            transform.position =
                Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, _towerSettings.SmoothTime);
        }

        public void Spawn()
        {
            _attackRadius.gameObject.SetActive(false);
            
            _materialPropertyBlock.SetColor(BaseColorID, _startColor);
            _meshRenderer.SetPropertyBlock(_materialPropertyBlock);
        }
    }
}