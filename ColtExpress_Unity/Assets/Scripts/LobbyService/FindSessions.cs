using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.SceneManagement;


public class FindSessions : MonoBehaviour
{
    private LobbyCommandsClient LobbyCommands = new LobbyCommandsClient();

    List<string> gameSessions;

    public Dropdown sessionsDropDown;
    public GameObject sessionText;
    public GameObject currentInLobbyText;
    public GameObject pingText;

    public Dictionary<string, SessionInformation> data;

    public Object SessionPrefab;


    // Start is called before the first frame update
    void Start()
    {   
        findGames();
    }

    public void findGames()
    {
        StartCoroutine(findGamesWait(1));
    }

    IEnumerator StartPing(string ip)
    {
        WaitForSeconds f = new WaitForSeconds(0.05f);
        Ping p = new Ping(ip);
        while (p.isDone == false)
        {
            yield return f;
        }
        Debug.Log("Ping is done   " + p.time);
        PingFinished(p);
    }


    public void PingFinished(Ping p)
    {
        Text pText = pingText.GetComponent<Text>();
        pText.text = "" + p.time + " ms";
    }

    public void DropdownValueChanged(Dropdown change)
    {
        string val = change.GetComponent<Dropdown>().options[change.value].text;

        Text sText = sessionText.GetComponent<Text>();
        Text cText = currentInLobbyText.GetComponent<Text>();
        Text pText = pingText.GetComponent<Text>();

        if (val.Equals("select game..."))
        {
            sText.text = "--";
            cText.text = "--";
            pText.text = "--";
        }
        else
        {
            // Update creator name
            sText.text = data[val].creator;
            // Update number of players
            int numPlayers = data[val].players.Count;
            cText.text = "" + numPlayers + "/6";
            // update player ping
            StartCoroutine(StartPing("google.com"));
        }
    }

    private IEnumerator findGamesWait(float time)
    {

        // Issue lobby service request
        LobbyCommands.getSessions(this);
        yield return new WaitForSeconds(time);
        string response = LobbyCommands.getResponse();
        Debug.Log(response);

        // Parse response
        JObject o = JObject.Parse(response);
        JObject sessions = JObject.Parse(o.SelectToken("sessions").ToString());

        this.data = new Dictionary<string, SessionInformation>();

        // Populate the dictionary with the info about all the current sessions
        foreach (var session in sessions)
        {
            string name = session.Key;
            JToken value = session.Value;
            SessionInformation sessInfo = value.ToObject<SessionInformation>();

            data.Add(name, sessInfo);
        }

        // Add all the available sessions to dropdown 
        List<string> dropdownKeys = new List<string>();
        dropdownKeys.Add("select game...");
        foreach (string s in data.Keys)
        {
            Debug.Log(s);
            dropdownKeys.Add(s);
        }


        sessionsDropDown.GetComponent<Dropdown>().ClearOptions();
        sessionsDropDown.GetComponent<Dropdown>().AddOptions(dropdownKeys);
    }

    public void joinSession()
    {
        GameObject sessionId;

        string sessIdStr = this.sessionsDropDown.options[sessionsDropDown.value].text;
        string urName = GameObject.Find("ID").GetComponent<Identification>().getUsername();
        string token = GameObject.Find("ID").GetComponent<Identification>().getToken();

        LobbyCommands.joinSession(this, sessIdStr, urName, token);

        string response = LobbyCommands.getResponse();
        Debug.Log(response);

        if (GameObject.Find("sessionId") == null)
        {
            sessionId = (GameObject)Instantiate(SessionPrefab);
            sessionId.name = "sessionId";
        }
        else
        {
            sessionId = GameObject.Find("sessionId");
        }

        sessionId.GetComponent<SessionPrefabScript>().setSessionId(sessIdStr);
        sessionId.GetComponent<SessionPrefabScript>().setCreator(false);
        
        SceneManager.LoadScene("Lobby");
    }
}
