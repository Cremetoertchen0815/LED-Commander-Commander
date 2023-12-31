﻿using System;
using System.IO;

namespace LED_Commander_Commander.RawData;
public struct RawSceneMapEntry
{
    public RawFixtureChannels[] FixtureChannels { get; set; } //16 * 10 Bytes

    public bool IsEmpty { get; set; }

    public bool Aux1 { get; set; }

    public bool Aux2 { get; set; }

    public byte[] FieldArea { get; set; }

    public static RawSceneMapEntry Empty
    {
        get
        {
            var settings = new RawSceneMapEntry();
            settings.FixtureChannels = new RawFixtureChannels[16];
            settings.IsEmpty = true;
            settings.FieldArea = new byte[] { 0, 0, 0, 0 };

            return settings;
        }
    }

    public static RawSceneMapEntry ReadScene(BinaryReader br)
    {
        var settings = new RawSceneMapEntry();
        //Load fixture data
        settings.FixtureChannels = new RawFixtureChannels[16];
        for (int j = 0; j < 16; j++) settings.FixtureChannels[j].ReadIn(br);


        //Load enabled channels(loaded in 4 Blocks of 5 Bytes each, every Block encodes enabled channels for 4 fixtures)
        for (int i = 0; i < 4; i++)
        {
            var blockData = new byte[8];
            new Span<byte>(br.ReadBytes(5)).CopyTo(blockData);
            var blockInt = BitConverter.ToInt64(blockData);

            //For all 4 devices in this block
            for (int j = 0; j < 4; j++)
            {
                settings.FixtureChannels[i * 4 + j].EnabledChannels = (ChannelType)(blockInt >> (j * 10) & 0b1111111111);
            }
        }

        //The following block of data contains various information on the scene
        //Some chunks could be deciphered, like the aux channels or the integrity bit that determines whether the scene exists
        //But it may contain other important data, so the whole block is stored as field area
        var fieldData = br.ReadBytes(4);

        //Load integrity check;
        settings.IsEmpty = 0 == (fieldData[0] & 1);

        //Load Aux 1 & Aux 2
        settings.Aux1 = (fieldData[1] & 1) == 1;
        settings.Aux2 = (fieldData[1] & 2) == 2;

        //Store rest of data
        settings.FieldArea = fieldData;

        return settings;
    }

    public void WriteScene(BinaryWriter bw)
    {

        for (int j = 0; j < 16; j++) FixtureChannels[j].WriteOut(bw);

        //Load enabled channels(loaded in 4 Blocks of 5 Bytes each, every Block encodes enabled channels for 4 fixtures)
        for (int i = 0; i < 4; i++)
        {
            long enableData = 0;
            //For all 4 devices in this block
            for (int j = 0; j < 4; j++)
            {
                enableData |= (long)FixtureChannels[i * 4 + j].EnabledChannels << (j * 10);
            }
            var enableBytes = new Span<byte>(BitConverter.GetBytes(enableData)).Slice(0, 5);
            bw.Write(enableBytes);
        }

        //Indicate that scene is existent
        bw.Write((byte)(IsEmpty ? 0 : 1));

        //Write Aux 1 & Aux 2
        bw.Write((byte)((Aux1 || Aux2 ? 16 : 0) | (Aux1 ? 1 : 0) | (Aux2 ? 2 : 0)));

        //Write weird byte
        bw.Write(FieldArea[2]);

        //Write trailing 0
        bw.Write((byte)0);
    }
}
