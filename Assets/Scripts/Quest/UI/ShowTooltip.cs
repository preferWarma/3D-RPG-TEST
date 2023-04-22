using System;
using Inventory.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Quest.UI
{
    public class ShowTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private ItemUI currentItemUI;

        private void Awake()
        {
            currentItemUI = GetComponent<ItemUI>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            QuestUI.Instance.toolTip.gameObject.SetActive(true);
            QuestUI.Instance.toolTip.SetToolTip(currentItemUI.currentItemDataSo);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            QuestUI.Instance.toolTip.gameObject.SetActive(false);
        }
    }// class ShowTooltip
}// namespace Quest.UI
