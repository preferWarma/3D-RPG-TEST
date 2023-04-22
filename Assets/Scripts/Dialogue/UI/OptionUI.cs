using Inventory.Logic;
using Quest.Logic;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Dialogue.UI
{
    public class OptionUI : MonoBehaviour
    {
        public Text optionText; // Button的文本对象

        private Button thisButton;  // 当前按钮本身
        private string targetID;    // 选项对应的目标跳转ID
        private DialoguePiece currentPiece; // 当前选项所属的对话
        private bool takeQuest;  // 是否接受任务

        private void Awake()
        {
            thisButton = GetComponent<Button>();
            thisButton.onClick.AddListener(OnClicked);
        }


        public void SetOption(DialoguePiece piece, DialogueOption option)
        {
            currentPiece = piece;
            optionText.text = option.text;
            targetID = option.targetID;
            takeQuest = option.takeQuest;
        }

        private void OnClicked()
        {
            if (targetID != "") // 设置下一条piece的索引位置
            {
                DialogueUI.Instance.currentDialogueIndex = DialogueUI.Instance.dialogueDataSo.indexDictionary[targetID];
            }
            
            if (currentPiece.receiveQuest)  // 如果当前对话可以接受任务
            {
                if (takeQuest)  // 如果当前选项接受了任务
                {
                    if (QuestManager.Instance.HaveQuest(currentPiece.receiveQuest)) // 已经接受了该任务
                    {
                        if (QuestManager.Instance.GetTask(currentPiece.receiveQuest).IsComplete)    // 如果任务完成
                        {
                            currentPiece.receiveQuest.GetRewards(); // 获取奖励
                            QuestManager.Instance.GetTask(currentPiece.receiveQuest).IsFinished = true; // 设置任务状态为完成
                        }
                    }
                    else // 接受任务
                    {
                        var newTask = new QuestTask { questDataSo = Instantiate(currentPiece.receiveQuest) };   // 生成并克隆数据
                        QuestManager.Instance.taskList.Add(newTask);    // 当前的任务列表中添加此任务
                        QuestManager.Instance.GetTask(newTask.questDataSo).IsStarted = true; // 任务状态改为开始
                        
                        foreach (var requirementName in newTask.questDataSo.RequirementNames()) // 在所需的名字列表中逐一检查
                        {
                            InventoryManager.Instance.CheckItem(requirementName);   // 然后更新
                        }
                    }
                }
            }
            if (targetID == "") // 已经是最后的回答了
            {
                DialogueUI.Instance.panel.SetActive(false); // 关闭窗口
            }
            else
            {
                var index = DialogueUI.Instance.dialogueDataSo.indexDictionary[targetID];
                DialogueUI.Instance.UpdateMainDialogue(DialogueUI.Instance.dialogueDataSo.dialoguePieces[index]);
            }
        }
    }// class OptionUI
}// namespace Dialogue.UI
