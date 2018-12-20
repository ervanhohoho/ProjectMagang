using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testProjectBCA.ATM
{
    public partial class ProyeksiATMForm : Form
    {
        Database1Entities db = new Database1Entities();
        public ProyeksiATMForm()
        {
            InitializeComponent();
            loadComboMetode();
            loadComboPkt();
            loadTreeView();
        }
        private void loadComboPkt()
        {
            List<String> listPkt = (from x in db.Pkts.AsEnumerable()
                                    select x.kodePktATM).Where(x=>!String.IsNullOrWhiteSpace(x)).OrderBy(x=>x).Distinct().ToList();
            KodePktCombo.DataSource = listPkt;
        }
        private void loadComboMetode()
        {
            MetodePenghitunganCombo.DataSource = Variables.listMetodeNonATM;
        }
        private void LoadBtn_Click(object sender, EventArgs e)
        {
            DateTime startDate = StartDatePicker.Value.Date,
                endDate = EndDatePicker.Value.Date,
                currDate = startDate;
            String kodePkt = KodePktCombo.SelectedItem.ToString(),
                metode = MetodePenghitunganCombo.SelectedItem.ToString();
            List<DateTime> kumpulanTanggal = new List<DateTime>();

            while(currDate <= endDate)
            {
                kumpulanTanggal.Add(currDate);
                currDate = currDate.AddDays(1);
            }
            KumpulanPrediksi prediksi = new KumpulanPrediksi(kodePkt, loadKumpulanTanggalUntukPrediksi(), startDate, endDate, metode, metode);
            List<String> eventType = prediksi.eventType;
            List<JenisEvent> listEvent = new List<JenisEvent>();
            
            for(int a = 0; a< eventType.Count;a++)
            {
                listEvent.Add(new JenisEvent() { tgl = startDate.AddDays(a), eventType = eventType[a].Split(' ')[3] + eventType[a].Split(' ')[4]});
            }

            var query = (from isiATM in prediksi.prediksiIsiAtm
                         join isiCRM in prediksi.isiCrm2 on isiATM.tgl equals isiCRM.tgl
                         join sislokATM in prediksi.rasioSislokAtm on isiATM.tgl equals sislokATM.tgl
                         join sislokCRM in prediksi.sislokCrm on isiATM.tgl equals sislokCRM.tgl
                         join sislokCDM in prediksi.sislokCdm on isiATM.tgl equals sislokCDM.tgl
                         join events in listEvent on isiATM.tgl equals events.tgl
                         select new ViewProyeksiAtm()
                         {
                             tanggal = isiATM.tgl,
                             isiATM100 = isiATM.d100,
                             isiATM50 = isiATM.d50,
                             isiATM20 = isiATM.d20,
                             isiCRM100 = isiCRM.d100,
                             isiCRM50 = isiCRM.d50,
                             isiCRM20 = isiCRM.d20,
                             sislokATM100 = (Int64) Math.Round(sislokATM.d100 * isiATM.d100),
                             sislokATM50 = (Int64)Math.Round(sislokATM.d50 * isiATM.d50),
                             sislokATM20 = (Int64)Math.Round(sislokATM.d20 * isiATM.d20),
                             sislokCRM100 = sislokCRM.d100,
                             sislokCRM50 = sislokCRM.d50,
                             sislokCRM20 = sislokCRM.d20,
                             sislokCDM100 = sislokCDM.d100,
                             sislokCDM50 = sislokCDM.d50,
                             sislokCDM20 = sislokCDM.d20,
                             eventType = events.eventType
                         }).ToList();
            dataGridView1.DataSource = query;

            for(int a=0;a<dataGridView1.ColumnCount;a++)
            {
                if (dataGridView1.Columns[a].ValueType == typeof(Int64))
                {
                    dataGridView1.Columns[a].DefaultCellStyle.Format = "C0";
                    dataGridView1.Columns[a].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
                }
            }
        }
        void loadTreeView()
        {
            //Load Tree
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    cmd.CommandText = "SELECT MIN(Tanggal), MAX(Tanggal) FROM TransaksiAtms";
                    sql.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        DateTime minTanggal = (DateTime)reader[0];
                        DateTime maxTanggal = (DateTime)reader[1];
                        Console.WriteLine("Min Date: " + minTanggal.ToShortDateString());
                        Console.WriteLine("Max Date: " + maxTanggal.ToShortDateString());

                        DateTime tempTanggal = new DateTime(minTanggal.Year, minTanggal.Month, 1);
                        int counter = 0;
                        bool firstRun = true;
                        while (tempTanggal <= maxTanggal)
                        {
                            treeView1.Nodes.Add(tempTanggal.Year.ToString());
                            int monthCounter;
                            if (firstRun)
                            {
                                monthCounter = minTanggal.Month;
                                firstRun = false;
                            }
                            else
                                monthCounter = 1;
                            while (tempTanggal <= maxTanggal && monthCounter <= 12)
                            {
                                Console.WriteLine(monthCounter);
                                treeView1.Nodes[counter].Nodes.Add((monthCounter++).ToString());
                                tempTanggal = tempTanggal.AddMonths(1);
                                Console.WriteLine("Temp Tanggal: " + tempTanggal.ToShortDateString());
                            }
                            counter++;
                        }
                    }
                }

            }
        }
        List<DateTime> loadKumpulanTanggalUntukPrediksi()
        {
            List<DateTime> kumpulanTanggalUntukPrediksi = new List<DateTime>();
            for (int i = 0; i < treeView1.Nodes.Count; i++)
            {
                for (int j = 0; j < treeView1.Nodes[i].Nodes.Count; j++)
                {
                    if (treeView1.Nodes[i].Nodes[j].Checked)
                    {
                        kumpulanTanggalUntukPrediksi.Add(new DateTime(Int32.Parse(treeView1.Nodes[i].Text), Int32.Parse(treeView1.Nodes[i].Nodes[j].Text), 1));
                    }
                }
            }
            return kumpulanTanggalUntukPrediksi;
        }
        private void CheckTreeViewNode(TreeNode node, Boolean isChecked)
        {
            foreach (TreeNode item in node.Nodes)
            {
                item.Checked = isChecked;

                if (item.Nodes.Count > 0)
                {
                    this.CheckTreeViewNode(item, isChecked);
                }
            }
        }
        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            CheckTreeViewNode(e.Node, e.Node.Checked);
        }
        private void treeView1_ParentChanged(object sender, EventArgs e)
        {

        }
        public class JenisEvent
        {
            public DateTime tgl { set; get; }
            public String eventType { set; get; }
        }
        public class ViewProyeksiAtm
        {
            public DateTime tanggal { set; get; }
            public Int64 isiATM100 { set; get; }
            public Int64 isiATM50 { set; get; }
            public Int64 isiATM20 { set; get; }
            public Int64 isiCRM100 { set; get; }
            public Int64 isiCRM50 { set; get; }
            public Int64 isiCRM20 { set; get; }
            public Int64 sislokATM100 { set; get; }
            public Int64 sislokATM50 { set; get; }
            public Int64 sislokATM20 { set; get; }
            public Int64 sislokCRM100 { set; get; }
            public Int64 sislokCRM50 { set; get; }
            public Int64 sislokCRM20 { set; get; }
            public Int64 sislokCDM100 { set; get; }
            public Int64 sislokCDM50 { set; get; }
            public Int64 sislokCDM20 { set; get; }
            public String eventType { set; get; }
        }
    }
  
}
