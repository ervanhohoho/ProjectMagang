﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testProjectBCA
{
    public partial class initTransaksi : Form
    {
        public initTransaksi()
        {
            InitializeComponent();
            Console.Write(alphabetToIndex('n'));
            Database1Entities db = new Database1Entities();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "Microsoft Excel | *.xls; *.xlsx; *.xlsm;";
            if(of.ShowDialog() == DialogResult.OK)
                initTable(of.FileName);
        }
        private void initTable(String filepath)
        {
            
            DataSet ds = Util.openExcel(filepath);
            DataTable tb = ds.Tables[0];
            DataTable dt = ds.Tables[0];
            dt.Rows.RemoveAt(0);

            DataRow[] dr = dt.Select("Column0 IS null");
            foreach (DataRow row in dr)
                dt.Rows.Remove(row);
            dataGridView1.DataSource = dt;

            using (SqlConnection conn = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand command = new SqlCommand("", conn))
                {

                    conn.Open();

                    command.CommandText = "DELETE FROM TransaksiAtms";
                    command.ExecuteNonQuery();
                    command.CommandText = "DBCC CHECKIDENT ('TransaksiAtms', RESEED, 1)";
                    command.ExecuteNonQuery();
                    //Creating temp table on database
                    command.CommandText = "CREATE TABLE #TempTable(idCashpoint VARCHAR(255), denom VARCHAR(255))";
                    command.ExecuteNonQuery();


                    //Bulk insert into temp table
                    using (SqlBulkCopy bulkcopy = new SqlBulkCopy(conn))
                    {
                        bulkcopy.BulkCopyTimeout = 660;
                        bulkcopy.DestinationTableName = "dbo.TransaksiAtms";
                        bulkcopy.ColumnMappings.Add(0, 2);
                        bulkcopy.ColumnMappings.Add(3, 1);
                        bulkcopy.ColumnMappings.Add(6, 3);
                        bulkcopy.ColumnMappings.Add(5, 4);
                        bulkcopy.ColumnMappings.Add(4, 5);
                        bulkcopy.ColumnMappings.Add(10, 6);
                        bulkcopy.ColumnMappings.Add(9, 7);
                        bulkcopy.ColumnMappings.Add(8, 8);
                        bulkcopy.ColumnMappings.Add(16, 9);
                        bulkcopy.ColumnMappings.Add(15, 10);
                        bulkcopy.ColumnMappings.Add(14, 11);
                        bulkcopy.ColumnMappings.Add(13, 12);
                        bulkcopy.ColumnMappings.Add(12, 13);
                        bulkcopy.ColumnMappings.Add(11, 14);
                        bulkcopy.ColumnMappings.Add(19, 15);
                        bulkcopy.ColumnMappings.Add(18, 16);
                        bulkcopy.ColumnMappings.Add(17, 17);
                        bulkcopy.ColumnMappings.Add(22, 18);
                        bulkcopy.ColumnMappings.Add(21, 19);
                        bulkcopy.ColumnMappings.Add(20, 20);
                        bulkcopy.ColumnMappings.Add(25, 21);
                        bulkcopy.ColumnMappings.Add(24, 22);
                        bulkcopy.ColumnMappings.Add(23, 23);
                        bulkcopy.ColumnMappings.Add(28, 24);
                        bulkcopy.ColumnMappings.Add(27, 25);
                        bulkcopy.ColumnMappings.Add(26, 26);
                        bulkcopy.ColumnMappings.Add(31, 27);
                        bulkcopy.ColumnMappings.Add(30, 28);
                        bulkcopy.ColumnMappings.Add(29, 29);
                        bulkcopy.ColumnMappings.Add(34, 30);
                        bulkcopy.ColumnMappings.Add(33, 31);
                        bulkcopy.ColumnMappings.Add(32, 32);
                        bulkcopy.WriteToServer(dt);
                        bulkcopy.Close();
                    }
                    //Update isi dari null jadi 0
                    command.CommandText = "UPDATE TransaksiAtms SET isiAtm100 = 0 WHERE isiAtm100 is NULL";
                    command.ExecuteNonQuery();
                    command.CommandText = "UPDATE TransaksiAtms SET isiAtm50 = 0 WHERE isiAtm50 is NULL";
                    command.ExecuteNonQuery();
                    command.CommandText = "UPDATE TransaksiAtms SET isiAtm20 = 0 WHERE isiAtm20 is NULL";
                    command.ExecuteNonQuery();

                    command.CommandText = "UPDATE TransaksiAtms SET isiCrm100 = 0 WHERE isiCrm100 is NULL";
                    command.ExecuteNonQuery();
                    command.CommandText = "UPDATE TransaksiAtms SET isiCrm50 = 0 WHERE isiCrm50 is NULL";
                    command.ExecuteNonQuery();
                    command.CommandText = "UPDATE TransaksiAtms SET isiCrm20 = 0 WHERE isiCrm20 is NULL";
                    command.ExecuteNonQuery();

                    //Update sislok dari null jadi 0
                    command.CommandText = "UPDATE TransaksiAtms SET sislokCrm100 = 0 WHERE sislokCrm100 is NULL";
                    command.ExecuteNonQuery();
                    command.CommandText = "UPDATE TransaksiAtms SET sislokCrm50 = 0 WHERE sislokCrm50 is NULL";
                    command.ExecuteNonQuery();
                    command.CommandText = "UPDATE TransaksiAtms SET sislokCrm20 = 0 WHERE sislokCrm20 is NULL";
                    command.ExecuteNonQuery();

                    command.CommandText = "UPDATE TransaksiAtms SET sislokCdm100 = 0 WHERE sislokCdm100 is NULL";
                    command.ExecuteNonQuery();
                    command.CommandText = "UPDATE TransaksiAtms SET sislokCdm50 = 0 WHERE sislokCdm50 is NULL";
                    command.ExecuteNonQuery();
                    command.CommandText = "UPDATE TransaksiAtms SET sislokCdm20 = 0 WHERE sislokCdm20 is NULL";
                    command.ExecuteNonQuery();

                    command.CommandText = "UPDATE TransaksiAtms SET sislokAtm100 = 0 WHERE sislokAtm100 is NULL";
                    command.ExecuteNonQuery();
                    command.CommandText = "UPDATE TransaksiAtms SET sislokAtm50 = 0 WHERE sislokAtm50 is NULL";
                    command.ExecuteNonQuery();
                    command.CommandText = "UPDATE TransaksiAtms SET sislokAtm20 = 0 WHERE sislokAtm20 is NULL";
                    command.ExecuteNonQuery();

                    //Update adhoc dari null jadi 0
                    command.CommandText = "UPDATE TransaksiAtms SET adhoc100 = 0 WHERE adhoc100 is NULL";
                    command.ExecuteNonQuery();
                    command.CommandText = "UPDATE TransaksiAtms SET adhoc50 = 0 WHERE adhoc50 is NULL";
                    command.ExecuteNonQuery();
                    command.CommandText = "UPDATE TransaksiAtms SET adhoc20 = 0 WHERE adhoc20 is NULL";
                    command.ExecuteNonQuery();

                    //Update setor dari null jadi 0 
                    command.CommandText = "UPDATE TransaksiAtms SET setor100 = 0 WHERE setor100 is NULL";
                    command.ExecuteNonQuery();
                    command.CommandText = "UPDATE TransaksiAtms SET setor50 = 0 WHERE setor50 is NULL";
                    command.ExecuteNonQuery();
                    command.CommandText = "UPDATE TransaksiAtms SET setor20 = 0 WHERE setor20 is NULL";
                    command.ExecuteNonQuery();

                    //Update bon dari null jadi 0
                    command.CommandText = "UPDATE TransaksiAtms SET bon100 = 0 WHERE bon100 is NULL";
                    command.ExecuteNonQuery();
                    command.CommandText = "UPDATE TransaksiAtms SET bon50 = 0 WHERE bon50 is NULL";
                    command.ExecuteNonQuery();
                    command.CommandText = "UPDATE TransaksiAtms SET bon20 = 0 WHERE bon20 is NULL";
                    command.ExecuteNonQuery();
                }

            }
            MessageBox.Show("Done!");
        }
        int alphabetToIndex(char aplhabet)
        {
            return (int)aplhabet - (int)'a';
        }
        private String doubleToString(double temp)
        {
            var x = Math.Round(temp, 0);
            return x + "";
        }
        private Int64 doubleToInt64(double temp)
        {
            var x = Math.Round(temp, 0);
            return (Int64)x;
        }
    }
}
