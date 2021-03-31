using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClientCommunicationAPI;

/* Author: Christina Pilip
 * Usage: Defines behaviour of the Phase 1 Turn Menu. 
 */

// TODO: Unfinished, more for an initial implementation.
public class ScheminPhaseManager : MonoBehaviour
{
    public GameObject deck;
    public GameObject timer;
    public GameObject discardPile;

    private GameObject playedCardsZone;

    public static List<GameObject> clientHand = new List<GameObject>();
    public static List<GameObject> clientDiscardPile = new List<GameObject>();

    void Start()
    {
        playedCardsZone = deck.transform.parent.GetChild(0).gameObject;
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

                    int i = clientHand.IndexOf(playedCardsZone.transform.GetChild(playedCardsZone.transform.childCount - 1).gameObject);
                    Debug.Log(i);

                    var definition = new
                    {
                        eventName = "CardMessage",
                        index = i
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
