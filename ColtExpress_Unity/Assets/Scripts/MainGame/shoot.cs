using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class shoot : MonoBehaviour
{
    public GameObject player;
    public GameObject train;
    public GameObject parentButton;
    public GameObject played;
    public GameObject discard;
    public GameObject bulletPile;
    public GameObject otherPlayerDeck;

    public void shootOther() {
        int parentIndex = player.transform.parent.transform.GetSiblingIndex();

        if (parentIndex >= 0 && parentIndex <= 6) {
            for (int i = 0; i <= 6; i++) {
                if (i != parentIndex) {
                    if (train.transform.GetChild(i).childCount > 0)
                    {
                        if (train.transform.GetChild(i).GetChild(0) != null)
                        {
                            if (bulletPile.transform.GetChild(0) != null)
                            {
                                (bulletPile.transform.GetChild(0)).SetParent(otherPlayerDeck.transform);
                                break;
                            }
                        }
                    }
                }

            }
        } else if (parentIndex >=7 && parentIndex <=13) {
            if (parentIndex == 7)
            {
                if (train.transform.GetChild(8).childCount > 0)
                {
                    if (train.transform.GetChild(8).GetChild(0) != null)
                    {
                        if (bulletPile.transform.GetChild(0) != null)
                        {
                            (bulletPile.transform.GetChild(0)).SetParent(otherPlayerDeck.transform);

                        }
                    }
                }
            }
            else if (parentIndex == 13)
            {
                if (train.transform.GetChild(12).childCount > 0)
                {
                    if (train.transform.GetChild(12).GetChild(0) != null)
                    {
                        if (bulletPile.transform.GetChild(0) != null)
                        {
                            (bulletPile.transform.GetChild(0)).SetParent(otherPlayerDeck.transform);

                        }
                    }

                }
            }
            else
            {
                if(train.transform.GetChild(parentIndex + 1).childCount >0 || train.transform.GetChild(parentIndex - 1).childCount > 0) {
                   // if (train.transform.GetChild(parentIndex + 1).GetChild(0) != null || train.transform.GetChild(parentIndex - 1).GetChild(0) != null)
                    
                        if (bulletPile.transform.GetChild(0) != null)
                        {
                            (bulletPile.transform.GetChild(0)).SetParent(otherPlayerDeck.transform);

                        }
                    
                }
            }

        }

        played.transform.GetChild(0).SetParent(discard.transform);
        parentButton.SetActive(false);
    }
}
