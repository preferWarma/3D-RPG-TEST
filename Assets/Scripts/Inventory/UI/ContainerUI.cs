using UnityEngine;

namespace Inventory.UI
{
    public class ContainerUI : MonoBehaviour
    {
        public SlotHolder[] slotHolders;    // 所有单元格的数组

        public void RefreshUI() // 刷新UI
        {
            for (var i = 0; i < slotHolders.Length; i++)
            {
                slotHolders[i].itemUI.Index = i;    // 将单元格下标一一对应
                slotHolders[i].UpdateItemUI();  // 更新单元格显示
            }
        }
        
    }// class Container.UI
}// namespace Inventory.UI
