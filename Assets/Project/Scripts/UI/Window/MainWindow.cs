using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Project.UI
{
    public class MainWindow : Window
    {
        [SerializeField]
        private Button _playButton;
        [SerializeField]
        private Button _settingButton;

        [SerializeField]
        private Button _assButton;

        [Inject]
        private LevelFlowController _levelFlowController;
        
        public override bool IsPopup
        {
            get => false;
        }

        protected override void Start()
        {
            base.Start();
            
            _playButton.onClick.AddListener(OnPlayButtonClick);
            _settingButton.onClick.AddListener(OnSettingButtonClick);
            _assButton.onClick.AddListener(OnAssButtonClick);
        }

        private void OnAssButtonClick()
        {
           UISystem.ShowWindow<AssSelcetorWindow>();
        }
        
        private void OnSettingButtonClick()
        {
            UISystem.ShowWindow<SettingPopup>();    
        }

        private void OnPlayButtonClick()
        {
            UISystem.ShowWindow<LevelSelectorWindow>();    
        }
    }
}