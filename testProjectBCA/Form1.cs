using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClosedXML;
using ClosedXML.Excel;
using Excel = Microsoft.Office.Interop.Excel;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.IO;
using ExcelDataReader;
using System.Globalization;

namespace testProjectBCA
{
    public partial class Form1 : Form
    {
        
        private List<Data> dataList = new List<Data>();
        public Form1()
        {
            InitializeComponent();
            //testReadClosedXML();
        }
        public void testReadExcel()
        {
            //Open excel workbook, must have excel installed in the computer
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(@"C:\Users\Ervan\Desktop\BCA\bahan\ADJK.xlsx");
            Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
            Excel.Range xlRange = xlWorksheet.UsedRange;

            int rowCount = xlRange.Rows.Count;
            int colCount = xlRange.Columns.Count;
            int sheetCount = xlWorkbook.Sheets.Count;

            Console.Write(xlRange.Cells[6, 6].Value2.ToString());

            //free ram
            GC.Collect();
            GC.WaitForPendingFinalizers();

            //release com objects to fully kill excel process from running in the background
            Marshal.ReleaseComObject(xlRange);
            Marshal.ReleaseComObject(xlWorksheet);

            //close and release
            xlWorkbook.Close(false);
            Marshal.ReleaseComObject(xlWorkbook);

            //quit and release
            xlApp.Quit();
            Marshal.ReleaseComObject(xlApp);
            
        }
        //ini bisa dipake untuk bikin file excel untuk output, tapi untuk read lambat.
        public void testReadClosedXML(string filepath)
        {
            var wb = new XLWorkbook(@filepath);
            var ws = wb.Worksheet(1);
            var row = ws.Row(6);
            var cell = row.Cell(7);
            var testRange = ws.Range("B15:K31");
            
            Console.Write(testRange.RowCount());
            Console.Write(cell.Value.ToString());
        }
        private void readOpti(string filePath)
        {
            DataSet result = excelReader.openFile(filePath);
            //Hitung jumlah sheet
            Console.WriteLine(result.Tables.Count.ToString());
            //Hitung jumlah row
            Console.WriteLine(result.Tables[0].Rows.Count.ToString());
        
            int counter = 0;
            foreach(DataRow temp in result.Tables[0].Rows)
            {
                if (counter == 0)
                { counter = 1; continue; }
                String t1 = temp[0].ToString();
                String t2 = temp[13].ToString();
                Int64 t3 = Convert.ToInt64(t2);
                Data d = new Data(temp[0].ToString(),t3);
                dataList.Add(d);
            }
            MessageBox.Show(dataList[23200].closeBalance.ToString());
            excelReader.close();
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "Office Files|*.xls;*.xlsx";
            if(of.ShowDialog() == DialogResult.OK) { 
                Console.Write(of.FileName);
                readOpti(of.FileName);
            }
        }
    }
    class Data
    {
        public Data()
        {

        }
        public Data (String cashpoint, Int64 closeBalance)
        {
            this.cashpoint = cashpoint;
            this.closeBalance = closeBalance;
        }
        public String cashpoint { set; get; }
        public Int64 closeBalance { set; get; }
    }
    static class excelReader
    {
        public static FileStream stream;
        static IExcelDataReader reader;
        public static DataSet openFile(string filePath)
        {
            stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            {

                // Auto-detect format, supports:
                //  - Binary Excel files (2.0-2003 format; *.xls)
                //  - OpenXml Excel files (2007 format; *.xlsx)
                reader = ExcelReaderFactory.CreateReader(stream);
                //return excel as tables
                return reader.AsDataSet();
            }
        }
        public static void close()
        {
            reader.Close();
        }
    }
}
