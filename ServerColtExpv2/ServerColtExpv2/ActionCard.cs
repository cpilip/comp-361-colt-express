using System;
using System.IO;
using GameUnitSpace;
using Newtonsoft.Json;

namespace CardSpace {
    
    public enum ActionKind {
        Move,
        ChangeFloor,
        Shoot,
        Rob,
        Marshal,
        Punch,
        Ride
    }

    class ActionCard:Card {
        [JsonProperty]
        private readonly ActionKind kind;

   
        [JsonProperty]
        private bool canBePlayed;
        
         public ActionCard(Player pPlayer, ActionKind k) : base(assignPlayer(pPlayer)) 
        {
            this.kind = k;
            canBePlayed = true;
        }

        public ActionKind getKind() {
            return this.kind;
        }

        public void cantBePlayedAnymore(){
            canBePlayed = false;
        }

        public bool isCanBePlayed(){
            return canBePlayed;
        }

        public void serialiazation(string filePath)
        {
            JsonSerializer jsonSerializer = new JsonSerializer();
            StreamWriter sw = new StreamWriter(filePath);
            JsonWriter jsonWriter = new JsonTextWriter(sw);
            var defination = new
            {
                className = "ActionCard",
                kind = kind,
                canBePlayed = canBePlayed,
                myPlayer = myPlayer

            };

            jsonSerializer.Serialize(jsonWriter, defination);
            jsonWriter.Close();
            sw.Close();
        }

        public override object deserialization<T>(string filePath)
        {
            if (File.Exists(filePath))
            {
                string txt = File.ReadAllText(filePath);
                //Console.WriteLine(txt);
                var obj = JsonConvert.DeserializeObject<T>(txt);
                return obj;
            }
            else
            {
                Console.WriteLine("Debug: file does not exist in deserialization");
                return null;
            }

        }
    }
}