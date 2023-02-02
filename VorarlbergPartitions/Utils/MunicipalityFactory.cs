using System;
using System.Collections.Generic;
using VorarlbergPartitions.Resources;

namespace VorarlbergPartitions
{
    class MunicipalityFactory
    {
        private readonly IMunicipalityDataSource _data;

        public MunicipalityFactory(IMunicipalityDataSource dataSource)
        {
            _data = dataSource;
        }

        public Municipality FromEntry(string[] csvEntry)
        {
            // get an overview of the column names and their respective indeces
            List<string> columns = _data.Columns;

            // create a new Municipality instance
            var municipality = new Municipality
            {
                Id = csvEntry[columns.IndexOf("ID")],
                Name = csvEntry[columns.IndexOf("name")],
                Population = int.Parse(csvEntry[columns.IndexOf("population")]),
                Area = double.Parse(csvEntry[columns.IndexOf("area")]),
            };

            // load the municipality's neighbours lazily
            municipality.PrepareNeighbours(new Lazy<List<Municipality>>(() =>
            {
                List<Municipality> neighbours = new();
                foreach (string neighbourID in csvEntry[columns.IndexOf("neighbours")].Split(','))
                {
                    // fetch the data for each neighbour and create a Municipality object from it
                    string[] neighbourEntry = _data.GetEntry(neighbourID);
                    Municipality neighbour = FromEntry(neighbourEntry);
                    neighbours.Add(neighbour);
                }

                return neighbours;
            }));

            // calculate density on the fly
            municipality.Density = municipality.Population / municipality.Area;

            return municipality;
        }
    }
}
