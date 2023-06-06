using System;
using Project;
using UnityEngine;
using Zenject;

public class TowerViewModel : PooledBehaviour
{
    public event Action Fire = delegate {  };
    
    private static readonly string BaseColorID = "_BaseColor";
    private static readonly string OutLineWidthID = "_OutlineWidth";
    private static readonly string OutLineColorID = "_OutlineColor";
    private static readonly string RenderModID = "_RenderingMode";

    private readonly int FireKey = Animator.StringToHash("IsFire");

    [SerializeField, Space]
    private SkinnedMeshRenderer _skinnedMeshRenderer = null;

    [SerializeField]
    private Animator _animator = null;

    private Color _startColor;
    private Color _spawnColor;

    private MaterialPropertyBlock _materialPropertyBlock;
    private int _towerLayer;
    private int _xRayLayer;
    private TowerHighLightSettings _highLightSettings;
    private AttackRadius _attackRadius;

    [field: SerializeField]
    public Transform FirePosition
    {
        get;
        private set;
    }

    [field: SerializeField]
    public TowerViewModelType ModelType
    {
        get;
        private set;
    }

    public override void Prepare(PooledObjectType pooledType)
    {
        base.Prepare(pooledType);

        _materialPropertyBlock = new MaterialPropertyBlock();
        _startColor = _skinnedMeshRenderer.material.color;
        _spawnColor = new Color(_startColor.r, _startColor.g, _startColor.b, _startColor.a / 2);
    }

    public void Setup(TowerViewModelType type)
    {
        ModelType = type;
    }

    public override void SpawnFromPool()
    {
        base.SpawnFromPool();

        _skinnedMeshRenderer.GetPropertyBlock(_materialPropertyBlock);
        _materialPropertyBlock.SetFloat(OutLineWidthID, 0.5f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
    }

    public void OnGetTowerFromPool(int towerLayer, int xRayLayer, TowerHighLightSettings towerHighLightSettings,
        AttackRadius attackRadius)
    {
        _attackRadius = attackRadius;
        _highLightSettings = towerHighLightSettings;
        _xRayLayer = xRayLayer;
        _towerLayer = towerLayer;
        _skinnedMeshRenderer.gameObject.layer = _towerLayer;
        _skinnedMeshRenderer.GetPropertyBlock(_materialPropertyBlock);
        _materialPropertyBlock.SetColor(BaseColorID, _spawnColor);
    }

    public void OnFire()
    {
        if (!_animator)
        {
            return;
        }
        
        _animator.SetBool(FireKey, true);
    }

    public void OnFireEnded()
    {
        if (!_animator)
        {
            return;
        }
        
        _animator.SetBool(FireKey, false);
    }

    public void FireAnim()
    {
        Fire();
    }

    public void OnBuild()
    {
        _skinnedMeshRenderer.GetPropertyBlock(_materialPropertyBlock);
        _materialPropertyBlock.SetColor(BaseColorID, _startColor);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
    }

    public void ChangeInteractionState(TowerInteractionState interactionState)
    {
        _skinnedMeshRenderer.GetPropertyBlock(_materialPropertyBlock);

        _skinnedMeshRenderer.gameObject.layer =
            interactionState == TowerInteractionState.CantSpawn ? _xRayLayer : _towerLayer;

        switch (interactionState)
        {
            case TowerInteractionState.Spawned:
                _materialPropertyBlock.SetColor(BaseColorID, _spawnColor);
                _materialPropertyBlock.SetColor(OutLineColorID,
                    _highLightSettings.GetHighlightPresetByType(interactionState).Color);
                _attackRadius.ToggleActive(true);
                _attackRadius.ChangeColor(true);
                break;

            case TowerInteractionState.Selected:
                _materialPropertyBlock.SetColor(OutLineColorID,
                    _highLightSettings.GetHighlightPresetByType(interactionState).Color);
                _attackRadius.ToggleActive(true);
                _attackRadius.ChangeColor(true);

                break;
            case TowerInteractionState.UnSelected:
                _materialPropertyBlock.SetColor(BaseColorID, _startColor);
                _materialPropertyBlock.SetColor(OutLineColorID,
                    _highLightSettings.GetHighlightPresetByType(interactionState).Color);
                _attackRadius.ToggleActive(false);
                break;
            case TowerInteractionState.CantSpawn:
                _materialPropertyBlock.SetColor(OutLineColorID,
                    _highLightSettings.GetHighlightPresetByType(interactionState).Color);
                _attackRadius.ChangeColor(false);
                break;
        }

        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        if (FirePosition)
        {
            Gizmos.DrawWireSphere(FirePosition.position, 0.2f);
        }
    }
#endif
}