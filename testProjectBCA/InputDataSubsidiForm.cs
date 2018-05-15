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
        public InputDataSubsidiForm()
        {
            InitializeComponent();
        }

        private void selectButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = Variables.excelFilter;
            if(of.ShowDialog() == DialogResult.OK)
            {
                DataSet ds = Util.openExcel(of.FileName);

                DataTable dt = ds.Tables[0];
                //dataGridView1.DataSource = dt;
                loadGBKF(dt);
            }
        }
        void loadGBKF(DataTable dt)
        {
            List<Nasabah> listNasabah = (from x in db.Nasabahs
                                         select x).ToList();
            DataRow [] rows = dt.Select("Column0 is null");
            foreach (var row in rows)
                dt.Rows.Remove(row);
            dt.Rows.RemoveAt(0);
            dataGridView1.DataSource = dt;

            for(int a = 0; a<dt.Rows.Count;a++)
            {
                DataRow row = dt.Rows[a];
                Nasabah toUpdate = listNasabah.Where(x => x.kodeNasabah == row[1].ToString().TrimStart('0')).FirstOrDefault();
                if(toUpdate == null)
                {
                    db.Nasabahs.Add(new Nasabah() {
                        kodeNasabah = row[1].ToString().TrimStart('0'),
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

    }
}
