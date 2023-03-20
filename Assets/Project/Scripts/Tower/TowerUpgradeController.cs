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

        private Action _onCloseWindowCallback = null;
        
        private CameraController _cameraController = null;
        private IUpgradeable _oldTower = null;
        private Dictionary<string, object> _upgrateTowerPopupInfo = null;
        private TowerController _towerController;

        public IUpgradeable CurrentTower
        {
            get;
            private set;
        }

        [Inject]
        private void Construct(CameraController cameraController, TowerController towerController)
        {
            _towerController = towerController;
            _cameraController = cameraController;
        }

        private void Start()
        {
            _onCloseWindowCallback = () =>
            {
                if (CurrentTower == null)
                {
                    return;
                }
                
                CurrentTower.UnSelected();
            };
            
            _upgrateTowerPopupInfo = new Dictionary<string, object>()
            {
                { TowerKey, null },
                { MousePositionXKey, null },
                { OnCloseWindowKey, _onCloseWindowCallback},
                { TowerUpgradeControllerKey, this }
            };
        }

        private void OnEnable()
        {
            JoystickController.Clicked += JoystickController_Clicked;
        }

        private void OnDisable()
        {
            JoystickController.Clicked -= JoystickController_Clicked;
        }

        private void JoystickController_Clicked(Vector2 clickPosition)
        {
            var screenPointToRay = _cameraController.Camera.ScreenPointToRay(clickPosition);

            if (Physics.Raycast(screenPointToRay, out var hit))
            {
                if (hit.collider.TryGetComponent(out IUpgradeable tower))
                {
                    CurrentTower = tower;
                    
                    if (_oldTower != null)
                    {
                        _oldTower.UnSelected();
                    }
                    
                    _oldTower = tower;
                    
                    var currentWindow = UISystem.Instance.CurrentWindow;

                    var upgradeTowerPopup = currentWindow as UpgradeTowerPopup;

                    CurrentTower.Selected();

                    if (upgradeTowerPopup != null && upgradeTowerPopup)
                    {
                        upgradeTowerPopup.RefreshData(CurrentTower,  Input.mousePosition.x);
                    }
                    else
                    {

                        RefreshTowerPopupInfo(tower, Input.mousePosition.x );
                        
                        UISystem.ShowWindow<UpgradeTowerPopup>(_upgrateTowerPopupInfo);
                    }
                }
            }
        }

        private void RefreshTowerPopupInfo(IUpgradeable tower, float mousePositionX)
        {
            _upgrateTowerPopupInfo[TowerKey] = tower;
            _upgrateTowerPopupInfo[MousePositionXKey] = mousePositionX;
        }

        public void CellTower()
        {
            CurrentTower.Cell();
            CurrentTower = null;
        }

        //TODO: Пересмотреть данный метод
        public void UnselectedCurrentTower()
        {
            CurrentTower.UnSelected();
            var currentWindow = UISystem.Instance.CurrentWindow;

            var upgradeTowerPopup = currentWindow as UpgradeTowerPopup;

            if (upgradeTowerPopup != null)
            {
                upgradeTowerPopup.Close();
            }
        }
    }
}