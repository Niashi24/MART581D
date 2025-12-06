using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroController : MonoBehaviour
{
    [System.Serializable]
    public class Frame
    {
        public Sprite image;
        public bool autoAdvance;
        public float autoAdvanceLength;
        
    }

    public Image image;
    public Image clickImage;
    public Frame[] frames;
    private int currentFrame = 0;
    private float autoAdvanceTimer = 0f;

    private void Start()
    {
        currentFrame = 0;
        SetFrame(frames[currentFrame]);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene("Game Scene", LoadSceneMode.Single);
        }
        
        if (currentFrame < 0 || currentFrame >= frames.Length) return;

        autoAdvanceTimer = Mathf.Max(autoAdvanceTimer - Time.deltaTime, 0f);

        var frame = frames[currentFrame];
        if (frame.autoAdvance)
        {
            if (autoAdvanceTimer == 0f)
            {
                MoveNext();
            }
        } else if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            MoveNext();
        }
    }

    private void MoveNext()
    {
        currentFrame++;
        if (currentFrame == frames.Length)
        {
            SceneManager.LoadScene("Game Scene", LoadSceneMode.Single);
        }
        else
        {
            SetFrame(frames[currentFrame]);
        }
    }

    private void SetFrame(Frame frame)
    {
        image.sprite = frame.image;
        autoAdvanceTimer = frame.autoAdvanceLength;
        clickImage.gameObject.SetActive(!frame.autoAdvance);
    }
}
