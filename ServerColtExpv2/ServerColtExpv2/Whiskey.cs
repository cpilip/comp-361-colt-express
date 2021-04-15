using System;
using System.IO;
using Newtonsoft.Json;

namespace GameUnitSpace {

    public enum WhiskeyKind{
        Unknown,
        Old,
        Normal
    }

    public enum WhiskeyStatus{
        Full,
        Half,
        Empty
    }
    class Whiskey : GameItem {
        [JsonProperty]
        private readonly WhiskeyKind aKind;
        [JsonProperty]
        private WhiskeyStatus aStatus;

        public Whiskey (WhiskeyKind pKind) : base (ItemType.Whiskey, 0){
            aKind = pKind;
            aStatus = WhiskeyStatus.Full;
        }

        public WhiskeyKind getWhiskeyKind(){
            return aKind;
        }

        public WhiskeyStatus getWhiskeyStatus(){
            return aStatus;
        }

        public void drinkASip(){
            if(aStatus == WhiskeyStatus.Full){
                aStatus = WhiskeyStatus.Half;
            }
            else if (aStatus == WhiskeyStatus.Half){
                aStatus = WhiskeyStatus.Empty;
            }
        }

        public bool isEmpty(){
            if(this.aStatus.Equals(WhiskeyStatus.Empty)){
                return true;
            }
            return false;
        }

        public void serialiazation(string filePath)
        {
            JsonSerializer jsonSerializer = new JsonSerializer();
            if (File.Exists(filePath)) File.Delete(filePath);
            StreamWriter sw = new StreamWriter(filePath);
            JsonWriter jsonWriter = new JsonTextWriter(sw);
            var defination = new
            {
                className = "Whiskey",
                aKind = aKind,
                aStatus = aStatus

            };

            jsonSerializer.Serialize(jsonWriter, defination);
            jsonWriter.Close();
            sw.Close();
        }

        public Object deserialization<T>(string filePath)
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