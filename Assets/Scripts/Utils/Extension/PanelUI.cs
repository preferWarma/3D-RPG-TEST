using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Utils.Extension
{
    // View层的脚本
    public class PanelUI : MonoBehaviour // MVC中的View, 为Controller提供管理UI的方法
    {
        public DataSo dataSo;

        private void Awake()
        {
            dataSo.buttons[0].onClick.AddListener(OnClicked);
        }

        private void Start()
        {
            dataSo.SetData("Hello World", null, new List<Button>());
        }
        
        public void UpdateData(string textData, Sprite imageSprite, List<Button> buttonList)
        {
            dataSo.SetData(textData, imageSprite, buttonList);
        }
        
        private static void OnClicked()
        {
            Debug.Log("On Clicked");
        }
    }
}