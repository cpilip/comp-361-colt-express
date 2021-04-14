using Coffee.UIEffects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HightlightTopCardListener : UIEventListenable
{
    public GameObject playedCards;
    public static bool pileFlipped = false;
    public override void updateElement(string data)
    {
        /* PRE: data 
        *
            {
                eventName = action,
            };
        */

        if (pileFlipped == false)
        {
            Debug.Log("PILE FLIPPED");
            for (var i = 0; i < playedCards.transform.childCount - 1; i++)
            {
                playedCards.transform.GetChild(0).gameObject.SetActive(false);
                playedCards.transform.GetChild(0).SetSiblingIndex(playedCards.transform.childCount - 1 - i);
            }
            playedCards.transform.GetChild(0).gameObject.SetActive(false);

            pileFlipped = true;

            if (playedCards.transform.childCount >= 8)
            {
                for (int i = playedCards.transform.childCount - 1; i > playedCards.transform.childCount - 7; i--)
                {
                    playedCards.transform.GetChild(i).gameObject.SetActive(true);
                }
            } else
            {
                foreach (Transform t in playedCards.transform)
                {
                    t.gameObject.SetActive(true);
                }
            }

            playedCards.GetComponent<Image>().color = new Color(1.000f, 0.812f, 0.357f, 0.392f);
        }

        GameObject topCard = playedCards.transform.GetChild(playedCards.transform.childCount - 1).gameObject;
        Debug.Log("[HighlightTopCardListener] " + topCard.GetComponent<CardID>().kind + " " + topCard.GetComponent<CardID>().isHidden);

        if (topCard.GetComponent<CardID>().isHidden)
        {
            GameUIManager.gameUIManagerInstance.flipCardObject(topCard.GetComponent<CardID>().c, topCard.GetComponent<CardID>().kind, topCard);
            //Debug.LogError("card flipped");
            topCard.GetComponent<CardID>().isHidden = false;
        }

        topCard.GetComponent<UIShiny>().enabled = true;

        Debug.Log("[HighlightTopCardListener] Highlighted top card.");
    }
}
