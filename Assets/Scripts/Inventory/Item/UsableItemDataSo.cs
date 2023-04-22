using UnityEngine;
using Utils.Extension;

namespace Inventory.Item
{
    [CreateAssetMenu(fileName = "Usable Item", menuName = "Inventory/Usable Item Data")]
    public class UsableItemDataSo : ScriptableObject
    {
        [Label("恢复生命值量")] public int curePoint;
        
    }// class UsableItemDataSo
}// namespace Inventory.Item
