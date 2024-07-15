using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        
        private void Form1_Load(object sender, EventArgs e)
        {
            this.BackColor = Color.Blue;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            panel1.Location = new Point(5, 5);
            panel1.BackColor = Color.Gray;     
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            panel1.Location = new Point(5, 5);
            panel1.Width = this.ClientRectangle.Width - 10;
            panel1.Height = this.ClientRectangle.Height - 10;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
