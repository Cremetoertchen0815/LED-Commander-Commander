using LED_Commander_Commander.RawData;

namespace LED_Commander_Commander.ManagedData;
public sealed class SceneFixtureData
{
    private readonly byte?[] _fixtureData = new byte?[10];

    public byte? Fun { get => _fixtureData[0]; set => _fixtureData[0] = value; }
    public byte? Red { get => _fixtureData[1]; set => _fixtureData[1] = value; }
    public byte? Green { get => _fixtureData[2]; set => _fixtureData[2] = value; }
    public byte? Blue { get => _fixtureData[3]; set => _fixtureData[3] = value; }
    public byte? Speed { get => _fixtureData[4]; set => _fixtureData[4] = value; }
    public byte? Color { get => _fixtureData[5]; set => _fixtureData[5] = value; }
    public byte? Strobe { get => _fixtureData[6]; set => _fixtureData[6] = value; }
    public byte? Dimmer { get => _fixtureData[7]; set => _fixtureData[7] = value; }
    public byte? Pan { get => _fixtureData[8]; set => _fixtureData[8] = value; }
    public byte? Tilt { get => _fixtureData[9]; set => _fixtureData[9] = value; }

    public byte? GetChannelData(FixtureChannels channel) => _fixtureData[(int)channel];
    public void SetChannelData(FixtureChannels channel, byte? value) => _fixtureData[(int)channel] = value;

    //Explicit cast to byte array possible
    public static explicit operator byte?[](SceneFixtureData source) => source._fixtureData; 

    public void CopyFromRaw(RawData.FixtureChannels data)
    {
        Fun = data.EnabledChannels.HasFlag(ChannelType.FUN) ? data.Fun : null;
        Red = data.EnabledChannels.HasFlag(ChannelType.RED) ? data.Red : null;
        Green = data.EnabledChannels.HasFlag(ChannelType.GREEN) ? data.Green : null;
        Blue = data.EnabledChannels.HasFlag(ChannelType.BLUE) ? data.Blue : null;
        Speed = data.EnabledChannels.HasFlag(ChannelType.SPEED) ? data.Speed : null;
        Color = data.EnabledChannels.HasFlag(ChannelType.COLOR) ? data.Color : null;
        Strobe = data.EnabledChannels.HasFlag(ChannelType.STROBE) ? data.Strobe : null;
        Dimmer = data.EnabledChannels.HasFlag(ChannelType.DIMMER) ? data.Dimmer : null;
        Pan = data.EnabledChannels.HasFlag(ChannelType.PAN) ? data.Pan : null;
        Tilt = data.EnabledChannels.HasFlag(ChannelType.TILT) ? data.Tilt : null;
    }
}
