using System.Collections.Generic;
using GameUnitSpace;
using Newtonsoft.Json;

namespace PositionSpace
{
    class TrainCar
    {
        //[JsonProperty]
        private bool isLocomotive { get; set; }
        [JsonProperty]
        private Position inside { get; set; }
        [JsonProperty]
        private Position roof { get; set; }

        public TrainCar(bool isLocomotive)
        {
            this.inside = new Position(this, Floor.Inside);
            this.roof = new Position(this, Floor.Roof);
            this.isLocomotive = isLocomotive;
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
    }

}
