namespace Project
{
    public interface IUpgradeable
    {
        TowerType Type
        {
            get;
        }

        void UnSelected();
        void Selected();
        void Cell();
        string GetTransformName();
        int GetUpgradeLevel(UpgradeLinePerkType perkLineType);
        void Upgrade(UpgradeLinePerkType upgradeLinePerkType, TowerUpgradeSettings.UpdatePreset[] presetByLineType);
        UpdateInfo GetUpgradeInfo();
    }
}