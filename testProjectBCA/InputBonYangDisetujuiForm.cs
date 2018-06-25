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
    public partial class InputBonYangDisetujuiForm : Form
    {
        Database1Entities db = new Database1Entities();
        public InputBonYangDisetujuiForm()
        {
            InitializeComponent();
            comboBox1.DataSource = (from x in db.Pkts
                                    orderby x.kodePkt ascending
                                    select x.kodePkt).ToList();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String kodePkt = comboBox1.SelectedItem.ToString();
            Int64 d100 = (Int64) bon100Num.Value, 
                  d50 = (Int64) bon50Num.Value, 
                  d20 = (Int64) bon20Num.Value;
            DateTime tanggal = dateTimePicker1.Value.Date;
            var q = (from x in db.laporanBons
                     where x.kodePkt == kodePkt
                     && x.tanggal == tanggal
                     select x).FirstOrDefault();
            if(q!=null)
            {
                q.C100 = d100;
                q.C50 = d50;
                q.C20 = d20;
            }
            else
            {
                db.laporanBons.Add(new laporanBon()
                {
                    kodePkt = kodePkt,
                    tanggal = tanggal,
                    C100 = d100,
                    C50 = d50,
                    C20 = d20
                });
            }
            db.SaveChanges();
        }
    }
}
