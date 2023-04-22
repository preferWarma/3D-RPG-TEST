using UnityEngine;
using UnityEngine.EventSystems;
using Utils;
using EventHandle = Utils.EventHandle;

namespace Manager   // 控件命名空间
{
    public class MouseManager : Singleton<MouseManager>
    {
        private RaycastHit _hitInfo;  // 碰撞射线
        private Camera _mainCamera; // 主摄像机
        public Texture2D point, doorWay, attack, target, arrow;   // 鼠标图案: 鼠标，传送门，攻击，选中目标，箭头

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this);    // 加载场景的时候不会被删除
        }

        private void Update()
        {
            SetCursorTexture();
            if (!IsClickedUI()) // 如果点击的不是UI
            {
                MouseControl();
            }
        }

        private void SetCursorTexture() // 设置指针贴图
        {
            // ReSharper disable once SuggestVarOrType_SimpleTypes
            _mainCamera = Camera.main;
            var ray = _mainCamera!.ScreenPointToRay(Input.mousePosition);    // 从摄像机发射到鼠标位置的射线
            if (!Physics.Raycast(ray, out _hitInfo)) return; // 如果没有射线碰撞到物体就返回，HitInfo为射线碰撞返回的信息体

            if (IsClickedUI())  // 如果点击的是UI
            {
                Cursor.SetCursor(arrow, Vector2.zero, CursorMode.Auto);  // 对UI执行同一个指针
                return;
            }
            // 切换鼠标贴图
            switch (_hitInfo.collider.gameObject.tag)   // 根据射线碰撞体的标签来判断
            {
                case "Ground":
                    Cursor.SetCursor(target, new Vector2(16,16), CursorMode.Auto);
                    break;
                case "Enemy":
                    Cursor.SetCursor(attack, new Vector2(16,16), CursorMode.Auto);
                    break;
                case "CanAttack":
                    Cursor.SetCursor(point, new Vector2(16,16), CursorMode.Auto);
                    break;
                case "Portal":
                    Cursor.SetCursor(doorWay, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Item":
                    Cursor.SetCursor(point, new Vector2(16,16), CursorMode.Auto);
                    break;
                default:
                    Cursor.SetCursor(arrow, new Vector2(16, 16), CursorMode.Auto);
                    break;
            }
        }

        private void MouseControl()
        {
            if (!Input.GetMouseButtonDown(0) || !_hitInfo.collider) return;
            
            // 点击鼠标左键(0)，并且射线的碰撞体不为空
            if (_hitInfo.collider.gameObject.CompareTag("Ground"))   // 碰撞体标签为地面
            {
                EventHandle.OnMouseClicked(_hitInfo.point); // 将射线碰撞位置传给鼠标点击事件
            }
            if (_hitInfo.collider.gameObject.CompareTag("Enemy"))   // 碰撞体标签为敌人
            {
                EventHandle.OnEnemyClicked(_hitInfo.collider.gameObject); // 将射线碰撞体传给点击敌人事件
            }
            // ReSharper disable once Unity.UnknownTag
            if (_hitInfo.collider.gameObject.CompareTag("CanAttack"))   // 碰撞体标签为可攻击物体
            {
                EventHandle.OnEnemyClicked(_hitInfo.collider.gameObject); // 将射线碰撞体传给点击事件
            }
            if (_hitInfo.collider.gameObject.CompareTag("Portal"))   // 碰撞体标签为传送门
            {
                EventHandle.OnMouseClicked(_hitInfo.point); // 将射线碰撞体传给点击事件
            }
            if (_hitInfo.collider.gameObject.CompareTag("Item"))   // 碰撞体标签为物体
            {
                EventHandle.OnMouseClicked(_hitInfo.point); // 将射线碰撞体传给点击事件
            }
        }

        private bool IsClickedUI()  // 是否在和UI进行交互
        {
            return EventSystem.current && EventSystem.current.IsPointerOverGameObject();
        }
        
    }//class MouseManager
    
}//namespace Manager