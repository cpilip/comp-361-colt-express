using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CalculateScore : MonoBehaviour
{

    public Text changeText;

    void Start()
    {
        Debug.Log("Calculate the final score");

        JObject o = JObject.Parse(data);
        Boolean isEnd = endofCard();
        //get all player's score

        List<Character> r_P = o.SelectToken("r_players").ToObject<List<Character>>();
        int score = o.SelectToken("finalScore").ToObject<int>();
        string username = o.SelectToken("");


        //how to call the CalculateGameScore();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 3);

        //change the text from the finalScores scene
        changeText.text = score;


    }


}
