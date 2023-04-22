using System;
using CharacterStatics.ScriptableObj;
using Combat;
using Inventory.Item;
using Manager;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace CharacterStatics.MonBehavior
{
    public class CharacterStats : MonoBehaviour
    {
        [Tooltip("模板数据")] public CharacterDataSo templateCharacterData;
        [HideInInspector] public CharacterDataSo characterData;   // 数据配置信息
        [Tooltip("模板攻击数据")] public AttackDataSo templateAttackDataSo;
        [HideInInspector] public AttackDataSo attackData;   // 伤害配置信息
        private AttackDataSo _currentTemplateAttackDataSo;  // 当前模板信息

        public event Action<int, int> UpdateHealthBarOnAttack; // 攻击事件

        [Header("Weapon")] 
        [Tooltip("武器的装备位置")] public Transform weaponSlot;
        [Tooltip("盾牌装备位置")] public Transform shieldSlot;
        
        [HideInInspector] public bool isCritical; // 是否暴击

        private Animator _animator; // 动画控制器
        private RuntimeAnimatorController _initialRuntimeAnimator;  // 初始的无武器的动画
        private static readonly int GetHit = Animator.StringToHash("GetHit"); // 动画对应的索引
        
        private void Awake()
        {
            if (templateCharacterData != null)
            {
                characterData = Instantiate(templateCharacterData);  // 根据模板复制一份独立的数据
            }
            if (templateAttackDataSo != null)
            {
                attackData = Instantiate(templateAttackDataSo);
                _currentTemplateAttackDataSo = Instantiate(templateAttackDataSo);
            }

            _animator = GetComponent<Animator>();
            _initialRuntimeAnimator = _animator.runtimeAnimatorController;
        }

        private void OnUpdateHealthBarOnAttack(int arg1, int arg2)
        {
            UpdateHealthBarOnAttack?.Invoke(arg1, arg2);
        }

        #region LevelUp

        private void UpdateExperience(int killPoint) // 更新经验值
        {
            CurrentExperience += killPoint;
            if (CurrentExperience >= BaseExperience)
            {
                LevelUp();  // 升级
            }
        }
        
        private void LevelUp()  // 升级可以升级的的全部属性
        {
            CurrentLevel = Mathf.Clamp(CurrentLevel + 1, 1, MaxLevel);  // 限制最大等级
            BaseExperience = (int)(BaseExperience * 1.5f);    // 更新升级所需的经验上限

            MaxHealth = (int)(MaxHealth * (1 + LevelBuff));  // 加最大生命值上限
            BaseDefence = (int)(BaseDefence * (1 + LevelBuff));  // 加基础防御力
            _currentTemplateAttackDataSo.data.minDamage = // 改当前模板
                (int)Mathf.Ceil(_currentTemplateAttackDataSo.data.minDamage * (1 + LevelBuff));   // 加最低攻击力, 向上取整数
            _currentTemplateAttackDataSo.data.maxDamage = // 改当前模板
                (int)Mathf.Ceil(_currentTemplateAttackDataSo.data.maxDamage * (1 + LevelBuff));   // 加最大攻击力, 向上取整数
            CurrentHealth = MaxHealth;    // 升级的时候恢复满血
            var tmp = Instantiate(_currentTemplateAttackDataSo);
            tmp.data.minDamage = (int)Mathf.Ceil(MinDamage * (1 + LevelBuff));
            tmp.data.maxDamage = (int)Mathf.Ceil(MaxDamage * (1 + LevelBuff));
            attackData = Instantiate(tmp);  // 更改当前数值
        }
        
        #endregion

        #region get and set CharacterData_SO
        public int MaxHealth
        {
            get => characterData != null ? characterData.data.maxHealth : 0;
            set => characterData.data.maxHealth = value;
        }

        public int CurrentHealth
        {
            get => characterData != null ? characterData.data.currentHealth : 0;
            set => characterData.data.currentHealth = value;
        }

        public int BaseDefence
        {
            get => characterData != null ? characterData.data.baseDefence : 0;
            set => characterData.data.baseDefence = value;
        }

        public int CurrentDefence
        {
            get => characterData != null ? characterData.data.currentDefence : 0;
            set => characterData.data.currentDefence = value;
        }

        public int CurrentLevel
        {
            get => characterData != null ? characterData.data.currentLevel : 0;
            set => characterData.data.currentLevel = value;
        }

        public int MaxLevel
        {
            get => characterData != null ? characterData.data.maxLevel : 0;
            set => characterData.data.maxLevel = value;
        }

        public int BaseExperience
        {
            get => characterData != null ? characterData.data.baseExperience : 0;
            set => characterData.data.baseExperience = value;
        }

        public int CurrentExperience
        {
            get => characterData != null ? characterData.data.currentExperience : 0;
            set => characterData.data.currentExperience = value;
        }
        
        public float LevelBuff
        {
            get => characterData != null ? characterData.data.levelBuff : 0;
            set => characterData.data.levelBuff = value;
        }

        public int KillPoint
        {
            get => characterData != null ? characterData.data.killPoint : 0;
            set => characterData.data.killPoint = value;
        }
        
        #endregion
        
        #region get and set AttackData_SO

        public float BaseAttackRange
        {
            get => attackData != null ? attackData.data.baseAttackRange : 0;
            set => attackData.data.baseAttackRange = value;
        }

        public float SkillAttackRange
        {
            get => attackData != null ? attackData.data.skillAttackRange : 0;
            set => attackData.data.skillAttackRange = value;
        }

        public float CoolDown
        {
            get => attackData != null ? attackData.data.coolDown : 0;
            set => attackData.data.coolDown = value;
        }

        public int MinDamage
        {
            get => attackData != null ? attackData.data.minDamage : 0;
            set => attackData.data.minDamage = value;
        }

        public int MaxDamage
        {
            get => attackData != null ? attackData.data.maxDamage : 0;
            set => attackData.data.maxDamage = value;
        }

        public float CriticalMultiplier
        {
            get => attackData != null ? attackData.data.criticalMultiplier : 0;
            set => attackData.data.criticalMultiplier = value;
        }

        public float CriticalChance
        {
            get => attackData != null ? attackData.data.criticalChance : 0;
            set => attackData.data.criticalChance = value;
        }

        #endregion
        
        #region  Character Combat

        public void ReceiveDamage(CharacterStats attacker, CharacterStats defender)    // 受到伤害
        {
            var damage = Mathf.Max(GenerateDamage(attacker) - defender.CurrentDefence, 0);  // 伤害值是正值
            defender.CurrentHealth = Mathf.Max(defender.CurrentHealth - damage, 0);  // 生命值最小为0
            if (attacker.isCritical)    // 如果暴击，则播放defender的受击动画
            {
                defender.GetComponent<Animator>().SetTrigger(GetHit);
            }
            // 更新UI
            OnUpdateHealthBarOnAttack(defender.CurrentHealth, defender.MaxHealth);
            // 升级人物属性
            if (defender.CurrentHealth <= 0)    // 如果受伤者死亡
            {
                attacker.UpdateExperience(defender.KillPoint);
            }
        }

        public void ReceiveDamage(int damage, CharacterStats defender)  // 另一个重载形式, 直接根据伤害值参数来计算伤害
        {
            damage = Mathf.Max(damage - defender.CurrentDefence, 0);
            defender.CurrentHealth = Mathf.Max(defender.CurrentHealth - damage, 0);
            // 更新UI
            OnUpdateHealthBarOnAttack(defender.CurrentHealth, defender.MaxHealth);
            // 升级人物属性
            if (defender.CurrentHealth <= 0)
            {
                GameManager.Instance.playerStats.UpdateExperience(defender.KillPoint);
            }
        }
        
        private int GenerateDamage(CharacterStats attacker)    // 根据MaxDamage和MinDamage生成随机伤害值
        {
            var tmp = Random.Range(attacker.MinDamage, attacker.MaxDamage);
            return (int)(attacker.isCritical ? tmp * attacker.CriticalMultiplier : tmp);
        }
        
        #endregion

        #region Euqip Weapon

        private void EquipWeapon(ItemDataSo weapon) // 装备武器, 更改属性
        {
            if (weapon.eqPrefabs)
            {
                Instantiate(weapon.eqPrefabs, weaponSlot);  // 在武器装备位置生成武器
                var curData = new AttackData    // 将要更新的新数据
                {
                    baseAttackRange = weapon.weaponData.data.baseAttackRange,   // 攻击范围与武器保持一致
                    skillAttackRange = weapon.weaponData.data.skillAttackRange, // 技能范围与武器保持一致
                    coolDown = weapon.weaponData.data.coolDown, // 冷却时间与武器保持一致
                    minDamage = weapon.weaponData.data.minDamage + MinDamage,   // 最小伤害叠加
                    maxDamage = weapon.weaponData.data.maxDamage + MaxDamage,   // 最大伤害叠加
                    criticalMultiplier = weapon.weaponData.data.criticalMultiplier + CriticalMultiplier, // 暴击伤害叠加
                    criticalChance = weapon.weaponData.data.criticalChance + CriticalChance //暴击率叠加
                };
                attackData.UpdateData(curData);  // 更新属性
                _animator.runtimeAnimatorController = weapon.weaponAnimator;    // 切换动画
            }
        }

        public void UnEquipWeapon()    // 卸下武器, 更改属性
        {
            if (weaponSlot.transform.childCount != 0)   // 有装备武器
            {
                for (var i = 0; i < weaponSlot.transform.childCount; i++)   // 逐个销毁子物体
                {
                    if (weaponSlot.transform.GetChild(i).gameObject.activeInHierarchy)
                        Destroy(weaponSlot.transform.GetChild(i).gameObject);
                }
                _animator.runtimeAnimatorController = _initialRuntimeAnimator;  // 还原动画
            }
            attackData.UpdateData(_currentTemplateAttackDataSo.data);   // 还原攻击力
        }

        public void ChangeWeapon(ItemDataSo weapon) // 切换武器, 参数是将要装备的武器
        {
            UnEquipWeapon();
            EquipWeapon(weapon);
        }

        #endregion
        
        #region Equip Shield

        private void EquipShield(ItemDataSo shield) // 装备武器, 更改属性
        {
            if (shield.eqPrefabs)
            {
                Instantiate(shield.eqPrefabs, shieldSlot);  // 在武器装备位置生成武器
                MaxHealth += 20;    // 加生命上限
                CurrentDefence += 3;    //加防御
            }
        }

        public void UnEquipShield()    // 卸下武器
        {
            if (shieldSlot.transform.childCount == 0) return; // 没有装备武器则结束
            
            var flag = false;   // 是否销毁过了
            for (var i = 0; i < shieldSlot.transform.childCount; i++)   // 逐个销毁子物体
            {
                if (!shieldSlot.transform.GetChild(i).gameObject.activeInHierarchy) continue;   // 没有激活的物体则跳过
                Destroy(shieldSlot.transform.GetChild(i).gameObject);
                flag = true;
            }

            if (!flag) return;  // 没有销毁则不改变属性
            // 更改属性
            MaxHealth -= 20;
            CurrentHealth = Mathf.Min(CurrentHealth, MaxHealth);
            CurrentDefence -= 3;
        }

        public void ChangeShield(ItemDataSo shield)
        {
            UnEquipShield();
            EquipShield(shield);
        }
        
        #endregion

        #region Use UsableItem

        public void CureHealth(int curePoint)   // 回血
        {
            CurrentHealth = Mathf.Min(CurrentHealth + curePoint, MaxHealth);
        }

        #endregion
        
    }// class CharacterStats
    
}// namespace CharacterStatics.MonBehavior
