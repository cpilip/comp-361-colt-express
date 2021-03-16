
using Newtonsoft.Json;
using PositionSpace;

namespace GameUnitSpace
{
    [JsonConverter(typeof(BaseConverter))]
    abstract class GameUnit
    {
        //[JsonProperty]
        private Position aPosition;
        public void setPosition(Position pPosition){
            if (this.aPosition != null)
            {
                this.aPosition.removeUnit(this);
            }

            aPosition = pPosition; 
            this.aPosition.addUnit(this);
        }

         public Position getPosition(){
            return aPosition;
        }
    }
}
