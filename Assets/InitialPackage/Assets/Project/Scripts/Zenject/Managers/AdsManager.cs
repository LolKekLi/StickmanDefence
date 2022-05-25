using System;

namespace Project
{
    public class AdsManager : ZenjectManager<AdsManager>
    {
        private AdvertisingController _advertisingController = null;

        protected override void Init()
        {
            base.Init();

            InitController();
        }

        public void ShowRewarded(string place, Action<bool> callback)
        {
            _advertisingController?.ShowRewarded(place, callback);
        }

        public void ShowInterstitial(string place)
        {
            _advertisingController?.ShowInterstitial(place);
        }

        public void ShowBanner()
        {
            _advertisingController?.ShowBanner();
        }

        public void HideBanner()
        {
            _advertisingController?.HideBanner();
        }

        private void InitController()
        {
            var targetStore = StoreHelper.GetTargetStore();

            switch (targetStore)
            {
                case TargetStoreType.Amazon:
                    _advertisingController = new VavedaAdvertisingController();
                    break;
                
                case TargetStoreType.SayGames:
                    _advertisingController = new SayGamesAdvertisingController();
                    break;
                
                default:
                    DebugSafe.LogException(new Exception(
                        $"Not found {nameof(AdsManager)} for {nameof(TargetStoreType)}: {targetStore}"));
                    break;
            }
            
            _advertisingController?.Init();
        }
    }
}