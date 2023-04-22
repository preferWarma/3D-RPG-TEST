using System.Collections.Generic;
using System.Linq;
using Inventory.Item;
using UnityEngine;
using Utils;

namespace Inventory.Logic
{
    [CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory/InventoryData")]
    public class InventoryDataSo : ScriptableObject
    {
        [Tooltip("背包物品列表")] public List<InventoryItem> items;

        public void AddItem(ItemDataSo newItemData, int amount)    // 添加物品
        {
            var foundSameType = false;  // 是否找到同类物品
            if (newItemData.itemData.overlap)   // 如果可堆叠
            {
                foreach (var item in items.
                             Where(item => item.itemDataSo == newItemData))   // 查找是否有相同物品
                {
                    item.amount += amount;  // 直接数量加一, 无需重新生成
                    foundSameType = true;
                    break;
                }
            }

            // 如果装备栏有空位并且没有找到同类物品, 则生成物品
            foreach (var item in items.
                         Where(item => item.itemDataSo == null && !foundSameType))
            {
                item.itemDataSo = newItemData;
                item.amount = amount;
                break;
            }
            
        }
        
    }// class InventoryDataSo
    
    
}// namespace Inventory.Logic
