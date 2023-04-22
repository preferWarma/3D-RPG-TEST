using System;
using Dialogue;
using Dialogue.Logic;
using UnityEngine;

namespace Quest.Logic
{
    [RequireComponent(typeof(DialogueController))]
    public class QuestGiver : MonoBehaviour
    {
        [Header("对话数据")] 
        [Tooltip("开始对话数据")] public DialogueDataSo startQuest;
        [Tooltip("进行中对话数据")] public DialogueDataSo progressQuest;
        [Tooltip("完成对话数据")] public DialogueDataSo completeQuest;
        [Tooltip("结束对话数据")] public DialogueDataSo finishQuest;
        
        private DialogueController _dialogueController; // 对话控制器
        private QuestDataSo _currentQuestDataSo;    // 当前的任务数据

        private bool IsStarted =>
            QuestManager.Instance.HaveQuest(_currentQuestDataSo) && QuestManager.Instance.GetTask(_currentQuestDataSo).IsStarted;

        private bool IsComplete =>
            QuestManager.Instance.HaveQuest(_currentQuestDataSo) && QuestManager.Instance.GetTask(_currentQuestDataSo).IsComplete;

        private bool IsFinished =>
            QuestManager.Instance.HaveQuest(_currentQuestDataSo) && QuestManager.Instance.GetTask(_currentQuestDataSo).IsFinished;

        private void Awake()
        {
            _dialogueController = GetComponent<DialogueController>();
        }

        private void Start()
        {
            _dialogueController.currentDialogueDataSo = startQuest; // 初始数据应该为开始对话数据
            _currentQuestDataSo = _dialogueController.currentDialogueDataSo.GetQuestDataSo();   // 获取当前对话数据中的任务数据
        }

        private void Update()
        {
            if (IsStarted)  // 任务开始了
            {
                _dialogueController.currentDialogueDataSo = IsComplete ? completeQuest : progressQuest; // 根据任务是否完成切换对话
            }

            if (IsFinished) // 任务结束了
            {
                _dialogueController.currentDialogueDataSo = finishQuest;
            }
        }
    }// class QuestGiver
}// namespace Quest.Logic
