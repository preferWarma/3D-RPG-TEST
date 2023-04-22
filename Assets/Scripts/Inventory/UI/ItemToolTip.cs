using Inventory.Item;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.UI
{
    public class ItemToolTip : MonoBehaviour
    {
        [Header("自身文本描述组件")] 
        [Tooltip("物品名称")] public Text itemName;
        [Tooltip("物品描述")] public Text description;

        private RectTransform _rectTransform;   // 自身显示的矩形位置范围

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            UpdatePosition();
        }

        private void Update()
        {
            UpdatePosition();
        }

        public void SetToolTip(ItemDataSo itemDataSo)
        {
            itemName.text = itemDataSo.itemData.name;
            description.text = itemDataSo.itemData.description;
        }

        private void UpdatePosition()   // 更新自身位置
        {
            var mousePosition = Input.mousePosition;
            var corns = new Vector3[4];
            _rectTransform.GetWorldCorners(corns);  // 获取矩形范围的世界坐标的四个顶点的坐标
            var height = corns[1].y - corns[0].y;
            var width = corns[3].x - corns[0].x;

            if (mousePosition.y < height)   // 最下端无法显示完整
            {
                _rectTransform.position = mousePosition + Vector3.up * (height * 0.6f);
            }
            else if (Screen.width - mousePosition.x > width) //右侧可以显示完整
            {
                _rectTransform.position = mousePosition + Vector3.right * (width * 0.6f);
            }
            else // 右侧无法显示完整
            {
                _rectTransform.position = mousePosition + Vector3.left * (width * 0.6f);
            }
        }
        
    }// class ItemToolTip
}// namespace Inventory.UI
