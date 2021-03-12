using System;
using System.Collections.Generic;


namespace Round {
    enum endOfRoundEvent {
        // TODO
    }


    class Round {
        private endOfRoundEvent event;
        public Stack<ActionCard> playedCards;

        private List<Turn> turns;

        public Round(endOfRoundEvent e) {
            this.event = e;
        }
    }
}
