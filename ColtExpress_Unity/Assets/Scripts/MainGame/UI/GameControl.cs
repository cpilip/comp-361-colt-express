using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControl : MonoBehaviour
{
    public GameObject GameMenu;
    public void OpenMenu(){
      if (GameMenu!=null){
        bool isActive = GameMenu.activeSelf;
        GameMenu.SetActive(!isActive);

        Scene currentScene = SceneManager.GetActiveScene();

        // Retrieve the name of this scene.
        string sceneName = currentScene.name;

        if (sceneName == "MainGame")
        {

            GameUIManager.gameUIManagerInstance.gameObject.GetComponent<SaveManager>().canSave();
        }
            
      }
    }
}
