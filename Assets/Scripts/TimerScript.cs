using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerScript : MonoBehaviour
{
     public float timer = 0f;

     public bool finished = false;

     public TMP_Text text;

     private void Update()
     {
          if (finished) return;

          timer += Time.deltaTime;

          float displayTime = timer;

          int minutes = Mathf.FloorToInt(displayTime / 60f);
          displayTime -= 60f * minutes;
          int seconds = Mathf.FloorToInt(displayTime);
          displayTime -= seconds;
          int milliseconds = Mathf.FloorToInt(displayTime * 1000f);

          text.text = $"{minutes}:{seconds:00}:{milliseconds:000}";
     }

     private void OnTriggerEnter2D(Collider2D other)
     {
          if (other.CompareTag("Player"))
          {
               finished = true;
          }
     }
}
