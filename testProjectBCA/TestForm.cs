using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testProjectBCA
{
    public partial class TestForm : Form
    {

        List<int> a = new List<int>() { 1, 2, 3, 4, 5 };
        List<int> b;
        public TestForm()
        {
            InitializeComponent();
            b = new List<int>(a);

            dataGridView1.DataSource = a.Select(x=>new { value = x }).ToList();
            dataGridView2.DataSource = b.Select(x => new { value = x }).ToList();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int x = 0; x < b.Count; x++)
                b[x] = b[x]+1;

            dataGridView2.DataSource = b.Select(x => new { value = x }).ToList();
            dataGridView1.DataSource = a.Select(x => new { value = x }).ToList();
            dataGridView2.Refresh();
            dataGridView1.Refresh();
        }
    }
}
