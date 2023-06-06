namespace Project
{
    public class Bugler : AceTower
    {
        public override TowerType TowerType
        {
            get => TowerType.Bugler;
        }

        public override UltimateType UltimateType
        {
            get => UltimateType.BuglerThief;
        }
    }
}