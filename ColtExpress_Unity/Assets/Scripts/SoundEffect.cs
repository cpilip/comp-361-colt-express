using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffect : MonoBehaviour
{
  public AudioSource soundEffect;
  public AudioClip clickEffect;

  public void ClickSound(){
    soundEffect.PlayOneShot(clickEffect);
  }
}
