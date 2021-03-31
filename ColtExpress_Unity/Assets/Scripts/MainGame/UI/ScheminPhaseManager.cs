﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClientCommunicationAPI;

/* Author: Christina Pilip
 * Usage: Defines behaviour of the Phase 1 Turn Menu. 
 */

public class ScheminPhaseManager : MonoBehaviour
{
    public GameObject deck;
    public GameObject timer;
    public GameObject discardPile;

    private GameObject playedCardsZone;

    private static int firstDisplayedCardIndex;
    private static int endOfBlock;
    private bool alreadyInFirst;

    void Start()
    {
        firstDisplayedCardIndex = 0;
        endOfBlock = 0;
        alreadyInFirst = true;
        playedCardsZone = deck.transform.parent.GetChild(0).gameObject;
    }


    public void iterateCards()
    {
        //Always displays all cards if there are <= 6 (the max amount that can be displayed)
        //Otherwise, there must be > 6 cards
        if (deck.transform.childCount <= 6)
        {
            foreach (Transform c in deck.transform)
            {
                c.gameObject.SetActive(true);
            }
        } else
        {
            //Hide all cards - better way to optimize?
            foreach (Transform c in deck.transform)
            {
                c.gameObject.SetActive(false);
            }
            
            //Jump to next block if already in the first block (fixes needing to double click iterator initially)
            if (alreadyInFirst)
            {
                alreadyInFirst = false;
                firstDisplayedCardIndex = firstDisplayedCardIndex + 6;

            }

            //If in in first block, display first six cards - recall, there must be > 6 cards at this point, so we can safely displayed (0, 5).
            if (firstDisplayedCardIndex == 0)
            {
                endOfBlock = firstDisplayedCardIndex + 5;
                for (int i = firstDisplayedCardIndex; i <= endOfBlock; i++)
                {
                    deck.transform.GetChild(i).gameObject.SetActive(true);
                }
                firstDisplayedCardIndex = firstDisplayedCardIndex + 6;
            //Otherwise, we must be in a consecutive block - firstDisplayedCardIndex is = 6, 12, etc...
            } else
            {
                //The end index of that block will be 
                endOfBlock = firstDisplayedCardIndex + 5;

                /*  Check whether this is the last block; the last block will have <= 6 cards, and we'll need to loop back to the first block
                *   Ex. if we have cards (0, 17), there are 18 cards
                *       firstDisplayedCardIndex = 12
                *       endOfBlock = 12 + 5 = 17
                *       (18 - 1) == 17, so display (12, 17), or the last 6 cards
                *   Ex. if we have cards (0, 16), there are 17 cards
                *       firstDisplayedCardIndex = 12
                *       endOfBlock = 12 + 5 = 17
                *       (17 - 1) < 17, so display (12, 16), or the last 5 cards
                *   Ex. if we have cards (0, 16), there are 17 cards
                *       firstDisplayedCardIndex = 6
                *       endOfBlock = 6 + 5 = 11
                *      (17 - 1) > 11, so display (6, 11) because this is not the last block
                */
                if (endOfBlock >= (deck.transform.childCount - 1))
                {
                    for (int i = firstDisplayedCardIndex; i <= (deck.transform.childCount - 1); i++)
                    {
                        deck.transform.GetChild(i).gameObject.SetActive(true);
                    }
                    firstDisplayedCardIndex = 0;
                }
                else
                {
                    for (int i = firstDisplayedCardIndex; i <= endOfBlock; i++)
                    {
                        deck.transform.GetChild(i).gameObject.SetActive(true);
                    }
                    firstDisplayedCardIndex = firstDisplayedCardIndex + 6;
                }
            }
        }
    }


    // Draw and add three cards to the deck
    public void drawCard()
    {
        if (deck != null)
        {
            /*
            for (int i = 0; i < 3; i++)
            {
                // Create a new subdeck if the last one is full
                if (lastSubdeck.childCount >= 6)
                {
                    GameObject newSubdeck = Instantiate(subdeckPrefab);
                    // Set properties
                    newSubdeck.transform.SetParent(deck.transform);
                    newSubdeck.transform.position = lastSubdeck.transform.position;
                    newSubdeck.transform.localScale = new Vector3(1, 1, 1);


                    // Disable view of the currently displayed subdeck
                    deck.transform.GetChild(displayedSubdeckNum - 1).gameObject.SetActive(false);

                    // Display and update to the new subdeck
                    lastSubdeck = newSubdeck.transform;
                    displayedSubdeckNum = deck.transform.childCount;
                }

                // Create a new card; no other values are changed, and the card prefab will definitely be altered at a later
                // date when we model low-level implementation
                //GameObject newCard = Instantiate(cardPrefab);

                // Put the card in the last subdeck

                if (discardPile.transform.childCount == 0)
                {
                    break;
                }
                Transform newCard = discardPile.transform.GetChild(0);

                newCard.SetParent(lastSubdeck.transform);
                //newCard.localScale = new Vector3(1, 1, 1);

            }

            // Display and update the currently displayed subdeck
            deck.transform.GetChild(displayedSubdeckNum - 1).gameObject.SetActive(false);
            displayedSubdeckNum = deck.transform.childCount;
            deck.transform.GetChild(displayedSubdeckNum - 1).gameObject.SetActive(true);

        }
            */
        }
        //Hide the turn menu and lock the hand
        GameUIManager.gameUIManagerInstance.toggleTurnMenu(false);
        GameUIManager.gameUIManagerInstance.lockHand();
    }

    public void playCard()
    {
        GameUIManager.gameUIManagerInstance.unlockHand();

        StartCoroutine("playingCard");
    }

    private IEnumerator playingCard()
    {
        // Fancy lambda logic for figuring out when the timer coroutine finishes and the player has timed out their turn
        bool timedOut = false;
        bool cardPlayed = false;

        OnChildrenUpdated.wasChildChanged cardWasPlayed = delegate () { cardPlayed = true; };
        playedCardsZone.GetComponent<OnChildrenUpdated>().notifyChildWasChanged += cardWasPlayed;

        StartCoroutine(timer.GetComponent<Timer>().waitForTimer(timedOut, value => timedOut = value));

        while (timedOut == false || cardPlayed == false)
        {
            
            if (timedOut || cardPlayed)
            {
                //Hide turn menu and lock the hand
                GameUIManager.gameUIManagerInstance.toggleTurnMenu(false);
                GameUIManager.gameUIManagerInstance.lockHand();

                timer.GetComponent<Timer>().resetTimer();

                //Do not do StopAllCoroutines(). Learned that the hard way.
                if (cardPlayed)
                {
                    Debug.Log("[ScheminPhaseManager] You played a card.");

                    //int i = clientHand.IndexOf(playedCardsZone.transform.GetChild(playedCardsZone.transform.childCount - 1).gameObject);
                    //Debug.Log(i);

                    var definition = new
                    {
                        eventName = "CardMessage",
                        index = 0
                    };

                    ClientCommunicationAPI.CommunicationAPI.sendMessageToServer(definition);
                }

                if (timedOut)
                {
                    Debug.Log("[ScheminPhaseManager] You timed out.");

                    var definition = new
                    {
                        eventName = "CardMessage",
                        index = -1
                    };

                    ClientCommunicationAPI.CommunicationAPI.sendMessageToServer(definition);
                }

                yield break;
            } else
            {
                yield return null;
            }
        }

        Debug.LogError("[ScheminPhaseManager] Coroutine playingCard execution was borked (The player did not play a card *and* timed out. This should not happen!).");

        yield break;

    }

}
