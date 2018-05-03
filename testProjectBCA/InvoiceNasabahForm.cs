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
        public InvoiceNasabahForm()
        {
            InitializeComponent();
            listTanggalAbacas = (from x in db.Abacas 
                                select (DateTime) x.tanggal).Distinct().OrderBy(x=>x).ToList();
            loadCombo();
            loadData();
            dataGridView1.Columns[1].DefaultCellStyle.Format = "C";
            dataGridView1.Columns[1].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
            for (int a = 7; a <= 10; a++)
            {
                dataGridView1.Columns[a].DefaultCellStyle.Format = "C";
                dataGridView1.Columns[a].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
            }
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
                          where ((DateTime)x.tanggal).Year == (int)tahunComboBox.SelectedValue
                          && ((DateTime)x.tanggal).Month == (int)bulanComboBox.SelectedValue
                          select new {
                              x.CustomerCode,
                              x.totalAmount,
                              frekuensi = y.metodeLayanan.ToLower() == "stc" ? 1 :1+hitungFrekuensi((Int64)x.totalAmount),
                          }).ToList();

            var query = (from x in source
                         group x by x.CustomerCode into g
                         select new { KodeNasabah = g.Key, Total = g.Sum(i => i.totalAmount), Frekuensi = g.Sum(i=>i.frekuensi)}).ToList();
            //Bikin khusus yang reguler
            var query2 = (from x in query
                          join y in db.Nasabahs on x.KodeNasabah equals y.kodeNasabah
                          select new { x.KodeNasabah, x.Total, x.Frekuensi,y.fasilitasLayanan, y.metodeLayanan, y.ring, JenisLayanan = "Adhoc", y.segmentasiNasabah, y.sentralisasi, y.subsidi, y.subsidiCabang }).ToList();
            //Cocokin sama harga
            var query3 = (from x in query2
                          join y in db.HargaLayanans on x.JenisLayanan equals y.jenisLayanan
                          where y.stc_cos == x.metodeLayanan.ToLower()
                          select new {
                              x.KodeNasabah,
                              GrandSetoran = x.Total,
                              TotalTrip = x.Frekuensi,
                              x.fasilitasLayanan,
                              x.metodeLayanan,
                              x.ring,
                              x.JenisLayanan,
                              y.hargaRing1,
                              AsuransiCIT = Math.Round((Double)(x.Total * rateAsuransi)),
                              TotalBiayaTrip = x.Frekuensi * y.hargaRing1,
                              PPN =  0.01 * (x.Frekuensi * y.hargaRing1 + Math.Round((Double)(x.Total * rateAsuransi))),
                              x.segmentasiNasabah,
                              x.subsidi,
                              x.subsidiCabang
                          }).ToList();
            
            dataGridView1.DataSource = query3;
            q3 = query3;
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
            loadData();
        }

        private void bulanComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            loadData();
        }
        Int64 hitungFrekuensi(Int64 amount)
        {
            return amount / 500000000;
        }
    }
    class InvoiceToExport
    {

    }
}
