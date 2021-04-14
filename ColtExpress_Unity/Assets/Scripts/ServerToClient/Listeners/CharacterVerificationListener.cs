using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterVerificationListener : UIEventListenable
{
    public GameObject characterChosenPopup;
    public GameObject selectCharacterButton;
    public override void updateElement(string data)
    {
        /* PRE: data 
        *
            {
                eventName = action,
                currentPlayer = (Character)args[0]
            };
        */

        JObject o = JObject.Parse(data);
        bool characterAlreadyChosen = o.SelectToken("characterAlreadyChosen").ToObject<bool>();

        if (characterAlreadyChosen)
        {
            characterChosenPopup.SetActive(true);
            StartCoroutine("displayingCharacterChosenPopup");
        }
        else
        {
            SceneManager.LoadScene(8);
        }
    }

    private IEnumerator displayingCharacterChosenPopup()
    {
        yield return new WaitForSeconds(1);
        characterChosenPopup.SetActive(false);

        selectCharacterButton.GetComponent<Button>().interactable = true;
    }

}
