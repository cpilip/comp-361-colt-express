using System;
using System.Collections.Generic;
using CardSpace;
using Newtonsoft.Json;

namespace RoundSpace {
    enum EndOfRoundEvent {
        
        //Normal End of Round Event
        AngryMarshal,
        SwivelArm,
        Braking,
        TakeItAll,
        PassengersRebellion,
        PantingHorses,

        //Arrival End of Round Event
        MarshalsRevenge,
        Pickpocketing,
        HostageConductor

        //Extension End of Round Event 

        //Final Round Event,
        //WhiskeyForMarshal,
        //HigherSpeed,
        //Escape
        
    }

    enum turnLayout {
        

    }

    class Round {
        [JsonProperty]
        private EndOfRoundEvent anEvent;
        [JsonProperty]
        private Boolean isLastRound;
        [JsonProperty]
        private Queue <ActionCard> playedCards;
        [JsonProperty]
        private List<Turn> turns;

        public Round(Boolean isLastRound, int nbOfPlayer) {
            
            this.isLastRound = isLastRound;

            this.turns = new List<Turn>();

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

        public ActionCard getTopOfPlayedCards() {
            return this.playedCards.Dequeue();
        }
        public ActionCard seeTopOfPlayedCards() {
            return this.playedCards.Peek();
        }

        public void intializeTurn(int nbOfPlayer){

            this.turns.Add(new Turn (TurnType.Standard));
            this.turns.Add(new Turn (TurnType.Tunnel));
            this.turns.Add(new Turn (TurnType.SpeedingUp));
            this.turns.Add(new Turn (TurnType.Switching));

            
            Random rnd = new Random ();
            int rand = rnd.Next(0,7);

            /*
            //if it is a last round
            if (isLastRound){
                switch (rand)
                {
                    case 0 :{
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Tunnel));
                        this.anEvent = EndOfRoundEvent.HostageConductor;
                        break;
                    }   
                    case 1 :{
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Tunnel));
                        this.anEvent = EndOfRoundEvent.Pickpocketing;
                        break;
                    }
                    case 2 :{
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Tunnel));
                        this.anEvent = EndOfRoundEvent.MarshalsRevenge;
                        break;
                    }
                }
            }
            //else, if there are 5-6 players 
            else if (nbOfPlayer>4){
                
                switch (rand)
                {
                    case 0 :{
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Tunnel));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Switching));
                        this.anEvent = EndOfRoundEvent.PassengersRebellion;
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
                        this.anEvent = EndOfRoundEvent.Braking;
                        break;
                    }
                    case 3 :{
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.SpeedingUp));
                        this.turns.Add(new Turn (TurnType.Switching));
                        this.anEvent = EndOfRoundEvent.TakeItAll;
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
                        this.anEvent = EndOfRoundEvent.AngryMarshal;
                        break;
                    }
                    case 6 :{
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Tunnel));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.anEvent = EndOfRoundEvent.SwivelArm;
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
                        this.anEvent = EndOfRoundEvent.SwivelArm;
                        break;
                    }   
                    case 1 :{
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Tunnel));
                        this.turns.Add(new Turn (TurnType.Switching));
                        this.anEvent = EndOfRoundEvent.AngryMarshal;
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
                        this.anEvent = EndOfRoundEvent.TakeItAll;
                        break;
                    }
                    case 4 :{
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.anEvent = EndOfRoundEvent.Braking;
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
                        this.anEvent = EndOfRoundEvent.PassengersRebellion;
                        break;
                    }
                }

            }*/

        }

        public Boolean getIsLastRound(){
            return this.isLastRound;
        }
    }
}
