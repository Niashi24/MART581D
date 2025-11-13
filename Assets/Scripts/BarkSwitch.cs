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

    public Color disabledColor = Color.red;
    public Color enabledColor = Color.green;

    public void Trigger()
    {
        if (state && !toggle) return;

        state = !state;
        sprite.color = state ? enabledColor : disabledColor;
        OnTrigger.Invoke(state);
    }
}
