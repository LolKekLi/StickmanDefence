using Project.UI;
using UnityEngine;
using Zenject;

namespace Project
{
    public class SceneBindings : MonoInstaller
    {
        [SerializeField]
        private TowerController _towerController = null;

        [Inject]
        private UISystem _uiSystem = null;

        public override void InstallBindings()
        {
            Container.Bind<TowerController>().FromInstance(_towerController).AsCached();
            Container.Inject(_uiSystem);
            
        }
    }
}