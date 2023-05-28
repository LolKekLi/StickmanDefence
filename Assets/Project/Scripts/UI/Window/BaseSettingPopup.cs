using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    public abstract class BaseSettingPopup : Window
    {
        [SerializeField]
        private Slider _musicSlider;
        [SerializeField]
        private Slider _soundSlider;
        [SerializeField]
        private Button _continueButton;

        [SerializeField, Space]
        private SelfTweenController _showTween;
        [SerializeField]
        private SelfTweenController _hideTween;

        public override bool IsPopup
        {
            get => true;
        }

        protected override void Start()
        {
            base.Start();
            
            _continueButton.onClick.AddListener(OnContinueButtonClick);
        }

        private void OnContinueButtonClick()
        {
            Hide();
        }

        protected override async void OnHide()
        {
            _hideTween.Play();
            await UniTask.Delay(TimeSpan.FromSeconds(_hideTween.LongestAnimationTime));
            
            base.OnHide();
        }

        protected override void OnShow()
        {
            base.OnShow();
            
            _showTween.Play();
        }
    }
}