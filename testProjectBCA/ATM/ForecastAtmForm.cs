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

namespace testProjectBCA.ATM
{
    public partial class ForecastAtmForm : Form
    {
        Database1Entities db = new Database1Entities();
        public ForecastAtmForm()
        {
            InitializeComponent();

            loadTreeViewBulanPrediksi();
            loadPkt();
            yangDilihatComboBox.SelectedIndex = 0;
            metodePenghitunganComboBox.DataSource = Variables.listMetodeNonATM;
            metodePenghitunganComboBox.SelectedIndex = 0;
            DateTime maxDate = (from x in db.TransaksiAtms
                                select x.tanggal).Max(x => x),
                     minDate = (from x in db.TransaksiAtms
                                select x.tanggal).Min(x => x);
            StartDatePicker.MaxDate = maxDate;
            StartDatePicker.MinDate = minDate;
            EndDatePicker.MaxDate = maxDate;
            EndDatePicker.MinDate = minDate;
        }
        void loadPkt()
        {
            List<String> temp = (from x in db.Pkts where x.kodePktATM.Length>1 select x.kodePktATM).Distinct().ToList();
            pktComboBox.DataSource = temp;
        }

        void loadTreeViewBulanPrediksi()
        {
            bulanPrediksiTreeView.CheckBoxes = true;
            var temp = (from x in db.TransaksiAtms select x).ToList();

            DateTime minTanggal = (DateTime)temp.Min(x=>x.tanggal);
            DateTime maxTanggal = (DateTime)temp.Max(x=>x.tanggal);

            DateTime tempTanggal = new DateTime(minTanggal.Year, minTanggal.Month, 1);
            int counter = 0;
            bool firstRun = true;
            while (tempTanggal <= maxTanggal)
            {
                bulanPrediksiTreeView.Nodes.Add(tempTanggal.Year.ToString());
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
                    bulanPrediksiTreeView.Nodes[counter].Nodes.Add((monthCounter++).ToString());
                    tempTanggal = tempTanggal.AddMonths(1);
                }
                counter++;
            }
        }
        public List<Denom> loadForecastValue(String jenis, DateTime startDate, DateTime endDate, string metode, List<DateTime> listTanggalHistorisUntukPrediksi, String kodePkt)
        {
            Database1Entities db = new Database1Entities();
            List<Denom> list = new List<Denom>();
            DateTime tanggal = startDate;
            DateTime maxTanggal = endDate;
            var q = (from x in db.TransaksiAtms
                        join y in db.Pkts on x.kodePkt equals y.kodePkt
                     where x.kodePkt == kodePkt
                        select x).ToList();

            var et = (from x in db.EventTanggals select x).ToList();
            KumpulanPrediksi kumpulanPrediksi = new KumpulanPrediksi(kodePkt, listTanggalHistorisUntukPrediksi, startDate, endDate, metode, metode);
            List<Denom> prediksiIsiAtm = new List<Denom>(), isiCrm = new List<Denom>(), sislokCrm = new List<Denom>(), sislokCdm = new List<Denom>();
            List<Rasio> rasioSislokAtm = new List<Rasio>();
            if (kumpulanPrediksi.success)
            {
                prediksiIsiAtm = kumpulanPrediksi.prediksiIsiAtm.GroupBy(x=>x.tanggal).Select(x=>new Denom() {
                    tgl = x.Key,
                    d100 = x.Sum(y=>y.d100),
                    d50 = x.Sum(y=>y.d50),
                    d20 = x.Sum(y=>y.d20),
                }).ToList();
                isiCrm = kumpulanPrediksi.isiCrm2.GroupBy(x => x.tanggal).Select(x => new Denom()
                {
                    tgl = x.Key,
                    d100 = x.Sum(y => y.d100),
                    d50 = x.Sum(y => y.d50),
                    d20 = x.Sum(y => y.d20),
                }).ToList();
                sislokCrm = kumpulanPrediksi.sislokCrm.GroupBy(x => x.tanggal).Select(x => new Denom()
                {
                    tgl = x.Key,
                    d100 = x.Sum(y => y.d100),
                    d50 = x.Sum(y => y.d50),
                    d20 = x.Sum(y => y.d20),
                }).ToList();
                rasioSislokAtm = kumpulanPrediksi.rasioSislokAtm.GroupBy(x => x.tanggal).Select(x => new Rasio()
                {
                    tgl = x.Key,
                    d100 = x.Sum(y => y.d100),
                    d50 = x.Sum(y => y.d50),
                    d20 = x.Sum(y => y.d20),
                }).ToList();
                sislokCdm = kumpulanPrediksi.sislokCdm.GroupBy(x => x.tanggal).Select(x => new Denom()
                {
                    tgl = x.Key,
                    d100 = x.Sum(y => y.d100),
                    d50 = x.Sum(y => y.d50),
                    d20 = x.Sum(y => y.d20),
                }).ToList();

                String eventType = "";
                foreach (var temp in kumpulanPrediksi.eventType)
                {
                    eventType += temp + "\n";
                }
                MessageBox.Show(eventType);
                Console.WriteLine("Prediksi isi atm count: " + prediksiIsiAtm.Count);
                Console.WriteLine("Isi crm count: " + isiCrm.Count);
                Console.WriteLine("Sislok crm count: " + sislokCrm.Count);
                Console.WriteLine("Rasio Sislok ATM count: " + rasioSislokAtm.Count);
                Console.WriteLine("Sislok cdm count: " + sislokCdm.Count);

            }
            Console.WriteLine("JENIS: " + jenis);
            Console.WriteLine("COUNT: " + sislokCrm.Count);
            if (jenis.ToUpper() == "SISLOK CDM")
                list = sislokCdm;
            if (jenis.ToUpper() == "SISLOK CRM")
                list = sislokCrm;
            if(jenis.ToUpper() == "SISLOK ATM")
            {
                list = (from x in prediksiIsiAtm
                        join y in rasioSislokAtm on x.tgl equals y.tgl
                        select new Denom() {
                            tgl = x.tgl,
                            d100 = (Int64) Math.Round(x.d100 * y.d100),
                            d50 = (Int64)Math.Round(x.d50 * y.d50),
                            d20 = (Int64)Math.Round(x.d20 * y.d20),
                        }).ToList();
            }
            if (jenis.ToUpper() == "ISI CRM")
                list = isiCrm;
            if (jenis.ToUpper() == "ISI ATM")
                list = prediksiIsiAtm;
            //while (tanggal <= maxTanggal)
            //{

            //    List<TransaksiAtm> query = new List<TransaksiAtm>();
            //    var eventT = (from x in et
            //                    where x.tanggal == tanggal
            //                    select new { x.workDay, x.@event }).FirstOrDefault();
            //    Console.WriteLine(tanggal.ToShortDateString() + "Workday: " + eventT.workDay + " Event: " + eventT.@event);
            //    foreach (var temp in listTanggalHistorisUntukPrediksi)
            //    {
            //        List<TransaksiAtm> q3 = (from x in q
            //                                    join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
            //                                    where temp.Month == ((DateTime)x.tanggal).Month
            //                                    && temp.Year == ((DateTime)x.tanggal).Year
            //                                    && eventT.workDay == y.workDay
            //                                    && eventT.@event == y.@event
            //                                    select x
            //                    ).AsEnumerable().Where(x => x.tanggal.DayOfWeek == tanggal.DayOfWeek).ToList();
            //        query.AddRange(q3);
            //    }
            //    if(query.Count == 0)
            //    {
            //        Console.WriteLine(tanggal + " Masuk Event2");
            //        foreach (var temp in listTanggalHistorisUntukPrediksi)
            //        {
            //            List<TransaksiAtm> q3 = (from x in q
            //                                     join y in db.EventTanggals.AsEnumerable() on x.tanggal equals y.tanggal
            //                                     where temp.Month == ((DateTime)x.tanggal).Month
            //                                     && temp.Year == ((DateTime)x.tanggal).Year
            //                                     && eventT.workDay == y.workDay
            //                                     && eventT.@event == y.@event
            //                                     select x
            //                  ).ToList();
            //            Console.WriteLine(temp.ToShortDateString() + " " + q3.Count);
            //            query.AddRange(q3);
            //        }
            //    }
            //    Console.WriteLine(tanggal + " Query: " + query.Count);

            //    if (jenis.ToUpper() == "SISLOK CDM")
            //        list.Add(new Denom() { tgl = tanggal, d100 = (Int64)Math.Round((Double)query.Average(x => x.sislokCDM100), 0), d50 = (Int64)Math.Round((Double)query.Average(x => x.sislokCDM50), 0), d20 = (Int64)Math.Round((Double)query.Average(x => x.sislokCDM20), 0) });
            //    else if(jenis.ToUpper() == "SISLOK CRM")
            //        list.Add(new Denom() { tgl = tanggal, d100 = (Int64)Math.Round((Double)query.Average(x => x.sislokCRM100), 0), d50 = (Int64)Math.Round((Double)query.Average(x => x.sislokCRM50), 0), d20 = (Int64)Math.Round((Double)query.Average(x => x.sislokCRM20), 0) });
            //    else if (jenis.ToUpper() == "ISI ATM")
            //        list.Add(new Denom() { tgl = tanggal, d100 = (Int64)Math.Round((Double)query.Average(x => x.isiATM100), 0), d50 = (Int64)Math.Round((Double)query.Average(x => x.isiATM50), 0), d20 = (Int64)Math.Round((Double)query.Average(x => x.isiATM20), 0) });
            //    else if (jenis.ToUpper() == "ISI CRM")
            //        list.Add(new Denom() { tgl = tanggal, d100 = (Int64)Math.Round((Double)query.Average(x => x.isiCRM100), 0), d50 = (Int64)Math.Round((Double)query.Average(x => x.isiCRM50), 0), d20 = (Int64)Math.Round((Double)query.Average(x => x.isiCRM20), 0) });
            //    else if (jenis.ToUpper() == "SISLOK ATM")
            //    {
            //        Int64 d100 = (Int64)Math.Round((Double)query.Average(x => x.sislokATM100), 0);
            //        Console.WriteLine("D100 : " + d100);
            //        list.Add(new Denom() { tgl = tanggal, d100 = (Int64)Math.Round((Double)query.Average(x => x.sislokATM100), 0), d50 = (Int64)Math.Round((Double)query.Average(x => x.sislokATM50), 0), d20 = (Int64)Math.Round((Double)query.Average(x => x.sislokATM20), 0) });
            //    }
            //    tanggal = tanggal.AddDays(1);
            //}
            Console.WriteLine("LIST COUNT IN FUNC: " + list.Count);
            return list;
        }

