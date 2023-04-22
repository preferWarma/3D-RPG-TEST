using DG.Tweening;
using Dialogue.Logic;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Dialogue.UI
{
    public class DialogueUI : Singleton<DialogueUI>
    {
        [Header("基本属性")] 
        [Tooltip("对话图片")] public Image icon;
        [Tooltip("文本")] public Text mainText;
        [Tooltip("按钮")] public Button nextButton;
        [Tooltip("对话面板对象")] public GameObject panel;

        [Header("选项信息")] 
        [Tooltip("选项相关panel")] public RectTransform optionPanel;
        [Tooltip("选项的预制件")] public OptionUI optionPrefab;

        [Header("数据信息")] 
        [Tooltip("对话数据")] public DialogueDataSo dialogueDataSo;

        public int currentDialogueIndex;   // 当前的对话索引
        
        protected override void Awake()
        {
            base.Awake();
            nextButton.onClick.AddListener(ContinueDialogue);
        }

        public void SetDialogueDataSo(DialogueDataSo other)
        {
            dialogueDataSo = other;
            currentDialogueIndex = 0;   // 保证改变对话数据集后每次都是从第一条对话开始
        }

        public void UpdateMainDialogue(DialoguePiece piece) // 参数为当前要播放的对话语句
        {
            panel.SetActive(true);  // 启动UI
            if (piece.sprite)
            {
                icon.enabled = true;
                icon.sprite = piece.sprite;
            }
            else
            {
                icon.enabled = false;
            }
            
            // mainText.text = piece.text; // 当前文本替换为对话信息
            mainText.text = ""; // 先清空一下显示, 以免在打字效果之前直接显示了文本
            var duration = piece.text.Length * 0.1f;   // 打字速度: 0.1s/个字
            mainText.DOText(piece.text, duration); // 使用DOTWeen插件，使得文字可以以打字的方式显示出来

            if (piece.dialogueOptions.Count == 0 && !piece.isEnd) // 没有玩家的对话选项并且不等于最后一条对话
            {
                nextButton.gameObject.SetActive(true);  // 则开启next选项
                currentDialogueIndex++; // 索引指向下一条
                
                nextButton.interactable = true; // 开启点按效果
                nextButton.transform.GetChild(0).gameObject.SetActive(true);   // 开启文字显示
            }
            else
            {
                nextButton.interactable = false;    // 取消点按
                nextButton.transform.GetChild(0).gameObject.SetActive(false);   // 关闭文字显示
            }
            CreateOptions(piece);   // 创建option
        }

        private void CreateOptions(DialoguePiece piece) // 创建参数piece的选项对象
        {
            if (optionPanel.childCount > 0) // 如果有则先销毁
            {
                for (var i = 0; i < optionPanel.childCount; i++)
                {
                    Destroy(optionPanel.GetChild(i).gameObject);
                }
            }
            
            foreach (var t in piece.dialogueOptions)    // 再生成该对话的选项
            {
                var option = Instantiate(optionPrefab, optionPanel);
                option.SetOption(piece, t);
            }
        }

        private void ContinueDialogue() // next选项绑定的函数
        {
            UpdateMainDialogue(dialogueDataSo.dialoguePieces[currentDialogueIndex]);
        }

    } // class DialogueUI
} // namespace Dialogue.UI
