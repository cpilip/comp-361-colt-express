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
    public GameObject roundEventDescriptionPopup;
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
        

        //Initialize turns
        JObject o = JObject.Parse(data);
        List<TurnType> turns = o.SelectToken("turns").ToObject<List<TurnType>>();
        EndOfRoundEvent roundEvent = o.SelectToken("roundEvent").ToObject<EndOfRoundEvent>();
        int roundIndex = o.SelectToken("roundIndex").ToObject<int>();

        _currentRound = roundIndex;
        this.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "" + _currentRound;

        //Destroy all previous turn prefabs
        int childCount = turnsLocation.transform.childCount;
        while (childCount > 0)
        {
            GameObject.DestroyImmediate(turnsLocation.transform.GetChild(0).gameObject);
            childCount--;
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
        string popUpText = "";
        switch (roundEvent)
        {
            //Set event
            case EndOfRoundEvent.AngryMarshal:
                roundEventText = "Angry Marshal";
                popUpText = "The Marshal will shoot any players on the roof of his car at the end of this round, beware!";
                break;
            case EndOfRoundEvent.SwivelArm:
                roundEventText = "Swivel Arm";
                popUpText = "Each bandit located on a roof will be sent to the roof of the caboose at the end of this round!";
                break;
            case EndOfRoundEvent.Braking:
                roundEventText = "Braking";
                popUpText = "Each bandit located on a roof will move on car towards the locomotive at the end of this round!";
                break;
            case EndOfRoundEvent.TakeItAll:
                roundEventText = "Take It All!";
                popUpText = "A second Strongbox will appear in the Marshal's car at the end of this round, try to get to it first at the next round! ";
                break;
            case EndOfRoundEvent.PassengersRebellion:
                roundEventText = "Passengers' Rebellion";
                popUpText = "At the end of the round, each bandit located inside a TrainCar will receive a neutral bullet, beware!";
                break;
            case EndOfRoundEvent.PantingHorses:
                roundEventText = "Panting Horses";
                popUpText = "Depending on the number of bandits robbing the train, one or two horses will leave the train at the end of this round. If you want to go on a ride, do it now!";
                break;
            case EndOfRoundEvent.WhiskeyForMarshal:
                roundEventText = "Whiskey For the Marshal";
                popUpText = "The Marshal is getting a bit thirsty... At the end of this round, if there is a whiskey in his space, he will drink it and you better not be in his way...";
                break;
            case EndOfRoundEvent.HigherSpeed:
                roundEventText = "Higher Speed";
                popUpText = "At the end of this round, the train will speed up. All bandits, all horses and the Stagecoach will move by one car towards the caboose.";
                break;
            case EndOfRoundEvent.ShotgunRage:
                popUpText = "The Shotgun is getting pretty upset... At the end of this round, he will shoot every bandits on or in the Stagecoach, as well as bandits located on or in the adjacent car of the Stagecoach. Beware!";
                roundEventText = "Shotgun's Rage";
                break;

            //Arrival End of Round Event
            case EndOfRoundEvent.MarshalsRevenge:
                roundEventText = "Marshal's Revenge";
                popUpText = "At the end of this last round, each bandit on the roof of the Marshal's car looses his least valuable purse.";
                break;
            case EndOfRoundEvent.Pickpocketing:
                roundEventText = "Pickpocketing";
                popUpText = "Each bandit that is alone in his space may take 1 purse from his space at the end of this final round.";
                break;
            case EndOfRoundEvent.HostageConductor:
                roundEventText = "Hostage Conductor";
                popUpText = "Each bandit inside or on the roof of the locomotive receives one $250 purse at the end of this final round.";
                break;
            case EndOfRoundEvent.SharingTheLoot:
                roundEventText = "Sharing the Loot";
                popUpText = "At the end of this last round, if several bandits end up at the same location and some of them have strongboxes, they will share them together.";
                break;
            case EndOfRoundEvent.Escape:
                popUpText = "At the end of this final round, each bandit located on the train will get caught and sent to prison for several years... You might want to get off the train before the end of the round!";
                roundEventText = "Escape";
                break;
            case EndOfRoundEvent.MortalBullet:
                roundEventText = "Mortal Bullet";
                popUpText = "During this final round, each bullet that you received will give you a $150 penalty. Try not to get shot!";
                break;
            case EndOfRoundEvent.Null:
                roundEventText = "No Event";
                popUpText = "There is no event this round!";
                break;
        }

        roundEventDescriptionPopup.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = popUpText;
        eventLocation.GetComponent<TextMeshProUGUI>().text = roundEventText;

        Debug.Log("[UpdateCurrentRoundListener] Round: " + _currentRound);


    }

    public int getCurrentRound()
    {
        return _currentRound;
    }
    public void setCurrentRound(int c)
    {
        _currentRound = c;
    }


}
