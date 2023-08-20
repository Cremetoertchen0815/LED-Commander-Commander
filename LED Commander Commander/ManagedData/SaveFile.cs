using LED_Commander_Commander.RawData;
using System.ComponentModel.DataAnnotations;

namespace LED_Commander_Commander.ManagedData;
public class SaveFile
{
    public string[] ChannelNames { get; init; } = new string[12];
    public FixtureSettings[] Fixtures { get; init; } = new FixtureSettings[16];
    public Scene?[] Scenes { get; init; } = new Scene?[16];
    public Chaser?[] Chasers { get; init; } = new Chaser?[16];

    public void LoadFromFile(string filename)
    {
        var rawData = DeviceSettings.LoadFromFile(filename);
        //Transfer channel names
        rawData.ChannelNames.CopyTo(ChannelNames, 0);
        //Transfer dimmer settings
        for (int f = 0; f < 16; f++)
        {
            var fixture = new FixtureSettings();
            fixture.IsDimmerActivated = rawData.DimmerSet[f];
            for (int i = 0; i < rawData.DimmedChannels[f].Length; i++) fixture.DimmedChannels |= rawData.DimmedChannels[f][i] ? (FixtureChannels)i : 0;
            Fixtures[f] = fixture;
        }
        //Transfer patch map
        for (int i = 0; i < rawData.PatchMap.Length; i++)
        {
            if (i > 159) continue;
            Fixtures[i / 10].GetAddressesPatchedToChannel((FixtureChannels)(i % 10)).Add((ushort)i);
        }
        //Transfer scenes
        for (int s = 0; s < 16; s++)
        {
            var scData = rawData.Scenes[s];
            if (scData.IsEmpty)
            {
                Scenes[s] = null;
                continue;
            }

            var scene = new Scene();
            scene.Aux1 = scData.Aux1;
            scene.Aux2 = scData.Aux2;
            for (int f = 0; f < 16; f++) scene.GetFixtureData(f).CopyFromRaw(scData.FixtureChannels[f]);
            Scenes[s] = scene;
        }
    }
}
