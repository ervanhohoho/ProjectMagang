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

namespace testProjectBCA.Test
{
    public partial class InputSaldoCRM : Form
    {
        public InputSaldoCRM()
        {
            InitializeComponent();
        }

        private void selectBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Multiselect = true;
            if (of.ShowDialog() == DialogResult.OK)
            {
                loadForm.ShowSplashScreen();
                String[] files = of.FileNames;
                foreach (var file in files)
                {
                    Database1Entities db = new Database1Entities();
                    List<SaldoMesin> saldoATMs = new List<SaldoMesin>();
                    String txt = File.ReadAllText(of.FileName);
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
                        Console.WriteLine(tempStr);
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
                            Console.WriteLine(a);
                            continue;
                        }
                        else if (tempStr.Length > 1 && !tempStr.Contains("--"))
                        {
                            Console.WriteLine("Length: " + tempStr.Length);
                            String standarPengisianCartridgeS = tempStr.Substring(15, 26).Trim().Replace(",", ""),
                                totalPengeluaranS = tempStr.Substring(41, 36).Trim().Replace(",", ""),
                                totalPemasukanS = tempStr.Substring(77, 19).Trim().Replace(",", ""),
                                saldoHS = tempStr.Substring(96, 19).Trim().Replace(",", ""),
                                saldoHariSebelumnyaS = tempStr.Length >= 133 ? tempStr.Substring(115, 18).Trim().Replace(",", "") : "0";
                            Int64 standarPengisianCartridge = 0,
                                totalPengeluaran = 0,
                                totalPemasukan = 0,
                                saldoH = 0,
                                saldoHariSebelumnya = 0,
                                buf = 0;
                            if (Int64.TryParse(standarPengisianCartridgeS, out buf))
                                standarPengisianCartridge = buf;
                            if (Int64.TryParse(totalPengeluaranS, out buf))
                                totalPengeluaran = buf;
                            if (Int64.TryParse(totalPemasukanS, out buf))
                                totalPemasukan = buf;
                            if (Int64.TryParse(saldoHS, out buf))
                                saldoH = buf;
                            if (Int64.TryParse(saldoHariSebelumnyaS, out buf))
                                saldoHariSebelumnya = buf;

                            var search = saldoATMs.Where(x => x.kodeLaporan == kodeLaporan && x.tanggal == tanggal && x.wsid == tempStr.Substring(1, 4)).FirstOrDefault();

                            if (search == null)
                            {
                                saldoATMs.Add(new SaldoMesin()
                                {
                                    tanggal = tanggal,
                                    wsid = tempStr.Substring(1, 4),
                                    kodeLaporan = kodeLaporan,
                                    cabang = cabang,
                                    standarPengisianCartridge = standarPengisianCartridge,
                                    totalPengeluaran = totalPengeluaran,
                                    totalPemasukan = totalPemasukan,
                                    saldoH = saldoH,
                                    saldoHariSebelumnya = saldoHariSebelumnya,
                                    jenisMesin = "CRM"
                                });
                            }else
                            {

                            }
                        }
                    }
                    saldoATMs = saldoATMs.GroupBy(x => new { x.tanggal, x.wsid, x.kodeLaporan, x.cabang }).Select(x => new SaldoMesin()
                    {
                        tanggal = x.Key.tanggal,
                        wsid = x.Key.wsid,
                        kodeLaporan = x.Key.kodeLaporan,
                        cabang = x.Key.cabang,
                        standarPengisianCartridge = x.Sum(y => y.standarPengisianCartridge),
                        jenisMesin = "ATM",
                        saldoH = x.Sum(y => y.saldoH),
                        saldoHariSebelumnya = x.Sum(y => y.saldoHariSebelumnya),
                        totalPemasukan = x.Sum(y => y.totalPemasukan),
                        totalPengeluaran = x.Sum(y => y.totalPengeluaran)
                    }).ToList();
                    dataGridView1.DataSource = saldoATMs;
                    db.SaldoMesins.AddRange(saldoATMs);
                    db.SaveChanges();

                }
                loadForm.CloseForm();
            }
        }
        
    }
}
