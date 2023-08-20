using System;

namespace LED_Commander_Commander.ManagedData;

[Flags]
public enum FixtureChannels
{
    FUN,
    RED,
    GREEN,
    BLUE,
    SPEED,
    COLOR,
    STROBE,
    DIMMER,
    PAN,
    TILT
}
