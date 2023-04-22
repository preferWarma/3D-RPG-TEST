using System.Collections.Generic;
using System.Linq;
using Inventory.Logic;
using UnityEngine;
using Utils;

namespace Quest.Logic
{
    [CreateAssetMenu(fileName = "New Quest", menuName = "Quest/Quest Data")]
    public class QuestDataSo : ScriptableObject
    {
        [Header("基本信息")]
        [Tooltip("任务名字")] public string questName;    
        [Tooltip("任务描述")] [TextArea] public string description;

        [HideInInspector] public bool isStarted;    // 任务是否开始
        [HideInInspector] public bool isComplete;   // 任务是否完成
        [HideInInspector] public bool isFinished;   // 任务是否结束

        [Tooltip("任务要求列表")] public List<QuestRequirement> requirementList = new List<QuestRequirement>();
        [Tooltip("任务奖励")] public List<InventoryItem> rewards;

        public void CheckQuestProgress()    // 检查任务进度
        {
            var finishRequirements 
                = requirementList.Where(r => r.requireAmount <= r.currentAmount);
            isComplete = finishRequirements.Count() == requirementList.Count;
        }

        public List<string> RequirementNames()  // 获取需求列表的名字的列表
        {
            return requirementList.Select(requirement => requirement.name).ToList();
        }

        public void GetRewards()    // 获取任务奖励
        {
            foreach (var reward in rewards)
            {
                if (reward.amount < 0)  // 任务需要的消耗品
                {
                    var count = Mathf.Abs(reward.amount);
                    if (InventoryManager.Instance.QuestItemInBag(reward.itemDataSo) != null)    // 背包有这个物体就先扣背包的
                    {
                        if (InventoryManager.Instance.QuestItemInBag(reward.itemDataSo).amount >= count)    // 背包数量够
                        {
                            InventoryManager.Instance.QuestItemInBag(reward.itemDataSo).amount -= count;    // 直接扣
                        }
                        else // 背包数量不够
                        {
                            count -= InventoryManager.Instance.QuestItemInBag(reward.itemDataSo).amount;
                            InventoryManager.Instance.QuestItemInBag(reward.itemDataSo).amount = 0;
                        }
                    }

                    if (count <= 0) continue;   // 如果已经满足了就结束

                    foreach (var item in InventoryManager.Instance.actionDataSo.items)  // 不满足则再去活动槽扣
                    {
                        if (item.amount >= count)
                        {
                            item.amount -= count;
                            count = 0;
                        }
                        else
                        {
                            count -= item.amount;
                            item.amount = 0;
                        }

                        if (count == 0) break;
                    }
                }
                else // 实际任务奖励
                {
                    InventoryManager.Instance.inventoryDataSo.AddItem(reward.itemDataSo, reward.amount);    // 添加到背包
                }
            }
            InventoryManager.Instance.actionContainerUI.RefreshUI();    // 更新快捷槽
            InventoryManager.Instance.inventoryContainerUI.RefreshUI(); // 更新背包槽
        }

    }// class QuestDataSo
}// namespace Quest.Logic
