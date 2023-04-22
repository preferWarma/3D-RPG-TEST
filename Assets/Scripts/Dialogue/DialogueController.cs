using Dialogue.Logic;
using Dialogue.UI;
using UnityEngine;

namespace Dialogue
{
    public class DialogueController : MonoBehaviour
    {
        public DialogueDataSo currentDialogueDataSo;    // 当前对话信息
        private bool canTalk;   // 是否可以进行对话

        private void Update()
        {
            if (canTalk && Input.GetMouseButtonDown(1)) // 如果可以对话，且按下鼠标右键，则开启对话
            {
                OpenDialogue();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && currentDialogueDataSo)    // 玩家接近时
            {
                canTalk = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player") || !currentDialogueDataSo) return;
            
            DialogueUI.Instance.panel.SetActive(false);
            canTalk = false;
        }

        private void OpenDialogue() // 开启对话
        {
            DialogueUI.Instance.SetDialogueDataSo(currentDialogueDataSo);   // 传输对话内容信息 
            DialogueUI.Instance.UpdateMainDialogue(currentDialogueDataSo.dialoguePieces[0]);    // 开始第一条对话
        }
    } // class DialogueController
} // namespace Dialogue
