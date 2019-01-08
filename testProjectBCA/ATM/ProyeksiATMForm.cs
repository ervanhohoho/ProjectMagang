using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testProjectBCA.ATM
{
    public partial class ProyeksiATMForm : Form
    {
        Database1Entities db = new Database1Entities();
        public ProyeksiATMForm()
        {
            InitializeComponent();
            loadComboMetode();
            loadComboPkt();
            loadTreeView();
            e2eComboBox.SelectedIndex = 0;
        }
        private void loadComboPkt()
        {
            List<String> listPkt = (from x in db.Pkts.AsEnumerable()
                                    select x.kanwil).Where(x => !String.IsNullOrWhiteSpace(x)).OrderBy(x => x).Distinct().ToList();
            listPkt.Add("All");
            listPkt.AddRange((from x in db.Pkts.AsEnumerable()
                                    select x.kodePktATM).Where(x=>!String.IsNullOrWhiteSpace(x)).OrderBy(x=>x).Distinct().ToList());
            KodePktCombo.DataSource = listPkt;
        }
        private void loadComboMetode()
        {
            MetodePenghitunganCombo.DataSource = Variables.listMetodeNonATM;
        }
        private void LoadBtn_Click(object sender, EventArgs e)
        {
            loadForm.ShowSplashScreen();
            DateTime startDate = Variables.todayDate.Date,
                endDate = EndDatePicker.Value.Date,
                currDate = startDate;
            String kodePkt = KodePktCombo.SelectedItem.ToString(),
                metode = MetodePenghitunganCombo.SelectedItem.ToString();
            List<DateTime> kumpulanTanggal = new List<DateTime>();
            List<ViewProyeksiAtm> hasil = new List<ViewProyeksiAtm>();
            while(currDate <= endDate)
            {
                kumpulanTanggal.Add(currDate);
                currDate = currDate.AddDays(1);
            }


            float targetRasio100 = (float)rasio100Num.Value, targetRasio50 = (float)rasio50Num.Value, targetRasio20 = (float)rasio20Num.Value;
           
            KumpulanPrediksi prediksi = new KumpulanPrediksi(kodePkt, loadKumpulanTanggalUntukPrediksi(), startDate, endDate, metode, metode);
            List<String> eventType = prediksi.eventType;
            List<JenisEvent> listEvent = new List<JenisEvent>();

            String e2e = e2eComboBox.SelectedItem.ToString();
            List<PktDenom> hasilPrediksi = prediksi.loadRekomendasiBon(targetRasio100, targetRasio50, targetRasio20, e2e);
            List<PktDenom> saldoAwalHariH = loadSaldoAwal(prediksi);
            List<PktDenom> saldoAwals = new List<PktDenom>();


            /************************/
            /*START Load Saldo Awal*/
            /**********************/
            saldoAwals.AddRange(saldoAwalHariH);
            currDate = startDate;
            Console.WriteLine("Load Saldo Awal\n=================");
            while (currDate < endDate)
            {
                saldoAwals.AddRange(
                    (from x in saldoAwals
                     join isiATM in prediksi.prediksiIsiAtm on new {x.tanggal, x.kodePkt} equals new {isiATM.tanggal, isiATM.kodePkt}
                     join isiCRM in prediksi.isiCrm2 on new { isiATM.tanggal, isiATM.kodePkt } equals new { isiCRM.tanggal, isiCRM.kodePkt }
                     join sislokATM in prediksi.rasioSislokAtm on new { isiATM.tanggal, isiATM.kodePkt } equals new { sislokATM.tanggal, sislokATM.kodePkt }
                     join sislokCRM in prediksi.sislokCrm on new { isiATM.tanggal, isiATM.kodePkt } equals new { sislokCRM.tanggal, sislokCRM.kodePkt }
                     join sislokCDM in prediksi.sislokCdm on new { isiATM.tanggal, isiATM.kodePkt } equals new { sislokCDM.tanggal, sislokCDM.kodePkt }
                     join hasilpred in hasilPrediksi on new { isiATM.tanggal, isiATM.kodePkt } equals new { hasilpred.tanggal, hasilpred.kodePkt }
                     where x.tanggal == currDate
                     select new PktDenom()
                     {
                         tanggal = x.tanggal.AddDays(1),
                         d100 = x.d100 - isiATM.d100 - isiCRM.d100 + (Int64) Math.Round(sislokATM.d100 * isiATM.d100, 0) + sislokCRM.d100 + sislokCDM.d100 + hasilpred.d100,
                         d50 = x.d50 - isiATM.d50 - isiCRM.d50 + (Int64) Math.Round(sislokATM.d50 * isiATM.d50, 0) + sislokCRM.d50 + sislokCDM.d50 + hasilpred.d50,
                         d20 = x.d20 - isiATM.d20 - isiCRM.d20 + (Int64) Math.Round(sislokATM.d20 * isiATM.d20, 0) + sislokCRM.d20 + sislokCDM.d20 + hasilpred.d20,
                         kodePkt = x.kodePkt
                     }).ToList()
                    );
                currDate = currDate.AddDays(1);
            }


            /************************/
            /*END Load Saldo Awal*/
            /**********************/

            for (int a = 0; a < eventType.Count; a++)
            {
                listEvent.Add(new JenisEvent() { tgl = startDate.AddDays(a), eventType = eventType[a].Split(' ')[3] + eventType[a].Split(' ')[4] });
            }
            Console.WriteLine("Isi ATM Count: " + prediksi.prediksiIsiAtm.Count);
            Console.WriteLine("Isi CRM Count: " + prediksi.isiCrm2.Count);
            Console.WriteLine("Sislok ATM Count: " + prediksi.rasioSislokAtm.Count);
            Console.WriteLine("Sislok CRM Count: " + prediksi.sislokCrm.Count);
            Console.WriteLine("Sislok CDM Count: " + prediksi.sislokCdm.Count);



            hasil.AddRange((from isiATM in prediksi.prediksiIsiAtm
                            join isiCRM in prediksi.isiCrm2 on new { isiATM.tanggal, isiATM.kodePkt } equals new { isiCRM.tanggal, isiCRM.kodePkt }
                            join sislokATM in prediksi.rasioSislokAtm on new { isiATM.tanggal, isiATM.kodePkt } equals new { sislokATM.tanggal, sislokATM.kodePkt }
                            join sislokCRM in prediksi.sislokCrm on new { isiATM.tanggal, isiATM.kodePkt } equals new { sislokCRM.tanggal, sislokCRM.kodePkt}
                            join sislokCDM in prediksi.sislokCdm on new { isiATM.tanggal, isiATM.kodePkt } equals new { sislokCDM.tanggal, sislokCDM.kodePkt }
                            join saldoAwal in saldoAwals on new { isiATM.tanggal, isiATM.kodePkt } equals new {saldoAwal.tanggal, saldoAwal.kodePkt}
                            join hasilpred in hasilPrediksi on new {isiATM.tanggal, isiATM.kodePkt} equals new {hasilpred.tanggal, hasilpred.kodePkt}
                            join events in listEvent on isiATM.tanggal equals events.tgl
                            select new ViewProyeksiAtm()
                            {
                                tanggal = isiATM.tanggal,
                                saldoAwal100 = saldoAwal.d100,
                                saldoAwal50 = saldoAwal.d50,
                                saldoAwal20 = saldoAwal.d20,
                                isiATM100 = isiATM.d100,
                                isiATM50 = isiATM.d50,
                                isiATM20 = isiATM.d20,
                                isiCRM100 = isiCRM.d100,
                                isiCRM50 = isiCRM.d50,
                                isiCRM20 = isiCRM.d20,
                                sislokATM100 = (Int64)Math.Round(sislokATM.d100 * isiATM.d100),
                                sislokATM50 = (Int64)Math.Round(sislokATM.d50 * isiATM.d50),
                                sislokATM20 = (Int64)Math.Round(sislokATM.d20 * isiATM.d20),
                                sislokCRM100 = sislokCRM.d100,
                                sislokCRM50 = sislokCRM.d50,
                                sislokCRM20 = sislokCRM.d20,
                                sislokCDM100 = sislokCDM.d100,
                                sislokCDM50 = sislokCDM.d50,
                                sislokCDM20 = sislokCDM.d20,
                                bon100 = hasilpred.d100,
                                bon50 = hasilpred.d50,
                                bon20 = hasilpred.d20,
                                eventType = events.eventType,
                                kodePkt = isiATM.kodePkt
                            }).ToList());
            hasil = hasil.GroupBy(x => new { x.tanggal, x.eventType }).Select(x=> new ViewProyeksiAtm()
            {
                tanggal = x.Key.tanggal,
                saldoAwal100 = x.Sum(y=>y.saldoAwal100),
                saldoAwal50 = x.Sum(y=>y.saldoAwal50),
                saldoAwal20 = x.Sum(y=>y.saldoAwal20),
                isiATM100 = x.Sum(y=>y.isiATM100),
                isiATM50 = x.Sum(y => y.isiATM50),
                isiATM20 = x.Sum(y => y.isiATM20),
                isiCRM100 = x.Sum(y => y.isiCRM100),
                isiCRM50 = x.Sum(y => y.isiCRM50),
                isiCRM20 = x.Sum(y => y.isiCRM20),
                sislokATM100 = x.Sum(y=>y.sislokATM100),
                sislokATM50 = x.Sum(y => y.sislokATM50),
                sislokATM20 = x.Sum(y => y.sislokATM20),
                sislokCRM100 = x.Sum(y=>y.sislokCRM100),
                sislokCRM50 = x.Sum(y => y.sislokCRM50),
                sislokCRM20 = x.Sum(y => y.sislokCRM20),
                sislokCDM100 = x.Sum(y => y.sislokCDM100),
                sislokCDM50 = x.Sum(y => y.sislokCDM50),
                sislokCDM20 = x.Sum(y => y.sislokCDM20),
                bon100 = x.Sum(y => y.bon100),
                bon50 = x.Sum(y => y.bon50),
                bon20 = x.Sum(y => y.bon20),
                eventType = x.Key.eventType,
                kodePkt = kodePkt
            }).ToList();

            //dataGridView1.DataSource = saldoAwals;
            dataGridView1.DataSource = hasil;
            for (int a = 0; a < dataGridView1.ColumnCount; a++)
            {
                if (dataGridView1.Columns[a].ValueType == typeof(Int64))
                {
                    dataGridView1.Columns[a].DefaultCellStyle.Format = "C0";
                    dataGridView1.Columns[a].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
                }
            }
            Console.WriteLine("Target Rasio 100 Input: " + targetRasio100);
            Console.WriteLine("Target Rasio 50 Input: " + targetRasio50);
            Console.WriteLine("Target Rasio 20 Input: " + targetRasio20);
            //dataGridView1.DataSource = hasilPrediksi;
            loadForm.CloseForm();
        }
        List<PktDenom> loadSaldoAwal(KumpulanPrediksi prediksi)
        {
            List<PktDenom> ret = new List<PktDenom>();
            String kodePkt = KodePktCombo.SelectedItem.ToString();
            if(kodePkt.ToLower().Contains("jabo") || kodePkt.ToLower().Contains("kanwil"))
            {
                List<String> listPkt = (from x in db.Pkts
                                  where x.kanwil == kodePkt && !x.kodePkt.Contains("CCAS")
                                  select x.kodePkt).ToList();

                foreach(var temp in listPkt)
                {
                    var q = (from x in db.TransaksiAtms.AsEnumerable()
                             where x.kodePkt == temp && x.tanggal == Variables.todayDate.AddDays(-1)
                             select new PktDenom {
                                 kodePkt = x.kodePkt,
                                 tanggal = x.tanggal.AddDays(1),
                                 d100 = (Int64)x.saldoAkhir100,
                                 d50 = (Int64)x.saldoAkhir50,
                                 d20 = (Int64)x.saldoAkhir20,
                             }).FirstOrDefault();
                    if (q != null)
                        ret.Add(q);
                    else
                        ret.Add(new PktDenom() { tanggal = Variables.todayDate, d100 = 0, d50 = 0, d20 = 0, kodePkt = temp });
                }

            }
            else if(kodePkt.ToLower() == "all")
            {
                List<String> listPkt = (from x in db.Pkts
                                        where !x.kodePkt.Contains("CCAS")
                                        select x.kodePkt).ToList();

                foreach (var temp in listPkt)
                {
                    var q = (from x in db.TransaksiAtms.AsEnumerable()
                             where x.kodePkt == temp && x.tanggal == Variables.todayDate.AddDays(-1)
                             select new PktDenom
                             {
                                 kodePkt = x.kodePkt,
                                 tanggal = x.tanggal.AddDays(1),
                                 d100 = (Int64)x.saldoAkhir100,
                                 d50 = (Int64)x.saldoAkhir50,
                                 d20 = (Int64)x.saldoAkhir20,
                             }).FirstOrDefault();
                    if (q != null)
                        ret.Add(q);
                    else
                        ret.Add(new PktDenom() { tanggal = Variables.todayDate, d100 = 0, d50 = 0, d20 = 0, kodePkt = temp });
                }
            }
            else
            {
                Denom saldoAwal = prediksi.loadSaldoAwal(kodePkt);
                var q = (from x in db.TransaksiAtms.AsEnumerable()
                         where x.kodePkt == kodePkt && x.tanggal == Variables.todayDate.AddDays(-1)
                         select new PktDenom
                         {
                             kodePkt = x.kodePkt,
                             tanggal = x.tanggal.AddDays(1),
                             d100 = (Int64)x.saldoAkhir100,
                             d50 = (Int64)x.saldoAkhir50,
                             d20 = (Int64)x.saldoAkhir20,
                         }).FirstOrDefault();
                if (q != null)
                    ret.Add(q);
                else
                    ret.Add(new PktDenom() { tanggal = Variables.todayDate, d100 = 0, d50 = 0, d20 = 0, kodePkt = kodePkt });
            }
            return ret;
        }
        void loadTreeView()
        {
            //Load Tree
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    cmd.CommandText = "SELECT MIN(Tanggal), MAX(Tanggal) FROM TransaksiAtms";
                    sql.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        DateTime minTanggal = (DateTime)reader[0];
                        DateTime maxTanggal = (DateTime)reader[1];
                        Console.WriteLine("Min Date: " + minTanggal.ToShortDateString());
                        Console.WriteLine("Max Date: " + maxTanggal.ToShortDateString());

                        DateTime tempTanggal = new DateTime(minTanggal.Year, minTanggal.Month, 1);
                        int counter = 0;
                        bool firstRun = true;
                        while (tempTanggal <= maxTanggal)
                        {
                            treeView1.Nodes.Add(tempTanggal.Year.ToString());
                            int monthCounter;
                            if (firstRun)
                            {
                                monthCounter = minTanggal.Month;
                                firstRun = false;
                            }
                            else
                                monthCounter = 1;
                            while (tempTanggal <= maxTanggal && monthCounter <= 12)
                            {
                                Console.WriteLine(monthCounter);
                                treeView1.Nodes[counter].Nodes.Add((monthCounter++).ToString());
                                tempTanggal = tempTanggal.AddMonths(1);
                                Console.WriteLine("Temp Tanggal: " + tempTanggal.ToShortDateString());
                            }
                            counter++;
                        }
                    }
                }

            }
        }
        List<DateTime> loadKumpulanTanggalUntukPrediksi()
        {
            List<DateTime> kumpulanTanggalUntukPrediksi = new List<DateTime>();
            for (int i = 0; i < treeView1.Nodes.Count; i++)
            {
                for (int j = 0; j < treeView1.Nodes[i].Nodes.Count; j++)
                {
                    if (treeView1.Nodes[i].Nodes[j].Checked)
                    {
                        kumpulanTanggalUntukPrediksi.Add(new DateTime(Int32.Parse(treeView1.Nodes[i].Text), Int32.Parse(treeView1.Nodes[i].Nodes[j].Text), 1));
                    }
                }
            }
            return kumpulanTanggalUntukPrediksi;
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
        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            CheckTreeViewNode(e.Node, e.Node.Checked);
        }
        private void treeView1_ParentChanged(object sender, EventArgs e)
        {

        }
        public class JenisEvent
        {
            public DateTime tgl { set; get; }
            public String eventType { set; get; }
        }
        public class ViewProyeksiAtm
        {
            public DateTime tanggal { set; get; }
            public String kodePkt { set; get; }
            public Int64 saldoAwal100 { set; get; }
            public Int64 saldoAwal50 { set; get; }
            public Int64 saldoAwal20 { set; get; }
            public Int64 bon100 { set; get; }
            public Int64 bon50 { set; get; }
            public Int64 bon20 { set; get; }
            public Int64 isiATM100 { set; get; }
            public Int64 isiATM50 { set; get; }
            public Int64 isiATM20 { set; get; }
            public Int64 isiCRM100 { set; get; }
            public Int64 isiCRM50 { set; get; }
            public Int64 isiCRM20 { set; get; }
            public Int64 sislokATM100 { set; get; }
            public Int64 sislokATM50 { set; get; }
            public Int64 sislokATM20 { set; get; }
            public Int64 sislokCRM100 { set; get; }
            public Int64 sislokCRM50 { set; get; }
            public Int64 sislokCRM20 { set; get; }
            public Int64 sislokCDM100 { set; get; }
            public Int64 sislokCDM50 { set; get; }
            public Int64 sislokCDM20 { set; get; }
            public String eventType { set; get; }
        }
    }
  
}
