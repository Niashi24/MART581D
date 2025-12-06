using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BarkSwitch : MonoBehaviour
{
    public SpriteRenderer sprite;

    public bool toggle = true;
    
    bool state = false;

    public UnityEvent<bool> OnTrigger;

    public Sprite disabledSprite;
    public Sprite enabledSprite;

    public void Trigger()
    {
        if (state && !toggle) return;

        state = !state;
        sprite.sprite = state ? enabledSprite : disabledSprite;
        OnTrigger.Invoke(state);
    }
}
