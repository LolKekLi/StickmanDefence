using UnityEngine;
using UnityEngine.Rendering.Universal;
using Zenject;

namespace Project
{
    public abstract class Tower : PooledBehaviour
    {
        private static readonly string BaseColorID = "_BaseColor";
        private static readonly string OutLineWidthID = "_OutlineWidth";
        private static readonly string OutLineColorID = "_OutlineColor";
        private static readonly string RenderModID = "_RenderingMode";

        [SerializeField]
        private AttackRadius _attackRadius = null;

        [SerializeField]
        private SkinnedMeshRenderer _skinnedMeshRenderer = null;

        [SerializeField]
        private Color _baseOutLineColor = default;

        [SerializeField]
        private Color _canrSpawnOutLIneColor = default;

        private bool _isTriggered = false;

        private Vector3 _velocity = Vector3.zero;
        private Color _startColor = default;
        private Color _spawnColor = default;

        private TowerSettings _towerSettings = null;
        private MaterialPropertyBlock _materialPropertyBlock;

        [field: SerializeField]
        public CantSpawnZone CantSpawnZone
        {
            get;
            private set;
        }

        public abstract TowerType Type
        {
            get;
        }

        public bool IsCanSpawn
        {
            get;
            private set;
        } = true;

        public bool IsSpawned
        {
            get;
            private set;
        }

        [Inject]
        private void Construct(TowerSettings towerSettings)
        {
            _towerSettings = towerSettings;
        }

        public override void SpawnFromPool()
        {
            base.SpawnFromPool();
            IsSpawned = false;

            _attackRadius.Setup(_towerSettings.GetPresetByType(Type).AttackRadius);
            _attackRadius.Show();

            _materialPropertyBlock = new MaterialPropertyBlock();

            _startColor = _skinnedMeshRenderer.material.color;
            _spawnColor = new Color(_startColor.r, _startColor.g, _startColor.b, _startColor.a / 2);


            _materialPropertyBlock.SetColor(BaseColorID, _spawnColor);
            _materialPropertyBlock.SetColor(OutLineColorID, _baseOutLineColor);
            _materialPropertyBlock.SetFloat(OutLineWidthID, 0.5f);

            _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
        }

        public void Move(Vector3 targetPosition)
        {
            transform.position =
                Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, _towerSettings.SmoothTime);
        }

        public void Spawn()
        {
            IsSpawned = true;
            _attackRadius.gameObject.SetActive(false);

            _materialPropertyBlock.SetColor(BaseColorID, _startColor);
            _materialPropertyBlock.SetColor(OutLineColorID, Color.gray);

            _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
        }

        public void ToggleSpawnAbility(bool isCanSpawn)
        {
            if (IsSpawned)
            {
                return;
            }
            
            IsCanSpawn = isCanSpawn;
            _attackRadius.ChangeColor(isCanSpawn);
            ChangeOutLineColor(isCanSpawn);
        }

        private void ChangeOutLineColor(bool isCanSpawn)
        {
            _materialPropertyBlock.SetColor(OutLineColorID, isCanSpawn ? _baseOutLineColor : _canrSpawnOutLIneColor);

            _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
        }
    }
}