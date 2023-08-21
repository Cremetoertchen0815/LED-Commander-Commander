using System.IO;

namespace LED_Commander_Commander.RawData;
public struct RawFixtureChannels
{
    public byte Fun;
    public byte Red;
    public byte Green;
    public byte Blue;
    public byte Speed;
    public byte Color;
    public byte Strobe;
    public byte Dimmer;
    public byte Pan;
    public byte Tilt;

    public ChannelType EnabledChannels;

    public void ReadIn(BinaryReader br)
    {
        Fun = br.ReadByte();
        Red = br.ReadByte();
        Green = br.ReadByte(); 
        Blue = br.ReadByte(); 
        Speed = br.ReadByte(); 
        Color = br.ReadByte(); 
        Strobe = br.ReadByte(); 
        Dimmer = br.ReadByte(); 
        Pan = br.ReadByte(); 
        Tilt = br.ReadByte();
    }

    public void WriteOut(BinaryWriter bw)
    {
        bw.Write(Fun);
        bw.Write(Red);
        bw.Write(Green);
        bw.Write(Blue);
        bw.Write(Speed);
        bw.Write(Color);
        bw.Write(Strobe);
        bw.Write(Dimmer);
        bw.Write(Pan);
        bw.Write(Tilt);
    }
}
