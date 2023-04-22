using Combat;
using UnityEngine;
using Utils;

namespace Inventory.Item
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item Data")]
    public class ItemDataSo : ScriptableObject
    {
        [Tooltip("物品属性")] public ItemData itemData;
        
        [Header("属性参数和预制件")]
        [Tooltip("预制件")] public GameObject eqPrefabs;
        [Tooltip("武器的属性")] public AttackDataSo weaponData;
        [Tooltip("可使用物品的属性")] public UsableItemDataSo usableItemDataSo;
        [Tooltip("武器配套的动画效果")] public AnimatorOverrideController weaponAnimator;
    }// class ItemDataSo
}// namespace Inventory.Item