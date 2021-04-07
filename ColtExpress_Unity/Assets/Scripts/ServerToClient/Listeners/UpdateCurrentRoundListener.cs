using Newtonsoft.Json.Linq;
using RoundSpace;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateCurrentRoundListener : UIEventListenable
{
    public GameObject turnsLocation;
    private static int _currentRound;

    private static Dictionary<TurnType, GameObject> turnPrefabs;
    private static GameObject[] turnTypePrefabs;

    private Vector3 scale = new Vector3(1f, 1f, 1f);

    private void Awake()
    {
        _currentRound = 0;
    
        turnTypePrefabs = Resources.LoadAll<GameObject>("TurnType Prefabs");

        turnPrefabs = new Dictionary<TurnType, GameObject>();

        foreach (GameObject p in turnTypePrefabs)
        {
            turnPrefabs.Add((TurnType)Enum.Parse(typeof(TurnType), p.name), p);
        }
    }

    public override void updateElement(string data)
    {
        /* PRE: data 
        *
            {
                eventName = "updateCurrentRound",
                isLastRound = bool,
                turns = list of turn types
            };
        */
        //Visually update the current round to the next round
        _currentRound++;
        this.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "" + _currentRound;

        //Initialize turns
        JObject o = JObject.Parse(data);
        List<TurnType> turns = o.SelectToken("turns").ToObject<List<TurnType>>();

        //Destroy all previous turn prefabs
        foreach (Transform child in turnsLocation.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        //Initialize new prefabs
        foreach (TurnType t in turns)
        {
            GameObject retrievedPrefab = null;
            if (turnPrefabs.TryGetValue(t, out retrievedPrefab))
            {
                GameObject generatedTurn = Instantiate(retrievedPrefab);
                generatedTurn.name = t.ToString();
                generatedTurn.transform.parent = turnsLocation.transform;
                generatedTurn.transform.localScale = scale;
            }
        }

        Debug.Log("[UpdateCurrentRoundListener] Round: " + _currentRound);


    }
}
