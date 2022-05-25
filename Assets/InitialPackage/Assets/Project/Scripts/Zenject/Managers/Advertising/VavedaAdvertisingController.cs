using System;

#if UNITY_AMAZON
using JambaEngine.Purchasing;
using Vaveda.Integration.Scripts;
using Vaveda.Integration.Scripts.Fads;
#endif

namespace Project
{
    public class VavedaAdvertisingController : AdvertisingController
    {
        private Action<bool> _callback = null;
        
        public override bool HasRewarded
        {
            get
            {
                bool hasRewarded = true;

#if UNITY_AMAZON
                hasRewarded = Services.Instance.FadsService.HasRewardedVideo;
#endif
                
                return hasRewarded;
            }
        }

        public override bool HasInterstitial
        {
            get
            {
                bool hasInterstitial = true;

#if UNITY_AMAZON
                hasInterstitial = Services.Instance.FadsService.HasInterstitial;
#endif
                
                return hasInterstitial;
            }
        }

        public override bool IsSubscribed
        {
            get
            {
                bool isSubscribed = false;
                
#if UNITY_AMAZON
                isSubscribed = JambaStartAppPurchasing.IsSubscribed;
#endif
                return isSubscribed;
            }
        }

        public override void Init()
        {
            base.Init();
            
#if UNITY_AMAZON
            Services.Instance.FadsService.RewardedShouldReward += FadsService_RewardedVideoComplete;
#endif
        }

        public override void ShowRewarded(string place, Action<bool> callback)
        {
            if (HasRewarded)
            {
                _callback = callback;
                
#if UNITY_AMAZON
                Services.Instance.FadsService.ShowRewardedVideo(Placements.PLACEMENT_REWARDED_LEVEL_GETHINTS);
#endif
            }
        }

        public override void ShowInterstitial(string place)
        {
            if (HasInterstitial && !IsSubscribed)
            {
#if UNITY_AMAZON
                Services.Instance.FadsService.ShowInterstitial(Placements.PLACEMENT_INTERSTITIAL_LEVEL_END);
#endif
            }
        }

        public override void ShowBanner()
        {
            if (!IsSubscribed)
            {
#if UNITY_AMAZON
                Services.Instance.FadsService.ShowBanner(Placements.PLACEMENT_BANNER_LEVEL);
#endif
            }
        }

        public override void HideBanner()
        {
#if UNITY_AMAZON
            Services.Instance.FadsService.HideBanner();
#endif
        }

        private void FadsService_RewardedVideoComplete()
        {
            _callback?.Invoke(true);
        }
    }
}