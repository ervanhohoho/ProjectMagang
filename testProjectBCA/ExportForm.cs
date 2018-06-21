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
            pktComboBox.DataSource = (from x in db.Pkts
                                      where x.kodePktATM != ""
                                      select x.kodePkt).OrderBy(x=>x).ToList();
        }

        private void ExportBtn_Click(object sender, EventArgs e)
        {
            SaveFileDialog sv = new SaveFileDialog();
            sv.Filter = Variables.csvFilter;
            DateTime startDate = startDatePicker.Value.Date, endDate = endDatePicker.Value.Date;
            if (sv.ShowDialog() == DialogResult.OK)
            {
                loadForm.ShowSplashScreen();
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
                loadForm.CloseForm();
                Console.WriteLine("Success");
                File.WriteAllText(sv.FileName, csv);
            }
        }

        private void viewButton_Click(object sender, EventArgs e)
        {
            loadForm.ShowSplashScreen();
            DateTime startDate = startDatePicker.Value.Date, endDate = endDatePicker.Value.Date;
            String kodePkt = pktComboBox.SelectedItem.ToString();
            Console.WriteLine(kodePkt);
            var list = (from x in db.TransaksiAtms.AsEnumerable()
                                       where ((DateTime)x.tanggal).Date >= startDate && ((DateTime)x.tanggal).Date <= endDate
                                       && x.kodePkt == kodePkt
                                       select new {
                                           x.tanggal,
                                           x.kodePkt,
                                           x.saldoAwal20,
                                           x.saldoAwal50,
                                           x.saldoAwal100,
                                           x.sislokATM20,
                                           x.sislokATM50,
                                           x.sislokATM100,
                                           x.sislokCRM20,
                                           x.sislokCRM50,
                                           x.sislokCRM100,
                                           x.sislokCDM20,
                                           x.sislokCDM50,
                                           x.sislokCDM100,
                                           x.isiATM20,
                                           x.isiATM50,
                                           x.isiATM100,
                                           x.isiCRM20,
                                           x.isiCRM50,
                                           x.isiCRM100,
                                           x.bon20,
                                           x.bon50,
                                           x.bon100,
                                           x.adhoc20,
                                           x.adhoc50,
                                           x.adhoc100,
                                           x.setor20,
                                           x.setor50,
                                           x.setor100,
                                           x.saldoAkhir20,
                                           x.saldoAkhir50,
                                           x.saldoAkhir100 }).ToList();
            
            dataGridView1.DataSource = list;
            for (int a = 2; a < dataGridView1.ColumnCount; a++)
                dataGridView1.Columns[a].DefaultCellStyle.Format = "N0";
            loadForm.CloseForm();
        }
    }
}
