using GraphVizWrapper;
using GraphVizWrapper.Commands;
using GraphVizWrapper.Queries;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace graph1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            mg.genGraph(50);
        }

        MyGraph mg = new MyGraph(40);

        private void generalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = mg.toGraphImage();
            Text =  ";" + mg.DFSBejaras();
        }

        List<bool> Visited;
        Queue<int> Nodes;

        private void bFSStartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mg.BFSStepInit(out Visited, out Nodes);
            pictureBox1.Image = mg.BFSStepToImage(Visited, Nodes);
        }

        private void bFSStepToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!mg.BFSStep(Visited, Nodes))
                mg.BFSNextComponent(Visited, Nodes);

            pictureBox1.Image = mg.BFSStepToImage(Visited, Nodes);
        }
    }
}
