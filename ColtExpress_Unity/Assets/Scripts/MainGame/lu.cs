using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class lu : MonoBehaviour
{
    public GameObject parent;
    public GameObject train;
    public GameObject player;
    public void check() {
        /* if (parent.transform.GetChild(0).gameObject != null) {
             GameObject col=GameObject.Instantiate(parent.transform.GetChild(0).gameObject);
             col.SetParent(parent.transorm);
         }*/
        Transform a = parent.transform;
        if (a.GetChild(0)!=null) {
            /* Transform b = a.GetChild(0);
             GameObject c = b.gameObject;
             GameObject d = GameObject.Instantiate(c);
             d.transform.SetParent(parent.transform);*/
            Transform b = train.transform;
            Transform c = b.GetChild(1);
            player.transform.SetParent(c);
        }
    }

    
}
