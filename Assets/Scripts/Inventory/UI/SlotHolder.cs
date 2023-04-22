using System;
using Inventory.Logic;
using Manager;
using Quest.Logic;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace Inventory.UI
{
    public class SlotHolder : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("装备槽类型")] public Enums.SlotType slotType;
        [Header("装备槽UI组件")] public ItemUI itemUI;

        public void UpdateItemUI()  // 更新当前单元格
        {
            switch (slotType)   // 绑定对应数据库
            {
                case Enums.SlotType.Bag:
                    itemUI.Bag = InventoryManager.Instance.inventoryDataSo; // 绑定对应数据库
                    break;
                case Enums.SlotType.Weapon:
                    itemUI.Bag = InventoryManager.Instance.equipmentDataSo;

                    if (itemUI.GetItemDataSo())
                    {
                        GameManager.Instance.playerStats.ChangeWeapon(itemUI.GetItemDataSo()); // 切换武器
                    }
                    else
                    {
                        GameManager.Instance.playerStats.UnEquipWeapon();
                    }
                    break;
                case Enums.SlotType.Shield:
                    itemUI.Bag = InventoryManager.Instance.equipmentDataSo;

                    if (itemUI.GetItemDataSo())
                    {
                        GameManager.Instance.playerStats.ChangeShield(itemUI.GetItemDataSo()); // 切换武器
                    }
                    else
                    {
                        GameManager.Instance.playerStats.UnEquipShield();
                    }
                    break;
                case Enums.SlotType.Action:
                    itemUI.Bag = InventoryManager.Instance.actionDataSo;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            var item = itemUI.Bag.items[itemUI.Index];  // 获取物品所在单元格数据
            itemUI.SetupItemUI(item.itemDataSo, item.amount);
        }
        
        public void OnPointerClick(PointerEventData eventData)  // 鼠标点击事件
        {
            if (eventData.clickCount % 2 == 0)  // 双击
            {
                UseCureItem();
            }
        }

        public void UseCureItem()  // 使用物品
        {
            if (!itemUI.GetItemDataSo()) return; // 避免双击空物体造成空引用异常
            
            if (itemUI.GetItemDataSo().itemData.itemType == Enums.ItemType.Usable
                && itemUI.Bag.items[itemUI.Index].amount > 0)   // 可以使用的物品并且数量大于0
            {
                GameManager.Instance.playerStats.CureHealth(itemUI.GetItemDataSo().usableItemDataSo.curePoint); // 回血
                itemUI.Bag.items[itemUI.Index].amount -= 1; // 数量减一
                // 更新任务进度
                QuestManager.Instance.CheckQuestProgress(itemUI.GetItemDataSo().itemData.name, -1);
            }
            UpdateItemUI();
        }

        public void OnPointerEnter(PointerEventData eventData)  // 鼠标悬停时触发
        {
            if (itemUI.GetItemDataSo())
            {
                InventoryManager.Instance.itemToolTip.SetToolTip(itemUI.GetItemDataSo());   // 更新描述
                InventoryManager.Instance.itemToolTip.gameObject.SetActive(true);   // 设置可见
            }
        }

        public void OnPointerExit(PointerEventData eventData)   // 鼠标离开时触发
        {
            InventoryManager.Instance.itemToolTip.gameObject.SetActive(false);   // 设置隐藏
        }

        private void OnDisable()
        {
            InventoryManager.Instance.itemToolTip.gameObject.SetActive(false);   // 关闭描述信息
        }
    }// class SlotHolder
}// Inventory.UI
