class TrainCar {
    private bool isLocomotive;

    private Position.Position inside;
    private Position.Position roof;

    public TrainCar() {
        this.inside = new Position.Position(this, Position.Floor.Inside);
        this.roof = new Position.Position(this, Position.Floor.Roof);
    }

    public void moveInsideCar(GameUnit fig) {
        fig.setPosition(inside);
    }

    public void moveRoofCar(GameUnit fig) {
        fig.setPosition(roof);
    }
}
