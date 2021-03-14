using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class EventTriggerTester : MonoBehaviour
{
    public static int x;
    public static int y;
    public static int z;

    // Update is called once per frame
    private void Start()
    {
        x = 1;
        y = 0;
        z = 0;
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.O))
        {
            EventManager.TriggerEvent("updateRound", x.ToString());
            x++;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            EventManager.TriggerEvent("updateTurns", "SpeedingUp");
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            EventManager.TriggerEvent("updateNextTurn", y.ToString());
            y++;

            if (y == 5)
            {
                y = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            EventManager.TriggerEvent("updateNextPlayer", z.ToString());
            z++;

            if (z == 3)
            {
                z = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.B))
        { 
           string data = JsonConvert.SerializeObject(TurnType.Tunnel, new Newtonsoft.Json.Converters.StringEnumConverter());
           EventManager.TriggerEvent("testTurn", data);
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            JSONTestObject myTestObject = new JSONTestObject(3, "hello!");
            myTestObject.Value = 3;
            myTestObject.Name = "bee!";

            string dataToSend = JsonConvert.SerializeObject(myTestObject);

            Debug.Log("testing json");

            EventManager.TriggerEvent("testJSON", dataToSend);
        }

    }
}

public class JSONTestObject {
    private int v;
    private string name;

    public JSONTestObject(int pValue, string pName)
    {
        v = pValue;
        name = pName;
    }

    public string Name { get { return name;  } set { name = value; } }
    public int Value { get { return v; } set { v = value; } }
}
