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
        public static string connectionString = "Data Source=MSI;Initial Catalog=CAC;Integrated Security=True";
    }
}
