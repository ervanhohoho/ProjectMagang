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
    public partial class SummaryTransaksiVault : Form
    {
        Database1Entities db = new Database1Entities();
        List<TableView> output = new List<TableView>();
        int sortColIdx = 0;
        bool asc = true;

        public SummaryTransaksiVault()
        {
            InitializeComponent();
            reloadTahun();
            comboTahun.SelectedIndex = 0;
            reloadBulan();
            loadData();
        }
        public void reloadBulan()
        {
            int tahun = Int32.Parse(comboTahun.SelectedItem.ToString());
            var query = (from x in db.RekonSaldoVaults.AsEnumerable()
                         where ((DateTime)x.dueDate).Year == tahun
                         select ((DateTime)x.dueDate).Month).Distinct().ToList();
            comboBulan.DataSource = query;
        }

        public void reloadTahun()
        {
            var query = (from x in db.RekonSaldoVaults.AsEnumerable()
                         select ((DateTime)x.dueDate).Year).Distinct().ToList();
            comboTahun.DataSource = query;
        }
        public void loadData()
        {
            int bulan = Int32.Parse(comboBulan.SelectedItem.ToString());
            int tahun = Int32.Parse(comboTahun.SelectedItem.ToString());
            var query = (from x in db.RekonSaldoVaults.AsEnumerable()
                         join y in db.Pkts on x.vaultId.Substring(0, 4) equals (y.kodePktCabang.Length >= 4 ? (y.kodePktCabang.Substring(0, 4) == "CCAS" ? "CCAS" : y.kodePktCabang) : " ")
                         where ((DateTime)x.dueDate).Month == bulan && ((DateTime)x.dueDate).Year == tahun
                         && (x.actionRekon.Contains("Delivery") && x.statusRekon.Contains("Confirmed")
                            || x.actionRekon.Contains("Return") && x.statusRekon.Contains("Transit"))
                         group x by new { x.vaultId, y.namaPkt, x.fundingSoure, x.dueDate, timeStampRekon = ((DateTime)x.timeStampRekon), x.actionRekon } into z
                         select new TableView()
                         {
                             vaultId = z.Key.vaultId,
                             namaPkt = z.Key.namaPkt,
                             fundingSoure = z.Key.fundingSoure,
                             dueDate = (DateTime)z.Key.dueDate,
                             timeStampRekon = z.Key.timeStampRekon,
                             txnType = z.Key.actionRekon,
                             amount = (Int64)z.Sum(a => a.currencyAmmount)
                         }).ToList();
            output = query;
            dataGridView1.DataSource = output;
            for (int a = 0; a < dataGridView1.ColumnCount; a++)
            {
                if (dataGridView1.Columns[a].ValueType == typeof(Int64))
                {
                    dataGridView1.Columns[a].DefaultCellStyle.Format = "C0";
                    dataGridView1.Columns[a].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
                }
            }
        }
        public class TableView
        {
            public String vaultId { set; get; }
            public String namaPkt { set; get; }
            public String fundingSoure { set; get; }
            public DateTime dueDate { set; get; }
            public DateTime timeStampRekon { set; get; }
            public String txnType { set; get; }
            public Int64 amount { set; get; }
        }

        private void combo_SelectionChangeCommitted(object sender, EventArgs e)
        {
            loadData();
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
                if (colName == "vaultId")
                    dataGridView1.DataSource = output.OrderBy(x => x.vaultId).ToList();
                else if (colName == "namaPkt")
                    dataGridView1.DataSource = output.OrderBy(x => x.namaPkt).ToList();
                else if (colName == "fundingSoure")
                    dataGridView1.DataSource = output.OrderBy(x => x.fundingSoure).ToList();
                else if (colName == "dueDate")
                    dataGridView1.DataSource = output.OrderBy(x => x.dueDate).ToList();
                else if (colName == "timeStampRekon")
                    dataGridView1.DataSource = output.OrderBy(x => x.timeStampRekon).ToList();
                else if (colName == "txnType")
                    dataGridView1.DataSource = output.OrderBy(x => x.txnType).ToList();
                else if (colName == "amount")
                    dataGridView1.DataSource = output.OrderBy(x => x.amount).ToList();
            }
            else
            {
                if (colName == "vaultId")
                    dataGridView1.DataSource = output.OrderByDescending(x => x.vaultId).ToList();
                else if (colName == "namaPkt")
                    dataGridView1.DataSource = output.OrderByDescending(x => x.namaPkt).ToList();
                else if (colName == "fundingSoure")
                    dataGridView1.DataSource = output.OrderByDescending(x => x.fundingSoure).ToList();
                else if (colName == "dueDate")
                    dataGridView1.DataSource = output.OrderByDescending(x => x.dueDate).ToList();
                else if (colName == "timeStampRekon")
                    dataGridView1.DataSource = output.OrderByDescending(x => x.timeStampRekon).ToList();
                else if (colName == "txnType")
                    dataGridView1.DataSource = output.OrderByDescending(x => x.txnType).ToList();
                else if (colName == "amount")
                    dataGridView1.DataSource = output.OrderByDescending(x => x.amount).ToList();
            }
            Console.WriteLine(props[e.ColumnIndex].Name);
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog sv = new SaveFileDialog();
            sv.Filter = Variables.csvFilter;

            if (sv.ShowDialog() == DialogResult.OK)
            {
                String csv = ServiceStack.Text.CsvSerializer.SerializeToCsv(output);
                File.WriteAllText(sv.FileName, csv);
            }
        }

        private void comboTahun_SelectionChangeCommitted(object sender, EventArgs e)
        {
            reloadBulan();
        }
    }
  
}
