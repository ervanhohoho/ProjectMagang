using LumenWorks.Framework.IO.Csv;
using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
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
            inputOpti();
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
            inputDataPkt();
        }
        private void inputDataDenomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            inputDataDenom();
        }
        private void informationBoardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InformationBoard ib = new InformationBoard();
            ib.MdiParent = this;
            ib.Show();

        }
        private void inputOpti()
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
                        using (SqlCommand command = new SqlCommand("DELETE FROM Opti", con))
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
        private void inputDataPkt()
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = Variables.excelFilter;
            if (of.ShowDialog() == DialogResult.OK)
            {

                DataSet ds = Util.openExcel(of.FileName);
                //for (int i = 1; i < ds.Tables[0].Rows.Count; i++)
                //{
                //    Database1Entities db = new Database1Entities();
                //    Console.WriteLine(ds.Tables[0].Rows[i][0].ToString());
                //    if (ds.Tables[0].Rows[i][0].ToString().Substring(0,1) == "B")
                //        continue;
                //    String temp = ds.Tables[0].Rows[i][0].ToString();
                //    var newC = (from x in db.Cashpoints where x.idCashpoint == temp select x).FirstOrDefault();
                //    if (newC == null)
                //    {
                //        newC = new Cashpoint();
                //        newC.idCashpoint = ds.Tables[0].Rows[i][0].ToString();
                //        newC.kodePkt = ds.Tables[0].Rows[i][5].ToString();
                //        Console.WriteLine(newC.idCashpoint);
                //        Console.WriteLine(newC.kodePkt);
                //        db.Cashpoints.Add(newC);
                //    }
                //    else
                //    {
                //        newC.kodePkt = ds.Tables[0].Rows[i][5].ToString();
                //    }
                //    db.SaveChanges();
                //}
                DataTable dt = ds.Tables[0];
                DataRow[] rows = ds.Tables[0].Select("Column0 not like 'A%' OR LEN(Column0) > 5");
                foreach (DataRow row in rows)
                {
                    dt.Rows.Remove(row);
                }
                UpdateDataCashpointPkt(dt);
            }
        }
        private void inputDataDenom()
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = Variables.excelFilter;
            if (of.ShowDialog() == DialogResult.OK)
            {
                //Database1Entities db = new Database1Entities();
                DataSet ds = Util.openExcel(of.FileName);


                //for (int i = 1; i < ds.Tables[0].Rows.Count; i++)
                //{
                //    Console.WriteLine(i);
                //    String idCashpoint = ds.Tables[0].Rows[i][0].ToString();
                //    Cashpoint newC = (from x in db.Cashpoints where x.idCashpoint == idCashpoint select x).FirstOrDefault();
                //    if(newC!=null)
                //    {
                //        Console.WriteLine(ds.Tables[0].Rows[i][6]);
                //        String denom = ds.Tables[0].Rows[i][6].ToString();
                //        newC.denom = denom;
                //    }
                //    else
                //    {
                //        MessageBox.Show("Data PKT Cashpoint " + ds.Tables[0].Rows[i][0].ToString() + " Tidak ada");
                //        newC = new Cashpoint();
                //        newC.idCashpoint = ds.Tables[0].Rows[i][0].ToString();
                //        db.Cashpoints.Add(newC);
                //    }
                //    db.SaveChanges();
                //}
                UpdateDataCashpointDenom(ds.Tables[0]);
            }
            else
            {

            }
        }
        public static void UpdateDataCashpointPkt(DataTable dt)
        {
            using (SqlConnection conn = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand command = new SqlCommand("", conn))
                {
                    try
                    {
                        conn.Open();

                        //Creating temp table on database
                        command.CommandText = "CREATE TABLE #TempTable(idCashpoint VARCHAR(255), kodePkt VARCHAR(255))";
                        command.ExecuteNonQuery();

                        //Bulk insert into temp table
                        using (SqlBulkCopy bulkcopy = new SqlBulkCopy(conn))
                        {
                            //bulkcopy.BulkCopyTimeout = 660;
                            bulkcopy.DestinationTableName = "#TempTable";
                            bulkcopy.ColumnMappings.Add(0, 0);
                            bulkcopy.ColumnMappings.Add(5, 1);
                            bulkcopy.WriteToServer(dt);
                            bulkcopy.Close();
                        }

                        // Updating destination table, and dropping temp table
                        command.CommandTimeout = 300;
                        command.CommandText = "INSERT INTO Cashpoint(idCashpoint, kodePkt) SELECT TT.idCashpoint, TT.kodePkt FROM #TempTable TT LEFT JOIN Cashpoint C ON TT.idCashpoint = C.idCashpoint WHERE C.idCashpoint IS  NULL";
                        command.ExecuteNonQuery();
                        command.CommandText = "UPDATE T SET T.kodePkt = TT.kodePkt FROM Cashpoint T INNER JOIN #TempTable TT ON TT.idCashpoint = T.idCashpoint; DROP TABLE #TempTable;";
                        command.ExecuteNonQuery();
                        command.CommandText = "DELETE FROM Cashpoint WHERE kodePkt = 'AMRT'";
                        command.ExecuteNonQuery();
                        command.CommandText = "UPDATE Cashpoint SET kodePkt = 'AMRT' WHERE kodePkt = 'AMRT2'";
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        // Handle exception properly
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }
        public static void UpdateDataCashpointDenom(DataTable dt)
        {
            using (SqlConnection conn = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand command = new SqlCommand("", conn))
                {
                    try
                    {
                        conn.Open();

                        //Creating temp table on database
                        command.CommandText = "CREATE TABLE #TempTable(idCashpoint VARCHAR(255), denom VARCHAR(255))";
                        command.ExecuteNonQuery();

                        dt.Rows.RemoveAt(0);
                        //Bulk insert into temp table
                        using (SqlBulkCopy bulkcopy = new SqlBulkCopy(conn))
                        {
                            //bulkcopy.BulkCopyTimeout = 660;
                            bulkcopy.DestinationTableName = "#TempTable";
                            bulkcopy.ColumnMappings.Add(0, 0);
                            bulkcopy.ColumnMappings.Add(6, 1);
                            bulkcopy.WriteToServer(dt);
                            bulkcopy.Close();
                        }

                        // Updating destination table, and dropping temp table
                        command.CommandTimeout = 300;
                        command.CommandText = "INSERT INTO Cashpoint(idCashpoint, denom) SELECT TT.idCashpoint, TT.denom FROM #TempTable TT LEFT JOIN Cashpoint C ON TT.idCashpoint = C.idCashpoint WHERE C.idCashpoint IS  NULL";
                        command.ExecuteNonQuery();
                        command.CommandText = "UPDATE T SET T.denom = TT.denom FROM Cashpoint T INNER JOIN #TempTable TT ON TT.idCashpoint = T.idCashpoint; DROP TABLE #TempTable;";
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        // Handle exception properly
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        private void insertDataCabangToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            if(of.ShowDialog() == DialogResult.OK)
            {
                insertDataPktCabang(of.FileName);
            }
        }

        private void insertDataPktCabang(String filename)
        {
            using (var csv = new CsvReader(new StreamReader(filename), true))
            {
                using (var sbc = new SqlBulkCopy(Variables.connectionString))
                {
                    sbc.DestinationTableName = "dbo.Cabang";
                    sbc.BatchSize = 1000;

                    sbc.AddColumnMapping(0, 0);
                    sbc.AddColumnMapping(4, 1);
                    sbc.WriteToServer(csv);
                }
            }
            using(SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    sql.Open();
                    cmd.Connection = sql;
                    cmd.CommandText = "UPDATE CABANG SET kodeCabang = RIGHT(kodeCabang,4)";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "UPDATE CABANG SET kodeCabang = IIF(kodeCabang like '000%',RIGHT(kodeCabang,1),IIF(kodeCabang like '00%', RIGHT(kodeCabang,2), IIF(kodeCabang like '0%', RIGHT(kodeCabang,3),kodeCabang)))";
                    cmd.ExecuteNonQuery();
                    sql.Close();
                }
            } 
            
        }

        private void insertDataKanwilCabangToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            if (of.ShowDialog() == DialogResult.OK)
            {
                insertDataKanwilCabang(of.FileName);
            }
        }
        private void insertDataKanwilCabang(String filepath)
        {

            DataSet ds = Util.openExcel(filepath);
            DataTable dt = ds.Tables[0];
            DataRow[] rows = dt.Select("Column3 not like 'JABO%'");


            foreach(var row in rows)
            {
                dt.Rows.Remove(row);
            }
            Console.WriteLine(dt.Rows[0][0].ToString());
            Console.WriteLine(dt.Rows[0][1].ToString());
            Console.WriteLine(dt.Rows[0][2].ToString());
            using (SqlConnection conn = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand command = new SqlCommand("", conn))
                {
                    try
                    {
                        conn.Open();

                        //Creating temp table on database
                        command.CommandText = "CREATE TABLE #TempTable(kodeCabang VARCHAR(255), kanwil VARCHAR(255))";
                        command.ExecuteNonQuery();

                        //Bulk insert into temp table
                        using (SqlBulkCopy bulkcopy = new SqlBulkCopy(conn))
                        {
                            //bulkcopy.BulkCopyTimeout = 660;
                            bulkcopy.DestinationTableName = "#TempTable";
                            bulkcopy.ColumnMappings.Add(0, 0);
                            bulkcopy.ColumnMappings.Add(2, 1);
                            bulkcopy.WriteToServer(dt);
                            bulkcopy.Close();
                        }

                        // Updating destination table, and dropping temp table
                        command.CommandTimeout = 300;
                        command.CommandText = "INSERT INTO Cabang(kodeCabang, kanwil) SELECT TT.kodeCabang, TT.kanwil FROM #TempTable TT LEFT JOIN Cabang C ON TT.kodeCabang = C.kodeCabang WHERE C.kodeCabang IS  NULL";
                        command.ExecuteNonQuery();
                        command.CommandText = "UPDATE T SET T.kanwil = TT.kanwil FROM Cabang T INNER JOIN #TempTable TT ON TT.kodeCabang = T.kodeCabang; DROP TABLE #TempTable;";
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        // Handle exception properly
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        private void inputTransaksiCabangToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InputTransaksiCabangForm itcf = new InputTransaksiCabangForm();
            itcf.MdiParent = this;
            itcf.Show();
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
static class DateTimeExtensions
{
    static GregorianCalendar _gc = new GregorianCalendar();
    public static int GetWeekOfMonth(this DateTime time)
    {
        DateTime first = new DateTime(time.Year, time.Month, 1);
        return time.GetWeekOfYear() - first.GetWeekOfYear() + 1;
    }

    static int GetWeekOfYear(this DateTime time)
    {
        return _gc.GetWeekOfYear(time, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
    }
}