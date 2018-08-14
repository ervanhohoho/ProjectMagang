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
    public partial class PembagianSaldoForm : Form
    {
        List<DataSisaPermintaan> listDataSisaPermintaan;
        List<String> listNamaPkt;
        public PembagianSaldoForm()
        {
            InitializeComponent();
        }
        public PembagianSaldoForm(List<StoreClass> morningBalance100, List<StoreClass> morningBalance50)
        {
            InitializeComponent();
            initPembagianGridView();
            initListSisaPermintaan();
        }
        public void initPembagianGridView()
        {
            Database1Entities db = new Database1Entities();
            //Nama PKT yang ada di ComboBox
            var listNamaPkt = db.Pkts.Where(x => x.kanwil.ToUpper().Contains("JABO") && !x.namaPkt.Contains("Alam Sutera")).Select(x => x.namaPkt).ToList();
            DataGridViewComboBoxColumn pkt = new DataGridViewComboBoxColumn() {
                DataSource = listNamaPkt,
                HeaderText = "namaPkt",
                ValueType = typeof(String)
            };
            this.listNamaPkt = listNamaPkt;
            var listTanggal = db.DetailApprovals.AsEnumerable().Where(x => x.bon100 != -1 && x.tanggal >= Variables.todayDate).Select(x => ((DateTime)x.tanggal).ToShortDateString()).Distinct().ToList();
            DataGridViewComboBoxColumn tgl = new DataGridViewComboBoxColumn()
            {
                DataSource = listTanggal,
                HeaderText = "tanggal",
                ValueType = typeof(String)
            };

            pembagianGridView.Columns.Add(tgl);
            pembagianGridView.Columns[0].Width = 100;

            pembagianGridView.Columns.Add(pkt);
            pembagianGridView.Columns[1].Width = 200;

            pembagianGridView.Columns.Add("100000", "100000");
            pembagianGridView.Columns.Add("50000", "50000");
            pembagianGridView.Columns.Add("20000", "20000");

            pembagianGridView.Columns["100000"].ValueType = typeof(Int64);
            pembagianGridView.Columns["100000"].DefaultCellStyle.Format = "N0";
            pembagianGridView.Columns["100000"].Width = 120;
            pembagianGridView.Columns["50000"].ValueType = typeof(Int64);
            pembagianGridView.Columns["50000"].DefaultCellStyle.Format = "N0";
            pembagianGridView.Columns["50000"].Width = 120;
            pembagianGridView.Columns["20000"].ValueType = typeof(Int64);
            pembagianGridView.Columns["20000"].DefaultCellStyle.Format = "N0";
            pembagianGridView.Columns["20000"].Width = 120;
        }
        public void initListSisaPermintaan()
        {
            listDataSisaPermintaan = new List<DataSisaPermintaan>();
            Database1Entities db = new Database1Entities();
            var q = (from x in db.Approvals
                     join y in db.DetailApprovals on x.idApproval equals y.idApproval
                     join z in db.Pkts on x.kodePkt equals z.kodePkt
                     where x.tanggal == Variables.todayDate && z.kanwil.ToUpper().Contains("JABO")
                     && y.bon100 != -1
                     select new { z.namaPkt, tanggal = (DateTime)y.tanggal, y.bon100, y.bon50,y.bon20, y.adhoc100, y.adhoc50, y.adhoc20 }).ToList();
            
            //Adhoc
            listDataSisaPermintaan.AddRange(
                q.Where(x => (DateTime)x.tanggal == Variables.todayDate)
                .Select(x => new DataSisaPermintaan() {
                    tanggal = x.tanggal,
                    namaPkt = x.namaPkt,
                    d100 = (Int64) x.adhoc100,
                    d50 = (Int64) x.adhoc50,
                    d20 = (Int64) x.adhoc20,
                }).ToList());
            
            //Bon Reguler
            listDataSisaPermintaan.AddRange(
                q.Where(x => (DateTime)x.tanggal > Variables.todayDate)
                .Select(x => new DataSisaPermintaan() {
                    tanggal = x.tanggal,
                    namaPkt = x.namaPkt,
                    d100 = (Int64) x.bon100,
                    d50 = (Int64) x.bon50,
                    d20 = (Int64) x.bon20
                }).ToList());

            sisaGridView.DataSource = listDataSisaPermintaan.OrderBy(x=>x.namaPkt).OrderBy(x=> x.tanggal).ToList();
            sisaGridView.Columns["tanggal"].Width = 70;
            sisaGridView.Columns["namaPkt"].Width = 200;
            sisaGridView.Columns["d100"].DefaultCellStyle.Format = "N0";
            sisaGridView.Columns["d50"].DefaultCellStyle.Format = "N0";
            sisaGridView.Columns["d20"].DefaultCellStyle.Format = "N0";
        }
        private void pembagianGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(numCol_KeyPress);
            if (pembagianGridView.CurrentCell.ColumnIndex == 1) //Desired Column
            {
                TextBox tb = e.Control as TextBox;
                if (tb != null)
                {
                    tb.KeyPress += new KeyPressEventHandler(numCol_KeyPress);
                }
            }
        }


        private void numCol_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void pembagianGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {

            List<DataInputanUser> listDataInputanUser = new List<DataInputanUser>();

            for (int a = 0; a<pembagianGridView.RowCount-1;a++)
            {
                DataGridViewRow row = pembagianGridView.Rows[a];
                listDataInputanUser.Add(new DataInputanUser() {
                    tanggal = DateTime.Parse(row.Cells[0].Value.ToString()),
                    namaPkt = row.Cells[1].Value.ToString(),
                    d100 = Int64.Parse(row.Cells[2].Value.ToString()),
                    d50 = Int64.Parse(row.Cells[3].Value.ToString()),
                    d20 = Int64.Parse(row.Cells[4].Value.ToString())
                });
            }
            initListSisaPermintaan();
            List<DataSisaPermintaan> tempListDataSisa = new List<DataSisaPermintaan>(listDataSisaPermintaan);
            foreach(var temp in listDataInputanUser)
            {
                Console.WriteLine(temp.tanggal.ToShortDateString() + " " + temp.namaPkt);
                
                var q = (from x in tempListDataSisa
                         where x.namaPkt == temp.namaPkt && x.tanggal == temp.tanggal
                         select x).FirstOrDefault();
                if (q != null)
                {
                    Console.WriteLine("NOT NULL");
                    q.d100 -= temp.d100;
                    q.d50 -= temp.d50;
                    q.d20 -= temp.d20;
                    sisaGridView.DataSource = tempListDataSisa.OrderBy(x=>x.namaPkt).OrderBy(x=>x.tanggal).ToList();
                    sisaGridView.Refresh();
                }
            }
        }

        private void pembagianGridView_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells[0].Value = Variables.todayDate.ToShortDateString();
            e.Row.Cells[1].Value = listNamaPkt[0];
            e.Row.Cells[2].Value = 0;
            e.Row.Cells[3].Value = 0;
            e.Row.Cells[4].Value = 0;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var rows = pembagianGridView.SelectedRows;
            foreach (DataGridViewRow row in rows)
                pembagianGridView.Rows.Remove(row);
        }
    }

    public class DataSisaPermintaan
    {
        public DateTime tanggal { set; get; }
        public String namaPkt { set; get; }
        public Int64 d100 { set; get; }
        public Int64 d50 { set; get; }
        public Int64 d20 { set; get; }
    }
    public class DataInputanUser
    {
        public DateTime tanggal { set; get; }
        public String namaPkt { set; get; }
        public Int64 d100 { set; get; }
        public Int64 d50 { set; get; }
        public Int64 d20 { set; get; }
    }
}
