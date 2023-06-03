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
        private int _selectedButtonIndex = -1;

        private TowerType _currentTowerType = default;

        private TowerSettings _towerSettings = null;

        private TowerSpawnButton[] _towerSpawnButtons = null;
        private UITowerSetting _uiTowerSetting;


        [Inject]
        private void Construct(TowerSettings towerSettings, UITowerSetting uiTowerSetting)
        {
            _uiTowerSetting = uiTowerSetting;
            _towerSettings = towerSettings;
        }

        private void OnEnable()
        {
            TowerSpawnController.TowerSpawned += TowerSpawnController_TowerSpawned;
        }

        private void OnDisable()
        {
            TowerSpawnController.TowerSpawned -= TowerSpawnController_TowerSpawned;
        }

        private void Start()
        {
            var towerTypes = (TowerType[])Enum.GetValues(typeof(TowerType));
            _towerSpawnButtons = new TowerSpawnButton[towerTypes.Length];

            for (int i = 0; i < towerTypes.Length; i++)
            {
                var towerSpawnButton = Instantiate(_towerSpawnButtonPrefab, _buttonGroup);
                var towerType = towerTypes[i];

                if (towerType == TowerType.Bugler)
                {
                    continue;
                }

                _towerSpawnButtons[i] = towerSpawnButton;

                var towerViewModelType = _towerSettings.GetTowerPresetByType(towerType).BaseViewModelType;

                if (towerViewModelType != null)
                {
                    towerSpawnButton.Setup(_uiTowerSetting.GetUITowerPreset(towerViewModelType), i, (index) =>
                    {
                        var isSameButtonClick = _selectedButtonIndex == index;
                        var isFirstClick = _selectedButtonIndex < 0;

                        if (!isFirstClick)
                        {
                            if (isSameButtonClick && _towerSpawnButtons[_selectedButtonIndex].IsSelected)
                            {
                                _isClicked = false;

                                _towerSpawnButtons[_selectedButtonIndex].Refresh(false);

                                return;
                            }

                            _towerSpawnButtons[_selectedButtonIndex].Refresh(false);
                        }

                        _selectedButtonIndex = index;
                        _towerSpawnButtons[_selectedButtonIndex].Refresh(true);
                        _currentTowerType = towerType;
                        _isClicked = true;
                    });
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            PointerEntered();

            if (_selectedButtonIndex >= 0)
            {
                _towerSpawnButtons[_selectedButtonIndex].Refresh(false);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_isClicked)
            {
                _isClicked = false;

                PointerExited(_currentTowerType);
            }
        }

        private void TowerSpawnController_TowerSpawned()
        {
            _towerSpawnButtons[_selectedButtonIndex].Refresh(false);
        }
    }
}