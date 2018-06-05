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
    public partial class loadPkt : Form
    {
        Database1Entities db = new Database1Entities();
        public loadPkt()
        {
            //Pkt newp = new Pkt();
            //newp.kodePkt = "AAA";
            //newp.namaPkt = "asdads";
            //newp.tipee2e = "none2e";
            //newp.koordinator = "CO JAKARTA";
            //newp.kanwil = "JABOTABEK";
            //newp.sentralisasi = "senkas";
            //db.Pkts.Add(newp);
            //db.SaveChanges();
            //Console.Write((from x in db.Pkts select x.namaPkt).FirstOrDefault().ToString());
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Database1Entities db = new Database1Entities();
            
            DataSet ds = new DataSet();
            OpenFileDialog of = new OpenFileDialog();
            if (of.ShowDialog() == DialogResult.OK)
            {
                ds = Util.openExcel(@of.FileName);
            }
            if (ds.Tables.Count != 0)
            {
                int counter = 0;

                List<Pkt> listPkt = (from x in db.Pkts select x).ToList();
                //Pkt newpkt = new Pkt();
                //newpkt.kodePkt = ds.Tables[0].Rows[41][1].ToString();
                //newpkt.namaPkt= ds.Tables[0].Rows[41][2].ToString();
                //newpkt.tipee2e = ds.Tables[0].Rows[41][3].ToString();
                //newpkt.koordinator = ds.Tables[0].Rows[41][4].ToString();
                //newpkt.kanwil = ds.Tables[0].Rows[41][5].ToString().Replace(" - ", " ");
                //newpkt.sentralisasi = ds.Tables[0].Rows[41][6].ToString().Replace(" - ", " ");
                //Console.WriteLine(newpkt.kodePkt + " " + newpkt.namaPkt);
                //db.Pkts.Add(newpkt);
                //db.SaveChanges();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    if (counter == 0)
                    {
                        counter++; continue; //Skip Header
                    }
                    Pkt toEdit = listPkt.Where(x => x.kodePkt == row[1].ToString()).FirstOrDefault();
                    if (toEdit != null)
                    {
                        toEdit.kodePkt = row[1].ToString();
                        Console.Write(row[1].ToString() + " ");
                        toEdit.namaPkt = row[4].ToString();
                        Console.Write(row[4].ToString());
                        toEdit.e2e = row[5].ToString();
                        Console.Write(row[5].ToString());
                        toEdit.koordinator = row[6].ToString();
                        Console.Write(row[6].ToString());
                        toEdit.kanwil = row[7].ToString().Replace(" -", "");
                        Console.Write(row[7].ToString());
                        toEdit.sentralisasi = row[8].ToString().Replace(" -", "");
                        Console.WriteLine(row[8].ToString());
                        toEdit.vendor = row[10].ToString();
                        if (String.IsNullOrEmpty(row[3].ToString()))
                            toEdit.kodePktCabang = "";
                        else
                            toEdit.kodePktCabang = row[3].ToString();

                        if (String.IsNullOrEmpty(row[2].ToString()))
                            toEdit.kodePktATM = "";
                        else
                            toEdit.kodePktATM = row[2].ToString();
                        counter++;
                    }
                    else
                    {
                        Pkt newPkt = new Pkt();
                        newPkt.kodePkt = row[1].ToString();
                        Console.Write(row[1].ToString() + " ");
                        newPkt.namaPkt = row[4].ToString();
                        Console.Write(row[4].ToString());
                        newPkt.e2e = row[5].ToString();
                        Console.Write(row[5].ToString());
                        newPkt.koordinator = row[6].ToString();
                        Console.Write(row[6].ToString());
                        newPkt.kanwil = row[7].ToString().Replace(" -", "");
                        Console.Write(row[7].ToString());
                        newPkt.sentralisasi = row[8].ToString().Replace(" -", "");
                        Console.WriteLine(row[8].ToString());
                        newPkt.vendor = row[10].ToString();
                        if (String.IsNullOrEmpty(row[3].ToString()))
                            newPkt.kodePktCabang = "";
                        else
                            newPkt.kodePktCabang = row[3].ToString();

                        if (String.IsNullOrEmpty(row[2].ToString()))
                            newPkt.kodePktATM = "";
                        else
                            newPkt.kodePktATM = row[2].ToString();
                        db.Pkts.Add(newPkt);
                        counter++;
                    }
                }
                db.SaveChanges();
                this.Close();
            }
        }
    }
}
