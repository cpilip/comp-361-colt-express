using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControl : MonoBehaviour
{
    public GameObject GameMenu;
    public void OpenMenu(){
      if (GameMenu!=null){
        bool isActive = GameMenu.activeSelf;
        GameMenu.SetActive(!isActive);
      }
    }
}
