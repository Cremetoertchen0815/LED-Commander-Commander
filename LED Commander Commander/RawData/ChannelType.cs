using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LED_Commander_Commander.RawData;

[Flags]
public enum ChannelType : int
{
    FUN = 1,
    RED = 2,
    GREEN = 4,
    BLUE = 8,
    SPEED = 16,
    COLOR = 32,
    STROBE = 64,
    DIMMER = 128,
    PAN = 256,
    TILT = 512
}
