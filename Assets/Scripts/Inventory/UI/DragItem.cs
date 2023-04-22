using System;
using Inventory.Logic;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;
// ReSharper disable Unity.InefficientPropertyAccess
// ReSharper disable ConvertIfStatementToConditionalTernaryExpression

namespace Inventory.UI
{
    [RequireComponent(typeof(ItemUI))]
    public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private ItemUI _currentItemUI;  // 当前的物品UI
        private SlotHolder _currentSlotHolder;  // 当前物品框组件
        private SlotHolder _targetHolder;    // 目标物品框组件


        private void Awake()
        {
            _currentItemUI = GetComponent<ItemUI>();
            _currentSlotHolder = GetComponentInParent<SlotHolder>();
        }

        public void OnBeginDrag(PointerEventData eventData) // 拖拽开始
        {
            // 记录原始信息
            InventoryManager.Instance.currentDragData = new DragData();
            var currentDragData = InventoryManager.Instance.currentDragData;
            currentDragData.originalHolder = _currentSlotHolder;    // 原有卡槽
            currentDragData.originalParent = (RectTransform)transform.parent;   // 原有父级
            transform.SetParent(InventoryManager.Instance.dragCanvas.transform,  true);  // 设置到拖拽父级, 避免遮挡
            

        }

        public void OnDrag(PointerEventData eventData)  // 拖拽过程中
        {
            transform.position = eventData.position;    // 跟随鼠标位置
        }
        
        public void OnEndDrag(PointerEventData eventData)   // 拖拽结束
        {
            // 是否指向卡槽
            if (EventSystem.current.IsPointerOverGameObject())  // 是否指向位置有GameObject
            {
                if (InventoryManager.Instance.CheckInventoryUI(eventData.position)  // 在背包栏
                    || InventoryManager.Instance.CheckActionUI(eventData.position)  // 在活动栏
                    || InventoryManager.Instance.CheckEquipmentUI(eventData.position))  // 在装备栏
                {
                    if (eventData.pointerEnter.gameObject.GetComponent<SlotHolder>())   // 鼠标松开的地方是否包含SlotHolder组件
                    {
                        _targetHolder = eventData.pointerEnter.gameObject.GetComponent<SlotHolder>();    // 对于空格子, 直接找
                    }
                    else
                    {
                        _targetHolder = eventData.pointerEnter.gameObject.GetComponentInParent<SlotHolder>();    // 对于有显示的格子, 找父级
                    }


                    var currentItemType = _currentItemUI.Bag.items[_currentItemUI.Index].itemDataSo.itemData.itemType;  // 当前的物品类型
                    if (_targetHolder != InventoryManager.Instance.currentDragData.originalHolder) {    // 如果当前holder不等于原有holder才交换
                        // 交换物品
                        switch (_targetHolder.slotType) // 目标位置位置的背包类型
                        {
                            case Enums.SlotType.Bag: // 背包里直接拖拽
                                SwapItem();
                                break;
                            case Enums.SlotType.Action:
                                if (currentItemType == Enums.ItemType.Usable) // 如果当前拖拽的物体是可以使用的道具
                                {
                                    SwapItem();
                                }

                                break;
                            case Enums.SlotType.Shield:
                                if (currentItemType == Enums.ItemType.Shield) // 如果当前拖拽的物体是防具类型
                                {
                                    SwapItem();
                                }

                                break;
                            case Enums.SlotType.Weapon:
                                if (currentItemType == Enums.ItemType.Weapon) // 如果当前拖拽的物体是武器类型
                                {
                                    SwapItem();
                                }

                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    _currentSlotHolder.UpdateItemUI();
                    _targetHolder.UpdateItemUI();
                }
            }
            transform.SetParent(InventoryManager.Instance.currentDragData.originalParent);
            var t = (RectTransform)transform;
            // 保证在格子中间
            t.offsetMax = -Vector2.one * 5;
            t.offsetMin = Vector2.one * 5; 

        }
        private void SwapItem()
        {
            var target = _targetHolder.itemUI.Bag.items[_targetHolder.itemUI.Index]; // 获取目标holder在数据库中的引用
            var tmp = _currentSlotHolder.itemUI.Bag.items[_currentSlotHolder.itemUI.Index]; // 获取当前holder在数据库中的引用

            if (target.itemDataSo == tmp.itemDataSo && target.itemDataSo.itemData.overlap)    // 目标类型相同且可堆叠
            {
                target.amount += tmp.amount;
                tmp.amount = 0;
                tmp.itemDataSo = null;
            }
            else // 交换，修改数据库
            {
                _currentSlotHolder.itemUI.Bag.items[_currentSlotHolder.itemUI.Index] = target;
                _targetHolder.itemUI.Bag.items[_targetHolder.itemUI.Index] = tmp;
            }
        }

    }// class DragItem
}// namespace Inventory.UI
