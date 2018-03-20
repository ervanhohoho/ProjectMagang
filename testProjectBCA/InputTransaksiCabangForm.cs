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
        String kodePkt = "";
        DataTable collectionLainnya;
        DataTable collectionCabang;
        DataTable collectionRetail;
        DataTable deliveryCabang;
        DataTable deliveryRetail;
        DataTable deliveryLainnya;
        public InputTransaksiCabangForm()
        {
            InitializeComponent();
        }

        private void SelectFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = Variables.excelFilter;
            if(of.ShowDialog() == DialogResult.OK)
            {
                String filename = of.FileName;
                kodePkt = of.FileName.ToString().Substring(filename.LastIndexOf(" ")+1, 4);
                //Console.WriteLine(kodePkt);
                DataSet ds = Util.openExcel(of.FileName);

                collectionCabang = ds.Tables[0];
                readCollectionCabang(ds);
                collectionRetail = ds.Tables[1];
                readCollectionRetail(ds);
                collectionLainnya = ds.Tables[2];
                readCollectionLainnya(ds);
                deliveryCabang = ds.Tables[3];
                readDeliveryCabang(ds);
                deliveryRetail = ds.Tables[5];
                readDeliveryRetail(ds);
                deliveryLainnya = ds.Tables[6];
                readDeliveryLainnya(ds);
            }
        }
        private void readCollectionCabang(DataSet ds)
        {
            DataTable dt = ds.Tables[0];
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
            //Console.WriteLine(dt.Rows[dt.Rows.Count - 1][dt.Columns.Count - 1].ToString());
            using (SqlBulkCopy sbc = new SqlBulkCopy(Variables.connectionString))
            {
                sbc.DestinationTableName = "dbo.CollectionCabang";
                sbc.ColumnMappings.Add(2, 3);
                sbc.ColumnMappings.Add(dt.Columns.Count - 1, 2);
                sbc.ColumnMappings.Add(0, 1);
                for (int a = 0; a < 15; a++)
                    sbc.ColumnMappings.Add(6 + a, 4 + a);
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
            //Console.WriteLine(dt.Rows[dt.Rows.Count - 1][dt.Columns.Count - 1].ToString());
            using (SqlBulkCopy sbc = new SqlBulkCopy(Variables.connectionString))
            {
                sbc.DestinationTableName = "dbo.CollectionRetail";
                //Kode Nasabah
                sbc.ColumnMappings.Add(1, 3);
                //Kode Pkt
                sbc.ColumnMappings.Add(dt.Columns.Count - 1, 2);
                //Tanggal
                sbc.ColumnMappings.Add(0, 1);
                //Uang
                for (int a = 0; a < 15; a++)
                    sbc.ColumnMappings.Add(4 + a, 4 + a);
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
            //Console.WriteLine(dt.Rows[dt.Rows.Count - 1][dt.Columns.Count - 1].ToString());
            using (SqlBulkCopy sbc = new SqlBulkCopy(Variables.connectionString))
            {
                sbc.DestinationTableName = "dbo.CollectionLainnya";
                //Kode Nasabah
                sbc.ColumnMappings.Add(1, 3);
                //Kode Pkt
                sbc.ColumnMappings.Add(dt.Columns.Count - 1, 2);
                //Tanggal
                sbc.ColumnMappings.Add(0, 1);
                //Uang
                for (int a = 0; a < 15; a++)
                    sbc.ColumnMappings.Add(4 + a, 4 + a);
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
            using (SqlBulkCopy sbc = new SqlBulkCopy(Variables.connectionString))
            {
                sbc.DestinationTableName = "dbo.DeliveryCabang";
                sbc.ColumnMappings.Add(2, 2);
                sbc.ColumnMappings.Add(dt.Columns.Count - 1, 3);
                sbc.ColumnMappings.Add(0, 1);
                sbc.ColumnMappings.Add(1, 4);
                for (int a = 0; a < 15; a++)
                    sbc.ColumnMappings.Add(5 + a, 5 + a);
                sbc.WriteToServer(dt);
                sbc.Close();
            }
        }
        private void readDeliveryRetail(DataSet ds)
        {
            DataTable dt = ds.Tables[5];

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
            //Console.WriteLine(dt.Rows[dt.Rows.Count - 1][dt.Columns.Count - 1].ToString());
            using (SqlBulkCopy sbc = new SqlBulkCopy(Variables.connectionString))
            {
                sbc.DestinationTableName = "dbo.DeliveryRetail";
                //Kode Nasabah
                sbc.ColumnMappings.Add(1, 3);
                //Kode Pkt
                sbc.ColumnMappings.Add(dt.Columns.Count - 1, 2);
                //Tanggal
                sbc.ColumnMappings.Add(0, 1);
                //Uang
                for (int a = 0; a < 15; a++)
                    sbc.ColumnMappings.Add(4 + a, 4 + a);
                sbc.WriteToServer(dt);
                sbc.Close();
            }
        }
        private void readDeliveryLainnya(DataSet ds)
        {
            DataTable dt = ds.Tables[6];
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
            //Console.WriteLine(dt.Rows[dt.Rows.Count - 1][dt.Columns.Count - 1].ToString());
            using (SqlBulkCopy sbc = new SqlBulkCopy(Variables.connectionString))
            {
                sbc.DestinationTableName = "dbo.DeliveryLainnya";
                //Kode Nasabah
                sbc.ColumnMappings.Add(1, 3);
                //Kode Pkt
                sbc.ColumnMappings.Add(dt.Columns.Count - 1, 2);
                //Tanggal
                sbc.ColumnMappings.Add(0, 1);
                //Uang
                for (int a = 0; a < 15; a++)
                    sbc.ColumnMappings.Add(4 + a, 4 + a);
                sbc.WriteToServer(dt);
                sbc.Close();
            }
        }
    }
}
