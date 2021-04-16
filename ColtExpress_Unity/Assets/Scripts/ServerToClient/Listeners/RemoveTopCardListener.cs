using Coffee.UIEffects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RemoveTopCardListener : UIEventListenable
{
    public GameObject playedCards;

    public override void updateElement(string data)
    {
        /* PRE: data 
        *
            {
                eventName = action
            };
        */

        GameObject topCard = playedCards.transform.GetChild(playedCards.transform.childCount - 1).gameObject;

        //Find the card before the first visible card and make it visible too
        if (playedCards.transform.childCount >= 8)
        {
            foreach (Transform t in playedCards.transform)
            {
                if (t.gameObject.activeSelf)
                {
                    playedCards.transform.GetChild(t.GetSiblingIndex() - 1).gameObject.SetActive(true);
                    break;
                }
            }
        }

        if (playedCards.transform.childCount < 7)
        {
            foreach (Transform t in playedCards.transform)
            {
                t.transform.gameObject.SetActive(true);
                break;
            }
        }


        Debug.Log("[RemoveTopCardListener] Popped top card " + topCard.GetComponent<CardID>().kind);
        DestroyImmediate(topCard);

        if (playedCards.transform.childCount == 0)
        {
            HightlightTopCardListener.pileFlipped = false;

            playedCards.GetComponent<Image>().color = new Color(1.000f, 1f, 1f, 0.392f);
        }

    }
}
