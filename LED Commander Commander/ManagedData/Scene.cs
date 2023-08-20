namespace LED_Commander_Commander.ManagedData;
public class Scene
{
    private readonly SceneFixtureData[] _fixtureData = new SceneFixtureData[16];

    public bool Aux1 { get; set; }
    public bool Aux2 { get; set; }

    public SceneFixtureData GetFixtureData(int fixtureNumber) => _fixtureData[fixtureNumber];
}
