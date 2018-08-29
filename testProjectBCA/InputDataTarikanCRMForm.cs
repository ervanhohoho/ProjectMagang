using FastMember;
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
    public partial class InputDataTarikanCRMForm : Form
    {
        public InputDataTarikanCRMForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = Variables.excelFilter;
            if (of.ShowDialog() == DialogResult.OK)
            {
                loadForm.ShowSplashScreen();
                DataSet ds = Util.openExcel(of.FileName);
                DataTable dt = ds.Tables[0];
                DataRow[] rowsToDelete = dt.Select("Column0 is NULL");
                foreach (var row in rowsToDelete)
                    dt.Rows.Remove(row);
                dt.Rows.RemoveAt(0);
                int COLINDEX_TANGGAL = 'A' - 'A',
                    COLINDEX_WSID = 'B' - 'A',
                    COLINDEX_NOMINALTARIKAN = 'E' - 'A';
                List<TabelTarikanSetoranCRM> dataExcel = new List<TabelTarikanSetoranCRM>();
                foreach (DataRow row in dt.Rows)
                {
                    dataExcel.Add(new TabelTarikanSetoranCRM()
                    {
                        tanggal = DateTime.Parse(row[COLINDEX_TANGGAL].ToString()),
                        wsid = row[COLINDEX_WSID].ToString(),
                        nominal = Int64.Parse(row[COLINDEX_NOMINALTARIKAN].ToString()),
                        jenis = "Tarikan"
                    });
                }
                using (SqlBulkCopy sbq = new SqlBulkCopy(Variables.connectionString))
                {
                    using (var reader = ObjectReader.Create(dataExcel, "tanggal", "wsid", "nominal", "jenis"))
                    {
                        sbq.DestinationTableName = "dbo.TabelTarikanSetoranCRM";
                        sbq.WriteToServer(reader);
                    }
                }
                loadForm.CloseForm();
            }
        }
    }
}
