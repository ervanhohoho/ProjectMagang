using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testProjectBCA.Dashboard
{
    public partial class ProyeksiApproval : Form
    {
        Database1Entities db = new Database1Entities();
        public ProyeksiApproval()
        {
            InitializeComponent();
            List<String> listKanwil = db.Pkts.Select(x => x.kanwil).Distinct().OrderBy(x=>x).ToList();
            listKanwil.Add("Nasional");
            kanwilComboBox.DataSource = listKanwil;
            tanggalApprovalPicker.MinDate = db.Approvals.Min(x => x.tanggal);
            tanggalApprovalPicker.MaxDate = db.Approvals.Max(x => x.tanggal);
        }
        void loadChart()
        {
            foreach (var toDel in cartesianChart1.Series)
            {
                cartesianChart1.Series.Remove(toDel);
            }
            foreach (var toDel in cartesianChart1.AxisX)
            {
                cartesianChart1.AxisX.Remove(toDel);
            }
            foreach (var toDel in cartesianChart1.AxisY)
            {
                cartesianChart1.AxisY.Remove(toDel);
            }
            String kanwil = kanwilComboBox.SelectedItem.ToString();
            DateTime tanggal = tanggalApprovalPicker.Value;

            List<DetailApproval> data = new List<DetailApproval>();
            if (kanwil != "Nasional")
            {
                data = (from x in db.Approvals
                        join y in db.DetailApprovals on x.idApproval equals y.idApproval
                        join z in db.Pkts on x.kodePkt equals z.kodePkt
                        where x.tanggal == tanggal
                        && z.kanwil == kanwil
                        select y).ToList();
            }
            else
            {
                data = (from x in db.Approvals
                        join y in db.DetailApprovals on x.idApproval equals y.idApproval
                        join z in db.Pkts on x.kodePkt equals z.kodePkt
                        where x.tanggal == tanggal
                        select y).ToList();
            }
            List<Double> rasio = data.GroupBy(x => x.tanggal).Select(x => (Double)((Double)(x.Sum(y => y.saldoAwal100 + y.saldoAwal20 + y.saldoAwal50)) / x.Sum(y => y.isiATM100 + y.isiATM20 + y.isiATM50 + y.isiCRM100 + y.isiCRM20 + y.isiCRM50))).ToList();
            ChartValues<Double> ts = new ChartValues<Double>();
            foreach(var toAdd in rasio)
            {
                ts.Add(toAdd);
            }
            LineSeries lineSeries = new LineSeries() {
                Title = "Rasio",
                Values = ts,
            };

            List<String> labels = data.Select(x => ((DateTime)x.tanggal).ToShortDateString()).ToList();
            cartesianChart1.Series.Add(lineSeries);
            cartesianChart1.AxisX.Add(new Axis() { Labels = labels, Separator = new Separator() {Step = 1} });
            cartesianChart1.AxisY.Add(new Axis() { Labels = rasio.Select(x => x.ToString("N2")).ToList()});
        }

        private void kanwilComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            loadChart();
        }

        private void tanggalApprovalPicker_ValueChanged(object sender, EventArgs e)
        {
            loadChart();
        }
    }
}
