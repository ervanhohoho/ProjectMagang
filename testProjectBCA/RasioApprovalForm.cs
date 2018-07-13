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
    public partial class RasioApprovalForm : Form
    {
        Database1Entities db = new Database1Entities();
        IEnumerable <Object> q;
        public RasioApprovalForm()
        {
            InitializeComponent();
            loadFilterCombo();
        }
        void loadFilterCombo()
        {
            var q = (from x in db.Pkts select x.kanwil).Distinct().OrderBy(x=> x).ToList();
            q.Add("All");
            filterComboBox.DataSource = q;
            filterComboBox.SelectedIndex = 0;
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            String kanwil = filterComboBox.SelectedItem.ToString();
            DateTime tanggalApproval = approvalDateTimePicker.Value.Date;
            
            var query = (from x in db.Approvals
                         join y in db.DetailApprovals on x.idApproval equals y.idApproval
                         where x.tanggal == tanggalApproval
                         orderby x.tanggal
                         orderby x.kodePkt
                         select new
                         {
                             x.kodePkt, 
                             y.tanggal,
                             y.saldoAwal100,
                             y.saldoAwal50,
                             y.saldoAwal20,
                             y.sislokATM100,
                             y.sislokATM50,
                             y.sislokATM20,
                             y.sislokCRM100,
                             y.sislokCRM50,
                             y.sislokCRM20,
                             y.sislokCDM100,
                             y.sislokCDM50,
                             y.sislokCDM20,
                             y.isiATM100,
                             y.isiATM50,
                             y.isiATM20,
                             y.isiCRM100,
                             y.isiCRM50,
                             y.isiCRM20,
                             y.bon100,
                             y.bon50,
                             y.bon20,
                             y.adhoc100,
                             y.adhoc50,
                             y.adhoc20,
                             y.setor100,
                             y.setor50,
                             rasio100 = y.isiATM100 + y.isiATM100 == 0 ? 0 : (Double)y.saldoAwal100 / (y.isiATM100 + y.isiCRM100),
                             rasio50 = y.isiATM50 + y.isiATM50 == 0 ? 0 : (Double)y.saldoAwal50 / (y.isiATM50 + y.isiCRM50),
                             rasio20 = y.isiATM20 + y.isiATM20 == 0 ? 0 : (Double)y.saldoAwal20 / (y.isiATM20 + y.isiCRM20),
                             rasioGabungan = y.isiATM100 + y.isiATM20 + y.isiATM50 + y.isiCRM100 + y.isiCRM20 + y.isiCRM50 == 0 ? 0 : (Double)(y.saldoAwal100 + y.saldoAwal50 + y.saldoAwal20) / (Double)(y.isiATM100 + y.isiATM20 + y.isiATM50 + y.isiCRM100 + y.isiCRM20 + y.isiCRM50),

                         }).ToList();
            if(kanwil != "All")
            {
                query = (from x in query
                         join y in db.Pkts on x.kodePkt equals y.kodePkt
                         where y.kanwil == kanwil
                         select new
                         {
                             x.kodePkt,
                             x.tanggal,
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
                             x.rasio100,
                             x.rasio50 ,
                             x.rasio20 ,
                             x.rasioGabungan,
                         }).ToList();
            }

            dataGridView1.DataSource = query;
            q = query;
            for(int a=0;a<dataGridView1.ColumnCount;a++)
            {
                if (a >= 0 && a <= 1)
                    continue;
                dataGridView1.Columns[a].DefaultCellStyle.Format = "N0";
                if (a >= dataGridView1.ColumnCount - 3)
                    dataGridView1.Columns[a].DefaultCellStyle.Format = "N2";
            }
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog sv = new SaveFileDialog();
            sv.Filter = Variables.csvFilter;
            if(sv.ShowDialog() == DialogResult.OK)
            {
                String csv = ServiceStack.Text.CsvSerializer.SerializeToString(q);
                File.WriteAllText(sv.FileName, csv);
            }
        }
    }
}
