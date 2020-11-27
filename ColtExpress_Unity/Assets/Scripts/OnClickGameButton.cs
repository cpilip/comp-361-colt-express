using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnClickGameButton : MonoBehaviour
{
    public void OnClickFunction()
    {
        GameObject contentPanel = GameObject.Find("Content");
        contentPanel.GetComponent<SelectedButton>().Button = this.gameObject;
        contentPanel.GetComponent<SelectedButton>().HasChanged = true;
    }
}
