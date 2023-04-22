using Dialogue.Logic;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(DialogueDataSo))]  // 对“对话系统数据”生效
    public class DialogueCustomEditor : UnityEditor.Editor  // 用于改变inspector窗口的显示
    {
        public override void OnInspectorGUI()   // 在绘制inspector窗口的时候调用
        {
            if (GUILayout.Button("Open in Editor")) // 点击对应按钮
            {
                DialogueEditor.InitWindow(target as DialogueDataSo);    // 打开对应的编辑窗口
            }
            base.OnInspectorGUI(); 
        }
    }// DialogueCustomEditor
}// namespace Editor