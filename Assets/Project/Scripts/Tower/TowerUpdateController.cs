using System;
using System.Collections.Generic;
using Project.UI;
using UnityEngine;
using Zenject;

namespace Project
{
    public class TowerUpdateController : MonoBehaviour
    {
        public static readonly string TowerKey = "TowerKey";
        public static readonly string OnCloseWindowKey = "OnCloseWindow";

        private CameraController _cameraController = null;

        [Inject]
        private void Construct(CameraController cameraController)
        {
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

        private void JoystickController_Clicked(Vector2 clickPosition)
        {
            var screenPointToRay = _cameraController.Camera.ScreenPointToRay(clickPosition);

            if (Physics.Raycast(screenPointToRay, out var hit))
            {
                if (hit.collider.TryGetComponent(out BaseTower tower))
                {
                    var currentWindow = UISystem.Instance.CurrentWindow;

                    var upgradeTowerPopup = currentWindow as UpgradeTowerPopup;

                    tower.ToggleHighlight(true);

                    if (upgradeTowerPopup != null)
                    {
                        upgradeTowerPopup.RefreshData(tower);
                    }
                    else
                    {
                        Action onCloseWindow = () =>
                        {
                            tower.ToggleHighlight(false);
                        };

                        UISystem.ShowWindow<UpgradeTowerPopup>(new Dictionary<string, object>()
                        {
                            { TowerKey, tower },
                            { OnCloseWindowKey, onCloseWindow }
                        });
                    }
                }
            }
        }
    }
}