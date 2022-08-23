using System.Collections.Generic;
using Project.UI;
using UnityEngine;
using Zenject;

namespace Project
{
    public class TowerUpdateController : MonoBehaviour
    {
        public static readonly string TowerKey = "TowerKey";

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
                if (hit.collider.TryGetComponent(out Tower tower))
                {
                    var currentWindow = UISystem.Instance.CurrentWindow;

                    var upgradeTowerPopup = currentWindow as UpgradeTowerPopup;

                    if (upgradeTowerPopup != null)
                    {
                        upgradeTowerPopup.RefreshData(tower);
                    }
                    else
                    {
                        UISystem.ShowWindow<UpgradeTowerPopup>(new Dictionary<string, object>()
                        {
                            { TowerKey, tower }
                        });
                    }
                }
            }
        }
    }
}