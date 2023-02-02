using System;
using System.Collections.Generic;

namespace VorarlbergPartitions.Models
{
    class Municipality
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Population { get; set; }
        public double Area { get; set; }
        public double Density { get; set; }
        private Lazy<List<Municipality>> _neighbours;

        public List<Municipality> Neighbours
        {
            get => _neighbours.Value;
        }

        public void PrepareNeighbours(Lazy<List<Municipality>> neighbourList)
        {
            _neighbours = neighbourList;
        }

        public override bool Equals(object obj)
        {
            return obj is Municipality municipality &&
                   Id == municipality.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public override string ToString()
        {
            return "Gemeinde " + Id;
        }
    }
}
