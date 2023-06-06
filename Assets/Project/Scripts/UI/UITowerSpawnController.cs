using System;
using System.Linq;
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
        private AssUISetting _assUISetting;
        private bool _aceWasSpawned;


        [Inject]
        private void Construct(TowerSettings towerSettings, UITowerSetting uiTowerSetting, AssUISetting assUISetting)
        {
            _assUISetting = assUISetting;
            _uiTowerSetting = uiTowerSetting;
            _towerSettings = towerSettings;
        }

        private void OnEnable()
        {
            _aceWasSpawned = false;
            
            TowerSpawnController.TowerSpawned += TowerSpawnController_TowerSpawned;
        }

        private void OnDisable()
        {
            TowerSpawnController.TowerSpawned -= TowerSpawnController_TowerSpawned;
        }

        private void Awake()
        {
            for (int i = _buttonGroup.childCount - 1; i >= 0; i--)
            {
                Destroy(_buttonGroup.GetChild(i).gameObject);
            }
        }

        private void Start()
        {
            var aceTowerType = (TowerType)LocalConfig.AceTowerType;

            var aceUIPreset = _assUISetting.GetUIPreset(aceTowerType);

            var aceSpawnButton = Instantiate(_towerSpawnButtonPrefab, _buttonGroup);

            var towerPresets = _towerSettings.TowerPresets.Where(x => !x.TowerPrefab.IsAssTower).ToArray()
                                             .OrderBy(x => x.Cost).ToArray();

            _towerSpawnButtons = new TowerSpawnButton[towerPresets.Length + 1];

            for (int i = 0; i < towerPresets.Length; i++)
            {
                var towerSpawnButton = Instantiate(_towerSpawnButtonPrefab, _buttonGroup);
                var towerType = towerPresets[i].TowerPrefab.TowerType;

                _towerSpawnButtons[i] = towerSpawnButton;

                var towerViewModelType = _towerSettings.GetTowerPresetByType(towerType).BaseViewModelType;

                towerSpawnButton.Setup(_uiTowerSetting.GetUITowerPreset(towerViewModelType).UIIcon, i,
                    _towerSettings.GetTowerPresetByType(towerType).Cost,
                    (index) => { OnClick(index, towerType); });
            }


            aceSpawnButton.Setup(aceUIPreset.Icon, towerPresets.Length, _towerSettings.GetTowerPresetByType(aceTowerType).Cost, (index) => OnClick(index, aceTowerType));

            _towerSpawnButtons[towerPresets.Length] = aceSpawnButton;
        }
        

        private void OnClick(int index, TowerType towerType)
        {
            if (!CanSelected(towerType) || !CanBoth(index, towerType))
            {
                return;
            }

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
        }

        private bool CanBoth(int index, TowerType towerType)
        {
            return _towerSettings.GetTowerPresetByType(towerType).Cost <= GameRuleController.Instance.CashCount;
            
        }

        private bool CanSelected(TowerType towerType)
        {
            if (towerType == (TowerType)LocalConfig.AceTowerType && _aceWasSpawned)
            {
                return false;
            }

            return true;
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

        private void TowerSpawnController_TowerSpawned(TowerType towerType)
        {
            if (towerType == (TowerType)LocalConfig.AceTowerType)
            {
                _aceWasSpawned = true;
            }

            _towerSpawnButtons[_selectedButtonIndex].Refresh(false);
        }
    }
}