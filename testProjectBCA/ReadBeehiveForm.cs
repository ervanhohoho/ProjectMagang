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
        List<String> listDataString = new List<String>();
        List<BeeHive> listHasil = new List<BeeHive>();
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


                        if (DateTime.TryParseExact(tanggalS, formatTanggal, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out bufTanggal))
                            tanggal = bufTanggal;
                        if (Int64.TryParse(totalNominalS, out bufTotalNominal))
                            totalNominal = bufTotalNominal;

                        listHasil.Add(new BeeHive() {
                            tanggal = tanggal,
                            kodePerusahaan = kodePerusahaan,
                            totalNominal = totalNominal
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

            List<Mcs> mcs = new List<Mcs>();

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
                dataGridView1.DataSource = dtCloned;

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
                            time = DateTime.Parse(dtCloned.Rows[i][1].ToString()),
                            customerCode = dtCloned.Rows[i][3].ToString().Trim('\''),
                            amountTotal = Int64.Parse(dtCloned.Rows[i][5].ToString()),
                            d100k = Int64.Parse(dtCloned.Rows[i][6].ToString()),
                            d50k = Int64.Parse(dtCloned.Rows[i][7].ToString()),
                            d20k = Int64.Parse(dtCloned.Rows[i][8].ToString()),
                            d10k = Int64.Parse(dtCloned.Rows[i][9].ToString()),
                            d5k = Int64.Parse(dtCloned.Rows[i][10].ToString()),
                            d2k = Int64.Parse(dtCloned.Rows[i][11].ToString()),
                            d1k = Int64.Parse(dtCloned.Rows[i][12].ToString()),
                            amountCoin = Int64.Parse(dtCloned.Rows[i][13].ToString()),
                        });
                    }

                }
                dataGridView1.DataSource = mcs;

            }
        }
    }
    public class BeeHive
    {
        public DateTime tanggal { set; get; }
        public String kodePerusahaan { set; get; }
        public Int64 totalNominal { set; get; }
    }

    public class Mcs
    {
        public DateTime tanggal { set; get; }
        public String kodePkt { set; get; }
        public DateTime time { set; get; }
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
}
