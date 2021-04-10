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
        WhiskeyForMarshal,
        HigherSpeed,
        ShotgunRage,

        //Arrival End of Round Event
        MarshalsRevenge,
        Pickpocketing,
        HostageConductor,
        SharingTheLoot,
        Escape, 
        MortalBullet,

        //NULL
        Null
        
    }

    class Round {
        [JsonProperty]
        private EndOfRoundEvent anEvent;
        [JsonProperty]
        private Boolean isLastRound;
        [JsonProperty]
        private Queue <ActionCard> playedCards = new Queue<ActionCard>();
        [JsonProperty]
        private List<Turn> turns;

        public Round(Boolean isLastRound, int nbOfPlayer) {
            this.isLastRound = isLastRound;
            this.turns = new List<Turn>();
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

        public EndOfRoundEvent getEvent(){
            
            return this.anEvent;
        }

        public void addToPlayedCards(ActionCard c){
            this.playedCards.Enqueue(c);
        }

        public ActionCard getTopOfPlayedCards() {
            return (this.playedCards.Count != 0) ? this.playedCards.Dequeue() : null;
        }
        public ActionCard seeTopOfPlayedCards() {
            return this.playedCards.Peek();
        }

        // Initialize the turns for a round from one of the preset layouts
        // Returns a unique ID to designate the layout that was chosen to avoid repeating layouts
        public int intializeTurn(int nbOfPlayer, int rand){

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
                    case 3 :{
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Tunnel));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.anEvent = EndOfRoundEvent.Escape;
                        break;
                    }
                    case 4 :{
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Tunnel));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.anEvent = EndOfRoundEvent.SharingTheLoot;
                        break;
                    }
                    case 5 :{
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Tunnel));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.anEvent = EndOfRoundEvent.MortalBullet;
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
                        return 0;
                    }   
                    case 1 :{
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Tunnel));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Tunnel));
                        return 1;
                    }
                    case 2 :{
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Tunnel));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.anEvent = EndOfRoundEvent.Braking;
                        return 2;
                    }
                    case 3 :{
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.SpeedingUp));
                        this.turns.Add(new Turn (TurnType.Switching));
                        this.anEvent = EndOfRoundEvent.TakeItAll;
                        return 3;
                    }
                    case 4 :{
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.SpeedingUp));
                        this.anEvent = EndOfRoundEvent.Null;
                            return 4;
                    }
                    case 5 :{
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Switching));
                        this.anEvent = EndOfRoundEvent.AngryMarshal;
                        return 5;
                    }
                    case 6 :{
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Tunnel));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.anEvent = EndOfRoundEvent.SwivelArm;
                        return 6;
                    }
                    case 7 :{
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Tunnel));
                        this.turns.Add(new Turn (TurnType.Turmoil));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.anEvent = EndOfRoundEvent.ShotgunRage;
                        return 7;
                    }
                    case 8 :{
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Tunnel));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.anEvent = EndOfRoundEvent.HigherSpeed;
                        return 8;
                    }
                    case 9 :{
                        this.turns.Add(new Turn (TurnType.Turmoil));
                        this.turns.Add(new Turn (TurnType.Tunnel));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.anEvent = EndOfRoundEvent.Null;
                        return 9;
                    }
                    case 10 :{
                        this.turns.Add(new Turn (TurnType.Turmoil));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.SpeedingUp));
                        this.anEvent = EndOfRoundEvent.PantingHorses;
                        return 10;
                    }
                    case 11 :{
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Tunnel));
                        this.turns.Add(new Turn (TurnType.Tunnel));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.anEvent = EndOfRoundEvent.WhiskeyForMarshal;
                        return 11;
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
                        return 0;
                    }   
                    case 1 :{
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Tunnel));
                        this.turns.Add(new Turn (TurnType.Switching));
                        this.anEvent = EndOfRoundEvent.AngryMarshal;
                        return 1;
                    }
                    case 2 :{
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.SpeedingUp));
                        this.turns.Add(new Turn (TurnType.Standard));
                        return 2;
                    }
                    case 3 :{
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Tunnel));
                        this.turns.Add(new Turn (TurnType.SpeedingUp));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.anEvent = EndOfRoundEvent.TakeItAll;
                        return 3;
                    }
                    case 4 :{
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.anEvent = EndOfRoundEvent.Braking;
                        return 4;
                    }
                    case 5 :{
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Tunnel));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Tunnel));
                        this.turns.Add(new Turn (TurnType.Standard));
                        return 5;
                    }
                    case 6 :{
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Tunnel));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.anEvent = EndOfRoundEvent.PassengersRebellion;
                        return 6;
                    }
                    case 7 :{
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Tunnel));
                        this.turns.Add(new Turn (TurnType.Turmoil));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.anEvent = EndOfRoundEvent.ShotgunRage;
                        return 7;
                    }
                    case 8 :{
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Tunnel));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.anEvent = EndOfRoundEvent.TakeItAll;
                        return 8;
                    }
                    case 9 :{
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Tunnel));
                        this.turns.Add(new Turn (TurnType.Tunnel));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.anEvent = EndOfRoundEvent.WhiskeyForMarshal;
                        return 9;
                    }
                    case 10 :{
                        this.turns.Add(new Turn (TurnType.Turmoil));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.SpeedingUp));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.anEvent = EndOfRoundEvent.PantingHorses;
                        return 10;
                    }
                    case 11 :{
                        this.turns.Add(new Turn (TurnType.Turmoil));
                        this.turns.Add(new Turn (TurnType.Tunnel));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.turns.Add(new Turn (TurnType.Standard));
                        this.anEvent = EndOfRoundEvent.Null;
                            return 11;
                    }
                }

            }
            return -1;
        }

        public Boolean getIsLastRound(){
            return this.isLastRound;
        }
    }
}
