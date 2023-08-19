﻿using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace LED_Commander_Commander.RawData;
public struct DeviceSettings
{
    public const int LEAD_OUT_LENGTH = 0x157C3;
    public const int CHANNEL_NAME_CHARS = 0x7;
    public const int CHANNEL_COUNT = 0xC;
    public const int SCENES_COUNT_REGULAR = 0x10;
    public const int SCENES_COUNT_CHASER = 0x7D0;
    public const int DMX_CHANNELS = 0x200;
    public const int CHASER_COUNT = 0x10;
    public const int CHASER_STEPS = 0x7D0;
    public const int PADDING_SIZE = 0x200;
    public const string IDENTIFIER_A = "acme\0";

    public string Header; //should be succeeded
    public SceneMapEntry[] Scenes; //Maps to the 16 scenes
    public SceneMapEntry[] ChaserSteps; //Contains slots for 2008 chaser steps in unspecified order
    public string[] ChannelNames; //Describes the name of every channel(FUN, Red, Green, etc.)
    public byte[] PatchMap; //0x200 Bytes, Value correlated to patched channel = 0x0A * fixture + channel type
    public byte[] ChaserLength; //x16, 3 Bytes each, xx 00 05, just take first
    public ushort[,] ChaserOrder; //x16 - 2000 UShort steps, describes index of chaser steps
    public byte[] TwoWeirdBytes; //Two bytes I don't know what it does
    public bool[] DimmerSet; //0x10 Byte bools
    public bool[][] DimmedChannels; //0x10 channels x16 bools, indicating which channels are dimmed and which are not

    public static DeviceSettings LoadFromFile(string filename)
    {
        var fileStream = File.OpenRead(filename);
        var br = new BinaryReader(fileStream, System.Text.Encoding.ASCII);
        
        var settings = new DeviceSettings();
        settings.Header = new string(br.ReadChars(PADDING_SIZE)).Replace("\0", string.Empty);
        //Read scenes and chaser steps
        settings.Scenes = new SceneMapEntry[SCENES_COUNT_REGULAR];
        for (int i = 0; i < SCENES_COUNT_REGULAR; i++) settings.Scenes[i] = SceneMapEntry.ReadScene(br);
        settings.ChaserSteps = new SceneMapEntry[SCENES_COUNT_CHASER];
        for (int i = 0; i < SCENES_COUNT_CHASER; i++) settings.ChaserSteps[i] = SceneMapEntry.ReadScene(br);
        //Read channel names and patching
        settings.ChannelNames = new string[CHANNEL_COUNT];
        for (int i = 0; i < CHANNEL_COUNT; i++) settings.ChannelNames[i] = new string(br.ReadChars(CHANNEL_NAME_CHARS)).Replace("\0", string.Empty);
        settings.PatchMap = br.ReadBytes(DMX_CHANNELS);
        //Read chaser lengths
        settings.ChaserLength = new byte[CHASER_COUNT];
        for (int i = 0; i < CHASER_COUNT; i++) settings.ChaserLength[i] = br.ReadBytes(0x3)[0];
        //Read inbetween padding
        if (new string(br.ReadChars(0x5)) != IDENTIFIER_A) throw new System.Exception("Invalid file! (no acme hint)");
        br.ReadBytes(PADDING_SIZE); //Read padding of two's
        //Read chaser map
        settings.ChaserOrder = new ushort[CHASER_COUNT, CHASER_STEPS];
        for (int i = 0; i < CHASER_COUNT; i++)
        {
            for (int j = 0; j < CHASER_STEPS; j++) settings.ChaserOrder[i,j] = br.ReadUInt16();
        }
        //Read weird constant
        if (br.ReadChar() != 'K' | br.ReadChar() != (char)1) throw new System.Exception("Wrong version???"); //Read string (maybe versioning???)
        //Read two weird bytes
        settings.TwoWeirdBytes = br.ReadBytes(2);
        //Read dimmer settings
        settings.DimmerSet = br.ReadBytes(0x10).Select(x => x > 0).ToArray();
        settings.DimmedChannels = new bool[16][];
        for (int i = 0; i < 16; i++) settings.DimmedChannels[i] = br.ReadBytes(10).Select(x => x > 0).ToArray();
        //Read file and check for file end
        br.ReadBytes(LEAD_OUT_LENGTH); //Read lead out trail of "0xFF"
        if (br.PeekChar() >= 0) throw new System.Exception("File not at end!");
        return settings;
    }

    public void SaveToFile(string filename)
    {
        if (filename == null || filename == string.Empty) return;

        var fileStream = File.OpenWrite(filename);
        var bw = new BinaryWriter(fileStream, System.Text.Encoding.ASCII);

        //Write header with padding
        bw.Write(Header.ToCharArray());
        bw.Write(Enumerable.Range(1, PADDING_SIZE - Header.Length).Select(_ => '\0').ToArray());
        //Write regular scenes
        for (int i = 0; i < SCENES_COUNT_REGULAR; i++) Scenes[i].WriteScene(bw);
        //Write chaser scenes
        for (int i = 0; i < SCENES_COUNT_CHASER; i++) ChaserSteps[i].WriteScene(bw);
        //Write channel names
        for (int i = 0; i < CHANNEL_COUNT; i++)
        {
            bw.Write(ChannelNames[i].ToCharArray());
            bw.Write(Enumerable.Range(1, CHANNEL_NAME_CHARS - ChannelNames[i].Length).Select(_ => '\0').ToArray());
        }
        //Write patch map
        bw.Write(PatchMap);
        //Write chaser length
        for (int i = 0; i < CHASER_COUNT; i++) bw.Write(new byte[] { ChaserLength[i], 0x00, 0x05 });
        //Write acme padding
        bw.Write(IDENTIFIER_A.ToCharArray());
        //Write two's padding
        bw.Write(Enumerable.Range(1, PADDING_SIZE).Select(_ => (byte)0x02).ToArray());
        //Write chaser order map
        for (int i = 0; i < CHASER_COUNT; i++)
        {
            for (int j = 0; j < CHASER_STEPS; j++) bw.Write(ChaserOrder[i, j]);
        }
        bw.Write(new char[] { 'K', (char)1 });
        bw.Write(TwoWeirdBytes);
        //Write dimmer settings
        for (int i = 0; i < CHASER_COUNT; i++) bw.Write((byte)(DimmerSet[i] ? 1 : 0));
        //Write random padding
        for (int i = 0; i < 16; i++) bw.Write(DimmedChannels[i].Select(x => (byte)(x ? 1 : 0)).ToArray());
        //Write lead out padding
        bw.Write(Enumerable.Range(1, LEAD_OUT_LENGTH).Select(_ => byte.MaxValue).ToArray());

        fileStream.Close();
    }
}
