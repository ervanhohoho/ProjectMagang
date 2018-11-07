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

namespace testProjectBCA.CabangMenu
{
    public partial class RekonSaldoForm : Form
    {
        Database1Entities db = new Database1Entities();
        DateTime bulanDanTahun;
        String kodePkt;
        List<TampilanRekonSaldo> tampilanRekonSaldos = new List<TampilanRekonSaldo>();
        public RekonSaldoForm()
        {
            InitializeComponent();
            loadComboTahun();
            loadComboBulan();
            loadComboPkt();

        }
        void loadComboTahun()
        {
            List<int> listTahun = (from x in db.RekonSaldoPerVendors
                                   join y in db.RekonSaldoVaults on new { x.orderDate, x.vendor } equals new { y.orderDate, vendor = y.vaultId }
                                   select ((DateTime)x.orderDate).Year
                                   ).Distinct().ToList();
            tahunComboBox.DataSource = listTahun;
            tahunComboBox.SelectedIndex = 0;
        }
        void loadComboBulan()
        {
            int tahun = Int32.Parse(tahunComboBox.SelectedItem.ToString());
            List<int> listBulan = (from x in db.RekonSaldoPerVendors
                                    join y in db.RekonSaldoVaults on new { x.orderDate, x.vendor } equals new { y.orderDate, vendor = y.vaultId }
                                    where ((DateTime)x.orderDate).Year == tahun
                                   select ((DateTime)x.orderDate).Month
                                ).Distinct().ToList();
            bulanComboBox.DataSource = listBulan;
            bulanComboBox.SelectedIndex = 0;
        }
        void loadComboPkt()
        {
            int tahun = Int32.Parse(tahunComboBox.SelectedItem.ToString()),
                bulan = Int32.Parse(bulanComboBox.SelectedItem.ToString());

            List<String> listPkt = (
                from x in db.RekonSaldoPerVendors
                join y in db.RekonSaldoVaults on new { x.orderDate, x.vendor } equals new { y.orderDate, vendor = y.vaultId }
                where ((DateTime)x.orderDate).Year == tahun && ((DateTime)x.orderDate).Month == bulan
                select x.vendor
                ).Distinct().ToList();
            pktComboBox.DataSource = listPkt;
            pktComboBox.SelectedIndex = 0;
            kodePkt = pktComboBox.SelectedItem.ToString();
        }
        private TampilanRekonSaldo loadDataUntukTampil(DateTime tanggal)
        {
            //START DARI SISTEM
            KumpulanQueryRekonSaldo kqrs = new KumpulanQueryRekonSaldo(kodePkt, tanggal);
            List<String> listKodePktCabang = db.Pkts.Select(x => x.kodePktCabang).Where(x => x.Length > 1).ToList();
            List<PivotCPC_BIBL> listReturnBIdanBankLain = new List<PivotCPC_BIBL>();
            List<PivotPerVendor_setoran> listSetoranCabang = new List<PivotPerVendor_setoran>();
            List<PivotCPC_ATMR> listATMReturn;
            List<PivotCPC_BIBLD> listDeliveryBIdanBankLain = new List<PivotCPC_BIBLD>();
            List<PivotPerVendor_bon> listBonCabang = new List<PivotPerVendor_bon>();
            List<PivotCPC_ATMD> listATMDelivery;
            listReturnBIdanBankLain = kqrs.listReturnBIdanBankLain;
            listSetoranCabang = kqrs.listSetoranCabang;
            listATMReturn = kqrs.listATMReturn;
            listDeliveryBIdanBankLain = kqrs.listDeliveryBIdanBankLain;
            listBonCabang = kqrs.listBonCabang;
            listATMDelivery = kqrs.listATMDelivery;

            Int64 inCabang = 0,
                inAtmCdm = 0,
                inBi = 0,
                inBankLain = 0,
                outCabang = 0,
                outAtmCdm = 0,
                outBi = 0,
                outBankLain = 0;
            
            //IN
            listSetoranCabang = listSetoranCabang.GroupBy(x => x.vendor).Select(x => new PivotPerVendor_setoran()
            {
                vendor = x.Key,
                belumValidasi = x.Sum(z => z.belumValidasi),
                sudahValidasi = x.Sum(z => z.sudahValidasi),
                grandTotal = x.Sum(z => z.grandTotal),
            }).ToList();
            //SETOR
            if (listSetoranCabang.Any())
                inCabang = listSetoranCabang[0].sudahValidasi + listSetoranCabang[0].belumValidasi;
            inBi = listReturnBIdanBankLain.Where(x => x.fundingSource.ToLower() == "bi").GroupBy(x => x).Select(x => x.Sum(y => y.plannedReturnNotVal) + x.Sum(y => y.emergencyReturnNotVal) + x.Sum(y => y.plannedReturnVal) + x.Sum(y => y.emergencyReturnVal)).FirstOrDefault();
            inBankLain = listReturnBIdanBankLain.Where(x => x.fundingSource.ToLower() != "bi").GroupBy(x => x).Select(x => x.Sum(y => y.plannedReturnNotVal) + x.Sum(y => y.emergencyReturnNotVal) + x.Sum(y => y.plannedReturnVal) + x.Sum(y => y.emergencyReturnVal)).FirstOrDefault();
            inAtmCdm = listATMReturn.Where(x => !listKodePktCabang.Where(y => y == x.fundingSource).ToList().Any()).GroupBy(x => x).Select(x => x.Sum(y => y.plannedReturnNotVal + y.emergencyReturnNotVal + y.plannedReturnVal + y.emergencyReturnVal)).FirstOrDefault();

            //IN
            listBonCabang = listBonCabang.GroupBy(x => x.vendor).Select(x => new PivotPerVendor_bon()
            {
                vendor = x.Key,
                belumValidasi = x.Sum(z => z.belumValidasi),
                sudahValidasi = x.Sum(z => z.sudahValidasi),
                grandTotal = x.Sum(z => z.grandTotal),
            }).ToList();
            if (listBonCabang.Any())
                outCabang = listBonCabang[0].sudahValidasi + listBonCabang[0].belumValidasi;

            outBi = listDeliveryBIdanBankLain.Where(x => x.fundingSource.ToLower() == "bi").GroupBy(x => x).Select(x => x.Sum(y => y.plannedDeliveryNotVal) + x.Sum(y => y.emergencyDeliveryNotVal) + x.Sum(y => y.plannedDeliveryVal) + x.Sum(y => y.emergencyDeliveryVal)).FirstOrDefault();
            outBankLain = listDeliveryBIdanBankLain.Where(x => x.fundingSource.ToLower() != "bi").GroupBy(x => x).Select(x => x.Sum(y => y.plannedDeliveryNotVal) + x.Sum(y => y.emergencyDeliveryNotVal) + x.Sum(y => y.plannedDeliveryVal) + x.Sum(y => y.emergencyDeliveryVal)).FirstOrDefault();
            outAtmCdm = listATMDelivery.Where(x => !listKodePktCabang.Where(y => y == x.fundingSource).ToList().Any()).GroupBy(x => x).Select(x => x.Sum(y => y.plannedDeliveryNotVal + y.emergencyDeliveryNotVal + y.plannedDeliveryVal + y.emergencyDeliveryVal)).FirstOrDefault();
            //END Dari Sistem


            //START INPUTAN USER
            List<String> jenisInputanUser = new List<String>() {
                "Cabang",
                "Retail",
                "Curex",
                "ATM/CDM",
                "BI",
                "Bank Lain",
                "Luar Kota",
                "Antar CPC",
                "Lain Lain"
            };
            List<RekonSaldoInputanUser> rekonSaldoInputanUsers = db.RekonSaldoInputanUsers.Where(x => x.tanggal == tanggal && x.kodePkt == kodePkt).ToList();
            var groupedRekonSaldoInputanUsers = rekonSaldoInputanUsers.GroupBy(x => new { x.in_out, x.jenis }).Select(x => new { jenis = x.Key.jenis, in_out = x.Key.in_out, value = x.Sum(y => y.value) }).ToList();

            Int64 userInCabang = 0,
                userInRetail = 0,
                userInCurex = 0,
                userInAtmCdm = 0,
                userInBi = 0,
                userInBankLain = 0,
                userInLuarKota = 0,
                userInAntarCpc = 0,
                userInLainLain = 0,
                userOutCabang = 0,
                userOutRetail = 0,
                userOutCurex = 0,
                userOutAtmCdm = 0,
                userOutBi = 0,
                userOutBankLain = 0,
                userOutLuarKota = 0,
                userOutAntarCpc = 0,
                userOutLainLain = 0;
            Int64? temp = 0;
            //IN 
            temp = groupedRekonSaldoInputanUsers.Where(x => x.jenis == jenisInputanUser[0] && x.in_out == "in").Select(x => x.value).FirstOrDefault();
            if (temp != null)
                userInCabang = (Int64)temp;
            temp = groupedRekonSaldoInputanUsers.Where(x => x.jenis == jenisInputanUser[1] && x.in_out == "in").Select(x => x.value).FirstOrDefault();
            if (temp != null)
                userInRetail= (Int64)temp;
            temp = groupedRekonSaldoInputanUsers.Where(x => x.jenis == jenisInputanUser[2] && x.in_out == "in").Select(x => x.value).FirstOrDefault();
            if (temp != null)
                userInCurex= (Int64)temp;
            temp = groupedRekonSaldoInputanUsers.Where(x => x.jenis == jenisInputanUser[3] && x.in_out == "in").Select(x => x.value).FirstOrDefault();
            if (temp != null)
                userInAtmCdm= (Int64)temp;
            temp = groupedRekonSaldoInputanUsers.Where(x => x.jenis == jenisInputanUser[4] && x.in_out == "in").Select(x => x.value).FirstOrDefault();
            if (temp != null)
                userInBi= (Int64)temp;
            temp = groupedRekonSaldoInputanUsers.Where(x => x.jenis == jenisInputanUser[5] && x.in_out == "in").Select(x => x.value).FirstOrDefault();
            if (temp != null)
                userInBankLain = (Int64)temp;
            temp = groupedRekonSaldoInputanUsers.Where(x => x.jenis == jenisInputanUser[6] && x.in_out == "in").Select(x => x.value).FirstOrDefault();
            if (temp != null)
                userInLuarKota= (Int64)temp;
            temp = groupedRekonSaldoInputanUsers.Where(x => x.jenis == jenisInputanUser[7] && x.in_out == "in").Select(x => x.value).FirstOrDefault();
            if (temp != null)
                userInAntarCpc = (Int64)temp;
            temp = groupedRekonSaldoInputanUsers.Where(x => x.jenis == jenisInputanUser[8] && x.in_out == "in").Select(x => x.value).FirstOrDefault();
            if (temp != null)
                userInLainLain = (Int64)temp;
            //OUT
            temp = groupedRekonSaldoInputanUsers.Where(x => x.jenis == jenisInputanUser[0] && x.in_out == "out").Select(x => x.value).FirstOrDefault();
            if (temp != null)
                userOutCabang = (Int64)temp;
            temp = groupedRekonSaldoInputanUsers.Where(x => x.jenis == jenisInputanUser[1] && x.in_out == "out").Select(x => x.value).FirstOrDefault();
            if (temp != null)
                userOutRetail = (Int64)temp;
            temp = groupedRekonSaldoInputanUsers.Where(x => x.jenis == jenisInputanUser[2] && x.in_out == "out").Select(x => x.value).FirstOrDefault();
            if (temp != null)
                userOutCurex = (Int64)temp;
            temp = groupedRekonSaldoInputanUsers.Where(x => x.jenis == jenisInputanUser[3] && x.in_out == "out").Select(x => x.value).FirstOrDefault();
            if (temp != null)
                userOutAtmCdm = (Int64)temp;
            temp = groupedRekonSaldoInputanUsers.Where(x => x.jenis == jenisInputanUser[4] && x.in_out == "out").Select(x => x.value).FirstOrDefault();
            if (temp != null)
                userOutBi = (Int64)temp;
            temp = groupedRekonSaldoInputanUsers.Where(x => x.jenis == jenisInputanUser[5] && x.in_out == "out").Select(x => x.value).FirstOrDefault();
            if (temp != null)
                userOutBankLain = (Int64)temp;
            temp = groupedRekonSaldoInputanUsers.Where(x => x.jenis == jenisInputanUser[6] && x.in_out == "out").Select(x => x.value).FirstOrDefault();
            if (temp != null)
                userOutLuarKota = (Int64)temp;
            temp = groupedRekonSaldoInputanUsers.Where(x => x.jenis == jenisInputanUser[7] && x.in_out == "out").Select(x => x.value).FirstOrDefault();
            if (temp != null)
                userOutAntarCpc = (Int64)temp;
            temp = groupedRekonSaldoInputanUsers.Where(x => x.jenis == jenisInputanUser[8] && x.in_out == "out").Select(x => x.value).FirstOrDefault();
            if (temp != null)
                userOutLainLain = (Int64)temp;



            TampilanRekonSaldo ret = new TampilanRekonSaldo() {
                tanggal = tanggal,
                inCabang = inCabang + userInCabang,
                inRetail = userInRetail,
                inCurex = userInCurex,
                inAtmCdm = inAtmCdm + userInAtmCdm,
                inBi =inBi + userInBi,
                inBankLain = inBankLain + userInBankLain,
                inLuarKota = userInLuarKota,
                inAntarCpc = userInAntarCpc,
                inLainLain = userInLainLain,
                outCabang = outCabang + userOutCabang,
                outRetail = userOutRetail,
                outCurex = userOutCurex,
                outAtmCdm = outAtmCdm + userOutAtmCdm,
                outBi = outBi + userOutBi,
                outBankLain = outBankLain + userOutBankLain,
                outLuarKota = userOutLuarKota,
                outAntarCpc = userOutAntarCpc,
                outLainLain = userOutLainLain
            };
            return ret;
        }
        private void loadBtn_Click(object sender, EventArgs e)
        {
            loadForm.ShowSplashScreen();

            int bulan = Int32.Parse(bulanComboBox.SelectedItem.ToString()), 
                tahun = Int32.Parse(tahunComboBox.SelectedItem.ToString());
            String kodePkt = pktComboBox.SelectedItem.ToString();
            DateTime maxTanggal = new DateTime(tahun, bulan, DateTime.DaysInMonth(tahun,bulan));
            DateTime minTanggal = new DateTime(tahun, bulan, 1);
            DateTime currTanggal = minTanggal;
            List<TampilanRekonSaldo> tampilanRekonSaldos = new List<TampilanRekonSaldo>();
            
            while(currTanggal<=maxTanggal)
            {
                tampilanRekonSaldos.Add(loadDataUntukTampil(currTanggal));
                currTanggal = currTanggal.AddDays(1);

            }
            this.tampilanRekonSaldos = tampilanRekonSaldos;

            for(int a=1;a<this.tampilanRekonSaldos.Count;a++)
            {
                var prev = this.tampilanRekonSaldos[a - 1];
                this.tampilanRekonSaldos[a].saldoAwal = 
                    prev.saldoAwal 
                    + prev.inCabang 
                    + prev.inRetail 
                    + prev.inCurex 
                    + prev.inBi 
                    + prev.inBankLain 
                    + prev.inAntarCpc 
                    + prev.inAtmCdm 
                    + prev.inLainLain 
                    + prev.inLuarKota
                    - prev.outCabang
                    - prev.outRetail
                    - prev.outCurex
                    - prev.outBi
                    - prev.outBankLain
                    - prev.outAntarCpc
                    - prev.outAtmCdm
                    - prev.outLainLain
                    - prev.outLuarKota;
            }
            dataGridView1.DataSource = this.tampilanRekonSaldos;
            dataGridView1.Columns[0].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
            dataGridView1.Columns[0].ReadOnly = true;
            for (int a = 1; a < dataGridView1.Columns.Count; a++)
            {
                dataGridView1.Columns[a].DefaultCellStyle.Format = "C0";
                dataGridView1.Columns[a].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
                dataGridView1.Columns[a].ReadOnly = true;
            }
            dataGridView1.Rows[0].Cells[1].ReadOnly = false;

            loadForm.CloseForm();

        }
        private void tahunComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            loadComboBulan();
        }

