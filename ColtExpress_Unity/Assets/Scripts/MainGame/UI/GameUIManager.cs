using CardSpace;
using Coffee.UIEffects;
using GameUnitSpace;
using HostageSpace;
using PositionSpace;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    //Grayscale shader material
    public Material grayscaleShaderMaterial;

    //Game object locations for major scripts
    public GameObject gameControllerLocation;
    public GameObject playerProfileLocation;
    public GameObject trainLocation;
    public GameObject deck;
    public GameObject playedCards;
    public GameObject discardPile;
    public GameObject hostagesList;

    public GameObject horseSetCaboose;

    public GameObject shotgun;
    public GameObject stagecoach;

    public GameObject horseTrack;

    public GameObject lootOverlayList;
    public GameObject stagecoachLootRoof;
    public GameObject stagecoachLootInterior;

    //Menus
    public GameObject turnMenu;
    public GameObject ghostMenu;
    public GameObject hostageMenu;
    public GameObject horseAttackMenu;
    public GameObject keepMenu;
    public GameObject punchShotgunButton;

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
    private Dictionary<Character, GameObject> charactersOnHorses = new Dictionary<Character, GameObject>();
    private Dictionary<HostageChar, GameObject> hostageMap = new Dictionary<HostageChar, GameObject>();
    private Dictionary<int, GameObject> horseSets = new Dictionary<int, GameObject>();

    //Loot strips
    private Dictionary<int, GameObject> trainCarRoofLoot = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> trainCarInteriorLoot = new Dictionary<int, GameObject>();

    //EventManager instance, game status, has another action status
    private static GameUIManager gameUIManager;
    public GameStatus gameStatus;

    //Other important information
    public bool isNormalTurn = false;
    public bool isTunnelTurn = false;
    public bool isTurmoilTurn = false;
    public bool whiskeyWasUsed = false;
    public bool abilityDisabled = false;
    public bool photographerHideDisabled = false;

    public (bool, ActionKind) actionBlocked = (false, ActionKind.Marshal);

    public int currentTurnIndex = 0;

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

    public void clearShootTargets()
    {
        List<GameObject> flattenList = playerProfiles.Values.ToList();

        foreach (GameObject t in flattenList)
        {
            t.transform.GetChild(0).gameObject.GetComponent<UIShiny>().enabled = false;
            t.transform.GetChild(0).gameObject.GetComponent<Button>().enabled = false;
            t.transform.GetChild(0).gameObject.GetComponent<Button>().onClick.RemoveListener(GameUIManager.gameUIManagerInstance.gameObject.GetComponent<StealinPhaseManager>().playerChoseTarget);
        }
    }

    
    public void clearPunchTargets()
    {
        List<GameObject> flattenList = playerProfiles.Values.ToList();

        foreach (GameObject t in flattenList)
        {
            t.transform.GetChild(0).gameObject.GetComponent<UIShiny>().enabled = false;
            t.transform.GetChild(0).gameObject.GetComponent<Button>().enabled = false;
            t.transform.GetChild(0).gameObject.GetComponent<Button>().onClick.RemoveListener(GameUIManager.gameUIManagerInstance.gameObject.GetComponent<StealinPhaseManager>().playerChoseTargetPunch);
        }

        punchShotgunButton.SetActive(false);
    }

    public void togglePunchShotgunButton(bool isVisible)
    {
        punchShotgunButton.SetActive(isVisible);
    }

    public void toggleKeepMenu(bool isVisible)
    {
        keepMenu.SetActive(isVisible);
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

        Image s_image = getStagecoachPosition(true).GetComponent<Image>();
        s_image.color = new Color(s_image.color.r, s_image.color.g, s_image.color.b, 0f);

        getStagecoachPosition(true).GetComponent<Button>().enabled = false;

        s_image = getStagecoachPosition(false).GetComponent<Image>();
        s_image.color = new Color(s_image.color.r, s_image.color.g, s_image.color.b, 0f);

        getStagecoachPosition(false).GetComponent<Button>().enabled = false;

    }

    public bool getShotgunByShotgunButton(GameObject shotgunButton)
    {
        return GameObject.ReferenceEquals(shotgunButton, punchShotgunButton);
    }

    public Character getCharacterByPlayerProfile(GameObject playerProfile)
    {
        List<Character> flattenList = playerProfiles.Keys.ToList();

        foreach (Character c in flattenList)
        {
            if (GameObject.ReferenceEquals(playerProfile, getPlayerProfileObject(c)))
            {
                return c;
            }
        }

        return Character.Marshal;
    }

    public void rideAhead()
    {
        var definition = new
        {
            eventName = "HorseAttackMessage",
            HorseAttackAction = "ride"
        };

        ClientCommunicationAPI.CommunicationAPI.sendMessageToServer(definition);
        toggleHorseAttackMenu(false);
    }

    public void enterCar()
    {
        var definition = new
        {
            eventName = "HorseAttackMessage",
            HorseAttackAction = "enter"
        };

        ClientCommunicationAPI.CommunicationAPI.sendMessageToServer(definition);
        toggleHorseAttackMenu(false);
    }

    public void toggleHorseAttackMenu(bool isVisible)
    {
        horseAttackMenu.SetActive(isVisible);
    }

    public void toggleGhostMenu(bool isVisible)
    {
        ghostMenu.SetActive(isVisible);
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

        //If stagecoach and same as roof
        if (GameObject.ReferenceEquals(trainCarPosition, getStagecoachPosition(true)))
        {
            return (false, -1);
        } else if (GameObject.ReferenceEquals(trainCarPosition, getStagecoachPosition(false)))
        {
            return (true, -1);
        }

        return (false, -1);
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

    public void blockActionCards(ActionKind kindToBlock)
    {
        foreach (Transform c in deck.transform)
        {
            if (c.gameObject.GetComponent<CardID>().kind == kindToBlock)
            {
                Destroy(c.gameObject.GetComponent<Draggable>());
                c.gameObject.GetComponent<Image>().material = grayscaleShaderMaterial;
            }
        }
    }

    //Grab all horse objects
    public void initializeHorses(List<Character> players)
    {
        //Add number of horse sets from locomotive
        for (int i = 0; i < players.Count; i++)
        {
            horseSets.Add(i, horseTrack.transform.GetChild(i).gameObject);
        }
        //Destroy the rest - remember button
        for (int i = players.Count; i < horseSetCaboose.transform.childCount - 1; i++)
        {
            Destroy(horseSetCaboose.transform.GetChild(i).gameObject);
            horseTrack.transform.GetChild(i).gameObject.SetActive(false);
        }

        //Add caboose at end
        horseSets.Add(players.Count, horseSetCaboose);

        //Put all the bandits on the free horses
        for (int i = 0; i < players.Count; i++)
        {
            //Parent bandit on free horse
            getCharacterObject(players[i]).transform.SetParent(horseSetCaboose.transform.GetChild(i).transform.GetChild(0).transform);
            //Debug.LogError("Parented " + players[i] + " on " + horseSetCaboose.transform.GetChild(i).gameObject.name);
            getCharacterObject(players[i]).transform.localScale = scale;

            //Add mapping
            charactersOnHorses.Add(players[i], horseSetCaboose.transform.GetChild(i).transform.GetChild(0).gameObject);
        }

        switch (players.Count)
        {
            case 6:
                horseSetCaboose.GetComponent<VerticalLayoutGroup>().spacing = -129;
                break;
            case 5:
                horseSetCaboose.GetComponent<VerticalLayoutGroup>().spacing = -127;
                break;
            case 4:
                horseSetCaboose.GetComponent<VerticalLayoutGroup>().spacing = -124;
                break;
            case 3:
                horseSetCaboose.GetComponent<VerticalLayoutGroup>().spacing = -119;
                break;
            case 2:
                horseSetCaboose.GetComponent<VerticalLayoutGroup>().spacing = -102;
                break;
            case 1:
                horseSetCaboose.GetComponent<VerticalLayoutGroup>().spacing = -102;
                break;
        }

        //Finally, update the horses to the caboose end
        trainCars.TryGetValue(players.Count, out GameObject caboose);

        //Change the caboose/caboose loot coordinates to be the new end of the train
        Vector3 lastTrainCarCoordinates = new Vector3(caboose.transform.position.x, horseSetCaboose.transform.position.y, horseSetCaboose.transform.position.z);

        horseSetCaboose.transform.position = lastTrainCarCoordinates;
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
        GameObject trainCarRLoot = null;
        GameObject trainCarILoot = null;
        trainCars.TryGetValue(index, out trainCar);
        trainCarRoofLoot.TryGetValue(index, out trainCarRLoot);
        trainCarInteriorLoot.TryGetValue(index, out trainCarILoot);

        if (index == numPlayers)
        {
            //Retrieve the caboose
            GameObject caboose = null;
            trainCars.TryGetValue(6, out caboose);

            //Retrieve the caboose loot strips
            GameObject cabooseRoofLoot = null;
            GameObject cabooseInteriorLoot = null;
            trainCarRoofLoot.TryGetValue(6, out cabooseRoofLoot);
            trainCarInteriorLoot.TryGetValue(6, out cabooseInteriorLoot);

            //Change the caboose/caboose loot coordinates to be the new end of the train
            Vector3 lastTrainCarCoordinates = trainCar.transform.position;

            caboose.transform.position = lastTrainCarCoordinates;
            cabooseRoofLoot.transform.parent.position = lastTrainCarCoordinates;
            cabooseInteriorLoot.transform.parent.position = lastTrainCarCoordinates;

            // Debug.Log(caboose.name + " now at " + lastTrainCarCoordinates);

            //Disable the rest of the cars and remove them from the map
            for (int i = index; i <= 5; i++)
            {
                trainCars.TryGetValue(i, out trainCar);
                trainCar.SetActive(false);
                trainCars.Remove(i);


                trainCarRoofLoot.TryGetValue(i, out trainCarRLoot);
                trainCarRLoot.transform.parent.gameObject.SetActive(false);
                trainCarRoofLoot.Remove(i);

                trainCarInteriorLoot.TryGetValue(i, out trainCarILoot);
                trainCarILoot.transform.parent.gameObject.SetActive(false);
                trainCarInteriorLoot.Remove(i);

                //Debug.Log("Removed " + trainCar.name + " at " + i);
            }

            //Remove the caboose mapping without disabling it
            trainCars.Remove(6);
            trainCarRoofLoot.Remove(6);
            trainCarInteriorLoot.Remove(6);
            //Debug.Log("Removed " + caboose.name + " at " + 6);

            //Replace the last train car's index with the caboose in the map
            trainCars.Add(index, caboose);
            trainCarRoofLoot.Add(index, cabooseRoofLoot);
            trainCarInteriorLoot.Add(index, cabooseInteriorLoot);
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

    public GameObject remapCharacterAndHorse(Character c, GameObject horsePosition)
    {
        charactersOnHorses.Remove(c);
        charactersOnHorses.Add(c, horsePosition);
        return horsePosition;
    }
    public GameObject getHorseSet(int index)
    {
        horseSets.TryGetValue(index, out GameObject horseSet);
        return horseSet;
    }

    public GameObject getHorsePositionByCharacter(Character c)
    {
        charactersOnHorses.TryGetValue(c, out GameObject horsePosition);
        return horsePosition;
    }

    public GameObject getHorseByCharacter(Character c)
    {
        charactersOnHorses.TryGetValue(c, out GameObject horsePosition);
        horsePosition = horsePosition.transform.parent.gameObject;
        return horsePosition;
    }

    public void adjustHorseSpacing(GameObject horseSet)
    {
        //Remember to ignore button
        switch (horseSet.transform.childCount - 1)
        {
            case 6:
                horseSetCaboose.GetComponent<VerticalLayoutGroup>().spacing = -129;
                break;
            case 5:
                horseSetCaboose.GetComponent<VerticalLayoutGroup>().spacing = -127;
                break;
            case 4:
                horseSetCaboose.GetComponent<VerticalLayoutGroup>().spacing = -124;
                break;
            case 3:
                horseSetCaboose.GetComponent<VerticalLayoutGroup>().spacing = -119;
                break;
            case 2:
                horseSetCaboose.GetComponent<VerticalLayoutGroup>().spacing = -102;
                break;
            case 1:
                horseSetCaboose.GetComponent<VerticalLayoutGroup>().spacing = -102;
                break;
        }
    }

    //Get a train car's loot strip - true for its roof, false for its interior
    public GameObject getTrainCarLoot(int index, bool isRoof)
    {
        GameObject trainCarStrip = null;
        if (isRoof)
        {
            trainCarRoofLoot.TryGetValue(index, out trainCarStrip);
            return trainCarStrip;
        }
        else
        {
            trainCarInteriorLoot.TryGetValue(index, out trainCarStrip);
            return trainCarStrip;
        }
    }

    public GameObject getStageCoachLoot(bool isRoof)
    {
        if (isRoof)
        {
            return stagecoachLootRoof;
        }
        else
        {
            return stagecoachLootInterior;
        }
    }

    public int getNumTrainCars()
    {
        return trainCars.Values.Count;
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
            return stagecoach.transform.GetChild(3).gameObject;
        }
        else
        {
            return stagecoach.transform.GetChild(1).gameObject;
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
            case ActionKind.Ride:
                loadedSprites.TryGetValue(c.ToString().ToLower() + "_cards_ride", out newCardSprite);
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
            case ActionKind.Ride:
                loadedSprites.TryGetValue(c.ToString().ToLower() + "_cards_ride", out cardSprite);
                break;
            default:
                break;
        }

        card.GetComponent<Image>().sprite = cardSprite;
        card.GetComponent<CardID>().isHidden = false;
    }

    //Create a new in-game bullet card object - true for in the deck, or false for the discard pile
    public GameObject createCardObject(Character? c, int num, bool inDeck)
    {
        GameObject newCard = null;
        Sprite newCardSprite = null;
        newCard = Instantiate(bulletCardPrefab);

        if (c == null)
        {

            loadedSprites.TryGetValue("neutral_bullet", out newCardSprite);
        } else
        {
            //Grab the corresponding bullet card sprite
            loadedSprites.TryGetValue(c.ToString().ToLower() + "_bullet_cards_" + num, out newCardSprite);


        }

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
        bool usable = false;
        if (getPlayerProfileObject(NamedClient.c).transform.GetChild(3).GetChild(0).gameObject.activeSelf)
        {
            usable = true;
        }

        if (getPlayerProfileObject(NamedClient.c).transform.GetChild(3).GetChild(1).gameObject.activeSelf)
        {
            usable = true;
        }

        if (getPlayerProfileObject(NamedClient.c).transform.GetChild(3).GetChild(2).gameObject.activeSelf)
        {
            usable = true;
        }
    
        return usable;

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
            trainCarRoofLoot.Add(i, lootOverlayList.transform.GetChild(i).GetChild(0).gameObject);
            trainCarInteriorLoot.Add(i, lootOverlayList.transform.GetChild(i).GetChild(1).gameObject);
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

    public bool isTurnMenuEnabled()
    {
        return turnMenu.activeSelf;
    }

    public void reset()
    {

        numPlayers = 0;
        characters.Clear();
        playerProfiles.Clear();
        trainCars.Clear();
        charactersOnHorses.Clear();
        hostageMap.Clear();
        horseSets.Clear();
        trainCarRoofLoot.Clear();
   
        //Other important information
        isNormalTurn = false;
        isTunnelTurn = false;
        isTurmoilTurn = false;
        whiskeyWasUsed = false;
        abilityDisabled = false;
        photographerHideDisabled = false;

        actionBlocked = (false, ActionKind.Marshal);

        currentTurnIndex = 0;
        loadedSprites.Clear();
    
    }

    public void deserializeGUIM(List<Serialized_Player_Profile_Object> pp, Serialized_GameUIManager guim, List<Serialized_Card_Object> h, List<Serialized_Card_Object> pc)
    {
        foreach (Serialized_Card_Object card in h)
        {

            GameObject c = createCardObject(card.s_CardID_c, card.s_CardID_kind, true);

            c.GetComponent<CardID>().isBulletCard = card.s_CardID_isBulletCard;
            c.GetComponent<CardID>().isHidden = card.s_CardID_isHidden;
            c.GetComponent<CardID>().playedByGhost = card.s_CardID_playedByGhost;

            if (card.hasDraggable)
            {
                c.GetComponent<Draggable>().originalIndex = card.s_originalIndex;
                c.GetComponent<Draggable>().fromDeck = card.s_fromDeck;
            }
            else
            {
                Destroy(c.GetComponent<Draggable>());
            }

            loadedSprites.TryGetValue(card.g_Sprite, out Sprite newCardSprite);
            c.GetComponent<Image>().sprite = newCardSprite;
        }

        foreach (Serialized_Card_Object card in pc)
        {

            GameObject c = createCardObject(card.s_CardID_c, card.s_CardID_kind, false);

            c.GetComponent<CardID>().isBulletCard = card.s_CardID_isBulletCard;
            c.GetComponent<CardID>().isHidden = card.s_CardID_isHidden;
            c.GetComponent<CardID>().playedByGhost = card.s_CardID_playedByGhost;

            if (card.hasDraggable)
            {
                c.GetComponent<Draggable>().originalIndex = card.s_originalIndex;
                c.GetComponent<Draggable>().fromDeck = card.s_fromDeck;
            }
            else
            {
                Destroy(c.GetComponent<Draggable>());
            }

            loadedSprites.TryGetValue(card.g_Sprite, out Sprite newCardSprite);
            c.GetComponent<Image>().sprite = newCardSprite;
        }

        GameUIManager.gameUIManagerInstance.gameStatus = guim.gameStatus;
        GameUIManager.gameUIManagerInstance.isNormalTurn = guim.isNormalTurn;

        GameUIManager.gameUIManagerInstance.isTunnelTurn = guim.isTunnelTurn;

        GameUIManager.gameUIManagerInstance.isTurmoilTurn = guim.isTurmoilTurn;
        GameUIManager.gameUIManagerInstance.whiskeyWasUsed = guim.whiskeyWasUsed;
        GameUIManager.gameUIManagerInstance.abilityDisabled = guim.abilityDisabled;

        GameUIManager.gameUIManagerInstance.photographerHideDisabled = guim.photographerHideDisabled;

        GameUIManager.gameUIManagerInstance.actionBlocked = guim.actionBlocked;
        GameUIManager.gameUIManagerInstance.currentTurnIndex = guim.currentTurnIndex;

        foreach(Character c in guim.characters)
        {
            createCharacterObject(c);
            createPlayerProfileObject(c);
        }

        foreach(Serialized_Player_Profile_Object ppo in pp)
        {
            GameObject profile = getPlayerProfileObject(ppo.p_Bandit);

            profile.transform.GetChild(4).gameObject.GetComponent<TextMeshProUGUI>().text = ppo.p_Hostage;

            string value = "x" + ppo.p_Unknown_Whiskey.ToString();
            profile.transform.GetChild(3).GetChild(0).GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = value;

            value = "x" + ppo.p_Normal_Whiskey.ToString();
            profile.transform.GetChild(3).GetChild(1).GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = value;

            value = "x" + ppo.p_Old_Whiskey.ToString();
            profile.transform.GetChild(3).GetChild(2).GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = value;

            value = "x" + ppo.p_Bullets.ToString();
            profile.transform.GetChild(2).GetChild(0).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = value;

            value = "x" + ppo.p_Strongboxes.ToString();
            profile.transform.GetChild(2).GetChild(1).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = value;
            

            value = "x" + ppo.p_Rubies.ToString();
            profile.transform.GetChild(2).GetChild(2).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = value;


            value = "x" + ppo.p_Purses.ToString();
            profile.transform.GetChild(2).GetChild(3).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = value;

            profile.transform.GetChild(5).gameObject.SetActive(ppo.p_FirstPlayer;
            profile.transform.GetChild(6).gameObject.SetActive(ppo.p_AbilityDisabled);
            profile.transform.GetChild(7).gameObject.SetActive(ppo.p_HideDisabled);

        }

        GameUIManager.gameUIManagerInstance.numPlayers = guim.numPlayers;

        int maxIndex = guim.indices.Count();
        int currentIndex = 0;

        while (currentIndex < maxIndex)
        {
            initializeTrainCar(currentIndex);
            currentIndex++;
        }

        trainCars.TryGetValue(maxIndex - 1, out GameObject caboose);

        Vector3 cabooseVector = new Vector3(guim.cabooseTransform.x, guim.cabooseTransform.y, guim.cabooseTransform.z);
        caboose.transform.position = cabooseVector;

        cabooseVector = new Vector3(guim.cabooseRoofLootTransform.x, guim.cabooseRoofLootTransform.y, guim.cabooseRoofLootTransform.z);
        getTrainCarLoot(maxIndex - 1, true).transform.position = cabooseVector;

        cabooseVector = new Vector3(guim.cabooseInteriorLootTransform.x, guim.cabooseInteriorLootTransform.y, guim.cabooseInteriorLootTransform.z);
        getTrainCarLoot(maxIndex - 1, false).transform.position = cabooseVector;

        Vector3 stageCoachVector = new Vector3(guim.stageCoachTransform.x, guim.stageCoachTransform.y, guim.stageCoachTransform.z);
        stagecoach.transform.position = stageCoachVector;

        stageCoachVector = new Vector3(guim.stageCoachRoofLootTransform.x, guim.stageCoachRoofLootTransform.y, guim.stageCoachRoofLootTransform.z);
        getStageCoachLoot(true).transform.position = stageCoachVector;

        stageCoachVector = new Vector3(guim.stageCoachInteriorLootTransform.x, guim.stageCoachInteriorLootTransform.y, guim.stageCoachInteriorLootTransform.z);
        getStageCoachLoot(false).transform.position = stageCoachVector;

        var flattenList = guim.horsesAtIndices.Values.ToList();

        //Add number of horse sets from locomotive
        for (int i = 0; i < guim.numPlayers; i++)
        {
            horseSets.Add(i, horseTrack.transform.GetChild(i).gameObject);
        }
        //Destroy the other horse sets and the extra horses
        for (int i = guim.numPlayers; i < horseSetCaboose.transform.childCount - 1; i++)
        {
            Destroy(horseSetCaboose.transform.GetChild(i).gameObject);
            horseTrack.transform.GetChild(i).gameObject.SetActive(false);
        }
        //Add available horses
        List<GameObject> availableHorses = new List<GameObject>();
        for (int i = 0; i < horseSetCaboose.transform.childCount - 1; i++)
        {
            availableHorses.Add(horseSetCaboose.transform.GetChild(i).gameObject);
        }

        foreach(int index in guim.horsesAtIndices.Keys)
        {
            guim.horsesAtIndices.TryGetValue(index, out int numHorsesAtLocation);
            
            while (numHorsesAtLocation > 0)
            {
                GameObject horse = availableHorses[0];
                horse.transform.parent = getHorseSet(index).transform;
                availableHorses.Remove(horse);
                numHorsesAtLocation--;
            }
        }
    }

    public void serializeGUIM(List<Serialized_Player_Profile_Object> pp, Serialized_GameUIManager guim)
    {
        foreach (Character c in playerProfiles.Keys)
        {
            GameObject profile = getPlayerProfileObject(c);
            Serialized_Player_Profile_Object ppo = new Serialized_Player_Profile_Object();

            ppo.p_Bandit = c;
            ppo.p_Hostage = profile.transform.GetChild(4).gameObject.GetComponent<TextMeshProUGUI>().text;

            string value = profile.transform.GetChild(3).GetChild(0).GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text;
            value = value.Substring(1);
            ppo.p_Unknown_Whiskey = Int32.Parse(value);

            value = profile.transform.GetChild(3).GetChild(1).GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text;
            value = value.Substring(1);
            ppo.p_Normal_Whiskey = Int32.Parse(value);

            value = profile.transform.GetChild(3).GetChild(2).GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text;
            value = value.Substring(1);
            ppo.p_Old_Whiskey = Int32.Parse(value);

            value = profile.transform.GetChild(2).GetChild(0).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text;
            value = value.Substring(1);
            ppo.p_Bullets = Int32.Parse(value);

            value = profile.transform.GetChild(2).GetChild(1).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text;
            value = value.Substring(1);
            ppo.p_Strongboxes = Int32.Parse(value);

            value = profile.transform.GetChild(2).GetChild(2).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text;
            value = value.Substring(1);
            ppo.p_Rubies = Int32.Parse(value);

            value = profile.transform.GetChild(2).GetChild(3).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text;
            value = value.Substring(1);
            ppo.p_Purses = Int32.Parse(value);

            ppo.p_FirstPlayer = profile.transform.GetChild(5).gameObject.activeSelf;
            ppo.p_AbilityDisabled = profile.transform.GetChild(6).gameObject.activeSelf;
            ppo.p_HideDisabled = profile.transform.GetChild(7).gameObject.activeSelf;

            pp.Add(ppo);
        }

        guim.numPlayers = numPlayers;

        guim.characters = characters.Keys.ToList();

        guim.indices = trainCars.Keys.ToList();

        guim.cabooseTransform.x = getTrainCar(getNumTrainCars() - 1).transform.position.x;
        guim.cabooseTransform.y = getTrainCar(getNumTrainCars() - 1).transform.position.y;
        guim.cabooseTransform.z = getTrainCar(getNumTrainCars() - 1).transform.position.z;

        guim.cabooseRoofLootTransform.x = getTrainCarLoot(getNumTrainCars() - 1, true).transform.position.x;
        guim.cabooseRoofLootTransform.y = getTrainCarLoot(getNumTrainCars() - 1, true).transform.position.y;
        guim.cabooseRoofLootTransform.z = getTrainCarLoot(getNumTrainCars() - 1, true).transform.position.z;

        guim.cabooseInteriorLootTransform.x = getTrainCarLoot(getNumTrainCars() - 1, false).transform.position.x;
        guim.cabooseInteriorLootTransform.y = getTrainCarLoot(getNumTrainCars() - 1, false).transform.position.y;
        guim.cabooseInteriorLootTransform.z = getTrainCarLoot(getNumTrainCars() - 1, false).transform.position.z;

        guim.stageCoachTransform.x = getStageCoach().transform.position.x;
        guim.stageCoachTransform.y = getStageCoach().transform.position.y;
        guim.stageCoachTransform.z = getStageCoach().transform.position.z;

        guim.stageCoachRoofLootTransform.x = getStageCoachLoot(true).transform.position.x;
        guim.stageCoachRoofLootTransform.y = getStageCoachLoot(true).transform.position.y;
        guim.stageCoachRoofLootTransform.z = getStageCoachLoot(true).transform.position.z;

        guim.stageCoachInteriorLootTransform.x = getStageCoachLoot(false).transform.position.x;
        guim.stageCoachInteriorLootTransform.y = getStageCoachLoot(false).transform.position.y;
        guim.stageCoachInteriorLootTransform.z = getStageCoachLoot(false).transform.position.z;
        foreach (int i in horseSets.Keys)
        {
            horseSets.TryGetValue(i, out GameObject t);
            int numHorses = t.transform.childCount - 1;
            guim.horsesAtIndices.Add(i, numHorses);
        }

        guim.gameStatus = gameStatus;
   
        guim.isNormalTurn = isNormalTurn;
        guim.isTunnelTurn = isTunnelTurn;
        guim.isTurmoilTurn = isTurmoilTurn;
        guim.whiskeyWasUsed = whiskeyWasUsed;
        guim.abilityDisabled = abilityDisabled;
        guim.photographerHideDisabled = photographerHideDisabled;
        guim.actionBlocked = actionBlocked;
        guim.currentTurnIndex = currentTurnIndex;
}
}
