using System;
using System.Collections.Generic;
using System.Text;

namespace VorarlbergPartitions
{
    class MunicipalityDatabase
    {
        private readonly CSVBuffer _csv;
        private readonly MunicipalityFactory _factory;

        public Municipality Total
        {
            get => _factory.FromEntry(_csv.Total);
        }


        public MunicipalityDatabase(string csvPath)
        {
            _csv = new CSVBuffer(csvPath);
            _factory = new MunicipalityFactory(csvPath);
        }

        public Municipality GetMunicipalityById(string municipalityId)
        {
            string[] entry = _csv.GetEntry(municipalityId);

            return _factory.FromEntry(entry);
        }

        public Municipality[] GetAllMunicipalities()
        {
            List<Municipality> municipalities = new List<Municipality>();

            foreach (string id in _csv.GetIDs())
            {
                var munipal = GetMunicipalityById(id);
                municipalities.Add(munipal);
            }
            return municipalities.ToArray();
        }

        public Municipality GetHighestDensityMunicipality()
        {
            string[] entry = _csv.GetHighestDensityMunicipality();
            return _factory.FromEntry(entry);
        }

        public Municipality GetLowestDensityMunicipality()
        {
            string[] entry = _csv.GetLowestDensityMunicipality();
            return _factory.FromEntry(entry);
        }
    }
}
