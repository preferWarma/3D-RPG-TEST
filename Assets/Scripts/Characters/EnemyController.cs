using System;
using CharacterStatics.MonBehavior;
using Inventory.Item;
using Manager;
using Quest.Logic;
using UnityEngine;
using UnityEngine.AI;
using Utils;
using Random = UnityEngine.Random;

namespace Characters
{
    [RequireComponent(typeof(NavMeshAgent))]    // 确保NavMeshAgent存在
    [RequireComponent(typeof(CharacterStats))]  // 确保CharacterStats存在
    public class EnemyController : MonoBehaviour, IEndGameObserver
    {
        [Header("基础设置")] 
        [Tooltip("可视范围")] public float sightRadius;
        [Tooltip("是否为站桩敌人")] public bool isGuard;
        [Tooltip("观察时间")] public float lookAtTime;

        [Header("巡逻状态设置")]
        [Tooltip("巡逻范围")] public float patrolRange;        

        private NavMeshAgent _agent;    // 导航控制器
        protected Animator MyAnimator; // 动画控制器
        protected CharacterStats MyCharacterStats; // 基础角色数值组件
        private Collider _collider;   // 碰撞控制器
        private LootSpawner _lootSpawner;   // 掉落物品脚本组件
        private bool _isSpawner;    // 是否已经掉落了
        
        private Enums.EnemyStates _enemyStates;   // 敌人状态
        protected GameObject AttackTarget; // 敌人的攻击目标
        private float _speed;   // 追击玩家时的速度

        private Vector3 _randomPoint;  // 随机巡逻位置
        private Vector3 _initialPoint;  // 最初的坐标位置
        private Quaternion _initialLookAt; // 最初的朝向

        private float _remainLookAtTime;    // 剩余观察时间
        private float _remainAttackTime;    // 剩余攻击冷却时间
        
        // 配合动画切换的bool值
        private bool _isWalk; 
        private bool _isChase;  
        private bool _isFollow;
        private bool _isCritical;   // 是否暴击
        private bool _isDead;  // 是否死亡

        // 对应的索引值
        private static readonly int Walk = Animator.StringToHash("Walk");
        private static readonly int Chase = Animator.StringToHash("Chase");
        private static readonly int Follow = Animator.StringToHash("Follow");
        private static readonly int BaseAttack = Animator.StringToHash("BaseAttack");
        private static readonly int SkillAttack = Animator.StringToHash("SkillAttack");
        private static readonly int Critical = Animator.StringToHash("Critical");
        private static readonly int Death = Animator.StringToHash("Death");
        private static readonly int Victory = Animator.StringToHash("Victory");

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();  // 获取导航控制器
            MyAnimator = GetComponent<Animator>();   // 获取动画控制器
            MyCharacterStats = GetComponent<CharacterStats>();   // 获取数值相关信息
            _collider = GetComponent<Collider>(); // 获取碰撞控制器
            _lootSpawner = GetComponent<LootSpawner>(); //获取物品掉落组件
            
            _speed = _agent.speed;  // 获取追击速度
            _initialPoint = transform.position; // 获取初始位置
            // ReSharper disable once Unity.InefficientPropertyAccess
            _initialLookAt = transform.rotation;    // 获取初始位置
            _remainLookAtTime = lookAtTime; // 获取初始化的观察时间
        }

        private void Start()
        {
            if (isGuard)
            {
                _enemyStates = Enums.EnemyStates.Guard; // 站桩怪
            }
            else
            {
                _enemyStates = Enums.EnemyStates.Patrol; // 巡逻怪
                GetNewRandomPoint();    // 得到一个初始值
            }
        }
        
        private void Update()
        {
            if (MyCharacterStats.CurrentHealth <= 0) // 死亡
            {
                _isDead = true;
            }
            if(GameManager.Instance.IsPlayerDead)   return; // 如果玩家死亡就不再切换了
            SwitchStates();
            SwitchAnimation();
            _remainAttackTime -= Time.deltaTime;
        }

        private void OnEnable()
        {
            GameManager.Instance.AddObserver(this);
        }

