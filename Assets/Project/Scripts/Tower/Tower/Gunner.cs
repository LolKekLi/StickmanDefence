namespace Project
{
    public class Gunner : BaseTower
    {
        public override TowerType TowerType
        {
            get => TowerType.Gunner;
        }

        public override bool IsAssTower
        {
            get => false;
        }
    }
}