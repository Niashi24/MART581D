using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        // player.On
        // source.Play();
    }
}
