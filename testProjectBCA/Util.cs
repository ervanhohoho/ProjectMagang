using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelDataReader;
using System.IO;
using System.Data;
using System.Globalization;

namespace testProjectBCA
{
    static class Util
    {
        public static FileStream stream;
        static IExcelDataReader reader;
        public static int GetIso8601WeekOfYear(DateTime time)
        {
            // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
        
        public static DataSet openExcel(string filePath)
        {
            try
            {
                using (stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    {

                        // Auto-detect format, supports:
                        //  - Binary Excel files (2.0-2003 format; *.xls)
                        //  - OpenXml Excel files (2007 format; *.xlsx)
                        using (reader = ExcelReaderFactory.CreateReader(stream))
                        {    //return excel as tables
                            return reader.AsDataSet();
                        }
                    }
                }
            }
            catch(Exception E)
            {
                System.Windows.Forms.MessageBox.Show(filePath + " Sedang dibuka");
                return new DataSet();
            }
        }
        public static DataSet openCsv(string filePath)
        {
            using (stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                {

                    // Auto-detect format, supports:
                    //  - Binary Excel files (2.0-2003 format; *.xls)
                    //  - OpenXml Excel files (2007 format; *.xlsx)
                    using (reader = ExcelReaderFactory.CreateCsvReader(stream))
                    {    //return excel as tables
                        return reader.AsDataSet();
                    }
                }
            }
        }
        public static void inputDataTransaksiATMToDB(transaksiPkt temp)
        {
            Database1Entities db = new Database1Entities();
            //Skip data yang udah ada
            var check = (from x in db.TransaksiAtms where x.kodePkt == temp.kodePkt && x.tanggal == temp.tanggalPengajuan select x).FirstOrDefault();
            if (check != null)
                return;

            TransaksiAtm newT = new TransaksiAtm();
            newT.kodePkt = temp.kodePkt;
            newT.tanggal = temp.tanggalPengajuan;
            newT.adhoc100 = temp.penerimaanBonAdhoc[0];
            newT.adhoc50 = temp.penerimaanBonAdhoc[1];
            newT.adhoc20 = temp.penerimaanBonAdhoc[2];
            newT.bon100 = temp.penerimaanBon[0];
            newT.bon50 = temp.penerimaanBon[1];
            newT.bon20 = temp.penerimaanBon[2];
            newT.isiATM100 = temp.pengisianAtm[0];
            newT.isiATM50 = temp.pengisianAtm[1];
            newT.isiATM20 = temp.pengisianAtm[2];
            newT.isiCRM100 = temp.pengisianCrm[0];
            newT.isiCRM50 = temp.pengisianCrm[1];
            newT.isiCRM20 = temp.pengisianCrm[2];
            newT.saldoAwal100 = temp.saldoAwalHitungan[0];
            newT.saldoAwal50 = temp.saldoAwalHitungan[1];
            newT.saldoAwal20 = temp.saldoAwalHitungan[2];
            newT.setor100 = temp.setorUang[0];
            newT.setor50 = temp.setorUang[1];
            newT.setor20 = temp.setorUang[2];
            newT.sislokATM100 = temp.bongkaranAtm[0];
            newT.sislokATM50 = temp.bongkaranAtm[1];
            newT.sislokATM20 = temp.bongkaranAtm[2];
            newT.sislokCDM100 = temp.bongkaranCdm[0];
            newT.sislokCDM50 = temp.bongkaranCdm[1];
            newT.sislokCDM20 = temp.bongkaranCdm[2];
            newT.sislokCRM100 = temp.bongkaranCrm[0];
            newT.sislokCRM50 = temp.bongkaranCrm[1];
            newT.sislokCRM20 = temp.bongkaranCrm[2];
            newT.saldoAkhir100 = temp.saldoAkhirHitungan[0];
            newT.saldoAkhir50 = temp.saldoAkhirHitungan[1];
            newT.saldoAkhir20 = temp.saldoAkhirHitungan[2];
            db.TransaksiAtms.Add(newT);
            db.SaveChanges();
            int counter = 1;
            foreach (var temp2 in temp.bonAtmYangDisetujui)
            {
                var query = (from x in db.laporanBons.AsEnumerable()
                             where ((DateTime)x.tanggal).Date == temp2.tgl.Date
                             && x.kodePkt == temp.kodePkt
                             select x).FirstOrDefault();
                if (query != null)
                {
                    query.C100 = temp2.d100;
                    query.C50 = temp2.d50;
                    query.C20 = temp2.d20;
                }
                else
                {
                    laporanBon newL = new laporanBon();
                    newL.kodePkt = temp.kodePkt;
                    newL.tanggal = temp2.tgl;
                    newL.C100 = temp2.d100;
                    newL.C50 = temp2.d50;
                    newL.C20 = temp2.d20;
                    db.laporanBons.Add(newL);
                }
                db.SaveChanges();
            }
            foreach(var temp2 in temp.permintaanBon)
            {
                var query = (from x in db.LaporanPermintaanBons.AsEnumerable()
                             where ((DateTime)x.tanggal).Date == temp2.tgl.Date
                             && x.kodePkt == temp.kodePkt
                             select x).FirstOrDefault();
                if (query != null)
                {
                    query.C100 = temp2.d100;
                    query.C50 = temp2.d50;
                    query.C20 = temp2.d20;
                }
                else
                {
                    LaporanPermintaanBon newL = new LaporanPermintaanBon();
                    newL.kodePkt = temp.kodePkt;
                    newL.tanggal = temp2.tgl;
                    newL.C100 = temp2.d100;
                    newL.C50 = temp2.d50;
                    newL.C20 = temp2.d20;
                    db.LaporanPermintaanBons.Add(newL);
                }
                db.SaveChanges();
            }
            var qLapPermintaanAdhoc = (from x in db.LaporanPermintaanAdhocs.AsEnumerable()
                                       where x.kodePkt == temp.kodePkt
                                       && x.tanggal == temp.tanggalPengajuan.AddDays(1)
                                       select x).FirstOrDefault();
            if (qLapPermintaanAdhoc != null)
            {
                qLapPermintaanAdhoc.C100 = temp.permintaanAdhoc[0];
                qLapPermintaanAdhoc.C50 = temp.permintaanAdhoc[1];
                qLapPermintaanAdhoc.C20 = temp.permintaanAdhoc[2];
            }
            else
            {
                LaporanPermintaanAdhoc newA = new LaporanPermintaanAdhoc();
                newA.kodePkt = temp.kodePkt;
                newA.tanggal = temp.tanggalPengajuan.AddDays(1);
                newA.C100 = temp.permintaanAdhoc[0];
                newA.C50 = temp.permintaanAdhoc[1];
                newA.C20 = temp.permintaanAdhoc[2];
                db.LaporanPermintaanAdhocs.Add(newA);
            }
            db.SaveChanges();
        }
        public static void closeExcel()
        {
            reader.Close();
        }
        public static int ExcelColumnNameToNumber(string columnName)
        {
            if (string.IsNullOrEmpty(columnName)) throw new ArgumentNullException("columnName");

            columnName = columnName.ToUpperInvariant();

            int sum = 0;

            for (int i = 0; i < columnName.Length; i++)
            {
                sum *= 26;
                sum += (columnName[i] - 'A' + 1);
            }

            return sum;
        }
    }
}
public class tanggalValue
{
    public DateTime tanggal { set; get; }
    public Int64 value { set; get; }
}
public class tanggalRasio
{
    public DateTime tanggal { set; get; }
    public Double value { set; get; }
}
