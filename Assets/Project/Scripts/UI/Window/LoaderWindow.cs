using Cysharp.Threading.Tasks;
using Project.Settings;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Project.UI
{
    public class LoaderWindow : Window
    {
        [SerializeField, Header("Loading")]
        private SlicedFilledImage _loadingProgress = null;
        
        [Inject]
        private LevelFlowController _levelFlowController;

        [Inject]
        private LoadingSettings _loadingSettings;
        
        public override bool IsPopup
        {
            get => false;
        }

        protected override void OnShow()
        {
            base.OnShow();

            var sceneName = GetDataValue<string>(LevelSelectorWindow.SceneNameKey);

            LoadScene(sceneName);
        }
        
        private async UniTask LoadScene(string sceneName)
        {
            float time = 0f;
            
            var levelWaiter = _levelFlowController.Load(sceneName);

            while (levelWaiter.Status == UniTaskStatus.Pending || time < _loadingSettings.LoadingTime)
            {
                _loadingProgress.fillAmount = time / _loadingSettings.LoadingTime;

                await UniTask.Yield();

                time += Time.deltaTime;
            }

            _loadingProgress.fillAmount = 1f;
        }
    }
}