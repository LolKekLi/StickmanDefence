using System;
using System.Threading;
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

        private bool _isClose = false;

        private Action _onCloseCallback = null;

        private BaseTower _targetTower = null;

        [Inject]
        private UISystem _uiSystem = null;

        [Inject]
        private TowerSettings _towerSettings = null;

        private CancellationTokenSource _closeToken;
        private CancellationToken _cancellationToken;

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

            _targetTower = GetDataValue<BaseTower>(TowerUpdateController.TowerKey);
            _onCloseCallback = GetDataValue<Action>(TowerUpdateController.OnCloseWindowKey);

            SetupElements();
        }

        public void RefreshData(BaseTower newTargetTower)
        {
            if (_isClose)
            {
                _isClose = false;
                UniTaskUtil.CancelToken(ref _closeToken);
                _onShowAnimation.Play();
            }
            
            _targetTower.ToggleHighlight(false);
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
            if (!_isClose)
            {
                try
                {
                    _isClose = true;

                    _onCloseCallback?.Invoke();

                    _onCloseAnimation.Play();

                    _cancellationToken = UniTaskUtil.RefreshToken(ref _closeToken);

                    await UniTask.Delay(TimeSpan.FromSeconds(_onCloseAnimation.duration),
                        cancellationToken: _cancellationToken);

                    _isClose = false;

                    Hide();
                }
                catch (OperationCanceledException e)
                {
                }
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