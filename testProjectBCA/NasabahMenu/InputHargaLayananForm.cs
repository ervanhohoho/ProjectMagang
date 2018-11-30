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
    public partial class InputHargaLayananForm : Form
    {
        Database1Entities db = new Database1Entities();
        List<HargaLayanan> listStc, listCos;
        public InputHargaLayananForm()
        {
            InitializeComponent();
            loadGridViews();
        }
        private void loadGridViews()
        {
            loadStcGridView();
            loadCosGridView();
            loadTukaranGridView();
            loadAsuransiGridView();
        }
        private void loadStcGridView()
        {
            listStc = (from x in db.HargaLayanans where x.stc_cos == "stc" && x.jenisLayanan != "Tukaran" select x).ToList();
            if(listStc.Count>0)
            {
                stcGridView.DataSource = listStc;
            }
            else
            {

            }
            stcGridView.Columns[0].ReadOnly = true;
            stcGridView.Columns[1].ReadOnly = true;
            for (int i = 2; i < stcGridView.Columns.Count; i++)
            {
                stcGridView.Columns[i].DefaultCellStyle.Format = "C";
                stcGridView.Columns[i].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
            }
        }
        private void loadCosGridView()
        {
            listCos = (from x in db.HargaLayanans where x.stc_cos == "cos" && x.jenisLayanan != "Tukaran" select x).ToList();
            if (listStc.Count > 0)
            {
                cosGridView.DataSource = listCos;
            }
            else
            {

            }
            cosGridView.Columns[0].ReadOnly = true;
            cosGridView.Columns[1].ReadOnly = true;
            for(int i = 2;i<cosGridView.Columns.Count;i++)
            {
                cosGridView.Columns[i].DefaultCellStyle.Format = "C";
                cosGridView.Columns[i].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
            }
        }
        private void loadTukaranGridView()
        {
            Int64 hargaTukaran = (Int64) (from x in db.HargaLayanans where x.jenisLayanan == "Tukaran" select x.hargaRing1).FirstOrDefault();
            List<DisplayTukaran> tukaranDisplayList = new List<DisplayTukaran>();
            tukaranDisplayList.Add(new DisplayTukaran() { hargaTukaran = hargaTukaran });
            tukaranGridView.DataSource = tukaranDisplayList;
            tukaranGridView.Columns[0].DefaultCellStyle.Format = "C";
            tukaranGridView.Columns[0].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
        }
        private void loadAsuransiGridView()
        {
            Double rateAsuransi = (Double)(from x in db.AsuransiLayanans select x.asuransi).FirstOrDefault();
            
            List<DisplayAsuransi> asuransiDisplayList = new List<DisplayAsuransi>();
            asuransiDisplayList.Add(new DisplayAsuransi() { rateAsuransi = rateAsuransi });
            asuransiGridView.DataSource = asuransiDisplayList;
            asuransiGridView.Columns[0].DefaultCellStyle.Format = "P4";
        }

        private void tukaranGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            tukaranGridView.Rows[0].Cells[0].Value = (Int64)tukaranGridView.Rows[0].Cells[0].Value;
        }

        private void tukaranGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            List<HargaLayanan> hargaLayanan = (from x in db.HargaLayanans where x.jenisLayanan == "Tukaran" select x).ToList();
            foreach (HargaLayanan temp in hargaLayanan)
            {
                temp.hargaRing1 = (Int64)tukaranGridView.Rows[0].Cells[0].Value;
                temp.hargaRing2 = (Int64)tukaranGridView.Rows[0].Cells[0].Value;
                temp.hargaRing3 = (Int64)tukaranGridView.Rows[0].Cells[0].Value;
                temp.hargaRing4 = (Int64)tukaranGridView.Rows[0].Cells[0].Value;
                temp.hargaRing5 = (Int64)tukaranGridView.Rows[0].Cells[0].Value;
            }
        }

        private void asuransiGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            asuransiGridView.Columns[0].DefaultCellStyle.Format = "N6";
        }

        private void asuransiGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            AsuransiLayanan asuransi = (from x in db.AsuransiLayanans select x).FirstOrDefault();
            asuransi.asuransi = (Double)asuransiGridView.Rows[0].Cells[0].Value;
            asuransiGridView.Columns[0].DefaultCellStyle.Format = "P4";
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            db.SaveChanges();
        }
    }
    class DisplayTukaran
    {
        public Int64 hargaTukaran { set; get; }
    }
    class DisplayAsuransi
    {
        public Double rateAsuransi { set; get; }
    }
}
