using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using RoundSpace;

public class TurnsElementListener : UIEventListenable
{
    private static Dictionary<TurnType, GameObject> turnPrefabs;
    private static GameObject[] turnTypePrefabs;

    private void Awake()
    {
        turnTypePrefabs = Resources.LoadAll<GameObject>("TurnType Prefabs");
        
        turnPrefabs = new Dictionary<TurnType, GameObject>();

        foreach (GameObject p in turnTypePrefabs)
        {
            turnPrefabs.Add((TurnType)Enum.Parse(typeof(TurnType), p.name), p);
        } 
    }

    public override void updateElement(String data)
    {
        foreach (Transform child in this.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        for (int i = 0; i < 2; i++)
        {
            GameObject generatedPrefab = null;
            if (turnPrefabs.TryGetValue(TurnType.SpeedingUp, out generatedPrefab))
            {
                GameObject generatedTurn = Instantiate(generatedPrefab);
                generatedTurn.transform.parent = this.transform;

                Vector3 scaleChange = new Vector3(1f, 1f, 1f);
                generatedTurn.transform.localScale = scaleChange;
            }
        }

        for (int i = 0; i < 2; i++)
        {
            GameObject generatedPrefab = null;
            if (turnPrefabs.TryGetValue(TurnType.Standard, out generatedPrefab))
            {
                GameObject generatedTurn = Instantiate(generatedPrefab);
                generatedTurn.transform.parent = this.transform;

                Vector3 scaleChange = new Vector3(1f, 1f, 1f);
                generatedTurn.transform.localScale = scaleChange;
            }
        }

        Debug.Log("Updated turns element");
    }
}


