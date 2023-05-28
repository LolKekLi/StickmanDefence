using System;
using System.Collections.Generic;
using SRDebugger.Services;
using SRF.Service;
using UnityEngine;
using Zenject;

namespace Project.UIDebug
{
    public class DebugMenu : ZenjectManager<DebugMenu>
    {
        private CustomDebugMenu _customDebugMenu = null;
        private HotKeyHelper _helper = null;
        
        private DiContainer _diContainer = null;
        private AdsManager _adsManager = null;
        private LevelFlowController _levelFlowController = null;

        [Inject]
        public void Construct(DiContainer diContainer, AdsManager adsManager, LevelFlowController levelFlowController)
        {
            _diContainer = diContainer;
            _adsManager = adsManager;
            _levelFlowController = levelFlowController;
        }
        
        protected override void Init()
        {
            base.Init();
            
            SRDebug.Init();
            var options = SRServiceManager.GetService<IOptionsService>();
            
            _customDebugMenu = new CustomDebugMenu();
            _diContainer.Inject(_customDebugMenu);
            options.AddContainer(_customDebugMenu);
            
            SRDebug.Instance.PanelVisibilityChanged += PanelVisibilityChanged;

            InitHotKeyHelper();
        }

        private void Update()
        {
            _helper.Tick();
        }

        private void InitHotKeyHelper()
        {
            _helper = new HotKeyHelper(new Dictionary<KeyCode, Action>
            {
                // { KeyCode.Z, () =>
                // {
                //     _customDebugMenu.CompleteLevel();
                // }},
                //
                // { KeyCode.X, () =>
                // {
                //     _customDebugMenu.ReloadLevel();
                // }},
                //
                // { KeyCode.C, () =>
                // {
                //     _customDebugMenu.PrevLevel();
                // }}
            });
        }

        private void PanelVisibilityChanged(bool isVisible)
        {
            Time.timeScale = isVisible ? 0 : 1;
            
            if (isVisible)
            {
                _adsManager.HideBanner();
            }
            else
            {
                _adsManager.ShowBanner();
            }
        }
    }
}