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
    public partial class DetailTransaksiVault : Form
    {
        Database1Entities en = new Database1Entities();
        List<ViewDetailTransaksiVault> viewDetailTransaksiVaults = new List<ViewDetailTransaksiVault>();
        public DetailTransaksiVault()
        {
            InitializeComponent();
            reloadComboPkt();
            //dateTimePicker1.Visible = false;
            comboSetBon.SelectedIndex = 0;
            comboVal.SelectedIndex = 0;
            reloadOutBelum();
        }
        public void reloadComboPkt()
        {
            //List<String> vaultId = new List<string>();

            //var query = (from x in en.RekonSaldoVaults
            //             join y in en.Pkts on x.vaultId.Substring(0,4) equals y.kodePktCabang == "CCASA" ?  "CCAS" : y.kodePktCabang into j
            //             from y in j.DefaultIfEmpty()
            //             //where x.vaultId.Contains("CCAS") || y.kanwil.ToLower().Contains("jabo")
            //             select x.vaultId.Substring(0,4) == "CCAS" ? "CCAS" : x.vaultId).Distinct().OrderBy(x=>x).ToList();

            //foreach (var item in query)
            //{
            //    vaultId.Add(item.ToString());
            //}
            List<String> fundingSource = new List<string>() {"BI","Bank Lain","ATM", "Cabang", "All"};
            comboPkt.DataSource = fundingSource;
        }
        public List<RekonSaldoVault> loadPrequery()
        {
            String fundingSource = comboPkt.SelectedItem.ToString();
            if (fundingSource == "Bank Lain")
                fundingSource = "OB";
            var prequery = (
                from x in en.RekonSaldoVaults.AsEnumerable()
                join y in en.Pkts on x.vaultId.Substring(0, 4) equals y.kodePktCabang == "CCASA" ? "CCAS" : y.kodePktCabang
                where (!String.IsNullOrWhiteSpace(x.fundingSoure)? (!y.kanwil.ToLower().Contains("jabo") ? x.fundingSoure.ToLower().Substring(0, 2) == "ob" || x.fundingSoure.ToLower().Substring(0, 2) == "bi" : true) : false)
                select x
            ).ToList();
            if (fundingSource != "All")
            {
                Console.WriteLine("!ALL");
                prequery = prequery.Where(x => (x.fundingSoure != null ?
                    ((fundingSource == "BI" || fundingSource == "OB") ?
                        x.fundingSoure.ToLower().Substring(0, 2) == fundingSource.ToLower() :
                        x.fundingSoure.ToLower().Substring(0, 2) != "bi" && x.fundingSoure.ToLower().Substring(0, 2) != "ob"
                    ) :
                    false)).ToList();
                if(fundingSource == "ATM")
                {
                    Database1Entities db = new Database1Entities();
                    List<String> listKodePktATM = db.Pkts.AsEnumerable().Where(x => !String.IsNullOrWhiteSpace(x.kodeOpti)).Select(x => x.kodeOpti).Distinct().ToList();
                    prequery = prequery.Where(x => listKodePktATM.Contains(x.fundingSoure)).ToList();
                }
                else if(fundingSource == "Cabang")
                {
                    Database1Entities db = new Database1Entities();
                    List<String> listKodePktCabang = db.Pkts.AsEnumerable().Where(x=>!String.IsNullOrWhiteSpace(x.kodePktCabang)).Select(x => x.kodePktCabang).Distinct().ToList();
                    prequery = prequery.Where(x => listKodePktCabang.Contains(x.fundingSoure)).ToList();
                }
            }
            else
                Console.WriteLine("ALL");
            return prequery;
        }
        public List<ViewDetailTransaksiVault> loadHasil(String actionRekon, bool validated)
        {
            DateTime date2 = dateTimePicker2.Value.Date;
            String val = validated ? "VALIDATED" : "NOT VALIDATED";
            List<RekonSaldoVault> prequery = loadPrequery();
            var emergency = (
                from x in prequery
                where x.actionRekon.Contains(actionRekon) && x.statusRekon.Equals("Confirmed") && (DateTime.Parse(x.dueDate.ToString()).Date == date2 || DateTime.Parse(x.realDate.ToString()) == date2)
                && x.actionRekon.ToLower().Contains("emergency")
                group x by new { x.dueDate, x.fundingSoure, x.timeStampRekon, x.actionRekon, x.statusRekon, x.blogMessage, x.vaultId } into z
                select new
                {
                    z.Key.vaultId,
                    z.Key.timeStampRekon,
                    z.Key.actionRekon,
                    z.Key.statusRekon,
                    z.Key.blogMessage,
                    z.Key.dueDate,
                    z.Key.fundingSoure,
                    emergency = z.Sum(x => x.currencyAmmount),
                    validation = z.Key.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(z.Key.timeStampRekon.ToString()).Hour < 21 ? z.Key.timeStampRekon : DateTime.Parse(z.Key.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                }).ToList();
            var regular = (
                from x in prequery
                where x.actionRekon.Contains(actionRekon) && x.statusRekon.Equals("Confirmed") && (DateTime.Parse(x.dueDate.ToString()).Date == dateTimePicker2.Value.Date || DateTime.Parse(x.realDate.ToString()) == dateTimePicker2.Value.Date)
                && !x.actionRekon.ToLower().Contains("emergency")
                group x by new { x.dueDate, x.fundingSoure, x.timeStampRekon, x.actionRekon, x.statusRekon, x.blogMessage, x.vaultId } into z
                select new
                {
                    z.Key.vaultId,
                    z.Key.timeStampRekon,
                    z.Key.actionRekon,
                    z.Key.statusRekon,
                    z.Key.blogMessage,
                    z.Key.dueDate,
                    z.Key.fundingSoure,
                    regular = z.Sum(x => x.currencyAmmount),
                    validation = z.Key.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(z.Key.timeStampRekon.ToString()).Hour < 21 ? z.Key.timeStampRekon : DateTime.Parse(z.Key.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                }).ToList();
            var prepareL = (from x in regular
                            join y in emergency on new { x.timeStampRekon, x.dueDate, x.fundingSoure, x.vaultId } equals new { y.timeStampRekon, y.dueDate, y.fundingSoure, y.vaultId } into temp
                            from y in temp.DefaultIfEmpty()
                            select new
                            {
                                x.vaultId,
                                x.timeStampRekon,
                                x.actionRekon,
                                x.statusRekon,
                                x.blogMessage,
                                x.dueDate,
                                x.fundingSoure,
                                x.regular,
                                emergency = (Int64?) 0,
                                validation = x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.timeStampRekon.ToString()).Hour < 21 ? x.timeStampRekon : DateTime.Parse(x.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                            }).ToList();
            var prepareR = (from x in emergency
                            join y in regular on new { x.timeStampRekon, x.dueDate, x.fundingSoure, x.vaultId } equals new { y.timeStampRekon, y.dueDate, y.fundingSoure, y.vaultId } into temp
                            from y in temp.DefaultIfEmpty()
                            select new
                            {
                                x.vaultId,
                                x.timeStampRekon,
                                x.actionRekon,
                                x.statusRekon,
                                x.blogMessage,
                                x.dueDate,
                                x.fundingSoure,
                                regular = (Int64?)0,
                                x.emergency,
                                validation = x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.timeStampRekon.ToString()).Hour < 21 ? x.timeStampRekon : DateTime.Parse(x.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                            }).ToList();
            var prepare = prepareL.Union(prepareR);
            var query = (from x in prepare
                         where x.actionRekon.Contains(actionRekon) && x.validation.Equals(val)
                         group x by new { x.vaultId, x.dueDate, x.fundingSoure, x.timeStampRekon } into z
                         select new { z.Key.vaultId, dueDate = z.Key.dueDate, timeStampRekon = z.Key.timeStampRekon, fundingSource = z.Key.fundingSoure, regular = z.Sum(x => x.regular), emergency = z.Sum(x => x.emergency), total = z.Sum(x => x.emergency + x.regular) }).ToList();

            List<ViewDetailTransaksiVault> ret = query.Select(x => new ViewDetailTransaksiVault()
            {
                vaultId = x.vaultId,
                dueDate = x.dueDate,
                emergency = x.emergency,
                fundingSource = x.fundingSource,
                timeStampRekon = x.timeStampRekon,
                total = x.total,
                regular = x.regular,
                inout = actionRekon.ToLower() == "delivery" ? "In" : "Out",
                validasi = validated ? "Validasi" : "Belum Validasi",
                koordinator = (from y in en.Pkts where y.kodePktCabang == (x.vaultId.Substring(0, 4) == "CCAS" ? "CCASA" : x.vaultId) select y.koordinator).Distinct().FirstOrDefault()
            }).ToList();
            return ret;
        }
        public void reloadOutBelum()
        {
            viewDetailTransaksiVaults = loadHasil("Return", false);
            dataGridView1.DataSource = viewDetailTransaksiVaults;
            formatting();
        }
        public void reloadOutSudah()
        {
            viewDetailTransaksiVaults = loadHasil("Return", true);
            dataGridView1.DataSource = viewDetailTransaksiVaults;
            formatting();
        }
        public void reloadInSudah()
        {
            viewDetailTransaksiVaults = loadHasil("Delivery", true);
            dataGridView1.DataSource = viewDetailTransaksiVaults;
            formatting();
        }
        public void reloadInBelum()
        {
            viewDetailTransaksiVaults = loadHasil("Delivery", false);
            dataGridView1.DataSource = viewDetailTransaksiVaults;
            formatting();
        }
        public void reloadAllInOut()
        {
            if (comboVal.SelectedIndex == 2)
            {
                Console.WriteLine("RELOAD ALL");
                reloadAll();
            }
            else if (comboVal.SelectedIndex == 0)
            {

                viewDetailTransaksiVaults = new List<ViewDetailTransaksiVault>();
                viewDetailTransaksiVaults.AddRange(loadHasil("Return", true));
                viewDetailTransaksiVaults.AddRange(loadHasil("Delivery", true));
                dataGridView1.DataSource = viewDetailTransaksiVaults;
            }
            else if (comboVal.SelectedIndex == 1)
            {
                viewDetailTransaksiVaults = new List<ViewDetailTransaksiVault>();
                viewDetailTransaksiVaults.AddRange(loadHasil("Return", false));
                viewDetailTransaksiVaults.AddRange(loadHasil("Delivery", false));
                dataGridView1.DataSource = viewDetailTransaksiVaults;
            }
            formatting();
        }
        public void reloadAllValidasi()
        {
            if (comboSetBon.SelectedIndex == 2)
            {
                Console.WriteLine("RELOAD ALL");
                reloadAll();
            }
            else if (comboSetBon.SelectedIndex == 0)
            {
                viewDetailTransaksiVaults = new List<ViewDetailTransaksiVault>();
                viewDetailTransaksiVaults.AddRange(loadHasil("Return", true));
                viewDetailTransaksiVaults.AddRange(loadHasil("Return", false));
            }
            else if (comboSetBon.SelectedIndex == 1)
            {
                viewDetailTransaksiVaults = new List<ViewDetailTransaksiVault>();
                viewDetailTransaksiVaults.AddRange(loadHasil("Delivery", true));
                viewDetailTransaksiVaults.AddRange(loadHasil("Delivery", false));
            }
            dataGridView1.DataSource = viewDetailTransaksiVaults;
            formatting();
        }
        public void reloadAll()
        {
            viewDetailTransaksiVaults = new List<ViewDetailTransaksiVault>();
            viewDetailTransaksiVaults.AddRange(loadHasil("Return", false));
            viewDetailTransaksiVaults.AddRange(loadHasil("Return", true));
            viewDetailTransaksiVaults.AddRange(loadHasil("Delivery", false));
            viewDetailTransaksiVaults.AddRange(loadHasil("Delivery", true));
            dataGridView1.DataSource = viewDetailTransaksiVaults;
            formatting();
        }
        class loadData
        {
            public DateTime tanggal { set; get; }
            public String kodeCabang { set; get; }
            public String nominal { set; get; }
        }

        public void formatting()
        {
            if (dataGridView1.Rows.Count > 0)
            {
                for (int a = 3; a < dataGridView1.ColumnCount; a++)
                {
                    if (dataGridView1.Columns[a].ValueType == typeof(Int64?))
                    {
                        dataGridView1.Columns[a].DefaultCellStyle.Format = "C0";
                        dataGridView1.Columns[a].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("ID-id");
                    }
                }
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            DataGridViewSelectedCellCollection cells = dataGridView1.SelectedCells;

            foreach (DataGridViewCell cell in cells)
            {
                int rowidx = cell.RowIndex;
                int colidx = cell.ColumnIndex;
                if (colidx >= 2)
                {
                    if (dataGridView1.Columns[colidx].ValueType == typeof(Int64?))
                    {
                        dataGridView1.Rows[rowidx].Cells[colidx].Style.Format = "F0";
                    }
                }
            }
            for (int a = 0; a < dataGridView1.Rows.Count; a++)
            {
                for (int b = 4; b < dataGridView1.Columns.Count; b++)
                {
                    if (!cells.Contains(dataGridView1.Rows[a].Cells[b]))
                    {
                        Int64 buf;
                        if (dataGridView1.Columns[b].ValueType == typeof(Int64?))
                        {
                            dataGridView1.Rows[a].Cells[b].Style.Format = "C0";
                            dataGridView1.Rows[a].Cells[b].Style.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
                        }
                    }
                }
            }
        }

        private void searchTextBox_TextChanged(object sender, EventArgs e)
        {
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {

        }

        private void comboPkt_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (comboSetBon.SelectedIndex == 0 && comboVal.SelectedIndex == 0)
            {
                reloadOutSudah();
            }
            else if (comboSetBon.SelectedIndex == 0 && comboVal.SelectedIndex == 1)
            {
                reloadOutBelum();
            }
            else if (comboSetBon.SelectedIndex == 1 && comboVal.SelectedIndex == 0)
            {
                reloadInSudah();
            }
            else if (comboSetBon.SelectedIndex == 1 && comboVal.SelectedIndex == 1)
            {
                reloadInBelum();
            }
            else if (comboSetBon.SelectedIndex == 2 && comboVal.SelectedIndex == 2)
            {
                reloadAll();
            }
            else if (comboSetBon.SelectedIndex == 2)
            {
                reloadAllInOut();

            }
            else if (comboVal.SelectedIndex == 2)
            {
                reloadAllValidasi();
            }
            dataGridView1.Refresh();
        }

        private void comboVal_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (comboSetBon.SelectedIndex == 0 && comboVal.SelectedIndex == 0)
            {
                reloadOutSudah();
            }
            else if (comboSetBon.SelectedIndex == 0 && comboVal.SelectedIndex == 1)
            {
                reloadOutBelum();
            }
            else if (comboSetBon.SelectedIndex == 1 && comboVal.SelectedIndex == 0)
            {
                reloadInSudah();
            }
            else if (comboSetBon.SelectedIndex == 1 && comboVal.SelectedIndex == 1)
            {
                reloadInBelum();
            }
            else if (comboSetBon.SelectedIndex == 2 && comboVal.SelectedIndex == 2)
            {
                reloadAll();
            }
            else if (comboSetBon.SelectedIndex == 2)
            {
                reloadAllInOut();
            }
            else if (comboVal.SelectedIndex == 2)
            {
                reloadAllValidasi();
            }
            dataGridView1.Refresh();
        }

        private void comboSetBon_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (comboSetBon.SelectedIndex == 0 && comboVal.SelectedIndex == 0)
            {
                reloadOutSudah();
            }
            else if (comboSetBon.SelectedIndex == 0 && comboVal.SelectedIndex == 1)
            {
                reloadOutBelum();
            }
            else if (comboSetBon.SelectedIndex == 1 && comboVal.SelectedIndex == 0)
            {
                reloadInSudah();
            }
            else if (comboSetBon.SelectedIndex == 1 && comboVal.SelectedIndex == 1)
            {
                reloadInBelum();
            }
            else if (comboSetBon.SelectedIndex == 2 && comboVal.SelectedIndex == 2)
            {
                reloadAll();
            }
            else if (comboSetBon.SelectedIndex == 2)
            {
                reloadAllInOut();
            }
            else if (comboVal.SelectedIndex == 2)
            {
                reloadAllValidasi();
            }
            dataGridView1.Refresh();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            if (comboSetBon.SelectedIndex == 0 && comboVal.SelectedIndex == 0)
            {
                reloadOutSudah();
            }
            else if (comboSetBon.SelectedIndex == 0 && comboVal.SelectedIndex == 1)
            {
                reloadOutBelum();
            }
            else if (comboSetBon.SelectedIndex == 1 && comboVal.SelectedIndex == 0)
            {
                reloadInSudah();
            }
            else if (comboSetBon.SelectedIndex == 1 && comboVal.SelectedIndex == 1)
            {
                reloadInBelum();
            }
            else if (comboSetBon.SelectedIndex == 2 && comboVal.SelectedIndex == 2)
            {
                reloadAll();
            }
            else if (comboSetBon.SelectedIndex == 2)
            {
                reloadAllInOut();
            }
            else if (comboVal.SelectedIndex == 2)
            {
                reloadAllValidasi();
            }
            dataGridView1.Refresh();
        }
        private void exportBtn_Click(object sender, EventArgs e)
        {
            SaveFileDialog sv = new SaveFileDialog();
            sv.Filter = Variables.csvFilter;
            if (sv.ShowDialog() == DialogResult.OK)
            {
                String csv = ServiceStack.Text.CsvSerializer.SerializeToCsv(viewDetailTransaksiVaults);
                File.WriteAllText(sv.FileName, csv);
            }
        }
        public class ViewDetailTransaksiVault
        {
           
            public String vaultId { set; get; }
            public String koordinator { set; get; }
            public DateTime? dueDate { set; get; }
            public DateTime? timeStampRekon { set; get; }
            public String fundingSource { set; get; }
            public Int64? regular { set; get; }
            public Int64? emergency { set; get; }
            public Int64? total { set; get; }
            public String inout { set; get; }
            public String validasi { set; get; }


        }

        int sortColIdx = 0;
        bool asc = true;
        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            Type fieldsType = typeof(ViewDetailTransaksiVault);
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
                    dataGridView1.DataSource = viewDetailTransaksiVaults.OrderBy(x => x.vaultId).ToList();
                else if (colName == "dueDate")
                    dataGridView1.DataSource = viewDetailTransaksiVaults.OrderBy(x => x.dueDate).ToList();
                else if (colName == "timeStampRekon")
                    dataGridView1.DataSource = viewDetailTransaksiVaults.OrderBy(x => x.timeStampRekon).ToList();
                else if (colName == "fundingSource")
                    dataGridView1.DataSource = viewDetailTransaksiVaults.OrderBy(x => x.fundingSource).ToList();
                else if (colName == "regular")
                    dataGridView1.DataSource = viewDetailTransaksiVaults.OrderBy(x => x.regular).ToList();
                else if (colName == "emergency")
                    dataGridView1.DataSource = viewDetailTransaksiVaults.OrderBy(x => x.emergency).ToList();
                else if (colName == "total")
                    dataGridView1.DataSource = viewDetailTransaksiVaults.OrderBy(x => x.total).ToList();

            }
            else
            {
                if (colName == "vaultId")
                    dataGridView1.DataSource = viewDetailTransaksiVaults.OrderByDescending(x => x.vaultId).ToList();
                else if (colName == "dueDate")
                    dataGridView1.DataSource = viewDetailTransaksiVaults.OrderByDescending(x => x.dueDate).ToList();
                else if (colName == "timeStampRekon")
                    dataGridView1.DataSource = viewDetailTransaksiVaults.OrderByDescending(x => x.timeStampRekon).ToList();
                else if (colName == "fundingSource")
                    dataGridView1.DataSource = viewDetailTransaksiVaults.OrderByDescending(x => x.fundingSource).ToList();
                else if (colName == "regular")
                    dataGridView1.DataSource = viewDetailTransaksiVaults.OrderByDescending(x => x.regular).ToList();
                else if (colName == "emergency")
                    dataGridView1.DataSource = viewDetailTransaksiVaults.OrderByDescending(x => x.emergency).ToList();
                else if (colName == "total")
                    dataGridView1.DataSource = viewDetailTransaksiVaults.OrderByDescending(x => x.total).ToList();
            }
            Console.WriteLine(props[e.ColumnIndex].Name);
        }
    }
}
