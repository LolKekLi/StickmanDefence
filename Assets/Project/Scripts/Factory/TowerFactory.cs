using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Project
{
    public class TowerFactory : ZenjectManager<TowerFactory>
    {
        [Inject]
        private TowerSettings _towerSettings;

        [Inject]
        private LevelFlowController _levelFlowController;

        [Inject]
        private DiContainer _diContainer;

        private readonly Dictionary<TowerType, List<BaseTower>> Towers = new Dictionary<TowerType, List<BaseTower>>()
        {
            { TowerType.Gunner, new List<BaseTower>(5) },
            { TowerType.ShotGunner, new List<BaseTower>(5) },
            { TowerType.Granatman, new List<BaseTower>(5) },
            { TowerType.Sniper, new List<BaseTower>(5) },
            { TowerType.PPMan, new List<BaseTower>(5) },
            { TowerType.Bugler, new List<BaseTower>(1) },
            { TowerType.Santa, new List<BaseTower>(1) },
        };

        protected override void Init()
        {
            base.Init();

            foreach (var pair in Towers)
            {
                var towers = pair.Value;

                var preset = _towerSettings.GetTowerPresetByType(pair.Key);

                for (var i = 0; i < towers.Capacity; i++)
                {
                    towers.Add(Instantiate(preset.TowerPrefab, transform));
                    towers[i].Prepare(preset.TowerPrefab.Type);
                    towers[i].Init();
                    towers[i].Free();

                    _diContainer.Inject(towers[i]);
                }
            }

            _levelFlowController.Loaded += LevelFlowController_Loaded;
        }

        public BaseTower Get(TowerType type, Vector3 position, Quaternion rotation, Transform parent)
        {
            var freeTower = Towers[type].FirstOrDefault(x => x.IsFree);

            if (!freeTower)
            {
                freeTower = Instantiate(_towerSettings.GetTowerPresetByType(type).TowerPrefab, transform);
                freeTower.Prepare(_towerSettings.GetTowerPresetByType(type).TowerPrefab.Type);
                freeTower.Init();
                _diContainer.Inject(freeTower);
                Towers[type].Add(freeTower);
            }

            var freeTowerTransform = freeTower.transform;
            freeTowerTransform.position = position;
            freeTowerTransform.rotation = rotation;

            if (parent != null)
            {
                freeTowerTransform.parent = parent;
            }

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