using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterSelect : MonoBehaviour{

   public Button startButton;
   public Button purpleButton;
   public Button redButton;
   public Button whiteButton;
   public Button greenButton;
   public Button blackButton;
   public Button blueButton;
   public int storeIndex;


    public void start(){
       startButton.onClick.AddListener(()=>ButtonClicked(9));
       purpleButton.onClick.AddListener(()=>ButtonClicked(0));
       redButton.onClick.AddListener(()=>ButtonClicked(1));
       whiteButton.onClick.AddListener(()=>ButtonClicked(2));
       greenButton.onClick.AddListener(()=>ButtonClicked(5));
       blackButton.onClick.AddListener(()=>ButtonClicked(3));
       blueButton.onClick.AddListener(()=>ButtonClicked(4));

   }

   public void ButtonClicked(int buttonNo){
        
        if(buttonNo == 9) {
             Debug.Log("start is = " + buttonNo);
             PlayerPrefs.SetInt("passValue",storeIndex);
             Debug.Log("The last is: "+storeIndex);
             SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 2 );
        }else{
		storeIndex = buttonNo;
		 Debug.Log("Button clicked character is = " + storeIndex);
           
            
        }
   }

}
