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
using AttackSpace;


enum GameStatus
{
    ChoosingBandits,
    Schemin,
    Stealin,
    FinalizingCard,
    Completed,
    HorseAttack
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
    private List<TrainCar> myTrain;
    private StageCoach myStageCoach;
    private Marshal aMarshal;
    private Shotgun aShotGun;
    private List<Hostage> availableHostages;
    private Boolean endHorseAttack;
    private List<AttackPosition> attPos;
    private int horseAttackCounter;

    private GameController()
    {
        this.players = new List<Player>();
        this.myTrain = new List<TrainCar>();
        this.rounds = new List<Round>();
        this.availableHostages = new List<Hostage>();
        totalPlayer = 3;
        this.endOfGame = false;
        this.endHorseAttack = false;
        this.horseAttackCounter = 0;
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
        if (players.Count == totalPlayer)
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

            this.aGameStatus = GameStatus.HorseAttack;
            //TO ALL PLAYERS
            CommunicationAPI.sendMessageToClient(null, "updateGameStatus", aGameStatus);

            this.currentRound = rounds[0];
            //TO ALL PLAYERS
            CommunicationAPI.sendMessageToClient(null, "updateCurrentRound", currentRound);

            this.currentTurn = currentRound.getTurns()[0];
            //TO ALL PLAYERS
            CommunicationAPI.sendMessageToClient(null, "updateCurrentTurn", this.currentRound.getTurns().IndexOf(currentTurn));


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

                if (p.getBandit().Equals(Character.Doc))
                {
                    int rand = rnd.Next(0, p.discardPile.Count);
                    Card aCard = p.discardPile[rand];
                    p.hand.Add(aCard);
                    cardsToAdd.Add(aCard);
                    p.discardPile.Remove(aCard);
                }

                CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(p), "updatePlayerHand", p.getBandit(), cardsToAdd);
            }

            // Initialize a List with an AttackPosition for each Player
            this.attPos = new List<AttackPosition>();

            for (int i = 0; i < this.totalPlayer; i++)
            {
                attPos.Add(new AttackPosition(this.players[i].getBandit(), this.totalPlayer));
            }

            //intializing the first player 
            this.currentPlayer = players[0];
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
        //adding the action card to the playedCard pile and removind it from player's hand
        if (c.isCanBePlayed())
        {
            this.currentRound.addToPlayedCards(c);
            //TODO see with Christina
            CommunicationAPI.sendMessageToClient(null, "updateTopCard",c.belongsTo(), c);
            this.currentPlayer.hand.Remove(c);
        }
        currentPlayer.setWaitingForInput(false);
        CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(currentPlayer), "updateWaitingForInput", currentPlayer.getBandit(), false);

        endOfTurn();
    }

    public void useWhiskey(WhiskeyKind aKind)
    {
        if (aKind.Equals(WhiskeyKind.Normal))
        {

            Whiskey aW = currentPlayer.getAWhiskey();
            aW.drinkASip();
            //TODO see with Christina
            CommunicationAPI.sendMessageToClient(null, "drinkWhiskey", currentPlayer, aKind);
            if (aW.isEmpty())
            {
                currentPlayer.removeWhiskey(aW);
                CommunicationAPI.sendMessageToClient(null, "decrementWhiskey", currentPlayer.getBandit(), aKind);
            }

            drawCards();



            currentPlayer.setGetsAnotherAction(true);
        }
        else
        {
            Whiskey aW = currentPlayer.getAWhiskey();
            aW.drinkASip();
            CommunicationAPI.sendMessageToClient(null, "drinkWhiskey", currentPlayer, aKind);
            if (aW.isEmpty())
            {
                currentPlayer.removeWhiskey(aW);
                CommunicationAPI.sendMessageToClient(null, "decrementWhiskey", currentPlayer.getBandit(), aKind);
            }

            currentPlayer.setUsedOldWhiskey(true);

            currentPlayer.setGetsAnotherAction(true);
            //CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateHasAnotherAction", currentPlayerIndex, true);

        }

        currentPlayer.setWaitingForInput(false);
        CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(currentPlayer), "updateWaitingForInput", currentPlayer.getBandit(), false);

        endOfTurn();

    }

    public void drawCards()
    {
        Random rnd = new Random();

        //taking three random cards from player's discardPile and adding them to the player's hand
        int rand = rnd.Next(0, this.currentPlayer.discardPile.Count);
        Card c = this.currentPlayer.discardPile[rand];
        this.currentPlayer.moveFromDiscardToHand(c);

        rand = rnd.Next(0, this.currentPlayer.discardPile.Count);
        Card c1 = this.currentPlayer.discardPile[rand];
        this.currentPlayer.moveFromDiscardToHand(c);

        rand = rnd.Next(0, this.currentPlayer.discardPile.Count);
        Card c2 = this.currentPlayer.discardPile[rand];
        this.currentPlayer.moveFromDiscardToHand(c);


        //TO SPECIFIC PLAYER 
        CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "addCards", c, c1, c2);

        currentPlayer.setWaitingForInput(false);
        CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(currentPlayer), "updateWaitingForInput", currentPlayer.getBandit(), false);


        endOfTurn();
    }

    public Boolean getEndHorseAttack()
    {
        return this.endHorseAttack;
    }

    /*public void chosenStartinPostion(Position p)
    {
        playerPtrForStarting.setPosition(p);
        //TO ALL PLAYERS
        CommunicationAPI.sendMessageToClient(null, "moveGameUnit", playerPtrForStarting, p);
        p.getTrainCar().setHasAHorse(true);
        playerPtrForStarting.setWaitingForInput(false);

    }*/

    public void chosenHostage(Hostage aHostage)
    {
        //TODO send all messages 
        availableHostages.Remove(aHostage);
        currentPlayer.setCapturedHostage(aHostage);
        CommunicationAPI.sendMessageToClient(null, "updateHostageName", currentPlayer.getBandit(), aHostage.getHostageChar());


        if (aHostage.getHostageChar().Equals(HostageChar.Teacher))
        {
            currentPlayer.actionCantBePlayed(ActionKind.Punch);
            //TODO new message 
            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "actionCantBePlayed", ActionKind.Punch);
        }

        if (aHostage.getHostageChar().Equals(HostageChar.Zealot))
        {
            currentPlayer.actionCantBePlayed(ActionKind.Ride);
            //TODO new message 
            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "actionCantBePlayed", ActionKind.Ride);
        }

        if (aHostage.getHostageChar().Equals(HostageChar.PokerPlayer))
        {
            currentPlayer.setHasSpecialAbility(false);
            //TODO new message 
            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "specialAbilityDisabled");
        }

        currentPlayer.setWaitingForInput(false);
        CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateWaitingForInput", currentPlayer.getBandit(), false);
        
        this.endOfCards();

    }

    public void chosenPosition(Position p)
    {
        ActionCard topOfPile = this.currentRound.seeTopOfPlayedCards();

        //if the action card is a Move Marshall action
        if (topOfPile.getKind().Equals(ActionKind.Marshal))
        {
            this.aMarshal.setPosition(p);
            CommunicationAPI.sendMessageToClient(null, "moveGameUnit", this.aMarshal, p, myTrain.IndexOf(p.getTrainCar()));

            //check for all players at position p 
            foreach (Player aPlayer in p.getPlayers())
            {
                BulletCard b = new BulletCard(-1);
                aPlayer.addToDiscardPile(b);
                p.getTrainCar().moveRoofCar(aPlayer);
                CommunicationAPI.sendMessageToClient(null, "moveGameUnit", aPlayer, p.getTrainCar().getRoof(), myTrain.IndexOf(p.getTrainCar()));
            }

            currentPlayer.setWaitingForInput(false);
            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateWaitingForInput", currentPlayer.getBandit(), false);
            
            this.endOfCards();
        }
        //if the action card is a Move action
        else if (topOfPile.getKind().Equals(ActionKind.Move))
        {
            currentPlayer.setPosition(p);
            CommunicationAPI.sendMessageToClient(null, "moveGameUnit", currentPlayer, p, myTrain.IndexOf(p.getTrainCar()));
            
            bool flag = true;

            //if the marshal is at position p, bullet card in deck + sent to the roof 
            if (p.hasMarshal(aMarshal))
            {
                BulletCard b = new BulletCard(-1);
                currentPlayer.addToDiscardPile(b);
                p.getTrainCar().moveRoofCar(currentPlayer);
                //TO ALL PLAYERS
                CommunicationAPI.sendMessageToClient(null, "moveGameUnit", currentPlayer, p.getTrainCar().getRoof(), myTrain.IndexOf(p.getTrainCar()));
            }

            flag = shotgunCheck(p);

            if (flag)
            {
                currentPlayer.setWaitingForInput(false);
                CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateWaitingForInput", currentPlayer.getBandit(), false);
                
                this.endOfCards();
            }


        }

        else if (topOfPile.getKind().Equals(ActionKind.Ride))
        {
            currentPlayer.setPosition(p);
            currentPlayer.getPosition().getTrainCar().setHasAHorse(true);
            //TODO new message 
            CommunicationAPI.sendMessageToClient(null, "updateCarHasAHorse", p.getTrainCar(), true);
            currentPlayer.setOnAHorse(false);

            bool flag = true;

            CommunicationAPI.sendMessageToClient(null, "moveGameUnit", currentPlayer, p, myTrain.IndexOf(p.getTrainCar()));

            if (p.hasMarshal(aMarshal))
            {
                BulletCard b = new BulletCard(-1);
                currentPlayer.addToDiscardPile(b);

                p.getTrainCar().moveRoofCar(currentPlayer);
                CommunicationAPI.sendMessageToClient(null, "moveGameUnit", currentPlayer, p.getTrainCar().getRoof(), myTrain.IndexOf(p.getTrainCar()));
                this.endOfCards();
            }

            if (p.isInStageCoach(myStageCoach))
            {
                if (availableHostages.Count() != 0)
                {
                    CommunicationAPI.sendMessageToClient(null, "availableHostages", availableHostages);
                    CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateSelectHostage");
                }
            }
            else
            {
                flag = shotgunCheck(p);
                
                if (flag)
                {
                    currentPlayer.setWaitingForInput(false);
                    CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateWaitingForInput", currentPlayer.getBandit(), false);
                   
                    this.endOfCards();
                }

            }
        }

    }

    public void chosenPunchTarget(Player victim, GameItem loot, Position dest)
    {
        bool flag = true;
        
        if (currentPlayer.getBandit().Equals(Character.Cheyenne) && currentPlayer.getHasSpecialAbility() && loot.getType().Equals(ItemType.Purse))
        {
            victim.possessions.Remove(loot);
            CommunicationAPI.sendMessageToClient(null, "decrementLoot", victim.getBandit(), loot);
            currentPlayer.addToPossessions(loot);
            CommunicationAPI.sendMessageToClient(null, "incrementLoot", currentPlayer.getBandit(), loot);
        }
        else
        {
            //loot is removed from victim possessions, droped at it position
            victim.possessions.Remove(loot);
            CommunicationAPI.sendMessageToClient(null, "decrementLoot",victim.getBandit(), loot);
            loot.setPosition(victim.getPosition());
            CommunicationAPI.sendMessageToClient(null, "moveGameItem", loot, victim.getPosition(), myTrain.IndexOf(victim.getPosition().getTrainCar()));
        }

        //send victim to destination 
        victim.setPosition(dest);
        CommunicationAPI.sendMessageToClient(null, "moveGameUnit", victim, dest, myTrain.IndexOf(dest.getTrainCar()));

        //if the marshal is at position dest, victim: bullet card in deck + sent to the roof 
        if (dest.hasMarshal(aMarshal))
        {
            BulletCard b = new BulletCard(-1);
            victim.addToDiscardPile(b);
            dest.getTrainCar().moveRoofCar(victim);
            CommunicationAPI.sendMessageToClient(null, "moveGameUnit", victim, dest.getTrainCar().getRoof(), myTrain.IndexOf(dest.getTrainCar()));
        }

        flag = shotgunCheck(dest);

        if (flag){
            currentPlayer.setWaitingForInput(false);
            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(currentPlayer), "updateWaitingForInput", currentPlayer.getBandit(), false);
            this.endOfCards();
        }
    }

    public void choseToPunchShootgun()
    {
        aShotGun.setPosition(myStageCoach.getAdjacentCar().getRoof());
        CommunicationAPI.sendMessageToClient(null, "moveGameUnit", aShotGun, myStageCoach.getAdjacentCar().getRoof(), -1);
        
        aShotGun.hasBeenPunched();
        GameItem aStrongBox = new GameItem(ItemType.Strongbox, 1000);
        currentPlayer.addToPossessions(aStrongBox);
        CommunicationAPI.sendMessageToClient(null, "incrementLoot", currentPlayer.getBandit(), aStrongBox);

        this.currentPlayer.setWaitingForInput(false);
        CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(currentPlayer), "updateWaitingForInput", currentPlayer.getBandit(), false);

        endOfCards();

    }

    public void chosenShootTarget(Player target)
    {
        //A BulletCard is transfered from bullets of currentPlayer to target's discardPile
        BulletCard aBullet = currentPlayer.getABullet();
        target.addToDiscardPile(aBullet);

        this.currentPlayer.shootBullet();
        CommunicationAPI.sendMessageToClient(null, "decrementBullets", currentPlayer.getBandit(), this.currentPlayer.getNumOfBulletsShot());

        //if the bandit is Django
        if (currentPlayer.getBandit().Equals(Character.Django) && currentPlayer.getHasSpecialAbility())
        {

            TrainCar targetCar = target.getPosition().getTrainCar();
            TrainCar playerCar = currentPlayer.getPosition().getTrainCar();

            //if the target is in front of django, and is not in the locomotive, move target one car to the front.
            if (myTrain.IndexOf(targetCar) < myTrain.IndexOf(playerCar) && !(targetCar.Equals(myTrain[0])))
            {

                if (target.getPosition().isInside())
                {
                    Position pos = myTrain[myTrain.IndexOf(targetCar) - 1].getInside();
                    target.setPosition(pos);
                    CommunicationAPI.sendMessageToClient(null, "moveGameUnit", target, pos, myTrain.IndexOf(pos.getTrainCar()));
                }
                else
                {
                    Position pos = myTrain[myTrain.IndexOf(targetCar) - 1].getRoof();
                    target.setPosition(pos);
                    CommunicationAPI.sendMessageToClient(null, "moveGameUnit", target, pos, myTrain.IndexOf(pos.getTrainCar()));
                }

            }
            //if the target is at the back of django and is not in the cabosse, moved one car at the back.
            else if (myTrain.IndexOf(targetCar) > myTrain.IndexOf(playerCar) && !(targetCar.Equals(myTrain[myTrain.Count()])))
            {

                if (target.getPosition().isInside())
                {
                    Position pos = myTrain[myTrain.IndexOf(targetCar) + 1].getInside();
                    target.setPosition(pos);
                    CommunicationAPI.sendMessageToClient(null, "moveGameUnit", target, pos, myTrain.IndexOf(pos.getTrainCar()));
                }
                else
                {
                    Position pos = myTrain[myTrain.IndexOf(targetCar) + 1].getRoof();
                    target.setPosition(pos);
                    CommunicationAPI.sendMessageToClient(null, "moveGameUnit", target, pos, myTrain.IndexOf(pos.getTrainCar()));
                }
            }
        }

        currentPlayer.setWaitingForInput(false);
        CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(currentPlayer), "updateWaitingForInput", currentPlayer.getBandit(), false);
        this.endOfCards();
    }

    public void chosenLoot(GameItem loot)
    {
        //the loot is transfered from the position to the currentPlayer possensions
        //TODO check with Christina, because we remove the loot from the deck
        loot.setPosition(null);
        CommunicationAPI.sendMessageToClient(null, "moveGameItem", loot, null);

        if (loot is Whiskey){
            currentPlayer.addToPossessions(loot);
            CommunicationAPI.sendMessageToClient(null, "incremenWhiskey", currentPlayer, loot);
        }
        else {
            currentPlayer.addToPossessions(loot);
            CommunicationAPI.sendMessageToClient(null, "incremenLoot", currentPlayer, loot);
        }
        
        
        currentPlayer.setWaitingForInput(false);
        CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(currentPlayer), "updateWaitingForInput", currentPlayer.getBandit(), false);
        
        this.endOfCards();
    }

    private bool shotgunCheck(Position p)
    {
        //if the shotgun is at position p, bullet card in deck + sent to adjacent car (if 2 adjacent, send current player request)
        if (p.hasShotgun(aShotGun))
        {
            BulletCard b = new BulletCard(-1);
            currentPlayer.addToDiscardPile(b);

            //if the shotgun is on the last cabosse, player sent to adjacent car
            if (p.getTrainCar().Equals(myTrain[myTrain.Count() - 1]))
            {
                Position pos = myTrain[myTrain.Count() - 2].getRoof();
                currentPlayer.setPosition(pos);
                CommunicationAPI.sendMessageToClient(null, "moveGameUnit", currentPlayer, pos, myTrain.IndexOf(pos.getTrainCar()));
            }
            //if not, the player must choose between the 2 positions 
            else
            {
                List<Position> aL = new List<Position>();
                TrainCar currentTraincar = p.getTrainCar();

                Position p1 = myTrain[myTrain.IndexOf(currentTraincar) - 1].getRoof();
                Position p2 = myTrain[myTrain.IndexOf(currentTraincar) + 1].getRoof();

                aL.Add(p1);
                aL.Add(p2);

                if (!currentPlayer.getWaitingForInput())
                {
                    currentPlayer.setWaitingForInput(true);
                    CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(currentPlayer), "updateWaitingForInput", currentPlayer.getBandit(), true);
                }

                CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateMovePositions", aL);

                return false;
            }
        }
        return true;
    }

    public void readyForNextMove()
    {
        ActionCard top = this.currentRound.seeTopOfPlayedCards();
    
        Boolean waiting = true;

        foreach (Player p in this.players)
        {
            if (p.getWaitingForInput()) waiting = false;
        }

        if (waiting)
        {
            this.currentPlayer = top.belongsTo();
            CommunicationAPI.sendMessageToClient(null, "updateCurrentPlayer", players.IndexOf(currentPlayer));
            //TODO message about the card currently beeing played 

            // Get the top of the played cards from the schemin phase
            switch (top.getKind())
            {
                case ActionKind.Move:
                    {
                        List<Position> moves = this.getPossibleMoves(this.currentPlayer);

                        if (moves.Count > 1)
                        {
                            //TODO ask Christina if we need this 
                            this.aGameStatus = GameStatus.FinalizingCard;
                            CommunicationAPI.sendMessageToClient(null, "updateGameStatus", GameStatus.FinalizingCard);

                            this.currentPlayer.setWaitingForInput(true);
                            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(currentPlayer), "updateWaitingForInput", currentPlayer.getBandit(), true);

                            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateMovePositions", moves);
                        }
                        else
                        {
                            chosenPosition(moves[0]);
                        }
                        break;
                    }
                case ActionKind.ChangeFloor:
                    {
                        //If the player is inside a car, goes on the roof
                        if (this.currentPlayer.getPosition().isInside())
                        {
                            bool flag = true;
                            Position pos = this.currentPlayer.getPosition().getTrainCar().getRoof();
                            this.currentPlayer.getPosition().getTrainCar().moveRoofCar(this.currentPlayer);
                            CommunicationAPI.sendMessageToClient(null, "moveGameUnit", currentPlayer,pos, myTrain.IndexOf(pos.getTrainCar()));

                            flag = shotgunCheck(pos);

                            if (flag)
                            {
                                this.endOfCards();
                            }

                        }
                        //If the player is on the roof of a car, goes inside the car
                        else
                        {
                            this.currentPlayer.getPosition().getTrainCar().moveInsideCar(this.currentPlayer);
                            CommunicationAPI.sendMessageToClient(null, "moveGameUnit", currentPlayer, this.currentPlayer.getPosition().getTrainCar().getInside(), myTrain.IndexOf(currentPlayer.getPosition().getTrainCar()));

                            //if the marshal is at position p, bullet card in deck + sent to the roof 
                            if (currentPlayer.getPosition().getTrainCar().getInside().hasMarshal(aMarshal))
                            {
                                BulletCard b = new BulletCard(-1);
                                currentPlayer.addToDiscardPile(b);
                                currentPlayer.getPosition().getTrainCar().moveRoofCar(currentPlayer);
                                CommunicationAPI.sendMessageToClient(null, "moveGameUnit", currentPlayer, currentPlayer.getPosition().getTrainCar().getRoof(), myTrain.IndexOf(currentPlayer.getPosition().getTrainCar()));
                            }

                            //if he ends up inside the StageCoach, he chooses a hostage (if there are any left)
                            if (this.currentPlayer.getPosition().isInStageCoach(myStageCoach))
                            {
                                if (availableHostages.Count() != 0)
                                {
                                    this.aGameStatus = GameStatus.FinalizingCard;
                                    CommunicationAPI.sendMessageToClient(null, "updateGameStatus", GameStatus.FinalizingCard);
                                    this.currentPlayer.setWaitingForInput(true);
                                    CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(currentPlayer), "updateWaitingForInput", currentPlayer.getBandit(), true);
                                    //TODO new massage 
                                    CommunicationAPI.sendMessageToClient(null, "availableHostages", availableHostages);
                                    CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateSelectHostage");
                                }
                            }
                            else
                            {
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
                        else
                        {
                            this.aGameStatus = GameStatus.FinalizingCard;
                            CommunicationAPI.sendMessageToClient(null, "updateGameStatus", GameStatus.FinalizingCard);

                            this.currentPlayer.setWaitingForInput(true);
                            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(currentPlayer), "updateWaitingForInput", currentPlayer.getBandit(), true);

                            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updatePossTarget", possTargets);
                        }
                        break;
                    }
                case ActionKind.Rob:
                    {
                        List<GameItem> atLocation = this.currentPlayer.getPosition().getItems();
                        
                        this.aGameStatus = GameStatus.FinalizingCard;
                        CommunicationAPI.sendMessageToClient(null, "updateGameStatus", GameStatus.FinalizingCard);
                        
                        this.currentPlayer.setWaitingForInput(true);
                        CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(currentPlayer), "updateWaitingForInput", currentPlayer.getBandit(), true);

                        CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateLootAtLocation", currentPlayer.getPosition(), atLocation);
                        break;
                    }
                case ActionKind.Marshal:
                    {
                        List<Position> possPosition = this.aMarshal.getPossiblePositions();
                        if (possPosition.Count == 1)
                        {
                            this.chosenPosition(possPosition[0]);
                        }
                        else
                        {
                            this.aGameStatus = GameStatus.FinalizingCard;
                            CommunicationAPI.sendMessageToClient(null, "updateGameStatus", GameStatus.FinalizingCard);
                            this.currentPlayer.setWaitingForInput(true);
                            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(currentPlayer), "updateWaitingForInput", currentPlayer.getBandit(), true);

                            CommunicationAPI.sendMessageToClient(null, "updateMovePositions", possPosition);
                        }
                        break;
                    }
                case ActionKind.Punch:
                    {
                        List<Player> atLocation = this.currentPlayer.getPosition().getPlayers();
                        bool isShotgunHere = false;
                        if (aShotGun.getPosition().Equals(currentPlayer.getPosition()) && aShotGun.getIsOnStageCoach()) isShotgunHere = true;

                        //Removing belle from the list of player if there are other players. 
                        if (atLocation.Count() > 1)
                        {
                            foreach (Player player in atLocation)
                            {
                                if (player.getBandit().Equals(Character.Belle) && player.getHasSpecialAbility())
                                {
                                    atLocation.Remove(player);
                                }
                            }
                        }

                        //TODO add Shotgun if p is roof of stagecoach and hasBeenPunched is false


                        this.aGameStatus = GameStatus.FinalizingCard;
                        CommunicationAPI.sendMessageToClient(null, "updateGameStatus", GameStatus.FinalizingCard);
                        this.currentPlayer.setWaitingForInput(true);
                        CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(currentPlayer), "updateWaitingForInput", currentPlayer.getBandit(), true);

                        CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updatePossTargetPunch", atLocation, isShotgunHere);
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
                            CommunicationAPI.sendMessageToClient(null, "updateGameStatus", GameStatus.FinalizingCard);
                            this.currentPlayer.setWaitingForInput(true);
                            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(currentPlayer), "updateWaitingForInput", currentPlayer.getBandit(), true);

                            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateRidePositions", moves);

                            //setting the on a horse action to false.
                            currentPlayer.getPosition().getTrainCar().setHasAHorse(false);
                            CommunicationAPI.sendMessageToClient(null, "updateCarHasAHorse", currentPlayer.getPosition().getTrainCar(), false);

                        }
                        break;
                    }
            }
        }
    }

    public void chosenHorseAttackAction(string haAction)
    {

        // Update Horse Attack position object for current player
        AttackPosition hap = this.getHAFromCharacter(this.currentPlayer.getBandit());
        if (haAction.Equals("ride"))
        {
            // Increment position of horse and update all players
            if (!hap.incrementPosition())
            {
                this.horseAttackCounter++;
            }
        }
        else if (haAction.Equals("enter"))
        {
            // Set Off horse for current Player and update all the players
            hap.getOffHorse();
            this.horseAttackCounter++;
        }

        // Check if all players have chosen a position where to stop
        if (this.horseAttackCounter == this.totalPlayer)
        {
            this.endHorseAttack = true;


            // Set all the players' positions in the train
            foreach (Player p in this.players)
            {
                AttackPosition ap = this.getHAFromCharacter(p.getBandit());
                p.setPosition(this.myTrain[ap.getPosition()].getInside());
            }

            // Update the train for all the players
            CommunicationAPI.sendMessageToClient(null, "updateTrain", myTrain);

            // Set currentPlayer back to 0 and start the game
            // Set current player as next player
            this.currentPlayer = players[0];
            currentPlayer.setWaitingForInput(true);
            //Send the current player as index and value for waiting for input for that index/player
            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateWaitingForInput", currentPlayer.getBandit(), currentPlayer.getWaitingForInput());

            aGameStatus = GameStatus.Schemin;
            CommunicationAPI.sendMessageToClient(null, "updateGameStatus", aGameStatus);

        }
        else
        {
            // Update all the players
            CommunicationAPI.sendMessageToClient(null, "updateHorseAttack", this.attPos);

            currentPlayer.setWaitingForInput(false);
            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateWaitingForInput", currentPlayer.getBandit(), currentPlayer.getWaitingForInput());

            this.currentPlayer = this.players[(this.players.IndexOf(this.currentPlayer) + 1) % this.totalPlayer];
            CommunicationAPI.sendMessageToClient(null, "updateCurrentPlayer", currentPlayer);

            currentPlayer.setWaitingForInput(true);
            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateWaitingForInput", currentPlayer.getBandit(), currentPlayer.getWaitingForInput());
        }
    }


    /**
    *   Private helper methods
    */
    private AttackPosition getHAFromCharacter(Character c)
    {
        foreach (AttackPosition ha in this.attPos)
        {
            if (ha.GetCharacter() == c)
            {
                return ha;
            }
        }
        return null;
    }

    private void endOfTurn()
    {
        //if the player has another action, then the anotherAction flag is set to false

        if (this.currentPlayer.isGetsAnotherAction())
        {
            currentPlayer.setWaitingForInput(true);
            CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(currentPlayer), "updateWaitingForInput", currentPlayer.getBandit(), true);

            //TODO should we keep it ?
            //CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateHasAnotherAction", currentPlayerIndex, false);

            if (currentPlayer.getUsedOldWhiskey())
            {
                currentPlayer.setUsedOldWhiskey(false);
            }
            else
            {
                this.currentPlayer.setGetsAnotherAction(false);
            }

        }
        else
        {
            //if it is the last player of the turn, change turn, check if last turn in round
            if (currentPlayer.Equals(players[players.Count() - 1]) && currentTurn.getType() == TurnType.Standard || currentPlayer.Equals(players[0]) && currentTurn.getType() == TurnType.Switching)
            {
                // if last turn in round
                if (currentTurn.Equals(currentRound.getTurns()[currentRound.getTurns().Count() - 1]))
                {
                    //prepare for Stealing phase 
                    foreach (Player p in this.players)
                    {
                        p.moveCardsToDiscard();
                    }
                    this.aGameStatus = GameStatus.Stealin;
                    CommunicationAPI.sendMessageToClient(null, "updateGameStatus", aGameStatus);

                    readyForNextMove();
                }
                //else if not, we set the next turn in the round
                else
                {
                    //setting next turn in round
                    currentTurn = currentRound.getTurns()[currentRound.getTurns().IndexOf(currentTurn) + 1];
                    CommunicationAPI.sendMessageToClient(null, "updateCurrentTurn", currentRound.getTurns().IndexOf(currentTurn));

                    if (this.currentTurn.getType() == TurnType.Switching)
                    {
                        currentPlayer = players[players.Count() - 1];
                        CommunicationAPI.sendMessageToClient(null, "updateCurrentPlayer", currentPlayer.getBandit());
                    }
                    else
                    {
                        currentPlayer = players[0];
                        CommunicationAPI.sendMessageToClient(null, "updateCurrentPlayer", currentPlayer.getBandit());
                    }
                    //if the turn is Speeding up, the next player has another action 
                    if (this.currentTurn.getType() == TurnType.SpeedingUp)
                    {
                        this.currentPlayer.setGetsAnotherAction(true);

                    }

                }

            }
            //if this in not the last player in the turn, we update the currentplayer 
            else
            {
                //if the turn is Switching, order of players is reversed, so next player is previous in the list
                if (this.currentTurn.getType() == TurnType.Switching)
                {
                    this.currentPlayer = this.players[this.players.IndexOf(this.currentPlayer) - 1 % this.totalPlayer];
                    CommunicationAPI.sendMessageToClient(null, "updateCurrentPlayer", currentPlayer.getBandit());
                }
                //otherwise, it is the next player in the list 
                else
                {
                    this.currentPlayer = this.players[this.players.IndexOf(this.currentPlayer) + 1 % this.totalPlayer];
                    CommunicationAPI.sendMessageToClient(null, "updateCurrentPlayer", currentPlayer.getBandit());
                }

                //if the turn is Speeding up, the new next player has another action 
                if (this.currentTurn.getType() == TurnType.SpeedingUp)
                {
                    this.currentPlayer.setGetsAnotherAction(true);

                }
            }
        }
        currentPlayer.setWaitingForInput(true);
        CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(currentPlayer), "updateWaitingForInput", currentPlayer.getBandit(), true);
    }

    private void endOfCards()
    {
        Card c = this.currentRound.getTopOfPlayedCards();
        this.currentPlayer.addToDiscardPile(c);
        
        CommunicationAPI.sendMessageToClient(null, "removeTopCard");

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
                CommunicationAPI.sendMessageToClient(null, "updateCurrentRound", this.rounds.IndexOf(currentRound));

                this.currentTurn = this.currentRound.getTurns()[0];
                CommunicationAPI.sendMessageToClient(null, "updateCurrentTurn", this.currentRound.getTurns().IndexOf(currentTurn));

                //setting the next player and game status of the game 
                this.currentPlayer = this.players[0];
                CommunicationAPI.sendMessageToClient(null, "updateCurrentPlayer", currentPlayer.getBandit());

                this.aGameStatus = GameStatus.Schemin;
                CommunicationAPI.sendMessageToClient(null, "updateGameStatus", aGameStatus);

                this.currentPlayer.setWaitingForInput(true);
                CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updateWaitingForInput", this.currentPlayer.getBandit(), true);

                checkLadyPool();

                //for each player, getting 6 cards from their Pile at randomn and adding them to their hand 
                foreach (Player p in this.players)
                {
                    
                    List<Card> cardsToAdd = new List<Card>();
                    //int index = this.players.IndexOf(p);

                    Random rnd = new Random();
                    for (int i = 0; i < 6; i++)
                    {
                        //if p has Minister as hostage, then he only draw 5 cards 
                        if (p.getHostage().getHostageChar().Equals(HostageChar.Minister) && i == 5) break;

                        int rand = rnd.Next(0, p.discardPile.Count);
                        Card aCard = p.discardPile[rand];
                        p.hand.Add(aCard);
                        cardsToAdd.Add(aCard);
                        p.discardPile.Remove(aCard);
                    }

                    if (p.getBandit().Equals(Character.Doc) && p.getHasSpecialAbility())
                    {
                        int rand = rnd.Next(0, p.discardPile.Count);
                        Card aCard = p.discardPile[rand];
                        p.hand.Add(aCard);
                        cardsToAdd.Add(aCard);
                        p.discardPile.Remove(aCard);
                    }

                    CommunicationAPI.sendMessageToClient(MyTcpListener.getClientByPlayer(this.currentPlayer), "updatePlayerHand", currentPlayer.getBandit(), cardsToAdd);

                }

                //Stagecoach moves by one car to the back, if not at the cabosse. 
                TrainCar curAdjacent = myStageCoach.getAdjacentCar();

                if (!curAdjacent.Equals(myTrain[myTrain.Count() - 1]))
                {
                    TrainCar newAdjacent = myTrain[myTrain.IndexOf(curAdjacent) - 1];
                    myStageCoach.setAdjacentCar(newAdjacent);
                    CommunicationAPI.sendMessageToClient(null, "moveStageCoach", newAdjacent);

                    //if Shotgun is not on the stage coach, it also moves by one car to the back
                    if (!aShotGun.getIsOnStageCoach())
                    {
                        aShotGun.setPosition(newAdjacent.getRoof());
                        CommunicationAPI.sendMessageToClient(null, "moveGameUnit", aShotGun, newAdjacent.getRoof(), myTrain.IndexOf(newAdjacent));
                    }
                }

            }
        }

        readyForNextMove();
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

        EndOfRoundEvent ev = this.currentRound.getEvent();
        // Take care of Train station event
        switch (this.currentRound.getEvent()) {
            case MarshalsRevenge: {
                // Each bandit on the roof of the Marshal's car looses his least valuable purse
                foreach (Player b in this.aMarshal.getPosition().getPlayers()) {
                    scores[b] =scores[b] - b.getLeastPurseValue();
                }

                break;
            }
            case Pickpocketing: {
                // Each bandit that is alone takes a purse if available on his spot
                break;
            }
            case HostageConductor: {
                // +250 to all on locomotive
                break;
            }
            case SharingTheLoot: {
                // bandits who own a strongbox and are not alone share the value of it with neighbours
                break;
            }
            case Escape: {
                // Every bandit who is in train loses
                break;
            }
            case MortalBullet: {
                // Players loose 150 per bullet received during this round
                break;
            }   
        }
        
        //Sorted list to send to clients
        var myList = scores.ToList();
        myList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));


        //TO ALL PLAYERS
        CommunicationAPI.sendMessageToClient(null, "finalGameScore", myList);

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
        // Variables to get random round layouts
        List<int> usedRounds = new List<>();
        Random r = new Random();
        int rand;

        // Initialize random unique layouts for 4 normal rounds
        for (int i = 0; i < 4; i++)
        {
            // Look for a random integer between 0 and 6 which has not been used before
            rand = r.Next(0, 12);
            while (true) {
                if (!usedRounds.Contains(rand)) {
                    break;
                } 
                rand = r.Next(0, 12);
            }
            usedRounds.Add(rand);
            Round aRound = new Round(false, totalPlayer);
            aRound.intializeTurn(this.totalPlayer, rand);
            this.rounds.Add(aRound);
        }

        // Initialize random layout for final round
        rand = r.Next(0, 6);
        Round aFinalRound = new Round(true, totalPlayer);
        aFinalRound.intializeTurn(this.totalPlayer, rand);
        this.rounds.Add(aFinalRound);
    }

    private List<Position> getPossibleMoves(Player p)
    {
        List<Position> possPos = new List<Position>();
        TrainCar playerCar = p.getPosition().getTrainCar();

        //if the player is on a horse 
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
                catch (System.IndexOutOfRangeException e)
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
                catch (System.IndexOutOfRangeException e)
                {
                    continue;
                }
            }

        }
        //if the player is inside or on the roof of the stagecoach
        else if (p.getPosition().isInStageCoach(myStageCoach))
        {
            if (p.getPosition().isInside())
            {
                return null;
            }
            else
            {
                TrainCar adjacentCar = myStageCoach.getAdjacentCar();

                // Add 1-2 distance forward or backwards
                for (int i = 1; i < 3; i++)
                {
                    try
                    {
                        // Add adjacent positions
                        possPos.Add(this.myTrain[this.myTrain.IndexOf(adjacentCar) - i].getRoof());
                    }
                    catch (System.IndexOutOfRangeException e)
                    {
                        continue;
                    }
                }

                for (int i = 1; i < 3; i++)
                {
                    try
                    {
                        // Add adjacent positions
                        possPos.Add(this.myTrain[this.myTrain.IndexOf(adjacentCar) + i].getRoof());
                    }
                    catch (System.IndexOutOfRangeException e)
                    {
                        continue;
                    }
                }

            }
        }
        //if the player is on the roof of a Car
        else if (!p.getPosition().isInside())
        {
            //if the playerCar is adjacent to the stageCoach, add roof of the stage coach 
            if (myStageCoach.getAdjacentCar().Equals(playerCar))
            {
                possPos.Add(myStageCoach.getRoof());
            }

            //if the current Player has Old Lady as Hostage, can only move one car at a time on the roof
            if (currentPlayer.getHostage().getHostageChar().Equals(HostageChar.OldLady))
            {
                try
                {
                    TrainCar wagon = this.myTrain[this.myTrain.IndexOf(playerCar) - 1];
                    // Add adjacent positions
                    possPos.Add(wagon.getRoof());

                }
                catch (System.IndexOutOfRangeException e)
                {

                }

                try
                {
                    // Add adjacent positions
                    possPos.Add(this.myTrain[this.myTrain.IndexOf(playerCar) + 1].getRoof());
                }
                catch (System.IndexOutOfRangeException e)
                {

                }
            }
            //otherwise, he can move 3 cars 
            else
            {

                // Add 1-3 distance forward or backwards
                for (int i = 1; i < 4; i++)
                {
                    try
                    {
                        // Add adjacent positions
                        possPos.Add(this.myTrain[this.myTrain.IndexOf(playerCar) - i].getRoof());
                    }
                    catch (System.IndexOutOfRangeException e)
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
                    catch (System.IndexOutOfRangeException e)
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

        }
        //if the player is inside a Car
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
            catch (System.IndexOutOfRangeException e)
            {

            }

            try
            {
                // Add adjacent positions
                possPos.Add(this.myTrain[this.myTrain.IndexOf(playerCar) + 1].getInside());
            }
            catch (System.IndexOutOfRangeException e)
            {

            }

        }

        return possPos;
    }

    //add a bullet to the player's discard pile that have LadyPool as hostage 
    private void checkLadyPool()
    {
        foreach (Player p in players)
        {
            if (p.getHostage().getHostageChar().Equals(HostageChar.LadyPoodle))
            {
                p.addToDiscardPile(new BulletCard(-1));
                break;
            }
        }
    }

    public Boolean getEndOfGame()
    {
        return endOfGame;
    }

    private List<Player> getPossibleShootTarget(Player p)
    {
        List<Player> possPlayers = new List<Player>();
        TrainCar playerCar = p.getPosition().getTrainCar();

        //If player is on the roof of the stagecoach 
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
        //If player is inside the stagecoach 
        else if (myStageCoach.getInside().Equals(p.getPosition()))
        {
            Position inSC = myStageCoach.getAdjacentCar().getInside();
            List<Player> playersOnWagon = inSC.getPlayers();
            if (playersOnWagon.Count != 0)
            {
                possPlayers.AddRange(playersOnWagon);
            }
        }
        //otherwise, if player is on the roof of a car
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

                // If the shotgun is in the line of sight, then he blocks it
                if (!aShotGun.getIsOnStageCoach())
                {
                    if (this.myTrain[i].getRoof().hasShotgun(aShotGun))
                    {
                        break;
                    }
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
                // If the shotgun is in the line of sight, then he blocks it
                if (!aShotGun.getIsOnStageCoach())
                {
                    if (this.myTrain[i].getRoof().hasShotgun(aShotGun))
                    {
                        break;
                    }
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

            //If player is tuco, then he can shoot on the roof of the same wagon 
            if (currentPlayer.getBandit().Equals(Character.Tuco) && currentPlayer.getHasSpecialAbility())
            {
                List<Player> playersInside = p.getPosition().getTrainCar().getInside().getPlayers();
                if (playersInside.Count != 0)
                {
                    possPlayers.AddRange(playersInside);
                }
            }
        }
        //else if player is inside a car 
        else
        {
            // Look for the players in the next wagon backwards
            try
            {
                // Add adjacent positions
                possPlayers.AddRange(this.myTrain[this.myTrain.IndexOf(playerCar) - 1].getInside().getPlayers());
            }
            catch (System.IndexOutOfRangeException e)
            {

            }

            // Loof for the players in the next wagon forward
            try
            {
                // Add adjacent positions
                possPlayers.AddRange(this.myTrain[this.myTrain.IndexOf(playerCar) + 1].getInside().getPlayers());
            }
            catch (System.IndexOutOfRangeException e)
            {

            }
            //Look for player inside the stageCoach, if the playerCar is adjacent to the stageCoach 
            if (myStageCoach.getAdjacentCar().Equals(playerCar))
            {

                List<Player> playersOnWagon3 = this.myStageCoach.getInside().getPlayers();
                if (playersOnWagon3.Count != 0)
                {
                    possPlayers.AddRange(playersOnWagon3);
                }
            }

            //If player is tuco, then he can shoot inside the same wagon 
            if (currentPlayer.getBandit().Equals(Character.Tuco) && currentPlayer.getHasSpecialAbility())
            {
                List<Player> playersOnTheRoof = p.getPosition().getTrainCar().getRoof().getPlayers();
                if (playersOnTheRoof.Count != 0)
                {
                    possPlayers.AddRange(playersOnTheRoof);
                }
            }
        }

        // If there is more than one possible player, we remove Belle.
        if (possPlayers.Count > 1)
        {
            foreach (Player pl in possPlayers)
            {
                if (pl.getBandit().Equals(Character.Belle) && pl.getHasSpecialAbility())
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
        return (ActionCard)al[index];
    }


}

