using UnityEngine;
using Utils;
using Utils.Extension;

namespace Transition
{
    public class TransitionPoint : MonoBehaviour
    {
        [Header("传送信息")] 
        [Label("要传送的场景名字")][Tooltip("若为同场景传送则为空")] public string destinationSceneName;
        public Enums.TransitionType myTransitionType; // 传送门类型
        public Enums.DestinationTag destinationTag; // 终点标签

        private bool _canTrans; // 是否可以传送

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E) && _canTrans)
            {
                // 传送
                SceneController.Instance.TransitionToDestination(this);
            }
        }

        private void OnTriggerStay(Collider other)  // 碰撞过程中调用
        {
            if (other.CompareTag("Player"))
            {
                _canTrans = true;
            }
        }

        private void OnTriggerExit(Collider other)  // 碰撞结束时调用
        {
            _canTrans = false;
        }
        
    }// class TransitionPoint
    
}// namespace Transition
