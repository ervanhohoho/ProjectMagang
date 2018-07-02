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
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
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
                             isiATM100 = y.isiATM100,
                             isiATM50 = y.isiATM50,
                             isiATM20 = y.isiATM20,
                             isiCRM100 = y.isiCRM100,
                             isiCRM50 = y.isiCRM50,
                             isiCRM20 = y.isiCRM20,
                             sislokATM100 = y.sislokATM100,
                             sislokATM50 = y.sislokATM50,
                             sislokATM20 = y.sislokATM20,
                             sislokCRM100 = y.sislokCRM100,
                             sislokCRM50 = y.sislokCRM50,
                             sislokCRM20 = y.sislokCRM20,
                             sislokCDM100 = y.sislokCDM100,
                             sislokCDM50 = y.sislokCDM50,
                             sislokCDM20 = y.sislokCDM20,
                             rasio100 = y.isiATM100 + y.isiATM100 == 0 ? 0 : (Double)y.saldoAwal100 / (y.isiATM100 + y.isiCRM100),
                             rasio50 = y.isiATM50 + y.isiATM50 == 0 ? 0 : (Double)y.saldoAwal50 / (y.isiATM50 + y.isiCRM50),
                             rasio20 = y.isiATM20 + y.isiATM20 == 0 ? 0 : (Double)y.saldoAwal20 / (y.isiATM20 + y.isiCRM20)
                         }).ToList();
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
