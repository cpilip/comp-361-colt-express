using System;

namespace CardSpace {
    
    public enum ActionKind {
        Move,
        ChangeFloor,
        Shoot,
        Rob,
        Marshal,
        Punch
    }

    class ActionCard: Card {
        private readonly ActionKind kind;

        public ActionCard(Player p, ActionKind k) {
            this.kind = k;
        }

        public ActionKind getKind() {
            return this.kind;
        }
    }
}