using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using PathCreation;
using UnityEngine;
using Zenject;

namespace Project
{
    public class EnemySpawner : MonoBehaviour
    {
        public static event Action<int> WaveStarted = delegate { };
        public static event Action<int> WaveCompleted = delegate(int i) { };

        [SerializeField]
        private EndOfPathInstruction _endOfPathInstruction = default;

        [SerializeField]
        private List<PathCreator> _paths = null;

        [SerializeField]
        private int _enemyCount = 0;
        
        private int _currentEnemyCount = 0;
        private int _currentWaveIndex;

        private EnemySettings _enemySettings = null;
        private PoolManager _poolManager = null;
       

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
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SpawnEnemy(EnemyType.Base, null);
            }
        }


        public async UniTaskVoid SpawnEnemyLoop(EnemyWaveSettings enemyWaveSettings,
            CancellationToken cancellationToken, Action<int> onEnemyReachPosition)
        {
            try
            {
                var wavePresetsLength = enemyWaveSettings.WavePresets.Length;
                
                while (_currentWaveIndex <  wavePresetsLength)
                {
                    WaveStarted(_currentWaveIndex);
                    var wavePreset = enemyWaveSettings.WavePresets[_currentWaveIndex];

                    var sequencePresets = wavePreset.SequencePresets;

                    for (var i = 0; i < sequencePresets.Length; i++)
                    {
                        var sequencePreset = sequencePresets[i];

                        for (int j = 0; j < sequencePreset.EnemyCount; j++)
                        {
                            SpawnEnemy(sequencePreset.Type, onEnemyReachPosition);

                            await UniTask.Delay(TimeSpan.FromSeconds(sequencePreset.SpawnDelay),
                                cancellationToken: cancellationToken);
                        }

                        await UniTask.Delay(TimeSpan.FromSeconds(sequencePreset.DelayAfterSpawn),
                            cancellationToken: cancellationToken);
                    }

                    await UniTask.WaitUntil(() => Enemies.Count <= 0, cancellationToken: cancellationToken);

                    WaveCompleted(_currentWaveIndex);
                    
                    _currentWaveIndex++;
                }

                GameRuleController.Instance.Complete();

            }
            catch (OperationCanceledException e)
            {
            }
        }

        private void SpawnEnemy(EnemyType enemyType, Action<int> onEnemyReachPosition)
        {
            var enemyPreset = _enemySettings.GetEnemyPreset(enemyType);

            var enemy = _poolManager.Get<Enemy>(enemyPreset.EnemyPrefab, _paths[0].bezierPath[0], Quaternion.identity);

            Enemies.Add(enemy);

            enemy.Setup(enemyPreset, () =>
            {
                OnEnemyDiedAction(enemy);
            },onEnemyReachPosition);

            enemy.StartFollowPath(_paths[0], _endOfPathInstruction);

            _currentEnemyCount++;
        }

        private void OnEnemyDiedAction(Enemy enemy)
        {
            Enemies.Remove(enemy);
        }
    }
}