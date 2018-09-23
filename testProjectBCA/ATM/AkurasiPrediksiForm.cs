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

namespace testProjectBCA.ATM
{
    public partial class AkurasiPrediksiForm : Form
    {
        Database1Entities db = new Database1Entities();
        DateTime tanggalAwal, tanggalAkhir;
        int groupIndex = 0;
        List<String> listGroup;
        List<TableApproval> realisasi;
        List<TableApproval> listApproval = new List<TableApproval>();
        List<RasioApproval> akurasi = new List<RasioApproval>();
        List<TempSelisih> selisihPrediksi;
        public AkurasiPrediksiForm()
        {
            Database1Entities db = new Database1Entities();
            InitializeComponent();
            loadGroupCombo();
            endDatePicker.MaxDate = db.TransaksiAtms.Max(x => x.tanggal);
            startDatePicker.MaxDate = db.TransaksiAtms.Max(x => x.tanggal);
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
                             select new TableApproval()
                             {
                                 tanggal = x.tanggal,
                                 kodePkt = x.Pkt.kodePkt,
                                 saldoAwal100 = (Int64) x.saldoAwal100,
                                 saldoAwal50 =  (Int64) x.saldoAwal50,
                                 saldoAwal20 =  (Int64) x.saldoAwal20,
                                 sislokATM100 = (Int64) x.sislokATM100,
                                 sislokATM50 =  (Int64) x.sislokATM50,
                                 sislokATM20 =  (Int64) x.sislokATM20,
                                 sislokCDM100 = (Int64) x.sislokCDM100,
                                 sislokCDM50 =  (Int64) x.sislokCDM50,
                                 sislokCDM20 =  (Int64) x.sislokCDM20,
                                 sislokCRM100 = (Int64)x.sislokCRM100,
                                 sislokCRM50 = (Int64)x.sislokCRM50,
                                 sislokCRM20 = (Int64)x.sislokCRM20,
                                 isiATM100 = (Int64)x.isiATM100,
                                 isiATM50 = (Int64)x.isiATM50,
                                 isiATM20 = (Int64)x.isiATM20,
                                 isiCRM100 = (Int64)x.isiCRM100,
                                 isiCRM50 = (Int64)x.isiCRM50,
                                 isiCRM20 = (Int64)x.isiCRM20,
                                 //Rasio100 = calculateRasio(x.saldoAwal100, x.isiCRM100, x.isiATM100),
                                 //Rasio50 = calculateRasio(x.saldoAwal50, x.isiCRM50, x.isiATM50),
                                 //Rasio20 = calculateRasio(x.saldoAwal20, x.isiCRM20, x.isiATM20),
                                 //RasioGabungan = calculateRasio(x.saldoAwal100 + x.saldoAwal50 + x.saldoAwal20, x.isiCRM100 + x.isiCRM50 + x.isiCRM20, x.isiATM100 + x.isiATM50 + x.isiATM20),
                             }).OrderByDescending(x => x.RasioGabungan).ToList();

