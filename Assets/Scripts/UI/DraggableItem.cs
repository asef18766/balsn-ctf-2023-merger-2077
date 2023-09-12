using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace UI
{
    public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private RectTransform rect;
        private RectTransform _mRect;
        private Rigidbody2D _rigidbody2D;
        public bool droppable;
        private float _oriGravScale;
        
        private void Start()
        {
            _mRect = GetComponent<RectTransform>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _oriGravScale = _rigidbody2D.gravityScale;

            _rigidbody2D.gravityScale = 0;
        }

        private Vector2 GetMousePos()
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, Input.mousePosition, null, out var outPos);
            return outPos;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _mRect.anchoredPosition = GetMousePos();
            _rigidbody2D.gravityScale = 0;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!droppable) return;
            _mRect.anchoredPosition = GetMousePos();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (droppable)
                _rigidbody2D.gravityScale = _oriGravScale;
            
        }
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (!col.CompareTag("DropBound")) return;
            droppable = true;
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            if (!col.CompareTag("DropBound")) return;
            droppable = false;
        }
    }
}