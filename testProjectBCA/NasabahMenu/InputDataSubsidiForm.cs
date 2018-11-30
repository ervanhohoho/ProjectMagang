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
    public partial class InputDataSubsidiForm : Form
    {
        Database1Entities db = new Database1Entities();
        List<Nasabah> listNasabah;
        public InputDataSubsidiForm()
        {
            InitializeComponent();
            listNasabah = (from x in db.Nasabahs select x).ToList();
        }

        private void selectButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = Variables.excelFilter;
            if(of.ShowDialog() == DialogResult.OK)
            {
                loadForm.ShowSplashScreen();
                DataSet ds = Util.openExcel(of.FileName);

                DataTable gbkf = ds.Tables[0];
                DataTable scm= ds.Tables[1];
                //dataGridView1.DataSource = dt;
                loadGBKF(gbkf);
                loadSCM(scm);
                loadForm.CloseForm();
                MessageBox.Show("Done!");
            }
        }
        void loadGBKF(DataTable dt)
        {
            
            DataRow [] rows = dt.Select("Column0 is null");
            foreach (var row in rows)
                dt.Rows.Remove(row);
            dt.Rows.RemoveAt(0);

            for(int a = 0; a<dt.Rows.Count;a++)
            {
                DataRow row = dt.Rows[a];
                Nasabah toUpdate = listNasabah.Where(x => x.kodeNasabah == row[1].ToString()).FirstOrDefault();
                if(toUpdate == null)
                {
                    db.Nasabahs.Add(new Nasabah() {
                        kodeNasabah = row[1].ToString(),
                        persentaseSubsidi = Double.Parse(row[6].ToString()),
                        subsidi = "GBKF"
                    });
                }
                else
                {
                    toUpdate.subsidi = "GBKF";
                    toUpdate.persentaseSubsidi = Double.Parse(row[6].ToString());
                }
            }
            db.SaveChanges();
        }
        void loadSCM(DataTable dt)
        {
            DataRow[] rows = dt.Select("Column0 is null");
            foreach (var row in rows)
                dt.Rows.Remove(row);
            dt.Rows.RemoveAt(0);
            for (int a = 0; a < dt.Rows.Count; a++)
            {
                DataRow row = dt.Rows[a];
                Nasabah toUpdate = listNasabah.Where(x => x.kodeNasabah == row[1].ToString()).FirstOrDefault();
                Console.WriteLine(row[5].ToString());
                if (toUpdate == null)
                {
                    db.Nasabahs.Add(new Nasabah()
                    {
                        kodeNasabah = row[1].ToString(),
                        persentaseSubsidi = 1,
                        subsidi = "SCM - " + (String.IsNullOrEmpty(row[5].ToString())?"Gereja": row[5].ToString()),
                        kuota = row[5].ToString().Contains("12X") ? 12 : 0
                    });
                }
                else
                {
                    toUpdate.subsidi = "SCM - " + (String.IsNullOrEmpty(row[5].ToString()) ? "Gereja" : row[5].ToString());
                    toUpdate.persentaseSubsidi = 1;
                    toUpdate.kuota = row[5].ToString().Contains("12X") ? 12 : 0;
                }
            }
           
            db.SaveChanges();
        }
    }
}
