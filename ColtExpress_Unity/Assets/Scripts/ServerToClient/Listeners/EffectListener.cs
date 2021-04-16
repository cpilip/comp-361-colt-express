using GameUnitSpace;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectListener : UIEventListenable
{
    public AudioSource audioPlayerEffect;
    public AudioClip shoot;
    public AudioClip punch;
    public AudioClip draw;
    public AudioClip drink;
    public AudioClip play;
    public AudioClip running;
    public AudioClip lootchanged;
    public AudioClip horse;

    public GameObject nightTime;
    public GameObject background;
    public GameObject midground;
    public GameObject foreground;
    private Color originalColor;

    public override void updateElement(string data)
    {
        JObject o = JObject.Parse(data);
        string e = o.SelectToken("effect").ToObject<string>();

        switch (e)
        {
            case "shoot":
                Character c = o.SelectToken("player").ToObject<Character>();

                GameUIManager.gameUIManagerInstance.getCharacterObject(c).GetComponent<Image>().color = new Color(0.749f, 1f, 1f, 1f);
                GameUIManager.gameUIManagerInstance.getCharacterObject(c).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);

                audioPlayerEffect.clip = shoot;
                audioPlayerEffect.PlayOneShot(shoot);
                break;
            case "punch":
                Character d = o.SelectToken("player").ToObject<Character>();

                GameUIManager.gameUIManagerInstance.getCharacterObject(d).GetComponent<Image>().color = new Color(0.749f, 1f, 1f, 1f);

                GameUIManager.gameUIManagerInstance.getCharacterObject(d).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);

                audioPlayerEffect.clip = punch;
                audioPlayerEffect.PlayOneShot(punch);
                break;
            case "draw":
                audioPlayerEffect.clip = draw;
                audioPlayerEffect.PlayOneShot(draw);
                break;
            case "drink":
                audioPlayerEffect.clip = drink;
                audioPlayerEffect.PlayOneShot(drink);
                break;
            case "play":
                audioPlayerEffect.clip = play;
                audioPlayerEffect.PlayOneShot(play);
                break;
            case "running":
                audioPlayerEffect.clip = running;
                audioPlayerEffect.PlayOneShot(running);
                break;
            case "lootchanged":
                audioPlayerEffect.clip = lootchanged;
                audioPlayerEffect.PlayOneShot(lootchanged);
                break;
            case "horse":
                audioPlayerEffect.clip = horse;
                audioPlayerEffect.PlayOneShot(horse);
                break;
            case "night":
                StartCoroutine("goNight");
                break;
            case "day":
                StartCoroutine("goDay");
                break;
            case "speed":
                midground.GetComponent<Parallax>().speed = new Vector2(4, 4);
                foreground.GetComponent<Parallax>().speed = new Vector2(6, 6);
                break;
            case "slow":
                midground.GetComponent<Parallax>().speed = new Vector2(1, 1);
                foreground.GetComponent<Parallax>().speed = new Vector2(2, 2);
                break;
        }


    }

    public IEnumerator goNight()
    {
        var tempColor = nightTime.GetComponent<SpriteRenderer>().color;
        originalColor = tempColor;
        while (nightTime.GetComponent<SpriteRenderer>().color.a < .333f)
        {
            tempColor = new Color(tempColor.r, tempColor.g, tempColor.b, tempColor.a + (Time.deltaTime / 1.0f));
            nightTime.GetComponent<SpriteRenderer>().color = tempColor;
            yield return null;
        }
       
    }

    public IEnumerator goDay()
    {
        var tempColor = nightTime.GetComponent<SpriteRenderer>().color;
        while (nightTime.GetComponent<SpriteRenderer>().color.a > 0f)
        {
            tempColor = new Color(tempColor.r, tempColor.g, tempColor.b, tempColor.a - (Time.deltaTime / 1.0f));
            nightTime.GetComponent<SpriteRenderer>().color = tempColor;
            yield return null;
        }

    }
}
