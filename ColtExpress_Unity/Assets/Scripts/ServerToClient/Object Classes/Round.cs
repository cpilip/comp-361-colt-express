using System;
using System.Collections.Generic;
using CardSpace;
using Newtonsoft.Json;

namespace RoundSpace {
    enum EndOfRoundEvent {
        One,
        Two,
        Three
    }


    class Round {
        [JsonProperty]
        private readonly EndOfRoundEvent anEvent;
        [JsonProperty]
        private Queue <ActionCard> playedCards;
        [JsonProperty]
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

        public void addToPlayedCards(ActionCard c){
            this.playedCards.Enqueue(c);
        }

        public ActionCard topOfPlayedCards() {
            return this.playedCards.Dequeue();
        }

    }
}
