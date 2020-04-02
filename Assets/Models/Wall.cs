
public class Wall : InstalledObject {

    public Wall() {
        CreatePrototype(this, 0, true, 1, 1);
    }
    
    protected override InstalledObject GenerateNewInstalledObject() {
        return new Wall();
    }
}
