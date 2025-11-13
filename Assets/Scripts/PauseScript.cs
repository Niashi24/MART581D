using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScript : MonoBehaviour
{
    public GameObject pauseOverlay;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 1f)
            {
                Time.timeScale = 0f;
                pauseOverlay.SetActive(true);
            }
            else
            {
                Time.timeScale = 1f;
                pauseOverlay.SetActive(false);
            }
        }
    }

    public void ReturnToTitle()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Title Scene", LoadSceneMode.Single);
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        pauseOverlay.SetActive(false);
        // gameObject.SetActive(false);
    }
}
