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

        private int _index = 0;

        private TowerType _type = default;
        private Action<int> _onClick;

        private void Start()
        {
            _button.onClick.AddListener(OnButtonClick);
        }

        public void Setup(TowerSettings.TowerPreset towerPreset, int index, Action<int> onClick)
        {
            _index = index;

            _icon.sprite = towerPreset.UIIcon;

            _onClick = onClick;

            Refresh();
        }

        public void Refresh(bool isSelected = false)
        {
            _sectedImage.enabled = isSelected;
        }

        private void OnButtonClick()
        {
        }

        public void OnPointerClick(PointerEventData eventData)
        {
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _onClick?.Invoke(_index);
        }
    }
}

