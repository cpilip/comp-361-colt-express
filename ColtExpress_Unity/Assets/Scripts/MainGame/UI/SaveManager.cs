using CardSpace;
using GameUnitSpace;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RoundSpace;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SaveManager : MonoBehaviour
{
    //UpdateCurrentPlayer
    //UpdateFirstPlayer
    public GameObject l_uCP_uFP_location;

    //UpdateCurrentRound
    public GameObject l_uCR_location;

    //UpdateCurrentTurn
    public GameObject l_uCT_location;

    //Played Cards
    public GameObject PC_location;
    //Hand
    public GameObject H_location;

    //Save Button
    public GameObject saveButton;

    private void Start()
    {
        saveButton.GetComponent<Button>().interactable = false;
    }

    public void canSave()
    {
        if (GameUIManager.gameUIManagerInstance.gameStatus == GameStatus.Schemin && 
            EventManager.EventManagerInstance.gameObject.GetComponent<NamedClient>().bufferIsEmpty() && 
            GameUIManager.gameUIManagerInstance.isTurnMenuEnabled() &&
            GameUIManager.gameUIManagerInstance.getPlayerProfileObject(NamedClient.c).GetComponent<Image>().color == new Color(1.000f, 0.933f, 0.427f, 0.914f))
        {
            saveButton.GetComponent<Button>().interactable = true;
        }
    }

    public void loadSavedGame(string clientState)
    {
        //Receive string data here

        string data;

        string path = Directory.GetCurrentDirectory();
        path += "\\clientstate.txt";
        var sr = new StreamReader(path);
        string fileContents = sr.ReadToEnd();
        sr.Close();

        data = fileContents;
        JObject o = JObject.Parse(data);

        NamedClient.c = o.SelectToken("NamedClient_c").ToObject<Character>();
        List<Serialized_Card_Object> h = o.SelectToken("h").ToObject<List<Serialized_Card_Object>>();
        List<Serialized_Card_Object> pc = o.SelectToken("pc").ToObject<List<Serialized_Card_Object>>();

        Serialized_Listeners sl = o.SelectToken("sl").ToObject<Serialized_Listeners>();
        l_uCP_uFP_location.GetComponent<UpdateCurrentPlayerListener>().setPreviousPlayer(sl.uCP_previousPlayer);
        l_uCP_uFP_location.GetComponent<UpdateFirstPlayerListener>().setPreviousPlayer(sl.uFP_previousPlayer);

        l_uCR_location.GetComponent<UpdateCurrentRoundListener>().setCurrentRound(sl.uCR__currentRound);
        l_uCT_location.GetComponent<UpdateCurrentTurnListener>().setPreviousTurn(sl.uCT_previousTurn);
        UpdateTopCardListener.turmoilCardsPlayed = sl.uTC_turmoilCardsPlayed;
        HightlightTopCardListener.pileFlipped = sl.hTC_pileFlipped;
        MoveStageCoachListener.atIndex = sl.mSC_atIndex;
            


        List<Serialized_Player_Profile_Object> pp = o.SelectToken("pp").ToObject<List<Serialized_Player_Profile_Object>>();
        Serialized_GameUIManager guim = o.SelectToken("guim").ToObject<Serialized_GameUIManager>();

        GameUIManager.gameUIManagerInstance.deserializeGUIM(pp, guim, h, pc);
        
    }

    public void createSaveGame()
    {
        if (EventManager.EventManagerInstance.gameObject.GetComponent<NamedClient>().bufferIsEmpty())
        {
            //Hand
            List<Serialized_Card_Object> h = new List<Serialized_Card_Object>();
            foreach (Transform card in H_location.transform)
            {
                if (card.GetComponent<Draggable>() != null)
                { 
                    if (card.GetComponent<Draggable>().enabled == true)
                    {
                        h.Add(new Serialized_Card_Object(
                               card.gameObject.GetComponent<Image>().sprite.name,
                               card.gameObject.GetComponent<CardID>().isBulletCard,
                               card.gameObject.GetComponent<CardID>().isHidden,
                               card.gameObject.GetComponent<CardID>().playedByGhost,
                               card.gameObject.GetComponent<CardID>().kind,
                               card.gameObject.GetComponent<CardID>().c,
                               true,
                               card.gameObject.GetComponent<Draggable>().originalIndex,
                               card.gameObject.GetComponent<Draggable>().fromDeck));
                    }
                }
                else
                {
                    h.Add(new Serialized_Card_Object(

                               card.gameObject.GetComponent<Image>().sprite.name,
                               card.gameObject.GetComponent<CardID>().isBulletCard,
                               card.gameObject.GetComponent<CardID>().isHidden,
                               card.gameObject.GetComponent<CardID>().playedByGhost,
                               card.gameObject.GetComponent<CardID>().kind,
                               card.gameObject.GetComponent<CardID>().c,
                               false,
                               0,
                               false));
                }
            }

            //Played Cards
            List<Serialized_Card_Object> pc = new List<Serialized_Card_Object>();
            foreach (Transform card in PC_location.transform)
            {
                if (card.GetComponent<Draggable>() != null)
                {
                    if (card.GetComponent<Draggable>().enabled == true)
                    {
                        pc.Add(new Serialized_Card_Object(
                               card.gameObject.GetComponent<Image>().sprite.name,
                               card.gameObject.GetComponent<CardID>().isBulletCard,
                               card.gameObject.GetComponent<CardID>().isHidden,
                               card.gameObject.GetComponent<CardID>().playedByGhost,
                               card.gameObject.GetComponent<CardID>().kind,
                               card.gameObject.GetComponent<CardID>().c,
                               true,
                               card.gameObject.GetComponent<Draggable>().originalIndex,
                               card.gameObject.GetComponent<Draggable>().fromDeck));
                    }
                }
                else
                {
                    pc.Add(new Serialized_Card_Object(
                               card.gameObject.GetComponent<Image>().sprite.name,
                               card.gameObject.GetComponent<CardID>().isBulletCard,
                               card.gameObject.GetComponent<CardID>().isHidden,
                               card.gameObject.GetComponent<CardID>().playedByGhost,
                               card.gameObject.GetComponent<CardID>().kind,
                               card.gameObject.GetComponent<CardID>().c,
                               false,
                               0,
                               false));
                }
            }


            //Listeners
            Serialized_Listeners sl = new Serialized_Listeners
            (
                l_uCP_uFP_location.GetComponent<UpdateCurrentPlayerListener>().getPreviousPlayer(),
                l_uCP_uFP_location.GetComponent<UpdateFirstPlayerListener>().getPreviousPlayer(),
                l_uCR_location.GetComponent<UpdateCurrentRoundListener>().getCurrentRound(),
                l_uCT_location.GetComponent<UpdateCurrentTurnListener>().getPreviousTurn(),
                UpdateTopCardListener.turmoilCardsPlayed,
                HightlightTopCardListener.pileFlipped,
                MoveStageCoachListener.atIndex
            );

            //Player Profiles
            List<Serialized_Player_Profile_Object> pp = new List<Serialized_Player_Profile_Object>();
            //GameUIManager
            Serialized_GameUIManager guim = new Serialized_GameUIManager();
            GameUIManager.gameUIManagerInstance.serializeGUIM(pp, guim);

            var ClientState = new
            {
                NamedClient_c = NamedClient.c,
                h = h,
                pc = pc,
                sl = sl,
                pp = pp,
                guim = guim
            };

            string path = Directory.GetCurrentDirectory();
            path += "\\clientstate.txt";
            using (StreamWriter file = File.CreateText(path)) 
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, ClientState);
            }

            Debug.Log("Client saved successfully.");

        }
    }

}

