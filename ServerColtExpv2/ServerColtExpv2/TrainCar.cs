using System.Collections.Generic;
using GameUnitSpace;
using Newtonsoft.Json;

namespace PositionSpace
{
    class TrainCar
    {
        //[JsonProperty]
        private readonly bool isLocomotive;
        [JsonProperty]
        private Position inside;
        [JsonProperty]
        private Position roof;
        private bool hasAHorse;
        public TrainCar(bool isLocomotive)
        {
            this.inside = new Position(this, Floor.Inside);
            this.roof = new Position(this, Floor.Roof);
            this.isLocomotive = isLocomotive;
            hasAHorse = false;
        }

        public Position getInside()
        {
            return this.inside;
        }

        public Position getRoof()
        {
            return this.roof;
        }

        // Move input GameUnit inside the car 
        public void moveInsideCar(GameUnit fig)
        {
            fig.setPosition(inside);
            // this.inside.addUnit(fig); **should be handled in setPosition
        }

        // Move input GameUnit inside the car 
        public void moveRoofCar(GameUnit fig)
        {
            fig.setPosition(roof);
            // this.roof.addUnit(fig); **should be handled in setPosition
        }

        // Inititialize the car's item at the beginning of the game
        // **replaces Position's "initializeRandomLayout"
        public void initializeItems(HashSet<GameItem> items)
        {
            foreach (GameItem item in items)
            {
                this.inside.addUnit(item);
            }
        }

        public void setHasAHorse(bool b){
            hasAHorse = b;
        }

        public bool hasHorseAtCarLevel(){
            return hasAHorse;
        }
    }

}
