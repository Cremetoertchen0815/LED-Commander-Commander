using System.Collections.Generic;
using System.Linq;

namespace LED_Commander_Commander.ManagedData;
public class FixtureSettings
{
    private readonly List<ushort>[] _patchedAddresses = Enumerable.Range(0, 10).Select(_ => new List<ushort>()).ToArray();

    public bool IsDimmerActivated { get; set; }
    public FixtureChannels DimmedChannels { get; set; }
    public List<ushort> GetAddressesPatchedToChannel(FixtureChannels channel) => _patchedAddresses[(int)channel];
}
