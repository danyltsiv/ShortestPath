using System;
using System.Collections.Generic;
using ShortestPath.Entities;

namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Relation> relations = new List<Relation>
            {
                new Relation("A", "B", 80),
                new Relation("A","C", 110),
                new Relation("A","E", 330),
                new Relation("B","A", 80),
                new Relation("B","F", 340),
                new Relation("C","A",110),
                new Relation("C","D", 60),
                new Relation("C","E", 205),
                new Relation("D","C", 60),
                new Relation("D","F", 192),
                new Relation("E","A", 330),
                new Relation("E","C", 205),
                new Relation("E","F", 80),
                new Relation("F","E", 80),
                new Relation("F","B", 340),
                new Relation("F","D", 192),
            };

            ShortestPathSeeker<Relation, string> shps = new ShortestPathSeeker<Relation, string>
                (
                    g => g.From,
                    g => g.To,
                    relations
                 );

            var sps = shps.FindAllPaths("A", "D");
            var shortestDistance = shps.GetShortestDistance("A", "D", g => g.Distance);

            foreach (var path in sps)
            {
                foreach(var pat in path)
                Console.Write(pat.From + "->" + pat.To + " ");

                Console.WriteLine();
            }

            foreach (var pat in shortestDistance.Item1)
            {
                Console.Write(pat.From + "->" + pat.To + " ");
            }
            Console.WriteLine("Distance = {0}", shortestDistance.Item2);

            Console.ReadKey();
        }
    }
}