public class Serialized_Card_Object
{
    public string g_Sprite;

    public bool         s_CardID_isBulletCard;
    public bool         s_CardID_isHidden;
    public bool         s_CardID_playedByGhost;

    public ActionKind   s_CardID_kind;
    public Character    s_CardID_c;

    public bool         hasDraggable;
    public int          s_originalIndex;
    public bool         s_fromDeck;

    public Serialized_Card_Object(string g_Sprite, bool s_CardID_isBulletCard, bool s_CardID_isHidden, bool s_CardID_playedByGhost, ActionKind s_CardID_kind, Character s_CardID_c, bool hasDraggable, int s_originalIndex, bool s_fromDeck)
    {
        this.g_Sprite =                 g_Sprite;

        this.s_CardID_isBulletCard =    s_CardID_isBulletCard;
        this.s_CardID_isHidden =        s_CardID_isHidden;
        this.s_CardID_playedByGhost =   s_CardID_playedByGhost;

        this.s_CardID_kind =            s_CardID_kind;
        this.s_CardID_c =               s_CardID_c;

        this.hasDraggable =             hasDraggable;
        this.s_originalIndex =          s_originalIndex;
        this.s_fromDeck =               s_fromDeck;
    }
}

public class Serialized_Player_Profile_Object
{
    public Character    p_Bandit;
    public string       p_Hostage;

