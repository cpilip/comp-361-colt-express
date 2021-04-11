using System;
using Newtonsoft.Json;

namespace RoundSpace {

    enum TurnType
    {
        Standard,
        Tunnel,
        SpeedingUp,
        Switching,
        Turmoil
    }

    class Turn{
        [JsonProperty]
        private readonly TurnType type;

        public Turn(TurnType t) {
            this.type = t;
        }

        public TurnType getType() { 
            return this.type;
        }
    }

}