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
    public GameObject parent;
 
 
    void Update()
    {

      
           
            Transform a = parent.transform;
            try { 
             if (a.GetChild(0) != null)
            {
                
                Debug.Log(a.GetChild(0).name);

                forward.SetActive(true);
                backward.SetActive(true);
                up.SetActive(true);
                down.SetActive(true);



            }
        }
            catch (Exception e) {
                return;
            }

        }
    
}
