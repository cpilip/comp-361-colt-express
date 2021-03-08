using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class punch : MonoBehaviour
{
    public GameObject player;
    public GameObject train;
    public GameObject parentButton;
    public GameObject played;
    public GameObject discard;
    public void punchOther() {
        int parentIndex = player.transform.parent.transform.GetSiblingIndex();
        int playerIndex = player.transform.GetSiblingIndex();
        int numChild = player.transform.parent.transform.childCount;
        ArrayList otherP = new ArrayList();

        if (numChild > 1)
        {
            for (int i = 0; i < numChild; i++)
            {
                if (i != playerIndex)
                {
                    otherP.Add(player.transform.parent.transform.GetChild(i).gameObject);
                }
            }
            if (parentIndex == 6 || parentIndex == 13)
            {
                int newParentIndex = parentIndex - 1;
                Transform newParent = train.transform.GetChild(newParentIndex);
                GameObject temp=(GameObject)otherP[0];
                temp.transform.SetParent(newParent);
            }
            else
            {

                int newParentIndex = parentIndex + 1;
                Transform newParent = train.transform.GetChild(newParentIndex);
                GameObject temp =(GameObject) otherP[0];
                temp.transform.SetParent(newParent);

            }

        }
        played.transform.GetChild(0).SetParent(discard.transform);
        parentButton.SetActive(false);
        
    }
}
