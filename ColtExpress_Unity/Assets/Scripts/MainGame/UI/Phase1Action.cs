using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phase1Action : MonoBehaviour
{
    public GameObject deck;
    public GameObject cardPrefab;
    public GameObject subdeckPrefab;
    public GameObject turnMenu;
    public GameObject handBlocker;

    private Transform currSubdeck;
    private int currSubdeckNum;

    void Start()
    {
        currSubdeckNum = 1;
        currSubdeck = deck.transform.GetChild(deck.transform.childCount - 1);
    }

    // Draw and add three cards to the deck
    public void drawCard()
    {
        if (deck != null)
        {
            for (int i = 0; i < 3; i++)
            {
                // Create a new subdeck if the current one is full
                if (currSubdeck.childCount >= 6)
                {
                    GameObject newSubdeck = Instantiate(subdeckPrefab);
                    newSubdeck.transform.SetParent(deck.transform);
                    newSubdeck.transform.position = currSubdeck.transform.position;
                    newSubdeck.transform.localScale = new Vector3(1, 1, 1);


                    // Disable view of the displayed subdeck
                    deck.transform.GetChild(currSubdeckNum - 1).gameObject.SetActive(false);

                    // Update to the current subdeck
                    currSubdeck = newSubdeck.transform;
                    currSubdeckNum = deck.transform.childCount;
                }

                // Create a new card; no other values are changed, and the card prefab will definitely be altered at a later
                // date when we model low-level implementation
                GameObject newCard = Instantiate(cardPrefab);

                // Put the card in the deck
                newCard.transform.SetParent(currSubdeck.transform);
                newCard.transform.localScale = new Vector3(1, 1, 1);

            }

            deck.transform.GetChild(currSubdeckNum - 1).gameObject.SetActive(false);
            currSubdeckNum = deck.transform.childCount;
            deck.transform.GetChild(currSubdeckNum - 1).gameObject.SetActive(true);

        }


        //Disable the turn menu
        //toggleTurnMenu();
    }



    public void playCard()
    {
        if (handBlocker != null)
        {
            handBlocker.SetActive(!handBlocker.activeSelf);
        }

    } 

    public void nextSubdeck()
    {

        // 2 subdecks
        //we're on subdeck 2 (the first)

        //Debug.Log("Display: " + currSubdeckNum);
        //Debug.Log(deck.transform.GetChild(currSubdeckIndex).gameObject + " at index " + currSubdeckIndex + " is the current subdeck.");

        currSubdeckNum--;

        // now index 1
        //Debug.Log("Display index (to disable): " + currSubdeckNum);
        deck.transform.GetChild(currSubdeckNum).gameObject.SetActive(false);
        //Debug.Log("Display index (to enable):" + (currSubdeckNum + 1) % deck.transform.childCount);
        deck.transform.GetChild((currSubdeckNum + 1) % deck.transform.childCount).gameObject.SetActive(true);

        currSubdeckNum = (currSubdeckNum + 1) % deck.transform.childCount + 1;

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
