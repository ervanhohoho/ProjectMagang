using FastMember;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testProjectBCA.Test
{
    public partial class InputSaldoATM : Form
    {
        public InputSaldoATM()
        {
            InitializeComponent();
        }

        private void selectBtn_Click(object sender, EventArgs e)
        {

            OpenFileDialog of = new OpenFileDialog();
            of.Multiselect = true;
            if (of.ShowDialog() == DialogResult.OK)
            {
                String[] files = of.FileNames;
                foreach (var file in files)
                {
                    loadForm.ShowSplashScreen();
                    Database1Entities db = new Database1Entities();
                    List<SaldoMesin> saldoATMs = new List<SaldoMesin>();
                    String txt = File.ReadAllText(file);
                    String[] txtSplit = txt.Split('\n');
                    int counter = 0;
                    String kodeLaporan = "",
                        cabang = "",
                        tanggalS = "";
                    DateTime tanggal = new DateTime(1, 1, 1);
                    String prefixKodeLaporan = "LAPORAN    : ",
                        prefixCabang = "CABANG     : ",
                        prefixTanggal = "TANGGAL : ";
                    for (int a = 0; a < txtSplit.Length; a++)
                    {
                        String tempStr = txtSplit[a];
                        //Console.WriteLine(tempStr);
                        if (tempStr.Trim() == "TOTAL")
                            break;
                        else if (tempStr.Contains("1RETENSI"))
                        {
                            counter = 0;
                            String barisPlus1 = txtSplit[a + 1];
                            String barisPlus2 = txtSplit[a + 2];
                            kodeLaporan = barisPlus1.Substring(barisPlus1.IndexOf(prefixKodeLaporan) + prefixKodeLaporan.Length, 20).Trim();
                            cabang = barisPlus2.Substring(barisPlus2.IndexOf(prefixCabang) + prefixCabang.Length, 20).Trim();
                            tanggalS = barisPlus1.Substring(barisPlus1.LastIndexOf(prefixTanggal) + prefixTanggal.Length, 8);
                            tanggal = DateTime.ParseExact(tanggalS, "dd-M-yy", System.Globalization.CultureInfo.InvariantCulture);
                            a += 5;
                            //Console.WriteLine(a);
                            continue;
                        }
                        else if (tempStr.Length > 1 && !tempStr.Contains("--"))
                        {
                            //Console.WriteLine("Length: " + tempStr.Length);
                            String standarPengisianCartridgeS = tempStr.Substring(15, 26).Trim().Replace(",", ""),
                                totalPengeluaranPemasukanS = tempStr.Substring(41, 46).Trim().Replace(",", ""),
                                saldoHS = tempStr.Substring(87, 26).Trim().Replace(",", ""),
                                saldoHariSebelumnyaS = tempStr.Length >= 131 ? tempStr.Substring(114, 17).Trim().Replace(",", "") : "0";
                            Int64 standarPengisianCartridge = 0,
                                totalPengeluaranPemasukan = 0,
                                saldoH = 0,
                                saldoHariSebelumnya = 0,
                                buf = 0;
                            if (Int64.TryParse(standarPengisianCartridgeS, out buf))
                                standarPengisianCartridge = buf;
                            if (Int64.TryParse(totalPengeluaranPemasukanS, out buf))
                                totalPengeluaranPemasukan = buf;
                            if (Int64.TryParse(saldoHS, out buf))
                                saldoH = buf;
                            if (Int64.TryParse(saldoHariSebelumnyaS, out buf))
                                saldoHariSebelumnya = buf;

                            var saldoatm = db.SaldoMesins.Where(x => x.tanggal == tanggal && x.wsid == tempStr.Substring(1, 4) && x.kodeLaporan == kodeLaporan).FirstOrDefault();
                            if (saldoatm == null)
                            {
                                saldoATMs.Add(new SaldoMesin()
                                {
                                    tanggal = tanggal,
                                    wsid = tempStr.Substring(1, 4),
                                    kodeLaporan = kodeLaporan,
                                    cabang = cabang,
                                    standarPengisianCartridge = standarPengisianCartridge,
                                    totalPengeluaran = totalPengeluaranPemasukan,
                                    totalPemasukan = 0,
                                    saldoH = saldoH,
                                    saldoHariSebelumnya = saldoHariSebelumnya,
                                    jenisMesin = "ATM"
                                });
                            }
                        }
                    }
                    saldoATMs = saldoATMs.GroupBy(x => new { x.tanggal, x.wsid, x.kodeLaporan, x.cabang }).Select(x => new SaldoMesin() {
                        tanggal = x.Key.tanggal,
                        wsid = x.Key.wsid,
                        kodeLaporan = x.Key.kodeLaporan,
                        cabang = x.Key.cabang,
                        standarPengisianCartridge = x.Sum(y=>y.standarPengisianCartridge),
                        jenisMesin = "ATM",
                        saldoH = x.Sum(y=>y.saldoH),
                        saldoHariSebelumnya = x.Sum(y=>y.saldoHariSebelumnya),
                        totalPemasukan = x.Sum(y=>y.totalPemasukan),
                        totalPengeluaran = x.Sum(y=>y.totalPengeluaran)
                    }).ToList();
                    dataGridView1.DataSource = saldoATMs;

                    Console.WriteLine("INSERT");

                    db.SaldoMesins.AddRange(saldoATMs);
                    db.SaveChanges();
                }
                loadForm.CloseForm();
            }
        }
        
    }
}
