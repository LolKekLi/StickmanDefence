namespace Project.UI
{
    public class ResultWindow : Window
    {
        public static readonly string ReceivedCoinsKey = "ReceivedCoinsKey";

        public override bool IsPopup
        {
            get => false;
        }

        protected override void OnShow()
        {
            base.OnShow();

            int receivedCoins = GetDataValue<int>(ReceivedCoinsKey, 0);   
        }
    }
}