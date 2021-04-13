using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClientCommunicationAPI;
using UnityEngine.UI;
using GameUnitSpace;

/* Author: Christina Pilip
 * Usage: Defines behaviour of the Phase 1 Turn Menu. 
 */

public class ScheminPhaseManager : MonoBehaviour
{
    public GameObject deck;
    public GameObject timer;
    public GameObject discardPile;

    public Button fullWhiskey;
    public Button normalWhiskey;
    public Button oldWhiskey;

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



    // Use a whiskey
    public void useWhiskey()
    {
        //Unlock sidebar and hide turn menu
        GameUIManager.gameUIManagerInstance.unlockSidebar();
        GameUIManager.gameUIManagerInstance.toggleTurnMenu(false);

        StartCoroutine("usingWhiskey");
    }
    private IEnumerator usingWhiskey()
    {
        // Fancy lambda logic for figuring out when the timer coroutine finishes and the player has timed out their turn
        bool timedOut = false;
        bool whiskeyUsed = false;

        //Retrieves Usables of player profile
        Transform usables = GameUIManager.gameUIManagerInstance.getPlayerProfileObject(NamedClient.c).transform.GetChild(3);
        fullWhiskey = usables.GetChild(0).GetComponent<Button>();
        normalWhiskey = usables.GetChild(1).GetComponent<Button>();
        oldWhiskey = usables.GetChild(2).GetComponent<Button>();

        OnWhiskeyUsed.wasWhiskeyUsed whiskeyWasUsed = delegate () { whiskeyUsed = true; };

        fullWhiskey.GetComponent<OnWhiskeyUsed>().notifyWhiskeyWasUsed += whiskeyWasUsed;
        normalWhiskey.GetComponent<OnWhiskeyUsed>().notifyWhiskeyWasUsed += whiskeyWasUsed;
        oldWhiskey.GetComponent<OnWhiskeyUsed>().notifyWhiskeyWasUsed += whiskeyWasUsed;

        StartCoroutine(timer.GetComponent<Timer>().waitForTimer(timedOut, value => timedOut = value));

        while (timedOut == false || whiskeyUsed == false)
        {

            if (timedOut || whiskeyUsed)
            {
                //Lock sidebar
                GameUIManager.gameUIManagerInstance.lockSidebar();

                timer.GetComponent<Timer>().resetTimer();

                if (whiskeyUsed)
                {
                    WhiskeyKind w = WhiskeyKind.Unknown;

                    //Find out which whiskey type was used
                    if (fullWhiskey.gameObject.GetComponent<OnWhiskeyUsed>().thisWhiskeyTypeUsed)
                    {
                        fullWhiskey.gameObject.GetComponent<OnWhiskeyUsed>().thisWhiskeyTypeUsed = false;
                        w = WhiskeyKind.Unknown;
                    } else if (normalWhiskey.gameObject.GetComponent<OnWhiskeyUsed>().thisWhiskeyTypeUsed)
                    {
                        normalWhiskey.gameObject.GetComponent<OnWhiskeyUsed>().thisWhiskeyTypeUsed = false;
                        w = WhiskeyKind.Normal;
                    }
                    else if (oldWhiskey.gameObject.GetComponent<OnWhiskeyUsed>().thisWhiskeyTypeUsed)
                    {
                        oldWhiskey.gameObject.GetComponent<OnWhiskeyUsed>().thisWhiskeyTypeUsed = false;
                        w = WhiskeyKind.Old;
                    }

                    Debug.Log("[ScheminPhaseManager - UseWhiskey] You used a whiskey [" + w + "].");
                    GameUIManager.gameUIManagerInstance.whiskeyWasUsed = true;

                    var definition = new
                    {
                        eventName = "WhiskeyMessage",
                        usedWhiskey = w
                    };

                    ClientCommunicationAPI.CommunicationAPI.sendMessageToServer(definition);
                }

                if (timedOut)
                {
                    Debug.Log("[ScheminPhaseManager - UseWhiskey] You timed out.");

                    var definition = new
                    {
                        eventName = "WhiskeyMessage",
                    };

                    ClientCommunicationAPI.CommunicationAPI.sendMessageToServer(definition);
                }

                yield break;
            }
            else
            {
                yield return null;
            }
        }

        Debug.LogError("[ScheminPhaseManager] Coroutine playingCard execution was borked (The player did not play a card *and* timed out. This should not happen!).");

        yield break;

    }

    // Draw and add three cards to the deck
    public void drawCard()
    {
        if (deck != null)
        {
            
            var definition = new
            {
                eventName = "DrawMessage"
            };

            ClientCommunicationAPI.CommunicationAPI.sendMessageToServer(definition);
            Debug.Log("[ScheminPhaseManager - DrawCard] Requested cards from server.");
            
        }
        //Hide the turn/whiskey menu and lock the hand
        GameUIManager.gameUIManagerInstance.toggleTurnMenu(false);
        GameUIManager.gameUIManagerInstance.lockHand();
    }

    //Play a card
    public void playCard()
    {
        //Unlock hand and hide turn menu
        GameUIManager.gameUIManagerInstance.unlockHand();
        GameUIManager.gameUIManagerInstance.toggleTurnMenu(false);

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
                //Lock hand
                GameUIManager.gameUIManagerInstance.lockHand();

                timer.GetComponent<Timer>().resetTimer();

                //Do not do StopAllCoroutines(). Learned that the hard way.
                if (cardPlayed)
                {

                    int i = playedCardsZone.transform.GetChild(playedCardsZone.transform.childCount - 1).gameObject.GetComponent<Draggable>().originalIndex;

                    Debug.Log("[ScheminPhaseManager - PlayCard] You played card " + i + ".");
                    var definition = new
                    {
                        eventName = "CardMessage",
                        index = i
                    };

                    ClientCommunicationAPI.CommunicationAPI.sendMessageToServer(definition);
                }

                if (timedOut)
                {
                    Debug.Log("[ScheminPhaseManager - PlayCard] You timed out.");

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
