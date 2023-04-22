using UnityEngine;
using Utils;

namespace Combat
{
    [CreateAssetMenu(fileName = "new Attack Data", menuName = "Attack/AttackData")]    // 添加此类到Asset菜单栏
    public class AttackDataSo : ScriptableObject
    {
        public AttackData data;   // 攻击类型数据
        
        public void UpdateData(AttackData otherData)    // 更新攻击数据
        {
            data = otherData;
        }

    }// class AttackDataSo
    
}// namespace Combat
