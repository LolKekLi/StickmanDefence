using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Project
{
    public class TowerViewModelFactory : ZenjectManager<TowerViewModelFactory>
    {
        [Inject]
        private TowerViewModelSettings _towerSettings;
        [Inject]
        private LevelFlowController _levelFlowController;

        [Inject]
        private DiContainer _diContainer;
        
        private readonly Dictionary<TowerViewModelType, List<TowerViewModel>> Towers =
            new Dictionary<TowerViewModelType, List<TowerViewModel>>()
            {
                { TowerViewModelType.BaseGunner, new List<TowerViewModel>(5) },
                { TowerViewModelType.Gunner1, new List<TowerViewModel>(5) },
                { TowerViewModelType.Gunner2, new List<TowerViewModel>(5) },
                { TowerViewModelType.Gunner3, new List<TowerViewModel>(5) },
                { TowerViewModelType.ShotGunner, new List<TowerViewModel>(5) },
                { TowerViewModelType.ShotGunner1, new List<TowerViewModel>(5) },
                { TowerViewModelType.ShotGunner2, new List<TowerViewModel>(5) },
                { TowerViewModelType.ShotGunner3, new List<TowerViewModel>(5) },
                { TowerViewModelType.Granatman, new List<TowerViewModel>(5) },
                { TowerViewModelType.Granatman1, new List<TowerViewModel>(5) },
                { TowerViewModelType.Granatman2, new List<TowerViewModel>(5) },
                { TowerViewModelType.Granatman3, new List<TowerViewModel>(5) },
                { TowerViewModelType.Sniper, new List<TowerViewModel>(5) },
                { TowerViewModelType.Sniper1, new List<TowerViewModel>(5) },
                { TowerViewModelType.Sniper2, new List<TowerViewModel>(5) },
                { TowerViewModelType.Sniper3, new List<TowerViewModel>(5) },
                { TowerViewModelType.PPMan, new List<TowerViewModel>(5) },
                { TowerViewModelType.PPMan1, new List<TowerViewModel>(5) },
                { TowerViewModelType.PPMan2, new List<TowerViewModel>(5) },
                { TowerViewModelType.PPMan3, new List<TowerViewModel>(5) },
                { TowerViewModelType.Santa, new List<TowerViewModel>(1) },
                { TowerViewModelType.Bugler, new List<TowerViewModel>(1) },
            };
        
        protected override void Init()
        {
            base.Init();

            foreach (var pair in Towers)
            {
                var towers = pair.Value;

                var preset = _towerSettings.GetPreset(pair.Key);

                for (var i = 0; i < towers.Capacity; i++)
                {
                    towers.Add(Instantiate(preset.TowerViewModelPrefab, transform));
                    towers[i].Prepare(preset.TowerViewModelPrefab.Type);
                    towers[i].Init();
                    towers[i].Free();
                    
                    _diContainer.Inject(_diContainer);
                }
            }
            
            _levelFlowController.Loaded += LevelFlowController_Loaded;
        }

        public TowerViewModel Get(TowerViewModelType type, Vector3 position, Quaternion rotation, Transform parent)
        {
            var freeTower = Towers[type].FirstOrDefault(x => x.IsFree);

            if (!freeTower)
            {
                freeTower = Instantiate(_towerSettings.GetPreset(type).TowerViewModelPrefab, transform);
                freeTower.Prepare(_towerSettings.GetPreset(type).TowerViewModelPrefab.Type);
                freeTower.Init();
                _diContainer.Inject(freeTower);
                Towers[type].Add(freeTower);
            }
            
            freeTower.Setup(type);

            var freeTowerTransform = freeTower.transform;
            freeTowerTransform.position = position;
            freeTowerTransform.rotation = rotation;
            freeTowerTransform.parent = parent;

            freeTower.SpawnFromPool();

            return freeTower;
        }
        
        private void LevelFlowController_Loaded()
        {
            foreach (var pair in Towers)
            {
                var towers = pair.Value;

                for (var i = 0; i < towers.Count; i++)
                {
                    towers[i].Free();
                }
            }
        }
    }
}