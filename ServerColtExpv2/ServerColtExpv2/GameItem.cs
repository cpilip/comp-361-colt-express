using System.IO;
using System;
using Newtonsoft.Json;

namespace GameUnitSpace
{
     enum ItemType
    {
        Purse,
        Strongbox,
        Ruby,
        Whiskey
    }
    
    class GameItem : GameUnit 
    {
        
        [JsonProperty]
        private int aValue;
        [JsonProperty]
        private ItemType aItemType;
        private Player myPlayer;

        public GameItem (ItemType pType, int pValue){
            aValue = pValue;
            aItemType = pType;
        }

        public void setType(ItemType pType){
           aItemType = pType;
        }

        public void setValue(int pValue){
            aValue = pValue;
        }

        public void setPlayer(Player pPlayer){
            myPlayer = pPlayer; 
        }



        public ItemType getType(){
            return aItemType;
        }

        public int getValue(){
            return aValue;
        }

        public Player getPlayer(){
            return myPlayer;
        }

        public void serialiazation(string filePath)
        {
            JsonSerializer jsonSerializer = new JsonSerializer();
            StreamWriter sw = new StreamWriter(filePath);
            JsonWriter jsonWriter = new JsonTextWriter(sw);
            var defination = new
            {
                className = "GameItem",
                aValue = aValue,
                aItemType = aItemType,
                myPlayer = myPlayer

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

