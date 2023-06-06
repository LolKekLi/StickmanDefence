namespace Project
{
    public class Granatman : BaseTower
    {
        public override TowerType TowerType
        {
            get => TowerType.Granatman;
        }

        public override bool IsAssTower
        {
            get => false;
        }
    }
}