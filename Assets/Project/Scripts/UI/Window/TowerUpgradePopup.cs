using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Project.UI
{
    public class TowerUpgradePopup : Window
    {
        [SerializeField]
        private Button _cellButton = null;

        [SerializeField]
        private Image _towerIcon = null;

        [SerializeField]
        private Button _closeButton = null;

        [SerializeField]
        private float _clickSideDetectOffset;

        [SerializeField]
        private TextMeshProUGUI _towerLabel = null;

        [SerializeField, Header("PerkItem")]
        private UIUpdatePerkItem[] _upgardePerkItems = null;

        [SerializeField, Header("Tweens")]
        private Vector2 _startPositionRight = Vector2.zero;

        [SerializeField]
        private Vector2 _endPositionRight = Vector2.zero;

        [SerializeField]
        private Vector2 _startPositionLeft = Vector2.zero;

        [SerializeField]
        private Vector2 _endPositionLeft = Vector2.zero;

        [SerializeField]
        private RectTransform _backgroundRectTransform;

        [SerializeField]
        private float _duration = 0f;

        [SerializeField]
        private AnimationCurve _tweenCurve;

        private bool _isClosing = false;
        private bool _isMoving = false;
        private bool _isRightClick;

        private Action _onCloseCallback = null;
        private CancellationToken _moveToken;

        //TODO: есть ли смысл в этой перменной ?
        private IUpgradeable _targetTower = null;
        private CancellationTokenSource _moveTokenSource = null;

        [Inject]
        private UISystem _uiSystem = null;

        [Inject]
        private TowerSettings _towerSettings = null;

        [Inject]
        private UITowerSetting _towerUiSetting;

        private TowerUpgradeController _towerUpgradeController;
        private Vector2 _startCloseButtonPos;
        private RectTransform _closeButtonRectTransform;
        private UpgradeLinePerkType? _maxUpgradeLinePerkType;

        public override bool IsPopup
        {
            get =>
                true;
        }

        public override void Preload()
        {
            base.Preload();

            _closeButtonRectTransform = _closeButton.GetComponent<RectTransform>();
            _startCloseButtonPos = _closeButtonRectTransform.anchoredPosition;

            _upgardePerkItems.Do(x =>
            {
                x.Prepare(() =>
                {
                    if (_towerUpgradeController.CanUpgrade(x.PerkLineType))
                    {
                        _towerUpgradeController.UpgradeTower(x.PerkLineType);

                        SetupPerkItems();
                    }
                });
            });
        }

        protected override void Start()
        {
            base.Start();

            _closeButton.onClick.AddListener(OnClose);
            _cellButton.onClick.AddListener(OnCellTower);

            _moveToken = UniTaskUtil.RefreshToken(ref _moveTokenSource);
        }

        protected override void OnShow()
        {
            base.OnShow();

            _targetTower = GetDataValue<BaseTower>(TowerUpgradeController.TowerKey);

            _onCloseCallback = GetDataValue<Action>(TowerUpgradeController.OnCloseWindowKey);
            var mousePositionX = GetDataValue<float>(TowerUpgradeController.MousePositionXKey);

            //TODO: Инжектить это дело 
            _towerUpgradeController =
                GetDataValue<TowerUpgradeController>(TowerUpgradeController.TowerUpgradeControllerKey);

            _isRightClick = IsRightClick(mousePositionX);

            _backgroundRectTransform.anchoredPosition = _isRightClick ? _startPositionLeft : _startPositionRight;

            _closeButtonRectTransform.anchoredPosition = !_isRightClick
                ? _startCloseButtonPos.ChangeX(-_startCloseButtonPos.x)
                : _startCloseButtonPos;

            MoveTo(_isRightClick ? _endPositionLeft : _endPositionRight).Forget();

            SetupElements();
        }

        public void RefreshData(IUpgradeable newTargetTower, float mousePositionX)
        {
            if (_isClosing)
            {
                var isRightClick = IsRightClick(mousePositionX);

                if (isRightClick != _isRightClick)
                {
                    _backgroundRectTransform.anchoredPosition = isRightClick ? _startPositionLeft : _startPositionRight;
                }

                _isRightClick = isRightClick;

                MoveTo(_isRightClick ? _endPositionLeft : _endPositionRight).Forget();
            }

            _targetTower = newTargetTower;

            SetupElements();
        }

        private void SetupElements()
        {
            var upgradeLinePerkType = _towerUpgradeController.GetMaxUpgradeType();

            OnFindMaxUpgradeType(upgradeLinePerkType);

            SetupTowerInfo();

            SetupPerkItems();
        }
        
        private void RefreshUpgradeLevelIndicators(UpgradeLinePerkType? upgradeLinePerkType)
        {
            _upgardePerkItems.Do(x =>
            {
                var maxUpgradeLvl = upgradeLinePerkType.HasValue
                    ? (x.PerkLineType == upgradeLinePerkType.Value
                        ? _towerUpgradeController.MaxUpgradeLvl
                        : _towerUpgradeController.MiddleUpgradeLvl)
                    : _towerUpgradeController.MaxUpgradeLvl;

                x.RefreshUpgradeLevelIndicator(_targetTower.GetUpgradeLevel(x.PerkLineType), maxUpgradeLvl);
            });
        }

        public void OnFindMaxUpgradeType(UpgradeLinePerkType? upgradeLinePerkType)
        {
            RefreshUpgradeLevelIndicators(upgradeLinePerkType);

            ChangePerkItemsBackColor(upgradeLinePerkType);
        }

        private void ChangePerkItemsBackColor(UpgradeLinePerkType? maxUpgradeLinePerkType)
        {
            _upgardePerkItems.Do(x =>
            {
                var upgradeLevel = _towerUpgradeController.GetUpgradeLevel(x.PerkLineType);

                x.ChangeBackColor(maxUpgradeLinePerkType.HasValue && (x.PerkLineType == maxUpgradeLinePerkType.Value
                    ? upgradeLevel == _towerUpgradeController.MaxUpgradeLvl
                    : upgradeLevel == _towerUpgradeController.MiddleUpgradeLvl));
            });
        }

        private void SetupPerkItems()
        {
            var maxUpgradeType = _towerUpgradeController.GetMaxUpgradeType();
            var lockLinePerkType = _towerUpgradeController.GetLockLineType();

            for (int i = 0; i < _upgardePerkItems.Length; i++)
            {
                var uiUpdatePerkItem = _upgardePerkItems[i];

                var upgradeLevel = _towerUpgradeController.GetUpgradeLevel(uiUpdatePerkItem.PerkLineType);

                var presetByLineType =
                    _towerUpgradeController.CurrentTowerUpdateSettings.GetPresetByLineType(
                        uiUpdatePerkItem.PerkLineType);

                var isMaxUpgradeLvlLine = maxUpgradeType.HasValue &&
                    (maxUpgradeType.Value == uiUpdatePerkItem.PerkLineType
                        ? upgradeLevel == _towerUpgradeController.MaxUpgradeLvl
                        : upgradeLevel == _towerUpgradeController.MiddleUpgradeLvl);

                var isLock = lockLinePerkType.HasValue && lockLinePerkType.Value == uiUpdatePerkItem.PerkLineType;

                var updatePreset =
                    presetByLineType[upgradeLevel == presetByLineType.Length ? upgradeLevel - 1 : upgradeLevel];

                uiUpdatePerkItem.Setup(updatePreset,
                    isMaxUpgradeLvlLine, isLock, upgradeLevel - 1);
            }
        }

        private void SetupTowerInfo()
        {
            var towerPreset = _towerUiSetting.GetUITowerPreset(_targetTower.TowerViewModelType);

            _towerIcon.sprite = towerPreset.UIIcon;
            _towerLabel.text = towerPreset.TowerLabel;
        }

        protected override void OnHide()
        {
            base.OnHide();

            _targetTower = null;
        }

        private async void OnClose()
        {
            if (!_isClosing)
            {
                try
                {
                    _isClosing = true;

                    _onCloseCallback?.Invoke();

                    await MoveTo(_isRightClick ? _startPositionLeft : _startPositionRight, () => { Hide(); });

                    _isClosing = false;
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
                //TODO: сделать инжект сразу в окна
                _towerUpgradeController.CellTower();

                OnClose();
            }
        }

        private bool IsRightClick(float xPos)
        {
            var isRightClick = xPos > Screen.height / 2f - _clickSideDetectOffset;
            return isRightClick;
        }

        private async UniTask MoveTo(Vector2 pos, Action callback = null)
        {
            if (_isMoving)
            {
                _moveToken = UniTaskUtil.RefreshToken(ref _moveTokenSource);
            }

            await MoveAsync(pos, _moveToken, callback);
        }

        private async UniTask MoveAsync(Vector2 endPos, CancellationToken token, Action callback)
        {
            try
            {
                _isMoving = true;

                var startPosition = _backgroundRectTransform.anchoredPosition;

                // var currentDuration = Math.Abs(startPosition.x - endPos.x) * _duration /
                //     (_startPositionRight.x - _endPositionRight.x);

                await UniTaskExtensions.Lerp(
                    x => { _backgroundRectTransform.anchoredPosition = Vector2.Lerp(startPosition, endPos, x); },
                    _duration, _tweenCurve, token);

                _isMoving = false;

                callback?.Invoke();
            }
            catch (OperationCanceledException e)
            {
            }
        }

        public void Close()
        {
            OnClose();
        }
    }
}