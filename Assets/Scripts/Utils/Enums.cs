namespace Utils
{
    public static class Enums
    {
        public enum EnemyStates // 敌人状态
        {
            Guard,  // 站桩
            Patrol, // 巡逻
            Chase,  // 追逐
            Dead    // 死亡
        }
        
        public enum RockStates  // 生成的石头的状态
        {
            AttackPlayer,   // 攻击玩家
            AttackStoneman, // 攻击石头人
            Station         //静止状态
        }
        
        public enum TransitionType  // 传送门类型
        {
            SameScene,   // 同场景
            DifferentScene  // 异场景
        }
        
        public enum DestinationTag // 终点的标签
        {
            Menu,   // 主菜单传送口
            Enter,  // 场景的入口
            A,  // 场景的A点
            B,  // 场景的B点
            C,  // 场景的C点
            D   // 场景的D点
        }
        
        public enum ItemType    // 物品类型
        {
            Usable, // 可使用的
            Weapon, // 武器
            Shield   // 盾牌
        }
        
        public enum SlotType    // 卡槽类型
        {
            Bag, // 背包格
            Weapon, // 武器格
            Shield, // 防具格
            Action  // 快捷栏
        }

    }// static class Enums
    
}// namespace Utils
