using System;

namespace Card{
    
    public enum ActionKind {
        Move,
        ChangeFloor,
        Shoot,
        Rob,
        Marshall,
        Punch
    }

    class ActionCard: Card {
        private readonly ActionKind kind;

        public ActionCard(ActionKind k) {
            this.kind = k;
        }

        public ActionKind getKind() {
            return this.kind;
        }
    }
}