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

namespace testProjectBCA.CabangMenu
{
    public partial class ProsesWPForm : Form
    {
        List<List<SumberData>> finalData;
        List<DataProses> result;
        String sheetNameSearch = "";
        Int64 limitDelivery = 30000000000;

        String keteranganKirim = "KIRIM KE %BANK%";
        String keteranganGeserVault = "GESER VAULT";
        String keteranganKirimVault = "KIRIM KE %NAMAPKT%, VAULT %BANK%";

        public ProsesWPForm()
        {
            InitializeComponent();
            sheetNameSearch = DateTime.Today.ToString("ddMMyyyy");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = Variables.excelFilter;
            if(of.ShowDialog() == DialogResult.OK)
            {
                loadForm.ShowSplashScreen();
                String path = of.FileName;
                DataSet ds = Util.openExcel(path);
                if (ds.Tables.Count == 0)
                {
                    loadForm.CloseForm();
                    return;
                }
                DataTable dt = new DataTable();
                int sheetindex = -1;
                for (int a = 0; a < ds.Tables.Count; a++)
                {
                    if (ds.Tables[a].TableName == sheetNameSearch)
                    {
                        sheetindex = a;
                        dt = ds.Tables[a];
                        break;
                    }
                }
                if(sheetindex == -1)
                {
                    MessageBox.Show("Sheet " + sheetNameSearch + " Tidak ada di file " + path);
                    return;
                }

                var data = (from DataRow x in dt.Rows
                            select new SumberData(){
                                namaBank = x[0].ToString(),
                                sumberDana = x[1].ToString(),
                                tujuan = x[2].ToString(),
                                kodeTujuan = x[3].ToString(),
                                d100 = convertToInt64(x[4].ToString()),
                                d50 = convertToInt64(x[5].ToString()),
                                d20 = convertToInt64(x[6].ToString()),
                                d10 = convertToInt64(x[7].ToString()),
                                d5 = convertToInt64(x[8].ToString()),
                                d2 = convertToInt64(x[9].ToString()),
                            }).ToList();
                var summaryRow = data.Where(x => x.namaBank == "Summary").FirstOrDefault();
                int summaryRowNumber = data.IndexOf(summaryRow);
                data.RemoveRange(0, summaryRowNumber + 2);

                Int64 buf;
                data = data.Where(x => 
                (x.d100 != 0 
                || x.d50 != 0 
                || x.d20 != 0 
                || x.d10 != 0 
                || x.d5 != 0 
                || x.d2 != 0)
                && !Int64.TryParse(x.sumberDana, out buf)).ToList();

                var toEdit = (from x in data
                            where x.namaBank == "0" && x.sumberDana.Contains("01 -")
                            select x
                            ).ToList();

                foreach (var tempToEdit in toEdit)
                    tempToEdit.namaBank = "NONE";

                {
                    var tempToDelete = data.Where(x => x.namaBank == "NONE").FirstOrDefault();
                    int idxStartDelete = data.IndexOf(tempToDelete);
                    if (idxStartDelete != -1)
                    {
                        Console.WriteLine("Index Start Delete: " + idxStartDelete);
                        Console.WriteLine("Data Count: " + data.Count);
                        Console.WriteLine("Count - Idx: " + (data.Count - idxStartDelete));
                        data.RemoveRange(idxStartDelete, data.Count - idxStartDelete);
                    }
                }

                finalData = new List<List<SumberData>>();
                List<SumberData> tempFinalData = new List<SumberData>();
                String tempNamaBank="";
                bool first = true;
                foreach (var temp in data)
                {
                    if (tempNamaBank != temp.namaBank)
                    {
                        Console.WriteLine("Add New List!");
                        if (!first)
                        {
                            finalData.Add(tempFinalData);
                        }
                        tempFinalData = new List<SumberData>();
                        tempFinalData.Add(temp);
                        tempNamaBank = temp.namaBank;
                        first = false;
                    }
                    else
                    {
                        Console.WriteLine("Add!");
                        tempFinalData.Add(temp);
                    }
                    if (temp == data[data.Count - 1])
                        finalData.Add(tempFinalData);
                }
                Console.WriteLine("Final Data Count: " + finalData.Count);
                dataGridView1.DataSource = finalData[0];
                loadForm.CloseForm();
            }
        }
        Int64? convertToInt64(String inp)
        {
            Int64 ret = 0;
            if (Int64.TryParse(inp, out ret))
                return ret;
            else
                return 0;
        }
        

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            SaveFileDialog sv = new SaveFileDialog();
            sv.Filter = Variables.csvFilter;
            if (sv.ShowDialog() == DialogResult.OK)
            {
                bool success = true;
                try
                {
                    File.WriteAllText(sv.FileName, "");
                }
                catch
                {
                    success = false;
                    MessageBox.Show("File " + sv.FileName + " sedang dibuka atau digunakan proses lain.");
                }
                if(success)
                {
                    loadForm.ShowSplashScreen();
                    Database1Entities db = new Database1Entities();
                    var allPkt = db.Pkts.ToList();
                    result = new List<DataProses>();
                    foreach (var temp in finalData)
                    {
                        foreach (var temp2 in temp)
                        {
                            String vault = temp2.namaBank.Split('-')[1].Trim(),
                                namaBank = temp2.namaBank.Split('-')[0].Trim();
                            String kodeSumber = temp2.sumberDana.Trim(),
                                kodeTujuan = temp2.kodeTujuan.Trim();
                            if (kodeSumber.Contains("-"))
                                kodeSumber = kodeSumber.Split('-')[1].Trim();
                            else
                                kodeSumber = "Kirim";
                            if (kodeTujuan.Contains("-"))
                                kodeTujuan = kodeTujuan.Split('-')[1].Trim();
                            String vendorSumber = allPkt.Where(x => x.kodePktCabang == kodeSumber).Select(x => x.vendor).FirstOrDefault(),
                                vendorTujuan = allPkt.Where(x => x.kodePktCabang == kodeTujuan).Select(x => x.vendor).FirstOrDefault();
                            String namaSumber = allPkt.Where(x => x.kodePktCabang == kodeSumber).Select(x => x.namaPkt).FirstOrDefault(),
                                namaTujuan = allPkt.Where(x => x.kodePktCabang == kodeTujuan).Select(x => x.namaPkt).FirstOrDefault();
                            if (String.IsNullOrWhiteSpace(namaTujuan))
                                namaTujuan = vault;

                            Console.WriteLine("Kode Tujuan: " + kodeTujuan);
                            Console.WriteLine("Nama Tujuan: " + namaTujuan);
                            Console.WriteLine("Vendor Sumber: " + kodeSumber + " " + vendorSumber);
                            Console.WriteLine("Vendor Tujuan: " + kodeTujuan + " " + vendorTujuan);
                            Console.WriteLine("Value 100: " + temp2.d100);
                            Console.WriteLine("Value 50: " + temp2.d50);
                            Console.WriteLine("Value 20: " + temp2.d20);

                            if (String.IsNullOrEmpty(kodeTujuan) && vendorSumber != allPkt.Where(x => x.kodePkt == "ADJK").Select(x => x.vendor).First())
                            {
                                prosesRow(temp2, keteranganKirim.Replace("%BANK%", namaBank));
                            }
                            else if (vendorSumber == allPkt.Where(x => x.kodePkt == "ADJK").Select(x => x.vendor).First())
                            {
                                DataProses tempResult = new DataProses();
                                tempResult.vendorSumber = vendorSumber;
                                tempResult.sumberDana = namaSumber;
                                tempResult.tujuan = namaBank.Trim();
                                tempResult.namaTujuan = namaTujuan.Trim();
                                tempResult.denom100 = (Int64)temp2.d100;
                                tempResult.denom50 = (Int64)temp2.d50;
                                tempResult.denom20 = (Int64)temp2.d20;
                                tempResult.denom10 = (Int64)temp2.d10;
                                tempResult.denom5 = (Int64)temp2.d5;
                                tempResult.denom2 = (Int64)temp2.d2;
                                tempResult.total = tempResult.denom100 + tempResult.denom50 + tempResult.denom20 + tempResult.denom10 + tempResult.denom5 + tempResult.denom2;

                                if (String.IsNullOrEmpty(kodeTujuan))
                                    tempResult.keterangan = keteranganKirim.Replace("%BANK%", namaBank);
                                else
                                {
                                    String namaPktTujuan = allPkt.Where(x => x.kodePktCabang == kodeTujuan).Select(x => x.namaPkt).FirstOrDefault();
                                    tempResult.keterangan = keteranganKirimVault.Replace("%BANK%", namaBank).Replace("%NAMAPKT%", namaPktTujuan);
                                }
                                result.Add(tempResult);
                            }
                            else if (vendorSumber == vendorTujuan)
                            {
                                Console.WriteLine("Nama Tujuan Vendor Sama: " + namaTujuan);
                                DataProses tempResult = new DataProses();
                                tempResult.vendorSumber = vendorSumber;
                                tempResult.sumberDana = namaSumber;
                                tempResult.tujuan = namaBank.Trim();
                                tempResult.namaTujuan = namaTujuan;
                                tempResult.keterangan = keteranganGeserVault;
                                tempResult.denom100 = (Int64)temp2.d100;
                                tempResult.denom50 = (Int64)temp2.d50;
                                tempResult.denom20 = (Int64)temp2.d20;
                                tempResult.denom10 = (Int64)temp2.d10;
                                tempResult.denom5 = (Int64)temp2.d5;
                                tempResult.denom2 = (Int64)temp2.d2;
                                tempResult.total = tempResult.denom100 + tempResult.denom50 + tempResult.denom20 + tempResult.denom10 + tempResult.denom5 + tempResult.denom2;
                                result.Add(tempResult);
                            }
                            else if (vendorSumber != vendorTujuan)
                            {
                                String namaPktTujuan = allPkt.Where(x => x.kodePktCabang == kodeTujuan).Select(x => x.namaPkt).FirstOrDefault();
                                prosesRow(temp2, keteranganKirimVault.Replace("%BANK%", namaBank).Replace("%NAMAPKT%", namaPktTujuan));
                            }
                        }
                    }
                    int counter = 1;
                    var resultGrouped = (from x in result
                                     group x by new { Gabung = (x.keterangan.ToLower() == "geser vault" ? 0 : counter++), x.tujuan, x.vendorSumber } into g
                                     select new
                                     {
                                         sumberDana = (g.ToList().Count > 1 || g.Key.Gabung ==  0 ? g.Select(y => y.namaTujuan).First() : g.Select(y => y.sumberDana).First()),
                                         tujuan = g.Select(y => y.tujuan).First().Trim(),
                                         denom100 = g.Sum(y => y.denom100),
                                         denom50 = g.Sum(y => y.denom50),
                                         denom20 = g.Sum(y => y.denom20),
                                         denom10 = g.Sum(y => y.denom10),
                                         denom5 = g.Sum(y => y.denom5),
                                         denom2 = g.Sum(y => y.denom2),
                                         total = g.Sum(y => y.total),
                                         keterangan = g.Select(y => y.keterangan).First(),
                                     }).ToList();
                    dataGridView1.DataSource = result;
                    String csv = ServiceStack.Text.CsvSerializer.SerializeToCsv(resultGrouped);
                    File.WriteAllText(sv.FileName, csv);
                    loadForm.CloseForm();
                    MessageBox.Show("DONE!");
                }
            }

        }
        void prosesRow(SumberData sumber, String keterangan)
        {
            Database1Entities db = new Database1Entities();
            var allPkt = db.Pkts.ToList();
            Int64 d100 = (Int64)sumber.d100,
                            d50 = (Int64)sumber.d50,
                            d20 = (Int64)sumber.d20,
                            d10 = (Int64)sumber.d10,
                            d5 = (Int64)sumber.d5,
                            d2 = (Int64)sumber.d2;
            Int64 totalValue = (Int64)(d100 + d50 + d20 + d10 + d5 + d2);
            DataProses tempResult = new DataProses();
            String vault = sumber.namaBank.Split('-')[0];

            if (sumber.sumberDana.Split('-')[1].Trim() == "CCAS")
                sumber.sumberDana = sumber.sumberDana.Replace("CCAS", "CCASA");

            Console.WriteLine("Masuk Proses Row");
            //Console.WriteLine(sumber.sumberDana);
            String kodePktCabangSumber = sumber.sumberDana.Split('-')[1].Trim();
            String sumberDana = allPkt.Where(x=>x.kodePktCabang == kodePktCabangSumber).Select(x=>x.namaPkt).FirstOrDefault(),
                vendorSumber = allPkt.Where(x=>x.kodePkt == kodePktCabangSumber).Select(x=>x.vendor).FirstOrDefault(),
                kodeTujuan = sumber.kodeTujuan.Trim();
            if (kodeTujuan.Contains("-"))
                kodeTujuan = kodeTujuan.Split('-')[1].Trim();

            String namaTujuan = allPkt.Where(x => x.kodePktCabang == kodeTujuan).Select(x => x.namaPkt).FirstOrDefault();
            //Console.WriteLine(sumberDana);
            tempResult.namaTujuan = namaTujuan;
            tempResult.sumberDana = sumberDana;
            tempResult.tujuan = vault;
            tempResult.keterangan = keterangan;
            tempResult.vendorSumber = vendorSumber;
            Int64 sisa = limitDelivery;

            //100K
            if (d100 > sisa)
            {
                while (d100 > sisa)
                {
                    tempResult.denom100 = sisa;
                    d100 -= sisa;
                    tempResult.total = tempResult.denom100 + tempResult.denom50 + tempResult.denom20 + tempResult.denom10 + tempResult.denom5 + tempResult.denom2;
                    result.Add(tempResult);
                    sisa = limitDelivery;
                    tempResult = new DataProses();
                    tempResult.sumberDana = sumberDana;
                    tempResult.tujuan = vault;
                    tempResult.keterangan = keterangan;
                }
            }
            if (d100 > 0)
            {
                tempResult.denom100 = d100;
                sisa -= d100;
            }
            //50K
            if (d50 > sisa)
            {
                while (d50 > sisa)
                {
                    tempResult.denom50 = sisa;
                    d50 -= sisa;
                    tempResult.total = tempResult.denom100 + tempResult.denom50 + tempResult.denom20 + tempResult.denom10 + tempResult.denom5 + tempResult.denom2;
                    result.Add(tempResult);
                    sisa = limitDelivery;
                    tempResult = new DataProses();
                    tempResult.sumberDana = sumberDana;
                    tempResult.tujuan = vault;
                    tempResult.keterangan = keterangan;
                }
            }
            if (d50 > 0)
            {
                tempResult.denom50 = d50;
                sisa -= d50;
            }
            //20K
            if (d20 > sisa)
            {
                while (d20 > sisa)
                {
                    tempResult.denom20 = sisa;
                    d20 -= sisa;
                    tempResult.total = tempResult.denom100 + tempResult.denom50 + tempResult.denom20 + tempResult.denom10 + tempResult.denom5 + tempResult.denom2;
                    result.Add(tempResult);
                    sisa = limitDelivery;
                    tempResult = new DataProses();
                    tempResult.sumberDana = sumberDana;
                    tempResult.tujuan = vault;
                    tempResult.keterangan = keterangan;
                }
            }
            if (d20 > 0)
            {
                tempResult.denom20 = d20;
                sisa -= d20;
            }
            //10K
            if (d10 > sisa)
            {
                while (d10 > sisa)
                {
                    tempResult.denom10 = sisa;
                    d10 -= sisa;
                    tempResult.total = tempResult.denom100 + tempResult.denom50 + tempResult.denom20 + tempResult.denom10 + tempResult.denom5 + tempResult.denom2;
                    result.Add(tempResult);
                    sisa = limitDelivery;
                    tempResult = new DataProses();
                    tempResult.sumberDana = sumberDana;
                    tempResult.tujuan = vault;
                    tempResult.keterangan = keterangan;
                }
            }
            if (d10 > 0)
            {
                tempResult.denom10 = d10;
                sisa -= d10;
            }
            //5K
            if (d5 > sisa)
            {
                while (d5 > sisa)
                {
                    tempResult.denom5 = sisa;
                    d5 -= sisa;
                    tempResult.total = tempResult.denom100 + tempResult.denom50 + tempResult.denom20 + tempResult.denom10 + tempResult.denom5 + tempResult.denom2;
                    result.Add(tempResult);
                    sisa = limitDelivery;
                    tempResult = new DataProses();
                    tempResult.sumberDana = sumberDana;
                    tempResult.tujuan = vault;
                    tempResult.keterangan = keterangan;
                }
            }
            if (d5 > 0)
            {
                tempResult.denom5 = d5;
                sisa -= d5;
            }
            //2K
            if (d2 > sisa)
            {
                while (d2 > sisa)
                {
                    tempResult.denom2 = sisa;
                    d2 -= sisa;
                    result.Add(tempResult);
                    sisa = limitDelivery;
                    tempResult = new DataProses();
                    tempResult.sumberDana = sumberDana;
                    tempResult.tujuan = vault;
                    tempResult.keterangan = keterangan;
                }
            }
            if (d2 > 0)
            {
                tempResult.denom2 = d2;
                sisa -= d2;
            }
            if (tempResult.denom100 + tempResult.denom50 + tempResult.denom20 + tempResult.denom10 + tempResult.denom5 + tempResult.denom2 > 0)
            {
                tempResult.total = tempResult.denom100 + tempResult.denom50 + tempResult.denom20 + tempResult.denom10 + tempResult.denom5 + tempResult.denom2;
                result.Add(tempResult);
            }
        }
        public class SumberData
        {
            public String namaBank { set; get; }
            public String sumberDana { set; get; }
            public String tujuan { set; get; }
            public String kodeTujuan { set; get; }
            public Int64? d100 { set; get; }
            public Int64? d50 { set; get; }
            public Int64? d20 { set; get; }
            public Int64? d10 { set; get; }
            public Int64? d5 { set; get; }
            public Int64? d2 { set; get; }
        }
        public class DataProses
        {
            public String sumberDana { set; get; }
            public String vendorSumber { set; get; }
            public String tujuan { set; get; }
            public String namaTujuan { set; get; }
            public Int64 denom100 { set; get; }
            public Int64 denom50 { set; get; }
            public Int64 denom20 { set; get; }
            public Int64 denom10 { set; get; }
            public Int64 denom5 { set; get; }
            public Int64 denom2 { set; get; }
            public Int64 total { set; get; }
            public String keterangan { set; get; }
            public String kodeBank { set; get; }
            public String inputOPR { set; get; }
            public String inputSPV { set; get; }
            public String validasiOPR { set; get; }
            public String validasiSPV { set; get; }
            

        }
    }
}
