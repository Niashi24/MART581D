using System;
using System.Collections;
using System.Collections.Generic;
using Mart581d.Extensions;
using Unity.VisualScripting;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform target;
    public Camera camera;
    public Vector2 offset;
    [Min(0.01f)]
    public float halfLife;

    private void Update()
    {
        if (target == null) return;

        Vector2 targetPosition = GetPositionRounded(target.position);
        transform.position = LerpSmooth(transform.position, targetPosition, Time.deltaTime, halfLife).Extend(transform.position.z);
    }

    static Vector2 LerpSmooth(Vector2 start, Vector2 end, float dt, float halfLife)
    {
        return Vector2.Lerp(start, end, 1 - Mathf.Pow(2f, -dt / halfLife));
    }

    Vector2 GetPositionRounded(Vector2 position)
    {
        float height = camera.orthographicSize * 2f;
        float width = 16f / 9f * height;

        return new Vector2(position.x.RoundTo(width), position.y.RoundTo(height));
    }

    void OnDrawGizmos()
    {
        if (camera == null) return;

        float height = camera.orthographicSize * 2f;
        float width = 16f / 9f * height;

        Vector2 position = GetPositionRounded(transform.position);

        var rect = Rect.MinMaxRect(-width / 2, -height / 2, width / 2, height / 2);
        rect.position += position;

        Span<Vector3> span = stackalloc Vector3[4];
        span[0] = new Vector3(rect.xMin, rect.yMin);
        span[1] = new Vector3(rect.xMin, rect.yMax);
        span[2] = new Vector3(rect.xMax, rect.yMax);
        span[3] = new Vector3(rect.xMax, rect.yMin);
        Gizmos.DrawLineStrip(span, true);
        
        Gizmos.DrawWireSphere(position, 2f);
    }
    
    
}
