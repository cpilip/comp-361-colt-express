using Newtonsoft.Json.Linq;
using RoundSpace;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateCurrentTurnListener : UIEventListenable
{
    private static GameObject previousTurn;
    private static TurnType previousTurnType;
    public GameObject playedCards;
    public override void updateElement(string data)
    {
        /* PRE: data 
        *
            {
                eventName = "updateCurrentTurn",
                currentTurn = i
            };
        */

        JObject o = JObject.Parse(data);
        int t = o.SelectToken("currentTurn").ToObject<int>();

        this.transform.GetChild(t).GetComponent<Image>().color = new Color(1.000f, 0.933f, 0.427f, 0.914f);

        //If true, turn before the listener was called was turmoil
        if (GameUIManager.gameUIManagerInstance.isTurmoilTurn)
        {
            //Grab last index of card
            int cardCount = playedCards.transform.childCount - 1;

            while (UpdateTopCardListener.turmoilCardsPlayed > 0)
            {
                GameObject card = playedCards.transform.GetChild(cardCount).gameObject;
                GameUIManager.gameUIManagerInstance.flipCardObject(card.GetComponent<CardID>().c, card.GetComponent<CardID>().kind, card);
                cardCount--;
                UpdateTopCardListener.turmoilCardsPlayed--;
            }

            UpdateTopCardListener.turmoilCardsPlayed = 0;
            GameUIManager.gameUIManagerInstance.isTurmoilTurn = false;
        }

        GameUIManager.gameUIManagerInstance.isNormalTurn = (this.transform.GetChild(t).gameObject.name == "Standard") ? true : false;
        GameUIManager.gameUIManagerInstance.isTunnelTurn = (this.transform.GetChild(t).gameObject.name == "Tunnel") ? true : false;

        if (this.transform.GetChild(t).gameObject.name == "Turmoil")
        {
            GameUIManager.gameUIManagerInstance.isTurmoilTurn = true;
        }

        //GameUIManager.gameUIManagerInstance.hasAnotherAction = (this.transform.GetChild(t).gameObject.name == "SpeedingUp") ? true : false;

        Debug.Log("[UpdateCurrentTurnListener] Turn: " + t);
        

        if (previousTurn == null)
        {
            previousTurn = this.transform.GetChild(t).gameObject;
        } 
        else
        {
            previousTurn.GetComponent<Image>().color = new Color(1.000f, 1f, 1f, 1f);
            previousTurn = this.transform.GetChild(t).gameObject;
        }

    }
}
