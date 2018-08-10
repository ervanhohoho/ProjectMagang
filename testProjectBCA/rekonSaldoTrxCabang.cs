using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testProjectBCA
{
    public partial class rekonSaldoTrxCabang : Form
    {
        Database1Entities en = new Database1Entities();
        public rekonSaldoTrxCabang()
        {
            InitializeComponent();
            comboSetBon.SelectedIndex = 0;
            comboVal.SelectedIndex = 0;
            reloadBonBelum();
        }

        private void buttonGenerate_Click(object sender, EventArgs e)
        {
            if (comboSetBon.SelectedIndex == 0 && comboVal.SelectedIndex == 0)
            {
                reloadSetoranSudah();
            }
            else if (comboSetBon.SelectedIndex == 0 && comboVal.SelectedIndex == 1)
            {
                reloadSetoranBelum();
            }
            else if (comboSetBon.SelectedIndex == 1 && comboVal.SelectedIndex == 0)
            {
                reloadBonSudah();
            }
            else if (comboSetBon.SelectedIndex == 1 && comboVal.SelectedIndex == 1)
            {
                reloadBonBelum();
            }
        }
        
        public void reloadSetoranBelum()
        {
            DateTime date1 = dateTimePicker1.Value.Date;
            DateTime date2 = dateTimePicker2.Value.Date;
            var query = (from x in en.RekonSaldoPerVendors
                         where x.actionRekon.Contains("Return") && x.validation.Equals("NOT VALIDATED") && x.statusRekon.Equals("In Transit")
                         && x.dueDate >= date1 && x.dueDate <= date2
                         group x by new { x.dueDate, x.cashPointtId } into z
                         select new { dueDate = z.Key.dueDate, cashPointId = z.Key.cashPointtId, currencyAmmount = z.Sum(x => x.currencyAmmount) }).ToList();

            dataGridView1.DataSource = query;

        }
        public void reloadSetoranSudah()
        {
            DateTime date1 = dateTimePicker1.Value.Date;
            DateTime date2 = dateTimePicker2.Value.Date;
            var query = (from x in en.RekonSaldoPerVendors
                         where x.actionRekon.Contains("Return") && x.validation.Equals("VALIDATED") && x.statusRekon.Equals("In Transit")
                         && x.dueDate >= date1 && x.dueDate <= date2
                         group x by new { x.dueDate, x.cashPointtId } into z
                         select new { dueDate = z.Key.dueDate, cashPointId = z.Key.cashPointtId, currencyAmmount = z.Sum(x => x.currencyAmmount) }).ToList();

            dataGridView1.DataSource = query;
        }
        public void reloadBonSudah()
        {
            DateTime date1 = dateTimePicker1.Value.Date;
            DateTime date2 = dateTimePicker2.Value.Date;
            var query = (from x in en.RekonSaldoPerVendors
                         where x.actionRekon.Contains("Delivery") && x.validation.Equals("NOT VALIDATED") && x.statusRekon.Equals("Confirmed")
                         && x.dueDate >= date1 && x.dueDate <= date2
                         group x by new { x.dueDate, x.cashPointtId } into z
                         select new {dueDate = z.Key.dueDate , cashPointId = z.Key.cashPointtId ,currencyAmmount = z.Sum(x => x.currencyAmmount) } ).ToList();

            dataGridView1.DataSource = query;
        }
        public void reloadBonBelum()
        {
            DateTime date1 = dateTimePicker1.Value.Date;
            DateTime date2 = dateTimePicker2.Value.Date;
            var query = (from x in en.RekonSaldoPerVendors
                         where x.actionRekon.Contains("Delivery") && x.validation.Equals("VALIDATED") && x.statusRekon.Equals("Confirmed")
                         && x.dueDate >= date1 && x.dueDate <= date2
                         group x by new { x.dueDate, x.cashPointtId } into z
                         select new { dueDate = z.Key.dueDate, cashPointId = z.Key.cashPointtId, currencyAmmount = z.Sum(x => x.currencyAmmount) }).ToList();

            dataGridView1.DataSource = query;
        }

        class loadData
        {
            public DateTime tanggal { set; get; }
            public String kodeCabang { set; get; }
            public String nominal { set; get; }
        }

    }
}
