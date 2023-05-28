using System;
using System.Collections.Generic;
using Project.Meta;
using UnityEngine;
using Zenject;

namespace Project.UI
{
    public abstract class Window : MonoBehaviour
    {
        public static event Action<Window> Shown = delegate { };
        public static event Action<Window> Hidden = delegate { };

        private Action _onHideAction = null;

        
        public abstract bool IsPopup
        {
            get;
        }

        protected virtual void OnEnable()
        {
            User.CurrencyChanged += User_CurrencyChanged;
        }

        protected virtual void OnDisable()
        {
            User.CurrencyChanged -= User_CurrencyChanged;
        }

        protected virtual void Start()
        {

        }

        public void Show(Action onHideAction)
        {
            _onHideAction = onHideAction;
            
            OnShow();

            Shown(this);
            
            AfterShown();
            
            Refresh();
        }

        public void Hide()
        {
            _onHideAction?.Invoke();
            
            OnHide();

            Hidden(this);
        }

        public virtual void Preload()
        {
            gameObject.SetActive(false);
        }

        protected virtual void OnShow()
        {
            gameObject.SetActive(true);
        }

        protected virtual void AfterShown()
        {
            
        }

        protected virtual void Refresh()
        {

        }

        protected virtual void OnHide()
        {
            gameObject.SetActive(false);
        }

        protected T GetDataValue<T>(string itemKey, T defaultValue = default, Dictionary<string, object> forcedData = null)
        {
            Dictionary<string, object> data = UISystem.Data;

            if (data == null || data.Count == 0)
            {
                return defaultValue;
            }

            if (!data.TryGetValue(itemKey, out object itemObject))
            {
                return defaultValue;
            }

            if (itemObject is T)
            {
                return (T)itemObject;
            }

            return defaultValue;
        }

        protected void SetDataValue<T>(string itemKey, T value)
        {
            if (!UISystem.Data.ContainsKey(itemKey))
            {
                UISystem.Data.Add(itemKey, value);
            }
            else
            {
                UISystem.Data[itemKey] = value;
            }
        }

        private void User_CurrencyChanged()
        {
            Refresh();
        }
    }
}