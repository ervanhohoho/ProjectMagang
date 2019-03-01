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
    public partial class OrderTrackingSelisihPopUpForm : Form
    {
        public OrderTrackingSelisihPopUpForm(String kodePkt, DateTime tanggal)
        {
            InitializeComponent();
            Database1Entities db = new Database1Entities();
            Console.WriteLine("KodePkt: " + kodePkt + "Tanggal: " + tanggal);
            var dataOrderTracking = (from x in db.OrderTrackings.AsEnumerable()
                         where kodePkt == "CCAS" ? x.kodePkt.Contains(kodePkt) : x.kodePkt == kodePkt && ((DateTime)x.tanggal).Date == tanggal.Date
                         group x by new {x.kodePkt, x.kodeCabang, reference = x.reference_master.Split('B')[0].Substring(x.reference_master.Split('B')[0].Length - 6, 6)} into x
                         select new { x.Key.kodePkt, x.Key.kodeCabang, nominalDispute = x.Sum(y=>y.nominalDispute), x.Key.reference}).ToList();
            var dataRekapSelisih = (from x in db.RekapSelisihAmbilSetors.AsEnumerable()
                                    where kodePkt == "CCAS" ? x.kodePenerimaDana.Contains(kodePkt) : x.kodePenerimaDana == kodePkt && ((DateTime)x.tanggalTransaksi).Date == tanggal.Date
                                    && x.noTxn.Length == 6
                                    group x by new { x.kodePenerimaDana, x.kodeSumberDana, x.noTxn } into x
                                    select new
                                    {
                                        x.Key.kodePenerimaDana,
                                        x.Key.kodeSumberDana,
                                        lebih = x.Sum(y => y.lebih),
                                        kurang = x.Sum(y => y.kurang),
                                        palsu = x.Sum(y => y.palsu),
                                        mutilasi = x.Sum(y => y.mutilasi),
                                        total = x.Sum(y => y.total),
                                        x.Key.noTxn,
                                    }).ToList();
            var query = (from x in dataOrderTracking
                         join y in dataRekapSelisih on x.reference equals y.noTxn into temp
                         from y in temp.DefaultIfEmpty()
                         select new
                         {
                             vendor=x.kodePkt,
                             cabang= x.kodeCabang.TrimStart('B'),
                             noTxn= x.reference,
                             nominalOrderTracking= x.nominalDispute,
                             lebih = y == null ? 0 : y.lebih,
                             kurang = y == null ? 0 : y.kurang,
                             palsu = y == null ? 0 : y.palsu,
                             total = y == null ? 0 : y.total,
                             mutilasi = y == null ? 0 : y.mutilasi,
                         }).GroupBy(x=>new { x.vendor, x.cabang, x.noTxn }).Select(x=>new
                         {
                             x.Key.vendor,
                             x.Key.cabang,
                             x.Key.noTxn,
                             nominalOrderTracking = x.Sum(y=>y.nominalOrderTracking),
                             total = x.Sum(y=>y.total),
                             lebih = x.Sum(y=>y.lebih),
                             kurang = x.Sum(y=>y.kurang),
                             palsu = x.Sum(y=>y.palsu),
                             mutilasi = x.Sum(y=>y.mutilasi)
                         }).ToList();

            query.Concat((from x in dataRekapSelisih
                            join y in dataOrderTracking on x.noTxn equals y.reference into temp
                            from y in temp.DefaultIfEmpty()
                            select new
                            {
                                vendor = x.kodePenerimaDana,
                                cabang = x.kodeSumberDana.TrimStart('B'),
                                noTxn = x.noTxn,
                                nominalOrderTracking = y == null ? 0 : y.nominalDispute,
                                total = x.total,
                                lebih = x.lebih,
                                kurang = x.kurang,
                                palsu = x.palsu,
                                mutilasi = x.mutilasi,
                            }).GroupBy(x => new { x.vendor, x.cabang, x.noTxn }).Select(x => new
                            {
                                x.Key.vendor,
                                x.Key.cabang,
                                x.Key.noTxn,
                                nominalOrderTracking = x.Sum(y => y.nominalOrderTracking),
                                total = x.Sum(y => y.total),
                                lebih = x.Sum(y => y.lebih),
                                kurang = x.Sum(y => y.kurang),
                                palsu = x.Sum(y => y.palsu),
                                mutilasi = x.Sum(y => y.mutilasi)
                            }).ToList());
            dataGridView1.DataSource = query.Distinct().ToList();
            for(int a=0;a< dataGridView1.Columns.Count;a++)
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
