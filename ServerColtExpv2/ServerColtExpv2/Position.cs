using System.Collections.Generic;

namespace PositionSpace {
    public enum Floor {
        Inside,
        Roof
    }
    class Position {
        private readonly Floor floor; // **Did not implement "setFloor"; Floor passed in constructor
        private readonly TrainCar trainCar;

        private HashSet<GameUnit> units = new HashSet<GameUnit>();

        public Position(TrainCar trainCar, Floor floor) {
            this.trainCar = trainCar;
            this.floor = floor;
        }

        public TrainCar getTrainCar() { 
            return this.trainCar;
        }

        // Returns true if the position is a floor; false if roof
        public bool isInside() {
            return this.floor == Floor.Inside;
        }

        // Add a unit to the Position's GameUnit HashSet
        public void addUnit(GameUnit unit) {
            this.units.Add(unit);
        }

        // Remove a unit from the Position's GameUnit HashSet
        public void removeUnit(GameUnit unit) {
            this.units.Remove(unit);
        }

        public List<Player> getPlayers() {
            List<Player> players = new List<Player>();
            for (GameUnit unit in units) { 
                if (unit.getType().Equals(typeof(Player))) {
                    players.Add(unit);
                }
            }
            return players;
        }

        public List<Player> getItems() {
            List<Player> players = new List<Player>();
            for (GameUnit unit in units) { 
                if (unit.getType().Equals(typeof(GameItem))) {
                    players.Add(unit);
                }
            }
            return players;
        }

        public boolean hasMarshal(Marshal m) {
            return units.Contains(m);
        }
    }
}