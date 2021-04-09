
using Newtonsoft.Json;
using PositionSpace;

namespace GameUnitSpace
{
    abstract class GameUnit
    {

        [JsonIgnore]
        private Position aPosition;
        public void setPosition(Position pPosition){
            if (this.aPosition != null)
            {
                this.aPosition.removeUnit(this);
            }

            aPosition = pPosition; 

            if(pPosition != null)
            {

                this.aPosition.addUnit(this);
            }
        }

         public Position getPosition(){
            return aPosition;
        }
    }
}
