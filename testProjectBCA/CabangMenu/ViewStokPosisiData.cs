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
    public partial class ViewStokPosisiData : Form
    {
        Database1Entities db = new Database1Entities();
        List<String> listDenom = new List<String>();
        public ViewStokPosisiData()
        {
            InitializeComponent();
            tanggalAwalPicker.MaxDate = (DateTime) (from x in db.StokPosisis select x.tanggal).Max(x => x);
            tanggalAwalPicker.MinDate = (DateTime) (from x in db.StokPosisis select x.tanggal).Min(x => x);
            tanggalAwalPicker.Value = tanggalAwalPicker.MaxDate;

            tanggalAkhirPicker.MaxDate = (DateTime)(from x in db.StokPosisis select x.tanggal).Max(x => x);
            tanggalAkhirPicker.MinDate = (DateTime)(from x in db.StokPosisis select x.tanggal).Min(x => x);
            tanggalAkhirPicker.Value = tanggalAkhirPicker.MaxDate;
            loadComboPkt();
            jenisUangComboBox.DataSource = new List<String>() { "All", "Kertas", "Koin" };
            jenisUangComboBox.SelectedIndex = 0;
            loadComboDenom();
        }
        void loadComboPkt()
        {
            DateTime tanggalAwal = tanggalAwalPicker.Value;
            DateTime tanggalAkhir = tanggalAwalPicker.Value;
            List<String> listPkt = new List<String>();
            listPkt.Add("All");
            listPkt.AddRange(db.Pkts.Select(x => x.kanwil).Distinct().ToList());
            listPkt.AddRange((from x in db.StokPosisis
                              where x.tanggal >= tanggalAwal
                              && x.tanggal <= tanggalAkhir
                              select x.namaPkt).Distinct().ToList());
            pktComboBox.DataSource = listPkt;
        }
        void loadComboDenom()
        {
            if (jenisUangComboBox.SelectedItem.ToString() == "All")
            {
                listDenom = new List<String>() { "All", "100000", "50000", "20000", "10000", "5000", "2000", "1000", "500", "200", "100" };
            }
            else if (jenisUangComboBox.SelectedItem.ToString() == "Kertas")
            {
                listDenom = new List<String>() { "All", "100000", "50000", "20000", "10000", "5000", "2000", "1000" };
            }
            else
                listDenom = new List<String>() { "All", "1000", "500", "200", "100" };

            denomComboBox.DataSource = listDenom;
        }
        List<StokPosisiToView> loadData()
        {
            DateTime tanggalAwal = tanggalAwalPicker.Value;
            DateTime tanggalAkhir = tanggalAkhirPicker.Value;
            String pkt = pktComboBox.SelectedItem.ToString(),
                jenisUang = jenisUangComboBox.SelectedItem.ToString(),
                denom = denomComboBox.SelectedItem.ToString();

            List<String> listNamaPkt = db.Pkts.Select(x => x.namaPkt).ToList();
            if (listNamaPkt.Where(x => x == pkt).FirstOrDefault() == null)
            {
                var listData = (from x in db.StokPosisis
                                join y in db.Pkts on x.namaPkt equals y.namaPkt
                                where
                                (
                                x.tanggal >= tanggalAwal
                                &&
                                x.tanggal <= tanggalAkhir
                                ) &&
                                (pkt == "All" ? 1 == 1 : y.kanwil == pkt)
                                &&
                                (jenisUang == "All" ? 1 == 1 : x.jenis == jenisUang)
                                &&
                                (denom == "All" ? 1 == 1 : x.denom == denom)
                                select new StokPosisiToView()
                                {
                                    tanggal = (DateTime)x.tanggal,
                                    namaPkt = x.namaPkt,
                                    jenis = x.jenis,
                                    denom = x.denom,
                                    newBaru = (Int64)x.newBaru,
                                    newLama = (Int64)x.newLama,
                                    fitBaru = (Int64)x.fitBaru,
                                    fitNKRI = (Int64)x.fitNKRI,
                                    fitLama = (Int64)x.fitLama,
                                    passThrough = (Int64)x.passThrough,
                                    unfitBaru = (Int64)x.unfitBaru,
                                    unfitNKRI = (Int64)x.unfitNKRI,
                                    unfitLama = (Int64)x.unfitLama,
                                    RRMBaru = (Int64)x.RRMBaru,
                                    RRMNKRI = (Int64)x.RRMNKRI,
                                    RRMLama = (Int64)x.RRMLama,
                                    RupiahRusakMayor = (Int64)x.RupiahRusakMayor,
                                    cekLaporan = (Int64)x.cekLaporan,
                                    openBalance = (Int64)x.openBalance,
                                    inCabang = (Int64)x.inCabang,
                                    inRetail = (Int64)x.inRetail,
                                    inBI = (Int64)x.inBI,
                                    inTUKAB = (Int64)x.inTUKAB,
                                    inOtherCPC = (Int64)x.inOtherCPC,
                                    inVaultATM = (Int64)x.inVaultATM,
                                    inTukaran = (Int64)x.inTukaran,
                                    outCabang = (Int64)x.outCabang,
                                    outRetail = (Int64)x.outRetail,
                                    outBIULE = (Int64)x.outBIULE,
                                    outBIUTLE = (Int64)x.outBIUTLE,
                                    outTUKAB = (Int64)x.outTUKAB,
                                    outOtherCPC = (Int64)x.outOtherCPC,
                                    outVaultATM = (Int64)x.outVaultATM,
                                    outTukaran = (Int64)x.outTukaran,
                                }).ToList();
                return listData;
            }
            else
            {
                var listData = (from x in db.StokPosisis
                                where
                                (
                                x.tanggal >= tanggalAwal
                                &&
                                x.tanggal <= tanggalAkhir
                                ) &&
                                (pkt == "All" ? 1 == 1 : x.namaPkt == pkt)
                                &&
                                (jenisUang == "All" ? 1 == 1 : x.jenis == jenisUang)
                                &&
                                (denom == "All" ? 1 == 1 : x.denom == denom)
                                select new StokPosisiToView()
                                {
                                    tanggal = (DateTime)x.tanggal,
                                    namaPkt = x.namaPkt,
                                    jenis = x.jenis,
                                    denom = x.denom,
                                    newBaru = (Int64)x.newBaru,
                                    newLama = (Int64)x.newLama,
                                    fitBaru = (Int64)x.fitBaru,
                                    fitNKRI = (Int64)x.fitNKRI,
                                    fitLama = (Int64)x.fitLama,
                                    passThrough = (Int64)x.passThrough,
                                    unfitBaru = (Int64)x.unfitBaru,
                                    unfitNKRI = (Int64)x.unfitNKRI,
                                    unfitLama = (Int64)x.unfitLama,
                                    RRMBaru = (Int64)x.RRMBaru,
                                    RRMNKRI = (Int64)x.RRMNKRI,
                                    RRMLama = (Int64)x.RRMLama,
                                    RupiahRusakMayor = (Int64)x.RupiahRusakMayor,
                                    cekLaporan = (Int64)x.cekLaporan,
                                    openBalance = (Int64)x.openBalance,
                                    inCabang = (Int64)x.inCabang,
                                    inRetail = (Int64)x.inRetail,
                                    inBI = (Int64)x.inBI,
                                    inTUKAB = (Int64)x.inTUKAB,
                                    inOtherCPC = (Int64)x.inOtherCPC,
                                    inVaultATM = (Int64)x.inVaultATM,
                                    inTukaran = (Int64)x.inTukaran,
                                    outCabang = (Int64)x.outCabang,
                                    outRetail = (Int64)x.outRetail,
                                    outBIULE = (Int64)x.outBIULE,
                                    outBIUTLE = (Int64)x.outBIUTLE,
                                    outTUKAB = (Int64)x.outTUKAB,
                                    outOtherCPC = (Int64)x.outOtherCPC,
                                    outVaultATM = (Int64)x.outVaultATM,
                                    outTukaran = (Int64)x.outTukaran,
                                }).ToList();
                return listData;
            }
        }
        private void loadButton_Click(object sender, EventArgs e)
        {
            DateTime tanggal = tanggalAwalPicker.Value.Date;
            String pkt = pktComboBox.SelectedItem.ToString(),
                jenisUang = jenisUangComboBox.SelectedItem.ToString(),
                denom = denomComboBox.SelectedItem.ToString();
            var listData = loadData();
            dataGridView1.DataSource = listData;
        }

        private void jenisUangComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            loadComboDenom();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            loadComboPkt();
        }

        private void exportBtn_Click(object sender, EventArgs e)
        {
            List<StokPosisiToView> listData = loadData();
            String text = ServiceStack.Text.CsvSerializer.SerializeToCsv(listData);
            SaveFileDialog sv = new SaveFileDialog();
            sv.Filter = Variables.csvFilter;
            if(sv.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(sv.FileName, text);
            }
        }
    }
    public class StokPosisiToView
    {
        public DateTime tanggal { set; get; }
        public String namaPkt { set; get; }
        public String jenis{ set; get; }
        public String denom{ set; get; }
        public Int64 newBaru{ set; get; }
        public Int64 newLama{ set; get; }
        public Int64 fitBaru{ set; get; }
        public Int64 fitNKRI{ set; get; }
        public Int64 fitLama{ set; get; }
        public Int64 passThrough{ set; get; }
        public Int64 unfitBaru{ set; get; }
        public Int64 unfitNKRI{ set; get; }
        public Int64 unfitLama{ set; get; }
        public Int64 RRMBaru{ set; get; }
        public Int64 RRMNKRI{ set; get; }
        public Int64 RRMLama{ set; get; }
        public Int64 RupiahRusakMayor{ set; get; }
        public Int64 cekLaporan{ set; get; }
        public Int64 openBalance{ set; get; }
        public Int64 inCabang{ set; get; }
        public Int64 inRetail{ set; get; }
        public Int64 inBI{ set; get; }
        public Int64 inTUKAB{ set; get; }
        public Int64 inOtherCPC{ set; get; }
        public Int64 inVaultATM{ set; get; }
        public Int64 inTukaran{ set; get; }
        public Int64 outCabang{ set; get; }
        public Int64 outRetail{ set; get; }
        public Int64 outBIULE{ set; get; }
        public Int64 outBIUTLE{ set; get; }
        public Int64 outTUKAB{ set; get; }
        public Int64 outOtherCPC{ set; get; }
        public Int64 outVaultATM{ set; get; }
        public Int64 outTukaran{ set; get; }
    }
}
