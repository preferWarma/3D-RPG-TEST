using UnityEngine;
using Utils;

namespace CharacterStatics.ScriptableObj
{
    [CreateAssetMenu(fileName = "new Data", menuName = "CharacterStatics/Data")]    // 添加此类到Asset菜单栏
    public class CharacterDataSo : ScriptableObject // 创建不需要挂载到游戏对象的类, 可以继承于ScriptableObject
    {
        public CharacterData data;  // 数值类型

        // public void UpdateExperience(int killPoint) // 更新经验值
        // {
        //     data.currentExperience += killPoint;
        //     if (data.currentExperience >= data.baseExperience)
        //     {
        //         LevelUp();  // 升级
        //     }
        // }
        //
        // private void LevelUp()  // 升级可以升级的的全部属性
        // {
        //     data.currentLevel = Mathf.Clamp(data.currentLevel + 1, 1, data.maxLevel);  // 限制最大等级
        //     data.baseExperience = (int)(data.baseExperience * 1.5f);    // 更新升级所需的经验上限
        //
        //     data.maxHealth = (int)(data.maxHealth * (1 + data.levelBuff));  // 加最大生命值上限
        //     data.baseDefence = (int)(data.baseDefence * (1 + data.levelBuff));  // 加基础防御力
        //     data.currentHealth = data.maxHealth;    // 升级的时候恢复满血
        //     
        // }
    }// class CharacterDataSo

}// namespace CharacterStatics
