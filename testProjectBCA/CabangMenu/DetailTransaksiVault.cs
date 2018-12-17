using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testProjectBCA.CabangMenu
{
    public partial class DetailTransaksiVault : Form
    {
        Database1Entities en = new Database1Entities();
        List<ViewDetailTransaksiVault> viewDetailTransaksiVaults = new List<ViewDetailTransaksiVault>();
        public DetailTransaksiVault()
        {
            InitializeComponent();
            reloadComboPkt();
            //dateTimePicker1.Visible = false;
            comboSetBon.SelectedIndex = 0;
            comboVal.SelectedIndex = 0;
            reloadOutBelum();
        }
        public void reloadComboPkt()
        {
            List<String> vaultId = new List<string>();

            var query = (from x in en.RekonSaldoVaults
                         join y in en.Pkts on x.vaultId.Substring(0,4) equals y.kodePktCabang == "CCASA" ?  "CCAS" : y.kodePktCabang into j
                         from y in j.DefaultIfEmpty()
                         //where x.vaultId.Contains("CCAS") || y.kanwil.ToLower().Contains("jabo")
                         select x.vaultId.Substring(0,4) == "CCAS" ? "CCAS" : x.vaultId).Distinct().OrderBy(x=>x).ToList();

            foreach (var item in query)
            {
                vaultId.Add(item.ToString());
            }

            comboPkt.DataSource = vaultId;
        }

        public void reloadOutBelum()
        {

            DateTime date2 = dateTimePicker2.Value.Date;
            var prequery = (
                from x in en.RekonSaldoVaults.AsEnumerable()
                join y in en.Pkts on x.vaultId.Substring(0,4) equals y.kodePktCabang == "CCASA" ? "CCAS" : y.kodePktCabang
                where (comboPkt.SelectedValue.ToString() == "CCAS" ? x.vaultId.Contains(comboPkt.SelectedValue.ToString()) : x.vaultId == comboPkt.SelectedValue.ToString())
                && y.kanwil.ToLower().Contains("jabo") ? (x.fundingSoure != null ? x.fundingSoure.ToLower().Contains("OB") || x.fundingSoure.ToLower().Contains("BI") : false ): true
                select x
            ).ToList();
            var emergency = (
                from x in prequery
                where (x.actionRekon.Contains("Return")) && (DateTime.Parse(x.dueDate.ToString()).Date == dateTimePicker2.Value.Date || DateTime.Parse(x.realDate.ToString()) == dateTimePicker2.Value.Date) && (comboPkt.SelectedValue.ToString() == "CCAS" ? x.vaultId.Contains(comboPkt.SelectedValue.ToString()) : x.vaultId == comboPkt.SelectedValue.ToString())
                && x.actionRekon.ToLower().Contains("emergency")
                group x by new { x.dueDate, x.fundingSoure, x.timeStampRekon, x.actionRekon, x.statusRekon, x.blogMessage } into z
                select new
                {
                    timeStampRekon = z.Key.timeStampRekon,
                    actionRekon = z.Key.actionRekon,
                    statusRekon = z.Key.statusRekon,
                    blogMessage = z.Key.blogMessage,
                    dueDate = z.Key.dueDate,
                    fundingSoure = z.Key.fundingSoure,
                    emergency = z.Sum(x => x.currencyAmmount),
                    //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.timeStampRekon.ToString()).Hour < 21 ? x.timeStampRekon : DateTime.Parse(x.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                    validation = z.Key.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(z.Key.timeStampRekon.ToString()).Hour < 21 ? z.Key.timeStampRekon : DateTime.Parse(z.Key.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                }).ToList();
            var regular = (
                from x in en.RekonSaldoVaults.AsEnumerable()
                where (x.actionRekon.Contains("Return")) && (DateTime.Parse(x.dueDate.ToString()).Date == dateTimePicker2.Value.Date || DateTime.Parse(x.realDate.ToString()) == dateTimePicker2.Value.Date) && (comboPkt.SelectedValue.ToString() == "CCAS" ? x.vaultId.Contains(comboPkt.SelectedValue.ToString()) : x.vaultId == comboPkt.SelectedValue.ToString())
                && !x.actionRekon.ToLower().Contains("emergency")
                group x by new { x.dueDate, x.fundingSoure, x.timeStampRekon, x.actionRekon, x.statusRekon, x.blogMessage } into z
                select new
                {
                    timeStampRekon = z.Key.timeStampRekon,
                    actionRekon = z.Key.actionRekon,
                    statusRekon = z.Key.statusRekon,
                    blogMessage = z.Key.blogMessage,
                    dueDate = z.Key.dueDate,
                    fundingSoure = z.Key.fundingSoure,
                    regular = z.Sum(x => x.currencyAmmount),
                    //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.timeStampRekon.ToString()).Hour < 21 ? x.timeStampRekon : DateTime.Parse(x.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                    validation = z.Key.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(z.Key.timeStampRekon.ToString()).Hour < 21 ? z.Key.timeStampRekon : DateTime.Parse(z.Key.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                }).ToList();
            var prepareL = (from x in regular
                            join y in emergency on new { x.timeStampRekon, x.dueDate, x.fundingSoure } equals new { y.timeStampRekon, y.dueDate, y.fundingSoure } into temp
                            from y in temp.DefaultIfEmpty()
                            select new
                            {
                                timeStampRekon = x.timeStampRekon,
                                actionRekon = x.actionRekon,
                                statusRekon = x.statusRekon,
                                blogMessage = x.blogMessage,
                                dueDate = x.dueDate,
                                fundingSoure = x.fundingSoure,
                                regular = x.regular,
                                emergency = y == null ? 0 : y.emergency,
                                //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.timeStampRekon.ToString()).Hour < 21 ? x.timeStampRekon : DateTime.Parse(x.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                                validation = x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.timeStampRekon.ToString()).Hour < 21 ? x.timeStampRekon : DateTime.Parse(x.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                            }).ToList();
            var prepareR = (from x in emergency
                            join y in regular on new { x.timeStampRekon, x.dueDate, x.fundingSoure } equals new { y.timeStampRekon, y.dueDate, y.fundingSoure } into temp
                            from y in temp.DefaultIfEmpty()
                            select new
                            {
                                timeStampRekon = x.timeStampRekon,
                                actionRekon = x.actionRekon,
                                statusRekon = x.statusRekon,
                                blogMessage = x.blogMessage,
                                dueDate = x.dueDate,
                                fundingSoure = x.fundingSoure,
                                regular = y == null ? 0 : y.regular,
                                emergency = x.emergency,
                                //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.timeStampRekon.ToString()).Hour < 21 ? x.timeStampRekon : DateTime.Parse(x.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                                validation = x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.timeStampRekon.ToString()).Hour < 21 ? x.timeStampRekon : DateTime.Parse(x.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                            }).ToList();
            var prepare = prepareL.Union(prepareR);

            
            //var prepare = (from x in en.RekonSaldoVaults.AsEnumerable()
            //               where (x.actionRekon.Contains("Return")) &&  (DateTime.Parse(x.dueDate.ToString()).Date == dateTimePicker2.Value.Date || DateTime.Parse(x.realDate.ToString())  == dateTimePicker2.Value.Date) && (comboPkt.SelectedValue.ToString() == "CCAS" ? x.vaultId.Contains(comboPkt.SelectedValue.ToString()) : x.vaultId == comboPkt.SelectedValue.ToString())
            //               group x by new { x.dueDate, x.fundingSoure, x.timeStampRekon, x.actionRekon, x.statusRekon, x.blogMessage } into z
            //               select new
            //               {
            //                   timeStampRekon = z.Key.timeStampRekon,
            //                   actionRekon = z.Key.actionRekon,
            //                   statusRekon = z.Key.statusRekon,
            //                   blogMessage = z.Key.blogMessage,
            //                   dueDate = z.Key.dueDate,
            //                   fundingSoure = z.Key.fundingSoure,
            //                   currencyAmmount = z.Sum(x => x.currencyAmmount),
            //                   //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.timeStampRekon.ToString()).Hour < 21 ? x.timeStampRekon : DateTime.Parse(x.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
            //                   validation = z.Key.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(z.Key.timeStampRekon.ToString()).Hour < 21 ? z.Key.timeStampRekon : DateTime.Parse(z.Key.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
            //               }).ToList();

            var query = (from x in prepare
                         where x.actionRekon.Contains("Return") && x.validation.Equals("NOT VALIDATED")
                         group x by new { x.dueDate, x.fundingSoure, x.timeStampRekon } into z
                         select new { dueDate = z.Key.dueDate, timeStampRekon = z.Key.timeStampRekon, fundingSource = z.Key.fundingSoure, regular = z.Sum(x => x.regular), emergency = z.Sum(x => x.emergency), total = z.Sum(x => x.emergency + x.regular) }).ToList();

            viewDetailTransaksiVaults = query.Select(x=> new ViewDetailTransaksiVault() {
                dueDate = x.dueDate,
                emergency = x.emergency,
                fundingSource = x.fundingSource,
                timeStampRekon = x.timeStampRekon,
                total = x.total,
                regular = x.regular
            }).ToList();
            dataGridView1.DataSource = query;

            formatting();

        }
        public void reloadOutSudah()
        {

            DateTime date2 = dateTimePicker2.Value.Date;
            var prequery = (
                from x in en.RekonSaldoVaults.AsEnumerable()
                join y in en.Pkts on x.vaultId.Substring(0, 4) equals y.kodePktCabang == "CCASA" ? "CCAS" : y.kodePktCabang
                where (comboPkt.SelectedValue.ToString() == "CCAS" ? x.vaultId.Contains(comboPkt.SelectedValue.ToString()) : x.vaultId == comboPkt.SelectedValue.ToString())
                && (!y.kanwil.ToLower().Contains("jabo") ? (x.fundingSoure != null ? x.fundingSoure.ToLower().Contains("OB") || x.fundingSoure.ToLower().Contains("BI") : false) : true)
                select x
            ).ToList();
           
            var emergency = (
                from x in prequery
                where (x.actionRekon.Contains("Return")) 
                && (DateTime.Parse(x.dueDate.ToString()).Date == dateTimePicker2.Value.Date || DateTime.Parse(x.realDate.ToString()) == dateTimePicker2.Value.Date) 
                && (comboPkt.SelectedValue.ToString() == "CCAS" ? x.vaultId.Contains(comboPkt.SelectedValue.ToString()) : x.vaultId == comboPkt.SelectedValue.ToString())
                && x.actionRekon.ToLower().Contains("emergency")
                group x by new { x.dueDate, x.fundingSoure, x.timeStampRekon, x.actionRekon, x.statusRekon, x.blogMessage } into z
                select new
                {
                    timeStampRekon = z.Key.timeStampRekon,
                    actionRekon = z.Key.actionRekon,
                    statusRekon = z.Key.statusRekon,
                    blogMessage = z.Key.blogMessage,
                    dueDate = z.Key.dueDate,
                    fundingSoure = z.Key.fundingSoure,
                    emergency = z.Sum(x => x.currencyAmmount),
                    //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.timeStampRekon.ToString()).Hour < 21 ? x.timeStampRekon : DateTime.Parse(x.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                    validation = z.Key.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(z.Key.timeStampRekon.ToString()).Hour < 21 ? z.Key.timeStampRekon : DateTime.Parse(z.Key.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                }).ToList();
            var regular = (
                from x in en.RekonSaldoVaults.AsEnumerable()
                where (x.actionRekon.Contains("Return")) && (DateTime.Parse(x.dueDate.ToString()).Date == dateTimePicker2.Value.Date || DateTime.Parse(x.realDate.ToString()) == dateTimePicker2.Value.Date) && (comboPkt.SelectedValue.ToString() == "CCAS" ? x.vaultId.Contains(comboPkt.SelectedValue.ToString()) : x.vaultId == comboPkt.SelectedValue.ToString())
                && !x.actionRekon.ToLower().Contains("emergency")
                group x by new { x.dueDate, x.fundingSoure, x.timeStampRekon, x.actionRekon, x.statusRekon, x.blogMessage } into z
                select new
                {
                    timeStampRekon = z.Key.timeStampRekon,
                    actionRekon = z.Key.actionRekon,
                    statusRekon = z.Key.statusRekon,
                    blogMessage = z.Key.blogMessage,
                    dueDate = z.Key.dueDate,
                    fundingSoure = z.Key.fundingSoure,
                    regular = z.Sum(x => x.currencyAmmount),
                    //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.timeStampRekon.ToString()).Hour < 21 ? x.timeStampRekon : DateTime.Parse(x.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                    validation = z.Key.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(z.Key.timeStampRekon.ToString()).Hour < 21 ? z.Key.timeStampRekon : DateTime.Parse(z.Key.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                }).ToList();

            var prepareL = (from x in regular
                            join y in emergency on new { x.timeStampRekon, x.dueDate, x.fundingSoure } equals new { y.timeStampRekon, y.dueDate, y.fundingSoure } into temp
                            from y in temp.DefaultIfEmpty()
                            select new
                            {
                                timeStampRekon = x.timeStampRekon,
                                actionRekon = x.actionRekon,
                                statusRekon = x.statusRekon,
                                blogMessage = x.blogMessage,
                                dueDate = x.dueDate,
                                fundingSoure = x.fundingSoure,
                                regular = x.regular,
                                emergency = y == null ? 0 : y.emergency,
                                //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.timeStampRekon.ToString()).Hour < 21 ? x.timeStampRekon : DateTime.Parse(x.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                                validation = x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.timeStampRekon.ToString()).Hour < 21 ? x.timeStampRekon : DateTime.Parse(x.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                            }).ToList();
            var prepareR = (from x in emergency
                            join y in regular on new { x.timeStampRekon, x.dueDate, x.fundingSoure } equals new { y.timeStampRekon, y.dueDate, y.fundingSoure } into temp
                            from y in temp.DefaultIfEmpty()
                            select new
                            {
                                timeStampRekon = x.timeStampRekon,
                                actionRekon = x.actionRekon,
                                statusRekon = x.statusRekon,
                                blogMessage = x.blogMessage,
                                dueDate = x.dueDate,
                                fundingSoure = x.fundingSoure,
                                regular = y == null ? 0 : y.regular,
                                emergency = x.emergency,
                                //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.timeStampRekon.ToString()).Hour < 21 ? x.timeStampRekon : DateTime.Parse(x.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                                validation = x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.timeStampRekon.ToString()).Hour < 21 ? x.timeStampRekon : DateTime.Parse(x.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                            }).ToList();
            var prepare = prepareL.Union(prepareR);

           
            //var prepare = (from x in en.RekonSaldoVaults.AsEnumerable()
            //               where (x.actionRekon.Contains("Return")) && (DateTime.Parse(x.dueDate.ToString()).Date == dateTimePicker2.Value.Date || DateTime.Parse(x.realDate.ToString()) == dateTimePicker2.Value.Date) && (comboPkt.SelectedValue.ToString() == "CCAS" ? x.vaultId.Contains(comboPkt.SelectedValue.ToString()) : x.vaultId == comboPkt.SelectedValue.ToString())
            //               group x by new { x.dueDate, x.fundingSoure, x.timeStampRekon, x.actionRekon, x.statusRekon, x.blogMessage } into z
            //               select new
            //               {
            //                   timeStampRekon = z.Key.timeStampRekon,
            //                   actionRekon = z.Key.actionRekon,
            //                   statusRekon = z.Key.statusRekon,
            //                   blogMessage = z.Key.blogMessage,
            //                   dueDate = z.Key.dueDate,
            //                   fundingSoure = z.Key.fundingSoure,
            //                   currencyAmmount = z.Sum(x => x.currencyAmmount),
            //                   validation = z.Key.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(z.Key.timeStampRekon.ToString()).Hour < 21 ? z.Key.timeStampRekon : DateTime.Parse(z.Key.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
            //               }).ToList();

            var query = (from x in prepare
                         where x.actionRekon.Contains("Return") && x.validation.Equals("VALIDATED")
                         group x by new { x.dueDate, x.fundingSoure, x.timeStampRekon } into z
                         select new { dueDate = z.Key.dueDate, timeStampRekon = z.Key.timeStampRekon, fundingSource = z.Key.fundingSoure, regular = z.Sum(x => x.regular), emergency = z.Sum(x => x.emergency), total = z.Sum(x => x.emergency + x.regular) }).ToList();

            dataGridView1.DataSource = query;
            viewDetailTransaksiVaults = query.Select(x => new ViewDetailTransaksiVault()
            {
                dueDate = x.dueDate,
                emergency = x.emergency,
                fundingSource = x.fundingSource,
                timeStampRekon = x.timeStampRekon,
                total = x.total,
                regular = x.regular
            }).ToList();
            formatting();

        }
        public void reloadInSudah()
        {
            DateTime date2 = dateTimePicker2.Value.Date;
            var prequery = (
                from x in en.RekonSaldoVaults.AsEnumerable()
                join y in en.Pkts on x.vaultId.Substring(0, 4) equals y.kodePktCabang == "CCASA" ? "CCAS" : y.kodePktCabang
                where (comboPkt.SelectedValue.ToString() == "CCAS" ? x.vaultId.Contains(comboPkt.SelectedValue.ToString()) : x.vaultId == comboPkt.SelectedValue.ToString())
                && (!y.kanwil.ToLower().Contains("jabo") ? (x.fundingSoure != null ? x.fundingSoure.ToLower().Contains("OB") || x.fundingSoure.ToLower().Contains("BI") : false) : true)
                select x
            ).ToList();
            var emergency = (
                from x in prequery
                where (x.actionRekon.Contains("Delivery") && x.statusRekon.Equals("Confirmed")) && (DateTime.Parse(x.dueDate.ToString()).Date == dateTimePicker2.Value.Date || DateTime.Parse(x.realDate.ToString()) == dateTimePicker2.Value.Date) && (comboPkt.SelectedValue.ToString() == "CCAS" ? x.vaultId.Contains(comboPkt.SelectedValue.ToString()) : x.vaultId == comboPkt.SelectedValue.ToString())
                && x.actionRekon.ToLower().Contains("emergency")
                group x by new { x.dueDate, x.fundingSoure, x.timeStampRekon, x.actionRekon, x.statusRekon, x.blogMessage } into z
                select new
                {
                    timeStampRekon = z.Key.timeStampRekon,
                    actionRekon = z.Key.actionRekon,
                    statusRekon = z.Key.statusRekon,
                    blogMessage = z.Key.blogMessage,
                    dueDate = z.Key.dueDate,
                    fundingSoure = z.Key.fundingSoure,
                    emergency = z.Sum(x => x.currencyAmmount),
                    //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.timeStampRekon.ToString()).Hour < 21 ? x.timeStampRekon : DateTime.Parse(x.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                    validation = z.Key.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(z.Key.timeStampRekon.ToString()).Hour < 21 ? z.Key.timeStampRekon : DateTime.Parse(z.Key.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                }).ToList();
            var regular = (
                from x in en.RekonSaldoVaults.AsEnumerable()
                where (x.actionRekon.Contains("Delivery") && x.statusRekon.Equals("Confirmed")) && (DateTime.Parse(x.dueDate.ToString()).Date == dateTimePicker2.Value.Date || DateTime.Parse(x.realDate.ToString()) == dateTimePicker2.Value.Date) && (comboPkt.SelectedValue.ToString() == "CCAS" ? x.vaultId.Contains(comboPkt.SelectedValue.ToString()) : x.vaultId == comboPkt.SelectedValue.ToString())
                && !x.actionRekon.ToLower().Contains("emergency")
                group x by new { x.dueDate, x.fundingSoure, x.timeStampRekon, x.actionRekon, x.statusRekon, x.blogMessage } into z
                select new
                {
                    timeStampRekon = z.Key.timeStampRekon,
                    actionRekon = z.Key.actionRekon,
                    statusRekon = z.Key.statusRekon,
                    blogMessage = z.Key.blogMessage,
                    dueDate = z.Key.dueDate,
                    fundingSoure = z.Key.fundingSoure,
                    regular = z.Sum(x => x.currencyAmmount),
                    //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.timeStampRekon.ToString()).Hour < 21 ? x.timeStampRekon : DateTime.Parse(x.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                    validation = z.Key.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(z.Key.timeStampRekon.ToString()).Hour < 21 ? z.Key.timeStampRekon : DateTime.Parse(z.Key.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                }).ToList();

            var prepareL = (from x in regular
                            join y in emergency on new { x.timeStampRekon, x.dueDate, x.fundingSoure } equals new { y.timeStampRekon, y.dueDate, y.fundingSoure } into temp
                            from y in temp.DefaultIfEmpty()
                            select new
                            {
                                timeStampRekon = x.timeStampRekon,
                                actionRekon = x.actionRekon,
                                statusRekon = x.statusRekon,
                                blogMessage = x.blogMessage,
                                dueDate = x.dueDate,
                                fundingSoure = x.fundingSoure,
                                regular = x.regular,
                                emergency = y == null ? 0 : y.emergency,
                                //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.timeStampRekon.ToString()).Hour < 21 ? x.timeStampRekon : DateTime.Parse(x.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                                validation = x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.timeStampRekon.ToString()).Hour < 21 ? x.timeStampRekon : DateTime.Parse(x.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                            }).ToList();
            var prepareR = (from x in emergency
                            join y in regular on new { x.timeStampRekon, x.dueDate, x.fundingSoure } equals new { y.timeStampRekon, y.dueDate, y.fundingSoure } into temp
                            from y in temp.DefaultIfEmpty()
                            select new
                            {
                                timeStampRekon = x.timeStampRekon,
                                actionRekon = x.actionRekon,
                                statusRekon = x.statusRekon,
                                blogMessage = x.blogMessage,
                                dueDate = x.dueDate,
                                fundingSoure = x.fundingSoure,
                                regular = y == null ? 0 : y.regular,
                                emergency = x.emergency,
                                //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.timeStampRekon.ToString()).Hour < 21 ? x.timeStampRekon : DateTime.Parse(x.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                                validation = x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.timeStampRekon.ToString()).Hour < 21 ? x.timeStampRekon : DateTime.Parse(x.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                            }).ToList();
            var prepare = prepareL.Union(prepareR);


            //var prepare = (from x in en.RekonSaldoVaults.AsEnumerable()
            //               where (x.actionRekon.Contains("Delivery") && x.statusRekon.Equals("Confirmed")) && (DateTime.Parse(x.dueDate.ToString()).Date == dateTimePicker2.Value.Date || DateTime.Parse(x.realDate.ToString()) == dateTimePicker2.Value.Date) && (comboPkt.SelectedValue.ToString() == "CCAS" ? x.vaultId.Contains(comboPkt.SelectedValue.ToString()) : x.vaultId == comboPkt.SelectedValue.ToString())
            //               group x by new { x.dueDate, x.fundingSoure, x.timeStampRekon, x.actionRekon, x.statusRekon, x.blogMessage } into z
            //               select new
            //               {
            //                   timeStampRekon = z.Key.timeStampRekon,
            //                   actionRekon = z.Key.actionRekon,
            //                   statusRekon = z.Key.statusRekon,
            //                   dueDate = z.Key.dueDate,
            //                   fundingSoure = z.Key.fundingSoure,
            //                   currencyAmmount = z.Sum(x => x.currencyAmmount),
            //                   validation = z.Key.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(z.Key.timeStampRekon.ToString()).Hour < 21 ? z.Key.timeStampRekon : DateTime.Parse(z.Key.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
            //               }).ToList();

            var query = (from x in prepare
                         where x.actionRekon.Contains("Delivery") && x.validation.Equals("VALIDATED") && x.statusRekon.Equals("Confirmed")
                         group x by new { x.dueDate, x.fundingSoure, x.timeStampRekon } into z
                         select new { dueDate = z.Key.dueDate, timeStampRekon = z.Key.timeStampRekon, fundingSource = z.Key.fundingSoure, regular = z.Sum(x => x.regular), emergency = z.Sum(x => x.emergency), total = z.Sum(x => x.emergency + x.regular) }).ToList();

            dataGridView1.DataSource = query;
            viewDetailTransaksiVaults = query.Select(x => new ViewDetailTransaksiVault()
            {
                dueDate = x.dueDate,
                emergency = x.emergency,
                fundingSource = x.fundingSource,
                timeStampRekon = x.timeStampRekon,
                total = x.total,
                regular = x.regular
            }).ToList();
            formatting();

        }
        public void reloadInBelum()
        {

            DateTime date2 = dateTimePicker2.Value.Date;
            var prequery = (
                from x in en.RekonSaldoVaults.AsEnumerable()
                join y in en.Pkts on x.vaultId.Substring(0, 4) equals y.kodePktCabang == "CCASA" ? "CCAS" : y.kodePktCabang
                where (comboPkt.SelectedValue.ToString() == "CCAS" ? x.vaultId.Contains(comboPkt.SelectedValue.ToString()) : x.vaultId == comboPkt.SelectedValue.ToString())
                && (!y.kanwil.ToLower().Contains("jabo") ? (x.fundingSoure != null ? x.fundingSoure.ToLower().Contains("OB") || x.fundingSoure.ToLower().Contains("BI") : false) : true)
                select x
            ).ToList();
            var emergency = (
              from x in prequery
              where (x.actionRekon.Contains("Delivery") && x.statusRekon.Equals("Confirmed")) && (DateTime.Parse(x.dueDate.ToString()).Date == dateTimePicker2.Value.Date || DateTime.Parse(x.realDate.ToString()) == dateTimePicker2.Value.Date) && (comboPkt.SelectedValue.ToString() == "CCAS" ? x.vaultId.Contains(comboPkt.SelectedValue.ToString()) : x.vaultId == comboPkt.SelectedValue.ToString())
              && x.actionRekon.ToLower().Contains("emergency")
              group x by new { x.dueDate, x.fundingSoure, x.timeStampRekon, x.actionRekon, x.statusRekon, x.blogMessage } into z
              select new
              {
                  timeStampRekon = z.Key.timeStampRekon,
                  actionRekon = z.Key.actionRekon,
                  statusRekon = z.Key.statusRekon,
                  blogMessage = z.Key.blogMessage,
                  dueDate = z.Key.dueDate,
                  fundingSoure = z.Key.fundingSoure,
                  emergency = z.Sum(x => x.currencyAmmount),
                  //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.timeStampRekon.ToString()).Hour < 21 ? x.timeStampRekon : DateTime.Parse(x.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                  validation = z.Key.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(z.Key.timeStampRekon.ToString()).Hour < 21 ? z.Key.timeStampRekon : DateTime.Parse(z.Key.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
              }).ToList();
            var regular = (
                from x in en.RekonSaldoVaults.AsEnumerable()
                where (x.actionRekon.Contains("Delivery") && x.statusRekon.Equals("Confirmed")) && (DateTime.Parse(x.dueDate.ToString()).Date == dateTimePicker2.Value.Date || DateTime.Parse(x.realDate.ToString()) == dateTimePicker2.Value.Date) && (comboPkt.SelectedValue.ToString() == "CCAS" ? x.vaultId.Contains(comboPkt.SelectedValue.ToString()) : x.vaultId == comboPkt.SelectedValue.ToString())
                && !x.actionRekon.ToLower().Contains("emergency")
                group x by new { x.dueDate, x.fundingSoure, x.timeStampRekon, x.actionRekon, x.statusRekon, x.blogMessage } into z
                select new
                {
                    timeStampRekon = z.Key.timeStampRekon,
                    actionRekon = z.Key.actionRekon,
                    statusRekon = z.Key.statusRekon,
                    blogMessage = z.Key.blogMessage,
                    dueDate = z.Key.dueDate,
                    fundingSoure = z.Key.fundingSoure,
                    regular = z.Sum(x => x.currencyAmmount),
                    //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.timeStampRekon.ToString()).Hour < 21 ? x.timeStampRekon : DateTime.Parse(x.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                    validation = z.Key.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(z.Key.timeStampRekon.ToString()).Hour < 21 ? z.Key.timeStampRekon : DateTime.Parse(z.Key.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                }).ToList();

            var prepareL = (from x in regular
                            join y in emergency on new { x.timeStampRekon, x.dueDate, x.fundingSoure } equals new { y.timeStampRekon, y.dueDate, y.fundingSoure } into temp
                            from y in temp.DefaultIfEmpty()
                            select new
                            {
                                timeStampRekon = x.timeStampRekon,
                                actionRekon = x.actionRekon,
                                statusRekon = x.statusRekon,
                                blogMessage = x.blogMessage,
                                dueDate = x.dueDate,
                                fundingSoure = x.fundingSoure,
                                regular = x.regular,
                                emergency = y == null ? 0 : y.emergency,
                                //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.timeStampRekon.ToString()).Hour < 21 ? x.timeStampRekon : DateTime.Parse(x.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                                validation = x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.timeStampRekon.ToString()).Hour < 21 ? x.timeStampRekon : DateTime.Parse(x.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                            }).ToList();
            var prepareR = (from x in emergency
                            join y in regular on new { x.timeStampRekon, x.dueDate, x.fundingSoure } equals new { y.timeStampRekon, y.dueDate, y.fundingSoure } into temp
                            from y in temp.DefaultIfEmpty()
                            select new
                            {
                                timeStampRekon = x.timeStampRekon,
                                actionRekon = x.actionRekon,
                                statusRekon = x.statusRekon,
                                blogMessage = x.blogMessage,
                                dueDate = x.dueDate,
                                fundingSoure = x.fundingSoure,
                                regular = y == null ? 0 : y.regular,
                                emergency = x.emergency,
                                //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.timeStampRekon.ToString()).Hour < 21 ? x.timeStampRekon : DateTime.Parse(x.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                                validation = x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.timeStampRekon.ToString()).Hour < 21 ? x.timeStampRekon : DateTime.Parse(x.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                            }).ToList();
            var prepare = prepareL.Union(prepareR);

            //var prepare = (from x in en.RekonSaldoVaults.AsEnumerable()
            //               where (x.actionRekon.Contains("Delivery") && x.statusRekon.Equals("Confirmed")) && (DateTime.Parse(x.dueDate.ToString()).Date == dateTimePicker2.Value.Date || DateTime.Parse(x.realDate.ToString()) == dateTimePicker2.Value.Date) && (comboPkt.SelectedValue.ToString() == "CCAS" ? x.vaultId.Contains(comboPkt.SelectedValue.ToString()) : x.vaultId == comboPkt.SelectedValue.ToString())
            //               group x by new { x.dueDate, x.fundingSoure, x.timeStampRekon, x.actionRekon, x.statusRekon, x.blogMessage } into z
            //               select new
            //               {
            //                   timeStampRekon = z.Key.timeStampRekon,
            //                   actionRekon = z.Key.actionRekon,
            //                   statusRekon = z.Key.statusRekon,
            //                   dueDate = z.Key.dueDate,
            //                   fundingSoure = z.Key.fundingSoure,
            //                   currencyAmmount = z.Sum(x => x.currencyAmmount),
            //                   validation = z.Key.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(z.Key.timeStampRekon.ToString()).Hour < 21 ? z.Key.timeStampRekon : DateTime.Parse(z.Key.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
            //               }).ToList();

            var query = (from x in prepare
                         where x.actionRekon.Contains("Delivery") && x.validation.Equals("NOT VALIDATED") && x.statusRekon.Equals("Confirmed")
                         group x by new { x.dueDate, x.fundingSoure, x.timeStampRekon } into z
                         select new { dueDate = z.Key.dueDate, timeStampRekon = z.Key.timeStampRekon, fundingSource = z.Key.fundingSoure, regular = z.Sum(x => x.regular), emergency = z.Sum(x => x.emergency), total = z.Sum(x => x.emergency + x.regular) }).ToList();

            dataGridView1.DataSource = query;
            viewDetailTransaksiVaults = query.Select(x => new ViewDetailTransaksiVault()
            {
                dueDate = x.dueDate,
                emergency = x.emergency,
                fundingSource = x.fundingSource,
                timeStampRekon = x.timeStampRekon,
                total = x.total,
                regular = x.regular
            }).ToList();
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
            if (dataGridView1.Rows.Count > 0)
            {
                for (int a = 0; a < dataGridView1.ColumnCount; a++)
                {
                    if (dataGridView1.Columns[a].ValueType == typeof(Int64?))
                    {
                        dataGridView1.Columns[a].DefaultCellStyle.Format = "C0";
                        dataGridView1.Columns[a].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("ID-id");
                    }
                }
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            DataGridViewSelectedCellCollection cells = dataGridView1.SelectedCells;

            foreach (DataGridViewCell cell in cells)
            {
                int rowidx = cell.RowIndex;
                int colidx = cell.ColumnIndex;
                if (colidx >= 2)
                {
                    dataGridView1.Rows[rowidx].Cells[colidx].Style.Format = "F0";
                }
            }
            for (int a = 0; a < dataGridView1.Rows.Count; a++)
            {
                for (int b = 2; b < dataGridView1.Columns.Count; b++)
                {
                    if (!cells.Contains(dataGridView1.Rows[a].Cells[b]))
                    {
                        Int64 buf;
                        dataGridView1.Rows[a].Cells[b].Style.Format = "C0";
                        dataGridView1.Rows[a].Cells[b].Style.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
                    }
                }
            }
        }

        private void searchTextBox_TextChanged(object sender, EventArgs e)
        {
            //(dataGridView1.DataSource as DataTable).DefaultView.RowFilter = string.Format("fundingSource LIKE '%{0}%'", textBoxSearch.Text);
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {

        }

        private void comboPkt_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (comboSetBon.SelectedIndex == 0 && comboVal.SelectedIndex == 0)
            {
                reloadOutSudah();
            }
            else if (comboSetBon.SelectedIndex == 0 && comboVal.SelectedIndex == 1)
            {
                reloadOutBelum();
            }
            else if (comboSetBon.SelectedIndex == 1 && comboVal.SelectedIndex == 0)
            {
                reloadInSudah();
            }
            else if (comboSetBon.SelectedIndex == 1 && comboVal.SelectedIndex == 1)
            {
                reloadInBelum();
            }
        }

        private void comboVal_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (comboSetBon.SelectedIndex == 0 && comboVal.SelectedIndex == 0)
            {
                reloadOutSudah();
            }
            else if (comboSetBon.SelectedIndex == 0 && comboVal.SelectedIndex == 1)
            {
                reloadOutBelum();
            }
            else if (comboSetBon.SelectedIndex == 1 && comboVal.SelectedIndex == 0)
            {
                reloadInSudah();
            }
            else if (comboSetBon.SelectedIndex == 1 && comboVal.SelectedIndex == 1)
            {
                reloadInBelum();
            }
        }

        private void comboSetBon_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (comboSetBon.SelectedIndex == 0 && comboVal.SelectedIndex == 0)
            {
                reloadOutSudah();
            }
            else if (comboSetBon.SelectedIndex == 0 && comboVal.SelectedIndex == 1)
            {
                reloadOutBelum();
            }
            else if (comboSetBon.SelectedIndex == 1 && comboVal.SelectedIndex == 0)
            {
                reloadInSudah();
            }
            else if (comboSetBon.SelectedIndex == 1 && comboVal.SelectedIndex == 1)
            {
                reloadInBelum();
            }
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            if (comboSetBon.SelectedIndex == 0 && comboVal.SelectedIndex == 0)
            {
                reloadOutSudah();
            }
            else if (comboSetBon.SelectedIndex == 0 && comboVal.SelectedIndex == 1)
            {
                reloadOutBelum();
            }
            else if (comboSetBon.SelectedIndex == 1 && comboVal.SelectedIndex == 0)
            {
                reloadInSudah();
            }
            else if (comboSetBon.SelectedIndex == 1 && comboVal.SelectedIndex == 1)
            {
                reloadInBelum();
            }
        }
        private void exportBtn_Click(object sender, EventArgs e)
        {
            SaveFileDialog sv = new SaveFileDialog();
            sv.Filter = Variables.csvFilter;
            if (sv.ShowDialog() == DialogResult.OK)
            {
                String csv = ServiceStack.Text.CsvSerializer.SerializeToCsv(viewDetailTransaksiVaults);
                File.WriteAllText(sv.FileName, csv);
            }
        }
        public class ViewDetailTransaksiVault
        {
           
            
            public DateTime? dueDate { set; get; }
            public DateTime? timeStampRekon { set; get; }
            public String fundingSource { set; get; }
            public Int64? regular { set; get; }
            public Int64? emergency { set; get; }
            public Int64? total { set; get; }


        }

        
    }
}
