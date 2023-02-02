using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;


namespace VorarlbergPartitions
{
    class ZoneBuilder
    {
        private readonly MunicipalityDatabase _db;
        private readonly SvgMap _map;

        public ZoneBuilder(MunicipalityDatabase database, SvgMap municipalityMap)
        {
            _map = municipalityMap;
            _db = database;
        }


        // maybe add a 'mode' later, which will be an enum (e.g. BuildMode.HIGHEST_DENSITY)
        public Zone BuildZone(Color colour, int maxPopulation)
        {
            // create a zone an start with a specific municipality as its initial member
            var zone = new Zone();
            Municipality initialMember = _db.GetHighestDensityMunicipality();
            ExpandZone(zone, initialMember, colour);

            // keep expanding the zone until it reaches its intended population
            while (zone.Population < maxPopulation)
            {
                HashSet<Municipality> plannedAnnexations = FindMostSuitableAnnexation(zone);

                foreach (Municipality annexation in plannedAnnexations)
                {
                    // add the neighbour with the highest density to the zone and colour it
                    ExpandZone(zone, annexation, colour);
                }
            }

            /*
             *  TODO: check by how many people we overshoot and if we can do a better last annexation
             */


            // save the changes to a new svg file
            _map.SaveChanges("Maps/Outputs/Vorarlberg_coloured.svg");

            return zone;
        }


        private HashSet<Municipality> FindMostSuitableAnnexation(Zone zone)
        {
            return FindMostSuitableAnnexation(zone, new HashSet<Municipality>(zone.Neighbours));
        }

        private HashSet<Municipality> FindMostSuitableAnnexation(Zone zone, HashSet<Municipality> possibleChoices)
        {
            // find the zone neighbour with the highest population density => neighbour to annex next
            Municipality plannedAnnexation = possibleChoices.MaxBy(m => m.Density);

            // check if the annexation of the selected municipality would cause "lonely municipalities" (islands)
            // abort this choice and find another one
            HashSet<Municipality> lonelyMunicipalities = CheckForLonelyMunicipalities(zone, plannedAnnexation);

            // if the hashset is empty, then we didn't run into problems with this planned annexation
            if (!lonelyMunicipalities.Any())
            {
                // proceed normally with the planned annexation
                return new HashSet<Municipality> { plannedAnnexation };
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Annexing {plannedAnnexation} would lead to lonely municipalities!");
            Console.ResetColor();

            foreach (Municipality municipality in lonelyMunicipalities)
            {
                Console.WriteLine("Would be unreachable: " + municipality);
            }

            // otherwise search for the next best annexation
            possibleChoices.Remove(plannedAnnexation);
            HashSet<Municipality> alternativeAnnexation = FindMostSuitableAnnexation(zone, possibleChoices);

            // check if it would be more worth it to annex the planned municipality including the whole "lonely" island
            lonelyMunicipalities.Add(plannedAnnexation);
            int worthAnnexing = CompareAnnexations(lonelyMunicipalities, alternativeAnnexation);

            if (worthAnnexing >= 0)
            {
                // annex the initially planned municipality together with all the emerging lonely municipalities
                return lonelyMunicipalities;
            }
            // otherwise perform the alternative annexation
            return alternativeAnnexation;
        }


        // check if annexation would cause the formation of "lonely municipalites"
        // lonely municipalities = municipalities that are not connected to the entirety of their respective zone (zone A, B, C... or "no zone")
        private HashSet<Municipality> CheckForLonelyMunicipalities(Zone zone, Municipality plannedAnnexation)
        {
            // create a new HashSet reprensenting the proposed zone (incorporating the plannedAnnexation)
            HashSet<Municipality> proposedZoneMembers = new HashSet<Municipality>(zone.Members);
            proposedZoneMembers.Add(plannedAnnexation);

            // create a HashSet of every municipality in the database that is not yet part of zone A
            HashSet<Municipality> zonelessMunipals = new HashSet<Municipality>(_db.GetAllMunicipalities());
            zonelessMunipals.ExceptWith(proposedZoneMembers);
            zonelessMunipals.Remove(plannedAnnexation);


            // create list to contain every municipality that is still connected to the first element in zonelesssMunipals
            // put one municipality out of the zonelessMunipals in this new list to check connectivity to all other municipalities from there
            List<Municipality> reachableMunipals = new();
            Municipality startMunipal = zonelessMunipals.ElementAt(0);
            reachableMunipals.Add(startMunipal);
            zonelessMunipals.Remove(startMunipal);

            // go through each municipality in reachableMunipals
            for (int n = 0; n < reachableMunipals.Count; n++)
            {
                // go through the neighbours of each of those
                foreach (Municipality neighbour in reachableMunipals[n].Neighbours)
                {
                    // if a neighbour is not part of the proposed zone or the reachableMunipals list
                    if (!proposedZoneMembers.Contains(neighbour) && !reachableMunipals.Contains(neighbour))
                    {
                        // add it to reachableMunipals and remove it from zonelessMunipals
                        reachableMunipals.Add(neighbour);
                        zonelessMunipals.Remove(neighbour);
                    }
                }
            }

            return zonelessMunipals;
        }

        private int CompareAnnexations(HashSet<Municipality> leftAnnexation, HashSet<Municipality> rightAnnexation)
        {
            // calculate density of left annexation
            int leftPopulation = leftAnnexation.Sum(mun => mun.Population);
            double leftArea = leftAnnexation.Sum(mun => mun.Area);
            double leftDensity = leftPopulation / leftArea;

            // calculate density of right annexation
            int rightPopulation = rightAnnexation.Sum(mun => mun.Population);
            double rightArea = rightAnnexation.Sum(mun => mun.Area);
            double rightDensity = rightPopulation / rightArea;

            double difference = Math.Round(leftDensity - rightDensity, 2);

            if (difference == 0.0)
            {
                return 0;   // equal densities (unrealistic)
            }
            else if (difference < 0)
            {
                return -1;  // island density was smaller
            }
            return 1;   // island density was greater
        }

        private void ExpandZone(Zone zone, Municipality member, Color colour)
        {
            zone.Add(member);
            _map.ChangeColour(member, colour);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Added to zone: " + member.Name);
            Console.ResetColor();
        }
    }
}