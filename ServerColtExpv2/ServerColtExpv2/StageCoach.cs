
namespace PositionSpace{


    class StageCoach : TrainCar {

        private TrainCar adjacentCar;

        public StageCoach (bool isLocomotive, TrainCar aCar) : base (isLocomotive)
        {
            adjacentCar = aCar;
        }

        public TrainCar getAdjacentCar(){
            return adjacentCar;
        }

        public void setAdjacentCar(TrainCar aTC){
            adjacentCar = aTC;
        }

    }
}