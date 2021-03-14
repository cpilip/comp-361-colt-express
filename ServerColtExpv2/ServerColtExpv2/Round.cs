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

        public Round(Boolean isLastRound, int nbOfPlayer) {
            
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
            intializeTurn(nbOfPlayer);

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

        public void intializeTurn(int nbOfPlayer){
            
            Random rnd = new Random ();
            int rand = rnd.Next(0,7);

            //if there are 5-6 players 
            if (nbOfPlayer>4){
                
                switch (rand)
                {
                    case 0 :{
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Tunnel));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Switching));
                        break;
                    }   
                    case 1 :{
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Tunnel));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Tunnel));
                        break;
                    }
                    case 2 :{
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Tunnel));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Standard));
                        break;
                    }
                    case 3 :{
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.SpeedingUp));
                        this.turns.Add(new Turn (TurnType.Switching));
                        break;
                    }
                    case 4 :{
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.SpeedingUp));
                        break;
                    }
                    case 5 :{
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Switching));
                        break;
                    }
                    case 6 :{
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Tunnel));
                        this.turns.Add(new Turn (TurnType.Standard));
                        break;
                    }
                }
            }

            //else, if there are 2-4 players
            else {
                switch (rand)
                {
                    case 0 :{
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Tunnel));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Standard));
                        break;
                    }   
                    case 1 :{
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Tunnel));
                        this.turns.Add(new Turn (TurnType.Switching));
                        break;
                    }
                    case 2 :{
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.SpeedingUp));
                        this.turns.Add(new Turn (TurnType.Standard));
                        break;
                    }
                    case 3 :{
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Tunnel));
                        this.turns.Add(new Turn (TurnType.SpeedingUp));
                        this.turns.Add(new Turn (TurnType.Standard));
                        break;
                    }
                    case 4 :{
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Standard));
                        break;
                    }
                    case 5 :{
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Tunnel));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Tunnel));
                        this.turns.Add(new Turn (TurnType.Standard));
                        break;
                    }
                    case 6 :{
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Tunnel));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Standard));
                        break;
                    }
                }

            }

        }


    }
}
