using System;

namespace Card {

    abstract class  Card {

        public Player myPlayer;

        public Player belongsTo() {
            return this.myPlayer;
        }
    }
    
}
