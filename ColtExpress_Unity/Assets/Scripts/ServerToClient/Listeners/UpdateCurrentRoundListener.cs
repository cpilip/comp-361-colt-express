using Newtonsoft.Json.Linq;
using RoundSpace;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateCurrentRoundListener : UIEventListenable
{
    public GameObject turnsLocation;
    public GameObject eventLocation;
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
                roundEvent
            };
        */
        //Visually update the current round to the next round
        _currentRound++;
        this.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "" + _currentRound;

        //Initialize turns
        JObject o = JObject.Parse(data);
        List<TurnType> turns = o.SelectToken("turns").ToObject<List<TurnType>>();
        EndOfRoundEvent roundEvent = o.SelectToken("roundEvent").ToObject<EndOfRoundEvent>();

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

        string roundEventText = "";
        switch (roundEvent)
        {
            //Set event
            case EndOfRoundEvent.AngryMarshal:
                roundEventText = "Angry Marshal";
                break;
            case EndOfRoundEvent.SwivelArm:
                roundEventText = "Swivel Arm";
                break;
            case EndOfRoundEvent.Braking:
                roundEventText = "Braking";
                break;
            case EndOfRoundEvent.TakeItAll:
                roundEventText = "Take It All!";
                break;
            case EndOfRoundEvent.PassengersRebellion:
                roundEventText = "Passengers' Rebellion";
                break;
            case EndOfRoundEvent.PantingHorses:
                roundEventText = "Panting Horses";
                break;
            case EndOfRoundEvent.WhiskeyForMarshal:
                roundEventText = "Whiskey For the Marshal";
                break;
            case EndOfRoundEvent.HigherSpeed:
                roundEventText = "Higher Speed";
                break;
            case EndOfRoundEvent.ShotgunRage:
                roundEventText = "Shotgun's Rage";
                break;

            //Arrival End of Round Event
            case EndOfRoundEvent.MarshalsRevenge:
                roundEventText = "Marshal's Revenge";
                break;
            case EndOfRoundEvent.Pickpocketing:
                roundEventText = "Pickpocketing";
                break;
            case EndOfRoundEvent.HostageConductor:
                roundEventText = "Hostage Conductor";
                break;
            case EndOfRoundEvent.SharingTheLoot:
                roundEventText = "Sharing the Loot";
                break;
            case EndOfRoundEvent.Escape:
                roundEventText = "Escape";
                break;
            case EndOfRoundEvent.MortalBullet:
                roundEventText = "Mortal Bullet";
                break;
            case EndOfRoundEvent.Null:
                roundEventText = "No Event";
                break;
        }

        eventLocation.GetComponent<TextMeshProUGUI>().text = roundEventText;

        Debug.Log("[UpdateCurrentRoundListener] Round: " + _currentRound);


    }
}
