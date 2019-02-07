using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testProjectBCA.CabangMenu
{
    public partial class SummaryTransaksiCabang : Form
    {
        Database1Entities en = new Database1Entities();
        List<TableView> tableView = new List<TableView>();
        int sortColIdx = 0;
        bool asc = true;

        public SummaryTransaksiCabang()
        {
            InitializeComponent();
            reloadBulan();
            reloadTahun();
            reloadSummaryTransaksiCabang();
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
        public void reloadSummaryTransaksiCabang()
        {
            var query = (from x in en.RekonSaldoPerVendors.AsEnumerable()
                         join y in en.Cabangs on x.cashPointtId.TrimStart('B') equals y.kodeCabang
                         where ((DateTime)x.dueDate).Month.ToString() == comboBulan.SelectedValue.ToString()
                         && ((DateTime)x.dueDate).Year.ToString() == comboTahun.SelectedValue.ToString()
                         && (x.actionRekon.Contains("Delivery") && x.statusRekon.Contains("Confirmed")
                            || x.actionRekon.Contains("Return") && x.statusRekon.Contains("Transit"))
                         group x by new { x.cashPointtId, y.namaCabang, x.vendor, x.dueDate, blogTime = ((DateTime)x.blogTime), x.actionRekon } into z
                         select new
                         {
                             cashPointId = z.Key.cashPointtId,
                             branch = z.Key.namaCabang,
                             vendor = z.Key.vendor,
                             dueDate = z.Key.dueDate,
                             blogTime = z.Key.blogTime,
                             txnType = z.Key.actionRekon,
                             amount = z.Sum(a => a.currencyAmmount)
                         }
                         ).ToList();

           tableView = new List<TableView>();

            foreach (var item in query)
            {
                tableView.Add(new TableView
                {
                    amount = Int64.Parse(item.amount.ToString()),
                    blogTime = item.blogTime,
                    branch = item.branch,
                    cashpointId = item.cashPointId,
                    dueDate = DateTime.Parse(item.dueDate.ToString()),
                    txnType = item.txnType.ToString(),
                    vendor = item.vendor.ToString()
                });
            }

            dataGridView1.DataSource = tableView;
            formatting();
        }

        private void comboBulan_SelectionChangeCommitted(object sender, EventArgs e)
        {
            reloadSummaryTransaksiCabang();
        }

        private void comboTahun_SelectionChangeCommitted(object sender, EventArgs e)
        {
            reloadSummaryTransaksiCabang();
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
                else if (colName == "dueDate")
                    dataGridView1.DataSource = tableView.OrderBy(x => x.dueDate).ToList();
                else if (colName == "blogTime")
                    dataGridView1.DataSource = tableView.OrderBy(x => x.blogTime).ToList();
                else if (colName == "txnType")
                    dataGridView1.DataSource = tableView.OrderBy(x => x.txnType).ToList();
                else if (colName == "amount")
                    dataGridView1.DataSource = tableView.OrderBy(x => x.amount).ToList();
            }
            else
            {
                if (colName == "cashPointId")
                    dataGridView1.DataSource = tableView.OrderByDescending(x => x.cashpointId).ToList();
                else if (colName == "branch")
                    dataGridView1.DataSource = tableView.OrderByDescending(x => x.branch).ToList();
                else if (colName == "vendor")
                    dataGridView1.DataSource = tableView.OrderByDescending(x => x.vendor).ToList();
                else if (colName == "dueDate")
                    dataGridView1.DataSource = tableView.OrderByDescending(x => x.dueDate).ToList();
                else if (colName == "blogTime")
                    dataGridView1.DataSource = tableView.OrderByDescending(x => x.blogTime).ToList();
                else if (colName == "txnType")
                    dataGridView1.DataSource = tableView.OrderByDescending(x => x.txnType).ToList();
                else if (colName == "amount")
                    dataGridView1.DataSource = tableView.OrderByDescending(x => x.amount).ToList();
            }
            Console.WriteLine(props[e.ColumnIndex].Name);
        }

        public void formatting()
        {
            if (dataGridView1.Rows.Count > 0)
            {
                dataGridView1.Columns[6].DefaultCellStyle.Format = "C0";
                dataGridView1.Columns[6].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("ID-id");

            }
        }

        class TableView
        {
            public String cashpointId { set; get; }
            public String branch { set; get; }
            public String vendor { set; get; }
            public DateTime dueDate { set; get; }
            public DateTime blogTime { set; get; }
            public String txnType { set; get; }
            public Int64 amount { set; get; }
        }
    }
}
