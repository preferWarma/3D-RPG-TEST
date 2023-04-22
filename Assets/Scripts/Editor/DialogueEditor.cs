using System.Collections.Generic;
using Dialogue.Logic;
using Quest.Logic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEditorInternal;
using Utils;
using System.IO;

namespace Editor
{
    public class DialogueEditor : EditorWindow
    {
        private DialogueDataSo _currentDataSo;  // 被编辑的对话数据
        private ReorderableList _pieceList;  // 对话列表
        private readonly Dictionary<string, ReorderableList> _optionDictionary = new Dictionary<string, ReorderableList>(); // 选项列表(string 是对话ID, ReorderableList是选项列表)
        private Vector2 scrollPos = Vector2.zero;  // 滚动条位置

        [MenuItem("LYF/Dialogue Editor")]   // 增加菜单选项
        public static void Init()
        {
            var editorWindow = GetWindow<DialogueEditor>("Dialogue"); // 生成窗口, 窗口名字叫Dialogue
            editorWindow.autoRepaintOnSceneChange = true;   // 场景变换的时候重新绘制
        }

        public static void InitWindow(DialogueDataSo dialogueDataSo)
        {
            var editorWindow = GetWindow<DialogueEditor>("Dialogue"); // 生成窗口, 窗口名字叫Dialogue
            editorWindow._currentDataSo = dialogueDataSo;
        }

        [OnOpenAsset]
        public static bool OpenAsset(int instanceID, int line)  // 使得双击可以打开编辑窗口
        {
            var dataSo = EditorUtility.InstanceIDToObject(instanceID) as DialogueDataSo;
            if (!dataSo) return false;
            InitWindow(dataSo);
            return true;
        }

        private void OnSelectionChange()    // 选择变化的时候重新绘制
        {
            var dataSo = Selection.activeObject as DialogueDataSo;
            if (dataSo)
            {
                _currentDataSo = dataSo;
                SetupReorderableList(); // 重新绘制列表
            }
            else
            {
                _pieceList = null;
                _optionDictionary.Clear();
            }
            Repaint();  // 重新绘制
        }

        private void OnGUI()    // 绘制窗口
        {
            if (_currentDataSo)
            {
                EditorGUILayout.LabelField(_currentDataSo.name, EditorStyles.boldLabel); // 黑体绘制一个文字框来显示当前数据名字
                GUILayout.Space(10); // 间隔一段距离
                
                scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)); // 开始绘制滚动条
                
                if (_pieceList == null)
                {
                    SetupReorderableList();
                }

