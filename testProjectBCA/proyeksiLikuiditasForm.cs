using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
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
            metodePrediksiComboBox.SelectedIndex = 0;
            tanggalMaxPrediksiPicker.MinDate = DateTime.Today.AddDays(1);
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
        List<DateTime> loadTanggalHistorisUntukPrediksi()
        {
            List<DateTime> listTanggal = new List<DateTime>();
            for (int i = 0; i < bulanPrediksiTreeView.Nodes.Count; i++)
            {
                for (int j = 0; j < bulanPrediksiTreeView.Nodes[i].Nodes.Count; j++)
                {
                    if (bulanPrediksiTreeView.Nodes[i].Nodes[j].Checked)
                    {
                        listTanggal.Add(new DateTime(Int32.Parse(bulanPrediksiTreeView.Nodes[i].Text), Int32.Parse(bulanPrediksiTreeView.Nodes[i].Nodes[j].Text), 1));
                    }
                }
            }
            return listTanggal;
        }
        List<tanggalValue> loadPrediksiInCabang100()
        {
            List<tanggalValue> result = new List<tanggalValue>();
            List<DateTime> listTanggalHistorisUntukPrediksi = loadTanggalHistorisUntukPrediksi();

            //Load semua in Cabang 100
            var q = (from x in db.DailyStocks
                     where x.in_out.ToUpper() == "IN"
                     && x.jenisTransaksi.Contains("Collection Cabang - Full - Process")
                     select new { x.tanggal, x.BN100K });
            List<EventTanggal> et = (from x in db.EventTanggals.AsEnumerable().AsEnumerable()
                                     select x).ToList();


            DateTime tanggal = DateTime.Today;
            DateTime maxTanggal = tanggalMaxPrediksiPicker.Value;

            while (tanggal <= maxTanggal)
            {
                List<Int64> query = new List<Int64>();
                var eventT = (from x in et
                              where x.tanggal == tanggal
                              select new { x.workDay, x.@event }).FirstOrDefault();
                
                foreach (var temp in listTanggalHistorisUntukPrediksi)
                {
                    List<Int64> q3 = (from x in q
                                      join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                      where temp.Month == ((DateTime)x.tanggal).Month
                                      && temp.Year == ((DateTime)x.tanggal).Year
                                      && eventT.workDay == y.workDay
                                      && eventT.@event == y.@event
                                      select new { tanggal = (DateTime) x.tanggal, BN100K = (Int64)x.BN100K }
                              ).AsEnumerable().Where(x=>x.tanggal.DayOfWeek == tanggal.DayOfWeek).Select(x=>x.BN100K).ToList();

                    if (q3.Count == 0)
                    {
                        q3 = (from x in q
                              join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                              where temp.Month == ((DateTime)x.tanggal).Month
                              && temp.Year == ((DateTime)x.tanggal).Year
                              && eventT.workDay == y.workDay
                              select (Int64)x.BN100K
                              ).ToList();
                    }
                    query.AddRange(q3);
                }
                result.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)Math.Round(query.Any() ? query.Average(x => x) : 0, 0) });
                tanggal = tanggal.AddDays(1);
            }
            return result;
        }
        List<tanggalValue> loadPrediksiInCabang50()
        {
            List<tanggalValue> result = new List<tanggalValue>();
            List<DateTime> listTanggalHistorisUntukPrediksi = loadTanggalHistorisUntukPrediksi();

            //Load semua in Cabang 100
            var q = (from x in db.DailyStocks
                     where x.in_out.ToUpper() == "IN"
                     && x.jenisTransaksi.Contains("Collection Cabang - Full - Process")
                     select new { x.tanggal, x.BN50K });
            List<EventTanggal> et = (from x in db.EventTanggals.AsEnumerable().AsEnumerable()
                                     select x).ToList();


            DateTime tanggal = DateTime.Today;
            DateTime maxTanggal = tanggalMaxPrediksiPicker.Value;

            while (tanggal <= maxTanggal)
            {
                List<Int64> query = new List<Int64>();
                var eventT = (from x in et
                              where x.tanggal == tanggal
                              select new { x.workDay, x.@event }).FirstOrDefault();


                foreach (var temp in listTanggalHistorisUntukPrediksi)
                {
                    List<Int64> q3 = (from x in q
                                      join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                      where temp.Month == ((DateTime)x.tanggal).Month
                                      && temp.Year == ((DateTime)x.tanggal).Year
                                      && eventT.workDay == y.workDay
                                      && eventT.@event == y.@event
                                      select new { tanggal = (DateTime)x.tanggal, BN50K = (Int64)x.BN50K }
                              ).AsEnumerable().Where(x => x.tanggal.DayOfWeek == tanggal.DayOfWeek).Select(x => x.BN50K).ToList();

                    if (q3.Count == 0)
                    {
                        q3 = (from x in q
                              join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                              where temp.Month == ((DateTime)x.tanggal).Month
                              && temp.Year == ((DateTime)x.tanggal).Year
                              && eventT.workDay == y.workDay
                              select (Int64)x.BN50K
                              ).ToList();
                    }
                    query.AddRange(q3);
                }
                result.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)Math.Round(query.Any() ? query.Average(x => x) : 0, 0) });
                tanggal = tanggal.AddDays(1);
            }
            return result;
        }
        List<tanggalValue> loadPrediksiInRetail100()
        {
            List<tanggalValue> result = new List<tanggalValue>();
            List<DateTime> listTanggalHistorisUntukPrediksi = loadTanggalHistorisUntukPrediksi();

            //Load semua in Cabang 100
            var q = (from x in db.DailyStocks
                     where x.in_out.ToUpper() == "IN"
                     && x.jenisTransaksi.Contains("Collection Retail")
                     select new { x.tanggal, x.BN100K });
            List<EventTanggal> et = (from x in db.EventTanggals.AsEnumerable().AsEnumerable()
                                     select x).ToList();


            DateTime tanggal = DateTime.Today;
            DateTime maxTanggal = tanggalMaxPrediksiPicker.Value;

            while (tanggal <= maxTanggal)
            {
                List<Int64> query = new List<Int64>();
                var eventT = (from x in et
                              where x.tanggal == tanggal
                              select new { x.workDay, x.@event }).FirstOrDefault();


                foreach (var temp in listTanggalHistorisUntukPrediksi)
                {
                    List<Int64> q3 = (from x in q
                                      join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                      where temp.Month == ((DateTime)x.tanggal).Month
                                      && temp.Year == ((DateTime)x.tanggal).Year
                                      && eventT.workDay == y.workDay
                                      && eventT.@event == y.@event
                                      select new { tanggal = (DateTime)x.tanggal, BN100K = (Int64)x.BN100K }
                              ).AsEnumerable().Where(x => x.tanggal.DayOfWeek == tanggal.DayOfWeek).Select(x => x.BN100K).ToList();

                    if (q3.Count == 0)
                    {
                        q3 = (from x in q
                              join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                              where temp.Month == ((DateTime)x.tanggal).Month
                              && temp.Year == ((DateTime)x.tanggal).Year
                              && eventT.workDay == y.workDay
                              select (Int64)x.BN100K
                              ).ToList();
                    }
                    query.AddRange(q3);
                }
                result.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)Math.Round(query.Any() ? query.Average(x => x) : 0, 0) });
                tanggal = tanggal.AddDays(1);
            }
            return result;
        }
        List<tanggalValue> loadPrediksiInRetail50()
        {
            List<tanggalValue> result = new List<tanggalValue>();
            List<DateTime> listTanggalHistorisUntukPrediksi = loadTanggalHistorisUntukPrediksi();

            //Load semua in Cabang 100
            var q = (from x in db.DailyStocks
                     where x.in_out.ToUpper() == "IN"
                     && x.jenisTransaksi.Contains("Collection Retail")
                     select new { x.tanggal, x.BN50K });
            List<EventTanggal> et = (from x in db.EventTanggals.AsEnumerable().AsEnumerable()
                                     select x).ToList();


            DateTime tanggal = DateTime.Today;
            DateTime maxTanggal = tanggalMaxPrediksiPicker.Value;

            while (tanggal <= maxTanggal)
            {
                List<Int64> query = new List<Int64>();
                var eventT = (from x in et
                              where x.tanggal == tanggal
                              select new { x.workDay, x.@event }).FirstOrDefault();


                foreach (var temp in listTanggalHistorisUntukPrediksi)
                {
                    List<Int64> q3 = (from x in q
                                      join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                      where temp.Month == ((DateTime)x.tanggal).Month
                                      && temp.Year == ((DateTime)x.tanggal).Year
                                      && eventT.workDay == y.workDay
                                      && eventT.@event == y.@event
                                      select new { tanggal = (DateTime)x.tanggal, BN50K = (Int64)x.BN50K }
                              ).AsEnumerable().Where(x => x.tanggal.DayOfWeek == tanggal.DayOfWeek).Select(x => x.BN50K).ToList();

                    if (q3.Count == 0)
                    {
                        q3 = (from x in q
                              join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                              where temp.Month == ((DateTime)x.tanggal).Month
                              && temp.Year == ((DateTime)x.tanggal).Year
                              && eventT.workDay == y.workDay
                              select (Int64)x.BN50K
                              ).ToList();
                    }
                    query.AddRange(q3);
                }
                result.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)Math.Round(query.Any() ? query.Average(x => x) : 0, 0) });
                tanggal = tanggal.AddDays(1);
            }
            return result;
        }
        List<tanggalValue> loadPrediksiOutCabang100()
        {
            List<tanggalValue> result = new List<tanggalValue>();
            List<DateTime> listTanggalHistorisUntukPrediksi = loadTanggalHistorisUntukPrediksi();

            //Load semua in Cabang 100
            var q = (from x in db.DailyStocks
                     where x.in_out.ToUpper() == "OUT"
                     && x.jenisTransaksi.Contains("Delivery Cabang")
                     select new { x.tanggal, x.BN100K });
            List<EventTanggal> et = (from x in db.EventTanggals.AsEnumerable().AsEnumerable()
                                     select x).ToList();


            DateTime tanggal = DateTime.Today;
            DateTime maxTanggal = tanggalMaxPrediksiPicker.Value;

            while (tanggal <= maxTanggal)
            {
                List<Int64> query = new List<Int64>();
                var eventT = (from x in et
                              where x.tanggal == tanggal
                              select new { x.workDay, x.@event }).FirstOrDefault();


                foreach (var temp in listTanggalHistorisUntukPrediksi)
                {
                    List<Int64> q3 = (from x in q
                                      join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                      where temp.Month == ((DateTime)x.tanggal).Month
                                      && temp.Year == ((DateTime)x.tanggal).Year
                                      && eventT.workDay == y.workDay
                                      && eventT.@event == y.@event
                                      select new { tanggal = (DateTime)x.tanggal, BN100K = (Int64)x.BN100K }
                              ).AsEnumerable().Where(x => x.tanggal.DayOfWeek == tanggal.DayOfWeek).Select(x => x.BN100K).ToList();

                    if (q3.Count == 0)
                    {
                        q3 = (from x in q
                              join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                              where temp.Month == ((DateTime)x.tanggal).Month
                              && temp.Year == ((DateTime)x.tanggal).Year
                              && eventT.workDay == y.workDay
                              select (Int64)x.BN100K
                              ).ToList();
                    }
                    query.AddRange(q3);
                }
                result.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)Math.Round(query.Any() ? query.Average(x => x) : 0, 0) });
                tanggal = tanggal.AddDays(1);
            }
            return result;
        }
        List<tanggalValue> loadPrediksiOutCabang50()
        {
            List<tanggalValue> result = new List<tanggalValue>();
            List<DateTime> listTanggalHistorisUntukPrediksi = loadTanggalHistorisUntukPrediksi();

            //Load semua in Cabang 50
            var q = (from x in db.DailyStocks
                     where x.in_out.ToUpper() == "OUT"
                     && x.jenisTransaksi.Contains("Delivery Cabang")
                     select new { x.tanggal, x.BN50K });
            List<EventTanggal> et = (from x in db.EventTanggals.AsEnumerable().AsEnumerable()
                                     select x).ToList();


            DateTime tanggal = DateTime.Today;
            DateTime maxTanggal = tanggalMaxPrediksiPicker.Value;

            while (tanggal <= maxTanggal)
            {
                List<Int64> query = new List<Int64>();
                var eventT = (from x in et
                              where x.tanggal == tanggal
                              select new { x.workDay, x.@event }).FirstOrDefault();


                foreach (var temp in listTanggalHistorisUntukPrediksi)
                {
                    List<Int64> q3 = (from x in q
                                      join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                      where temp.Month == ((DateTime)x.tanggal).Month
                                      && temp.Year == ((DateTime)x.tanggal).Year
                                      && eventT.workDay == y.workDay
                                      && eventT.@event == y.@event
                                      select new { tanggal = (DateTime)x.tanggal, BN50K = (Int64)x.BN50K }
                              ).AsEnumerable().Where(x => x.tanggal.DayOfWeek == tanggal.DayOfWeek).Select(x => x.BN50K).ToList();

                    if (q3.Count == 0)
                    {
                        q3 = (from x in q
                              join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                              where temp.Month == ((DateTime)x.tanggal).Month
                              && temp.Year == ((DateTime)x.tanggal).Year
                              && eventT.workDay == y.workDay
                              select (Int64)x.BN50K
                              ).ToList();
                    }
                    query.AddRange(q3);
                }
                result.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)Math.Round(query.Any() ? query.Average(x => x) : 0, 0) });
                tanggal = tanggal.AddDays(1);
            }
            return result;
        }
        List<tanggalValue> loadPrediksiOutAtm100()
        {
            List<tanggalValue> result = new List<tanggalValue>();
            List<DateTime> listTanggalHistorisUntukPrediksi = loadTanggalHistorisUntukPrediksi();
            List<tanggalValue> isiAtm100 = new List<tanggalValue>(),
                isiCrm100 = new List<tanggalValue>(),
                sislokCdm100 = new List<tanggalValue>(),
                sislokCrm100 = new List<tanggalValue>(),
                saldo100 = new List<tanggalValue>();
            List<tanggalRasio> sislokAtm100 = new List<tanggalRasio>();
            Double targetRasio100;

            Double buf;
            if (Double.TryParse(rasio100TextBox.Text, out buf))
                targetRasio100 = buf;
            else
                targetRasio100 = 0;

            //Load semua in Cabang 100
            var q = (from x in db.TransaksiAtms
                     select x).ToList();

            var et = (from x in db.EventTanggals select x).ToList();


            DateTime tanggal = DateTime.Today;
            DateTime maxTanggal = tanggalMaxPrediksiPicker.Value;

            if (!q.Where(x => x.tanggal == tanggal).Any())
                MessageBox.Show("Data Laporan PKT Hari ini belum ada");
            saldo100.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)q.Where(x => x.tanggal == tanggal).Select(x => x.saldoAwal100).Sum() });
            Console.WriteLine(saldo100.Count);
            while(tanggal<=maxTanggal){
                
                List<TransaksiAtm> query = new List<TransaksiAtm>();
                var eventT = (from x in et
                              where x.tanggal == tanggal
                              select new { x.workDay, x.@event }).FirstOrDefault();


                foreach (var temp in listTanggalHistorisUntukPrediksi)
                {
                    List<TransaksiAtm> q3 = (from x in q
                                      join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                      where temp.Month == ((DateTime)x.tanggal).Month
                                      && temp.Year == ((DateTime)x.tanggal).Year
                                      && eventT.workDay == y.workDay
                                      && eventT.@event == y.@event
                                      select x
                              ).AsEnumerable().Where(x => x.tanggal.DayOfWeek == tanggal.DayOfWeek).ToList();

                    if (q3.Count == 0)
                    {
                        q3 = (from x in q
                              join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                              where temp.Month == ((DateTime)x.tanggal).Month
                              && temp.Year == ((DateTime)x.tanggal).Year
                              && eventT.workDay == y.workDay
                              select x
                              ).ToList();
                    }
                    query.AddRange(q3);
                }
                
                var query2 = (from x in query
                              group x by x.tanggal into g
                              select new {
                                  isiATM100 = g.Sum(x => x.isiATM100),
                                  isiCRM100 = g.Sum(x => x.isiCRM100),
                                  sislokATM100 = g.Sum(x => x.sislokATM100),
                                  sislokCRM100 = g.Sum(x => x.sislokCRM100),
                                  sislokCDM100 = g.Sum(x => x.sislokCDM100),
                                  RasioSislokAtm100 = g.Average(x=> (Double) x.sislokATM100/(Double)(x.isiATM100 == 0 ? 1 : x.isiATM100))
                              });
                isiAtm100.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)Math.Round((Double)query2.Average(x => x.isiATM100), 0) });
                isiCrm100.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)Math.Round((Double)query2.Average(x => x.isiATM100), 0) });
                sislokAtm100.Add(new tanggalRasio() { tanggal = tanggal, value = Math.Round((Double)query2.Average(x => x.RasioSislokAtm100), 2) });
                sislokCrm100.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)Math.Round((Double)query2.Average(x => x.sislokCRM100), 0) });
                sislokCdm100.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)Math.Round((Double)query2.Average(x => x.sislokCDM100), 0) });

                tanggal = tanggal.AddDays(1);
            }

            Console.WriteLine("Sislok ATM 100");
            Console.WriteLine("================");
            foreach (var temp in sislokAtm100)
            {
                Console.WriteLine(temp.tanggal + " " + temp.value.ToString("n2"));
            }

            
            DateTime today = DateTime.Today.Date;
            var bon = (from x in db.laporanBons
                   where x.tanggal >= today
                   select x).ToList();

            var setoran = (from x in db.Approvals
                           join y in db.DetailApprovals on x.idApproval equals y.idApproval
                           select new { y.tanggal, y.setor100 }).ToList();

            //Udah ada saldo awal dan saldo ideal untuk h+2

            saldo100.Add(new tanggalValue()
            {
                tanggal = today.AddDays(1),
                value = 
                    saldo100[0].value 
                    + (Int64)Math.Round(sislokAtm100[0].value * isiAtm100[0].value , 0)
                    + sislokCdm100[0].value 
                    + sislokCrm100[0].value 
                    - isiAtm100[0].value 
                    - isiCrm100[0].value 
                    + (bon.Any() ? (Int64)bon[0].C100 : 0 )
                    - (setoran.Where(x=>x.tanggal == tanggal).Select(x=>x.setor100).Any()?(Int64)setoran.Where(x=>x.tanggal == today).Select(x=>x.setor100).ToList()[setoran.Where(x => x.tanggal == today).Select(x => x.setor100).ToList().Count - 1] : 0)
            });
            
            for(int a=2;a<maxTanggal.CompareTo(today)+1;a++)
            {
                saldo100.Add(new tanggalValue() { tanggal = today.AddDays(a), value = (Int64) Math.Round(targetRasio100 * isiAtm100[a].value,0) });
            }

            Console.WriteLine("Saldo Awal");
            Console.WriteLine("=============");
            foreach(var temp in saldo100)
            {
                Console.WriteLine(temp.tanggal + " " + temp.value.ToString("C", CultureInfo.GetCultureInfo("id-ID")));
            }

            return result;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            loadForm.ShowSplashScreen();
            List<DateTime> listTanggal = loadTanggalHistorisUntukPrediksi();

            List<tanggalValue> prediksiInCabang100 = loadPrediksiInCabang100();
            List<tanggalValue> prediksiInCabang50 = loadPrediksiInCabang50();
            List<tanggalValue> prediksiInRetail100 = loadPrediksiInRetail100();
            List<tanggalValue> prediksiInRetail50 = loadPrediksiInRetail50();
            List<tanggalValue> prediksiOutCabang100 = loadPrediksiOutCabang100();
            List<tanggalValue> prediksiOutCabang50 = loadPrediksiOutCabang50();
            List<tanggalValue> prediksiOutAtm100 = loadPrediksiOutAtm100();

            List<ForecastDetail> list = new List<ForecastDetail>();
            DateTime tgl = DateTime.Today.Date;
            DateTime maxTanggal = tanggalMaxPrediksiPicker.Value.Date;
            while (tgl<= maxTanggal)
            {
                list.Add(new ForecastDetail() { tanggal = tgl,
                    inCabang100 = prediksiInCabang100.Where(x => x.tanggal == tgl).Select(x => x.value).FirstOrDefault(),
                    inRetail100 = prediksiInRetail100.Where(x => x.tanggal == tgl).Select(x => x.value).FirstOrDefault(),
                    inCabang50 = prediksiInCabang50.Where(x => x.tanggal == tgl).Select(x => x.value).FirstOrDefault(),
                    inRetail50 = prediksiInRetail50.Where(x => x.tanggal == tgl).Select(x => x.value).FirstOrDefault(),
                    outCabang100 = prediksiOutCabang100.Where(x => x.tanggal == tgl).Select(x => x.value).FirstOrDefault(),
                    outCabang50 = prediksiOutCabang50.Where(x => x.tanggal == tgl).Select(x => x.value).FirstOrDefault()
                });
                tgl = tgl.AddDays(1);
            }
            dataGridView1.DataSource = list;
            loadForm.CloseForm();
        }
    }
    class tanggalValue
    {
        public DateTime tanggal { set; get; }
        public Int64 value { set; get; }
    }
    class tanggalRasio
    {
        public DateTime tanggal { set; get; }
        public Double value { set; get; }
    }
}
