using System.Linq;

namespace LED_Commander_Commander.ManagedData;
public class Scene
{
    private readonly SceneFixtureData[] _fixtureData = Enumerable.Range(0, 16).Select(_ => new SceneFixtureData()).ToArray();

    public bool Aux1 { get; set; }
    public bool Aux2 { get; set; }

    public SceneFixtureData GetFixtureData(int fixtureNumber) => _fixtureData[fixtureNumber];
}
