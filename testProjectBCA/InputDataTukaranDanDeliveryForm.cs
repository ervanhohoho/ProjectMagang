﻿using System;
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
                String [] paths = of.FileNames;
                List<Abaca> listAbacas = new List<Abaca>();
                List<Abaca> dataDb = (from x in db.Abacas select x).ToList();
                foreach (string path in paths)
                {
                    DataSet ds = Util.openExcel(path);
                    DataTable dt = ds.Tables[0];
                    DataRow[] delrows = dt.Select("Column0 is null OR Column0 like 'F%'");

                    foreach (var row in delrows)
                        dt.Rows.Remove(row);

                    for (int a = 0; a < dt.Rows.Count; a++)
                    {
                        DataRow row = dt.Rows[a];
                        if (dataDb.Where(x => x.tanggal == DateTime.Parse(row[3].ToString()) && x.CustomerCode == row[0].ToString() ).FirstOrDefault()!=null )
                        {
                            var q = dataDb.Where(x => x.tanggal == DateTime.Parse(row[3].ToString()) && x.CustomerCode == row[0].ToString()).FirstOrDefault();
                            q.totalAmount = Int64.Parse(row[2].ToString());
                        }
                        else if(listAbacas.Where(x => x.tanggal == DateTime.Parse(row[3].ToString()) && x.CustomerCode == row[0].ToString()).FirstOrDefault() != null)
                        {
                            var q = listAbacas.Where(x => x.tanggal == DateTime.Parse(row[3].ToString()) && x.CustomerCode == row[0].ToString()).FirstOrDefault();
                            q.totalAmount = Int64.Parse(row[2].ToString());
                        }
                        else
                        {
                            listAbacas.Add(new Abaca()
                            {
                                CustomerCode = row[0].ToString(),
                                tanggal = DateTime.Parse(row[3].ToString()),
                                totalAmount = Int64.Parse(row[2].ToString()),
                                CustomerName = row[1].ToString()
                            });
                        }
                    }
                    db.Abacas.AddRange(listAbacas);
                    db.SaveChanges();
                }
            }
        }
    }
}
