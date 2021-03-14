using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class MarshalControl : MonoBehaviour
{
    public GameObject parent;
    public GameObject train;
    public GameObject player;
    public GameObject parentButton;
    public GameObject other1;
    public GameObject discard;

    public void check()
    {
        Transform a = parent.transform;

        if (a.GetChild(0) != null)
        {
            int pos = player.transform.parent.transform.GetSiblingIndex();
            //Debug.Log("pos"+pos);
            if (gameObject.name == "frontMarshal")
            {

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
                else
                {
                    if (pos == 13)
                    {
                        pos = pos;
                    }
                    else
                    {
                        pos++;
                    }

                }


                Transform b = train.transform;
                Transform c = b.GetChild(pos);
                player.transform.SetParent(c);
            }
            else if (gameObject.name == "backMarshal")
            {
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

            
        }

        parentButton.SetActive(false);
        other1.SetActive(false);
        a.GetChild(0).SetParent(discard.transform);
    }
}
