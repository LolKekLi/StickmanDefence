using System;
using System.Linq;
using UnityEngine;

namespace Project
{
    [Serializable]
    public class FinishCoinPreset
    {
        [field: SerializeField]
        public IntRange LevelIndex
        {
            get;
            private set;
        }

        [field: SerializeField]
        public int Coin
        {
            get;
            private set;
        }
    }
    
    [CreateAssetMenu(fileName = "LevelSettings", menuName = "Scriptable/LevelSettings", order = 0)]
    public class LevelSettings : ScriptableObject
    {
#if UNITY_EDITOR
        [SerializeField, Header("Test Group")]
        private bool _isTestSceneEnabled = false;

        [SerializeField, EnabledIf(nameof(_isTestSceneEnabled), true, EnabledIfAttribute.HideMode.Invisible)]
        private string _testSceneName = string.Empty;
#endif
        
        [SerializeField, Header("Main Group")]
        private string _tutorialSceneName = string.Empty;

        [SerializeField]
        private string[] _levels = null;
        
        [SerializeField]
        private string[] _loopedLevels = null;

        [Header("Finish")]
        [SerializeField]
        private FinishCoinPreset[] _coinPresets = null;
        
        [field: SerializeField]
        public float ResultDelay
        {
            get;
            private set;
        }
        
        public string GetScene
        {
            get
            {
#if UNITY_EDITOR
                if (_isTestSceneEnabled)
                {
                    return _testSceneName;
                }
#endif
                
                int levelIndex = LocalConfig.LevelIndex;

                if (levelIndex == 0)
                {
                    return _tutorialSceneName;
                }
                else
                {
                    // NOTE: учитываем туториал
                    levelIndex -= 1;
                }

                if (levelIndex < _levels.Length)
                {
                    return _levels[levelIndex % _levels.Length];
                }
                else
                {
                    levelIndex -= _levels.Length;

                    return _loopedLevels[levelIndex % _loopedLevels.Length];
                }
            }
        }
        
        public int CompleteCoinCount
        {
            get
            {
                var coinPreset = _coinPresets.FirstOrDefault(pr => pr.LevelIndex.InRange(LocalConfig.LevelIndex - 1));
                if (coinPreset == null)
                {
                    coinPreset = _coinPresets[_coinPresets.Length - 1];
                }

                return coinPreset.Coin;
            }
        }
    }
}