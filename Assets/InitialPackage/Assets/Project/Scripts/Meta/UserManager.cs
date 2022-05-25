namespace Project.Meta
{
    public class UserManager
    {
        private User _user = null;
        private IUser _iUser = null;

        public int Coins
        {
            get => _user.Coins;
        }
        
        public UserManager()
        {
            _user = new User();
            _iUser = _user;
        }

        public bool CanUpgrade(CurrencyType type, int amount)
        {
            return _iUser.CanUpgrade(type, amount);
        }

        public void SetCurrency(CurrencyType type, int amount)
        {
            _iUser.SetCurrency(type, amount);
        }

        public void Subscribe()
        {
            _iUser.Subscribe();
        }
    }
}