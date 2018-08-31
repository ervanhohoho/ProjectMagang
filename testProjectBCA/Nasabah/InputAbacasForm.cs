using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testProjectBCA
{
    public partial class InputAbacasForm : Form
    {
        Database1Entities db = new Database1Entities();
        public InputAbacasForm()
        {
            InitializeComponent();
            
        }

        private void selectFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = Variables.excelFilter;
            if(of.ShowDialog() == DialogResult.OK)
            {
                loadForm.ShowSplashScreen();
                DataSet ds = Util.openExcel(of.FileName);
                DataTable dt = ds.Tables[0];

                dt.Rows.RemoveAt(0);
                dt.Rows.RemoveAt(0);
                dt.Rows.RemoveAt(0);


                DataRow[] toRemove = dt.Select("Column2 IS NULL");
                foreach (var rem in toRemove)
                    dt.Rows.Remove(rem);
                foreach (DataRow drOutput in dt.Rows)
                {
                    drOutput[2] = drOutput[2].ToString().Replace("\'", "");
                    drOutput[4] = drOutput[4].ToString().Replace("\'", "");
                    drOutput[1] = drOutput[1].ToString().Replace(" 12:00:00 AM", "");
                    //your remaining code
                }
                Console.WriteLine(dt.Rows[4][1].GetType());
                dataGridView1.DataSource = dt;

                var allAbacas = (from x in db.Abacas
                                 select x).ToList();
                List<Abaca> toInput = new List<Abaca>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Console.WriteLine("i = " + i);
                    if (dt.Rows[i][0].ToString() == "Transaction Date")
                    {
                        String kodeNasabah = (String)dt.Rows[i + 2][2];
                        String namaNasabah = (String)dt.Rows[i + 1][2];
                        int counter = 4;
                        while (dt.Rows[i + counter][0].ToString() != "Total Amount")
                        {

                            DateTime tanggal = DateTime.ParseExact(dt.Rows[i + counter][1].ToString(), "d/M/yyyy", CultureInfo.InvariantCulture);
                            String vendorCardNo = dt.Rows[i + counter][2].ToString();
                            String vendorName = dt.Rows[i + counter][3].ToString();
                            String jobId = dt.Rows[i + counter][4].ToString();

                            Int64 totalAmount = Int64.Parse(dt.Rows[i + counter][5].ToString());

                            var checkData = allAbacas.Where(x => x.jobId == jobId && x.tanggal == tanggal).FirstOrDefault();
                            if (checkData != null)
                            {
                                counter++;
                                continue;
                            }
                            Abaca newItem = new Abaca()
                            {
                                jobId = jobId,
                                tanggal = tanggal,
                                vendorCardNo = vendorCardNo,
                                vendorName = vendorName,
                                totalAmount = totalAmount,
                                CustomerCode = kodeNasabah.TrimStart(':').TrimStart(' ').ToUpper(),
                                CustomerName = namaNasabah.TrimStart(':').TrimStart(' '),
                                TotalUangBesar = 0
                            };
                            toInput.Add(newItem);
                            counter++;
                        }
                        
                        i += counter;
                    }
                }
                db.Abacas.AddRange(toInput);
                db.SaveChanges();
                loadForm.CloseForm();
                MessageBox.Show("Done!");
            }
        }
    }
}
