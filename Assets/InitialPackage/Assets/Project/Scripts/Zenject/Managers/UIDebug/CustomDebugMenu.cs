using System.ComponentModel;
using UnityEngine;
using Zenject;

namespace Project.UIDebug
{
    public class CustomDebugMenu
    {
        private LevelFlowController _levelFlowController = null;

        [Inject]
        private void Construct(LevelFlowController levelFlowController)
        {
            _levelFlowController = levelFlowController;
        }
        
        [Category("Level")]
        public void CompleteLevel()
        {
            _levelFlowController.Complete();
        }

        [Category("Level")]
        public async void ReloadLevel()
        {
            await _levelFlowController.Load();
        }

        [Category("Level")]
        public async void PrevLevel()
        {
            LocalConfig.LevelIndex--;

            await _levelFlowController.Load();
        }
    }
}