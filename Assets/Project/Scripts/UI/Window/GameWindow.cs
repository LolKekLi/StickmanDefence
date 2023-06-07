using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Zenject;

namespace Project.UI
{
    public class GameWindow : Window
    {
        public static event Action<UltimateType> UltimateApplied = delegate {  }; 
        
        public readonly int MaxPerkCount = 10;
        
        [SerializeField, Space]
        public TextMeshProUGUI _waveCounter;

        [SerializeField]
        private TextMeshProUGUI _cashCounter;

        [SerializeField]
        private TextMeshProUGUI _healthCounter;

        [SerializeField, Space]
        private TextMeshProUGUI _waveAnoncer;

        [SerializeField]
        private BaseTweenController _waveAnonscerTweeen;

        [SerializeField, Space]
        public UIUltimateItem _UIUltimateItemPrefab;

        [SerializeField]
        public Transform _UIUltimateGroup;

        [Inject]
        private UltimateSettings _ultimateSettings;

        private List<UIUltimateItem> _uiUltimateItems = new List<UIUltimateItem>();

        public override bool IsPopup
        {
            get =>
                false;
        }

        public override void Preload()
        {
            base.Preload();

            for (int i = 0; i < MaxPerkCount; i++)
            {
                var uiUltimateItem = Instantiate(_UIUltimateItemPrefab, _UIUltimateGroup);
                uiUltimateItem.ToggleActive(false);
                _uiUltimateItems.Add(uiUltimateItem);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            EnemySpawner.WaveCompleted += EnemySpawner_WaveCompleted;
            EnemySpawner.WaveStarted += EnemySpawner_WaveStarted;
            GameRuleController.HPChanged += GameRuleController_HPChanged;
            GameRuleController.CashCnaged += GameRuleController_CashCnaged;
            AceTower.Built += AceTower_Built;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            EnemySpawner.WaveStarted -= EnemySpawner_WaveStarted;
            EnemySpawner.WaveCompleted -= EnemySpawner_WaveCompleted;
            GameRuleController.HPChanged -= GameRuleController_HPChanged;
            AceTower.Built -= AceTower_Built;
        }

        protected override void OnShow()
        {
            base.OnShow();

            RefreshHealthCounter(LevelSelectorWindow.HpOnLvl);
            RefreshWaveCounter(0);
            RefreshCashCounter(LevelSelectorWindow.StartCashValue);
        }

        private void RefreshHealthCounter(int hp)
        {
            _healthCounter.text = $"{hp}";
        }

        private void RefreshWaveCounter(int waveIndex)
        {
            _waveCounter.text = $"{waveIndex}/{LevelSelectorWindow.WaveSettings.WavePresets.Length}";
        }

        private void RefreshCashCounter(int currentCashValue)
        {
            _cashCounter.text = $"{currentCashValue}$";
        }

        private void OnUIEllementClick(UltimateType type)
        {
            UltimateApplied(type);
        }

        private void EnemySpawner_WaveCompleted(int waveIndex)
        {
            RefreshWaveCounter(waveIndex);
        }

        private void GameRuleController_HPChanged(int currentHp)
        {
            RefreshHealthCounter(currentHp);
        }

        private void EnemySpawner_WaveStarted(int index)
        {
            RefreshWaveCounter(index+1);
            _waveAnoncer.text = $"WAVE {index + 1}";
            _waveAnonscerTweeen.Play();
        }

        private void AceTower_Built(UltimateType ultimateType)
        {
            var freeUIUltimateItem = _uiUltimateItems.FirstOrDefault(x=>x.enabled);

            if (freeUIUltimateItem == null)
            {
                Debug.LogError("Нет свободных перков");
                
                return;
            }

            var ultimatePreset = _ultimateSettings.GetPreset(ultimateType);
            freeUIUltimateItem.Setup(ultimatePreset.Sprite, ultimatePreset.ReloadTime, ultimateType, OnUIEllementClick);
            freeUIUltimateItem.ToggleActive(true);
        }

        private void GameRuleController_CashCnaged(int value)
        {
            RefreshCashCounter(value);
        }
    }
}