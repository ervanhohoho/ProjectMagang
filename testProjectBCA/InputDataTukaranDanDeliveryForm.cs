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
    public partial class InputDataTukaranDanDeliveryForm : Form
    {
        Database1Entities db = new Database1Entities();
        public InputDataTukaranDanDeliveryForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = Variables.excelFilter;
            of.Multiselect = true;
            if (of.ShowDialog() == DialogResult.OK)
            {
                loadForm.ShowSplashScreen();
                String [] paths = of.FileNames;
                List<Abaca> listAbacas = new List<Abaca>();

                int COL_CUSTOMER_CODE = 1,
                    COL_TOTAL_AMOUNT = 'Q' - 'A',
                    COL_TANGGAL = 0,
                    COL_NAMA_NASABAH = 2,
                    COL_KERTAS_START = 'D'-'A',
                    COL_KERTAS_END = 'J'-'A';

                foreach (string path in paths)
                {
                    List<Abaca> dataDb = (from x in db.Abacas
                                          where x.CustomerCode.ToUpper().Contains("T")
                                          || x.CustomerCode.ToUpper().Contains("D")
                                          select x).ToList();
                    listAbacas = new List<Abaca>();
                    DataSet ds = Util.openExcel(path);
                    DataTable dt = ds.Tables[0];
                    DataRow[] delrows = dt.Select("Column1 is null OR Column1 like 'F%' OR Column1 like 'KODE%'");

                    foreach (var row in delrows)
                        dt.Rows.Remove(row);

                    for (int a = 0; a < dt.Rows.Count; a++)
                    {
                        DataRow row = dt.Rows[a];
                        Int64 totalUangBesar = 0;

                        for(int b = COL_KERTAS_START; b<=COL_KERTAS_END;b++)
                        {
                            Int64 buff;
                            String temp = row[b].ToString();
                            if (!String.IsNullOrEmpty(temp))
                                if (Int64.TryParse(temp, out buff))
                                    totalUangBesar += buff;
                        }   

                        if (dataDb.Where(x => ((DateTime)x.tanggal).Date == DateTime.Parse(row[COL_TANGGAL].ToString()).Date && x.CustomerCode == row[COL_CUSTOMER_CODE].ToString()).ToList().Any())
                        {
                            Console.WriteLine("Revisi");
                            var q = dataDb.Where(x => x.tanggal == DateTime.Parse(row[COL_TANGGAL].ToString()) && x.CustomerCode == row[COL_CUSTOMER_CODE].ToString()).FirstOrDefault();
                            q.totalAmount = Int64.Parse(row[COL_TOTAL_AMOUNT].ToString());
                            q.TotalUangBesar = totalUangBesar;
                            db.SaveChanges();
                        }
                        //else if(listAbacas.Where(x => ((DateTime)x.tanggal).Date == DateTime.Parse(row[COL_TANGGAL].ToString()).Date && x.CustomerCode == row[COL_CUSTOMER_CODE].ToString()).ToList().Any())
                        //{
                        //    var q = listAbacas.Where(x => x.tanggal == DateTime.Parse(row[COL_TANGGAL].ToString()) && x.CustomerCode == row[COL_CUSTOMER_CODE].ToString()).FirstOrDefault();
                        //    q.TotalUangBesar = totalUangBesar;
                        //    q.totalAmount = Int64.Parse(row[COL_TOTAL_AMOUNT].ToString());
                        //}
                        else
                        {
                            Console.WriteLine(row[COL_TANGGAL].ToString());
                            Int64 totalamount = 0, buf;
                            if(!String.IsNullOrEmpty(row[COL_TOTAL_AMOUNT].ToString()))
                                if (Int64.TryParse(row[COL_TOTAL_AMOUNT].ToString(), out buf))
                                    totalamount = buf;
                            
                            listAbacas.Add(new Abaca()
                            {
                                CustomerCode = row[COL_CUSTOMER_CODE].ToString().ToUpper(),
                                tanggal = DateTime.Parse(row[COL_TANGGAL].ToString()),
                                totalAmount = totalamount,
                                CustomerName = row[COL_NAMA_NASABAH].ToString(),
                                TotalUangBesar = totalUangBesar
                            });
                        }
                    }
                    listAbacas = (from x in listAbacas
                                  group x by new { x.CustomerCode, x.tanggal, x.CustomerName } into g
                                  select new Abaca()
                                  {
                                      tanggal = g.Key.tanggal,
                                      CustomerCode = g.Key.CustomerCode,
                                      CustomerName = g.Key.CustomerName,
                                      jobId = "",
                                      vendorCardNo = "",
                                      vendorName = "",
                                      totalAmount = g.Sum(x=>x.totalAmount),
                                      TotalUangBesar = g.Sum(x=>x.TotalUangBesar)
                                  }).ToList();

                    int counter = 0;
                    //foreach(var temp in listAbacas)
                    //{
                    //    Console.WriteLine(counter++);
                    //    Console.WriteLine(temp.CustomerCode + " " + temp.CustomerName + " " + temp.totalAmount + " " + temp.TotalUangBesar);
                    //    db.Abacas.Add(temp);
                    //    db.SaveChanges();
                    //}
                    db.Abacas.AddRange(listAbacas);
                    db.SaveChanges();
                }
                loadForm.CloseForm();
                MessageBox.Show("Done!");
            }
       
        }
    }
}
