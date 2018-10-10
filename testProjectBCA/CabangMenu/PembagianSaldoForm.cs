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
        List<DataPermintaanDanSumber> listDataPermintaan;
        List<String> listNamaPkt;
        List<String> listNamaPktSumber;
        List<DataPermintaanDanSumber> listDataSumber;
        List<StoreClass> morningbalance100;
        List<StoreClass> morningbalance50;
        List<StoreClass> newNote100;
        List<StoreClass> newNote50;

        List<ApprovalPembagianSaldo> deliveryCabang;
        List<ApprovalPembagianSaldo> adhocCabang;

        List<DataPermintaanDanSumber> listDataTambahanSumber;
        List<DataPermintaanDanSumber> listDataTambahanPermintaan;
        Double persenUnprocessed;
        public PembagianSaldoForm()
        {
            InitializeComponent();
        }
        public PembagianSaldoForm(List<StoreClass> morningBalance100, List<StoreClass> morningBalance50, List<StoreClass> newNote100, List<StoreClass>newNote50, Double persenUnprocessed, List<ApprovalPembagianSaldo> deliveryCabang, List<ApprovalPembagianSaldo>adhocCabang, List<DataPermintaanDanSumber> listDataTambahanSumber, List<DataPermintaanDanSumber>listDataTambahanPermintaan)
        {
            InitializeComponent();

            this.morningbalance100 = morningBalance100;
            this.morningbalance50 = morningBalance50;
            this.persenUnprocessed = persenUnprocessed;
            this.deliveryCabang = deliveryCabang;
            this.adhocCabang = adhocCabang;
            this.listDataTambahanPermintaan = listDataTambahanPermintaan;
            this.listDataTambahanSumber = listDataTambahanSumber;
            Database1Entities db = new Database1Entities();
            DateTime maxDateDetailApproval = (DateTime) db.DetailApprovals.Where(x => x.bon100 != -1).Max(x => x.tanggal);

            this.morningbalance100 = this.morningbalance100.Where(x => x.tanggal <= maxDateDetailApproval).ToList();
            this.morningbalance50 = this.morningbalance50.Where(x => x.tanggal <= maxDateDetailApproval).ToList();
            if(newNote100.Any())
            {
                foreach (var temp in newNote100)
                {
                    var toedit = morningbalance100.Where(x => x.kodePkt == temp.kodePkt && x.tanggal == temp.tanggal).FirstOrDefault();
                    toedit.val = toedit.val - temp.val;
                }
                foreach (var temp in newNote50)
                {
                    var toedit = morningbalance50.Where(x => x.kodePkt == temp.kodePkt && x.tanggal == temp.tanggal).FirstOrDefault();
                    toedit.val = toedit.val - temp.val;
                }
                this.newNote100 = newNote100;
                this.newNote50 = newNote50;
            }
            else
            {
                this.newNote100 = loadNewNote100();
                this.newNote50 = loadNewNote50();
            }
            initPembagianGridView();
            initListSisaPermintaan();
            initListSumber();
        }
        List<StoreClass> loadNewNote100()
        {
            Database1Entities db = new Database1Entities();
            List<StoreClass> result = new List<StoreClass>();
            var query = (from x in db.StokPosisis
                         select x).ToList();
            var q2 = (from x in query
                      where x.tanggal == Variables.todayDate
                      && x.denom == "100000"
                      select x).ToList();

            result = q2.Select(x => new StoreClass()
            {
                kodePkt = x.namaPkt,
                tanggal = (DateTime)x.tanggal,
                val = (Int64)x.newBaru
            }).ToList();
            return result;
        }
        List<StoreClass> loadNewNote50()
        {
            Database1Entities db = new Database1Entities();
            List<StoreClass> result = new List<StoreClass>();
            var query = (from x in db.StokPosisis
                         select x).ToList();
            var q2 = (from x in query
                      where x.tanggal == Variables.todayDate
                      && x.denom == "50000"
                      select x).ToList();

            result = q2.Select(x => new StoreClass()
            {
                kodePkt = x.namaPkt,
                tanggal = (DateTime)x.tanggal,
                val = (Int64)x.newBaru
            }).ToList();
            return result;
        }
        public void initListSumber()
        {
            var q = (from x in morningbalance100
                     from y in morningbalance50.Where(y => y.kodePkt == x.kodePkt && y.tanggal == x.tanggal)
                     select new DataPermintaanDanSumber()
                     {
                         tanggal = x.tanggal,
                         namaPkt = x.kodePkt,
                         jenisUang = "FIT",
                         d100 = x.val,
                         d50 = y.val,
                         d20 = 0
                     }).ToList();
            q.AddRange((from x in newNote100
                       join y in newNote50 on new { x.kodePkt, x.tanggal } equals new { y.kodePkt, y.tanggal }
                       select new DataPermintaanDanSumber()
                       {
                           tanggal = x.tanggal,
                           namaPkt = x.kodePkt,
                           jenisUang = "NEW NOTE",
                           d100 = x.val,
                           d50 = y.val,
                           d20 = 0
                       }).ToList());
            q.AddRange(listDataTambahanSumber);
            sumberDanaGridView.DataSource = q.OrderBy(x => x.namaPkt).OrderBy(x => x.tanggal).ToList();
            listDataSumber = q;

            sumberDanaGridView.Columns[0].Width = 70;
            sumberDanaGridView.Columns[1].Width = 200;
            sumberDanaGridView.Columns[2].DefaultCellStyle.Format = "N0";
            sumberDanaGridView.Columns[3].DefaultCellStyle.Format = "N0";
            sumberDanaGridView.Columns[4].DefaultCellStyle.Format = "N0";
        }
        public void initPembagianGridView()
        {
            Database1Entities db = new Database1Entities();
            //Nama PKT yang ada di ComboBox

            //Nanti ganti data dari master bank tukab
            List<String> bankTukab = db.DataBankLains.Select(x => x.namaBank.Replace("PT. ", "")).ToList();
            var listNamaPkt = db.Pkts.Where(x => x.kanwil.ToUpper().Contains("JABO") && !x.namaPkt.Contains("Alam Sutera")).Select(x => x.namaPkt).ToList();
            listNamaPkt.AddRange(bankTukab);

            DataGridViewComboBoxColumn pktTujuan = new DataGridViewComboBoxColumn() {
                DataSource = listNamaPkt,
                HeaderText = "Nama Pkt Tujuan",
                ValueType = typeof(String)
            };
            var listNamaPktSumber = morningbalance100.Select(x => x.kodePkt).Distinct().OrderBy(x => x).ToList();
            listNamaPktSumber.AddRange(bankTukab);
            DataGridViewComboBoxColumn pktSumber = new DataGridViewComboBoxColumn()
            {
                DataSource = listNamaPktSumber,
                HeaderText = "Nama Pkt Sumber",
                ValueType = typeof(String)
            };
            var listTanggal = db.DetailApprovals.AsEnumerable().Where(x => x.bon100 != -1 && x.tanggal >= Variables.todayDate).Select(x => ((DateTime)x.tanggal).ToShortDateString()).Distinct().ToList();
            DataGridViewComboBoxColumn tgl = new DataGridViewComboBoxColumn()
            {
                DataSource = listTanggal,
                HeaderText = "Tanggal",
                ValueType = typeof(String)
            };
           
            var listJenisUang = new List<String>() { "FIT", "NEW NOTE" };
            DataGridViewComboBoxColumn jenisUang = new DataGridViewComboBoxColumn()
            {
                DataSource = listJenisUang,
                HeaderText = "Jenis Uang",
                ValueType = typeof(String)
            };

            this.listNamaPktSumber = listNamaPktSumber;
            this.listNamaPkt = listNamaPkt;
            pembagianGridView.Columns.Add(tgl);
            pembagianGridView.Columns[0].Width = 100;

            pembagianGridView.Columns.Add(pktSumber);
            pembagianGridView.Columns[1].Width = 200;

            pembagianGridView.Columns.Add(pktTujuan);
            pembagianGridView.Columns[2].Width = 200;

            pembagianGridView.Columns.Add(jenisUang);
            pembagianGridView.Columns[3].Width = 70;

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
            listDataPermintaan = new List<DataPermintaanDanSumber>();
            Database1Entities db = new Database1Entities();
            var q = (from x in db.Approvals
                     join y in db.DetailApprovals on x.idApproval equals y.idApproval
                     join z in db.Pkts on x.kodePkt equals z.kodePkt
                     where x.tanggal == Variables.todayDate && z.kanwil.ToUpper().Contains("JABO")
                     && y.bon100 != -1
                     select new { z.namaPkt, tanggal = (DateTime)y.tanggal, y.bon100, y.bon50,y.bon20, y.adhoc100, y.adhoc50, y.adhoc20 }).ToList();
            
            //Adhoc
            listDataPermintaan.AddRange(
                q.Where(x => (DateTime)x.tanggal == Variables.todayDate)
                .Select(x => new DataPermintaanDanSumber() {
                    tanggal = x.tanggal,
                    namaPkt = x.namaPkt,
                    d100 = (Int64) x.adhoc100,
                    d50 = (Int64) x.adhoc50,
                    d20 = (Int64) x.adhoc20,
                }).ToList());
            
            //Bon Reguler
            listDataPermintaan.AddRange(
                q.Where(x => (DateTime)x.tanggal > Variables.todayDate)
                .Select(x => new DataPermintaanDanSumber() {
                    tanggal = x.tanggal,
                    namaPkt = x.namaPkt,
                    d100 = (Int64) x.bon100,
                    d50 = (Int64) x.bon50,
                    d20 = (Int64) x.bon20
                }).ToList());
            listDataPermintaan.AddRange(listDataTambahanPermintaan);
            sisaGridView.DataSource = listDataPermintaan.Where(x => x.d100 + x.d50 != 0).OrderBy(x=>x.namaPkt).OrderBy(x=> x.tanggal).ToList();
            sisaGridView.Columns["tanggal"].Width = 70;
            sisaGridView.Columns["namaPkt"].Width = 200;
            sisaGridView.Columns["d100"].DefaultCellStyle.Format = "N0";
            sisaGridView.Columns["d50"].DefaultCellStyle.Format = "N0";
            sisaGridView.Columns["d20"].DefaultCellStyle.Format = "N0";
        }
        public List<String>cekKapasitas()
        {
            Database1Entities db = new Database1Entities();
            Double persenKapasitas = (Double) kapasitasNumeric.Value / 100;

            var pcsDeliveryCabang = (from x in deliveryCabang
                                     group x by new { x.Tanggal, x.PktSumber } into g
                                     where g.Key.Tanggal == Variables.todayDate.AddDays(1)
                                     select new {
                                         NamaPkt = g.Key.PktSumber,
                                         value = g.Sum(x => (x.D100 / 100000) + (x.D50 / 50000)),
                                     }).ToList();

            var qfit = (from x in db.StokPosisis
                        group x by new { x.namaPkt, x.tanggal, x.denom } into g
                        where g.Key.tanggal == Variables.todayDate
                        && (g.Key.denom == "100000" || g.Key.denom == "50000")
                        select new
                        {
                            g.Key.namaPkt,
                            g.Key.tanggal,
                            g.Key.denom,
                            FIT = g.Sum(x => x.fitBaru + x.fitLama + x.fitNKRI)
                        }).ToList();
            var fit = (from x in qfit
                       group x by x.namaPkt into g
                       select new
                       {
                           namaPkt = g.Key,
                           FIT = g.Sum(x => x.FIT / Int32.Parse(x.denom))
                       }).ToList();
            var qunprocessed = (from x in db.StokPosisis
                        group x by new { x.namaPkt, x.tanggal, x.denom } into g
                        where g.Key.tanggal == Variables.todayDate
                        && (g.Key.denom == "100000" || g.Key.denom == "50000")
                        select new
                        {
                            g.Key.namaPkt,
                            g.Key.tanggal,
                            g.Key.denom,
                            unprocessed = g.Sum(x => x.unprocessed)
                        }).ToList();
            var unprocessed = (from x in qunprocessed
                       group x by x.namaPkt into g
                       select new
                       {
                           namaPkt = g.Key,
                           unprocessed = g.Sum(x => x.unprocessed / Int32.Parse(x.denom)) * persenUnprocessed
                       }).ToList();
            var listDataInputanUser = LoadDataInputanUser();
            var dataDariTabelPembagian = (from x in listDataInputanUser
                                          group x by new { x.jenisUang, x.namaPktSumber, x.tanggal } into g
                                          where g.Key.tanggal == Variables.todayDate.AddDays(1)
                                          && g.Key.jenisUang.ToUpper() == "FIT"
                                          select new {
                                              g.Key.namaPktSumber,
                                              value = g.Sum(x=> (x.d100 / 100000) + (x.d50 / 50000))
                                          }).ToList();

            var valuePerbandingan = (from x in unprocessed
                                     join y in dataDariTabelPembagian on x.namaPkt equals y.namaPktSumber into xy
                                     from y in xy.DefaultIfEmpty()
                                     join z in fit on x.namaPkt equals z.namaPkt into xz
                                     from z in xz.DefaultIfEmpty()
                                     join zz in pcsDeliveryCabang on x.namaPkt equals zz.NamaPkt into xzz
                                     from zz in xzz.DefaultIfEmpty()
                                     select new {
                                         x.namaPkt,
                                         value = (y == null? 0 :y.value) + (zz == null? 0 : zz.value )- z.FIT - x.unprocessed
                                     }).ToList();

            var yangKuning = (from x in valuePerbandingan
                              join y in db.Pkts on x.namaPkt equals y.namaPkt
                              where x.value > (y.kapasitasCPC * persenKapasitas)
                              select x.namaPkt).ToList();
           
            return yangKuning;
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
        List<DataInputanUser> LoadDataInputanUser()
        {
            List<DataInputanUser> listDataInputanUser = new List<DataInputanUser>();

            for (int a = 0; a < pembagianGridView.RowCount - 1; a++)
            {
                DataGridViewRow row = pembagianGridView.Rows[a];
                listDataInputanUser.Add(new DataInputanUser()
                {
                    tanggal = DateTime.Parse(row.Cells[0].Value.ToString()),
                    namaPktSumber = row.Cells[1].Value.ToString(),
                    namaPktTujuan = row.Cells[2].Value.ToString(),
                    jenisUang = row.Cells[3].Value.ToString(),
                    d100 = Int64.Parse(row.Cells[4].Value.ToString()),
                    d50 = Int64.Parse(row.Cells[5].Value.ToString()),
                    d20 = Int64.Parse(row.Cells[6].Value.ToString())
                });
            }
            return listDataInputanUser;
        }
        private void pembagianGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            List<DataInputanUser> listDataInputanUser = LoadDataInputanUser();

            //Kurangin dari tabel permintaan
            initListSisaPermintaan();
            List<DataPermintaanDanSumber> tempListDataSisa = new List<DataPermintaanDanSumber>(listDataPermintaan);
            foreach(var temp in listDataInputanUser)
            {
                Console.WriteLine(temp.tanggal.ToShortDateString() + " " + temp.namaPktTujuan);
                
                var q = (from x in tempListDataSisa
                         where x.namaPkt == temp.namaPktTujuan && x.tanggal == temp.tanggal 
                         select x).FirstOrDefault();
                if (q != null)
                {
                    Console.WriteLine("NOT NULL");
                    q.d100 -= temp.d100;
                    q.d50 -= temp.d50;
                    q.d20 -= temp.d20;
                    sisaGridView.DataSource = tempListDataSisa.Where(x => x.d100 + x.d50 != 0).OrderBy(x=>x.namaPkt).OrderBy(x=>x.tanggal).ToList();
                    sisaGridView.Refresh();
                }
            }
            //Kurangin dari tabel sumber
            initListSumber();
            List<DataPermintaanDanSumber> tempListDataSumber = new List<DataPermintaanDanSumber>(listDataSumber);
            foreach (var temp in listDataInputanUser)
            {
                var q = (from x in tempListDataSumber
                         where x.namaPkt == temp.namaPktSumber && x.tanggal == temp.tanggal && x.jenisUang == temp.jenisUang
                         select x).FirstOrDefault();
                if (q != null)
                {
                    q.d100 -= temp.d100;
                    q.d50 -= temp.d50;
                    q.d20 -= temp.d20;
                    sumberDanaGridView.DataSource = tempListDataSumber.OrderBy(x => x.namaPkt).OrderBy(x => x.tanggal).ToList();
                    sumberDanaGridView.Refresh();
                }
            }
            cekDanger();
        }
        void cekDanger()
        {
            List<string> listDanger = cekKapasitas();
            if (listDanger.Any())
                Console.WriteLine("Yang danger");
            foreach (var temp in listDanger)
                Console.WriteLine(temp);
            //[0] -> tanggal
            for (int a = 0; a < sumberDanaGridView.RowCount; a++)
            {
                var row = sumberDanaGridView.Rows[a];
                Console.WriteLine(row.Cells[0].Value.ToString());
                if (DateTime.Parse(row.Cells[0].Value.ToString()) == Variables.todayDate.AddDays(1))
                {
                    Console.WriteLine("Tanggal Sama");
                    var find = listDanger.Where(x => x == row.Cells[1].Value.ToString()).FirstOrDefault();
                    if (find != null)
                        row.DefaultCellStyle.BackColor = Color.Yellow;
                }
            }
        }
        private void pembagianGridView_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells[0].Value = Variables.todayDate.ToShortDateString();
            e.Row.Cells[1].Value = listNamaPktSumber[0];
            e.Row.Cells[2].Value = listNamaPkt[0];
            e.Row.Cells[3].Value = "FIT";
            e.Row.Cells[4].Value = 0;
            e.Row.Cells[5].Value = 0;
            e.Row.Cells[6].Value = 0;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var rows = pembagianGridView.SelectedRows;
            foreach (DataGridViewRow row in rows)
                pembagianGridView.Rows.Remove(row);
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            Database1Entities db = new Database1Entities();
            db.ApprovalPembagianSaldoes.AddRange(deliveryCabang);
            db.ApprovalPembagianSaldoes.AddRange(adhocCabang);
            var dataInputanUser = LoadDataInputanUser();

            var dataInputanUserToInsert = (from x in dataInputanUser
                                           select new ApprovalPembagianSaldo() {
                                               JenisTransaksi = "Reguler",
                                               D100 = x.d100,
                                               D50 = x.d50,
                                               JenisUang = x.jenisUang,
                                               PktSumber = x.namaPktSumber,
                                               PktTujuan = x.namaPktTujuan,
                                               Tanggal = x.tanggal
                                           }).ToList();
            db.ApprovalPembagianSaldoes.AddRange(dataInputanUserToInsert);
            db.SaveChanges();
        }
    }

    public class DataPermintaanDanSumber
    {
        public DateTime tanggal { set; get; }
        public String namaPkt { set; get; }
        public String jenisUang { set; get; }
        public Int64 d100 { set; get; }
        public Int64 d50 { set; get; }
        public Int64 d20 { set; get; }
    }
    public class DataInputanUser
    {
        public DateTime tanggal { set; get; }
        public String jenisUang { set; get; }
        public String namaPktTujuan { set; get; }
        public String namaPktSumber { set; get; }
        public Int64 d100 { set; get; }
        public Int64 d50 { set; get; }
        public Int64 d20 { set; get; }
    }
}
