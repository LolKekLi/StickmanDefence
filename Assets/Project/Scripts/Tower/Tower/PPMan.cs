namespace Project
{
    public class PPMan : BaseTower
    {
        public override TowerType TowerType
        {
            get => TowerType.PPMan;
        }

        public override bool IsAssTower
        {
            get => false;
        }
    }
}