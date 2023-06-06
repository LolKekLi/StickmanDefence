using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Project
{
    public class UIUltimateItem : MonoBehaviour
    {
        [SerializeField]
        private Image _iconImage;

        [SerializeField]
        private Button _button;

        [SerializeField]
        private Image _reloadImage;

        private float _reloadTime;
        
        private Action<UltimateType> _onClickAction;
        private UltimateType _type;

        private CancellationTokenSource _reloadToken;

        private bool _canClick = true;
        
        private void Start()
        {
            _button.onClick.AddListener(OnButtonClick);
        }

        private void OnDisable()
        {
            UniTaskUtil.CancelToken(ref _reloadToken);
        }

        public void ToggleActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
        
        public void Setup(Sprite icon, float reloadTime, UltimateType type, Action<UltimateType> onClickAction)
        {
            _reloadTime = reloadTime;
            _type = type;
            _onClickAction = onClickAction;

            _iconImage.sprite = icon;

            _reloadImage.fillAmount = 0;
        }

        private void OnButtonClick()
        {
            if (!_canClick)
            {
                return;
            }
            
            _onClickAction.Invoke(_type);

            _canClick = false;

            ReloadAsync(_reloadImage, UniTaskUtil.RefreshToken(ref _reloadToken)).Forget();
        }

        private async UniTaskVoid ReloadAsync(Image reloadImage, CancellationToken refreshToken)
        {
            try
            {
                reloadImage.fillAmount = 1;
                
                await UniTaskExtensions.Lerp(time =>
                {
                    reloadImage.fillAmount = Mathf.Lerp(1, 0, time);
                }, _reloadTime, token: refreshToken);
                
                _canClick = true;
            }
            catch (OperationCanceledException e)
            {
            }
        }
    }
}