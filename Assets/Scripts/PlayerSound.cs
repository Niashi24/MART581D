using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerSound : MonoBehaviour
{
    public PlayerScript player;
    public AudioSource source;

    public AudioClip bark;
    public AudioClip hurt;
    public AudioClip jump;
    public AudioClip land;
    public AudioClip slide;

    private void Start()
    {
        player.OnBark += Bark;
        player.OnTakeDamage += Hurt;
        player.OnJump += Jump;
        player.OnLand += Land;
        // source.Play();
        source.Play();
    }

    private void LateUpdate()
    {
        if (player.state == PlayerScript.PlayerState.WallSlide)
        {
            if (!source.isPlaying) source.Play();
            source.clip = slide;
        }
        else
        {
            // if (source.isPlaying) source.Stop
            source.clip = null;
        }
    }

    private void Bark()
    {
        // AudioSource.;
        source.pitch = Random.Range(1.0f, 1.1f);
        source.PlayOneShot(bark);
        // source.pitch = 1f;
    }

    private void Hurt()
    {
        source.pitch = Random.Range(0.9f, 1.1f);
        source.PlayOneShot(hurt);
    }

    private void Jump()
    {
        source.pitch = 1f;
        source.PlayOneShot(jump);
    }

    private void Land()
    {
        source.pitch = 1f;
        source.PlayOneShot(land);
    }
}
