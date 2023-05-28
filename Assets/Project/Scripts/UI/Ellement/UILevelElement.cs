using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;

namespace Project.UI
{
    public class UILevelElement : UiReactionOnPointerElement
    {
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
            public GameObject StarImage
            {
                get;
                private set;
            }
        }

        [SerializeField, Space]
        private Button _button;

        [SerializeField]
        private Image _levelIcon;

        [SerializeField]
        private TextMeshProUGUI _levelName;

        [SerializeField]
        private DifficultPreset[] _difficultPresets = null;

        private Action _onButtonClick;

        public string SceneName
        {
            get;
            private set;
        }

        private void Start()
        {
            _button.onClick.AddListener(OnButtonClick);
            _difficultPresets.Do(x => x.StarImage.SetActive(false));
        }

        private void OnButtonClick()
        {
            _onButtonClick.Invoke();
        }

        public void Setup(Sprite icon, string levelName, string sceneName, Action onButtonClick)
        {
            _onButtonClick = onButtonClick;
            _levelIcon.sprite = icon;
            _levelName.text = levelName;

            SceneName = sceneName;
        }

#if UNITY_EDITOR
        [SerializeField, Space]
        private UniformModifier _back;

        [SerializeField]
        private UniformModifier _front;

        [SerializeField]
        private float _value;

        [SerializeField]
        private float _offset;


        private void OnValidate()
        {
            if (Application.isPlaying || !_back || !_front)
            {
                return;
            }
            
            _back.Radius = _value;
            _front.Radius = _value - _offset;
        }
#endif
    }
}