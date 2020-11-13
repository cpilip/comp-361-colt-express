using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class setting : MonoBehaviour
{
    public GameObject Panel;
    
    
    public void showhidePanel() {
       

        if (Panel != null) {
            bool isActive = Panel.activeSelf;
            Panel.SetActive(!isActive); }
       

    }
    public AudioMixer audioMixer;
    public void setVolume(float volume) {
        audioMixer.SetFloat("volume", volume);


    }
}
