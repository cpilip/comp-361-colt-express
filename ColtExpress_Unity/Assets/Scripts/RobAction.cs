using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobAction : MonoBehaviour
{
    public GameObject player;
    public GameObject train;
    public GameObject played;
    public GameObject discard;

    public void Start()
    {
        int parentIndex = player.transform.parent.transform.GetSiblingIndex();
        int playerIndex = player.transform.GetSiblingIndex();
        int numChild = player.transform.parent.transform.childCount;
        Debug.Log("parentIndex: "+parentIndex+" playerIndex: "+playerIndex+" numChild: "+numChild);

        ArrayList ValuableList = new ArrayList();
    }
}
