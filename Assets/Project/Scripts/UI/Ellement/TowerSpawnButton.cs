using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Project.UI
{
    public class TowerSpawnButton : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField]
        private Button _button = null;

        [SerializeField]
        private Image _icon = null;

        [SerializeField]
        private Image _sectedImage = null;
        
        [SerializeField]
        private Color _selcetedColor = Color.black;
        [SerializeField]
        private Color _unselcetedColor = Color.black;

        private int _index = 0;

        private TowerType _type = default;
        private Action<int> _onClick;
        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
        }

        private void Start()
        {
            _button.onClick.AddListener(OnButtonClick);
        }

        public void Setup(UITowerSetting.UITowerPreset towerPreset, int index, Action<int> onClick)
        {
            _index = index;

            _icon.sprite = towerPreset.UIIcon;

            _onClick = onClick;

            Refresh();
        }

        public void Refresh(bool isSelected = false)
        {
            _isSelected = isSelected;
            _sectedImage.color = isSelected ? _selcetedColor : _unselcetedColor;
        }

        private void OnButtonClick()
        {
        }
        
        //Нужно чтобы именно вытягивать башенку из списка 
        public void OnPointerDown(PointerEventData eventData)
        {
            _onClick?.Invoke(_index);
        }
    }
}

