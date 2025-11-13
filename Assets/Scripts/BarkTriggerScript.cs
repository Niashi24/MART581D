using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BarkTriggerScript : MonoBehaviour
{
    public UnityEvent<PlayerScript> triggerEvent;
    
    public void Trigger(PlayerScript player)
    {
        triggerEvent.Invoke(player);
    }
}
