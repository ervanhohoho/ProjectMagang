using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testProjectBCA
{
    public partial class rekonSaldoTrxCabang : Form
    {
        Database1Entities en = new Database1Entities();
        List<ViewRekonSaldoTrxCabang> viewRekonSaldoTrxCabangs = new List<ViewRekonSaldoTrxCabang>();
        public rekonSaldoTrxCabang()
        {
            InitializeComponent();
            reloadComboPkt();
            reloadComboCabang();
            //dateTimePicker1.Visible = false;
            buttonSearch.Visible = false;
            textBoxSearch.Visible = false;
            comboSetBon.SelectedIndex = 0;
            comboVal.SelectedIndex = 0;
            reloadBonBelum();
        }


        public void reloadComboPkt()
        {
            List<String> vendor = new List<string>();
            vendor.Add("ALL VENDOR");

            var query = (from x in en.RekonSaldoPerVendors
                         select x.vendor.Substring(0, 4) == "CCAS" ? "CCAS" : x.vendor).Distinct().ToList();

            foreach (var item in query)
            {
                vendor.Add(item.ToString());
            }


            comboPkt.DataSource = vendor;
        }

        public void reloadComboCabang()
        {
            List<String> cabang = new List<String>();
            cabang.Add("ALL CABANG");
            var query = (from x in en.Cabangs
                         where ((x.kodePkt != null) && (comboPkt.SelectedValue.ToString() == "ALL VENDOR" ? true : x.kodePkt == comboPkt.SelectedValue.ToString()))
                         select x.kodeCabang).Distinct().ToList();

            foreach (var item in query)
            {
                if (!String.IsNullOrEmpty(item))
                {
                    cabang.Add("B" + item.ToString());
                }
            }

            comboCabang.DataSource = cabang;

        }

        public List<ViewRekonSaldoTrxCabang> reloadSetoranBelum()
        {
            Console.WriteLine("ini dia: " + comboPkt.SelectedValue.ToString());
            DateTime date2 = dateTimePicker2.Value.Date;
            var prequery = (
              from x in en.RekonSaldoPerVendors.AsEnumerable()
              join y in en.Pkts on x.vendor.Substring(0, 4) equals y.kodePktCabang == "CCASA" ? "CCAS" : y.kodePktCabang
              join z in en.Cabangs on x.cashPointtId.TrimStart('B') equals z.kodeCabang
              where ((comboCabang.SelectedValue.ToString() == "ALL CABANG" ? true : (z.kodeCabang == comboCabang.SelectedValue.ToString().TrimStart('B'))) && ((comboPkt.SelectedValue.ToString() == "ALL VENDOR" ? true : (comboPkt.SelectedValue.ToString() == "CCAS" ? x.vendor.Contains(comboPkt.SelectedValue.ToString()) : x.vendor == comboPkt.SelectedValue.ToString()))))
              select x
            ).ToList();

            var emergency = (
                from x in prequery
                where (x.actionRekon.Contains("Return") && x.statusRekon.Equals("In Transit")) && (DateTime.Parse(x.dueDate.ToString()).Date == dateTimePicker2.Value.Date || DateTime.Parse(x.realDate.ToString()) == dateTimePicker2.Value.Date)
                && x.actionRekon.ToLower().Contains("emergency")
                group x by new { x.dueDate, x.cashPointtId, x.blogTime, x.actionRekon, x.statusRekon, x.blogMessage, x.vendor } into z
                select new
                {
                    vendor = z.Key.vendor,
                    blogTime = z.Key.blogTime,
                    actionRekon = z.Key.actionRekon,
                    statusRekon = z.Key.statusRekon,
                    blogMessage = z.Key.blogMessage,
                    dueDate = z.Key.dueDate,
                    cashPointtId = z.Key.cashPointtId,
                    emergency = z.Sum(x => x.currencyAmmount),
                    //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                    validation = z.Key.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(z.Key.blogTime.ToString()).Hour < 21 ? z.Key.blogTime : DateTime.Parse(z.Key.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                }).ToList();
            var regular = (
                from x in prequery
                where (x.actionRekon.Contains("Return") && x.statusRekon.Equals("In Transit")) && (DateTime.Parse(x.dueDate.ToString()).Date == dateTimePicker2.Value.Date || DateTime.Parse(x.realDate.ToString()) == dateTimePicker2.Value.Date)
                && !x.actionRekon.ToLower().Contains("emergency")
                group x by new { x.dueDate, x.cashPointtId, x.blogTime, x.actionRekon, x.statusRekon, x.blogMessage, x.vendor } into z
                select new
                {
                    vendor = z.Key.vendor,
                    blogTime = z.Key.blogTime,
                    actionRekon = z.Key.actionRekon,
                    statusRekon = z.Key.statusRekon,
                    blogMessage = z.Key.blogMessage,
                    dueDate = z.Key.dueDate,
                    cashPointtId = z.Key.cashPointtId,
                    regular = z.Sum(x => x.currencyAmmount),
                    //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                    validation = z.Key.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(z.Key.blogTime.ToString()).Hour < 21 ? z.Key.blogTime : DateTime.Parse(z.Key.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                }).ToList();

            var prepareL = (from x in regular
                            join y in emergency on new { x.blogTime, x.dueDate, x.cashPointtId } equals new { y.blogTime, y.dueDate, y.cashPointtId } into temp
                            from y in temp.DefaultIfEmpty()
                            select new
                            {
                                vendor = x.vendor,
                                blogTime = x.blogTime,
                                actionRekon = x.actionRekon,
                                statusRekon = x.statusRekon,
                                blogMessage = x.blogMessage,
                                dueDate = x.dueDate,
                                cashPointtId = x.cashPointtId,
                                regular = x.regular,
                                emergency = y == null ? 0 : y.emergency,
                                //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                                validation = x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                            }).ToList();
            var prepareR = (from x in emergency
                            join y in regular on new { x.blogTime, x.dueDate, x.cashPointtId } equals new { y.blogTime, y.dueDate, y.cashPointtId } into temp
                            from y in temp.DefaultIfEmpty()
                            select new
                            {
                                vendor = x.vendor,
                                blogTime = x.blogTime,
                                actionRekon = x.actionRekon,
                                statusRekon = x.statusRekon,
                                blogMessage = x.blogMessage,
                                dueDate = x.dueDate,
                                cashPointtId = x.cashPointtId,
                                regular = y == null ? 0 : y.regular,
                                emergency = x.emergency,
                                //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                                validation = x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                            }).ToList();
            var prepare = prepareL.Union(prepareR);

            var query = (from x in prepare
                         join y in en.Cabangs on x.cashPointtId.TrimStart('B') equals y.kodeCabang
                         where x.actionRekon.Contains("Return") && x.validation.Equals("NOT VALIDATED") && x.statusRekon.Equals("In Transit")
                         group x by new { x.dueDate, x.cashPointtId, x.blogTime, x.vendor, y.namaCabang } into z
                         select new { cabang = z.Key.namaCabang, vendor = z.Key.vendor, dueDate = z.Key.dueDate, blogTime = z.Key.blogTime, cashPointId = z.Key.cashPointtId, regular = z.Sum(x => x.regular), emergency = z.Sum(x => x.emergency), total = z.Sum(x => x.emergency + x.regular) }).ToList();

            viewRekonSaldoTrxCabangs = query.Select(x => new ViewRekonSaldoTrxCabang()
            {
                vendor = x.vendor,
                dueDate = x.dueDate,
                emergency = x.emergency,
                blogTime = x.blogTime,
                cashpointId = x.cashPointId,
                cabang = x.cabang,
                total = x.total,
                regular = x.regular,
                SetoranBon = "SETORAN",
                Validasi = "BELUM VALIDASI"
            }).ToList();
            dataGridView1.DataSource = viewRekonSaldoTrxCabangs;
            formatting();
            return viewRekonSaldoTrxCabangs;

        }
        public List<ViewRekonSaldoTrxCabang> reloadSetoranSudah()
        {
            Console.WriteLine("ini dia: " + comboPkt.SelectedValue.ToString());
            DateTime date2 = dateTimePicker2.Value.Date;
            var prequery = (
              from x in en.RekonSaldoPerVendors.AsEnumerable()
              join y in en.Pkts on x.vendor.Substring(0, 4) equals y.kodePktCabang == "CCASA" ? "CCAS" : y.kodePktCabang
              join z in en.Cabangs on x.cashPointtId.TrimStart('B') equals z.kodeCabang
              where ((comboCabang.SelectedValue.ToString() == "ALL CABANG" ? true : (z.kodeCabang == comboCabang.SelectedValue.ToString().TrimStart('B'))) && (comboPkt.SelectedValue.ToString() == "ALL VENDOR" ? true : (comboPkt.SelectedValue.ToString() == "CCAS" ? x.vendor.Contains(comboPkt.SelectedValue.ToString()) : x.vendor == comboPkt.SelectedValue.ToString())))
              select x
            ).ToList();
            if (prequery == null)
            {
                Console.WriteLine("ini kosong");
            }
            var emergency = (
                from x in prequery
                where (x.actionRekon.Contains("Return") && x.statusRekon.Equals("In Transit")) && (DateTime.Parse(x.dueDate.ToString()).Date == dateTimePicker2.Value.Date || DateTime.Parse(x.realDate.ToString()) == dateTimePicker2.Value.Date)
                && x.actionRekon.ToLower().Contains("emergency")
                group x by new { x.vendor, x.dueDate, x.cashPointtId, x.blogTime, x.actionRekon, x.statusRekon, x.blogMessage } into z
                select new
                {
                    vendor = z.Key.vendor,
                    blogTime = z.Key.blogTime,
                    actionRekon = z.Key.actionRekon,
                    statusRekon = z.Key.statusRekon,
                    blogMessage = z.Key.blogMessage,
                    dueDate = z.Key.dueDate,
                    cashPointtId = z.Key.cashPointtId,
                    emergency = z.Sum(x => x.currencyAmmount),
                    //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                    validation = z.Key.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(z.Key.blogTime.ToString()).Hour < 21 ? z.Key.blogTime : DateTime.Parse(z.Key.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                }).ToList();
            var regular = (
                from x in prequery
                where (x.actionRekon.Contains("Return") && x.statusRekon.Equals("In Transit")) && (DateTime.Parse(x.dueDate.ToString()).Date == dateTimePicker2.Value.Date || DateTime.Parse(x.realDate.ToString()) == dateTimePicker2.Value.Date)
                && !x.actionRekon.ToLower().Contains("emergency")
                group x by new { x.vendor, x.dueDate, x.cashPointtId, x.blogTime, x.actionRekon, x.statusRekon, x.blogMessage } into z
                select new
                {
                    vendor = z.Key.vendor,
                    blogTime = z.Key.blogTime,
                    actionRekon = z.Key.actionRekon,
                    statusRekon = z.Key.statusRekon,
                    blogMessage = z.Key.blogMessage,
                    dueDate = z.Key.dueDate,
                    cashPointtId = z.Key.cashPointtId,
                    regular = z.Sum(x => x.currencyAmmount),
                    //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                    validation = z.Key.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(z.Key.blogTime.ToString()).Hour < 21 ? z.Key.blogTime : DateTime.Parse(z.Key.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                }).ToList();

            var prepareL = (from x in regular
                            join y in emergency on new { x.blogTime, x.dueDate, x.cashPointtId } equals new { y.blogTime, y.dueDate, y.cashPointtId } into temp
                            from y in temp.DefaultIfEmpty()
                            select new
                            {
                                vendor = x.vendor,
                                blogTime = x.blogTime,
                                actionRekon = x.actionRekon,
                                statusRekon = x.statusRekon,
                                blogMessage = x.blogMessage,
                                dueDate = x.dueDate,
                                cashPointtId = x.cashPointtId,
                                regular = x.regular,
                                emergency = y == null ? 0 : y.emergency,
                                //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                                validation = x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                            }).ToList();
            var prepareR = (from x in emergency
                            join y in regular on new { x.blogTime, x.dueDate, x.cashPointtId } equals new { y.blogTime, y.dueDate, y.cashPointtId } into temp
                            from y in temp.DefaultIfEmpty()
                            select new
                            {
                                vendor = x.vendor,
                                blogTime = x.blogTime,
                                actionRekon = x.actionRekon,
                                statusRekon = x.statusRekon,
                                blogMessage = x.blogMessage,
                                dueDate = x.dueDate,
                                cashPointtId = x.cashPointtId,
                                regular = y == null ? 0 : y.regular,
                                emergency = x.emergency,
                                //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                                validation = x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                            }).ToList();
            var prepare = prepareL.Union(prepareR);

            var query = (from x in prepare
                         join y in en.Cabangs on x.cashPointtId.TrimStart('B') equals y.kodeCabang
                         where x.actionRekon.Contains("Return") && x.validation.Equals("VALIDATED") && x.statusRekon.Equals("In Transit")
                         group x by new { x.vendor, x.dueDate, x.cashPointtId, x.blogTime, y.namaCabang } into z
                         select new { cabang = z.Key.namaCabang, vendor = z.Key.vendor, dueDate = z.Key.dueDate, blogTime = z.Key.blogTime, cashPointId = z.Key.cashPointtId, regular = z.Sum(x => x.regular), emergency = z.Sum(x => x.emergency), total = z.Sum(x => x.emergency + x.regular) }).ToList();


            viewRekonSaldoTrxCabangs = query.Select(x => new ViewRekonSaldoTrxCabang()
            {
                vendor = x.vendor,
                dueDate = x.dueDate,
                emergency = x.emergency,
                blogTime = x.blogTime,
                cashpointId = x.cashPointId,
                cabang = x.cabang,
                total = x.total,
                regular = x.regular,
                SetoranBon = "SETORAN",
                Validasi = "SUDAH VALIDASI"
            }).ToList();
            dataGridView1.DataSource = viewRekonSaldoTrxCabangs;
            formatting();
            return viewRekonSaldoTrxCabangs;

        }
        public List<ViewRekonSaldoTrxCabang> reloadBonSudah()
        {
            Console.WriteLine("ini dia: " + comboPkt.SelectedValue.ToString());
            DateTime date2 = dateTimePicker2.Value.Date;
            var prequery = (
              from x in en.RekonSaldoPerVendors.AsEnumerable()
              join y in en.Pkts on x.vendor.Substring(0, 4) equals y.kodePktCabang == "CCASA" ? "CCAS" : y.kodePktCabang
              join z in en.Cabangs on x.cashPointtId.TrimStart('B') equals z.kodeCabang
              where ((comboCabang.SelectedValue.ToString() == "ALL CABANG" ? true : (z.kodeCabang == comboCabang.SelectedValue.ToString().TrimStart('B'))) && (comboPkt.SelectedValue.ToString() == "ALL VENDOR" ? true : (comboPkt.SelectedValue.ToString() == "CCAS" ? x.vendor.Contains(comboPkt.SelectedValue.ToString()) : x.vendor == comboPkt.SelectedValue.ToString())))
              select x
            ).ToList();
            if (prequery == null)
            {
                Console.WriteLine("ini kosong");
            }
            var emergency = (
                from x in prequery
                where (x.actionRekon.Contains("Delivery") && x.statusRekon.Equals("Confirmed")) && (DateTime.Parse(x.dueDate.ToString()).Date == dateTimePicker2.Value.Date || DateTime.Parse(x.realDate.ToString()) == dateTimePicker2.Value.Date)
                && x.actionRekon.ToLower().Contains("emergency")
                group x by new { x.vendor, x.dueDate, x.cashPointtId, x.blogTime, x.actionRekon, x.statusRekon, x.blogMessage } into z
                select new
                {
                    vendor = z.Key.vendor,
                    blogTime = z.Key.blogTime,
                    actionRekon = z.Key.actionRekon,
                    statusRekon = z.Key.statusRekon,
                    blogMessage = z.Key.blogMessage,
                    dueDate = z.Key.dueDate,
                    cashPointtId = z.Key.cashPointtId,
                    emergency = z.Sum(x => x.currencyAmmount),
                    //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                    validation = z.Key.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(z.Key.blogTime.ToString()).Hour < 21 ? z.Key.blogTime : DateTime.Parse(z.Key.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                }).ToList();
            var regular = (
                from x in prequery
                where (x.actionRekon.Contains("Delivery") && x.statusRekon.Equals("Confirmed")) && (DateTime.Parse(x.dueDate.ToString()).Date == dateTimePicker2.Value.Date || DateTime.Parse(x.realDate.ToString()) == dateTimePicker2.Value.Date)
                && !x.actionRekon.ToLower().Contains("emergency")
                group x by new { x.vendor, x.dueDate, x.cashPointtId, x.blogTime, x.actionRekon, x.statusRekon, x.blogMessage } into z
                select new
                {
                    vendor = z.Key.vendor,
                    blogTime = z.Key.blogTime,
                    actionRekon = z.Key.actionRekon,
                    statusRekon = z.Key.statusRekon,
                    blogMessage = z.Key.blogMessage,
                    dueDate = z.Key.dueDate,
                    cashPointtId = z.Key.cashPointtId,
                    regular = z.Sum(x => x.currencyAmmount),
                    //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                    validation = z.Key.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(z.Key.blogTime.ToString()).Hour < 21 ? z.Key.blogTime : DateTime.Parse(z.Key.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                }).ToList();

            var prepareL = (from x in regular
                            join y in emergency on new { x.blogTime, x.dueDate, x.cashPointtId } equals new { y.blogTime, y.dueDate, y.cashPointtId } into temp
                            from y in temp.DefaultIfEmpty()
                            select new
                            {
                                vendor = x.vendor,
                                blogTime = x.blogTime,
                                actionRekon = x.actionRekon,
                                statusRekon = x.statusRekon,
                                blogMessage = x.blogMessage,
                                dueDate = x.dueDate,
                                cashPointtId = x.cashPointtId,
                                regular = x.regular,
                                emergency = y == null ? 0 : y.emergency,
                                //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                                validation = x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                            }).ToList();
            var prepareR = (from x in emergency
                            join y in regular on new { x.blogTime, x.dueDate, x.cashPointtId } equals new { y.blogTime, y.dueDate, y.cashPointtId } into temp
                            from y in temp.DefaultIfEmpty()
                            select new
                            {
                                vendor = x.vendor,
                                blogTime = x.blogTime,
                                actionRekon = x.actionRekon,
                                statusRekon = x.statusRekon,
                                blogMessage = x.blogMessage,
                                dueDate = x.dueDate,
                                cashPointtId = x.cashPointtId,
                                regular = y == null ? 0 : y.regular,
                                emergency = x.emergency,
                                //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                                validation = x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                            }).ToList();
            var prepare = prepareL.Union(prepareR);

            var query = (from x in prepare
                         join y in en.Cabangs on x.cashPointtId.TrimStart('B') equals y.kodeCabang
                         where x.actionRekon.Contains("Delivery") && x.validation.Equals("VALIDATED") && x.statusRekon.Equals("Confirmed")
                         group x by new { x.vendor, x.dueDate, x.cashPointtId, x.blogTime, y.namaCabang } into z
                         select new { cabang = z.Key.namaCabang, vendor = z.Key.vendor, dueDate = z.Key.dueDate, blogTime = z.Key.blogTime, cashPointId = z.Key.cashPointtId, regular = z.Sum(x => x.regular), emergency = z.Sum(x => x.emergency), total = z.Sum(x => x.emergency + x.regular) }).ToList();


            viewRekonSaldoTrxCabangs = query.Select(x => new ViewRekonSaldoTrxCabang()
            {
                vendor = x.vendor,
                dueDate = x.dueDate,
                emergency = x.emergency,
                blogTime = x.blogTime,
                cabang = x.cabang,
                cashpointId = x.cashPointId,
                total = x.total,
                regular = x.regular,
                SetoranBon = "BON",
                Validasi = "SUDAH VALIDASI"
            }).ToList();
            dataGridView1.DataSource = viewRekonSaldoTrxCabangs;
            formatting();
            return viewRekonSaldoTrxCabangs;

        }
        public List<ViewRekonSaldoTrxCabang> reloadBonBelum()
        {
            Console.WriteLine("ini dia: " + comboPkt.SelectedValue.ToString());
            DateTime date2 = dateTimePicker2.Value.Date;
            var prequery = (
              from x in en.RekonSaldoPerVendors.AsEnumerable()
              join y in en.Pkts on x.vendor.Substring(0, 4) equals y.kodePktCabang == "CCASA" ? "CCAS" : y.kodePktCabang
              join z in en.Cabangs on x.cashPointtId.TrimStart('B') equals z.kodeCabang
              where ((comboCabang.SelectedValue.ToString() == "ALL CABANG" ? true : (z.kodeCabang == comboCabang.SelectedValue.ToString().TrimStart('B'))) && (comboPkt.SelectedValue.ToString() == "ALL VENDOR" ? true : (comboPkt.SelectedValue.ToString() == "CCAS" ? x.vendor.Contains(comboPkt.SelectedValue.ToString()) : x.vendor == comboPkt.SelectedValue.ToString())))
              select x
            ).ToList();
            if (prequery == null)
            {
                Console.WriteLine("ini kosong");
            }
            var emergency = (
              from x in prequery
              where (x.actionRekon.Contains("Delivery") && x.statusRekon.Equals("Confirmed")) && (DateTime.Parse(x.dueDate.ToString()).Date == dateTimePicker2.Value.Date || DateTime.Parse(x.realDate.ToString()) == dateTimePicker2.Value.Date)
              && x.actionRekon.ToLower().Contains("emergency")
              group x by new { x.vendor, x.dueDate, x.cashPointtId, x.blogTime, x.actionRekon, x.statusRekon, x.blogMessage } into z
              select new
              {
                  vendor = z.Key.vendor,
                  blogTime = z.Key.blogTime,
                  actionRekon = z.Key.actionRekon,
                  statusRekon = z.Key.statusRekon,
                  blogMessage = z.Key.blogMessage,
                  dueDate = z.Key.dueDate,
                  cashPointtId = z.Key.cashPointtId,
                  emergency = z.Sum(x => x.currencyAmmount),
                  //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                  validation = z.Key.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(z.Key.blogTime.ToString()).Hour < 21 ? z.Key.blogTime : DateTime.Parse(z.Key.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
              }).ToList();
            var regular = (
                from x in prequery
                where (x.actionRekon.Contains("Delivery") && x.statusRekon.Equals("Confirmed")) && (DateTime.Parse(x.dueDate.ToString()).Date == dateTimePicker2.Value.Date || DateTime.Parse(x.realDate.ToString()) == dateTimePicker2.Value.Date)
                && !x.actionRekon.ToLower().Contains("emergency")
                group x by new { x.vendor, x.dueDate, x.cashPointtId, x.blogTime, x.actionRekon, x.statusRekon, x.blogMessage } into z
                select new
                {
                    vendor = z.Key.vendor,
                    blogTime = z.Key.blogTime,
                    actionRekon = z.Key.actionRekon,
                    statusRekon = z.Key.statusRekon,
                    blogMessage = z.Key.blogMessage,
                    dueDate = z.Key.dueDate,
                    cashPointtId = z.Key.cashPointtId,
                    regular = z.Sum(x => x.currencyAmmount),
                    //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                    validation = z.Key.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(z.Key.blogTime.ToString()).Hour < 21 ? z.Key.blogTime : DateTime.Parse(z.Key.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                }).ToList();

            var prepareL = (from x in regular
                            join y in emergency on new { x.blogTime, x.dueDate, x.cashPointtId } equals new { y.blogTime, y.dueDate, y.cashPointtId } into temp
                            from y in temp.DefaultIfEmpty()
                            select new
                            {
                                vendor = x.vendor,
                                blogTime = x.blogTime,
                                actionRekon = x.actionRekon,
                                statusRekon = x.statusRekon,
                                blogMessage = x.blogMessage,
                                dueDate = x.dueDate,
                                cashPointtId = x.cashPointtId,
                                regular = x.regular,
                                emergency = y == null ? 0 : y.emergency,
                                //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                                validation = x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                            }).ToList();
            var prepareR = (from x in emergency
                            join y in regular on new { x.blogTime, x.dueDate, x.cashPointtId } equals new { y.blogTime, y.dueDate, y.cashPointtId } into temp
                            from y in temp.DefaultIfEmpty()
                            select new
                            {
                                vendor = x.vendor,
                                blogTime = x.blogTime,
                                actionRekon = x.actionRekon,
                                statusRekon = x.statusRekon,
                                blogMessage = x.blogMessage,
                                dueDate = x.dueDate,
                                cashPointtId = x.cashPointtId,
                                regular = y == null ? 0 : y.regular,
                                emergency = x.emergency,
                                //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                                validation = x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                            }).ToList();
            var prepare = prepareL.Union(prepareR);


            var query = (from x in prepare
                         join y in en.Cabangs on x.cashPointtId.TrimStart('B') equals y.kodeCabang
                         where x.actionRekon.Contains("Delivery") && x.validation.Equals("NOT VALIDATED") && x.statusRekon.Equals("Confirmed")
                         group x by new { y.namaCabang, x.vendor, x.dueDate, x.cashPointtId, x.blogTime } into z
                         select new { cabang = z.Key.namaCabang, z.Key.vendor, dueDate = z.Key.dueDate, blogTime = z.Key.blogTime, cashPointId = z.Key.cashPointtId, regular = z.Sum(x => x.regular), emergency = z.Sum(x => x.emergency), total = z.Sum(x => x.emergency + x.regular) }).ToList();

            viewRekonSaldoTrxCabangs = query.Select(x => new ViewRekonSaldoTrxCabang()
            {
                vendor = x.vendor,
                dueDate = x.dueDate,
                emergency = x.emergency,
                blogTime = x.blogTime,
                cabang = x.cabang,
                cashpointId = x.cashPointId,
                total = x.total,
                regular = x.regular,
                SetoranBon = "BON",
                Validasi = "BELUM VALIDASI"

            }).ToList();
            dataGridView1.DataSource = viewRekonSaldoTrxCabangs;
            formatting();
            return viewRekonSaldoTrxCabangs;


        }

        public void reloadSetBonSudah()
        {
            List<ViewRekonSaldoTrxCabang> reloadSetBonSudah = reloadSetoranSudah();
            reloadSetBonSudah.AddRange(reloadBonSudah());
            dataGridView1.DataSource = reloadSetBonSudah;
            formatting();
        }

        public void reloadSetBonBelum()
        {
            List<ViewRekonSaldoTrxCabang> reloadSetBonBelum = reloadSetoranBelum();
            reloadSetBonBelum.AddRange(reloadBonBelum());
            dataGridView1.DataSource = reloadSetBonBelum;
            formatting();
        }

        public void reloadSudBelBon()
        {
            List<ViewRekonSaldoTrxCabang> reloadSudBelBon = reloadBonBelum();
            reloadSudBelBon.AddRange(reloadBonSudah());
            dataGridView1.DataSource = reloadSudBelBon;
            formatting();
        }

        public void reloadSudBelSetoran()
        {
            List<ViewRekonSaldoTrxCabang> reloadSudBelSetoran = reloadSetoranSudah();
            reloadSudBelSetoran.AddRange(reloadSetoranBelum());
            dataGridView1.DataSource = reloadSudBelSetoran;
            formatting();
        }

        public void reloadAll()
        {
            List<ViewRekonSaldoTrxCabang> reloadSemua = reloadSetoranBelum();
            reloadSemua.AddRange(reloadSetoranSudah());
            reloadSemua.AddRange(reloadBonBelum());
            reloadSemua.AddRange(reloadBonSudah());
            dataGridView1.DataSource = reloadSemua;
            formatting();
        }


        class loadData
        {
            public DateTime tanggal { set; get; }
            public String kodeCabang { set; get; }
            public String nominal { set; get; }
        }

        public void formatting()
        {
            //if (dataGridView1.Rows.Count > 0)
            //{ 
            //        dataGridView1.Columns[2].DefaultCellStyle.Format = "c";
            //        dataGridView1.Columns[2].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("ID-id");
            //}
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            DataGridViewSelectedCellCollection cells = dataGridView1.SelectedCells;

            foreach (DataGridViewCell cell in cells)
            {
                int rowidx = cell.RowIndex;
                int colidx = cell.ColumnIndex;
                Console.WriteLine(rowidx + ", " + colidx);
                Console.WriteLine(dataGridView1.Rows[rowidx].Cells[colidx].Value.ToString().Replace("Rp.", "").Replace(".", ""));
                if (colidx >= 4)
                {
                    if (dataGridView1.Columns[colidx].ValueType == typeof(Int64?))
                    {
                        dataGridView1.Rows[rowidx].Cells[colidx].Style.Format = "F0";
                    }
                }
            }
            for (int a = 0; a < dataGridView1.Rows.Count; a++)
            {
                for (int b = 4; b < dataGridView1.Columns.Count; b++)
                {
                    if (!cells.Contains(dataGridView1.Rows[a].Cells[b]))
                    {
                        if (dataGridView1.Columns[b].ValueType == typeof(Int64?))
                        {
                            dataGridView1.Rows[a].Cells[b].Style.Format = "C0";
                            dataGridView1.Rows[a].Cells[b].Style.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
                        }
                    }
                }
            }
        }

        private void searchTextBox_TextChanged(object sender, EventArgs e)
        {
            //(dataGridView1.DataSource as DataTable).DefaultView.RowFilter = string.Format("cashPointId LIKE '%{0}%'", textBoxSearch.Text);
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {

        }



        private void comboPkt_SelectionChangeCommitted(object sender, EventArgs e)
        {
            reloadComboCabang();
            if (comboSetBon.SelectedIndex == 0 && comboVal.SelectedIndex == 0)
            {
                reloadSetoranSudah();
            }
            if (comboSetBon.SelectedIndex == 0 && comboVal.SelectedIndex == 1)
            {
                reloadSetoranBelum();
            }
            if (comboSetBon.SelectedIndex == 0 && comboVal.SelectedIndex == 2)
            {
                reloadSudBelSetoran();
            }
            if (comboSetBon.SelectedIndex == 1 && comboVal.SelectedIndex == 0)
            {
                reloadBonSudah();
            }
            if (comboSetBon.SelectedIndex == 1 && comboVal.SelectedIndex == 1)
            {
                reloadBonBelum();
            }
            if (comboSetBon.SelectedIndex == 1 && comboVal.SelectedIndex == 2)
            {
                reloadSudBelBon();
            }
            if (comboSetBon.SelectedIndex == 2 && comboVal.SelectedIndex == 0)
            {
                reloadSetBonSudah();
            }
            if (comboSetBon.SelectedIndex == 2 && comboVal.SelectedIndex == 1)
            {
                reloadSetBonBelum();
            }
            if (comboSetBon.SelectedIndex == 2 && comboVal.SelectedIndex == 2)
            {
                reloadAll();
            }


        }

        private void comboVal_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (comboSetBon.SelectedIndex == 0 && comboVal.SelectedIndex == 0)
            {
                reloadSetoranSudah();
            }
            if (comboSetBon.SelectedIndex == 0 && comboVal.SelectedIndex == 1)
            {
                reloadSetoranBelum();
            }
            if (comboSetBon.SelectedIndex == 0 && comboVal.SelectedIndex == 2)
            {
                reloadSudBelSetoran();
            }
            if (comboSetBon.SelectedIndex == 1 && comboVal.SelectedIndex == 0)
            {
                reloadBonSudah();
            }
            if (comboSetBon.SelectedIndex == 1 && comboVal.SelectedIndex == 1)
            {
                reloadBonBelum();
            }
            if (comboSetBon.SelectedIndex == 1 && comboVal.SelectedIndex == 2)
            {
                reloadSudBelBon();
            }
            if (comboSetBon.SelectedIndex == 2 && comboVal.SelectedIndex == 0)
            {
                reloadSetBonSudah();
            }
            if (comboSetBon.SelectedIndex == 2 && comboVal.SelectedIndex == 1)
            {
                reloadSetBonBelum();
            }
            if (comboSetBon.SelectedIndex == 2 && comboVal.SelectedIndex == 2)
            {
                reloadAll();
            }

        }

        private void comboSetBon_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (comboSetBon.SelectedIndex == 0 && comboVal.SelectedIndex == 0)
            {
                reloadSetoranSudah();
            }
            if (comboSetBon.SelectedIndex == 0 && comboVal.SelectedIndex == 1)
            {
                reloadSetoranBelum();
            }
            if (comboSetBon.SelectedIndex == 0 && comboVal.SelectedIndex == 2)
            {
                reloadSudBelSetoran();
            }
            if (comboSetBon.SelectedIndex == 1 && comboVal.SelectedIndex == 0)
            {
                reloadBonSudah();
            }
            if (comboSetBon.SelectedIndex == 1 && comboVal.SelectedIndex == 1)
            {
                reloadBonBelum();
            }
            if (comboSetBon.SelectedIndex == 1 && comboVal.SelectedIndex == 2)
            {
                reloadSudBelBon();
            }
            if (comboSetBon.SelectedIndex == 2 && comboVal.SelectedIndex == 0)
            {
                reloadSetBonSudah();
            }
            if (comboSetBon.SelectedIndex == 2 && comboVal.SelectedIndex == 1)
            {
                reloadSetBonBelum();
            }
            if (comboSetBon.SelectedIndex == 2 && comboVal.SelectedIndex == 2)
            {
                reloadAll();
            }

        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            if (comboSetBon.SelectedIndex == 0 && comboVal.SelectedIndex == 0)
            {
                reloadSetoranSudah();
            }
            if (comboSetBon.SelectedIndex == 0 && comboVal.SelectedIndex == 1)
            {
                reloadSetoranBelum();
            }
            if (comboSetBon.SelectedIndex == 0 && comboVal.SelectedIndex == 2)
            {
                reloadSudBelSetoran();
            }
            if (comboSetBon.SelectedIndex == 1 && comboVal.SelectedIndex == 0)
            {
                reloadBonSudah();
            }
            if (comboSetBon.SelectedIndex == 1 && comboVal.SelectedIndex == 1)
            {
                reloadBonBelum();
            }
            if (comboSetBon.SelectedIndex == 1 && comboVal.SelectedIndex == 2)
            {
                reloadSudBelBon();
            }
            if (comboSetBon.SelectedIndex == 2 && comboVal.SelectedIndex == 0)
            {
                reloadSetBonSudah();
            }
            if (comboSetBon.SelectedIndex == 2 && comboVal.SelectedIndex == 1)
            {
                reloadSetBonBelum();
            }
            if (comboSetBon.SelectedIndex == 2 && comboVal.SelectedIndex == 2)
            {
                reloadAll();
            }

        }
        public class ViewRekonSaldoTrxCabang
        {

            public String vendor { set; get; }
            public DateTime? dueDate { set; get; }
            public DateTime? blogTime { set; get; }
            public String cashpointId { set; get; }
            public String cabang { set; get; }
            public Int64? regular { set; get; }
            public Int64? emergency { set; get; }
            public String SetoranBon { set; get; }
            public String Validasi { set; get; }
            public Int64? total { set; get; }


        }

        private void ExportBtn_Click(object sender, EventArgs e)
        {
            SaveFileDialog sv = new SaveFileDialog();
            sv.Filter = Variables.csvFilter;
            if (sv.ShowDialog() == DialogResult.OK)
            {
                String csv = ServiceStack.Text.CsvSerializer.SerializeToCsv(viewRekonSaldoTrxCabangs);
                File.WriteAllText(sv.FileName, csv);
            }
        }
        int sortColIdx = 0;
        bool asc = true;
        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            Type fieldsType = typeof(ViewRekonSaldoTrxCabang);
            PropertyInfo[] props = fieldsType.GetProperties(BindingFlags.Public
            | BindingFlags.Instance);
            int colidx = e.ColumnIndex;
            String colName = props[colidx].Name;
            if (sortColIdx == colidx)
            {
                Console.WriteLine("Kolom Sama");
                asc = !asc;
            }
            else
            {
                asc = true;
                sortColIdx = colidx;
            }
            if (asc)
            {
                if (colName == "vendor")
                    dataGridView1.DataSource = viewRekonSaldoTrxCabangs.OrderBy(x => x.vendor).ToList();
                else if (colName == "dueDate")
                    dataGridView1.DataSource = viewRekonSaldoTrxCabangs.OrderBy(x => x.dueDate).ToList();
                else if (colName == "blogTime")
                    dataGridView1.DataSource = viewRekonSaldoTrxCabangs.OrderBy(x => x.blogTime).ToList();
                else if (colName == "cashpointId")
                    dataGridView1.DataSource = viewRekonSaldoTrxCabangs.OrderBy(x => x.cashpointId).ToList();
                else if (colName == "regular")
                    dataGridView1.DataSource = viewRekonSaldoTrxCabangs.OrderBy(x => x.regular).ToList();
                else if (colName == "emergency")
                    dataGridView1.DataSource = viewRekonSaldoTrxCabangs.OrderBy(x => x.emergency).ToList();
                else if (colName == "total")
                    dataGridView1.DataSource = viewRekonSaldoTrxCabangs.OrderBy(x => x.total).ToList();

            }
            else
            {
                if (colName == "vendor")
                    dataGridView1.DataSource = viewRekonSaldoTrxCabangs.OrderByDescending(x => x.vendor).ToList();
                else if (colName == "dueDate")
                    dataGridView1.DataSource = viewRekonSaldoTrxCabangs.OrderByDescending(x => x.dueDate).ToList();
                else if (colName == "blogTime")
                    dataGridView1.DataSource = viewRekonSaldoTrxCabangs.OrderByDescending(x => x.blogTime).ToList();
                else if (colName == "cashpointId")
                    dataGridView1.DataSource = viewRekonSaldoTrxCabangs.OrderByDescending(x => x.cashpointId).ToList();
                else if (colName == "regular")
                    dataGridView1.DataSource = viewRekonSaldoTrxCabangs.OrderByDescending(x => x.regular).ToList();
                else if (colName == "emergency")
                    dataGridView1.DataSource = viewRekonSaldoTrxCabangs.OrderByDescending(x => x.emergency).ToList();
                else if (colName == "total")
                    dataGridView1.DataSource = viewRekonSaldoTrxCabangs.OrderByDescending(x => x.total).ToList();
            }
            Console.WriteLine(props[e.ColumnIndex].Name);
        }

        private void comboCabang_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (comboSetBon.SelectedIndex == 0 && comboVal.SelectedIndex == 0)
            {
                reloadSetoranSudah();
            }
            if (comboSetBon.SelectedIndex == 0 && comboVal.SelectedIndex == 1)
            {
                reloadSetoranBelum();
            }
            if (comboSetBon.SelectedIndex == 0 && comboVal.SelectedIndex == 2)
            {
                reloadSudBelSetoran();
            }
            if (comboSetBon.SelectedIndex == 1 && comboVal.SelectedIndex == 0)
            {
                reloadBonSudah();
            }
            if (comboSetBon.SelectedIndex == 1 && comboVal.SelectedIndex == 1)
            {
                reloadBonBelum();
            }
            if (comboSetBon.SelectedIndex == 1 && comboVal.SelectedIndex == 2)
            {
                reloadSudBelBon();
            }
            if (comboSetBon.SelectedIndex == 2 && comboVal.SelectedIndex == 0)
            {
                reloadSetBonSudah();
            }
            if (comboSetBon.SelectedIndex == 2 && comboVal.SelectedIndex == 1)
            {
                reloadSetBonBelum();
            }
            if (comboSetBon.SelectedIndex == 2 && comboVal.SelectedIndex == 2)
            {
                reloadAll();
            }
        }
    }
}