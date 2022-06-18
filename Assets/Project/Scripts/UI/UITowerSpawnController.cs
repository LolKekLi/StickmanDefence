using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Project.UI
{
    public class UITowerSpawnController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public static event Action<TowerType> PointerExited = delegate { };
        public static event Action PointerEntered = delegate { };

        [SerializeField]
        private Transform _buttonGroup = null;

        [SerializeField]
        private TowerSpawnButton _towerSpawnButtonPrefab = null;

        private bool _isClicked = false;

        private TowerType _currentTowerType = default;

        private TowerSettings _towerSettings = null;

        private TowerSpawnButton[] _towerSpawnButtons = null;

        [Inject]
        private void Construct(TowerSettings towerSettings)
        {
            _towerSettings = towerSettings;
        }

        private void Start()
        {
            var towerTypes = (TowerType[])Enum.GetValues(typeof(TowerType));
            _towerSpawnButtons = new TowerSpawnButton[towerTypes.Length];

            for (int i = 0; i < towerTypes.Length; i++)
            {
                var towerSpawnButton = Instantiate(_towerSpawnButtonPrefab, _buttonGroup);
                var towerType = towerTypes[i];

                _towerSpawnButtons[i] = towerSpawnButton;

                towerSpawnButton.Setup(_towerSettings.GetPresetByType(towerType), i, (index) =>
                {
                    _currentTowerType = towerType;
                    _isClicked = true;

                    for (int j = 0; j < _towerSpawnButtons.Length; j++)
                    {
                        _towerSpawnButtons[j].Refresh(j == index);
                    }
                    
                });
            }
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            PointerEntered();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_isClicked)
            {
                _isClicked = false;

                PointerExited(_currentTowerType);
            }
        }
    }
}