using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testProjectBCA
{
    public partial class InsertStokPosisiForm : Form
    {
        int ROWSTARTKERTAS = 12,
            ROWENDKERTAS = 19,
            ROWSTARTKOIN = 23,
            ROWENDKOIN = 28,
            COL_DENOM = 0,
            COL_OPEN_BALANCE = 1,
            COL_IN_CABANG = 2,
            COL_IN_RETAIL = 3,
            COL_IN_BI = 4,
            COL_IN_TUKAB = 5,
            COL_IN_OTHER_CPC = 6,
            COL_IN_VAULT_ATM = 7,
            COL_IN_TUKARAN = 8,
            COL_OUT_CABANG = 9,
            COL_OUT_RETAIL = 10,
            COL_OUT_BI_ULE = 11,
            COL_OUT_BI_UTLE = 12,
            COL_OUT_TUKAB = 13,
            COL_OUT_OTHER_CPC = 14,
            COL_OUT_VAULT_ATM = 15,
            COL_OUT_TUKARAN = 16,
            COL_UNPROCESSED = 27,
            COL_NEW_BARU = 28,
            COL_NEW_LAMA = 29,
            COL_FIT_BARU = 30,
            COL_FIT_NKRI = 31,
            COL_FIT_LAMA = 32,
            COL_PASSTROUGH = 33,
            COL_UNFIT_BARU = 34,
            COL_UNFIT_NKRI = 35,
            COL_UNFIT_LAMA = 36,
            COL_RRM_BARU = 37,
            COL_RRM_NKRI = 38,
            COL_RRM_LAMA = 39,
            COL_RUPIAH_RUSAK_MAYOR = 40,
            COL_CEK_LAPORAN = 41;
        public InsertStokPosisiForm()
        {
            Database1Entities db = new Database1Entities();
            InitializeComponent();
            DateTime maxTanggal = ((DateTime)db.StokPosisis.Select(x => x.tanggal).Max());
            label1.Text = "Tanggal Terakhir " + maxTanggal.ToShortDateString();
            List<String> listYangAda = (from x in db.StokPosisis
                                        where x.tanggal == maxTanggal
                                        select x.namaPkt).Distinct().ToList();
            List<Pkt> semuaNamaPkt = db.Pkts.Where(x => x.kodePktCabang.Length > 2).ToList();
            foreach(var temp in listYangAda)
            {
                var toRemove = semuaNamaPkt.Where(x => x.namaPkt == temp).FirstOrDefault();
                semuaNamaPkt.Remove(toRemove);
            }

            List<String> listKanwil = semuaNamaPkt.Select(x => x.kanwil).Distinct().ToList();
            foreach(var temp in listKanwil)
                treeView1.Nodes.Add(temp);
            for(int a = 0; a<treeView1.Nodes.Count;a++)
            {
                var node = treeView1.Nodes[a];
                var toAdd = semuaNamaPkt.Where(x => x.kanwil == node.Text).Select(x => x.namaPkt);
                foreach(var add in toAdd)
                {
                    node.Nodes.Add(add);
                }
            }
            
        }
        DateTime lastDate;
        bool first;
        private void selectButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Multiselect = true;
            of.Filter = Variables.excelFilter;

            if(of.ShowDialog() == DialogResult.OK)
            {
                GC.Collect();
                loadForm.ShowSplashScreen();
                foreach (var temp in of.FileNames)
                    processFile(temp);
                loadForm.CloseForm();
                MessageBox.Show("Done!");
            }
        }
        void processFile(String path)
        {
            GC.Collect();
            DataSet ds = Util.openExcel(path);
            Console.WriteLine("Ganti Excel: " + path.Substring(path.LastIndexOf('\\'), path.Length - path.LastIndexOf('\\')));
            first = true;
            foreach (DataTable temp in ds.Tables)
            {
                Console.WriteLine(temp);
                if (!processTable(temp))
                {
                    MessageBox.Show(path.Substring(path.LastIndexOf('\\'), path.Length - path.LastIndexOf('\\') + 1) + " Sheet " + temp.TableName + " Tidak sesuai format.\nData stop dimasukkan");
                    break;
                }
            }
        }
        bool processTable(DataTable table)
        {
            
            Console.WriteLine("Ganti Table: " + table.TableName);

            String namaPkt; 
            String tanggalS;

            try
            {
                namaPkt = table.Rows[5][alphabetToNumber('L')].ToString();
                tanggalS = table.Rows[8][alphabetToNumber('B')].ToString();
            }
            catch
            {
                return false;
            }


            DateTime testDate;
            if (!DateTime.TryParse(tanggalS, out testDate) || namaPkt == null)
                return false;

            DateTime buf, tanggal = new DateTime(1,1,1);

            if (DateTime.TryParse(tanggalS, out buf))
                tanggal = buf;
            
            //Skip Tanggal
            //if (first)
            //{
            //    lastDate = buf;
            //    first = false;
            //}
            //else
            //{
            //    if ((buf - lastDate).TotalDays > 1)
            //    {
            //        MessageBox.Show(table.TableName + " skip 1 tanggal");
            //        return false;
            //    }
            //    else
            //        lastDate = buf;
            //}
            using (Database1Entities db = new Database1Entities())
            {
                List<StokPosisi> dataDb = (from x in db.StokPosisis.AsEnumerable()
                                           where x.namaPkt == namaPkt
                                           && ((DateTime)x.tanggal).Date == tanggal.Date
                                           select x).ToList();
                if (dataDb.Any())
                {
                    return true;
                }
                else
                {
                    List<StokPosisi> toInsert = new List<StokPosisi>();

                    DataRowCollection rows = table.Rows;
                    for (int a = ROWSTARTKERTAS; a <= ROWENDKERTAS; a++)
                    {
                        String unprocessedS = rows[a][COL_UNPROCESSED].ToString(),
                            newBaruS = rows[a][COL_NEW_BARU].ToString(),
                            newLamaS = rows[a][COL_NEW_LAMA].ToString(),
                            fitBaruS = rows[a][COL_FIT_BARU].ToString(),
                            fitNKRIS = rows[a][COL_FIT_NKRI].ToString(),
                            fitLamaS = rows[a][COL_FIT_LAMA].ToString(),
                            passThroughS = rows[a][COL_PASSTROUGH].ToString(),
                            unfitBaruS = rows[a][COL_UNFIT_BARU].ToString(),
                            unfitNKRIS = rows[a][COL_UNFIT_NKRI].ToString(),
                            unfitLamaS = rows[a][COL_UNFIT_LAMA].ToString(),
                            RRMBaruS = rows[a][COL_RRM_BARU].ToString(),
                            RRMNKRIS = rows[a][COL_RRM_NKRI].ToString(),
                            RRMLamaS = rows[a][COL_RRM_LAMA].ToString(),
                            RupiahRusakMayorS = rows[a][COL_RUPIAH_RUSAK_MAYOR].ToString(),
                            cekLaporanS = rows[a][COL_CEK_LAPORAN].ToString(),
                            jenisS = "Kertas",
                            denomS = rows[a][COL_DENOM].ToString(),
                            openBalanceS = rows[a][COL_OPEN_BALANCE].ToString(),
                            inCabangS = rows[a][COL_IN_CABANG].ToString(),
                            inRetailS = rows[a][COL_IN_RETAIL].ToString(),
                            inBIS = rows[a][COL_IN_BI].ToString(),
                            inTUKABS = rows[a][COL_IN_TUKAB].ToString(),
                            inOtherCPCS = rows[a][COL_IN_OTHER_CPC].ToString(),
                            inVaultATMS = rows[a][COL_IN_VAULT_ATM].ToString(),
                            inTukaranS = rows[a][COL_IN_TUKARAN].ToString(),
                            outCabangS = rows[a][COL_OUT_CABANG].ToString(),
                            outRetailS = rows[a][COL_OUT_RETAIL].ToString(),
                            outBIULES = rows[a][COL_OUT_BI_ULE].ToString(),
                            outBIUTLES = rows[a][COL_OUT_BI_UTLE].ToString(),
                            outTUKABS = rows[a][COL_OUT_TUKAB].ToString(),
                            outOtherCPCS = rows[a][COL_OUT_OTHER_CPC].ToString(),
                            outVaultATMS = rows[a][COL_OUT_VAULT_ATM].ToString(),
                            outTukaranS = rows[a][COL_OUT_TUKARAN].ToString();

                        Int64 buff,
                            unprocessed = 0,
                            newBaru = 0,
                            newLama = 0,
                            fitBaru = 0,
                            fitNKRI = 0,
                            fitLama = 0,
                            passThrough = 0,
                            unfitBaru = 0,
                            unfitNKRI = 0,
                            unfitLama = 0,
                            RRMBaru = 0,
                            RRMNKRI = 0,
                            RRMLama = 0,
                            RupiahRusakMayor = 0,
                            cekLaporan = 0,
                            openBalance = 0,
                            inCabang = 0,
                            inRetail = 0,
                            inBI = 0,
                            inTUKAB = 0,
                            inOtherCPC = 0,
                            inVaultATM = 0,
                            inTukaran = 0,
                            outCabang = 0,
                            outRetail = 0,
                            outBIULE = 0,
                            outBIUTLE = 0,
                            outTUKAB = 0,
                            outOtherCPC = 0,
                            outVaultATM = 0,
                            outTukaran = 0;


                        if (!String.IsNullOrEmpty(unprocessedS))
                            if (Int64.TryParse(unprocessedS, out buff))
                                unprocessed = buff;
                        if (!String.IsNullOrEmpty(newBaruS))
                            if (Int64.TryParse(newBaruS, out buff))
                                newBaru = buff;
                        if (!String.IsNullOrEmpty(newLamaS))
                            if (Int64.TryParse(newLamaS, out buff))
                                newLama = buff;
                        if (!String.IsNullOrEmpty(fitBaruS))
                            if (Int64.TryParse(fitBaruS, out buff))
                                fitBaru = buff;
                        if (!String.IsNullOrEmpty(fitNKRIS))
                            if (Int64.TryParse(fitNKRIS, out buff))
                                fitNKRI = buff;
                        if (!String.IsNullOrEmpty(fitLamaS))
                            if (Int64.TryParse(fitLamaS, out buff))
                                fitLama = buff;
                        if (!String.IsNullOrEmpty(passThroughS))
                            if (Int64.TryParse(passThroughS, out buff))
                                passThrough = buff;
                        if (!String.IsNullOrEmpty(unfitBaruS))
                            if (Int64.TryParse(unfitBaruS, out buff))
                                unfitBaru = buff;
                        if (!String.IsNullOrEmpty(unfitNKRIS))
                            if (Int64.TryParse(unfitNKRIS, out buff))
                                unfitNKRI = buff;
                        if (!String.IsNullOrEmpty(unfitLamaS))
                            if (Int64.TryParse(unfitLamaS, out buff))
                                unfitLama = buff;
                        if (!String.IsNullOrEmpty(RRMBaruS))
                            if (Int64.TryParse(RRMBaruS, out buff))
                                RRMBaru = buff;
                        if (!String.IsNullOrEmpty(RRMNKRIS))
                            if (Int64.TryParse(RRMNKRIS, out buff))
                                RRMNKRI = buff;
                        if (!String.IsNullOrEmpty(RRMLamaS))
                            if (Int64.TryParse(RRMLamaS, out buff))
                                RRMLama = buff;
                        if (!String.IsNullOrEmpty(RupiahRusakMayorS))
                            if (Int64.TryParse(RupiahRusakMayorS, out buff))
                                RupiahRusakMayor = buff;
                        if (!String.IsNullOrEmpty(cekLaporanS))
                            if (Int64.TryParse(cekLaporanS, out buff))
                                cekLaporan = buff;
                        if (!String.IsNullOrEmpty(openBalanceS))
                            if (Int64.TryParse(openBalanceS, out buff))
                                openBalance = buff;
                        if (!String.IsNullOrEmpty(inCabangS))
                            if (Int64.TryParse(inCabangS, out buff))
                                inCabang = buff;
                        if (!String.IsNullOrEmpty(inRetailS))
                            if (Int64.TryParse(inRetailS, out buff))
                                inRetail = buff;
                        if (!String.IsNullOrEmpty(inBIS))
                            if (Int64.TryParse(inBIS, out buff))
                                inBI = buff;
                        if (!String.IsNullOrEmpty(inTUKABS))
                            if (Int64.TryParse(inTUKABS, out buff))
                                inTUKAB = buff;
                        if (!String.IsNullOrEmpty(inOtherCPCS))
                            if (Int64.TryParse(inOtherCPCS, out buff))
                                inOtherCPC = buff;
                        if (!String.IsNullOrEmpty(inVaultATMS))
                            if (Int64.TryParse(inVaultATMS, out buff))
                                inVaultATM = buff;
                        if (!String.IsNullOrEmpty(inTukaranS))
                            if (Int64.TryParse(inTukaranS, out buff))
                                inTukaran = buff;
                        if (!String.IsNullOrEmpty(outCabangS))
                            if (Int64.TryParse(outCabangS, out buff))
                                outCabang = buff;
                        if (!String.IsNullOrEmpty(outRetailS))
                            if (Int64.TryParse(outRetailS, out buff))
                                outRetail = buff;
                        if (!String.IsNullOrEmpty(outBIULES))
                            if (Int64.TryParse(outBIULES, out buff))
                                outBIULE = buff;
                        if (!String.IsNullOrEmpty(outBIUTLES))
                            if (Int64.TryParse(outBIUTLES, out buff))
                                outBIUTLE = buff;
                        if (!String.IsNullOrEmpty(outTUKABS))
                            if (Int64.TryParse(outTUKABS, out buff))
                                outTUKAB = buff;
                        if (!String.IsNullOrEmpty(outOtherCPCS))
                            if (Int64.TryParse(outOtherCPCS, out buff))
                                outOtherCPC = buff;
                        if (!String.IsNullOrEmpty(outVaultATMS))
                            if (Int64.TryParse(outVaultATMS, out buff))
                                outVaultATM = buff;
                        if (!String.IsNullOrEmpty(outTukaranS))
                            if (Int64.TryParse(outTukaranS, out buff))
                                outTukaran = buff;

                        toInsert.Add(new StokPosisi()
                        {
                            namaPkt = namaPkt,
                            tanggal = tanggal,
                            denom = denomS,
                            unprocessed = unprocessed,
                            newBaru = newBaru,
                            newLama = newLama,
                            fitBaru = fitBaru,
                            fitNKRI = fitNKRI,
                            fitLama = fitLama,
                            passThrough = passThrough,
                            unfitBaru = unfitBaru,
                            unfitNKRI = unfitNKRI,
                            unfitLama = unfitLama,
                            RRMBaru = RRMBaru,
                            RRMNKRI = RRMNKRI,
                            RRMLama = RRMLama,
                            RupiahRusakMayor = RupiahRusakMayor,
                            cekLaporan = cekLaporan,
                            jenis = jenisS,
                            openBalance = 0,
                            inCabang = 0,
                            inRetail = 0,
                            inBI = 0,
                            inTUKAB = 0,
                            inOtherCPC = 0,
                            inVaultATM = 0,
                            inTukaran =0,
                            outCabang = 0,
                            outRetail = 0,
                            outBIULE = 0,
                            outBIUTLE = 0,
                            outOtherCPC = 0,
                            outTUKAB = 0,
                            outVaultATM = 0,
                            outTukaran = 0,
                        });
                        var harisebelom = tanggal.AddDays(-1);
                        var datasebelom = (from x in db.StokPosisis.AsEnumerable()
                                           where x.namaPkt == namaPkt
                                           && ((DateTime)x.tanggal).Date == harisebelom.Date
                                           && x.denom == denomS
                                           && x.jenis == jenisS
                                           select x).FirstOrDefault();
                        if (datasebelom != null)
                        {
                            datasebelom.openBalance = openBalance;
                            //in
                            datasebelom.inCabang = inCabang;
                            datasebelom.inRetail = inRetail;
                            datasebelom.inBI = inBI;
                            datasebelom.inTUKAB = inTUKAB;
                            datasebelom.inOtherCPC = inOtherCPC;
                            datasebelom.inVaultATM = inVaultATM;
                            datasebelom.inTukaran = inTukaran;
                            //out
                            datasebelom.outCabang = outCabang;
                            datasebelom.outRetail = outRetail;
                            datasebelom.outBIULE = outBIULE;
                            datasebelom.outBIUTLE = outBIUTLE;
                            datasebelom.outTUKAB = outTUKAB;
                            datasebelom.outOtherCPC = outOtherCPC;
                            datasebelom.outVaultATM = outVaultATM;
                            datasebelom.outTukaran = outTukaran;
                            db.SaveChanges();
                        }
                    }
                    for (int a = ROWSTARTKOIN; a <= ROWENDKOIN; a++)
                    {
                        String unprocessedS = rows[a][COL_UNPROCESSED].ToString(),
                             newBaruS = rows[a][COL_NEW_BARU].ToString(),
                             newLamaS = rows[a][COL_NEW_LAMA].ToString(),
                             fitBaruS = rows[a][COL_FIT_BARU].ToString(),
                             fitNKRIS = rows[a][COL_FIT_NKRI].ToString(),
                             fitLamaS = rows[a][COL_FIT_LAMA].ToString(),
                             passThroughS = rows[a][COL_PASSTROUGH].ToString(),
                             unfitBaruS = rows[a][COL_UNFIT_BARU].ToString(),
                             unfitNKRIS = rows[a][COL_UNFIT_NKRI].ToString(),
                             unfitLamaS = rows[a][COL_UNFIT_LAMA].ToString(),
                             RRMBaruS = rows[a][COL_RRM_BARU].ToString(),
                             RRMNKRIS = rows[a][COL_RRM_NKRI].ToString(),
                             RRMLamaS = rows[a][COL_RRM_LAMA].ToString(),
                             RupiahRusakMayorS = rows[a][COL_RUPIAH_RUSAK_MAYOR].ToString(),
                             cekLaporanS = rows[a][COL_CEK_LAPORAN].ToString(),
                             jenisS = "Koin",
                             denomS = rows[a][COL_DENOM].ToString(),
                             openBalanceS = rows[a][COL_OPEN_BALANCE].ToString(),
                             inCabangS = rows[a][COL_IN_CABANG].ToString(),
                             inRetailS = rows[a][COL_IN_RETAIL].ToString(),
                             inBIS = rows[a][COL_IN_BI].ToString(),
                             inTUKABS = rows[a][COL_IN_TUKAB].ToString(),
                             inOtherCPCS = rows[a][COL_IN_OTHER_CPC].ToString(),
                             inVaultATMS = rows[a][COL_IN_VAULT_ATM].ToString(),
                             inTukaranS = rows[a][COL_IN_TUKARAN].ToString(),
                             outCabangS = rows[a][COL_OUT_CABANG].ToString(),
                             outRetailS = rows[a][COL_OUT_RETAIL].ToString(),
                             outBIULES = rows[a][COL_OUT_BI_ULE].ToString(),
                             outBIUTLES = rows[a][COL_OUT_BI_UTLE].ToString(),
                             outTUKABS = rows[a][COL_OUT_TUKAB].ToString(),
                             outOtherCPCS = rows[a][COL_OUT_OTHER_CPC].ToString(),
                             outVaultATMS = rows[a][COL_OUT_VAULT_ATM].ToString(),
                             outTukaranS = rows[a][COL_OUT_TUKARAN].ToString();

                        Int64 buff,
                            unprocessed = 0,
                            newBaru = 0,
                            newLama = 0,
                            fitBaru = 0,
                            fitNKRI = 0,
                            fitLama = 0,
                            passThrough = 0,
                            unfitBaru = 0,
                            unfitNKRI = 0,
                            unfitLama = 0,
                            RRMBaru = 0,
                            RRMNKRI = 0,
                            RRMLama = 0,
                            RupiahRusakMayor = 0,
                            cekLaporan = 0,
                            openBalance = 0,
                            inCabang = 0,
                            inRetail = 0,
                            inBI = 0,
                            inTUKAB = 0,
                            inOtherCPC = 0,
                            inVaultATM = 0,
                            inTukaran = 0,
                            outCabang = 0,
                            outRetail = 0,
                            outBIULE = 0,
                            outBIUTLE = 0,
                            outTUKAB = 0,
                            outOtherCPC = 0,
                            outVaultATM = 0,
                            outTukaran = 0;


                        if (!String.IsNullOrEmpty(unprocessedS))
                            if (Int64.TryParse(unprocessedS, out buff))
                                unprocessed = buff;
                        if (!String.IsNullOrEmpty(newBaruS))
                            if (Int64.TryParse(newBaruS, out buff))
                                newBaru = buff;
                        if (!String.IsNullOrEmpty(newLamaS))
                            if (Int64.TryParse(newLamaS, out buff))
                                newLama = buff;
                        if (!String.IsNullOrEmpty(fitBaruS))
                            if (Int64.TryParse(fitBaruS, out buff))
                                fitBaru = buff;
                        if (!String.IsNullOrEmpty(fitNKRIS))
                            if (Int64.TryParse(fitNKRIS, out buff))
                                fitNKRI = buff;
                        if (!String.IsNullOrEmpty(fitLamaS))
                            if (Int64.TryParse(fitLamaS, out buff))
                                fitLama = buff;
                        if (!String.IsNullOrEmpty(passThroughS))
                            if (Int64.TryParse(passThroughS, out buff))
                                passThrough = buff;
                        if (!String.IsNullOrEmpty(unfitBaruS))
                            if (Int64.TryParse(unfitBaruS, out buff))
                                unfitBaru = buff;
                        if (!String.IsNullOrEmpty(unfitNKRIS))
                            if (Int64.TryParse(unfitNKRIS, out buff))
                                unfitNKRI = buff;
                        if (!String.IsNullOrEmpty(unfitLamaS))
                            if (Int64.TryParse(unfitLamaS, out buff))
                                unfitLama = buff;
                        if (!String.IsNullOrEmpty(RRMBaruS))
                            if (Int64.TryParse(RRMBaruS, out buff))
                                RRMBaru = buff;
                        if (!String.IsNullOrEmpty(RRMNKRIS))
                            if (Int64.TryParse(RRMNKRIS, out buff))
                                RRMNKRI = buff;
                        if (!String.IsNullOrEmpty(RRMLamaS))
                            if (Int64.TryParse(RRMLamaS, out buff))
                                RRMLama = buff;
                        if (!String.IsNullOrEmpty(RupiahRusakMayorS))
                            if (Int64.TryParse(RupiahRusakMayorS, out buff))
                                RupiahRusakMayor = buff;
                        if (!String.IsNullOrEmpty(cekLaporanS))
                            if (Int64.TryParse(cekLaporanS, out buff))
                                cekLaporan = buff;
                        if (!String.IsNullOrEmpty(openBalanceS))
                            if (Int64.TryParse(openBalanceS, out buff))
                                openBalance = buff;
                        if (!String.IsNullOrEmpty(inCabangS))
                            if (Int64.TryParse(inCabangS, out buff))
                                inCabang = buff;
                        if (!String.IsNullOrEmpty(inRetailS))
                            if (Int64.TryParse(inRetailS, out buff))
                                inRetail = buff;
                        if (!String.IsNullOrEmpty(inBIS))
                            if (Int64.TryParse(inBIS, out buff))
                                inBI = buff;
                        if (!String.IsNullOrEmpty(inTUKABS))
                            if (Int64.TryParse(inTUKABS, out buff))
                                inTUKAB = buff;
                        if (!String.IsNullOrEmpty(inOtherCPCS))
                            if (Int64.TryParse(inOtherCPCS, out buff))
                                inOtherCPC = buff;
                        if (!String.IsNullOrEmpty(inVaultATMS))
                            if (Int64.TryParse(inVaultATMS, out buff))
                                inVaultATM = buff;
                        if (!String.IsNullOrEmpty(inTukaranS))
                            if (Int64.TryParse(inTukaranS, out buff))
                                inTukaran = buff;
                        if (!String.IsNullOrEmpty(outCabangS))
                            if (Int64.TryParse(outCabangS, out buff))
                                outCabang = buff;
                        if (!String.IsNullOrEmpty(outRetailS))
                            if (Int64.TryParse(outRetailS, out buff))
                                outRetail = buff;
                        if (!String.IsNullOrEmpty(outBIULES))
                            if (Int64.TryParse(outBIULES, out buff))
                                outBIULE = buff;
                        if (!String.IsNullOrEmpty(outBIUTLES))
                            if (Int64.TryParse(outBIUTLES, out buff))
                                outBIUTLE = buff;
                        if (!String.IsNullOrEmpty(outTUKABS))
                            if (Int64.TryParse(outTUKABS, out buff))
                                outTUKAB = buff;
                        if (!String.IsNullOrEmpty(outOtherCPCS))
                            if (Int64.TryParse(outOtherCPCS, out buff))
                                outOtherCPC = buff;
                        if (!String.IsNullOrEmpty(outVaultATMS))
                            if (Int64.TryParse(outVaultATMS, out buff))
                                outVaultATM = buff;
                        if (!String.IsNullOrEmpty(outTukaranS))
                            if (Int64.TryParse(outTukaranS, out buff))
                                outTukaran = buff;

                        toInsert.Add(new StokPosisi()
                        {
                            namaPkt = namaPkt,
                            tanggal = tanggal,
                            denom = denomS,
                            unprocessed = unprocessed,
                            newBaru = newBaru,
                            newLama = newLama,
                            fitBaru = fitBaru,
                            fitNKRI = fitNKRI,
                            fitLama = fitLama,
                            passThrough = passThrough,
                            unfitBaru = unfitBaru,
                            unfitNKRI = unfitNKRI,
                            unfitLama = unfitLama,
                            RRMBaru = RRMBaru,
                            RRMNKRI = RRMNKRI,
                            RRMLama = RRMLama,
                            RupiahRusakMayor = RupiahRusakMayor,
                            cekLaporan = cekLaporan,
                            jenis = jenisS,
                            openBalance = 0,
                            inCabang = 0,
                            inRetail = 0,
                            inBI = 0,
                            inTUKAB = 0,
                            inOtherCPC = 0,
                            inVaultATM = 0,
                            inTukaran = 0,
                            outCabang = 0,
                            outRetail = 0,
                            outBIULE = 0,
                            outBIUTLE = 0,
                            outOtherCPC = 0,
                            outTUKAB = 0,
                            outVaultATM = 0,
                            outTukaran = 0,
                        });
                        var harisebelom = tanggal.AddDays(-1);
                        var datasebelom = (from x in db.StokPosisis.AsEnumerable()
                                           where x.namaPkt == namaPkt
                                           && ((DateTime)x.tanggal).Date == harisebelom.Date
                                           && x.denom == denomS
                                           && x.jenis == jenisS
                                           select x).FirstOrDefault();
                        if (datasebelom != null)
                        {
                            datasebelom.openBalance = openBalance;
                            //in
                            datasebelom.inCabang = inCabang;
                            datasebelom.inRetail = inRetail;
                            datasebelom.inBI = inBI;
                            datasebelom.inTUKAB = inTUKAB;
                            datasebelom.inOtherCPC = inOtherCPC;
                            datasebelom.inVaultATM = inVaultATM;
                            datasebelom.inTukaran = inTukaran;
                            //out
                            datasebelom.outCabang = outCabang;
                            datasebelom.outRetail = outRetail;
                            datasebelom.outBIULE = outBIULE;
                            datasebelom.outBIUTLE = outBIUTLE;
                            datasebelom.outTUKAB = outTUKAB;
                            datasebelom.outOtherCPC = outOtherCPC;
                            datasebelom.outVaultATM = outVaultATM;
                            datasebelom.outTukaran = outTukaran;
                            db.SaveChanges();
                        }
                    }
                    db.StokPosisis.AddRange(toInsert);
                    db.SaveChanges();
                    toInsert.Clear();
                    toInsert = new List<StokPosisi>();
                    GC.Collect();
                }
            }
            return true;
        }
        int alphabetToNumber(char alpha)
        {
            return (int)alpha - (int)'A';
        }
    }
}
