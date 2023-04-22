using System;
using System.Collections.Generic;
using Inventory.Item;
using Inventory.UI;
using Quest.Logic;
using UnityEngine;

namespace Utils
{
    // 基础人物属性
    [Serializable]
    public class CharacterData
    {
        [Header("基础人物属性")] 
        [Tooltip("最大生命值")] public int maxHealth;
        [Tooltip("当前生命值")] public int currentHealth;
        [Tooltip("基础防御力")] public int baseDefence;
        [Tooltip("当前防御力")] public int currentDefence;
        
        [Header("等级属性")] 
        [Tooltip("当前等级")] public int currentLevel;
        [Tooltip("最高等级")] public int maxLevel;
        [Tooltip("基础经验值")] public int baseExperience;
        [Tooltip("当前经验值")] public int currentExperience;
        [Tooltip("升级的属性加成")] public float levelBuff;

        public float LevelMultiplier => 1.5f; // 升级的倍率

        [Header("击败属性")]
        [Tooltip("击败可获得的经验")] public int killPoint;

    }// class CharacterData
    
    // 基础攻击属性
    [Serializable]
    public class AttackData
    {
        [Header("基础攻击属性")] 
        [Tooltip("普通攻击距离")] public float baseAttackRange;
        [Tooltip("技能攻击距离")] public float skillAttackRange;
        [Tooltip("攻击间隔时间")] public float coolDown;
        [Tooltip("最小攻击伤害")] public int minDamage;   // 希望攻击伤害不固定, 而是范围内的随机值, 所以设置最小伤害和最大伤害
        [Tooltip("最大攻击伤害")] public int maxDamage;
        [Tooltip("暴击伤害倍率")] public float criticalMultiplier;
        [Tooltip("暴击率")] [Range(0,1)] public float criticalChance;
    }// class AttackData
    
    // 基础属性
    [Serializable]
    public class ItemData
    {
        [Header("物品基础属性")] 
        public Enums.ItemType itemType;
        [Tooltip("名字")] public string name;
        [Tooltip("图标")] public Sprite icon;
        [Tooltip("是否可堆叠")] public bool overlap;
        [Tooltip("数量")] public int amount;
        [Tooltip("详情")] [TextArea] public string description;

    }// class ItemData

    // 背包栏属性
    [Serializable]
    public class InventoryItem
    {
        [Header("背包栏属性")]
        [Tooltip("物品属性")] public ItemDataSo itemDataSo;
        [Tooltip("物品数量")] public int amount;
    }// class InventoryData

    // 拖拽时需要保存的原有数据
    [Serializable]
    public class DragData
    {
        [Header("拖拽时需要保存的原有数据")] 
        [Tooltip("原有父级")] public RectTransform originalParent;
        [Tooltip("原有卡槽")] public SlotHolder originalHolder;
    }// class DragData
    
    // 物品掉落相关信息
    [Serializable]
    public class LootItem
    {
        [Header("物品掉落相关信息")]
        [Tooltip("掉落的物品")] public GameObject item;
        [Tooltip("掉落的概率")] [Range(0, 1)] public float weight;
    }// class LootItem

    // 对话选项相关
    [Serializable]
    public class DialogueOption
    {
        [Header("选项信息")]
        [Tooltip("选项显示的对话内容")] public string text;
        [Tooltip("选项对应的目标对话")] public string targetID;
        [Tooltip("是否接受任务")] public bool takeQuest;
    }// class DialogueOption
    
    // 对话信息相关
    [Serializable]
    public class DialoguePiece
    {
        [Header("对话信息")] 
        [Tooltip("对话ID")] public string id;
        [Tooltip("对话人物的图片")] public Sprite sprite;
        [Tooltip("对话内容")] [TextArea] public string text;
        [Tooltip("含有的任务信息")] public QuestDataSo receiveQuest;
        [Tooltip("对话选项列表")] public List<DialogueOption> dialogueOptions = new List<DialogueOption>();
        [Tooltip("是否是将要结束的对话")] public bool isEnd;
        [HideInInspector] public bool canExpend;    // 是否可以展开对话
        [HideInInspector] public bool canExpendOption;    // 是否可以展开选项
    }// class DialoguePiece
    
    // 任务目标相关
    [Serializable]
    public class QuestRequirement
    {
        [Header("需求信息")]
        [Tooltip("目标名字")] public string name;
        [Tooltip("目标所需数量")] public int requireAmount;
        [Tooltip("已经完成数量")] public int currentAmount;

    }// class QuestRequirement

    // 为了保证本身的游戏数据中的模板QuestDataSo不会被修改, 只修改QuestTask中的QuestDataSo
    [Serializable]
    public class QuestTask
    {
        [Tooltip("任务数据")] public QuestDataSo questDataSo;
        public bool IsStarted
        {
            get => questDataSo.isStarted;
            set => questDataSo.isStarted = value;
        }

        public bool IsComplete
        {
            get => questDataSo.isComplete;
            set => questDataSo.isComplete = value;
        }

        public bool IsFinished
        {
            get => questDataSo.isFinished;
            set => questDataSo.isFinished = value;
        }
    }
    
}// namespace Utils
