using System.Collections;
using System.Collections.Generic;
using PathCreation;
using UnityEngine;
using Zenject;

namespace Project
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField]
        private EndOfPathInstruction _endOfPathInstruction = default;
       
        [SerializeField]
        private List<PathCreator> _paths = null;

        [SerializeField]
        private int _enemyCount = 0;
        [SerializeField]
        private float _spawnDelay = 0f;

        private int _currentEnemyCount = 0;

        private EnemySettings _enemySettings = null;
        private PoolManager _poolManager = null;

        private Coroutine _spawnEnemyCor = null;

        [field: SerializeField]
        public List<Enemy> Enemies
        {
            get;
            private set;
        }

        [Inject]
        private void Construct(EnemySettings enemySettings, PoolManager poolManager)
        {
            _enemySettings = enemySettings;
            _poolManager = poolManager;
        }
        
        private void Start()
        {
            Enemies = new List<Enemy>();

         //   _spawnEnemyCor = StartCoroutine(SpawnEnemyCor());
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SpawnEnemy();
            }
        }

        private IEnumerator SpawnEnemyCor()
        {
            var waiter = new WaitForSeconds(_spawnDelay);
            
            while (_currentEnemyCount < _enemyCount)
            {
                SpawnEnemy();
                
                yield return waiter;
            }
        }

        private void SpawnEnemy()
        {
            var enemyPreset = _enemySettings.GetEnemyPreset(EnemyType.Base);

            var enemy = _poolManager.Get<Enemy>(enemyPreset.EnemyPrefab, _paths[0].bezierPath[0], Quaternion.identity);
                
            Enemies.Add(enemy);

            enemy.Setup(enemyPreset,() =>
            {
                Enemies.Remove(enemy);
            });
                
            enemy.StartFollowPath(_paths[0], _endOfPathInstruction);

            _currentEnemyCount++;
        }
    }
}