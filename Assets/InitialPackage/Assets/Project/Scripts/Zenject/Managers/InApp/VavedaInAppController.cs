namespace Project
{
#if UNITY_AMAZON
    public class CustomJambaListener : JambaSubscriptionListener
    {
        private readonly InAppController _inAppController = null;

        public CustomJambaListener(InAppController inAppController)
        {
            _inAppController = inAppController;
        }
        
        public override void OnInitialized()
        {
            base.OnInitialized();
            
            if (_inAppController.CanShow)
            {
                
            }
        }
    }
#endif
    
    public class VavedaInAppController : InAppController
    {
        public override bool CanShow
        {
            get
            {
                bool canShow = true;
                
#if UNITY_AMAZON
                canShow = InAppHelper.IsInAppEnabled && JambaStartAppPurchasing.IsCanShow;
#endif
                
                return canShow;
            }
        }

        public override void Init()
        {
#if UNITY_AMAZON
            JambaStartAppPurchasing.OnSubscriptionUpdated += JambaStartAppPurchasing_OnSubscriptionUpdated;
            
            JambaSubscriptionListener listener = new CustomJambaListener(_skinSettings);
            
            JambaStartAppPurchasing.Initialize(listener);
#endif
        }

        public override void DeInit()
        {
#if UNITY_AMAZON
            JambaStartAppPurchasing.OnSubscriptionUpdated -= JambaStartAppPurchasing_OnSubscriptionUpdated;
#endif
        }

        public override void ShowPopup()
        {
#if UNITY_AMAZON
            JambaStartAppPurchasing.ShowSubscriptionDialog();
#endif
        }

        private void JambaStartAppPurchasing_OnSubscriptionUpdated(bool isSuccess)
        {
            OnSubscribe(isSuccess);
        }
    }
}