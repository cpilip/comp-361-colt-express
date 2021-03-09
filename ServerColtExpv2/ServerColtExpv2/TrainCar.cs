using System.Collections.Generic;
class TrainCar {
    private readonly bool isLocomotive;
    private Position.Position inside;
    private Position.Position roof;

    public TrainCar(bool isLocomotive) {
        this.inside = new Position.Position(this, Position.Floor.Inside);
        this.roof = new Position.Position(this, Position.Floor.Roof);
        this.isLocomotive = isLocomotive;
    }

    // Move input GameUnit inside the car 
    public void moveInsideCar(GameUnit fig) {
        fig.setPosition(inside);
        // this.inside.addUnit(fig); **should be handled in setPosition
    }

    // Move input GameUnit inside the car 
    public void moveRoofCar(GameUnit fig) {
        fig.setPosition(roof);
        // this.roof.addUnit(fig); **should be handled in setPosition
    }

    // Inititialize the car's item at the beginning of the game
    // **replaces Position's "initializeRandomLayout"
    public void initializeItems(HashSet<GameItem> items) {
        foreach (GameItem item in items) {
            this.inside.addUnit(item);
        }
    }
}
