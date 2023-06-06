using System;
using ModestTree;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Project.UI
{
    public class AceSelcetorWindow : Window
    {
        [SerializeField]
        private Button _backButton;

        [SerializeField]
        private Image _assIcon;

        [SerializeField]
        private Button _nextAssControllButton;

        [SerializeField]
        private Button _prevAssControllButton;

        [SerializeField]
        private TextMeshProUGUI _assLabel;

        [SerializeField]
        private TextMeshProUGUI _assDescription;

        [SerializeField]
        private Button _selectedButton;

        [Inject]
        private AssUISetting _assTowerUISetting;

        private TowerType[] _assTypes;
        private int _currentIndex;

        public override bool IsPopup
        {
            get =>
                false;
        }

        public override void Preload()
        {
            base.Preload();

            var assUIPresets = _assTowerUISetting.AssUIPresets;
            _assTypes = new TowerType[assUIPresets.Length];


            for (int i = 0; i < _assTypes.Length; i++)
            {
                _assTypes[i] = assUIPresets[i].TowerType;
            }
        }

        protected override void Start()
        {
            base.Start();

            _nextAssControllButton.onClick.AddListener(OnNextButtonClick);
            
            _prevAssControllButton.onClick.AddListener(OnPrevButtonClick);
            
            _selectedButton.onClick.AddListener(OnSelectedButtonClick);
            
            _backButton.onClick.AddListener(OnBackButtonClick);
        }

        private static void OnBackButtonClick()
        {
            UISystem.ShowWindow<MainWindow>();
        }

        private void OnSelectedButtonClick()
        {
            LocalConfig.AceTowerType = (int)_assTypes[_currentIndex];
        }

        private void OnPrevButtonClick()
        {
            _currentIndex--;

            if (_currentIndex < 0)
            {
                _currentIndex = _assTypes.Length - 1;
            }

            Setup();
        }

        private void OnNextButtonClick()
        {
            _currentIndex++;

            if (_currentIndex > _assTypes.Length - 1)
            {
                _currentIndex = 0;
            }

            Setup();
        }

        protected override void OnShow()
        {
            base.OnShow();

            var assTowerType = LocalConfig.AceTowerType;

            for (var i = 0; i < _assTypes.Length; i++)
            {
                if (_assTypes[i] == (TowerType)assTowerType)
                {
                    _currentIndex = i;
            
                    break;
                }
            }

            Setup();
        }

        private void Setup()
        {
            var assUIPreset = _assTowerUISetting.GetUIPreset(_assTypes[_currentIndex]);

             _assIcon.sprite = assUIPreset.Icon;
             _assLabel.text = assUIPreset.Lable;
             _assDescription.text = assUIPreset.Description;
        }
    }
}