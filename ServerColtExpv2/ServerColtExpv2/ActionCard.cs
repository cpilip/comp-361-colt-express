using System;
using GameUnitSpace;
using Newtonsoft.Json;

namespace CardSpace {
    
    public enum ActionKind {
        Move,
        ChangeFloor,
        Shoot,
        Rob,
        Marshal,
        Punch,
        Ride
    }

    class ActionCard:Card {
        [JsonProperty]
        private readonly ActionKind kind;

        [JsonProperty]
        private bool canBePlayed;

        public ActionCard(ActionKind k) {
            this.kind = k;
            canBePlayed = true;
        }

        public ActionKind getKind() {
            return this.kind;
        }

        public void cantBePlayedAnymore(){
            canBePlayed = false;
        }

        public bool isCanBePlayed(){
            return canBePlayed;
        }
    }
}