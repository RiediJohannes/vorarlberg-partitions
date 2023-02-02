using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VorarlbergPartitions.Resources
{
    internal interface IMunicipalityDataSource
    {
        Dictionary<string, int> Columns { get; }
        string[] Header { get; }
        string[] Total { get; }

        string[] GetIDs();
        string[] GetEntry(string municipalityId);

        string[] GetEntryWithMaxAttribute(string attributeName);
        string[] GetEntryWithMinAttribute(string attributeName);
    }
}
