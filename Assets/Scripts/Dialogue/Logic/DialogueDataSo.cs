using System.Collections.Generic;
using System.Linq;
using Quest.Logic;
using UnityEngine;
using Utils;

namespace Dialogue.Logic
{
    [CreateAssetMenu(fileName = "New DialogueData", menuName = "Dialogue/DialogueData")]
    public class DialogueDataSo : ScriptableObject
    {
        [Tooltip("对话列表")] public List<DialoguePiece> dialoguePieces = new List<DialoguePiece>();  // 对话条
        public readonly Dictionary<string, int> indexDictionary = new Dictionary<string, int>();    // ID和对象索引的匹配字典

#if UNITY_EDITOR    // unity编辑器中
        private void OnValidate()   // 当我们在unity窗口更改数据的时候调用
        {
            indexDictionary.Clear();
            for (var i = 0; i < dialoguePieces.Count; i++)
            {
                indexDictionary[dialoguePieces[i].id] = i;
            }
        }
#else   // 打包时
    void Awake()//保证在打包执行的游戏里第一时间获得对话的所有字典匹配 
    {
        indexDictionary.Clear();
            for (var i = 0; i < dialoguePieces.Count; i++)
            {
                indexDictionary[dialoguePieces[i].id] = i;
            }
    }
#endif

        public QuestDataSo GetQuestDataSo() // 获取任务数据
        {
            QuestDataSo result = null;
            foreach (var dialoguePiece in dialoguePieces.Where(dialoguePiece => dialoguePiece.receiveQuest != null))
            {
                result = dialoguePiece.receiveQuest;
            }

            return result;
        }
        
    }// class DialogueDataSo
}// namespace Dialogue.Logic
