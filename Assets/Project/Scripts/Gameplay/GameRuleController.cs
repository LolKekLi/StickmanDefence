using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Project;
using Project.UI;
using UnityEngine;
using Zenject;

public class GameRuleController : MonoBehaviour
{
    public static event Action<int> CashCnaged = delegate { };
    public static event Action<int> HPChanged = delegate { };

    [SerializeField] private EnemySpawner _enemySpawner;

    [SerializeField] private float _delayBeforeStartWave;

    private int _currentHp;
    private int _cashCount;

    private CancellationTokenSource _spawnToken;

    public static GameRuleController Instance;

    [Inject] private TowerSettings _towerSettings;

    [Inject] private LevelFlowController _levelFlowController;

    [Inject] private EnemySettings _enemySettings;

    public int CashCount
    {
        get =>
            _cashCount;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        _currentHp = LevelSelectorWindow.HpOnLvl;
        _cashCount = LevelSelectorWindow.StartCashValue;
        
        Debug.Log(_enemySettings);
    }

    private void OnEnable()
    {
        TowerSpawnController.TowerSpawned += TowerSpawnController_TowerSpawned;
        Enemy.Died += Enemy_Died;
    }

    private void Enemy_Died(EnemyType type)
    {
        ChangeCash(_enemySettings.GetEnemyPreset(type).Cost);
    }

    private void OnDisable()
    {
        TowerSpawnController.TowerSpawned -= TowerSpawnController_TowerSpawned;

        UniTaskUtil.RefreshToken(ref _spawnToken);
    }

    private async void Start()
    {
        try
        {
            var cancellationToken = UniTaskUtil.RefreshToken(ref _spawnToken);

            await UniTask.Delay(TimeSpan.FromSeconds(_delayBeforeStartWave), cancellationToken: cancellationToken);

            _enemySpawner.SpawnEnemyLoop(LevelSelectorWindow.WaveSettings, cancellationToken, OnEnemyReachPosition)
                .Forget();
        }
        catch (OperationCanceledException e)
        {
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ChangeCash(1000);
        }
    }

    private void OnEnemyReachPosition(int damage)
    {
        _currentHp -= damage;
        HPChanged(_currentHp);

        if (_currentHp <= 0)
        {
            UniTaskUtil.RefreshToken(ref _spawnToken);
            _levelFlowController.Fail();
        }
    }

    private void TowerSpawnController_TowerSpawned(TowerType towerType)
    {
        ChangeCash(-_towerSettings.GetTowerPresetByType(towerType).Cost);
    }

    public void OnTowerCell(TowerType towerType)
    {
        ChangeCash(_towerSettings.GetTowerPresetByType(towerType).Cost / 2);
    }

    public void OnTowerUpgrade(int cost)
    {
        ChangeCash(-cost);
    }

    private void ChangeCash(int cost)
    {
        _cashCount += cost;
        CashCnaged(_cashCount);
    }

    public void Complete()
    {
        _levelFlowController.Complete();
    }
}