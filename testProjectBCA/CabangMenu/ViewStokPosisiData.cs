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
    public partial class ViewStokPosisiData : Form
    {
        Database1Entities db = new Database1Entities();
        List<String> listDenom = new List<String>();
        public ViewStokPosisiData()
        {
            InitializeComponent();
            dateTimePicker1.MaxDate = (DateTime) (from x in db.StokPosisis select x.tanggal).Max(x => x);
            dateTimePicker1.MinDate = (DateTime) (from x in db.StokPosisis select x.tanggal).Min(x => x);
            dateTimePicker1.Value = dateTimePicker1.MaxDate;
            loadComboPkt();
            jenisUangComboBox.DataSource = new List<String>() { "All", "Kertas", "Koin" };
            jenisUangComboBox.SelectedIndex = 0;
            loadComboDenom();
        }
        void loadComboPkt()
        {
            DateTime tanggal = dateTimePicker1.Value;
            List<String> listPkt = new List<String>();
            listPkt.Add("All");
            listPkt.AddRange((from x in db.StokPosisis
                              where x.tanggal == tanggal
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
        private void loadButton_Click(object sender, EventArgs e)
        {
            DateTime tanggal = dateTimePicker1.Value.Date;
            String pkt = pktComboBox.SelectedItem.ToString(),
                jenisUang = jenisUangComboBox.SelectedItem.ToString(),
                denom = denomComboBox.SelectedItem.ToString();
            List<StokPosisi> listData = (from x in db.StokPosisis
                                         where
                                         x.tanggal == tanggal
                                         &&
                                         (pkt == "All" ? 1==1 : x.namaPkt == pkt)
                                         &&
                                         (jenisUang == "All" ? 1==1 : x.jenis == jenisUang)
                                         &&
                                         (denom == "All" ? 1==1 : x.denom == denom)
                                         select x).ToList();
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
    }
}
