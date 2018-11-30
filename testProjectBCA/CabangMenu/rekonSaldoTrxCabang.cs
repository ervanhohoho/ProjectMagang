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

namespace testProjectBCA
{
    public partial class rekonSaldoTrxCabang : Form
    {
        Database1Entities en = new Database1Entities();
        public rekonSaldoTrxCabang()
        {
            InitializeComponent();
            reloadComboPkt();
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

            var query = (from x in en.RekonSaldoPerVendors
                         select x.vendor).Distinct().ToList();

            foreach (var item in query)
            {
                vendor.Add(item.ToString());
            }

            comboPkt.DataSource = vendor;
        }
        
        public void reloadSetoranBelum()
        {

            DateTime date2 = dateTimePicker2.Value.Date;
            var emergency = (
                from x in en.RekonSaldoPerVendors.AsEnumerable()
                where (x.actionRekon.Contains("Return") && x.statusRekon.Equals("In Transit")) && (DateTime.Parse(x.dueDate.ToString()).Date == dateTimePicker2.Value.Date || DateTime.Parse(x.realDate.ToString()) == dateTimePicker2.Value.Date) && x.vendor == comboPkt.SelectedValue.ToString()
                && x.actionRekon.ToLower().Contains("emergency")
                group x by new { x.dueDate, x.cashPointtId, x.blogTime, x.actionRekon, x.statusRekon, x.blogMessage } into z
                select new
                {
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
                from x in en.RekonSaldoPerVendors.AsEnumerable()
                where (x.actionRekon.Contains("Return") && x.statusRekon.Equals("In Transit")) && (DateTime.Parse(x.dueDate.ToString()).Date == dateTimePicker2.Value.Date || DateTime.Parse(x.realDate.ToString()) == dateTimePicker2.Value.Date) && x.vendor == comboPkt.SelectedValue.ToString()
                && !x.actionRekon.ToLower().Contains("emergency")
                group x by new { x.dueDate, x.cashPointtId, x.blogTime, x.actionRekon, x.statusRekon, x.blogMessage } into z
                select new
                {
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
            //var prepare = (from x in en.RekonSaldoPerVendors.AsEnumerable()
            //               where (x.actionRekon.Contains("Return") && x.statusRekon.Equals("In Transit")) &&  (DateTime.Parse(x.dueDate.ToString()).Date == dateTimePicker2.Value.Date || DateTime.Parse(x.realDate.ToString())  == dateTimePicker2.Value.Date) && x.vendor == comboPkt.SelectedValue.ToString()
            //               group x by new { x.dueDate, x.cashPointtId, x.blogTime, x.actionRekon, x.statusRekon, x.blogMessage } into z
            //               select new
            //               {
            //                   blogTime = z.Key.blogTime,
            //                   actionRekon = z.Key.actionRekon,
            //                   statusRekon = z.Key.statusRekon,
            //                   blogMessage = z.Key.blogMessage,
            //                   dueDate = z.Key.dueDate,
            //                   cashPointtId = z.Key.cashPointtId,
            //                   currencyAmmount = z.Sum(x => x.currencyAmmount),
            //                   //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
            //                   validation = z.Key.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(z.Key.blogTime.ToString()).Hour < 21 ? z.Key.blogTime : DateTime.Parse(z.Key.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
            //               }).ToList();

            var query = (from x in prepare
                         where x.actionRekon.Contains("Return") && x.validation.Equals("NOT VALIDATED") && x.statusRekon.Equals("In Transit")
                         group x by new { x.dueDate, x.cashPointtId, x.blogTime } into z
                         select new { dueDate = z.Key.dueDate, blogTime = z.Key.blogTime, cashPointId = z.Key.cashPointtId, regular = z.Sum(x => x.regular), emergency = z.Sum(x => x.emergency), total = z.Sum(x => x.emergency + x.regular) }).ToList();

            dataGridView1.DataSource = query;

            formatting();

        }
        public void reloadSetoranSudah()
        {
         
            DateTime date2 = dateTimePicker2.Value.Date;
            var emergency = (
                from x in en.RekonSaldoPerVendors.AsEnumerable()
                where (x.actionRekon.Contains("Return") && x.statusRekon.Equals("In Transit")) && (DateTime.Parse(x.dueDate.ToString()).Date == dateTimePicker2.Value.Date || DateTime.Parse(x.realDate.ToString()) == dateTimePicker2.Value.Date) && x.vendor == comboPkt.SelectedValue.ToString()
                && x.actionRekon.ToLower().Contains("emergency")
                group x by new { x.dueDate, x.cashPointtId, x.blogTime, x.actionRekon, x.statusRekon, x.blogMessage } into z
                select new
                {
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
                from x in en.RekonSaldoPerVendors.AsEnumerable()
                where (x.actionRekon.Contains("Return") && x.statusRekon.Equals("In Transit")) && (DateTime.Parse(x.dueDate.ToString()).Date == dateTimePicker2.Value.Date || DateTime.Parse(x.realDate.ToString()) == dateTimePicker2.Value.Date) && x.vendor == comboPkt.SelectedValue.ToString()
                && !x.actionRekon.ToLower().Contains("emergency")
                group x by new { x.dueDate, x.cashPointtId, x.blogTime, x.actionRekon, x.statusRekon, x.blogMessage } into z
                select new
                {
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

            //var prepare = (from x in en.RekonSaldoPerVendors.AsEnumerable()
            //               where (x.actionRekon.Contains("Return") && x.statusRekon.Equals("In Transit")) && (DateTime.Parse(x.dueDate.ToString()).Date == dateTimePicker2.Value.Date || DateTime.Parse(x.realDate.ToString()) == dateTimePicker2.Value.Date) && x.vendor == comboPkt.SelectedValue.ToString()
            //               group x by new { x.dueDate, x.cashPointtId, x.blogTime, x.actionRekon, x.statusRekon, x.blogMessage } into z
            //               select new
            //               {
            //                   blogTime = z.Key.blogTime,
            //                   actionRekon = z.Key.actionRekon,
            //                   statusRekon = z.Key.statusRekon,
            //                   blogMessage = z.Key.blogMessage,
            //                   dueDate = z.Key.dueDate,
            //                   cashPointtId = z.Key.cashPointtId,
            //                   currencyAmmount = z.Sum(x => x.currencyAmmount),
            //                   validation = z.Key.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(z.Key.blogTime.ToString()).Hour < 21 ? z.Key.blogTime : DateTime.Parse(z.Key.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
            //               }).ToList();

            var query = (from x in prepare
                         where x.actionRekon.Contains("Return") && x.validation.Equals("VALIDATED") && x.statusRekon.Equals("In Transit")
                         group x by new { x.dueDate, x.cashPointtId, x.blogTime } into z
                         select new { dueDate = z.Key.dueDate, blogTime = z.Key.blogTime, cashPointId = z.Key.cashPointtId, regular = z.Sum(x => x.regular), emergency = z.Sum(x => x.emergency), total = z.Sum(x => x.emergency + x.regular) }).ToList();

            dataGridView1.DataSource = query;

            formatting();

        }
        public void reloadBonSudah()
        {
            DateTime date2 = dateTimePicker2.Value.Date;

            var emergency = (
                from x in en.RekonSaldoPerVendors.AsEnumerable()
                where (x.actionRekon.Contains("Delivery") && x.statusRekon.Equals("Confirmed")) && (DateTime.Parse(x.dueDate.ToString()).Date == dateTimePicker2.Value.Date || DateTime.Parse(x.realDate.ToString()) == dateTimePicker2.Value.Date) && x.vendor == comboPkt.SelectedValue.ToString()
                && x.actionRekon.ToLower().Contains("emergency")
                group x by new { x.dueDate, x.cashPointtId, x.blogTime, x.actionRekon, x.statusRekon, x.blogMessage } into z
                select new
                {
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
                from x in en.RekonSaldoPerVendors.AsEnumerable()
                where (x.actionRekon.Contains("Delivery") && x.statusRekon.Equals("Confirmed")) && (DateTime.Parse(x.dueDate.ToString()).Date == dateTimePicker2.Value.Date || DateTime.Parse(x.realDate.ToString()) == dateTimePicker2.Value.Date) && x.vendor == comboPkt.SelectedValue.ToString()
                && !x.actionRekon.ToLower().Contains("emergency")
                group x by new { x.dueDate, x.cashPointtId, x.blogTime, x.actionRekon, x.statusRekon, x.blogMessage } into z
                select new
                {
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


            //var prepare = (from x in en.RekonSaldoPerVendors.AsEnumerable()
            //               where (x.actionRekon.Contains("Delivery") && x.statusRekon.Equals("Confirmed")) && (DateTime.Parse(x.dueDate.ToString()).Date == dateTimePicker2.Value.Date || DateTime.Parse(x.realDate.ToString()) == dateTimePicker2.Value.Date) && x.vendor == comboPkt.SelectedValue.ToString()
            //               group x by new { x.dueDate, x.cashPointtId, x.blogTime, x.actionRekon, x.statusRekon, x.blogMessage } into z
            //               select new
            //               {
            //                   blogTime = z.Key.blogTime,
            //                   actionRekon = z.Key.actionRekon,
            //                   statusRekon = z.Key.statusRekon,
            //                   dueDate = z.Key.dueDate,
            //                   cashPointtId = z.Key.cashPointtId,
            //                   currencyAmmount = z.Sum(x => x.currencyAmmount),
            //                   validation = z.Key.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(z.Key.blogTime.ToString()).Hour < 21 ? z.Key.blogTime : DateTime.Parse(z.Key.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
            //               }).ToList();

            var query = (from x in prepare
                         where x.actionRekon.Contains("Delivery") && x.validation.Equals("VALIDATED") && x.statusRekon.Equals("Confirmed")
                         group x by new { x.dueDate, x.cashPointtId, x.blogTime } into z
                         select new {dueDate = z.Key.dueDate ,blogTime = z.Key.blogTime, cashPointId = z.Key.cashPointtId , regular = z.Sum(x => x.regular), emergency = z.Sum(x => x.emergency), total = z.Sum(x => x.emergency + x.regular) } ).ToList();

            dataGridView1.DataSource = query;

            formatting();

        }
        public void reloadBonBelum()
        {
 
            DateTime date2 = dateTimePicker2.Value.Date;

            var emergency = (
              from x in en.RekonSaldoPerVendors.AsEnumerable()
              where (x.actionRekon.Contains("Delivery") && x.statusRekon.Equals("Confirmed")) && (DateTime.Parse(x.dueDate.ToString()).Date == dateTimePicker2.Value.Date || DateTime.Parse(x.realDate.ToString()) == dateTimePicker2.Value.Date) && x.vendor == comboPkt.SelectedValue.ToString()
              && x.actionRekon.ToLower().Contains("emergency")
              group x by new { x.dueDate, x.cashPointtId, x.blogTime, x.actionRekon, x.statusRekon, x.blogMessage } into z
              select new
              {
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
                from x in en.RekonSaldoPerVendors.AsEnumerable()
                where (x.actionRekon.Contains("Delivery") && x.statusRekon.Equals("Confirmed")) && (DateTime.Parse(x.dueDate.ToString()).Date == dateTimePicker2.Value.Date || DateTime.Parse(x.realDate.ToString()) == dateTimePicker2.Value.Date) && x.vendor == comboPkt.SelectedValue.ToString()
                && !x.actionRekon.ToLower().Contains("emergency")
                group x by new { x.dueDate, x.cashPointtId, x.blogTime, x.actionRekon, x.statusRekon, x.blogMessage } into z
                select new
                {
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

            //var prepare = (from x in en.RekonSaldoPerVendors.AsEnumerable()
            //               where (x.actionRekon.Contains("Delivery") && x.statusRekon.Equals("Confirmed")) && (DateTime.Parse(x.dueDate.ToString()).Date == dateTimePicker2.Value.Date || DateTime.Parse(x.realDate.ToString()) == dateTimePicker2.Value.Date) && x.vendor == comboPkt.SelectedValue.ToString()
            //               group x by new { x.dueDate, x.cashPointtId, x.blogTime, x.actionRekon, x.statusRekon, x.blogMessage } into z
            //               select new
            //               {
            //                   blogTime = z.Key.blogTime,
            //                   actionRekon = z.Key.actionRekon,
            //                   statusRekon = z.Key.statusRekon,
            //                   dueDate = z.Key.dueDate,
            //                   cashPointtId = z.Key.cashPointtId,
            //                   currencyAmmount = z.Sum(x => x.currencyAmmount),
            //                   validation = z.Key.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(z.Key.blogTime.ToString()).Hour < 21 ? z.Key.blogTime : DateTime.Parse(z.Key.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(dateTimePicker2.Value.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
            //               }).ToList();

            var query = (from x in prepare
                         where x.actionRekon.Contains("Delivery") && x.validation.Equals("NOT VALIDATED") && x.statusRekon.Equals("Confirmed")
                         group x by new { x.dueDate, x.cashPointtId, x.blogTime } into z
                         select new { dueDate = z.Key.dueDate, blogTime = z.Key.blogTime, cashPointId = z.Key.cashPointtId, regular = z.Sum(x => x.regular), emergency = z.Sum(x => x.emergency), total = z.Sum(x => x.emergency + x.regular) }).ToList();

            dataGridView1.DataSource = query;

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
                    dataGridView1.Columns[2].DefaultCellStyle.Format = "c";
                    dataGridView1.Columns[2].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("ID-id");
            }
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
            //(dataGridView1.DataSource as DataTable).DefaultView.RowFilter = string.Format("cashPointId LIKE '%{0}%'", textBoxSearch.Text);
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
           
        }

        private void comboPkt_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (comboSetBon.SelectedIndex == 0 && comboVal.SelectedIndex == 0)
            {
                reloadSetoranSudah();
            }
            else if (comboSetBon.SelectedIndex == 0 && comboVal.SelectedIndex == 1)
            {
                reloadSetoranBelum();
            }
            else if (comboSetBon.SelectedIndex == 1 && comboVal.SelectedIndex == 0)
            {
                reloadBonSudah();
            }
            else if (comboSetBon.SelectedIndex == 1 && comboVal.SelectedIndex == 1)
            {
                reloadBonBelum();
            }
        }

        private void comboVal_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (comboSetBon.SelectedIndex == 0 && comboVal.SelectedIndex == 0)
            {
                reloadSetoranSudah();
            }
            else if (comboSetBon.SelectedIndex == 0 && comboVal.SelectedIndex == 1)
            {
                reloadSetoranBelum();
            }
            else if (comboSetBon.SelectedIndex == 1 && comboVal.SelectedIndex == 0)
            {
                reloadBonSudah();
            }
            else if (comboSetBon.SelectedIndex == 1 && comboVal.SelectedIndex == 1)
            {
                reloadBonBelum();
            }
        }

        private void comboSetBon_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (comboSetBon.SelectedIndex == 0 && comboVal.SelectedIndex == 0)
            {
                reloadSetoranSudah();
            }
            else if (comboSetBon.SelectedIndex == 0 && comboVal.SelectedIndex == 1)
            {
                reloadSetoranBelum();
            }
            else if (comboSetBon.SelectedIndex == 1 && comboVal.SelectedIndex == 0)
            {
                reloadBonSudah();
            }
            else if (comboSetBon.SelectedIndex == 1 && comboVal.SelectedIndex == 1)
            {
                reloadBonBelum();
            }
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            if (comboSetBon.SelectedIndex == 0 && comboVal.SelectedIndex == 0)
            {
                reloadSetoranSudah();
            }
            else if (comboSetBon.SelectedIndex == 0 && comboVal.SelectedIndex == 1)
            {
                reloadSetoranBelum();
            }
            else if (comboSetBon.SelectedIndex == 1 && comboVal.SelectedIndex == 0)
            {
                reloadBonSudah();
            }
            else if (comboSetBon.SelectedIndex == 1 && comboVal.SelectedIndex == 1)
            {
                reloadBonBelum();
            }
        }
    }
}
