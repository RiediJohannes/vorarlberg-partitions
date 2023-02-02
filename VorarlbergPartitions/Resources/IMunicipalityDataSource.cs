using System.Collections.Generic;

namespace VorarlbergPartitions.Resources
{
    internal interface IMunicipalityDataSource
    {
        List<string> Columns { get; }
        string[] Header { get; }
        string[] Total { get; }

        string[] GetIDs();
        string[] GetEntry(string municipalityId);

        string[] GetEntryWithMaxAttribute(string attributeName);
        string[] GetEntryWithMinAttribute(string attributeName);
    }
}
