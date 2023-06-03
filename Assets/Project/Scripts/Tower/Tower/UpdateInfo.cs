using System.Collections.Generic;

namespace Project
{
    public class UpdateInfo
    {
        public UpgradeLinePerkType? MaxUpgradeType
        {
            get;
            private set;
        }

        public UpgradeLinePerkType? LockLineType
        {
            get;
            private set;
        }

        public Dictionary<UpgradeLinePerkType, int> LevelInfo
        {
            get;
            private set;
        }

        public UpdateInfo()
        {
            LevelInfo = new Dictionary<UpgradeLinePerkType, int>()
            {
                { UpgradeLinePerkType.FirstLine, 0 },
                { UpgradeLinePerkType.SecondLine, 0 },
                { UpgradeLinePerkType.ThirdLine, 0 },
            };
        }

        public void Update(UpgradeLinePerkType upgradeLinePerkType)
        {
            LevelInfo[upgradeLinePerkType]++;
        }

        public void SetLockLineType(UpgradeLinePerkType result)
        {
            LockLineType = result;
        }

        public void SetMaxUpgradeType(UpgradeLinePerkType result)
        {
            MaxUpgradeType = result;
        }
    }
}