using Project.UI;
using UnityEngine;
using Zenject;

public class UIBindings : MonoInstaller
{
    [SerializeField]
    private UISystem _uiSystem = null;
    
    public override void InstallBindings()
    {
        Container.ParentContainers[0].Bind<UISystem>().FromInstance(_uiSystem).AsCached();
    }
}