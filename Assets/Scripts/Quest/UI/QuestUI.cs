using Inventory.Item;
using Inventory.UI;
using Quest.Logic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utils;

namespace Quest.UI
{
    public class QuestUI : Singleton<QuestUI>
    {
        [Header("基本属性")] 
        [Tooltip("Quest panel")] public GameObject questPanel;
        [Tooltip("描述详情")] public ItemToolTip toolTip;
        [HideInInspector] public bool isOpen;

        [Header("任务属性")] 
        [Tooltip("任务显示位置")] public RectTransform questListRectTransform;
        [Tooltip("任务对应按钮prefab")] public QuestNameButtonUI questNameButton;

        [FormerlySerializedAs("questContent")]
        [Header("详情属性")] 
        [Tooltip("任务描述组件")] public Text questDescription;

        [Header("任务需求")] 
        [Tooltip("任务需求的显示位置")] public RectTransform requirementRectTransform;
        [Tooltip("任务需求对象prefab")] public QuestRequirementUI requirement;

        [Header("任务奖励")] 
        [Tooltip("任务奖励显示位置")] public RectTransform rewardRectTransform;
        [Tooltip("奖励物品prefab")] public ItemUI rewardUI;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                isOpen = !isOpen;
                questPanel.SetActive(isOpen);   // 面板显示开关
                questDescription.text = string.Empty;   // 清空初始的描述
                SetupQuestList();   // 显示任务的面板
                if (!isOpen)
                    toolTip.gameObject.SetActive(false);    // 避免关闭面板再次打开的时候还会显示
            }
        }

        public void SetupQuestList()  // 显示任务面板
        {
            foreach (Transform item in questListRectTransform)  // 销毁左侧初始的任务列表
            {
                Destroy(item.gameObject);
            }

            foreach (Transform item in requirementRectTransform)    // 销毁初始的任务需求列表
            {
                Destroy(item.gameObject);
            }

            foreach (Transform item in rewardRectTransform) // 销毁初始的任务奖励列表
            {
                Destroy(item.gameObject);
            }

            foreach (var task in QuestManager.Instance.taskList)
            {
                var newTask = Instantiate(questNameButton, questListRectTransform);
                newTask.SetupButton(task.questDataSo);
                newTask.description = questDescription;
            }
        }

        public void SetupRequirementList(QuestDataSo questDataSo)  // 显示需求列表
        {
            foreach (Transform item in requirementRectTransform)    // 销毁初始的任务需求列表
            {
                Destroy(item.gameObject);
            }

            foreach (var require in questDataSo.requirementList)
            {
                var q = Instantiate(requirement, requirementRectTransform);
                q.SetUpRequirement(require, questDataSo.isFinished);    // 显示需求信息
                
            }
        }

        public void SetupReward(ItemDataSo itemDataSo, int amount)  // 显示奖励列表
        {
            var item = Instantiate(rewardUI, rewardRectTransform);
            item.SetupItemUI(itemDataSo, amount);
        }
        
        
    }// class QuestUI
}// namespace Quest.UI
