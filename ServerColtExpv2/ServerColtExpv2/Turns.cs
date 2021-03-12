using System;

namespace Round {

    enum TurnType {
        Standard,
        Tunnel,
        SpeedingUp,
        Switching
    }
    
    class Turn{
        private readonly TurnType type;

        public Turn(TurnType t) {
            this.type = t;
        }
    }
}