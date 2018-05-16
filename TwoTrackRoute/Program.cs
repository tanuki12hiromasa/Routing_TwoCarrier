using System;

namespace TwoTrack
{
    class salesman
    {
        const int width = 9;
        const int hight = 9;
        const string mapfile = "neighbor_mat.csv";
        const string destfile = "destination.csv";

        static void Main(string[] args)
        {
            Console.WriteLine(System.IO.Directory.GetCurrentDirectory());
            var pf = new PathFinder(width, hight);
            pf.ex(mapfile, destfile);
        }


    }
}
