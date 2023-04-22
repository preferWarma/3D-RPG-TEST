using UnityEngine;
using Utils;
using CharacterStatics.MonBehavior;
using UnityEngine.AI;

namespace Characters.Enemy
{
    public class StonemanController : EnemyController
    {
        [Header("Skill Attack")] 
        [Tooltip("击退的力的大小")] public float repelForce = 25;
        [Tooltip("生成的石头预制件")] public GameObject rock;
        [Tooltip("生成石头的位置")] public Transform generatePosition;
        
        private static readonly int Dizzy = Animator.StringToHash("Dizzy");
        private static readonly int Attack = Animator.StringToHash("SkillAttack");
        private static readonly int BaseAttack1 = Animator.StringToHash("BaseAttack");

        // Animation Event
        public void RepelTarget()   // 击退目标
        {
            if (AttackTarget == null) return;  // 防止攻击时玩家跑开导致空引用
            if (!transform.IsFacingTarget(AttackTarget.transform)) return;  // 如果不再前方扇形范围内，则视为不受伤害
            
            var targetStats = AttackTarget.GetComponent<CharacterStats>(); // 拿到敌人身上的数值
            var attackTargetAgent = AttackTarget.GetComponent<NavMeshAgent>();  // 拿到敌人的agent
            
            transform.LookAt(AttackTarget.transform);   // 朝向敌人
            // ReSharper disable once Unity.InefficientPropertyAccess
            var direction = (AttackTarget.transform.position - transform.position).normalized;  // 归一化后的方向
            attackTargetAgent.isStopped = true; // 先打断玩家移动
            attackTargetAgent.velocity = direction * repelForce;    // 击退
            AttackTarget.GetComponent<Animator>().SetTrigger(Dizzy);    // 让玩家眩晕
            targetStats.ReceiveDamage(MyCharacterStats, targetStats);    // 敌人受到伤害
        }
        
        // Animation Event
        public void ThrowRock() // 扔石头
        {
            if (AttackTarget == null)
            {
                AttackTarget = FindObjectOfType<PlayerController>().gameObject; // 尽量生成出来, 避免无效动画
            }
            Instantiate(rock, generatePosition.position, Quaternion.identity);   // 在手部生成一个石头
            var rockController = rock.GetComponent<RockController>();
            rockController.remainTime = rockController.lastTime;    // 初始化停留时间
            rockController.attackTarget = AttackTarget;    // 确定石头的飞向目标
        }

        protected override void DongingAttackTarget()
        {
            transform.LookAt(AttackTarget.transform);  // 转向攻击目标
            if (TargetInBaseAttackRange())
            {
                MyAnimator.SetTrigger(BaseAttack1);
            }
            else if (TargetInSkillRange())
            {
                MyAnimator.SetTrigger(Attack);
            }
        }
        
    }// class StonemanController
}// namespace StonemanController
