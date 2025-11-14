using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObstacleScript : MonoBehaviour
{
    // public Transform respawnLocation;
    //
    // void OnDrawGizmos()
    // {
    //     if (respawnLocation == null) return;
    //     Gizmos.DrawLine(transform.position, respawnLocation.position);
    //     Gizmos.DrawWireSphere(respawnLocation.position, 2);
    // }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerScript>(out var player))
        {
            player.TakeDamage();
            // player.transform.position = respawnLocation.position;
        }
    }
}
