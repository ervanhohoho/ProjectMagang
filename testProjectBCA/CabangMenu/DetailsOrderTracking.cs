using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testProjectBCA.CabangMenu
{
    public partial class DetailsOrderTracking : Form
    {
        public DetailsOrderTracking(String kodePkt, DateTime tanggal)
        {
            InitializeComponent();
            Database1Entities db = new Database1Entities();
            Console.WriteLine("KodePkt: " + kodePkt + "Tanggal: " + tanggal);
            var query = (from x in db.OrderTrackings.AsEnumerable()
                         where x.kodePkt == kodePkt && ((DateTime)x.tanggal).Date == tanggal.Date
                         select new { x.kodePkt, x.kodeCabang, x.nominalDispute }).ToList();
            dataGridView1.DataSource = query;
            for(int a=0;a<dataGridView1.ColumnCount;a++)
            {
                if(dataGridView1.Columns[a].ValueType == typeof(Int64?))
                {
                    dataGridView1.Columns[a].DefaultCellStyle.Format = "C0";
                    dataGridView1.Columns[a].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
                }
            }
        }
    }
}
