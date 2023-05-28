using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Project.UI
{
    public abstract class UiReactionOnPointerElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private Image _back;

        [SerializeField]
        private Color _selectedColor = Color.white;

        [SerializeField]
        private Color _unselectedColor = Color.white;

        private void OnDisable()
        {
            _back.color = _unselectedColor;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _back.color = _selectedColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _back.color = _unselectedColor;
        }
    }
}