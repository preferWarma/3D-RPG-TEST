using Inventory.Item;
using Inventory.Logic;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.UI
{
    public class ItemUI : MonoBehaviour
    {
        [Header("相关组件")] 
        [Tooltip("图片栏")] public Image icon;
        [Tooltip("文字描述栏")] public Text amount;
        [HideInInspector] public ItemDataSo currentItemDataSo;  // 自身的物品数据, 方便在任务中显示tooltip

        public InventoryDataSo Bag { get; set; }   // 当前所属的数据库及种类
        public int Index { get; set; } = -1; // 对应数据库中的下标

        public void SetupItemUI(ItemDataSo itemData, int itemNum)  // 更新数据库中的数据并显示
        {
            if (itemNum <= 0)
            {
                if (Bag)    // 避免在任务列表中显示出错
                    Bag.items[Index].itemDataSo = null;
                icon.gameObject.SetActive(false);
                return;
            }
            
            if (itemData)
            {
                currentItemDataSo = itemData;
                icon.sprite = itemData.itemData.icon;
                amount.text = itemNum.ToString();
                icon.gameObject.SetActive(true);
            }
            else
            {
                icon.gameObject.SetActive(false);
            }
        }

        public ItemDataSo GetItemDataSo()   // 返回背包中对应的数据
        {
            return Bag.items[Index].itemDataSo;
        }

    }// class ItemUI
}// namespace Inventory.UI
