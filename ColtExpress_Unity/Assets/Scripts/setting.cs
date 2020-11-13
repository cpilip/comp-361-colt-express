using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class setting : MonoBehaviour
{
    public GameObject Panel;
    public Dropdown resolutionDropdown;
    Resolution[] resolutions;
    void Start() {
       resolutions= Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();

        for (int i = 0; i < resolutions.Length; i++) {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);
        }

        resolutionDropdown.AddOptions(options);
    }
    
    
    public void showhidePanel() {
       

        if (Panel != null) {
            bool isActive = Panel.activeSelf;
            Panel.SetActive(!isActive); }
       

    }
    public AudioMixer audioMixer;
    public void setVolume(float volume) {
        audioMixer.SetFloat("volume", volume);


    }

    public void setQuality(int qualityIndex) {
        QualitySettings.SetQualityLevel(qualityIndex);
    }
    public void setFullscreen(bool isFull) {
        Screen.fullScreen = isFull;
    }
}
