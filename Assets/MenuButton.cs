using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButton : MonoBehaviour
{
    public AudioClip hover;
    public AudioClip click;

    public AudioSource source;
    
    public void OnHover()
    {
        source.PlayOneShot(hover, 1.0f);
    }

    public void OnClick()
    {
        source.PlayOneShot(click, 1.0f);
    }
}
