using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Project.UI
{
    public class ResultWindow : Window
    {
        [SerializeField]
        private Button _homeButton;

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
        }

        private void OnHomeButtonClick()
        {
            _levelFlowController.LoadHub();
        }
    }
}