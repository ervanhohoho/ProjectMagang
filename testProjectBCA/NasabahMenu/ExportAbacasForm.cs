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

namespace testProjectBCA.NasabahMenu
{
    public partial class ExportAbacasForm : Form
    {
        Database1Entities db = new Database1Entities();
        public ExportAbacasForm()
        {
            InitializeComponent();
            loadComboTahun();
            loadComboBulan();
        }
        void loadComboTahun()
        {
            List<Int32> listTahun = (from x in db.Abacas
                                     select ((DateTime)x.tanggal).Year).Distinct().OrderByDescending(x => x).ToList();
            tahunCombo.DataSource = listTahun;
            tahunCombo.SelectedIndex = 0;
        }
        void loadComboBulan()
        {
            int tahun = Int32.Parse(tahunCombo.SelectedItem.ToString());
            List<Int32> listBulan = (from x in db.Abacas
                                     where ((DateTime)x.tanggal).Year == tahun
                                     select ((DateTime)x.tanggal).Month).Distinct().OrderByDescending(x => x).ToList();
            bulanCombo.DataSource = listBulan;
            bulanCombo.SelectedIndex = 0;
        }

        private void tahunCombo_SelectedValueChanged(object sender, EventArgs e)
        {
            loadComboBulan();
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog sv = new SaveFileDialog();
            sv.Filter = Variables.csvFilter;
            if (sv.ShowDialog() == DialogResult.OK)
            {
                loadForm.ShowSplashScreen();
                Int32 bulan = Int32.Parse(bulanCombo.SelectedItem.ToString()),
                tahun = Int32.Parse(tahunCombo.SelectedItem.ToString());

                List<Abaca> abacas = (from x in db.Abacas
                                      where ((DateTime)x.tanggal).Year == tahun
                                      && ((DateTime)x.tanggal).Month == bulan
                                      select x).ToList();
                String csv = ServiceStack.Text.CsvSerializer.SerializeToString(abacas);
                File.WriteAllText(sv.FileName, csv);
                loadForm.CloseForm();
                MessageBox.Show("Done!");
            }
        }
    }
}
