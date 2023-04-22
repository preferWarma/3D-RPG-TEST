using UnityEngine;
using UnityEngine.AI;

namespace Characters.Enemy
{
    public class GoblinController : EnemyController
    {
        [Header("Skill Attack")] 
        [Tooltip("击退的力的大小")]public float repelForce = 10;

        private static readonly int Dizzy = Animator.StringToHash("Dizzy");

        // Animation Event
        public void RepelTarget()   // 击退
        {
            if (AttackTarget == null) return;
            transform.LookAt(AttackTarget.transform);
            // ReSharper disable once Unity.InefficientPropertyAccess
            var direction = AttackTarget.transform.position - transform.position;
            direction.Normalize();  // 归一化
            var attackTargetAgent = AttackTarget.GetComponent<NavMeshAgent>();
            attackTargetAgent.isStopped = true; // 先打断玩家移动
            attackTargetAgent.velocity = direction * repelForce;    // 击退
            AttackTarget.GetComponent<Animator>().SetTrigger(Dizzy);    // 让玩家眩晕
        }
    }// class GoblinController
    
}// namespace Character.Enemy
