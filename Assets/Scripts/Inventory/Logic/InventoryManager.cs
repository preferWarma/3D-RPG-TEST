using System.Globalization;
using Inventory.Item;
using Inventory.UI;
using Manager;
using Quest.Logic;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Inventory.Logic
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        [Header("背包数据")]
        [Tooltip("背包数据模板")] public InventoryDataSo templateInventoryDataSo;
        [HideInInspector] [Tooltip("背包数据集")] public InventoryDataSo inventoryDataSo;
        
        [Tooltip("背包数据模板")] public InventoryDataSo templateActionDataSo;
        [HideInInspector] [Tooltip("活动框数据集")] public InventoryDataSo actionDataSo;
        
        [Tooltip("背包数据模板")] public InventoryDataSo templateEquipmentDataSo;
        [HideInInspector] [Tooltip("装备栏数据集")] public InventoryDataSo equipmentDataSo;

        [Header("UI容器")] 
        [Tooltip("背包容器")] public ContainerUI inventoryContainerUI;
        [Tooltip("活动框容器")] public ContainerUI actionContainerUI;
        [Tooltip("装备栏容器")] public ContainerUI equipmentContainerUI;

        [Header("拖拽时所用canvas")] public Canvas dragCanvas;
        [HideInInspector] public DragData currentDragData;  // 保存拖拽时的临时值

        [Header("UI Panel")] 
        [Tooltip("背包栏")] public GameObject inventoryUIGameObject;  // 背包栏
        [Tooltip("人物栏")] public GameObject characterStatsUIGameObject;   // 人物数据栏

        [Header("人物数据显示UI")] 
        [Tooltip("血量方面文本")] public Text heathText;
        [Tooltip("攻击方面文本")] public Text attackText;
        [Tooltip("防御方面文本")] public Text defenceText;
        [Tooltip("暴击方面文本")] public Text criticalText;

        [Header("物品详情")] 
        [Tooltip("物品详情组件")] public ItemToolTip itemToolTip;

        private bool _isOpen; // 是否开启


        protected override void Awake()
        {
            base.Awake();
            if (templateInventoryDataSo)
                inventoryDataSo = Instantiate(templateInventoryDataSo);
            if (templateActionDataSo)
                actionDataSo = Instantiate(templateActionDataSo);
            if (templateEquipmentDataSo)
                equipmentDataSo = Instantiate(templateEquipmentDataSo);
        }

        private void Start()
        {
            LoadData();
            inventoryContainerUI.RefreshUI();
            actionContainerUI.RefreshUI();
            equipmentContainerUI.RefreshUI();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))    // 按B开启或关闭背包
            {
                _isOpen = !_isOpen;
                inventoryUIGameObject.SetActive(_isOpen);
                characterStatsUIGameObject.SetActive(_isOpen);
            }

            var playerInfo = GameManager.Instance.playerStats;
            UpdateText(playerInfo.CurrentHealth, playerInfo.MinDamage, playerInfo.MaxDamage, playerInfo.CurrentDefence,
                playerInfo.CriticalChance);
        }
        
        private void UpdateText(int health, int minAttack, int maxAttack, int defence, float criticalChance)    // 更新数值显示
        {
            heathText.text = health.ToString();
            attackText.text = minAttack + "—" + maxAttack;
            defenceText.text = defence.ToString();
            criticalText.text = (criticalChance * 100).ToString(CultureInfo.InvariantCulture) + "%";
        }

        public void SaveData()
        {
            SaveDataManager.Instance.SaveData(inventoryDataSo, inventoryDataSo.name);
            SaveDataManager.Instance.SaveData(actionDataSo, actionDataSo.name);
            SaveDataManager.Instance.SaveData(equipmentDataSo, equipmentDataSo.name);
        }

        public void LoadData()
        {
            SaveDataManager.Instance.LoadData(inventoryDataSo, inventoryDataSo.name);
            SaveDataManager.Instance.LoadData(actionDataSo, actionDataSo.name);
            SaveDataManager.Instance.LoadData(equipmentDataSo, equipmentDataSo.name);
        }
        
        #region 检查拖拽物品是否在Slot的范围内

        public bool CheckInventoryUI(Vector3 mousePosition)  // 是否在背包槽
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var item in inventoryContainerUI.slotHolders)
            {
                var tmp = (RectTransform)item.transform;
                if (RectTransformUtility.RectangleContainsScreenPoint(tmp, mousePosition)) // 如果鼠标在槽内, 返回true
                {
                    return true;
                }
            }
            return false;
        }

        public bool CheckActionUI(Vector3 mousePosition)  // 是否在活动槽
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var item in actionContainerUI.slotHolders)
            {
                var tmp = (RectTransform)item.transform;
                if (RectTransformUtility.RectangleContainsScreenPoint(tmp, mousePosition)) // 如果鼠标在槽内, 返回true
                {
                    return true;
                }
            }
            return false;
        }
        
        public bool CheckEquipmentUI(Vector3 mousePosition)  // 是否在装备槽
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var item in equipmentContainerUI.slotHolders)
            {
                var tmp = (RectTransform)item.transform;
                if (RectTransformUtility.RectangleContainsScreenPoint(tmp, mousePosition)) // 如果鼠标在槽内, 返回true
                {
                    return true;
                }
            }
            return false;
        }
        
        #endregion

        #region 检测任务物品

        public void CheckItem(string itemName)
        {
            CheckBag(itemName);
            CheckAction(itemName);
        }

        private void CheckBag(string itemName)
        {
            foreach (var item in inventoryDataSo.items)
            {
                if (!item.itemDataSo) continue; // 空格子
                if (item.itemDataSo.itemData.name != itemName) continue;    // 当前物品与任务所需物品不匹配
                QuestManager.Instance.CheckQuestProgress(itemName, item.amount);    // 更新任务显示
            }
        }

        private void CheckAction(string itemName)
        {
            foreach (var item in actionDataSo.items)
            {
                if (!item.itemDataSo) continue; // 空格子
                if (item.itemDataSo.itemData.name != itemName) continue;    // 当前物品与任务所需物品不匹配
                QuestManager.Instance.CheckQuestProgress(itemName, item.amount);    // 更新任务显示
            }
        }

        #endregion

        #region 返回任务物品

        public InventoryItem QuestItemInBag(ItemDataSo itemDataSo)
        {
            return inventoryDataSo.items.Find(t => t.itemDataSo == itemDataSo);
        }
        
        public InventoryItem QuestItemInAction(ItemDataSo itemDataSo)
        {
            return actionDataSo.items.Find(t => t.itemDataSo == itemDataSo);
        }

        #endregion
        
        
    }// class InventoryManager
}// namespace Inventory.Logic
