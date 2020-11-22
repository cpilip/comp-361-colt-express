using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindGames : MonoBehaviour
{
    private LobbyCommandsClient LobbyCommands = new LobbyCommandsClient();

    // Start is called before the first frame update
    void Start()
    {
        findGames();
    }

    public void findGames() {
        StartCoroutine(findGamesWait(1));
    }

    private IEnumerator findGamesWait(float time){
        LobbyCommands.getGameServices(this);
        yield return new WaitForSeconds(time);
        string response = LobbyCommands.getResponse();
        Debug.Log(response);
        response = response.Replace("[", "");
        response = response.Replace("]", "");
        response = response.Replace("\"", "");
        List<string> gameServicesNames = new List<string>(response.Split(','));
        foreach (string st in gameServicesNames) {
            Debug.Log(st);
        }
        GameObject.Find("GameChooser").GetComponent<GameList>().setGames(gameServicesNames); 
    }
}
