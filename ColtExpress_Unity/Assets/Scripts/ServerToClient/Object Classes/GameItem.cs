

using Newtonsoft.Json;

namespace GameUnitSpace
{
     enum ItemType
    {
        Purse,
        Strongbox,
        Ruby
    }
    
    class GameItem : GameUnit 
    {
        [JsonProperty]
        private int aValue { get; set; }
        [JsonProperty]
        private ItemType aItemType { get; set; }
        [JsonProperty]
        private Player myPlayer { get; set; }


        public GameItem(ItemType pType, int pValue)
        {
            aValue = pValue;
            aItemType = pType;
        }

        public void setType(ItemType pType)
        {
            aItemType = pType;
        }

        public void setValue(int pValue)
        {
            aValue = pValue;
        }

        public void setPlayer(Player pPlayer)
        {
            myPlayer = pPlayer;
        }



        public ItemType getType()
        {
            return aItemType;
        }

        public int getValue()
        {
            return aValue;
        }

        public Player getPlayer()
        {
            return myPlayer;
        }

    }

}


