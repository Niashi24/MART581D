using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndTime : MonoBehaviour
{
    public TMP_Text text;
    
    // Start is called before the first frame update
    void Start()
    {
        text.text = $"Time: {TimerScript.TIMER_TEXT}";
    }

    public void ReturnToTitle()
    {
        SceneManager.LoadScene("Title Scene");
    }
}
