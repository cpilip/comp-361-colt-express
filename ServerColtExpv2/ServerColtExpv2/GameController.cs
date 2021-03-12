using System;
using System.Collections.Generic;

enum GameStatus {
    ChoosingBandits,
    Schemin,
    Stealin,
    FinalizingCard,
    Completed
}

class GameController {
    
    private static GameController myGameController = new GameController();
    private readonly int totalPlayer;
    private GameStatus aGameStatus;
    private Round currentRound;
    private Turn currentTurn;
    private List <Round> rounds;
    private List <Player> players; 
    private Player currentPlayer;
    private int currentPlayerIndex;
    //added this for Switching turn
    private List <TrainCar> myTrain;
    private Marshal aMarshal;

    private GameController(){
        this.players = new List <Player> ();
        this.myTrain = new List <TrainCar> ();
        this.rounds = new List <Round> ();
    }

    public GameController getInstance(){
        return this;
    }


    public void chosenCharacter(Character aChar){
        
        this.players.Add(new Player(aChar));
        
        if (players.Count == totalPlayer){
            
            initializeGameBoard();
            
            for(int i=0; i<myTrain.Count; i++){
                
                if (i%2==0){
                    myTrain[myTrain.Count - 2].moveInsideCar(players[i]);
                }
                else {
                    myTrain[myTrain.Count - 1].moveInsideCar(players[i]);
                }

            }
            
            initializeLoot();

            intializeRounds();

            this.aGameStatus = GameStatus.Schemin;

            this.currentRound =rounds[0];

            //get method for turns
            this.currentTurn = currentRound.turns[0];

            players[0].setWaitingForInput(true);

            //this.currentPlayer = players[0];
        }
    }

    private void initializeGameBoard(){
        
        //initializing Locomotive
        this.myTrain.Add(new TrainCar(true));

        //initializing train cars
        for (int i=0; i<myTrain.Count; i++){
            this.myTrain.Add(new TrainCar(false));
        }

        // initializing the marshall and init his position
        this.aMarshal = Marshal.getInstance();
        myTrain[0].moveInsideCar(aMarshal);

    }

    private void initializeLoot(){
        //TO DO
    }
    
    private void intializeRounds(){
        //TO DO
    }

    private void endOfTurn(){

        if (this.currentPlayer.isGetsAnotherAction()){

            this.currentPlayer.setsGetAnotherAction(false);
        }
        else {

            this.currentPlayer.setWaitingForInput(false);

                //not sure if this correct, why -1 ?
            if (!this.currentTurn.equals(this.currentRound.turns[-1])){
                
                if (this.currentTurn.getType().equals(TurnType.Switching)){

                    this.currentPlayerIndex = this.currentPlayerIndex - 1 % this.totalPlayer;
                    this.currentPlayer = this.players[this.currentPlayerIndex];
                }
                else {
                    this.currentPlayerIndex = this.currentPlayerIndex + 1 % this.totalPlayer;
                    this.currentPlayer = this.players[this.currentPlayerIndex];
                }

                if (this.currentTurn.getType().equals(TurnType.SpeedingUp)){
                    this.currentPlayer.setAnotherAction(true);
                }
            }
            else {
                
                foreach (Player p in this.players){
                    
                    p.moveCardsToDiscard();
                    p.setWaitingForInput(true);
                    this.aGameStatus = GameStatus.Stealin;
                }
            }
        }
    }
    
    private void endOfCards(){
        
        this.currentPlayer.discardPile.Add(this.currentRound.playedCards.pop());

        if(this.currentRound.playedCards.isEmpty()){

            if(this.currentRound.equals(this.rounds[-1])){
                calculateGameScore();
            }
            else {
                this.currentRound = this.rounds[this.rounds.IndexOf(this.currentRound + 1)];
                this.currentTurn = this.currentRound.turns[0];
                //why?
                this.currentPlayer = this.players[this.rounds.IndexOf(currentRound)];
                this.currentPlayer.setWaitingForInput(true);
                this.aGameStatus = GameStatus.Schemin;

                foreach (Player p in this.players){
                    Random rnd = new Random();
                    for (int i=0; i<6; i++){
                        //TO CHECK
                        int rand = rnd.Next(0, this.p.discardPile.Count);
                        this.p.hand.Add(this.p.discardPile[rand]);
                        this.p.discardPile.Remove(rand);
                    }
                }
            }
        }


    }

    public void playActionCard(ActionCard c){
        //TO DO
    }

    public void drawCards(){
        //TO DO
    }

    public void chosenPosition(Player answeringBandit, Position p){
        //TO DO
    }

    public void chosenPunchTarget(Player p, GameItem loot, Position dest){
        //TO DO
    }

    public void chosenShootTarget(Player target ){
        //TO DO
    }

    public void chosenLoot(Player answeringBandit, GameItem loot){
        //TO DO
    }

