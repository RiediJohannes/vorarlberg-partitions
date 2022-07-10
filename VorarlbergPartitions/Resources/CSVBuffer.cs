using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace VorarlbergPartitions
{
    class CSVBuffer
    {
        private readonly Dictionary<string, string[]> _entries = new Dictionary<string, string[]>();

        public Dictionary<string, int> Columns { get; } = new Dictionary<string, int>();
        public string[] Header { get; }
        public string[] Total { get; }


        public CSVBuffer(string path)
        {
            string actualPath = "../../../" + path;
            // read the whole CSV file
            string[] data = File.ReadAllLines(actualPath, Encoding.GetEncoding("Latin1"));

            // save columns names in a dictionary
            string[] columnNames = data[0].Split(';');
            for (int i = 0; i < columnNames.Length; i++)
            {
                Columns.Add(columnNames[i], i);
            }
            
            // save header data
            Header = data[1].Split(';');

            // save the lines containing the actual data in a Dictionary
            string[] lines = data.Skip(2).Take(data.Length - 3).ToArray();  // take the rest of the lines in the CSV
            foreach (string line in lines)
            {
                // the municipality id becomes the key in our Dictionary
                int firstDelimiter = line.IndexOf(';');
                string key = line.Substring(0, firstDelimiter);
                string[] values = line.Split(';');
                
                _entries.Add(key, values);
            }

            // the last entry in the csv holds the total values for the whole of Vorarlberg
            Total = data[data.Length - 1].Split(';');
        }

        public string[] GetEntry(string municipalityId)
        {
            return _entries.GetValueOrDefault(municipalityId);
        }

        public string[] GetIDs()
        {
            return _entries.Keys.ToArray();
        }

        public string[] GetHighestDensityMunicipality()
        {
            string bestMunicipalityID = _entries.First().Key;
            double highestDensity = 0.0;

            foreach (string[] entry in _entries.Values)
            {
                double density = double.Parse(entry[Columns["density"]]);
                if (density > highestDensity)
                {
                    bestMunicipalityID = entry[Columns["ID"]];
                    highestDensity = density;
                }
            }
            return _entries.GetValueOrDefault(bestMunicipalityID);
        }

        public string[] GetLowestDensityMunicipality()
        {
            string bestMunicipalityID = _entries.First().Key;
            double lowestDensity = double.Parse(_entries.First().Value[Columns["density"]]);

            foreach (string[] entry in _entries.Values)
            {
                double density = double.Parse(entry[Columns["density"]]);
                if (density < lowestDensity)
                {
                    bestMunicipalityID = entry[Columns["ID"]];
                    lowestDensity = density;
                }
            }
            return _entries.GetValueOrDefault(bestMunicipalityID);
        }
    }
}
