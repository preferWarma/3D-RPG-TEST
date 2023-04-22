using Inventory.Logic;
using Quest.Logic;
using UnityEngine;

namespace Inventory.Item
{
    public class ItemPickUp : MonoBehaviour
    {
        [Tooltip("物品属性信息")] public ItemDataSo itemDataSo;   // 物品属性信息
        private void OnTriggerEnter(Collider other) // 碰撞开始的时候触发
        {
            if (!other.CompareTag("Player")) return;
            InventoryManager.Instance.inventoryDataSo.AddItem(itemDataSo, itemDataSo.itemData.amount);    // 物品添加到背包
            InventoryManager.Instance.inventoryContainerUI.RefreshUI(); // 刷新背包显示
            QuestManager.Instance.CheckQuestProgress(itemDataSo.itemData.name, itemDataSo.itemData.amount);
            Destroy(gameObject);
        }
    }// class ItemPickUp
    
}// namespace Inventory.Item
