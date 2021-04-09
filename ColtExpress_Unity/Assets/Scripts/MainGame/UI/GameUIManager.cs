using CardSpace;
using GameUnitSpace;
using HostageSpace;
using PositionSpace;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    //Game object locations for major scripts
    public GameObject gameControllerLocation;
    public GameObject playerProfileLocation;
    public GameObject trainLocation;
    public GameObject deck;
    public GameObject playedCards;
    public GameObject discardPile;
    public GameObject hostagesList;

    public GameObject shotgun;
    public GameObject stagecoach;

    //Menus
    public GameObject turnMenu;
    public GameObject hostageMenu;

    //Blockers
    public GameObject boardBlocker;
    public GameObject handBlocker;
    public GameObject sidebarBlocker;

    //Prefabs
    public GameObject characterPrefab;
    public GameObject playerProfilePrefab;
    public GameObject actionCardPrefab;
    public GameObject bulletCardPrefab;

    //We treat a player's bandit (which is unique) as the corresponding player
    //Player : in-game character object map
    //Player profile : in-game character object map
    //Train car index : train car map
    int numPlayers = 0;
    private Dictionary<Character, GameObject> characters = new Dictionary<Character, GameObject>();
    private Dictionary<Character, GameObject> playerProfiles = new Dictionary<Character, GameObject>();
    private Dictionary<int, GameObject> trainCars = new Dictionary<int, GameObject>();
    private Dictionary<HostageChar, GameObject> hostageMap = new Dictionary<HostageChar, GameObject>();

    //EventManager instance, game status, has another action status
    private static GameUIManager gameUIManager;
    public GameStatus gameStatus;

    //Other important information
    public bool isNormalTurn = false;
    public bool isTunnelTurn = false;
    public bool whiskeyWasUsed = false;

    //Loaded sprites
    public static Dictionary<string, Sprite> loadedSprites = new Dictionary<string, Sprite>();

    private Vector3 scale = new Vector3(1f, 1f, 1f);

    public static GameUIManager gameUIManagerInstance
    {
        get
        {
            return gameUIManager;
        }
    }

    public GameObject getHostage(HostageChar h)
    {
        GameObject retrievedHostageObject = null;
        hostageMap.TryGetValue(h, out retrievedHostageObject);
        return retrievedHostageObject;
    }

    public void clearHostages()
    {
        List<GameObject> flattenList = hostageMap.Values.ToList();

        foreach (GameObject t in flattenList)
        {
            t.SetActive(false);
        }
    }

    public void clearMovePositions()
    {
        List<int> flattenList = trainCars.Keys.ToList();

        foreach (int i in flattenList)
        {
            Image image = getTrainCarPosition(i, true).GetComponent<Image>();
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0f);

            getTrainCarPosition(i, true).GetComponent<Button>().enabled = false;

            image = getTrainCarPosition(i, false).GetComponent<Image>();
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0f);

            getTrainCarPosition(i, false).GetComponent<Button>().enabled = false;

        }

    }

    public (bool, int) getTrainCarIndexByPosition(GameObject trainCarPosition)
    {
        List<int> flattenList = trainCars.Keys.ToList();

        foreach (int i in flattenList)
        {
            //If passed train car position is the same as the roof
            if (GameObject.ReferenceEquals(trainCarPosition, getTrainCarPosition(i, true)))
            {
                return (false, i);
            }
            //Not the roof
            else if (GameObject.ReferenceEquals(trainCarPosition, getTrainCarPosition(i, false)))
            {
                //Reverse on server - true if inside
                return (true, i);
            }
        }

        return (true, -1);
    }

    //Get corresponding in-game character object to a character
    public GameObject getCharacterObject(Character c)
    {
        GameObject requestedPlayer = null;
        characters.TryGetValue(c, out requestedPlayer);
        return requestedPlayer;
    }

    //Create a new in-game character object
    public GameObject createCharacterObject(Character c)
    {
        GameObject newPlayer = Instantiate(characterPrefab);
        Sprite newPlayerSprite = null;

        //Get the right character sprite
        loadedSprites.TryGetValue(c.ToString().ToLower() + "_character", out newPlayerSprite);
        newPlayer.GetComponent<Image>().sprite = newPlayerSprite;

        //Fixing the scale
        newPlayer.transform.localScale = scale;

        characters.Add(c, newPlayer);
        return newPlayer;
    }

    //Get corresponding player profile to a player
    public GameObject getPlayerProfileObject(Character c)
    {
        GameObject requestedPlayer = null;
        playerProfiles.TryGetValue(c, out requestedPlayer);
        return requestedPlayer;
    }

    //Create a new player profile
    public GameObject createPlayerProfileObject(Character c)
    {
        GameObject newPlayerProfile = Instantiate(playerProfilePrefab);
        Sprite newPlayerProfilePortrait = null;

        //Get the right portrait
        loadedSprites.TryGetValue(c.ToString().ToLower() + "_portrait", out newPlayerProfilePortrait);
        newPlayerProfile.transform.GetChild(0).GetComponent<Image>().sprite = newPlayerProfilePortrait;

        //Making sure to parent the profile under the profile list and fixing the scale
        newPlayerProfile.transform.SetParent(playerProfileLocation.transform);
        newPlayerProfile.transform.localScale = scale;

        playerProfiles.Add(c, newPlayerProfile);
        numPlayers++;
        return newPlayerProfile;
    }

    //Initialize the train - we make sure the last index mapped is set to the caboose and the caboose's position is updated
    public GameObject initializeTrainCar(int index)
    {
        GameObject trainCar = null;
        trainCars.TryGetValue(index, out trainCar);

        if (index == numPlayers)
        {
            //Retrieve the caboose
            GameObject caboose = null;
            trainCars.TryGetValue(6, out caboose);

            //Debug.Log(caboose.name);

            //Change the caboose coordinates to be the new end of the train
            Vector3 lastTrainCarCoordinates = trainCar.transform.position;

            //Debug.Log(caboose.name + " previously at " + caboose.transform.position);

            caboose.transform.position = lastTrainCarCoordinates;

           // Debug.Log(caboose.name + " now at " + lastTrainCarCoordinates);

            //Disable the rest of the cars and remove them from the map
            for (int i = index; i <= 5; i++)
            {
                trainCars.TryGetValue(i, out trainCar);
                trainCar.SetActive(false);
                trainCars.Remove(i);

                //Debug.Log("Removed " + trainCar.name + " at " + i);
            }

            //Remove the caboose mapping without disabling it
            trainCars.Remove(6);

            //Debug.Log("Removed " + caboose.name + " at " + 6);

            //Replace the last train car's index with the caboose in the map
            trainCars.Add(index, caboose);
            //Debug.Log("Added " + caboose.name + " at " + index);

        }

        //Re-retrieve train car (in case caboose was updated)
        trainCars.TryGetValue(index, out trainCar);
        return trainCar;
    }

    //Get a train car
    public GameObject getTrainCar(int index)
    {
        GameObject trainCar = null;
        trainCars.TryGetValue(index, out trainCar);
        return trainCar;
    }

    //Get a train car's position - true for its roof, false for its interior
    public GameObject getTrainCarPosition(int index, bool isRoof)
    {
        GameObject trainCar = null;
        trainCars.TryGetValue(index, out trainCar);

        if (isRoof)
        {
            return trainCar.transform.GetChild(1).gameObject;
        } else
        {
            return trainCar.transform.GetChild(2).gameObject;
        }

    }

    public GameObject getStageCoach()
    {
        return stagecoach;
    }

    //Get a train car's position - true for its roof, false for its interior
    public GameObject getStagecoachPosition(bool isRoof)
    {
        if (isRoof)
        {
            return stagecoach.transform.GetChild(2).gameObject;
        }
        else
        {
            return stagecoach.transform.GetChild(0).gameObject;
        }

    }

    //Create a new in-game Action card object - true for in the deck, or false for the discard pile
    public GameObject createCardObject(Character c, ActionKind k, bool inDeck)
    {
        GameObject newCard = null;
        Sprite newCardSprite = null;
        newCard = Instantiate(actionCardPrefab);

        switch (k)
        {
            case ActionKind.Move:
                loadedSprites.TryGetValue(c.ToString().ToLower() + "_cards_move", out newCardSprite);
                break;
            case ActionKind.Shoot:
                loadedSprites.TryGetValue(c.ToString().ToLower() + "_cards_shoot", out newCardSprite);
                break;
            case ActionKind.Rob:
                loadedSprites.TryGetValue(c.ToString().ToLower() + "_cards_rob", out newCardSprite);
                break;
            case ActionKind.Marshal:
                loadedSprites.TryGetValue(c.ToString().ToLower() + "_cards_marshal", out newCardSprite);
                break;
            case ActionKind.Punch:
                loadedSprites.TryGetValue(c.ToString().ToLower() + "_cards_punch", out newCardSprite);
                break;
            case ActionKind.ChangeFloor:
                loadedSprites.TryGetValue(c.ToString().ToLower() + "_cards_floor", out newCardSprite);
                break;
            default:
                break;
        }

        newCard.GetComponent<Image>().sprite = newCardSprite;
        newCard.GetComponent<CardID>().kind = k;
        newCard.GetComponent<CardID>().c = c;

        //Making sure to parent the card under the hand/deck or playedCards and fixing the scale
        if (inDeck)
        {
            newCard.transform.SetParent(deck.transform);
        } else
        {
            newCard.transform.SetParent(playedCards.transform);
        }
        newCard.transform.localScale = scale;

        newCard.SetActive(false);

        return newCard;
    }

    public void flipCardObject(Character c, ActionKind k, GameObject card)
    {
        Sprite cardSprite = null;

        switch (k)
        {
            case ActionKind.Move:
                loadedSprites.TryGetValue(c.ToString().ToLower() + "_cards_move", out cardSprite);
                break;
            case ActionKind.Shoot:
                loadedSprites.TryGetValue(c.ToString().ToLower() + "_cards_shoot", out cardSprite);
                break;
            case ActionKind.Rob:
                loadedSprites.TryGetValue(c.ToString().ToLower() + "_cards_rob", out cardSprite);
                break;
            case ActionKind.Marshal:
                loadedSprites.TryGetValue(c.ToString().ToLower() + "_cards_marshal", out cardSprite);
                break;
            case ActionKind.Punch:
                loadedSprites.TryGetValue(c.ToString().ToLower() + "_cards_punch", out cardSprite);
                break;
            case ActionKind.ChangeFloor:
                loadedSprites.TryGetValue(c.ToString().ToLower() + "_cards_floor", out cardSprite);
                break;
            default:
                break;
        }

        card.GetComponent<Image>().sprite = cardSprite;
    }

    //Create a new in-game bullet card object - true for in the deck, or false for the discard pile
    public GameObject createCardObject(Character c, int num, bool inDeck)
    {
        GameObject newCard = null;
        Sprite newCardSprite = null;
        newCard = Instantiate(bulletCardPrefab);

        //Grab the corresponding bullet card sprite
        loadedSprites.TryGetValue(c.ToString().ToLower() + "_cards_" + num, out newCardSprite);

        newCard.GetComponent<Image>().sprite = newCardSprite;
        newCard.GetComponent<CardID>().isBulletCard = true;

        //Making sure to parent the card under the hand/deck or discard pile, updating the client collection of cards in the hand/deck or discard pile, and fixing the scale
        if (inDeck)
        {
            newCard.transform.SetParent(deck.transform);
        }
        else
        {
            newCard.transform.SetParent(discardPile.transform);
        }
        newCard.transform.localScale = scale;

        newCard.SetActive(false);

        return newCard;
    }

    //Enable the turn menu (true for visible, false for invisible)
    public void toggleTurnMenu(bool isVisible)
    {
        if (turnMenu != null)
        {
            turnMenu.SetActive(isVisible);

            //Ensure all buttons are available (whiskey if it is a normal turn)
            turnMenu.transform.GetChild(1).gameObject.SetActive(true);
            turnMenu.transform.GetChild(2).gameObject.SetActive(true);
            turnMenu.transform.GetChild(3).gameObject.SetActive(isNormalTurn && playerHasWhiskey());
            
        }
    }

    public bool playerHasWhiskey()
    {
        //3 - Usables, all children disabled?
        return (getPlayerProfileObject(NamedClient.c).transform.GetChild(3).GetChild(0).gameObject.activeSelf &&
                getPlayerProfileObject(NamedClient.c).transform.GetChild(3).GetChild(1).gameObject.activeSelf &&
                getPlayerProfileObject(NamedClient.c).transform.GetChild(3).GetChild(2).gameObject.activeSelf
                );
    }


    //Enable the Play or Draw buttons or both on the turn menu - if this is called, it is because of hasAnotherAction
    //hasAnotherAction is triggered by SpeedingUp turns or whiskey usage
    public void toggleTurnMenuButtons(string buttons)
    {
        if (turnMenu != null)
        {
            //Disable whiskey button
            turnMenu.transform.GetChild(3).gameObject.SetActive(false);

            switch (buttons)
            {
                case "draw":
                    turnMenu.transform.GetChild(1).gameObject.SetActive(true);
                    turnMenu.transform.GetChild(2).gameObject.SetActive(false);
                    break;
                case "play":
                    turnMenu.transform.GetChild(1).gameObject.SetActive(false);
                    turnMenu.transform.GetChild(2).gameObject.SetActive(true);
                    break;
                case "both":
                    turnMenu.transform.GetChild(1).gameObject.SetActive(true);
                    turnMenu.transform.GetChild(2).gameObject.SetActive(true);
                    break;
            }
                
        }
    }

    //Unlock the board (user can interact with trains, horses, etc.)
    public void unlockBoard()
    {
        if (boardBlocker != null)
        {
            boardBlocker.SetActive(false);
        }
    }

    //Lock the board (user cannot interact with train, horses, etc.)
    public void lockBoard()
    {
        if (boardBlocker != null)
        {
            boardBlocker.SetActive(true);
        }
    }

    //Unlock the hand (played card zone, deck - card iterator is always allowed)
    public void unlockHand()
    {
        if (handBlocker != null)
        {
            handBlocker.SetActive(false);
        }
    }

    //Lock the hand (played card zone, deck - card iterator is always allowed)
    public void lockHand()
    {
        if (handBlocker != null)
        {
            handBlocker.SetActive(true);
        }
    }

    //Unlock the sidebar (use whiskey buttons)
    public void unlockSidebar()
    {
        if (sidebarBlocker != null)
        {
            sidebarBlocker.SetActive(false);
        }
    }

    //Lock the sidebar (use whiskey buttons)
    public void lockSidebar()
    {
        if (sidebarBlocker != null)
        {
            sidebarBlocker.SetActive(true);
        }
    }

    //Enable the hostage menu (true for visible, false for invisible)
    public void toggleHostageMenu(bool isVisible)
    {
        if (hostageMenu != null)
        {
            hostageMenu.SetActive(isVisible);
        }
    }

    void Start()
    {
        int i = 0;
        //Add train cars
        foreach (Transform t in trainLocation.transform)
        {
            //Debug.Log("Index added " + i);
            trainCars.Add(i, t.gameObject);
            i++;
        }

        //Load all sprites in Resources/Sprites
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites");

        foreach (Sprite s in sprites) {
            loadedSprites.Add(s.name, s);
        }

        HostageChar hostage;
        foreach (Transform h in hostagesList.transform)
        {
            Enum.TryParse(h.gameObject.name, out hostage);
            hostageMap.Add(hostage, h.gameObject);
            h.gameObject.SetActive(false);
        }

        characters.Add(Character.Shotgun, shotgun);
    }

    void Awake()
    {
        if (!gameUIManager)
        {
            //Obtain the EventManager instance
            gameUIManager = FindObjectOfType(typeof(GameUIManager)) as GameUIManager;

            //Initialize the EventManager
            if (gameUIManager == null)
            {
                Debug.LogError("GameController failed to initialize.");
            }
          
        }

        DontDestroyOnLoad(gameUIManager);
    }

}
