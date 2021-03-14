using System;
using GameUnitSpace;

namespace CardSpace {

    abstract class  Card {

        public readonly Player myPlayer;

        public Player belongsTo() {
            return this.myPlayer;
        }
    }
    
}