            var toView = (from x in realisasi
                          group x by x.kodePkt into g
                          select new
                          {
                              kodePkt = g.Key,
                              saldoAwal100 = g.Average(x => x.saldoAwal100),
                              saldoAwal50 = g.Average(x => x.saldoAwal50),
                              saldoAwal20 = g.Average(x => x.saldoAwal20),

                              sislokATM100 = g.Average(x => x.sislokATM100),
                              sislokATM50 = g.Average(x => x.sislokATM50),
                              sislokATM20 = g.Average(x => x.sislokATM20),

                              sislokCDM100 = g.Average(x => x.sislokCDM100),
                              sislokCDM50 = g.Average(x => x.sislokCDM50),
                              sislokCDM20 = g.Average(x => x.sislokCDM20),

                              sislokCRM100 = g.Average(x => x.sislokCRM100),
                              sislokCRM50 = g.Average(x => x.sislokCRM50),
                              sislokCRM20 = g.Average(x => x.sislokCRM20),

                              isiATM100 = g.Average(x => x.isiATM100),
                              isiATM50 = g.Average(x => x.isiATM50),
                              isiATM20 = g.Average(x => x.isiATM20),

                              isiCRM100 = g.Average(x => x.isiCRM100),
                              isiCRM50 = g.Average(x => x.isiCRM50),
                              isiCRM20 = g.Average(x => x.isiCRM20),


                              Rasio100 = g.Average(x => x.saldoAwal100) / g.Average(x=>x.isiATM100 + x.isiCRM100),
                              Rasio50 = g.Average(x => x.saldoAwal50) / g.Average(x=>x.isiATM50 + x.isiCRM50),
                              Rasio20 = g.Average(x => x.saldoAwal20) / g.Average(x=>x.isiATM20 + x.isiCRM20),

                              RasioGabungan = g.Average(x => x.saldoAwal100 + x.saldoAwal20 + x.saldoAwal50) / g.Average(x=>x.isiATM100 + x.isiATM20 + x.isiATM50 + x.isiCRM100 + x.isiCRM20 + x.isiCRM50)
                          }).ToList();
            realisasiGridView.DataSource = toView.OrderByDescending(x=>x.RasioGabungan).ToList();
            this.realisasi = realisasi;
            for (int a = 1; a < realisasiGridView.Columns.Count; a++)
            {
                
                realisasiGridView.Columns[a].DefaultCellStyle.Format = "C";
                realisasiGridView.Columns[a].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
                if (a >= realisasiGridView.ColumnCount - 4)
                {
                    realisasiGridView.Columns[a].DefaultCellStyle.Format = "N2";
                }
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
                                 select new TableApproval()
                                 {
                                     tanggal = (DateTime) y.tanggal,
                                     kodePkt = x.kodePkt,
                                     saldoAwal100 = (Int64)y.saldoAwal100,
                                     saldoAwal50 = (Int64)y.saldoAwal50,
                                     saldoAwal20 = (Int64)y.saldoAwal20,
                                     sislokATM100 = (Int64)y.sislokATM100,
                                     sislokATM50 =  (Int64)y.sislokATM50,
                                     sislokATM20 =  (Int64)y.sislokATM20,
                                     sislokCDM100 = (Int64)y.sislokCDM100,
                                     sislokCDM50 =  (Int64)y.sislokCDM50,
                                     sislokCDM20 =  (Int64)y.sislokCDM20,
                                     sislokCRM100 = (Int64)y.sislokCRM100,
                                     sislokCRM50 =  (Int64)y.sislokCRM50,
                                     sislokCRM20 =  (Int64)y.sislokCRM20,
                                     isiATM100 =    (Int64)y.isiATM100,
                                     isiATM50 =     (Int64)y.isiATM50,
                                     isiATM20 =     (Int64)y.isiATM20,
                                     isiCRM100 =    (Int64)y.isiCRM100,
                                     isiCRM50 =     (Int64)y.isiCRM50,
                                     isiCRM20 =     (Int64)y.isiCRM20,
                                     //Rasio100 = calculateRasio(y.saldoAwal100, y.isiCRM100, y.isiATM100),
                                     //Rasio50 = calculateRasio(y.saldoAwal50, y.isiCRM50, y.isiATM50),
                                     //Rasio20 = calculateRasio(y.saldoAwal20, y.isiCRM20, y.isiATM20),
                                     //RasioGabungan = calculateRasio(y.saldoAwal100 + y.saldoAwal50 + y.saldoAwal20, y.isiCRM100 + y.isiCRM50 + y.isiCRM20, y.isiATM100 + y.isiATM50 + y.isiATM20)
                                 }).OrderByDescending(x=>x.RasioGabungan).ToList();
                if (realisasi != null)
                {
                    listApproval.AddRange(realisasi);
                }
            }
            var toView = (from x in listApproval
                          group x by x.kodePkt into g
                          select new
                          {
                              kodePkt = g.Key,
                              saldoAwal100 = g.Average(x => x.saldoAwal100),
                              saldoAwal50 = g.Average(x => x.saldoAwal50),
                              saldoAwal20 = g.Average(x => x.saldoAwal20),

                              sislokATM100 = g.Average(x => x.sislokATM100),
                              sislokATM50 = g.Average(x => x.sislokATM50),
                              sislokATM20 = g.Average(x => x.sislokATM20),

                              sislokCDM100 = g.Average(x => x.sislokCDM100),
                              sislokCDM50 = g.Average(x => x.sislokCDM50),
                              sislokCDM20 = g.Average(x => x.sislokCDM20),

                              sislokCRM100 = g.Average(x => x.sislokCRM100),
                              sislokCRM50 = g.Average(x => x.sislokCRM50),
                              sislokCRM20 = g.Average(x => x.sislokCRM20),

                              isiATM100 = g.Average(x => x.isiATM100),
                              isiATM50 = g.Average(x => x.isiATM50),
                              isiATM20 = g.Average(x => x.isiATM20),

                              isiCRM100 = g.Average(x => x.isiCRM100),
                              isiCRM50 = g.Average(x => x.isiCRM50),
                              isiCRM20 = g.Average(x => x.isiCRM20),


                              Rasio100 = g.Average(x => x.saldoAwal100) / g.Average(x => x.isiATM100 + x.isiCRM100),
                              Rasio50 = g.Average(x => x.saldoAwal50) / g.Average(x => x.isiATM50 + x.isiCRM50),
                              Rasio20 = g.Average(x => x.saldoAwal20) / g.Average(x => x.isiATM20 + x.isiCRM20),

                              RasioGabungan = g.Average(x => x.saldoAwal100 + x.saldoAwal50 + x.saldoAwal20) / g.Average(x => x.isiATM100 + x.isiATM20 + x.isiATM50 + x.isiCRM100 + x.isiCRM20 + x.isiCRM50)
                          }).ToList();
            approvalGridView.DataSource = toView.OrderByDescending(x=>x.RasioGabungan).ToList();
            for (int a = 1; a < approvalGridView.Columns.Count; a++)
            {
                
                approvalGridView.Columns[a].DefaultCellStyle.Format = "C";
                approvalGridView.Columns[a].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
                if (a >= approvalGridView.ColumnCount - 4)
                {
                    approvalGridView.Columns[a].DefaultCellStyle.Format = "N2";
                }
            }
        }
        private void loadRealisasiE2E()
        {
            tanggalAwal = startDatePicker.Value.Date;
            tanggalAkhir = endDatePicker.Value.Date;
            var realisasi = (from x in db.TransaksiAtms.AsEnumerable()
                             where x.tanggal >= tanggalAwal && x.tanggal <= tanggalAkhir && x.Pkt.e2e == listGroup[groupIndex]
                             select new TableApproval()
                             {
                                 tanggal = x.tanggal,
                                 kodePkt = x.Pkt.kodePkt,
                                 saldoAwal100 = (Int64)x.saldoAwal100,
                                 saldoAwal50 = (Int64)x.saldoAwal50,
                                 saldoAwal20 = (Int64)x.saldoAwal20,
                                 sislokATM100 = (Int64)x.sislokATM100,
                                 sislokATM50 = (Int64)x.sislokATM50,
                                 sislokATM20 = (Int64)x.sislokATM20,
                                 sislokCDM100 = (Int64)x.sislokCDM100,
                                 sislokCDM50 = (Int64)x.sislokCDM50,
                                 sislokCDM20 = (Int64)x.sislokCDM20,
                                 sislokCRM100 = (Int64)x.sislokCRM100,
                                 sislokCRM50 = (Int64)x.sislokCRM50,
                                 sislokCRM20 = (Int64)x.sislokCRM20,
                                 isiATM100 = (Int64)x.isiATM100,
                                 isiATM50 = (Int64)x.isiATM50,
                                 isiATM20 = (Int64)x.isiATM20,
                                 isiCRM100 = (Int64)x.isiCRM100,
                                 isiCRM50 = (Int64)x.isiCRM50,
                                 isiCRM20 = (Int64)x.isiCRM20,
                                 Rasio100 = calculateRasio(x.saldoAwal100, x.isiCRM100, x.isiATM100),
                                 Rasio50 = calculateRasio(x.saldoAwal50, x.isiCRM50, x.isiATM50),
                                 Rasio20 = calculateRasio(x.saldoAwal20, x.isiCRM20, x.isiATM20),
                             }).ToList();
            var toView = (from x in realisasi
                          group x by x.kodePkt into g
                          select new
                          {
                              kodePkt = g.Key,
                              saldoAwal100 = g.Average(x => x.saldoAwal100),
                              saldoAwal50 = g.Average(x => x.saldoAwal50),
                              saldoAwal20 = g.Average(x => x.saldoAwal20),

                              sislokATM100 = g.Average(x => x.sislokATM100),
                              sislokATM50 = g.Average(x => x.sislokATM50),
                              sislokATM20 = g.Average(x => x.sislokATM20),

                              sislokCDM100 = g.Average(x => x.sislokCDM100),
                              sislokCDM50 = g.Average(x => x.sislokCDM50),
                              sislokCDM20 = g.Average(x => x.sislokCDM20),

                              sislokCRM100 = g.Average(x => x.sislokCRM100),
                              sislokCRM50 = g.Average(x => x.sislokCRM50),
                              sislokCRM20 = g.Average(x => x.sislokCRM20),

                              isiATM100 = g.Average(x => x.isiATM100),
                              isiATM50 = g.Average(x => x.isiATM50),
                              isiATM20 = g.Average(x => x.isiATM20),

                              isiCRM100 = g.Average(x => x.isiCRM100),
                              isiCRM50 = g.Average(x => x.isiCRM50),
                              isiCRM20 = g.Average(x => x.isiCRM20),


                              Rasio100 = g.Average(x => x.Rasio100),
                              Rasio50 = g.Average(x => x.Rasio50),
                              Rasio20 = g.Average(x => x.Rasio20),

                              RasioGabungan = g.Average(x => x.RasioGabungan)
                          }).ToList();
            realisasiGridView.DataSource = toView.OrderByDescending(x => x.RasioGabungan).ToList();
            this.realisasi = realisasi;
            for (int a = 1; a < realisasiGridView.Columns.Count; a++)
            {
              
                realisasiGridView.Columns[a].DefaultCellStyle.Format = "C";
                realisasiGridView.Columns[a].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
                if (a >= realisasiGridView.ColumnCount - 4)
                {
                    realisasiGridView.Columns[a].DefaultCellStyle.Format = "N2";
                }
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
                                 select new TableApproval()
                                 {
                                     tanggal = (DateTime)y.tanggal,
                                     kodePkt = x.kodePkt,
                                     sislokATM100 = (Int64)y.sislokATM100,
                                     sislokATM50 = (Int64)y.sislokATM50,
                                     sislokATM20 = (Int64)y.sislokATM20,
                                     sislokCDM100 = (Int64)y.sislokCDM100,
                                     sislokCDM50 = (Int64)y.sislokCDM50,
                                     sislokCDM20 = (Int64)y.sislokCDM20,
                                     sislokCRM100 = (Int64)y.sislokCRM100,
                                     sislokCRM50 = (Int64)y.sislokCRM50,
                                     sislokCRM20 = (Int64)y.sislokCRM20,
                                     isiATM100 = (Int64)y.isiATM100,
                                     isiATM50 = (Int64)y.isiATM50,
                                     isiATM20 = (Int64)y.isiATM20,
                                     isiCRM100 = (Int64)y.isiCRM100,
                                     isiCRM50 = (Int64)y.isiCRM50,
                                     isiCRM20 = (Int64)y.isiCRM20,
                                     Rasio100 = calculateRasio(y.saldoAwal100, y.isiCRM100, y.isiATM100),
                                     Rasio50 = calculateRasio(y.saldoAwal50, y.isiCRM50, y.isiATM50),
                                     Rasio20 = calculateRasio(y.saldoAwal20, y.isiCRM20, y.isiATM20),
                                     RasioGabungan = calculateRasio(y.saldoAwal20 + y.saldoAwal50 + y.saldoAwal100, y.isiCRM20 + y.isiCRM100 + y.isiCRM50, y.isiATM20 + y.isiATM100 + y.isiATM50)
                                 }).OrderByDescending(x=>x.RasioGabungan).ToList();
                if (realisasi != null)
                {
                    listApproval.AddRange(realisasi);
                }
            }
            approvalGridView.DataSource = listApproval;
            for (int a = 1; a < approvalGridView.Columns.Count; a++)
            {
                
                approvalGridView.Columns[a].DefaultCellStyle.Format = "C";
                approvalGridView.Columns[a].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
                if (a >= approvalGridView.ColumnCount - 4)
                {
                    approvalGridView.Columns[a].DefaultCellStyle.Format = "N2";
                }
            }
        }
        private void loadRealisasiPkt()
        {
            tanggalAwal = startDatePicker.Value.Date;
            tanggalAkhir = endDatePicker.Value.Date;
            var realisasi = (from x in db.TransaksiAtms.AsEnumerable()
                             where x.tanggal >= tanggalAwal && x.tanggal <= tanggalAkhir && x.kodePkt == listGroup[groupIndex]
                             select new TableApproval()
                             {
                                 tanggal = x.tanggal,
                                 kodePkt = x.Pkt.kodePkt,
                                 saldoAwal100 = (Int64)x.saldoAwal100,
                                 saldoAwal50 = (Int64)x.saldoAwal50,
                                 saldoAwal20 = (Int64)x.saldoAwal20,
                                 sislokATM100 = (Int64)x.sislokATM100,
                                 sislokATM50 = (Int64)x.sislokATM50,
                                 sislokATM20 = (Int64)x.sislokATM20,
                                 sislokCDM100 = (Int64)x.sislokCDM100,
                                 sislokCDM50 = (Int64)x.sislokCDM50,
                                 sislokCDM20 = (Int64)x.sislokCDM20,
                                 sislokCRM100 = (Int64)x.sislokCRM100,
                                 sislokCRM50 = (Int64)x.sislokCRM50,
                                 sislokCRM20 = (Int64)x.sislokCRM20,
                                 isiATM100 = (Int64)x.isiATM100,
                                 isiATM50 = (Int64)x.isiATM50,
                                 isiATM20 = (Int64)x.isiATM20,
                                 isiCRM100 = (Int64)x.isiCRM100,
                                 isiCRM50 = (Int64)x.isiCRM50,
                                 isiCRM20 = (Int64)x.isiCRM20,
                                 Rasio100 = calculateRasio(x.saldoAwal100, x.isiCRM100, x.isiATM100),
                                 Rasio50 = calculateRasio(x.saldoAwal50, x.isiCRM50, x.isiATM50),
                                 Rasio20 = calculateRasio(x.saldoAwal20, x.isiCRM20, x.isiATM20),
                                 RasioGabungan = calculateRasio(x.saldoAwal100 + x.saldoAwal50 + x.saldoAwal20, x.isiCRM100 + x.isiCRM50 + x.isiCRM20, x.isiATM100 + x.isiATM50 +x.isiATM20),
                             }).ToList();


            realisasiGridView.DataSource = realisasi;
            this.realisasi = realisasi;
            for (int a = 2; a < realisasiGridView.Columns.Count - 4; a++)
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
            var toView = (from x in listApproval
                          group x by x.kodePkt into g
                          select new
                          {
                              kodePkt = g.Key,
                              saldoAwal100 = g.Average(x => x.saldoAwal100),
                              saldoAwal50 = g.Average(x => x.saldoAwal50),
                              saldoAwal20 = g.Average(x => x.saldoAwal20),

                              sislokATM100 = g.Average(x => x.sislokATM100),
                              sislokATM50 = g.Average(x => x.sislokATM50),
                              sislokATM20 = g.Average(x => x.sislokATM20),

                              sislokCDM100 = g.Average(x => x.sislokCDM100),
                              sislokCDM50 = g.Average(x => x.sislokCDM50),
                              sislokCDM20 = g.Average(x => x.sislokCDM20),

                              sislokCRM100 = g.Average(x => x.sislokCRM100),
                              sislokCRM50 = g.Average(x => x.sislokCRM50),
                              sislokCRM20 = g.Average(x => x.sislokCRM20),

                              isiATM100 = g.Average(x => x.isiATM100),
                              isiATM50 = g.Average(x => x.isiATM50),
                              isiATM20 = g.Average(x => x.isiATM20),

                              isiCRM100 = g.Average(x => x.isiCRM100),
                              isiCRM50 = g.Average(x => x.isiCRM50),
                              isiCRM20 = g.Average(x => x.isiCRM20),


                              Rasio100 = g.Average(x => x.Rasio100),
                              Rasio50 = g.Average(x => x.Rasio50),
                              Rasio20 = g.Average(x => x.Rasio20),

                              RasioGabungan = g.Average(x => x.RasioGabungan)
                          }).ToList();
            approvalGridView.DataSource = toView.OrderByDescending(x => x.RasioGabungan).ToList();
            for (int a = 1; a < approvalGridView.Columns.Count - 4; a++)
            {
                approvalGridView.Columns[a].DefaultCellStyle.Format = "C";
                approvalGridView.Columns[a].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
            }
        }
        private void loadAkurasi()
        {
            DateTime endDate = endDatePicker.Value.Date;
            DateTime startDate = startDatePicker.Value.Date;
            var transaksiAtms = (from x in db.TransaksiAtms
                                 where x.tanggal <= endDate && x.tanggal >= startDate
                                 select x).ToList();
            var query = (from x in realisasi
                         from y in listApproval.Where(z => z.kodePkt == x.kodePkt && z.tanggal == x.tanggal).DefaultIfEmpty()
                         select new
                         {
                             tanggal = x.tanggal,
                             kodePkt = x.kodePkt,

                             saldoAwal100 = transaksiAtms.Where(a => a.kodePkt == x.kodePkt && a.tanggal == x.tanggal).Select(a => a.saldoAwal100).FirstOrDefault(),
                             saldoAwal50 = transaksiAtms.Where(a => a.kodePkt == x.kodePkt && a.tanggal == x.tanggal).Select(a => a.saldoAwal50).FirstOrDefault(),
                             saldoAwal20 = transaksiAtms.Where(a => a.kodePkt == x.kodePkt && a.tanggal == x.tanggal).Select(a => a.saldoAwal20).FirstOrDefault(),

                             realisasiSislokATM100 = x.sislokATM100,
                             realisasiSislokATM50 = x.sislokATM50,
                             realisasiSislokATM20 = x.sislokATM20,

                             realisasiSislokCDM100 = x.sislokCDM100,
                             realisasiSislokCDM50 = x.sislokCDM50,
                             realisasiSislokCDM20 = x.sislokCDM20,

                             realisasiSislokCRM100 = x.sislokCRM100,
                             realisasiSislokCRM50 = x.sislokCRM50,
                             realisasiSislokCRM20 = x.sislokCRM20,

                             realisasiIsiCRM100 = x.isiCRM100,
                             realisasiIsiCRM50 = x.isiCRM50,
                             realisasiIsiCRM20 = x.isiCRM20,

                             realisasiIsiATM100 = x.isiATM100,
                             realisasiIsiATM50 = x.isiATM50,
                             realisasiIsiATM20 = x.isiATM20,

                             forecastSislokATM100 = y != null ? y.sislokATM100 : Double.NaN,
                             forecastSislokATM50 = y != null ? y.sislokATM50 : Double.NaN,
                             forecastSislokATM20 = y != null ? y.sislokATM20 : Double.NaN,

                             forecastSislokCDM100 = y != null ? y.sislokCDM100 : Double.NaN,
                             forecastSislokCDM50 = y != null ? y.sislokCDM50 : Double.NaN,
                             forecastSislokCDM20 = y != null ? y.sislokCDM20 : Double.NaN,

                             forecastSislokCRM100 = y != null ? y.sislokCRM100 : Double.NaN,
                             forecastSislokCRM50 = y != null ? y.sislokCRM50 : Double.NaN,
                             forecastSislokCRM20 = y != null ? y.sislokCRM20 : Double.NaN,

                             forecastIsiCRM100 = y != null ? y.isiCRM100 : Double.NaN,
                             forecastIsiCRM50 = y != null ? y.isiCRM50 : Double.NaN,
                             forecastIsiCRM20 = y != null ? y.isiCRM20 : Double.NaN,

                             forecastIsiATM100 = y != null ? y.isiATM100 : Double.NaN,
                             forecastIsiATM50 = y != null ? y.isiATM50 : Double.NaN,
                             forecastIsiATM20 = y != null ? y.isiATM20 : Double.NaN,
                         }).ToList();
            var query2 = (from x in query
                          group x by x.kodePkt into g
                          select new {
                              kodePkt = g.Key,

                              realisasiSislokATM100 = g.Average(x => x.realisasiSislokATM100),
                              realisasiSislokATM50 = g.Average(x => x.realisasiSislokATM50),
                              realisasiSislokATM20 = g.Average(x => x.realisasiSislokATM20),

                              realisasiSislokCDM100 = g.Average(x => x.realisasiSislokCDM100),
                              realisasiSislokCDM50 = g.Average(x => x.realisasiSislokCDM50),
                              realisasiSislokCDM20 = g.Average(x => x.realisasiSislokCDM20),

                              realisasiSislokCRM100 = g.Average(x => x.realisasiSislokCRM100),
                              realisasiSislokCRM50 = g.Average(x => x.realisasiSislokCRM50),
                              realisasiSislokCRM20 = g.Average(x => x.realisasiSislokCRM20),

                              realisasiIsiCRM100 = g.Average(x => x.realisasiIsiCRM100),
                              realisasiIsiCRM50 = g.Average(x => x.realisasiIsiCRM50),
                              realisasiIsiCRM20 = g.Average(x => x.realisasiIsiCRM20),

                              realisasiIsiATM100 = g.Average(x => x.realisasiIsiATM100),
                              realisasiIsiATM50 = g.Average(x => x.realisasiIsiATM50),
                              realisasiIsiATM20 = g.Average(x => x.realisasiIsiATM20),

                              forecastSislokATM100 = g.Average(x => x.forecastSislokATM100),
                              forecastSislokATM50 = g.Average(x => x.forecastSislokATM50),
                              forecastSislokATM20 = g.Average(x => x.forecastSislokATM20),

                              forecastSislokCDM100 = g.Average(x => x.forecastSislokCDM100),
                              forecastSislokCDM50 = g.Average(x => x.forecastSislokCDM50),
                              forecastSislokCDM20 = g.Average(x => x.forecastSislokCDM20),

                              forecastSislokCRM100 = g.Average(x => x.forecastSislokCRM100),
                              forecastSislokCRM50 = g.Average(x => x.forecastSislokCRM50),
                              forecastSislokCRM20 = g.Average(x => x.forecastSislokCRM20),

                              forecastIsiCRM100 = g.Average(x => x.forecastIsiCRM100),
                              forecastIsiCRM50 = g.Average(x => x.forecastIsiCRM50),
                              forecastIsiCRM20 = g.Average(x => x.forecastIsiCRM20),

                              forecastIsiATM100 = g.Average(x => x.forecastIsiATM100),
                              forecastIsiATM50 = g.Average(x => x.forecastIsiATM50),
                              forecastIsiATM20 = g.Average(x => x.forecastIsiATM20),

                              saldoAwal100 = g.Average(x => x.saldoAwal100),
                              saldoAwal50 = g.Average(x => x.saldoAwal50),
                              saldoAwal20 = g.Average(x => x.saldoAwal20)
                          }).ToList();
            var resultQuery = (from x in query2
                               select new RasioApproval()
                               {
                                   kodePkt = x.kodePkt,

                                   sislokATM100 = x.forecastSislokATM100 / x.realisasiSislokATM100,
                                   sislokATM50 = x.forecastSislokATM50 / x.realisasiSislokATM50,
                                   sislokATM20 = x.forecastSislokATM20 / x.realisasiSislokATM20,

                                   sislokCRM100 = x.forecastSislokCRM100 / x.realisasiSislokCRM100,
                                   sislokCRM50 = x.forecastSislokCRM50 / x.realisasiSislokCRM50,
                                   sislokCRM20 = x.forecastSislokCRM100 / x.realisasiSislokCRM20,

                                   sislokCDM100 = x.forecastSislokCDM100 / x.realisasiSislokCDM100,
                                   sislokCDM50 = x.forecastSislokCDM50 / x.realisasiSislokCDM50,
                                   sislokCDM20 = x.forecastSislokCDM20 / x.realisasiSislokCDM20,

                                   isiATM100 = x.forecastIsiATM100 / x.realisasiIsiATM100,
                                   isiATM50 = x.forecastIsiATM50 / x.realisasiIsiATM50,
                                   isiATM20 = x.forecastIsiATM20 / x.realisasiIsiATM20,

                                   isiCRM100 = x.forecastIsiCRM100 / x.realisasiIsiCRM100,
                                   isiCRM50 = x.forecastIsiCRM50 / x.realisasiIsiCRM50,
                                   isiCRM20 = x.forecastIsiCRM20 / x.realisasiIsiCRM20,

                                   rasio100 = (Double) ((x.realisasiIsiATM100 + x.realisasiIsiCRM100) == 0 ? 0 : x.saldoAwal100 / (x.realisasiIsiATM100 + x.realisasiIsiCRM100)),
                                   rasio50 = (Double) ((x.realisasiIsiATM50 + x.realisasiIsiCRM50) == 0 ? 0 : x.saldoAwal50 / (x.realisasiIsiATM50 + x.realisasiIsiCRM50)),
                                   rasio20 = (Double) ((x.realisasiIsiATM20 + x.realisasiIsiCRM20) == 0 ? 0 : x.saldoAwal20 / (x.realisasiIsiATM20 + x.realisasiIsiCRM20)),
                                   RasioGabungan = (Double) ((x.realisasiIsiATM100 + x.realisasiIsiATM50 + x.realisasiIsiATM20 + x.realisasiIsiCRM100 + x.realisasiIsiCRM20 + x.realisasiIsiCRM50) == 0? 0 : (x.saldoAwal100 + x.saldoAwal50 + x.saldoAwal20) / (x.realisasiIsiATM100 + x.realisasiIsiATM50 + x.realisasiIsiATM20 + x.realisasiIsiCRM100 + x.realisasiIsiCRM20 + x.realisasiIsiCRM50))
                                   }).ToList();

            Console.WriteLine("Query: " + query.Count);
            resultQuery = resultQuery.OrderByDescending(x => x.RasioGabungan).ToList();
            if (resultQuery.Any())
            {
                resultQuery.Add(new RasioApproval()
                {
                    kodePkt = "AVERAGE",

                    isiATM100 = resultQuery.Average(x => x.isiATM100),
                    isiATM50 = resultQuery.Average(x => x.isiATM50),
                    isiATM20 = resultQuery.Average(x => x.isiATM20),
                    isiCRM100 = resultQuery.Average(x => x.isiCRM100),
                    isiCRM50 = resultQuery.Average(x => x.isiCRM50),
                    isiCRM20 = resultQuery.Average(x => x.isiCRM20),

                    sislokATM100 = resultQuery.Average(x => x.sislokATM100),
                    sislokATM50 = resultQuery.Average(x => x.sislokATM50),
                    sislokATM20 = resultQuery.Average(x => x.sislokATM20),
                    sislokCRM100 = resultQuery.Average(x => x.sislokCRM100),
                    sislokCRM50 = resultQuery.Average(x => x.sislokCRM50),
                    sislokCRM20 = resultQuery.Average(x => x.sislokCRM20),
                    sislokCDM100 = resultQuery.Average(x => x.sislokCDM100),
                    sislokCDM50 = resultQuery.Average(x => x.sislokCDM50),
                    sislokCDM20 = resultQuery.Average(x => x.sislokCDM20),

                    rasio100 = resultQuery.Average(x => x.rasio100),
                    rasio50 = resultQuery.Average(x => x.rasio50),
                    rasio20 = resultQuery.Average(x => x.rasio20),
                    RasioGabungan = resultQuery.Average(x => x.RasioGabungan)
                });
            }
            this.akurasi = resultQuery;
            AkurasiForecastGridView.DataSource = resultQuery;
            for (int a = 0; a < AkurasiForecastGridView.Columns.Count; a++)
            {
                if (a == 0)
                    continue;
                AkurasiForecastGridView.Columns[a].DefaultCellStyle.Format = "0.00%";
                if (a >= AkurasiForecastGridView.ColumnCount - 4)
                    AkurasiForecastGridView.Columns[a].DefaultCellStyle.Format = "N2";
            }

            if (resultQuery.Count > 0)
                AkurasiForecastGridView.Rows[AkurasiForecastGridView.Rows.Count - 1].DefaultCellStyle.BackColor = Color.Bisque;
        }
        void loadSelisihPrediksi()
        {
            selisihPrediksi = new List<TempSelisih>();

            DateTime endDate = endDatePicker.Value.Date;
            DateTime startDate = startDatePicker.Value.Date;
            var transaksiAtms = (from x in db.TransaksiAtms
                                 where x.tanggal <= endDate && x.tanggal >= startDate
                                 select x).ToList();

            var listRealisasi = (from x in realisasi
                                 group x by x.kodePkt into g
                                 select new
                                 {
                                     kodePkt = g.Key,
                                     saldoAwal100 = g.Average(x => x.saldoAwal100),
                                     saldoAwal50 = g.Average(x => x.saldoAwal50),
                                     saldoAwal20 = g.Average(x => x.saldoAwal20),

                                     sislokATM100 = g.Average(x => x.sislokATM100),
                                     sislokATM50 = g.Average(x => x.sislokATM50),
                                     sislokATM20 = g.Average(x => x.sislokATM20),

                                     sislokCDM100 = g.Average(x => x.sislokCDM100),
                                     sislokCDM50 = g.Average(x => x.sislokCDM50),
                                     sislokCDM20 = g.Average(x => x.sislokCDM20),

                                     sislokCRM100 = g.Average(x => x.sislokCRM100),
                                     sislokCRM50 = g.Average(x => x.sislokCRM50),
                                     sislokCRM20 = g.Average(x => x.sislokCRM20),

                                     isiATM100 = g.Average(x => x.isiATM100),
                                     isiATM50 = g.Average(x => x.isiATM50),
                                     isiATM20 = g.Average(x => x.isiATM20),

                                     isiCRM100 = g.Average(x => x.isiCRM100),
                                     isiCRM50 = g.Average(x => x.isiCRM50),
                                     isiCRM20 = g.Average(x => x.isiCRM20),


                                     Rasio100 = g.Average(x => x.Rasio100),
                                     Rasio50 = g.Average(x => x.Rasio50),
                                     Rasio20 = g.Average(x => x.Rasio20),

                                     RasioGabungan = g.Average(x => x.RasioGabungan)
                                 }).ToList();
            var listApproval2 = (from x in listApproval
                                 group x by x.kodePkt into g
                                 select new
                                 {
                                     kodePkt = g.Key,
                                     saldoAwal100 = g.Average(x => x.saldoAwal100),
                                     saldoAwal50 = g.Average(x => x.saldoAwal50),
                                     saldoAwal20 = g.Average(x => x.saldoAwal20),

                                     sislokATM100 = g.Average(x => x.sislokATM100),
                                     sislokATM50 = g.Average(x => x.sislokATM50),
                                     sislokATM20 = g.Average(x => x.sislokATM20),

                                     sislokCDM100 = g.Average(x => x.sislokCDM100),
                                     sislokCDM50 = g.Average(x => x.sislokCDM50),
                                     sislokCDM20 = g.Average(x => x.sislokCDM20),

                                     sislokCRM100 = g.Average(x => x.sislokCRM100),
                                     sislokCRM50 = g.Average(x => x.sislokCRM50),
                                     sislokCRM20 = g.Average(x => x.sislokCRM20),

                                     isiATM100 = g.Average(x => x.isiATM100),
                                     isiATM50 = g.Average(x => x.isiATM50),
                                     isiATM20 = g.Average(x => x.isiATM20),

                                     isiCRM100 = g.Average(x => x.isiCRM100),
                                     isiCRM50 = g.Average(x => x.isiCRM50),
                                     isiCRM20 = g.Average(x => x.isiCRM20),


                                     Rasio100 = g.Average(x => x.Rasio100),
                                     Rasio50 = g.Average(x => x.Rasio50),
                                     Rasio20 = g.Average(x => x.Rasio20),

                                     RasioGabungan = g.Average(x => x.RasioGabungan)
                                 }).ToList();
            for (int a = 0; a < listApproval2.Count; a++)
            {
                var tempApproval = listApproval2[a];

                var tempRealisasi = listRealisasi.Where(x => x.kodePkt == tempApproval.kodePkt).FirstOrDefault();
                
                if(tempRealisasi != null)
                {
                    TempSelisih temp = new TempSelisih()
                    {
                        kodePkt = tempRealisasi.kodePkt,

                        sislokATM100 = (Int64)tempApproval.sislokATM100 - (Int64)tempRealisasi.sislokATM100,
                        sislokATM50 = (Int64)tempApproval.sislokATM50 - (Int64)tempRealisasi.sislokATM50,
                        sislokATM20 = (Int64)tempApproval.sislokATM20 - (Int64)tempRealisasi.sislokATM20,

                        sislokCDM100 = (Int64)tempApproval.sislokCDM100 - (Int64)tempRealisasi.sislokCDM100,
                        sislokCDM50 = (Int64)tempApproval.sislokCDM50 - (Int64)tempRealisasi.sislokCDM50,
                        sislokCDM20 = (Int64)tempApproval.sislokCDM20 - (Int64)tempRealisasi.sislokCDM20,

                        sislokCRM100 = (Int64)tempApproval.sislokCRM100 - (Int64)tempRealisasi.sislokCRM100,
                        sislokCRM50 = (Int64)tempApproval.sislokCRM50 - (Int64)tempRealisasi.sislokCRM50,
                        sislokCRM20 = (Int64)tempApproval.sislokCRM20 - (Int64)tempRealisasi.sislokCRM20,

                        isiATM100 = (Int64)tempApproval.isiATM100 - (Int64)tempRealisasi.isiATM100,
                        isiATM50 = (Int64)tempApproval.isiATM50 - (Int64)tempRealisasi.isiATM50,
                        isiATM20 = (Int64)tempApproval.isiATM20 - (Int64)tempRealisasi.isiATM20,

                        isiCRM100 = (Int64)tempApproval.isiCRM100 - (Int64)tempRealisasi.isiCRM100,
                        isiCRM50 = (Int64)tempApproval.isiCRM50 - (Int64)tempRealisasi.isiCRM50,
                        isiCRM20 = (Int64)tempApproval.isiCRM20 - (Int64)tempRealisasi.isiCRM20,

                        realisasiIsiATM100 = tempRealisasi.isiATM100,
                        realisasiIsiATM50 = tempRealisasi.isiATM50,
                        realisasiIsiATM20 = tempRealisasi.isiATM20,
                        realisasiIsiCRM100 = tempRealisasi.isiCRM100,
                        realisasiIsiCRM50 = tempRealisasi.isiCRM50,
                        realisasiIsiCRM20 = tempRealisasi.isiCRM20,

                        saldoAwal100 = (Int64) transaksiAtms.Where(c => c.kodePkt == tempRealisasi.kodePkt && (DateTime) c.tanggal >= startDate  && (DateTime)c.tanggal <= endDate).Select(c => c.saldoAwal100).Sum(),
                        saldoAwal50 =  (Int64) transaksiAtms.Where(c => c.kodePkt == tempRealisasi.kodePkt && (DateTime) c.tanggal >= startDate  && (DateTime)c.tanggal <= endDate).Select(c => c.saldoAwal50).Sum(),
                        saldoAwal20 =  (Int64) transaksiAtms.Where(c => c.kodePkt == tempRealisasi.kodePkt && (DateTime) c.tanggal >= startDate  && (DateTime)c.tanggal <= endDate).Select(c => c.saldoAwal20).Sum(),
                    };
                    selisihPrediksi.Add(temp);
                }
            }
            Console.WriteLine(selisihPrediksi.Count);

            Console.WriteLine("Load Selisih");
            Console.WriteLine("Selisih Prediksi Count: "+selisihPrediksi.Count);


            //Tambahin Average
            //List<TableApproval> listUntukTampil = selisihPrediksi.OrderByDescending(x => Math.Abs(ubahKolomKeBarisSelisih(x).Where(y => y > 0).Average())).ToList();
            //List<TableApproval> listUntukHitung = selisihPrediksi.OrderByDescending(x => Math.Abs(ubahKolomKeBarisSelisih(x).Where(y => y > 0).Average())).ToList();
            
            var resultQuery = (from x in selisihPrediksi
                               group x by x.kodePkt into g
                               select new
                               {
                                   kodePkt = g.Key,

                                   sislokATM100 = g.Sum(x => x.sislokATM100),
                                   sislokATM50 = g.Sum(x => x.sislokATM50),
                                   sislokATM20 = g.Sum(x => x.sislokATM20),

                                   sislokCDM100 = g.Sum(x => x.sislokCDM100),
                                   sislokCDM50 = g.Sum(x => x.sislokCDM50),
                                   sislokCDM20 = g.Sum(x => x.sislokCDM20),

                                   sislokCRM100 = g.Sum(x => x.sislokCRM100),
                                   sislokCRM50 = g.Sum(x => x.sislokCRM50),
                                   sislokCRM20 = g.Sum(x => x.sislokCRM20),

                                   isiATM100 = g.Sum(x => x.isiATM100),
                                   isiATM50 = g.Sum(x => x.isiATM50),
                                   isiATM20 = g.Sum(x => x.isiATM20),

                                   isiCRM100 = g.Sum(x => x.isiCRM100),
                                   isiCRM50 = g.Sum(x => x.isiCRM50),
                                   isiCRM20 = g.Sum(x => x.isiCRM20),
                                   rasioGabungan = calculateRasio(g.Sum(x => x.saldoAwal100 + x.saldoAwal50 + x.saldoAwal20), realisasi.Where(x=>x.kodePkt == g.Key).Sum(x =>(Int64) (x.isiCRM100 + x.isiCRM50 + x.isiCRM20)), realisasi.Where(x => x.kodePkt == g.Key).Sum(x=>(Int64)(x.isiATM100 + x.isiATM50 + x.isiATM20)))
                               }).OrderByDescending(x=>x.rasioGabungan).ToList();
            if (resultQuery.Any())
            {
                resultQuery.Add(new
                {
                    kodePkt = "Average",
                    sislokATM100 = (Int64)resultQuery.Average(x => x.sislokATM100),
                    sislokATM50 = (Int64)resultQuery.Average(x => x.sislokATM50),
                    sislokATM20 = (Int64)resultQuery.Average(x => x.sislokATM20),

                    sislokCDM100 = (Int64)resultQuery.Average(x => x.sislokCDM100),
                    sislokCDM50 = (Int64)resultQuery.Average(x => x.sislokCDM50),
                    sislokCDM20 = (Int64)resultQuery.Average(x => x.sislokCDM20),

                    sislokCRM100 = (Int64)resultQuery.Average(x => x.sislokCRM100),
                    sislokCRM50 = (Int64)resultQuery.Average(x => x.sislokCRM50),
                    sislokCRM20 = (Int64)resultQuery.Average(x => x.sislokCRM20),

                    isiATM100 = (Int64)resultQuery.Average(x => x.isiATM100),
                    isiATM50 = (Int64)resultQuery.Average(x => x.isiATM50),
                    isiATM20 = (Int64)resultQuery.Average(x => x.isiATM20),

                    isiCRM100 = (Int64)resultQuery.Average(x => x.isiCRM100),
                    isiCRM50 = (Int64)resultQuery.Average(x => x.isiCRM50),
                    isiCRM20 = (Int64)resultQuery.Average(x => x.isiCRM20),
                    rasioGabungan = (Double)0
                });
            }
            selisihPrediksiGridView.DataSource = resultQuery;

            for (int a = 1; a < selisihPrediksiGridView.Columns.Count; a++)
            {
                selisihPrediksiGridView.Columns[a].DefaultCellStyle.Format = "C";
                selisihPrediksiGridView.Columns[a].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
                if(a==selisihPrediksiGridView.ColumnCount - 1)
                    selisihPrediksiGridView.Columns[a].DefaultCellStyle.Format = "N2";
            }
            if(resultQuery.Count>0)
                selisihPrediksiGridView.Rows[selisihPrediksiGridView.Rows.Count - 1].DefaultCellStyle.BackColor = Color.Bisque;
        }
        public Double calculateRasio(Int64? saldoAwal, Int64? isiCRM, Int64? isiATM)
        {
            Console.WriteLine("Calculate Rasio");
            Console.WriteLine("SA " + saldoAwal);
            Console.WriteLine("isiCRM " + isiCRM);
            Console.WriteLine("isiATM " + isiATM);

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

        private Double hitungAverageSelisih(TableApproval var)
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
            return ret.Average();
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
        public Double RasioGabungan { set; get; }
    }

    
    public class RasioApproval
    {
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
        public Double rasio100 { set; get; }
        public Double rasio50 { set; get; }
        public Double rasio20 { set; get; }
        public Double RasioGabungan { set; get; }
    }

    public class TempSelisih
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
        public Double realisasiIsiATM100 { set; get; }
        public Double realisasiIsiATM50 { set; get; }
        public Double realisasiIsiATM20 { set; get; }
        public Double realisasiIsiCRM100 { set; get; }
        public Double realisasiIsiCRM50 { set; get; }
        public Double realisasiIsiCRM20 { set; get; }
        public Double Rasio100 { set; get; }
        public Double Rasio50 { set; get; }
        public Double Rasio20 { set; get; }
        public Double RasioGabungan { set; get; }
    }
}
