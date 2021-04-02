using CardSpace;
using GameUnitSpace;
using PositionSpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    //Game object locations for major scripts
    public GameObject gameControllerLocation;
    public GameObject playerProfileLocation;
    public GameObject trainLocation;
    public GameObject deck;
    public GameObject discardPile;

    //Menus
    public GameObject turnMenu;
    public GameObject boardBlocker;
    public GameObject handBlocker;

    //Prefabs
    public GameObject characterPrefab;
    public GameObject playerProfilePrefab;
    public GameObject actionCardPrefab;
    public GameObject bulletCardPrefab;

    //Character sprites
    public Sprite Tuco;
    public Sprite Django;
    public Sprite Ghost;
    public Sprite Doc;
    public Sprite Che;
    public Sprite Belle;

    //We treat a player's bandit (which is unique) as the corresponding player
    //Player : in-game character object map
    //Player profile : in-game character object map
    //Train car index : train car map
    int numPlayers = 0;
    private Dictionary<Character, GameObject> characters = new Dictionary<Character, GameObject>();
    private Dictionary<Character, GameObject> playerProfiles = new Dictionary<Character, GameObject>();
    private Dictionary<int, GameObject> trainCars = new Dictionary<int, GameObject>();

    //EventManager instance, game status
    private static GameUIManager gameUIManager;
    public bool gameStatus;

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

    //Get corresponding in-game character object to a player
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

        switch (c)
        {
            case Character.Tuco:
                newPlayer.GetComponent<Image>().sprite = Tuco;
                break;
            case Character.Django:
                newPlayer.GetComponent<Image>().sprite = Django;
                break;
            case Character.Ghost:
                newPlayer.GetComponent<Image>().sprite = Ghost;
                break;
            case Character.Doc:
                newPlayer.GetComponent<Image>().sprite = Doc;
                break;
            case Character.Cheyenne:
                newPlayer.GetComponent<Image>().sprite = Che;
                break;
            case Character.Belle:
                newPlayer.GetComponent<Image>().sprite = Belle;
                break;
            default:
                newPlayer.GetComponent<Image>().sprite = Belle;
                break;

        }

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

        switch (c)
        {
            case Character.Tuco:
                newPlayerProfile.transform.GetChild(0).GetComponent<Image>().sprite = Tuco;
                break;
            case Character.Django:
                newPlayerProfile.transform.GetChild(0).GetComponent<Image>().sprite = Django;
                break;
            case Character.Ghost:
                newPlayerProfile.transform.GetChild(0).GetComponent<Image>().sprite = Ghost;
                break;
            case Character.Doc:
                newPlayerProfile.transform.GetChild(0).GetComponent<Image>().sprite = Doc;
                break;
            case Character.Cheyenne:
                newPlayerProfile.transform.GetChild(0).GetComponent<Image>().sprite = Che;
                break;
            case Character.Belle:
                newPlayerProfile.transform.GetChild(0).GetComponent<Image>().sprite = Belle;
                break;
            default:
                newPlayerProfile.transform.GetChild(0).GetComponent<Image>().sprite = Belle;
                break;

        }

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

            Debug.Log(caboose.name);

            //Change the caboose coordinates to be the new end of the train
            Vector3 lastTrainCarCoordinates = trainCar.transform.position;

            Debug.Log(caboose.name + " previously at " + caboose.transform.position);

            caboose.transform.position = lastTrainCarCoordinates;

            Debug.Log(caboose.name + " now at " + lastTrainCarCoordinates);

            //Disable the rest of the cars and remove them from the map
            for (int i = index; i <= 5; i++)
            {
                trainCars.TryGetValue(i, out trainCar);
                trainCar.SetActive(false);
                trainCars.Remove(i);

                Debug.Log("Removed " + trainCar.name + " at " + i);
            }

            //Remove the caboose mapping without disabling it
            trainCars.Remove(6);

            Debug.Log("Removed " + caboose.name + " at " + 6);

            //Replace the last train car's index with the caboose in the map
            trainCars.Add(index, caboose);
            Debug.Log("Added " + caboose.name + " at " + index);

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

        //Making sure to parent the card under the hand/deck or discard pile, updating the client collection of cards in the hand/deck or discard pile, and fixing the scale
        if (inDeck)
        {
            newCard.transform.SetParent(deck.transform);
        } else
        {
            newCard.transform.SetParent(discardPile.transform);
        }
        newCard.transform.localScale = scale;

        return newCard;
    }

    //Create a new in-game Bullet card object 
    /*
    public GameObject createCardObject(Character c, int k)
    {
        GameObject newCard = Instantiate(bulletCardPrefab);


        //Fixing the scale
        newPlayer.transform.localScale = scale;

        characters.Add(c, newPlayer);
        return newPlayer;
    }
    */

    //Enable the turn menu (true for visible, false for invisible)
    public void toggleTurnMenu(bool isVisible)
    {
        if (turnMenu != null)
        {
            turnMenu.SetActive(isVisible);
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

    //Lock the hand (played card zone, deck - card iterator is always allowed)
    public void unlockHand()
    {
        if (handBlocker != null)
        {
            handBlocker.SetActive(false);
        }
    }

    //Unlock the hand (played card zone, deck - card iterator is always allowed)
    public void lockHand()
    {
        if (handBlocker != null)
        {
            handBlocker.SetActive(true);
        }
    }

    void Start()
    {
        int i = 0;
        foreach (Transform t in trainLocation.transform)
        {
            //Debug.Log("Index added " + i);
            trainCars.Add(i, t.gameObject);
            i++;
        }

        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites");

        foreach (Sprite s in sprites) {
            loadedSprites.Add(s.name, s);
        }
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
