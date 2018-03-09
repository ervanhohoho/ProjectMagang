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
    public partial class initTransaksi : Form
    {
        public initTransaksi()
        {
            InitializeComponent();
            Console.Write(alphabetToIndex('n'));
            Database1Entities db = new Database1Entities();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "Microsoft Excel | *.xls; *.xlsx; *.xlsm;";
            if(of.ShowDialog() == DialogResult.OK);
                initTable(of.FileName);
        }
        private void initTable(String filepath)
        {
            DataSet ds = Util.openExcel(filepath);
            DataTable tb = ds.Tables[1];
            int counter = 0;
            for (int i = 4; i < 27850; i++)
            {
                
                //Dari form input
                transaksiPkt pkt = new transaksiPkt();
                pkt.kodePkt = tb.Rows[i][0].ToString();
                pkt.tanggalPengajuan = (DateTime)tb.Rows[i][3];

                //Saldo awal
                for (int j = 4; j < alphabetToIndex('g') + 1; j++)
                {
                    if(tb.Rows[i][j].ToString()!="")
                        pkt.saldoAwal.Add(doubleToInt64((double)tb.Rows[i][j]));
                    else
                        pkt.saldoAwal.Add(0);
                }
                //Sislok/bongkaran atm
                for (int j = alphabetToIndex('i'); j < alphabetToIndex('k') + 1; j++)
                {
                    if (tb.Rows[i][j].ToString() != "")
                        pkt.bongkaranAtm.Add(doubleToInt64((double)tb.Rows[i][j]));
                    else
                        pkt.bongkaranAtm.Add(0);
                }
                //Sislok/bongkaran cdm
                for (int j = alphabetToIndex('l'); j < alphabetToIndex('n') + 1; j++)
                {
                    if (tb.Rows[i][j].ToString() != "")
                        pkt.bongkaranCdm.Add(doubleToInt64((double)tb.Rows[i][j]));
                    else
                        pkt.bongkaranCdm.Add(0);
                }
                //Bongkaran crm
                for (int j = alphabetToIndex('o'); j < alphabetToIndex('q') + 1; j++)
                {
                    if (tb.Rows[i][j].ToString() != "")
                        pkt.bongkaranCrm.Add(doubleToInt64((double)tb.Rows[i][j]));
                    else
                        pkt.bongkaranCrm.Add(0);
                }
                //Isi ATM
                for (int j = alphabetToIndex('r'); j < alphabetToIndex('t') + 1; j++)
                {
                    if (tb.Rows[i][j].ToString() != "")
                        pkt.pengisianAtm.Add(doubleToInt64((double)tb.Rows[i][j]));
                    else
                        pkt.pengisianAtm.Add(0);
                }
                //Isi CRM
                for (int j = alphabetToIndex('u'); j < alphabetToIndex('w') + 1; j++)
                {
                    if (tb.Rows[i][j].ToString() != "")
                        pkt.pengisianCrm.Add(doubleToInt64((double)tb.Rows[i][j]));
                    else
                        pkt.pengisianCrm.Add(0);
                }
                //Bon
                for (int j = alphabetToIndex('x'); j < alphabetToIndex('z') + 1; j++)
                {
                    if (tb.Rows[i][j].ToString() != "")
                        pkt.penerimaanBon.Add(doubleToInt64((double)tb.Rows[i][j]));
                    else
                        pkt.penerimaanBon.Add(0);
                }
                //Adhoc
                for (int j = alphabetToIndex('z') + 1; j < alphabetToIndex('z') + 4; j++)
                {
                    if (tb.Rows[i][j].ToString() != "")
                        pkt.penerimaanBonAdhoc.Add(doubleToInt64((double)tb.Rows[i][j]));
                    else
                        pkt.penerimaanBonAdhoc.Add(0);
                }
                //Setor
                for (int j = alphabetToIndex('z') + 4; j < alphabetToIndex('z') + 7; j++)
                {
                    if (tb.Rows[i][j].ToString() != "")
                        pkt.setorUang.Add(doubleToInt64((double)tb.Rows[i][j]));
                    else
                        pkt.setorUang.Add(0);
                }
                //Saldo Akhir
                for (int j = alphabetToIndex('z') + 7; j < alphabetToIndex('z') + 10; j++)
                {
                    if (tb.Rows[i][j].ToString() != "")
                        pkt.saldoAkhir.Add(doubleToInt64((double)tb.Rows[i][j]));
                    else
                        pkt.saldoAkhir.Add(0);
                }
                Util.inputDataTransaksiATMToDB(pkt);
            }
            MessageBox.Show("Done!");
        }
        int alphabetToIndex(char aplhabet)
        {
            return (int)aplhabet - (int)'a';
        }
        private String doubleToString(double temp)
        {
            var x = Math.Round(temp, 0);
            return x + "";
        }
        private Int64 doubleToInt64(double temp)
        {
            var x = Math.Round(temp, 0);
            return (Int64)x;
        }
    }
}
