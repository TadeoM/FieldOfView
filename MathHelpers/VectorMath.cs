using UnityEngine;

namespace ShadowsIn2D.MathHelpers
{
    /// <summary>
    /// Common mathematical functions with vectors
    /// </summary>
    public static class VectorMath
    {

        /// <summary>
        /// Computes the intersection point of the line p1-p2 with p3-p4
        /// </summary>        
        public static Vector2 LineLineIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
        {
            // From http://paulbourke.net/geometry/lineline2d/
            var s = ((p4.x - p3.x) * (p1.y - p3.y) - (p4.y - p3.y) * (p1.x - p3.x))
                    / ((p4.y - p3.y) * (p2.x - p1.x) - (p4.x - p3.x) * (p2.y - p1.y));
            return new Vector2(p1.x + s * (p2.x - p1.x), p1.y + s * (p2.y - p1.y));
        }

        /// <summary>
        /// Returns if the point is 'left' of the line p1-p2
        /// </summary>        
        public static bool LeftOf(Vector2 p1, Vector2 p2, Vector2 point)
        {
            float cross = (p2.x - p1.x) * (point.y - p1.y)
                        - (p2.y - p1.y) * (point.x - p1.x);

            return cross < 0;
        }
                
        /// <summary>
        /// Returns a slightly shortened version of the vector:
        /// p * (1 - f) + q * f
        /// </summary>        
        public static Vector2 Interpolate(Vector2 p, Vector2 q, float f)
        {
            return new Vector2(p.x * (1.0f - f) + q.x * f, p.y * (1.0f - f) + q.y * f);
        }
    }
}
