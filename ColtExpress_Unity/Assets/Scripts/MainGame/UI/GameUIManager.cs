using GameUnitSpace;
using PositionSpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public GameObject gameControllerLocation;
    public GameObject trainLocation;
    public GameObject playerProfileLocation;
    
    public GameObject characterPrefab;
    public GameObject trainCarRoofPrefab;
    public GameObject trainCarPrefab;
    public GameObject trainCarLocomotivePrefab;
    public GameObject playerProfilePrefab;

    public Sprite Tuco;
    public Sprite Django;
    public Sprite Ghost;
    public Sprite Doc;
    public Sprite Che;
    public Sprite Belle;

    int numPlayers = 0;
    private Dictionary<Character, GameObject> characters = new Dictionary<Character, GameObject>();
    private Dictionary<Character, GameObject> playerProfiles = new Dictionary<Character, GameObject>();;
    private Dictionary<int, GameObject> trainCarPositions = new Dictionary<int, GameObject>();

    //EventManager instance
    private static GameUIManager gameController;

    public static GameUIManager gameControllerInstance
    {
        get
        {
            return gameController;
        }
    }

    public GameObject getCharacterObject(Character c)
    {
        GameObject requestedPlayer = null;
        characters.TryGetValue(c, out requestedPlayer);
        return requestedPlayer;
    }

    public GameObject createCharacterObject(Character c)
    {
        GameObject newPlayer = Instantiate(characterPrefab);
        newPlayer.GetComponent<getMainCharacter>().setSprite(c); 
        characters.Add(c, newPlayer);
        numPlayers++;
        return newPlayer;
    }
    public GameObject getPlayerProfileObject(Character c)
    {
        GameObject requestedPlayer = null;
        playerProfiles.TryGetValue(c, out requestedPlayer);
        return requestedPlayer;
    }

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

        newPlayerProfile.transform.SetParent(playerProfileLocation.transform);

        playerProfiles.Add(c, newPlayerProfile);
        numPlayers++;
        return newPlayerProfile;
    }


    public GameObject getTrainCarPosition(int index)
    {
        GameObject trainCarPosition = null;
        trainCarPositions.TryGetValue(index, out trainCarPosition);
        return trainCarPosition;
    }

    public GameObject createTrainCarObject(int index)
    {
        GameObject newTrainCar;
        if (index == 0)
        {
            newTrainCar = Instantiate(trainCarRoofPrefab);
        } else if (index != numPlayers)
        {
            newTrainCar = Instantiate(trainCarPrefab);
        } else
        {
            newTrainCar = Instantiate(trainCarLocomotivePrefab);
        }

        trainCarPositions.Add(index, newTrainCar);
        newTrainCar.transform.SetParent(trainLocation.transform);
        
        return newTrainCar;
    }

    void Awake()
    {
        if (!gameController)
        {
            //Obtain the EventManager instance
            gameController = FindObjectOfType(typeof(GameUIManager)) as GameUIManager;

            //Initialize the EventManager
            if (gameController == null)
            {
                Debug.LogError("GameController failed to initialize.");
            }
            else
            {
                characters = new Dictionary<Character, GameObject>();
            }
        }

        DontDestroyOnLoad(gameController);
    }

}
