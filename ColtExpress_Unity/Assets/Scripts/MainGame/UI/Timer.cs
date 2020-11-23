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
    private float time = 15;
    private bool timerIsRunning = false;

    public TextMeshProUGUI timer;

    void Start()
    {
        timer = this.transform.gameObject.GetComponent<TextMeshProUGUI>();
    }


    public IEnumerator waitForTimer(bool timedOut, System.Action<bool> result)
    {
        // Coroutine for the timer
        //Debug.Log("Coroutine waitForTimer started.");
        timerIsRunning = true;

        while (timerIsRunning)
        {
            if (time > 0)
            {
                time -= Time.deltaTime;
                displayTimer(time);
                yield return null;
            }
            else
            {
                time = 0;
                timerIsRunning = false;
                displayTimer(time);
                //Debug.Log("Coroutine waitForTimer terminating.");
                result(true);
                yield break;
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
