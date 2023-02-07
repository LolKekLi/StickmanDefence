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

                            if ((tower.transform.position.ChangeY(enemy.transform.position.y) -
                                    enemy.transform.position)
                                .sqrMagnitude <= tower.SqrAttackRadius)
                            {
                                tower.ChangeTarget(enemy);
                                tower.Attack();

                                break;
                            }
                        }
                    }
                    else
                    {
                        if ((tower.transform.position - tower.Target.transform.position).sqrMagnitude >=
                            tower.SqrAttackRadius)
                        {
                            var enemy = TryFindNewTarget(tower);

                            if (enemy != null)
                            {
                                tower.ChangeTarget(enemy);
                            }
                            else
                            {
                                Debug.Log("TowerController stop attack");
                                tower.StopAttack();
                            }
                        }
                        else if (tower.Target.IsDied)
                        {
                            Debug.Log("TowerController stop attack 1");
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

                if ((tower.transform.position.ChangeY(enemy.transform.position.y) - enemy.transform.position)
                    .sqrMagnitude <= tower.SqrAttackRadius)
                {
                    return enemy;
                }
            }

            return null;
        }

        public BaseTower GetTower(BaseTower tower, Vector3 hitPoint, Quaternion quaternion)
        {
            var twr = _poolManager.Get<BaseTower>(tower, hitPoint, quaternion);

            twr.Spawn(_towerSettings, _poolManager, () => { Towers.Add(twr); });

            return twr;
        }

        public void CellTower(BaseTower targetTower)
        {
            Towers.Remove(targetTower);

            targetTower.Cell();
        }
    }
}