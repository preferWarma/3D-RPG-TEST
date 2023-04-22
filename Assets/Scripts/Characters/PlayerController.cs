using System.Collections;
using Characters.Enemy;
using CharacterStatics.MonBehavior;
using Manager;
using UnityEngine;
using UnityEngine.AI;
using Utils;
using Random = UnityEngine.Random;

namespace Characters
{
    public class PlayerController : MonoBehaviour
    {
        private NavMeshAgent _agent;    // 导航控制器
        private Collider _collider; // 碰撞控制器

        private GameObject _attackTarget;   // 攻击目标
        private float _nextAttackTime;  // 距离下次攻击还需要的时间
        private float _attackCd;    // 攻击的CD时间
        private bool _isCritical;   // 是否暴击
        private bool _isDead;   // 是否死亡
        private float _initialStoppingDistance; // 初始的停止距离
        
        private CharacterStats _characterStats; // 基础角色数值组件
        
        private Animator _animator; // 动画控制器
        private static readonly int Speed = Animator.StringToHash("Speed"); // 动画控制器中Speed的索引
        private static readonly int Attack = Animator.StringToHash("Attack");   // 动画控制器中Attack的索引
        private static readonly int Critical = Animator.StringToHash("Critical");   // 动画控制器中Critical的索引
        private static readonly int Death = Animator.StringToHash("Death"); // 动画控制器中Death的索引
        

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();  // 获取导航控制器
            _animator = GetComponent<Animator>();   // 获取动画控制器
            _characterStats = GetComponent<CharacterStats>();   // 获取数值信息
            _attackCd = _characterStats.CoolDown;   // 获取攻击CD时间
            _initialStoppingDistance = _agent.stoppingDistance; // 获取初始停止距离
        }

        private void Start()
        {
            SaveDataManager.Instance.LoadPlayerData();
        }

        private void OnEnable()
        {
            EventHandle.MouseClicked += EventMoveToTarget;   // 将移动到目标这个方法添加到鼠标点击的事件中去
            EventHandle.EnemyClicked += EventAttackEnemy;
            
            GameManager.Instance.RegisterPlayer(_characterStats);   // 注册玩家信息
        }
        
        private void OnDisable()
        {
            EventHandle.MouseClicked -= EventMoveToTarget;
            EventHandle.EnemyClicked -= EventAttackEnemy;
        }

        private void Update()
        {
            if (_characterStats.CurrentHealth <= 0)
            {
                _isDead = true;
                OnDisable();    // 死亡后取消订阅
            }
            SwitchAnimation();
            _nextAttackTime -= Time.deltaTime;
        }

        private void EventMoveToTarget(Vector3 targetPosition)    // 移动到鼠标点击位置位置(既OnMouseClicked事件的vector3参数)
        {
            StopAllCoroutines();   // 使得移动可以打断其他协程，如攻击等
            _agent.isStopped = false;   // 避免人物无法移动
            _agent.destination = targetPosition;    // 移到到目标点
            _agent.stoppingDistance = _initialStoppingDistance; // 恢复
        }
        
        private void EventAttackEnemy(GameObject enemy)    // 移动并且攻击敌人
        {
            if (!enemy) return;
            _attackTarget = enemy;
            StartCoroutine(MoveToAttackTarget());   // 开启协程
        }

        private IEnumerator MoveToAttackTarget()    // 协程, 移动到攻击目标前
        {
            _agent.isStopped = false;   // 保证人物可以移动
            
            transform.LookAt(_attackTarget.transform);  // 人物转向到敌人
            _agent.stoppingDistance = _characterStats.BaseAttackRange;  // 攻击的时候假设在攻击范围内就可以停止移动了
            
            while (Vector3.Distance(_attackTarget.transform.position, transform.position) > 
                   _characterStats.BaseAttackRange)  // 和敌人的距离大于攻击距离时
            {
                _agent.destination = _attackTarget.transform.position;  // 往敌人方向移动
                yield return null;
            }

            _agent.isStopped = true;    // 和敌人的距离在人物攻击距离内，停止移动

            if (_nextAttackTime < 0)  // 攻击冷却结束
            {
                _isCritical = Random.value <= _characterStats.CriticalChance;  // 是否暴击
                _characterStats.isCritical = _isCritical;   // 同步更新
                _animator.SetBool(Critical, _isCritical);   // 设置是否暴击
                _animator.SetTrigger(Attack);   // 设置攻击动画
                _nextAttackTime = _attackCd; // 攻击进入冷却
            }
        }

        private void SwitchAnimation()  // 切换动画
        {
            _animator.SetFloat(Speed, _agent.velocity.sqrMagnitude);
            _animator.SetBool(Death, _isDead);
        }
        
        // Animation Event
        // ReSharper disable once UnusedMember.Local
        private void Hit()
        {
            if (_attackTarget == null) return;
            // ReSharper disable once Unity.UnknownTag
            if (_attackTarget.CompareTag("CanAttack"))  // 如果是可攻击的物体
            {
                if (_attackTarget.GetComponent<RockController>() != null)   // 如果是石头
                {
                    var rockController = _attackTarget.GetComponent<RockController>();
                    rockController.rockStates = Enums.RockStates.AttackStoneman;    // 切换石头状态为攻击石头人
                    var rigidbodyComponent = _attackTarget.GetComponent<Rigidbody>();
                    rigidbodyComponent.velocity = Vector3.one;  // 避免速度太小更新为静止状态
                    rigidbodyComponent.AddForce(transform.forward * rockController.force, ForceMode.Impulse);   // 反击!
                }
            }
            else
            {
                var targetStats = _attackTarget.GetComponent<CharacterStats>(); // 拿到敌人身上的数值
                targetStats.ReceiveDamage(_characterStats, targetStats);    // 敌人受到伤害
            }
            
        }
        
    }// PlayerController
    
}//namespace Character
