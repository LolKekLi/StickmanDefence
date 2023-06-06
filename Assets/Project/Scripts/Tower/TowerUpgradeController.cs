using System;
using System.Collections.Generic;
using Project.UI;
using UnityEngine;
using Zenject;

namespace Project
{
    public class TowerUpgradeController : MonoBehaviour
    {
        public static readonly string TowerKey = "TowerKey";
        public static readonly string OnCloseWindowKey = "OnCloseWindow";
        public static readonly string MousePositionXKey = "MousePosition";
        public static readonly string TowerUpgradeControllerKey = "TowerUpgradeController";

        public readonly int MaxUpgradeLvl = 4;
        public readonly int MiddleUpgradeLvl = 2;

        private Action _onCloseWindowCallback = null;

        private CameraController _cameraController = null;
        private IUpgradeable _oldTower = null;
        private Dictionary<string, object> _upgradeTowerPopupInfo = null;
        private TowerController _towerController;

        private UpgradeLinePerkType[] _upgradeLinePerkTypes;
        private TowerSettings _towerSettings;
        private TowerUpgradePopup _upgradeTowerPopup;
        private float _mousePositionX;

        public TowerUpgradeSettings CurrentTowerUpdateSettings
        {
            get;
            private set;
        }

        public IUpgradeable CurrentTower
        {
            get;
            private set;
        }

        [Inject]
        private void Construct(CameraController cameraController, TowerController towerController,
            TowerSettings towerSettings)
        {
            _towerSettings = towerSettings;
            _towerController = towerController;
            _cameraController = cameraController;
        }

        private void OnEnable()
        {
            JoystickController.Clicked += JoystickController_Clicked;
        }

        private void OnDisable()
        {
            JoystickController.Clicked -= JoystickController_Clicked;
        }

        private void Start()
        {
            _upgradeLinePerkTypes = (UpgradeLinePerkType[])Enum.GetValues(typeof(UpgradeLinePerkType));

            _onCloseWindowCallback = () =>
            {
                if (CurrentTower == null)
                {
                    return;
                }

                CurrentTower.UnSelected();
            };

            _upgradeTowerPopupInfo = new Dictionary<string, object>()
            {
                { TowerKey, null },
                { MousePositionXKey, null },
                { OnCloseWindowKey, _onCloseWindowCallback },
                { TowerUpgradeControllerKey, this },
            };
        }

        private void RefreshTowerPopupInfo(IUpgradeable tower, float mousePositionX)
        {
            _upgradeTowerPopupInfo[TowerKey] = tower;
            _upgradeTowerPopupInfo[MousePositionXKey] = mousePositionX;
        }

        public void CellTower()
        {
            GameRuleController.Instance.OnTowerCell(CurrentTower.TowerType);
            CurrentTower.Sell();
            ChangeCurrentTower(null);
        }

        //TODO: Пересмотреть данный метод
        public void UnselectedCurrentTower()
        {
            CurrentTower.UnSelected();

            var currentWindow = UISystem.Instance.CurrentWindow;

            var upgradeTowerPopup = currentWindow as TowerUpgradePopup;

            if (upgradeTowerPopup != null)
            {
                upgradeTowerPopup.Close();
            }
        }

        public bool CanUpgrade(UpgradeLinePerkType upgradeLinePerkType)
        {
            var maxUpgradeType = CurrentTower.GetUpgradeInfo().MaxUpgradeType;

            if (maxUpgradeType.HasValue && maxUpgradeType.Value != upgradeLinePerkType)
            {
                return CurrentTower.GetUpgradeLevel(upgradeLinePerkType) < MiddleUpgradeLvl;
            }

            var level = CurrentTower.GetUpgradeInfo().LevelInfo[upgradeLinePerkType];


            return CurrentTower.GetUpgradeLevel(upgradeLinePerkType) < MaxUpgradeLvl &&
                CurrentTowerUpdateSettings.GetPresetByLineType(upgradeLinePerkType)[level].Cost <=
                GameRuleController.Instance.CashCount;
        }

