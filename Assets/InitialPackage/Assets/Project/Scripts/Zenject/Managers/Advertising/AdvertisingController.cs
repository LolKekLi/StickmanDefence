using System;

namespace Project
{
    public abstract class AdvertisingController
    {
        public abstract bool HasRewarded
        {
            get;
        }

        public abstract bool HasInterstitial
        {
            get;
        }

        public abstract bool IsSubscribed
        {
            get;
        }

        public virtual void Init()
        {
            
        }
        
        public abstract void ShowRewarded(string place, Action<bool> callback);
        public abstract void ShowInterstitial(string place);
        public abstract void ShowBanner();
        public abstract void HideBanner();
    }
}