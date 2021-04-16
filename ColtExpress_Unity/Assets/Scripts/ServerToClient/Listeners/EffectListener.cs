using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectListener : UIEventListenable
{
    public AudioSource audioPlayer;

    public AudioClip shoot;
    public AudioClip punch;
    public AudioClip draw;
    public AudioClip drink;
    public AudioClip play;
    public AudioClip running;
    public AudioClip lootchanged;
    public AudioClip horse;

    public AudioClip theme;
    public AudioClip train;

    public GameObject nightTime;
    public GameObject background;
    public GameObject midground;
    public GameObject foreground;

    public override void updateElement(string data)
    {

    }
}
