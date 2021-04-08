using System;
using GameUnitSpace;
using Newtonsoft.Json;

namespace CardSpace {

    abstract class Card {

        public readonly Player myPlayer;

        protected Card(Player pPlayer)
        {
            myPlayer = pPlayer;
        }
        public Player belongsTo()
        {
            return this.myPlayer;
        }
        protected static Player assignPlayer(Player playerToAssign)
        {
            return playerToAssign;
        }
    }
    
}
