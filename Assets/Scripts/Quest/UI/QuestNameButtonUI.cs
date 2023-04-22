using System;
using Quest.Logic;
using UnityEngine;
using UnityEngine.UI;

namespace Quest.UI
{
    public class QuestNameButtonUI : MonoBehaviour
    {
        [Tooltip("任务名")] public Text questNameText;
        [Tooltip("任务数据")] public QuestDataSo questDataSo;
        [Tooltip("任务描述")] public Text description;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(OpenContent);
        }

        public void SetupButton(QuestDataSo quest)  // 设置当前button显示
        {
            questDataSo = quest;
            questNameText.text = quest.isComplete ? quest.questName + "(已完成)" : quest.questName;
            if (quest.isFinished)   // 任务奖励领取完之后切换一下显示
            {
                questNameText.text = quest.questName + "(已结束)";
            }
        }

        private void OpenContent()   // 显示任务内容
        {
            description.text = questDataSo.description; // 显示任务描述
            QuestUI.Instance.SetupRequirementList(questDataSo); // 显示需求描述

            foreach (Transform reward  in QuestUI.Instance.rewardRectTransform) // 销毁之前存留的奖励显示
            {
                Destroy(reward.gameObject);
            }
            
            foreach (var reward in questDataSo.rewards) // 显示奖励需求
            {
                QuestUI.Instance.SetupReward(reward.itemDataSo, reward.amount);
            }
        }
    }
}
