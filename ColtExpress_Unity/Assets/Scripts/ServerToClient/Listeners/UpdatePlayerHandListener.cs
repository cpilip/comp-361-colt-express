using CardSpace;
using GameUnitSpace;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatePlayerHandListener : UIEventListenable
{
    public override void updateElement(string data)
    {
        /* PRE: data 
        *
                {
                    eventName = "updatePlayerHand",
                    player = c,
                    cardsToAdd = l
                };
        */

        JObject o = JObject.Parse(data);
        Character player = o.SelectToken("player").ToObject<Character>();

        JsonSerializer serializer = new JsonSerializer();
        serializer.Converters.Add(new ClientCommunicationAPIHandler.CardConverter());

        if (player == NamedClient.c)
        {
            //Clear the old hand
            foreach (GameObject c in GameUIManager.gameUIManagerInstance.deck.transform)
            {
                Destroy(c);
            }

            //Get list of JSON cards
            IEnumerable listOfCardTokens = o.SelectToken("cardsToAdd").Children();

            foreach (JToken c in listOfCardTokens)
            {
                //Deserialize each JSON card to an ActionCard or BulletCard
                Card card = c.ToObject<Card>(serializer);
                
                //Call the appropriate card object function
                if (card.GetType() == typeof(BulletCard))
                {
                    //TODO PLAYER MUST BE FROM PLAYER WHO SHOT
                    GameUIManager.gameUIManagerInstance.createCardObject(player, ((BulletCard)card).getNumBullets(), true);
                } else
                {
                    GameUIManager.gameUIManagerInstance.createCardObject(player, ((ActionCard)card).getKind(), true);
                }
            }

            //Activate the six cards
            for (int i = 0; i < 6; i++) 
            {
                GameUIManager.gameUIManagerInstance.deck.transform.GetChild(i).gameObject.SetActive(true);
            }

            Debug.Log("[UpdatePlayerHandListener] Player hand updated for " + player + ".");
        }
    }

}



