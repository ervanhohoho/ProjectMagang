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
    public partial class PerbandinganSaldoForm : Form
    {
        int jumlahDetailApproval;
        public PerbandinganSaldoForm()
        {
            Database1Entities db = new Database1Entities();
            InitializeComponent();

            int maxIdApproval = 0;
            jumlahDetailApproval = 1;
            if (db.Approvals.ToList().Any())
            {
                var temp = db.Approvals.Where(x => x.tanggal == Variables.todayDate).ToList();
                maxIdApproval = !temp.Any() ? 0 : temp.Max(x => x.idApproval);
                if (!temp.Any())
                    MessageBox.Show("Belum ada approval!");
                jumlahDetailApproval = db.DetailApprovals.Where(x => x.idApproval == maxIdApproval).ToList().Count();
            }
            loadGroupingCombo();
            calendarSelect.MaxSelectionCount = jumlahDetailApproval;
        }
        void loadGroupingCombo()
        {
            Database1Entities db = new Database1Entities();
            List<String> listGrouping = new List<String>();
            listGrouping.Add("Nasional");
            listGrouping.Add("Non Jabo");
            listGrouping.AddRange(db.Pkts.Select(x => x.kanwil).OrderBy(x => x).Distinct().ToList());
            groupingComboBox.DataSource = listGrouping;
            groupingComboBox.SelectedIndex = 0;
        }
        List<tanggalValue> loadListRealisasi()
        {
            List<tanggalValue> res = new List<tanggalValue>();
            Database1Entities db = new Database1Entities();
            DateTime startDate = calendarSelect.SelectionStart,
                endDate = calendarSelect.SelectionEnd;

            if (groupingComboBox.SelectedItem.ToString() == "Nasional")
            {
                res = (from x in db.TransaksiAtms
                       group x by x.tanggal into g
                       where g.Key <= endDate && g.Key >= startDate
                       select new tanggalValue() {
                           tanggal = g.Key,
                           value = (Int64) g.Sum(x=>x.saldoAwal100 + x.saldoAwal50 + x.saldoAwal20)
                       }).ToList();
            }
            else if(groupingComboBox.SelectedItem.ToString() == "Non Jabo")
            {
                var q = (from x in db.TransaksiAtms
                         join y in db.Pkts on x.kodePkt equals y.kodePkt
                         where !y.kanwil.ToUpper().Contains("JABO")
                         select x
                         ).ToList();
                res = (from x in q
                       group x by x.tanggal into g
                       where g.Key <= endDate && g.Key >= startDate
                       select new tanggalValue()
                       {
                           tanggal = g.Key,
                           value = (Int64)g.Sum(x => x.saldoAwal100 + x.saldoAwal50 + x.saldoAwal20)
                       }).ToList();
            }
            else
            {
                String kanwil = groupingComboBox.SelectedItem.ToString();
                var q = (from x in db.TransaksiAtms
                         join y in db.Pkts on x.kodePkt equals y.kodePkt
                         where y.kanwil.ToUpper().Contains(kanwil.ToUpper())
                         select x
                         ).ToList();
                res = (from x in q
                       group x by x.tanggal into g
                       where g.Key <= endDate && g.Key >= startDate
                       select new tanggalValue()
                       {
                           tanggal = g.Key,
                           value = (Int64)g.Sum(x => x.saldoAwal100 + x.saldoAwal50 + x.saldoAwal20)
                       }).ToList();
            }

            for(int a=0;a<res.Count;a++)
            {
                res[a].tanggal = Variables.todayDate.AddDays(a);
            }

            Console.WriteLine("List Realisasi\n=================");
            foreach(var temp in res)
            {
                Console.WriteLine(temp.tanggal.ToShortDateString() + " " + temp.value);
            }
            return res;
        }
        List<tanggalValue> loadListApproval()
        {
            List<tanggalValue> res = new List<tanggalValue>();
            Database1Entities db = new Database1Entities();

            var q = (from x in db.Approvals.AsEnumerable()
                     join y in db.DetailApprovals on x.idApproval equals y.idApproval
                     where x.tanggal.Date == Variables.todayDate.Date
                     select new { A = x, DA = y }).ToList();
            if(groupingComboBox.SelectedItem.ToString() == "Nasional")
            {
                res = (from x in q
                       group x by x.DA.tanggal into g
                       select new tanggalValue() {
                           tanggal = (DateTime) g.Key,
                           value = (Int64) g.Sum(x=> x.DA.saldoAwal100 + x.DA.saldoAwal50 + x.DA.saldoAwal20)
                       }).ToList();
            }
            else if(groupingComboBox.SelectedItem.ToString() == "Non Jabo")
            {
                var q2 = (from x in q
                          join y in db.Pkts on x.A.kodePkt equals y.kodePkt
                          where !y.kanwil.ToUpper().Contains("JABO")
                          select x).ToList();
                res = (from x in q2
                       group x by x.DA.tanggal into g
                       select new tanggalValue()
                       {
                           tanggal = (DateTime)g.Key,
                           value = (Int64)g.Sum(x => x.DA.saldoAwal100 + x.DA.saldoAwal50 + x.DA.saldoAwal20)
                       }).ToList();
            }
            else
            {
                String kanwil = groupingComboBox.SelectedItem.ToString().ToUpper();
                var q2 = (from x in q
                          join y in db.Pkts on x.A.kodePkt equals y.kodePkt
                          where y.kanwil.ToUpper().Contains(kanwil)
                          select x).ToList();
                res = (from x in q2
                       group x by x.DA.tanggal into g
                       select new tanggalValue()
                       {
                           tanggal = (DateTime)g.Key,
                           value = (Int64)g.Sum(x => x.DA.saldoAwal100 + x.DA.saldoAwal50 + x.DA.saldoAwal20)
                       }).ToList();
            }
            Console.WriteLine("List Approval\n=================");
            foreach (var temp in res)
                Console.WriteLine(temp.tanggal.ToShortDateString() + " " + temp.value);
            
            return res;
        }
        List<tanggalValue> loadBonYangDisetujui()
        {
            List<tanggalValue> res = new List<tanggalValue>();
            Database1Entities db = new Database1Entities();

            var q = (from x in db.Approvals.AsEnumerable()
                     join y in db.DetailApprovals on x.idApproval equals y.idApproval
                     where x.tanggal.Date == Variables.todayDate.Date
                     select new { A = x, DA = y }).ToList();
            if (groupingComboBox.SelectedItem.ToString() == "Nasional")
            {
                res = (from x in q
                       group x by x.DA.tanggal into g
                       select new tanggalValue()
                       {
                           tanggal = (DateTime)g.Key,
                           value = (Int64)g.Sum(x => x.DA.bon100 + x.DA.bon50 + x.DA.bon20)
                       }).ToList();
            }
            else if (groupingComboBox.SelectedItem.ToString() == "Non Jabo")
            {
                var q2 = (from x in q
                          join y in db.Pkts on x.A.kodePkt equals y.kodePkt
                          where !y.kanwil.ToUpper().Contains("JABO")
                          select x).ToList();
                res = (from x in q2
                       group x by x.DA.tanggal into g
                       select new tanggalValue()
                       {
                           tanggal = (DateTime)g.Key,
                           value = (Int64)g.Sum(x => x.DA.bon100 + x.DA.bon50 + x.DA.bon20)
                       }).ToList();
            }
            else
            {
                String kanwil = groupingComboBox.SelectedItem.ToString().ToUpper();
                var q2 = (from x in q
                          join y in db.Pkts on x.A.kodePkt equals y.kodePkt
                          where y.kanwil.ToUpper().Contains(kanwil)
                          select x).ToList();
                res = (from x in q2
                       group x by x.DA.tanggal into g
                       select new tanggalValue()
                       {
                           tanggal = (DateTime)g.Key,
                           value = (Int64)g.Sum(x => x.DA.bon100 + x.DA.bon50 + x.DA.bon20)
                       }).ToList();
            }

            foreach (var temp in res)
                if (temp.value < 0)
                    temp.value = 0;
            return res;
        }
        private void LoadBtn_Click(object sender, EventArgs e)
        {
            List<tanggalValue> listRealisasi = loadListRealisasi(),
                listApproval = loadListApproval(),
                listBon = loadBonYangDisetujui();

            List<GVDisplay> disp = (from x in listRealisasi
                                    join y in listApproval on x.tanggal equals y.tanggal
                                    join z in listBon on x.tanggal equals z.tanggal
                                    select new GVDisplay() {
                                        tanggalForecast = x.tanggal ,
                                        saldoAwalForecast = y.value,
                                        saldoAwalRealisasi = x.value,
                                        bonYangDisetujui = z.value
                                    }).ToList();
            var toDisp1 = (from x in disp
                          select new
                          {
                              tanggalForecast = x.tanggalForecast,
                              saldoAwalForecast = x.saldoAwalForecast,
                              saldoAwalRealisasi = x.saldoAwalRealisasi,
                              bonYangDisetujui = x.bonYangDisetujui,
                              persentaseForecastPerRealisasi = (Double)x.saldoAwalForecast / x.saldoAwalRealisasi
                          }).ToList();

            dataGridView1.DataSource = toDisp1;
            for (int a = 1; a < dataGridView1.ColumnCount; a++)
            {
                dataGridView1.Columns[a].DefaultCellStyle.Format = "N0";
                if (a == dataGridView1.ColumnCount - 1)
                    dataGridView1.Columns[a].DefaultCellStyle.Format = "0.00%";
            }
        }
    }

    public class GVDisplay
    {
        public DateTime tanggalForecast { set; get; }
        public Int64 saldoAwalForecast { set; get; }
        public Int64 saldoAwalRealisasi { set; get; }
        public Int64 bonYangDisetujui { set; get; }
    }
}
