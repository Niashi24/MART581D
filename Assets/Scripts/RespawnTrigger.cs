using System;
using UnityEngine;

public class RespawnTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerScript>(out var player))
        {
            player.SetRespawnLocation(transform.position);
        }
    }
}