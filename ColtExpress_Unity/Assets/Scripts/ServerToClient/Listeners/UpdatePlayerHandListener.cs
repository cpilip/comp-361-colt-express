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
            foreach (Transform c in GameUIManager.gameUIManagerInstance.deck.transform)
            {
                Destroy(c.gameObject);
            }

            //Get list of JSON cards
            IEnumerable listOfCardTokens = o.SelectToken("cardsToAdd").Children();

            if (o.SelectToken("cardsToAdd").HasValues)
            {
                foreach (JToken c in listOfCardTokens)
                {
                    //Deserialize each JSON card to an ActionCard or BulletCard
                    Card card = c.ToObject<Card>(serializer);

                    //Call the appropriate card object function
                    if (card.GetType() == typeof(BulletCard))
                    {
                        BulletCard pewpewCard = c.ToObject<BulletCard>();
                        if (pewpewCard.belongsTo() == null)
                        {
                            GameUIManager.gameUIManagerInstance.createCardObject(null, pewpewCard.getNumBullets(), true);
                        } 
                        else
                        {

                            GameUIManager.gameUIManagerInstance.createCardObject(pewpewCard.belongsTo().getBandit(), pewpewCard.getNumBullets(), true);
                        }

                    }
                    else
                    {
                        GameUIManager.gameUIManagerInstance.createCardObject(player, ((ActionCard)card).getKind(), true);
                    }
                }


                Debug.Log(GameUIManager.gameUIManagerInstance.deck.transform.childCount);

                int cardsToShow = 6;
                if (GameUIManager.gameUIManagerInstance.deck.transform.childCount < 6)
                {
                    cardsToShow = GameUIManager.gameUIManagerInstance.deck.transform.childCount;
                }

                //Activate the six cards
                for (int i = 0; i < cardsToShow; i++)
                {
                    GameUIManager.gameUIManagerInstance.deck.transform.GetChild(i).gameObject.SetActive(true);

                }

            }

            //Block any action cards that are Ride or Punch if the client has the appropriate hostage
            if (GameUIManager.gameUIManagerInstance.actionBlocked.Item1 == true)
            {
                GameUIManager.gameUIManagerInstance.blockActionCards(GameUIManager.gameUIManagerInstance.actionBlocked.Item2);
            }

            Debug.Log("[UpdatePlayerHandListener] Player hand updated for " + player + ".");
        }
    }

}



