using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class buttonManger : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject forward;
    public GameObject backward;
    public lu fm;
    public lu bm;
    public GameObject parent;
   
 
    void Update()
    {

        if ((fm.hasSet || bm.hasSet) == true)
        {
            return;

        }
        else
        {
           
            Transform a = parent.transform;
            try { 
             if (a.GetChild(0) != null)
            {
                forward.SetActive(true);
                backward.SetActive(true);
                    fm.hasSet = false;
                    bm.hasSet = false;

            }
        }
            catch (Exception e) {
                return;
            }

        }
    }
}
