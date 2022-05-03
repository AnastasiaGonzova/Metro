using System;
using System.Collections.Generic;
using System.IO;

namespace Metro
{
    class Program
    {
        static void Main(string[] args)
        {
            List<char> stations = new List<char>();
            List<List<char>> metroScheme = new List<List<char>>();

            string file = "metro.txt";
            if(!ReadFromFile(file, ref stations, ref metroScheme))
            {
                throw new ArgumentException("The data in the file is incorrect!");
            }


            bool[] used = new bool[stations.Count];
            int[] distance = new int[stations.Count];
            Queue<int> queue = new Queue<int>();
            List<List<int>> parents = new List<List<int>>();
            for(int i = 0; i < stations.Count; i++)
            {
                List<int> p = new List<int>();
                parents.Add(p);
            }

            
            Console.Write("Enter the start station: ");
            string line = Console.ReadLine();
            int startStationIndex = stations.IndexOf(char.Parse(line));
            used[startStationIndex] = true;
            queue.Enqueue(startStationIndex);

            
            Console.Write("Enter the finish station: ");
            line = Console.ReadLine();
            int finishStationIndex = stations.IndexOf(char.Parse(line));


            BreadthFirstSearch(ref queue, ref parents, ref distance, ref used, in metroScheme);


            List<List<int>> path = new List<List<int>>();
            BuildPath(finishStationIndex, startStationIndex, null, ref parents, ref path);
            

            int resultTransfer = 0;
            int resultPathIndex = CheckTransfer(ref path, ref metroScheme, out resultTransfer);

            Console.WriteLine("Result: ");
            for(int i = 0; i < path[resultPathIndex].Count; i++)
            {
                Console.Write(getStationName(path[resultPathIndex][i], ref stations) + "  ");
            }
            Console.WriteLine("\nThe number of stations is " + path[resultPathIndex].Count + ", the number of transfers is " + resultTransfer);

            Console.ReadLine();
        }
        
        public static bool ReadFromFile(string fileName, ref List<char> stations, ref List<List<char>> metroScheme)
        {
            using (StreamReader reader = new StreamReader(fileName))
            {
                string fileLine;
                int N = 0;
                if ((fileLine = reader.ReadLine()) != null)
                {
                    if(!int.TryParse(fileLine, out N))
                    {
                        return false;
                    }
                }
                if ((fileLine = reader.ReadLine()) != null)
                {
                    for (int i = 0; i < fileLine.Length; i++)
                    {
                        stations.Add(fileLine[i]);
                    }
                    if(stations.Count != N)
                    {
                        return false;
                    }
                }
                int index = 0;
                while ((fileLine = reader.ReadLine()) != null)
                {
                    metroScheme.Add(new List<char>());
                    for (int i = 0; i < fileLine.Length; i++)
                    {
                        metroScheme[index].Add(fileLine[i]);
                    }
                    if(metroScheme[index].Count != N)
                    {
                        return false;
                    }
                    index++;
                }
                if(metroScheme.Count != N)
                {
                    return false;
                }

                return true;
            }
        }

        public static char getStationName(int index, ref List<char> stations)
        {
            return stations[index];
        }

        public static void BreadthFirstSearch(ref Queue<int> queue, ref List<List<int>> parents, ref int[] distance, ref bool[] used, in List<List<Char>> metroScheme)
        {
            while (queue.Count != 0)
            {
                int current = queue.Dequeue();
                for (int i = 0; i < metroScheme[current].Count; i++)
                {
                    char to = metroScheme[current][i];
                    if (to != '_')
                    {
                        if (!used[i])
                        {
                            used[i] = true;
                            distance[i] = distance[current] + 1;
                            if (distance[i] > distance[current]) parents[i].Add(current);
                            queue.Enqueue(i);
                        }
                        else
                        {
                            if (distance[i] > distance[current]) parents[i].Add(current);
                        }
                    }
                }
            }
        }

        public static void BuildPath(int stationIndex, int startStationIndex, List<int> path, ref List<List<int>> parents, ref List<List<int>> paths)
        {
            if(path == null)
            {
                path = new List<int>();
            }
            path.Add(stationIndex);
            if (stationIndex == startStationIndex)
            {
                List<int> result = new List<int>();
                for(int i = path.Count - 1; i >= 0; i--)
                {
                    result.Add(path[i]);
                }
                paths.Add(result);
                path.RemoveAt(path.Count - 1);
                return;
            }
            for(int i = 0; i < parents[stationIndex].Count; i++)
            {
                BuildPath(parents[stationIndex][i], startStationIndex, path, ref parents, ref paths);
            }
            if (path.Count != 0) path.RemoveAt(path.Count - 1);
        }

        public static int CheckTransfer(ref List<List<int>> path, ref List<List<char>> metroScheme, out int resultTransfer)
        {
            int resultPathIndex = -1;
            resultTransfer = int.MaxValue;
            for (int i = 0; i < path.Count; i++)
            {
                int transfer = 0;
                for (int j = 1; j < path[i].Count - 1; j++)
                {
                    if (metroScheme[path[i][j - 1]][path[i][j]] != metroScheme[path[i][j]][path[i][j + 1]])
                    {
                        transfer++;
                    }
                }
                if (transfer < resultTransfer)
                {
                    resultTransfer = transfer;
                    resultPathIndex = i;
                }
            }
            return resultPathIndex;
        }
    }
}