    // Milo TODO
    public void readyForNextMove(){
        this,currentPlayer.setWaitingForInput(false);
        boolean waiting = true;

        for (Player p in this.players) {
            if (p.getWaitingForInput()) {
                waiting = false;
            }
        }

        if (waiting) {
            // Get the top of the played cards from the schemin phase
            card top = this.currentRound.PlayedCards.Pop();

            switch(top.getType()) {
                case ActionKind.Move : 
                {
                    List<Position> moves = this.getPossibleMoves(this.currentPlayer);

                    // SENDMESSAGE with moves
                    if (moves.Count > 1) {
                        this.GameStatus= FinalizingCard;
                    } else {
                        this.chosenPosition(this.currentPlayer, moves[0]);
                    }
                    break;
                }
                case ActionKind.ChangeFloor :
                {
                    if (this.currentPlayer.position.isInside()) {
                        this.currentPlayer.position.getTrainCar().moveRoofCar(this.currentPlayer);
                    } else {
                        this.currentPlayer.position.getTrainCar().moveInsideCar(this.currentPlayer);
                        
                        if (this.currentPlayer.position.hasMarshal(this.marshal)){
                            this.currentPlayer.addToDiscardPile(new BulletCard());
                            this.currentPlayer.position.getTrainCar().moveRoofCar(this.currentPlayer);
                        }
                    }
                    break;
                }
                case ActionKind.Shoot :
                {
                    List<Player> possTargets = this.getPossibleShootTarget(this.currentPlayer);
                    if (possTargets.Count == 1) {
                        this.chosenShootTarget(possTargets[0]);
                    } else {
                        // SENDMESSAGE with possTargets
                        this.GameStatus = FinalizingCard;
                        this.currentPlayer.SetWaitingForInput(true);
                    }
                    break;
                }
                case ActionKind.Rob :
                {
                    List<Player> atLocation = this.currentPlayer.position.getItems();
                    // Send message with atLocation
                    this.GameStatus = FinalizingCard;
                    this.currentPlayer.SetWaitingForInput(true);
                    break;
                }
                case ActionKind.Marshal :
                {
                    List<Position> possPosition = this.marshal.getPossiblePositions();
                    if (possPosition.Count == 1) {
                        this.chosenPosition(null, possPosition[0]);
                    } else {
                        // SENDMESSAGE with possPosition
                        this.GameStatus = FinalizingCard;
                        this.currentPlayer.SetWaitingForInput(true);
                    }
                    break;
                }
                case ActionKind.Punch :
                {
                    List<Player> atLocation = this.currentPlayer.position.getPlayers();
                    // Send message with atLocation
                    this.GameStatus = FinalizingCard;
                    this.currentPlayer.SetWaitingForInput(true);
                    break;
                }
                default : {
                    this.endOfCards();
                }  
            }
        }
    }
 
    private Player getBandits(){
        //TO DO
    }

    // Get a list of all possible wagons where a player p can move from its current position
    private ArrayList<Position> getPossibleMoves(Player p) {
        ArrayList<Position> possPos = new ArrayList<Position>();
        trainCar playerCar = p.position.getTrainCar();
        // Check if on a roof or not
        if (p.position.floor == Roof) { 
            // Add 1-3 distance forward or backwards
            for (int i = 1 ; i < 4 ; i++) {
                try {
                    // Add adjacent positions
                    possPos.add(this.train[train.IndexOf(playerCar) - i].getRoof());
                } catch (System.IndexOutOfRangeException e) {
                    continue;
                }
            }

            for (int i = 1 ; i < 4 ; i++) {
                try {
                    // Add adjacent positions
                    possPos.add(this.train[train.IndexOf(playerCar) + i].getRoof());
                } catch (System.IndexOutOfRangeException e) {
                    continue;
                }
            }
        } else { 
            try {
                // Add adjacent positions
                possPos.add(this.train[train.IndexOf(playerCar) - 1].getInside());
            } catch (System.IndexOutOfRangeException e) {
                continue;
            }
            try {
                // Add adjacent positions
                possPos.add(this.train[train.IndexOf(playerCar) + 1].getInside());
            } catch (System.IndexOutOfRangeException e) {
                continue;
            }
            
        }
        return possPos;
    }

    
    private ArrayList<Player> getPossibleShootTarget(Player p) {
        ArrayList<Player> possPlayers = new ArrayList<Player>();
        trainCar playerCar = p.position.getTrainCar();

        if (p.position.floor == roof) {
            // Look for the players in line of sight forward on roof
            for (int i = train.IndexOf(playerCar) + 1; i < train.Count ; i++) {
                playersOnWagon = this.train[i].getInside().getPlayers()
                if (playersOnWagon.Count != 0) {
                    possPlayers.addRange(playersOnWagon);
                    break;
                }
            }

            // Look for the players in line of sight backwards on roof
            for (int i = train.IndexOf(playerCar) - 1; i >= 0 ; i--) {
                playersOnWagon = this.train[i].getInside().getPlayers()
                if (playersOnWagon.Count != 0) {
                    possPlayers.addRange(playersOnWagon);
                    break;
                }
            }
        } else {
            // Look for the players in the next wagon backwards
            try {
                // Add adjacent positions
                possPlayers.addRange(this.train[train.IndexOf(playerCar) - 1].getInside().getPlayers());
            } catch (System.IndexOutOfRangeException e) {
                continue;
            }

            // Loof for the players in the next wagon forward
            try {
                // Add adjacent positions
                possPos.add(this.train[train.IndexOf(playerCar) + 1].getInside().getPlayers());
            } catch (System.IndexOutOfRangeException e) {
                continue;
            }
        }

        // If there is more than one possible player, we remove Belle.
        if (possPlayers.Count > 1) {
            for (Player p in possPlayers) { 
                if (p.getBandit() == Belle) {
                    possPlayers.Remove(p);
                }
        }
        }
        
    }

    // need calculateGameScore() method 

}
