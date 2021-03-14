﻿using System.Collections;
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
    SceneManager.LoadScene("Play");
  }

  public void signIn()
  {
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 4);
  }

  public void inviteFriend()
  {
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
  }

  public void newGame()
  {
    SceneManager.LoadScene ("NewGame");
  }

  public void loadGame()
  {
    SceneManager.LoadScene ("LoadGame");
  }

  public void findGame()
  {
    SceneManager.LoadScene ("FindGame");
  }

  public void newSession()
  {
    SceneManager.LoadScene("NewSession");
  }

  // public void loadGame()
  // {
  //   SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
  // }

  public void startGame()
  {
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 3);
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

}
