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

        public ActionCard(ActionKind k) {
            this.kind = k;
        }

        public ActionKind getKind() {
            return this.kind;
        }
    }
}