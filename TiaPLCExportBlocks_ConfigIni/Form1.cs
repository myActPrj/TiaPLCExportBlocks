using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TiaPLCExportBlocks_ConfigIni
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private void btn_SelectFileProject_Click(object sender, EventArgs e)
        {
            //openFileDialog1.Filter = "*.zip";
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                MessageBox.Show("OK");
            }
        }


    }
}
