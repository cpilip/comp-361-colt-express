using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;
using GameUnitSpace;
using CardSpace;
using PositionSpace;
using RoundSpace;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text.RegularExpressions;

public class CommunicationAPIHandler : MonoBehaviour
{
    private static KnownTypesBinder knownTypesBinder = new KnownTypesBinder
    {
        KnownTypes = new List<Type> {
            typeof(Card),
            typeof(ActionCard),
            typeof(BulletCard),
            typeof(GameItem),
            typeof(GameUnit),
            typeof(GameStatus),
            typeof(Marshal),
            typeof(Player),
            typeof(Position),
            typeof(Round),
            typeof(TrainCar),
            typeof(Turn)
        }
    };

    //Will include object type in JSON string
    public static JsonSerializerSettings settings = new JsonSerializerSettings
    {
        TypeNameHandling = TypeNameHandling.Auto
    };

    //Get JSON from server and figure out what event to trigger
    public static void getMessageFromServer(string data)
    {
        
        //action, all the objects
        Debug.Log("e");
        JObject o = JObject.Parse(data);
        string eventName = o.SelectToken("eventName").ToString();

        
        EventManager.TriggerEvent(eventName, data);


    }
}



public class BaseSpecifiedConcreteClassConverter : DefaultContractResolver
{
    protected override JsonConverter ResolveContractConverter(Type objectType)
    {
        if (typeof(GameUnit).IsAssignableFrom(objectType) && !objectType.IsAbstract)
            return null; // pretend TableSortRuleConvert is not specified (thus avoiding a stack overflow)
        return base.ResolveContractConverter(objectType);
    }
}

public class BaseConverter : JsonConverter
{
    static JsonSerializerSettings SpecifiedSubclassConversion = new JsonSerializerSettings() { ContractResolver = new BaseSpecifiedConcreteClassConverter() };

    public override bool CanConvert(Type objectType)
    {
        return (objectType == typeof(GameUnit));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {


        JObject jo = JObject.Load(reader);
        Debug.Log(jo["$type"].Value<string>());
        switch (jo["$type"].Value<string>())
        {
            case "GameItem":
                return JsonConvert.DeserializeObject<GameItem>(jo.ToString(), SpecifiedSubclassConversion);
            case "Player":
                return JsonConvert.DeserializeObject<Player>(jo.ToString(), SpecifiedSubclassConversion);
            case "Marshal":
                return JsonConvert.DeserializeObject<Marshal>(jo.ToString(), SpecifiedSubclassConversion);
            default:
                throw new Exception();
        }
        throw new NotImplementedException();
    }

    public override bool CanWrite
    {
        get { return false; }
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException(); // won't be called because CanWrite returns false
    }
}

public class CardSpecifiedConcreteClassConverter : DefaultContractResolver
{
    protected override JsonConverter ResolveContractConverter(Type objectType)
    {
        if (typeof(Card).IsAssignableFrom(objectType) && !objectType.IsAbstract)
            return null; // pretend TableSortRuleConvert is not specified (thus avoiding a stack overflow)
        return base.ResolveContractConverter(objectType);
    }
}

public class CardConverter : JsonConverter
{
    static JsonSerializerSettings SpecifiedSubclassConversion = new JsonSerializerSettings() { ContractResolver = new CardSpecifiedConcreteClassConverter() };

    public override bool CanConvert(Type objectType)
    {
        return (objectType == typeof(Card));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {


        JObject jo = JObject.Load(reader);
        Debug.Log(jo["$type"].Value<string>());
        switch (jo["$type"].Value<string>())
        {
            case "BulletCard":
                return JsonConvert.DeserializeObject<GameItem>(jo.ToString(), SpecifiedSubclassConversion);
            case "ActionCard":
                return JsonConvert.DeserializeObject<Player>(jo.ToString(), SpecifiedSubclassConversion);
            default:
                throw new Exception();
        }
        throw new NotImplementedException();
    }

    public override bool CanWrite
    {
        get { return false; }
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException(); // won't be called because CanWrite returns false
    }
}


//Clean up type formatting
public class KnownTypesBinder : ISerializationBinder
{
    public IList<Type> KnownTypes { get; set; }

    public Type BindToType(string assemblyName, string typeName)
    {
        return KnownTypes.SingleOrDefault(t => t.Name == typeName);
    }

    public void BindToName(Type serializedType, out string assemblyName, out string typeName)
    {
        assemblyName = null;
        typeName = serializedType.Name;
    }
}

public enum GameStatus
{
    Schemin,
    Stealin
}
