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
        List<Mcs> mcs = new List<Mcs>();


        public ReadBeehiveForm()
        {
            InitializeComponent();
        }

        private void inputBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = Variables.txtFilter;
            if(of.ShowDialog() == DialogResult.OK)
            {
                listDataString = new List<String>();
                using (StreamReader sr = File.OpenText(of.FileName))
                {
                    String s = "", stringToAdd = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        if(s.Contains("LAPORAN DAFTAR TRANSAKSI"))
                        {

                            listDataString.Add(stringToAdd);
                            stringToAdd = "";
                        }
                        stringToAdd += s + "\n";
                    }
                }
                listDataString.Remove("");
            }
            prosesString();
        }
        void prosesString()
        {
            listHasil = new List<BeeHive>();
            foreach (String temp in listDataString)
            {
                List<String> listUntukProses = temp.Split('\n').ToList();
                listUntukProses.RemoveRange(0,6);
                List<String> toDelete = listUntukProses.Where(x => String.IsNullOrEmpty(x)).ToList();
                foreach (var tempDel in toDelete)
                    listUntukProses.Remove(tempDel);
                for (int a=0;a<listUntukProses.Count;a++)
                {
                    if(a%2==1)
                    {
                        String temp2 = listUntukProses[a];
                        DateTime tanggal = new DateTime(1,1,1), bufTanggal;
                        String kodePerusahaan = temp2.Substring(21,8);
                        Int64 totalNominal=0, bufTotalNominal;
                        String formatTanggal = "dd-MM-yyyy";
                        String tanggalS = temp2.Substring(7, 10),totalNominalS = temp2.Substring(77,32).TrimStart().Replace(".00","").Replace(",","");
                        String namaFile = temp2.Substring(32, 25).Trim(' ');


                        if (DateTime.TryParseExact(tanggalS, formatTanggal, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out bufTanggal))
                            tanggal = bufTanggal;
                        if (Int64.TryParse(totalNominalS, out bufTotalNominal))
                            totalNominal = bufTotalNominal;

                        listHasil.Add(new BeeHive() {
                            tanggal = tanggal,
                            kodePerusahaan = kodePerusahaan,
                            totalNominal = totalNominal,
                            namaFIle= namaFile
                        });
                    }
                }
            }
            dataGridView1.DataSource = listHasil;
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
                dataGridView2.DataSource = mcs;

            }
        }

        private void buttonSaveBeehive_Click(object sender, EventArgs e)
        {
            loadForm.ShowSplashScreen();
            List<saveBeeHive> saveBeeHive = (from x in listHasil
                                             select new saveBeeHive()
                                             {
                                                 kodePerusahaan = x.kodePerusahaan,
                                                 tanggal = x.tanggal,
                                                 totalNominal = x.totalNominal,
                                                 namaFile = x.namaFIle
                                             }).ToList();

            DateTime maxdate = saveBeeHive.Max(x => (DateTime)x.tanggal);
            DateTime mindate = saveBeeHive.Min(x => (DateTime)x.tanggal);

            var query = (from x in en.saveBeeHives
                         where (DateTime)x.tanggal >= mindate && (DateTime)x.tanggal <= maxdate
                         select x).ToList();

            List<saveBeeHive> bhToRemove = new List<saveBeeHive>();

            foreach (var item in saveBeeHive)
            {
                var query2 = (from x in query
                              where x.tanggal == item.tanggal && 
                                    x.kodePerusahaan == item.kodePerusahaan &&
                                    x.namaFile == item.namaFile
                              select x).FirstOrDefault();

                if (query2 != null)
                {
                    query2.totalNominal = item.totalNominal;
                    en.SaveChanges();
                    bhToRemove.Add(item);
                }
                Console.WriteLine("hap");
            }

            foreach (var item in bhToRemove)
            {
                saveBeeHive.Remove(item);
                Console.WriteLine("hup");
            }

            Console.WriteLine("keluar");
            en.saveBeeHives.AddRange(saveBeeHive);
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
           
        }

       
    }
    public class BeeHive
    {
        public DateTime tanggal { set; get; }
        public String kodePerusahaan { set; get; }
        public Int64 totalNominal { set; get; }
        public String namaFIle { set; get; }
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

    public class Processed
    {
        public DateTime realDate { set; get; }
        public Int64 nominalBeeHive { set; get; }
        public Int64 nominalMCS { set; get; }
        public String keterangan { set; get; }
    }



}
