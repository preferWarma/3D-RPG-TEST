using System;
using UnityEngine;

namespace Utils
{
    public static class EventHandle
    {
        public static event Action<Vector3> MouseClicked; // 鼠标点击事件,vector3在这里指代鼠标点击的位置
        public static void OnMouseClicked(Vector3 obj)
        {
            MouseClicked?.Invoke(obj);
        }

        public static event Action<GameObject> EnemyClicked;    // 鼠标点击敌人事件, GameObject指代敌人

        public static void OnEnemyClicked(GameObject obj)
        {
            EnemyClicked?.Invoke(obj);
        }
        
    }// static class EventHandle
    
}// namespace Utils
