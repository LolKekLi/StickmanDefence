using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Project
{
    public class TowerController : MonoBehaviour
    {
        private EnemySpawner _enemySpawner = null;
        private TowerSettings _towerSettings = null;
        private TowerFactory _towerFactory;

        [Inject]
        private PoolManager _poolManager;

        private TowerViewModelFactory _towerViewModelFactory;
        private TowerViewModelSettings _towerViewModelSettings;
        private AttackControllerFactory _attackControllerFactory;

        [field: SerializeField]
        public List<BaseTower> Towers
        {
            get;
            private set;
        } = new List<BaseTower>();


        [Inject]
        private void Construct(EnemySpawner enemySpawner, TowerSettings towerSettings, TowerFactory towerFactory,
            TowerViewModelFactory towerViewModelFactory, TowerViewModelSettings towerViewModelSettings)
        {
            _towerViewModelSettings = towerViewModelSettings;
            _towerViewModelFactory = towerViewModelFactory;
            _towerFactory = towerFactory;
            _enemySpawner = enemySpawner;
            _towerSettings = towerSettings;
        }

        private void Start()
        {
            _attackControllerFactory = new AttackControllerFactory();
        }

        private void Update()
        {
            if (Towers.Count > 0 && _enemySpawner.Enemies.Count > 0)
            {
                for (int i = 0; i < Towers.Count; i++)
                {
                    var tower = Towers[i];

                    if (!tower.IsAttack)
                    {
                        for (int j = 0; j < _enemySpawner.Enemies.Count; j++)
                        {
                            var enemy = _enemySpawner.Enemies[j];

                            if (tower.SeeTarget(enemy.transform.position))
                            {
                                tower.ChangeTarget(enemy);
                                tower.Attack();

                                break;
                            }
                        }
                    }
                    else
                    {
                        if (tower.IsLostTarget)
                        {
                            var enemy = TryFindNewTarget(tower);

                            if (enemy != null)
                            {
                                tower.ChangeTarget(enemy);
                            }
                            else
                            {
                                tower.StopAttack();
                            }
                        }
                        else if (tower.Target.IsDied)
                        {
                            tower.StopAttack();
                        }
                    }
                }
            }
        }

        private Enemy TryFindNewTarget(BaseTower tower)
        {
            for (int i = 0; i < _enemySpawner.Enemies.Count; i++)
            {
                var enemy = _enemySpawner.Enemies[i];

                if (tower.SeeTarget(enemy.transform.position))
                {
                    return enemy;
                }
            }

            return null;
        }

        public BaseTower GetTower(TowerType tower, TowerViewModelType towerViewModelType, Vector3 hitPoint,
            Quaternion quaternion)
        {
            var twr = _towerFactory.Get(tower, hitPoint, quaternion, transform);

            var towerViewModelPreset = _towerViewModelSettings.GetPreset(towerViewModelType);
            var towerViewModel = _towerViewModelFactory.Get(towerViewModelPreset.TowerViewModelType,
                Vector3.zero, Quaternion.identity, twr.transform);
            

            twr.OnGetTowerFromPool(_towerSettings, _poolManager,
                () =>
                {
                    Towers.Add(twr);
                },
                () =>
                {
                    Towers.Remove(twr);

#if UNITY_EDITOR
                    RefreshName();
#endif
                }, towerViewModel);

            return twr;
        }

#if UNITY_EDITOR
        public void RefreshName()
        {
            for (var i = 0; i < Towers.Count; i++)
            {
                Towers[i].transform.name = $"{Towers[i].TowerType} {i}";
            }
        }
#endif
        public void ChangeViewModel(IUpgradeable currentTower, TowerViewModelType newViewModelType,
            FirePreset firePreset)
        {
            var indexOf = Towers.IndexOf((BaseTower)currentTower);

            var tower = Towers[indexOf];
            var towerViewModel = _towerViewModelFactory.Get(newViewModelType, Vector3.zero, Quaternion.identity, tower.transform);

            tower.ChangeViewModel(towerViewModel, _attackControllerFactory.GetAttackController(firePreset.FireType), firePreset);
        }
    }
}