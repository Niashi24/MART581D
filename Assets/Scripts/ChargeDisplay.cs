using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeDisplay : MonoBehaviour
{
    public PlayerScript player;

    public GameObject displayBase;

    public Transform displayMeter;

    // Update is called once per frame
    void Update()
    {
        var scale = displayMeter.localScale;
        scale.x = player.GetBarkCooldownPercent();
        displayMeter.localScale = scale;

        displayBase.SetActive(scale.x != 1f);
    }
}
