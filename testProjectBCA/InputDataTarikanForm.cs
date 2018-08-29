using FastMember;
using MoreLinq;
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
    public partial class InputDataTarikanForm : Form
    {
        public InputDataTarikanForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            if(of.ShowDialog() == DialogResult.OK)
            {
                Database1Entities db = new Database1Entities();
                loadForm.ShowSplashScreen();
                DataSet ds = Util.openExcel(of.FileName);
                DataTable dt = ds.Tables[0];
                DataRow[] rowsToDelete = dt.Select("Column0 is NULL");
                foreach (var row in rowsToDelete)
                    dt.Rows.Remove(row);
                dt.Rows.RemoveAt(0);
                int COLINDEX_TANGGAL = 'A'-'A',
                    COLINDEX_WSID = 'B'-'A',
                    COLINDEX_TOTALTARIKAN = 'C'-'A';
                List<TabelTarikan> dataExcel = new List<TabelTarikan>();
                foreach (DataRow row in dt.Rows)
                {
                    dataExcel.Add(new TabelTarikan()
                    {
                        tanggal = DateTime.Parse(row[COLINDEX_TANGGAL].ToString()),
                        wsid = row[COLINDEX_WSID].ToString(),
                        totalTarikan = Int64.Parse(row[COLINDEX_TOTALTARIKAN].ToString())
                    });
                }

                
                Console.WriteLine("Masuk ke db");
                
                //var toUpdate = (from x in db.TabelTarikans.AsEnumerable()
                //                join y in dataExcel on new { x.tanggal.Date, x.wsid } equals new { y.tanggal.Date, y.wsid }
                //                select new TabelTarikan() {
                //                    wsid = x.wsid,
                //                    tanggal = x.tanggal,
                //                    totalTarikan = y.totalTarikan
                //                }).ToList();
                //var datasToDelete = new List<TabelTarikan>(toUpdate);
                    
                //foreach(var row in toUpdate)
                //{
                //    Console.WriteLine("Update");
                //    var temp = db.TabelTarikans.AsEnumerable().Where(x => x.tanggal.Date == row.tanggal.Date && x.wsid == row.wsid).FirstOrDefault();
                //    if(temp!= null)
                //    {
                //        temp.totalTarikan = row.totalTarikan;
                //        db.SaveChanges();
                //    }
                //}
                //foreach (var row in datasToDelete)
                //{
                //    Console.WriteLine("Delete");
                //    dataExcel.Remove(row);
                //}

                using (SqlBulkCopy sbq = new SqlBulkCopy(Variables.connectionString))
                {
                    using (var reader = ObjectReader.Create(dataExcel, "tanggal", "wsid", "totalTarikan"))
                    {
                        sbq.DestinationTableName = "dbo.TabelTarikan";
                        sbq.WriteToServer(reader);
                    }
                }


            }
            loadForm.CloseForm();
            
        }
    }
}
