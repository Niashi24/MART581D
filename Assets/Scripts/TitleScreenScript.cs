using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenScript : MonoBehaviour
{


    public void MoveToGame()
    {
        SceneManager.LoadScene("Intro", LoadSceneMode.Single);
    }
}
