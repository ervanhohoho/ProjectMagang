using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testProjectBCA.CabangMenu
{
    class KumpulanQueryRekonSaldo
    {

        Database1Entities db = new Database1Entities();
        public String kodePkt { set; get; }
        public DateTime tanggal { set; get; }
        public List<PivotCPC_BIBL> listReturnBIdanBankLain { set; get; }
        public List<PivotPerVendor_setoran> listSetoranCabang { set; get; }
        public List<PivotCPC_ATMR> listATMReturn { set; get; }
        public List<PivotCPC_BIBLD> listDeliveryBIdanBankLain { set; get; }
        public List<PivotPerVendor_bon> listBonCabang { set; get; }
        public List<PivotCPC_ATMD> listATMDelivery { set; get; }
        public KumpulanQueryRekonSaldo(String kodePkt, DateTime tanggal)
        {
            this.tanggal = tanggal;
            this.kodePkt = kodePkt;
            listReturnBIdanBankLain = new List<PivotCPC_BIBL>();
            listSetoranCabang = new List<PivotPerVendor_setoran>();
            listATMReturn = new List<PivotCPC_ATMR>();
            listDeliveryBIdanBankLain = new List<PivotCPC_BIBLD>();
            listBonCabang = new List<PivotPerVendor_bon>();
            listATMDelivery = new List<PivotCPC_ATMD>();

            loadListReturnBIdanBankLain();
            loadListATMReturn();
            loadListSetoranCabang(); //udah
            loadListDeliveryBIdanBankLain();
            loadListATMDelivery();
            loadListBonCabang(); //udah
        }

        public List<PivotPerVendor_setoran> reloadSetoranBelum()
        {

            DateTime date2 = tanggal;
            var prequery = (
              from x in db.RekonSaldoPerVendors.AsEnumerable()
              join y in db.Pkts on x.vendor.Substring(0, 4) equals y.kodePktCabang == "CCASA" ? "CCAS" : y.kodePktCabang
              where (kodePkt == "CCAS" ? x.vendor.Contains(kodePkt) : x.vendor == kodePkt)
              select x
            ).ToList();

            var emergency = (
                from x in prequery
                where (x.actionRekon.Contains("Return") && x.statusRekon.Equals("In Transit")) && (DateTime.Parse(x.dueDate.ToString()).Date == tanggal && DateTime.Parse(x.realDate.ToString()) > tanggal) && (kodePkt == "CCAS" ? x.vendor.Contains(kodePkt) : x.vendor == kodePkt)
                && x.actionRekon.ToLower().Contains("emergency")
                group x by new { x.dueDate, x.cashPointtId, x.blogTime, x.actionRekon, x.statusRekon, x.blogMessage, x.realDate } into z
                select new
                {
                    z.Key.realDate,
                    z.Key.blogTime,
                    z.Key.actionRekon,
                    z.Key.statusRekon,
                    z.Key.blogMessage,
                    z.Key.dueDate,
                    z.Key.cashPointtId,
                    emergency = z.Sum(x => x.currencyAmmount),
                    validation = z.Key.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(z.Key.blogTime.ToString()).Hour < 21 ? z.Key.blogTime : DateTime.Parse(z.Key.blogTime.ToString()).AddDays(1)).ToString()).Date <= tanggal.Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                }).ToList();
            var regular = (
                from x in prequery
                where (x.actionRekon.Contains("Return") && x.statusRekon.Equals("In Transit")) && (DateTime.Parse(x.dueDate.ToString()).Date == tanggal && DateTime.Parse(x.realDate.ToString()) > tanggal) && (kodePkt == "CCAS" ? x.vendor.Contains(kodePkt) : x.vendor == kodePkt)
                && !x.actionRekon.ToLower().Contains("emergency")
                group x by new { x.dueDate, x.cashPointtId, x.blogTime, x.actionRekon, x.statusRekon, x.blogMessage, x.realDate } into z
                select new
                {
                    z.Key.realDate,
                    z.Key.blogTime,
                    z.Key.actionRekon,
                    z.Key.statusRekon,
                    z.Key.blogMessage,
                    z.Key.dueDate,
                    z.Key.cashPointtId,
                    regular = z.Sum(x => x.currencyAmmount),
                    //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                    validation = z.Key.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(z.Key.blogTime.ToString()).Hour < 21 ? z.Key.blogTime : DateTime.Parse(z.Key.blogTime.ToString()).AddDays(1)).ToString()).Date <= tanggal.Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                }).ToList();

            var prepareL = (from x in regular
                            join y in emergency on new { x.blogTime, x.dueDate, x.cashPointtId, x.realDate } equals new { y.blogTime, y.dueDate, y.cashPointtId, y.realDate } into temp
                            from y in temp.DefaultIfEmpty()
                            select new
                            {
                                x.realDate,
                                x.blogTime,
                                x.actionRekon,
                                x.statusRekon,
                                x.blogMessage,
                                x.dueDate,
                                x.cashPointtId,
                                x.regular,
                                emergency = y == null ? 0 : y.emergency,
                                //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                                validation = x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= tanggal.Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                            }).ToList();
            var prepareR = (from x in emergency
                            join y in regular on new { x.blogTime, x.dueDate, x.cashPointtId, x.realDate } equals new { y.blogTime, y.dueDate, y.cashPointtId, y.realDate } into temp
                            from y in temp.DefaultIfEmpty()
                            select new
                            {
                                x.realDate,
                                x.blogTime,
                                x.actionRekon,
                                x.statusRekon,
                                x.blogMessage,
                                x.dueDate,
                                x.cashPointtId,
                                regular = y == null ? 0 : y.regular,
                                x.emergency,
                                validation = x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= tanggal.Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                            }).ToList();
            var prepare = prepareL.Union(prepareR);
            var query = (from x in prepare
                         where x.actionRekon.Contains("Return") && x.validation.Equals("NOT VALIDATED") && x.statusRekon.Equals("In Transit")
                         group x by new { x.dueDate, x.cashPointtId, x.blogTime, x.realDate } into z
                         select new { dueDate = z.Key.dueDate, blogTime = z.Key.blogTime, cashPointId = z.Key.cashPointtId, regular = z.Sum(x => x.regular), emergency = z.Sum(x => x.emergency), total = z.Sum(x => x.emergency + x.regular), z.Key.realDate, vendor = kodePkt }).ToList();
            List<PivotPerVendor_setoran> ret = query
               .GroupBy(x => new { x.vendor, x.dueDate, x.blogTime, x.realDate })
               .Select(x => new PivotPerVendor_setoran()
               {
                   sudahValidasi = 0,
                   belumValidasi = (Int64)x.Sum(z => z.regular + z.emergency),
                   grandTotal = (Int64)x.Sum(z => z.regular + z.emergency),
                   dueDate = (DateTime)x.Key.dueDate,
                   vendor = x.Key.vendor,
                   valDate = (DateTime)x.Key.realDate
               }).ToList();
            return ret;

        }
        public List<PivotPerVendor_setoran> reloadSetoranSudah()
        {

            DateTime date2 = tanggal;
            var prequery = (
              from x in db.RekonSaldoPerVendors.AsEnumerable()
              join y in db.Pkts on x.vendor.Substring(0, 4) equals y.kodePktCabang == "CCASA" ? "CCAS" : y.kodePktCabang
              where (kodePkt == "CCAS" ? x.vendor.Contains(kodePkt) : x.vendor == kodePkt)
              select x
            ).ToList();
            var emergency = (
                from x in prequery
                where (x.actionRekon.Contains("Return") && x.statusRekon.Equals("In Transit")) && (DateTime.Parse(x.dueDate.ToString()).Date == tanggal || DateTime.Parse(x.realDate.ToString()) == tanggal) && (kodePkt == "CCAS" ? x.vendor.Contains(kodePkt) : x.vendor == kodePkt)
                && x.actionRekon.ToLower().Contains("emergency")
                group x by new { x.dueDate, x.cashPointtId, x.blogTime, x.actionRekon, x.statusRekon, x.blogMessage, x.realDate } into z
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
                    validation = z.Key.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(z.Key.blogTime.ToString()).Hour < 21 ? z.Key.blogTime : DateTime.Parse(z.Key.blogTime.ToString()).AddDays(1)).ToString()).Date <= tanggal.Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                }).ToList();
            var regular = (
                from x in prequery
                where (x.actionRekon.Contains("Return") && x.statusRekon.Equals("In Transit")) && (DateTime.Parse(x.dueDate.ToString()).Date == tanggal || DateTime.Parse(x.realDate.ToString()) == tanggal) && (kodePkt == "CCAS" ? x.vendor.Contains(kodePkt) : x.vendor == kodePkt)
                && !x.actionRekon.ToLower().Contains("emergency")
                group x by new { x.dueDate, x.cashPointtId, x.blogTime, x.actionRekon, x.statusRekon, x.blogMessage, x.realDate } into z
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
                    validation = z.Key.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(z.Key.blogTime.ToString()).Hour < 21 ? z.Key.blogTime : DateTime.Parse(z.Key.blogTime.ToString()).AddDays(1)).ToString()).Date <= tanggal.Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
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
                                validation = x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= tanggal.Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
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
                                validation = x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= tanggal.Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                            }).ToList();
            var prepare = prepareL.Union(prepareR);
            var query = (from x in prepare
                         where x.actionRekon.Contains("Return") && x.validation.Equals("VALIDATED") && x.statusRekon.Equals("In Transit")
                         group x by new { x.dueDate, x.cashPointtId, x.blogTime } into z
                         select new { dueDate = z.Key.dueDate, blogTime = z.Key.blogTime, cashPointId = z.Key.cashPointtId, regular = z.Sum(x => x.regular), emergency = z.Sum(x => x.emergency), total = z.Sum(x => x.emergency + x.regular), vendor = kodePkt }).ToList();
            List<PivotPerVendor_setoran> ret = query
                .GroupBy(x => new { x.vendor, x.dueDate, x.blogTime })
                .Select(x => new PivotPerVendor_setoran()
                {
                    sudahValidasi = (Int64)x.Sum(z => z.regular + z.emergency),
                    belumValidasi = 0,
                    grandTotal = (Int64)x.Sum(z => z.regular + z.emergency),
                    dueDate = (DateTime)x.Key.dueDate,
                    vendor = x.Key.vendor,
                    valDate = tanggal
                }).ToList();
            return ret;
        }
        public List<PivotPerVendor_bon> reloadBonBelum()
        {

            DateTime date2 = tanggal.Date;
            var prequery = (
              from x in db.RekonSaldoPerVendors.AsEnumerable()
              join y in db.Pkts on x.vendor.Substring(0, 4) equals y.kodePktCabang == "CCASA" ? "CCAS" : y.kodePktCabang
              where (kodePkt == "CCAS" ? x.vendor.Contains(kodePkt) : x.vendor == kodePkt)
              select x
            ).ToList();
            var emergency = (
              from x in prequery
              where (x.actionRekon.Contains("Delivery") && x.statusRekon.Equals("Confirmed")) && (DateTime.Parse(x.dueDate.ToString()).Date == tanggal.Date && DateTime.Parse(x.realDate.ToString()) > tanggal.Date) && (kodePkt == "CCAS" ? x.vendor.Contains(kodePkt) : x.vendor == kodePkt)
              && x.actionRekon.ToLower().Contains("emergency")
              group x by new { x.dueDate, x.cashPointtId, x.blogTime, x.actionRekon, x.statusRekon, x.blogMessage, x.realDate } into z
              select new
              {
                  z.Key.realDate,
                  blogTime = z.Key.blogTime,
                  actionRekon = z.Key.actionRekon,
                  statusRekon = z.Key.statusRekon,
                  blogMessage = z.Key.blogMessage,
                  dueDate = z.Key.dueDate,
                  cashPointtId = z.Key.cashPointtId,
                  emergency = z.Sum(x => x.currencyAmmount),
                  //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                  validation = z.Key.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(z.Key.blogTime.ToString()).Hour < 21 ? z.Key.blogTime : DateTime.Parse(z.Key.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(tanggal.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
              }).ToList();
            var regular = (
                from x in prequery
                where (x.actionRekon.Contains("Delivery") && x.statusRekon.Equals("Confirmed")) && (DateTime.Parse(x.dueDate.ToString()).Date == tanggal.Date && DateTime.Parse(x.realDate.ToString()) > tanggal.Date) && (kodePkt == "CCAS" ? x.vendor.Contains(kodePkt) : x.vendor == kodePkt)
                && !x.actionRekon.ToLower().Contains("emergency")
                group x by new { x.dueDate, x.cashPointtId, x.blogTime, x.actionRekon, x.statusRekon, x.blogMessage, x.realDate } into z
                select new
                {
                    z.Key.realDate,
                    blogTime = z.Key.blogTime,
                    actionRekon = z.Key.actionRekon,
                    statusRekon = z.Key.statusRekon,
                    blogMessage = z.Key.blogMessage,
                    dueDate = z.Key.dueDate,
                    cashPointtId = z.Key.cashPointtId,
                    regular = z.Sum(x => x.currencyAmmount),
                    //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                    validation = z.Key.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(z.Key.blogTime.ToString()).Hour < 21 ? z.Key.blogTime : DateTime.Parse(z.Key.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(tanggal.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                }).ToList();

            var prepareL = (from x in regular
                            join y in emergency on new { x.blogTime, x.dueDate, x.cashPointtId, x.realDate } equals new { y.blogTime, y.dueDate, y.cashPointtId, y.realDate } into temp
                            from y in temp.DefaultIfEmpty()
                            select new
                            {
                                x.realDate,
                                blogTime = x.blogTime,
                                actionRekon = x.actionRekon,
                                statusRekon = x.statusRekon,
                                blogMessage = x.blogMessage,
                                dueDate = x.dueDate,
                                cashPointtId = x.cashPointtId,
                                regular = x.regular,
                                emergency = y == null ? 0 : y.emergency,
                                //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                                validation = x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(tanggal.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                            }).ToList();
            var prepareR = (from x in emergency
                            join y in regular on new { x.blogTime, x.dueDate, x.cashPointtId } equals new { y.blogTime, y.dueDate, y.cashPointtId } into temp
                            from y in temp.DefaultIfEmpty()
                            select new
                            {
                                x.realDate,
                                blogTime = x.blogTime,
                                actionRekon = x.actionRekon,
                                statusRekon = x.statusRekon,
                                blogMessage = x.blogMessage,
                                dueDate = x.dueDate,
                                cashPointtId = x.cashPointtId,
                                regular = y == null ? 0 : y.regular,
                                emergency = x.emergency,
                                //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                                validation = x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(tanggal.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                            }).ToList();
            var prepare = prepareL.Union(prepareR);
            var query = (from x in prepare
                         where x.actionRekon.Contains("Delivery") && x.validation.Equals("NOT VALIDATED") && x.statusRekon.Equals("Confirmed")
                         group x by new { x.dueDate, x.cashPointtId, x.blogTime, x.realDate } into z
                         select new { dueDate = z.Key.dueDate, blogTime = z.Key.blogTime, cashPointId = z.Key.cashPointtId, regular = z.Sum(x => x.regular), emergency = z.Sum(x => x.emergency), total = z.Sum(x => x.emergency + x.regular), z.Key.realDate, vendor = kodePkt }).ToList();
            List<PivotPerVendor_bon> ret = query
               .GroupBy(x => new { x.vendor, x.dueDate, x.blogTime, x.realDate })
               .Select(x => new PivotPerVendor_bon()
               {
                   sudahValidasi = 0,
                   belumValidasi = (Int64)x.Sum(z => z.regular + z.emergency),
                   grandTotal = (Int64)x.Sum(z => z.regular + z.emergency),
                   dueDate = (DateTime)x.Key.dueDate,
                   vendor = x.Key.vendor,
                   valDate = (DateTime)x.Key.realDate
               }).ToList();
            return ret;
        }
        public List<PivotPerVendor_bon> reloadBonSudah()
        {
            DateTime date2 = tanggal.Date;
            var prequery = (
              from x in db.RekonSaldoPerVendors.AsEnumerable()
              join y in db.Pkts on x.vendor.Substring(0, 4) equals y.kodePktCabang == "CCASA" ? "CCAS" : y.kodePktCabang
              where (kodePkt == "CCAS" ? x.vendor.Contains(kodePkt) : x.vendor == kodePkt)
              select x
            ).ToList();
            var emergency = (
                from x in prequery
                where (x.actionRekon.Contains("Delivery") && x.statusRekon.Equals("Confirmed")) && (DateTime.Parse(x.dueDate.ToString()).Date == tanggal.Date || DateTime.Parse(x.realDate.ToString()) == tanggal.Date) && (kodePkt == "CCAS" ? x.vendor.Contains(kodePkt) : x.vendor == kodePkt)
                && x.actionRekon.ToLower().Contains("emergency")
                group x by new { x.dueDate, x.cashPointtId, x.blogTime, x.actionRekon, x.statusRekon, x.blogMessage, x.realDate } into z
                select new
                {
                    z.Key.realDate,
                    blogTime = z.Key.blogTime,
                    actionRekon = z.Key.actionRekon,
                    statusRekon = z.Key.statusRekon,
                    blogMessage = z.Key.blogMessage,
                    dueDate = z.Key.dueDate,
                    cashPointtId = z.Key.cashPointtId,
                    emergency = z.Sum(x => x.currencyAmmount),
                    //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                    validation = z.Key.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(z.Key.blogTime.ToString()).Hour < 21 ? z.Key.blogTime : DateTime.Parse(z.Key.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(tanggal.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                }).ToList();
            var regular = (
                from x in prequery
                where (x.actionRekon.Contains("Delivery") && x.statusRekon.Equals("Confirmed")) && (DateTime.Parse(x.dueDate.ToString()).Date == tanggal.Date || DateTime.Parse(x.realDate.ToString()) == tanggal.Date) && (kodePkt == "CCAS" ? x.vendor.Contains(kodePkt) : x.vendor == kodePkt)
                && !x.actionRekon.ToLower().Contains("emergency")
                group x by new { x.dueDate, x.cashPointtId, x.blogTime, x.actionRekon, x.statusRekon, x.blogMessage, x.realDate } into z
                select new
                {
                    z.Key.realDate,
                    blogTime = z.Key.blogTime,
                    actionRekon = z.Key.actionRekon,
                    statusRekon = z.Key.statusRekon,
                    blogMessage = z.Key.blogMessage,
                    dueDate = z.Key.dueDate,
                    cashPointtId = z.Key.cashPointtId,
                    regular = z.Sum(x => x.currencyAmmount),
                    //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                    validation = z.Key.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(z.Key.blogTime.ToString()).Hour < 21 ? z.Key.blogTime : DateTime.Parse(z.Key.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(tanggal.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                }).ToList();

            var prepareL = (from x in regular
                            join y in emergency on new { x.blogTime, x.dueDate, x.cashPointtId, x.realDate } equals new { y.blogTime, y.dueDate, y.cashPointtId, y.realDate } into temp
                            from y in temp.DefaultIfEmpty()
                            select new
                            {
                                x.realDate,
                                blogTime = x.blogTime,
                                actionRekon = x.actionRekon,
                                statusRekon = x.statusRekon,
                                blogMessage = x.blogMessage,
                                dueDate = x.dueDate,
                                cashPointtId = x.cashPointtId,
                                regular = x.regular,
                                emergency = y == null ? 0 : y.emergency,
                                //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                                validation = x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(tanggal.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                            }).ToList();
            var prepareR = (from x in emergency
                            join y in regular on new { x.blogTime, x.dueDate, x.cashPointtId, x.realDate } equals new { y.blogTime, y.dueDate, y.cashPointtId, y.realDate } into temp
                            from y in temp.DefaultIfEmpty()
                            select new
                            {
                                x.realDate,
                                blogTime = x.blogTime,
                                actionRekon = x.actionRekon,
                                statusRekon = x.statusRekon,
                                blogMessage = x.blogMessage,
                                dueDate = x.dueDate,
                                cashPointtId = x.cashPointtId,
                                regular = y == null ? 0 : y.regular,
                                emergency = x.emergency,
                                //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                                validation = x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(tanggal.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                            }).ToList();
            var prepare = prepareL.Union(prepareR);


            //var prepare = (from x in db.RekonSaldoPerVendors.AsEnumerable()
            //               where (x.actionRekon.Contains("Delivery") && x.statusRekon.Equals("Confirmed")) && (DateTime.Parse(x.dueDate.ToString()).Date == tanggal.Date || DateTime.Parse(x.realDate.ToString()) == tanggal.Date) && (kodePkt == "CCAS" ? x.vendor.Contains(kodePkt) : x.vendor == kodePkt)
            //               group x by new { x.dueDate, x.cashPointtId, x.blogTime, x.actionRekon, x.statusRekon, x.blogMessage } into z
            //               select new
            //               {
            //                   blogTime = z.Key.blogTime,
            //                   actionRekon = z.Key.actionRekon,
            //                   statusRekon = z.Key.statusRekon,
            //                   dueDate = z.Key.dueDate,
            //                   cashPointtId = z.Key.cashPointtId,
            //                   currencyAmmount = z.Sum(x => x.currencyAmmount),
            //                   validation = z.Key.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(z.Key.blogTime.ToString()).Hour < 21 ? z.Key.blogTime : DateTime.Parse(z.Key.blogTime.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(tanggal.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
            //               }).ToList();

            var query = (from x in prepare
                         where x.actionRekon.Contains("Delivery") && x.validation.Equals("VALIDATED") && x.statusRekon.Equals("Confirmed")
                         group x by new { x.dueDate, x.cashPointtId, x.blogTime } into z
                         select new { dueDate = z.Key.dueDate, blogTime = z.Key.blogTime, cashPointId = z.Key.cashPointtId, regular = z.Sum(x => x.regular), emergency = z.Sum(x => x.emergency), total = z.Sum(x => x.emergency + x.regular), vendor = kodePkt }).ToList();

            List<PivotPerVendor_bon> ret = query
                .GroupBy(x => new { x.vendor, x.dueDate, x.blogTime })
                .Select(x => new PivotPerVendor_bon()
                {
                    sudahValidasi = (Int64)x.Sum(z => z.regular + z.emergency),
                    belumValidasi = 0,
                    grandTotal = (Int64)x.Sum(z => z.regular + z.emergency),
                    dueDate = (DateTime)x.Key.dueDate,
                    vendor = x.Key.vendor,
                    valDate = tanggal
                }).ToList();
            return ret;

        }
        public List<PivotCPC_BIBL> reloadReturnBIBankLainBelum()
        {

            DateTime date2 = tanggal.Date;
            var prequery = (
                from x in db.RekonSaldoVaults.AsEnumerable()
                join y in db.Pkts on x.vaultId.Substring(0, 4) equals y.kodePktCabang == "CCASA" ? "CCAS" : y.kodePktCabang
                where (kodePkt == "CCAS" ? x.vaultId.Contains(kodePkt) : x.vaultId == kodePkt)
                && y.kanwil.ToLower().Contains("jabo") ? (x.fundingSoure != null ? x.fundingSoure.ToLower().Contains("OB") || x.fundingSoure.ToLower().Contains("BI") : false) : true
                select x
            ).ToList();
            var emergency = (
                from x in prequery
                where (x.actionRekon.Contains("Return")) && (DateTime.Parse(x.dueDate.ToString()).Date == tanggal.Date && DateTime.Parse(x.realDate.ToString()) > tanggal.Date) && (kodePkt == "CCAS" ? x.vaultId.Contains(kodePkt) : x.vaultId == kodePkt)
                && x.actionRekon.ToLower().Contains("emergency")
                group x by new { x.dueDate, x.fundingSoure, x.timeStampRekon, x.actionRekon, x.statusRekon, x.blogMessage, x.realDate } into z
                select new
                {
                    z.Key.realDate,
                    timeStampRekon = z.Key.timeStampRekon,
                    actionRekon = z.Key.actionRekon,
                    statusRekon = z.Key.statusRekon,
                    blogMessage = z.Key.blogMessage,
                    dueDate = z.Key.dueDate,
                    fundingSoure = z.Key.fundingSoure,
                    emergency = z.Sum(x => x.currencyAmmount),
                    //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.timeStampRekon.ToString()).Hour < 21 ? x.timeStampRekon : DateTime.Parse(x.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                    validation = z.Key.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(z.Key.timeStampRekon.ToString()).Hour < 21 ? z.Key.timeStampRekon : DateTime.Parse(z.Key.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(tanggal.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                }).ToList();
            var regular = (
                from x in db.RekonSaldoVaults.AsEnumerable()
                where (x.actionRekon.Contains("Return")) && (DateTime.Parse(x.dueDate.ToString()).Date == tanggal.Date && DateTime.Parse(x.realDate.ToString()) > tanggal.Date) && (kodePkt == "CCAS" ? x.vaultId.Contains(kodePkt) : x.vaultId == kodePkt)
                && !x.actionRekon.ToLower().Contains("emergency")
                group x by new { x.dueDate, x.fundingSoure, x.timeStampRekon, x.actionRekon, x.statusRekon, x.blogMessage, x.realDate } into z
                select new
                {
                    z.Key.realDate,
                    timeStampRekon = z.Key.timeStampRekon,
                    actionRekon = z.Key.actionRekon,
                    statusRekon = z.Key.statusRekon,
                    blogMessage = z.Key.blogMessage,
                    dueDate = z.Key.dueDate,
                    fundingSoure = z.Key.fundingSoure,
                    regular = z.Sum(x => x.currencyAmmount),
                    //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.timeStampRekon.ToString()).Hour < 21 ? x.timeStampRekon : DateTime.Parse(x.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                    validation = z.Key.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(z.Key.timeStampRekon.ToString()).Hour < 21 ? z.Key.timeStampRekon : DateTime.Parse(z.Key.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(tanggal.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                }).ToList();
            var prepareL = (from x in regular
                            join y in emergency on new { x.timeStampRekon, x.dueDate, x.fundingSoure, x.realDate } equals new { y.timeStampRekon, y.dueDate, y.fundingSoure, y.realDate } into temp
                            from y in temp.DefaultIfEmpty()
                            select new
                            {
                                x.realDate,
                                timeStampRekon = x.timeStampRekon,
                                actionRekon = x.actionRekon,
                                statusRekon = x.statusRekon,
                                blogMessage = x.blogMessage,
                                dueDate = x.dueDate,
                                fundingSoure = x.fundingSoure,
                                regular = x.regular,
                                emergency = y == null ? 0 : y.emergency,
                                //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.timeStampRekon.ToString()).Hour < 21 ? x.timeStampRekon : DateTime.Parse(x.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                                validation = x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.timeStampRekon.ToString()).Hour < 21 ? x.timeStampRekon : DateTime.Parse(x.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(tanggal.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                            }).ToList();
            var prepareR = (from x in emergency
                            join y in regular on new { x.timeStampRekon, x.dueDate, x.fundingSoure, x.realDate } equals new { y.timeStampRekon, y.dueDate, y.fundingSoure, y.realDate } into temp
                            from y in temp.DefaultIfEmpty()
                            select new
                            {
                                x.realDate,
                                timeStampRekon = x.timeStampRekon,
                                actionRekon = x.actionRekon,
                                statusRekon = x.statusRekon,
                                blogMessage = x.blogMessage,
                                dueDate = x.dueDate,
                                fundingSoure = x.fundingSoure,
                                regular = y == null ? 0 : y.regular,
                                emergency = x.emergency,
                                //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.timeStampRekon.ToString()).Hour < 21 ? x.timeStampRekon : DateTime.Parse(x.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                                validation = x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.timeStampRekon.ToString()).Hour < 21 ? x.timeStampRekon : DateTime.Parse(x.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(tanggal.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                            }).ToList();
            var prepare = prepareL.Union(prepareR);
            var query = (from x in prepare
                         where x.actionRekon.Contains("Return") && x.validation.Equals("NOT VALIDATED")
                         group x by new { x.dueDate, x.fundingSoure, x.timeStampRekon, x.realDate } into z
                         select new { dueDate = z.Key.dueDate, timeStampRekon = z.Key.timeStampRekon, fundingSource = z.Key.fundingSoure, regular = z.Sum(x => x.regular), emergency = z.Sum(x => x.emergency), total = z.Sum(x => x.emergency + x.regular), z.Key.realDate, vendor = kodePkt }).ToList();
            List<PivotCPC_BIBL> ret = query
               .GroupBy(x => new { x.vendor, x.dueDate, x.timeStampRekon, x.realDate, x.fundingSource })
               .Select(x => new PivotCPC_BIBL()
               {
                   fundingSource = x.Key.fundingSource,
                   plannedReturnNotVal = (Int64)x.Sum(z => z.regular + z.emergency),
                   plannedReturnVal = 0,
                   dueDate = (DateTime) x.Key.dueDate,
                   realDate = (DateTime) x.Key.realDate,
                   grandTotal = (Int64) x.Sum(z => z.regular + z.emergency),
                   vaultId = x.Key.vendor,
               }).ToList();
            return ret;
        }
        public List<PivotCPC_BIBL> reloadReturnBIBankLainSudah()
        {

            DateTime date2 = tanggal.Date;
            var prequery = (
                from x in db.RekonSaldoVaults.AsEnumerable()
                join y in db.Pkts on x.vaultId.Substring(0, 4) equals y.kodePktCabang == "CCASA" ? "CCAS" : y.kodePktCabang
                where (kodePkt == "CCAS" ? x.vaultId.Contains(kodePkt) : x.vaultId == kodePkt)
                && (!y.kanwil.ToLower().Contains("jabo") ? (x.fundingSoure != null ? x.fundingSoure.ToLower().Contains("OB") || x.fundingSoure.ToLower().Contains("BI") : false) : true)
                select x
            ).ToList();

            var emergency = (
                from x in prequery
                where (x.actionRekon.Contains("Return"))
                && (DateTime.Parse(x.dueDate.ToString()).Date == tanggal.Date || DateTime.Parse(x.realDate.ToString()) == tanggal.Date)
                && (kodePkt == "CCAS" ? x.vaultId.Contains(kodePkt) : x.vaultId == kodePkt)
                && x.actionRekon.ToLower().Contains("emergency")
                group x by new { x.dueDate, x.fundingSoure, x.timeStampRekon, x.actionRekon, x.statusRekon, x.blogMessage, x.realDate } into z
                select new
                {
                    z.Key.realDate,
                    timeStampRekon = z.Key.timeStampRekon,
                    actionRekon = z.Key.actionRekon,
                    statusRekon = z.Key.statusRekon,
                    blogMessage = z.Key.blogMessage,
                    dueDate = z.Key.dueDate,
                    fundingSoure = z.Key.fundingSoure,
                    emergency = z.Sum(x => x.currencyAmmount),
                    //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.timeStampRekon.ToString()).Hour < 21 ? x.timeStampRekon : DateTime.Parse(x.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                    validation = z.Key.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(z.Key.timeStampRekon.ToString()).Hour < 21 ? z.Key.timeStampRekon : DateTime.Parse(z.Key.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(tanggal.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                }).ToList();
            var regular = (
                from x in db.RekonSaldoVaults.AsEnumerable()
                where (x.actionRekon.Contains("Return")) && (DateTime.Parse(x.dueDate.ToString()).Date == tanggal.Date || DateTime.Parse(x.realDate.ToString()) == tanggal.Date) && (kodePkt == "CCAS" ? x.vaultId.Contains(kodePkt) : x.vaultId == kodePkt)
                && !x.actionRekon.ToLower().Contains("emergency")
                group x by new { x.dueDate, x.fundingSoure, x.timeStampRekon, x.actionRekon, x.statusRekon, x.blogMessage, x.realDate } into z
                select new
                {
                    z.Key.realDate,
                    timeStampRekon = z.Key.timeStampRekon,
                    actionRekon = z.Key.actionRekon,
                    statusRekon = z.Key.statusRekon,
                    blogMessage = z.Key.blogMessage,
                    dueDate = z.Key.dueDate,
                    fundingSoure = z.Key.fundingSoure,
                    regular = z.Sum(x => x.currencyAmmount),
                    //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.timeStampRekon.ToString()).Hour < 21 ? x.timeStampRekon : DateTime.Parse(x.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                    validation = z.Key.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(z.Key.timeStampRekon.ToString()).Hour < 21 ? z.Key.timeStampRekon : DateTime.Parse(z.Key.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(tanggal.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                }).ToList();

            var prepareL = (from x in regular
                            join y in emergency on new { x.timeStampRekon, x.dueDate, x.fundingSoure, x.realDate } equals new { y.timeStampRekon, y.dueDate, y.fundingSoure, y.realDate } into temp
                            from y in temp.DefaultIfEmpty()
                            select new
                            {
                                x.realDate,
                                timeStampRekon = x.timeStampRekon,
                                actionRekon = x.actionRekon,
                                statusRekon = x.statusRekon,
                                blogMessage = x.blogMessage,
                                dueDate = x.dueDate,
                                fundingSoure = x.fundingSoure,
                                regular = x.regular,
                                emergency = y == null ? 0 : y.emergency,
                                //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.timeStampRekon.ToString()).Hour < 21 ? x.timeStampRekon : DateTime.Parse(x.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                                validation = x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.timeStampRekon.ToString()).Hour < 21 ? x.timeStampRekon : DateTime.Parse(x.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(tanggal.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                            }).ToList();
            var prepareR = (from x in emergency
                            join y in regular on new { x.timeStampRekon, x.dueDate, x.fundingSoure, x.realDate } equals new { y.timeStampRekon, y.dueDate, y.fundingSoure, y.realDate } into temp
                            from y in temp.DefaultIfEmpty()
                            select new
                            {
                                x.realDate,
                                timeStampRekon = x.timeStampRekon,
                                actionRekon = x.actionRekon,
                                statusRekon = x.statusRekon,
                                blogMessage = x.blogMessage,
                                dueDate = x.dueDate,
                                fundingSoure = x.fundingSoure,
                                regular = y == null ? 0 : y.regular,
                                emergency = x.emergency,
                                //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.timeStampRekon.ToString()).Hour < 21 ? x.timeStampRekon : DateTime.Parse(x.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                                validation = x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.timeStampRekon.ToString()).Hour < 21 ? x.timeStampRekon : DateTime.Parse(x.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(tanggal.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                            }).ToList();
            var prepare = prepareL.Union(prepareR);
            var query = (from x in prepare
                         where x.actionRekon.Contains("Return") && x.validation.Equals("VALIDATED")
                         group x by new { x.dueDate, x.fundingSoure, x.timeStampRekon, x.realDate } into z
                         select new { dueDate = z.Key.dueDate, timeStampRekon = z.Key.timeStampRekon, fundingSource = z.Key.fundingSoure, regular = z.Sum(x => x.regular), emergency = z.Sum(x => x.emergency), total = z.Sum(x => x.emergency + x.regular), z.Key.realDate, vendor = kodePkt }).ToList();
            List<PivotCPC_BIBL> ret = query
                         .GroupBy(x => new { x.vendor, x.dueDate, x.timeStampRekon, x.realDate,x.fundingSource })
                         .Select(x => new PivotCPC_BIBL()
                         {
                             fundingSource = x.Key.fundingSource,
                             plannedReturnNotVal = 0,
                             plannedReturnVal = (Int64)x.Sum(z => z.regular + z.emergency),
                             dueDate = (DateTime)x.Key.dueDate,
                             realDate = (DateTime)x.Key.realDate,
                             grandTotal = (Int64)x.Sum(z => z.regular + z.emergency),
                             vaultId = x.Key.vendor,
                         }).ToList();
            return ret;
        }
        public List<PivotCPC_BIBLD> reloadDeliveryBIBankLainSudah()
        {
            DateTime date2 = tanggal.Date;
            var prequery = (
                from x in db.RekonSaldoVaults.AsEnumerable()
                join y in db.Pkts on x.vaultId.Substring(0, 4) equals y.kodePktCabang == "CCASA" ? "CCAS" : y.kodePktCabang
                where (kodePkt == "CCAS" ? x.vaultId.Contains(kodePkt) : x.vaultId == kodePkt)
                && (!y.kanwil.ToLower().Contains("jabo") ? (x.fundingSoure != null ? x.fundingSoure.ToLower().Contains("OB") || x.fundingSoure.ToLower().Contains("BI") : false) : true)
                select x
            ).ToList();
            var emergency = (
                from x in prequery
                where (x.actionRekon.Contains("Delivery") && x.statusRekon.Equals("Confirmed")) && (DateTime.Parse(x.dueDate.ToString()).Date == tanggal.Date || DateTime.Parse(x.realDate.ToString()) == tanggal.Date) && (kodePkt == "CCAS" ? x.vaultId.Contains(kodePkt) : x.vaultId == kodePkt)
                && x.actionRekon.ToLower().Contains("emergency")
                group x by new { x.dueDate, x.fundingSoure, x.timeStampRekon, x.actionRekon, x.statusRekon, x.blogMessage, x.realDate } into z
                select new
                {
                    z.Key.realDate,
                    timeStampRekon = z.Key.timeStampRekon,
                    actionRekon = z.Key.actionRekon,
                    statusRekon = z.Key.statusRekon,
                    blogMessage = z.Key.blogMessage,
                    dueDate = z.Key.dueDate,
                    fundingSoure = z.Key.fundingSoure,
                    emergency = z.Sum(x => x.currencyAmmount),
                    //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.timeStampRekon.ToString()).Hour < 21 ? x.timeStampRekon : DateTime.Parse(x.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                    validation = z.Key.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(z.Key.timeStampRekon.ToString()).Hour < 21 ? z.Key.timeStampRekon : DateTime.Parse(z.Key.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(tanggal.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                }).ToList();
            var regular = (
                from x in db.RekonSaldoVaults.AsEnumerable()
                where (x.actionRekon.Contains("Delivery") && x.statusRekon.Equals("Confirmed")) && (DateTime.Parse(x.dueDate.ToString()).Date == tanggal.Date || DateTime.Parse(x.realDate.ToString()) == tanggal.Date) && (kodePkt == "CCAS" ? x.vaultId.Contains(kodePkt) : x.vaultId == kodePkt)
                && !x.actionRekon.ToLower().Contains("emergency")
                group x by new { x.dueDate, x.fundingSoure, x.timeStampRekon, x.actionRekon, x.statusRekon, x.blogMessage, x.realDate } into z
                select new
                {
                    z.Key.realDate,
                    timeStampRekon = z.Key.timeStampRekon,
                    actionRekon = z.Key.actionRekon,
                    statusRekon = z.Key.statusRekon,
                    blogMessage = z.Key.blogMessage,
                    dueDate = z.Key.dueDate,
                    fundingSoure = z.Key.fundingSoure,
                    regular = z.Sum(x => x.currencyAmmount),
                    //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.timeStampRekon.ToString()).Hour < 21 ? x.timeStampRekon : DateTime.Parse(x.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                    validation = z.Key.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(z.Key.timeStampRekon.ToString()).Hour < 21 ? z.Key.timeStampRekon : DateTime.Parse(z.Key.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(tanggal.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                }).ToList();

            var prepareL = (from x in regular
                            join y in emergency on new { x.timeStampRekon, x.dueDate, x.fundingSoure, x.realDate } equals new { y.timeStampRekon, y.dueDate, y.fundingSoure, y.realDate } into temp
                            from y in temp.DefaultIfEmpty()
                            select new
                            {
                                x.realDate,
                                timeStampRekon = x.timeStampRekon,
                                actionRekon = x.actionRekon,
                                statusRekon = x.statusRekon,
                                blogMessage = x.blogMessage,
                                dueDate = x.dueDate,
                                fundingSoure = x.fundingSoure,
                                regular = x.regular,
                                emergency = y == null ? 0 : y.emergency,
                                //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.timeStampRekon.ToString()).Hour < 21 ? x.timeStampRekon : DateTime.Parse(x.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                                validation = x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.timeStampRekon.ToString()).Hour < 21 ? x.timeStampRekon : DateTime.Parse(x.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(tanggal.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                            }).ToList();
            var prepareR = (from x in emergency
                            join y in regular on new { x.timeStampRekon, x.dueDate, x.fundingSoure, x.realDate } equals new { y.timeStampRekon, y.dueDate, y.fundingSoure, y.realDate } into temp
                            from y in temp.DefaultIfEmpty()
                            select new
                            {
                                x.realDate,
                                timeStampRekon = x.timeStampRekon,
                                actionRekon = x.actionRekon,
                                statusRekon = x.statusRekon,
                                blogMessage = x.blogMessage,
                                dueDate = x.dueDate,
                                fundingSoure = x.fundingSoure,
                                regular = y == null ? 0 : y.regular,
                                emergency = x.emergency,
                                //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.timeStampRekon.ToString()).Hour < 21 ? x.timeStampRekon : DateTime.Parse(x.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                                validation = x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.timeStampRekon.ToString()).Hour < 21 ? x.timeStampRekon : DateTime.Parse(x.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(tanggal.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                            }).ToList();
            var prepare = prepareL.Union(prepareR);
            var query = (from x in prepare
                         where x.actionRekon.Contains("Delivery") && x.validation.Equals("VALIDATED") && x.statusRekon.Equals("Confirmed")
                         group x by new { x.dueDate, x.fundingSoure, x.timeStampRekon, x.realDate } into z
                         select new { dueDate = z.Key.dueDate, timeStampRekon = z.Key.timeStampRekon, fundingSource = z.Key.fundingSoure, regular = z.Sum(x => x.regular), emergency = z.Sum(x => x.emergency), total = z.Sum(x => x.emergency + x.regular), z.Key.realDate, vendor = kodePkt }).ToList();
            List<PivotCPC_BIBLD> ret = query
              .GroupBy(x => new { x.vendor, x.dueDate, x.timeStampRekon, x.realDate, x.fundingSource })
              .Select(x => new PivotCPC_BIBLD()
              {
                  fundingSource = x.Key.fundingSource,
                  plannedDeliveryNotVal = (Int64)x.Sum(z => z.regular + z.emergency),
                  plannedDeliveryVal = 0,
                  dueDate = (DateTime)x.Key.dueDate,
                  realDate = (DateTime)x.Key.realDate,
                  grandTotal = (Int64)x.Sum(z => z.regular + z.emergency),
                  vaultId = x.Key.vendor,
              }).ToList();
            return ret;
        }
        public List<PivotCPC_BIBLD> reloadDeliveryBIBankLainBelum()
        {

            DateTime date2 = tanggal.Date;
            var prequery = (
                from x in db.RekonSaldoVaults.AsEnumerable()
                join y in db.Pkts on x.vaultId.Substring(0, 4) equals y.kodePktCabang == "CCASA" ? "CCAS" : y.kodePktCabang
                where (kodePkt == "CCAS" ? x.vaultId.Contains(kodePkt) : x.vaultId == kodePkt)
                && (!y.kanwil.ToLower().Contains("jabo") ? (x.fundingSoure != null ? x.fundingSoure.ToLower().Contains("OB") || x.fundingSoure.ToLower().Contains("BI") : false) : true)
                select x
            ).ToList();
            var emergency = (
              from x in prequery
              where (x.actionRekon.Contains("Delivery") && x.statusRekon.Equals("Confirmed")) && (DateTime.Parse(x.dueDate.ToString()).Date == tanggal.Date && DateTime.Parse(x.realDate.ToString()) > tanggal.Date) && (kodePkt == "CCAS" ? x.vaultId.Contains(kodePkt) : x.vaultId == kodePkt)
              && x.actionRekon.ToLower().Contains("emergency")
              group x by new { x.dueDate, x.fundingSoure, x.timeStampRekon, x.actionRekon, x.statusRekon, x.blogMessage, x.realDate } into z
              select new
              {
                  z.Key.realDate,
                  timeStampRekon = z.Key.timeStampRekon,
                  actionRekon = z.Key.actionRekon,
                  statusRekon = z.Key.statusRekon,
                  blogMessage = z.Key.blogMessage,
                  dueDate = z.Key.dueDate,
                  fundingSoure = z.Key.fundingSoure,
                  emergency = z.Sum(x => x.currencyAmmount),
                  //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.timeStampRekon.ToString()).Hour < 21 ? x.timeStampRekon : DateTime.Parse(x.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                  validation = z.Key.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(z.Key.timeStampRekon.ToString()).Hour < 21 ? z.Key.timeStampRekon : DateTime.Parse(z.Key.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(tanggal.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
              }).ToList();
            var regular = (
                from x in db.RekonSaldoVaults.AsEnumerable()
                where (x.actionRekon.Contains("Delivery") && x.statusRekon.Equals("Confirmed")) && (DateTime.Parse(x.dueDate.ToString()).Date == tanggal.Date && DateTime.Parse(x.realDate.ToString()) > tanggal.Date) && (kodePkt == "CCAS" ? x.vaultId.Contains(kodePkt) : x.vaultId == kodePkt)
                && !x.actionRekon.ToLower().Contains("emergency")
                group x by new { x.dueDate, x.fundingSoure, x.timeStampRekon, x.actionRekon, x.statusRekon, x.blogMessage, x.realDate } into z
                select new
                {
                    z.Key.realDate,
                    timeStampRekon = z.Key.timeStampRekon,
                    actionRekon = z.Key.actionRekon,
                    statusRekon = z.Key.statusRekon,
                    blogMessage = z.Key.blogMessage,
                    dueDate = z.Key.dueDate,
                    fundingSoure = z.Key.fundingSoure,
                    regular = z.Sum(x => x.currencyAmmount),
                    //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.timeStampRekon.ToString()).Hour < 21 ? x.timeStampRekon : DateTime.Parse(x.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                    validation = z.Key.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(z.Key.timeStampRekon.ToString()).Hour < 21 ? z.Key.timeStampRekon : DateTime.Parse(z.Key.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(tanggal.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                }).ToList();

            var prepareL = (from x in regular
                            join y in emergency on new { x.timeStampRekon, x.dueDate, x.fundingSoure, x.realDate } equals new { y.timeStampRekon, y.dueDate, y.fundingSoure, y.realDate } into temp
                            from y in temp.DefaultIfEmpty()
                            select new
                            {
                                x.realDate,
                                timeStampRekon = x.timeStampRekon,
                                actionRekon = x.actionRekon,
                                statusRekon = x.statusRekon,
                                blogMessage = x.blogMessage,
                                dueDate = x.dueDate,
                                fundingSoure = x.fundingSoure,
                                regular = x.regular,
                                emergency = y == null ? 0 : y.emergency,
                                //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.timeStampRekon.ToString()).Hour < 21 ? x.timeStampRekon : DateTime.Parse(x.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                                validation = x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.timeStampRekon.ToString()).Hour < 21 ? x.timeStampRekon : DateTime.Parse(x.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(tanggal.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                            }).ToList();
            var prepareR = (from x in emergency
                            join y in regular on new { x.timeStampRekon, x.dueDate, x.fundingSoure, x.realDate } equals new { y.timeStampRekon, y.dueDate, y.fundingSoure, y.realDate } into temp
                            from y in temp.DefaultIfEmpty()
                            select new
                            {
                                x.realDate,
                                timeStampRekon = x.timeStampRekon,
                                actionRekon = x.actionRekon,
                                statusRekon = x.statusRekon,
                                blogMessage = x.blogMessage,
                                dueDate = x.dueDate,
                                fundingSoure = x.fundingSoure,
                                regular = y == null ? 0 : y.regular,
                                emergency = x.emergency,
                                //x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.timeStampRekon.ToString()).Hour < 21 ? x.timeStampRekon : DateTime.Parse(x.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(x.dueDate.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                                validation = x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.timeStampRekon.ToString()).Hour < 21 ? x.timeStampRekon : DateTime.Parse(x.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= DateTime.Parse(tanggal.ToString()).Date ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                            }).ToList();
            var prepare = prepareL.Union(prepareR);
            var query = (from x in prepare
                         where x.actionRekon.Contains("Delivery") && x.validation.Equals("NOT VALIDATED") && x.statusRekon.Equals("Confirmed")
                         group x by new { x.dueDate, x.fundingSoure, x.timeStampRekon, x.realDate } into z
                         select new { dueDate = z.Key.dueDate, timeStampRekon = z.Key.timeStampRekon, fundingSource = z.Key.fundingSoure, regular = z.Sum(x => x.regular), emergency = z.Sum(x => x.emergency), total = z.Sum(x => x.emergency + x.regular), z.Key.realDate, vendor = kodePkt }).ToList();
            List<PivotCPC_BIBLD> ret = query
              .GroupBy(x => new { x.vendor, x.dueDate, x.timeStampRekon, x.realDate,x.fundingSource })
              .Select(x => new PivotCPC_BIBLD()
              {
                  fundingSource = x.Key.fundingSource,
                  plannedDeliveryNotVal = 0,
                  plannedDeliveryVal = (Int64)x.Sum(z => z.regular + z.emergency),
                  dueDate = (DateTime)x.Key.dueDate,
                  realDate = (DateTime)x.Key.realDate,
                  grandTotal = (Int64)x.Sum(z => z.regular + z.emergency),
                  vaultId = x.Key.vendor,
              }).ToList();
            return ret;
        }


        //IN
        public void loadListReturnBIdanBankLain()
        {
            //listReturnBIdanBankLain = new List<PivotCPC_BIBL>();
            //Database1Entities db = new Database1Entities();

            //var query = (from x in db.RekonSaldoVaults.AsEnumerable()
            //             where !String.IsNullOrEmpty(x.fundingSoure)
            //             && kodePkt == "CCAS" ? x.vaultId.Contains(kodePkt) : x.vaultId == kodePkt
            //             select x).ToList();
            //query = (from x in query
            //         where (x.fundingSoure == null ? false : (x.fundingSoure == "BI" || x.fundingSoure.Contains("OB"))) 
            //         && (((DateTime)x.dueDate).Date == tanggal || ((DateTime)x.realDate).Date == tanggal)
            //         select x).ToList();

            //List<PivotCPC> pc = new List<PivotCPC>();

            //String bufferVaultId = "";

            //foreach (var item in query)
            //{
            //    pc.Add(new PivotCPC
            //    {
            //        vaultId = item.vaultId,
            //        confId = item.confId,
            //        action = item.actionRekon,
            //        status = item.statusRekon,
            //        blogMessage = item.blogMessage,
            //        orderDate = ((DateTime)item.orderDate).Date,
            //        dueDate = ((DateTime)item.dueDate).Date,
            //        timeStamp = (DateTime)item.timeStampRekon,
            //        currencyAmmount = Int64.Parse(item.currencyAmmount.ToString()),
            //        fundingSource = item.fundingSoure,
            //        realDate = ((DateTime)item.timeStampRekon).Hour < 21 ? ((DateTime)item.timeStampRekon).Date : ((DateTime)item.timeStampRekon).AddDays(1).Date,
            //        validation = item.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(item.timeStampRekon.ToString()).Hour < 21 ? item.timeStampRekon : DateTime.Parse(item.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= tanggal ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
            //    });

            //}
            //var pivot = pc.GroupBy(c => new { c.vaultId, c.fundingSource, c.dueDate, c.realDate }).Select(g => new
            //{
            //    dueDate = g.Key.dueDate,
            //    realDate = g.Key.realDate,
            //    vaultID = g.Key.vaultId,
            //    fundingSource = g.Key.fundingSource,
            //    emergencyReturn = g.Where(c => c.action == "Emergency Return").Sum(c => c.currencyAmmount),
            //    emergencyReturnVal = g.Where(c => c.action == "Emergency Return" && c.validation == "VALIDATED").Sum(c => c.currencyAmmount),
            //    emergencyReturnNotVal = g.Where(c => c.action == "Emergency Return" && c.validation == "NOT VALIDATED").Sum(c => c.currencyAmmount),
            //    plannedReturn = g.Where(c => c.action == "Planned Return").Sum(c => c.currencyAmmount),
            //    plannedReturnVal = g.Where(c => c.action == "Planned Return" && c.validation == "VALIDATED").Sum(c => c.currencyAmmount),
            //    plannedReturnNotVal = g.Where(c => c.action == "Planned Return" && c.validation == "NOT VALIDATED").Sum(c => c.currencyAmmount)

            //}).ToList();
            //var pivot2 = (from x in pivot
            //              select new { x.dueDate, x.vaultID, x.fundingSource, x.emergencyReturn, x.emergencyReturnVal, x.emergencyReturnNotVal, x.plannedReturn, x.plannedReturnVal, x.plannedReturnNotVal, grandTotal = x.emergencyReturn + x.plannedReturn, x.realDate });

            //var bibl = new List<PivotCPC_BIBL>();

            //foreach (var item in pivot2)
            //{
            //    if (!(item.plannedReturn == 0 && item.emergencyReturn == 0))
            //    {
            //        bibl.Add(new PivotCPC_BIBL
            //        {
            //            dueDate = item.dueDate,
            //            realDate = item.realDate,
            //            vaultId = item.vaultID,
            //            fundingSource = item.fundingSource,
            //            //emergencyReturn = item.emergencyReturn,
            //            emergencyReturnVal = item.emergencyReturnVal,
            //            emergencyReturnNotVal = item.emergencyReturnNotVal,
            //            //plannedReturn = item.plannedReturn,
            //            plannedReturnVal = item.plannedReturnVal,
            //            plannedReturnNotVal = item.plannedReturnNotVal,
            //            grandTotal = item.grandTotal
            //        });
            //    }

            //}
            List<PivotCPC_BIBL> tempSudah = reloadReturnBIBankLainSudah();
            List<PivotCPC_BIBL> tempBelum = reloadReturnBIBankLainBelum();
            List<PivotCPC_BIBL> bibl = tempBelum.Union(tempSudah).ToList();
            listReturnBIdanBankLain = bibl;
        }
        public void loadListATMReturn()
        {
            listATMReturn = new List<PivotCPC_ATMR>();
            Database1Entities db = new Database1Entities();
            //preparing data for pivot - atm return
            var queryar = (from x in db.RekonSaldoVaults.AsEnumerable()
                           join y in db.Pkts.AsEnumerable() on x.vaultId equals y.kodePktCabang == "CCASA" ? "CCAS" : y.kodePktCabang
                           where !String.IsNullOrEmpty(x.fundingSoure) && y.kanwil.Like("Jabotabek")
                           select x).ToList();
            queryar = (from x in queryar
                       where (x.fundingSoure != "BI" && !x.fundingSoure.Contains("OB")) && (((DateTime)x.dueDate).Date == tanggal || ((DateTime)x.realDate).Date == tanggal)
                       select x).ToList();

            var pc = new List<PivotCPC>();

            String bufferVaultId4 = "";

            foreach (var item in queryar)
            {
                pc.Add(new PivotCPC
                {
                    vaultId = item.vaultId,
                    confId = item.confId,
                    action = item.actionRekon,
                    blogMessage = item.blogMessage,
                    status = item.statusRekon,
                    orderDate = ((DateTime)item.orderDate).Date,
                    dueDate = ((DateTime)item.dueDate).Date,
                    timeStamp = (DateTime)item.timeStampRekon,
                    currencyAmmount = Int64.Parse(item.currencyAmmount.ToString()),
                    fundingSource = item.fundingSoure,
                    realDate = ((DateTime)item.timeStampRekon).Hour < 21 ? ((DateTime)item.timeStampRekon).Date : ((DateTime)item.timeStampRekon).AddDays(1).Date,
                    validation = item.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(item.timeStampRekon.ToString()).Hour < 21 ? item.timeStampRekon : DateTime.Parse(item.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= tanggal ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                });

            }

            //creating pivotCPC - atm return
            var pivotar = pc.GroupBy(c => new { c.vaultId, c.fundingSource, c.dueDate, c.realDate }).Select(g => new
            {
                dueDate = g.Key.dueDate,
                realDate = g.Key.realDate,
                vaultID = g.Key.vaultId,
                fundingSource = g.Key.fundingSource,
                emergencyReturn = g.Where(c => c.action == "Emergency Return").Sum(c => c.currencyAmmount),
                emergencyReturnVal = g.Where(c => c.action == "Emergency Return" && c.validation == "VALIDATED").Sum(c => c.currencyAmmount),
                emergencyReturnNotVal = g.Where(c => c.action == "Emergency Return" && c.validation == "NOT VALIDATED").Sum(c => c.currencyAmmount),
                plannedReturn = g.Where(c => c.action == "Planned Return").Sum(c => c.currencyAmmount),
                plannedReturnVal = g.Where(c => c.action == "Planned Return" && c.validation == "VALIDATED").Sum(c => c.currencyAmmount),
                plannedReturnNotVal = g.Where(c => c.action == "Planned Return" && c.validation == "NOT VALIDATED").Sum(c => c.currencyAmmount)
            }).ToList();

            var pivotar2 = (from x in pivotar
                            select new { x.dueDate, x.vaultID, x.realDate, x.fundingSource, x.emergencyReturn, x.plannedReturn, grandTotal = x.emergencyReturn + x.plannedReturn, x.plannedReturnVal, x.plannedReturnNotVal, x.emergencyReturnVal, x.emergencyReturnNotVal });

            var atmr = new List<PivotCPC_ATMR>();

            foreach (var item in pivotar2)
            {
                if (!(item.plannedReturn == 0 && item.emergencyReturn == 0))
                {
                    atmr.Add(new PivotCPC_ATMR
                    {
                        dueDate = item.dueDate,
                        realDate = item.realDate,
                        vaultId = item.vaultID,
                        fundingSource = item.fundingSource,
                        // emergencyReturn = item.emergencyReturn,
                        emergencyReturnVal = item.emergencyReturnVal,
                        emergencyReturnNotVal = item.emergencyReturnNotVal,
                        // plannedReturn = item.plannedReturn,
                        plannedReturnVal = item.plannedReturnVal,
                        plannedReturnNotVal = item.plannedReturnNotVal,
                        grandTotal = item.grandTotal
                    });
                }

            }

            listATMReturn = atmr;
        }

    
        public void loadListSetoranCabang()
        {
            List<PivotPerVendor_setoran> tempSudah = reloadSetoranSudah();
            List<PivotPerVendor_setoran> tempBelum = reloadSetoranBelum();
            List<PivotPerVendor_setoran> ppvs = tempBelum.Union(tempSudah).ToList();
            listSetoranCabang = ppvs;
        }
        //OUT
        public void loadListDeliveryBIdanBankLain()
        {
            //listDeliveryBIdanBankLain = new List<PivotCPC_BIBLD>();
            //Database1Entities db = new Database1Entities();
            ////preparing data for Pivot CPC - BI dan BankLain delivery
            //var queryd = (from x in db.RekonSaldoVaults.AsEnumerable()
            //              join y in db.Pkts.AsEnumerable() on x.vaultId equals y.kodePktCabang == "CCASA" ? "CCAS" : y.kodePktCabang
            //              where !String.IsNullOrEmpty(x.fundingSoure) && y.kanwil.Like("Jabotabek")
            //              select x).ToList();
            //queryd = (from x in queryd
            //          where (x.fundingSoure == "BI" || x.fundingSoure.Contains("OB")) && (((DateTime)x.dueDate).Date == tanggal || ((DateTime)x.realDate).Date == tanggal)
            //          select x).ToList();

            //var pc = new List<PivotCPC>();

            //foreach (var item in queryd)
            //{
            //    pc.Add(new PivotCPC
            //    {
            //        vaultId = item.vaultId,
            //        confId = item.confId,
            //        action = item.actionRekon,
            //        status = item.statusRekon,
            //        blogMessage = item.blogMessage,
            //        orderDate = ((DateTime)item.orderDate).Date,
            //        dueDate = ((DateTime)item.dueDate).Date,
            //        timeStamp = (DateTime)item.timeStampRekon,
            //        currencyAmmount = Int64.Parse(item.currencyAmmount.ToString()),
            //        fundingSource = item.fundingSoure,
            //        realDate = ((DateTime)item.timeStampRekon).Hour < 21 ? ((DateTime)item.timeStampRekon).Date : ((DateTime)item.timeStampRekon).AddDays(1).Date,
            //        validation = item.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(item.timeStampRekon.ToString()).Hour < 21 ? item.timeStampRekon : DateTime.Parse(item.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= tanggal ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
            //    });

            //}

            ////creating pivotCPC - BI dan BankLain Delivery
            //var pivotd = pc.GroupBy(c => new { c.vaultId, c.fundingSource, c.dueDate, c.realDate }).Select(g => new
            //{
            //    dueDate = g.Key.dueDate,
            //    realDate = g.Key.realDate,
            //    vaultID = g.Key.vaultId,
            //    fundingSource = g.Key.fundingSource,
            //    emergencyDelivery = g.Where(c => c.action == "Emergency Delivery").Sum(c => c.currencyAmmount),
            //    emergencyDeliveryVal = g.Where(c => c.action == "Emergency Delivery" && c.validation == "VALIDATED").Sum(c => c.currencyAmmount),
            //    emergencyDeliveryNotVal = g.Where(c => c.action == "Emergency Delivery" && c.validation == "NOT VALIDATED").Sum(c => c.currencyAmmount),
            //    plannedDelivery = g.Where(c => c.action == "Planned Delivery").Sum(c => c.currencyAmmount),
            //    plannedDeliveryVal = g.Where(c => c.action == "Planned Delivery" && c.validation == "VALIDATED").Sum(c => c.currencyAmmount),
            //    plannedDeliveryNotVal = g.Where(c => c.action == "Planned Delivery" && c.validation == "NOT VALIDATED").Sum(c => c.currencyAmmount)
            //}).ToList();

            //var pivotd2 = (from x in pivotd
            //               select new { x.dueDate, x.vaultID, x.fundingSource, x.emergencyDelivery, x.plannedDelivery, grandTotal = x.emergencyDelivery + x.plannedDelivery, x.plannedDeliveryVal, x.plannedDeliveryNotVal, x.emergencyDeliveryNotVal, x.emergencyDeliveryVal, x.realDate });

            //var bibld = new List<PivotCPC_BIBLD>();

            //foreach (var item in pivotd2)
            //{
            //    if (!(item.emergencyDelivery == 0 && item.plannedDelivery == 0))
            //    {
            //        bibld.Add(new PivotCPC_BIBLD
            //        {
            //            dueDate = item.dueDate,
            //            realDate = item.realDate,
            //            vaultId = item.vaultID,
            //            fundingSource = item.fundingSource,
            //            // emergencyDelivery = item.emergencyDelivery,
            //            emergencyDeliveryVal = item.emergencyDeliveryVal,
            //            emergencyDeliveryNotVal = item.emergencyDeliveryNotVal,
            //            //  plannedDelivery = item.plannedDelivery,
            //            plannedDeliveryVal = item.plannedDeliveryVal,
            //            plannedDeliveryNotVal = item.plannedDeliveryNotVal,
            //            grandTotal = item.grandTotal
            //        });
            //    }

            //}
            List<PivotCPC_BIBLD> tempSudah = reloadDeliveryBIBankLainSudah();
            List<PivotCPC_BIBLD> tempBelum = reloadDeliveryBIBankLainBelum();
            List<PivotCPC_BIBLD> bibld = tempSudah.Union(tempBelum).ToList();

            listDeliveryBIdanBankLain = bibld;
        }
        public void loadListATMDelivery()
        {
            Database1Entities db = new Database1Entities();
            //preparing data for pivot - ATM delivery
            var queryad = (from x in db.RekonSaldoVaults.AsEnumerable()
                           join y in db.Pkts.AsEnumerable() on x.vaultId equals y.kodePktCabang == "CCASA" ? "CCAS" : y.kodePktCabang 
                           where !String.IsNullOrEmpty(x.fundingSoure) && y.kanwil.Like("Jabotabek")
                           select x).ToList();
            queryad = (from x in queryad
                       where (x.fundingSoure != "BI" && !x.fundingSoure.Contains("OB")) && (((DateTime)x.dueDate).Date == tanggal || ((DateTime)x.realDate).Date == tanggal)
                       select x).ToList();

            var pc = new List<PivotCPC>();

            String bufferVaultId3 = "";

            foreach (var item in queryad)
            {
                pc.Add(new PivotCPC
                {
                    vaultId = item.vaultId,
                    confId = item.confId,
                    action = item.actionRekon,
                    blogMessage = item.blogMessage,
                    status = item.statusRekon,
                    orderDate = ((DateTime)item.orderDate).Date,
                    dueDate = ((DateTime)item.dueDate).Date,
                    timeStamp = (DateTime)item.timeStampRekon,
                    currencyAmmount = Int64.Parse(item.currencyAmmount.ToString()),
                    fundingSource = item.fundingSoure,
                    realDate = ((DateTime)item.timeStampRekon).Hour < 21 ? ((DateTime)item.timeStampRekon).Date : ((DateTime)item.timeStampRekon).AddDays(1).Date,
                    validation = item.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(item.timeStampRekon.ToString()).Hour < 21 ? item.timeStampRekon : DateTime.Parse(item.timeStampRekon.ToString()).AddDays(1)).ToString()).Date <= tanggal ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
                });

            }

            //creating pivot - atm delivery
            var pivotad = pc.GroupBy(c => new { c.vaultId, c.fundingSource, c.dueDate, c.realDate }).Select(g => new
            {
                dueDate = g.Key.dueDate,
                realDate = g.Key.realDate,
                vaultID = g.Key.vaultId,
                fundingSource = g.Key.fundingSource,
                emergencyDelivery = g.Where(c => c.action == "Emergency Delivery").Sum(c => c.currencyAmmount),
                emergencyDeliveryVal = g.Where(c => c.action == "Emergency Delivery" && c.validation == "VALIDATED").Sum(c => c.currencyAmmount),
                emergencyDeliveryNotVal = g.Where(c => c.action == "Emergency Delivery" && c.validation == "NOT VALIDATED").Sum(c => c.currencyAmmount),
                plannedDelivery = g.Where(c => c.action == "Planned Delivery").Sum(c => c.currencyAmmount),
                plannedDeliveryVal = g.Where(c => c.action == "Planned Delivery" && c.validation == "VALIDATED").Sum(c => c.currencyAmmount),
                plannedDeliveryNotVal = g.Where(c => c.action == "Planned Delivery" && c.validation == "NOT VALIDATED").Sum(c => c.currencyAmmount)
            }).ToList();

            var pivotad2 = (from x in pivotad
                            select new { x.dueDate, x.vaultID, x.fundingSource, x.emergencyDelivery, x.plannedDelivery, grandTotal = x.emergencyDelivery + x.plannedDelivery, x.plannedDeliveryNotVal, x.plannedDeliveryVal, x.emergencyDeliveryNotVal, x.emergencyDeliveryVal, x.realDate });

            var atmd = new List<PivotCPC_ATMD>();

            foreach (var item in pivotad2)
            {
                if (!(item.emergencyDelivery == 0 && item.plannedDelivery == 0))
                {
                    atmd.Add(new PivotCPC_ATMD
                    {
                        dueDate = item.dueDate,
                        realDate = item.realDate,
                        vaultId = item.vaultID,
                        fundingSource = item.fundingSource,
                        //  emergencyDelivery = item.emergencyDelivery,
                        emergencyDeliveryVal = item.emergencyDeliveryVal,
                        emergencyDeliveryNotVal = item.emergencyDeliveryNotVal,
                        //  plannedDelivery = item.plannedDelivery,
                        plannedDeliveryVal = item.plannedDeliveryVal,
                        plannedDeliveryNotVal = item.plannedDeliveryNotVal,
                        grandTotal = item.grandTotal
                    });
                }

            }

            listATMDelivery = atmd;

        }
        public void loadListBonCabang()
        {
            //listBonCabang = new List<PivotPerVendor_bon>();
            //Database1Entities db = new Database1Entities();
            //var queryS = (from x in db.RekonSaldoPerVendors.AsEnumerable()
            //              join y in db.Pkts.AsEnumerable() on x.vendor equals y.kodePktCabang
            //              where !String.IsNullOrEmpty(x.vendor) && y.kanwil.Like("Jabotabek")
            //              select x).ToList();
            //var query = (from x in queryS
            //             join y in db.Pkts.AsEnumerable() on x.vendor equals y.kodePktCabang
            //             where (!String.IsNullOrEmpty(x.vendor) && y.kanwil.Like("Jabotabek") && x.actionRekon.Contains("Delivery") && x.statusRekon.Equals("Confirmed")) && (((DateTime)x.dueDate).Date == tanggal || ((DateTime)x.realDate).Date == tanggal)
            //             select new
            //             {
            //                 cashPointId = x.cashPointtId,
            //                 confId = x.confId,
            //                 orderDate = x.orderDate,
            //                 vendor = x.vendor,
            //                 actionRekon = x.actionRekon,
            //                 statusRekon = x.statusRekon,
            //                 dueDate = x.dueDate,
            //                 blogTime = x.blogTime,
            //                 currencyAmmount = x.currencyAmmount,
            //                 realDate = x.realDate,
            //                 blogMessage = x.blogMessage,
            //                 validation = x.blogMessage.Contains("GL") ? (DateTime.Parse((DateTime.Parse(x.blogTime.ToString()).Hour < 21 ? x.blogTime : DateTime.Parse(x.blogTime.ToString()).AddDays(1)).ToString()).Date <= tanggal ? "VALIDATED" : "NOT VALIDATED") : "NOT VALIDATED"
            //             }).ToList();

            ////generating pivotbon
            //var ppvb = new List<PivotPerVendor_bon>();

            //var pivot = query.GroupBy(c => new { c.vendor, c.dueDate, c.realDate }).Select(g => new
            //{
            //    dueDate = g.Key.dueDate,
            //    valDate = g.Key.realDate,
            //    vendor = g.Key.vendor,
            //    belumValidasi = g.Where(c => c.validation == "NOT VALIDATED").Sum(c => c.currencyAmmount),
            //    sudahValidasi = g.Where(c => c.validation == "VALIDATED").Sum(c => c.currencyAmmount),
            //    grandTotal = g.Where(c => c.validation == "NOT VALIDATED").Sum(c => c.currencyAmmount) +
            //                 g.Where(c => c.validation == "VALIDATED").Sum(c => c.currencyAmmount)

            //}).ToList();

            //Console.WriteLine(pivot.Count);

            //foreach (var item in pivot)
            //{
            //    ppvb.Add(new PivotPerVendor_bon
            //    {
            //        dueDate = ((DateTime)item.dueDate).Date,
            //        valDate = ((DateTime)item.valDate).Date,
            //        vendor = item.vendor,
            //        belumValidasi = (Int64)item.belumValidasi,
            //        sudahValidasi = (Int64)item.sudahValidasi,
            //        grandTotal = (Int64)item.grandTotal
            //    });
            //}
            List<PivotPerVendor_bon> tempSudah = reloadBonSudah();
            List<PivotPerVendor_bon> tempBelum = reloadBonBelum();
            List<PivotPerVendor_bon> ppvb = tempSudah.Union(tempBelum).ToList();
            listBonCabang = ppvb;
        }
    }
}
