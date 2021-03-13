using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameList : MonoBehaviour
{
    public List<string> Games;
    public GameObject ButtonPrefab;

    public void setGames(List<string> newList) 
    {
        this.Games = new List<string>(newList);
        this.refreshAllButtons();
    }

    private void refreshAllButtons()
    {
       
        GameObject contentPanel = GameObject.Find("Content");
        foreach (Transform child in contentPanel.transform) {
            GameObject.Destroy(child.gameObject);
        }
        foreach (string st in this.Games)
        {
            GameObject newButton = Instantiate(ButtonPrefab) as GameObject;
            newButton.transform.SetParent(contentPanel.transform);
            newButton.GetComponentInChildren<Text>().text = st;
            newButton.tag = "listButton";
        }

    }
}
