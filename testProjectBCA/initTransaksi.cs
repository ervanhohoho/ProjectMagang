using System;
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
                }
            }

                    
                        //int counter = 0;
                        //for (int i = 4; i < 27850; i++)
                        //{

                        //    //Dari form input
                        //    transaksiPkt pkt = new transaksiPkt();
                        //    pkt.kodePkt = tb.Rows[i][0].ToString();
                        //    pkt.tanggalPengajuan = (DateTime)tb.Rows[i][3];

                        //    //Saldo awal
                        //    for (int j = 4; j < alphabetToIndex('g') + 1; j++)
                        //    {
                        //        if(tb.Rows[i][j].ToString()!="")
                        //            pkt.saldoAwal.Add(doubleToInt64((double)tb.Rows[i][j]));
                        //        else
                        //            pkt.saldoAwal.Add(0);
                        //    }
                        //    //Sislok/bongkaran atm
                        //    for (int j = alphabetToIndex('i'); j < alphabetToIndex('k') + 1; j++)
                        //    {
                        //        if (tb.Rows[i][j].ToString() != "")
                        //            pkt.bongkaranAtm.Add(doubleToInt64((double)tb.Rows[i][j]));
                        //        else
                        //            pkt.bongkaranAtm.Add(0);
                        //    }
                        //    //Sislok/bongkaran cdm
                        //    for (int j = alphabetToIndex('l'); j < alphabetToIndex('n') + 1; j++)
                        //    {
                        //        if (tb.Rows[i][j].ToString() != "")
                        //            pkt.bongkaranCdm.Add(doubleToInt64((double)tb.Rows[i][j]));
                        //        else
                        //            pkt.bongkaranCdm.Add(0);
                        //    }
                        //    //Bongkaran crm
                        //    for (int j = alphabetToIndex('o'); j < alphabetToIndex('q') + 1; j++)
                        //    {
                        //        if (tb.Rows[i][j].ToString() != "")
                        //            pkt.bongkaranCrm.Add(doubleToInt64((double)tb.Rows[i][j]));
                        //        else
                        //            pkt.bongkaranCrm.Add(0);
                        //    }
                        //    //Isi ATM
                        //    for (int j = alphabetToIndex('r'); j < alphabetToIndex('t') + 1; j++)
                        //    {
                        //        if (tb.Rows[i][j].ToString() != "")
                        //            pkt.pengisianAtm.Add(doubleToInt64((double)tb.Rows[i][j]));
                        //        else
                        //            pkt.pengisianAtm.Add(0);
                        //    }
                        //    //Isi CRM
                        //    for (int j = alphabetToIndex('u'); j < alphabetToIndex('w') + 1; j++)
                        //    {
                        //        if (tb.Rows[i][j].ToString() != "")
                        //            pkt.pengisianCrm.Add(doubleToInt64((double)tb.Rows[i][j]));
                        //        else
                        //            pkt.pengisianCrm.Add(0);
                        //    }
                        //    //Bon
                        //    for (int j = alphabetToIndex('x'); j < alphabetToIndex('z') + 1; j++)
                        //    {
                        //        if (tb.Rows[i][j].ToString() != "")
                        //            pkt.penerimaanBon.Add(doubleToInt64((double)tb.Rows[i][j]));
                        //        else
                        //            pkt.penerimaanBon.Add(0);
                        //    }
                        //    //Adhoc
                        //    for (int j = alphabetToIndex('z') + 1; j < alphabetToIndex('z') + 4; j++)
                        //    {
                        //        if (tb.Rows[i][j].ToString() != "")
                        //            pkt.penerimaanBonAdhoc.Add(doubleToInt64((double)tb.Rows[i][j]));
                        //        else
                        //            pkt.penerimaanBonAdhoc.Add(0);
                        //    }
                        //    //Setor
                        //    for (int j = alphabetToIndex('z') + 4; j < alphabetToIndex('z') + 7; j++)
                        //    {
                        //        if (tb.Rows[i][j].ToString() != "")
                        //            pkt.setorUang.Add(doubleToInt64((double)tb.Rows[i][j]));
                        //        else
                        //            pkt.setorUang.Add(0);
                        //    }
                        //    //Saldo Akhir
                        //    for (int j = alphabetToIndex('z') + 7; j < alphabetToIndex('z') + 10; j++)
                        //    {
                        //        if (tb.Rows[i][j].ToString() != "")
                        //            pkt.saldoAkhir.Add(doubleToInt64((double)tb.Rows[i][j]));
                        //        else
                        //            pkt.saldoAkhir.Add(0);
                        //    }
                        //    Util.inputDataTransaksiATMToDB(pkt);
                        //}
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
