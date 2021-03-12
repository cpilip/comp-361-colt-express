using System;

namespace Card {

    abstract class  Card {

        public readonly Player myPlayer;

        public Card(Player p) {
            this.myPlayer = p;
        }

        public Player belongsTo() {
            return this.myPlayer;
        }
    }
    
}
