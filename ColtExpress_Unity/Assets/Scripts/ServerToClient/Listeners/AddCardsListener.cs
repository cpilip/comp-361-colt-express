using CardSpace;
using Coffee.UIEffects;
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

        if (cardIterator != null)
        {
            cardIterator.GetComponent<UIShiny>().Play();
        }

        JObject o = JObject.Parse(data);
        
        JsonSerializer serializer = new JsonSerializer();
        serializer.Converters.Add(new ClientCommunicationAPIHandler.CardConverter());

       
        //Get list of JSON cards
        IEnumerable listOfCardTokens = o.SelectToken("cardsToAdd").Children();

        foreach (JToken c in listOfCardTokens)
        {
            //Deserialize each JSON card to an ActionCard or BulletCard
            Card card = c.ToObject<Card>(serializer);

            //Call the appropriate card object function
            GameUIManager.gameUIManagerInstance.createCardObject(NamedClient.c, ((ActionCard)card).getKind(), true);

        }

        Debug.Log("[AddCardsListener] Cards added.");
        
    }
}
