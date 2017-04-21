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

        public Image toImage()
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
    }
}
