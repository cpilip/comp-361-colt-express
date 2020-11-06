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
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 3);
  }

  public void signIn()
  {
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 4);
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
}
