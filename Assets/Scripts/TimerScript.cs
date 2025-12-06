using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerScript : MonoBehaviour
{
     // public float timer = 0f;

     public bool finished = false;

     // public TMP_Text text;

     public static float TIMER = 0f;
     public static string TIMER_TEXT = "";

     private void Start()
     {
          TIMER = 0f;
     }

     private void Update()
     {
          if (finished) return;

          TIMER += Time.deltaTime;

          float displayTime = TIMER;

          int minutes = Mathf.FloorToInt(displayTime / 60f);
          displayTime -= 60f * minutes;
          int seconds = Mathf.FloorToInt(displayTime);
          displayTime -= seconds;
          int milliseconds = Mathf.FloorToInt(displayTime * 1000f);

          TIMER_TEXT = $"{minutes}:{seconds:00}:{milliseconds:000}";
     }

     private void OnTriggerEnter2D(Collider2D other)
     {
          if (other.CompareTag("Player"))
          {
               finished = true;
          }
     }
}
