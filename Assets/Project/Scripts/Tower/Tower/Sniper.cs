namespace Project
{
    public class Sniper : BaseTower
    {
        public override TowerType TowerType
        {
            get => TowerType.Sniper;
        }

        public override bool IsAssTower
        {
            get => false;
        }
    }
}