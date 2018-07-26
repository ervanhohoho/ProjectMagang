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
    public partial class ExportOpti : Form
    {
        public ExportOpti()
        {
            InitializeComponent();

            loadKanwilComboBox();
        }
        void loadKanwilComboBox()
        {
            Database1Entities db = new Database1Entities();
            var q = (from x in db.Pkts
                     select x.kanwil).OrderBy(x => x).Distinct().ToList();
            kanwilComboBox.DataSource = q;
        }

        private void ExportBtn_Click(object sender, EventArgs e)
        {
            SaveFileDialog sv = new SaveFileDialog();
            sv.Filter = Variables.csvFilter;
            if(sv.ShowDialog() == DialogResult.OK)
            {
                Database1Entities db = new Database1Entities();

                String kanwil = kanwilComboBox.SelectedItem.ToString();
                var q = (from x in db.Optis
                         join y in db.Cashpoints on x.idCashpoint equals y.idCashpoint
                         join z in db.Pkts on y.kodePkt equals z.kodePkt
                         where z.kanwil == kanwil
                         select new { kodePkt = z.kodePkt, tanggal = x.tanggal, y.denom, x.prediksi}).ToList();

                String csv = ServiceStack.Text.CsvSerializer.SerializeToCsv(q);

                File.WriteAllText(sv.FileName, csv);
            }
        }
    }
}
