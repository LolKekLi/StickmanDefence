using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Project.UI;
using UnityEngine.SceneManagement;

namespace Project
{
    public class LevelFlowController
    {
        public event Action Loaded = null;
        public event Action Started = null;
        public event Action<bool> Finished = null;

        public LevelSettings LevelSettings
        {
            get;
        }
        
        public LevelFlowController(LevelSettings levelSettings)
        {
            LevelSettings = levelSettings;
        }

        public void Start(Action callback = null)
        {
            Started?.Invoke();
            
            callback?.Invoke();
            
            UISystem.ShowWindow<GameWindow>();
        }

        public async void Complete(Dictionary<string, object> data = null, Action callback = null)
        {
            LocalConfig.LevelIndex++;
            
            Finished?.Invoke(true);
            
            callback?.Invoke();
            
            await UniTask.Delay(TimeSpan.FromSeconds(LevelSettings.ResultDelay));

            if (data == null)
            {
                data = new Dictionary<string, object>();
            }
            
            data.Add(ResultWindow.ReceivedCoinsKey, LevelSettings.CompleteCoinCount);
            
            UISystem.ShowWindow<ResultWindow>(data);
        }

        public async void Fail(Action callback = null)
        {
            Finished?.Invoke(false);

            callback?.Invoke();

            await UniTask.Delay(TimeSpan.FromSeconds(LevelSettings.ResultDelay));
            
            UISystem.ShowWindow<FailWindow>();
        }
        
        public async UniTask Load(Action callback = null)
        {
            await SceneManager.LoadSceneAsync(LevelSettings.GetScene);

            Loaded?.Invoke();
            
            callback?.Invoke();
            
            UISystem.ShowWindow<GameWindow>();
        }
    }
}