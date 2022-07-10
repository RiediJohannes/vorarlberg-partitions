using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace VorarlbergPartitions
{
    class Zone
    {
        private readonly HashSet<Municipality> _members = new HashSet<Municipality>();
        private readonly HashSet<Municipality> _neighbours = new HashSet<Municipality>();
        private int _population;
        private double _area;


        public int Population
        {
            get => _population;
        }

        public double Area
        {
            get => Math.Round(_area, 2);
        }

        public double Density
        {
            get => Math.Round(_population / _area, 2);
        }

        public HashSet<Municipality> Members
        {
            get => _members;
        }

        public HashSet<Municipality> Neighbours
        {
            get => _neighbours;
        }


        public bool Add(Municipality newMember)
        {
            // if the zone already contains the proposed new member, cancel this operation
            if (Contains(newMember))
            {
                return false;
            }

            _members.Add(newMember);                // add the new member to the zone
            _population += newMember.Population;    // add its population to the zone population
            _area += newMember.Area;                // add its area to the zone area

            // the new member is no longer a zone neighbour -> we remove it from the zone neighbours
            _neighbours.Remove(newMember); 

            // add the neighbours of the new member to the zone neighbours if they are not already part of the zone itself
            newMember.Neighbours.ForEach(neighbour =>
            {
                if (!Contains(neighbour))
                {
                    _neighbours.Add(neighbour);
                }
            });
            return true;
        }

        public bool Remove(Municipality member)
        {
            // if the zone doesn't contain the given municipality, cancel this operation
            if (!Contains(member))
            {
                return false;
            }

            // else remove it from the zone and change the zone's properties accordingly
            _members.Remove(member);
            _population -= member.Population;
            _area -= member.Area;

            // also remove its neighbours from the zone neighbours if they no longer touch the remaining zone
            member.Neighbours.ForEach((neighbour) =>
            {
                if (CanRemoveNeighbour(neighbour))
                {
                    _neighbours.Remove(neighbour);
                }
            });

            return true;
        }

        public bool Contains(Municipality municipality)
        {
            return _members.Contains(municipality);
        }

        
        private bool CanRemoveNeighbour(Municipality neighbour)
        {
            // don't remove a neighbour if it is part of the zone itself
            if (Contains(neighbour))
            {
                return false;
            }

            // also don't remove a neighbour if one of their neighbours is still part of the zone
            foreach (Municipality neighboursNeighbour in neighbour.Neighbours)
            {
                if (Contains(neighboursNeighbour))
                {
                    return false;
                }
            }

            // else, we can savely remove it from the neighbours list
            return true;
        }
    }
}
