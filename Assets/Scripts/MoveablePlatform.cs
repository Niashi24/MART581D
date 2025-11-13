using System;
using System.Collections;
using System.Collections.Generic;
using Mart581d.Extensions;
using UnityEngine;

public class MoveablePlatform : MonoBehaviour
{
    public Transform end;

    public Rigidbody2D rbdy;

    private Vector3 start;

    public bool movingToEnd = true;

    public bool autoRepeat = true;

    public float moveToSpeed = 20f;
    public float moveBackSpeed = 20f;
    
    // Start is called before the first frame update
    void Start()
    {
        start = this.transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var (target, speed) = movingToEnd ? (end.position, moveToSpeed) : (start, moveBackSpeed);

        var newPosition = Vector3.MoveTowards(rbdy.position, target, speed * Time.deltaTime);
        rbdy.MovePosition(newPosition);

        if (autoRepeat && rbdy.position.AlmostEqual(target))
        {
            movingToEnd = !movingToEnd;
        }
    }

    public void SetMovingToEnd(bool moving)
    {
        this.movingToEnd = moving;
    }

    private void OnDrawGizmos()
    {
        if (end == null) return;
        Gizmos.DrawLine(transform.position, end.position);
    }
}
