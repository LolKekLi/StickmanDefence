namespace Project.Meta
{
    public interface IUser
    {
        bool CanUpgrade(CurrencyType type, int amount);
        void SetCurrency(CurrencyType type, int amount);
        void Subscribe();
    }
}