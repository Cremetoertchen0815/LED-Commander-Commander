using LED_Commander_Commander.RawData;
using static System.Formats.Asn1.AsnWriter;

namespace LED_Commander_Commander.ManagedData;
public class SaveFile
{
    public string[] ChannelNames { get; init; } = new string[12];
    public FixtureSettings[] Fixtures { get; init; } = new FixtureSettings[16];
    public Scene?[] Scenes { get; init; } = new Scene?[16];
    public Chaser?[] Chasers { get; init; } = new Chaser?[16];

    public void LoadFromFile(string filename)
    {
        var rawData = RawDeviceData.LoadFromFile(filename);
        //Transfer channel names
        rawData.ChannelNames.CopyTo(ChannelNames, 0);
        //Transfer dimmer settings
        for (int f = 0; f < 16; f++)
        {
            var fixture = new FixtureSettings();
            fixture.IsDimmerActivated = rawData.DimmerSet[f];
            for (int i = 0; i < rawData.DimmedChannels[f].Length; i++) fixture.DimmedChannels |= rawData.DimmedChannels[f][i] ? (ChannelType)i : 0;
            Fixtures[f] = fixture;
        }
        //Transfer patch map
        for (int i = 0; i < rawData.PatchMap.Length; i++)
        {
            var value = rawData.PatchMap[i];
            if (value > 159) continue;
            Fixtures[value / 10].GetAddressesPatchedToChannel((ChannelType)(value % 10)).Add((ushort)i);
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
        //Transfer chaser
        for (int c = 0; c < 16; c++)
        {
            var length = rawData.ChaserLength[c];
            if (length < 1)
            {
                Chasers[c] = null;
                continue;
            }

            var chaser = new Chaser();
            for (int s = 0; s < length; s++)
            {
                var index = rawData.ChaserOrder[c, s];
                var stepData = rawData.ChaserSteps[index];

                if (stepData.IsEmpty) continue;

                var step = new Scene();
                step.Aux1 = stepData.Aux1;
                step.Aux2 = stepData.Aux2;
                for (int f = 0; f < 16; f++) step.GetFixtureData(f).CopyFromRaw(stepData.FixtureChannels[f]);
                chaser.Steps.Add(step);
            }
            Chasers[c] = chaser;
        }
    }

    public void SaveToFile(string filename)
    {
        var rawData = RawDeviceData.Empty;

        //Transfer channel names
        rawData.ChannelNames = ChannelNames;

        //Transfer dimmer settings & patch map
        rawData.DimmerSet = new bool[16];
        rawData.DimmedChannels = new bool[16][];
        for (int f = 0; f < 16; f++)
        {
            var fixture = Fixtures[f];
            rawData.DimmerSet[f] = fixture.IsDimmerActivated;
            rawData.DimmedChannels[f] = new bool[10];
            for (int i = 0; i < 10; i++) rawData.DimmedChannels[f][i] = fixture.DimmedChannels.HasFlag((ChannelType)i);

            for (int ch = 0; ch < RawDeviceData.CHANNEL_COUNT_WITH_AUX; ch++)
            {
                var patchData = fixture.GetAddressesPatchedToChannel((ChannelType)ch);
                for (int i = 0; i < patchData.Count; i++) rawData.PatchMap[patchData[i]] = (byte)(f * RawDeviceData.CHANNEL_COUNT_WITHOUT_AUX + ch);
            }
        }

        //Transfer scenes
        for (int s = 0; s < 16; s++)
        {
            var scene = Scenes[s];
            var rawScene = RawSceneMapEntry.Empty;

            if (scene is not null)
            {
                rawScene.IsEmpty = false;
                rawScene.Aux1 = scene.Aux1;
                rawScene.Aux2 = scene.Aux2;
                for (int f = 0; f < 16; f++) scene.GetFixtureData(f).CopyToRaw(ref rawScene.FixtureChannels[f]);
            }
        }


        //Transfer chasers
        int totalStepIdx = 0;
        for (int s = 0; s < 16; s++)
        {
            var chaser = Chasers[s];
            if (chaser is null || chaser.Steps.Count < 1) continue;

            rawData.ChaserLength[s] = (byte)chaser.Steps.Count;
            for (int st = 0; st < chaser.Steps.Count; st++)
            {
                var step = chaser.Steps[st];
                var rawStep = RawSceneMapEntry.Empty;
                rawStep.IsEmpty = false;
                rawStep.Aux1 = step.Aux1;
                rawStep.Aux2 = step.Aux2;
                for (int f = 0; f < 16; f++) step.GetFixtureData(f).CopyToRaw(ref rawStep.FixtureChannels[f]);

                rawData.ChaserSteps[totalStepIdx] = rawStep;
                rawData.ChaserOrder[s, st] = (ushort)totalStepIdx++;
            }
        }
    }
}
