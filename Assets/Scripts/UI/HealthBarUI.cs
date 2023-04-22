using CharacterStatics.MonBehavior;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HealthBarUI : MonoBehaviour
    {
        [Header("基础设置")] 
        [Tooltip("血条预制件")] public GameObject healthBarPrefab;
        [Tooltip("敌人头顶的位置")] public Transform healthBarPoint;
        [Tooltip("是否一直可见")] public bool alwaysVisible;
        [Tooltip("可视化时间")] public float lastVisibleTime;
        
        private Image _healthSlider;    // 滑动条组件
        private Transform _uiBarPos;   // 血条位置
        private Transform _cameraPosition;  // 摄像机位置
        private CharacterStats _characterStats; // 数值
        private float _lifeTime; // 剩余显示时间

        private void Awake()
        {
            _characterStats = GetComponent<CharacterStats>();
        }
        
        private void OnEnable()
        {
            _characterStats.UpdateHealthBarOnAttack += UpdateHealthBar;

            if (Camera.main != null) _cameraPosition = Camera.main.transform;   // 获取相机位置
            foreach (var canvas in FindObjectsOfType<Canvas>()) // 遍历每一个canvas
            {
                if (canvas.renderMode != RenderMode.WorldSpace) continue; // 世界坐标模式
                _uiBarPos = Instantiate(healthBarPrefab, canvas.transform).transform; // 生成血条并获取位置，且生成的物体为canvas的子物体
                _healthSlider = _uiBarPos.GetChild(0).GetComponent<Image>();    // 第一个子物体的Image组件
                _uiBarPos.gameObject.SetActive(alwaysVisible);  // 设置是否可见
            }
        }

        private void OnDisable()
        {
            _characterStats.UpdateHealthBarOnAttack -= UpdateHealthBar;
        }

        private void UpdateHealthBar(int currentHealth, int maxHealth)
        {
            if (currentHealth <= 0)
            {
                if (_uiBarPos && _uiBarPos.gameObject)  // 防止空引用报错
                    Destroy(_uiBarPos.gameObject); // 血量为0的时候就不需要显示血条了
                return;
            }
            _uiBarPos.gameObject.SetActive(true);   // 受到攻击时一定可见
            _lifeTime = lastVisibleTime;    // 设置生命周期
            var percent = 1.0f * currentHealth / maxHealth;    // 血量百分比
            _healthSlider.fillAmount = percent; // 更改UI显示
        }
        
        private void LateUpdate()
        {
            if (!_uiBarPos) return;  // 如果血条已经被销毁
            _uiBarPos.position = healthBarPoint.transform.position;    // 保持在敌人头顶上
            _uiBarPos.forward = -_cameraPosition.forward;   // 保证血条正对着摄像机
            if (_lifeTime <= 0 && !alwaysVisible)   // 如果不是一直显示并且显示时间到了
            {
                _uiBarPos.gameObject.SetActive(false);  //关闭显示
            }
            else if (!alwaysVisible)
            {
                _lifeTime -= Time.deltaTime;
            }
        }
    }// class HealthBarUI
}// namespace UI
