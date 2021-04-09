using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.Net;
using System.Net.Sockets;

using RoundSpace;
using CardSpace;
using GameUnitSpace;
using PositionSpace;
using HostageSpace;
using System.Threading;

enum GameStatus
{
    ChoosingBandits,
    Schemin,
    Stealin,
    FinalizingCard,
    Completed
}

class GameController
{
    private static GameController myGameController = new GameController();
    private readonly int totalPlayer;
    private Boolean endOfGame;
    private GameStatus aGameStatus;
    private Round currentRound;
    private Turn currentTurn;
    private List<Round> rounds;
    private List<Player> players;
    private Player currentPlayer;
    private Player playerPtrForStarting;
    private int currentPlayerIndex;
    private int firstPlayerIndex;
    private List<TrainCar> myTrain;
    private StageCoach myStageCoach;
    private Marshal aMarshal;
    private Shotgun aShotGun;
    private List<Hostage> availableHostages;

    private GameController()
    {
        this.players = new List<Player>();
        this.myTrain = new List<TrainCar>();
        this.rounds = new List<Round>();
        this.availableHostages = new List<Hostage>();
        totalPlayer = 3;
        this.endOfGame = false;
    }

    public static GameController getInstance()
    {
        return myGameController;
    }



    /**
        * Public utility methods
    */

    public void chosenCharacter(Character aChar)
    {

        //adding a new player to the list of players 
        Player tmp = new Player(aChar);
        this.players.Add(tmp);

        MyTcpListener.addPlayerWithClient(tmp);

        Console.WriteLine("A player picked a character.");

        //if all players are here (HARD-CODED, usually is players.Count == totalPlayers )
        if (players.Count == 3)
        {
            initializeGameBoard();

            //Send all Player objects
            CommunicationAPI.sendMessageToClient(null, "updatePlayers", players);

            initializeLoot();

            //initialize the hostages 
            availableHostages = Hostage.getSomeHostages(totalPlayer);

            //Send message to all cient about the available hostages for this game
            CommunicationAPI.sendMessageToClient(null, "availableHostages", availableHostages);

            //TO ALL PLAYERS
            CommunicationAPI.sendMessageToClient(null, "updateTrain", myTrain);

            intializeRounds();

            this.aGameStatus = GameStatus.Schemin;
            //TO ALL PLAYERS
            CommunicationAPI.sendMessageToClient(null, "updateGameStatus", this.aGameStatus);

            this.currentRound = rounds[0];
            //TO ALL PLAYERS
            CommunicationAPI.sendMessageToClient(null, "updateCurrentRound", currentRound);

            this.currentTurn = currentRound.getTurns()[0];
            //TO ALL PLAYERS
            CommunicationAPI.sendMessageToClient(null, "updateCurrentTurn", this.currentRound.getTurns().IndexOf(currentTurn));

            players[0].setWaitingForInput(true);

            this.currentPlayer = players[0];

            //TO ALL PLAYERS
            //Send the current player to all clients and value for waiting for input for that player
            CommunicationAPI.sendMessageToClient(null, "updateCurrentPlayer", this.currentPlayer.getBandit());
            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateWaitingForInput", currentPlayer.getBandit(), currentPlayer.getWaitingForInput());

            //for each player, getting 6 cards from their Pile at randomn and adding them to their hand 
            foreach (Player p in this.players)
            {
                List<Card> cardsToAdd = new List<Card>();
                int index = this.players.IndexOf(p);

                Random rnd = new Random();
                for (int i = 0; i < 6; i++)
                {
                    int rand = rnd.Next(0, p.discardPile.Count);
                    Card aCard = p.discardPile[rand];
                    p.hand.Add(aCard);
                    cardsToAdd.Add(aCard);
                    p.discardPile.Remove(aCard);
                }
                //TODO NEED TO SEE WITH CRISTINA
                //TO SPECIFIC PLAYER
                CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(p), "updatePlayerHand", p.getBandit(), cardsToAdd);
            }

            //Horse attack:
            for (int i = 0; i < players.Count; i++)
            {
                playerPtrForStarting = players[i];
                playerPtrForStarting.setWaitingForInput(true);
                CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(playerPtrForStarting), "chooseStartingPosition");

                //Wait for the player to answer, chosenStartingPos will be called and then move to next player
            }

            //intializing the first player 
            this.currentPlayer = players[0];
            this.currentPlayerIndex = 0;
            this.firstPlayerIndex = 0;

            CommunicationAPI.sendMessageToClient(null, "updateFirstPlayer", this.currentPlayer.getBandit());

            currentPlayer.setWaitingForInput(true);
            //TO ALL PLAYERS
            //Send the current player as index and value for waiting for input for that index/player
            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateWaitingForInput", currentPlayer.getBandit(), currentPlayer.getWaitingForInput());

