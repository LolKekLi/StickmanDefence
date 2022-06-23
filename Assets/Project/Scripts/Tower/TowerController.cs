using System.Collections.Generic;
using Project;
using UnityEngine;
using Zenject;

public class TowerController : MonoBehaviour
{
    private PoolManager _poolManager = null;
    private EnemySpawner _enemySpawner = null;
    private TowerSettings _towerSettings = null;

    private List<Tower> _towers = new List<Tower>();

    [Inject]
    private void Construct(PoolManager poolManager, EnemySpawner enemySpawner, TowerSettings towerSettings)
    {
        _poolManager = poolManager;
        _enemySpawner = enemySpawner;
        _towerSettings = towerSettings;
    }
    
    void Update()
    {
        if (_towers.Count > 0 && _enemySpawner.Enemies.Count > 0)
        {
            for (int i = 0; i < _towers.Count; i++)
            {
                var tower = _towers[i];

                if (!tower.IsAttacked)
                {
                    for (int j = _enemySpawner.Enemies.Count - 1; j >= 0; j--)
                    {
                        var enemy = _enemySpawner.Enemies[j];

                        if ((tower.transform.position.ChangeY(enemy.transform.position.y) - enemy.transform.position).sqrMagnitude <= tower.SqrAttackRadius)
                        {
                            tower.Attack(enemy);
                            
                            break;
                        }
                    }
                }
                else
                {
                    if ((tower.transform.position - tower.Target.transform.position).sqrMagnitude >= tower.SqrAttackRadius)
                    {
                        tower.StopAttack();
                    }
                }
            }
        }
    }

    public Tower GetTower(Tower tower, Vector3 hitPoint, Quaternion quaternion)
    {
        var twr = _poolManager.Get<Tower>(tower, hitPoint, quaternion);

        twr.Spawn(_towerSettings, () =>
        {
            _towers.Add(twr);
        });

        return twr;
    }
}