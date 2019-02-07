using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testProjectBCA.CabangMenu
{
    public partial class ReportRekonSaldo : Form
    {
        Database1Entities en = new Database1Entities();
        List<TableView> tableView = new List<TableView>();
        int sortColIdx = 0;
        bool asc = true;

        public ReportRekonSaldo()
        {
            InitializeComponent();
            reloadBulan();
            reloadTahun();
            reloadReportRekonSaldo();
        }

        public void reloadBulan()
        {
            var query = (from x in en.RekonSaldoPerVendors.AsEnumerable()
                         select ((DateTime)x.dueDate).Month).Distinct().ToList();
            comboBulan.DataSource = query;
        }

        public void reloadTahun()
        {
            var query = (from x in en.RekonSaldoPerVendors.AsEnumerable()
                         select ((DateTime)x.dueDate).Year).Distinct().ToList();
            comboTahun.DataSource = query;
        }

        public void reloadReportRekonSaldo()
        {
            var query = (from x in en.RekonSaldoPerVendors.AsEnumerable()
                         join y in en.Cabangs on x.cashPointtId.TrimStart('B') equals y.kodeCabang
                         where ((DateTime)x.dueDate).Date.ToString() != ((DateTime)x.blogTime).Date.ToString()
                         && ((DateTime)x.dueDate).Month.ToString() == comboBulan.SelectedValue.ToString()
                         && ((DateTime)x.dueDate).Year.ToString() == comboTahun.SelectedValue.ToString()
                         && (x.actionRekon.Contains("Delivery") && x.statusRekon.Contains("Confirmed")
                            || x.actionRekon.Contains("Return") && x.statusRekon.Contains("Transit"))
                         group x by new { x.cashPointtId, x.vendor, y.namaCabang } into z
                         select new
                         {
                             cashPointId = z.Key.cashPointtId,
                             branch = z.Key.namaCabang,
                             vendor = z.Key.vendor,
                             count = z.Count()
                         }
                         ).ToList();

           tableView = new List<TableView>();

            foreach (var item in query)
            {
                tableView.Add(new TableView
                {
                    cashpointId = item.cashPointId.ToString(),
                    branch = item.branch.ToString(),
                    vendor = item.vendor.ToString(),
                    count = Int64.Parse(item.count.ToString())
                });
            }

            dataGridView1.DataSource = tableView;
        }

        private void comboTahun_SelectionChangeCommitted(object sender, EventArgs e)
        {
            reloadReportRekonSaldo();
        }

        private void comboBulan_SelectionChangeCommitted(object sender, EventArgs e)
        {
            reloadReportRekonSaldo();
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog sv = new SaveFileDialog();
            sv.Filter = Variables.csvFilter;
            if (sv.ShowDialog() == DialogResult.OK)
            {
                String csv = ServiceStack.Text.CsvSerializer.SerializeToCsv(tableView);
                File.WriteAllText(sv.FileName, csv);
            }
        }

        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            Type fieldsType = typeof(TableView);
            PropertyInfo[] props = fieldsType.GetProperties(BindingFlags.Public
            | BindingFlags.Instance);
            int colidx = e.ColumnIndex;
            String colName = props[colidx].Name;
            if (sortColIdx == colidx)
            { 
                Console.WriteLine("Kolom Sama");
                asc = !asc;
            }
            else
            {
                asc = true;
                sortColIdx = colidx;
            }
            if (asc)
            {
                if (colName == "cashPointId")
                    dataGridView1.DataSource = tableView.OrderBy(x => x.cashpointId).ToList();
                else if (colName == "branch")
                    dataGridView1.DataSource = tableView.OrderBy(x => x.branch).ToList();
                else if (colName == "vendor")
                    dataGridView1.DataSource = tableView.OrderBy(x => x.vendor).ToList();
                else if (colName == "count")
                    dataGridView1.DataSource = tableView.OrderBy(x => x.count).ToList();
            }
            else
            {
               if(colName == "cashPointId")
                    dataGridView1.DataSource = tableView.OrderByDescending(x => x.cashpointId).ToList();
                else if (colName == "branch")
                    dataGridView1.DataSource = tableView.OrderByDescending(x => x.branch).ToList();
                else if (colName == "vendor")
                    dataGridView1.DataSource = tableView.OrderByDescending(x => x.vendor).ToList();
                else if (colName == "count")
                    dataGridView1.DataSource = tableView.OrderByDescending(x => x.count).ToList();
            }
            Console.WriteLine(props[e.ColumnIndex].Name);
        }

        class TableView
        {
            public String cashpointId { set; get; }
            public String branch { set; get; }
            public String vendor { set; get; }
            public Int64 count { set; get; }
        }
    }
}
