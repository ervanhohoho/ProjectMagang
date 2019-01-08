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
    public partial class ReportRekonSaldo : Form
    {
        Database1Entities en = new Database1Entities();
        public ReportRekonSaldo()
        {
            InitializeComponent();
            reloadBulan();
            reloadTahun();
            reloadReportRekonSaldo();
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

        public void reloadReportRekonSaldo()
        {
            var query = (from x in en.RekonSaldoPerVendors.AsEnumerable()
                         join y in en.Cabangs on x.cashPointtId.TrimStart('B') equals y.kodeCabang
                         where ((DateTime)x.dueDate).Date.ToString() != ((DateTime)x.blogTime).Date.ToString()
                         && ((DateTime)x.dueDate).Month.ToString() == comboBulan.SelectedValue.ToString()
                         && ((DateTime)x.dueDate).Year.ToString() == comboTahun.SelectedValue.ToString()
                         && (x.actionRekon.Contains("Delivery") && x.statusRekon.Contains("Confirmed")
                            || x.actionRekon.Contains("Return") && x.statusRekon.Contains("Transit"))
                         group x by new { x.cashPointtId, x.vendor, y.namaCabang } into z
                         select new
                         {
                             cashPointId = z.Key.cashPointtId,
                             branch = z.Key.namaCabang,
                             vendor = z.Key.vendor,
                             count = z.Count()
                         }
                         ).ToList();

            dataGridView1.DataSource = query;
        }

        private void comboTahun_SelectionChangeCommitted(object sender, EventArgs e)
        {
            reloadReportRekonSaldo();
        }

        private void comboBulan_SelectionChangeCommitted(object sender, EventArgs e)
        {
            reloadReportRekonSaldo();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}