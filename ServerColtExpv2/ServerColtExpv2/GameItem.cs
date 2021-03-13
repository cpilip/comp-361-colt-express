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

        private int aValue;
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

    }
}

