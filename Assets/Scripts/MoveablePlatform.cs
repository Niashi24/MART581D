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

    public bool autoRepeat = true;
    public float repeatStartCooldown = 0.1f;
    public float repeatEndCooldown = 0f;

    public float moveToSpeed = 20f;
    public float moveBackSpeed = 20f;

    public float repeatTimer = 0f;
    public bool movingToEnd = true;
    
    // Start is called before the first frame update
    void Start()
    {
        start = this.transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        repeatTimer = Mathf.Max(0f, repeatTimer - Time.deltaTime);
        if (repeatTimer > 0f) return;
        var (target, speed) = movingToEnd ? (end.position, moveToSpeed) : (start, moveBackSpeed);

        var newPosition = Vector3.MoveTowards(rbdy.position, target, speed * Time.deltaTime);
        rbdy.MovePosition(newPosition);

        if (autoRepeat && rbdy.position.AlmostEqual(target))
        {
            repeatTimer = movingToEnd ? repeatEndCooldown : repeatStartCooldown;
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
        Gizmos.DrawWireCube(end.position, transform.lossyScale);
    }
}
