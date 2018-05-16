using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TwoTrack //二台のトラックでの最短時間を求める
{
    public class Destination
        {
            public string name;
            public int pos;
            public int[] cost;
            public List<int>[] route;
        }

    public class Node { public int pos, prev, cost; };

    public class PathFinder
    {
        int _width;int _height;
        int nCross; //交差点の数 width*height 
        int[,] map; //グラフ（地図）データ
        Destination[] dest; //配達地点　並びはcsvの順
        List<int> path1; //配達順.一人目
        List<int> path2; //二人目
        int startpoint; //開始点/終着点
        int[] isDest; //その場所がDestかどうか(Destならその番号、違うなら-1)
        protected string outfile = "betterpath.txt";

        public PathFinder(int width,int height)
        {
            _width = width; _height = height;
            nCross = _width * _height;
            map = new int[nCross, nCross];
            isDest = new int[nCross];
            for (int i =0;i<isDest.Length;i++) isDest[i] = -1;
        }

        public int ex(string mapfile,string destsfile) //一連の計算を行う
        {
            ReadMap(mapfile);
            ReadDest(destsfile);
            findRoute();
            makePath(dest, startpoint, out path1,out path2);
            WritePath();
            return 0;
        }

        void findRoute()
        {

            for(int i = 0; i < dest.Length; i++)
            {                
                int dLeft = dest.Length;
                var nextNodeList = new List<Node>();
                var visitedNodes = new Node[nCross];
                nextNodeList.Add(new Node { pos = dest[i].pos, prev = -1, cost = 0 });

                while (dLeft > 0)
                {
                    nextNodeList.Sort((a, b) => a.cost - b.cost);
                    var node = nextNodeList[0];
                    nextNodeList.RemoveAt(0);
                    if (visitedNodes[node.pos] == null)
                    {
                        visitedNodes[node.pos] = node;
                        if (isDest[node.pos] != -1) dLeft--;
                        for (int n = 0; n < nCross; n++)
                        {
                            if (map[node.pos, n] == 1)
                            {
                                nextNodeList.Add(new Node { pos = n, prev = node.pos, cost = node.cost + 1 });
                            }
                        }
                    }
                }
                for(int j = 0; j < dest.Length; j++) //prevを辿ってrouteに格納
                {
                    if (i == j) continue;
                    var pos = dest[j].pos;
                    while (pos != dest[i].pos)
                    {
                        dest[i].route[j].Add(pos);
                        pos = visitedNodes[pos].prev;
                    }
                    dest[i].route[j].Reverse();
                    dest[i].cost[j] = visitedNodes[dest[j].pos].cost;
                }
                Console.WriteLine(dest[i].name + ":done");
            }
            Console.WriteLine("findRoute:done");
            WriteRoute();
        }

        virtual protected void makePath(Destination[] dest,int startpoint,out List<int> path1,out List<int> path2)
        {
            var p1 = new List<int>();
            var p2 = new List<int>();





            path1 = p1; path2 = p2;
        }

        int SumCost(List<int> path)
        {
            int sum = 0;
            for (int i = 0; i < path.Count - 1; i++) sum += dest[path[i]].cost[path[i + 1]];
            return sum;
        }



        void ReadMap(string file)
        {
            try
            {
                using (var str = new System.IO.StreamReader(file))
                {
                    for (int i = 0; i < nCross; i++)
                    {
                        var line = str.ReadLine();
                        var nodes = line.Split(", ");
                        for (int j = 0; j < nCross; j++) map[i, j] = int.Parse(nodes[j]);
                    }
                }
                System.Console.WriteLine("reading map done");
                for(int i = 0; i < nCross; i++)
                {
                    for (int j = 0; j < nCross; j++) System.Console.Write(map[i, j] + " ");
                    System.Console.Write('\n');
                }
            }
            catch(System.Exception e)
            {
                System.Console.WriteLine(e.Message);
            }

        }

        void ReadDest(string file)
        {
            try
            {
                using(var str = new System.IO.StreamReader(file))
                {
                    var dList = new List<Destination>();
                    while (!str.EndOfStream)
                    {
                        var line = str.ReadLine();
                        var nodes = line.Split(',');
                        dList.Add(new Destination { name = nodes[0], pos = int.Parse(nodes[1]) });
                    }
                    dest = dList.ToArray();
                    for (int i = 0; i < dest.Length; i++)
                    {
                        dest[i].cost = new int[dest.Length];
                        dest[i].route = new List<int>[dest.Length];
                        for (int j = 0; j < dest.Length; j++) dest[i].route[j] = new List<int>();
                        if (dest[i].name == "SP") startpoint = i;
                        isDest[dest[i].pos] = i;
                    }
                    for (int i = 0; i < dest.Length; i++) Console.WriteLine("name:" + dest[i].name + " pos:" + dest[i].pos);
                    Console.WriteLine("startpoint:" + startpoint + "(" + dest[startpoint].pos + ")");
                }
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e.Message);
            }

        }

        public void WriteRoute()
        {
            for(int i = 0; i < dest.Length; i++)
            {
                Console.Write(dest[i].name + ":");
                for(int j = 0; j < dest.Length; j++)
                {
                    Console.Write(dest[i].cost[j]+" ");
                }
                Console.WriteLine();
            }
        }

        public void WritePath()
        {
            try
            {
                var p1cost = SumCost(path1); var p2cost = SumCost(path2);
                using(var stw = new System.IO.StreamWriter(outfile))
                {

                    stw.WriteLine("<１台目> time:" + p1cost / 6 + "h" + p1cost % 6 * 10 + "m");
                    for(int i = 0; i < path1.Count-1; i++)
                    {
                        stw.Write(dest[path1[i]].name+": ");
                        for (int j = 0; j < dest[path1[i]].route[path1[i+1]].Count; j++) stw.Write(dest[path1[i]].route[path1[i+1]][j]+" ");
                        stw.WriteLine();
                    }
                    stw.WriteLine(dest[path1[path1.Count - 1]].name);
                    stw.WriteLine("<２台目> time:" + p2cost / 6 + "h" + p2cost % 6 * 10 + "m");
                    for (int i = 0; i < path2.Count - 1; i++)
                    {
                        stw.Write(dest[path2[i]].name + ": ");
                        for (int j = 0; j < dest[path2[i]].route[path2[i + 1]].Count; j++) stw.Write(dest[path2[i]].route[path2[i + 1]][j] + " ");
                        stw.WriteLine();
                    }
                    stw.WriteLine(dest[path2[path2.Count - 1]].name);
                    stw.WriteLine("所要時間:" + Math.Max(p1cost, p2cost));
                }
            }
            catch(System.Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
        }
    }
}
