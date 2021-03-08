using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using System;

public class buttonManger : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject forward;
    public GameObject backward;
    public GameObject up;
    public GameObject down;
    public GameObject punch;
    public GameObject parent;
 
 
    void Update()
    {

      
           
            Transform a = parent.transform;
            try {
            if (a.GetChild(0) != null && ((a.GetChild(0).name).Equals("move1") || (a.GetChild(0).name).Equals("move2")))
            {

              

                forward.SetActive(true);
                backward.SetActive(true);




            }
            else if (a.GetChild(0) != null && ((a.GetChild(0).name).Equals("changeFloor1") || (a.GetChild(0).name).Equals("changeFloor2")))
            {



                up.SetActive(true);
                down.SetActive(true);



            }
            else if (a.GetChild(0) != null && (a.GetChild(0).name).Equals("punch")) {
                Debug.Log("true");
                punch.SetActive(true);
            }
        }
            catch (Exception e) {
                return;
            }

        }
    
}
