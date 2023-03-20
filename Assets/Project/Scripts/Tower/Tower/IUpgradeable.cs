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
    }
}