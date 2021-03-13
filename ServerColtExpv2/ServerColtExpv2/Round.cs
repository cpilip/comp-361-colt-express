using System;
using System.Collections.Generic;
using CardSpace;


namespace RoundSpace {
    enum EndOfRoundEvent {
        One,
        Two,
        Three
    }


    class Round {
        private readonly EndOfRoundEvent anEvent;
        private Queue <ActionCard> playedCards;

        private List<Turn> turns;

        public Round(EndOfRoundEvent e) {
            this.anEvent = e;
        }


        /*
            Get methods
        */
        
        public List<Turn> getTurns(){
            return turns;
        }

        public Queue<ActionCard> getPlayedCards(){
            return this.playedCards;
        }

        public EndOfRoundEvent getEvent(EndOfRoundEvent e){
            return this.anEvent;
        }

    }
}
