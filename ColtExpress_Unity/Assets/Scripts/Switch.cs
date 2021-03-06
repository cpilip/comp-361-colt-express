using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Switch : MonoBehaviour
{
  public void readRule()
  {
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
  }

  public void readCharacter()
  {
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
  }

  public void playGame()
  {
    // Redirect to sign in if player is not logged in
    if (GameObject.Find("ID") != null) {
      SceneManager.LoadScene("Play");
    } else {
      SceneManager.LoadScene("SignIn");
    }
  }

  public void signIn()
  {
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 4);
  }
    public void goToSeting()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 9);
    }

    public void inviteFriend()
  {
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
  }

  public void loadGame()
  {
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
  }

  public void startGame()
  {
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 3);
  }
    public void load()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ruleBack()
  {
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
  }

  public void characterBack()
  {
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 2);
  }

  public void playBack()
  {
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 3);
  }

  public void signInBack()
  {
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 4);
  }

  public void inviteBack()
  {
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
  }

  public void loadBack()
  {
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 2);
  }
    public void setingBack()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 9);
    }

    public void quitMainGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 8);
    }

    public void toGoLobby()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }
    public void getCharacter()
    {
        SceneManager.LoadScene("ChooseCharacter");
    }

    public void goNewSession()
    {
        SceneManager.LoadScene("NewSession");
    }

    public void goFindSession()
    {
        SceneManager.LoadScene("FindSession");
    }

    public void goMainMenu()
    {

        StartCoroutine("UnloadScene");
    }

    private IEnumerator UnloadScene()
    {
        GameObject MainCamera = GameUIManager.gameUIManagerInstance.gameObject;
        GameObject EventManager = FindObjectOfType<NamedClient>().gameObject;

        Destroy(MainCamera);
        Destroy(EventManager);
        // Wait a frame so every Awake and Start method is called
        yield return new WaitForEndOfFrame();

        SceneManager.LoadScene("Menu");
    }

    public void exitGameApplication()
    {
        Application.Quit();
    }

}
