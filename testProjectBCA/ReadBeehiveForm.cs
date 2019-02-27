using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testProjectBCA
{
    public partial class ReadBeehiveForm : Form
    {
        Database1Entities en = new Database1Entities();
        List<String> listDataString = new List<String>();
        List<BeeHive> listHasil = new List<BeeHive>();
        List<ProsesBeeHive> prosesBeeHive = new List<ProsesBeeHive>();
        List<ProsesVa> prosesVa = new List<ProsesVa>();
        List<Mcs> mcs = new List<Mcs>();
        List<Va> va = new List<Va>();


        public ReadBeehiveForm()
        {
            InitializeComponent();


        }

        private void inputBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = Variables.txtFilter;
            if (of.ShowDialog() == DialogResult.OK)
            {

                listDataString = new List<String>();
                using (StreamReader sr = File.OpenText(of.FileName))
                {
                    String s = "", stringToAdd = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        if (s.Contains("LAPORAN DAFTAR TRANSAKSI"))
                        {

                            listDataString.Add(stringToAdd);
                            stringToAdd = "";
                        }
                        stringToAdd += s + "\n";
                    }
                }
                listDataString.Remove("");

                MessageBox.Show("ok!");
            }
            prosesString();
        }
        void prosesString()
        {
            prosesBeeHive = new List<ProsesBeeHive>();
            listHasil = new List<BeeHive>();
            foreach (String temp in listDataString)
            {
                List<String> listUntukProses = temp.Split('\n').ToList();
                listUntukProses.RemoveRange(0, 6);
                List<String> toDelete = listUntukProses.Where(x => String.IsNullOrEmpty(x)).ToList();
                foreach (var tempDel in toDelete)
                    listUntukProses.Remove(tempDel);
                for (int a = 0; a < listUntukProses.Count; a++)
                {
                    if (a % 2 == 1)
                    {
                        Console.WriteLine(a);
                        String temp2 = listUntukProses[a];
                        DateTime tanggalKredit = new DateTime(1, 1, 1),
                            bufTanggal;
                        DateTime tanggalTransaksi = new DateTime(1, 1, 1);
                        String kodePerusahaan = temp2.Substring(21, 8);
                        Int64 totalNominal = 0, bufTotalNominal;
                        String formatTanggal = "dd-MM-yyyy";
                        String tanggalS = temp2.Substring(7, 10), totalNominalS = temp2.Substring(77, 32).TrimStart().Replace(".00", "").Replace(",", "");
                        String tanggals2 = temp2.Substring(49, 6).Trim(' ');
                        String message = temp2.Substring(33, 25).ToString();

                        if (DateTime.TryParseExact(tanggalS, formatTanggal, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out bufTanggal))
                            tanggalKredit = bufTanggal;
                        if (Int64.TryParse(totalNominalS, out bufTotalNominal))
                            totalNominal = bufTotalNominal;
                        if (DateTime.TryParseExact(tanggals2, "yyMMdd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out bufTanggal))
                        { tanggalTransaksi = bufTanggal; message = ""; Console.WriteLine("selamat kamu berhasil"); }

                        else
                            tanggalTransaksi = tanggalKredit;




                        listHasil.Add(new BeeHive()
                        {
                            tanggalTransaksi = tanggalTransaksi,
                            tanggalKredit = tanggalKredit,
                            kodePerusahaan = kodePerusahaan,
                            totalNominal = totalNominal,
                            message = message
                            //tanggalkredit

                        });
                    }
                }
            }
            var query = (from x in listHasil
                         group x by new { x.kodePerusahaan, x.tanggalTransaksi, x.tanggalKredit, x.message } into g
                         select new { message = g.Key.message, tanggalKredit = g.Key.tanggalKredit, kodePerusahaan = g.Key.kodePerusahaan, tanggalTransaksi = g.Key.tanggalTransaksi, totalNominal = g.Sum(x => x.totalNominal) }).ToList();

            foreach (var item in query)
            {
                prosesBeeHive.Add(new ProsesBeeHive
                {
                    kodePerusahaan = item.kodePerusahaan,
                    tanggalTransaksi = item.tanggalTransaksi,
                    tanggalKredit = item.tanggalKredit,
                    totalNominal = item.totalNominal,
                    namaFile = item.message,
                    jenisFile = "BEEHIVE"
                });
            }
            dataGridView1.DataSource = prosesBeeHive;

            //dataGridView1.DataSource = prosesBeeHive;
        }

        private void inputButtonMCS_Click(object sender, EventArgs e)
        {
            String kodePkt = "";
            String tanggal = "";
            Int64 temp = 0;

            mcs = new List<Mcs>();

            OpenFileDialog of = new OpenFileDialog();
            of.Filter = Variables.excelFilter;
            if (of.ShowDialog() == DialogResult.OK)
            {
                loadForm.ShowSplashScreen();
                String filename = of.FileName;
                DataSet ds = Util.openExcel(filename);
                DataTable dt = ds.Tables[0];

                DataTable dtCloned = dt.Clone();
                dtCloned.Columns[0].DataType = typeof(String);
                foreach (DataRow row in dt.Rows)
                {
                    dtCloned.ImportRow(row);
                }

                DataRow[] results = dtCloned.Select("Column0 like ' '");

                foreach (var item in results)
                {
                    dtCloned.Rows.Remove(item);
                }

                if (dtCloned.Columns.Count > 23)
                {
                    for (int i = 0; i < dtCloned.Rows.Count; i++)
                    {
                        // maret
                        Console.WriteLine("masuk maret");
                        if (dtCloned.Rows[i][0].ToString() == "Transaction Date")
                        {
                            tanggal = dtCloned.Rows[i][2].ToString().Trim(':').Trim(' ');
                            Console.WriteLine(tanggal);
                        }
                        if (dtCloned.Rows[i][0].ToString() == "Vendor Code")
                        {
                            kodePkt = dtCloned.Rows[i][2].ToString().Trim(':').Trim(' ');
                            Console.WriteLine(kodePkt);
                        }
                        if (Int64.TryParse(dtCloned.Rows[i][0].ToString(), out temp))
                        {
                            if (dtCloned.Rows[i][7].ToString() == "Cash Pickup")
                            {
                                Console.WriteLine(temp);
                                mcs.Add(new Mcs
                                {
                                    tanggal = DateTime.ParseExact(tanggal, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                                    kodePkt = kodePkt,
                                    time = DateTime.Parse(dtCloned.Rows[i][1].ToString()).TimeOfDay,
                                    realDate = DateTime.Parse(dtCloned.Rows[i][1].ToString()).Hour >= 17 ? DateTime.ParseExact(tanggal, "dd/MM/yyyy", CultureInfo.InvariantCulture).AddDays(1).Date : DateTime.ParseExact(tanggal, "dd/MM/yyyy", CultureInfo.InvariantCulture).Date,
                                    customerCode = dtCloned.Rows[i][3].ToString().Trim('\''),
                                    amountTotal = Int64.Parse(dtCloned.Rows[i][5].ToString()),
                                    scheduled = dtCloned.Rows[i][6].ToString(),
                                    d100k = Int64.Parse(dtCloned.Rows[i][8].ToString()),
                                    d50k = Int64.Parse(dtCloned.Rows[i][9].ToString()),
                                    d20k = Int64.Parse(dtCloned.Rows[i][10].ToString()),
                                    d10k = Int64.Parse(dtCloned.Rows[i][11].ToString()),
                                    d5k = Int64.Parse(dtCloned.Rows[i][12].ToString()),
                                    d2k = Int64.Parse(dtCloned.Rows[i][13].ToString()),
                                    d1k = Int64.Parse(dtCloned.Rows[i][14].ToString()),
                                    amountCoin = Int64.Parse(dtCloned.Rows[i][15].ToString()),
                                    cn1k = String.IsNullOrEmpty(dtCloned.Rows[i][16].ToString()) ? 0 : Int64.Parse(dtCloned.Rows[i][16].ToString()),
                                    cn500 = String.IsNullOrEmpty(dtCloned.Rows[i][17].ToString()) ? 0 : Int64.Parse(dtCloned.Rows[i][17].ToString()),
                                    cn200 = String.IsNullOrEmpty(dtCloned.Rows[i][18].ToString()) ? 0 : Int64.Parse(dtCloned.Rows[i][18].ToString()),
                                    cn100 = String.IsNullOrEmpty(dtCloned.Rows[i][19].ToString()) ? 0 : Int64.Parse(dtCloned.Rows[i][19].ToString()),
                                    cn50 = String.IsNullOrEmpty(dtCloned.Rows[i][20].ToString()) ? 0 : Int64.Parse(dtCloned.Rows[i][20].ToString()),
                                    cn25 = String.IsNullOrEmpty(dtCloned.Rows[i][21].ToString()) ? 0 : Int64.Parse(dtCloned.Rows[i][21].ToString()),
                                });
                            }
                        }
                    }
                }

                else
                {
                    for (int i = 0; i < dtCloned.Rows.Count; i++)
                    {
                        // sebelon maret
                        Console.WriteLine("masuk sebelon maret");
                        if (dtCloned.Rows[i][0].ToString() == "Transaction Date")
                        {
                            Console.WriteLine("dapet date");
                            tanggal = dtCloned.Rows[i][2].ToString().Trim(':').Trim(' ');
                            Console.WriteLine(tanggal);
                        }
                        if (dtCloned.Rows[i][0].ToString() == "Vendor Code")
                        {
                            Console.WriteLine("dapet vendor code");
                            kodePkt = dtCloned.Rows[i][2].ToString().Trim(':').Trim(' ');
                            Console.WriteLine(kodePkt);
                        }
                        if (Int64.TryParse(dtCloned.Rows[i][0].ToString(), out temp))
                        {
                            if (dtCloned.Rows[i][6].ToString() == "Cash Pickup")
                            {
                                Console.WriteLine("masuk cashpickup");
                                Console.WriteLine(temp);
                                mcs.Add(new Mcs
                                {
                                    tanggal = DateTime.ParseExact(tanggal, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                                    kodePkt = kodePkt,
                                    time = DateTime.Parse(dtCloned.Rows[i][1].ToString()).TimeOfDay,
                                    realDate = DateTime.Parse(dtCloned.Rows[i][1].ToString()).Hour >= 17 ? DateTime.ParseExact(tanggal, "dd/MM/yyyy", CultureInfo.InvariantCulture).AddDays(1).Date : DateTime.ParseExact(tanggal, "dd/MM/yyyy", CultureInfo.InvariantCulture).Date,
                                    customerCode = dtCloned.Rows[i][3].ToString().Trim('\''),
                                    amountTotal = Int64.Parse(dtCloned.Rows[i][5].ToString()),
                                    d100k = Int64.Parse(dtCloned.Rows[i][7].ToString()),
                                    d50k = Int64.Parse(dtCloned.Rows[i][8].ToString()),
                                    d20k = Int64.Parse(dtCloned.Rows[i][9].ToString()),
                                    d10k = Int64.Parse(dtCloned.Rows[i][10].ToString()),
                                    d5k = Int64.Parse(dtCloned.Rows[i][11].ToString()),
                                    d2k = Int64.Parse(dtCloned.Rows[i][12].ToString()),
                                    d1k = Int64.Parse(dtCloned.Rows[i][13].ToString()),
                                    amountCoin = Int64.Parse(dtCloned.Rows[i][14].ToString()),
                                    cn1k = String.IsNullOrEmpty(dtCloned.Rows[i][15].ToString()) ? 0 : Int64.Parse(dtCloned.Rows[i][15].ToString()),
                                    cn500 = String.IsNullOrEmpty(dtCloned.Rows[i][16].ToString()) ? 0 : Int64.Parse(dtCloned.Rows[i][16].ToString()),
                                    cn200 = String.IsNullOrEmpty(dtCloned.Rows[i][17].ToString()) ? 0 : Int64.Parse(dtCloned.Rows[i][17].ToString()),
                                    cn100 = String.IsNullOrEmpty(dtCloned.Rows[i][18].ToString()) ? 0 : Int64.Parse(dtCloned.Rows[i][18].ToString()),
                                    cn50 = String.IsNullOrEmpty(dtCloned.Rows[i][19].ToString()) ? 0 : Int64.Parse(dtCloned.Rows[i][19].ToString()),
                                    cn25 = String.IsNullOrEmpty(dtCloned.Rows[i][20].ToString()) ? 0 : Int64.Parse(dtCloned.Rows[i][20].ToString()),
                                });
                            }
                        }
                    }


                }
                loadForm.CloseForm();
                MessageBox.Show("ok!");
            }
        }

        private void inputButtonVa_Click(object sender, EventArgs e)
        {
            va = new List<Va>();
            prosesVa = new List<ProsesVa>();

            OpenFileDialog of = new OpenFileDialog();
            of.Filter = Variables.excelFilter;
            if (of.ShowDialog() == DialogResult.OK)
            {
                loadForm.ShowSplashScreen();
                String filename = of.FileName;
                DataSet ds = Util.openExcel(filename);
                DataTable dt = ds.Tables[0];

                DataTable dtCloned = dt.Clone();
                dtCloned.Columns[0].DataType = typeof(String);
                foreach (DataRow row in dt.Rows)
                {
                    dtCloned.ImportRow(row);
                }

                DataRow[] results = dtCloned.Select("Column0 like ' '");

                foreach (var item in results)
                {
                    dtCloned.Rows.Remove(item);
                }

                for (int i = 1; i < dtCloned.Rows.Count; i++)
                {
                    //input disini
                    Console.WriteLine("i yang ke:" + i);
                    Console.WriteLine(dt.Rows[i][2]);
                    va.Add(new Va
                    {
                        kodePerusahaan = dt.Rows[i][4].ToString(),
                        //tanggalTransaksi = DateTime.ParseExact(dt.Rows[i][2].ToString(),"M/d/yyyy", CultureInfo.InvariantCulture),
                        //tanggalKredit = DateTime.ParseExact(dt.Rows[i][1].ToString(), "M/d/yyyy", CultureInfo.InvariantCulture),
                        tanggalTransaksi = DateTime.Parse(dt.Rows[i][2].ToString()),
                        tanggalKredit = DateTime.Parse(dt.Rows[i][1].ToString()),
                        totalNominal = Int64.Parse(dt.Rows[i][6].ToString()),
                        //tanggalkredit

                    });
                }

                var query = (from x in va
                             group x by new { x.kodePerusahaan, x.tanggalTransaksi, x.tanggalKredit } into g
                             select new { tanggalKredit = g.Key.tanggalKredit, kodePerusahaan = g.Key.kodePerusahaan, tanggalTransaksi = g.Key.tanggalTransaksi, totalNominal = g.Sum(x => x.totalNominal) }).ToList();

                foreach (var item in query)
                {
                    prosesVa.Add(new ProsesVa
                    {
                        namaFile = "",
                        kodePerusahaan = item.kodePerusahaan,
                        tanggalTransaksi = item.tanggalTransaksi,
                        tanggalKredit = item.tanggalKredit,
                        totalNominal = item.totalNominal,
                        jenisFile = "VA",
                        //tanggalkredit
                    });
                }

                //dataGridView2.DataSource = prosesVa;
                loadForm.CloseForm();
                MessageBox.Show("ok!");

            }
        }

        private void buttonSaveBeehive_Click(object sender, EventArgs e)
        {
            loadForm.ShowSplashScreen();

            if (prosesBeeHive.Count > 0)
            {
                DateTime maxdate = prosesBeeHive.Max(x => (DateTime)x.tanggalTransaksi);
                DateTime mindate = prosesBeeHive.Min(x => (DateTime)x.tanggalTransaksi);

                List<saveBeeHive> query = (from x in en.saveBeeHives
                                           where x.tanggalTransaksi >= mindate && x.tanggalTransaksi <= maxdate && x.jenisFile == "BEEHIVE"
                                           select x).ToList();

                List<saveBeeHive> toDelete = new List<saveBeeHive>();
                foreach (var item in prosesBeeHive)
                {
                    var simpan = query.Where(x => x.jenisFile == item.jenisFile && x.kodePerusahaan == item.kodePerusahaan && x.namaFile == item.namaFile && x.tanggalKredit == item.tanggalKredit && x.tanggalTransaksi == item.tanggalTransaksi && x.totalNominal == item.totalNominal).FirstOrDefault();
                    if (simpan != null)
                    {
                        toDelete.Add(simpan);
                    }

                }

                Console.WriteLine(toDelete.Count);

                if (toDelete.Any())
                {
                    en.saveBeeHives.RemoveRange(toDelete);
                }

                List<saveBeeHive> saveBee = (from x in prosesBeeHive
                                             select new saveBeeHive()
                                             {
                                                 kodePerusahaan = x.kodePerusahaan,
                                                 tanggalTransaksi = x.tanggalTransaksi,
                                                 tanggalKredit = x.tanggalKredit,
                                                 totalNominal = x.totalNominal,
                                                 namaFile = x.namaFile,
                                                 jenisFile = x.jenisFile
                                             }).ToList();

                Console.WriteLine(saveBee.Count);
                en.saveBeeHives.AddRange(saveBee);
                Console.WriteLine("masuk savebeehive");

                //List<saveBeeHive> saveBeeHive = (from x in prosesBeeHive
                //                                 select new saveBeeHive()
                //                                 {
                //                                     kodePerusahaan = x.kodePerusahaan,
                //                                     tanggal = x.tanggal,
                //                                     totalNominal = x.totalNominal,
                //                                     jenisFile = x.jenisFile
                //                                 }).ToList();

                //DateTime maxdate = saveBeeHive.Max(x => (DateTime)x.tanggal);
                //DateTime mindate = saveBeeHive.Min(x => (DateTime)x.tanggal);

                //var query = (from x in en.saveBeeHives
                //             where (DateTime)x.tanggal >= mindate && (DateTime)x.tanggal <= maxdate
                //             select x).ToList();

                //List<saveBeeHive> bhToRemove = new List<saveBeeHive>();

                //foreach (var item in saveBeeHive)
                //{
                //    var query2 = (from x in query
                //                  where x.tanggal == item.tanggal &&
                //                        x.kodePerusahaan == item.kodePerusahaan
                //                  select x).FirstOrDefault();

                //    if (query2 != null)
                //    {
                //        query2.totalNominal = item.totalNominal;
                //        en.SaveChanges();
                //        bhToRemove.Add(item);
                //    }
                //    Console.WriteLine("hap");
                //}

                //foreach (var item in bhToRemove)
                //{
                //    saveBeeHive.Remove(item);
                //    Console.WriteLine("hup");
                //}

                //Console.WriteLine("keluar");
                //en.saveBeeHives.AddRange(saveBeeHive);
                //en.SaveChanges();

            }
            if (prosesVa.Count > 0)
            {
                DateTime maxdate = prosesVa.Max(x => (DateTime)x.tanggalTransaksi);
                DateTime mindate = prosesVa.Min(x => (DateTime)x.tanggalTransaksi);

                var query2 = (from x in en.saveBeeHives
                              where x.tanggalTransaksi >= mindate && x.tanggalTransaksi <= maxdate && x.jenisFile == "VA"
                              select x).ToList();

                if (query2.Count > 0)
                {
                    en.saveBeeHives.RemoveRange(query2);
                }

                List<saveBeeHive> saveVa = (from x in prosesVa
                                            select new saveBeeHive()
                                            {
                                                kodePerusahaan = "0" + x.kodePerusahaan.ToString(),
                                                tanggalTransaksi = x.tanggalTransaksi,
                                                tanggalKredit = x.tanggalKredit,
                                                totalNominal = x.totalNominal,
                                                namaFile = x.namaFile,
                                                jenisFile = x.jenisFile
                                            }).ToList();

                en.saveBeeHives.AddRange(saveVa);
                Console.WriteLine("masuk saveva");
            }
            en.SaveChanges();
            loadForm.CloseForm();
        }

        private void buttonSaveMcs_Click(object sender, EventArgs e)
        {
            loadForm.ShowSplashScreen();
            List<saveMc> saveMc = (from x in mcs
                                   select new saveMc()
                                   {
                                       tanggal = x.tanggal,
                                       realDate = x.realDate,
                                       customerCode = x.customerCode,
                                       amountCoin = x.amountCoin,
                                       amountTotal = x.amountTotal,
                                       d100k = x.d100k,
                                       d10k = x.d10k,
                                       d1k = x.d1k,
                                       d20k = x.d20k,
                                       d2k = x.d2k,
                                       d50k = x.d50k,
                                       d5k = x.d5k,
                                       time = x.time,
                                       cn100 = x.cn100,
                                       cn1k = x.cn1k,
                                       cn200 = x.cn200,
                                       cn25 = x.cn25,
                                       cn50 = x.cn50,
                                       cn500 = x.cn500
                                   }).ToList();

            DateTime maxdate = saveMc.Max(x => (DateTime)x.tanggal);
            DateTime mindate = saveMc.Min(x => (DateTime)x.tanggal);

            var query = (from x in en.saveMcs
                         where (DateTime)x.tanggal >= mindate && (DateTime)x.tanggal <= maxdate
                         select x).ToList();

            List<saveMc> mcsToRemove = new List<saveMc>();

            foreach (var item in saveMc)
            {
                var query2 = (from x in query
                              where x.tanggal == item.tanggal &&
                                    x.customerCode == item.customerCode &&
                                    x.time == item.time
                              select x).FirstOrDefault();

                if (query2 != null)
                {
                    query2.amountTotal = item.amountTotal;
                    query2.amountCoin = item.amountCoin;
                    query2.d100k = item.d100k;
                    query2.d10k = item.d10k;
                    query2.d1k = item.d1k;
                    query2.d20k = item.d20k;
                    query2.d2k = item.d2k;
                    query2.d50k = item.d50k;
                    query2.d5k = item.d5k;
                    query2.cn1k = item.cn1k;
                    query2.cn500 = item.cn500;
                    query2.cn200 = item.cn200;
                    query2.cn100 = item.cn100;
                    query2.cn50 = item.cn50;
                    query2.cn25 = item.cn25;
                    en.SaveChanges();
                    mcsToRemove.Add(item);
                }
            }

            foreach (var item in mcsToRemove)
            {
                saveMc.Remove(item);
            }

            en.saveMcs.AddRange(saveMc);
            en.SaveChanges();
            loadForm.CloseForm();

        }

        private void buttonProses_Click(object sender, EventArgs e)
        {
            List<HasilProcessed> hasil = new List<HasilProcessed>();

            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "select  distinct"
                                     + " [tanggalTransaksi] = ISNULL(ISNULL(sb.tanggalTransaksi, ds.tanggal), sm.tanggal),"
                                     + " [kodeNasabah] = ISNULL(ISNULL(sb.kodePerusahaan, ds.kode),sm.customerCode),"
                                     + " [nominalBeeHive] = SUM(ISNULL(sb.totalNominal, 0)),"
                                     + " [nominalMcs] = SUM(ISNULL(sm.amountTotal, 0)),"
                                     + " [nominalDailystock] = sum(ISNULL(ds.BN100K + ds.BN50K + ds.BN20K + ds.BN10K + ds.BN5K + ds.BN2K + ds.BN1K + ds.BN500 + ds.BN200 + ds.BN100 + ds.CN1K + ds.CN500 + ds.CN200 + ds.CN100 + ds.CN50 + ds.CN25, 0)),"
                                     + " [selisihBeehiveMcs] =  sum(ISNULL(sb.totalNominal, 0) - ISNULL(sm.amountTotal, 0)),"
                                     + " [selisihBeehiveDailystock] = sum(ISNULL(sb.totalNominal, 0) - ISNULL(ds.BN100K + ds.BN50K + ds.BN20K + ds.BN10K + ds.BN5K + ds.BN2K + ds.BN1K + ds.BN500 + ds.BN200 + ds.BN100 + ds.CN1K + ds.CN500 + ds.CN200 + ds.CN100 + ds.CN50 + ds.CN25, 0))"
                                     + " from("
                                     + " select tanggalTransaksi, kodePerusahaan, [totalNominal] = sum(totalNominal), jenisFile, tanggalKredit, namaFile"
                                     + " from saveBeeHive"
                                     + " where namaFile = '' and tanggalTransaksi = '" + dateTimePicker1.Value.Date + "'"
                                     + " group by tanggalTransaksi, kodePerusahaan, jenisFile, tanggalKredit, namaFile) as sb"
                                     + " full outer join("
                                     + " select tanggal, realDate, customerCode, [amountTotal] = sum(amountTotal)"
                                     + " from ("
                                     + " select distinct [tanggal] = tanggal, [realDate] = realDate, [customerCode] = customerCode, [amountTotal] = amountTotal"
                                     + " from saveMcs"
                                     + " ) as daleman"
                                     + " where tanggal = '" + dateTimePicker1.Value.Date + "'"
                                     + " group by customerCode, tanggal, realDate) as sm on sb.kodePerusahaan = sm.customerCode"
                                     + " full outer join("
                                     + " select idTransaksi, tanggal, kodePkt, [in/out], jenisTransaksi, [kode] = case when len(kode) = 7 then '0'+kode else kode end, nama, keterangan,"
                                     + " BN100K = sum(BN100K), BN50K = sum(BN50K), BN20K = sum(BN20K), BN10K = sum(BN10K), BN5K = sum(BN5K), BN2K = sum(BN2K), BN1K = sum(BN1K), BN500 = sum(BN500), BN200 = sum(BN200),"
                                     + " BN100 = sum(BN100), CN1K = sum(CN1K), CN500 = sum(CN500), CN200 = sum(CN200), CN100 = sum(CN100), CN50 = sum(CN50), CN25 = sum(CN25)"
                                     + " from DailyStock"
                                     + " where jenisTransaksi = 'Collection Retail' and tanggal = '" + dateTimePicker1.Value.Date + "' and len(kode) >= 7"
                                     + " group by idTransaksi, tanggal, kodePkt, [in/out], jenisTransaksi, case when len(kode) = 7 then '0'+kode else kode end, nama, keterangan"
                                     + " ) ds on sb.kodePerusahaan = case when len(ds.kode) = 7 then '0'+ds.kode else ds.kode end"
                                     + " group by ISNULL(ISNULL(sb.tanggalTransaksi, ds.tanggal ),sm.tanggal), ISNULL(ISNULL(sb.kodePerusahaan, ds.kode),sm.customerCode)";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        hasil.Add(new HasilProcessed
                        {
                            tanggalTransaksi = DateTime.Parse(reader[0].ToString()),
                            kodeNasabah = reader[1].ToString(),
                            nominalBeeHive = Int64.Parse(reader[2].ToString()),
                            nominalMCS = Int64.Parse(reader[3].ToString()),
                            nominalDailystock = Int64.Parse(reader[4].ToString()),
                            selisihBeehiveMcs = Int64.Parse(reader[5].ToString()),
                            selisihBeehiveDailystock = Int64.Parse(reader[6].ToString()),
                            keterangan = Int64.Parse(reader[5].ToString()) == 0 && Int64.Parse(reader[6].ToString()) == 0 ? "SAMA" : "TIDAK SAMA"
                        });
                    }
                    dataGridView1.DataSource = hasil;
                }
            }

            //var join1 = (from left in en.saveBeeHives
            //             join right in en.saveMcs on left.kodePerusahaan equals right.customerCode into temp
            //             where left.namaFile == ""
            //             from right in temp.DefaultIfEmpty()
            //             select new {
            //                 tanggalMcs = right.tanggal,
            //                 tanggalBeeHive = left.tanggalTransaksi,
            //                 kodeNasabah = right.customerCode,
            //                 nominalBeehive = left.totalNominal,
            //                 nominalMcs = right.amountTotal,
            //                 selisihBeehiveMcs = left.totalNominal - right.amountTotal,
            //             }).ToList();
            //var join2 = (from right in en.saveMcs
            //             join left in en.saveBeeHives on right.customerCode equals left.kodePerusahaan into temp
            //             from left in temp.Where(x => x.namaFile == "").DefaultIfEmpty()
            //             select new {
            //                 tanggalMcs = right.tanggal,
            //                 tanggalBeeHive = left.tanggalTransaksi,
            //                 kodeNasabah = left.kodePerusahaan,
            //                 nominalBeehive = left.totalNominal,
            //                 nominalMcs = right.amountTotal,
            //                 selisihBeehiveMcs = left.totalNominal - right.amountTotal,
            //             }).ToList();
            //var join12 = join1.Union(join2).Distinct().ToList();

            //var join3 = (from left in join12
            //             join right in en.DailyStocks on left.kodeNasabah equals (right.kode.Length == 7 ? "0" + right.kode : right.kode) into temp
            //             from right in temp.Where(x => x.jenisTransaksi == "Collection Retail").DefaultIfEmpty()
            //             select new
            //             {
            //                 tanggalMcs = left?.tanggalMcs,
            //                 tanggalBeeHive = left?.tanggalBeeHive,
            //                 tanggalDailystock = right?.tanggal,
            //                 kodeNasabah = left?.kodeNasabah,
            //                 nominalBeehive = left?.nominalBeehive,
            //                 nominalMcs = left?.nominalMcs,
            //                 nominalDailystock = right?.BN100K + right?.BN50K + right?.BN20K + right?.BN10K + right?.BN5K + right?.BN2K + right?.BN1K + right?.BN500 + right?.BN200 + right?.BN100 + right?.CN1K + right?.CN500 + right?.CN200 + right?.CN100 + right?.CN50 + right?.CN25,
            //                 selisihBeehiveMcs = left?.selisihBeehiveMcs,
            //                 selisihBeehiveDailystock = left?.nominalBeehive - (right?.BN100K + right?.BN50K + right?.BN20K + right?.BN10K + right?.BN5K + right?.BN2K + right?.BN1K + right?.BN500 + right?.BN200 + right?.BN100 + right?.CN1K + right?.CN500 + right?.CN200 + right?.CN100 + right?.CN50 + right?.CN25)
            //             }).ToList();

            //var join4 = (from right in en.DailyStocks
            //             join left in join12 on (right.kode.Length == 7 ? "0" + right.kode : right.kode) equals left.kodeNasabah into temp
            //             where right.jenisTransaksi == "Collection Retail"
            //             from left in temp.DefaultIfEmpty()
            //             select new
            //             {
            //                 tanggalMcs = left.tanggalMcs,
            //                 tanggalBeeHive = left.tanggalBeeHive,
            //                 tanggalDailystock = right.tanggal,
            //                 kodeNasabah = (right.kode.Length == 7 ? "0" + right.kode : right.kode),
            //                 nominalBeehive = left.nominalBeehive,
            //                 nominalMcs = left.nominalMcs,
            //                 nominalDailystock = right.BN100K + right.BN50K + right.BN20K + right.BN10K + right.BN5K + right.BN2K + right.BN1K + right.BN500 + right.BN200 + right.BN100 + right.CN1K + right.CN500 + right.CN200 + right.CN100 + right.CN50 + right.CN25,
            //                 selisihBeehiveMcs = left.selisihBeehiveMcs,
            //                 selisihBeehiveDailystock = left.nominalBeehive - (right.BN100K + right.BN50K + right.BN20K + right.BN10K + right.BN5K + right.BN2K + right.BN1K + right.BN500 + right.BN200 + right.BN100 + right.CN1K + right.CN500 + right.CN200 + right.CN100 + right.CN50 + right.CN25)
            //             }).ToList();

            //var join34 = join3.Union(join4).Distinct().ToList();



            //var query = (from x in join34
            //             //join z in en.DailyStocks on y.customerCode equals  "0" + z.kode 
            //             //join z in en.DailyStocks on y.customerCode equals (z.kode.Length < 8 ? "0" + z.kode : z.kode)
            //             where x.tanggalMcs == dateTimePicker1.Value.Date && x.tanggalDailystock == dateTimePicker1.Value.Date && x.tanggalBeeHive == dateTimePicker1.Value.Date 
            //             select new
            //             {
            //                 tanggalTransaksi = x.tanggalBeeHive,
            //                 kodeNasabah = x.kodeNasabah,
            //                 nominalBeeHive = x.nominalBeehive,
            //                 nominalMcs = x.nominalMcs,
            //                 nominalDailyStock = x.nominalDailystock,
            //                 selisihBeeHiveMcs = x.selisihBeehiveMcs,
            //                 selisihBeeHiveDailyStock = x.selisihBeehiveDailystock
            //             }).ToList();

            //var query2 = query.GroupBy(x => new { x.tanggalTransaksi, x.kodeNasabah }).Select(z => new
            //{
            //    tanggalTransaksi = z.Key.tanggalTransaksi,
            //    kodeNasabah = z.Key.kodeNasabah,
            //    nominalBeehive = z.Sum(x => x.nominalBeeHive),
            //    nominalMcs = z.Sum(x => x.nominalMcs),
            //    nominalDailyStock = z.Sum(x => x.nominalDailyStock),
            //    selisihBeeHiveMcs = z.Sum(x => x.selisihBeeHiveMcs),
            //    selisihBeeHiveDailyStock = z.Sum(x => x.selisihBeeHiveDailyStock)
            //}).ToList();

            //var query3 = query2.Select(z => new
            //{
            //    tanggalTransaksi = z.tanggalTransaksi,
            //    kodeNasabah = z.kodeNasabah,
            //    nominalBeehive = z.nominalBeehive,
            //    nominalMcs = z.nominalMcs,
            //    nominalDailyStock = z.nominalDailyStock,
            //    selisihBeehiveMcs = z.selisihBeeHiveMcs,
            //    selisihBeehiveDailystock = z.selisihBeeHiveDailyStock,
            //    keterangan = z.nominalBeehive == z.nominalMcs && z.nominalMcs == z.nominalDailyStock ? "SAMA" : "TIDAK SAMA"
            //}).ToList();

            //dataGridView1.DataSource = query3;
            if (dataGridView1.Rows.Count > 0)
            {
                for (int i = 2; i < 7; i++)
                {
                    dataGridView1.Columns[i].DefaultCellStyle.Format = "c";
                    dataGridView1.Columns[i].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("ID-id");
                }
            }
        }

        private void buttonShowRusak_Click(object sender, EventArgs e)
        {
            var query = (from x in en.saveBeeHives
                         where x.tanggalTransaksi == dateTimePicker1.Value.Date && x.namaFile != ""
                         select new
                         {
                             //tanggalTransaksi = x.tanggalTransaksi,
                             tanggalKredit = x.tanggalKredit,
                             kodePerusahaan = x.kodePerusahaan,
                             totalNominal = x.totalNominal,
                             namaFile = x.namaFile
                         }).ToList();

            dataGridView1.DataSource = query;

            //dataGridView1.Columns[3].DefaultCellStyle.Format = "c";
            //dataGridView1.Columns[3].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("ID-id");

        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            DataGridViewSelectedCellCollection cells = dataGridView1.SelectedCells;
            foreach (DataGridViewCell cell in cells)
            {
                int rowidx = cell.RowIndex;
                int colidx = cell.ColumnIndex;
                if (colidx > 0)
                {
                    dataGridView1.Rows[rowidx].Cells[colidx].Style.Format = "F0";
                }

            }
            for (int a = 0; a < dataGridView1.Rows.Count; a++)
            {
                for (int b = 1; b < dataGridView1.Columns.Count; b++)
                {
                    if (!cells.Contains(dataGridView1.Rows[a].Cells[b]))
                    {

                        dataGridView1.Rows[a].Cells[b].Style.Format = "C0";
                        dataGridView1.Rows[a].Cells[b].Style.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
                    }
                }
            }
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog sv = new SaveFileDialog();
            sv.Filter = Variables.csvFilter;
            if (sv.ShowDialog() == DialogResult.OK)
            {
                loadForm.ShowSplashScreen();
                SaveDataGridViewToCSV(sv.FileName);
                loadForm.CloseForm();
            }
        }

        void SaveDataGridViewToCSV(string filename)
        {
            // Choose whether to write header. Use EnableWithoutHeaderText instead to omit header.
            dataGridView1.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            // Select all the cells
            dataGridView1.SelectAll();
            // Copy selected cells to DataObject
            //for (int a = 0; a < dataGridView1.Columns.Count; a++)
            //{
            //    dataGridView1.Columns[a].DefaultCellStyle.Format = "";
            //    //for(int b=0;b<dataGridView1.Rows.Count;b++)
            //    //{
            //    //    dataGridView1.Rows[b].Cells[a].Value = (Int64)dataGridView1.Rows[b].Cells[a].Value;
            //    //}
            //}

            DataObject dataObject = dataGridView1.GetClipboardContent();
            // Get the text of the DataObject, and serialize it to a file
            File.WriteAllText(filename, dataObject.GetText(TextDataFormat.CommaSeparatedValue));
            //for (int a = 4; a < dataGridView1.Columns.Count; a++)
            //{
            //    dataGridView1.Columns[a].DefaultCellStyle.Format = "C";
            //    //for(int b=0;b<dataGridView1.Rows.Count;b++)
            //    //{
            //    //    dataGridView1.Rows[b].Cells[a].Value = (Int64)dataGridView1.Rows[b].Cells[a].Value;
            //    //}
            //}
        }
    }
    public class BeeHive
    {
        public DateTime tanggalTransaksi { set; get; }
        public DateTime tanggalKredit { set; get; }
        public String kodePerusahaan { set; get; }
        public Int64 totalNominal { set; get; }
        public String jenisFile { set; get; }
        public String message { set; get; }

    }
    public class Va
    {
        public DateTime tanggalTransaksi { set; get; }
        public DateTime tanggalKredit { set; get; }
        public String kodePerusahaan { set; get; }
        public Int64 totalNominal { set; get; }
        public String jenisFile { set; get; }
    }

    public class ProsesBeeHive
    {
        public DateTime tanggalTransaksi { set; get; }
        public DateTime tanggalKredit { set; get; }
        public String kodePerusahaan { set; get; }
        public Int64 totalNominal { set; get; }
        public String namaFile { set; get; }
        public String jenisFile { set; get; }

    }


    public class ProsesVa
    {
        public DateTime tanggalTransaksi { set; get; }
        public DateTime tanggalKredit { set; get; }
        public String kodePerusahaan { set; get; }
        public Int64 totalNominal { set; get; }
        public String namaFile { set; get; }
        public String jenisFile { set; get; }
    }


    public class Mcs
    {
        public DateTime tanggal { set; get; }
        public DateTime realDate { set; get; }
        public String kodePkt { set; get; }
        public TimeSpan time { set; get; }
        public String customerCode { set; get; }
        public String scheduled { set; get; }
        public String serviceType { set; get; }
        public Int64 amountTotal { set; get; }
        public Int64 d100k { set; get; }
        public Int64 d50k { set; get; }
        public Int64 d20k { set; get; }
        public Int64 d10k { set; get; }
        public Int64 d5k { set; get; }
        public Int64 d2k { set; get; }
        public Int64 d1k { set; get; }
        public Int64 amountCoin { set; get; }
        public Int64 cn1k { set; get; }
        public Int64 cn500 { set; get; }
        public Int64 cn200 { set; get; }
        public Int64 cn100 { set; get; }
        public Int64 cn50 { set; get; }
        public Int64 cn25 { set; get; }
    }

    public class HasilProcessed
    {
        public DateTime tanggalTransaksi { set; get; }
        public String kodeNasabah { set; get; }
        public Int64 nominalBeeHive { set; get; }
        public Int64 nominalMCS { set; get; }
        public Int64 nominalDailystock { set; get; }
        public Int64 selisihBeehiveMcs { set; get; }
        public Int64 selisihBeehiveDailystock { set; get; }
        public String keterangan { set; get; }

    }
}