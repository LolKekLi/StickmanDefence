using System;
using Zenject;

namespace Project
{
    public class AnalyticsManager : ZenjectManager<AnalyticsManager>
    {
        private AnalyticsController _analyticsController = null;

        private LevelSettings _levelSettings = null;
        
        [Inject]
        public void Construct(LevelFlowController levelFlowController)
        {
            _levelSettings = levelFlowController.LevelSettings;
        }
        
        protected override void Init()
        {
            base.Init();

            InitController();
        }

        protected override void DeInit()
        {
            base.DeInit();
        }

        private void InitController()
        {
            var targetStore = StoreHelper.GetTargetStore();

            switch (targetStore)
            {
                case TargetStoreType.Amazon:
                    _analyticsController = new VavedaAnalyticsController();
                    break;
                
                case TargetStoreType.SayGames:
                    _analyticsController = new SayGamesAnalyticsController();
                    break;
                
                default:
                    DebugSafe.LogException(new Exception(
                        $"Not found {nameof(AnalyticsController)} for {nameof(TargetStoreType)}: {targetStore}"));
                    break;
            }
            
            _analyticsController?.Init(_levelSettings);
        }
    }
}