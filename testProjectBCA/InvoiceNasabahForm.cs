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
    public partial class InvoiceNasabahForm : Form
    {
        IEnumerable<Object> q3;
        Database1Entities db = new Database1Entities();
        List<DateTime> listTanggalAbacas;
        List<HargaLayanan> listHargaLayanan;
        List<EventTanggal> listEventTanggal;
        public InvoiceNasabahForm()
        {
            InitializeComponent();
            listTanggalAbacas = (from x in db.Abacas 
                                select (DateTime) x.tanggal).Distinct().OrderBy(x=>x).ToList();

            listHargaLayanan = (from x in db.HargaLayanans.AsEnumerable() select x).ToList();
            listEventTanggal = (from x in db.EventTanggals select x).ToList();
            loadCombo();
            //dataGridView1.Columns[1].DefaultCellStyle.Format = "C";
            //dataGridView1.Columns[1].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
            for (int a = 17; a < dataGridView1.Columns.Count; a++)
            {
                if (a == 15 || a == 16 || a == 18 || a == 19 || a==20 || a==21 || a==22)
                    continue;
                dataGridView1.Columns[a].DefaultCellStyle.Format = "C";
                dataGridView1.Columns[a].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
            }
            maxCosNumeric.Value = 500000000;
            maxTukaranKertasNumeric.Value = 100000000;
            maxTukaranKoinMax.Value = 5000000;
        }
        public void loadCombo()
        {
            loadComboTahun();
            loadComboBulan();
        }
        public void loadComboTahun()
        {
            tahunComboBox.DataSource = (from x in listTanggalAbacas
                                   select x.Year).Distinct().OrderBy(x => x).ToList();

        }
        public void loadComboBulan()
        {
            bulanComboBox.DataSource = (from x in listTanggalAbacas
                                        where x.Year == (int)tahunComboBox.SelectedValue
                                        select x.Month).Distinct().OrderBy(x => x).ToList();
        }
        public void loadData()
        {
            Double rateAsuransi = (Double) (from x in db.AsuransiLayanans select x.asuransi).FirstOrDefault();

            var source = (from x in db.Abacas.AsEnumerable()
                          join y in db.Nasabahs on x.CustomerCode equals y.kodeNasabah
                          join z in db.Pkts on y.kodePktCabang equals z.kodePktCabang
                          where ((DateTime)x.tanggal).Year == (int)tahunComboBox.SelectedValue
                          && ((DateTime)x.tanggal).Month == (int)bulanComboBox.SelectedValue
                          && y.metodeLayanan != null
                          select new
                          {
                              x.CustomerCode,
                              x.totalAmount,
                              frekuensi = x.CustomerCode.ToLower().Contains("t") ? hitungFrekuensiTukaran((Int64) x.TotalUangBesar, (Int64) x.totalAmount) : hitungFrekuensi(y.metodeLayanan, (Int64)x.totalAmount),
                              jenisLayanan = getJenisLayanan(x.CustomerCode, x.tanggal, z.vendor)
                          }).ToList();

            var query = (from x in source
                         group x by new { x.CustomerCode, x.jenisLayanan } into g
                         select new { KodeNasabah = g.Key.CustomerCode, jenisLayanan = g.Key.jenisLayanan, Total = g.Sum(i => i.totalAmount), Frekuensi = g.Sum(i => i.frekuensi) }).ToList();
            //Bikin khusus yang reguler
            var query2 = (from x in query
                          join y in db.Nasabahs on x.KodeNasabah equals y.kodeNasabah
                          select new { x.KodeNasabah, x.Total, x.Frekuensi,y.kuota, y.fasilitasLayanan, y.metodeLayanan, y.ring, x.jenisLayanan, y.segmentasiNasabah, y.sentralisasi, y.subsidi, y.persentaseSubsidi }).ToList();
            
            //Cocokin sama harga
            var query3 = (from x in query2
                          join y in db.HargaLayanans on x.jenisLayanan equals y.jenisLayanan
                          where y.stc_cos.ToLower() == x.metodeLayanan.ToLower()
                          select new
                          {
                              x.KodeNasabah,
                              GrandSetoran = x.Total,
                              TotalTrip = x.Frekuensi,
                              x.fasilitasLayanan,
                              x.metodeLayanan,
                              x.ring,
                              x.jenisLayanan,
                              hargaRing = hitungHargaRing(x.ring, x.metodeLayanan, x.jenisLayanan),
                              AsuransiCIT = Math.Round((Double)(x.Total * rateAsuransi)),
                              TotalBiayaTrip = x.Frekuensi * hitungHargaRing(x.ring, x.metodeLayanan, x.jenisLayanan),
                              PPN = 0.01 * (x.Frekuensi * hitungHargaRing(x.ring, x.metodeLayanan, x.jenisLayanan) + Math.Round((Double)(x.Total * rateAsuransi))),
                              x.segmentasiNasabah,
                              x.subsidi,
                              x.persentaseSubsidi,
                              x.kuota
                          }).ToList();

            var query4 = (from x in query3
                          join y in db.Nasabahs.AsEnumerable() on x.KodeNasabah equals y.kodeNasabah
                          join z in db.Cabangs.AsEnumerable() on y.kodeCabang.TrimStart('0') equals z.kodeCabang
                          join aa in db.Pkts.AsEnumerable() on y.kodePktCabang equals aa.kodePktCabang
                          group x by new {
                              x.KodeNasabah,
                              x.fasilitasLayanan,
                              x.metodeLayanan,
                              x.ring,
                              x.hargaRing,
                              x.subsidi,
                              x.persentaseSubsidi,
                              x.kuota,
                              x.jenisLayanan,
                              y.kodePktCabang,
                              y.namaNasabah,
                              y.NomorRekening,
                              y.GroupNasabah,
                              y.subsidiCabang,
                              y.segmentasiNasabah,
                              y.sentralisasi,
                              y.kodeCabang,
                              aa.namaPkt,
                              z.tipe,
                              z.namaCabang
                          } into g
                          select new {
                            KodeNasabah = g.Key.KodeNasabah,
                            g.Key.fasilitasLayanan,
                            g.Key.metodeLayanan,
                            KodePkt = g.Key.kodePktCabang,
                            NamaPkt = g.Key.namaPkt,
                            GrupNasabah = g.Key.GroupNasabah,
                            NamaNasabah = g.Key.namaNasabah,
                            NomorRekening = g.Key.NomorRekening,
                            KodeCabang = g.Key.kodeCabang,
                            StatusCabang = g.Key.tipe,
                            NamaCabang = g.Key.namaCabang,
                            SegmentasiNasabah = g.Key.segmentasiNasabah,
                            Sentralisasi = g.Key.sentralisasi,
                            SubsidiCabang = g.Key.subsidiCabang,
                            g.Key.subsidi,
                            g.Key.persentaseSubsidi,
                            g.Key.kuota,
                            GrandSetoran = g.Sum(x => x.GrandSetoran),
                            TotalTripReguler = g.Key.jenisLayanan == "Reguler" ? g.Sum(x=>x.TotalTrip) : 0,
                            TotalTripRegulerLibur = g.Key.jenisLayanan == "Reguler - Hari Libur" ? g.Sum(x => x.TotalTrip) : 0,
                            TotalTripAdhoc = g.Key.jenisLayanan == "Adhoc" ? g.Sum(x => x.TotalTrip) : 0,
                            TotalTripAdhocLibur = g.Key.jenisLayanan == "Adhoc - Hari Libur" ? g.Sum(x => x.TotalTrip) : 0,
                            HargaReguler = g.Key.jenisLayanan == "Reguler" ? g.Key.hargaRing : 0,
                            HargaRegulerLibur = g.Key.jenisLayanan == "Reguler - Hari Libur" ? g.Key.hargaRing : 0,
                            HargaAdhoc = g.Key.jenisLayanan == "Adhoc" ? g.Key.hargaRing : 0,
                            HargaAdhocLibur = g.Key.jenisLayanan == "Adhoc - Hari Libur" ? g.Key.hargaRing : 0,
                            TotalTrip = g.Sum(x => x.TotalTrip),
                            g.Key.ring,
                            AsuransiCIT = g.Sum(x => x.AsuransiCIT),
                            TotalBiayaTrip = g.Sum(x => x.TotalBiayaTrip),
                            PPN = g.Sum(x => x.PPN),
                            jenisLayanan = g.Key.jenisLayanan,
                            BiayaTripSubsidi = hitungBiayaSubsidi(g.Key.jenisLayanan, g.Key.subsidi, g.Sum(x=>x.TotalBiayaTrip), g.Key.persentaseSubsidi, g.Key.kuota, g.Sum(x=>x.TotalTrip)),
                            BiayaTripNasabah = g.Sum(x => x.TotalBiayaTrip) - (hitungBiayaSubsidi(g.Key.jenisLayanan, g.Key.subsidi, g.Sum(x => x.TotalBiayaTrip), g.Key.persentaseSubsidi, g.Key.kuota, g.Sum(x => x.TotalTrip))),
                            AsuransiSubsidi = hitungBiayaSubsidi(g.Key.jenisLayanan, g.Key.subsidi, g.Sum(x => (Double)x.AsuransiCIT), g.Key.persentaseSubsidi, g.Key.kuota, g.Sum(x => x.TotalTrip)),
                            AsuransiNasabah = g.Sum(x => x.AsuransiCIT) - (hitungBiayaSubsidi(g.Key.jenisLayanan, g.Key.subsidi, g.Sum(x => (Int64)x.AsuransiCIT), g.Key.persentaseSubsidi, g.Key.kuota, g.Sum(x => x.TotalTrip))),
                            PPNSubsidi = hitungBiayaSubsidi(g.Key.jenisLayanan, g.Key.subsidi, g.Sum(x => (Double)x.PPN), g.Key.persentaseSubsidi, g.Key.kuota, g.Sum(x => x.TotalTrip)),
                            PPNNasabah = g.Sum(x => x.PPN) - (hitungBiayaSubsidi(g.Key.jenisLayanan, g.Key.subsidi, g.Sum(x => (Double)x.PPN), g.Key.persentaseSubsidi, g.Key.kuota, g.Sum(x => x.TotalTrip))),
                      }).ToList();

            var query5 = (from x in query4
                          group x by new
                          {
                              x.KodeNasabah,
                              x.fasilitasLayanan,
                              x.metodeLayanan,
                              x.ring,
                              x.subsidi,
                              x.persentaseSubsidi,
                              x.kuota,
                              x.KodePkt,
                              x.NamaNasabah,
                              x.NomorRekening,
                              x.GrupNasabah,
                              x.SubsidiCabang,
                              x.SegmentasiNasabah,
                              x.Sentralisasi,
                              x.KodeCabang,
                              x.NamaPkt,
                              x.StatusCabang,
                              x.NamaCabang
                          } into g
                          select new
                          {
                              g.Key.KodeNasabah,
                              g.Key.fasilitasLayanan,
                              g.Key.metodeLayanan,
                              g.Key.KodePkt,
                              g.Key.NamaPkt,
                              g.Key.GrupNasabah,
                              g.Key.NamaNasabah,
                              g.Key.NomorRekening,
                              g.Key.KodeCabang,
                              g.Key.StatusCabang,
                              g.Key.NamaCabang,
                              g.Key.SegmentasiNasabah,
                              g.Key.Sentralisasi,
                              g.Key.SubsidiCabang,
                              g.Key.subsidi,
                              g.Key.persentaseSubsidi,
                              g.Key.kuota,
                              GrandSetoran = g.Sum(x => x.GrandSetoran),
                              TotalTripReguler = g.Sum(x=>x.TotalTripReguler),
                              TotalTripRegulerLibur = g.Sum(x => x.TotalTripRegulerLibur),
                              TotalTripAdhoc = g.Sum(x => x.TotalTripAdhoc),
                              TotalTripAdhocLibur = g.Sum(x => x.TotalTripAdhocLibur),
                              TotalTrip = g.Sum(x => x.TotalTrip),
                              g.Key.ring,
                              AsuransiCIT = g.Sum(x => x.AsuransiCIT),
                              HargaReguler = g.Sum(x => x.HargaReguler),
                              HargaRegulerLibur = g.Sum(x => x.HargaRegulerLibur),
                              HargaAdhoc = g.Sum(x => x.HargaAdhoc),
                              HargaAdhocLibur = g.Sum(x => x.HargaAdhocLibur),
                              TotalBiayaTrip = g.Sum(x => x.TotalBiayaTrip),
                              PPN = g.Sum(x => x.PPN),
                              TotalBiaya = g.Sum(x=>x.TotalBiayaTrip + x.AsuransiCIT + x.PPN),
                              ProfitCAC = 5000 * g.Sum(x=>x.TotalTrip),
                              BiayaTripSubsidi = g.Sum(x=>x.BiayaTripSubsidi),
                              AsuransiSubsidi = g.Sum(x=>x.AsuransiSubsidi),
                              PPNSubsidi = g.Sum(x=>x.PPNSubsidi),
                              TotalBiayaSubsidi = g.Sum(x => x.BiayaTripSubsidi + x.AsuransiSubsidi + x.PPNSubsidi),
                              BiayaTripNasabah = g.Sum(x=>x.BiayaTripNasabah),
                              AsuransiNasabah = g.Sum(x=>x.AsuransiNasabah),
                              PPNNasabah = g.Sum(x => x.PPNNasabah),
                              TotalBiayaNasabah = g.Sum(x => x.BiayaTripNasabah + x.AsuransiNasabah + x.PPNNasabah),
                          }).ToList();

            dataGridView1.DataSource = query5;
            q3 = query5;
        }
       String getJenisLayanan(String customerCode, DateTime? date, String vendor)
        {
            DateTime temp;
            if (customerCode.Contains("T"))
            {
                return "Tukaran";
            }
            else
            if (date == null)
            {
                return "";
            }
            else
                temp = (DateTime) date;

            if ((temp.DayOfWeek.ToString().ToUpper() == "SUNDAY" || temp.DayOfWeek.ToString().ToUpper() == "SATURDAY"))
                return "Reguler - Hari Libur";
            else
            {
                String workDay = (from x in listEventTanggal
                                  where x.tanggal.Date == temp.Date
                                  select x.workDay).FirstOrDefault();
                if (workDay.ToUpper() == "WORKDAY")
                    return "Reguler";
                else
                    return "Reguler - Hari Libur";
            }
        }
        Int64 hitungHargaRing(String ring, String metodeLayanan, String jenisLayanan)
        {
            if (ring.Contains("1"))
            {
                return (Int64)(from x in listHargaLayanan where x.stc_cos.ToLower() == metodeLayanan.ToLower() && x.jenisLayanan.ToLower() == jenisLayanan.ToLower() select x.hargaRing1).FirstOrDefault();
            }
            else if (ring.Contains("2"))
            {
                return (Int64)(from x in listHargaLayanan
                               where x.stc_cos.ToLower() == metodeLayanan.ToLower() 
                               && x.jenisLayanan.ToLower() == jenisLayanan.ToLower()
                               select x.hargaRing2).FirstOrDefault();
            }
            else if (ring.Contains("3"))
            {
                return (Int64)(from x in listHargaLayanan where x.stc_cos.ToLower() == metodeLayanan.ToLower() && x.jenisLayanan.ToLower() == jenisLayanan.ToLower() select x.hargaRing3).FirstOrDefault();
            }
            else if (ring.Contains("4"))
            {
                return (Int64)(from x in listHargaLayanan where x.stc_cos.ToLower() == metodeLayanan.ToLower() && x.jenisLayanan.ToLower() == jenisLayanan.ToLower() select x.hargaRing4).FirstOrDefault();
            }
            else if (ring.Contains("5"))
            {
                return (Int64)(from x in listHargaLayanan where x.stc_cos.ToLower() == metodeLayanan.ToLower() && x.jenisLayanan.ToLower() == jenisLayanan.ToLower() select x.hargaRing5).FirstOrDefault();
            }
            else
                return 0;
        }
        Double hitungBiayaSubsidi(String jenislayanan, String subsidi, Double? biayaTrip, Double? persenSubsidi, int? kuota, Int64 TotalTrip)
        {
            if (subsidi == "" || persenSubsidi == null)
            {
                return 0;
            }
            else if (subsidi.ToUpper().Contains("GBKF"))
            {
                return (Double)biayaTrip * (Double)persenSubsidi;
            }
            else if (subsidi.ToUpper().Contains("SCM - HARI KALENDER"))
            {
                return (Double) biayaTrip;
            }
            else if(subsidi.ToUpper().Contains("SCM"))
            {
                if (subsidi.ToUpper().Contains("KERJA"))
                {
                    if (!jenislayanan.ToUpper().Contains("LIBUR"))
                    {
                        Int64 biayaYangDisubsidi = (Int64)biayaTrip;
                        if (TotalTrip > kuota)
                            biayaYangDisubsidi = (Int64)((biayaYangDisubsidi * (int)kuota / TotalTrip));
                        return biayaYangDisubsidi;
                    }
                    else
                        return 0;

                }
                else
                {
                    Int64 biayaYangDisubsidi = (Int64)biayaTrip;
                    if (TotalTrip > kuota)
                        biayaYangDisubsidi = (Int64)((biayaYangDisubsidi * (int)kuota / TotalTrip));
                    return biayaYangDisubsidi;
                }
            }
            else
                return 0;
        }
        private void exportButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog sv = new SaveFileDialog();
            sv.Filter = Variables.csvFilter;
            if (sv.ShowDialog() == DialogResult.OK)
            {
                String csv = ServiceStack.Text.CsvSerializer.SerializeToString(q3);
                File.WriteAllText(sv.FileName, csv);
            }
        }

        private void tahunComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            loadComboBulan();
        }

        private void bulanComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
        }
        Int64 hitungFrekuensi(String metodeLayanan, Int64 amount)
        {

            Int64 divider = (Int64)maxCosNumeric.Value;
            if (metodeLayanan.ToLower() == "stc")
                return 1;
            else if (metodeLayanan.ToLower() == "cos")
            {
                if (amount % divider == 0)
                    return (amount / divider);
                return 1 + (amount / divider);
            }
            return 0;
        }
        Int64 hitungFrekuensiTukaran(Int64 uangKertas, Int64 totalAmount)
        {
            Int64 uangKoin= totalAmount - uangKertas;

            int frekKertas, frekKoin, maxKertas = (int)maxTukaranKertasNumeric.Value, maxKoin = (int)maxTukaranKoinMax.Value;

            if (uangKertas % maxKertas == 0)
                frekKertas = (int)(uangKertas / maxKertas);
            else
                frekKertas = 1 + (int)(uangKertas / maxKertas);

            if (uangKoin % maxKoin == 0)
                frekKoin = (int)(uangKoin / maxKoin);
            else
                frekKoin = 1 + (int)(uangKoin / maxKoin);

            if (frekKertas > frekKoin)
                return frekKertas;
            else
                return frekKoin;
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            loadData();
        }
    }
    class InvoiceToExport
    {

    }
}
