using Newtonsoft.Json;
using PositionSpace;

namespace GameUnitSpace
{
    abstract class GameUnit
    {
        [JsonProperty]
        private Position aPosition;
        public void setPosition(Position pPosition){
            this.aPosition.removeUnit(this);
            aPosition = pPosition; 
            this.aPosition.addUnit(this);
        }

         public Position getPosition(){
            return aPosition;
        }
    }
}
