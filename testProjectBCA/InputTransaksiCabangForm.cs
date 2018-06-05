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
    public partial class InputTransaksiCabangForm : Form
    {
        Database1Entities db = new Database1Entities();
        String kodePkt = "";
        DataTable collectionLainnya;
        DataTable collectionCabang;
        DataTable collectionRetail;
        DataTable deliveryCabang;
        DataTable deliveryRetail;
        DataTable deliveryLainnya;
        List<Pkt> listPkt;
        public InputTransaksiCabangForm()
        {
            InitializeComponent();
        }

        private void SelectFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Multiselect = true;
            of.Filter = Variables.excelFilter;
            listPkt = (from x in db.Pkts
                       select x).ToList();
            if(of.ShowDialog() == DialogResult.OK)
            {
                String [] filenames = of.FileNames;


               
                foreach (String filename in filenames)
                {
                    //List<Pkt> listPkt = (from x in db.Pkts select x).ToList(); 
                    

                    
                   //Console.WriteLine(kodePkt);
                    DataSet ds = Util.openExcel(filename);

                    DateTime date = (DateTime)ds.Tables[0].Rows[2][0];
                    deleteFromDB(date, kodePkt);

                    collectionCabang = ds.Tables[0];
                    readCollectionCabang(ds);
                    collectionRetail = ds.Tables[1];
                    readCollectionRetail(ds);
                    collectionLainnya = ds.Tables[2];
                    readCollectionLainnya(ds);
                    deliveryCabang = ds.Tables[3];
                    readDeliveryCabang(ds);
                    deliveryRetail = ds.Tables[4];
                    readDeliveryRetail(ds);
                    deliveryLainnya = ds.Tables[5];
                    readDeliveryLainnya(ds);
                }
                hilangkanNull();
            }
        }
        private void deleteFromDB(DateTime date, string kodePkt)
        {
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();
                    cmd.CommandText = "DELETE FROM DailyStock WHERE kodePkt = '" + kodePkt + "' AND tanggal = '" + date.ToShortDateString()+"'";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void hilangkanNull()
        {
            using (SqlConnection sql = new SqlConnection(Variables.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = sql;
                    sql.Open();

                    cmd.CommandText = "UPDATE DailyStock SET BN100K = 0 WHERE BN100K is NULL";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "UPDATE DailyStock SET BN50K = 0 WHERE BN50K is NULL";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "UPDATE DailyStock SET BN20K = 0 WHERE BN20K is NULL";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "UPDATE DailyStock SET BN10K = 0 WHERE BN10K is NULL";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "UPDATE DailyStock SET BN5K = 0 WHERE BN5K is NULL";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "UPDATE DailyStock SET BN2K = 0 WHERE BN2K is NULL";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "UPDATE DailyStock SET BN1K = 0 WHERE BN1K is NULL";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "UPDATE DailyStock SET BN500 = 0 WHERE BN500 is NULL";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "UPDATE DailyStock SET BN200 = 0 WHERE BN200 is NULL";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "UPDATE DailyStock SET BN100 = 0 WHERE BN100 is NULL";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "UPDATE DailyStock SET CN1K = 0 WHERE CN1K is NULL";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "UPDATE DailyStock SET CN500 = 0 WHERE CN500 is NULL";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "UPDATE DailyStock SET CN200 = 0 WHERE CN200 is NULL";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "UPDATE DailyStock SET CN100 = 0 WHERE CN100 is NULL";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "UPDATE DailyStock SET CN50 = 0 WHERE CN50 is NULL";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "UPDATE DailyStock SET CN25 = 0 WHERE CN25 is NULL";
                    cmd.ExecuteNonQuery();
                }
            }
        }
        private void readCollectionCabang(DataSet ds)
        {
            DataTable dt = ds.Tables[0];
            //Ambil Nama PKT
            String namaPkt = dt.Rows[0][1].ToString();
            kodePkt = (from x in listPkt
                       where x.namaPkt == namaPkt
                       select x.kodePkt).FirstOrDefault();

            DataRow[] rows = dt.Select("not LEN(Column2) = 4 OR Column1 IN('Vendor', 'Tanggal%')");
            foreach (var row in rows)
            {
                dt.Rows.Remove(row);
            }
            dt.Rows.Remove(dt.Rows[0]); dt.Rows.Remove(dt.Rows[dt.Rows.Count - 1]);
            //Console.WriteLine(dt.Rows.Count);
            //Console.WriteLine(dt.Rows[dt.Rows.Count - 1][2].ToString());
            dt.Columns.Add("kodePkt", typeof(String));
            dt.Columns["kodePkt"].DefaultValue = kodePkt;
            for (int a = 0; a < dt.Rows.Count; a++)
            {
                dt.Rows[a]["kodePkt"] = kodePkt;
            }

            dt.Columns.Add("jenisTransaksi", typeof(String));
            for (int a = 0; a < dt.Rows.Count; a++)
            {
                dt.Rows[a]["jenisTransaksi"] = "Collection Cabang - " + dt.Rows[a][1].ToString();
            }

            dt.Columns.Add("inout", typeof(String));
            for (int a = 0; a < dt.Rows.Count; a++)
            {
                dt.Rows[a]["inout"] = "IN";
            }
            //Console.WriteLine(dt.Rows[dt.Rows.Count - 1][dt.Columns.Count - 1].ToString());
            using (SqlBulkCopy sbc = new SqlBulkCopy(Variables.connectionString))
            {
                sbc.DestinationTableName = "dbo.DailyStock";
                sbc.ColumnMappings.Add(2, 5); //Kode Cabang
                sbc.ColumnMappings.Add(3, 6); //Nama Cabang
                sbc.ColumnMappings.Add(4, 7); //Keterangan Cabang
                sbc.ColumnMappings.Add(dt.Columns.Count - 3, 2); //kodePkt
                sbc.ColumnMappings.Add(dt.Columns.Count - 2, 4); //Jenis Transaksi
                sbc.ColumnMappings.Add(dt.Columns.Count - 1, 3); //IN-OUT
                sbc.ColumnMappings.Add(0, 1); //Tanggal
                for (int a = 0; a < 15; a++)
                    sbc.ColumnMappings.Add(6 + a, 8 + a); //BN + Coin
                sbc.WriteToServer(dt);
                sbc.Close();
            }
        }
        private void readCollectionRetail(DataSet ds)
        {
            DataTable dt = ds.Tables[1];
            dt.Rows.RemoveAt(0);
            dt.Rows.Remove(dt.Rows[0]); dt.Rows.Remove(dt.Rows[dt.Rows.Count - 1]);

            DataRow[] rows = dt.Select("Column2 is NULL");
            foreach (var row in rows)
            {
                dt.Rows.Remove(row);
            }
            
            
            //Console.WriteLine(dt.Rows.Count);
            //Console.WriteLine(dt.Rows[4][4]);
            //Console.WriteLine(dt.Rows[dt.Rows.Count - 1][2].ToString());
            dt.Columns.Add("kodePkt", typeof(String));
            dt.Columns["kodePkt"].DefaultValue = kodePkt;
            for (int a = 0; a < dt.Rows.Count; a++)
            {
                dt.Rows[a]["kodePkt"] = kodePkt;
            }

            dt.Columns.Add("jenisTransaksi", typeof(String));
            for (int a = 0; a < dt.Rows.Count; a++)
            {
                dt.Rows[a]["jenisTransaksi"] = "Collection Retail";
            }

            dt.Columns.Add("inout", typeof(String));
            for (int a = 0; a < dt.Rows.Count; a++)
            {
                dt.Rows[a]["inout"] = "IN";
            }

            //Console.WriteLine(dt.Rows[dt.Rows.Count - 1][dt.Columns.Count - 1].ToString());
            using (SqlBulkCopy sbc = new SqlBulkCopy(Variables.connectionString))
            {
                sbc.DestinationTableName = "dbo.DailyStock";
                //Kode Nasabah
                sbc.ColumnMappings.Add(1, 5);
                //Nama Nasabah
                sbc.ColumnMappings.Add(2, 6);
                //Kode Pkt
                sbc.ColumnMappings.Add(dt.Columns.Count - 3, 2);
                //Jenis Transaksi
                sbc.ColumnMappings.Add(dt.Columns.Count - 2, 4);
                //IN - OUT
                sbc.ColumnMappings.Add(dt.Columns.Count - 1, 3);
                //Tanggal
                sbc.ColumnMappings.Add(0, 1);
                //Uang
                for (int a = 0; a < 15; a++)
                    sbc.ColumnMappings.Add(4 + a, 8 + a);
                sbc.WriteToServer(dt);
                sbc.Close();
            }
        }
        private void readCollectionLainnya(DataSet ds)
        {
            DataTable dt = ds.Tables[2];
            dt.Rows.RemoveAt(0);
            dt.Rows.Remove(dt.Rows[0]); dt.Rows.Remove(dt.Rows[dt.Rows.Count - 1]);

            DataRow[] rows = dt.Select("Column2 is NULL");
            foreach (var row in rows)
            {
                dt.Rows.Remove(row);
            }


            //Console.WriteLine(dt.Rows.Count);
            //Console.WriteLine(dt.Rows[0][2]);
            //Console.WriteLine(dt.Rows[dt.Rows.Count - 1][2].ToString());
            dt.Columns.Add("kodePkt", typeof(String));
            dt.Columns["kodePkt"].DefaultValue = kodePkt;
            for (int a = 0; a < dt.Rows.Count; a++)
            {
                dt.Rows[a]["kodePkt"] = kodePkt;
            }

            dt.Columns.Add("jenisTransaksi", typeof(String));
            for (int a = 0; a < dt.Rows.Count; a++)
            {
                dt.Rows[a]["jenisTransaksi"] = "Collection Lainnya - "+dt.Rows[a][1].ToString();
            }

            dt.Columns.Add("inout", typeof(String));
            for (int a = 0; a < dt.Rows.Count; a++)
            {
                dt.Rows[a]["inout"] = "IN";
            }

            //Console.WriteLine(dt.Rows[dt.Rows.Count - 1][dt.Columns.Count - 1].ToString());
            using (SqlBulkCopy sbc = new SqlBulkCopy(Variables.connectionString))
            {
                sbc.DestinationTableName = "dbo.DailyStock";
                //Kode Pkt
                sbc.ColumnMappings.Add(dt.Columns.Count - 3, 2);
                //Jenis Transaksi
                sbc.ColumnMappings.Add(dt.Columns.Count - 2, 4);
                //IN-OUT
                sbc.ColumnMappings.Add(dt.Columns.Count - 1, 3);
                //Tanggal
                sbc.ColumnMappings.Add(0, 1);
                //Keterangan Sumber Dana
                sbc.ColumnMappings.Add(2, 7);
                //Uang
                for (int a = 0; a < 15; a++)
                    sbc.ColumnMappings.Add(4 + a, 8 + a);
                sbc.WriteToServer(dt);
                sbc.Close();
            }
        }
        private void readDeliveryCabang(DataSet ds)
        {
            DataTable dt = ds.Tables[3];
            dt.Rows.RemoveAt(0);
            dt.Rows.Remove(dt.Rows[0]); dt.Rows.Remove(dt.Rows[dt.Rows.Count - 1]);
            DataRow[] rows = dt.Select("Column2 is NULL");
            foreach (var row in rows)
            {
                dt.Rows.Remove(row);
            }
            
            dt.Columns.Add("kodePkt", typeof(String));
            dt.Columns["kodePkt"].DefaultValue = kodePkt;
            for (int a = 0; a < dt.Rows.Count; a++)
            {
                dt.Rows[a]["kodePkt"] = kodePkt;
            }

            dt.Columns.Add("jenisTransaksi", typeof(String));
            for (int a = 0; a < dt.Rows.Count; a++)
            {
                dt.Rows[a]["jenisTransaksi"] = "Delivery Cabang - " + dt.Rows[a][1].ToString();
            }

            dt.Columns.Add("inout", typeof(String));
            for (int a = 0; a < dt.Rows.Count; a++)
            {
                dt.Rows[a]["inout"] = "OUT";
            }

            using (SqlBulkCopy sbc = new SqlBulkCopy(Variables.connectionString))
            {
                sbc.DestinationTableName = "dbo.DailyStock";
                sbc.ColumnMappings.Add(2, 5); //Kode Cabang
                sbc.ColumnMappings.Add(3, 6); //Nama Cabang
                sbc.ColumnMappings.Add(4, 7); //Keterangan Cabang
                sbc.ColumnMappings.Add(dt.Columns.Count - 3, 2); //kodePkt
                sbc.ColumnMappings.Add(dt.Columns.Count - 2, 4); //Jenis Transaksi
                sbc.ColumnMappings.Add(dt.Columns.Count - 1, 3); //IN-OUT
                sbc.ColumnMappings.Add(0, 1); //Tanggal
                for (int a = 0; a < 15; a++)
                    sbc.ColumnMappings.Add(5 + a, 8 + a); //BN + Coin
                sbc.WriteToServer(dt);
                sbc.Close();
            }
        }
        private void readDeliveryRetail(DataSet ds)
        {
            DataTable dt = ds.Tables[4];

            dataGridView1.DataSource = dt;
            dt.Rows.RemoveAt(0);
            dt.Rows.Remove(dt.Rows[0]); dt.Rows.Remove(dt.Rows[dt.Rows.Count - 1]);
            DataRow[] rows = dt.Select("Column2 is NULL");
            foreach (var row in rows)
            {
                dt.Rows.Remove(row);
            }


            //Console.WriteLine(dt.Rows.Count);
            //Console.WriteLine(dt.Rows[0][4]);
            //Console.WriteLine(dt.Rows[dt.Rows.Count - 1][2].ToString());
            dt.Columns.Add("kodePkt", typeof(String));
            dt.Columns["kodePkt"].DefaultValue = kodePkt;
            for (int a = 0; a < dt.Rows.Count; a++)
            {
                dt.Rows[a]["kodePkt"] = kodePkt;
            }

            dt.Columns.Add("jenisTransaksi", typeof(String));
            for (int a = 0; a < dt.Rows.Count; a++)
            {
                dt.Rows[a]["jenisTransaksi"] = "Delivery Retail";
            }

            dt.Columns.Add("inout", typeof(String));
            for (int a = 0; a < dt.Rows.Count; a++)
            {
                dt.Rows[a]["inout"] = "OUT";
            }

            //Console.WriteLine(dt.Rows[dt.Rows.Count - 1][dt.Columns.Count - 1].ToString());
            using (SqlBulkCopy sbc = new SqlBulkCopy(Variables.connectionString))
            {
                sbc.DestinationTableName = "dbo.DailyStock";
                //Kode Nasabah
                sbc.ColumnMappings.Add(1, 5);
                //Nama Nasabah
                sbc.ColumnMappings.Add(2, 6);
                //Kode Pkt
                sbc.ColumnMappings.Add(dt.Columns.Count - 3, 2);
                //Jenis Transaksi
                sbc.ColumnMappings.Add(dt.Columns.Count - 2, 4);
                //IN - OUT
                sbc.ColumnMappings.Add(dt.Columns.Count - 1, 3);
                //Tanggal
                sbc.ColumnMappings.Add(0, 1);
                //Uang
                for (int a = 0; a < 15; a++)
                    sbc.ColumnMappings.Add(4 + a, 8 + a);
                sbc.WriteToServer(dt);
                sbc.Close();
            }
        }
        private void readDeliveryLainnya(DataSet ds)
        {
            DataTable dt = ds.Tables[5];
            dt.Rows.RemoveAt(0);
            dt.Rows.Remove(dt.Rows[0]); dt.Rows.Remove(dt.Rows[dt.Rows.Count - 1]);

            dataGridView1.DataSource = dt;

            DataRow[] rows = dt.Select("Column2 is NULL");
            foreach (var row in rows)
            {
                dt.Rows.Remove(row);
            }


            //Console.WriteLine(dt.Rows.Count);
            //Console.WriteLine(dt.Rows[0][2]);
            //Console.WriteLine(dt.Rows[dt.Rows.Count - 1][2].ToString());
            dt.Columns.Add("kodePkt", typeof(String));
            dt.Columns["kodePkt"].DefaultValue = kodePkt;
            for (int a = 0; a < dt.Rows.Count; a++)
            {
                dt.Rows[a]["kodePkt"] = kodePkt;
            }
            dt.Columns.Add("jenisTransaksi", typeof(String));
            for (int a = 0; a < dt.Rows.Count; a++)
            {
                dt.Rows[a]["jenisTransaksi"] = "Delivery Lainnya - " + dt.Rows[a][1].ToString();
            }

            dt.Columns.Add("inout", typeof(String));
            for (int a = 0; a < dt.Rows.Count; a++)
            {
                dt.Rows[a]["inout"] = "OUT";
            }

            //Console.WriteLine(dt.Rows[dt.Rows.Count - 1][dt.Columns.Count - 1].ToString());
            using (SqlBulkCopy sbc = new SqlBulkCopy(Variables.connectionString))
            {
                sbc.DestinationTableName = "dbo.DailyStock";
                //Kode Pkt
                sbc.ColumnMappings.Add(dt.Columns.Count - 3, 2);
                //Jenis Transaksi
                sbc.ColumnMappings.Add(dt.Columns.Count - 2, 4);
                //IN-OUT
                sbc.ColumnMappings.Add(dt.Columns.Count - 1, 3);
                //Tanggal
                sbc.ColumnMappings.Add(0, 1);
                //Keterangan Sumber Dana
                sbc.ColumnMappings.Add(1, 7);
                //Uang
                for (int a = 0; a < 15; a++)
                    sbc.ColumnMappings.Add(4 + a, 8 + a);
                sbc.WriteToServer(dt);
                sbc.Close();
            }
        }

        private void InputButton_Click(object sender, EventArgs e)
        {

        }
    }
}
