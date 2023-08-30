using LED_Commander_Commander.RawData;
using System.Collections.Generic;
using System.Linq;

namespace LED_Commander_Commander.ManagedData;
public class FixtureSettings
{
    private readonly List<ushort>[] _patchedAddresses = Enumerable.Range(0, 10).Select(_ => new List<ushort>()).ToArray();

    public bool IsDimmerActivated { get; set; }
    public ChannelType DimmedChannels { get; set; }
    public List<ushort> GetAddressesPatchedToChannel(int channel) => _patchedAddresses[channel];
}
