using System;

namespace Project.Meta
{
    public class User : IUser
    {
        public static event Action Changed = delegate { };
        public static event Action CurrencyChanged = delegate { };
        
        public int Coins
        {
            get
            {
                return LocalConfig.Coins;
            }

            private set
            {
                LocalConfig.Coins = value;
                CurrencyChanged();
            }
        }

        public User()
        {
            LoadData();
        }

        private void LoadData()
        {
            Changed();
        }
        
        bool IUser.CanUpgrade(CurrencyType type, int amount)
        {
            bool canPurchase = false;

            switch (type)
            {
                case CurrencyType.Coin:
                    canPurchase = amount <= Coins;
                    break;

                default:
                    canPurchase = true;
                    break;
            }

            return canPurchase;
        }

        void IUser.Subscribe()
        {
            
        }

        void IUser.SetCurrency(CurrencyType type, int amount)
        {
            if (type == CurrencyType.Coin)
            {
                Coins += amount;
            }
        }
    }
}