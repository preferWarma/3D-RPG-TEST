using System;
using CharacterStatics.MonBehavior;
using UnityEngine;
using UnityEngine.AI;
using Utils;

namespace Characters.Enemy
{
    public class RockController : MonoBehaviour
    {
        [Header("基础设置")] 
        [Tooltip("攻击力度")] public float force;
        [Tooltip("攻击目标")] public GameObject attackTarget;
        [Tooltip("基础伤害值")] public int baseDamage;
        [Tooltip("散射效果")] public GameObject breakEffect;
        [Tooltip("静止时最大停留时间")] public float lastTime;

        private Rigidbody _rigidbody;   // 刚体
        [HideInInspector] public Enums.RockStates rockStates;   // 石头的状态
        [HideInInspector] public float remainTime;    // 剩余消失的时间
        
        private static readonly int Dizzy = Animator.StringToHash("Dizzy");
        
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>(); // 获取刚体组件
        }

        private void Start()
        {
            _rigidbody.velocity = Vector3.one;  // 避免一开始速度太小被认为是静止
            remainTime = lastTime;  // 避免刚开始生成就销毁了
            rockStates = Enums.RockStates.AttackPlayer;    // 生成的时候是攻击玩家的
            FlyToTarget();
        }

        private void FixedUpdate()
        {
            if (_rigidbody.velocity.sqrMagnitude < 1f)   // 大致静止
            {
                rockStates = Enums.RockStates.Station;
            }
            if (rockStates == Enums.RockStates.Station)
            {
                remainTime -= Time.deltaTime;
            }
            if (remainTime <= 0)
            {
                Destroy(gameObject);
            }
        }

        private void FlyToTarget()   // 飞向目标
        {
            if (attackTarget == null)
            {
                attackTarget = FindObjectOfType<PlayerController>().gameObject;
            }
            var direction = (attackTarget.transform.position - transform.position + Vector3.up).normalized;
            _rigidbody.AddForce(direction * force, ForceMode.Impulse);  // 以冲击力的形式施加
        }

        private void OnCollisionEnter(Collision collision)  // 碰撞的时候进行调用的函数
        {
            switch (rockStates)
            {
                case Enums.RockStates.AttackPlayer:
                    DoingWithAttackPlayerState(collision);
                    break;
                case Enums.RockStates.AttackStoneman:
                    DoingWithAttackStonemanState(collision);
                    break;
                case Enums.RockStates.Station:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void DoingWithAttackPlayerState(Collision collision)
        {
            remainTime = lastTime;
            if (collision.gameObject.CompareTag("Player"))  // 碰撞对象是玩家
            {
                var target = collision.gameObject;
                target.GetComponent<NavMeshAgent>().isStopped = true;   // 停止移动
                target.GetComponent<Animator>().SetTrigger(Dizzy);  // 眩晕一下
                var stats = target.gameObject.GetComponent<CharacterStats>();
                var damage = baseDamage + (int)(stats.MaxHealth * 0.1f); // 造成baseDamage + 最大生命值10%的伤害
                stats.ReceiveDamage(damage, stats);

                rockStates = Enums.RockStates.Station; // 攻击完后切换为静止状态
            }
        }
        private void DoingWithAttackStonemanState(Collision collision)
        {
            remainTime = lastTime;
            if (collision.gameObject.GetComponent<StonemanController>() != null)    // 如果是石头人
            {
                var stats = collision.gameObject.GetComponent<CharacterStats>();
                var damage = baseDamage + (int)(stats.MaxHealth * 0.1f); // 造成baseDamage + 最大生命值10%的伤害
                stats.ReceiveDamage(damage, stats);
                Instantiate(breakEffect, transform.position, Quaternion.identity);  // 产生爆炸效果
                Destroy(gameObject);    // 反击完后销毁该对象
            }
        }
    }// class RockController
    
}// namespace Character.Enemy