        private void OnDisable()
        {
            if (!GameManager.IsInitialized || !_isDead) return;   // 避免Instance在OnDestroy中提前销毁导致空引用报错
            GameManager.Instance.RemoveObserver(this);
            if (!QuestManager.IsInitialized || !_isDead) return;
            QuestManager.Instance.CheckQuestProgress(name, 1);  // 死亡时记录
        }

        private void SwitchStates() // 切换状态
        {
            if (_isDead)
            {
                _enemyStates = Enums.EnemyStates.Dead;  // 死亡状态优先级最高
            }
            else if (FindPlayer())   // 如果发现玩家，切换到Chase状态
            {
                _enemyStates = Enums.EnemyStates.Chase;
            }
            
            switch (_enemyStates)
            {
                case Enums.EnemyStates.Guard:   // 站桩
                    DoingWhileGuard();
                    break;
                case Enums.EnemyStates.Patrol:  // 巡逻
                    DoingWhilePortal();
                    break;
                case Enums.EnemyStates.Chase:   // 追逐
                    DoingWhileChase();
                    break;
                case Enums.EnemyStates.Dead:    // 死亡
                    DoingWhileDead();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SwitchAnimation()  // 切换动画
        {
            MyAnimator.SetBool(Walk, _isWalk);
            MyAnimator.SetBool(Chase, _isChase);
            MyAnimator.SetBool(Follow, _isFollow);
            MyAnimator.SetBool(Critical, _isCritical);
            MyAnimator.SetBool(Death, _isDead);

        }
        
        private bool FindPlayer()   // 查找可视范围是否存在玩家
        {
            // ReSharper disable once Unity.PreferNonAllocApi
            var colliders = Physics.OverlapSphere(transform.position, sightRadius);
            foreach (var target in colliders)
            {
                if (target.CompareTag("Player"))
                {
                    AttackTarget = target.gameObject;
                    _remainLookAtTime = lookAtTime;
                    return true;
                }
            }

            AttackTarget = null;
            
            return false;
        }

        private void DoingWhileGuard()  // 守卫
        {
            _isChase = false;
            if (transform.position != _initialPoint)    // 当前位置不等于初始位置
            {
                _isWalk = true;
                _agent.isStopped = false;
                _agent.destination = _initialPoint; // 回到初始位置
                if (Vector3.Distance(_initialPoint, transform.position) <= _agent.stoppingDistance) // 到达
                {
                    _isWalk = false;
                    transform.rotation = Quaternion.Lerp(transform.rotation, _initialLookAt, 0.01f); // 转回原有的朝向
                }
            }
        }

        private void DoingWhilePortal() // 巡逻
        {
            _isChase = false;   // 退出追逐动画播放的状态
            _agent.speed = _speed * 0.5f;   // 巡逻状态下速度为追逐的一半
            
            if (Vector3.Distance(transform.position, _randomPoint) <= _agent.stoppingDistance)  // 已经到了巡逻点
            {
                _isWalk = false;
                if (_remainLookAtTime > 0)  // 到达巡逻点后在此处观察一段时间
                {
                    _remainLookAtTime -= Time.deltaTime;
                }
                GetNewRandomPoint();    // 寻找下一个随即巡逻点
                _remainLookAtTime = lookAtTime; // 重新设置观察停留的时间
            }
            else
            {
                _isWalk = true;
                _agent.destination = _randomPoint;  // 去往下一个随即巡逻点
            }
        }

        private void DoingWhileChase()  // 追逐
        {
            _agent.speed = _speed;
            _isWalk = false;
            _isChase = true;

            if (!FindPlayer())  // 找不到玩家，脱战
            {
                _isFollow = false;  // 脱战后不再播放追逐动画
                
                if (_remainLookAtTime > 0)  // 脱战后也观察一段时间
                {
                    _agent.destination = transform.position;    // 脱战后不再追逐
                    _remainLookAtTime -= Time.deltaTime;
                }
                else // 结束了观察时间，切换状态, 重置观察时间
                {
                    _enemyStates = isGuard ? Enums.EnemyStates.Guard : Enums.EnemyStates.Patrol;
                    _remainLookAtTime = lookAtTime;
                }
            }
            else // 找得到玩家
            {
                _isFollow = true;   // 可以找到玩家，切换到追逐动画
                _agent.isStopped = false;   // 保证可以追玩家了
                _agent.destination = AttackTarget.transform.position;  // 追逐Player
            }
            
            if (TargetInBaseAttackRange() || TargetInSkillRange())  // 如果在攻击范围内
            {
                _isFollow = false;
                _agent.isStopped = true;
                if (_remainAttackTime < 0)  // 攻击冷却时间结束了
                {
                    _remainAttackTime = MyCharacterStats.CoolDown;   // 重置冷却时间
                    _isCritical = Random.value <= MyCharacterStats.CriticalChance;  // 是否暴击
                    MyCharacterStats.isCritical = _isCritical;   // 同步更新
                    DongingAttackTarget(); // 执行攻击
                }
            }
        }

        private void DoingWhileDead()   // 死亡
        {
            _collider.enabled = false;   // 关闭碰撞
            // _agent.enabled = false; // 直接关闭导航可能会导致动画种获取的组件报错
            _agent.radius = 0;  // 可以达到关闭的效果
            
            if (_lootSpawner && !_isSpawner)   // 如果掉落组件存在并且没有掉落
            {
                _lootSpawner.SpawnLoot();   // 概率掉落
                _isSpawner = true;
            }
            Destroy(gameObject, 2f);    // 销毁对象
        }

        private void OnDrawGizmosSelected() // 画出可使范围和巡逻范围
        {
            Gizmos.color = Color.red;
            var position = transform.position;
            Gizmos.DrawWireSphere(position, sightRadius);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(position, patrolRange);
        }

        private void GetNewRandomPoint()    // 获取随机巡逻坐标
        {
            var randomX = Random.Range(-patrolRange, patrolRange);
            var randomZ = Random.Range(-patrolRange, patrolRange);
            var randomPoint = new Vector3(_initialPoint.x + randomX, transform.position.y, _initialPoint.z + randomZ);
            // 基于NavMesh, 寻找randomPoint在patrolRange范围内最近的walkable的点, 如果周围都是not walkable, 则返回false
            var flag = NavMesh.SamplePosition(randomPoint, out var hit, patrolRange, 1);
            _randomPoint = flag ? hit.position : transform.position;    // 保证不会走到not walkable的点
        }

        protected bool TargetInBaseAttackRange()  // 目标是否在基础攻击范围内
        {
            if (!AttackTarget) return false;
            return Vector3.Distance(AttackTarget.transform.position, transform.position) <=
                   MyCharacterStats.BaseAttackRange;
        }

        protected bool TargetInSkillRange()   // 目标是否在技能攻击范围内
        {
            if (!AttackTarget) return false;

            return Vector3.Distance(AttackTarget.transform.position, transform.position) <=
                   MyCharacterStats.SkillAttackRange;
        }

        protected virtual void DongingAttackTarget() // 执行攻击
        {
            transform.LookAt(AttackTarget.transform);  // 转向攻击目标
            if (TargetInSkillRange())
            {
                MyAnimator.SetTrigger(SkillAttack);
            }
            else if (TargetInBaseAttackRange())
            {
                MyAnimator.SetTrigger(BaseAttack);
            }
        }

        // Animation Event
        // ReSharper disable once UnusedMember.Local
        private void Hit()
        {
            if (AttackTarget == null) return;  // 防止攻击时玩家跑开导致空引用
            if (!transform.IsFacingTarget(AttackTarget.transform)) return;  // 如果不再前方扇形范围内，则视为不受伤害
            
            var targetStats = AttackTarget.GetComponent<CharacterStats>(); // 拿到敌人身上的数值
            targetStats.ReceiveDamage(MyCharacterStats, targetStats);    // 敌人受到伤害
        }
        
        public void EndNotify() // 游戏结束时需要做的事情
        {
            if (MyAnimator)
                MyAnimator.SetBool(Victory, true);   // 播放获胜动画
            _isChase = false;
            _isWalk = false;
            _isFollow = false;
            AttackTarget = null;
        }
    }// class EnemyController
    
}// namespace Characters
