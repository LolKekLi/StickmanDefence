using System;

namespace Project
{
    public class SayGamesAdvertisingController : AdvertisingController
    {
        public override bool HasRewarded
        {
            get
            {
                bool hasRewarded = true;

#if SAYKIT
                hasRewarded = SayKit.isInitialized;
#endif
                
                return hasRewarded;
            }
        }

        public override bool HasInterstitial
        {
            get => true;
        }

        public override bool IsSubscribed
        {
            get => false;
        }

        public override void ShowRewarded(string place, Action<bool> callback)
        {
            if (HasRewarded)
            {
#if SAYKIT
                SayKit.showRewarded(place, callback);
                
                DebugSafe.Log($"ShowRewarded: {place}");
#endif
            }
        }

        public override void ShowInterstitial(string place)
        {
#if SAYKIT
            SayKit.showInterstitial(place);
            
            DebugSafe.Log($"ShowInterstitial: {place}");
#endif
        }

        public override void ShowBanner()
        {
#if SAYKIT
            SayKit.showBanner();
#endif
        }

        public override void HideBanner()
        {
#if SAYKIT
            SayKit.hideBanner();
#endif
        }
    }
}