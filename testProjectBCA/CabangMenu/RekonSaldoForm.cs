using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testProjectBCA.CabangMenu
{
    public partial class RekonSaldoForm : Form
    {
        String kodePkt;
        List<PivotCPC_BIBL> listBIdanBankLain = new List<PivotCPC_BIBL>();
        List<PivotPerVendor_setoran> listPivotPerVendorSetoran = new List<PivotPerVendor_setoran>();
        public RekonSaldoForm()
        {
            InitializeComponent();
            loadComboPkt();
            pktComboBox.SelectedIndex = 0;
        }
        void loadComboPkt()
        {
            Database1Entities db = new Database1Entities();
            List<String> listKodePkt = db.Pkts.Where(x => x.kanwil.ToLower().Contains("jabo") && x.kodePktCabang.Length > 1).Select(x => x.kodePktCabang).OrderBy(x=>x).ToList();
            pktComboBox.DataSource = listKodePkt;
        }
        private void loadBtn_Click(object sender, EventArgs e)
        {
            loadListBIdanBankLain();
            loadListSetoranCabang();
            dataSistemGridView.DataSource = listPivotPerVendorSetoran;
        }
        private void loadListBIdanBankLain()
        {
            Database1Entities db = new Database1Entities();

            var query = (from x in db.RekonSaldoVaults.AsEnumerable()
                         where !String.IsNullOrEmpty(x.fundingSoure)
                         && x.vaultId == kodePkt
                         select x).ToList();
            query = (from x in query
                     where (x.fundingSoure == "BI" || x.fundingSoure.Contains("OB")) && (((DateTime)x.dueDate).Date == dateTimePicker1.Value.Date || ((DateTime)x.realDate).Date == dateTimePicker1.Value.Date)
                     select x).ToList();

            List<PivotCPC> pc = new List<PivotCPC>();

            String bufferVaultId = "";

            foreach (var item in query)
            {
                pc.Add(new PivotCPC
                {
                    vaultId = item.vaultId,
                    confId = item.confId,
                    action = item.actionRekon,
                    status = item.statusRekon,
                    blogMessage = item.blogMessage,
                    orderDate = ((DateTime)item.orderDate).Date,
                    dueDate = ((DateTime)item.dueDate).Date,
                    timeStamp = (DateTime)item.timeStampRekon,
                    currencyAmmount = Int64.Parse(item.currencyAmmount.ToString()),
                    fundingSource = item.fundingSoure,
                    realDate = ((DateTime)item.timeStampRekon).Hour < 21 ? ((DateTime)item.timeStampRekon).Date : ((DateTime)item.timeStampRekon).AddDays(1).Date,
                    validation = item.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(item.timeStampRekon.ToString()).Hour < 21 ? item.timeStampRekon : DateTime.Parse(item.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker1.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                });

            }
            var pivot = pc.GroupBy(c => new { c.vaultId, c.fundingSource, c.dueDate, c.realDate }).Select(g => new
            {
                dueDate = g.Key.dueDate,
                realDate = g.Key.realDate,
                vaultID = g.Key.vaultId,
                fundingSource = g.Key.fundingSource,
                emergencyReturn = g.Where(c => c.action == "Emergency Return").Sum(c => c.currencyAmmount),
                emergencyReturnVal = g.Where(c => c.action == "Emergency Return" && c.validation == "VALIDATED").Sum(c => c.currencyAmmount),
                emergencyReturnNotVal = g.Where(c => c.action == "Emergency Return" && c.validation == "NOT VALIDATED").Sum(c => c.currencyAmmount),
                plannedReturn = g.Where(c => c.action == "Planned Return").Sum(c => c.currencyAmmount),
                plannedReturnVal = g.Where(c => c.action == "Planned Return" && c.validation == "VALIDATED").Sum(c => c.currencyAmmount),
                plannedReturnNotVal = g.Where(c => c.action == "Planned Return" && c.validation == "NOT VALIDATED").Sum(c => c.currencyAmmount)

            }).ToList();
            var pivot2 = (from x in pivot
                          select new { x.dueDate, x.vaultID, x.fundingSource, x.emergencyReturn, x.emergencyReturnVal, x.emergencyReturnNotVal, x.plannedReturn, x.plannedReturnVal, x.plannedReturnNotVal, grandTotal = x.emergencyReturn + x.plannedReturn, x.realDate });

            var bibl = new List<PivotCPC_BIBL>();

            foreach (var item in pivot2)
            {
                if (!(item.plannedReturn == 0 && item.emergencyReturn == 0))
                {
                    bibl.Add(new PivotCPC_BIBL
                    {
                        dueDate = item.dueDate,
                        realDate = item.realDate,
                        vaultId = item.vaultID,
                        fundingSource = item.fundingSource,
                        //emergencyReturn = item.emergencyReturn,
                        emergencyReturnVal = item.emergencyReturnVal,
                        emergencyReturnNotVal = item.emergencyReturnNotVal,
                        //plannedReturn = item.plannedReturn,
                        plannedReturnVal = item.plannedReturnVal,
                        plannedReturnNotVal = item.plannedReturnNotVal,
                        grandTotal = item.grandTotal
                    });
                }

            }

            listBIdanBankLain = bibl;
        }
        private void loadListSetoranCabang()
        {
            Database1Entities db = new Database1Entities();
            var queryS = (from x in db.RekonSaldoPerVendors.AsEnumerable()
                          where !String.IsNullOrEmpty(x.vendor) && x.vendor == kodePkt
                          select x).ToList();
            var querysetoran = (from x in queryS
                                where (x.actionRekon.Contains("Return") && x.statusRekon.Equals("In Transit")) && (((DateTime)x.dueDate).Date == dateTimePicker1.Value.Date || ((DateTime)x.realDate).Date == dateTimePicker1.Value.Date)
                                select new
                                {
                                    cashPointId = x.cashPointtId,
                                    confId = x.confId,
                                    orderDate = x.orderDate,
                                    vendor = x.vendor,
                                    actionRekon = x.actionRekon,
                                    statusRekon = x.statusRekon,
                                    dueDate = x.dueDate,
                                    blogTime = x.blogTime,
                                    currencyAmmount = x.currencyAmmount,
                                    realDate = x.realDate,
                                    blogMessage = x.blogMessage,
                                    validation = x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker1.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                                    //DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED"
                                }).ToList();
            var ppvs = new List<PivotPerVendor_setoran>();

            var pivotsetoran = querysetoran.GroupBy(c => new { c.vendor, c.dueDate, c.realDate }).Select(g => new
            {
                dueDate = g.Key.dueDate,
                valDate = g.Key.realDate,
                vendor = g.Key.vendor,
                belumValidasi = g.Where(c => c.validation == "NOT VALIDATED").Sum(c => c.currencyAmmount),
                sudahValidasi = g.Where(c => c.validation == "VALIDATED").Sum(c => c.currencyAmmount),
                grandTotal = g.Where(c => c.validation == "NOT VALIDATED").Sum(c => c.currencyAmmount) +
                             g.Where(c => c.validation == "VALIDATED").Sum(c => c.currencyAmmount)

            }).ToList();

            foreach (var item in pivotsetoran)
            {
                ppvs.Add(new PivotPerVendor_setoran
                {
                    dueDate = ((DateTime)item.dueDate).Date,
                    valDate = ((DateTime)item.valDate).Date,
                    vendor = item.vendor,
                    belumValidasi = (Int64)item.belumValidasi,
                    sudahValidasi = (Int64)item.sudahValidasi,
                    grandTotal = (Int64)item.grandTotal
                });
            }

            listPivotPerVendorSetoran = ppvs;
        }
        private void pktComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            kodePkt = pktComboBox.SelectedItem.ToString();
        }
    }
}
