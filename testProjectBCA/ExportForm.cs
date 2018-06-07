using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testProjectBCA
{
    public partial class ExportForm : Form
    {
        Database1Entities db = new Database1Entities();
        public ExportForm()
        {
            InitializeComponent();
            pktComboBox.DataSource = (from x in db.Pkts select x.kodePkt).ToList();
        }

        private void ExportBtn_Click(object sender, EventArgs e)
        {
            SaveFileDialog sv = new SaveFileDialog();
            sv.Filter = Variables.csvFilter;
            DateTime startDate = startDatePicker.Value.Date, endDate = endDatePicker.Value.Date;
            if (sv.ShowDialog() == DialogResult.OK)
            {
                String kodePkt = pktComboBox.SelectedItem.ToString();
                Console.WriteLine(kodePkt);
                List<TransaksiAtm> list = (from x in db.TransaksiAtms.AsEnumerable()
                                           where ((DateTime)x.tanggal).Date >= startDate && ((DateTime)x.tanggal).Date <= endDate
                                           && x.kodePkt == kodePkt
                                           select x).ToList();
                String csv = "tanggal,kodePkt,saldoAwal20,saldoAwal50,saldoAwal100,sislokATM20,sislokATM50,sislokATM100,sislokCRM20,sislokCRM50,sislokCRM100,sislokCDM20,sislokCDM50,sislokCDM100,isiATM20,isiATM50,isiATM100,isiCRM20,isiCRM50,isiCRM100,bon20,bon50,bon100,adhoc20,adhoc50,adhoc100,setor20,setor50,setor100,saldoAkhir20,saldoAkhir50,saldoAkhir100";
                foreach(var temp in list)
                {
                    csv +=
                        "\n" + temp.tanggal + ","
                        + temp.kodePkt + ","
                        + temp.saldoAwal20 + "," + temp.saldoAwal50 + "," + temp.saldoAwal100 + ","
                        + temp.sislokATM20 + "," + temp.sislokATM50 + "," + temp.sislokATM100 + ","
                        + temp.sislokCRM20 + "," + temp.sislokCRM50 + "," + temp.sislokCRM100 + ","
                        + temp.sislokCDM20 + "," + temp.sislokCDM50 + "," + temp.sislokCDM100 + ","
                        + temp.isiATM20 + "," + temp.isiATM50 + "," + temp.isiATM100 + ","
                        + temp.isiCRM20 + "," + temp.isiCRM50 + "," + temp.isiCRM100 + ","
                        + temp.bon20 + "," + temp.bon50 + "," + temp.bon100 + ","
                        + temp.adhoc20 + "," + temp.adhoc50 + "," + temp.adhoc100 + ","
                        + temp.setor20 + "," + temp.setor50 + "," + temp.setor100 + ","
                        + temp.saldoAkhir20 + "," + temp.saldoAkhir50 + "," + temp.saldoAkhir100 + ",";
                }
                Console.WriteLine("Success");
                File.WriteAllText(sv.FileName, csv);

            }
        }
    }
}
