using System;
using System.Collections.Generic;
using System.Text;
using VorarlbergPartitions.Resources;

namespace VorarlbergPartitions
{
    class MunicipalityDatabase
    {
        private readonly IMunicipalityDataSource _data;
        private readonly MunicipalityFactory _factory;

        public Municipality Total
        {
            get => _factory.FromEntry(_data.Total);
        }


        public MunicipalityDatabase(IMunicipalityDataSource dataSource)
        {
            _data = dataSource;
            _factory = new MunicipalityFactory(dataSource);
        }

        public Municipality GetMunicipalityById(string municipalityId)
        {
            string[] entry = _data.GetEntry(municipalityId);

            return _factory.FromEntry(entry);
        }

        public Municipality[] GetAllMunicipalities()
        {
            List<Municipality> municipalities = new List<Municipality>();

            foreach (string id in _data.GetIDs())
            {
                var munipal = GetMunicipalityById(id);
                municipalities.Add(munipal);
            }
            return municipalities.ToArray();
        }

        public Municipality GetHighestDensityMunicipality()
        {
            string[] entry = _data.GetEntryWithMaxAttribute("density");
            return _factory.FromEntry(entry);
        }

        public Municipality GetLowestDensityMunicipality()
        {
            string[] entry = _data.GetEntryWithMinAttribute("density");
            return _factory.FromEntry(entry);
        }
    }
}
