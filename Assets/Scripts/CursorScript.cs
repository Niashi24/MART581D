using System;
using System.Collections;
using System.Collections.Generic;
using Mart581d.Extensions;
using UnityEngine;

public class CursorScript : MonoBehaviour
{
    public PlayerScript player;
    public LineRenderer lineRenderer;

    private void LateUpdate()
    {
        if (Time.timeScale == 0f) return;
        
        var mousePos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var directionToMouse = (mousePos - (Vector2)player.transform.position).WithMaxMagnitude(player.barkDistance);

        transform.position = transform.position.WithXY((Vector2)player.transform.position + directionToMouse);
        lineRenderer.SetPosition(1, player.transform.position - transform.position);
    }
}
