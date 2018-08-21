using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
    }
    public class BeeHive
    {
        public DateTime tanggal { set; get; }
        public String kodePerusahaan { set; get; }
        public Int64 totalNominal { set; get; }
    }
}
