using System.Collections.Generic;

namespace Position {
    public enum Floor {
        Inside,
        Roof
    }
    class Position {
        private Floor floor;
        private TrainCar trainCar;

        public HashSet<GameUnit> units = new HashSet<GameUnit>();

        public Position(TrainCar trainCar, Floor floor) {
            this.trainCar = trainCar;
            this.floor = floor;
        }

        public Floor isFloor() {
            return this.floor;
        }

        public void setFloor(Floor floor) {
            this.floor = floor;
        }


    }
}