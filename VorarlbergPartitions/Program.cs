using System;
using System.Diagnostics;
using System.Drawing;

namespace VorarlbergPartitions
{
    class Program
    {
        static void Main(string[] args)
        {
            // Set up stopwatch to measure program runtime
            Stopwatch runTimer = new Stopwatch();
            runTimer.Start();


            var db = new MunicipalityDatabase("Datasheets/data_Gemeinden.csv");
            var VlbgMap = new SvgMap("Maps/Vorarlberg_Gemeinden.svg");

            ZoneBuilder builder = new ZoneBuilder(db, VlbgMap);
            Zone zoneA = builder.BuildZone(Color.FromArgb(120, 30, 10), Convert.ToInt32(db.Total.Population / 3));

            Console.WriteLine("\nSuccessfully built high density zone!");
            Console.WriteLine($"Population: {zoneA.Population} people");
            Console.WriteLine($"Area: {zoneA.Area} km²\n");

            // stop the stopwatch
            runTimer.Stop();
            Console.WriteLine("Program runtime: " + runTimer.Elapsed);
        }
    }
}
