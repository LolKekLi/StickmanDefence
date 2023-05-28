using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using Cysharp.Threading.Tasks;
using Project.Settings;
using Project.UI;

namespace Project
{
    public class StartupManager : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup[] _canvasGroups = null;

        
        [SerializeField, Header("Loading")]
        private SlicedFilledImage _loadingProgress = null;
        
        private const string MainUI = "UICommon";

        private LevelFlowController _levelFlowController = null;
        private LoadingSettings _loadingSettings;

        [Inject]
        public void Construct(LevelFlowController levelFlowController, LoadingSettings loadingSettings)
        {
            _loadingSettings = loadingSettings;
            _levelFlowController = levelFlowController;
        }
        
        private async void Start()
        {
            DontDestroyOnLoad(gameObject);

            SetupProjectSettings();
            
            await LoadScene();
            await FadeAsync();

            Destroy(gameObject);
        }


        private void SetupProjectSettings()
        {
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
            Screen.orientation = ScreenOrientation.Portrait;
        }

        private async UniTask LoadScene()
        {
            var uiWaiter = SceneManager.LoadSceneAsync(MainUI);
            float time = 0f;

            void updateLoadingProgress()
            {
                _loadingProgress.fillAmount = Mathf.Clamp(time / _loadingSettings.LoadingTime, 0, 1);
            }

            while (uiWaiter.progress < 1)
            {
                updateLoadingProgress();

                await UniTask.Yield();

                time += Time.deltaTime;
            }

            var levelWaiter = _levelFlowController.LoadHub();

            while (levelWaiter.Status == UniTaskStatus.Pending || time < _loadingSettings.LoadingTime)
            {
                _loadingProgress.fillAmount = time / _loadingSettings.LoadingTime;

                await UniTask.Yield();

                time += Time.deltaTime;
            }

            _loadingProgress.fillAmount = 1f;
        }

        private async UniTask FadeAsync()
        {
            foreach (var canvasGroup in _canvasGroups)
            {
                await UniTaskExtensions.Lerp(progress =>
                    {
                        canvasGroup.alpha = 1 - progress;
                    },
                    _loadingSettings.FadeTime, _loadingSettings.FadeCurve, CancellationToken.None);
            }
        }
    }
}