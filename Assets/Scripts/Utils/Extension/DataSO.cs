using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Utils.Extension
{
    [SerializeField]
    public class DataSo : ScriptableObject  // MVC中的Model, 1. 定义和存储数据，并提供数据的更新方法
    {
        /// <summary>
        /// 数据本身
        /// </summary>
        public Text text;
        public Image image;
        public List<Button> buttons;
        
        public void SetData(string textData, Sprite imageSprite, List<Button> buttonList)   // 告诉它怎么更新
        {
            text.text = textData;
            image.sprite = imageSprite;
            buttons = buttonList;
        }


    }
}