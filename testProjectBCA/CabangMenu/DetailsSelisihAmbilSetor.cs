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
    public partial class DetailsSelisihAmbilSetor : Form
    {
        public DetailsSelisihAmbilSetor(String kodePkt, DateTime tanggal, bool txnPanjang)
        {
            InitializeComponent();
            Database1Entities db = new Database1Entities();
            if (!txnPanjang)
            {
                var query = (from x in db.RekapSelisihAmbilSetors.AsEnumerable()
                             where kodePkt == "CCAS" ? x.kodePenerimaDana.Contains(kodePkt) : x.kodePenerimaDana == kodePkt && ((DateTime)x.tanggalTransaksi).Date == tanggal.Date
                             && x.noTxn.Length == 6
                             select new {
                                 x.kodePenerimaDana,
                                 x.kodeSumberDana,
                                 x.lebih,
                                 x.kurang,
                                 x.palsu,
                                 x.mutilasi,
                                 x.total,
                                 x.noTxn,
                                 x.tanggalTransaksi,
                                 x.tanggalTemu,
                                 x.noBA,
                                 x.keterangan,
                                 x.transaksi,
                             }).ToList();
                dataGridView1.DataSource = query;
                
            }
            else
            {
                var query = (from x in db.RekapSelisihAmbilSetors.AsEnumerable()
                             where  ((DateTime)x.tanggalTransaksi).Date == tanggal.Date
                             && x.noTxn.Length > 6
                             select new{
                                 x.kodePenerimaDana,
                                 x.kodeSumberDana,
                                 x.lebih,
                                 x.kurang,
                                 x.palsu,
                                 x.mutilasi,
                                 x.total,
                                 x.noTxn,
                                 x.tanggalTransaksi,
                                 x.tanggalTemu,
                                 x.noBA,
                                 x.keterangan,
                                 x.transaksi,
                             }).ToList();
                dataGridView1.DataSource = query;
            }
            for(int a=1;a<dataGridView1.ColumnCount;a++)
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
