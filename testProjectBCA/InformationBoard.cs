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
    public partial class InformationBoard : Form
    {
        List<String> KodePkt;
        Int64 prediksiIsiAtm100;
        Int64 prediksiIsiAtm50;
        Int64 prediksiIsiAtm20;
        int pktIndex;
        DateTime tanggalOpti; 
        /**
            Opti itu datanya pasti selalu H+1, H+2,...
            Belom tentu semua pkt ada data opti
        **/
        public InformationBoard()
        {
            InitializeComponent();
            pktIndex = 0;
            Database1Entities db = new Database1Entities();
            List<String> tempListKodePkt = (from x in db.Pkts
                       select x.kodePkt).ToList();

            KodePkt = new List<String>();
            
            foreach(String temp2 in tempListKodePkt)
            {
                var query = (from x in db.Optis
                             where x.Cashpoint.kodePkt == temp2
                             select x).ToList();
                if (query.Count > 0)
                    KodePkt.Add(temp2);
            }
            pktComboBox.DataSource = KodePkt;
            loadPrediksi();
            /*tanggalOpti = (DateTime) query[0].tanggal*/;
            //Console.WriteLine(query[0]);
            //MessageBox.Show(tanggalOpti.ToShortDateString());
        }

        private void pktComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            pktIndex = pktComboBox.SelectedIndex;
            loadPrediksi();
        }
        private void loadPrediksi()
        {
            Database1Entities db = new Database1Entities();
            prediksiIsiAtm100 = 0;
            prediksiIsiAtm50 = 0;
            prediksiIsiAtm20 = 0;
            MessageBox.Show(KodePkt[pktIndex]);
            var query = (from x in db.Optis
                         where x.Cashpoint.kodePkt == KodePkt[pktIndex]
                         select x).ToList();
            for (int i = 0; i < query.Count; i++)
            {
                if (query[i].Cashpoint.denom == "100000")
                {
                    prediksiIsiAtm100 += (Int64)query[i].prediksi;
                }
                if (query[i].Cashpoint.denom == "50000")
                {
                    prediksiIsiAtm50 += (Int64)query[i].prediksi;
                }
                if (query[i].Cashpoint.denom == "20000")
                {
                    prediksiIsiAtm20 += (Int64)query[i].prediksi;
                }
            }
            denom100Label.Text = prediksiIsiAtm100.ToString();
            denom50Label.Text = prediksiIsiAtm50.ToString();
            denom20Label.Text = prediksiIsiAtm20.ToString();
        }
    }
}
