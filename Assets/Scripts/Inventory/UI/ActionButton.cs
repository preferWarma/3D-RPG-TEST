using System;
using UnityEngine;
using Utils.Extension;

namespace Inventory.UI
{
    public class ActionButton : MonoBehaviour
    {
        [Label("快捷按键")] public KeyCode key;

        private SlotHolder _slotHolder;
        
        private void Awake()
        {
            _slotHolder = GetComponent<SlotHolder>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(key) && _slotHolder.itemUI.GetItemDataSo())
            {
                _slotHolder.UseCureItem();
            }
        }
    }// class ActionButton
}// namespace Inventory.UI
