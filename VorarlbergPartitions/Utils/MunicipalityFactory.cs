using System;
using System.Collections.Generic;
using System.Text;
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
            Dictionary<string, int> columns = _data.Columns;

            // create a new Municipality instance
            var municipality = new Municipality
            {
                Id = csvEntry[columns["ID"]],
                Name = csvEntry[columns["name"]],
                Population = int.Parse(csvEntry[columns["population"]]),
                Area = double.Parse(csvEntry[columns["area"]]),
            };

            // load the municipality's neighbours lazily
            municipality.PrepareNeighbours(new Lazy<List<Municipality>>(() =>
            {
                List<Municipality> neighbours = new List<Municipality>();
                foreach (string neighbourID in csvEntry[columns["neighbours"]].Split(','))
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
