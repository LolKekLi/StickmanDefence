using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Project.UI
{
    public class LevelSelectorWindow : Window
    {
        public static string SceneNameKey = "SceneNameKey";

        [SerializeField]
        private Button _backButton;

        [SerializeField]
        private GameObject[] _stages;

        [SerializeField, Header("Stage 1")]
        private Transform _uiLevelEllementParent;

        [SerializeField]
        private UILevelElement _uiLevelElementPrefab;

        [SerializeField, Header("Stage 2")]
        private UILevelElement _nightLevel;

        [SerializeField]
        private UILevelElement _level;

        [SerializeField, Header("Stage 3")]
        private UIDifficultElement[] _uiDifficultElements;

        private int _selectedLevelIndex;
       

        private AsyncReactiveProperty<int> _activeStageIndex = new AsyncReactiveProperty<int>(0);

        private UILevelElement[] _uiLevelElements;
        
        [Inject]
        private LevelFlowController _levelFlowController;

        private LevelSettings _levelSettings;
        private Dictionary<string,object> _data;

        public override bool IsPopup
        {
            get =>
                false;
        }

        protected override void Start()
        {
            base.Start();

            _backButton.onClick.AddListener(OnBackButtonClick);
        }

        public override void Preload()
        {
            base.Preload();

            _levelSettings = _levelFlowController.LevelSettings;

            SetupFirstStage();

            SetupSecondStage();

            SetupThirdStage();

            _activeStageIndex.Subscribe(OnActiveStageChanged);
        }

        private void SetupThirdStage()
        {
            var prise = _levelSettings.Levels[_selectedLevelIndex].Prise;

            _data = new Dictionary<string, object>()
            {
                { SceneNameKey, null }
            };

            for (var i = 0; i < _uiDifficultElements.Length; i++)
            {
                _uiDifficultElements[i].Setup((int)(prise *
                    _levelSettings.MultiplierPresets.FirstOrDefault(x => x.Type == _uiDifficultElements[i].Type)
                                  .Multiplier), () =>
                {
                    UISystem.ShowWindow<LoaderWindow>(_data);
                });
            }
        }

        private void SetupSecondStage()
        {
            var levelPreset = _levelSettings.Levels[_selectedLevelIndex];

            var sceneName = levelPreset.NightSceneName;
            _nightLevel.Setup(levelPreset.NightIcon, levelPreset.LevelName, sceneName, () =>
            {
                _data[SceneNameKey] = sceneName;
                _activeStageIndex.Value++;
            });

            sceneName = levelPreset.SceneName;
            _level.Setup(levelPreset.Icon, levelPreset.LevelName, sceneName, () =>
            {
                _data[SceneNameKey] = sceneName;
                _activeStageIndex.Value++;
            });
        }

        private void SetupFirstStage()
        {
            _uiLevelElements = new UILevelElement[_levelSettings.Levels.Length];

            for (var i = 0; i < _levelSettings.Levels.Length; i++)
            {
                _uiLevelElements[i] = Instantiate(_uiLevelElementPrefab, _uiLevelEllementParent);

                var levelIndex = i;
                var levelPreset = _levelSettings.Levels[i];

                _uiLevelElements[i].Setup(levelPreset.Icon, levelPreset.LevelName, levelPreset.SceneName, () =>
                {
                    _activeStageIndex.Value++;
                    _selectedLevelIndex = levelIndex;
                });
            }
        }

        protected override void OnShow()
        {
            base.OnShow();

            _activeStageIndex.Value = 0;
        }

        private void ShowActiveStage()
        {
            _stages.Do(x => x.SetActive(false));
            _stages[_activeStageIndex.Value].SetActive(true);
        }

        private void OnActiveStageChanged(int activeStageIndex)
        {
            if (activeStageIndex < 0)
            {
                UISystem.ShowWindow<MainWindow>();

                return;
            }

            ShowActiveStage();
        }

        private void OnBackButtonClick()
        {
            _activeStageIndex.Value--;
        }
    }
}