using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CalculateScore : MonoBehaviour
{

    void Start()
    {
        Debug.Log("Calculate the final score");

        //JObject o = JObject.Parse(data);
        //bool isEnd = endofCard();

        //get player's score
        Character player = o.SelectToken("player").ToObject<Character>();

        int score = o.SelectToken("finalScore").ToObject<int>();
        string username = o.SelectToken("");


        //how to call the CalculateGameScore();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 3);

        //changeScoreText sct = 

    }


}
