using Project.Meta;
using Project.Settings;
using UnityEngine;
using Zenject;

namespace Project
{
    [CreateAssetMenu(fileName = "ProjectContextInstaller", menuName = "Scriptable/Zenject/Project Context Installer")]
    public class ProjectContextInstaller : ScriptableObjectInstaller<ProjectContextInstaller>
    {
        [SerializeField]
        private LevelSettings _levelSettings = null;

        [SerializeField]
        private PoolSettings _poolSettings = null;

        [SerializeField]
        private SkinSettings _skinSettings = null;

        [SerializeField]
        private ParticleSettings _particleSettings = null;

        [SerializeField]
        private TowerSettings _towerSettings = null;

        [SerializeField]
        private EnemySettings _enemySettings = null;

        [SerializeField]
        private TowerHighLightSettings _towerHighLightSettings = null;

        [SerializeField]
        private LoadingSettings _loadingSettings;

        [SerializeField]
        private AssUISetting _assUISetting;

        [SerializeField]
        private UITowerSetting _uiTowerSetting;

        [SerializeField]
        private TowerViewModelSettings _towerViewModelSettings;

        [SerializeField]
        private UltimateSettings _ultimateSettings;


        public override void InstallBindings()
        {
            InstallSignalBus();

            BindInstances();

            BindControllers();
            
            BindManagers();
            
            BindFactory();

            BindMeta();
        }

       

        private void InstallSignalBus()
        {
            SignalBusInstaller.Install(Container);
        }

        private void BindInstances()
        {
            Container.BindInstance(_poolSettings).AsCached();
            Container.BindInstance(_particleSettings).AsCached();
            Container.BindInstance(_skinSettings).AsCached();
            Container.BindInstance(_towerSettings).AsCached();
            Container.BindInstance(_enemySettings).AsCached();
            Container.BindInstance(_towerHighLightSettings).AsCached();
            Container.BindInstance(_loadingSettings).AsCached();
            Container.BindInstance(_assUISetting).AsCached();
            Container.BindInstance(_uiTowerSetting).AsCached();
            Container.BindInstance(_towerViewModelSettings).AsCached();
            Container.BindInstance(_ultimateSettings).AsCached();
        }

        private void BindControllers()
        {
            Container.BindInstance(new LevelFlowController(_levelSettings)).AsCached();
        }
        
        private void BindManagers()
        {
            BindManager(PoolManager.GetManager);
            BindManager(AudioManager.GetManager);
            BindManager(ParticlesManager.GetManager);
            BindManager(UIOverlayMessage.GetManager);
            BindManager(AnalyticsManager.GetManager);
            BindManager(AdsManager.GetManager);
            BindManager(InAppManager.GetManager);
            BindManager(TowerFactory.GetManager);
            BindManager(TowerViewModelFactory.GetManager);
            
#if FORCE_DEBUG
            BindManager(UIDebug.DebugMenu.GetManager);
#endif
        }
        
        private void BindFactory()
        {
        }

        private void BindMeta()
        {
            Container.BindInstance(new UserManager()).AsCached();
            Container.Bind<SkinController>().AsCached().NonLazy();
        }

        private void BindManager<T>(T managerPrefab) where T : ZenjectManager<T>
        {
            var manager = Container.InstantiatePrefabForComponent<T>(managerPrefab);
            
            Container.Bind<T>().FromInstance(manager).AsCached();
        }
        
        private void BindPrefab<T>(T prefab) where T : Component
        {
            var controller = Container.InstantiatePrefabForComponent<T>(prefab);
            Container.Bind<T>()
                .FromInstance(controller).AsCached();
        }
    }
}