using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Project.UI
{
    public class FailWindow : Window
    {
        [SerializeField]
        private Button _homeButton;

        [SerializeField]
        private Button _restartButton;
        
        [Inject]
        private LevelFlowController _levelFlowController;
        
        public override bool IsPopup
        {
            get => false;
        }

        protected override void Start()
        {
            base.Start();
            
            _homeButton.onClick.AddListener(OnHomeButtonClick);
            _restartButton.onClick.AddListener(OnRestartButtonClick);
        }

        private void OnRestartButtonClick()
        {
            _levelFlowController.Load(LevelSelectorWindow.SceneNameKey);
        }

        private void OnHomeButtonClick()
        {
            _levelFlowController.LoadHub();
        }
    }
}