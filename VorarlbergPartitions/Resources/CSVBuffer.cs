using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VorarlbergPartitions.Resources;

namespace VorarlbergPartitions
{
    class CSVBuffer : IMunicipalityDataSource
    {
        private readonly Dictionary<string, string[]> _entries = new Dictionary<string, string[]>();

        public List<string> Columns { get; } = new List<string>();
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
                Columns.Add(columnNames[i]);
            }
            
            // save header data
            Header = data[1].Split(';');

            // save the lines containing the actual data in a Dictionary
            string[] lines = data.Skip(2).Take(data.Length - 3).ToArray();  // take the rest of the lines in the CSV
            foreach (string line in lines)
            {
                // the municipality id becomes the key in our Dictionary
                int firstDelimiter = line.IndexOf(';');
                string key = line[..firstDelimiter];
                string[] values = line.Split(';');
                
                _entries.Add(key, values);
            }

            // the last entry in the csv holds the total values for the whole of Vorarlberg
            Total = data[^1].Split(';');
        }

        public string[] GetIDs()
        {
            return _entries.Keys.ToArray();
        }

        public string[] GetEntry(string municipalityId)
        {
            return _entries.GetValueOrDefault(municipalityId);
        }

        public string[] GetEntryWithMaxAttribute(string attributeName)
        {
            if (!Columns.Contains(attributeName)) {
                throw new ArgumentException("Data does not contain values with an attribute named " + attributeName);
            }

            int attributeIndex = Columns.IndexOf(attributeName);
            string maxDensityID = _entries.MaxBy(entry => double.Parse(entry.Value[attributeIndex])).Key;
            return _entries.GetValueOrDefault(maxDensityID);
        }

        public string[] GetEntryWithMinAttribute(string attributeName)
        {
            if (!Columns.Contains(attributeName))
            {
                throw new ArgumentException("Data does not contain values with an attribute named " + attributeName);
            }

            int attributeIndex = Columns.IndexOf(attributeName);
            string maxDensityID = _entries.MinBy(entry => double.Parse(entry.Value[attributeIndex])).Key;
            return _entries.GetValueOrDefault(maxDensityID);
        }
    }
}
