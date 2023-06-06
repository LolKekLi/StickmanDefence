namespace Project
{
    public class ShotGunner : BaseTower
    {
        public override TowerType TowerType
        {
            get => TowerType.ShotGunner;
        }

        public override bool IsAssTower
        {
            get => false;
        }
    }
}