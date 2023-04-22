using UnityEngine;

namespace Utils
{
    public static class ExtensionMethod
    {
        private const float DotThreshold = 0.5f;
        
        // 拓展方法, 第一个参数是需要拓展改方法的类，第二个参数是拓展方法的参数
        public static bool IsFacingTarget(this Transform transform, Transform target)
        {
            var vectorToTarget = (target.position - transform.position).normalized;  // 目标相对于自己的方向(归一化)
            return Vector3.Dot(transform.forward, vectorToTarget) >= DotThreshold;  // 当目标位于面前120°的扇形区域为true
        }
    }// static class ExtensionMethod
    
}// namespace Utils