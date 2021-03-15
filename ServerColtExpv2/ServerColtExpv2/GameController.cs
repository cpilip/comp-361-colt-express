using System;
using System.Collections.Generic;
using System.Net;

using RoundSpace;
using CardSpace;
using GameUnitSpace;
using PositionSpace;


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
    private List<Round> rounds;
    private List<Player> players;
    private Player currentPlayer;
    private int currentPlayerIndex;
    //added this for Switching turn
    private List<TrainCar> myTrain;
    private Marshal aMarshal;

    private Dictionary<GameUnitSpace.Character, IPAddress> playerToIP = new Dictionary<GameUnitSpace.Character, IPAddress>();
    private Dictionary<IPAddress, GameUnitSpace.Character> IPtoPlayer = new Dictionary<IPAddress, GameUnitSpace.Character>();


    private GameController() {
        this.players = new List<Player>();
        this.myTrain = new List<TrainCar>();
        this.rounds = new List<Round>();
    }

    public static GameController getInstance() {
        return myGameController;
    }


    /**
        * Public utility methods
    */

    public void chosenCharacter(Character aChar) {

        //adding a new player to the list of players 
        this.players.Add(new Player(aChar));

        //if all players are here 
        if (players.Count == totalPlayer) {

            initializeGameBoard();

            //setting players' positions
            for (int i = 0; i < myTrain.Count; i++) {
                if (i % 2 == 0) {
                    myTrain[myTrain.Count - 2].moveInsideCar(players[i]);
                } else {
                    myTrain[myTrain.Count - 1].moveInsideCar(players[i]);
                }
            }
            // SEND MESSAGE to all player

            initializeLoot();

            intializeRounds();

            this.aGameStatus = GameStatus.Schemin;
            //SEND MESSAGE to all player 

            this.currentRound = rounds[0];
            //SEND MESSAGE to all player 

            this.currentTurn = currentRound.getTurns()[0];
            //SEND MESSAGE to all player

            players[0].setWaitingForInput(true);

            this.currentPlayer = players[0];
        }
    }

    public void playActionCard(ActionCard c) {

        //adding the action card to the playedCard pile and removind it from player's hand
        this.currentRound.addToPlayedCards(c);
        this.currentPlayer.hand.Remove(c);

        endOfTurn();
    }

    public void drawCards() {
        Random rnd = new Random();

        //taking three random cards from player's discardPile and adding them to the player's hand
        for (int i = 0; i < 3; i++) {
            int rand = rnd.Next(0, this.currentPlayer.discardPile.Count);
            Card c = this.currentPlayer.discardPile[rand];
            this.currentPlayer.moveFromDiscardToHand(c);
        }
        endOfTurn();
    }

    public void chosenPosition(Position p) {
        ActionCard topOfPile = this.currentRound.topOfPlayedCards();

        //if the action card is a Move Marshall action
        if (topOfPile.getKind().Equals(ActionKind.Marshal)) {
            this.aMarshal.setPosition(p);
            //SEND MESSAGE to all player

            //check for all players at position p 
            foreach (Player aPlayer in p.getPlayers()) {
                BulletCard b = new BulletCard();
                aPlayer.addToDiscardPile(b);
                p.getTrainCar().moveRoofCar(aPlayer);
                //SEND MESSAGE to all player

            }
        }
        //if the action card is a Move action
        if (topOfPile.getKind() == ActionKind.Move) {
            currentPlayer.setPosition(p);
            //SEND MESSAGE Game unit / to all player

            //if the marshal is at position p, bullet card in deck + sent to the roof 
            if (p.hasMarshal(aMarshal)) {
                BulletCard b = new BulletCard();
                currentPlayer.addToDiscardPile(b);
                p.getTrainCar().moveRoofCar(currentPlayer);
                //SEND MESSAGE to all player
            }
        }

        endOfCards();

    }

    public void chosenPunchTarget(Player victim, GameItem loot, Position dest) {

        //drop the loot at victim position, sends victim to destination 
        loot.setPosition(victim.getPosition());
        //SEND 
        victim.setPosition(dest);

        //loot is removed from victime possessions
        victim.possessions.Remove(loot);

        //if the marshal is at position dest, victim: bullet card in deck + sent to the roof 
        if (dest.hasMarshal(aMarshal)) {
            BulletCard b = new BulletCard();
            victim.discardPile.Add(b);
            dest.getTrainCar().moveRoofCar(victim);
        }

        endOfCards();
    }

    public void chosenShootTarget(Player target) {
        //A BulletCard is transfered from bullets of currentPlayer to target's discardPile
        BulletCard aBullet = currentPlayer.getABullet();
        target.addToDiscardPile(aBullet);
        this.currentPlayer.shootBullet();
        endOfCards();
    }

    public void chosenLoot(GameItem loot) {
        //the loot is transfered from the position to the currentPlayer possensions
        loot.setPosition(null);
        currentPlayer.addToPossessions(loot);
        endOfCards();
    }

    /**
        Private helper methods
    */

    private void endOfTurn() {

        //if the player has another action, then the anotherAction flag is set to false
        if (this.currentPlayer.isGetsAnotherAction()) {
            this.currentPlayer.setGetsAnotherAction(false);
            //SEND Another action Player + 
        } else {
            this.currentPlayer.setWaitingForInput(false);

            //if this is not the last turn of the round
            if (!this.currentTurn.Equals((this.currentRound.getTurns()[this.currentRound.getTurns().Count - 1]))) {

                //determining the next player 
                //if the turn is Switching, order of players is reversed
                if (this.currentTurn.getType() == TurnType.Switching) {
                    this.currentPlayerIndex = this.currentPlayerIndex - 1 % this.totalPlayer;
                    //don't use playerIndex
                    this.currentPlayer = this.players[this.players.IndexOf(this.currentPlayer) - 1];
                }
                //otherwise, it is the next player in the list 
                else {
                    this.currentPlayerIndex = this.currentPlayerIndex + 1 % this.totalPlayer;
                    this.currentPlayer = this.players[this.players.IndexOf(this.currentPlayer) + 1];
                }

                //if the turn is Speeding up, the next player has another action 
                if (this.currentTurn.getType() == TurnType.SpeedingUp) {
                    this.currentPlayer.setGetsAnotherAction(true);
                }
            }
            // if it is the last turn of the round 
            else {
                //prepare for Stealing phase 
                foreach (Player p in this.players) {

                    p.moveCardsToDiscard();
                    p.setWaitingForInput(true);
                    this.aGameStatus = GameStatus.Stealin;
                }
            }
        }
    }

    private void endOfCards() {

        this.currentPlayer.addToDiscardPile(this.currentRound.topOfPlayedCards());

        //if all cards in the pile have been played 
        if (this.currentRound.getPlayedCards().Count == 0) {

            //if this is the last round 
            if (this.currentRound.Equals(this.rounds[this.rounds.Count - 1])) {
                calculateGameScore();
            } else {
                //setting the next round, setting the first turn of the round 
                this.currentRound = this.rounds[this.rounds.IndexOf(this.currentRound) + 1];
                this.currentTurn = this.currentRound.getTurns()[0];
                //SEND index 

                //setting the next player and game status of the game 
                this.currentPlayer = this.players[this.rounds.IndexOf(currentRound)];
                this.currentPlayer.setWaitingForInput(true);
                //SEND player

                this.aGameStatus = GameStatus.Schemin;

                //for each player, getting 6 cards from their Pile at randomn and adding them to their hand 
                foreach (Player p in this.players) {
                    Random rnd = new Random();
                    for (int i = 0; i < 6; i++) {
                        int rand = rnd.Next(0, p.discardPile.Count);
                        p.hand.Add(p.discardPile[rand]);
                        p.discardPile.RemoveAt(rand);
                    }
                }
            }
        }


    }

    private Dictionary<Player, int> calculateGameScore() {

        Dictionary<Player, int> scores = new Dictionary<Player, int>();
        int max = -1;
        Player maxPlayer = null;

        foreach (Player pl in this.players) {
            scores.Add(pl, pl.getPossesionsValue());
            if (pl.getNumOfBulletsShot() > max) {
                max = pl.getNumOfBulletsShot();
                maxPlayer = pl;
            }
        }
        //TODO Sort Dictionnary for the clients 
        scores[maxPlayer] = scores[maxPlayer] + 1000;
        return scores;
    }

    private void initializeGameBoard() {

        //initializing Locomotive
        this.myTrain.Add(new TrainCar(true));

        //initializing train cars
        for (int i = 0; i < players.Count; i++) {
            this.myTrain.Add(new TrainCar(false));
        }

        // initializing the marshall and init his position to inside locomotive
        this.aMarshal = Marshal.getInstance();
        myTrain[0].moveInsideCar(aMarshal);

    }

    private void initializeLoot() {
        //initializaing a Strongbox in the locomotive 
        GameItem locomotiveStongBox = new GameItem(ItemType.Strongbox, 1000);
        locomotiveStongBox.setPosition(myTrain[0].getInside());

        // depending on the number of player, 
        // we'll initialize loots for totalPlayer number of wagon
        for (int i = 0; i < totalPlayer; i++) {

            switch (i) {
                //intializing loots in first wagon
                case 0: {
                        GameItem anItem = new GameItem(ItemType.Purse, 500);
                        anItem.setPosition(myTrain[1].getInside());
                        GameItem anItem1 = new GameItem(ItemType.Purse, 250);
                        anItem1.setPosition(myTrain[1].getInside());
                        GameItem anItem2 = new GameItem(ItemType.Purse, 250);
                        anItem2.setPosition(myTrain[1].getInside());
                        break;
                    }
                //intializing loots in second wagon
                case 1: {
                        GameItem anItem = new GameItem(ItemType.Ruby, 500);
                        anItem.setPosition(myTrain[2].getInside());
                        GameItem anItem1 = new GameItem(ItemType.Ruby, 500);
                        anItem1.setPosition(myTrain[2].getInside());
                        GameItem anItem2 = new GameItem(ItemType.Ruby, 500);
                        anItem2.setPosition(myTrain[2].getInside());
                        break;
                    }
                //intializing loots in third wagon
                case 2: {
                        GameItem anItem = new GameItem(ItemType.Ruby, 500);
                        anItem.setPosition(myTrain[3].getInside());
                        GameItem anItem1 = new GameItem(ItemType.Purse, 500);
                        anItem1.setPosition(myTrain[3].getInside());
                        break;
                    }
                //intializing loots in 4th wagon
                case 3: {
                        GameItem anItem = new GameItem(ItemType.Ruby, 500);
                        anItem.setPosition(myTrain[4].getInside());
                        GameItem anItem1 = new GameItem(ItemType.Purse, 500);
                        anItem1.setPosition(myTrain[4].getInside());
                        GameItem anItem2 = new GameItem(ItemType.Purse, 250);
                        anItem1.setPosition(myTrain[4].getInside());
                        GameItem anItem3 = new GameItem(ItemType.Purse, 250);
                        anItem1.setPosition(myTrain[4].getInside());
                        break;
                    }
                //intializing loots in 5th wagon
                case 4: {
                        GameItem anItem = new GameItem(ItemType.Ruby, 500);
                        anItem.setPosition(myTrain[5].getInside());
                        GameItem anItem1 = new GameItem(ItemType.Purse, 500);
                        anItem1.setPosition(myTrain[5].getInside());
                        GameItem anItem2 = new GameItem(ItemType.Purse, 250);
                        anItem1.setPosition(myTrain[5].getInside());
                        GameItem anItem3 = new GameItem(ItemType.Purse, 250);
                        anItem1.setPosition(myTrain[5].getInside());
                        GameItem anItem4 = new GameItem(ItemType.Purse, 250);
                        anItem1.setPosition(myTrain[5].getInside());
                        break;
                    }
                //intializing loots in 6th wagon
                case 5: {
                        GameItem anItem = new GameItem(ItemType.Purse, 500);
                        anItem.setPosition(myTrain[6].getInside());
                        break;
                    }
            }
        }
    }

    private void intializeRounds() {
        for (int i = 0; i < 4; i++) {
            Round aRound = new Round(false, totalPlayer);
            this.rounds.Add(aRound);
        }
        Round aFinalRound = new Round(true, totalPlayer);
        this.rounds.Add(aFinalRound);
    }

    public void readyForNextMove() {
        this.currentPlayer.setWaitingForInput(false);
        Boolean waiting = true;

        foreach (Player p in this.players) {
            if (p.getWaitingForInput())
                waiting = false;
        }

        if (waiting) {
            // Get the top of the played cards from the schemin phase
            ActionCard top = this.currentRound.topOfPlayedCards();

            switch (top.getKind()) {
                case ActionKind.Move: {
                        List<Position> moves = this.getPossibleMoves(this.currentPlayer);

                        // SENDMESSAGE with moves
                        if (moves.Count > 1) {
                            this.aGameStatus = GameStatus.FinalizingCard;
                        } else {
                            chosenPosition(moves[0]);
                        }
                        break;
                    }
                case ActionKind.ChangeFloor: {
                        if (this.currentPlayer.getPosition().isInside()) {
                            this.currentPlayer.getPosition().getTrainCar().moveRoofCar(this.currentPlayer);
                        } else {
                            this.currentPlayer.getPosition().getTrainCar().moveInsideCar(this.currentPlayer);

                            if (this.currentPlayer.getPosition().hasMarshal(this.aMarshal)) {
                                this.currentPlayer.addToDiscardPile(new BulletCard());
                                this.currentPlayer.getPosition().getTrainCar().moveRoofCar(this.currentPlayer);
                            }
                        }
                        break;
                    }
                case ActionKind.Shoot: {
                        List<Player> possTargets = this.getPossibleShootTarget(this.currentPlayer);
                        if (possTargets.Count == 1) {
                            this.chosenShootTarget(possTargets[0]);
                        } else {
                            // SENDMESSAGE with possTargets
                            this.aGameStatus = GameStatus.FinalizingCard;
                            this.currentPlayer.setWaitingForInput(true);
                        }
                        break;
                    }
                case ActionKind.Rob: {
                        List<GameItem> atLocation = this.currentPlayer.getPosition().getItems();
                        // Send message with atLocation
                        this.aGameStatus = GameStatus.FinalizingCard;
                        this.currentPlayer.setWaitingForInput(true);
                        break;
                    }
                case ActionKind.Marshal: {
                        List<Position> possPosition = this.aMarshal.getPossiblePositions();
                        if (possPosition.Count == 1) {
                            this.chosenPosition(possPosition[0]);
                        } else {
                            // SENDMESSAGE with possPosition
                            this.aGameStatus = GameStatus.FinalizingCard;
                            this.currentPlayer.setWaitingForInput(true);
                        }
                        break;
                    }
                case ActionKind.Punch: {
                        List<Player> atLocation = this.currentPlayer.getPosition().getPlayers();
                        // Send message with atLocation
                        this.aGameStatus = GameStatus.FinalizingCard;
                        this.currentPlayer.setWaitingForInput(true);
                        break;
                    }
            }
            this.endOfCards();
        }
    }

    // Get a list of all possible wagons where a player p can move from its current position
    private List<Position> getPossibleMoves(Player p) {
        List<Position> possPos = new List<Position>();
        TrainCar playerCar = p.getPosition().getTrainCar();
        // Check if on a roof or not
        if (!p.getPosition().isInside()) {
            // Add 1-3 distance forward or backwards
            for (int i = 1; i < 4; i++) {
                try {
                    // Add adjacent positions
                    possPos.Add(this.myTrain[this.myTrain.IndexOf(playerCar) - i].getRoof());
                } catch (System.IndexOutOfRangeException e) {
                    continue;
                }
            }

            for (int i = 1; i < 4; i++) {
                try {
                    // Add adjacent positions
                    possPos.Add(this.myTrain[this.myTrain.IndexOf(playerCar) + i].getRoof());
                } catch (System.IndexOutOfRangeException e) {
                    continue;
                }
            }
        } else {
            try {
                // Add adjacent positions
                possPos.Add(this.myTrain[this.myTrain.IndexOf(playerCar) - 1].getInside());
            } catch (System.IndexOutOfRangeException e) {

            }

            try {
                // Add adjacent positions
                possPos.Add(this.myTrain[this.myTrain.IndexOf(playerCar) + 1].getInside());
            } catch (System.IndexOutOfRangeException e) {

            }

        }
        return possPos;
    }

    private List<Player> getPossibleShootTarget(Player p) {
        List<Player> possPlayers = new List<Player>();
        TrainCar playerCar = p.getPosition().getTrainCar();

        if (!p.getPosition().isInside()) {
            // Look for the players in line of sight forward on roof
            for (int i = this.myTrain.IndexOf(playerCar) + 1; i < myTrain.Count; i++) {
                List<Player> playersOnWagon = this.myTrain[i].getInside().getPlayers();
                if (playersOnWagon.Count != 0) {
                    possPlayers.AddRange(playersOnWagon);
                    break;
                }
            }

            // Look for the players in line of sight backwards on roof
            for (int i = this.myTrain.IndexOf(playerCar) - 1; i >= 0; i--) {
                List<Player> playersOnWagon = this.myTrain[i].getInside().getPlayers();
                if (playersOnWagon.Count != 0) {
                    possPlayers.AddRange(playersOnWagon);
                    break;
                }
            }
        } else {
            // Look for the players in the next wagon backwards
            try {
                // Add adjacent positions
                possPlayers.AddRange(this.myTrain[this.myTrain.IndexOf(playerCar) - 1].getInside().getPlayers());
            } catch (System.IndexOutOfRangeException e) {

            }

            // Loof for the players in the next wagon forward
            try {
                // Add adjacent positions
                possPlayers.AddRange(this.myTrain[this.myTrain.IndexOf(playerCar) + 1].getInside().getPlayers());
            } catch (System.IndexOutOfRangeException e) {

            }
        }

        // If there is more than one possible player, we remove Belle.
        if (possPlayers.Count > 1) {
            foreach (Player pl in possPlayers) {
                if (pl.getBandit() == Character.Belle) {
                    possPlayers.Remove(pl);
                }
            }
        }

        return possPlayers;

    }


}

