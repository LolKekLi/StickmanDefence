using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Project.UI
{
    public class UpgradeTowerPopup : Window
    {
        [SerializeField]
        private Button _cellButton = null;

        [SerializeField]
        private Image _icon = null;

        [SerializeField]
        private Button _closeButton = null;

        [SerializeField]
        private DOTweenAnimation _onShowAnimation = null;

        [SerializeField]
        private DOTweenAnimation _onCloseAnimation = null;

        private bool _isClosen = false;

        private Tower _targetTower = null;

        [Inject]
        private UISystem _uiSystem = null;
        [Inject]
        private TowerSettings _towerSettings = null;

        public override bool IsPopup
        {
            get =>
                true;
        }

        protected override void Start()
        {
            base.Start();

            _closeButton.onClick.AddListener(OnClose);
            _cellButton.onClick.AddListener(OnCellTower);
        }

        protected override void OnShow()
        {
            base.OnShow();
            
            _onShowAnimation.Play();

            _targetTower = GetDataValue<Tower>(TowerUpdateController.TowerKey);

            SetupElements();
        }
        
        public void RefreshData(Tower newTargetTower)
        {
            _targetTower = newTargetTower;

            SetupElements();
        }
        
        private void SetupElements()
        {
            var towerPreset = _towerSettings.GetPresetByType(_targetTower.Type);

            _icon.sprite = towerPreset.UIIcon;
        }

        private async void OnClose()
        {
            if (!_isClosen)
            {
                _isClosen = true;

                _onCloseAnimation.Play();

                await UniTask.Delay(TimeSpan.FromSeconds(_onCloseAnimation.duration));

                _isClosen = false;
                
                Hide();
            }
        }

        private void OnCellTower()
        {
            if (_targetTower != null)
            {
                _uiSystem.TowerController.CellTower(_targetTower);
                
                _targetTower = null;
                
                OnClose();
            }
        }
    }
}