                _pieceList!.DoLayoutList();
                GUILayout.EndScrollView();  // 结束绘制滚动条
            }
            else
            {
                if (GUILayout.Button("创建新的对话数据"))
                {
                    const string dataPath = "Assets/GameData/DialogueData/"; // 路径
                    if (!Directory.Exists(dataPath))    // 如果路径不存在
                    {
                        Directory.CreateDirectory(dataPath); // 创建路径
                    }
                    const string dataName = "New Dialogue Data"; // 名字
                    var newDialogueDataSo = CreateInstance<DialogueDataSo>(); // 创建一个新的数据
                    AssetDatabase.CreateAsset(newDialogueDataSo, dataPath + dataName + ".asset"); // 创建一个新的asset
                    _currentDataSo = newDialogueDataSo;
                }
                GUILayout.Label("NO DATA SELECT !", EditorStyles.boldLabel);    // 数据为空则显示
            }
        }

        private void OnDisable()    // 禁用的时候清空选项列表
        {
            _optionDictionary.Clear();  // 禁用的时候清空选项列表
        }

        private void SetupReorderableList() // 新建一个列表
        {
            _pieceList = new ReorderableList(_currentDataSo.dialoguePieces, typeof(DialoguePiece),
                true,true,true,true);   // 增加拖拽, 显示名字，添加按钮，删除按钮
            _pieceList.drawHeaderCallback += OnDrawPieceHeader; // 绘制标题
            _pieceList.drawElementCallback += OnDrawPieceElement;   // 绘制元素
            _pieceList.elementHeightCallback += OntHeightChanged; // 元素高度变化
        }

        private float OntHeightChanged(int index)   // 元素高度变化
        {
            if (!_currentDataSo.dialoguePieces[index].canExpend) return EditorGUIUtility.singleLineHeight;
            
            var height = EditorGUIUtility.singleLineHeight * 9;
            var options = _currentDataSo.dialoguePieces[index].dialogueOptions;
           
            if (options.Count > 0 && _currentDataSo.dialoguePieces[index].canExpendOption)  // 有选项且处于展开状态
            {
                height += EditorGUIUtility.singleLineHeight * 3 + 5;
                height += (EditorGUIUtility.singleLineHeight + 5) * options.Count;
            }
            if (options.Count == 0 && _currentDataSo.dialoguePieces[index].canExpendOption) // 没有选项的时候且现在是展开状态
            {
                height += EditorGUIUtility.singleLineHeight * 4 + 5;
            }
            return height;
        }

        private void OnDrawPieceElement(Rect rect, int index, bool isActive, bool isFocused)    // 绘制元素
        {
            EditorUtility.SetDirty(_currentDataSo); // 使得数据可以实时保存
            
            if (index >= _currentDataSo.dialoguePieces.Count) return; // 绘制数量应该等于对话列表的元素个数
            
            var textStyle = new GUIStyle("TextArea"); // 设置一个文本框的样式
            var piece = _currentDataSo.dialoguePieces[index];
            var tmpRect = rect;
            tmpRect.height = EditorGUIUtility.singleLineHeight; // 设置高度为单行高度
            
            // 绘制一个折叠框
            piece.canExpend = EditorGUI.Foldout(tmpRect,piece.canExpend, piece.id);
            if (!piece.canExpend) return; // 如果折叠了就不绘制下面的内容
            tmpRect.y += tmpRect.height + 5; // 下移换行
            
            // 绘制一个ID相关
            tmpRect.width = 30; // 设置宽度
            EditorGUI.LabelField(tmpRect, "ID"); // 绘制每个元素的ID
            tmpRect.x += tmpRect.width; // 右移
            tmpRect.width = 100;
            piece.id = EditorGUI.TextField(tmpRect, piece.id); // 在ID后面绘制一个ID的显示和编辑框
            
            
            // 绘制Quest相关
            tmpRect.x += tmpRect.width + 50;
            EditorGUI.LabelField(tmpRect, "Quest"); // 绘制一个Quest的Label
            tmpRect.x += 45;
            tmpRect.width = 200;
            piece.receiveQuest = EditorGUI.ObjectField(tmpRect, piece.receiveQuest, // 绘制一个Quest的显示和编辑框
                typeof(QuestDataSo), false) as QuestDataSo; // allowSceneObjects = false的意思是不允许场景中的对象
            
            // 换行
            tmpRect.y += tmpRect.height + 5; // 下移换行
            tmpRect.x = rect.x; // 换行后x坐标回到最左边
            
           // 绘制Spire相关
            EditorGUI.LabelField(tmpRect, "Spire"); // 绘制一个Quest的Label
            tmpRect.x += 45;
            tmpRect.height = 60;
            tmpRect.width = 60;
            piece.sprite = EditorGUI.ObjectField(tmpRect, piece.sprite, typeof(Sprite), false) as Sprite;   // 绘制一个Sprite的显示和编辑框
            

            // 绘制Text相关
            tmpRect.x += 75;
            tmpRect.width = rect.width * 0.65f;
            textStyle.wordWrap = true;  // 设置文本框自动换行
            piece.text = EditorGUI.TextField(tmpRect, piece.text, textStyle); // 绘制一个Text的显示和编辑框
        
            // 换行
            tmpRect.y += tmpRect.height + 5; // 下移换行
            tmpRect.x = rect.x; // 换行后x坐标回到最左边

            // 绘制isEnd相关
            tmpRect.height = EditorGUIUtility.singleLineHeight;
            piece.isEnd = EditorGUI.Toggle(tmpRect, "isEnd (是否结束对话)", piece.isEnd); // 绘制一个Is Last的显示和编辑框
            
            // 换行
            tmpRect.y += tmpRect.height + 5; // 下移换行
            tmpRect.x = rect.x; // 换行后x坐标回到最左边

            // 绘制选项相关
            tmpRect.width = rect.width;
            tmpRect.height = EditorGUIUtility.singleLineHeight;
            var optionListKey = piece.id + piece.text; // 选项列表的key是对话ID和对话内容的组合
            
            // 为选项列表绘制一个折叠框
            piece.canExpendOption = EditorGUI.Foldout(tmpRect, piece.canExpendOption, "Options (选项)");
            if (!piece.canExpendOption) return; // 如果折叠了就不绘制下面的内容
            tmpRect.y += tmpRect.height + 5; // 下移换行
            
            if (optionListKey == string.Empty) return;

            if (!_optionDictionary.ContainsKey(optionListKey))
            {
                var optionList = new ReorderableList(piece.dialogueOptions, typeof(DialogueOption), true, true, true, true)
                {
                    drawElementCallback = (optionRect, optionIndex, optionActive, optionOnFocused) =>
                    {
                        OnDrawOptionElement(piece, optionRect, optionIndex, optionActive, optionOnFocused, piece);
                    },
                    drawHeaderCallback = (optionRect) =>
                    {
                        OnDrawOptionHeader(optionRect, piece);
                    }
                };  // 新建一个选项列表

                _optionDictionary[optionListKey] = optionList;  // 添加到字典中
                    
            }
            _optionDictionary[optionListKey].DoList(tmpRect); // 绘制选项列表
        }

        private static void OnDrawOptionHeader(Rect optionRect, DialoguePiece piece)
        {
            // 三个label分别对应element
            var tmpRect = optionRect;
            
            tmpRect.width = optionRect.width * 0.5f;
            EditorGUI.LabelField(tmpRect, "选项内容");
            
            tmpRect.x += tmpRect.width + optionRect.width * 0.05f;
            tmpRect.width = optionRect.width * 0.3f;
            EditorGUI.LabelField(tmpRect, "目标ID");
            
            tmpRect.x += tmpRect.width + optionRect.width * 0.05f;
            tmpRect.width = optionRect.width * 0.2f;
            EditorGUI.LabelField(tmpRect, "任务");
            
        }

        private static void OnDrawOptionElement(DialoguePiece piece, Rect optionRect, int optionIndex, bool optionActive, bool optionOnFocused, DialoguePiece dialoguePiece)
        {
            var currentOption = piece.dialogueOptions[optionIndex]; // 当前选项
            var tmpRect = optionRect;

            // 绘制选项内容
            tmpRect.width = optionRect.width * 0.5f;
            currentOption.text = EditorGUI.TextField(tmpRect, currentOption.text);

            // 绘制选项对应的对话
            tmpRect.x += tmpRect.width + optionRect.width * 0.05f;
            tmpRect.width = optionRect.width * 0.3f;
            currentOption.targetID = EditorGUI.TextField(tmpRect, currentOption.targetID);

            // 绘制是否接受任务
            tmpRect.x += tmpRect.width + optionRect.width * 0.05f;
            tmpRect.width = optionRect.width * 0.1f;
            currentOption.takeQuest = EditorGUI.Toggle(tmpRect, currentOption.takeQuest);

        }


        private static void OnDrawPieceHeader(Rect rect)
        {
            GUI.Label(rect, "Dialogue Pieces"); // 绘制标题栏
        }
    }// class DialogueEditor
} // namespace Editor