        private void JoystickController_Clicked(Vector2 clickPosition)
        {
            var screenPointToRay = _cameraController.Camera.ScreenPointToRay(clickPosition);

            if (Physics.Raycast(screenPointToRay, out var hit))
            {
                if (hit.collider.TryGetComponent(out IUpgradeable tower))
                {
                    if (tower.TowerType == (TowerType)LocalConfig.AceTowerType)
                    {
                        return;
                    }

                    ChangeCurrentTower(tower);

                    if (_oldTower != null)
                    {
                        _oldTower.UnSelected();
                    }

                    _oldTower = tower;

                    _upgradeTowerPopup = UISystem.Instance.CurrentWindow as TowerUpgradePopup;

                    CurrentTower.Selected();

                    _mousePositionX = Input.mousePosition.x;
                    if (_upgradeTowerPopup != null && _upgradeTowerPopup)
                    {
                        _upgradeTowerPopup.RefreshData(CurrentTower, _mousePositionX);
                    }
                    else
                    {
                        RefreshTowerPopupInfo(tower, _mousePositionX);

                        UISystem.ShowWindow<TowerUpgradePopup>(_upgradeTowerPopupInfo);

                        _upgradeTowerPopup = UISystem.Instance.CurrentWindow as TowerUpgradePopup;
                    }
                }
            }
        }

        private void ChangeCurrentTower(IUpgradeable tower)
        {
            CurrentTower = tower;
            CurrentTowerUpdateSettings =
                tower != null ? _towerSettings.GetTowerUpdateSettings(CurrentTower.TowerType) : null;
        }

        public UpgradeLinePerkType? GetLockLineType()
        {
            var upgradeInfo = CurrentTower.GetUpgradeInfo();

            if (upgradeInfo.LockLineType.HasValue)
            {
                return upgradeInfo.LockLineType.Value;
            }

            int zeroCount = 0;
            UpgradeLinePerkType? result = null;

            for (int i = 0; i < _upgradeLinePerkTypes.Length; i++)
            {
                var upgradeLinePerkType = _upgradeLinePerkTypes[i];

                if (CurrentTower.GetUpgradeLevel(upgradeLinePerkType) == 0)
                {
                    zeroCount++;

                    if (zeroCount > 1)
                    {
                        return null;
                    }

                    result = upgradeLinePerkType;
                }
            }

            if (result.HasValue)
            {
                upgradeInfo.SetLockLineType(result.Value);
            }

            return result;
        }

        public UpgradeLinePerkType? GetMaxUpgradeType()
        {
            var upgradeInfo = CurrentTower.GetUpgradeInfo();

            if (upgradeInfo.MaxUpgradeType.HasValue)
            {
                return upgradeInfo.MaxUpgradeType.Value;
            }

            UpgradeLinePerkType? result = null;

            for (int i = 0; i < _upgradeLinePerkTypes.Length; i++)
            {
                var upgradeLevel = CurrentTower.GetUpgradeLevel(_upgradeLinePerkTypes[i]);
                if (upgradeLevel > 2)
                {
                    result = _upgradeLinePerkTypes[i];
                }
            }

            if (result.HasValue)
            {
                upgradeInfo.SetMaxUpgradeType(result.Value);
                _upgradeTowerPopup.OnFindMaxUpgradeType(result.Value);
            }

            return result;
        }

        public void UpgradeTower(UpgradeLinePerkType upgradeLinePerkType)
        {
            var presetByLineType = CurrentTowerUpdateSettings.GetPresetByLineType(upgradeLinePerkType);

            var level = CurrentTower.GetUpgradeInfo().LevelInfo[upgradeLinePerkType];
            
            GameRuleController.Instance.OnTowerUpgrade(
                (int)CurrentTowerUpdateSettings.GetPresetByLineType(upgradeLinePerkType)[level].Cost);

            CurrentTower.Upgrade(upgradeLinePerkType, presetByLineType,
                (newViewModelType, firePreset) =>
                {
                    _towerController.ChangeViewModel(CurrentTower, newViewModelType, firePreset);
                });

            _upgradeTowerPopup.RefreshData(CurrentTower, _mousePositionX);
        }

        public int GetUpgradeLevel(UpgradeLinePerkType perkLineType)
        {
            return CurrentTower.GetUpgradeLevel(perkLineType);
        }
    }
}