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
    public partial class AkurasiPrediksiForm : Form
    {
        Database1Entities db = new Database1Entities();
        DateTime tanggalAwal, tanggalAkhir;
        int groupIndex = 0;
        List<String> listGroup;
        IEnumerable<Object> realisasi;
        List<TableApproval> listApproval = new List<TableApproval>();
        List<RasioApproval> akurasi = new List<RasioApproval>();
        List<TableApproval> selisihPrediksi;
        public AkurasiPrediksiForm()
        {
            InitializeComponent();
            loadGroupCombo();
        }
        private void loadGroupCombo()
        {
            List<String> group = (from x in db.Pkts select x.kanwil).Distinct().ToList();
            group.AddRange((from x in db.Pkts select x.e2e).Distinct().ToList());
            group.AddRange((from x in db.Pkts select x.kodePkt).Distinct().ToList());
            GroupComboBox.DataSource = group;
            listGroup = group;
        }
        private void loadRealisasiKanwil()
        {
            tanggalAwal = startDatePicker.Value.Date;
            tanggalAkhir = endDatePicker.Value.Date;
            var realisasi = (from x in db.TransaksiAtms.AsEnumerable()
                             where x.tanggal >= tanggalAwal && x.tanggal <= tanggalAkhir && x.Pkt.kanwil == listGroup[groupIndex]
                             select new
                             {
                                 x.tanggal,
                                 x.Pkt.kodePkt,
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
        private void loadApprovalKanwil()
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
                                 join z in db.Pkts on x.kodePkt equals z.kodePkt
                                 where x.tanggal == tanggalApproval && z.kanwil == listGroup[groupIndex] && y.tanggal == tanggal.Date
                                 select new
                                 {
                                     y.tanggal,
                                     x.kodePkt,
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
        

        private void loadRealisasiE2E()
        {
            tanggalAwal = startDatePicker.Value.Date;
            tanggalAkhir = endDatePicker.Value.Date;
            var realisasi = (from x in db.TransaksiAtms.AsEnumerable()
                             where x.tanggal >= tanggalAwal && x.tanggal <= tanggalAkhir && x.Pkt.e2e == listGroup[groupIndex]
                             select new
                             {
                                 x.tanggal,
                                 x.Pkt.kodePkt,
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
        private void loadApprovalE2E()
        {
            //List<TableApproval> list = new List<TableApproval>();
            listApproval = new List<TableApproval>();
            for (DateTime tanggal = startDatePicker.Value.Date; tanggal <= endDatePicker.Value.Date; tanggal = tanggal.AddDays(1))
            {
                DateTime tanggalApproval = (from x in db.Approvals.AsEnumerable()
                                            where x.tanggal < tanggal
                                            select x).Max(x => x.tanggal);
                var realisasi = (from x in db.Approvals.AsEnumerable()
                                 join y in db.DetailApprovals.AsEnumerable() on x.idApproval equals y.idApproval
                                 join z in db.Pkts on x.kodePkt equals z.kodePkt
                                 where x.tanggal == tanggalApproval && z.e2e == listGroup[groupIndex] && y.tanggal == tanggal.Date
                                 select new
                                 {
                                     y.tanggal,
                                     x.kodePkt,
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

        private void loadRealisasiPkt()
        {
            tanggalAwal = startDatePicker.Value.Date;
            tanggalAkhir = endDatePicker.Value.Date;
            var realisasi = (from x in db.TransaksiAtms.AsEnumerable()
                             where x.tanggal >= tanggalAwal && x.tanggal <= tanggalAkhir && x.kodePkt == listGroup[groupIndex]
                             select new
                             {
                                 x.tanggal,
                                 x.Pkt.kodePkt,
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
        private void loadApprovalPkt()
        {
            //List<TableApproval> list = new List<TableApproval>();
            listApproval = new List<TableApproval>();
            for (DateTime tanggal = startDatePicker.Value.Date; tanggal <= endDatePicker.Value.Date; tanggal = tanggal.AddDays(1))
            {
                DateTime tanggalApproval = (from x in db.Approvals.AsEnumerable()
                                            where x.tanggal < tanggal
                                            select x).Max(x => x.tanggal);
                var realisasi = (from x in db.Approvals.AsEnumerable()
                                 join y in db.DetailApprovals.AsEnumerable() on x.idApproval equals y.idApproval
                                 join z in db.Pkts on x.kodePkt equals z.kodePkt
                                 where x.tanggal == tanggalApproval && z.kodePkt == listGroup[groupIndex] && y.tanggal == tanggal.Date
                                 select new
                                 {
                                     y.tanggal,
                                     x.kodePkt,
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
            akurasi = new List<RasioApproval>();
            for (int a = 0; a < approvalGridView.Rows.Count; a++)
            {
                for (int b = 0; b < realisasiGridView.Rows.Count; b++)
                {
                    Console.WriteLine(realisasiGridView.Rows[b].Cells[0].Value + " " + approvalGridView.Rows[a].Cells[0].Value);
                    if (realisasiGridView.Rows[b].Cells[0].Value.ToString() == approvalGridView.Rows[a].Cells[0].Value.ToString() && realisasiGridView.Rows[b].Cells[1].Value.ToString() == approvalGridView.Rows[a].Cells[1].Value.ToString())
                    {
                        Console.WriteLine("Jalan");
                        RasioApproval temp = new RasioApproval()
                        {
                            tanggal = Convert.ToDateTime(realisasiGridView.Rows[b].Cells[0].Value.ToString()),
                            kodePkt = realisasiGridView.Rows[b].Cells[1].Value.ToString(),

                            sislokATM100 = Math.Round(1 - ((Double)Math.Abs((Int64)realisasiGridView.Rows[b].Cells[2].Value - (Int64)approvalGridView.Rows[a].Cells[2].Value) / (Int64)approvalGridView.Rows[a].Cells[2].Value), 4),
                            sislokATM50 = Math.Round(1 - ((Double)Math.Abs((Int64)realisasiGridView.Rows[b].Cells[3].Value - (Int64)approvalGridView.Rows[a].Cells[3].Value) / (Int64)approvalGridView.Rows[a].Cells[3].Value), 4),
                            sislokATM20 = Math.Round(1 - ((Double)Math.Abs((Int64)realisasiGridView.Rows[b].Cells[4].Value - (Int64)approvalGridView.Rows[a].Cells[4].Value) / (Int64)approvalGridView.Rows[a].Cells[4].Value), 4),

                            sislokCDM100 = Math.Round(1 - ((Double)Math.Abs((Int64)realisasiGridView.Rows[b].Cells[5].Value - (Int64)approvalGridView.Rows[a].Cells[5].Value) / (Int64)approvalGridView.Rows[a].Cells[5].Value), 4),
                            sislokCDM50 = Math.Round(1 - ((Double)Math.Abs((Int64)realisasiGridView.Rows[b].Cells[6].Value - (Int64)approvalGridView.Rows[a].Cells[6].Value) / (Int64)approvalGridView.Rows[a].Cells[6].Value), 4),
                            sislokCDM20 = Math.Round(1 - ((Double)Math.Abs((Int64)realisasiGridView.Rows[b].Cells[7].Value - (Int64)approvalGridView.Rows[a].Cells[7].Value) / (Int64)approvalGridView.Rows[a].Cells[7].Value), 4),

                            sislokCRM100 = Math.Round(1 - ((Double)Math.Abs((Int64)realisasiGridView.Rows[b].Cells[8].Value - (Int64)approvalGridView.Rows[a].Cells[8].Value) / (Int64)approvalGridView.Rows[a].Cells[8].Value), 4),
                            sislokCRM50 = Math.Round(1 - ((Double)Math.Abs((Int64)realisasiGridView.Rows[b].Cells[9].Value - (Int64)approvalGridView.Rows[a].Cells[9].Value) / (Int64)approvalGridView.Rows[a].Cells[9].Value), 4),
                            sislokCRM20 = Math.Round(1 - ((Double)Math.Abs((Int64)realisasiGridView.Rows[b].Cells[10].Value - (Int64)approvalGridView.Rows[a].Cells[10].Value) / (Int64)approvalGridView.Rows[a].Cells[10].Value), 4),

                            isiATM100 = Math.Round(1 - ((Double)Math.Abs((Int64)realisasiGridView.Rows[b].Cells[11].Value - (Int64)approvalGridView.Rows[a].Cells[11].Value) / (Int64)approvalGridView.Rows[a].Cells[11].Value), 4),
                            isiATM50 = Math.Round(1 - ((Double)Math.Abs((Int64)realisasiGridView.Rows[b].Cells[12].Value - (Int64)approvalGridView.Rows[a].Cells[12].Value) / (Int64)approvalGridView.Rows[a].Cells[12].Value), 4),
                            isiATM20 = Math.Round(1 - ((Double)Math.Abs((Int64)realisasiGridView.Rows[b].Cells[13].Value - (Int64)approvalGridView.Rows[a].Cells[13].Value) / (Int64)approvalGridView.Rows[a].Cells[13].Value), 4),

                            isiCRM100 = Math.Round(1 - ((Double)Math.Abs((Int64)realisasiGridView.Rows[b].Cells[14].Value - (Int64)approvalGridView.Rows[a].Cells[14].Value) / (Int64)approvalGridView.Rows[a].Cells[14].Value), 4),
                            isiCRM50 = Math.Round(1 - ((Double)Math.Abs((Int64)realisasiGridView.Rows[b].Cells[15].Value - (Int64)approvalGridView.Rows[a].Cells[15].Value) / (Int64)approvalGridView.Rows[a].Cells[15].Value), 4),
                            isiCRM20 = Math.Round(1 - ((Double)Math.Abs((Int64)realisasiGridView.Rows[b].Cells[16].Value - (Int64)approvalGridView.Rows[a].Cells[16].Value) / (Int64)approvalGridView.Rows[a].Cells[16].Value), 4)
                        };
                        akurasi.Add(temp);
                    }
                }
            }
            Console.WriteLine(akurasi.Count);

            List<RasioApproval>listUntukTampil = akurasi.OrderBy(x => ubahKolomKeBarisRasio(x).Where(y=>y>0).Average()).Take(5).ToList();
            List<RasioApproval> listUntukHitung = akurasi.OrderBy(x => ubahKolomKeBarisRasio(x).Where(y => y > 0).Average()).Take(5).ToList();
            if (listUntukHitung.Count > 0)
            {
                listUntukTampil.Add(new RasioApproval()
                {
                    tanggal = new DateTime(0),
                    kodePkt = "",

                    sislokATM100 = listUntukHitung.Average(x => x.sislokATM100),
                    sislokATM50 = listUntukHitung.Average(x => x.sislokATM50),
                    sislokATM20 = listUntukHitung.Average(x => x.sislokATM20),

                    sislokCDM100 = listUntukHitung.Average(x => x.sislokCDM100),
                    sislokCDM50 = listUntukHitung.Average(x => x.sislokCDM50),
                    sislokCDM20 = listUntukHitung.Average(x => x.sislokCDM20),

                    sislokCRM100 = listUntukHitung.Average(x => x.sislokCRM100),
                    sislokCRM50 = listUntukHitung.Average(x => x.sislokCRM50),
                    sislokCRM20 = listUntukHitung.Average(x => x.sislokCRM20),

                    isiATM100 = listUntukHitung.Average(x => x.isiATM100),
                    isiATM50 = listUntukHitung.Average(x => x.isiATM50),
                    isiATM20 = listUntukHitung.Average(x => x.isiATM20),

                    isiCRM100 = listUntukHitung.Average(x => x.isiCRM100),
                    isiCRM50 = listUntukHitung.Average(x => x.isiCRM50),
                    isiCRM20 = listUntukHitung.Average(x => x.isiCRM20)
                });
            }
            AkurasiForecastGridView.DataSource = listUntukTampil;
            for (int a = 2; a < AkurasiForecastGridView.Columns.Count; a++)
            {
                AkurasiForecastGridView.Columns[a].DefaultCellStyle.Format = "0.00%";
            }
            if(listUntukTampil.Count>0)
                AkurasiForecastGridView.Rows[AkurasiForecastGridView.Rows.Count - 1].DefaultCellStyle.BackColor = Color.Bisque;
        }

        void loadSelisihPrediksi()
        {
            selisihPrediksi = new List<TableApproval>();
            for (int a = 0; a < approvalGridView.Rows.Count; a++)
            {
                for (int b = 0; b < realisasiGridView.Rows.Count; b++)
                {
                    Console.WriteLine(realisasiGridView.Rows[b].Cells[0].Value + " " + approvalGridView.Rows[a].Cells[0].Value);
                    if (realisasiGridView.Rows[b].Cells[0].Value.ToString() == approvalGridView.Rows[a].Cells[0].Value.ToString() && realisasiGridView.Rows[b].Cells[1].Value.ToString() == approvalGridView.Rows[a].Cells[1].Value.ToString())
                    {
                        Console.WriteLine("Jalan");
                        TableApproval temp = new TableApproval()
                        {
                            tanggal = Convert.ToDateTime(realisasiGridView.Rows[b].Cells[0].Value.ToString()),
                            kodePkt = realisasiGridView.Rows[b].Cells[1].Value.ToString(),

                            sislokATM100 =  (Int64)approvalGridView.Rows[a].Cells[2].Value - (Int64)realisasiGridView.Rows[b].Cells[2].Value,
                            sislokATM50 =   -(Int64)realisasiGridView.Rows[b].Cells[3].Value + (Int64)approvalGridView.Rows[a].Cells[3].Value,
                            sislokATM20 =   -(Int64)realisasiGridView.Rows[b].Cells[4].Value + (Int64)approvalGridView.Rows[a].Cells[4].Value,
                                            
                            sislokCDM100 =  -(Int64)realisasiGridView.Rows[b].Cells[5].Value + (Int64)approvalGridView.Rows[a].Cells[5].Value,
                            sislokCDM50 =   -(Int64)realisasiGridView.Rows[b].Cells[6].Value + (Int64)approvalGridView.Rows[a].Cells[6].Value,
                            sislokCDM20 =   -(Int64)realisasiGridView.Rows[b].Cells[7].Value + (Int64)approvalGridView.Rows[a].Cells[7].Value,
                                            
                            sislokCRM100 =  -(Int64)realisasiGridView.Rows[b].Cells[8].Value + (Int64)approvalGridView.Rows[a].Cells[8].Value,
                            sislokCRM50 =   -(Int64)realisasiGridView.Rows[b].Cells[9].Value + (Int64)approvalGridView.Rows[a].Cells[9].Value,
                            sislokCRM20 =   -(Int64)realisasiGridView.Rows[b].Cells[10].Value + (Int64)approvalGridView.Rows[a].Cells[10].Value,
                                            
                            isiATM100 =     -(Int64)realisasiGridView.Rows[b].Cells[11].Value + (Int64)approvalGridView.Rows[a].Cells[11].Value,
                            isiATM50 =      -(Int64)realisasiGridView.Rows[b].Cells[12].Value + (Int64)approvalGridView.Rows[a].Cells[12].Value,
                            isiATM20 =      -(Int64)realisasiGridView.Rows[b].Cells[13].Value + (Int64)approvalGridView.Rows[a].Cells[13].Value,
                                            
                            isiCRM100 =     -(Int64)realisasiGridView.Rows[b].Cells[14].Value + (Int64)approvalGridView.Rows[a].Cells[14].Value,
                            isiCRM50 =      -(Int64)realisasiGridView.Rows[b].Cells[15].Value + (Int64)approvalGridView.Rows[a].Cells[15].Value,
                            isiCRM20 =      -(Int64)realisasiGridView.Rows[b].Cells[16].Value + (Int64)approvalGridView.Rows[a].Cells[16].Value

                        };
                        selisihPrediksi.Add(temp);
                    }
                }
            }
            Console.WriteLine(selisihPrediksi.Count);

            List<TableApproval> listUntukTampil = selisihPrediksi.OrderByDescending(x => Math.Abs(ubahKolomKeBarisSelisih(x).Where(y => y > 0).Average())).Take(5).ToList();
            List<TableApproval> listUntukHitung = selisihPrediksi.OrderByDescending(x => Math.Abs(ubahKolomKeBarisSelisih(x).Where(y => y > 0).Average())).Take(5).ToList();
            if (listUntukHitung.Count > 0)
            {
                listUntukTampil.Add(new TableApproval()
                {
                    tanggal = new DateTime(0),
                    kodePkt = "",

                    sislokATM100 = (Int64)Math.Round(listUntukHitung.Average(x => x.sislokATM100)),
                    sislokATM50 = (Int64)Math.Round(listUntukHitung.Average(x => x.sislokATM50)),
                    sislokATM20 = (Int64)Math.Round(listUntukHitung.Average(x => x.sislokATM20)),

                    sislokCDM100 = (Int64)Math.Round(listUntukHitung.Average(x => x.sislokCDM100)),
                    sislokCDM50 = (Int64)Math.Round(listUntukHitung.Average(x => x.sislokCDM50)),
                    sislokCDM20 = (Int64)Math.Round(listUntukHitung.Average(x => x.sislokCDM20)),

                    sislokCRM100 = (Int64)Math.Round(listUntukHitung.Average(x => x.sislokCRM100)),
                    sislokCRM50 = (Int64)Math.Round(listUntukHitung.Average(x => x.sislokCRM50)),
                    sislokCRM20 = (Int64)Math.Round(listUntukHitung.Average(x => x.sislokCRM20)),

                    isiATM100 = (Int64)Math.Round(listUntukHitung.Average(x => x.isiATM100)),
                    isiATM50 = (Int64)Math.Round(listUntukHitung.Average(x => x.isiATM50)),
                    isiATM20 = (Int64)Math.Round(listUntukHitung.Average(x => x.isiATM20)),

                    isiCRM100 = (Int64)Math.Round(listUntukHitung.Average(x => x.isiCRM100)),
                    isiCRM50 = (Int64)Math.Round(listUntukHitung.Average(x => x.isiCRM50)),
                    isiCRM20 = (Int64)Math.Round(listUntukHitung.Average(x => x.isiCRM20))
                });
            }
            selisihPrediksiGridView.DataSource = listUntukTampil;

            for (int a = 2; a < selisihPrediksiGridView.Columns.Count - 3; a++)
            {
                selisihPrediksiGridView.Columns[a].DefaultCellStyle.Format = "C";
                selisihPrediksiGridView.Columns[a].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
            }
            if(listUntukTampil.Count>0)
                selisihPrediksiGridView.Rows[selisihPrediksiGridView.Rows.Count - 1].DefaultCellStyle.BackColor = Color.Bisque;
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
            groupIndex = GroupComboBox.SelectedIndex;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog sv = new SaveFileDialog();
            sv.Filter = Variables.csvFilter;
            if (sv.ShowDialog() == DialogResult.OK)
            {
                String csv = ServiceStack.Text.CsvSerializer.SerializeToString(akurasi);
                File.WriteAllText(sv.FileName, csv);
            }
        }

        private List<Double> ubahKolomKeBarisRasio(RasioApproval var)
        {
            List<Double> ret = new List<Double>();
            ret.Add(var.isiATM100);
            ret.Add(var.isiATM50);
            ret.Add(var.isiATM20);
            ret.Add(var.isiCRM100);
            ret.Add(var.isiCRM50);
            ret.Add(var.isiCRM20);
            ret.Add(var.sislokATM100);
            ret.Add(var.sislokATM50);
            ret.Add(var.sislokATM20);

            ret.Add(var.sislokCRM100);
            ret.Add(var.sislokCRM50);
            ret.Add(var.sislokCRM20);

            ret.Add(var.sislokCDM100);
            ret.Add(var.sislokCDM50);
            ret.Add(var.sislokCDM20);
            return ret;
        }
        private List<Int64> ubahKolomKeBarisSelisih(TableApproval var)
        {
            List<Int64> ret = new List<Int64>();
            ret.Add(var.isiATM100);
            ret.Add(var.isiATM50);
            ret.Add(var.isiATM20);
            ret.Add(var.isiCRM100);
            ret.Add(var.isiCRM50);
            ret.Add(var.isiCRM20);
            ret.Add(var.sislokATM100);
            ret.Add(var.sislokATM50);
            ret.Add(var.sislokATM20);

            ret.Add(var.sislokCRM100);
            ret.Add(var.sislokCRM50);
            ret.Add(var.sislokCRM20);

            ret.Add(var.sislokCDM100);
            ret.Add(var.sislokCDM50);
            ret.Add(var.sislokCDM20);
            return ret;
        }
        private void loadBtn_Click(object sender, EventArgs e)
        {
            loadForm.ShowSplashScreen();
            int jumlahPkt = (from x in db.Pkts select x).ToList().Count();
            if (groupIndex < listGroup.Count - jumlahPkt - 2)
            {
                loadRealisasiKanwil();
                loadApprovalKanwil();
            }
            else if(groupIndex<listGroup.Count - jumlahPkt)
            {
                loadRealisasiE2E();
                loadApprovalE2E();
            }
            else
            {
                loadRealisasiPkt();
                loadApprovalPkt();
            }
            loadAkurasi();
            loadSelisihPrediksi();
            loadForm.CloseForm();
        }
    }
    public class TableApproval
    {
        public DateTime tanggal { set; get; }
        public String kodePkt { set; get; }
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
        public String kodePkt { set; get; }
        public Double sislokATM100 { set; get; }
        public Double sislokATM50 { set; get; }
        public Double sislokATM20 { set; get; }
        public Double sislokCDM100 { set; get; }
        public Double sislokCDM50 { set; get; }
        public Double sislokCDM20 { set; get; }
        public Double sislokCRM100 { set; get; }
        public Double sislokCRM50 { set; get; }
        public Double sislokCRM20 { set; get; }
        public Double isiATM100 { set; get; }
        public Double isiATM50 { set; get; }
        public Double isiATM20 { set; get; }
        public Double isiCRM100 { set; get; }
        public Double isiCRM50 { set; get; }
        public Double isiCRM20 { set; get; }
    }
}
