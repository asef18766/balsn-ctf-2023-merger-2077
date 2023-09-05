using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private RectTransform rect;
        private RectTransform _mRect;
        private Rigidbody2D _rigidbody2D;
        private void Start()
        {
            _mRect = GetComponent<RectTransform>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private Vector2 GetMousePos()
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, Input.mousePosition, null, out var outPos);
            return outPos;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _mRect.anchoredPosition = GetMousePos();
            _rigidbody2D.simulated = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            _mRect.anchoredPosition = GetMousePos();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Debug.Log("end drag");
            _rigidbody2D.simulated = true;
        }
    }
}