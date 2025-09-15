using UnityEngine;

namespace com.fscigliano.CommonExtensions
{
    /// <summary>
    /// Product Name:    Common extensions
    /// Developers:      Franco Scigliano
    /// Description:     Simple angle math to convert angle to direction, and an unclamped version of inverselerp
    /// Changelog:       
    /// </summary>
    public static class MathHelper
    {
        private static Vector2 RadianToVector2(float radian)
        {
            return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
        }

        public static Vector2 DegreeToVector2(float degree)
        {
            return RadianToVector2(degree * Mathf.Deg2Rad);
        }
        public static float InverseLerpUnclamped(float a, float b, float v)
        {
            return (v - a) / (b - a);
        }
    }

}
