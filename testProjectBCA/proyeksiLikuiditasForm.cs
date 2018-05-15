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
                     join y in db.Pkts on x.kodePkt equals y.kodePkt
                     where y.kanwil.ToUpper().Contains("JABO")
                     select x).ToList();

            var et = (from x in db.EventTanggals select x).ToList();


            DateTime tanggal = DateTime.Today;
            DateTime maxTanggal = tanggalMaxPrediksiPicker.Value;

            if (!q.Where(x => x.tanggal == tanggal).Any())
                MessageBox.Show("Data Laporan PKT Hari ini belum ada");
            saldo100.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)q.Where(x => x.tanggal == tanggal).Select(x => x.saldoAwal100).Sum() });
            Console.WriteLine(saldo100.Count);
            while (tanggal <= maxTanggal.AddDays(1))
            {

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
                              select new
                              {
                                  tanggal = g.Key,
                                  isiATM100 = g.Sum(x => x.isiATM100),
                                  isiCRM100 = g.Sum(x => x.isiCRM100),
                                  sislokATM100 = g.Sum(x => x.sislokATM100),
                                  sislokCRM100 = g.Sum(x => x.sislokCRM100),
                                  sislokCDM100 = g.Sum(x => x.sislokCDM100),
                                  RasioSislokAtm100 = g.Average(x => (Double)x.sislokATM100 / (Double)(x.isiATM100 == 0 ? 1 : x.isiATM100))
                              });

                isiAtm100.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)Math.Round((Double)query2.Average(x => x.isiATM100), 0) });
                isiCrm100.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)Math.Round((Double)query2.Average(x => x.isiATM100), 0) });
                sislokAtm100.Add(new tanggalRasio() { tanggal = tanggal, value = Math.Round((Double)query2.Average(x => x.RasioSislokAtm100), 2) });
                sislokCrm100.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)Math.Round((Double)query2.Average(x => x.sislokCRM100), 0) });
                sislokCdm100.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)Math.Round((Double)query2.Average(x => x.sislokCDM100), 0) });

                tanggal = tanggal.AddDays(1);
            }

           


            DateTime today = DateTime.Today.Date;
            var bon = (from x in db.laporanBons
                       join y in db.Pkts on x.kodePkt equals y.kodePkt
                       where x.tanggal >= today
                       where y.kanwil.ToUpper().Contains("JABO")
                       group x by tanggal into g
                       select new { g.Key, C100 = g.Sum(x => x.C100) }
                       ).ToList();

            var listApproval = (from x in db.Approvals
                                join y in db.DetailApprovals on x.idApproval equals y.idApproval
                                join z in db.Pkts on x.kodePkt equals z.kodePkt
                                where z.kanwil.ToUpper().Contains("JABO")
                                group y by y.tanggal into g
                                select new { g.Key, setor100 = g.Sum(x => x.setor100), bon100 = g.Sum(x => x.bon100) }).ToList();

            //Udah ada saldo awal dan saldo ideal untuk h+2

            //hitung saldo tiap hari dari laporan bon dan prediksi
            for (int a = 0; a < bon.Count; a++)
            {
                saldo100.Add(new tanggalValue()
                {
                    tanggal = today.AddDays(a + 1),
                    value =
                      saldo100.Where(x => x.tanggal == today.AddDays(a)).Select(x => x.value).First()
                      + (Int64)Math.Round(sislokAtm100.Where(x => x.tanggal == today.AddDays(a)).Select(x => x.value).First() * isiAtm100.Where(x => x.tanggal == today.AddDays(a)).Select(x => x.value).First(), 0)
                      + sislokCdm100.Where(x => x.tanggal == today.AddDays(a)).Select(x => x.value).First()
                      + sislokCrm100.Where(x => x.tanggal == today.AddDays(a)).Select(x => x.value).First()
                      - isiAtm100.Where(x => x.tanggal == today.AddDays(a)).Select(x => x.value).First()
                      - isiCrm100.Where(x => x.tanggal == today.AddDays(a)).Select(x => x.value).First()
                      + (Int64)bon[a].C100
                      - (listApproval.Where(x => x.Key == tanggal).Select(x => x.setor100).Any() ? (Int64)listApproval.Where(x => x.Key == today.AddDays(a)).Select(x => x.setor100).ToList()[listApproval.Where(x => x.Key == today.AddDays(a)).Select(x => x.setor100).ToList().Count - 1] : 0)
                });
                if (a > 0)
                    result.Add(new tanggalValue() { tanggal = bon[a].Key, value = (Int64)bon[a].C100 });
            }

            //Cek bon di approval
            if (listApproval.Max(x => x.Key) > saldo100.Max(x => x.tanggal))
            {
                DateTime tTanggal = saldo100.Max(x => x.tanggal);
                var tempLA = listApproval.Where(x => x.Key == tTanggal).FirstOrDefault();
                int idx = listApproval.IndexOf(tempLA);

                for (int a = idx; a < listApproval.Count; a++)
                {
                    saldo100.Add(new tanggalValue()
                    {
                        tanggal = tTanggal.AddDays(a + 1),
                        value =
                          saldo100.Where(x => x.tanggal == tTanggal.AddDays(a)).Select(x => x.value).First()
                          + (Int64)Math.Round(sislokAtm100.Where(x => x.tanggal == tTanggal.AddDays(a)).Select(x => x.value).First() * isiAtm100.Where(x => x.tanggal == tTanggal.AddDays(a)).Select(x => x.value).First(), 0)
                          + sislokCdm100.Where(x => x.tanggal == tTanggal.AddDays(a)).Select(x => x.value).First()
                          + sislokCrm100.Where(x => x.tanggal == tTanggal.AddDays(a)).Select(x => x.value).First()
                          - isiAtm100.Where(x => x.tanggal == tTanggal.AddDays(a)).Select(x => x.value).First()
                          - isiCrm100.Where(x => x.tanggal == tTanggal.AddDays(a)).Select(x => x.value).First()
                          + (Int64)bon.Where(x => x.Key == tTanggal.AddDays(a)).Select(x => x.C100).First()
                          - (listApproval.Where(x => x.Key == tanggal).Select(x => x.setor100).Any() ? (Int64)listApproval.Where(x => x.Key == tTanggal.AddDays(a)).Select(x => x.setor100).ToList()[listApproval.Where(x => x.Key == tTanggal.AddDays(a)).Select(x => x.setor100).ToList().Count - 1] : 0)
                    });
                }
            }

            for (DateTime a = saldo100.Select(x => x.tanggal).Max().AddDays(1); a <= maxTanggal.AddDays(1); a = a.AddDays(1))
            {
                saldo100.Add(new tanggalValue() { tanggal = a, value = (Int64)Math.Round(targetRasio100 * isiAtm100.Where(x => x.tanggal == a).Select(x => x.value).First(), 0) });
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
                    Int64 tsetor = (Int64)(listApproval.Where(x => x.Key == a).Select(x => x.setor100).FirstOrDefault() == null ? listApproval.Where(x => x.Key == a).Select(x => x.setor100).First() : 0);

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

            saldo50.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)q.Where(x => x.tanggal == tanggal).Select(x => x.saldoAwal50).Sum() });
            Console.WriteLine(saldo50.Count);
            while (tanggal <= maxTanggal.AddDays(1))
            {

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
                isiCrm50.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)Math.Round((Double)query2.Average(x => x.isiATM50), 0) });
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
            var bon = (from x in db.laporanBons
                       join y in db.Pkts on x.kodePkt equals y.kodePkt
                       where x.tanggal >= today
                       where y.kanwil.ToUpper().Contains("JABO")
                       group x by tanggal into g
                       select new { g.Key, C50 = g.Sum(x => x.C50) }
                       ).ToList();

            var listApproval = (from x in db.Approvals
                                join y in db.DetailApprovals on x.idApproval equals y.idApproval
                                join z in db.Pkts on x.kodePkt equals z.kodePkt
                                where z.kanwil.ToUpper().Contains("JABO")
                                group y by y.tanggal into g
                                select new { g.Key, setor50 = g.Sum(x => x.setor50), bon50 = g.Sum(x => x.bon50) }).ToList();

            //Udah ada saldo awal dan saldo ideal untuk h+2

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

            //Cek bon di approval
            if (listApproval.Max(x => x.Key) > saldo50.Max(x => x.tanggal))
            {
                DateTime tTanggal = saldo50.Max(x => x.tanggal);
                var tempLA = listApproval.Where(x => x.Key == tTanggal).FirstOrDefault();
                int idx = listApproval.IndexOf(tempLA);

                for (int a = idx; a < listApproval.Count; a++)
                {
                    saldo50.Add(new tanggalValue()
                    {
                        tanggal = tTanggal.AddDays(a + 1),
                        value =
                          saldo50.Where(x => x.tanggal == tTanggal.AddDays(a)).Select(x => x.value).First()
                          + (Int64)Math.Round(sislokAtm50.Where(x => x.tanggal == tTanggal.AddDays(a)).Select(x => x.value).First() * isiAtm50.Where(x => x.tanggal == tTanggal.AddDays(a)).Select(x => x.value).First(), 0)
                          + sislokCdm50.Where(x => x.tanggal == tTanggal.AddDays(a)).Select(x => x.value).First()
                          + sislokCrm50.Where(x => x.tanggal == tTanggal.AddDays(a)).Select(x => x.value).First()
                          - isiAtm50.Where(x => x.tanggal == tTanggal.AddDays(a)).Select(x => x.value).First()
                          - isiCrm50.Where(x => x.tanggal == tTanggal.AddDays(a)).Select(x => x.value).First()
                          + (Int64)bon.Where(x => x.Key == tTanggal.AddDays(a)).Select(x => x.C50).First()
                          - (listApproval.Where(x => x.Key == tanggal).Select(x => x.setor50).Any() ? (Int64)listApproval.Where(x => x.Key == tTanggal.AddDays(a)).Select(x => x.setor50).ToList()[listApproval.Where(x => x.Key == tTanggal.AddDays(a)).Select(x => x.setor50).ToList().Count - 1] : 0)
                    });
                }
            }

            for (DateTime a = saldo50.Select(x => x.tanggal).Max().AddDays(1); a <= maxTanggal.AddDays(1); a = a.AddDays(1))
            {
                saldo50.Add(new tanggalValue() { tanggal = a, value = (Int64)Math.Round(targetRasio50 * isiAtm50.Where(x => x.tanggal == a).Select(x => x.value).First(), 0) });
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
                    Int64 tsetor = (Int64)(listApproval.Where(x => x.Key == a).Select(x => x.setor50).FirstOrDefault() == null ? listApproval.Where(x => x.Key == a).Select(x => x.setor50).First() : 0);

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
            List<tanggalValue> prediksiOutAtm50 = loadPrediksiOutAtm50();

            list = new List<ForecastDetail>();
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
                    outCabang50 = prediksiOutCabang50.Where(x => x.tanggal == tgl).Select(x => x.value).FirstOrDefault(),
                    outATM100 = prediksiOutAtm100.Where(x=>x.tanggal == tgl).Select(x=>x.value).FirstOrDefault(),
                    outATM50 = prediksiOutAtm50.Where(x=>x.tanggal == tgl).Select(x=>x.value).FirstOrDefault()
                });
                tgl = tgl.AddDays(1);
            }
            
            dataGridView1.DataSource = list;
            for(int a=2;a<dataGridView1.Columns.Count-1;a++)
            {
                dataGridView1.Columns[a].DefaultCellStyle.Format = "C";
                dataGridView1.Columns[a].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
            }
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
