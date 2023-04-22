using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using System.Linq;
using Manager;

namespace Quest.Logic
{
    public class QuestManager : Singleton<QuestManager>
    {
        [Tooltip("当前任务接收的任务列表")] public List<QuestTask> taskList = new List<QuestTask>();


        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this);
        }

        private void Start()
        {
            LoadQuest();
        }

        public bool HaveQuest(QuestDataSo questDataSo) // 是否已经接受了任务
        {
            return questDataSo && taskList.Any(quest => quest.questDataSo.questName == questDataSo.questName);
        }

        public QuestTask GetTask(QuestDataSo questDataSo)   // 查找任务
        {
            return taskList.FirstOrDefault(task => task.questDataSo.questName == questDataSo.questName);
        }
        
        // 敌人死亡，拾取物品的时候来进行调用
        public void CheckQuestProgress(string questName, int amount)    // 检查任务进度
        {
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var task in taskList)
            {
                if (task.IsFinished) continue; // 如果任务已经完成了就跳过
                var matchTask = task.questDataSo.requirementList.Find(r => r.name == questName);    // 找到匹配的任务
                if (matchTask != null)
                {
                    matchTask.currentAmount += amount;
                }
                task.questDataSo.CheckQuestProgress();  // 检查当前任务数据进度
            }
        }
        
        public void SaveQuest()   // 保存任务系统
        {
            PlayerPrefs.SetInt("QuestCount", taskList.Count);   // 保存任务数量
            for (var i = 0; i < taskList.Count; i++)
            {
                SaveDataManager.Instance.SaveData(taskList[i].questDataSo, "task" + i); // 逐一保存每个任务(List不可直接保存)
            }
        }

        public void LoadQuest() // 加载任务系统
        {
            var count = PlayerPrefs.GetInt("QuestCount");   // 拿到任务数量
            for (var i = 0; i < count; i++)
            {
                var newQuestDataSo = ScriptableObject.CreateInstance<QuestDataSo>();   // 生成一个新的task
                SaveDataManager.Instance.LoadData(newQuestDataSo, "task" + i);  // 写入对应的数据
                taskList.Add(new QuestTask {questDataSo = newQuestDataSo}); // 然后给任务列表添加上
            }
        }
        
    }// class QuestManager
}// namespace Quest.Logic
