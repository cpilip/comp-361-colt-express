using System;
using GameUnitSpace;
using Newtonsoft.Json;

namespace CardSpace {
    abstract class Card {
        [JsonProperty]
        public readonly Player myPlayer;

        protected Card(Player pPlayer)
        {
            myPlayer = pPlayer;
        }
        public Player belongsTo() {
            return this.myPlayer;
        }
        protected static Player assignPlayer(Player playerToAssign)
        {
            return playerToAssign;
        }

        public void serialiazation(string filePath) { }

        public abstract Object deserialization<T>(string filePath);
    }
    
}
