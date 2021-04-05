using System;
using GameUnitSpace;
using Newtonsoft.Json;

namespace CardSpace {

    abstract class Card {

        public readonly Player myPlayer;

        public Player belongsTo() {
            return this.myPlayer;
        }
    }
    
}
