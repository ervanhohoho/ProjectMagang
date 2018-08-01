using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiveCharts;
using LiveCharts.Wpf;

namespace testProjectBCA
{
    public partial class PopupInformationBoard : Form
    {
        String kodePkt;
        int month;
        int year;
        public PopupInformationBoard()
        {
            InitializeComponent();
            
        }
        public PopupInformationBoard(String pkt)
        {
            InitializeComponent();
            kodePkt = pkt;
            Console.WriteLine(kodePkt);
            loadTahunCombo();
            loadBulanCombo();
        }
        void loadTahunCombo()
        {
            Database1Entities db = new Database1Entities();
            var q = db.TransaksiAtms.Select(x => ((DateTime)x.tanggal).Year).Distinct().OrderByDescending(x=>x).ToList();
            foreach (var temp in q)
            {
                Console.WriteLine("Tahun: " + temp);
                tahunCombo.Items.Add(temp.ToString());
            }
            tahunCombo.SelectedIndex = 0;
            Console.WriteLine("Tahun");
            Console.WriteLine(tahunCombo.SelectedItem.ToString());
            year = Int32.Parse(tahunCombo.SelectedItem.ToString());
        }
        void loadBulanCombo()
        {
            Database1Entities db = new Database1Entities();
            var q = db.TransaksiAtms.Where(x=>((DateTime)x.tanggal).Year == year).Select(x => ((DateTime)x.tanggal).Month).Distinct().OrderByDescending(x => x).ToList();
            foreach (var temp in q)
                bulanCombo.Items.Add(temp.ToString());
            bulanCombo.SelectedIndex = 0;
            month = Int32.Parse(bulanCombo.SelectedItem.ToString());
        }
        private void addButton_Click(object sender, EventArgs e)
        {
            int buf;
            if(Int32.TryParse(bulanCombo.SelectedText, out buf))
                month = buf;
            if (Int32.TryParse(tahunCombo.SelectedText, out buf))
                year = buf;
            Database1Entities db = new Database1Entities();
            var q = (from x in db.TransaksiAtms
                     where x.kodePkt == kodePkt && ((DateTime)x.tanggal).Month == month && ((DateTime)x.tanggal).Year == year
                     select new
                     { Rasio = (Double)(x.saldoAwal100 + x.saldoAwal50 + x.saldoAwal20) / (x.isiATM100 + x.isiATM20 + x.isiATM50 + x.isiCRM100 + x.isiCRM20 + x.isiCRM50) }
                     ).ToList();
            if (rasioChart.Series.Count == 0)
            {
                ChartValues<Double> values = new ChartValues<Double>();
                foreach (var temp in q)
                    values.Add((Double)Math.Round((Double)temp.Rasio,2));
                rasioChart.Series.Add(
                    new LineSeries()
                    {
                        Values = values,
                        Title = "Rasio " + month + " - " + year,
                        DataLabels = true
                    });
            }
            else
            {
                if (rasioChart.Series.Count == 1)
                {
                    var tempValues = rasioChart.Series[0].Values;
                    String tempTitle = rasioChart.Series[0].Title;
                    rasioChart.Series.Clear();

                    rasioChart.Series.Add(new LineSeries() { Values = tempValues, Title = tempTitle, DataLabels = false });
                }
                ChartValues<Double> values = new ChartValues<Double>();
                foreach (var temp in q)
                    values.Add(Math.Round((Double)temp.Rasio));
                List<String> listTgl = new List<String>();
                for(int a= 1; a<= values.Count; a++)
                {
                    listTgl.Add(a.ToString());
                }
                if(rasioChart.AxisX.Count == 0)
                {
                    rasioChart.AxisX.Add(new Axis() { Labels = listTgl});
                }
                rasioChart.Series.Add(
                    new LineSeries()
                    {
                        Values = values,
                        Title = "Rasio " + month + " - " + year,
                        DataLabels = false,
                    });
            }
            rasioChart.LegendLocation = LegendLocation.Right;
        }


        private void bulanCombo_SelectionChangeCommitted(object sender, EventArgs e)
        {
        
            month = Int32.Parse(bulanCombo.SelectedItem.ToString());
            Console.WriteLine("Month: " + month);
        }

        private void tahunCombo_SelectionChangeCommitted(object sender, EventArgs e)
        {
           
            year = Int32.Parse(tahunCombo.SelectedItem.ToString());
            Console.WriteLine("Year: " + year);
        }

        private void bulanCombo_SelectedValueChanged(object sender, EventArgs e)
        {

        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            int buf;
            if (Int32.TryParse(bulanCombo.SelectedText, out buf))
                month = buf;
            if (Int32.TryParse(tahunCombo.SelectedText, out buf))
                year = buf;
            Database1Entities db = new Database1Entities();
            var q = (from x in db.TransaksiAtms
                     where x.kodePkt == kodePkt && ((DateTime)x.tanggal).Month == month && ((DateTime)x.tanggal).Year == year
                     select new
                     { Rasio = (x.isiATM100 + x.isiATM20 + x.isiATM50 + x.isiCRM100 + x.isiCRM20 + x.isiCRM50) == 0 ? 0 : (Double)(x.saldoAwal100 + x.saldoAwal50 + x.saldoAwal20) / (x.isiATM100 + x.isiATM20 + x.isiATM50 + x.isiCRM100 + x.isiCRM20 + x.isiCRM50) }
                     ).ToList();
            rasioChart.Series.Clear();
            ChartValues<Double> values = new ChartValues<Double>();
            foreach (var temp in q)
                values.Add(Math.Round((Double)temp.Rasio,2));
            rasioChart.Series.Add(
                new LineSeries()
                {
                    Values = values,
                    Title = "Rasio " + month + " - " + year,
                    DataLabels = true
                });
            rasioChart.LegendLocation = LegendLocation.Right;
        }

        private void averageCheckBox_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void averageCheckBox_CheckStateChanged(object sender, EventArgs e)
        {
            Console.WriteLine(averageCheckBox.Checked.ToString());
            if (averageCheckBox.Checked == false)
            {
                List<LiveCharts.Definitions.Series.ISeriesView> todelete = new List<LiveCharts.Definitions.Series.ISeriesView>();
                foreach (var temp in rasioChart.Series)
                {
                    if (temp.Title.Contains("Average"))
                    {
                        todelete.Add(temp);
                    }
                }
                foreach (var temp in todelete)
                    rasioChart.Series.Remove(temp);
            }
            else
            {
                List<ChartValues<Double>> listSeries = new List<LiveCharts.ChartValues<Double>>();
                List<ChartValues<Double>> averages = new List<LiveCharts.ChartValues<Double>>();
                List<String> titles = new List<String>();
                foreach (var temp in rasioChart.Series)
                {
                    listSeries.Add((ChartValues<Double>)temp.ActualValues);
                    titles.Add(temp.Title);
                }
                foreach(var temp in listSeries)
                {
                    Double avg = temp.Average(x => x);
                    ChartValues<Double> cv = new ChartValues<double>();
                    foreach(var temp2 in temp)
                    {
                        cv.Add(avg);
                    }
                    averages.Add(cv);
                }
                int counter = 0;
                foreach(var temp in averages)
                {
                    rasioChart.Series.Add(new LineSeries()
                    {
                        Title = "Average " + titles[counter++],
                        Values = temp
                    });
                }
            }
        }

        private void averageCheckBox_Click(object sender, EventArgs e)
        {
        }
    }
}