            Console.WriteLine("Finished initialization.");
        }
    }

    public Player getCurrentPlayer()
    {
        return this.currentPlayer;
    }

    public Player getPlayerByCharacter(Character aChar)
    {
        foreach (Player p in players)
        {
            if (p.getBandit().Equals(aChar))
            {
                return p;
            }
        }
        return null;
    }

    public void playActionCard(ActionCard c)
    {
        if (c == null)
        {
            endOfTurn();
            return;
        }
        //adding the action card to the playedCard pile and removind it from player's hand
        this.currentRound.addToPlayedCards(c);

        CommunicationAPI.sendMessageToClient(null, "updateTopCard", this.currentPlayer.getBandit(), c.getKind());

        this.currentPlayer.hand.Remove(c);

        endOfTurn();
    }

    public void useWhiskey(WhiskeyKind? aKind){
        if (aKind == null)
        {
            endOfTurn();
            return;
        }

        //If the kind is normal/old, such a whiskey of this kind must already exist in the player's possessions and is half
        if (aKind.Equals(WhiskeyKind.Normal)){

            CommunicationAPI.sendMessageToClient(null, "decrementWhiskey", this.currentPlayer.getBandit(), aKind);

            Whiskey aW = currentPlayer.getAWhiskey(aKind.Value);
            aW.drinkASip();
            if(aW.isEmpty()) currentPlayer.removeWhiskey(aW);
            
            drawCards();


            //TO SPECIFIC PLAYER
            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateHasAnotherAction", this.currentPlayer.getBandit(), true, "play");

        }
        else if (aKind.Equals(WhiskeyKind.Old))
        {
            CommunicationAPI.sendMessageToClient(null, "decrementWhiskey", this.currentPlayer.getBandit(), aKind);

            Whiskey aW = currentPlayer.getAWhiskey(aKind.Value);
            aW.drinkASip();
            if(aW.isEmpty()) currentPlayer.removeWhiskey(aW);

            currentPlayer.setGetsAnotherAction(true);

            //TO SPECIFIC PLAYER
            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateHasAnotherAction", this.currentPlayer.getBandit(), true, "play");

        }
        //If the kind is unknown, the player has at least one full whiskey
        else
        {
            CommunicationAPI.sendMessageToClient(null, "decrementWhiskey", this.currentPlayer.getBandit(), WhiskeyKind.Unknown);

            //Retrieve the first full whiskey the player has and do the appropriate action depending on its kind
            Whiskey aW = currentPlayer.getAWhiskey();
            aW.drinkASip();

            //Increment the whiskey
            CommunicationAPI.sendMessageToClient(null, "incrementWhiskey", this.currentPlayer.getBandit(), aW.getWhiskeyKind());

            if (aW.getWhiskeyKind().Equals(WhiskeyKind.Normal))
            {
                
                drawCards();
                CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateHasAnotherAction", this.currentPlayer.getBandit(), true, "play");
            }
            else if (aW.getWhiskeyKind().Equals(WhiskeyKind.Old))
            {
                currentPlayer.setGetsAnotherAction(true);
                CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateHasAnotherAction", this.currentPlayer.getBandit(), true, "play");

            }
        }
    }

    public void drawCards()
    {
        Random rnd = new Random();

        //taking three random cards from player's discardPile and adding them to the player's hand while the discardpile has cards
        List<Card> cardsToAdd = new List<Card>();

        try
        {
            int rand = rnd.Next(0, this.currentPlayer.discardPile.Count);
            Card c = this.currentPlayer.discardPile[rand];
            this.currentPlayer.moveFromDiscardToHand(c);
            cardsToAdd.Add(c);

            rand = rnd.Next(0, this.currentPlayer.discardPile.Count);
            c = this.currentPlayer.discardPile[rand];
            this.currentPlayer.moveFromDiscardToHand(c);
            cardsToAdd.Add(c);

            rand = rnd.Next(0, this.currentPlayer.discardPile.Count);
            c = this.currentPlayer.discardPile[rand];
            this.currentPlayer.moveFromDiscardToHand(c);
            cardsToAdd.Add(c);
        } catch (Exception e) when(e is System.IndexOutOfRangeException || e is System.ArgumentOutOfRangeException)
        { 
            //Out of cards to draw
        }

        //TO SPECIFIC PLAYER 
        CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "addCards", cardsToAdd);

        endOfTurn();
    }

    public void chosenStartinPostion(Position p)
    {
        playerPtrForStarting.setPosition(p);
        //TO ALL PLAYERS
        CommunicationAPI.sendMessageToClient(null, "moveGameUnit", playerPtrForStarting, p, getIndexByTrainCar(p.getTrainCar()));
        p.getTrainCar().setHasAHorse(true);
        playerPtrForStarting.setWaitingForInput(false);

    }

    public void chosenHostage(HostageChar aHostage)
    {
        //TODO send messages 
        Hostage retrievedHostage = availableHostages.Find(x => x.getHostageChar() == aHostage);
        availableHostages.Remove(retrievedHostage);
        currentPlayer.setCapturedHostage(retrievedHostage);
        currentPlayer.setWaitingForInput(false);

        CommunicationAPI.sendMessageToClient(null, "updateHostageName ", currentPlayer.getBandit(), retrievedHostage.getHostageChar());
        this.currentRound.getTopOfPlayedCards();
        CommunicationAPI.sendMessageToClient(null, "removeTopCard");
        this.endOfCards();
    }
    
    public void chosenPosition(Position p)
    {
        //Card removed
        ActionCard topOfPile = this.currentRound.getTopOfPlayedCards();

        //if the action card is a Move Marshall action
        if (topOfPile.getKind().Equals(ActionKind.Marshal))
        {
            this.aMarshal.setPosition(p);
            //TO ALL PLAYERS
            CommunicationAPI.sendMessageToClient(null, "moveGameUnit", this.aMarshal, p, getIndexByTrainCar(p.getTrainCar()));
            //check for all players at position p 
            foreach (Player aPlayer in p.getPlayers())
            {
                BulletCard b = new BulletCard(null, -1);
                aPlayer.addToDiscardPile(b);
                p.getTrainCar().moveRoofCar(aPlayer);

                //TO ALL PLAYERS
                CommunicationAPI.sendMessageToClient(null, "moveGameUnit", aPlayer, p.getTrainCar().getRoof(), getIndexByTrainCar(p.getTrainCar())); 
            }
            currentPlayer.setWaitingForInput(false);
            CommunicationAPI.sendMessageToClient(null, "removeTopCard");
            this.endOfCards();
        }
        //if the action card is a Move action
        else if (topOfPile.getKind().Equals(ActionKind.Move))
        {
            currentPlayer.setPosition(p);
            //TO ALL PLAYERS
            CommunicationAPI.sendMessageToClient(null, "moveGameUnit", currentPlayer, p, getIndexByTrainCar(p.getTrainCar()));

            //if the marshal is at position p, bullet card in deck + sent to the roof 
            if (p.hasMarshal(aMarshal))
            {
                BulletCard b = new BulletCard(null, -1);
                currentPlayer.addToDiscardPile(b);
                p.getTrainCar().moveRoofCar(currentPlayer);

                CommunicationAPI.sendMessageToClient(null, "moveGameUnit", currentPlayer, p.getTrainCar().getRoof(), getIndexByTrainCar(p.getTrainCar()));
            }
            //TODO Same with Shotgun 


            currentPlayer.setWaitingForInput(false);
            CommunicationAPI.sendMessageToClient(null, "removeTopCard");
            this.endOfCards();
        }

        else if (topOfPile.getKind().Equals(ActionKind.Ride))
        {
            currentPlayer.setPosition(p);

            currentPlayer.getPosition().getTrainCar().setHasAHorse(true);
            currentPlayer.setOnAHorse(false);

            //TO ALL PLAYERS
            CommunicationAPI.sendMessageToClient(null, "moveGameUnit", currentPlayer, p, getIndexByTrainCar(p.getTrainCar()));

            if (p.hasMarshal(aMarshal))
            {
                BulletCard b = new BulletCard(null, -1);
                currentPlayer.addToDiscardPile(b);
                p.getTrainCar().moveRoofCar(currentPlayer);
                //TO ALL PLAYERS
                CommunicationAPI.sendMessageToClient(null, "moveGameUnit", currentPlayer, p.getTrainCar().getRoof(), getIndexByTrainCar(p.getTrainCar()));
                CommunicationAPI.sendMessageToClient(null, "removeTopCard");
                this.endOfCards();
            }
            //TODO Same with Shotgun 

            else if (p.isInStageCoach(myStageCoach))
            {
                if (availableHostages.Count() != 0)
                {

                    CommunicationAPI.sendMessageToClient(null, "availableHostages", availableHostages);
                    CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateSelectHostage");
                }
            }
            else
            {
                currentPlayer.setWaitingForInput(false);
                CommunicationAPI.sendMessageToClient(null, "removeTopCard");
                this.endOfCards();
            }
        }

    }

    public void chosenPunchTarget(Player victim, GameItem loot, Position dest)
    {
        this.currentRound.getTopOfPlayedCards();

        //drop the loot at victim position, sends victim to destination 
        if (loot != null)
        {
            loot.setPosition(victim.getPosition());
            //TO ALL PLAYERS
            CommunicationAPI.sendMessageToClient(null, "moveGameItem", loot, victim.getPosition());
        }

        victim.setPosition(dest);
        //TO ALL PLAYERS
        CommunicationAPI.sendMessageToClient(null, "moveGameUnit", victim, dest, getIndexByTrainCar(dest.getTrainCar()));

        //loot is removed from victime possessions
        if (loot != null)
        {
            victim.possessions.Remove(loot);
            //TO ALL PLAYERS
            CommunicationAPI.sendMessageToClient(null, "decrementLoot", victim.getBandit(), loot);
            CommunicationAPI.sendMessageToClient(null, "incrementLoot", this.currentPlayer.getBandit(), loot);
        }
        //if the marshal is at position dest, victim: bullet card in deck + sent to the roof 
        if (dest.hasMarshal(aMarshal))
        {
            BulletCard b = new BulletCard(null, -1);
            victim.addToDiscardPile(b);
            dest.getTrainCar().moveRoofCar(victim);
            //TO ALL PLAYERS
            CommunicationAPI.sendMessageToClient(null, "moveGameUnit", victim, dest.getTrainCar().getRoof(), getIndexByTrainCar(dest.getTrainCar()));
        }
        currentPlayer.setWaitingForInput(false);

        CommunicationAPI.sendMessageToClient(null, "removeTopCard");
        this.endOfCards();
    }

    public void choseToPunchShootgun()
    {
        aShotGun.setPosition(myStageCoach.getAdjacentCar().getRoof());

        CommunicationAPI.sendMessageToClient(null, "moveGameUnit", aShotGun, myStageCoach.getAdjacentCar().getRoof(), getIndexByTrainCar(myStageCoach.getAdjacentCar()));

        aShotGun.hasBeenPunched();

        GameItem loot = new GameItem(ItemType.Strongbox, 1000);
        currentPlayer.addToPossessions(loot);

        CommunicationAPI.sendMessageToClient(null, "incrementLoot", this.currentPlayer.getBandit(), loot);

        currentPlayer.setWaitingForInput(false);
        CommunicationAPI.sendMessageToClient(null, "removeTopCard");
        this.endOfCards();
    }
    
    public void chosenShootTarget(Player target)
    {
        this.currentRound.getTopOfPlayedCards();
        //A BulletCard is transfered from bullets of currentPlayer to target's discardPile
        BulletCard aBullet = currentPlayer.getABullet();
        target.addToDiscardPile(aBullet);
        this.currentPlayer.shootBullet();
        //TO ALL PLAYERS
        CommunicationAPI.sendMessageToClient(null, "decrementBullets", this.currentPlayer.getBandit(), this.currentPlayer.getNumOfBulletsShot());
        currentPlayer.setWaitingForInput(false);
        CommunicationAPI.sendMessageToClient(null, "removeTopCard");
        this.endOfCards();
    }

    public void chosenLoot(GameItem loot)
    {
        this.currentRound.getTopOfPlayedCards();
        //the loot is transfered from the position to the currentPlayer possensions
        loot.setPosition(null);
        currentPlayer.addToPossessions(loot);
        //TO ALL PLAYERS
        CommunicationAPI.sendMessageToClient(null, "incrementLoot", this.currentPlayer.getBandit(), loot);
        CommunicationAPI.sendMessageToClient(null, "updateLootAtLocation", this.currentPlayer.getPosition(), getIndexByTrainCar(this.currentPlayer.getPosition().getTrainCar()), this.currentPlayer.getPosition().getItems(), true);
        currentPlayer.setWaitingForInput(false);
        CommunicationAPI.sendMessageToClient(null, "removeTopCard");
        this.endOfCards();
    }

    public void readyForNextMove()
    {
        //See the top card in the queue and return it to its player's discard pile
        //Not removed yet
        ActionCard top = this.currentRound.seeTopOfPlayedCards();
        top.belongsTo().addToDiscardPile(top);

        //Figure out who is the current player
        this.currentPlayer = top.belongsTo();
        CommunicationAPI.sendMessageToClient(null, "updateCurrentPlayer", this.currentPlayer.getBandit());

        Console.WriteLine("Resolving " + top.getKind() + " from " + top.belongsTo().getBandit());

        this.currentPlayer.setWaitingForInput(false);
        Boolean waiting = true;

        if (waiting)
        {
            // Get the top of the played cards from the schemin phase
            switch (top.getKind())
            {
                case ActionKind.Move:
                    {
                        List<Position> moves = this.getPossibleMoves(this.currentPlayer);
                        List<int> indices = new List<int>();
                        
                        //Remove the stagecoach interior from the list of moves - you cannot use a Move card to go there
                        //You can move to the roof, though
                        if (moves.Contains(myStageCoach.getInside()))
                        {
                            moves.Remove(myStageCoach.getInside());
                        }

                        moves.ForEach(m => indices.Add(getIndexByTrainCar(m.getTrainCar())));

                        if (moves.Count > 1)
                        {
                            this.aGameStatus = GameStatus.FinalizingCard;
                            this.currentPlayer.setWaitingForInput(true);
                            //TO SPECIFIC PLAYERS
                            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateMovePositions", moves, indices);
                            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateWaitingForInput", this.currentPlayer.getBandit(), true);
                        }
                        else
                        {
                            chosenPosition(moves[0]);
                        }
                        break;
                    }
                case ActionKind.ChangeFloor:
                    {
                        //If the player is inside a car
                        if (this.currentPlayer.getPosition().isInside())
                        {
                            this.currentPlayer.getPosition().getTrainCar().moveRoofCar(this.currentPlayer);
                            //TO ALL PLAYERS
                            CommunicationAPI.sendMessageToClient(null, "moveGameUnit", currentPlayer, this.currentPlayer.getPosition().getTrainCar().getRoof(), getIndexByTrainCar(this.currentPlayer.getPosition().getTrainCar()));

                            this.currentRound.getTopOfPlayedCards();
                            CommunicationAPI.sendMessageToClient(null, "removeTopCard");
                            this.endOfCards();
                        }
                        //If the player is on the roof of a car 
                        else
                        {
                            this.currentPlayer.getPosition().getTrainCar().moveInsideCar(this.currentPlayer);
                            //TO ALL PLAYERS
                            CommunicationAPI.sendMessageToClient(null, "moveGameUnit", currentPlayer, this.currentPlayer.getPosition().getTrainCar().getInside(), getIndexByTrainCar(this.currentPlayer.getPosition().getTrainCar()));

                            //If the player ends up in a car with the Marshal, he takes a bullet
                            if (this.currentPlayer.getPosition().hasMarshal(this.aMarshal))
                            {
                                this.currentPlayer.addToDiscardPile(new BulletCard(null, -1));
                                this.currentPlayer.getPosition().getTrainCar().moveRoofCar(this.currentPlayer);
                                //TO ALL PLAYERS
                                CommunicationAPI.sendMessageToClient(null, "moveGameUnit", currentPlayer, this.currentPlayer.getPosition().getTrainCar().getRoof(), getIndexByTrainCar(this.currentPlayer.getPosition().getTrainCar()));

                                this.currentRound.getTopOfPlayedCards();
                                CommunicationAPI.sendMessageToClient(null, "removeTopCard");
                                this.endOfCards();
                            }

                            //TODO Same with Shotgun 

                            //Else, if he ends up in the StageCoach, he chooses a hostage (if there are any left)
                            else if (this.currentPlayer.getPosition().isInStageCoach(myStageCoach))
                            {
                                if (availableHostages.Count() != 0)
                                {
                                    this.aGameStatus = GameStatus.FinalizingCard;
                                    this.currentPlayer.setWaitingForInput(true);
                                    //TODO new massage
                                    CommunicationAPI.sendMessageToClient(null, "availableHostages", availableHostages);
                                    CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateSelectHostage");
                                }
                            }
                            else
                            {
                                this.currentRound.getTopOfPlayedCards();
                                CommunicationAPI.sendMessageToClient(null, "removeTopCard");
                                this.endOfCards();
                            }
                        }
                        break;
                    }
                case ActionKind.Shoot:
                    {
                        List<Player> possTargets = this.getPossibleShootTarget(this.currentPlayer);
                        if (possTargets.Count == 1)
                        {
                            this.chosenShootTarget(possTargets[0]);
                        }
                        else if (possTargets.Count > 1)
                        {
                            this.aGameStatus = GameStatus.FinalizingCard;
                            this.currentPlayer.setWaitingForInput(true);
                            //TO SPECIFIC PLAYER
                            
                            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updatePossTarget", possTargets);
                        } else
                        {
                            this.currentRound.getTopOfPlayedCards();
                            CommunicationAPI.sendMessageToClient(null, "removeTopCard");
                            this.endOfCards();
                        }
                        break;
                    }
                case ActionKind.Rob:
                    {

                        List<GameItem> atLocation = this.currentPlayer.getPosition().getItems();

                        if (atLocation.Count == 0)
                        {
                            //Empty
                            this.currentRound.getTopOfPlayedCards();
                            CommunicationAPI.sendMessageToClient(null, "removeTopCard");
                            this.endOfCards();
                        } else if ()
                        {
                            this.aGameStatus = GameStatus.FinalizingCard;
                            this.currentPlayer.setWaitingForInput(true);
                            //TO SPECIFIC PLAYER

                            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateLootAtLocation", this.currentPlayer.getPosition(), getIndexByTrainCar(this.currentPlayer.getPosition().getTrainCar()), atLocation, false);
                            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateWaitingForInput", this.currentPlayer.getBandit(), true);
                        }
                       
                        break;
                    }
                case ActionKind.Marshal:
                    {
                        
                        List<Position> possPosition = this.getPossibleMoves(this.aMarshal);
                        List<int> indices = new List<int>();
                        
                        possPosition.ForEach(m => indices.Add(getIndexByTrainCar(m.getTrainCar())));

                        if (possPosition.Count > 1)
                        {
                            this.aGameStatus = GameStatus.FinalizingCard;
                            this.currentPlayer.setWaitingForInput(true);
                            //TO SPECIFIC PLAYERS
                            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateMovePositions", possPosition, indices);
                            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateWaitingForInput", this.currentPlayer.getBandit(), true);
                        }
                        else
                        {
                            this.chosenPosition(possPosition[0]);
                        }
                        break;
                    }
                case ActionKind.Punch:
                    {
                        bool shotgunIsATarget = (this.currentPlayer.getPosition().isInStageCoach(this.myStageCoach) && this.currentPlayer.getPosition().isInside() && this.aShotGun.getIsOnStageCoach() == false);
                        List<Player> atLocation = this.currentPlayer.getPosition().getPlayers();

                        //Remove the player who requested the action themself, lol
                        atLocation.Remove(this.currentPlayer);
                        
                        if (atLocation.Count == 0 && shotgunIsATarget == false)
                        {
                            //Empty
                            this.currentRound.getTopOfPlayedCards();
                            CommunicationAPI.sendMessageToClient(null, "removeTopCard");
                            this.endOfCards();

                        } else 
                        {
                            this.aGameStatus = GameStatus.FinalizingCard;
                            this.currentPlayer.setWaitingForInput(true);
                            //TO SPECIFIC PLAYER
                            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updatePossTargetPunch", atLocation, shotgunIsATarget);
                        }
                        break;
                    }
                case ActionKind.Ride:
                    {
                        if (currentPlayer.getPosition().getTrainCar().hasHorseAtCarLevel())
                        {
                            //set current player on a horse 
                            currentPlayer.setOnAHorse(true);

                            //geting all possible move when player is on a horse
                            List<Position> moves = getPossibleMoves(this.currentPlayer);

                            this.aGameStatus = GameStatus.FinalizingCard;
                            this.currentPlayer.setWaitingForInput(true);

                            //NEW message for a ride action 
                            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "PossRidePositions", moves);

                            //setting the on a horse action to false.
                            currentPlayer.getPosition().getTrainCar().setHasAHorse(false);
                        }
                        break;
                    }
            }

            
        }
    }


    /**
    *   Private helper methods
    */
    private void endOfTurn()
    {
        //if the player has another action, then the anotherAction flag is set to false
        if (this.currentPlayer.isGetsAnotherAction())
        {
            //TO SPECIFIC PLAYER
            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateHasAnotherAction", this.currentPlayer.getBandit(), true, "both");
            this.currentPlayer.setGetsAnotherAction(false);
            
        }
        else
        {
            this.currentPlayer.setWaitingForInput(false);

           
            //determining the next player 
            //if the turn is Switching, order of players is reversed, so next player is previous in the list
            if (this.currentTurn.getType() == TurnType.Switching)
            {
                this.currentPlayerIndex = mod((this.currentPlayerIndex - 1), this.totalPlayer);
                this.currentPlayer = this.players[this.currentPlayerIndex];
            }
            //otherwise, it is the next player in the list 
            else
            {
                this.currentPlayerIndex = mod((this.currentPlayerIndex + 1), this.totalPlayer);
                this.currentPlayer = this.players[this.currentPlayerIndex];
            }

            //Verify whether the next player is the first player again; if so, go to the next turn
            if (this.currentPlayerIndex == this.firstPlayerIndex)
            {
                int nextTurnIndex = this.currentRound.getTurns().IndexOf(this.currentTurn) + 1;

                //If the next turn index is out of bounds, the current turn is the last turn
                if (nextTurnIndex == this.currentRound.getTurns().Count)
                {
                    //prepare for Stealing phase 
                    foreach (Player p in this.players)
                    {
                        p.moveCardsToDiscard();
                        //Send an empty list to the client's hand
                        CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(p), "updatePlayerHand", p.getBandit(), new List<Card>());

                    }

                    this.aGameStatus = GameStatus.Stealin;
                    //TO ALL PLAYERS
                    CommunicationAPI.sendMessageToClient(null, "updateGameStatus", this.aGameStatus);

                    Thread.Sleep(1000);

                    endOfCards();
                    return;
                }
                else
                {
                    this.currentTurn = this.currentRound.getTurns()[nextTurnIndex];
                    CommunicationAPI.sendMessageToClient(null, "updateCurrentTurn", nextTurnIndex);

                    if (this.currentTurn.getType() == TurnType.Switching)
                    {
                        this.currentPlayerIndex = firstPlayerIndex;
                        this.currentPlayer = this.players[firstPlayerIndex];
                    }
                }
            } 
            
            if (this.currentTurn.getType() == TurnType.SpeedingUp)
            {
                this.currentPlayer.setGetsAnotherAction(true);
            }

            //TO ALL PLAYERS

            CommunicationAPI.sendMessageToClient(null, "updateCurrentPlayer", currentPlayer.getBandit());
            //TO CURRENT PLAYER
            if (this.currentPlayer.isGetsAnotherAction())
            {
                CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateHasAnotherAction", this.currentPlayer.getBandit(), true, "both");
            }
            else
            {
                this.currentPlayer.setWaitingForInput(true);
                CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateWaitingForInput", currentPlayer.getBandit(), true);
            }

            Console.WriteLine(this.currentTurn.getType());
            Console.WriteLine(this.currentPlayerIndex);

        }
    }

    private void endOfCards()
    {
        try
        {
            Card c = this.currentRound.seeTopOfPlayedCards();
            CommunicationAPI.sendMessageToClient(null, "highlightTopCard");

            Thread.Sleep(3000);

            readyForNextMove();
        }
        catch (Exception e) when (e is InvalidOperationException)
        {
            //The queue is empty
            //if all cards in the pile have been played 
            if (this.currentRound.getPlayedCards().Count == 0)
            {
                //if this is the last round 
                if (this.currentRound.getIsLastRound())
                {
                    calculateGameScore();
                }
                else
                {
                    //setting the next round, setting the first turn of the round 
                    this.currentRound = this.rounds[this.rounds.IndexOf(this.currentRound) + 1];
                    //TO ALL PLAYERS
                    CommunicationAPI.sendMessageToClient(null, "updateCurrentRound", this.currentRound);

                    this.currentTurn = this.currentRound.getTurns()[0];
                    //TO ALL PLAYERS
                    CommunicationAPI.sendMessageToClient(null, "updateCurrentTurn", this.currentRound.getTurns().IndexOf(currentTurn));

                    //setting the next First player and game status of the game
                    this.firstPlayerIndex = (this.firstPlayerIndex == totalPlayer - 1) ? 0 : firstPlayerIndex++;
                    this.currentPlayer = this.players[firstPlayerIndex];
                    this.currentPlayerIndex = this.players.IndexOf(currentPlayer);
                    //TO ALL PLAYERS

                    CommunicationAPI.sendMessageToClient(null, "updateFirstPlayer", this.currentPlayer.getBandit());
                    CommunicationAPI.sendMessageToClient(null, "updateCurrentPlayer", this.currentPlayer.getBandit());

                    this.aGameStatus = GameStatus.Schemin;
                    //TO ALL PLAYERS
                    CommunicationAPI.sendMessageToClient(null, "updateGameStatus", this.aGameStatus);

                    this.currentPlayer.setWaitingForInput(true);
                    //TO SPECIFIC PLAYER
                    CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateWaitingForInput", this.currentPlayer.getBandit(), true);

                    //for each player, getting 6 cards from their Pile at randomn and adding them to their hand 
                    foreach (Player p in this.players)
                    {
                        List<Card> cardsToAdd = new List<Card>();
                        int index = this.players.IndexOf(p);

                        Random rnd = new Random();
                        for (int i = 0; i < 6; i++)
                        {
                            int rand = rnd.Next(0, p.discardPile.Count);
                            Card aCard = p.discardPile[rand];
                            p.hand.Add(aCard);
                            cardsToAdd.Add(aCard);
                            p.discardPile.Remove(aCard);
                        }
                        //TODO NEED TO SEE WITH CRISTINA
                        //TO SPECIFIC PLAYER
                        CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(p), "updatePlayerHand", p.getBandit(), cardsToAdd);

                    }

                    //Stagecoach moves by one car to the back, if not at the last one of the train.
                    TrainCar curAdjacent = myStageCoach.getAdjacentCar();

                    if (!curAdjacent.Equals(myTrain[myTrain.Count() - 1]))
                    {
                        TrainCar newAdjacent = myTrain[myTrain.IndexOf(curAdjacent) + 1];
                        myStageCoach.setAdjacentCar(newAdjacent);

                        CommunicationAPI.sendMessageToClient(null, "moveStageCoach");

                        //if Shotgun is not on the stage coach, it also moves by one car to the back
                        if (!aShotGun.getIsOnStageCoach())
                        {
                            aShotGun.setPosition(newAdjacent.getRoof());
                        }
                    }

                }
            }
        }

    }

    private void calculateGameScore()
    {

        Dictionary<Player, int> scores = new Dictionary<Player, int>();
        int max = -1;
        Player maxPlayer = null;

        foreach (Player pl in this.players)
        {
            int totalValue = pl.getPossesionsValue() + pl.getHostageValue();

            scores.Add(pl, totalValue);
            if (pl.getNumOfBulletsShot() > max)
            {
                max = pl.getNumOfBulletsShot();
                maxPlayer = pl;
            }
        }
        scores[maxPlayer] = scores[maxPlayer] + 1000;

        //Sorted list to send to clients
        var myList = scores.ToList();
        myList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));

        //TO ALL PLAYERS
        CommunicationAPI.sendMessageToClient(null, "finalGameScores", myList, maxPlayer);

        this.endOfGame = true;
    }

    private void initializeGameBoard()
    {

        //initializing Locomotive
        this.myTrain.Add(new TrainCar(true));

        //initializing train cars
        for (int i = 0; i < players.Count; i++)
        {
            this.myTrain.Add(new TrainCar(false));
        }

        //initializing StageCoach
        myStageCoach = new StageCoach(false, myTrain[0]);

        // initializing the marshall and init his position to inside locomotive
        aMarshal = Marshal.getInstance();
        myTrain[0].moveInsideCar(aMarshal);

        // initializing the shotgun and init his position to the roof of the stageCoach
        aShotGun = Shotgun.getInstance();
        myStageCoach.moveRoofCar(aShotGun);

        //TESTING
        foreach (Player p in this.players) {
            p.setPosition(this.myTrain[this.myTrain.Count - 1].getInside());
        }
    }

    private void initializeLoot()
    {
        //initializaing a Strongbox in the locomotive 
        GameItem locomotiveStongBox = new GameItem(ItemType.Strongbox, 1000);
        locomotiveStongBox.setPosition(myTrain[0].getInside());

            //REMARK strongbox only accessible when you punch the shotgun 
        //initializaing a Strongbox in the stage coach 
        //GameItem stageCoachStongBox = new GameItem(ItemType.Strongbox, 1000);
        //stageCoachStongBox.setPosition(myStageCoach.getInside());

        // depending on the number of player, 
        // we'll initialize loots for totalPlayer number of wagon
        for (int i = 0; i < totalPlayer; i++)
        {
            switch (i)
            {
                //intializing loots in first wagon
                case 0:
                    {
                        GameItem anItem = new GameItem(ItemType.Purse, 500);
                        anItem.setPosition(myTrain[1].getInside());
                        GameItem anItem1 = new GameItem(ItemType.Purse, 250);
                        anItem1.setPosition(myTrain[1].getInside());
                        GameItem anItem2 = new GameItem(ItemType.Purse, 250);
                        anItem2.setPosition(myTrain[1].getInside());
                        Whiskey aW = new Whiskey(WhiskeyKind.Normal);
                        break;
                    }
                //intializing loots in second wagon
                case 1:
                    {
                        GameItem anItem = new GameItem(ItemType.Ruby, 500);
                        anItem.setPosition(myTrain[2].getInside());
                        GameItem anItem1 = new GameItem(ItemType.Ruby, 500);
                        anItem1.setPosition(myTrain[2].getInside());
                        GameItem anItem2 = new GameItem(ItemType.Ruby, 500);
                        anItem2.setPosition(myTrain[2].getInside());
                        Whiskey aW = new Whiskey(WhiskeyKind.Old);
                        break;
                    }
                //intializing loots in third wagon
                case 2:
                    {
                        GameItem anItem = new GameItem(ItemType.Ruby, 500);
                        anItem.setPosition(myTrain[3].getInside());
                        GameItem anItem1 = new GameItem(ItemType.Purse, 500);
                        anItem1.setPosition(myTrain[3].getInside());
                        Whiskey aW = new Whiskey(WhiskeyKind.Normal);
                        break;
                    }
                //intializing loots in 4th wagon
                case 3:
                    {
                        GameItem anItem = new GameItem(ItemType.Ruby, 500);
                        anItem.setPosition(myTrain[4].getInside());
                        GameItem anItem1 = new GameItem(ItemType.Purse, 500);
                        anItem1.setPosition(myTrain[4].getInside());
                        GameItem anItem2 = new GameItem(ItemType.Purse, 250);
                        anItem1.setPosition(myTrain[4].getInside());
                        GameItem anItem3 = new GameItem(ItemType.Purse, 250);
                        anItem1.setPosition(myTrain[4].getInside());
                        Whiskey aW = new Whiskey(WhiskeyKind.Normal);
                        break;
                    }
                //intializing loots in 5th wagon
                case 4:
                    {
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
                        Whiskey aW = new Whiskey(WhiskeyKind.Normal);
                        break;
                    }
                //intializing loots in 6th wagon
                case 5:
                    {
                        GameItem anItem = new GameItem(ItemType.Purse, 500);
                        anItem.setPosition(myTrain[6].getInside());
                        Whiskey aW = new Whiskey(WhiskeyKind.Normal);
                        break;
                    }
            }
        }
    }

    private void intializeRounds()
    {
        for (int i = 0; i < 4; i++)
        {
            Round aRound = new Round(false, totalPlayer);
            this.rounds.Add(aRound);
        }
        Round aFinalRound = new Round(true, totalPlayer);
        this.rounds.Add(aFinalRound);
    }

    private List<Position> getPossibleMoves(Marshal m)
    {
        List<Position> possPos = new List<Position>();
        TrainCar marshalCar = m.getPosition().getTrainCar();

        try
        {
            // Add adjacent position
            possPos.Add(this.myTrain[this.myTrain.IndexOf(marshalCar) - 1].getInside());
        }
        catch (Exception e) when (e is System.IndexOutOfRangeException || e is System.ArgumentOutOfRangeException)
        {
        }

        try
        {
            // Add adjacent position
            possPos.Add(this.myTrain[this.myTrain.IndexOf(marshalCar) + 1].getInside());


        }
        catch (Exception e) when (e is System.IndexOutOfRangeException || e is System.ArgumentOutOfRangeException)
        {
        }
        

        return possPos;
    }

    private List<Position> getPossibleMoves(Player p)
    {
        List<Position> possPos = new List<Position>();
        TrainCar playerCar = p.getPosition().getTrainCar();
        // Check if on a roof or not

        if (currentPlayer.isPlayerOnAHorse())
        {
            for (int i = 1; i < 4; i++)
            {
                try
                {
                    TrainCar tmp = this.myTrain[this.myTrain.IndexOf(playerCar) - i];

                    // Add adjacent possible positions at the back of current position
                    possPos.Add(tmp.getInside());
                    //if there is the stage coach et the desired level, stagecoach is counted. 
                    if (myStageCoach.getAdjacentCar().Equals(tmp))
                    {
                        possPos.Add(myStageCoach.getInside());
                    }
                }
                catch (Exception e) when (e is System.IndexOutOfRangeException || e is System.ArgumentOutOfRangeException)
                {
                    continue;
                }
            }
            for (int i = 1; i < 4; i++)
            {
                try
                {
                    TrainCar tmp = this.myTrain[this.myTrain.IndexOf(playerCar) + i];
                    // Add adjacent possible positions at the front of current position
                    possPos.Add(tmp.getInside());
                    if (myStageCoach.getAdjacentCar().Equals(tmp))
                    {
                        possPos.Add(myStageCoach.getInside());
                    }
                }
                catch (Exception e) when (e is System.IndexOutOfRangeException || e is System.ArgumentOutOfRangeException)
                {
                    continue;
                }
            }

        }
        else if (!p.getPosition().isInside())
        {
            //if the playerCar is adjacent to the stageCoach, add roof of the stage coach 
            if (myStageCoach.getAdjacentCar().Equals(playerCar))
            {
                possPos.Add(myStageCoach.getRoof());
            }

            // Add 1-3 distance forward or backwards
            for (int i = 1; i < 4; i++)
            {
                try
                {
                    // Add adjacent positions
                    possPos.Add(this.myTrain[this.myTrain.IndexOf(playerCar) - i].getRoof());
                }
                catch (Exception e) when (e is System.IndexOutOfRangeException || e is System.ArgumentOutOfRangeException)
                {
                    continue;
                }
                //if the car is adjacent to the stageCoach, add roof of the stage coach to possPos
                if (myStageCoach.getAdjacentCar().Equals(myTrain[myTrain.IndexOf(playerCar) - i]))
                {
                    possPos.Add(myStageCoach.getRoof());
                }
            }

            for (int i = 1; i < 4; i++)
            {
                try
                {
                    // Add adjacent positions
                    possPos.Add(this.myTrain[this.myTrain.IndexOf(playerCar) + i].getRoof());


                }
                catch (Exception e) when (e is System.IndexOutOfRangeException || e is System.ArgumentOutOfRangeException)
                {
                    continue;
                }

                //if the car is adjacent to the stageCoach, add roof of the stage coach to possPos
                if (myStageCoach.getAdjacentCar().Equals(myTrain[myTrain.IndexOf(playerCar) + i]))
                {
                    possPos.Add(myStageCoach.getRoof());
                }
            }
        }
        else
        {
            //if the playerCar is adjacent to the stageCoach, add inside of the stage coach 
            if (myStageCoach.getAdjacentCar().Equals(playerCar))
            {
                possPos.Add(myStageCoach.getInside());
            }

            try
            {
                TrainCar wagon = this.myTrain[this.myTrain.IndexOf(playerCar) - 1];
                // Add adjacent positions
                possPos.Add(wagon.getInside());

            }
            catch (Exception e) when (e is System.IndexOutOfRangeException || e is System.ArgumentOutOfRangeException)
            {

            }

            try
            {
                // Add adjacent positions
                possPos.Add(this.myTrain[this.myTrain.IndexOf(playerCar) + 1].getInside());
            }
            catch (Exception e) when (e is System.IndexOutOfRangeException || e is System.ArgumentOutOfRangeException)
            {

            }

        }
        return possPos;
    }

    public void getPossiblePunchMoves(Player p)
    {
        List<Position> possPos = new List<Position>();
        TrainCar playerCar = p.getPosition().getTrainCar();

        //Move the target to the floor of an adjacent car        
        //if the playerCar is adjacent to the stageCoach, add inside of the stage coach 
        if (myStageCoach.getAdjacentCar().Equals(playerCar))
        {
            possPos.Add(myStageCoach.getInside());
        }

        try
        {
            TrainCar wagon = this.myTrain[this.myTrain.IndexOf(playerCar) - 1];
            // Add adjacent positions
            possPos.Add(wagon.getInside());

        }
        catch (Exception e) when (e is System.IndexOutOfRangeException || e is System.ArgumentOutOfRangeException)
        {

        }

        try
        {
            // Add adjacent positions
            possPos.Add(this.myTrain[this.myTrain.IndexOf(playerCar) + 1].getInside());
        }
        catch (Exception e) when (e is System.IndexOutOfRangeException || e is System.ArgumentOutOfRangeException)
        {

        }

        List<int> indices = new List<int>();

        possPos.ForEach(m => indices.Add(getIndexByTrainCar(m.getTrainCar())));

        CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updatePunchPositions", possPos, indices);

        CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateWaitingForInput", this.currentPlayer.getBandit(), true);
    }

    public Boolean getEndOfGame()
    {
        return endOfGame;
    }

    private List<Player> getPossibleShootTarget(Player p)
    {
        List<Player> possPlayers = new List<Player>();
        TrainCar playerCar = p.getPosition().getTrainCar();

        //If on roof of stagecoach, all players on the train's roofs are targets
        if (myStageCoach.getRoof().Equals(p.getPosition()))
        {

            for (int i = 0; i < myTrain.Count(); i++)
            {
                List<Player> playersOnWagon = myTrain[i].getRoof().getPlayers();
                if (playersOnWagon.Count != 0)
                {
                    possPlayers.AddRange(playersOnWagon);
                    break;
                }
            }
        }
        //If inside, add adjacent car's players as targets
        else if (myStageCoach.getInside().Equals(p.getPosition()))
        {
            Position inSC = myStageCoach.getAdjacentCar().getInside();
            List<Player> playersOnWagon = inSC.getPlayers();
            if (playersOnWagon.Count != 0)
            {
                possPlayers.AddRange(playersOnWagon);
            }
        }
        //On roof
        else if (!p.getPosition().isInside())
        {
            // Look for the players in line of sight forward on roof
            for (int i = this.myTrain.IndexOf(playerCar) + 1; i < myTrain.Count; i++)
            {
                List<Player> playersOnWagon = this.myTrain[i].getRoof().getPlayers();
                if (playersOnWagon.Count != 0)
                {
                    possPlayers.AddRange(playersOnWagon);
                    break;
                }
            }

            // Look for the players in line of sight backwards on roof
            for (int i = this.myTrain.IndexOf(playerCar) - 1; i >= 0; i--)
            {
                List<Player> playersOnWagon2 = this.myTrain[i].getRoof().getPlayers();
                if (playersOnWagon2.Count != 0)
                {
                    possPlayers.AddRange(playersOnWagon2);
                    break;
                }
            }

            //Look for player on the roof of the stageCoach, if the playerCar is adjacent to the stageCoach 
            if (myStageCoach.getAdjacentCar().Equals(playerCar))
            {

                List<Player> playersOnWagon3 = this.myStageCoach.getRoof().getPlayers();
                if (playersOnWagon3.Count != 0)
                {
                    possPlayers.AddRange(playersOnWagon3);
                }
            }
        }
        //Inside
        else
        {
            // Look for the players in the next wagon backwards
            try
            {
                // Add adjacent positions
                possPlayers.AddRange(this.myTrain[this.myTrain.IndexOf(playerCar) - 1].getInside().getPlayers());
            }
            catch (Exception e) when (e is System.IndexOutOfRangeException || e is System.ArgumentOutOfRangeException)
            {

            }

            // Loof for the players in the next wagon forward
            try
            {
                // Add adjacent positions
                possPlayers.AddRange(this.myTrain[this.myTrain.IndexOf(playerCar) + 1].getInside().getPlayers());
            }
            catch (Exception e) when (e is System.IndexOutOfRangeException || e is System.ArgumentOutOfRangeException)
            {

            }
            //Look for player on the roof of the stageCoach, if the playerCar is adjacent to the stageCoach 
            if (myStageCoach.getAdjacentCar().Equals(playerCar))
            {

                List<Player> playersOnWagon3 = this.myStageCoach.getInside().getPlayers();
                if (playersOnWagon3.Count != 0)
                {
                    possPlayers.AddRange(playersOnWagon3);
                }
            }
        }

        // If there is more than one possible player, we remove Belle.
        if (possPlayers.Count > 1)
        {
            foreach (Player pl in possPlayers)
            {
                if (pl.getBandit() == Character.Belle)
                {
                    possPlayers.Remove(pl);
                }
            }
        }

        return possPlayers;

    }

    public Position getPositionByIndex(int index, Boolean inside)
    {
        if (inside)
        {
            return this.myTrain[index].getInside();
        }
        else
        {
            return this.myTrain[index].getRoof();
        }
    }

    public GameItem getItemfromTypePosition(ItemType aType)
    {
        List<GameItem> al = this.currentPlayer.getPosition().getItems();
        foreach (GameItem anItem in al)
        {
            if (anItem.getType().Equals(aType))
            {
                return anItem;
            }
        }
        return null;
    }

    public GameItem getItemfromTypePosition(WhiskeyStatus aStatus, WhiskeyKind aKind)
    {
        List<GameItem> al = this.currentPlayer.getPosition().getItems();
        foreach (GameItem anItem in al)
        {
            if (anItem is Whiskey)
            {
                if (((Whiskey)anItem).getWhiskeyKind() == aKind && ((Whiskey)anItem).getWhiskeyStatus() == aStatus)
                {
                    return (Whiskey)anItem;
                } 
                else if (((Whiskey)anItem).getWhiskeyKind() == aKind && ((Whiskey)anItem).getWhiskeyStatus() == aStatus)
                {
                    return (Whiskey)anItem;
                }
            }
        }
        
        return null;
    }

    public GameItem getItemfromTypePossession(ItemType aType)
    {
        List<GameItem> al = this.currentPlayer.possessions;
        foreach (GameItem anItem in al)
        {
            if (anItem.getType().Equals(aType))
            {
                return anItem;
            }
        }
        return null;
    }

    public ActionCard getCardByIndex(int index)
    {
        List<Card> al = this.currentPlayer.hand;
        //Console.WriteLine("INDEX AT " + index +" WITH "+ ((ActionCard)al[index]).getKind());
        return (ActionCard)al[index];
    }

    public int getIndexByTrainCar(TrainCar trainCar)
    {
        return myTrain.IndexOf(trainCar);
    }

    //C# does a remainder, not a true modulo
    private int mod(int x, int m)
    {
        int r = x % m;
        return r < 0 ? r + m : r;
    }
}

