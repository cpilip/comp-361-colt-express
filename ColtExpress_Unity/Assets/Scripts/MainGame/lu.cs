using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class lu : MonoBehaviour
{
    public GameObject parent;
    public GameObject train;
    public GameObject player;
    public GameObject parentButton;
    public GameObject other1;
    public bool hasSet = false;
    
    public void check() {
  
        Transform a = parent.transform;
 
        if (a.GetChild(0)!=null) {
            if (gameObject.name == "fowardMan") {

                Transform b = train.transform;
                Transform c = b.GetChild(2);
                player.transform.SetParent(c);
            } else if (gameObject.name == "backwordMan") {
                Transform b = train.transform;
                Transform c = b.GetChild(0);
                player.transform.SetParent(c);
            }
           
            
        }
        parentButton.SetActive(false);
        other1.SetActive(false);
        hasSet = true;
      
    }

    
}
