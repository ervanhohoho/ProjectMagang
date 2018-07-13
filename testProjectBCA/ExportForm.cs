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
            List<String> kodePkts = (from x in db.Pkts
                                     where x.kodePktATM != ""
                                     select x.kodePkt).OrderBy(x => x).ToList();
            kodePkts.Add("All Vendor");
            pktComboBox.DataSource = kodePkts;
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
                var list = (from x in db.TransaksiAtms.AsEnumerable()
                            where ((DateTime)x.tanggal).Date >= startDate && ((DateTime)x.tanggal).Date <= endDate
                            && x.kodePkt == kodePkt
                            orderby x.tanggal
                            select new
                            {
                                x.tanggal,
                                x.kodePkt,
                                x.saldoAwal100,
                                x.saldoAwal50,
                                x.saldoAwal20,
                                x.sislokATM100,
                                x.sislokATM50,
                                x.sislokATM20,
                                x.sislokCRM100,
                                x.sislokCRM50,
                                x.sislokCRM20,
                                x.sislokCDM100,
                                x.sislokCDM50,
                                x.sislokCDM20,
                                x.isiATM100,
                                x.isiATM50,
                                x.isiATM20,
                                x.isiCRM100,
                                x.isiCRM50,
                                x.isiCRM20,
                                x.bon100,
                                x.bon50,
                                x.bon20,
                                x.adhoc100,
                                x.adhoc50,
                                x.adhoc20,
                                x.setor100,
                                x.setor50,
                                x.setor20,
                                x.saldoAkhir100,
                                x.saldoAkhir50,
                                x.saldoAkhir20,
                                rasio100 = x.isiATM100 == 0 ? 0 : (Double)x.saldoAwal100 / (Double)(x.isiATM100 + x.isiCRM100),
                                rasio50 = x.isiATM50 == 0 ? 0 : (Double)x.saldoAwal50 / (Double)(x.isiATM50 + x.isiCRM50),
                                rasio20 = x.isiATM20 == 0 ? 0 : (Double)x.saldoAwal20 / (Double)(x.isiATM20 + x.isiCRM20),
                                rasioGabungan = x.isiATM100 + x.isiATM20 + x.isiATM50 + x.isiCRM100 + x.isiCRM20 + x.isiCRM50 == 0 ? 0 : (Double)(x.saldoAwal100 + x.saldoAwal50 + x.saldoAwal20) / (Double)(x.isiATM100 + x.isiATM20 + x.isiATM50 + x.isiCRM100 + x.isiCRM20 + x.isiCRM50),
                            }).ToList();

                if(kodePkt == "All Vendor")
                {
                    list = (from x in db.TransaksiAtms.AsEnumerable()
                            where ((DateTime)x.tanggal).Date >= startDate && ((DateTime)x.tanggal).Date <= endDate
                            orderby x.tanggal ascending
                            orderby x.kodePkt ascending
                            select new
                            {
                                x.tanggal,
                                x.kodePkt,
                                x.saldoAwal100,
                                x.saldoAwal50,
                                x.saldoAwal20,
                                x.sislokATM100,
                                x.sislokATM50,
                                x.sislokATM20,
                                x.sislokCRM100,
                                x.sislokCRM50,
                                x.sislokCRM20,
                                x.sislokCDM100,
                                x.sislokCDM50,
                                x.sislokCDM20,
                                x.isiATM100,
                                x.isiATM50,
                                x.isiATM20,
                                x.isiCRM100,
                                x.isiCRM50,
                                x.isiCRM20,
                                x.bon100,
                                x.bon50,
                                x.bon20,
                                x.adhoc100,
                                x.adhoc50,
                                x.adhoc20,
                                x.setor100,
                                x.setor50,
                                x.setor20,
                                x.saldoAkhir100,
                                x.saldoAkhir50,
                                x.saldoAkhir20,
                                rasio100 = x.isiATM100 == 0 ? 0 : (Double) x.saldoAwal100 / (Double)(x.isiATM100 + x.isiCRM100),
                                rasio50 = x.isiATM50 == 0 ? 0 : (Double) x.saldoAwal50 / (Double)(x.isiATM50 + x.isiCRM50),
                                rasio20 = x.isiATM20 == 0 ? 0 : (Double) x.saldoAwal20 / (Double)(x.isiATM20 + x.isiCRM20),
                                rasioGabungan = x.isiATM100 + x.isiATM20 + x.isiATM50 + x.isiCRM100 + x.isiCRM20 + x.isiCRM50 == 0 ? 0 : (Double)(x.saldoAwal100 + x.saldoAwal50 + x.saldoAwal20) / (Double)(x.isiATM100 + x.isiATM20 + x.isiATM50 + x.isiCRM100 + x.isiCRM20 + x.isiCRM50),
                            }).ToList();
                }
                String csv = "tanggal,kodePkt,saldoAwal100,saldoAwal50,saldoAwal20,sislokATM100,sislokATM50,sislokATM20,sislokCRM100,sislokCRM50,sislokCRM20,sislokCDM100,sislokCDM50,sislokCDM20,isiATM100,isiATM50,isiATM20,isiCRM100,isiCRM50,isiCRM20,bon100,bon50,bon20,adhoc100,adhoc50,adhoc20,setor100,setor50,setor20,saldoAkhir100,saldoAkhir50,saldoAkhir20,rasio100,rasio50,rasio20";
                foreach(var temp in list)
                {
                    csv +=
                        "\n" + temp.tanggal + ","
                        + temp.kodePkt + ","
                        + temp.saldoAwal100 + "," + temp.saldoAwal50 + "," + temp.saldoAwal20 + ","
                        + temp.sislokATM100 + "," + temp.sislokATM50 + "," + temp.sislokATM20 + ","
                        + temp.sislokCRM100 + "," + temp.sislokCRM50 + "," + temp.sislokCRM20 + ","
                        + temp.sislokCDM100 + "," + temp.sislokCDM50 + "," + temp.sislokCDM20 + ","
                        + temp.isiATM100 + "," + temp.isiATM50 + "," + temp.isiATM20 + ","
                        + temp.isiCRM100 + "," + temp.isiCRM50 + "," + temp.isiCRM20 + ","
                        + temp.bon100 + "," + temp.bon50 + "," + temp.bon20 + ","
                        + temp.adhoc100 + "," + temp.adhoc50 + "," + temp.adhoc20 + ","
                        + temp.setor100 + "," + temp.setor50 + "," + temp.setor20 + ","
                        + temp.saldoAkhir100 + "," + temp.saldoAkhir50 + "," + temp.saldoAkhir20 + ","
                        + temp.rasio100 + "," + temp.rasio50 + "," + temp.rasio20;
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
            GC.Collect();
            db = new Database1Entities();
            var list = (from x in db.TransaksiAtms.AsEnumerable()
                        where ((DateTime)x.tanggal).Date >= startDate && ((DateTime)x.tanggal).Date <= endDate
                        && x.kodePkt == kodePkt
                        orderby x.tanggal
                        select new {
                            x.tanggal,
                            x.kodePkt,
                            x.saldoAwal100,
                            x.saldoAwal50,
                            x.saldoAwal20,
                            x.sislokATM100,
                            x.sislokATM50,
                            x.sislokATM20,
                            x.sislokCRM100,
                            x.sislokCRM50,
                            x.sislokCRM20,
                            x.sislokCDM100,
                            x.sislokCDM50,
                            x.sislokCDM20,
                            x.isiATM100,
                            x.isiATM50,
                            x.isiATM20,
                            x.isiCRM100,
                            x.isiCRM50,
                            x.isiCRM20,
                            x.bon100,
                            x.bon50,
                            x.bon20,
                            x.adhoc100,
                            x.adhoc50,
                            x.adhoc20,
                            x.setor100,
                            x.setor50,
                            x.setor20,
                            x.saldoAkhir100,
                            x.saldoAkhir50,
                            x.saldoAkhir20,
                            rasio100 = x.isiATM100 == 0 ? 0 : (Double)x.saldoAwal100 / (Double)(x.isiATM100 + x.isiCRM100),
                            rasio50 = x.isiATM50 == 0 ? 0 : (Double)x.saldoAwal50 / (Double)(x.isiATM50 + x.isiCRM50),
                            rasio20 = x.isiATM20 == 0 ? 0 : (Double)x.saldoAwal20 / (Double)(x.isiATM20 + x.isiCRM20),
                            rasioGabungan = x.isiATM100 + x.isiATM20 + x.isiATM50 + x.isiCRM100 + x.isiCRM20 + x.isiCRM50 == 0 ? 0 : (Double)(x.saldoAwal100 + x.saldoAwal50 + x.saldoAwal20) / (Double)(x.isiATM100 + x.isiATM20 + x.isiATM50 + x.isiCRM100 + x.isiCRM20 + x.isiCRM50),
                        }).ToList();
            if (kodePkt == "All Vendor")
            {
                list = (from x in db.TransaksiAtms.AsEnumerable()
                        where ((DateTime)x.tanggal).Date >= startDate && ((DateTime)x.tanggal).Date <= endDate
                        orderby x.tanggal ascending
                        orderby x.kodePkt ascending
                        select new
                        {
                            x.tanggal,
                            x.kodePkt,
                            x.saldoAwal100,
                            x.saldoAwal50,
                            x.saldoAwal20,
                            x.sislokATM100,
                            x.sislokATM50,
                            x.sislokATM20,
                            x.sislokCRM100,
                            x.sislokCRM50,
                            x.sislokCRM20,
                            x.sislokCDM100,
                            x.sislokCDM50,
                            x.sislokCDM20,
                            x.isiATM100,
                            x.isiATM50,
                            x.isiATM20,
                            x.isiCRM100,
                            x.isiCRM50,
                            x.isiCRM20,
                            x.bon100,
                            x.bon50,
                            x.bon20,
                            x.adhoc100,
                            x.adhoc50,
                            x.adhoc20,
                            x.setor100,
                            x.setor50,
                            x.setor20,
                            x.saldoAkhir100,
                            x.saldoAkhir50,
                            x.saldoAkhir20,
                            rasio100 = x.isiATM100 == 0 ? 0 : (Double)x.saldoAwal100 / (Double)(x.isiATM100 + x.isiCRM100),
                            rasio50 = x.isiATM50 == 0 ? 0 : (Double)x.saldoAwal50 / (Double)(x.isiATM50 + x.isiCRM50),
                            rasio20 = x.isiATM20 == 0 ? 0 : (Double)x.saldoAwal20 / (Double)(x.isiATM20 + x.isiCRM20),
                            rasioGabungan = x.isiATM100 + x.isiATM20 + x.isiATM50 + x.isiCRM100 + x.isiCRM20 + x.isiCRM50 == 0 ? 0 : (Double)(x.saldoAwal100 + x.saldoAwal50 + x.saldoAwal20) / (Double)(x.isiATM100 + x.isiATM20 + x.isiATM50 + x.isiCRM100 + x.isiCRM20 + x.isiCRM50),
                        }).ToList();
            }
            dataGridView1.DataSource = list;

            for (int a = 2; a < dataGridView1.ColumnCount; a++)
            {
                
                dataGridView1.Columns[a].DefaultCellStyle.Format = "N0";
            }

            dataGridView1.Columns[dataGridView1.ColumnCount - 3].DefaultCellStyle.Format = "N2";
            dataGridView1.Columns[dataGridView1.ColumnCount - 2].DefaultCellStyle.Format = "N2";
            dataGridView1.Columns[dataGridView1.ColumnCount - 1].DefaultCellStyle.Format = "N2";

            loadForm.CloseForm();
        }

        private void pktComboBox_SelectedValueChanged(object sender, EventArgs e)
        {

        }
    }
}
