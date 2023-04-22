using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Utils.Extension
{
    public class Controller : MonoBehaviour  // MVC中的Controller, 根据业务需要来调用View层的方法
    {
        private PanelUI _panelUI;

        private void Awake()
        {
            _panelUI = GetComponent<PanelUI>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))    // Logic
            {
                _panelUI.UpdateData("Hello World", null, new List<Button>());
            }
        }
    }
}