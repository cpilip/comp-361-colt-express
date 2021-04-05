
namespace GameUnitSpace {

    public enum WhiskeyKind{
        Old,
        Normal
    }

    public enum WhiskeyStatus{
        Full,
        Half,
        Empty
    }
    class Whiskey : GameItem {

        private readonly WhiskeyKind aKind;
        private WhiskeyStatus aStatus;

        public Whiskey (WhiskeyKind pKind) : base (ItemType.Whiskey, 0){
            aKind = pKind;
            aStatus = WhiskeyStatus.Full;
        }

        public WhiskeyKind getWhiskeyKind(){
            return aKind;
        }

        public WhiskeyStatus getWhiskeyStatus(){
            return aStatus;
        }

        public void drinkASip(){
            if(aStatus == WhiskeyStatus.Full){
                aStatus = WhiskeyStatus.Half;
            }
            else if (aStatus == WhiskeyStatus.Half){
                aStatus = WhiskeyStatus.Empty;
            }
        }

        public bool isEmpty(){
            if(this.aStatus.Equals(WhiskeyStatus.Empty)){
                return true;
            }
            return false;
        }




    }



}