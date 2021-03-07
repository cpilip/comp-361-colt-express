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
    public GameObject other2;
    public GameObject other3;
    public GameObject discard;
    
    public void check() {
      GameObject try1 = GameObject.Find("Train Car 4");
        Transform a = parent.transform;
 
        if (a.GetChild(0)!=null) {
            int pos = player.transform.parent.transform.GetSiblingIndex();
            // Debug.Log("pos"+pos);
            if (gameObject.name == "fowardMan") {

                if (pos >= 0 && pos <= 6)
                {
                    if (pos == 0)
                    {
                        pos++;
                    }
                    else if (pos == 6)
                    {
                        pos = pos;
                    }
                    else
                    {
                        pos++;
                    }


                }
                else {
                    if (pos == 13)
                    {
                        pos = pos;
                    }
                    else {
                        pos++;
                    }

                }


                Transform b = train.transform;
                Transform c = b.GetChild(pos);
                player.transform.SetParent(c);
            } else if (gameObject.name == "backwordMan") {
                if (pos >= 0 && pos <= 6)
                {
                    if (pos == 0)
                    {
                        pos = pos;
                    }

                    else
                    {
                        pos--;
                    }


                }
                else
                {
                    if (pos == 7)
                    {
                        pos = pos;
                    }
                    else
                    {
                        pos--;
                    }

                }



                Transform b = train.transform;
                Transform c = b.GetChild(pos);
                player.transform.SetParent(c);
            }
            else if (gameObject.name == "downMan") {
                if (pos >= 0 && pos <= 6)
                {
                    pos = pos + 7;
                }
                else {
                    pos = pos;
                }
                Transform b = train.transform;
                Transform c = b.GetChild(pos);
                player.transform.SetParent(c);
            }
            else if (gameObject.name=="upMan") {
                if (pos >= 7 && pos <= 13)
                {
                    pos = pos - 7;
                }
                else {
                    pos = pos;
                }
                Transform b = train.transform;
                Transform c = b.GetChild(pos);
                player.transform.SetParent(c);
            }
           
           
            
        }
      
        parentButton.SetActive(false);
        other1.SetActive(false);
        other2.SetActive(false);
        other3.SetActive(false);


        a.GetChild(0).SetParent(discard.transform);
       // DestroyImmediate(a.GetChild(0).gameObject,true);


    }

    
}
