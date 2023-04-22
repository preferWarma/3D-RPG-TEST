using UnityEngine;

namespace Photographer
{
    public class CameraController : MonoBehaviour
    {
        [Header("目标")] 
        public Transform lookAtTarget;

        [Header("移动和旋转速度")] 
        [Tooltip("缓动插值/秒")] public float lerp = 5;
        [Tooltip("旋转速度:角度/秒")] public float rotateSpeed = 360;

        [Header("视距缩放")] 
        [Tooltip("默认摄像机缩放距离")] public float defaultZoom = 10;
        [Tooltip("最小缩放距离")] public float minZoom = 2;
        [Tooltip("最大缩放距离")] public float maxZoom = 20;
        [Tooltip("缩放速度")] public float zoomSpeed = 8;

        private float _currentZoom; // 当前缩放
        private float _rotateYaw; // 上下旋转
        private Vector3 _direction; // 目标相对摄像机的方向
        private Vector3 _offsetPosition; // 偏移位置

        private void Start()
        {
            var myTransform = transform; // 摄像机的位置信息
            var lookAtPosition = lookAtTarget.position; // 朝向的位置
            
            _currentZoom = defaultZoom; // 缩放距离改为默认
            _direction = myTransform.position - lookAtPosition; //目标相对摄像机的方向
            myTransform.position = lookAtPosition + _direction.normalized * defaultZoom;    //设定默认位置
            transform.LookAt(lookAtPosition);   // 设定默认朝向
        }

        private void Update()
        {
            //更新滚轮缩放
            _currentZoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
            _currentZoom = Mathf.Clamp(_currentZoom, minZoom, maxZoom);
            
            if (Input.GetMouseButton(1))    //按下鼠标右键旋转视角
            {
                _rotateYaw += Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime;
            }

            //计算出来当前Yaw轴需要增加旋转的角度
            var rot = Quaternion.AngleAxis(_rotateYaw, Vector3.up);
            //旋转，缩放后新的偏移位置
            _offsetPosition =   // 使用插值
                Vector3.Slerp(_offsetPosition, rot * (_direction.normalized * _currentZoom), Time.deltaTime * lerp);
        }
        
        private void LateUpdate()   // 更新位置和朝向，必须在LateUpdate里，不然会莫名抖动
        {
            var lookAtPosition = lookAtTarget.position;
            transform.position = lookAtPosition + _offsetPosition;
            transform.LookAt(lookAtPosition);
        }
    }
}