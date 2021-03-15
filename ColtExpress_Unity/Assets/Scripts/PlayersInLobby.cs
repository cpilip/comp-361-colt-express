using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayersInLobby : MonoBehaviour
{
    private string[] players {get; set;}
    public Text text;

    void update() {
        string str = "";
        for (int i = 0 ; i < this.players.Length; i++) {
            str = str + players;
        }
        text.text = str;
    } 

    public void setPlayers(string[] arg) {
        this.players = arg;
    } 
}
