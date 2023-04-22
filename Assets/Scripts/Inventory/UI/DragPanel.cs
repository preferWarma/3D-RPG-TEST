using Inventory.Logic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Inventory.UI
{
    public class DragPanel : MonoBehaviour, IDragHandler,IPointerDownHandler
    {
        private RectTransform _rectTransform;   // 当前panel的范围
        private Canvas _canvas; // 改物体所属的canvas

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvas = InventoryManager.Instance.GetComponent<Canvas>();
        }

        public void OnDrag(PointerEventData eventData)
        {
            _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor; // panel的中心坐标加上鼠标的偏移量
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _rectTransform.SetSiblingIndex(2);  // 保证拖拽时可以不被另一个界面遮挡
        }
    }// class DragPanel
}// namespace Inventory.UI
