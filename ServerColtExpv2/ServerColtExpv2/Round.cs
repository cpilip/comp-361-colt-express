using System;
using System.Collections.Generic;
using CardSpace;


namespace RoundSpace {
    enum EndOfRoundEvent {
        AngryMarshal,
        SwivelArm,
        Braking,
        TakeItAll,
        PassengersRebellion,
        PantingHorses,
        WhiskeyForMarshal,
        HigherSpeed,
        MarshalsRevenge,
        Pickpocketing,
        HostageConductor,
        Escape
    }

    enum turnLayout {
        

    }

    class Round {
        private readonly EndOfRoundEvent anEvent;
        private Boolean isLastRound;
        private Queue <ActionCard> playedCards;
        private List<Turn> turns;

        public Round(Boolean isLastRound) {
            
            this.isLastRound = isLastRound;

            if(!isLastRound){
                /*
                    here, need to use a randomn number to chose between the first 8 EndOfRoundEvents 
                */
                this.anEvent = EndOfRoundEvent.AngryMarshal;
            }
            else {
                /*
                    here, need to use a randomn number to chose between the last 4 EndOfRoundEvents 
                */
                this.anEvent = EndOfRoundEvent.MarshalsRevenge;
            }

            /*
                Here, we'll have to choose between a valid game layout 
            */
            intializeTurn();

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

        public void intializeTurn(){
            
            this.turns.Add(new Turn (TurnType.Standard));
            this.turns.Add(new Turn (TurnType.Tunnel));
            this.turns.Add(new Turn (TurnType.SpeedingUp));
            this.turns.Add(new Turn (TurnType.Switching));

        }


    }
}
