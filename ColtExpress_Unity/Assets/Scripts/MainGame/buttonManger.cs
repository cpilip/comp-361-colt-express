using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using System;

public class buttonManger : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject forward;
    public GameObject forwardM;
    public GameObject backward;
    public GameObject backwardM;

    public GameObject up;
    public GameObject down;
    public GameObject punch;
    public GameObject shoot;
    public GameObject robMan;
    public GameObject parent;
    public GameObject phase1;
    public GameObject phase2;
    public GameObject turnMenu;
    public bool checkPhase = false;

    void Update()
    {



        Transform a = parent.transform;

        int numClick = PlayerPrefs.GetInt("clickTimes");
        if (numClick == 3)
        {
            phase1.SetActive(false);
            phase2.SetActive(true);
            checkPhase = true;
            turnMenu.SetActive(false);


        }
        if (checkPhase == true)
        {

            try
            {
                if (a.childCount == 0)
                {
                    phase1.SetActive(true);
                    phase2.SetActive(false);
                    PlayerPrefs.SetInt("clickTimes", 0);
                    checkPhase = false;
                    return;
                }
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
                else if (a.GetChild(0) != null && (a.GetChild(0).name).Equals("punch"))
                {

                    punch.SetActive(true);
                }
                else if (a.GetChild(0) != null && ((a.GetChild(0).name).Equals("shoot1") || (a.GetChild(0).name).Equals("shoot2")))
                {

                    shoot.SetActive(true);
                }
                else if (a.GetChild(0) != null && (a.GetChild(0).name).Equals("Marshal"))
                {

                    forwardM.SetActive(true);
                    backwardM.SetActive(true);
                }
                else if (a.GetChild(0) != null && (a.GetChild(0).name).Equals("rob1"))
                {

                    robMan.SetActive(true);
                }


                Debug.Log(PlayerPrefs.GetInt("clickTimes"));
            }
            catch (Exception e)
            {
                return;
            }

        }
        Debug.Log(PlayerPrefs.GetInt("clickTimes"));
    }
    
}
