using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class setting : MonoBehaviour
{
    public GameObject Panel;
    int counter;
    
    public void showhidePanel() {
        counter++;

        if (counter % 2 == 1) { Panel.gameObject.SetActive(false); }
        else
        {
            Panel.gameObject.SetActive(true);
        }

    }
}
