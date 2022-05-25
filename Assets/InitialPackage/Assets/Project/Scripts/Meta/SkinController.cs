using System;
using System.Linq;
using Zenject;

namespace Project.Meta
{
    public class SkinController
    {
        public event Action Selected = delegate { };

        private AdsManager _adsManager = null;
        private InAppManager _inAppManager = null;

        public SkinSettings SkinSettings
        {
            get;
            private set;
        }

        [Inject]
        private void Construct(SkinSettings skinSettings, AdsManager adsManager, InAppManager inAppManager)
        {
            SkinSettings = skinSettings;
            _adsManager = adsManager;
            _inAppManager = inAppManager;
        }

        public void OnClick(SkinPreset skinPreset, Action callback)
        {
            if (IsUnlocked(skinPreset))
            {
                Select(skinPreset);
            }
            else if (skinPreset.UnlockType == UnlockType.Ads)
            {
                _adsManager.ShowRewarded(AdsPlacements.ShopItem, isSuccess =>
                {
                    if (isSuccess)
                    {
                        LocalConfig.SetSkinClaimProgress(skinPreset.SkinType, skinPreset.SkinPartType);

                        var claimProgress =
                            LocalConfig.GetSkinClaimProgress(skinPreset.SkinType, skinPreset.SkinPartType);
                        
                        if (claimProgress >= skinPreset.SkinUnlockCount)
                        {
                            Unlock(skinPreset);
                        }
                    }
                });
            }
            if (_inAppManager.CanShow)
            {
                _inAppManager.ShowPopup();
            }

            callback?.Invoke();
        }

        public void UnlockRandom(SkinPartType skinPartType)
        {
            var skinPresets = SkinSettings.GetSkinPresets(skinPartType)
                .Where(IsAvailableToUnlock).ToList();

            var skinPreset = skinPresets.RandomElement();
            
            Unlock(skinPreset);
        }

        public bool IsUnlocked(SkinPreset skinPreset)
        {
            return LocalConfig.IsSkinUnlocked(skinPreset.SkinType, skinPreset.SkinPartType);
        }

        private void Select(SkinPreset skinPreset)
        {
            LocalConfig.SelectSkin(skinPreset.SkinType, skinPreset.SkinPartType);

            Selected?.Invoke();
        }

        private void Unlock(SkinPreset skinPreset)
        {
            LocalConfig.UnlockSkin(skinPreset.SkinType, skinPreset.SkinPartType, true);

            Select(skinPreset);
        }

        private void Remove(SkinPreset skinPreset)
        {
            LocalConfig.UnlockSkin(skinPreset.SkinType, skinPreset.SkinPartType, false);
        }

        private bool IsAvailableToUnlock(SkinPreset skinPreset)
        {
            return !IsUnlocked(skinPreset) && (skinPreset.RarityType != RarityType.Vip || !InAppHelper.IsInAppEnabled) &&
                   skinPreset.UnlockType != UnlockType.Ads;
        }
    }
}