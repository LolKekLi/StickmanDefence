using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Project
{
    public class TowerController : MonoBehaviour
    {
        private PoolManager _poolManager = null;
        private EnemySpawner _enemySpawner = null;
        private TowerSettings _towerSettings = null;

        [field: SerializeField]
        public List<BaseTower> Towers
        {
            get;
            private set;
        } = new List<BaseTower>();

        [Inject]
        private void Construct(PoolManager poolManager, EnemySpawner enemySpawner, TowerSettings towerSettings)
        {
            _poolManager = poolManager;
            _enemySpawner = enemySpawner;
            _towerSettings = towerSettings;
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

                            if(tower.SeeTarget(enemy.transform.position))
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

        public BaseTower GetTower(BaseTower tower, Vector3 hitPoint, Quaternion quaternion)
        {
            var twr = _poolManager.Get<BaseTower>(tower, hitPoint, quaternion);

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
            });
            
            return twr;
        }

#if UNITY_EDITOR
        public void RefreshName()
        {
            for (var i = 0; i < Towers.Count; i++)
            {
                Towers[i].transform.name = $"{Towers[i].Type} {i}";
            }
        }
#endif
    }
}