using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testProjectBCA
{
    public partial class InputMasterNasabahForm : Form
    {
        Database1Entities db = new Database1Entities();
        public InputMasterNasabahForm()
        {
            InitializeComponent();
        }

        private void selectFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = Variables.excelFilter;
            if(of.ShowDialog() == DialogResult.OK)
            {
                loadForm.ShowSplashScreen();
                var dataYangAda = (from x in db.Nasabahs select x.kodeNasabah).ToList();
                DataSet ds = Util.openExcel(of.FileName);
                DataTable dt = ds.Tables[0];
                dt.Rows.RemoveAt(0);

                List<Nasabah> listNasabah = new List<Nasabah>();

                for(int i = 0; i< dt.Rows.Count;i++)
                {
                    Console.WriteLine(i);
                    var row = dt.Rows[i];
                    if((from y in dataYangAda where y == row[0].ToString() select y).FirstOrDefault() != null || (from y in listNasabah where y.kodeNasabah == row[0].ToString() select y).FirstOrDefault()!=null)
                    {
                        continue;
                    }
                    Console.WriteLine(row[0].ToString());
                    Console.WriteLine(row[1].ToString());
                    Console.WriteLine(row[2].ToString());
                    Console.WriteLine(row[3].ToString());
                    Console.WriteLine(row[6].ToString());
                    listNasabah.Add(new Nasabah() {
                        kodeNasabah = row[0].ToString(),
                        fasilitasLayanan = row[1].ToString(),
                        metodeLayanan = row[2].ToString(),
                        kodeCabang = row[3].ToString(),
                        kodePktCabang = row[6].ToString(),
                        NomorRekening = row[7].ToString(),
                        GroupNasabah = row[8].ToString(),
                        namaNasabah = row[9].ToString(),
                        segmentasiNasabah = row[10].ToString(),
                        sentralisasi =row[11].ToString(),
                        subsidi = row[12].ToString(),
                        subsidiCabang = row[13].ToString(),
                        ring = row[14].ToString()
                    });
                }
                foreach(Nasabah temp in listNasabah)
                {
                    temp.kodeNasabah = temp.kodeNasabah.TrimStart('0');
                    temp.kodeCabang = temp.kodeCabang.TrimStart('0');
                }
                db.Nasabahs.AddRange(listNasabah);
                db.SaveChanges();
                loadForm.CloseForm();
            }
        }
    }
}
