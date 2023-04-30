using Project;
using UnityEngine;

public class TowerViewModel : PooledBehaviour
{
    [SerializeField, Space]
    private SkinnedMeshRenderer _skinnedMeshRenderer = null;

    [SerializeField]
    private Animator _animator = null;

    [field: SerializeField]
    public Transform FirePosition
    {
        get;
        private set;
    }

    [field: SerializeField]
    public TowerMeshType TowerMeshType
    {
        get;
        private set;
    }
}