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
                        counter++; continue;
                    }
                    if (counter > 66)
                        break;
                    Pkt newPkt = new Pkt();
                    newPkt.kodePkt = row[1].ToString();
                    Console.Write(row[1].ToString() + " ");
                    newPkt.namaPkt = row[2].ToString();
                    Console.Write(row[2].ToString());
                    newPkt.e2e = row[3].ToString();
                    Console.Write(row[3].ToString());
                    newPkt.koordinator = row[4].ToString();
                    Console.Write(row[4].ToString());
                    newPkt.kanwil = row[5].ToString().Replace(" -","");
                    Console.Write(row[5].ToString());
                    newPkt.sentralisasi = row[6].ToString().Replace(" -", "");
                    Console.WriteLine(row[6].ToString());
                    db.Pkts.Add(newPkt);
                    counter++;
                }
                db.SaveChanges();
            }
        }
    }
}
