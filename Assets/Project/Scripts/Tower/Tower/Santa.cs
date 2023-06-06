namespace Project
{
    public class Santa : AceTower
    {
        public override TowerType TowerType
        {
            get =>
                TowerType.Santa;
        }

        public override UltimateType UltimateType
        {
            get => UltimateType.SantaBigExplosion;
        }
    }
}