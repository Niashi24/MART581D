using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarkCrystal : MonoBehaviour
{
    public SpriteRenderer sprite;

    public bool respawns = true;
    public float respawnTimer = 1f;

    public Color enabledColor = Color.cyan;
    public Color disabledColor = Color.gray;

    public bool available;
    public float timer = 0f;

    public void Update()
    {
        timer = Mathf.Max(0f, timer - Time.deltaTime);
        if (!available && respawns && timer == 0f)
        {
            available = true;
            sprite.color = enabledColor;
        }
    }
    
    public void Trigger(PlayerScript player)
    {
        if (!available) return;
        
        available = false;
        timer = respawnTimer;
        sprite.color = disabledColor;

        player.ResetBark();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!available) return;
        
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerScript>();
            available = false;
            timer = respawnTimer;
            sprite.color = disabledColor;
            player.ResetBark();
        }
    }
}