        List<TransaksiAtm>loadDataRealisasi(DateTime startDate, DateTime endDate, string kodePkt)
        {
            return (from x in db.TransaksiAtms where x.tanggal <= endDate && x.tanggal >= startDate && x.kodePkt == kodePkt select x).ToList();
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

        private void loadButton_Click(object sender, EventArgs e)
        {
            loadForm.ShowSplashScreen();
            DateTime startDate = StartDatePicker.Value.Date;
            DateTime endDate = EndDatePicker.Value.Date;
            String kodePkt = pktComboBox.SelectedItem.ToString();
            String yangDilihat = yangDilihatComboBox.SelectedItem.ToString();
            String metode = metodePenghitunganComboBox.SelectedItem.ToString();
            Console.WriteLine(kodePkt);
            Console.WriteLine(yangDilihat);
            Console.WriteLine(metode);
            List<Denom> list = loadForecastValue(yangDilihat, startDate, endDate, metode, loadTanggalHistorisUntukPrediksi(), kodePkt);

            Console.WriteLine("LIST FORECAST COUNT: " + list.Count);
            List<TransaksiAtm> dataRealisasi = loadDataRealisasi(startDate, endDate, kodePkt);
            List<Denom> listRealisasi = new List<Denom>();
            if (yangDilihat.ToLower() == "sislok atm")
                listRealisasi = (from x in dataRealisasi select new Denom() { tgl = (DateTime)x.tanggal, d100 = (Int64)x.sislokATM100, d50 = (Int64)x.sislokATM50, d20 = (Int64)x.sislokATM20 }).ToList();
            if (yangDilihat.ToLower() == "sislok crm")
                listRealisasi = (from x in dataRealisasi select new Denom() { tgl = (DateTime)x.tanggal, d100 = (Int64)x.sislokCRM100, d50 = (Int64)x.sislokCRM50, d20 = (Int64)x.sislokCRM20 }).ToList();
            if (yangDilihat.ToLower() == "sislok cdm")
                listRealisasi = (from x in dataRealisasi select new Denom() { tgl = (DateTime)x.tanggal, d100 = (Int64)x.sislokCDM100, d50 = (Int64)x.sislokCDM50, d20 = (Int64)x.sislokCDM20 }).ToList();
            if (yangDilihat.ToLower() == "isi atm")
                listRealisasi = (from x in dataRealisasi select new Denom() { tgl = (DateTime)x.tanggal, d100 = (Int64)x.isiATM100, d50 = (Int64)x.isiATM50, d20 = (Int64)x.isiATM20 }).ToList();
            if (yangDilihat.ToLower() == "isi crm")
                listRealisasi = (from x in dataRealisasi select new Denom() { tgl = (DateTime)x.tanggal, d100 = (Int64)x.isiCRM100, d50 = (Int64)x.isiCRM50, d20 = (Int64)x.isiCRM20 }).ToList();
            var q = (from x in list
                        join y in listRealisasi on x.tgl equals y.tgl into xy
                        from y in xy.DefaultIfEmpty()
                        select new
                        {
                            Tanggal = x.tgl,
                            Realisasi100 = y == null? 0 : y.d100,
                            Forecast100 = x.d100,
                            Akurasi100 = y == null ? 0 : (Double)x.d100 / (Double)y.d100,
                            Realisasi50 = y == null ? 0 : y.d50,
                            Forecast50 = x.d50,
                            Akurasi50 = y == null ? 0 : (Double)x.d50 / (Double)y.d50,
                            Realisasi20 = y == null ? 0 : y.d20,
                            Forecast20 = x.d20,
                            Akurasi20 = y == null ? 0 : (Double)x.d20 / (Double)y.d20,
                        }).ToList();

            var sum = (from x in q
                       group x by true into g
                       select new {
                           Tanggal = new DateTime(0001,1,1),
                           Realisasi100 = g.Sum(x => x.Realisasi100),
                           Forecast100 = g.Sum(x=>x.Forecast100),
                           Akurasi100 = (Double) g.Sum(x => x.Forecast100) /(Double) g.Sum(x => x.Realisasi100),
                           Realisasi50 = g.Sum(x=>x.Realisasi50),
                           Forecast50 = g.Sum(x=>x.Forecast50),
                           Akurasi50 = (Double)g.Sum(x => x.Forecast50) / (Double)g.Sum(x => x.Realisasi50),
                           Realisasi20 = g.Sum(x=>x.Realisasi20),
                           Forecast20 = g.Sum(x=>x.Forecast20),
                           Akurasi20 = (Double)g.Sum(x => x.Forecast20) / (Double)g.Sum(x => x.Realisasi20),
                       }).FirstOrDefault();
            q.Add(sum);
            dataGridView1.DataSource = q;
            dataGridView1.Rows[dataGridView1.Rows.Count - 1].DefaultCellStyle.BackColor = Color.Bisque;

            for(int a=1;a<dataGridView1.Columns.Count;a++)
            {
                dataGridView1.Columns[a].DefaultCellStyle.Format = "C";
                dataGridView1.Columns[a].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
                if (a == 3 || a == 6 || a == 9)
                    dataGridView1.Columns[a].DefaultCellStyle.Format = "P";
            }
            loadForm.CloseForm();
        }

        private void bulanPrediksiTreeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            CheckTreeViewNode(e.Node, e.Node.Checked);
        }
    }
}
