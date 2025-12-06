using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarkCrystal : MonoBehaviour
{
    public SpriteRenderer sprite;
    public AudioSource audioSource;

    public AudioClip crystalBreak;
    public AudioClip crystalReform;

    public bool respawns = true;
    public float respawnTimer = 1f;

    public Sprite enabledSprite;
    public Sprite disabledSprite;

    public bool available;
    public float timer = 0f;

    public void Update()
    {
        timer = Mathf.Max(0f, timer - Time.deltaTime);
        if (!available && respawns && timer == 0f)
        {
            available = true;
            sprite.sprite = enabledSprite;
            audioSource.PlayOneShot(crystalReform, 0.8f);
        }
    }
    
    public void Trigger(PlayerScript player)
    {
        if (!available) return;
        if (player.canBark) return;
        
        available = false;
        timer = respawnTimer;
        sprite.sprite = disabledSprite;
        audioSource.PlayOneShot(crystalBreak, 3.0f);

        player.ResetBark();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!available) return;
        
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerScript>();
            if (player.canBark) return;
            available = false;
            timer = respawnTimer;
            sprite.sprite = disabledSprite;
            audioSource.PlayOneShot(crystalBreak, 3.0f);
            player.ResetBark();
        }
    }
}
