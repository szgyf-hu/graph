using GraphVizWrapper;
using GraphVizWrapper.Commands;
using GraphVizWrapper.Queries;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graph1
{
    class MyGraph
    {
        int NodesCount;
        double[,] AdjMatrix;

        public MyGraph(int NodesCount)
        {
            this.NodesCount = NodesCount;
            AdjMatrix = new double[NodesCount, NodesCount];
            for (int y = 0; y < NodesCount; y++)
                for (int x = 0; x < NodesCount; x++)
                    AdjMatrix[x, y] = Double.PositiveInfinity;
        }

        public void genGraph(int EdgeCount)
        {
            Random r = new Random();

            for (int e = 0; e < EdgeCount; e++)
            {

                int n1;
                int n2;
                do
                {
                    n1 = r.Next(NodesCount);
                    do
                    {
                        n2 = r.Next(NodesCount);
                    } while (n1 == n2);
                } while (
                    AdjMatrix[n1, n2] != Double.PositiveInfinity
                    ||
                    AdjMatrix[n2, n1] != Double.PositiveInfinity
                    );

                int weight = r.Next(10) + 1;

                AdjMatrix[n1, n2] = weight;
                AdjMatrix[n2, n1] = weight;
            }
        }

        public Image toDigraphImage()
        {
            var getStartProcessQuery = new GetStartProcessQuery();
            var getProcessStartInfoQuery = new GetProcessStartInfoQuery();
            var registerLayoutPluginCommand = new RegisterLayoutPluginCommand(getProcessStartInfoQuery, getStartProcessQuery);

            var wrapper = new GraphGeneration(getStartProcessQuery,
                                              getProcessStartInfoQuery,
                                              registerLayoutPluginCommand);

            StringBuilder sb = new StringBuilder("digraph{");

            for (int n1 = 0; n1 < NodesCount; n1++)
                for (int n2 = 0; n2 < NodesCount; n2++)
                    if (AdjMatrix[n1, n2] != Double.PositiveInfinity)
                        sb.AppendLine(String.Format("n{0}->n{1}", n1, n2));

            sb.AppendLine("}");

            byte[] output = wrapper.GenerateGraph(sb.ToString(), Enums.GraphReturnType.Png);

            using (MemoryStream ms = new MemoryStream(output))
            {
                return Image.FromStream(ms);
            }
        }

        public Image toGraphImage()
        {
            var getStartProcessQuery = new GetStartProcessQuery();
            var getProcessStartInfoQuery = new GetProcessStartInfoQuery();
            var registerLayoutPluginCommand = new RegisterLayoutPluginCommand(getProcessStartInfoQuery, getStartProcessQuery);

            var wrapper = new GraphGeneration(getStartProcessQuery,
                                              getProcessStartInfoQuery,
                                              registerLayoutPluginCommand);

            StringBuilder sb = new StringBuilder("graph{");

            for (int n1 = 0; n1 < NodesCount; n1++)
            {
                bool vanekapcsolata = false;

                for (int n2 = n1 + 1; n2 < NodesCount; n2++)
                    if (AdjMatrix[n1, n2] != Double.PositiveInfinity)
                    {
                        vanekapcsolata = true;
                        sb.AppendLine(String.Format("n{0}--n{1}", n1, n2));
                    }

                if (!vanekapcsolata)
                    sb.AppendLine(String.Format("n{0}", n1));
            }

            sb.AppendLine("}");

            byte[] output = wrapper.GenerateGraph(sb.ToString(), Enums.GraphReturnType.Png);

            using (MemoryStream ms = new MemoryStream(output))
            {
                return Image.FromStream(ms);
            }
        }

        public Image StringToImage(String s)
        {
            var getStartProcessQuery = new GetStartProcessQuery();
            var getProcessStartInfoQuery = new GetProcessStartInfoQuery();
            var registerLayoutPluginCommand = new RegisterLayoutPluginCommand(getProcessStartInfoQuery, getStartProcessQuery);

            var wrapper = new GraphGeneration(getStartProcessQuery,
                                              getProcessStartInfoQuery,
                                              registerLayoutPluginCommand);

            wrapper.RenderingEngine = Enums.RenderingEngine.Fdp;

            byte[] output = wrapper.GenerateGraph(s, Enums.GraphReturnType.Png);

            using (MemoryStream ms = new MemoryStream(output))
            {
                return Image.FromStream(ms);
            }
        }

        public Image BFSStepToImage(List<bool> Visited, Queue<int> Nodes)
        {
            StringBuilder sb = new StringBuilder("graph {");
            sb.AppendLine("node [style=filled]");

            for (int i = 0; i < NodesCount; i++)
            {
                sb.Append(String.Format("n{0}", i));
                if (Visited[i])
                    sb.AppendLine("[color = red];");
                else
                {
                    int idx = Nodes.ToList().IndexOf(i);

                    switch (idx)
                    {
                        case -1: sb.AppendLine(";"); break;
                        case 0: sb.AppendLine("[color = yellow];"); break;
                        //case 1: sb.AppendLine("[color = yellow];"); break;
                        default: sb.AppendLine("[color = green];"); break;
                    }
                }
            }

            for (int n1 = 0; n1 < NodesCount; n1++)
                for (int n2 = n1 + 1; n2 < NodesCount; n2++)
                    if (AdjMatrix[n1, n2] != Double.PositiveInfinity)
                        sb.AppendLine(String.Format("n{0}--n{1};", n1, n2));

            sb.AppendLine("}");

            return StringToImage(sb.ToString());
        }

        public void BFSStepInit(out List<bool> Visited, out Queue<int> Nodes)
        {
            Visited = new List<bool>(new bool[NodesCount]);
            Nodes = new Queue<int>();
            Nodes.Enqueue(0);
        }

        public bool BFSStep(List<bool> Visited, Queue<int> Nodes)
        {
            if (Nodes.Count == 0)
                return false;

            int n_index = Nodes.Dequeue();
            Visited[n_index] = true;
            for (int i = 0; i < NodesCount; i++)
                if (i != n_index) // Nem önmaga
                    if (!Visited[i]) // Vigyáz arra hogy ciklikus gráf esetében ne legyen végtelen ciklus, ne térjünk vissza már feldolgozott csomóponthoz
                        if (AdjMatrix[n_index, i] != Double.PositiveInfinity)
                            if (Nodes.ToList().IndexOf(i) < 0)
                                Nodes.Enqueue(i);

            return Nodes.Count > 0;
        }

        public bool BFSNextComponent(List<bool> Visited, Queue<int> Nodes)
        {
            for (int i = 0; i < NodesCount; i++)
                if (!Visited[i])
                {
                    Nodes.Enqueue(i);
                    return true;
                }

            return false;
        }

        public void DFS(int NodeIndex, List<bool> Visited)
        {
            Visited[NodeIndex] = true;

            for (int i = 0; i < NodesCount; i++)
                if (AdjMatrix[NodeIndex, i] != Double.PositiveInfinity)
                    if (!Visited[i])
                        DFS(i, Visited);
        }

        public int DFSBejaras()
        {
            List<bool> Visited = new List<bool>(new bool[NodesCount]);

            int needtovisit;

            int komponensekszama = 0;

            while (true)
            {
                needtovisit = -1;

                for (int i = 0; i < NodesCount; i++)
                    if (!Visited[i])
                    {
                        needtovisit = i;
                        break;
                    }

                if (needtovisit == -1)
                    break;

                komponensekszama++;
                DFS(needtovisit, Visited);
            }

            return komponensekszama;
        }

        public void DijkstraInit(out List<bool> Processed, out List<double> Cost, out List<int> Path)
        {
            Processed = new List<bool>(new bool[this.NodesCount]);
            Cost = new List<double>(new double[this.NodesCount]);
            Path = new List<int>(new int[this.NodesCount]);

            for (int i = 0; i < Cost.Count; i++)
            {
                Processed[i] = false;
                Cost[i] = double.PositiveInfinity;
                Path[i] = -1;
            }

            Cost[0] = 0;
        }

        public void DijkstraStep(List<bool> Processed, List<double> Cost, List<int> Path)
        {
            double min = double.PositiveInfinity;
            int minindex = NodesCount;

            // keresés tétele
            // keressük az első még fel nem dolgozott csomópontot
            for (int i = 0; i < NodesCount; i++)
                if (Processed[i] == false)
                {
                    min = Cost[i];
                    minindex = i;
                    break;
                }

            if (minindex == NodesCount)
                return;

            // minimumkeresés tétele
            for (int i = minindex + 1; i < Cost.Count; i++)
                if (Cost[i] < min && Processed[i] == false)
                {
                    min = Cost[i];
                    minindex = i;
                }

            for (int i = 0; i < NodesCount; i++)
                if (minindex != i)
                {
                    if (AdjMatrix[minindex, i] != double.PositiveInfinity)
                    {
                        double koltseg =
                            Cost[minindex]  // jelenlegi csúcspontba utazás költsége
                            + AdjMatrix[minindex, i]; // i. csúcspontba történő utazás költsége

                        if (Cost[i] > koltseg)
                        {
                            Cost[i] = koltseg;
                            Path[i] = minindex;
                        }
                    }
                }

            Processed[minindex] = true;
        }

        public Image DijkstraStepToImage(List<bool> Processed, List<double> Cost, List<int> Path)
        {
            StringBuilder sb = new StringBuilder("graph{");

            for (int i = 0; i < NodesCount; i++)
            {
                if (Processed[i])
                    sb.AppendLine(String.Format("n{0} [color = red];", i));
                else
                    sb.AppendLine(String.Format("n{0};", i));
            }

            for (int n1 = 0; n1 < NodesCount; n1++)
                for (int n2 = n1 + 1; n2 < NodesCount; n2++)
                    if (AdjMatrix[n1, n2] < double.PositiveInfinity)
                        if (Path[n1] == n2 || Path[n2] == n1)
                            sb.AppendLine(String.Format("n{0}--n{1} [color=red];", n1, n2));
                        else
                            sb.AppendLine(String.Format("n{0}--n{1};", n1, n2));

            sb.AppendLine("}");

            return StringToImage(sb.ToString());
        }
    }
}
