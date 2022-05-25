using System;

namespace Project
{
    public abstract class InAppController
    {
        public event Action<bool> Subscribed = null; 
        
        public abstract bool CanShow
        {
            get;
        }

        protected void OnSubscribe(bool isSuccess)
        {
            Subscribed?.Invoke(isSuccess);
        }

        public abstract void Init();

        public abstract void DeInit();

        public abstract void ShowPopup();
    }
}