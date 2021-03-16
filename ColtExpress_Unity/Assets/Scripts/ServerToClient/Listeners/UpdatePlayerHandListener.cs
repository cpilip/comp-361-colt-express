using CardSpace;
using GameUnitSpace;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatePlayerHandListener : UIEventListenable
{
    public GameObject deck;
    public GameObject actionCardPrefabMove;
    public GameObject actionCardPrefabShoot;
    public GameObject actionCardPrefabRob;
    public GameObject actionCardPrefabPunch;
    public GameObject actionCardPrefabChangeFloor;
    public GameObject actionCardPrefabMarshal;
    public override void updateElement(string data)
    {
        Debug.Log("Making CARD HAND!");
        JObject o = JObject.Parse(data);
        Character player = o.SelectToken("currentPlayerIndex").ToObject<Character>();

        if (player == NamedClient.c)
    {
            // PRE: data = Player packet
            


            //INITIALIZE CARDS

            /*var definition = new
            {
            eventName = "updatePlayerHand",
            currentPlayerIndex = character
            cardsToAdd = c_a 
            };*/

            List<ActionKind> h_a = o.SelectToken("cardsToAdd").ToObject<List<ActionKind>>();

            foreach (ActionKind a in h_a)
            {
                switch (a)
                {
                    case ActionKind.Move:
                        Instantiate(actionCardPrefabMove).transform.SetParent(deck.transform);
                        break;

                    case ActionKind.Shoot:
                        Instantiate(actionCardPrefabShoot).transform.SetParent(deck.transform);
                        break;
                    case ActionKind.Rob:
                        Instantiate(actionCardPrefabRob).transform.SetParent(deck.transform);
                        break;
                    case ActionKind.Marshal:
                        Instantiate(actionCardPrefabMarshal).transform.SetParent(deck.transform);
                        break;
                    case ActionKind.Punch:
                        Instantiate(actionCardPrefabPunch).transform.SetParent(deck.transform);
                        break;
                    case ActionKind.ChangeFloor:
                        Instantiate(actionCardPrefabChangeFloor).transform.SetParent(deck.transform);
                        break;
                    default:
                        break;
                }
            }
        }
    }

}



