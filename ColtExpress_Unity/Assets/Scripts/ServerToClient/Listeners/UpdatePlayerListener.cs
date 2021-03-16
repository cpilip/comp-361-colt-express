using CardSpace;
using GameUnitSpace;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatePlayerListener : UIEventListenable
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
        // PRE: data = Player packet
        Debug.Log("Making PLAYER!");
        JObject o = JObject.Parse(data);
        Character player = o.SelectToken("player").ToObject<Character>();

        /*
        
        var definition = new
        {
            eventName = action,
            player = n.getBandit(),
            h_ActionCards = n.getHand_actionCards(),
            h_BulletCards = n.getHand_bulletCards(),
            //d_ActionCards = n.getDiscard_actionCards(),
            //d_BulletCards = n.getDiscard_bulletCards()
        };
        
         */

        GameUIManager.gameControllerInstance.createPlayerProfileObject(player);

        if (player == NamedClient.c)
        {

            //INITIALIZE CARDS
            List<ActionKind> h_a = o.SelectToken("h_ActionCards").ToObject<List<ActionKind>>();

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
