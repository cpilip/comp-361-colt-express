using System;
using System.Collections.Generic;
using GameUnitSpace;
using Newtonsoft.Json;

namespace PositionSpace
{
    public enum Floor
    {
        Inside,
        Roof
    }
    class Position
    {
        [JsonProperty]
        private readonly Floor floor; // **Did not implement "setFloor"; Floor passed in constructor
        [JsonProperty]
        private readonly TrainCar trainCar;
        [JsonProperty]
        private HashSet<GameUnit> units = new HashSet<GameUnit>();

        public Position(TrainCar trainCar, Floor floor)
        {
            this.trainCar = trainCar;
            this.floor = floor;
        }

        public TrainCar getTrainCar()
        {
            return this.trainCar;
        }

        // Returns true if the position is a floor; false if roof
        public Boolean isInside() {
            return this.floor == Floor.Inside;
        }

        // Add a unit to the Position's GameUnit HashSet
        public void addUnit(GameUnit unit)
        {
            this.units.Add(unit);
        }

        // Remove a unit from the Position's GameUnit HashSet
        public void removeUnit(GameUnit unit)
        {
            this.units.Remove(unit);
        }

        public List<Player> getPlayers()
        {
            List<Player> players = new List<Player>();
            foreach (GameUnit unit in units) { 
                if (unit.GetType().Equals(typeof(Player))) {
                    players.Add((Player) unit);
                }
            }
            return players;
        }

        public List<GameItem> getItems() {
            List<GameItem> items = new List<GameItem>();
            foreach (GameUnit unit in units) { 
                if (unit.GetType().Equals(typeof(GameItem))) {
                    items.Add((GameItem) unit);
                }
            }
            return items;
        }

        public Boolean hasMarshal(Marshal m) {
            return units.Contains(m);
        }
    }
}