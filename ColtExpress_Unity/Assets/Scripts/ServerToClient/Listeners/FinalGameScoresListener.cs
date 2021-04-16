using GameUnitSpace;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FinalGameScoresListener : UIEventListenable
{
    public GameObject scoreInformationPrefab;
    public GameObject scoreParent;
    private string dataForParse;

    void Start()
    {
        
    }

    public override void updateElement(string data)
    {
        dataForParse = data;

        StartCoroutine("LoadScene");

        Debug.Log("Coroutine started.");
    }

    private void continueWithScores()
    {
        scoreParent = GameObject.Find("ScoreList");

        JObject o = JObject.Parse(dataForParse);
        List<KeyValuePair<Player, int>> scores = o.SelectToken("scores").ToObject<List<KeyValuePair<Player, int>>>();
        Player gunslinger = o.SelectToken("gunslinger").ToObject<Player>();

        int medal = 0;
        foreach (KeyValuePair<Player, int> pair in scores)
        {
            GameObject newScore = Instantiate(scoreInformationPrefab);
            if (pair.Key.getBandit() == gunslinger.getBandit())
            {
                newScore.transform.GetChild(1).GetChild(0).transform.gameObject.SetActive(true);
            }

            Sprite bandit = null;
            GameUIManager.loadedSprites.TryGetValue(pair.Key.getBandit().ToString().ToLower() + "_character", out bandit);
            newScore.transform.GetChild(1).GetComponent<Image>().sprite = bandit;
            newScore.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = String.Format("${0}", pair.Value);

            if (medal <= 2)
            {
                newScore.transform.GetChild(3).GetChild(medal).transform.gameObject.SetActive(true);
                medal++;
            }

            newScore.transform.parent = scoreParent.transform;
            newScore.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        Debug.Log("[FinalGameScoresListener] Scores presented.");
    }

    private IEnumerator LoadScene()
    {
        // Start loading the scene
        AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync("FinalScores");
        // Wait until the level finish loading
        while (asyncLoadLevel.isDone == false)
        {
            Debug.Log("Scene not loaded yet.");
            yield return null;
        }
        // Wait a frame so every Awake and Start method is called
        yield return new WaitForEndOfFrame();

        continueWithScores();
    }
}
