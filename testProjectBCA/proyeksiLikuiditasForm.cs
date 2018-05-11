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
    public partial class proyeksiLikuiditasForm : Form
    {
        Database1Entities db = new Database1Entities();
        public proyeksiLikuiditasForm()
        {
            InitializeComponent();
            loadBulanPrediksiTreeView();
        }

        void loadBulanPrediksiTreeView()
        {

            DateTime minTanggal;
            DateTime maxTanggal;

            var q = (from x in db.DailyStocks select (DateTime)x.tanggal).ToList();
            minTanggal = q.Min(x => x);
            maxTanggal = q.Max(x => x);
            DateTime tempTanggal = minTanggal;
            int counter = 0;
            while (tempTanggal < maxTanggal)
            {
                bulanPrediksiTreeView.Nodes.Add(tempTanggal.Year.ToString());
                int monthCounter = tempTanggal.Month;
                while (tempTanggal < maxTanggal && monthCounter <= 12)
                {
                    bulanPrediksiTreeView.Nodes[counter].Nodes.Add((monthCounter++).ToString());
                    tempTanggal = tempTanggal.AddMonths(1);
                }
                counter++;
            }
            bulanPrediksiTreeView.CheckBoxes = true;
        }
        private void CheckTreeViewNode(TreeNode node, Boolean isChecked)
        {
            foreach (TreeNode item in node.Nodes)
            {
                item.Checked = isChecked;

                if (item.Nodes.Count > 0)
                {
                    this.CheckTreeViewNode(item, isChecked);
                }
            }
        }

        private void bulanPrediksiTreeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            CheckTreeViewNode(e.Node, e.Node.Checked);
        }
    }
}
