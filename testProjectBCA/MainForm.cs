using LumenWorks.Framework.IO.Csv;
using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testProjectBCA
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            //Console.WriteLine(Variables.todayDate.AddDays(-1));
        }

        private void inputOptiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = Variables.csvFilter;
            String connectionString = Variables.connectionString;
            if (of.ShowDialog() == DialogResult.OK)
            { 
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        con.Open();
                        using (SqlCommand command = new SqlCommand("DELETE FROM Opti",con))
                        {
                            command.ExecuteNonQuery();
                        }
                        con.Close();
                    }
                }
                catch (SystemException ex)
                {
                    MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));
                }

                using (var csv = new CsvReader(new StreamReader(of.FileName), true))
                {
                    // Field headers will automatically be used as column names
                    //dataGridView1.DataSource = csv;
                    using (var sbc = new SqlBulkCopy(connectionString))
                    {
                        sbc.DestinationTableName = "dbo.Opti";
                        sbc.BatchSize = 1000;

                        sbc.AddColumnMapping(0, 1);
                        sbc.AddColumnMapping(12, 3);
                        sbc.AddColumnMapping(2, 2);
                        sbc.WriteToServer(csv);
                    }
                }
            }
        }
        private void inputDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MultipleFilesInputForm input = new MultipleFilesInputForm();
            input.MdiParent = this;
            input.Show();
        }

        private void revisiDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Revisi rev = new Revisi();
            rev.MdiParent = this;
            rev.Show();

        }

        private void inputDataPktToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = Variables.excelFilter;
            if (of.ShowDialog() == DialogResult.OK)
            {

                DataSet ds = Util.openExcel(of.FileName);
                for (int i = 2; i < ds.Tables[0].Rows.Count; i++)
                {
                    Database1Entities db = new Database1Entities();
                    Cashpoint newC = (from x in db.Cashpoints where x.idCashpoint == ds.Tables[0].Rows[i][0].ToString() select x).FirstOrDefault();
                    if (newC == null)
                    {
                        newC = new Cashpoint();
                        newC.idCashpoint = ds.Tables[0].Rows[i][0].ToString();
                        newC.kodePkt = ds.Tables[0].Rows[i][5].ToString();
                        Console.WriteLine(newC.idCashpoint);
                        Console.WriteLine(newC.kodePkt);
                        db.Cashpoints.Add(newC);
                    }
                    else
                    {
                        newC.kodePkt= ds.Tables[0].Rows[i][5].ToString();
                    }
                    db.SaveChanges();
                }
            }
        }

        private void inputDataDenomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = Variables.excelFilter;
            if (of.ShowDialog() == DialogResult.OK)
            {
                Database1Entities db = new Database1Entities();
                DataSet ds = Util.openExcel(of.FileName);
               

                for (int i = 1; i < ds.Tables[0].Rows.Count; i++)
                {
                    Console.WriteLine(i);
                    String idCashpoint = ds.Tables[0].Rows[i][0].ToString();
                    Cashpoint newC = (from x in db.Cashpoints where x.idCashpoint == idCashpoint select x).FirstOrDefault();
                    if(newC!=null)
                    {
                        Console.WriteLine(ds.Tables[0].Rows[i][6]);
                        String denom = ds.Tables[0].Rows[i][6].ToString();
                        newC.denom = denom;
                    }
                    else
                    {
                        MessageBox.Show("Data PKT Cashpoint " + ds.Tables[0].Rows[i][0].ToString() + " Tidak ada");
                        newC = new Cashpoint();
                        newC.idCashpoint = ds.Tables[0].Rows[i][0].ToString();
                        db.Cashpoints.Add(newC);
                    }
                    db.SaveChanges();
                }
            }
            else
            {

            }
        }

        private void informationBoardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InformationBoard ib = new InformationBoard();
            ib.MdiParent = this;
            ib.Show();

        }
    }
}
public static class SqlBulkCopyExtensions
{
    public static SqlBulkCopyColumnMapping AddColumnMapping(this SqlBulkCopy sbc, int sourceColumnOrdinal, int targetColumnOrdinal)
    {
        var map = new SqlBulkCopyColumnMapping(sourceColumnOrdinal, targetColumnOrdinal);
        sbc.ColumnMappings.Add(map);

        return map;
    }

    public static SqlBulkCopyColumnMapping AddColumnMapping(this SqlBulkCopy sbc, string sourceColumn, string targetColumn)
    {
        var map = new SqlBulkCopyColumnMapping(sourceColumn, targetColumn);
        sbc.ColumnMappings.Add(map);

        return map;
    }
}