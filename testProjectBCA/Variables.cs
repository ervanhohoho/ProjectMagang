using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testProjectBCA
{
    class Variables
    { 
        public static DateTime todayDate = DateTime.Today;
        public static int todayYear = todayDate.Year;
        public static int todayMonth = todayDate.Month;
        public static int todayDay = todayDate.Day;
        public static string parentFolder = "C:\\";
        public static string excelFilter = "Microsoft Excel | *.xls; *.xlsx; *.xlsm;";
        public static string csvFilter = "CSV | *.csv";
        public static string txtFilter = "TXT | *.txt";
        public static string connectionString = "Data Source=MSI;Initial Catalog=CAC;Persist Security Info=True;User ID=TEST;Password=1234;";
        public static List<String> listMetodeNonATM = new List<string>() { "Historis", "Std Deviasi", "Historis + Std Deviasi" };
        public static List<String> listMetodeATM = new List<String>() { "Opti", "Historis", "Opti + Historis", "Historis + Std Deviasi", "Opti + Historis + Std Deviasi" };
        /**Bekas SLA Proses
        //var allDenomIn = (from x in listData
        //                  where ((DateTime)x.tanggal).Day == tgl + 1
        //                  && ((DateTime)x.tanggal).Month == bln
        //                  && ((DateTime)x.tanggal).Year == thn
        //                  select x).ToList();

        //if (tgl == DateTime.DaysInMonth(thn, bln))
        //{
        //    int tTgl = 1;

        //    if (bln == 12)
        //    {
        //        allDenomIn = (from x in listData
        //                      where ((DateTime)x.tanggal).Day == tTgl
        //                      && ((DateTime)x.tanggal).Month == 1
        //                      && ((DateTime)x.tanggal).Year == thn + 1
        //                      select x).ToList();
        //    }
        //    else
        //    {
        //        allDenomIn = (from x in listData
        //                      where ((DateTime)x.tanggal).Day == tTgl
        //                      && ((DateTime)x.tanggal).Month == bln
        //                      && ((DateTime)x.tanggal).Year == thn
        //                      select x).ToList();
        //    }
        //}
        //else
        //{
        //    DateTime temp = new DateTime(thn, bln, tgl + 1);
        //    Console.WriteLine(temp);
        //    allDenomIn = (from x in listData
        //                  where (DateTime)x.tanggal == temp
        //                  select x).ToList();
        //}
    **/
    }
}