    public int          p_Unknown_Whiskey;
    public int          p_Normal_Whiskey;
    public int          p_Old_Whiskey;

    public int          p_Bullets;
    public int          p_Purses;
    public int          p_Strongboxes;
    public int          p_Rubies;

    public bool         p_FirstPlayer;
    public bool         p_AbilityDisabled;
    public bool         p_HideDisabled;

    public Serialized_Player_Profile_Object()
    {

    }
}

public class Serialized_Listeners
{
    public          Character? uCP_previousPlayer;
    public          Character? uFP_previousPlayer;

    //UpdateCurrentRound
    public int      uCR__currentRound;

    //UpdateCurrentTurn
    public int      uCT_previousTurn;

    //UpdateTopCard
    //HighlightTopCard
    public int      uTC_turmoilCardsPlayed;
    public bool     hTC_pileFlipped;

    //MoveStageCoach
    public int      mSC_atIndex;

    public Serialized_Listeners(Character? uCP_previousPlayer, Character? uFP_previousPlayer, int uCR__currentRound, int uCT_previousTurn, int uTC_turmoilCardsPlayed, bool hTC_pileFlipped, int mSC_atIndex)
    {
        this.uCP_previousPlayer =       uCP_previousPlayer;
        this.uFP_previousPlayer =       uFP_previousPlayer;

        this.uCR__currentRound =        uCR__currentRound;

        this.uCT_previousTurn =         uCT_previousTurn;

        this.uTC_turmoilCardsPlayed =   uTC_turmoilCardsPlayed;
        this.hTC_pileFlipped =          hTC_pileFlipped;

        this.mSC_atIndex =              mSC_atIndex;
    }

}

public class Serialized_Vector3
{
    public float x;
    public float y;
    public float z;

    public Serialized_Vector3()
    {

    }
}

public class Serialized_GameUIManager
{
    public int numPlayers;
    public List<Character> characters = new List<Character>();
    
    public List<int> indices = new List<int>();

    public Serialized_Vector3 cabooseTransform = new Serialized_Vector3();
    public Serialized_Vector3 cabooseRoofLootTransform = new Serialized_Vector3();
    public Serialized_Vector3 cabooseInteriorLootTransform = new Serialized_Vector3();

    public Serialized_Vector3 stageCoachTransform = new Serialized_Vector3();
    public Serialized_Vector3 stageCoachRoofLootTransform = new Serialized_Vector3();
    public Serialized_Vector3 stageCoachInteriorLootTransform = new Serialized_Vector3();

    public Dictionary<int, int> horsesAtIndices = new Dictionary<int, int>();

    public GameStatus gameStatus;

    public bool isNormalTurn;
    public bool isTunnelTurn;
    public bool isTurmoilTurn;
    public bool whiskeyWasUsed;
    public bool abilityDisabled;
    public bool photographerHideDisabled;
    public (bool, ActionKind) actionBlocked = (false, ActionKind.Marshal);
    public int currentTurnIndex;

    public Serialized_GameUIManager()
    {

    }                    

}