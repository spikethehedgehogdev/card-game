using UnityEngine;

namespace Shared.Math
{
    public static class Bezier
    {
        public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            t = Mathf.Clamp01(t);
            var u = 1.0f - t;
            return
                u * u * u * p0 +
                3.0f * u * u * t * p1 +
                3.0f * u * t * t * p2 +
                t * t * t * p3;
            
        }

        public static Vector3 GetTangent(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) => 
            GetFirstDerivative(p0, p1, p2, p3, t).normalized;

        private static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            
            t = Mathf.Clamp01(t);
            var u = 1 - t;
            return
                3 * u * u * (p1 - p0) +
                6 * u * t * (p2 - p1) +
                3 * t * t * (p3 - p2);
        }
    }
}