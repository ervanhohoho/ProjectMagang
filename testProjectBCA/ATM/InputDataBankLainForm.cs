using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testProjectBCA.ATM
{
    public partial class InputDataBankLainForm : Form
    {
        public InputDataBankLainForm()
        {
            InitializeComponent();
        }

        private void SelectButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = Variables.excelFilter;
            if(of.ShowDialog() == DialogResult.OK)
            {
                loadForm.ShowSplashScreen();
                DataSet ds = Util.openExcel(of.FileName);
                DataTable dt = ds.Tables[0];
                dt.Rows.RemoveAt(0);
                Database1Entities db = new Database1Entities();
                List<DataBankLain> listDariDb = db.DataBankLains.ToList();
                List<DataBankLain> listDariExcel = new List<DataBankLain>();
                List<DataBankLain> toDelete = new List<DataBankLain>();
                foreach (DataRow row in dt.Rows)
                {
                    listDariExcel.Add(new DataBankLain() {
                        idBank = row[0].ToString(),
                        namaBank = row[1].ToString()
                    });
                }

                foreach(var temp in listDariExcel)
                {
                    var cariDB = listDariDb.Where(x => x.idBank == temp.idBank).FirstOrDefault();
                    if(cariDB!=null)
                    {
                        cariDB.namaBank = temp.namaBank;
                        toDelete.Add(temp);
                    }
                }
                db.SaveChanges();
                foreach (var temp in toDelete)
                    listDariExcel.Remove(temp);
                db.DataBankLains.AddRange(listDariExcel);
                db.SaveChanges();
                loadForm.CloseForm();
            }
        }
    }
}
