using System;

namespace Project
{
    public class InAppManager : ZenjectManager<InAppManager>
    {
        private InAppController _inAppController = null;

        public bool CanShow
        {
            get => _inAppController?.CanShow ?? false;
        }
        
        protected override void Init()
        {
            base.Init();

            InitController();
        }

        public void ShowPopup()
        {
            _inAppController?.ShowPopup();
        }
        
        private void InitController()
        {
            var targetStore = StoreHelper.GetTargetStore();

            switch (targetStore)
            {
                case TargetStoreType.Amazon:
                    _inAppController = new VavedaInAppController();
                    break;
                
                case TargetStoreType.SayGames:
                    _inAppController = new SayGamesInAppController();
                    break;
                
                default:
                    DebugSafe.LogException(new Exception(
                        $"Not found {nameof(AnalyticsController)} for {nameof(TargetStoreType)}: {targetStore}"));
                    break;
            }
            
            _inAppController?.Init();
        }
    }
}