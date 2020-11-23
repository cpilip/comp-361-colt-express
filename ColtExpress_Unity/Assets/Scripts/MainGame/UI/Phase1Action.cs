using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Author: Christina Pilip
 * Usage: Defines behaviour of the Phase 1 Turn Menu. 
 */

// TODO: Unfinished, more for an initial implementation.
public class Phase1Action : MonoBehaviour
{
    public GameObject deck;
    public GameObject cardPrefab;
    public GameObject subdeckPrefab;
    public GameObject turnMenu;
    public GameObject timer;
    public GameObject handBlocker;

    private Transform lastSubdeck;
    private int displayedSubdeckNum;
    private GameObject playedCardsZone;

    void Start()
    {
        displayedSubdeckNum = 1;
        lastSubdeck = deck.transform.GetChild(deck.transform.childCount - 1);

        playedCardsZone = deck.transform.GetChild(0).gameObject;
    }

    // Draw and add three cards to the deck
    public void drawCard()
    {
        if (deck != null)
        {
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
                GameObject newCard = Instantiate(cardPrefab);

                // Put the card in the last subdeck
                newCard.transform.SetParent(lastSubdeck.transform);
                newCard.transform.localScale = new Vector3(1, 1, 1);

            }

            // Display and update the currently displayed subdeck
            deck.transform.GetChild(displayedSubdeckNum - 1).gameObject.SetActive(false);
            displayedSubdeckNum = deck.transform.childCount;
            deck.transform.GetChild(displayedSubdeckNum - 1).gameObject.SetActive(true);

        }

        //Disable the turn menu
        toggleTurnMenu();
    }


    //TODO: Figure out how to update subdecks

    public void playCard()
    {
        if (handBlocker != null)
        {
            handBlocker.SetActive(!handBlocker.activeSelf);
        }

        StartCoroutine("startTimer");
    }
    private IEnumerator startTimer()
    {
        // Fancy lambda logic for figuring out when the timer coroutine finishes and the player has timed out their turn
        bool timedOut = false;
        yield return StartCoroutine(timer.GetComponent<Timer>().waitForTimer(timedOut, value => timedOut = value));

        if (timedOut)
        {
            Debug.Log("Player timed out or played a card.");
            toggleTurnMenu();

            if (handBlocker != null)
            {
                handBlocker.SetActive(!handBlocker.activeSelf);
            }

            StopAllCoroutines();
        }
        

    }

    public void nextSubdeck()
    {
        // Correct the currently displayed subdeck for proper indexing
        displayedSubdeckNum--;

        // Display the next subdeck
        deck.transform.GetChild(displayedSubdeckNum).gameObject.SetActive(false);
        deck.transform.GetChild((displayedSubdeckNum + 1) % deck.transform.childCount).gameObject.SetActive(true);

        // Revert the index change
        displayedSubdeckNum = (displayedSubdeckNum + 1) % deck.transform.childCount + 1;

    }

    // Toggle turn menu
    private void toggleTurnMenu()
    {
        if (turnMenu != null)
        {
            turnMenu.SetActive(!turnMenu.activeSelf);
        }
    }
}
