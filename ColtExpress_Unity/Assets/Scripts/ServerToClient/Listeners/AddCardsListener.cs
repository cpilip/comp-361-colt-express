using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddCardsListener : UIEventListenable
{
    public override void updateElement(string data)
    {
        /* PRE: data 
        *
                {
                    eventName = "addCards",
                    c1 = ActionKind
                    c2 = ActionKind
                    c3 = ActionKind
                };
        */

        JObject o = JObject.Parse(data);
        
    }
}
