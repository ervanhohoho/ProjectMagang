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
    public partial class SummaryTransaksiCabang : Form
    {
        Database1Entities en = new Database1Entities();
        public SummaryTransaksiCabang()
        {
            InitializeComponent();
            reloadBulan();
            reloadTahun();
            reloadSummaryTransaksiCabang();
        }

        public void reloadBulan()
        {
            var query = (from x in en.RekonSaldoPerVendors.AsEnumerable()
                         select ((DateTime)x.dueDate).Month).Distinct().ToList();
            comboBulan.DataSource = query;
        }

        public void reloadTahun()
        {
            var query = (from x in en.RekonSaldoPerVendors.AsEnumerable()
                         select ((DateTime)x.dueDate).Year).Distinct().ToList();
            comboTahun.DataSource = query;
        }
        public void reloadSummaryTransaksiCabang()
        {
            var query = (from x in en.RekonSaldoPerVendors.AsEnumerable()
                         join y in en.Cabangs on x.cashPointtId.TrimStart('B') equals y.kodeCabang
                         where ((DateTime)x.dueDate).Month.ToString() == comboBulan.SelectedValue.ToString()
                         && ((DateTime)x.dueDate).Year.ToString() == comboTahun.SelectedValue.ToString()
                         && (x.actionRekon.Contains("Delivery") && x.statusRekon.Contains("Confirmed") 
                            || x.actionRekon.Contains("Return") && x.statusRekon.Contains("Transit"))
                         group x by new { x.cashPointtId, y.namaCabang, x.vendor, x.dueDate, blogTime = ((DateTime)x.blogTime).Date, x.actionRekon} into z
                         select new {
                             cashPointId = z.Key.cashPointtId,
                             branch = z.Key.namaCabang,
                             vendor = z.Key.vendor,
                             dueDate = z.Key.dueDate,
                             blogTime = z.Key.blogTime.Date,
                             txnType = z.Key.actionRekon,
                             amount = z.Sum(a => a.currencyAmmount)
                         }
                         ).ToList();

            dataGridView1.DataSource = query;
        }

        private void comboBulan_SelectionChangeCommitted(object sender, EventArgs e)
        {
            reloadSummaryTransaksiCabang();
        }

        private void comboTahun_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void comboTahun_SelectionChangeCommitted(object sender, EventArgs e)
        {
            reloadSummaryTransaksiCabang();
        }

        private void dataGridView1_AllowUserToAddRowsChanged(object sender, EventArgs e)
        {

        }
    }
}
