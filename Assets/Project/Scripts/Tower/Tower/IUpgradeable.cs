using System;

namespace Project
{
    public interface IUpgradeable
    {
        TowerType TowerType
        {
            get;
        }

        TowerViewModelType TowerViewModelType
        {
            get;
        }

        void UnSelected();
        void Selected();
        void Sell();
        string GetTransformName();
        int GetUpgradeLevel(UpgradeLinePerkType perkLineType);

        void Upgrade(UpgradeLinePerkType upgradeLinePerkType, TowerUpgradeSettings.UpdatePreset[] presetByLineType, 
            Action<TowerViewModelType, FirePreset> changeViewModelCallback);

        UpdateInfo GetUpgradeInfo();
    }
}