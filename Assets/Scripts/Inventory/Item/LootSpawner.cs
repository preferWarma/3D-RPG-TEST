using UnityEngine;
using Utils;
using Utils.Extension;

namespace Inventory.Item
{
    public class LootSpawner : MonoBehaviour
    {
        [Tooltip("该怪物可能掉落的物品列表")] public LootItem[] lootItems;

        public void SpawnLoot() // 掉落物品
        {
            var currentWeight = Random.value;   // 概率生成
            foreach (var lootItem in lootItems)
            {
                if (currentWeight >= lootItem.weight) continue; // 概率不足则跳过
                
                var obj = Instantiate(lootItem.item);   // 生成
                obj.transform.position = transform.position + Vector3.up * 3;   // 从天而降
                break;  // 只掉落一个物品
            }
        }
    }// class LootSpawner
}// namespace Inventory.Item
