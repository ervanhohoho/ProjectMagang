﻿using System;
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
    public partial class UpdateStokPosisiForm : Form
    {
        public UpdateStokPosisiForm()
        {
            InitializeComponent();
        }

        private void selectFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Multiselect = true;
            of.Filter = Variables.excelFilter;
            if(of.ShowDialog() == DialogResult.OK)
            {
                loadForm.ShowSplashScreen();
                foreach (string path in of.FileNames)
                {
                    processFile(path);
                }
                loadForm.CloseForm();
                MessageBox.Show("Done!");
            }
        }
        void processFile(String path)
        {
            GC.Collect();
            DataSet ds = Util.openExcel(path);

            foreach (DataTable temp in ds.Tables)
                processTable(temp, path);

        }
        void processTable(DataTable table, String path)
        {
            GC.Collect();

            int ROWSTARTKERTAS = 12,
                ROWENDKERTAS = 19,
                ROWSTARTKOIN = 23,
                ROWENDKOIN = 28,
                COL_DENOM = 0,
                COL_IN_CABANG = 2,
                COL_IN_RETAIL = 3,
                COL_OUT_CABANG = (int)'J' - (int)'A',
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

            String namaPkt = "";
            try {
                namaPkt = table.Rows[5]['L' - 'A'].ToString();
            }catch(Exception p)
            {
                MessageBox.Show(path + " Sheet " + table.TableName + " tidak benar");
            }
            String tanggalS = table.Rows[8]['B' - 'A'].ToString();

            DateTime buf, tanggal = new DateTime(1, 1, 1);

            if (DateTime.TryParse(tanggalS, out buf))
                tanggal = buf;

            Database1Entities db = new Database1Entities();
            List<StokPosisi> dataDb = (from x in db.StokPosisis.AsEnumerable()
                                       where x.namaPkt == namaPkt
                                       && ((DateTime)x.tanggal).Date == tanggal.Date
                                       select x).ToList();
            bool firstUpdateIN= true;
            if (dataDb.Any())
            {
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
                        inCabangS = rows[a][COL_IN_CABANG].ToString(),
                        inRetailS = rows[a][COL_IN_RETAIL].ToString(),
                        outCabangS = rows[a][COL_OUT_CABANG].ToString();

                    var toEdit = dataDb.Where(x => x.denom == denomS).FirstOrDefault();

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
                        inCabang = 0,
                        inRetail = 0,
                        outCabang = 0;

                    Console.WriteLine("New Baru: " + newBaruS);

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
                    if (!String.IsNullOrEmpty(inCabangS))
                        if (Int64.TryParse(inCabangS, out buff))
                            inCabang = buff;
                    if (!String.IsNullOrEmpty(outCabangS))
                        if (Int64.TryParse(outCabangS, out buff))
                            outCabang = buff;
                    if (!String.IsNullOrEmpty(inRetailS))
                        if (Int64.TryParse(inRetailS, out buff))
                            inRetail = buff;
                    if (toEdit != null)
                    {
                        toEdit.cekLaporan = cekLaporan;
                        toEdit.fitBaru = fitBaru;
                        toEdit.fitLama = fitLama;
                        toEdit.fitNKRI = fitNKRI;
                        toEdit.newBaru = newBaru;
                        toEdit.newLama = newLama;
                        toEdit.passThrough = passThrough;
                        toEdit.RRMBaru = RRMBaru;
                        toEdit.RRMLama = RRMLama;
                        toEdit.RRMNKRI = RRMNKRI;
                        toEdit.RupiahRusakMayor = RupiahRusakMayor;
                        toEdit.unfitBaru = unfitBaru;
                        toEdit.unfitLama = unfitLama;
                        toEdit.unfitNKRI = unfitNKRI;
                        toEdit.unprocessed = unprocessed;
                    }

                    var harisebelom = tanggal.AddDays(-1);
                    var datasebelom = (from x in db.StokPosisis.AsEnumerable()
                                       where x.namaPkt == namaPkt
                                       && ((DateTime)x.tanggal).Date == harisebelom.Date
                                       && x.denom == denomS
                                       && x.jenis == jenisS
                                       select x).FirstOrDefault();
                    if (datasebelom != null)
                    {
                        datasebelom.inCabang = inCabang;
                        datasebelom.inRetail = inRetail;
                        datasebelom.outCabang = outCabang;
                        db.SaveChanges();
                    }
                    else
                    {
                        if (firstUpdateIN)
                        {
                            MessageBox.Show("Data tanggal " + tanggal.AddDays(-1).Date.ToShortDateString() + " " + namaPkt + " tidak ada");
                            firstUpdateIN = false;
                        }
                        db.StokPosisis.Add(new StokPosisi()
                        {
                            tanggal = tanggal.AddDays(-1),
                            namaPkt = namaPkt,
                            denom = denomS,
                            jenis = "Kertas",
                            cekLaporan = 0,
                            fitBaru = 0,
                            fitLama = 0,
                            fitNKRI = 0,
                            newBaru = 0,
                            newLama = 0,
                            passThrough = 0,
                            RRMBaru = 0,
                            RRMLama = 0,
                            RRMNKRI = 0,
                            RupiahRusakMayor = 0,
                            unfitBaru = 0,
                            unfitLama = 0,
                            unfitNKRI = 0,
                            unprocessed = 0,
                            inCabang = inCabang,
                            inRetail = inRetail,
                            outCabang = outCabang
                        });
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
                        inCabangS = rows[a][COL_IN_CABANG].ToString(),
                        inRetailS = rows[a][COL_IN_RETAIL].ToString(),
                        outCabangS = rows[a][COL_OUT_CABANG].ToString();

                    var toEdit = dataDb.Where(x => x.denom == denomS).FirstOrDefault();

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
                        inCabang = 0,
                        inRetail = 0,
                        outCabang = 0;

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
                    if (!String.IsNullOrEmpty(inCabangS))
                        if (Int64.TryParse(inCabangS, out buff))
                            inCabang = buff;
                    if (!String.IsNullOrEmpty(outCabangS))
                        if (Int64.TryParse(outCabangS, out buff))
                            outCabang = buff;
                    if (!String.IsNullOrEmpty(inRetailS))
                        if (Int64.TryParse(inRetailS, out buff))
                            inRetail = buff;
                    if (toEdit != null)
                    {
                        toEdit.cekLaporan = cekLaporan;
                        toEdit.fitBaru = fitBaru;
                        toEdit.fitLama = fitLama;
                        toEdit.fitNKRI = fitNKRI;
                        toEdit.newBaru = newBaru;
                        toEdit.newLama = newLama;
                        toEdit.passThrough = passThrough;
                        toEdit.RRMBaru = RRMBaru;
                        toEdit.RRMLama = RRMLama;
                        toEdit.RRMNKRI = RRMNKRI;
                        toEdit.RupiahRusakMayor = RupiahRusakMayor;
                        toEdit.unfitBaru = unfitBaru;
                        toEdit.unfitLama = unfitLama;
                        toEdit.unfitNKRI = unfitNKRI;
                        toEdit.unprocessed = unprocessed;
                    }
                    DateTime harisebelom = tanggal.AddDays(-1);
                    var datasebelom = (from x in db.StokPosisis.AsEnumerable()
                                       where x.namaPkt == namaPkt
                                       && ((DateTime)x.tanggal).Date == harisebelom.Date
                                       && x.denom == denomS
                                       && x.jenis == jenisS
                                       select x).FirstOrDefault();
                    if (datasebelom != null)
                    {
                        datasebelom.inCabang = inCabang;
                        datasebelom.inRetail = inRetail;
                        datasebelom.outCabang = outCabang;
                        db.SaveChanges();
                    }
                    else
                    {
                        db.StokPosisis.Add(new StokPosisi()
                        {
                            tanggal = tanggal.AddDays(-1),
                            namaPkt = namaPkt,
                            denom = denomS,
                            jenis = "Koin",
                            cekLaporan = 0,
                            fitBaru = 0,
                            fitLama = 0,
                            fitNKRI = 0,
                            newBaru = 0,
                            newLama = 0,
                            passThrough = 0,
                            RRMBaru = 0,
                            RRMLama = 0,
                            RRMNKRI = 0,
                            RupiahRusakMayor = 0,
                            unfitBaru = 0,
                            unfitLama = 0,
                            unfitNKRI = 0,
                            unprocessed = 0,
                            inCabang = inCabang,
                            inRetail = inRetail,
                            outCabang = outCabang
                        });
                        db.SaveChanges();
                    }
                }
                db.SaveChanges();
                db.Dispose();
            }
            else
            {
                return;
            }
        }
    }
}
