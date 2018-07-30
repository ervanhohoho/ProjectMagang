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
        List<ForecastDetail> list = new List<ForecastDetail>();

        List<tanggalValue> prediksiInCabang100; 
        List<tanggalValue> prediksiInCabang50;
        List<tanggalValue> prediksiInRetail100;
        List<tanggalValue> prediksiInRetail50;
        List<tanggalValue> prediksiOutCabang100;
        List<tanggalValue> prediksiOutCabang50;
        List<tanggalValue> prediksiOutAtm100;
        List<tanggalValue> prediksiOutAtm50;
        List<tanggalValue> morningBalance100;
        List<tanggalValue> morningBalance50;
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

            var q = (from x in db.StokPosisis select (DateTime)x.tanggal).ToList();
            minTanggal = q.Min(x => x);
            maxTanggal = q.Max(x => x);
            DateTime tempTanggal = new DateTime(minTanggal.Year, minTanggal.Month, 1);
            
            int counter = 0;
            while (tempTanggal <= maxTanggal)
            {
                Console.WriteLine(tempTanggal);
                bulanPrediksiTreeView.Nodes.Add(tempTanggal.Year.ToString());
                int monthCounter = tempTanggal.Month;
                while (tempTanggal < maxTanggal && monthCounter <= 12)
                {
                    Console.WriteLine(tempTanggal);
                    bulanPrediksiTreeView.Nodes[counter].Nodes.Add((monthCounter++).ToString());
                    tempTanggal = tempTanggal.AddMonths(1);
                }
                tempTanggal = tempTanggal.AddMonths(1);
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
                                      select new { tanggal = (DateTime)x.tanggal, BN100K = x.BN100K == null? 0:(Int64)x.BN100K }
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
                                      select new { tanggal = (DateTime)x.tanggal, BN50K = x.BN50K == null? 0:(Int64)x.BN50K }
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
                                      select new { tanggal = (DateTime)x.tanggal, BN100K = x.BN100K == null? 0:(Int64)x.BN100K }
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
                                      select new { tanggal = (DateTime)x.tanggal, BN50K = x.BN50K == null? 0:(Int64)x.BN50K }
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
                                      select new { tanggal = (DateTime)x.tanggal, BN100K = x.BN100K == null? 0:(Int64)x.BN100K }
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
                                      select new { tanggal = (DateTime)x.tanggal, BN50K = x.BN50K == null? 0:(Int64)x.BN50K }
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
            Console.WriteLine("Prediksi ATM");
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
                     join y in db.Pkts on x.kodePkt equals y.kodePkt
                     where y.kanwil.ToUpper().Contains("JABO")
                     select x).ToList();

            var et = (from x in db.EventTanggals select x).ToList();


            DateTime tanggal = DateTime.Today;
            DateTime maxTanggal = tanggalMaxPrediksiPicker.Value;

            while (tanggal <= maxTanggal.AddDays(1))
            {

                List<TransaksiAtm> query = new List<TransaksiAtm>();
                var eventT = (from x in et
                              where x.tanggal == tanggal
                              select new { x.tanggal, x.workDay, x.@event }).FirstOrDefault();


                foreach (var temp in listTanggalHistorisUntukPrediksi)
                {
                    List<TransaksiAtm> q3 = (from x in q
                                             join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                             where temp.Month == ((DateTime)x.tanggal).Month
                                             && temp.Year == ((DateTime)x.tanggal).Year
                                             && eventT.workDay == y.workDay
                                             && eventT.@event == y.@event
                                             && eventT.tanggal.DayOfWeek == y.tanggal.DayOfWeek
                                             select x
                              ).AsEnumerable().Where(x => x.tanggal.DayOfWeek == tanggal.DayOfWeek).ToList();

                    if (q3.Count == 0)
                    {
                        q3 = (from x in q
                              join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                              where temp.Month == ((DateTime)x.tanggal).Month
                              && temp.Year == ((DateTime)x.tanggal).Year
                              && eventT.workDay == y.workDay
                              && eventT.@event == y.@event
                              select x
                              ).ToList();
                    }
                    query.AddRange(q3);
                }
                //Disum dulu untuk semua PKT
                var query2 = (from x in query
                              group x by x.tanggal into g
                              select new
                              {
                                  tanggal = g.Key,
                                  isiATM100 = g.Sum(x => x.isiATM100),
                                  isiCRM100 = g.Sum(x => x.isiCRM100),
                                  sislokATM100 = g.Sum(x => x.sislokATM100),
                                  sislokCRM100 = g.Sum(x => x.sislokCRM100),
                                  sislokCDM100 = g.Sum(x => x.sislokCDM100),
                                  RasioSislokAtm100 = g.Average(x => (Double)x.sislokATM100 / (Double)(x.isiATM100 == 0 ? 1 : x.isiATM100))
                              }).ToList();
                Console.WriteLine("Jumlah Query2: " + query2.Count);

                isiAtm100.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)Math.Round((Double)query2.Average(x => x.isiATM100), 0) });
                isiCrm100.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)Math.Round((Double)query2.Average(x => x.isiCRM100), 0) });
                sislokAtm100.Add(new tanggalRasio() { tanggal = tanggal, value = Math.Round((Double)query2.Average(x => x.RasioSislokAtm100), 2) });
                sislokCrm100.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)Math.Round((Double)query2.Average(x => x.sislokCRM100), 0) });
                sislokCdm100.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)Math.Round((Double)query2.Average(x => x.sislokCDM100), 0) });

                tanggal = tanggal.AddDays(1);
            }




            DateTime today = DateTime.Today.Date;
            DateTime tanggalLastApproval = db.Approvals.Max(x => x.tanggal);
            var bon = (from x in db.Approvals.AsEnumerable()
                       join z in db.DetailApprovals.AsEnumerable() on x.idApproval equals z.idApproval
                       join y in db.Pkts.AsEnumerable() on x.kodePkt equals y.kodePkt
                       where z.tanggal >= today
                       && y.kanwil.ToUpper().Contains("JABO")
                       && ((DateTime)x.tanggal).Date == tanggalLastApproval.Date
                       && z.bon100 != -1
                       group new { x, z } by (DateTime)z.tanggal into g
                       select new { g.Key, C100 = g.Sum(x => x.z.bon100) }
                       ).ToList();

            var listApproval = (from x in db.Approvals
                                join y in db.DetailApprovals on x.idApproval equals y.idApproval
                                join z in db.Pkts on x.kodePkt equals z.kodePkt
                                where z.kanwil.ToUpper().Contains("JABO")
                                group y by y.tanggal into g
                                select new { g.Key, setor100 = g.Sum(x => x.setor100), bon100 = g.Sum(x => x.bon100) }).ToList();

            //Udah ada saldo awal dan saldo ideal untuk h+2
            DateTime tanggalKemarin = Variables.todayDate.AddDays(-1).Date;
            saldo100.Add(new tanggalValue() { tanggal = today, value = (Int64) (from x in db.TransaksiAtms.AsEnumerable() join y in db.Pkts on x.kodePkt equals y.kodePkt where y.kanwil.ToUpper().Contains("JABO") && ((DateTime)x.tanggal).Date == tanggalKemarin.Date group x by true into g select g.Sum(x=>x.saldoAkhir100)).FirstOrDefault() });

            //hitung saldo tiap hari dari laporan bon dan prediksi

            Console.WriteLine("Mulai Add Saldo Awal");
            for (int a = 0; a < bon.Count; a++)
            {
                Console.WriteLine("A: " + a);
                Int64 tSaldo = saldo100.Where(x => x.tanggal == today.AddDays(a)).Select(x => x.value).FirstOrDefault(),
                    tSislokAtm = (Int64)Math.Round(sislokAtm100.Where(x => x.tanggal == today.AddDays(a)).Select(x => x.value).First() * isiAtm100.Where(x => x.tanggal == today.AddDays(a)).Select(x => x.value).First(), 0),
                    tSislokCdm = sislokCdm100.Where(x => x.tanggal == today.AddDays(a)).Select(x => x.value).First(),
                    tSislokCrm = sislokCrm100.Where(x => x.tanggal == today.AddDays(a)).Select(x => x.value).First(),
                    tIsiAtm = isiAtm100.Where(x => x.tanggal == today.AddDays(a)).Select(x => x.value).First(),
                    tIsiCrm = isiCrm100.Where(x => x.tanggal == today.AddDays(a)).Select(x => x.value).First(),
                    tBon = (Int64) bon[a].C100;

                Int64 value =
                      tSaldo
                      +tSislokAtm
                      +tSislokCdm
                      +tSislokCrm
                      - isiAtm100.Where(x => x.tanggal == today.AddDays(a)).Select(x => x.value).First()
                      - isiCrm100.Where(x => x.tanggal == today.AddDays(a)).Select(x => x.value).First()
                      + tBon
                      - (listApproval.Where(x => x.Key == tanggal).Select(x => x.setor100).Any() ? (Int64)listApproval.Where(x => x.Key == today.AddDays(a)).Select(x => x.setor100).ToList()[listApproval.Where(x => x.Key == today.AddDays(a)).Select(x => x.setor100).ToList().Count - 1] : 0);

                Console.WriteLine(today.AddDays(a) + " Value Saldo: " + tSaldo);
                Console.WriteLine(today.AddDays(a) + " Value Sislok ATM: " + tSislokAtm);
                Console.WriteLine(today.AddDays(a) + " Value Sislok CDM: " + tSislokCdm);
                Console.WriteLine(today.AddDays(a) + " Value Sislok CRM: " + tSislokCrm);
                Console.WriteLine(today.AddDays(a) + " Value Isi ATM: " + tIsiAtm);
                Console.WriteLine(today.AddDays(a) + " Value Isi CRM: " + tIsiCrm);
                Console.WriteLine(today.AddDays(a) + " Value Bon: " + tBon);
                Console.WriteLine(today.AddDays(a + 1) + " Value Saldo awal: " + value);

                saldo100.Add(new tanggalValue()
                {
                    tanggal = today.AddDays(a + 1),
                    value = value
                });
                if (a >= 0)
                    result.Add(new tanggalValue() { tanggal = bon[a].Key, value = (Int64)bon[a].C100 });
            }

            Console.WriteLine("Result Sementara\n===============");
            foreach (var temp in result)
                Console.WriteLine(temp.value.ToString("n2"));
            ////Cek bon di approval
            //if (listApproval.Max(x => x.Key) > saldo100.Max(x => x.tanggal))
            //{
            //    DateTime tTanggal = saldo100.Max(x => x.tanggal);
            //    var tempLA = listApproval.Where(x => x.Key == tTanggal).FirstOrDefault();
            //    int idx = listApproval.IndexOf(tempLA);

            //    for (int a = idx; a < listApproval.Count; a++)
            //    {
            //        Int64 tSaldo = saldo100.Where(x => x.tanggal == today.AddDays(a)).Select(x => x.value).FirstOrDefault(),
            //            tSislokAtm = (Int64)Math.Round(sislokAtm100.Where(x => x.tanggal == today.AddDays(a)).Select(x => x.value).FirstOrDefault() * isiAtm100.Where(x => x.tanggal == today.AddDays(a)).Select(x => x.value).FirstOrDefault(), 0),
            //            tSislokCdm = sislokCdm100.Where(x => x.tanggal == today.AddDays(a)).Select(x => x.value).FirstOrDefault(),
            //            tSislokCrm = sislokCrm100.Where(x => x.tanggal == today.AddDays(a)).Select(x => x.value).FirstOrDefault(),
            //            tIsiAtm = isiAtm100.Where(x => x.tanggal == today.AddDays(a)).Select(x => x.value).FirstOrDefault(),
            //            tIsiCrm = isiCrm100.Where(x => x.tanggal == today.AddDays(a)).Select(x => x.value).FirstOrDefault(),
            //            tBon = bon.Where(x => x.Key == tTanggal.AddDays(a)).Select(x => x.C100).FirstOrDefault() == null ? 0 : (Int64)bon.Where(x => x.Key == tTanggal.AddDays(a)).Select(x => x.C100).FirstOrDefault();

            //        Console.WriteLine(tSaldo + "\n" + tSislokAtm + "\n" + tSislokCdm + "\n" + tSislokCrm + "\n" + tIsiAtm + "\n" + tIsiCrm + "\n" + tBon);
            //        saldo100.Add(new tanggalValue()
            //        {
            //            tanggal = tTanggal.AddDays(a + 1),
            //            value =
            //              saldo100.Where(x => x.tanggal == tTanggal.AddDays(a)).Select(x => x.value).FirstOrDefault()
            //              + (Int64)Math.Round(sislokAtm100.Where(x => x.tanggal == tTanggal.AddDays(a)).Select(x => x.value).FirstOrDefault() * isiAtm100.Where(x => x.tanggal == tTanggal.AddDays(a)).Select(x => x.value).FirstOrDefault(), 0)
            //              + sislokCdm100.Where(x => x.tanggal == tTanggal.AddDays(a)).Select(x => x.value).FirstOrDefault()
            //              + sislokCrm100.Where(x => x.tanggal == tTanggal.AddDays(a)).Select(x => x.value).FirstOrDefault()
            //              - isiAtm100.Where(x => x.tanggal == tTanggal.AddDays(a)).Select(x => x.value).FirstOrDefault()
            //              - isiCrm100.Where(x => x.tanggal == tTanggal.AddDays(a)).Select(x => x.value).FirstOrDefault()
            //              + (Int64)bon.Where(x => x.Key == tTanggal.AddDays(a)).Select(x => x.C100).FirstOrDefault()
            //              - (listApproval.Where(x => x.Key == tanggal).Select(x => x.setor100).Any() ? (Int64)listApproval.Where(x => x.Key == tTanggal.AddDays(a)).Select(x => x.setor100).ToList()[listApproval.Where(x => x.Key == tTanggal.AddDays(a)).Select(x => x.setor100).ToList().Count - 1] : 0)
            //        });
            //    }
            //}

            var q2 = (from x in db.Approvals
                      join y in db.DetailApprovals on x.idApproval equals y.idApproval
                      join z in db.Pkts on x.kodePkt equals z.kodePkt
                      where ((DateTime)y.tanggal) >= today
                      && y.bon100 != -1
                      && z.kanwil.ToUpper().Contains("JABO")
                      select new { A = x, DA = y }).ToList();
            if (saldo100.Max(x => x.tanggal) < q2.Max(x => x.DA.tanggal))
            {
                DateTime tempT = saldo100.Max(x => x.tanggal);
                
                while (tempT <= q2.Max(x => x.DA.tanggal))
                {
                    Int64 tempBon = (Int64)q2.Where(x => x.A.tanggal == tanggalLastApproval && x.DA.tanggal == tempT).Sum(x => x.DA.bon100);
                    Console.WriteLine("Temp Bon: " + tempBon);
                    Int64 saldo = saldo100.Where(x => x.tanggal == tempT).Select(x => x.value).First()
                          + (Int64)Math.Round(sislokAtm100.Where(x => x.tanggal == tempT).Select(x => x.value).First() * isiAtm100.Where(x => x.tanggal == tempT).Select(x => x.value).First(), 0)
                          + sislokCdm100.Where(x => x.tanggal == tempT).Select(x => x.value).First()
                          + sislokCrm100.Where(x => x.tanggal == tempT).Select(x => x.value).First()
                          - isiAtm100.Where(x => x.tanggal == tempT).Select(x => x.value).First()
                          - isiCrm100.Where(x => x.tanggal == tempT).Select(x => x.value).First()
                          + tempBon;
                    tempT = tempT.AddDays(1);
                    saldo100.Add(new tanggalValue()
                    {
                        tanggal = tempT,
                        value = saldo
                    });
                }

            }

            for (DateTime a = saldo100.Select(x => x.tanggal).Max().AddDays(1); a <= maxTanggal.AddDays(1); a = a.AddDays(1))
            {
                saldo100.Add(new tanggalValue() { tanggal = a, value = (Int64)Math.Round(targetRasio100 * (isiAtm100.Where(x => x.tanggal == a).Select(x => x.value).First() + isiCrm100.Where(x=>x.tanggal == a).Select(x=>x.value).First()), 0) });
            }


            if (result.Any())
            {
                for (DateTime a = result.Select(x => x.tanggal).Max().AddDays(1); a <= maxTanggal; a = a.AddDays(1))
                {
                    Int64 tsaldoAkhirIdeal = saldo100.Where(x => x.tanggal == a.AddDays(1)).Select(x => x.value).First();
                    Int64 tsaldoAwal = saldo100.Where(x => x.tanggal == a).Select(x => x.value).First();
                    Int64 tsislokAtm = (Int64)(sislokAtm100.Where(x => x.tanggal == a).Select(x => x.value).First() * isiAtm100.Where(x => x.tanggal == a).Select(x => x.value).First());
                    Int64 tsislokCrm = sislokCrm100.Where(x => x.tanggal == a).Select(x => x.value).First();
                    Int64 tsislokCdm = sislokCdm100.Where(x => x.tanggal == a).Select(x => x.value).First();
                    Int64 tisiAtm = isiAtm100.Where(x => x.tanggal == a).Select(x => x.value).First();
                    Int64 tisiCrm = isiCrm100.Where(x => x.tanggal == a).Select(x => x.value).First();
                    Int64 tsetor = 0;
                    if (listApproval.Where(x=>x.Key == a).Any())
                        tsetor = (Int64) (listApproval.Where(x => x.Key == a).Select(x => x.setor100).FirstOrDefault() == null ? listApproval.Where(x => x.Key == a).Select(x => x.setor100).First() : 0); 


                    Int64 nilaibon = tsaldoAkhirIdeal - tsaldoAwal - tsislokAtm - tsislokCdm - tsislokCrm + tisiAtm + tisiCrm + tsetor;
                    result.Add(new tanggalValue() { tanggal = a, value = nilaibon });
                }
            }
            else
            {
                for (DateTime a = today; a <= maxTanggal; a = a.AddDays(1))
                {
                    Int64 tsaldoAkhirIdeal = saldo100.Where(x => x.tanggal == a.AddDays(1)).Select(x => x.value).First();
                    Int64 tsaldoAwal = saldo100.Where(x => x.tanggal == a).Select(x => x.value).First();
                    Int64 tsislokAtm = (Int64)(sislokAtm100.Where(x => x.tanggal == a).Select(x => x.value).First() * isiAtm100.Where(x => x.tanggal == a).Select(x => x.value).First());
                    Int64 tsislokCrm = sislokCrm100.Where(x => x.tanggal == a).Select(x => x.value).First();
                    Int64 tsislokCdm = sislokCdm100.Where(x => x.tanggal == a).Select(x => x.value).First();
                    Int64 tisiAtm = isiAtm100.Where(x => x.tanggal == a).Select(x => x.value).First();
                    Int64 tisiCrm = isiCrm100.Where(x => x.tanggal == a).Select(x => x.value).First();
                    Int64 tsetor = 0;
                    if (listApproval.Where(x => x.Key == a).Select(x => x.setor100).ToList().Any())
                    {
                        tsetor = (Int64)listApproval.Where(x => x.Key == a).Select(x => x.setor100).ToList()[0];
                    }
                    Int64 nilaibon = tsaldoAkhirIdeal - tsaldoAwal - tsislokAtm - tsislokCdm - tsislokCrm + tisiAtm + tisiCrm + tsetor;
                    result.Add(new tanggalValue() { tanggal = a, value = nilaibon });
                }
            }
            Console.WriteLine("Saldo Awal");
            Console.WriteLine("=============");
            foreach (var temp in saldo100)
            {
                Console.WriteLine(temp.tanggal + " " + temp.value.ToString("C", CultureInfo.GetCultureInfo("id-ID")));
            }
            Console.WriteLine("Sislok ATM 100");
            Console.WriteLine("================");
            foreach (var temp in sislokAtm100)
            {
                Console.WriteLine(temp.tanggal + " " + temp.value.ToString("n2"));
            }

            Console.WriteLine("Isi ATM 100");
            Console.WriteLine("================");
            foreach (var temp in isiAtm100)
            {
                Console.WriteLine(temp.tanggal + " " + temp.value.ToString("n2"));
            }

            Console.WriteLine("Rekomendasi Bon");
            Console.WriteLine("================");
            foreach (var temp in result)
            {
                Console.WriteLine(temp.tanggal + " " + temp.value.ToString("C", CultureInfo.GetCultureInfo("id-ID")));
            }

            foreach (var temp in result)
            {
                var tempq = q2.Where(x => x.DA.tanggal == temp.tanggal).ToList();
                if (tempq.Any())
                {
                    tanggalLastApproval = tempq.Max(x => x.A.tanggal);
                    tempq = tempq.Where(x => x.A.tanggal == tanggalLastApproval).ToList();
                    temp.value = (Int64)tempq.Where(x => x.DA.bon50 != -1).Sum(x => x.DA.bon100);
                }
            }


            return result;
        }
        List<tanggalValue> loadPrediksiOutAtm50()
        {
            List<tanggalValue> result = new List<tanggalValue>();
            List<DateTime> listTanggalHistorisUntukPrediksi = loadTanggalHistorisUntukPrediksi();
            List<tanggalValue> isiAtm50 = new List<tanggalValue>(),
                isiCrm50 = new List<tanggalValue>(),
                sislokCdm50 = new List<tanggalValue>(),
                sislokCrm50 = new List<tanggalValue>(),
                saldo50 = new List<tanggalValue>();
            List<tanggalRasio> sislokAtm50 = new List<tanggalRasio>();
            Double targetRasio50;

            Double buf;
            if (Double.TryParse(rasio50TextBox.Text, out buf))
                targetRasio50 = buf;
            else
                targetRasio50 = 0;

            //Load semua in Cabang 50
            var q = (from x in db.TransaksiAtms
                     join y in db.Pkts on x.kodePkt equals y.kodePkt
                     where y.kanwil.ToUpper().Contains("JABO")
                     select x).ToList();

            var et = (from x in db.EventTanggals select x).ToList();


            DateTime tanggal = DateTime.Today;
            DateTime maxTanggal = tanggalMaxPrediksiPicker.Value;

            Console.WriteLine(saldo50.Count);
            while (tanggal <= maxTanggal.AddDays(1))
            {

                List<TransaksiAtm> query = new List<TransaksiAtm>();
                var eventT = (from x in et
                              where x.tanggal == tanggal
                              select new {x.tanggal, x.workDay, x.@event }).FirstOrDefault();


                foreach (var temp in listTanggalHistorisUntukPrediksi)
                {
                    List<TransaksiAtm> q3 = (from x in q
                                             join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                             where temp.Month == ((DateTime)x.tanggal).Month
                                             && temp.Year == ((DateTime)x.tanggal).Year
                                             && eventT.workDay == y.workDay
                                             && eventT.@event == y.@event
                                             && eventT.tanggal.DayOfWeek == y.tanggal.DayOfWeek
                                             select x
                              ).AsEnumerable().Where(x => x.tanggal.DayOfWeek == tanggal.DayOfWeek).ToList();

                    if (q3.Count == 0)
                    {
                        q3 = (from x in q
                              join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                              where temp.Month == ((DateTime)x.tanggal).Month
                              && temp.Year == ((DateTime)x.tanggal).Year
                              && eventT.workDay == y.workDay
                              && eventT.@event == y.@event
                              select x
                              ).ToList();
                    }
                    query.AddRange(q3);
                }

                var query2 = (from x in query
                              group x by x.tanggal into g
                              select new
                              {
                                  isiATM50 = g.Sum(x => x.isiATM50),
                                  isiCRM50 = g.Sum(x => x.isiCRM50),
                                  sislokATM50 = g.Sum(x => x.sislokATM50),
                                  sislokCRM50 = g.Sum(x => x.sislokCRM50),
                                  sislokCDM50 = g.Sum(x => x.sislokCDM50),
                                  RasioSislokAtm50 = g.Average(x => (Double)x.sislokATM50 / (Double)(x.isiATM50 == 0 ? 1 : x.isiATM50))
                              });
                isiAtm50.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)Math.Round((Double)query2.Average(x => x.isiATM50), 0) });
                isiCrm50.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)Math.Round((Double)query2.Average(x => x.isiCRM50), 0) });
                sislokAtm50.Add(new tanggalRasio() { tanggal = tanggal, value = Math.Round((Double)query2.Average(x => x.RasioSislokAtm50), 2) });
                sislokCrm50.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)Math.Round((Double)query2.Average(x => x.sislokCRM50), 0) });
                sislokCdm50.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)Math.Round((Double)query2.Average(x => x.sislokCDM50), 0) });

                tanggal = tanggal.AddDays(1);
            }

            Console.WriteLine("Sislok ATM 50");
            Console.WriteLine("================");
            foreach (var temp in sislokAtm50)
            {
                Console.WriteLine(temp.tanggal + " " + temp.value.ToString("n2"));
            }


            DateTime today = DateTime.Today.Date;
            DateTime tanggalLastApproval = db.Approvals.Max(x => x.tanggal);
            var bon = (from x in db.Approvals.AsEnumerable()
                       join z in db.DetailApprovals.AsEnumerable() on x.idApproval equals z.idApproval
                       join y in db.Pkts.AsEnumerable() on x.kodePkt equals y.kodePkt
                       where z.tanggal >= today
                       && y.kanwil.ToUpper().Contains("JABO")
                       && ((DateTime)x.tanggal).Date == tanggalLastApproval.Date
                       && z.bon100 != -1
                       group new { x, z } by (DateTime)z.tanggal into g
                       select new { g.Key, C50 = g.Sum(x => x.z.bon50) }
                       ).ToList();

            var listApproval = (from x in db.Approvals
                                join y in db.DetailApprovals on x.idApproval equals y.idApproval
                                join z in db.Pkts on x.kodePkt equals z.kodePkt
                                where z.kanwil.ToUpper().Contains("JABO")
                                group y by y.tanggal into g
                                select new { g.Key, setor50 = g.Sum(x => x.setor50), bon50 = g.Sum(x => x.bon50) }).ToList();

            //Udah ada saldo awal dan saldo ideal untuk h+2
            DateTime tanggalKemarin = Variables.todayDate.AddDays(-1).Date;
            Console.WriteLine("Tanggal Kemarin: " + tanggalKemarin);
            saldo50.Add(new tanggalValue() { tanggal = today, value = (Int64)(from x in db.TransaksiAtms.AsEnumerable() join y in db.Pkts on x.kodePkt equals y.kodePkt where y.kanwil.ToUpper().Contains("JABO") && ((DateTime)x.tanggal).Date == tanggalKemarin.Date group x by true into g select g.Sum(x => x.saldoAkhir50)).FirstOrDefault() });

            //hitung saldo tiap hari dari laporan bon dan prediksi
            for (int a = 0; a < bon.Count; a++)
            {
                saldo50.Add(new tanggalValue()
                {
                    tanggal = today.AddDays(a + 1),
                    value =
                      saldo50.Where(x => x.tanggal == today.AddDays(a)).Select(x => x.value).First()
                      + (Int64)Math.Round(sislokAtm50.Where(x => x.tanggal == today.AddDays(a)).Select(x => x.value).First() * isiAtm50.Where(x => x.tanggal == today.AddDays(a)).Select(x => x.value).First(), 0)
                      + sislokCdm50.Where(x => x.tanggal == today.AddDays(a)).Select(x => x.value).First()
                      + sislokCrm50.Where(x => x.tanggal == today.AddDays(a)).Select(x => x.value).First()
                      - isiAtm50.Where(x => x.tanggal == today.AddDays(a)).Select(x => x.value).First()
                      - isiCrm50.Where(x => x.tanggal == today.AddDays(a)).Select(x => x.value).First()
                      + (Int64)bon[a].C50
                      - (listApproval.Where(x => x.Key == tanggal).Select(x => x.setor50).Any() ? (Int64)listApproval.Where(x => x.Key == today.AddDays(a)).Select(x => x.setor50).ToList()[listApproval.Where(x => x.Key == today.AddDays(a)).Select(x => x.setor50).ToList().Count - 1] : 0)
                });
                if (a > 0)
                    result.Add(new tanggalValue() { tanggal = bon[a].Key, value = (Int64)bon[a].C50 });
            }

            ////Cek bon di approval
            //if (listApproval.Max(x => x.Key) > saldo50.Max(x => x.tanggal))
            //{
            //    DateTime tTanggal = saldo50.Max(x => x.tanggal);
            //    var tempLA = listApproval.Where(x => x.Key == tTanggal).FirstOrDefault();
            //    int idx = listApproval.IndexOf(tempLA);

            //    for (int a = idx; a < listApproval.Count; a++)
            //    {
            //        saldo50.Add(new tanggalValue()
            //        {
            //            tanggal = tTanggal.AddDays(a + 1),
            //            value =
            //              saldo50.Where(x => x.tanggal == tTanggal.AddDays(a)).Select(x => x.value).First()
            //              + (Int64)Math.Round(sislokAtm50.Where(x => x.tanggal == tTanggal.AddDays(a)).Select(x => x.value).First() * isiAtm50.Where(x => x.tanggal == tTanggal.AddDays(a)).Select(x => x.value).First(), 0)
            //              + sislokCdm50.Where(x => x.tanggal == tTanggal.AddDays(a)).Select(x => x.value).First()
            //              + sislokCrm50.Where(x => x.tanggal == tTanggal.AddDays(a)).Select(x => x.value).First()
            //              - isiAtm50.Where(x => x.tanggal == tTanggal.AddDays(a)).Select(x => x.value).First()
            //              - isiCrm50.Where(x => x.tanggal == tTanggal.AddDays(a)).Select(x => x.value).First()
            //              + (Int64)bon.Where(x => x.Key == tTanggal.AddDays(a)).Select(x => x.C50).First()
            //              - (listApproval.Where(x => x.Key == tanggal).Select(x => x.setor50).Any() ? (Int64)listApproval.Where(x => x.Key == tTanggal.AddDays(a)).Select(x => x.setor50).ToList()[listApproval.Where(x => x.Key == tTanggal.AddDays(a)).Select(x => x.setor50).ToList().Count - 1] : 0)
            //        });
            //    }
            //}

            var q2 = (from x in db.Approvals
                      join y in db.DetailApprovals on x.idApproval equals y.idApproval
                      join z in db.Pkts on x.kodePkt equals z.kodePkt
                      where ((DateTime)y.tanggal) >= today
                      && y.bon100 != -1
                      && z.kanwil.ToUpper().Contains("JABO")
                      select new { A = x, DA = y }).ToList();
            if(saldo50.Max(x=>x.tanggal) < q2.Max(x=>x.DA.tanggal))
            {
                DateTime tempT = saldo50.Max(x => x.tanggal);
                tanggalLastApproval = q2.Max(x => x.A.tanggal);
                while(tempT <= q2.Max(x=>x.DA.tanggal))
                {
                    Int64 tempBon = (Int64)q2.Where(x => x.A.tanggal == tanggalLastApproval && x.DA.tanggal == tempT).Sum(x => x.DA.bon50);
                    Int64 saldo = saldo50.Where(x => x.tanggal == tempT).Select(x => x.value).First()
                          + (Int64)Math.Round(sislokAtm50.Where(x => x.tanggal == tempT).Select(x => x.value).First() * isiAtm50.Where(x => x.tanggal == tempT).Select(x => x.value).First(), 0)
                          + sislokCdm50.Where(x => x.tanggal == tempT).Select(x => x.value).First()
                          + sislokCrm50.Where(x => x.tanggal == tempT).Select(x => x.value).First()
                          - isiAtm50.Where(x => x.tanggal == tempT).Select(x => x.value).First()
                          - isiCrm50.Where(x => x.tanggal == tempT).Select(x => x.value).First()
                          + tempBon;
                    tempT = tempT.AddDays(1);
                    saldo50.Add(new tanggalValue() {
                        tanggal = tempT,
                        value = saldo
                    });
                }

            }


            for (DateTime a = saldo50.Select(x => x.tanggal).Max().AddDays(1); a <= maxTanggal.AddDays(1); a = a.AddDays(1))
            {
                saldo50.Add(new tanggalValue() { tanggal = a, value = (Int64)Math.Round(targetRasio50 * (isiAtm50.Where(x => x.tanggal == a).Select(x => x.value).First() + isiCrm50.Where(x => x.tanggal == a).Select(x => x.value).First()), 0) });
            }


            if (result.Any())
            {
                for (DateTime a = result.Select(x => x.tanggal).Max().AddDays(1); a <= maxTanggal; a = a.AddDays(1))
                {
                    Int64 tsaldoAkhirIdeal = saldo50.Where(x => x.tanggal == a.AddDays(1)).Select(x => x.value).First();
                    Int64 tsaldoAwal = saldo50.Where(x => x.tanggal == a).Select(x => x.value).First();
                    Int64 tsislokAtm = (Int64)(sislokAtm50.Where(x => x.tanggal == a).Select(x => x.value).First() * isiAtm50.Where(x => x.tanggal == a).Select(x => x.value).First());
                    Int64 tsislokCrm = sislokCrm50.Where(x => x.tanggal == a).Select(x => x.value).First();
                    Int64 tsislokCdm = sislokCdm50.Where(x => x.tanggal == a).Select(x => x.value).First();
                    Int64 tisiAtm = isiAtm50.Where(x => x.tanggal == a).Select(x => x.value).First();
                    Int64 tisiCrm = isiCrm50.Where(x => x.tanggal == a).Select(x => x.value).First();
                    Int64 tsetor = 0;
                    if (listApproval.Where(x => x.Key == a).Any())
                        tsetor = (Int64)(listApproval.Where(x => x.Key == a).Select(x => x.setor50).FirstOrDefault() == null ? listApproval.Where(x => x.Key == a).Select(x => x.setor50).First() : 0);

                    Int64 nilaibon = tsaldoAkhirIdeal - tsaldoAwal - tsislokAtm - tsislokCdm - tsislokCrm + tisiAtm + tisiCrm + tsetor;
                    result.Add(new tanggalValue() { tanggal = a, value = nilaibon });
                }
            }
            else
            {
                for (DateTime a = today; a <= maxTanggal; a = a.AddDays(1))
                {
                    Int64 tsaldoAkhirIdeal = saldo50.Where(x => x.tanggal == a.AddDays(1)).Select(x => x.value).First();
                    Int64 tsaldoAwal = saldo50.Where(x => x.tanggal == a).Select(x => x.value).First();
                    Int64 tsislokAtm = (Int64)(sislokAtm50.Where(x => x.tanggal == a).Select(x => x.value).First() * isiAtm50.Where(x => x.tanggal == a).Select(x => x.value).First());
                    Int64 tsislokCrm = sislokCrm50.Where(x => x.tanggal == a).Select(x => x.value).First();
                    Int64 tsislokCdm = sislokCdm50.Where(x => x.tanggal == a).Select(x => x.value).First();
                    Int64 tisiAtm = isiAtm50.Where(x => x.tanggal == a).Select(x => x.value).First();
                    Int64 tisiCrm = isiCrm50.Where(x => x.tanggal == a).Select(x => x.value).First();
                    Int64 tsetor = 0;
                    if (listApproval.Where(x => x.Key == a).Select(x => x.setor50).ToList().Any())
                    {
                        tsetor = (Int64)listApproval.Where(x => x.Key == a).Select(x => x.setor50).ToList()[0];
                    }
                    Int64 nilaibon = tsaldoAkhirIdeal - tsaldoAwal - tsislokAtm - tsislokCdm - tsislokCrm + tisiAtm + tisiCrm + tsetor;
                    result.Add(new tanggalValue() { tanggal = a, value = nilaibon });
                }
            }
            Console.WriteLine("Saldo Awal");
            Console.WriteLine("=============");
            foreach (var temp in saldo50)
            {
                Console.WriteLine(temp.tanggal + " " + temp.value.ToString("C", CultureInfo.GetCultureInfo("id-ID")));
            }

            Console.WriteLine("Rekomendasi Bon");
            Console.WriteLine("================");
            foreach (var temp in result)
            {
                Console.WriteLine(temp.tanggal + " " + temp.value.ToString("C", CultureInfo.GetCultureInfo("id-ID")));
            }
           
            foreach(var temp in result)
            {
                var tempq = q2.Where(x => x.DA.tanggal == temp.tanggal).ToList();
                if(tempq.Any())
                {
                    tanggalLastApproval = tempq.Max(x => x.A.tanggal);
                    tempq = tempq.Where(x => x.A.tanggal == tanggalLastApproval).ToList();
                    temp.value = (Int64) tempq.Where(x=>x.DA.bon50 != -1).Sum(x => x.DA.bon50);
                }
            }

            return result;
        }

        List<tanggalValue> loadPrediksiInCabang100SP()
        {
            List<tanggalValue> result = new List<tanggalValue>();
            List<DateTime> listTanggalHistorisUntukPrediksi = loadTanggalHistorisUntukPrediksi();

            //Load semua in Cabang 100
            var q = (from x in db.StokPosisis
                     where x.denom == "100000"
                     select new { x.tanggal, BN100K = x.inCabang });

            
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
                                      select new { tanggal = (DateTime)x.tanggal, BN100K = x.BN100K == null? 0 : (Int64)x.BN100K }
                              ).AsEnumerable().Where(x => x.tanggal.DayOfWeek == tanggal.DayOfWeek).Select(x => x.BN100K).ToList();

                    if (q3.Count == 0)
                    {
                        q3 = (from x in q
                              join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                              where temp.Month == ((DateTime)x.tanggal).Month
                              && temp.Year == ((DateTime)x.tanggal).Year
                              && eventT.workDay == y.workDay
                              select x.BN100K == null ? 0 : (Int64)x.BN100K
                              ).ToList();
                    }
                    query.AddRange(q3);
                }
                result.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)Math.Round(query.Any() ? query.Average(x => x) : 0, 0) });
                tanggal = tanggal.AddDays(1);
            }
            return result;
        }
        List<tanggalValue> loadPrediksiInCabang50SP()
        {
            List<tanggalValue> result = new List<tanggalValue>();
            List<DateTime> listTanggalHistorisUntukPrediksi = loadTanggalHistorisUntukPrediksi();

            var q = (from x in db.StokPosisis
                     where x.denom == "50000"
                     select new { x.tanggal, BN50K = x.inCabang });
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
                                      select new { tanggal = (DateTime)x.tanggal, BN50K = x.BN50K == null ? 0 : (Int64)x.BN50K }
                              ).AsEnumerable().Where(x => x.tanggal.DayOfWeek == tanggal.DayOfWeek).Select(x => x.BN50K).ToList();

                    if (q3.Count == 0)
                    {
                        q3 = (from x in q
                              join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                              where temp.Month == ((DateTime)x.tanggal).Month
                              && temp.Year == ((DateTime)x.tanggal).Year
                              && eventT.workDay == y.workDay
                              select x.BN50K == null? 0 : (Int64)x.BN50K
                              ).ToList();
                    }
                    query.AddRange(q3);
                }
                result.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)Math.Round(query.Any() ? query.Average(x => x) : 0, 0) });
                tanggal = tanggal.AddDays(1);
            }
            return result;
        }
        List<tanggalValue> loadPrediksiInRetail100SP()
        {
            List<tanggalValue> result = new List<tanggalValue>();
            List<DateTime> listTanggalHistorisUntukPrediksi = loadTanggalHistorisUntukPrediksi();

            //Load semua in Cabang 100
            var q = (from x in db.StokPosisis
                     where x.denom == "100000"
                     select new { x.tanggal, BN100K = x.inRetail});
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
                                      select new { tanggal = (DateTime)x.tanggal, BN100K = x.BN100K == null? 0: (Int64)x.BN100K }
                              ).AsEnumerable().Where(x => x.tanggal.DayOfWeek == tanggal.DayOfWeek).Select(x => x.BN100K).ToList();

                    if (q3.Count == 0)
                    {
                        q3 = (from x in q
                              join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                              where temp.Month == ((DateTime)x.tanggal).Month
                              && temp.Year == ((DateTime)x.tanggal).Year
                              && eventT.workDay == y.workDay
                              select x.BN100K == null ? 0 : (Int64)x.BN100K
                              ).ToList();
                    }
                    query.AddRange(q3);
                }
                result.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)Math.Round(query.Any() ? query.Average(x => x) : 0, 0) });
                tanggal = tanggal.AddDays(1);
            }
            return result;
        }
        List<tanggalValue> loadPrediksiInRetail50SP()
        {
            List<tanggalValue> result = new List<tanggalValue>();
            List<DateTime> listTanggalHistorisUntukPrediksi = loadTanggalHistorisUntukPrediksi();

            var q = (from x in db.StokPosisis
                     where x.denom == "50000"
                     select new { x.tanggal, BN50K = x.inRetail});
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
                                      select new { tanggal = (DateTime)x.tanggal, BN50K = x.BN50K == null? 0 : (Int64)x.BN50K }
                              ).AsEnumerable().Where(x => x.tanggal.DayOfWeek == tanggal.DayOfWeek).Select(x => x.BN50K).ToList();

                    if (q3.Count == 0)
                    {
                        q3 = (from x in q
                              join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                              where temp.Month == ((DateTime)x.tanggal).Month
                              && temp.Year == ((DateTime)x.tanggal).Year
                              && eventT.workDay == y.workDay
                              select x.BN50K == null? 0 : (Int64)x.BN50K
                              ).ToList();
                    }
                    query.AddRange(q3);
                }
                result.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)Math.Round(query.Any() ? query.Average(x => x) : 0, 0) });
                tanggal = tanggal.AddDays(1);
            }
            return result;
        }
        List<tanggalValue> loadPrediksiOutCabang100SP()
        {
            List<tanggalValue> result = new List<tanggalValue>();
            List<DateTime> listTanggalHistorisUntukPrediksi = loadTanggalHistorisUntukPrediksi();

            //Load semua in Cabang 100
            var q = (from x in db.StokPosisis
                     where x.denom == "100000"
                     select new { x.tanggal, BN100K = x.outCabang});
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
                                      select new { tanggal = (DateTime)x.tanggal, BN100K = x.BN100K == null? 0:(Int64)x.BN100K }
                              ).AsEnumerable().Where(x => x.tanggal.DayOfWeek == tanggal.DayOfWeek).Select(x => x.BN100K).ToList();

                    if (q3.Count == 0)
                    {
                        q3 = (from x in q
                              join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                              where temp.Month == ((DateTime)x.tanggal).Month
                              && temp.Year == ((DateTime)x.tanggal).Year
                              && eventT.workDay == y.workDay
                              select x.BN100K == null ? 0 : (Int64)x.BN100K
                              ).ToList();
                    }
                    query.AddRange(q3);
                }
                result.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)Math.Round(query.Any() ? query.Average(x => x) : 0, 0) });
                tanggal = tanggal.AddDays(1);
            }
            return result;
        }
        List<tanggalValue> loadPrediksiOutCabang50SP()
        {
            List<tanggalValue> result = new List<tanggalValue>();
            List<DateTime> listTanggalHistorisUntukPrediksi = loadTanggalHistorisUntukPrediksi();

            //Load semua in Cabang 50
            var q = (from x in db.StokPosisis
                     where x.denom == "50000"
                     select new { x.tanggal, BN50K = x.outCabang});
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
                                      select new { tanggal = (DateTime)x.tanggal, BN50K = x.BN50K == null? 0:(Int64)x.BN50K }
                              ).AsEnumerable().Where(x => x.tanggal.DayOfWeek == tanggal.DayOfWeek).Select(x => x.BN50K).ToList();

                    if (q3.Count == 0)
                    {
                        q3 = (from x in q
                              join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                              where temp.Month == ((DateTime)x.tanggal).Month
                              && temp.Year == ((DateTime)x.tanggal).Year
                              && eventT.workDay == y.workDay
                              select x.BN50K == null ? 0 : (Int64)x.BN50K
                              ).ToList();
                    }
                    query.AddRange(q3);
                }
                result.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)Math.Round(query.Any() ? query.Average(x => x) : 0, 0) });
                tanggal = tanggal.AddDays(1);
            }
            return result;
        }


        List<tanggalValue> loadMorningBalance100SP()
        {
            List<tanggalValue> listMorningBalance = new List<tanggalValue>();
            List<tanggalValue> listEndingBalance = new List<tanggalValue>();
            DateTime today = DateTime.Today.Date;
            DateTime maxTanggal = tanggalMaxPrediksiPicker.Value.Date;
            var query = (from x in db.StokPosisis
                         select x).ToList();
            var q2 = (from x in query
                      where x.tanggal == today
                      select x).ToList();

            //Morning Balance Hari Pertama
            listMorningBalance.Add(new tanggalValue() {
                tanggal = today,
                value = (Int64) q2.Sum(x=>(x.fitBaru + x.fitLama + x.fitNKRI + x.newBaru + x.newLama + x.RRMBaru + x.RRMLama + x.RRMNKRI + x.RupiahRusakMayor + x.unfitBaru + x.unfitLama + x.unfitNKRI + x.unprocessed + x.passThrough))
              });

            //Hitung Ending Balance + Morning Balance Selanjutnya
            DateTime temp = today;
            while(temp<=maxTanggal)
            {
                Int64 morningBal = listMorningBalance.Where(x => x.tanggal == temp).Select(x => x.value).First();
                Int64 totalIn = (from x in prediksiInRetail100
                                 join y in prediksiInCabang100 on x.tanggal equals y.tanggal
                                 where x.tanggal == temp
                                 select x.value+y.value).First();
                Int64 adhoc100 = (Int64) adhoc100Num.Value;
                Int64 endBal;
                if (temp == today)
                    endBal = morningBal + totalIn - adhoc100;
                else
                    endBal = morningBal + totalIn;

                listEndingBalance.Add(new tanggalValue() {
                    tanggal = temp,
                    value = endBal
                });

                if (temp == maxTanggal)
                    break;
                else
                {
                    DateTime temph1 = temp.AddDays(1);
                    Int64 totalOut = (from x in prediksiOutAtm100
                                      join y in prediksiOutAtm50 on x.tanggal equals y.tanggal
                                      select x.value + y.value).First();
                    Int64 morningBalh1 = endBal - totalOut;
                    listMorningBalance.Add(new tanggalValue() { tanggal = temph1, value = morningBalh1});
                }
                temp = temp.AddDays(1);
            }
            return listMorningBalance;
        }
        List<tanggalValue> loadMorningBalance50SP()
        {
            List<tanggalValue> listMorningBalance = new List<tanggalValue>();
            List<tanggalValue> listEndingBalance = new List<tanggalValue>();
            DateTime today = DateTime.Today.Date;
            DateTime maxTanggal = tanggalMaxPrediksiPicker.Value.Date;
            var query = (from x in db.StokPosisis
                         select x).ToList();
            var q2 = (from x in query
                      where x.tanggal == today
                      select x).ToList();

            //Morning Balance Hari Pertama
            listMorningBalance.Add(new tanggalValue()
            {
                tanggal = today,
                value = (Int64)q2.Sum(x => (x.fitBaru + x.fitLama + x.fitNKRI + x.newBaru + x.newLama + x.RRMBaru + x.RRMLama + x.RRMNKRI + x.RupiahRusakMayor + x.unfitBaru + x.unfitLama + x.unfitNKRI + x.unprocessed + x.passThrough))
            });

            //Hitung Ending Balance + Morning Balance Selanjutnya
            DateTime temp = today;
            while (temp <= maxTanggal)
            {
                Int64 morningBal = listMorningBalance.Where(x => x.tanggal == temp).Select(x => x.value).First();
                Int64 totalIn = (from x in prediksiInRetail50
                                 join y in prediksiInCabang50 on x.tanggal equals y.tanggal
                                 where x.tanggal == temp
                                 select x.value + y.value).First();
                Int64 adhoc50 = (Int64)adhoc50Num.Value;
                Int64 endBal;
                if (temp == today)
                    endBal = morningBal + totalIn - adhoc50;
                else
                    endBal = morningBal + totalIn;

                listEndingBalance.Add(new tanggalValue()
                {
                    tanggal = temp,
                    value = endBal
                });

                if (temp == maxTanggal)
                    break;
                else
                {
                    DateTime temph1 = temp.AddDays(1);
                    Int64 totalOut = (from x in prediksiOutAtm50
                                      join y in prediksiOutAtm50 on x.tanggal equals y.tanggal
                                      select x.value + y.value).First();
                    Int64 morningBalh1 = endBal - totalOut;
                    listMorningBalance.Add(new tanggalValue() { tanggal = temph1, value = morningBalh1 });
                }
                temp = temp.AddDays(1);
            }
            return listMorningBalance;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            loadForm.ShowSplashScreen();
            List<DateTime> listTanggal = loadTanggalHistorisUntukPrediksi();

            prediksiInCabang100 = loadPrediksiInCabang100SP();
            prediksiInCabang50 = loadPrediksiInCabang50SP();
            prediksiInRetail100 = loadPrediksiInRetail100SP();
            prediksiInRetail50 = loadPrediksiInRetail50SP();
            prediksiOutCabang100 = loadPrediksiOutCabang100SP();
            prediksiOutCabang50 = loadPrediksiOutCabang50SP();
            prediksiOutAtm100 = loadPrediksiOutAtm100();
            prediksiOutAtm50 = loadPrediksiOutAtm50();
            morningBalance100 = loadMorningBalance100SP();
            morningBalance50 = loadMorningBalance50SP();
            list = new List<ForecastDetail>();
            DateTime tgl = DateTime.Today.Date;
            DateTime maxTanggal = tanggalMaxPrediksiPicker.Value.Date;
            //while (tgl<= maxTanggal)
            //{
            //    list.Add(new ForecastDetail() { tanggal = tgl,
            //        inCabang100 = prediksiInCabang100.Where(x => x.tanggal == tgl).Select(x => x.value).FirstOrDefault(),
            //        inRetail100 = prediksiInRetail100.Where(x => x.tanggal == tgl).Select(x => x.value).FirstOrDefault(),
            //        inCabang50 = prediksiInCabang50.Where(x => x.tanggal == tgl).Select(x => x.value).FirstOrDefault(),
            //        inRetail50 = prediksiInRetail50.Where(x => x.tanggal == tgl).Select(x => x.value).FirstOrDefault(),
            //        outCabang100 = prediksiOutCabang100.Where(x => x.tanggal == tgl).Select(x => x.value).FirstOrDefault(),
            //        outCabang50 = prediksiOutCabang50.Where(x => x.tanggal == tgl).Select(x => x.value).FirstOrDefault(),
            //        outATM100 = prediksiOutAtm100.Where(x=>x.tanggal == tgl).Select(x=>x.value).FirstOrDefault(),
            //        outATM50 = prediksiOutAtm50.Where(x=>x.tanggal == tgl).Select(x=>x.value).FirstOrDefault()
            //    });
            //    tgl = tgl.AddDays(1);
            //}

            Double fit100 = (Double)fit100Num.Value / 100, fit50 = (Double)fit50Num.Value / 100;

            var qView = (from a in prediksiInCabang100
                         join b in prediksiInCabang50 on a.tanggal equals b.tanggal
                         join c in prediksiInRetail100 on a.tanggal equals c.tanggal
                         join d in prediksiInRetail50 on a.tanggal equals d.tanggal
                         join f in prediksiOutCabang100 on a.tanggal equals f.tanggal
                         join g in prediksiOutCabang50 on a.tanggal equals g.tanggal
                         join h in prediksiOutAtm100 on a.tanggal equals h.tanggal
                         join i in prediksiOutAtm50 on a.tanggal equals i.tanggal
                         join j in morningBalance100 on a.tanggal equals j.tanggal
                         join k in morningBalance50 on a.tanggal equals k.tanggal
                         select new {
                             Tanggal = a.tanggal,
                             InCabang100 = a.value * fit100,
                             InCabang50 = b.value * fit50,
                             InRetail100 = c.value * fit100,
                             InRetail50 = d.value * fit50,
                             OutCabang100 = f.value * fit100,
                             OutCabang50 = g.value * fit50,
                             OutATM100 = h.value * fit100,
                             OutATM50 = i.value * fit50,
                             MorningBalance100 = j.value,
                             MorningBalance50 = k.value
                         }).ToList();
            dataGridView1.DataSource = qView;
            for(int a = 1; a < dataGridView1.Columns.Count; a++)
            {
                dataGridView1.Columns[a].DefaultCellStyle.Format = "C0";
                dataGridView1.Columns[a].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
            }
            //for(int a=3;a<dataGridView1.Columns.Count;a++)
            //{
            //    dataGridView1.Columns[a].DefaultCellStyle.Format = "C";
            //    dataGridView1.Columns[a].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
            //}
            loadForm.CloseForm();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            db.ForecastHeaders.Add(new ForecastHeader() { tanggal = DateTime.Today.Date });
            db.SaveChanges();
            int idForecast = (from x in db.ForecastHeaders
                              orderby x.idForecast descending
                              select x.idForecast).FirstOrDefault();
            foreach(var temp in list)
            {
                temp.idForecast = idForecast;
            }
            db.ForecastDetails.AddRange(list);
            db.SaveChanges();
        }
    }
    
}
