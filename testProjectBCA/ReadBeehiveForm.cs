using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
                loadForm.ShowSplashScreen();
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
                loadForm.CloseForm();
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
                //dataGridView1.DataSource = dtCloned;

                for (int i = 0; i < dtCloned.Rows.Count; i++)
                {
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
                        Console.WriteLine(temp);
                        mcs.Add(new Mcs
                        {
                            tanggal = DateTime.ParseExact(tanggal, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                            kodePkt = kodePkt,
                            time = DateTime.Parse(dtCloned.Rows[i][1].ToString()).TimeOfDay,
                            realDate = DateTime.Parse(dtCloned.Rows[i][1].ToString()).Hour >= 17 ? DateTime.ParseExact(tanggal, "dd/MM/yyyy", CultureInfo.InvariantCulture).AddDays(1).Date : DateTime.ParseExact(tanggal, "dd/MM/yyyy", CultureInfo.InvariantCulture).Date,
                            customerCode = dtCloned.Rows[i][3].ToString().Trim('\''),
                            amountTotal = Int64.Parse(dtCloned.Rows[i][5].ToString()),
                            d100k = Int64.Parse(dtCloned.Rows[i][6].ToString()),
                            d50k = Int64.Parse(dtCloned.Rows[i][7].ToString()),
                            d20k = Int64.Parse(dtCloned.Rows[i][8].ToString()),
                            d10k = Int64.Parse(dtCloned.Rows[i][9].ToString()),
                            d5k = Int64.Parse(dtCloned.Rows[i][10].ToString()),
                            d2k = Int64.Parse(dtCloned.Rows[i][11].ToString()),
                            d1k = Int64.Parse(dtCloned.Rows[i][12].ToString()),
                            amountCoin = Int64.Parse(dtCloned.Rows[i][13].ToString())
                        });
                    }

                }
                //dataGridView2.DataSource = mcs;
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

                var query = (from x in en.saveBeeHives
                             where x.tanggalTransaksi >= mindate && x.tanggalTransaksi <= maxdate && x.jenisFile == "BEEHIVE"
                             select x).ToList();

                if (query.Count > 0)
                {
                    en.saveBeeHives.RemoveRange(query);
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
                                       time = x.time
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
            var query = (from x in en.saveBeeHives
                         join y in en.saveMcs on x.kodePerusahaan equals y.customerCode
                         join z in en.DailyStocks on y.customerCode equals "0" + z.kode
                         where x.tanggalTransaksi == dateTimePicker1.Value.Date && y.tanggal == dateTimePicker1.Value.Date && z.tanggal == dateTimePicker1.Value.Date && x.namaFile == ""
                         select new
                         {
                             tanggalTransaksi = y.tanggal,
                             kodeNasabah = y.customerCode,
                             nominalBeeHive = x.totalNominal,
                             nominalMcs = y.amountTotal,
                             nominalDailyStock = z.BN100K + z.BN50K + z.BN20K + z.BN10K + z.BN5K + z.BN2K + z.BN1K + z.BN500 + z.BN200 + z.BN100 + z.CN1K + z.CN500 + z.CN200 + z.CN100 + z.CN50 + z.CN25,
                             selisihBeeHiveMcs = x.totalNominal - y.amountTotal,
                             selisihBeeHiveDailyStock = x.totalNominal - (z.BN100K + z.BN50K + z.BN20K + z.BN10K + z.BN5K + z.BN2K + z.BN1K + z.BN500 + z.BN200 + z.CN1K + z.BN100 + z.CN500 + z.CN200 + z.CN100 + z.CN50 + z.CN25),
                             keterangan = (
                                x.totalNominal == y.amountTotal && y.amountTotal == z.BN100K + z.BN50K + z.BN20K + z.BN10K + z.BN5K + z.BN2K + z.BN1K + z.BN500 + z.BN200 + z.BN100 + z.CN1K + z.CN500 + z.CN200 + z.CN100 + z.CN50 + z.CN25 ? "SAMA" : "TIDAK SAMA"
                             )
                         }).ToList();

            dataGridView1.DataSource = query;
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
        public Int64 amountTotal { set; get; }
        public Int64 d100k { set; get; }
        public Int64 d50k { set; get; }
        public Int64 d20k { set; get; }
        public Int64 d10k { set; get; }
        public Int64 d5k { set; get; }
        public Int64 d2k { set; get; }
        public Int64 d1k { set; get; }
        public Int64 amountCoin { set; get; }

    }

    public class HasilProcessed
    {
        public DateTime realDate { set; get; }
        public String kodeNasabah { set; get; }
        public Int64 nominalBeeHive { set; get; }
        public Int64 nominalMCS { set; get; }
        public String keterangan { set; get; }
    }
}