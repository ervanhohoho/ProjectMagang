using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testProjectBCA
{
    public partial class RekapApproval : Form
    {
        Database1Entities db = new Database1Entities();
        DateTime tanggalAwal, tanggalAkhir;
        int pktIndex = 0;
        List<String> listPkt;
        IEnumerable<Object> realisasi;
        List<TableApproval> listApproval = new List<TableApproval>();
        public RekapApproval()
        {
            InitializeComponent();
            loadPktCombo();
        }
        private void loadPktCombo()
        {
            var kodePkt = (from x in db.Pkts select x.kodePkt).ToList();
            pktComboBox.DataSource = kodePkt;
            listPkt = kodePkt;
        }
        private void loadRealisasi()
        {
            tanggalAwal = startDatePicker.Value.Date;
            tanggalAkhir = endDatePicker.Value.Date;
            var realisasi = (from x in db.TransaksiAtms.AsEnumerable()
                             where x.tanggal >= tanggalAwal && x.tanggal <= tanggalAkhir && x.kodePkt == listPkt[pktIndex]
                             select new
                             {
                                 x.tanggal,
                                 x.Pkt.kodePkt,
                                 x.saldoAwal100,
                                 x.saldoAwal50,
                                 x.saldoAwal20,
                                 x.sislokATM100,
                                 x.sislokATM50,
                                 x.sislokATM20,
                                 x.sislokCDM100,
                                 x.sislokCDM50,
                                 x.sislokCDM20,
                                 x.sislokCRM100,
                                 x.sislokCRM50,
                                 x.sislokCRM20,
                                 x.isiATM100,
                                 x.isiATM50,
                                 x.isiATM20,
                                 x.isiCRM100,
                                 x.isiCRM50,
                                 x.isiCRM20,
                                 Rasio100 = calculateRasio(x.saldoAwal100, x.isiCRM100, x.isiATM100),
                                 Rasio50 = calculateRasio(x.saldoAwal50, x.isiCRM50, x.isiATM50),
                                 Rasio20 = calculateRasio(x.saldoAwal20, x.isiCRM20, x.isiATM20),
                             });
            realisasiGridView.DataSource = realisasi.ToList();
            this.realisasi = realisasi;
            for (int a = 2; a < realisasiGridView.Columns.Count - 3; a++)
            {
                realisasiGridView.Columns[a].DefaultCellStyle.Format = "C";
                realisasiGridView.Columns[a].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
            }
        }
        private void loadApproval()
        {
            //List<TableApproval> list = new List<TableApproval>();
            listApproval = new List<TableApproval>();
            for (DateTime tanggal = startDatePicker.Value.Date ; tanggal <= endDatePicker.Value.Date ; tanggal = tanggal.AddDays(1))
            {
                DateTime tanggalApproval = (from x in db.Approvals.AsEnumerable()
                                            where x.tanggal < tanggal
                                            select x).Max(x => x.tanggal);
                var realisasi = (from x in db.Approvals.AsEnumerable()
                                 join y in db.DetailApprovals.AsEnumerable() on x.idApproval equals y.idApproval
                                 where x.tanggal == tanggalApproval && x.kodePkt == listPkt[pktIndex] && y.tanggal == tanggal.Date
                                 select new
                                 {
                                     y.tanggal,
                                     x.kodePkt,
                                     y.saldoAwal100,
                                     y.saldoAwal50,
                                     y.saldoAwal20,
                                     y.sislokATM100,
                                     y.sislokATM50,
                                     y.sislokATM20,
                                     y.sislokCDM100,
                                     y.sislokCDM50,
                                     y.sislokCDM20,
                                     y.sislokCRM100,
                                     y.sislokCRM50,
                                     y.sislokCRM20,
                                     y.isiATM100,
                                     y.isiATM50,
                                     y.isiATM20,
                                     y.isiCRM100,
                                     y.isiCRM50,
                                     y.isiCRM20,
                                     Rasio100 = calculateRasio(y.saldoAwal100, y.isiCRM100, y.isiATM100),
                                     Rasio50 = calculateRasio(y.saldoAwal50, y.isiCRM50, y.isiATM50),
                                     Rasio20 = calculateRasio(y.saldoAwal20, y.isiCRM20, y.isiATM20),
                                 }).FirstOrDefault();
                if (realisasi != null)
                {
                    listApproval.Add(new TableApproval
                    {
                        tanggal = (DateTime)realisasi.tanggal,
                        kodePkt = realisasi.kodePkt,
                        saldoAwal100 = (Int64)realisasi.saldoAwal100,
                        saldoAwal50 = (Int64)realisasi.saldoAwal50,
                        saldoAwal20 = (Int64)realisasi.saldoAwal20,
                        sislokATM100 = (Int64)realisasi.sislokATM100,
                        sislokATM50 = (Int64)realisasi.sislokATM50,
                        sislokATM20 = (Int64)realisasi.sislokATM20,
                        sislokCDM100 = (Int64)realisasi.sislokCDM100,
                        sislokCDM50 = (Int64)realisasi.sislokCDM50,
                        sislokCDM20 = (Int64)realisasi.sislokCDM20,
                        sislokCRM100 = (Int64)realisasi.sislokCRM100,
                        sislokCRM50 = (Int64)realisasi.sislokCRM50,
                        sislokCRM20 = (Int64)realisasi.sislokCRM20,
                        isiATM100 = (Int64)realisasi.isiATM100,
                        isiATM50 = (Int64)realisasi.isiATM50,
                        isiATM20 = (Int64)realisasi.isiATM20,
                        isiCRM100 = (Int64)realisasi.isiCRM100,
                        isiCRM50 = (Int64)realisasi.isiCRM50,
                        isiCRM20 = (Int64)realisasi.isiCRM20,
                        Rasio100 = realisasi.Rasio100,
                        Rasio50 = realisasi.Rasio50,
                        Rasio20 = realisasi.Rasio20,
                    }
                  );
                }
            }
            approvalGridView.DataSource = listApproval;
            for (int a = 2; a < approvalGridView.Columns.Count - 3; a++)
            {
                approvalGridView.Columns[a].DefaultCellStyle.Format = "C";
                approvalGridView.Columns[a].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
            }
        }
        private void loadAkurasi()
        {
            List<RasioApproval> akurasi = new List<RasioApproval>();
            for(int a=0;a<approvalGridView.Rows.Count;a++)
            {
                for(int b=0;b<realisasiGridView.Rows.Count;b++)
                {
                    Console.WriteLine(realisasiGridView.Rows[b].Cells[0].Value + " " + approvalGridView.Rows[a].Cells[0].Value);
                    if(realisasiGridView.Rows[b].Cells[0].Value.ToString() == approvalGridView.Rows[a].Cells[0].Value.ToString())
                    {
                        Console.WriteLine("Jalan");
                        RasioApproval temp = new RasioApproval()
                        {
                            tanggal = Convert.ToDateTime(realisasiGridView.Rows[b].Cells[0].Value.ToString()),

                            sislokATM100 = Math.Round(1 - ((Double)Math.Abs((Int64)realisasiGridView.Rows[b].Cells[5].Value - (Int64)approvalGridView.Rows[a].Cells[5].Value) / (Int64)approvalGridView.Rows[a].Cells[5].Value), 4),
                            sislokATM50 = Math.Round(1 - ((Double)Math.Abs((Int64)realisasiGridView.Rows[b].Cells[6].Value - (Int64)approvalGridView.Rows[a].Cells[6].Value) / (Int64)approvalGridView.Rows[a].Cells[6].Value), 4),
                            sislokATM20 = Math.Round(1 - ((Double)Math.Abs((Int64)realisasiGridView.Rows[b].Cells[7].Value - (Int64)approvalGridView.Rows[a].Cells[7].Value) / (Int64)approvalGridView.Rows[a].Cells[7].Value), 4),

                            sislokCDM100 = Math.Round(1 - ((Double)Math.Abs((Int64)realisasiGridView.Rows[b].Cells[8].Value - (Int64)approvalGridView.Rows[a].Cells[8].Value) / (Int64)approvalGridView.Rows[a].Cells[8].Value), 4),
                            sislokCDM50 = Math.Round(1 - ((Double)Math.Abs((Int64)realisasiGridView.Rows[b].Cells[9].Value - (Int64)approvalGridView.Rows[a].Cells[9].Value) / (Int64)approvalGridView.Rows[a].Cells[9].Value), 4),
                            sislokCDM20 = Math.Round(1 - ((Double)Math.Abs((Int64)realisasiGridView.Rows[b].Cells[10].Value - (Int64)approvalGridView.Rows[a].Cells[10].Value) / (Int64)approvalGridView.Rows[a].Cells[10].Value), 4),

                            sislokCRM100 = Math.Round(1 - ((Double)Math.Abs((Int64)realisasiGridView.Rows[b].Cells[11].Value - (Int64)approvalGridView.Rows[a].Cells[11].Value) / (Int64)approvalGridView.Rows[a].Cells[11].Value), 4),
                            sislokCRM50 = Math.Round(1 - ((Double)Math.Abs((Int64)realisasiGridView.Rows[b].Cells[12].Value - (Int64)approvalGridView.Rows[a].Cells[12].Value) / (Int64)approvalGridView.Rows[a].Cells[12].Value), 4),
                            sislokCRM20 = Math.Round(1 - ((Double)Math.Abs((Int64)realisasiGridView.Rows[b].Cells[13].Value - (Int64)approvalGridView.Rows[a].Cells[13].Value) / (Int64)approvalGridView.Rows[a].Cells[13].Value), 4),

                            isiATM100 = Math.Round(1 - ((Double)Math.Abs((Int64)realisasiGridView.Rows[b].Cells[14].Value - (Int64)approvalGridView.Rows[a].Cells[14].Value) / (Int64)approvalGridView.Rows[a].Cells[14].Value), 4),
                            isiATM50 = Math.Round(1 - ((Double)Math.Abs((Int64)realisasiGridView.Rows[b].Cells[15].Value - (Int64)approvalGridView.Rows[a].Cells[15].Value) / (Int64)approvalGridView.Rows[a].Cells[15].Value), 4),
                            isiATM20 = Math.Round(1 - ((Double)Math.Abs((Int64)realisasiGridView.Rows[b].Cells[16].Value - (Int64)approvalGridView.Rows[a].Cells[16].Value) / (Int64)approvalGridView.Rows[a].Cells[16].Value), 4),

                            isiCRM100 = Math.Round(1 - ((Double)Math.Abs((Int64)realisasiGridView.Rows[b].Cells[17].Value - (Int64)approvalGridView.Rows[a].Cells[17].Value) / (Int64)approvalGridView.Rows[a].Cells[17].Value), 4),
                            isiCRM50 = Math.Round(1 - ((Double)Math.Abs((Int64)realisasiGridView.Rows[b].Cells[18].Value - (Int64)approvalGridView.Rows[a].Cells[18].Value) / (Int64)approvalGridView.Rows[a].Cells[18].Value), 4),
                            isiCRM20 = Math.Round(1 - ((Double)Math.Abs((Int64)realisasiGridView.Rows[b].Cells[19].Value - (Int64)approvalGridView.Rows[a].Cells[19].Value) / (Int64)approvalGridView.Rows[a].Cells[19].Value), 4)
                        };
                        akurasi.Add(temp);
                    }
                }
            }
            Console.WriteLine(akurasi.Count);

            AkurasiForecastGridView.DataSource = akurasi;

            for (int a = 1; a < AkurasiForecastGridView.Columns.Count; a++)
            {
                AkurasiForecastGridView.Columns[a].DefaultCellStyle.Format = "0.00%";
            }
        }
        public Double calculateRasio(Int64? saldoAwal, Int64? isiCRM, Int64? isiATM)
        {
            if (saldoAwal == null || isiATM == null || isiCRM == null)
                return 0;
            return Math.Round((Double)saldoAwal / ((Double)isiCRM + (Double)isiATM),2);
        }
        private void RekapApproval_Load(object sender, EventArgs e)
        {

        }

        private void pktComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            pktIndex = pktComboBox.SelectedIndex;
        }

        private void loadBtn_Click(object sender, EventArgs e)
        {
            loadForm.ShowSplashScreen();
            loadRealisasi();
            loadApproval();
            loadAkurasi();
            loadForm.CloseForm();
        }
    }
    public class TableApproval
    {
        public DateTime tanggal { set; get; }
        public String kodePkt { set; get; }
        public Int64 saldoAwal100 { set; get; }
        public Int64 saldoAwal50 { set; get; }
        public Int64 saldoAwal20 { set; get; }
        public Int64 sislokATM100 { set; get; }
        public Int64 sislokATM50 { set; get; }
        public Int64 sislokATM20 { set; get; }
        public Int64 sislokCDM100 { set; get; }
        public Int64 sislokCDM50 { set; get; }
        public Int64 sislokCDM20 { set; get; }
        public Int64 sislokCRM100 { set; get; }
        public Int64 sislokCRM50 { set; get; }
        public Int64 sislokCRM20 { set; get; }
        public Int64 isiATM100 { set; get; }
        public Int64 isiATM50 { set; get; }
        public Int64 isiATM20 { set; get; }
        public Int64 isiCRM100 { set; get; }
        public Int64 isiCRM50 { set; get; }
        public Int64 isiCRM20 { set; get; }
        public Double Rasio100 { set; get; }
        public Double Rasio50 { set; get; }
        public Double Rasio20 { set; get; }
    }
    public class RasioApproval
    {
        public DateTime tanggal { set; get; }
        public Double sislokCDM100 { set; get; }
        public Double sislokCDM50 { set; get; }
        public Double sislokCDM20 { set; get; }
        public Double sislokCRM100 { set; get; }
        public Double sislokCRM50 { set; get; }
        public Double sislokCRM20 { set; get; }
        public Double sislokATM100 { set; get; }
        public Double sislokATM50 { set; get; }
        public Double sislokATM20 { set; get; }

        public Double isiATM100 { set; get; }
        public Double isiATM50 { set; get; }
        public Double isiATM20 { set; get; }
        public Double isiCRM100 { set; get; }
        public Double isiCRM50 { set; get; }
        public Double isiCRM20 { set; get; }
    }
}
