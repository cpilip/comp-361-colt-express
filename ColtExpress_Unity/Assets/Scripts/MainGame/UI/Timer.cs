using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/* Author: Christina Pilip
 * Usage: Timer class for a timer Unity object. 
 */
public class Timer : MonoBehaviour
{
    private float time = 180;
    private bool timerIsRunning = false;

    public TextMeshProUGUI timer;

    void Start()
    {
        timerIsRunning = true;
        timer = this.transform.gameObject.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timerIsRunning)
        {
            if (time > 0)
            {
                time -= Time.deltaTime;
                displayTimer(time);
            }
            else
            {
                time = 0;
                timerIsRunning = false;
            }
        }
    }

    public void displayTimer(float time)
    {
        float minutes = Mathf.FloorToInt(time / 60);
        float seconds = Mathf.FloorToInt(time % 60);

        timer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
