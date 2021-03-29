using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateGameStatusListener : UIEventListenable
{
    public override void updateElement(string data)
    {
        /* PRE: data 
        *
            {
                eventName = "updateGameStatus",
                statusIs = true if Schemin', false if Stealin,
            };
        */

        JObject o = JObject.Parse(data);
        bool status = o.SelectToken("statusIs").ToObject<bool>();

        if (status)
        {
            this.transform.GetChild(0).gameObject.SetActive(true);
        } else
        {
            this.transform.GetChild(0).gameObject.SetActive(false);
        }
        GameUIManager.gameUIManagerInstance.gameStatus = status;

        string gameStatus = (status) ? "SCHEMIN" : "STEALING";
        Debug.Log("[UpdateGameStatusListener] Status: " + gameStatus);
    }
}
