﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.SqlServer;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using testProjectBCA.ATM;

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
        List<tanggalEvent> listTanggalEvent; //Diload saat load out atm 100
        List<StoreClass> morningBalance100HariH, 
            morningBalance50HariH,
            inCabangUntukKirim100,
            inCabangUntukKirim50,
            inRetailUntukKirim100,
            inRetailUntukKirim50,
            outCabangUntukKirim100,
            outCabangUntukKirim50,
            outAtmUntukKirim100,
            outAtmUntukKirim50,
            newNote100,
            newNote50;
        List<ApprovalPembagianSaldo> listAdhocCabang;
        List<ApprovalPembagianSaldo> listDeliveryCabang;
        List<DataPermintaanDanSumber> listBITUKABPermintaanUntukKirim, listBITUKABSumberUntukKirim;
        public proyeksiLikuiditasForm()
        {
            InitializeComponent();
            loadBulanPrediksiTreeView();
            metodePrediksiComboBox.DataSource = Variables.listMetodeNonATM;
            metodePrediksiComboBox.SelectedIndex = 0;
            tanggalMaxPrediksiPicker.MinDate = DateTime.Today.AddDays(1);
            outBITukabDateTimePicker.MinDate = Variables.todayDate.AddDays(1);
            inBITukabDateTimePicker.MinDate = Variables.todayDate.AddDays(1);
            var q = (from x in db.Approvals
                     join y in db.DetailApprovals on x.idApproval equals y.idApproval
                     join z in db.Pkts on x.kodePkt equals z.kodePkt
                     where (DateTime)x.tanggal == Variables.todayDate
                     && (DateTime)y.tanggal == Variables.todayDate
                     && z.kanwil.ToUpper().Contains("JABO")
                     select y).ToList();
            if(q.Any())
            {
                Int64 adhoc100 = q.Sum(x => (Int64) x.adhoc100),
                    adhoc50 = q.Sum(x=>(Int64) x.adhoc50);
                adhocATM100Num.Value = adhoc100;
                adhocATM50Num.Value = adhoc50;
            }
            var maxApprovalDate = db.DetailApprovals.Where(x => x.bon100 != -1).Max(x => x.tanggal);
            tanggalMaxPrediksiPicker.Value = (DateTime)maxApprovalDate;
            adhocATM100Num.ThousandsSeparator = true;
            adhocATM50Num.ThousandsSeparator = true;
            adhocATM100Num.Enabled = false;
            adhocATM50Num.Enabled = false;
            adhocCabang100Num.Enabled = false;
            adhocCabang50Num.Enabled = false;
            listAdhocCabang = new List<ApprovalPembagianSaldo>();
            listDeliveryCabang = new List<ApprovalPembagianSaldo>();
            List<String> listNamaPkt = new List<String>();
            listNamaPkt.Add("ALL VENDOR(JABO)");
            listNamaPkt.Add("DETAIL PER VENDOR");
            groupingStokMorningBalanceComboBox.DataSource = listNamaPkt;
            
            //label8.Visible = false;
            //label9.Visible = false;
        }

        void loadBulanPrediksiTreeView()
        {

            DateTime minTanggal;
            DateTime maxTanggal;

            var q = (from x in db.DailyStocks select (DateTime)x.tanggal).ToList();
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
            inCabangUntukKirim100 = new List<StoreClass>();
            //Load semua in Cabang 100
            var q = (from x in db.DailyStocks.AsEnumerable()
                     join y in db.Cabangs.AsEnumerable() on x.kode.TrimStart('0') equals y.kodeCabang.TrimStart('0')
                     join z in db.Pkts on !String.IsNullOrEmpty(y.kodePkt) ? y.kodePkt.Substring(0, 4) : y.kodePkt equals z.kodePktCabang.Length >= 4 ? z.kodePktCabang.Substring(0, 4) : z.kodePktCabang
                     where x.in_out.ToUpper() == "IN"
                     && x.jenisTransaksi.Contains("Collection Cabang - Full - Process")
                     && z.kodePktCabang != "CCASB"
                     select new { x.tanggal, x.BN100K, kodePkt = z.namaPkt });
            List<EventTanggal> et = (from x in db.EventTanggals.AsEnumerable().AsEnumerable()
                                     select x).ToList();


            DateTime tanggal = DateTime.Today;
            DateTime maxTanggal = tanggalMaxPrediksiPicker.Value;

            while (tanggal <= maxTanggal)
            {
                List<StoreClass> query = new List<StoreClass>();
                var eventT = (from x in et
                              where x.tanggal == tanggal
                              select new { x.workDay, x.@event }).FirstOrDefault();

                foreach (var temp in listTanggalHistorisUntukPrediksi)
                {
                    List<StoreClass> q3 = (from x in q
                                      join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                      where temp.Month == ((DateTime)x.tanggal).Month
                                      && temp.Year == ((DateTime)x.tanggal).Year
                                      && eventT.workDay == y.workDay
                                      && eventT.@event == y.@event
                                      select new StoreClass(){ tanggal = (DateTime)x.tanggal, val = x.BN100K == null ? 0 : (Int64)x.BN100K, kodePkt = x.kodePkt }
                              ).AsEnumerable().Where(x => x.tanggal.DayOfWeek == tanggal.DayOfWeek).Select(x => new StoreClass(){ val = x.val, tanggal = x.tanggal, kodePkt = x.kodePkt }).ToList();
                    query.AddRange(q3);
                }
                if (!query.Any())
                {
                    foreach (var temp in listTanggalHistorisUntukPrediksi)
                    {
                        List<StoreClass> q3 = (from x in q
                                               join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                               where temp.Month == ((DateTime)x.tanggal).Month
                                               && temp.Year == ((DateTime)x.tanggal).Year
                                               && eventT.workDay == y.workDay
                                               select new StoreClass() { tanggal = (DateTime)x.tanggal, val = x.BN100K == null ? 0 : (Int64)x.BN100K, kodePkt = x.kodePkt }
                                  ).ToList();
                        query.AddRange(q3);
                    }
                }
                if (!query.Any())
                {
                    foreach (var temp in listTanggalHistorisUntukPrediksi)
                    {
                        List<StoreClass> q3 = (from x in q
                                               join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                               where temp.Month == ((DateTime)x.tanggal).Month
                                               && temp.Year == ((DateTime)x.tanggal).Year
                                               select new StoreClass() { tanggal = (DateTime)x.tanggal, val = x.BN100K == null ? 0 : (Int64)x.BN100K, kodePkt = x.kodePkt }
                                  ).ToList();
                        query.AddRange(q3);
                    }
                }

                var query2 = (from x in query
                              group x by x.tanggal into g
                              select new { g.Key, val = g.Sum(x => x.val) }).ToList();

                Console.WriteLine("QUERY 2(100) COUNT: " + query2.Count);
                result.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)Math.Round(query.Any() ? query2.Average(x => x.val) : 0, 0) });


                var query3 = (from x in query
                              group x by new { x.tanggal, x.kodePkt } into g
                              select new { g.Key, val = g.Sum(x => x.val) }).ToList();
                inCabangUntukKirim100.AddRange(
                    (from x in query3
                     group x by x.Key.kodePkt into g
                     select new StoreClass()
                     {
                         kodePkt = g.Key,
                         tanggal = tanggal,
                         val = (Int64) g.Average(x=>x.val)
                     }).ToList());
                tanggal = tanggal.AddDays(1);
            }
            return result;
        }
        List<tanggalValue> loadPrediksiInCabang50()
        {
            List<tanggalValue> result = new List<tanggalValue>();
            List<DateTime> listTanggalHistorisUntukPrediksi = loadTanggalHistorisUntukPrediksi();
            inCabangUntukKirim50 = new List<StoreClass>();
            //Load semua in Cabang 50
            var q = (from x in db.DailyStocks.AsEnumerable()
                     join y in db.Cabangs.AsEnumerable() on x.kode.TrimStart('0') equals y.kodeCabang.TrimStart('0')
                     join z in db.Pkts on !String.IsNullOrEmpty(y.kodePkt) ? y.kodePkt.Substring(0, 4) : y.kodePkt equals z.kodePktCabang.Length >= 4 ? z.kodePktCabang.Substring(0, 4) : z.kodePktCabang
                     where x.in_out.ToUpper() == "IN"
                     && x.jenisTransaksi.Contains("Collection Cabang - Full - Process")
                     && z.kodePktCabang != "CCASB"
                     select new { x.tanggal, x.BN50K, kodePkt = z.namaPkt });
            List<EventTanggal> et = (from x in db.EventTanggals.AsEnumerable().AsEnumerable()
                                     select x).ToList();


            DateTime tanggal = DateTime.Today;
            DateTime maxTanggal = tanggalMaxPrediksiPicker.Value;

            while (tanggal <= maxTanggal)
            {
                List<StoreClass> query = new List<StoreClass>();
                var eventT = (from x in et
                              where x.tanggal == tanggal
                              select new { x.workDay, x.@event }).FirstOrDefault();


                foreach (var temp in listTanggalHistorisUntukPrediksi)
                {
                    List<StoreClass> q3 = (from x in q
                                      join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                      where temp.Month == ((DateTime)x.tanggal).Month
                                      && temp.Year == ((DateTime)x.tanggal).Year
                                      && eventT.workDay == y.workDay
                                      && eventT.@event == y.@event
                                      select new StoreClass(){ tanggal = (DateTime)x.tanggal, val = x.BN50K == null ? 0 : (Int64)x.BN50K, kodePkt = x.kodePkt}
                              ).AsEnumerable().Where(x => x.tanggal.DayOfWeek == tanggal.DayOfWeek).Select(x =>new StoreClass() { kodePkt = x.kodePkt, val = x.val, tanggal = x.tanggal}).ToList();
                    query.AddRange(q3);
                }
                if (!query.Any())
                {
                    foreach (var temp in listTanggalHistorisUntukPrediksi)
                    {
                        List<StoreClass> q3 = (from x in q
                                               join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                               where temp.Month == ((DateTime)x.tanggal).Month
                                               && temp.Year == ((DateTime)x.tanggal).Year
                                               && eventT.workDay == y.workDay
                                               select new StoreClass() { kodePkt = x.kodePkt, tanggal = (DateTime)x.tanggal, val = (Int64)x.BN50K }
                                  ).ToList();
                        query.AddRange(q3);
                    }
                }
                if (!query.Any())
                {
                    foreach (var temp in listTanggalHistorisUntukPrediksi)
                    {
                        List<StoreClass> q3 = (from x in q
                                               join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                               where temp.Month == ((DateTime)x.tanggal).Month
                                               && temp.Year == ((DateTime)x.tanggal).Year
                                               select new StoreClass() { kodePkt = x.kodePkt, tanggal = (DateTime)x.tanggal, val = (Int64)x.BN50K }
                                  ).ToList();
                        query.AddRange(q3);
                    }
                }
                if (!query.Any())
                {
                    foreach (var temp in listTanggalHistorisUntukPrediksi)
                    {
                        List<StoreClass> q3 = (from x in q
                                               join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                               select new StoreClass() { kodePkt = x.kodePkt, tanggal = (DateTime)x.tanggal, val = 0 }
                                  ).ToList();
                        query.AddRange(q3);
                    }
                }

                var query2 = (from x in query 
                              group x by x.tanggal into g
                              select new { g.Key, val = g.Sum(x=>x.val)}).ToList();

                Console.WriteLine("QUERY 2(50) COUNT: " + query2.Count);
                result.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)Math.Round(query.Any() ? query2.Average(x => x.val) : 0, 0) });

                var query3 = (from x in query
                              group x by new { x.tanggal, x.kodePkt } into g
                              select new { g.Key, val = g.Sum(x => x.val) }).ToList();
                inCabangUntukKirim50.AddRange(
                    (from x in query3
                     group x by x.Key.kodePkt into g
                     select new StoreClass()
                     {
                         kodePkt = g.Key,
                         tanggal = tanggal,
                         val = (Int64)g.Average(x => x.val)
                     }).ToList());

                tanggal = tanggal.AddDays(1);
            }
            return result;
        }
        List<tanggalValue> loadPrediksiInRetail100()
        {
            List<tanggalValue> result = new List<tanggalValue>();
            List<DateTime> listTanggalHistorisUntukPrediksi = loadTanggalHistorisUntukPrediksi();
            inRetailUntukKirim100 = new List<StoreClass>();
            //Load semua in Cabang 100
            var q = (from x in db.DailyStocks
                     join z in db.Pkts on x.kodePkt equals z.kodePktCabang
                     where x.in_out.ToUpper() == "IN"
                     && x.jenisTransaksi.Contains("Collection Retail")
                     select new { x.tanggal, x.BN100K, kodePkt = z.namaPkt });
            List<EventTanggal> et = (from x in db.EventTanggals.AsEnumerable().AsEnumerable()
                                     select x).ToList();


            DateTime tanggal = DateTime.Today;
            DateTime maxTanggal = tanggalMaxPrediksiPicker.Value;

            while (tanggal <= maxTanggal)
            {
                List<StoreClass> query = new List<StoreClass>();
                var eventT = (from x in et
                              where x.tanggal == tanggal
                              select new { x.workDay, x.@event }).FirstOrDefault();


                foreach (var temp in listTanggalHistorisUntukPrediksi)
                {
                    List<StoreClass> q3 = (from x in q
                                           join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                           where temp.Month == ((DateTime)x.tanggal).Month
                                           && temp.Year == ((DateTime)x.tanggal).Year
                                           && eventT.workDay == y.workDay
                                           && eventT.@event == y.@event
                                           select new StoreClass() { tanggal = (DateTime)x.tanggal, val = x.BN100K == null ? 0 : (Int64)x.BN100K, kodePkt = x.kodePkt }
                              ).AsEnumerable().Where(x => x.tanggal.DayOfWeek == tanggal.DayOfWeek).Select(x => new StoreClass() { kodePkt = x.kodePkt, val = x.val, tanggal = x.tanggal }).ToList();
                    query.AddRange(q3);
                }
                if (!query.Any())
                {
                    foreach (var temp in listTanggalHistorisUntukPrediksi)
                    {
                        List<StoreClass> q3 = (from x in q
                                               join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                               where temp.Month == ((DateTime)x.tanggal).Month
                                               && temp.Year == ((DateTime)x.tanggal).Year
                                               && eventT.workDay == y.workDay
                                               select new StoreClass() { kodePkt = x.kodePkt, tanggal = (DateTime)x.tanggal, val = (Int64)x.BN100K }
                                  ).ToList();
                        query.AddRange(q3);
                    }
                }
                if (!query.Any())
                {
                    foreach (var temp in listTanggalHistorisUntukPrediksi)
                    {
                        List<StoreClass> q3 = (from x in q
                                               join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                               where temp.Month == ((DateTime)x.tanggal).Month
                                               && temp.Year == ((DateTime)x.tanggal).Year
                                               select new StoreClass() { kodePkt = x.kodePkt, tanggal = (DateTime)x.tanggal, val = (Int64)x.BN100K }
                                  ).ToList();
                        query.AddRange(q3);
                    }
                }
                if (!query.Any())
                {
                    foreach (var temp in listTanggalHistorisUntukPrediksi)
                    {
                        List<StoreClass> q3 = (from x in q
                                               join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                               select new StoreClass() { kodePkt = x.kodePkt, tanggal = (DateTime)x.tanggal, val = 0 }
                                  ).ToList();
                        query.AddRange(q3);
                    }
                }

                var query2 = (from x in query
                              group x by x.tanggal into g
                              select new { g.Key, val = g.Sum(x => x.val) }).ToList();

                Console.WriteLine("QUERY 2(100) COUNT: " + query2.Count);
                result.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)Math.Round(query.Any() ? query2.Average(x => x.val) : 0, 0) });

                var query3 = (from x in query
                              group x by new { x.tanggal, x.kodePkt } into g
                              select new { g.Key, val = g.Sum(x => x.val) }).ToList();
                inRetailUntukKirim100.AddRange(
                    (from x in query3
                     group x by x.Key.kodePkt into g
                     select new StoreClass()
                     {
                         kodePkt = g.Key,
                         tanggal = tanggal,
                         val = (Int64)g.Average(x => x.val)
                     }).ToList());

                tanggal = tanggal.AddDays(1);
            }
            return result;
        }
        List<tanggalValue> loadPrediksiInRetail50()
        {
            List<tanggalValue> result = new List<tanggalValue>();
            List<DateTime> listTanggalHistorisUntukPrediksi = loadTanggalHistorisUntukPrediksi();
            inRetailUntukKirim50 = new List<StoreClass>();
            //Load semua in Cabang 50
            var q = (from x in db.DailyStocks
                     join z in db.Pkts on x.kodePkt equals z.kodePktCabang
                     where x.in_out.ToUpper() == "IN"
                     && x.jenisTransaksi.Contains("Collection Retail")
                     select new { x.tanggal, x.BN50K, kodePkt = z.namaPkt });
            List<EventTanggal> et = (from x in db.EventTanggals.AsEnumerable().AsEnumerable()
                                     select x).ToList();


            DateTime tanggal = DateTime.Today;
            DateTime maxTanggal = tanggalMaxPrediksiPicker.Value;

            while (tanggal <= maxTanggal)
            {
                List<StoreClass> query = new List<StoreClass>();
                var eventT = (from x in et
                              where x.tanggal == tanggal
                              select new { x.workDay, x.@event }).FirstOrDefault();


                foreach (var temp in listTanggalHistorisUntukPrediksi)
                {
                    List<StoreClass> q3 = (from x in q
                                           join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                           where temp.Month == ((DateTime)x.tanggal).Month
                                           && temp.Year == ((DateTime)x.tanggal).Year
                                           && eventT.workDay == y.workDay
                                           && eventT.@event == y.@event
                                           select new StoreClass() { tanggal = (DateTime)x.tanggal, val = x.BN50K == null ? 0 : (Int64)x.BN50K, kodePkt = x.kodePkt }
                              ).AsEnumerable().Where(x => x.tanggal.DayOfWeek == tanggal.DayOfWeek).Select(x => new StoreClass() { kodePkt = x.kodePkt, val = x.val, tanggal = x.tanggal }).ToList();
                    query.AddRange(q3);
                }
                if (!query.Any())
                {
                    foreach (var temp in listTanggalHistorisUntukPrediksi)
                    {
                        List<StoreClass> q3 = (from x in q
                                               join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                               where temp.Month == ((DateTime)x.tanggal).Month
                                               && temp.Year == ((DateTime)x.tanggal).Year
                                               && eventT.workDay == y.workDay
                                               select new StoreClass() { kodePkt = x.kodePkt, tanggal = (DateTime)x.tanggal, val = (Int64)x.BN50K }
                                  ).ToList();
                        query.AddRange(q3);
                    }
                }
                if (!query.Any())
                {
                    foreach (var temp in listTanggalHistorisUntukPrediksi)
                    {
                        List<StoreClass> q3 = (from x in q
                                               join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                               where temp.Month == ((DateTime)x.tanggal).Month
                                               && temp.Year == ((DateTime)x.tanggal).Year
                                               select new StoreClass() { kodePkt = x.kodePkt, tanggal = (DateTime)x.tanggal, val = (Int64)x.BN50K }
                                  ).ToList();
                        query.AddRange(q3);
                    }
                }
                if (!query.Any())
                {
                    foreach (var temp in listTanggalHistorisUntukPrediksi)
                    {
                        List<StoreClass> q3 = (from x in q
                                               join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                               select new StoreClass() { kodePkt = x.kodePkt, tanggal = (DateTime)x.tanggal, val = 0 }
                                  ).ToList();
                        query.AddRange(q3);
                    }
                }

                var query2 = (from x in query
                              group x by x.tanggal into g
                              select new { g.Key, val = g.Sum(x => x.val) }).ToList();

                Console.WriteLine("QUERY 2(50) COUNT: " + query2.Count);
                result.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)Math.Round(query.Any() ? query2.Average(x => x.val) : 0, 0) });

                var query3 = (from x in query
                              group x by new { x.tanggal, x.kodePkt } into g
                              select new { g.Key, val = g.Sum(x => x.val) }).ToList();
                inRetailUntukKirim50.AddRange(
                    (from x in query3
                     group x by x.Key.kodePkt into g
                     select new StoreClass()
                     {
                         kodePkt = g.Key,
                         tanggal = tanggal,
                         val = (Int64)g.Average(x => x.val)
                     }).ToList());

                tanggal = tanggal.AddDays(1);
            }
            return result;
        }
        List<tanggalValue> loadPrediksiOutCabang100()
        {
            List<tanggalValue> result = new List<tanggalValue>();
            List<DateTime> listTanggalHistorisUntukPrediksi = loadTanggalHistorisUntukPrediksi();
            outCabangUntukKirim100 = new List<StoreClass>();
            //Load semua in Cabang 50
            var q = (from x in db.DailyStocks.AsEnumerable()
                     join y in db.Cabangs.AsEnumerable() on x.kode.TrimStart('0') equals y.kodeCabang.TrimStart('0')
                     join z in db.Pkts on !String.IsNullOrEmpty(y.kodePkt) ? y.kodePkt.Substring(0, 4) : y.kodePkt equals z.kodePktCabang.Length >= 4 ? z.kodePktCabang.Substring(0, 4) : z.kodePktCabang
                     where x.in_out.ToUpper() == "OUT"
                     && z.kodePktCabang != "CCASB"
                     && x.jenisTransaksi.Contains("Delivery Cabang")
                     select new { x.tanggal, x.BN100K, kodePkt = z.namaPkt });
            List<EventTanggal> et = (from x in db.EventTanggals.AsEnumerable().AsEnumerable()
                                     select x).ToList();


            DateTime tanggal = DateTime.Today;
            DateTime maxTanggal = tanggalMaxPrediksiPicker.Value;
            var dariExcel = (from x in listDeliveryCabang
                             group x by x.Tanggal into g
                             select new tanggalValue()
                             {
                                 tanggal = g.Key,
                                 value = g.Sum(x => x.D100)
                             }).ToList();
            while (tanggal <= maxTanggal)
            {
                List<StoreClass> query = new List<StoreClass>();
                var eventT = (from x in et
                              where x.tanggal == tanggal
                              select new { x.workDay, x.@event }).FirstOrDefault();
                var tempDariExcel = dariExcel.Where(x => x.tanggal == tanggal).FirstOrDefault();
                if (tempDariExcel != null)
                {
                    result.Add(tempDariExcel);
                    tanggal = tanggal.AddDays(1);
                }
                else
                {
                    foreach (var temp in listTanggalHistorisUntukPrediksi)
                    {
                        List<StoreClass> q3 = (from x in q
                                               join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                               where temp.Month == ((DateTime)x.tanggal).Month
                                               && temp.Year == ((DateTime)x.tanggal).Year
                                               && eventT.workDay == y.workDay
                                               && eventT.@event == y.@event
                                               select new StoreClass() { tanggal = (DateTime)x.tanggal, val = x.BN100K == null ? 0 : (Int64)x.BN100K, kodePkt = x.kodePkt }
                                  ).AsEnumerable().Where(x => x.tanggal.DayOfWeek == tanggal.DayOfWeek).Select(x => new StoreClass() { kodePkt = x.kodePkt, val = x.val, tanggal = x.tanggal }).ToList();
                        query.AddRange(q3);
                    }
                    if (!query.Any())
                    {
                        foreach (var temp in listTanggalHistorisUntukPrediksi)
                        {
                            List<StoreClass> q3 = (from x in q
                                                   join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                                   where temp.Month == ((DateTime)x.tanggal).Month
                                                   && temp.Year == ((DateTime)x.tanggal).Year
                                                   && eventT.workDay == y.workDay
                                                   select new StoreClass() { kodePkt = x.kodePkt, tanggal = (DateTime)x.tanggal, val = (Int64)x.BN100K }
                                      ).ToList();
                            query.AddRange(q3);
                        }
                    }
                    if (!query.Any())
                    {
                        foreach (var temp in listTanggalHistorisUntukPrediksi)
                        {
                            List<StoreClass> q3 = (from x in q
                                                   join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                                   where temp.Month == ((DateTime)x.tanggal).Month
                                                   && temp.Year == ((DateTime)x.tanggal).Year
                                                   select new StoreClass() { kodePkt = x.kodePkt, tanggal = (DateTime)x.tanggal, val = (Int64)x.BN100K }
                                      ).ToList();
                            query.AddRange(q3);
                        }
                    }
                    if (!query.Any())
                    {
                        foreach (var temp in listTanggalHistorisUntukPrediksi)
                        {
                            List<StoreClass> q3 = (from x in q
                                                   join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                                   select new StoreClass() { kodePkt = x.kodePkt, tanggal = (DateTime)x.tanggal, val = 0 }
                                      ).ToList();
                            query.AddRange(q3);
                        }
                    }

                    var query2 = (from x in query
                                  group x by x.tanggal into g
                                  select new { g.Key, val = g.Sum(x => x.val) }).ToList();

                    Console.WriteLine("QUERY 2(100) COUNT: " + query2.Count);
                    result.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)Math.Round(query.Any() ? query2.Average(x => x.val) : 0, 0) });

                    var query3 = (from x in query
                                  group x by new { x.tanggal, x.kodePkt } into g
                                  select new { g.Key, val = g.Sum(x => x.val) }).ToList();
                    outCabangUntukKirim100.AddRange(
                        (from x in query3
                         group x by x.Key.kodePkt into g
                         select new StoreClass()
                         {
                             kodePkt = g.Key,
                             tanggal = tanggal,
                             val = (Int64)g.Average(x => x.val)
                         }).ToList());

                    tanggal = tanggal.AddDays(1);
                }
            }
            
            return result;
        }
        List<tanggalValue> loadPrediksiOutCabang50()
        {
            List<tanggalValue> result = new List<tanggalValue>();
            List<DateTime> listTanggalHistorisUntukPrediksi = loadTanggalHistorisUntukPrediksi();
            outCabangUntukKirim50 = new List<StoreClass>();
            //Load semua in Cabang 50
            var q = (from x in db.DailyStocks.AsEnumerable()
                     join y in db.Cabangs.AsEnumerable() on x.kode.TrimStart('0') equals y.kodeCabang.TrimStart('0')
                     join z in db.Pkts on !String.IsNullOrEmpty(y.kodePkt) ? y.kodePkt.Substring(0, 4) : y.kodePkt equals z.kodePktCabang.Length >= 4 ? z.kodePktCabang.Substring(0, 4) : z.kodePktCabang
                     where x.in_out.ToUpper() == "OUT"
                     && x.jenisTransaksi.Contains("Delivery Cabang")
                     && z.kodePktCabang != "CCASB"
                     select new { x.tanggal, x.BN50K, kodePkt = z.namaPkt });
            List<EventTanggal> et = (from x in db.EventTanggals.AsEnumerable().AsEnumerable()
                                     select x).ToList();

            var dariExcel = (from x in listDeliveryCabang
                             group x by x.Tanggal into g
                             select new tanggalValue()
                             {
                                 tanggal = g.Key,
                                 value = g.Sum(x => x.D50)
                             }).ToList();
            DateTime tanggal = DateTime.Today;
            DateTime maxTanggal = tanggalMaxPrediksiPicker.Value;

            while (tanggal <= maxTanggal)
            {
                var tempDariExcel = dariExcel.Where(x => x.tanggal == tanggal).FirstOrDefault();
                if (tempDariExcel != null)
                {
                    result.Add(tempDariExcel);
                    tanggal = tanggal.AddDays(1);
                }
                else
                {
                    List<StoreClass> query = new List<StoreClass>();
                    var eventT = (from x in et
                                  where x.tanggal == tanggal
                                  select new { x.workDay, x.@event }).FirstOrDefault();


                    foreach (var temp in listTanggalHistorisUntukPrediksi)
                    {
                        List<StoreClass> q3 = (from x in q
                                               join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                               where temp.Month == ((DateTime)x.tanggal).Month
                                               && temp.Year == ((DateTime)x.tanggal).Year
                                               && eventT.workDay == y.workDay
                                               && eventT.@event == y.@event
                                               select new StoreClass() { tanggal = (DateTime)x.tanggal, val = x.BN50K == null ? 0 : (Int64)x.BN50K, kodePkt = x.kodePkt }
                                  ).AsEnumerable().Where(x => x.tanggal.DayOfWeek == tanggal.DayOfWeek).Select(x => new StoreClass() { kodePkt = x.kodePkt, val = x.val, tanggal = x.tanggal }).ToList();
                        query.AddRange(q3);
                    }
                    if (!query.Any())
                    {
                        foreach (var temp in listTanggalHistorisUntukPrediksi)
                        {
                            List<StoreClass> q3 = (from x in q
                                                   join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                                   where temp.Month == ((DateTime)x.tanggal).Month
                                                   && temp.Year == ((DateTime)x.tanggal).Year
                                                   && eventT.workDay == y.workDay
                                                   select new StoreClass() { kodePkt = x.kodePkt, tanggal = (DateTime)x.tanggal, val = (Int64)x.BN50K }
                                      ).ToList();
                            query.AddRange(q3);
                        }
                    }
                    if (!query.Any())
                    {
                        foreach (var temp in listTanggalHistorisUntukPrediksi)
                        {
                            List<StoreClass> q3 = (from x in q
                                                   join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                                   where temp.Month == ((DateTime)x.tanggal).Month
                                                   && temp.Year == ((DateTime)x.tanggal).Year
                                                   select new StoreClass() { kodePkt = x.kodePkt, tanggal = (DateTime)x.tanggal, val = (Int64)x.BN50K }
                                      ).ToList();
                            query.AddRange(q3);
                        }
                    }
                    if (!query.Any())
                    {
                        foreach (var temp in listTanggalHistorisUntukPrediksi)
                        {
                            List<StoreClass> q3 = (from x in q
                                                   join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                                   select new StoreClass() { kodePkt = x.kodePkt, tanggal = (DateTime)x.tanggal, val = 0 }
                                      ).ToList();
                            query.AddRange(q3);
                        }
                    }

                    var query2 = (from x in query
                                  group x by x.tanggal into g
                                  select new { g.Key, val = g.Sum(x => x.val) }).ToList();

                    Console.WriteLine("QUERY 2(50) COUNT: " + query2.Count);
                    result.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)Math.Round(query.Any() ? query2.Average(x => x.val) : 0, 0) });

                    var query3 = (from x in query
                                  group x by new { x.tanggal, x.kodePkt } into g
                                  select new { g.Key, val = g.Sum(x => x.val) }).ToList();
                    outCabangUntukKirim50.AddRange(
                        (from x in query3
                         group x by x.Key.kodePkt into g
                         select new StoreClass()
                         {
                             kodePkt = g.Key,
                             tanggal = tanggal,
                             val = (Int64)g.Average(x => x.val)
                         }).ToList());

                    tanggal = tanggal.AddDays(1);
                }
            }
            return result;
        }
        List<tanggalValue> loadPrediksiOutAtm100()
        {
            listTanggalEvent = new List<tanggalEvent>();
            Console.WriteLine("Prediksi ATM");
            List<tanggalValue> result = new List<tanggalValue>();
            List<DateTime> listTanggalHistorisUntukPrediksi = loadTanggalHistorisUntukPrediksi();
            List<tanggalValue> isiAtm100 = new List<tanggalValue>(),
                isiCrm100 = new List<tanggalValue>(),
                sislokCdm100 = new List<tanggalValue>(),
                sislokCrm100 = new List<tanggalValue>(),
                saldo100 = new List<tanggalValue>();
            List<tanggalRasio> sislokAtm100 = new List<tanggalRasio>();
            String jenis = metodePrediksiComboBox.SelectedItem.ToString();
            Double targetRasio100;

            Double buf;
            if (Double.TryParse(rasio100TextBox.Text, out buf))
                targetRasio100 = buf;
            else
                targetRasio100 = 0;

            DateTime tanggal = DateTime.Today;
            DateTime maxTanggal = tanggalMaxPrediksiPicker.Value;
            KumpulanPrediksi prediksi = new KumpulanPrediksi("Jabotabek", listTanggalHistorisUntukPrediksi, tanggal, maxTanggal, jenis, jenis);

            isiAtm100.AddRange(prediksi.prediksiIsiAtm.GroupBy(x => x.tanggal).Select(x => new tanggalValue() { tanggal = x.Key, value = x.Sum(y => y.d100) }).ToList());
            isiCrm100.AddRange(prediksi.isiCrm2.GroupBy(x => x.tanggal).Select(x => new tanggalValue() { tanggal = x.Key, value = x.Sum(y => y.d100) }).ToList());
            sislokAtm100.AddRange(prediksi.rasioSislokAtm.GroupBy(x => x.tanggal).Select(x => new tanggalRasio() { tanggal = x.Key, value = x.Average(y => y.d100) }).ToList());
            sislokCdm100.AddRange(prediksi.sislokCdm.GroupBy(x => x.tanggal).Select(x => new tanggalValue() { tanggal = x.Key, value = x.Sum(y => y.d100) }).ToList());
            sislokCrm100.AddRange(prediksi.sislokCrm.GroupBy(x => x.tanggal).Select(x => new tanggalValue() { tanggal = x.Key, value = x.Sum(y => y.d100) }).ToList());


            //Load semua in Cabang 100
            var q = (from x in db.TransaksiAtms
                     join y in db.Pkts on x.kodePkt equals y.kodePkt
                     where y.kanwil.ToUpper().Contains("JABO")
                     select x).ToList();

            var et = (from x in db.EventTanggals select x).ToList();




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
                    query.AddRange(q3);

                }
                if (!query.Any())
                {
                    foreach (var temp in listTanggalHistorisUntukPrediksi)
                    {

                        List<TransaksiAtm> q3 = (from x in q
                                                 join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                                 where temp.Month == ((DateTime)x.tanggal).Month
                                                 && temp.Year == ((DateTime)x.tanggal).Year
                                                 && eventT.workDay == y.workDay
                                                 && eventT.@event == y.@event
                                                 select x
                             ).ToList();
                        query.AddRange(q3);
                    }
                    if (!query.Any())
                    {
                        foreach (var temp in listTanggalHistorisUntukPrediksi)
                        {

                            List<TransaksiAtm> q3 = (from x in q
                                                     join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                                     where temp.Month == ((DateTime)x.tanggal).Month
                                                     && temp.Year == ((DateTime)x.tanggal).Year
                                                     && eventT.workDay == y.workDay
                                                     select x
                                 ).ToList();
                            query.AddRange(q3);
                        }
                        listTanggalEvent.Add(new tanggalEvent() { tanggal = tanggal, ev = "Event3" });
                    }
                    else
                        listTanggalEvent.Add(new tanggalEvent() { tanggal = tanggal, ev = "Event2" });
                }
                else
                {
                    listTanggalEvent.Add(new tanggalEvent() { tanggal = tanggal, ev = "Event1" });
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

                if (!query.Any())
                {
                    //isiAtm100.Add(new tanggalValue() { tanggal = tanggal, value = 0 });
                    //isiCrm100.Add(new tanggalValue() { tanggal = tanggal, value = 0 });
                    //sislokAtm100.Add(new tanggalRasio() { tanggal = tanggal, value = 0 });
                    //sislokCrm100.Add(new tanggalValue() { tanggal = tanggal, value = 0 });
                    //sislokCdm100.Add(new tanggalValue() { tanggal = tanggal, value = 0 });
                }
                else
                {
                    //isiAtm100.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)Math.Round((Double)query2.Average(x => x.isiATM100), 0) });
                    //isiCrm100.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)Math.Round((Double)query2.Average(x => x.isiCRM100), 0) });
                    //sislokAtm100.Add(new tanggalRasio() { tanggal = tanggal, value = Math.Round((Double)query2.Average(x => x.RasioSislokAtm100), 2) });
                    //sislokCrm100.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)Math.Round((Double)query2.Average(x => x.sislokCRM100), 0) });
                    //sislokCdm100.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)Math.Round((Double)query2.Average(x => x.sislokCDM100), 0) });
                }
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

            Console.WriteLine("Isi ATM COUNT: " + isiAtm100.Count);
            Console.WriteLine("Sislok ATM COUNT: " + sislokAtm100.Count);
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
            String jenis = metodePrediksiComboBox.SelectedIndex.ToString();
            KumpulanPrediksi prediksi = new KumpulanPrediksi("Jabotabek", listTanggalHistorisUntukPrediksi, tanggal, maxTanggal, jenis, jenis);

            isiAtm50.AddRange(prediksi.prediksiIsiAtm.GroupBy(x => x.tanggal).Select(x => new tanggalValue() { tanggal = x.Key, value = x.Sum(y => y.d50) }).ToList());
            isiCrm50.AddRange(prediksi.isiCrm2.GroupBy(x => x.tanggal).Select(x => new tanggalValue() { tanggal = x.Key, value = x.Sum(y => y.d50) }).ToList());
            sislokAtm50.AddRange(prediksi.rasioSislokAtm.GroupBy(x => x.tanggal).Select(x => new tanggalRasio() { tanggal = x.Key, value = x.Average(y => y.d50) }).ToList());
            sislokCdm50.AddRange(prediksi.sislokCdm.GroupBy(x => x.tanggal).Select(x => new tanggalValue() { tanggal = x.Key, value = x.Sum(y => y.d50) }).ToList());
            sislokCrm50.AddRange(prediksi.sislokCrm.GroupBy(x => x.tanggal).Select(x => new tanggalValue() { tanggal = x.Key, value = x.Sum(y => y.d50) }).ToList());



            //
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
                if(!query.Any())
                {
                    foreach (var temp in listTanggalHistorisUntukPrediksi)
                    {
                        List<TransaksiAtm> q3 = (from x in q
                                                 join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                                 where temp.Month == ((DateTime)x.tanggal).Month
                                                 && temp.Year == ((DateTime)x.tanggal).Year
                                                 && eventT.workDay == y.workDay
                                                 && eventT.@event == y.@event
                                                 select x
                                  ).ToList();
                        query.AddRange(q3);
                    }
                    if (!query.Any())
                    {
                        foreach (var temp in listTanggalHistorisUntukPrediksi)
                        {

                            List<TransaksiAtm> q3 = (from x in q
                                                     join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                                     where temp.Month == ((DateTime)x.tanggal).Month
                                                     && temp.Year == ((DateTime)x.tanggal).Year
                                                     && eventT.workDay == y.workDay
                                                     select x
                                 ).ToList();
                            query.AddRange(q3);
                        }
                        listTanggalEvent.Add(new tanggalEvent() { tanggal = tanggal, ev = "Event3" });
                    }
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
                if (!query.Any())
                {
                    //isiAtm50.Add(new tanggalValue() { tanggal = tanggal, value = 0 });
                    //isiCrm50.Add(new tanggalValue() { tanggal = tanggal, value = 0 });
                    //sislokAtm50.Add(new tanggalRasio() { tanggal = tanggal, value = 0 });
                    //sislokCrm50.Add(new tanggalValue() { tanggal = tanggal, value = 0 });
                    //sislokCdm50.Add(new tanggalValue() { tanggal = tanggal, value = 0 });
                }
                else
                {
                    //isiAtm50.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)Math.Round((Double)query2.Average(x => x.isiATM50), 0) });
                    //isiCrm50.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)Math.Round((Double)query2.Average(x => x.isiCRM50), 0) });
                    //sislokAtm50.Add(new tanggalRasio() { tanggal = tanggal, value = Math.Round((Double)query2.Average(x => x.RasioSislokAtm50), 2) });
                    //sislokCrm50.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)Math.Round((Double)query2.Average(x => x.sislokCRM50), 0) });
                    //sislokCdm50.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)Math.Round((Double)query2.Average(x => x.sislokCDM50), 0) });
                }
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
                if (a >= 0)
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
            
            if (saldo50.Max(x=>x.tanggal) < q2.Max(x=>x.DA.tanggal))
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
                     select new { x.namaPkt, x.tanggal, BN100K = x.inCabang });

            
            List<EventTanggal> et = (from x in db.EventTanggals.AsEnumerable().AsEnumerable()
                                     select x).ToList();


            DateTime tanggal = DateTime.Today;
            DateTime maxTanggal = tanggalMaxPrediksiPicker.Value;
            inCabangUntukKirim100 = new List<StoreClass>();
            while (tanggal <= maxTanggal)
            {
                List<StoreClass> query = new List<StoreClass>();
                var eventT = (from x in et
                              where x.tanggal == tanggal
                              select new { x.workDay, x.@event }).FirstOrDefault();

                foreach (var temp in listTanggalHistorisUntukPrediksi)
                {
                    List<StoreClass> q3 = (from x in q
                                      join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                      where temp.Month == ((DateTime)x.tanggal).Month
                                      && temp.Year == ((DateTime)x.tanggal).Year
                                      && eventT.workDay == y.workDay
                                      && eventT.@event == y.@event
                                      select new { tanggal = (DateTime)x.tanggal, BN100K = x.BN100K == null ? 0 : (Int64)x.BN100K, x.namaPkt }
                              ).AsEnumerable().Where(x => x.tanggal.DayOfWeek == tanggal.DayOfWeek).Select(x => new StoreClass() { tanggal = x.tanggal, kodePkt = x.namaPkt, val = x.BN100K }).ToList();
                    query.AddRange(q3);
                }
                if (!query.Any())
                {
                    foreach (var temp in listTanggalHistorisUntukPrediksi)
                    {
                        List<StoreClass> q3 = (from x in q
                                               join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                               where temp.Month == ((DateTime)x.tanggal).Month
                                               && temp.Year == ((DateTime)x.tanggal).Year
                                               && eventT.workDay == y.workDay
                                               && eventT.@event == y.@event
                                               select new StoreClass()
                                               {
                                                   tanggal = (DateTime)x.tanggal,
                                                   kodePkt = x.namaPkt,
                                                   val = (Int64)x.BN100K
                                               }
                                  ).ToList();
                        query.AddRange(q3);
                    }
                    if(!query.Any())
                    {
                        foreach (var temp in listTanggalHistorisUntukPrediksi)
                        {
                            List<StoreClass> q3 = (from x in q
                                                   join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                                   where temp.Month == ((DateTime)x.tanggal).Month
                                                   && temp.Year == ((DateTime)x.tanggal).Year
                                                   && eventT.workDay == y.workDay
                                                   select new StoreClass()
                                                   {
                                                       tanggal = (DateTime)x.tanggal,
                                                       kodePkt = x.namaPkt,
                                                       val = (Int64)x.BN100K
                                                   }
                                      ).ToList();
                            query.AddRange(q3);
                        }
                    }
                }

                var query2 = (from x in query
                              group x by x.tanggal into g
                              select new { g.Key, val = g.Sum(x => x.val) }).ToList();

                if (!query2.Any())
                {
                    result.Add(new tanggalValue() { tanggal = tanggal, value = 0 });
                    inCabangUntukKirim100.AddRange(q.Select(x => new StoreClass() { tanggal = tanggal, kodePkt = x.namaPkt, val = 0 }).Distinct().ToList());
                }
                else
                {
                    result.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)Math.Round(query.Any() ? query2.Average(x => x.val) : 0, 0) });
                    inCabangUntukKirim100.AddRange(query.GroupBy(x => x.kodePkt).Select(x => new StoreClass() { tanggal = tanggal, kodePkt = x.Key, val = (Int64)x.Average(y => y.val) }).ToList());
                }
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
                     select new { x.tanggal, BN50K = x.inCabang, x.namaPkt });
            List<EventTanggal> et = (from x in db.EventTanggals.AsEnumerable().AsEnumerable()
                                     select x).ToList();


            DateTime tanggal = DateTime.Today;
            DateTime maxTanggal = tanggalMaxPrediksiPicker.Value;
            inCabangUntukKirim50 = new List<StoreClass>();
            while (tanggal <= maxTanggal)
            {
                List<StoreClass> query = new List<StoreClass>();
                var eventT = (from x in et
                              where x.tanggal == tanggal
                              select new { x.workDay, x.@event }).FirstOrDefault();

                foreach (var temp in listTanggalHistorisUntukPrediksi)
                {
                    List<StoreClass> q3 = (from x in q
                                           join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                           where temp.Month == ((DateTime)x.tanggal).Month
                                           && temp.Year == ((DateTime)x.tanggal).Year
                                           && eventT.workDay == y.workDay
                                           && eventT.@event == y.@event
                                           select new { tanggal = (DateTime)x.tanggal, BN50K = x.BN50K == null ? 0 : (Int64)x.BN50K, x.namaPkt }
                              ).AsEnumerable().Where(x => x.tanggal.DayOfWeek == tanggal.DayOfWeek).Select(x => new StoreClass() { tanggal = x.tanggal, kodePkt = x.namaPkt, val = x.BN50K }).ToList();
                    query.AddRange(q3);
                }
                if (!query.Any())
                {
                    foreach (var temp in listTanggalHistorisUntukPrediksi)
                    {
                        List<StoreClass> q3 = (from x in q
                                               join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                               where temp.Month == ((DateTime)x.tanggal).Month
                                               && temp.Year == ((DateTime)x.tanggal).Year
                                               && eventT.workDay == y.workDay
                                               && eventT.@event == y.@event
                                               select new StoreClass()
                                               {
                                                   tanggal = (DateTime)x.tanggal,
                                                   kodePkt = x.namaPkt,
                                                   val = (Int64)x.BN50K
                                               }
                                  ).ToList();
                        query.AddRange(q3);
                    }
                    if (!query.Any())
                    {
                        foreach (var temp in listTanggalHistorisUntukPrediksi)
                        {
                            List<StoreClass> q3 = (from x in q
                                                   join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                                   where temp.Month == ((DateTime)x.tanggal).Month
                                                   && temp.Year == ((DateTime)x.tanggal).Year
                                                   && eventT.workDay == y.workDay
                                                   select new StoreClass()
                                                   {
                                                       tanggal = (DateTime)x.tanggal,
                                                       kodePkt = x.namaPkt,
                                                       val = (Int64)x.BN50K
                                                   }
                                      ).ToList();
                            query.AddRange(q3);
                        }
                    }
                }
                var query2 = (from x in query
                              group x by x.tanggal into g
                              select new { g.Key, val = g.Sum(x => x.val) });
                if (!query2.Any())
                {
                    result.Add(new tanggalValue() { tanggal = tanggal, value = 0 });
                    inCabangUntukKirim50.AddRange(q.Select(x => new StoreClass() { tanggal = tanggal, kodePkt = x.namaPkt, val = 0 }).Distinct().ToList());
                }
                else
                {
                    result.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)Math.Round(query.Any() ? query2.Average(x => x.val) : 0, 0) });
                    inCabangUntukKirim50.AddRange(query.GroupBy(x => x.kodePkt).Select(x => new StoreClass() { tanggal = tanggal, kodePkt = x.Key, val = (Int64)x.Average(y => y.val) }).ToList());
                }
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
                     select new { x.tanggal, BN100K = x.inRetail, x.namaPkt});
            List<EventTanggal> et = (from x in db.EventTanggals.AsEnumerable().AsEnumerable()
                                     select x).ToList();


            DateTime tanggal = DateTime.Today;
            DateTime maxTanggal = tanggalMaxPrediksiPicker.Value;
            inRetailUntukKirim100 = new List<StoreClass>();
            while (tanggal <= maxTanggal)
            {
                List<StoreClass> query = new List<StoreClass>();
                var eventT = (from x in et
                              where x.tanggal == tanggal
                              select new { x.workDay, x.@event }).FirstOrDefault();

                foreach (var temp in listTanggalHistorisUntukPrediksi)
                {
                    List<StoreClass> q3 = (from x in q
                                           join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                           where temp.Month == ((DateTime)x.tanggal).Month
                                           && temp.Year == ((DateTime)x.tanggal).Year
                                           && eventT.workDay == y.workDay
                                           && eventT.@event == y.@event
                                           select new { tanggal = (DateTime)x.tanggal, BN100K = x.BN100K == null ? 0 : (Int64)x.BN100K, x.namaPkt }
                              ).AsEnumerable().Where(x => x.tanggal.DayOfWeek == tanggal.DayOfWeek).Select(x => new StoreClass() { tanggal = x.tanggal, kodePkt = x.namaPkt, val = x.BN100K }).ToList();
                    query.AddRange(q3);
                }
                if (!query.Any())
                {
                    foreach (var temp in listTanggalHistorisUntukPrediksi)
                    {
                        List<StoreClass> q3 = (from x in q
                                               join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                               where temp.Month == ((DateTime)x.tanggal).Month
                                               && temp.Year == ((DateTime)x.tanggal).Year
                                               && eventT.workDay == y.workDay
                                               && eventT.@event == y.@event
                                               select new StoreClass()
                                               {
                                                   tanggal = (DateTime)x.tanggal,
                                                   kodePkt = x.namaPkt,
                                                   val = (Int64)x.BN100K
                                               }
                                  ).ToList();
                        query.AddRange(q3);
                    }
                    if (!query.Any())
                    {
                        foreach (var temp in listTanggalHistorisUntukPrediksi)
                        {
                            List<StoreClass> q3 = (from x in q
                                                   join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                                   where temp.Month == ((DateTime)x.tanggal).Month
                                                   && temp.Year == ((DateTime)x.tanggal).Year
                                                   && eventT.workDay == y.workDay
                                                   select new StoreClass()
                                                   {
                                                       tanggal = (DateTime)x.tanggal,
                                                       kodePkt = x.namaPkt,
                                                       val = (Int64)x.BN100K
                                                   }
                                      ).ToList();
                            query.AddRange(q3);
                        }
                    }
                }
                var query2 = (from x in query
                              group x by x.tanggal into g
                              select new { g.Key, val = g.Sum(x => x.val) });
                if (!query2.Any())
                {
                    result.Add(new tanggalValue() { tanggal = tanggal, value = 0 });
                    inRetailUntukKirim100.AddRange(q.Select(x => new StoreClass() { tanggal = tanggal, kodePkt = x.namaPkt, val = 0 }).Distinct().ToList());
                }
                else
                {
                    result.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)Math.Round(query.Any() ? query2.Average(x => x.val) : 0, 0) });
                    inRetailUntukKirim100.AddRange(query.GroupBy(x => x.kodePkt).Select(x => new StoreClass() { tanggal = tanggal, kodePkt = x.Key, val = (Int64)x.Average(y => y.val) }).ToList());
                }
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
                     select new { x.tanggal, BN50K = x.inRetail, x.namaPkt});
            List<EventTanggal> et = (from x in db.EventTanggals.AsEnumerable().AsEnumerable()
                                     select x).ToList();


            DateTime tanggal = DateTime.Today;
            DateTime maxTanggal = tanggalMaxPrediksiPicker.Value;
            inRetailUntukKirim50 = new List<StoreClass>();
            while (tanggal <= maxTanggal)
            {
                List<StoreClass> query = new List<StoreClass>();
                var eventT = (from x in et
                              where x.tanggal == tanggal
                              select new { x.workDay, x.@event }).FirstOrDefault();

                foreach (var temp in listTanggalHistorisUntukPrediksi)
                {
                    List<StoreClass> q3 = (from x in q
                                           join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                           where temp.Month == ((DateTime)x.tanggal).Month
                                           && temp.Year == ((DateTime)x.tanggal).Year
                                           && eventT.workDay == y.workDay
                                           && eventT.@event == y.@event
                                           select new { tanggal = (DateTime)x.tanggal, BN50K = x.BN50K == null ? 0 : (Int64)x.BN50K, x.namaPkt }
                              ).AsEnumerable().Where(x => x.tanggal.DayOfWeek == tanggal.DayOfWeek).Select(x => new StoreClass() { tanggal = x.tanggal, kodePkt = x.namaPkt, val = x.BN50K }).ToList();
                    query.AddRange(q3);
                }
                if (!query.Any())
                {
                    foreach (var temp in listTanggalHistorisUntukPrediksi)
                    {
                        List<StoreClass> q3 = (from x in q
                                               join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                               where temp.Month == ((DateTime)x.tanggal).Month
                                               && temp.Year == ((DateTime)x.tanggal).Year
                                               && eventT.workDay == y.workDay
                                               && eventT.@event == y.@event
                                               select new StoreClass()
                                               {
                                                   tanggal = (DateTime)x.tanggal,
                                                   kodePkt = x.namaPkt,
                                                   val = (Int64)x.BN50K
                                               }
                                  ).ToList();
                        query.AddRange(q3);
                    }
                    if (!query.Any())
                    {
                        foreach (var temp in listTanggalHistorisUntukPrediksi)
                        {
                            List<StoreClass> q3 = (from x in q
                                                   join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                                   where temp.Month == ((DateTime)x.tanggal).Month
                                                   && temp.Year == ((DateTime)x.tanggal).Year
                                                   && eventT.workDay == y.workDay
                                                   select new StoreClass()
                                                   {
                                                       tanggal = (DateTime)x.tanggal,
                                                       kodePkt = x.namaPkt,
                                                       val = (Int64)x.BN50K
                                                   }
                                      ).ToList();
                            query.AddRange(q3);
                        }
                    }
                }
                var query2 = (from x in query
                              group x by x.tanggal into g
                              select new { g.Key, val = g.Sum(x => x.val) });
                if (!query2.Any())
                {
                    result.Add(new tanggalValue() { tanggal = tanggal, value = 0 });
                    inRetailUntukKirim50.AddRange(q.Select(x => new StoreClass() { tanggal = tanggal, kodePkt = x.namaPkt, val = 0 }).Distinct().ToList());
                }
                else
                {
                    result.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)Math.Round(query.Any() ? query2.Average(x => x.val) : 0, 0) });
                    inRetailUntukKirim50.AddRange(query.GroupBy(x => x.kodePkt).Select(x => new StoreClass() { tanggal = tanggal, kodePkt = x.Key, val = (Int64)x.Average(y => y.val) }).ToList());
                }
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
                     select new { x.tanggal, BN100K = x.outCabang, x.namaPkt});
            List<EventTanggal> et = (from x in db.EventTanggals.AsEnumerable().AsEnumerable()
                                     select x).ToList();


            DateTime tanggal = DateTime.Today;
            DateTime maxTanggal = tanggalMaxPrediksiPicker.Value;
            outCabangUntukKirim100 = new List<StoreClass>();
            while (tanggal <= maxTanggal)
            {
                List<StoreClass> query = new List<StoreClass>();
                var eventT = (from x in et
                              where x.tanggal == tanggal
                              select new { x.workDay, x.@event }).FirstOrDefault();

                foreach (var temp in listTanggalHistorisUntukPrediksi)
                {
                    List<StoreClass> q3 = (from x in q
                                           join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                           where temp.Month == ((DateTime)x.tanggal).Month
                                           && temp.Year == ((DateTime)x.tanggal).Year
                                           && eventT.workDay == y.workDay
                                           && eventT.@event == y.@event
                                           select new { tanggal = (DateTime)x.tanggal, BN100K = x.BN100K == null ? 0 : (Int64)x.BN100K, x.namaPkt }
                              ).AsEnumerable().Where(x => x.tanggal.DayOfWeek == tanggal.DayOfWeek).Select(x => new StoreClass() { tanggal = x.tanggal, kodePkt = x.namaPkt, val = x.BN100K }).ToList();
                    query.AddRange(q3);
                }
                if (!query.Any())
                {
                    foreach (var temp in listTanggalHistorisUntukPrediksi)
                    {
                        List<StoreClass> q3 = (from x in q
                                               join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                               where temp.Month == ((DateTime)x.tanggal).Month
                                               && temp.Year == ((DateTime)x.tanggal).Year
                                               && eventT.workDay == y.workDay
                                               && eventT.@event == y.@event
                                               select new StoreClass()
                                               {
                                                   tanggal = (DateTime)x.tanggal,
                                                   kodePkt = x.namaPkt,
                                                   val = (Int64)x.BN100K
                                               }
                                  ).ToList();
                        query.AddRange(q3);
                    }
                    if (!query.Any())
                    {
                        foreach (var temp in listTanggalHistorisUntukPrediksi)
                        {
                            List<StoreClass> q3 = (from x in q
                                                   join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                                   where temp.Month == ((DateTime)x.tanggal).Month
                                                   && temp.Year == ((DateTime)x.tanggal).Year
                                                   && eventT.workDay == y.workDay
                                                   select new StoreClass()
                                                   {
                                                       tanggal = (DateTime)x.tanggal,
                                                       kodePkt = x.namaPkt,
                                                       val = (Int64)x.BN100K
                                                   }
                                      ).ToList();
                            query.AddRange(q3);
                        }
                    }
                }
                var query2 = (from x in query
                              group x by x.tanggal into g
                              select new { g.Key, val = g.Sum(x => x.val) });
                if (!query2.Any())
                {
                    result.Add(new tanggalValue() { tanggal = tanggal, value = 0 });
                    outCabangUntukKirim100.AddRange(q.Select(x => new StoreClass() { tanggal = tanggal, kodePkt = x.namaPkt, val = 0 }).Distinct().ToList());
                }
                else
                {
                    result.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)Math.Round(query.Any() ? query2.Average(x => x.val) : 0, 0) });
                    outCabangUntukKirim100.AddRange(query.GroupBy(x => x.kodePkt).Select(x => new StoreClass() { tanggal = tanggal, kodePkt = x.Key, val = (Int64)x.Average(y => y.val) }).ToList());
                }
                tanggal = tanggal.AddDays(1);
            }
            return result;
        }
        void loadListBITUKABuntukKirim()
        {
            listBITUKABPermintaanUntukKirim = new List<DataPermintaanDanSumber>();
            listBITUKABSumberUntukKirim = new List<DataPermintaanDanSumber>();
            for (int a = 0; a < inOutBITUKABGridView.RowCount; a++)
            {
                DataGridViewRow row = inOutBITUKABGridView.Rows[a];
                if (row.Cells[0].Value == null)
                    break;
                if (row.Cells[2].Value.ToString().ToLower() == "out")
                {
                    listBITUKABPermintaanUntukKirim.Add(new DataPermintaanDanSumber()
                    {
                        jenisUang = "",
                        tanggal = DateTime.Parse(row.Cells[0].Value.ToString()),
                        namaPkt = row.Cells[1].Value.ToString(),
                        d100 = row.Cells[3].Value == null ? 0 : Int64.Parse(row.Cells[3].Value.ToString()),
                        d50 = row.Cells[4].Value == null ? 0 : Int64.Parse(row.Cells[4].Value.ToString()),
                    });
                }
                if(row.Cells[2].Value.ToString().ToLower() == "in")
                {
                    listBITUKABSumberUntukKirim.Add(new DataPermintaanDanSumber()
                    {
                        jenisUang = "",
                        tanggal = DateTime.Parse(row.Cells[0].Value.ToString()),
                        namaPkt = row.Cells[1].Value.ToString(),
                        d100 = row.Cells[3].Value == null ? 0 : Int64.Parse(row.Cells[3].Value.ToString()),
                        d50 = row.Cells[4].Value == null ? 0 : Int64.Parse(row.Cells[4].Value.ToString()),
                    });
                }
            }
        }

        private void InputAdhocBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = Variables.excelFilter;
            if(of.ShowDialog() == DialogResult.OK)
            {
                loadForm.ShowSplashScreen();
                listAdhocCabang = new List<ApprovalPembagianSaldo>();
                DataSet ds = Util.openExcel(of.FileName);
                DataTable dt = ds.Tables[0];
                int COL_TANGGAL = 'a' - 'a',
                    COL_KODECABANG = 'b' - 'a',
                    COL_PKT = 'd' - 'a',
                    COL_D100 = 'e' - 'a',
                    COL_D50 = 'f' - 'a',
                    COL_JENIS_UANG = 'n'-'a';
                foreach (DataRow row in dt.Rows)
                {
                    //Skip header / data kosong / data invalid
                    if(row[COL_KODECABANG].ToString().Length != 5 || String.IsNullOrEmpty(row[COL_TANGGAL].ToString()))
                        continue;

                    String tanggalS = row[COL_TANGGAL].ToString(),
                        kodeCabang = row[COL_KODECABANG].ToString().Substring(1,4),
                        pktSumber = row[COL_PKT].ToString(),
                        jenisTransaksi = "Adhoc Cabang",
                        d100S = row[COL_D100].ToString(),
                        d50S = row[COL_D50].ToString(),
                        jenisUang = row[COL_JENIS_UANG].ToString();
                    Int64 d100 = 0,
                        d50 = 0,
                        buf;
                    if (Int64.TryParse(d100S, out buf))
                        d100 = buf;
                    if (Int64.TryParse(d50S, out buf))
                        d50 = buf;
                    Console.WriteLine(kodeCabang);
                    String pktTujuan = (from x in db.Cabangs.AsEnumerable()
                                        where x.kodeCabang.TrimStart('0') == kodeCabang.TrimStart('0').TrimEnd()
                                        select x.kodePkt).FirstOrDefault();

                    listAdhocCabang.Add(new ApprovalPembagianSaldo()
                    {
                        Tanggal = DateTime.Parse(tanggalS),
                        JenisTransaksi = jenisTransaksi,
                        JenisUang = jenisUang,
                        PktSumber = pktSumber,
                        PktTujuan = pktTujuan,
                        D50 = d50 * 50000,
                        D100 = d100 * 100000,
                    });
                }
                adhocCabang100Num.Value = listAdhocCabang.Sum(x => x.D100);
                adhocCabang50Num.Value = listAdhocCabang.Sum(x => x.D50);
                loadForm.CloseForm();
            }
        }
        private void InputDeliveryBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = Variables.excelFilter;
            if (of.ShowDialog() == DialogResult.OK)
            {
                loadForm.ShowSplashScreen();
                listDeliveryCabang = new List<ApprovalPembagianSaldo>();
                List<ApprovalPembagianSaldo> listDariExcel = new List<ApprovalPembagianSaldo>();
                DataSet ds = Util.openExcel(of.FileName);
                DataTable dt = ds.Tables[0];
                int COL_TANGGAL = 'a' - 'a',
                    COL_KODECABANG = 'b' - 'a',
                    COL_PKT = 'l' - 'a',
                    COL_DENOM = 'j' - 'a',
                    COL_AMOUNT = 'n' - 'a',
                    COL_QUALITY = 'k'-'a',
                    COL_ACTION = 'm'-'a',
                    COL_STATUS = 'g'-'a';
                foreach (DataRow row in dt.Rows)
                {
                    String tanggalS = row[COL_TANGGAL].ToString(),
                    kodeCabang = row[COL_KODECABANG].ToString().Substring(1,4),
                    pkt = row[COL_PKT].ToString(),
                    denom = row[COL_DENOM].ToString(),
                    amountS = row[COL_AMOUNT].ToString(),
                    quality = row[COL_QUALITY].ToString(),
                    action = row[COL_ACTION].ToString(),
                    jenisTransaksi = "Delivery Cabang",
                    status = row[COL_STATUS].ToString();
                    Int64 amount = 0, buf;
                    if (Int64.TryParse(amountS, out buf))
                        amount = buf;

                    if (status != "Branch Approved" && action != "Delivery" && denom != "100000" && denom != "50000" & kodeCabang.Length != 5)
                    {
                        Console.WriteLine(kodeCabang + " " + denom + " " + status + " " + action + " Tidak masuk");
                        continue;
                    }
                    String pktTujuan = db.Cabangs.AsEnumerable().Where(x => x.kodeCabang.TrimStart('0') == kodeCabang.TrimStart('0')).Select(x => x.kodePkt).FirstOrDefault();
                    if (denom == "100000")
                        listDariExcel.Add(new ApprovalPembagianSaldo()
                        {

                            Tanggal = DateTime.Parse(tanggalS),
                            PktTujuan = pktTujuan,
                            PktSumber = pkt,
                            JenisUang = quality,
                            JenisTransaksi = jenisTransaksi,
                            D100 = amount,
                            D50 = 0,
                        });
                    if (denom == "50000")
                        listDariExcel.Add(new ApprovalPembagianSaldo()
                        {

                            Tanggal = DateTime.Parse(tanggalS),
                            PktTujuan = pktTujuan,
                            PktSumber = pkt,
                            JenisUang = quality,
                            JenisTransaksi = jenisTransaksi,
                            D100 = 0,
                            D50 = amount,
                        });
                }
                Console.WriteLine("LIST DARI EXCEL COUNT: " + listDariExcel.Count);
                listDeliveryCabang = listDariExcel
                    .GroupBy(x => new { x.Tanggal, x.PktSumber, x.PktTujuan, x.JenisUang, x.JenisTransaksi })
                    .Select(x => new ApprovalPembagianSaldo() {
                        Tanggal = x.Key.Tanggal,
                        PktTujuan = x.Key.PktTujuan,
                        PktSumber = x.Key.PktSumber,
                        JenisTransaksi = x.Key.JenisTransaksi,
                        JenisUang = x.Key.JenisUang,
                        D100 = x.Sum(y=>y.D100),
                        D50 = x.Sum(y=>y.D50)
                    }).ToList();
                loadForm.CloseForm();
            }
        }
        List<tanggalValue> loadPrediksiOutCabang50SP()
        {
            List<tanggalValue> result = new List<tanggalValue>();
            List<DateTime> listTanggalHistorisUntukPrediksi = loadTanggalHistorisUntukPrediksi();

            //Load semua in Cabang 50
            var q = (from x in db.StokPosisis
                     where x.denom == "50000"
                     select new { x.tanggal, BN50K = x.outCabang, x.namaPkt });
            List<EventTanggal> et = (from x in db.EventTanggals.AsEnumerable().AsEnumerable()
                                     select x).ToList();


            DateTime tanggal = DateTime.Today;
            DateTime maxTanggal = tanggalMaxPrediksiPicker.Value;
            outCabangUntukKirim50 = new List<StoreClass>();
            while (tanggal <= maxTanggal)
            {
                List<StoreClass> query = new List<StoreClass>();
                var eventT = (from x in et
                              where x.tanggal == tanggal
                              select new { x.workDay, x.@event }).FirstOrDefault();

                foreach (var temp in listTanggalHistorisUntukPrediksi)
                {
                    List<StoreClass> q3 = (from x in q
                                           join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                           where temp.Month == ((DateTime)x.tanggal).Month
                                           && temp.Year == ((DateTime)x.tanggal).Year
                                           && eventT.workDay == y.workDay
                                           && eventT.@event == y.@event
                                           select new { tanggal = (DateTime)x.tanggal, BN50K = x.BN50K == null ? 0 : (Int64)x.BN50K, x.namaPkt }
                              ).AsEnumerable().Where(x => x.tanggal.DayOfWeek == tanggal.DayOfWeek).Select(x => new StoreClass() { tanggal = x.tanggal, kodePkt = x.namaPkt, val = x.BN50K }).ToList();
                    query.AddRange(q3);
                }
                if (!query.Any())
                {
                    foreach (var temp in listTanggalHistorisUntukPrediksi)
                    {
                        List<StoreClass> q3 = (from x in q
                                               join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                               where temp.Month == ((DateTime)x.tanggal).Month
                                               && temp.Year == ((DateTime)x.tanggal).Year
                                               && eventT.workDay == y.workDay
                                               && eventT.@event == y.@event
                                               select new StoreClass()
                                               {
                                                   tanggal = (DateTime)x.tanggal,
                                                   kodePkt = x.namaPkt,
                                                   val = (Int64)x.BN50K
                                               }
                                  ).ToList();
                        query.AddRange(q3);
                    }
                    if (!query.Any())
                    {
                        foreach (var temp in listTanggalHistorisUntukPrediksi)
                        {
                            List<StoreClass> q3 = (from x in q
                                                   join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
                                                   where temp.Month == ((DateTime)x.tanggal).Month
                                                   && temp.Year == ((DateTime)x.tanggal).Year
                                                   && eventT.workDay == y.workDay
                                                   select new StoreClass()
                                                   {
                                                       tanggal = (DateTime)x.tanggal,
                                                       kodePkt = x.namaPkt,
                                                       val = (Int64)x.BN50K
                                                   }
                                      ).ToList();
                            query.AddRange(q3);
                        }
                    }
                }
                var query2 = (from x in query
                              group x by x.tanggal into g
                              select new { g.Key, val = g.Sum(x => x.val) });
                if (!query2.Any())
                {
                    result.Add(new tanggalValue() { tanggal = tanggal, value = 0 });
                    outCabangUntukKirim50.AddRange(q.Select(x => new StoreClass() { tanggal = tanggal, kodePkt = x.namaPkt, val = 0 }).Distinct().ToList());
                }
                else
                {
                    result.Add(new tanggalValue() { tanggal = tanggal, value = (Int64)Math.Round(query.Any() ? query2.Average(x => x.val) : 0, 0) });
                    outCabangUntukKirim50.AddRange(query.GroupBy(x => x.kodePkt).Select(x => new StoreClass() { tanggal = tanggal, kodePkt = x.Key, val = (Int64)x.Average(y => y.val) }).ToList());
                }
                tanggal = tanggal.AddDays(1);
            }
            return result;
        }
        List<tanggalValue> loadInATM(String denom)
        {
            Database1Entities db = new Database1Entities();
            DateTime newestApprovalDate = db.Approvals.Max(x => x.tanggal);

            List<tanggalValue> res = new List<tanggalValue>();
            if (denom == "100000")
            {
                res = (from x in db.Approvals
                       join y in db.DetailApprovals on x.idApproval equals y.idApproval
                       join z in db.Pkts on x.kodePkt equals z.kodePkt
                       where z.kanwil.ToUpper().Contains("JABO")
                       && (DateTime)x.tanggal == Variables.todayDate
                       group y by y.tanggal into g
                       select new tanggalValue() { tanggal = (DateTime)g.Key, value = (Int64)g.Sum(o => o.setor100 + o.setorAdhoc100) }).ToList();

            }
            if (denom == "50000")
            {
                res = (from x in db.Approvals
                       join y in db.DetailApprovals on x.idApproval equals y.idApproval
                       join z in db.Pkts on x.kodePkt equals z.kodePkt
                       where z.kanwil.ToUpper().Contains("JABO")
                       && (DateTime)x.tanggal == Variables.todayDate
                       group y by y.tanggal into g
                       select new tanggalValue() { tanggal = (DateTime)g.Key, value = (Int64)g.Sum(o => o.setor50 + o.setorAdhoc50) }).ToList();
            }
            return res;
        }

        private void groupingStokMorningBalanceComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            loadTableStokMorningBalance();
        }

        private void inOutBITUKABGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            Int64 sum100 = 0, 
                sum50 = 0;
            for(int a= 0; a < inOutBITUKABGridView.RowCount-1;a++)
            {
                DataGridViewRow row = inOutBITUKABGridView.Rows[a];
                Console.WriteLine(a);
                Console.WriteLine(row);
                Console.WriteLine(row.Cells[3]);
                Console.WriteLine(row.Cells[3].Value);
                if (row == null)
                    break;


                String s100 = row.Cells[3] == null || row.Cells[3].Value == null ? "0" : row.Cells[3].Value.ToString(), 
                    s50 = row.Cells[4] == null || row.Cells[4].Value == null ? "0" : row.Cells[4].Value.ToString();
                if (!String.IsNullOrEmpty(s100))
                    sum100 += Int64.Parse(s100);
                if (!String.IsNullOrEmpty(s50))
                    sum50 += Int64.Parse(s50);
            }
            InOutTUKABSum100Lbl.Text = "Sum 100: " + sum100.ToString("N0");
            InOutTUKABSum50Lbl.Text = "Sum 50: " + sum50.ToString("N0");
        }

        List<tanggalValue> loadMorningBalance100SP()
        {
            List<tanggalValue> listMorningBalance = new List<tanggalValue>();
            List<tanggalValue> listEndingBalance = new List<tanggalValue>();

            DateTime today = DateTime.Today.Date;
            DateTime maxTanggal = tanggalMaxPrediksiPicker.Value.Date;
            bool newNote = newNoteCheckBox.Checked;
            Double persenUnprocessed = (Double)persenUnprocessedNum.Value / 100;

            var query = (from x in db.StokPosisis
                         select x).ToList();
            var q2 = (from x in query
                      where x.tanggal == today
                      && x.denom == "100000"
                      select x).ToList();

            tanggalValue adhocATM100 = new tanggalValue() { tanggal = Variables.todayDate, value = (Int64)adhocATM100Num.Value },
                adhocCabang100 = new tanggalValue() { tanggal = Variables.todayDate, value = (Int64)adhocCabang100Num.Value },
                adhocTukab100 = new tanggalValue() { tanggal = Variables.todayDate, value = (Int64)adhocTukab100Num.Value },
                inBITukab100 = new tanggalValue() { tanggal = inBITukabDateTimePicker.Value.Date, value = (Int64)inBITukab100Num.Value },
                outBITukab100 = new tanggalValue() { tanggal = outBITukabDateTimePicker.Value.Date, value = (Int64)outBITukab100Num.Value };
            List<tanggalValue> listInAtm100 = loadInATM("100000");
            List<tanggalValue> AdhocCabang = (from x in listAdhocCabang
                                              group x by x.Tanggal into g
                                              select new tanggalValue() {
                                                  tanggal = g.Key,
                                                  value = g.Sum(x=>x.D100)
                                              }).ToList();
            List<tanggalValue> DeliveryCabang = (from x in listDeliveryCabang
                                              group x by x.Tanggal into g
                                              select new tanggalValue()
                                              {
                                                  tanggal = g.Key,
                                                  value = g.Sum(x => x.D100)
                                              }).ToList();

            //Morning Balance Hari Pertama
            listMorningBalance.Add(new tanggalValue() {
                tanggal = today,
                value = (Int64) q2.Sum(x=>(x.fitBaru + x.fitLama + x.fitNKRI + (newNote == true ? (x.newBaru + x.newLama ) : 0) + (x.unprocessed * persenUnprocessed) + x.passThrough ))
              });
            morningBalance100HariH = q2.Select(x => new StoreClass() {
                tanggal = today,
                kodePkt = x.namaPkt,
                val = (Int64)(x.fitBaru + x.fitLama + x.fitNKRI + (newNote == true ? (x.newBaru + x.newLama) : 0) + (x.unprocessed * persenUnprocessed) + x.passThrough)
            }).ToList();


            //Hitung Ending Balance + Morning Balance Selanjutnya
            DateTime temp = today;
            while(temp<=maxTanggal)
            {
                String debugText = "Tanggal Morning Balance " + temp.AddDays(1) + " = ";
                Int64 morningBal = listMorningBalance.Where(x => x.tanggal == temp).Select(x => x.value).First();
                debugText += morningBal;

                Int64 totalIn = (from x in prediksiInRetail100
                                 join y in prediksiInCabang100 on x.tanggal equals y.tanggal
                                 where x.tanggal == temp
                                 select x.value+y.value).First();
                Int64 endBal;
                endBal = morningBal + totalIn;
                debugText += " + " + totalIn;

                if (temp == adhocATM100.tanggal)
                {
                    endBal = endBal - adhocATM100.value;
                    debugText += " - " + (Int64) adhocATM100.value;
                }
                if (temp == adhocCabang100.tanggal)
                {
                    endBal = endBal - adhocCabang100.value;
                }
                if (temp == adhocTukab100.tanggal)
                {
                    endBal = endBal - adhocTukab100.value;
                }
                if (temp == inBITukab100.tanggal)
                {
                    endBal += inBITukab100.value;
                }
                if (temp == outBITukab100.tanggal)
                {
                    endBal -= outBITukab100.value;
                }
                //Kalo tambah setor
                //var inAtm100 = listInAtm100.Where(x => x.tanggal == temp).Select(x => x.value).FirstOrDefault();
                //endBal += inAtm100;
                //debugText += " + " + inAtm100 + "(In ATM)";
                
                listEndingBalance.Add(new tanggalValue() {
                    tanggal = temp,
                    value = endBal
                });

                if (temp == maxTanggal)
                    break;



                DateTime temph1 = temp.AddDays(1);
                Int64 totalOut;
               
                totalOut = (from x in prediksiOutAtm100
                            join y in prediksiOutCabang100 on x.tanggal equals y.tanggal
                            where x.tanggal == temph1
                            select x.value + y.value).First();
                var valueAdhoc = AdhocCabang.Where(x => x.tanggal == temp).FirstOrDefault();
                if ( valueAdhoc != null)
                {
                    endBal -= valueAdhoc.value;
                }
                Int64 morningBalh1 = endBal - totalOut;
                
                debugText += " - " + totalOut;
                Console.WriteLine(debugText);
                listMorningBalance.Add(new tanggalValue() { tanggal = temph1, value = morningBalh1});
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
            bool newNote = newNoteCheckBox.Checked;
            Double persenUnprocessed = (Double)persenUnprocessedNum.Value / 100;
            var query = (from x in db.StokPosisis
                         select x).ToList();
            var q2 = (from x in query
                      where x.tanggal == today
                      && x.denom == "50000"
                      select x).ToList();
            tanggalValue adhocATM50 = new tanggalValue() { tanggal = Variables.todayDate, value = (Int64)adhocATM50Num.Value },
                adhocCabang50 = new tanggalValue() { tanggal = Variables.todayDate, value = (Int64)adhocCabang50Num.Value },
                adhocTukab50 = new tanggalValue() { tanggal = Variables.todayDate, value = (Int64)adhocTukab50Num.Value },
                inBITukab50 = new tanggalValue() { tanggal = inBITukabDateTimePicker.Value.Date, value = (Int64)inBITukab50Num.Value },
                outBITukab50 = new tanggalValue() { tanggal = outBITukabDateTimePicker.Value.Date, value = (Int64)outBITukab50Num.Value };
            List<tanggalValue> listInAtm50 = loadInATM("50000");
            List<tanggalValue> AdhocCabang = (from x in listAdhocCabang
                                              group x by x.Tanggal into g
                                              select new tanggalValue()
                                              {
                                                  tanggal = g.Key,
                                                  value = g.Sum(x => x.D50)
                                              }).ToList();
            //Morning Balance Hari Pertama
            listMorningBalance.Add(new tanggalValue()
            {
                tanggal = today,
                value = (Int64)q2.Sum(x => (x.fitBaru + x.fitLama + x.fitNKRI + (newNote == true ? (x.newBaru + x.newLama) : 0) + (x.unprocessed * persenUnprocessed) + x.passThrough))
            });
            morningBalance50HariH = q2.Select(x => new StoreClass()
            {
                tanggal = today,
                kodePkt = x.namaPkt,
                val = (Int64)(x.fitBaru + x.fitLama + x.fitNKRI + (newNote == true ? (x.newBaru + x.newLama) : 0) + (x.unprocessed * persenUnprocessed) + x.passThrough)
            }).ToList();


            //Hitung Ending Balance + Morning Balance Selanjutnya
            DateTime temp = today;
            while (temp <= maxTanggal)
            {
                Int64 morningBal = listMorningBalance.Where(x => x.tanggal == temp).Select(x => x.value).First();
                Int64 totalIn = (from x in prediksiInRetail50
                                 join y in prediksiInCabang50 on x.tanggal equals y.tanggal
                                 where x.tanggal == temp
                                 select x.value + y.value).First();
                Int64 adhoc50 = (Int64)adhocATM50Num.Value;
                Int64 endBal;
                endBal = morningBal + totalIn;
                if (temp == adhocATM50.tanggal)
                    endBal = endBal - adhocATM50.value;
                if (temp == adhocCabang50.tanggal)
                    endBal = endBal - adhocCabang50.value;
                if (temp == adhocTukab50.tanggal)
                    endBal = endBal - adhocTukab50.value;
                if (temp == inBITukab50.tanggal)
                    endBal += inBITukab50.value;
                if (temp == outBITukab50.tanggal)
                    endBal -= outBITukab50.value;

                //Kalo tambah setor
                //var inAtm50 = listInAtm50.Where(x => x.tanggal == temp).Select(x => x.value).FirstOrDefault();
                //endBal += inAtm50;

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
                                      join y in prediksiOutCabang50 on x.tanggal equals y.tanggal
                                      where x.tanggal == temph1
                                      select x.value + y.value ).First();
                    var valueAdhoc = AdhocCabang.Where(x => x.tanggal == temp).FirstOrDefault();
                    if (valueAdhoc != null)
                    {
                        endBal -= valueAdhoc.value;
                    }
                    Int64 morningBalh1 = endBal - totalOut;
                    listMorningBalance.Add(new tanggalValue() { tanggal = temph1, value = morningBalh1 });
                }
                temp = temp.AddDays(1);
            }
            return listMorningBalance;
        }
        List<StoreClass> loadNewNote100()
        {
            List<StoreClass> result = new List<StoreClass>();
            var query = (from x in db.StokPosisis
                         select x).ToList();
            var q2 = (from x in query
                      where x.tanggal == Variables.todayDate
                      && x.denom == "100000"
                      select x).ToList();

            result = q2.Select(x => new StoreClass() {
                kodePkt = x.namaPkt,
                tanggal = (DateTime)x.tanggal,
                val = (Int64)x.newBaru
            }).ToList();
            return result;
        }
        List<StoreClass> loadNewNote50()
        {
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
        void loadTableStokMorningBalance()
        {
           
            var query = (from x in db.StokPosisis
                         where x.tanggal == Variables.todayDate
                         && (x.denom == "100000" || x.denom == "50000")
                         select x).ToList();

            var q2 = (from x in query
                      select x).ToList();
            if (groupingStokMorningBalanceComboBox.SelectedItem.ToString() == "ALL VENDOR(JABO)")
            {
                var toShow = (from x in q2
                              group x by x.denom into g
                              select new
                              {
                                  denom = g.Key,
                                  unprocessed = g.Sum(x => x.unprocessed),
                                  newBaru = g.Sum(x => x.newBaru),
                                  newLama = g.Sum(x => x.newLama),
                                  fitBaru = g.Sum(x => x.fitBaru),
                                  fitNKRI = g.Sum(x => x.fitNKRI),
                                  fitLama = g.Sum(x => x.fitLama),
                                  passtrough = g.Sum(x => x.passThrough),
                              }).ToList();
                stokMorningBalanceDataGridView.DataSource = toShow;
                for (int a = 1; a < stokMorningBalanceDataGridView.ColumnCount; a++)
                {
                    stokMorningBalanceDataGridView.Columns[a].DefaultCellStyle.Format = "N0";
                }
            }
            else
            {
                var toShow = (from x in q2
                              group x by new { x.denom, x.namaPkt } into g
                              select new
                              {
                                  g.Key.denom,
                                  g.Key.namaPkt,
                                  unprocessed = g.Sum(x => x.unprocessed),
                                  newBaru = g.Sum(x => x.newBaru),
                                  newLama = g.Sum(x => x.newLama),
                                  fitBaru = g.Sum(x => x.fitBaru),
                                  fitNKRI = g.Sum(x => x.fitNKRI),
                                  fitLama = g.Sum(x => x.fitLama),
                                  passtrough = g.Sum(x => x.passThrough),
                              }).ToList();
                stokMorningBalanceDataGridView.DataSource = toShow;
                for (int a = 2; a < stokMorningBalanceDataGridView.ColumnCount; a++)
                {
                    stokMorningBalanceDataGridView.Columns[a].DefaultCellStyle.Format = "N0";
                }
            }
        }
        void initInOutBiTukabGridView()
        {
            inOutBITUKABGridView.Columns.Clear();
            inOutBITUKABGridView.Rows.Clear();
            //Init comboboxes
            List<String> listTanggal = new List<String>();
            if (adhocTukab100Num.Value != 0 || adhocTukab50Num.Value != 0)
            {
                listTanggal.Add(Variables.todayDate.ToShortDateString());
            }
            if (inBITukab100Num.Value > 0 || inBITukab50Num.Value > 0)
            {
                listTanggal.Add(inBITukabDateTimePicker.Value.Date.ToShortDateString());
                Console.WriteLine(inBITukabDateTimePicker.Value.Date.ToShortDateString());

            }
            if (outBITukab100Num.Value > 0 || outBITukab50Num.Value > 0)
            {
                listTanggal.Add(outBITukabDateTimePicker.Value.Date.ToShortDateString());
            }
            List<String> inOut = new List<String>() { "in", "out" };
            List<String> listNamaBank = (from x in db.DataBankLains select x.namaBank).ToList();
            DataGridViewComboBoxColumn colTanggal = new DataGridViewComboBoxColumn()
            {
                HeaderText = "Tanggal",
                DataSource = listTanggal,
                ValueType = typeof(String),
                Width = 100
            };
            DataGridViewComboBoxColumn colNamaBank = new DataGridViewComboBoxColumn()
            {
                HeaderText = "Nama Bank",
                DataSource = listNamaBank,
                ValueType = typeof(String)
            };
            DataGridViewComboBoxColumn colInOut = new DataGridViewComboBoxColumn()
            {
                HeaderText = "IN / OUT",
                DataSource = inOut,
                ValueType = typeof(String)
            };
            DataGridViewTextBoxColumn d100 = new DataGridViewTextBoxColumn()
            {
                HeaderText = "D100",
                ValueType = typeof(Int64)
            };
            DataGridViewTextBoxColumn d50 = new DataGridViewTextBoxColumn()
            {
                HeaderText = "D50",
                ValueType = typeof(Int64)
            };
            inOutBITUKABGridView.Columns.Add(colTanggal);
            inOutBITUKABGridView.Columns.Add(colNamaBank);
            inOutBITUKABGridView.Columns.Add(colInOut);
            inOutBITUKABGridView.Columns.Add(d100);
            inOutBITUKABGridView.Columns.Add(d50);
            inOutBITUKABGridView.Enabled = !(listTanggal.Count == 0);
        }
        void loadKuota()
        {
            var query = (from x in db.StokPosisis
                         where x.tanggal == Variables.todayDate
                         && (x.denom == "100000" || x.denom == "50000")
                         group x by x.denom into g
                         select new
                         {
                             denom = g.Key,
                             fit = g.Sum(x => x.fitBaru + x.fitLama + x.fitNKRI),
                         }).ToList();
            List<tanggalValue> mbh100 = new List<tanggalValue>();
            List<tanggalValue> mbh50 = new List<tanggalValue>();
            mbh100.Add(new tanggalValue() { tanggal = Variables.todayDate, value = (Int64)morningBalance100.Where(x => x.tanggal == Variables.todayDate).Select(x => x.value).FirstOrDefault() });
            mbh50.Add(new tanggalValue(){ tanggal = Variables.todayDate, value = (Int64)morningBalance50.Where(x => x.tanggal == Variables.todayDate).Select(x => x.value).FirstOrDefault() });

            List<PenampungKuota> kuota = (from x in mbh100
                                          join y in mbh50 on x.tanggal equals y.tanggal
                                          select new PenampungKuota(){ tanggal = x.tanggal.AddDays(1), d100 = x.value, d50 = y.value }).ToList();

            kuota[0].d100 += (Int64)(((inRetailUntukKirim100.Where(x => x.tanggal == Variables.todayDate).GroupBy(x=>x.tanggal).Select(x => x.Sum(y=>y.val)).FirstOrDefault() + inCabangUntukKirim100.Where(x => x.tanggal == Variables.todayDate).GroupBy(x => x.tanggal).Select(x => x.Sum(y => y.val)).FirstOrDefault())) * (Double)inflowNum.Value / 100);
            kuota[0].d50 += (Int64)(((inRetailUntukKirim50.Where(x => x.tanggal == Variables.todayDate).GroupBy(x => x.tanggal).Select(x => x.Sum(y => y.val)).FirstOrDefault() + inCabangUntukKirim50.Where(x => x.tanggal == Variables.todayDate).GroupBy(x => x.tanggal).Select(x => x.Sum(y => y.val)).FirstOrDefault())) * (Double)inflowNum.Value / 100);

            if (listAdhocCabang.Any())
            {
                var adhoc = (from x in listAdhocCabang
                             group x by x.Tanggal into g
                             select new
                             {
                                 d100 = g.Sum(x => x.D100),
                                 d50 = g.Sum(x => x.D50)
                             }).FirstOrDefault();
                kuota[0].d100 -= adhoc.d100;
                kuota[0].d50 -= adhoc.d50;
            }
            if (listDeliveryCabang.Any())
            {
                var del = (from x in listDeliveryCabang
                           where x.Tanggal == Variables.todayDate.AddDays(1)
                             group x by x.Tanggal into g
                             select new
                             {
                                 d100 = g.Sum(x => x.D100),
                                 d50 = g.Sum(x => x.D50)
                             }).FirstOrDefault();
                kuota[0].d100 -= del.d100;
                kuota[0].d50 -= del.d50;
            }
            var adhocAtm100 = (from x in db.Approvals.AsEnumerable()
                               join y in db.DetailApprovals.AsEnumerable() on x.idApproval equals y.idApproval
                               join z in db.Pkts.AsEnumerable() on x.kodePkt equals z.kodePkt
                               where x.tanggal == Variables.todayDate
                               && y.tanggal == Variables.todayDate
                               && z.kanwil.Contains("Jabo")
                               group y by y.tanggal into g
                               select g.Sum(x => x.adhoc100)).FirstOrDefault();
            var adhocAtm50 = (from x in db.Approvals.AsEnumerable()
                              join y in db.DetailApprovals.AsEnumerable() on x.idApproval equals y.idApproval
                              join z in db.Pkts.AsEnumerable() on x.kodePkt equals z.kodePkt
                              where x.tanggal == Variables.todayDate
                              && y.tanggal == Variables.todayDate
                              && z.kanwil.Contains("Jabo")
                              group y by y.tanggal into g
                              select g.Sum(x => x.adhoc50)).FirstOrDefault();
            var bonAtm100 = (from x in db.Approvals.AsEnumerable()
                             join y in db.DetailApprovals.AsEnumerable() on x.idApproval equals y.idApproval
                             join z in db.Pkts.AsEnumerable() on x.kodePkt equals z.kodePkt
                             where x.tanggal == Variables.todayDate
                             && y.tanggal == Variables.todayDate.AddDays(1)
                             && z.kanwil.Contains("Jabo")
                             group y by y.tanggal into g
                             select g.Sum(x => x.bon100)).FirstOrDefault();
            var bonAtm50 = (from x in db.Approvals.AsEnumerable()
                            join y in db.DetailApprovals.AsEnumerable() on x.idApproval equals y.idApproval
                            join z in db.Pkts.AsEnumerable() on x.kodePkt equals z.kodePkt
                            where x.tanggal == Variables.todayDate
                            && y.tanggal == Variables.todayDate.AddDays(1)
                            && z.kanwil.Contains("Jabo")
                            group y by y.tanggal into g
                            select g.Sum(x => x.bon50)).FirstOrDefault();

            if (adhocAtm100 != null)
                kuota[0].d100 -= (Int64)bonAtm100;
            if (adhocAtm50 != null)
                kuota[0].d50 -= (Int64)bonAtm50;
            if (bonAtm100 != null)
                kuota[0].d100 -= (Int64)bonAtm100;
            if (bonAtm50 != null)
                kuota[0].d50 -= (Int64)bonAtm50;


            KuotaGridView.DataSource = kuota;

            KuotaGridView.Columns[1].DefaultCellStyle.Format = "N0";
            KuotaGridView.Columns[2].DefaultCellStyle.Format = "N0";
        }
        private void button1_Click(object sender, EventArgs e)
        {
            loadForm.ShowSplashScreen();
            List<DateTime> listTanggal = loadTanggalHistorisUntukPrediksi();


            prediksiInCabang100 = loadPrediksiInCabang100();
            prediksiInCabang50 = loadPrediksiInCabang50();
            prediksiInRetail100 = loadPrediksiInRetail100();
            prediksiInRetail50 = loadPrediksiInRetail50();
            prediksiOutCabang100 = loadPrediksiOutCabang100();
            prediksiOutCabang50 = loadPrediksiOutCabang50();
            prediksiOutAtm100 = loadPrediksiOutAtm100();
            prediksiOutAtm50 = loadPrediksiOutAtm50();
            morningBalance100 = loadMorningBalance100SP();
            morningBalance50 = loadMorningBalance50SP();
            list = new List<ForecastDetail>();
            DateTime tgl = DateTime.Today.Date;
            DateTime maxTanggal = tanggalMaxPrediksiPicker.Value.Date;
            while (tgl <= maxTanggal)
            {
                list.Add(new ForecastDetail()
                {
                    tanggal = tgl,
                    inCabang100 = prediksiInCabang100.Where(x => x.tanggal == tgl).Select(x => x.value).FirstOrDefault(),
                    inRetail100 = prediksiInRetail100.Where(x => x.tanggal == tgl).Select(x => x.value).FirstOrDefault(),
                    inCabang50 = prediksiInCabang50.Where(x => x.tanggal == tgl).Select(x => x.value).FirstOrDefault(),
                    inRetail50 = prediksiInRetail50.Where(x => x.tanggal == tgl).Select(x => x.value).FirstOrDefault(),
                    outCabang100 = prediksiOutCabang100.Where(x => x.tanggal == tgl).Select(x => x.value).FirstOrDefault(),
                    outCabang50 = prediksiOutCabang50.Where(x => x.tanggal == tgl).Select(x => x.value).FirstOrDefault(),
                    outATM100 = prediksiOutAtm100.Where(x => x.tanggal == tgl).Select(x => x.value).FirstOrDefault(),
                    outATM50 = prediksiOutAtm50.Where(x => x.tanggal == tgl).Select(x => x.value).FirstOrDefault()
                });
                tgl = tgl.AddDays(1);
            }

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
                         join te in listTanggalEvent on a.tanggal equals te.tanggal
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
                             MorningBalance50 = k.value,
                             Event = te.ev
                         }).ToList();
            dataGridView1.DataSource = qView.Distinct().ToList();
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
            loadTableStokMorningBalance();

            newNote100 = new List<StoreClass>();
            newNote50 = new List<StoreClass>();
            if(newNoteCheckBox.Checked)
            {
                newNote100 = loadNewNote100();
                newNote50 = loadNewNote50();
            }
            initInOutBiTukabGridView();
            loadKuota();
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

        private void nextButton_Click(object sender, EventArgs e)
        {
            Console.WriteLine("NEXT BUTTON CLICKED!");
            List<StoreClass> morningBalance100UntukKirim = new List<StoreClass>(), 
                morningBalance50UntukKirim = new List<StoreClass>();



            morningBalance100UntukKirim.AddRange(morningBalance100HariH);
            morningBalance50UntukKirim.AddRange(morningBalance50HariH);



            var qOutAtm100 = (from x in db.Approvals
                              join y in db.DetailApprovals on x.idApproval equals y.idApproval
                              join z in db.Pkts on x.kodePkt equals z.kodePkt
                              where x.tanggal == Variables.todayDate
                              && z.kanwil.ToUpper().Contains("JABO")
                              select new StoreClass()
                              {
                                  tanggal = (DateTime)y.tanggal,
                                  kodePkt = z.namaPkt,
                                  val = (Int64)y.bon100 + (Int64)y.adhoc100
                              }).ToList();
            var qOutAtm50 = (from x in db.Approvals
                              join y in db.DetailApprovals on x.idApproval equals y.idApproval
                              join z in db.Pkts on x.kodePkt equals z.kodePkt
                              where x.tanggal == Variables.todayDate
                              && z.kanwil.ToUpper().Contains("JABO")
                              select new StoreClass()
                              {
                                  tanggal = (DateTime)y.tanggal,
                                  kodePkt = z.namaPkt,
                                  val = (Int64)y.bon50 + (Int64)y.adhoc50
                              }).ToList();

            DateTime tanggal = Variables.todayDate;
            DateTime maxTgl = morningBalance100.Max(x => x.tanggal);
            inRetailUntukKirim100.Remove(inRetailUntukKirim100.Where(x => String.IsNullOrWhiteSpace(x.kodePkt)).FirstOrDefault());
            inCabangUntukKirim100.Remove(inCabangUntukKirim100.Where(x => String.IsNullOrWhiteSpace(x.kodePkt)).FirstOrDefault());
            inRetailUntukKirim50.Remove(inRetailUntukKirim50.Where(x => String.IsNullOrWhiteSpace(x.kodePkt)).FirstOrDefault());
            inCabangUntukKirim50.Remove(inCabangUntukKirim50.Where(x => String.IsNullOrWhiteSpace(x.kodePkt)).FirstOrDefault());
            
            while (tanggal <= maxTgl)
            {
                Console.WriteLine(tanggal);
                var toAdd = (from mb in morningBalance100UntukKirim
                             join ir in inRetailUntukKirim100 on new { mb.kodePkt, mb.tanggal } equals new { ir.kodePkt, ir.tanggal } into mbir
                             from ir in mbir.DefaultIfEmpty()
                             join ic in inCabangUntukKirim100 on new {mb.kodePkt, mb.tanggal} equals new {ic.kodePkt, ic.tanggal} into mbic
                             from ic in mbic.DefaultIfEmpty()
                             where mb.tanggal == tanggal
                             select new StoreClass {
                                 kodePkt = mb.kodePkt,
                                 tanggal = tanggal.AddDays(1),
                                 val = mb.val + (ir == null? 0 :ir.val) + (ic == null ? 0 : ic.val)
                             }).ToList();


                morningBalance100UntukKirim.AddRange(toAdd);
                var toAdd50 = (from mb in morningBalance50UntukKirim
                               join ir in inRetailUntukKirim50 on new { mb.kodePkt, mb.tanggal } equals new { ir.kodePkt, ir.tanggal } into mbir
                               from ir in mbir.DefaultIfEmpty()
                               join ic in inCabangUntukKirim50 on new { mb.kodePkt, mb.tanggal } equals new { ic.kodePkt, ic.tanggal } into mbic
                               from ic in mbic.DefaultIfEmpty()
                               where mb.tanggal == tanggal
                               select new StoreClass
                               {
                                   kodePkt = mb.kodePkt,
                                   tanggal = tanggal.AddDays(1),
                                   val = mb.val + (ir == null ? 0 : ir.val) + (ic == null ? 0 : ic.val)
                               }).ToList();
                morningBalance50UntukKirim.AddRange(toAdd50);
                tanggal = tanggal.AddDays(1);
            }

            loadListBITUKABuntukKirim();
            Console.WriteLine("Open Form!");
            PembagianSaldoForm psf = new PembagianSaldoForm(morningBalance100UntukKirim, morningBalance50UntukKirim, newNote100, newNote50, (Double)persenUnprocessedNum.Value / 100, listDeliveryCabang, listAdhocCabang, listBITUKABSumberUntukKirim, listBITUKABPermintaanUntukKirim);
            psf.MdiParent = this.ParentForm;
            psf.Show();
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }
    }
    public class tanggalEvent
    {
        public DateTime tanggal { set; get; }
        public String ev { set; get; }
    }
    public class StoreClass
    {
        public DateTime tanggal { set; get; }
        public String kodePkt { set; get; }
        public Int64 val { set; get; }
    }
    public class PenampungKuota
    {
        public DateTime tanggal { set; get; }
        public Int64 d100 { set; get; }
        public Int64 d50 { set; get; }
    }
    public class PenampungKuotaDetail
    {
        public DateTime tanggal { set; get; }
        public String namaPkt { set; get; }
        public Int64 d100 { set; get; }
        public Int64 d50 { set; get; }
    }
}