        private void pktComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            kodePkt = pktComboBox.SelectedItem.ToString();
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            for (int a = 1; a < this.tampilanRekonSaldos.Count; a++)
            {
                var prev = this.tampilanRekonSaldos[a - 1];
                this.tampilanRekonSaldos[a].saldoAwal =
                    prev.saldoAwal
                    + prev.inCabang
                    + prev.inRetail
                    + prev.inCurex
                    + prev.inBi
                    + prev.inBankLain
                    + prev.inAntarCpc
                    + prev.inAtmCdm
                    + prev.inLainLain
                    + prev.inLuarKota
                    - prev.outCabang
                    - prev.outRetail
                    - prev.outCurex
                    - prev.outBi
                    - prev.outBankLain
                    - prev.outAntarCpc
                    - prev.outAtmCdm
                    - prev.outLainLain
                    - prev.outLuarKota;
            }
            dataGridView1.DataSource = this.tampilanRekonSaldos;
            dataGridView1.Refresh();
        }

        class TampilanRekonSaldo
        {
            public DateTime tanggal { set; get; }
            public Int64 saldoAwal { set; get; }
            public Int64 inCabang { set; get; }
            public Int64 inRetail { set; get; }
            public Int64 inCurex { set; get; }
            public Int64 inAtmCdm { set; get; }
            public Int64 inBi { set; get; }
            public Int64 inBankLain { set; get; }
            public Int64 inLuarKota { set; get; }
            public Int64 inAntarCpc { set; get; }
            public Int64 inLainLain { set; get; }

            public Int64 outCabang { set; get; }
            public Int64 outRetail { set; get; }
            public Int64 outCurex { set; get; }
            public Int64 outAtmCdm { set; get; }
            public Int64 outBi { set; get; }
            public Int64 outBankLain { set; get; }
            public Int64 outLuarKota { set; get; }
            public Int64 outAntarCpc { set; get; }
            public Int64 outLainLain { set; get; }
        }
    }
    
}
