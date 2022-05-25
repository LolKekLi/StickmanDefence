using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using Cysharp.Threading.Tasks;
using Project.UI;

namespace Project
{
    public class StartupManager : MonoBehaviour
    {
        private const string MainUI = "UICommon";

        private LevelFlowController _levelFlowController = null;

        [Inject]
        public void Construct(LevelFlowController levelFlowController)
        {
            _levelFlowController = levelFlowController;
        }
        
        private async void Start()
        {
            DontDestroyOnLoad(gameObject);

            SetupProjectSettings();

            await LoadScene();

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
            await SceneManager.LoadSceneAsync(MainUI);
            
            await _levelFlowController.Load();
        }
    }
}