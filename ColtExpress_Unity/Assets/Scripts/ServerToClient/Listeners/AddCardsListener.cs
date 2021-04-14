using CardSpace;
using Coffee.UIEffects;
using GameUnitSpace;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddCardsListener : UIEventListenable
{
    public GameObject cardIterator;
    public override void updateElement(string data)
    {
        /* PRE: data 
        *
                {
                    eventName = action,
                    cardsToAdd = (List<Card>)args[0]
                };
        */


        JObject o = JObject.Parse(data);
        
        JsonSerializer serializer = new JsonSerializer();
        serializer.Converters.Add(new ClientCommunicationAPIHandler.CardConverter());

       
        //Get list of JSON cards
        IEnumerable listOfCardTokens = o.SelectToken("cardsToAdd").Children();
        int count = 0;

        

        foreach (JToken c in listOfCardTokens)
        {
            //Deserialize each JSON card to an ActionCard or BulletCard
            Card card = c.ToObject<Card>(serializer);

            //Call the appropriate card object function
            if (card.GetType() == typeof(BulletCard))
            {
                Character? from = c.SelectToken("myPlayer").SelectToken("bandit").ToObject<Character>();
                BulletCard pewpewCard = c.ToObject<BulletCard>();
                if (from == null)
                {
                    GameUIManager.gameUIManagerInstance.createCardObject(null, pewpewCard.getNumBullets(), true);
                }
                else
                {

                    GameUIManager.gameUIManagerInstance.createCardObject(from, pewpewCard.getNumBullets(), true);
                }
            }
            else
            {
                GameUIManager.gameUIManagerInstance.createCardObject(NamedClient.c, ((ActionCard)card).getKind(), true);
            }
            count++;
        }

        if (GameUIManager.gameUIManagerInstance.actionBlocked.Item1 == true)
        {
            GameUIManager.gameUIManagerInstance.blockActionCards(GameUIManager.gameUIManagerInstance.actionBlocked.Item2);
        }

        if (cardIterator != null && count > 0)
        {
            cardIterator.GetComponent<UIShiny>().Play();
        }

        Debug.Log("[AddCardsListener] Cards added.");
        
    }
}
