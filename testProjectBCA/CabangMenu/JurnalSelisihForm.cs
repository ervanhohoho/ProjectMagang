using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testProjectBCA.CabangMenu
{
    public partial class JurnalSelisihForm : Form
    {
        String kodePkt;
        int selectedYear = -1, selectedMonth = -1;
        Database1Entities db;
        DataTable tableHasil;
        public JurnalSelisihForm()
        {
            InitializeComponent();
            db = new Database1Entities();
            loadPktCombo();
            loadTahunComboBox();
            loadBulanComboBox();
        }
        void loadPktCombo()
        {
            Database1Entities db = new Database1Entities();
            List<String> listPkt = (from x in db.OrderTrackings
                                    join y in db.Pkts on x.kodePkt equals y.kodePktCabang
                                    where y.kanwil.ToLower().Contains("jabo")
                                    select x.kodePkt.Contains("CCAS") ? "CCAS" : x.kodePkt).Distinct().ToList();
            pktComboBox.DataSource = listPkt;
            pktComboBox.SelectedIndex = 0;
            kodePkt = pktComboBox.SelectedItem.ToString();
        }
        void loadTahunComboBox()
        {
            List<int> listtahun = (from x in db.OrderTrackings
                                   where x.kodePkt == kodePkt
                                   select ((DateTime)x.tanggal).Year).OrderBy(x => x).Distinct().ToList();
            tahunComboBox.DataSource = listtahun;
            if (listtahun.Contains(selectedYear))
                tahunComboBox.SelectedItem = selectedYear;
            else if (listtahun.Any())
            {
                tahunComboBox.SelectedIndex = 0;
                selectedYear = Int32.Parse(tahunComboBox.SelectedItem.ToString());
            }
        }
        void loadBulanComboBox()
        {
            List<int> listBulan = (from x in db.OrderTrackings
                                   where x.kodePkt == kodePkt
                                   && ((DateTime)x.tanggal).Year == selectedYear
                                   select ((DateTime)x.tanggal).Month).OrderBy(x => x).Distinct().ToList();
            bulanComboBox.DataSource = listBulan;
            if (listBulan.Contains(selectedMonth))
                bulanComboBox.SelectedItem = selectedMonth;
            else if (listBulan.Any())
            {
                bulanComboBox.SelectedIndex = 0;
                selectedMonth = Int32.Parse(bulanComboBox.SelectedItem.ToString());
            }
        }
        private void loadBtn_Click(object sender, EventArgs e)
        {
            List<OrderTracking> source = (from x in db.OrderTrackings
                                          where x.kodePkt == kodePkt
                                          && (((DateTime)x.dueDate).Year == selectedYear || ((DateTime)x.tanggal).Year == selectedYear)
                                          && (((DateTime)x.dueDate).Month == selectedMonth || ((DateTime)x.tanggal).Month == selectedMonth)
                                          select x).ToList();
            List<DateTime> listDueDate = source.Select(x => (DateTime)x.dueDate).OrderBy(x => x).Distinct().ToList();
            List<DateTime> listTanggal = source.Select(x => (DateTime)x.tanggal).OrderBy(x => x).Distinct().ToList();
            DataTable table = new DataTable();
            table.Columns.Add("Due Date");
            foreach (var temp in listTanggal)
            {
                table.Columns.Add(temp.ToString("dd-MMM-yyyy"), typeof(Int64));
            }
            foreach(var temp in listDueDate)
            {
                Console.WriteLine("Tanggal: " + temp.ToString("dd-MMM-yyyy"));
                DataRow row = table.NewRow();
                var tempSrc = source
                    .Where(x => x.dueDate == temp)
                    .GroupBy(x=>new { x.dueDate, x.tanggal })
                    .Select(x=> new { x.Key.tanggal, x.Key.dueDate, nominalDispute = x.Sum(y => y.nominalDispute) })
                    .ToList();
                row["Due Date"] = temp.ToString("dd-MMM-yyyy");
                var listTanggalSrc = tempSrc.Select(x => x.tanggal).Distinct().ToList();
                foreach (var tanggalSrc in listTanggalSrc)
                {
                    row[((DateTime)tanggalSrc).ToString("dd-MMM-yyyy")] = tempSrc.Where(x=>x.tanggal == tanggalSrc).Select(x=>x.nominalDispute).First();
                }
                table.Rows.Add(row);
            }
            tableHasil = table;
            dataGridView1.DataSource = tableHasil;
            for(int a=0;a<dataGridView1.Columns.Count;a++)
            {
                if(dataGridView1.Columns[a].ValueType == typeof(Int64))
                {
                    dataGridView1.Columns[a].DefaultCellStyle.Format = "C0";
                    dataGridView1.Columns[a].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
                }
            }
        }

        private void tahunComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            selectedYear = Int32.Parse(tahunComboBox.SelectedItem.ToString());
            loadBulanComboBox();
        }

        private void bulanComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedMonth = Int32.Parse(bulanComboBox.SelectedItem.ToString());
        }

        private void exportBtn_Click(object sender, EventArgs e)
        {
            SaveFileDialog sv = new SaveFileDialog();
            sv.Filter = Variables.csvFilter;
            if (sv.ShowDialog() == DialogResult.OK)
            {
                DataTable dt = tableHasil;
                StringBuilder sb = new StringBuilder();
                string[] columnNames = dt.Columns.Cast<DataColumn>().
                                      Select(column => column.ColumnName).
                                      ToArray();
                sb.AppendLine(string.Join(",", columnNames));

                foreach (DataRow row in dt.Rows)
                {
                    string[] fields = row.ItemArray.Select(field => field.ToString()).
                                                    ToArray();
                    sb.AppendLine(string.Join(",", fields));
                }

                File.WriteAllText(sv.FileName, sb.ToString());
                MessageBox.Show("Done!");
            }
        }

        private void pktComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            kodePkt = pktComboBox.SelectedItem.ToString();
            loadTahunComboBox();
            loadBulanComboBox();
        }
    }
}
