using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Quest.UI
{
    public class QuestRequirementUI : MonoBehaviour
    {
        private Text requirementName;   // 自身Text对象
        private Text progress;  // 进度文本对象
        
        private void Awake()
        {
            requirementName = GetComponent<Text>();
            progress = transform.GetChild(0).GetComponent<Text>();
        }
        
        public void SetUpRequirement(QuestRequirement requirement, bool isFinished)
        {
            requirementName.text = requirement.name;
            progress.text = isFinished ? "已完成" : requirement.currentAmount + " / " + requirement.requireAmount;;
        }
        
    }// class QuestRequirementUI
}// namespace Quest.UI
