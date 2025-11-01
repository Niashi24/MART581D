using UnityEngine;

namespace Mart581d.Extensions
{
    public static class ObjectExtensions
    {

        public static T Log<T>(this T obj)
        {
            Debug.Log(obj);
            return obj;
        }

        public static void DebugCircle(Vector2 pos, float r, Color c)
        {
            const int verts = 8;
            for (int i = 0; i < verts; i++)
            {
                float t1 = (float)i / verts;
                Vector2 a = new Vector2(Mathf.Cos(t1 * Mathf.PI * 2f), Mathf.Sin(t1 * Mathf.PI * 2));
                float t2 = (float)(i + 1) / verts;
                Vector2 b = new Vector2(Mathf.Cos(t2 * Mathf.PI * 2f), Mathf.Sin(t2 * Mathf.PI * 2));

                Debug.DrawLine(pos + a * r, pos + b * r, c);
            }
        }
    }
}