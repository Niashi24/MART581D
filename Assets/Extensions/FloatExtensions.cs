using System.Runtime.CompilerServices;
using UnityEngine;

namespace Mart581d.Extensions
{
    public static class FloatExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AtLeast(this float f, float min) => Mathf.Max(min, f);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 WithX(this Vector2 v, float x)
        {
            return new Vector2(x, v.y);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 WithY(this Vector2 v, float y)
        {
            return new Vector2(v.x, y);
        }

        public static float RoundTo(this float f, float step)
        {
            return Mathf.Round(f / step) * step;
        }
        
        public static float WithMaxMagnitude(this float f, float maxMagnitude)
        {
            return Mathf.Min(Mathf.Abs(f), Mathf.Abs(maxMagnitude)) * Mathf.Sign(f);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="curVel"></param>
        /// <param name="dir"></param>
        /// <param name="accel"></param>
        /// <param name="skidAccel"></param>
        /// <param name="maxSpeed"></param>
        /// <returns></returns>
        public static float AddAccelerationInDirection(float curVel, float dir, float accel, float skidAccel, float maxSpeed)
        {
            float sign = Mathf.Sign(curVel);
            float a = sign == Mathf.Sign(dir) ? accel : skidAccel;
            curVel *= sign;  // now is positive/abs
            a *= sign * dir;
            return sign * AddAcceleration(curVel, a, maxSpeed);
        }
        
        /// <summary>
        /// Adds the given acceleration without going over maximum speed
        /// </summary>
        /// <param name="curSpeed">Current speed (absolute value)</param>
        /// <param name="accel">Acceleration (may be negative)</param>
        /// <param name="maxSpeed">Max speed (absolute value)</param>
        /// <returns>min(curSpeed + accel, maxSpeed)</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AddAcceleration(float curSpeed, float accel, float maxSpeed)
        {
            if (curSpeed < maxSpeed || accel < 0)
                return Mathf.Min(curSpeed + accel, maxSpeed);
            else
                return curSpeed;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 WithMaxMagnitude(this Vector2 v, float maxMag)
        {
            if (v.magnitude < maxMag) return v;
            return v.normalized * maxMag;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ChangeToSurface(Vector2 dir, Vector2 surfaceNormal)
        {
            if (Vector2.Dot(dir, surfaceNormal) >= 0) return dir;
            return CastToSurface(dir, surfaceNormal);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 CastToSurface(Vector2 dir, Vector2 surfaceNormal)
        {
            return dir - Vector2.Dot(dir, surfaceNormal) * surfaceNormal;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 WithDirection(this Vector2 cur, Vector2 newDir) => newDir * cur.magnitude;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 WithMagnitude(this Vector2 cur, float magnitude) => cur.normalized * magnitude;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Reflect(this Vector2 v, Vector2 normal)
        {
            // Dot product of v and normal
            float dot = Vector2.Dot(v, normal);
    
            // Reflection formula: v - 2 * dot * normal
            return v - 2 * dot * normal;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Extend(this Vector2 v, float z) => new Vector3(v.x, v.y, z);
    }
}