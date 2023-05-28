using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    public class UIDifficultElement : UiReactionOnPointerElement
    {
        [field: SerializeField]
        public LevelDifficultType Type
        {
            get;
            private set;
        }

        [SerializeField]
        private TextMeshProUGUI _peizeText;

        [SerializeField]
        private Button _button;
        
        private Action _callback;

        private void Start()
        {
            _button.onClick.AddListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            _callback.Invoke();
        }

        public void Setup(int prizeValue, Action callback)
        {
            _callback = callback;
            _peizeText.text = $"+{prizeValue}";
        }

#if UNITY_EDITOR
        [Serializable]
        public class DifficultPreset
        {
            [field: SerializeField]
            public LevelDifficultType Type
            {
                get;
                private set;
            }

            [field: SerializeField]
            public Color Color
            {
                get;
                private set;
            }

            [field: SerializeField]
            public string Label
            {
                get;
                private set;
            }
        }

        [SerializeField, Space]
        private TextMeshProUGUI _lable;

        [SerializeField]
        private Image _frontImage;

        [SerializeField]
        private DifficultPreset[] _difficultPreset;

        private void OnValidate()
        {
            ChangeVisual();
        }

        private void ChangeVisual()
        {
            var preset = _difficultPreset.FirstOrDefault(x=>x.Type == Type);

            if (preset == null)
            {
                return;
            }
            
            _lable.text = preset.Label;
            _frontImage.color = preset.Color;
        }
#endif
    }
}