using GameUnitSpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectManager : MonoBehaviour
{
    public GameObject Bella;
    public GameObject Tuco;
    public GameObject Django;
    public GameObject Ghost;
    public GameObject Doc;
    public GameObject Cheyenne;
    private Vector3 CharacterPosition;
    private Vector3 OffScreen;
    private int characterIndex = 1;
    private readonly string selectedCharacter = "SelectedCharacter";

    private void Awake() {
        CharacterPosition = Bella.transform.position;
        OffScreen = Tuco.transform.position;
        PlayerPrefs.SetInt(selectedCharacter, 6);
    }

    public void NextCharacter() {
        switch (characterIndex) {
            case 1:
                PlayerPrefs.SetInt(selectedCharacter, 1);
                Bella.SetActive(false);
                Bella.transform.position = OffScreen;
                Tuco.transform.position = CharacterPosition;
                Tuco.SetActive(true);
                characterIndex++;
                break;
            case 2:
                PlayerPrefs.SetInt(selectedCharacter, 2);
                Tuco.SetActive(false);
                Tuco.transform.position = OffScreen;
                Django.transform.position = CharacterPosition;
                Django.SetActive(true);
                characterIndex++;
                break;
            case 3:
                PlayerPrefs.SetInt(selectedCharacter, 3);
                Django.SetActive(false);
                Django.transform.position = OffScreen;
                Ghost.transform.position = CharacterPosition;
                Ghost.SetActive(true);
                characterIndex++;
                break;
            case 4:
                PlayerPrefs.SetInt(selectedCharacter, 4);
                Ghost.SetActive(false);
                Ghost.transform.position = OffScreen;
                Doc.transform.position = CharacterPosition;
                Doc.SetActive(true);
                characterIndex++;
                break;
            case 5:
                PlayerPrefs.SetInt(selectedCharacter, 5);
                Doc.SetActive(false);
                Doc.transform.position = OffScreen;
                Cheyenne.transform.position = CharacterPosition;
                Cheyenne.SetActive(true);
                characterIndex++;
                break;
            case 6:
                PlayerPrefs.SetInt(selectedCharacter, 6);
                Cheyenne.SetActive(false);
                Cheyenne.transform.position = OffScreen;
                Bella.transform.position = CharacterPosition;
                Bella.SetActive(true);
                characterIndex++;
                ResetInt();
                break;
            default:
                PlayerPrefs.SetInt(selectedCharacter, 6);
                ResetInt();
                break;
        }

    }

    private void ResetInt() {
        if (characterIndex >= 6) characterIndex = 1;
        else characterIndex = 6;
    }

    public void PreviousCharacter()
    {
        switch (characterIndex) {
            case 1:
                PlayerPrefs.SetInt(selectedCharacter, 5);
                Bella.SetActive(false);
                Bella.transform.position = OffScreen;
                Cheyenne.transform.position = CharacterPosition;
                Cheyenne.SetActive(true);
                //characterIndex--;
                ResetInt();
                break;
            case 2:
                PlayerPrefs.SetInt(selectedCharacter, 6);
                Tuco.SetActive(false);
                Tuco.transform.position = OffScreen;
                Bella.transform.position = CharacterPosition;
                Bella.SetActive(true);
                characterIndex--;
                break;
            case 3:
                PlayerPrefs.SetInt(selectedCharacter, 1);
                Django.SetActive(false);
                Django.transform.position = OffScreen;
                Tuco.transform.position = CharacterPosition;
                Tuco.SetActive(true);
                characterIndex--;
                break;
            case 4:
                PlayerPrefs.SetInt(selectedCharacter, 2);
                Ghost.SetActive(false);
                Ghost.transform.position = OffScreen;
                Django.transform.position = CharacterPosition;
                Django.SetActive(true);
                characterIndex--;
                break;
            case 5:
                PlayerPrefs.SetInt(selectedCharacter, 3);
                Doc.SetActive(false);
                Doc.transform.position = OffScreen;
                Ghost.transform.position = CharacterPosition;
                Ghost.SetActive(true);
                characterIndex--;
                break;
            case 6:
                PlayerPrefs.SetInt(selectedCharacter, 4);
                Cheyenne.SetActive(false);
                Cheyenne.transform.position = OffScreen;
                Doc.transform.position = CharacterPosition;
                Doc.SetActive(true);
                characterIndex--;
                break;
        }
    }



    public void startButton() {

        int getCharacter = PlayerPrefs.GetInt(selectedCharacter);
        switch (getCharacter)
        {
            case 1:
                NamedClient.c = Character.Tuco;
                Debug.Log("[GetCharacter] You selected Tuco.");
                ClientCommunicationAPI.CommunicationAPI.sendMessageToServer(Character.Tuco);
                break;
            case 2:
                NamedClient.c = Character.Django;
                Debug.Log("[GetCharacter] You selected Django.");
                ClientCommunicationAPI.CommunicationAPI.sendMessageToServer(Character.Django);
                break;
            case 3:
                NamedClient.c = Character.Ghost;
                Debug.Log("[GetCharacter] You selected Ghost.");
                ClientCommunicationAPI.CommunicationAPI.sendMessageToServer(Character.Ghost);
                break;
            case 4:
                NamedClient.c = Character.Doc;
                Debug.Log("[GetCharacter] You selected Doc.");
                ClientCommunicationAPI.CommunicationAPI.sendMessageToServer(Character.Doc);
                break;
            case 5:
                NamedClient.c = Character.Cheyenne;
                Debug.Log("[GetCharacter] You selected Cheyenne.");
                ClientCommunicationAPI.CommunicationAPI.sendMessageToServer(Character.Cheyenne);
                break;
            case 6:
                NamedClient.c = Character.Belle;
                Debug.Log("[GetCharacter] You selected Belle.");
                ClientCommunicationAPI.CommunicationAPI.sendMessageToServer(Character.Belle);
                break;
            default:
                Debug.LogError("[GetCharacter] No character selected. Something went wrong!");
                break;

        }

        SceneManager.LoadScene(8);
    }

}
