﻿using System;
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
    public partial class RekonSaldoForm : Form
    {
        Database1Entities en = new Database1Entities();
        public RekonSaldoForm()
        {
            InitializeComponent();
            buttonProcessVault.Visible = false;
            buttonProcessSetoranCpc.Visible = false;
            buttonProsesOrderBlogHistory.Visible = false;
            dataGridView1.Visible = false;
            //buttonGeneratePivot.Enabled = false;
            //buttonGeneratePivotPerVendor.Enabled = false;

            
        }


        List<VOBH> vobh;
        List<VO> vo;
        List<OBH> obh;
        List<VaultProcessed> vp;
        List<SetoranCPC> sc;
        List<PivotCPC> pc;
        List<PivotCPC_BIBL> bibl;
        List<PivotCPC_BIBLD> bibld;
        List<PivotCPC_ATMD> atmd;
        List<PivotCPC_ATMR> atmr;
        List<OBHProcessed> obhp;
        List<PivotPerVendor_bon> ppvb;
        List<PivotPerVendor_setoran> ppvs;

        private void buttonUploadVaultOrderBlogHistory_Click(object sender, EventArgs e)
        {
            int flagUpVOBH = 0;
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = Variables.csvFilter;
            
            if (of.ShowDialog() == DialogResult.OK)
            {
                String filename = of.FileName;
                DataSet ds = Util.openCsv(filename);
                DataTable dt = ds.Tables[0];

                dataGridView1.DataSource = dt;

                vobh = new List<VOBH>();

                for (int i = 1; i < dt.Rows.Count - 1; i++)
                {
                    Console.WriteLine(i);
                    Console.WriteLine(dt.Rows[i][7].ToString());
                    vobh.Add(new VOBH
                    {
                        vaultId = dt.Rows[i][0].ToString(),
                        confId = dt.Rows[i][4].ToString(),
                        orderDate = Convert.ToDateTime(dt.Rows[i][5].ToString()).Date,
                        dueDate = Convert.ToDateTime(dt.Rows[i][6].ToString()).Date,
                        timeStamp = DateTime.ParseExact(dt.Rows[i][7].ToString(), "M/d/yyyy H:mm", System.Globalization.CultureInfo.InvariantCulture),
                        action = dt.Rows[i][2].ToString(),
                        status = dt.Rows[i][3].ToString(),
                        currencyAmmount = String.IsNullOrEmpty(dt.Rows[i][11].ToString()) ? Int64.Parse(dt.Rows[i - 1][11].ToString().Replace("IDR:", "")) : Int64.Parse(dt.Rows[i][11].ToString().Replace("IDR:", ""))
                    });
                }
                label1.Text = filename.Substring(filename.LastIndexOf('\\'), filename.Length - filename.LastIndexOf('\\'));
            }
            
            flagUpVOBH = 1;
        }

        private void buttonUploadVaultOrder_Click(object sender, EventArgs e)
        {
            int flagUpVO = 0;
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = Variables.csvFilter;

            if (of.ShowDialog() == DialogResult.OK)
            {
                String filename = of.FileName;
                DataSet ds = Util.openCsv(filename);
                DataTable dt = ds.Tables[0];

                dataGridView1.DataSource = dt;

                vo = new List<VO>();

                for (int i = 1; i < dt.Rows.Count - 1; i++)
                {
                    vo.Add(new VO
                    {
                        vaultId = dt.Rows[i][0].ToString(),
                        confId = dt.Rows[i][2].ToString(),
                        fundingSource = dt.Rows[i][3].ToString(),
                        orderDate = Convert.ToDateTime(dt.Rows[i][5].ToString()).Date,
                        dueDate = Convert.ToDateTime(dt.Rows[i][6].ToString()).Date,
                        action = dt.Rows[i][1].ToString(),
                        status = dt.Rows[i][4].ToString(),
                        currencyAmmount = Int64.Parse(dt.Rows[i][11].ToString())
                    });
                }
                label2.Text = filename.Substring(filename.LastIndexOf('\\'), filename.Length - filename.LastIndexOf('\\'));
            }
            flagUpVO = 1;
        }

        private void buttonUploadOrderBlogHistory_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = Variables.csvFilter;
            if (of.ShowDialog() == DialogResult.OK)
            {
                String filename = of.FileName;
                DataSet ds = Util.openCsv(filename);
                DataTable dt = ds.Tables[0];

                //dataGridView1.DataSource = dt;

                obh = new List<OBH>();
                   
                for (int i = 1; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i][6].ToString() == "Confirmed" || dt.Rows[i][6].ToString() == "In Transit")
                    {
                        Console.WriteLine(i);
                        obh.Add(new OBH
                        {
                            cashpointId = dt.Rows[i][0].ToString(),
                            confId = dt.Rows[i][5].ToString(),
                            orderDate = Convert.ToDateTime(dt.Rows[i][3].ToString()).Date,
                            dueDate = Convert.ToDateTime(dt.Rows[i][4].ToString()).Date,
                            blogTime = DateTime.ParseExact(dt.Rows[i][9].ToString(), "M/d/yyyy H:mm", System.Globalization.CultureInfo.InvariantCulture),
                            action = dt.Rows[i][2].ToString(),
                            status = dt.Rows[i][6].ToString(),
                            currencyAmmount = CurrencyFill(String.IsNullOrEmpty(dt.Rows[i][11].ToString()) ? dt.Rows[i - 1][11].ToString().Replace("IDR:", "") : dt.Rows[i][11].ToString().Replace("IDR:", ""))
                        });
                    }
                    
                }

                label3.Text = filename.Substring(filename.LastIndexOf('\\'),filename.Length - filename.LastIndexOf('\\'));
                dataGridView1.DataSource = obh;

            }
        }

        private void buttonProsesOrderBlogHistory_Click(object sender, EventArgs e)
        {

            obhp = new List<OBHProcessed>();

            var cabangs = en.Cabangs.Select(x=>x).ToList();



            var query = (from x in obh
                         join y in cabangs on x.cashpointId.TrimStart('B').TrimStart('0') equals y.kodeCabang
                         select new { x.cashpointId, x.confId, x.orderDate, x.dueDate, x.blogTime, x.action, x.status, x.currencyAmmount, y.kodePkt }
                         ).ToList();

            foreach (var item in query)
            {
                obhp.Add(new OBHProcessed
                {
                    cashpointId = item.cashpointId,
                    vendor = item.kodePkt,
                    confId = item.confId,
                    orderDate = item.orderDate,
                    dueDate = item.dueDate,
                    blogTime = item.blogTime,
                    action = item.action,
                    status = item.status,
                    currencyAmmount = item.currencyAmmount,
                    realDate = item.blogTime.Hour < 21 ? item.blogTime.Date : item.blogTime.AddDays(1).Date,
                    validation = (item.blogTime.Hour < 21 ? item.blogTime : item.blogTime.AddDays(1)).Date <= item.dueDate.Date ? "VALIDATED" : "NOT VALIDATED"
                });
            }
            dataGridView1.DataSource = obhp;

        }

        private void buttonProcessVault_Click(object sender, EventArgs e)
        {
            var query = (from x in vobh
                         select new { x.vaultId, x.confId, x.action , x.status, x.orderDate, x.dueDate, x.timeStamp, x.currencyAmmount, fundingSource = vo.Where(y => y.confId == x.confId).Select(y => y.fundingSource).FirstOrDefault() }
                         ).ToList();

            vp = new List<VaultProcessed>();

            foreach (var item in query)
            {
                vp.Add(new VaultProcessed
                {
                    vaultId = item.vaultId,
                    confId = item.confId,
                    action = item.action,
                    status = item.status,
                    orderDate = item.orderDate,
                    dueDate = item.dueDate,
                    timeStamp = item.timeStamp,
                    currencyAmmount = item.currencyAmmount,
                    fundingSource = item.fundingSource,
                    realDate = item.timeStamp.Hour < 21 ? item.timeStamp.Date : item.timeStamp.AddDays(1).Date,
                    validation = (item.timeStamp.Hour < 21 ? item.timeStamp : item.timeStamp.AddDays(1)).Date <= item.dueDate.Date ? "VALIDATED" : "NOT VALIDATED"
                });
            }
            Console.WriteLine(vp.Count);
            dataGridView1.DataSource = vp;
           
        }

        private void buttonProcessSetoranCpc_Click(object sender, EventArgs e)
        {
            var query = (from x in vp
                         where x.status == "Confirmed"
                         select x
                         ).ToList();

            sc = new List<SetoranCPC>();

            foreach (var item in query)
            {
                sc.Add(new SetoranCPC
                {
                    vaultId = item.vaultId,
                    confId = item.confId,
                    action = item.action,
                    status = item.status,
                    orderDate = item.orderDate,
                    dueDate = item.dueDate,
                    timeStamp = item.timeStamp,
                    currencyAmmount = item.currencyAmmount,
                    fundingSource = item.fundingSource,
                    realDate = item.timeStamp.Hour < 21 ? item.timeStamp.Date : item.timeStamp.AddDays(1).Date,
                    validation = (item.timeStamp.Hour < 21 ? item.timeStamp : item.timeStamp.AddDays(1)).Date <= item.dueDate.Date ? "VALIDATED" : "NOT VALIDATED"
                });
            }
            Console.WriteLine(sc.Count);
            dataGridView1.DataSource = sc;
        }

        private void buttonGeneratePivotPerVendor_Click(object sender, EventArgs e)
        {
            ProsesOrderBlogHistory();

            //preparing data for pivot pervendor bon
            var query = (from x in obhp
                         where x.action.Contains("Delivery")
                         select x).ToList();
            
            //generating pivotbon
            ppvb = new List<PivotPerVendor_bon>();

            dataGridView7.DataSource = query;

            Console.WriteLine(query.Count);

            var pivot = query.GroupBy(c => new { c.vendor, c.dueDate }).Select(g => new
            {
                dueDate = g.Key.dueDate,
                vendor = g.Key.vendor,
                belumValidasi = g.Where(c => c.validation == "NOT VALIDATED").Sum(c => c.currencyAmmount),
                sudahValidasi = g.Where(c => c.validation == "VALIDATED").Sum(c => c.currencyAmmount),
                grandTotal = g.Where(c => c.validation == "NOT VALIDATED").Sum(c => c.currencyAmmount)+
                             g.Where(c => c.validation == "VALIDATED").Sum(c => c.currencyAmmount) 

            }).ToList();

            Console.WriteLine(pivot.Count);

            foreach (var item in pivot)
            {
                ppvb.Add(new PivotPerVendor_bon
                {
                    dueDate = item.dueDate,
                    vendor = item.vendor,
                    belumValidasi = item.belumValidasi,
                    sudahValidasi = item.sudahValidasi,
                    grandTotal = item.grandTotal
                });
            }

            dataGridView6.DataSource = ppvb;

            //preparing data for pivot pervendor setoran
            var querysetoran = (from x in obhp
                         where x.action.Contains("Return")
                         select x).ToList();

            //generating pivot
            ppvs = new List<PivotPerVendor_setoran>();

            var pivotsetoran = querysetoran.GroupBy(c => new { c.vendor, c.dueDate }).Select(g => new
            {
                dueDate = g.Key.dueDate,
                vendor = g.Key.vendor,
                belumValidasi = g.Where(c => c.validation == "NOT VALIDATED").Sum(c => c.currencyAmmount),
                sudahValidasi = g.Where(c => c.validation == "VALIDATED").Sum(c => c.currencyAmmount),
                grandTotal = g.Where(c => c.validation == "NOT VALIDATED").Sum(c => c.currencyAmmount) +
                             g.Where(c => c.validation == "VALIDATED").Sum(c => c.currencyAmmount)

            }).ToList();

            foreach (var item in pivotsetoran)
            {
                ppvs.Add(new PivotPerVendor_setoran
                {
                    dueDate = item.dueDate,
                    vendor = item.vendor,
                    belumValidasi = item.belumValidasi,
                    sudahValidasi = item.sudahValidasi,
                    grandTotal = item.grandTotal
                });
            }

            dataGridView7.DataSource = ppvs;
        }


        private void buttonGeneratePivot_Click(object sender, EventArgs e)
        {
            ProsesVault();
            ProsesSetoranCPC();
            //preparing data for pivotCPC - BI dan BankLain Return
            var query = (from x in sc
                         where x.fundingSource == "BI" || x.fundingSource.Contains("OB")
                         select x).ToList();

            pc = new List<PivotCPC>();

            String bufferVaultId = "";

            foreach (var item in query)
            {
                pc.Add(new PivotCPC
                {
                    vaultId = item.vaultId,
                    confId = item.confId,
                    action = item.action,
                    status = item.status,
                    orderDate = item.orderDate,
                    dueDate = item.dueDate,
                    timeStamp = item.timeStamp,
                    currencyAmmount = item.currencyAmmount,
                    fundingSource = item.fundingSource,
                    realDate = item.timeStamp.Hour < 21 ? item.timeStamp.Date : item.timeStamp.AddDays(1).Date,
                    validation = (item.timeStamp.Hour < 21 ? item.timeStamp : item.timeStamp.AddDays(1)).Date <= item.dueDate.Date ? "VALIDATED" : "NOT VALIDATED"
                });

            }

            //creating pivotCPC - BI dan BankLain Return
            var pivot = pc.GroupBy(c => new { c.vaultId, c.fundingSource, c.dueDate}).Select(g => new
            {
                dueDate = g.Key.dueDate,
                vaultID = g.Key.vaultId,
                fundingSource = g.Key.fundingSource,
                emergencyReturn = g.Where(c => c.action == "Emergency Return").Sum(c => c.currencyAmmount),
                plannedReturn = g.Where(c => c.action == "Planned Return").Sum(c => c.currencyAmmount)
                
            }).ToList();
            
            var pivot2 = (from x in pivot
                          select new { x.dueDate, x.vaultID, x.fundingSource, x.emergencyReturn, x.plannedReturn, grandTotal =x.emergencyReturn+ x.plannedReturn });

            bibl = new List<PivotCPC_BIBL>();

            foreach (var item in pivot2)
            {
                if (!(item.plannedReturn == 0 && item.emergencyReturn == 0))
                {
                    bibl.Add(new PivotCPC_BIBL
                    {
                        dueDate = item.dueDate,
                        vaultId = item.vaultID,
                        fundingSource = item.fundingSource,
                        emergencyReturn = item.emergencyReturn,
                        plannedReturn = item.plannedReturn,
                        grandTotal = item.grandTotal
                    });
                }
                
            }

            dataGridView2.DataSource = bibl;

            //preparing data for Pivot CPC - BI dan BankLain delivery
            var queryd = (from x in sc
                         where x.fundingSource == "BI" || x.fundingSource.Contains("OB")
                         select x).ToList();

            pc = new List<PivotCPC>();

            String bufferVaultId2 = "";

            foreach (var item in queryd)
            {
                pc.Add(new PivotCPC
                {
                    vaultId = item.vaultId,
                    confId = item.confId,
                    action = item.action,
                    status = item.status,
                    orderDate = item.orderDate,
                    dueDate = item.dueDate,
                    timeStamp = item.timeStamp,
                    currencyAmmount = item.currencyAmmount,
                    fundingSource = item.fundingSource,
                    realDate = item.timeStamp.Hour < 21 ? item.timeStamp.Date : item.timeStamp.AddDays(1).Date,
                    validation = (item.timeStamp.Hour < 21 ? item.timeStamp : item.timeStamp.AddDays(1)).Date <= item.dueDate.Date ? "VALIDATED" : "NOT VALIDATED"
                });

            }

            //creating pivotCPC - BI dan BankLain Delivery
            var pivotd = pc.GroupBy(c => new { c.vaultId, c.fundingSource, c.dueDate }).Select(g => new
            {
                dueDate = g.Key.dueDate,
                vaultID = g.Key.vaultId,
                fundingSource = g.Key.fundingSource,
                emergencyDelivery = g.Where(c => c.action == "Emergency Delivery").Sum(c => c.currencyAmmount),
                plannedDelivery = g.Where(c => c.action == "Planned Delivery").Sum(c => c.currencyAmmount)
            }).ToList();
            
            var pivotd2 = (from x in pivotd
                          select new {x.dueDate, x.vaultID, x.fundingSource, x.emergencyDelivery, x.plannedDelivery, grandTotal = x.emergencyDelivery + x.plannedDelivery });

            bibld = new List<PivotCPC_BIBLD>();

            foreach (var item in pivotd2)
            {
                if (!(item.emergencyDelivery == 0 && item.plannedDelivery == 0))
                {
                    bibld.Add(new PivotCPC_BIBLD
                    {
                        dueDate = item.dueDate,
                        vaultId = item.vaultID,
                        fundingSource = item.fundingSource,
                        emergencyDelivery = item.emergencyDelivery,
                        plannedDelivery = item.plannedDelivery,
                        grandTotal = item.grandTotal
                    });
                }
                
            }

            dataGridView3.DataSource = bibld;

            //preparing data for pivot - ATM delivery
            var queryad = (from x in sc
                          where x.fundingSource != "BI" && !x.fundingSource.Contains("OB")
                          select x).ToList();

            pc = new List<PivotCPC>();

            String bufferVaultId3 = "";

            foreach (var item in queryad)
            {
                pc.Add(new PivotCPC
                {
                    vaultId = item.vaultId,
                    confId = item.confId,
                    action = item.action,
                    status = item.status,
                    orderDate = item.orderDate,
                    dueDate = item.dueDate,
                    timeStamp = item.timeStamp,
                    currencyAmmount = item.currencyAmmount,
                    fundingSource = item.fundingSource,
                    realDate = item.timeStamp.Hour < 21 ? item.timeStamp.Date : item.timeStamp.AddDays(1).Date,
                    validation = (item.timeStamp.Hour < 21 ? item.timeStamp : item.timeStamp.AddDays(1)).Date <= item.dueDate.Date ? "VALIDATED" : "NOT VALIDATED"
                });

            }

            //creating pivot - atm delivery
            var pivotad = pc.GroupBy(c => new { c.vaultId, c.fundingSource, c.dueDate }).Select(g => new
            {
                dueDate = g.Key.dueDate,
                vaultID = g.Key.vaultId,
                fundingSource = g.Key.fundingSource,
                emergencyDelivery = g.Where(c => c.action == "Emergency Delivery").Sum(c => c.currencyAmmount),
                plannedDelivery = g.Where(c => c.action == "Planned Delivery").Sum(c => c.currencyAmmount)
            }).ToList();

            var pivotad2 = (from x in pivotad
                           select new {x.dueDate, x.vaultID, x.fundingSource, x.emergencyDelivery, x.plannedDelivery, grandTotal = x.emergencyDelivery + x.plannedDelivery });

            atmd = new List<PivotCPC_ATMD>();

            foreach (var item in pivotad2)
            {
                if (!(item.emergencyDelivery == 0 && item.plannedDelivery == 0))
                {
                    atmd.Add(new PivotCPC_ATMD
                    {
                        dueDate = item.dueDate,
                        vaultId = item.vaultID,
                        fundingSource = item.fundingSource,
                        emergencyDelivery = item.emergencyDelivery,
                        plannedDelivery = item.plannedDelivery,
                        grandTotal = item.grandTotal
                    });
                }

            }

            dataGridView4.DataSource = atmd;

            //preparing data for pivot - atm return
            var queryar = (from x in sc
                           where x.fundingSource != "BI" && !x.fundingSource.Contains("OB")
                           select x).ToList();

            pc = new List<PivotCPC>();

            String bufferVaultId4 = "";

            foreach (var item in queryar)
            {
                pc.Add(new PivotCPC
                {
                    vaultId = item.vaultId,
                    confId = item.confId,
                    action = item.action,
                    status = item.status,
                    orderDate = item.orderDate,
                    dueDate = item.dueDate,
                    timeStamp = item.timeStamp,
                    currencyAmmount = item.currencyAmmount,
                    fundingSource = item.fundingSource,
                    realDate = item.timeStamp.Hour < 21 ? item.timeStamp.Date : item.timeStamp.AddDays(1).Date,
                    validation = (item.timeStamp.Hour < 21 ? item.timeStamp : item.timeStamp.AddDays(1)).Date <= item.dueDate.Date ? "VALIDATED" : "NOT VALIDATED"
                });

            }

            //creating pivotCPC - atm return
            var pivotar = pc.GroupBy(c => new { c.vaultId, c.fundingSource, c.dueDate }).Select(g => new
            {
                dueDate = g.Key.dueDate,
                vaultID = g.Key.vaultId,
                fundingSource = g.Key.fundingSource,
                emergencyReturn = g.Where(c => c.action == "Emergency Return").Sum(c => c.currencyAmmount),
                plannedReturn = g.Where(c => c.action == "Planned Return").Sum(c => c.currencyAmmount)
            }).ToList();

            var pivotar2 = (from x in pivotar
                          select new { x.dueDate, x.vaultID, x.fundingSource, x.emergencyReturn, x.plannedReturn, grandTotal = x.emergencyReturn + x.plannedReturn });

            atmr = new List<PivotCPC_ATMR>();

            foreach (var item in pivotar2)
            {
                if (!(item.plannedReturn == 0 && item.emergencyReturn == 0))
                {
                    atmr.Add(new PivotCPC_ATMR
                    {
                        dueDate = item.dueDate,
                        vaultId = item.vaultID,
                        fundingSource = item.fundingSource,
                        emergencyReturn = item.emergencyReturn,
                        plannedReturn = item.plannedReturn,
                        grandTotal = item.grandTotal
                    });
                }

            }

            dataGridView5.DataSource = atmr;
        }



        class VOBH //vault order blog history
        {
            public String vaultId { set; get; }
            public String confId { set; get; }
            public DateTime orderDate { set; get; }
            public DateTime dueDate { set; get; }
            public DateTime timeStamp { set; get; }
            public String action { set; get; }
            public String status { set; get; }
            public Int64 currencyAmmount { set; get; }
        }

        class VO //vault order
        {
            public String vaultId { set; get; }
            public String confId { set; get; }
            public String fundingSource { set; get; }
            public DateTime orderDate { set; get; }
            public DateTime dueDate { set; get; }
            public String action { set; get; }
            public String status { set; get; }
            public Int64 currencyAmmount { set; get; }
        }

        class VaultProcessed //class menampung proses vault 
        {
            public String vaultId { set; get; }
            public String confId { set; get; }
            public String fundingSource { set; get; }
            public String action { set; get; }
            public String status { set; get; }
            public DateTime orderDate { set; get; }
            public DateTime dueDate { set; get; }
            public DateTime timeStamp { set; get; }
            public Int64 currencyAmmount { set; get; }
            public DateTime realDate { set; get; }
            public String validation { set; get; }

        }

        class SetoranCPC //class menampung setoran cpc (vaultprocessed yang sudah di proses)
        {
            public String vaultId { set; get; }
            public String confId { set; get; }
            public String fundingSource { set; get; }
            public String action { set; get; }
            public String status { set; get; }
            public DateTime orderDate { set; get; }
            public DateTime dueDate { set; get; }
            public DateTime timeStamp { set; get; }
            public Int64 currencyAmmount { set; get; }
            public DateTime realDate { set; get; }
            public String validation { set; get; }
        }

        class OBH //order blog history 
        {
            public String cashpointId { set; get; }
            public String confId { set; get; }
            public DateTime orderDate { set; get; }
            public DateTime dueDate { set; get; }
            public DateTime blogTime { set; get; }
            public String action { set; get; }
            public String status { set; get; }
            public Int64 currencyAmmount { set; get; }
        }

        class OBHProcessed //obh yang di proses
        {
            public String cashpointId { set; get; }
            public String vendor { set; get; }
            public String confId { set; get; }
            public DateTime orderDate { set; get; }
            public DateTime dueDate { set; get; }
            public DateTime blogTime { set; get; }
            public String action { set; get; }
            public String status { set; get; }
            public Int64 currencyAmmount { set; get; }
            public DateTime realDate { set; get; }
            public String validation { set; get; }
        }

        class PivotCPC //class pivotCPC (canvas for detailed pivot)
        {
            public String vaultId { set; get; }
            public String confId { set; get; }
            public String fundingSource { set; get; }
            public String action { set; get; }
            public String status { set; get; }
            public DateTime orderDate { set; get; }
            public DateTime dueDate { set; get; }
            public DateTime timeStamp { set; get; }
            public Int64 currencyAmmount { set; get; }
            public DateTime realDate { set; get; }
            public String validation { set; get; }
       
        }

        class PivotCPC_BIBL //pivotCPC - BI dan BankLain return
        {
            public String vaultId { set; get; }
            public String fundingSource { set; get; }
            public Int64 plannedReturn { set; get; }
            public Int64 emergencyReturn { set; get; }
            public Int64 grandTotal { set; get; }
            public DateTime dueDate { set; get; }
        }

        class PivotCPC_BIBLD//pivotCPC - BI dan BankLain delivery
        {
            public String vaultId { set; get; }
            public String fundingSource { set; get; }
            public Int64 plannedDelivery { set; get; }
            public Int64 emergencyDelivery { set; get; }
            public Int64 grandTotal { set; get; }
            public DateTime dueDate { set; get; }
        }

        class PivotCPC_ATMD //pivotcpc - atm delivery
        {
            public String vaultId { set; get; }
            public String fundingSource { set; get; }
            public Int64 plannedDelivery { set; get; }
            public Int64 emergencyDelivery { set; get; }
            public Int64 grandTotal { set; get; }
            public DateTime dueDate { set; get; }
        }

        class PivotCPC_ATMR //pivotcpc - atm return
        {
            public String vaultId { set; get; }
            public String fundingSource { set; get; }
            public Int64 plannedReturn{ set; get; }
            public Int64 emergencyReturn{ set; get; }
            public Int64 grandTotal { set; get; }
            public DateTime dueDate { set; get; }
        }

        class PivotPerVendor_bon
        {
            public String vendor { set; get; }
            public Int64 belumValidasi { set; get; }
            public Int64 sudahValidasi { set; get; }
            public Int64 grandTotal { set; get; }
            public DateTime dueDate { set; get; }
        }

        class PivotPerVendor_setoran
        {
            public String vendor { set; get; }
            public Int64 belumValidasi { set; get; }
            public Int64 sudahValidasi { set; get; }
            public Int64 grandTotal { set; get; }
            public DateTime dueDate { set; get; }
        }

        public Int64 CurrencyFill(String a)
        {
            Int64 result = 0;

            if (a.Contains("e") || a.Contains("E"))
            {
                result = (Int64)Double.Parse(a, System.Globalization.NumberStyles.Float);
            }
            else
            {
                result = Int64.Parse(a);
            }

            return result;
        }

        public void ProsesVault()
        {
            {
                var query = (from x in vobh
                             select new { x.vaultId, x.confId, x.action, x.status, x.orderDate, x.dueDate, x.timeStamp, x.currencyAmmount, fundingSource = vo.Where(y => y.confId == x.confId).Select(y => y.fundingSource).FirstOrDefault() }
                             ).ToList();

                vp = new List<VaultProcessed>();

                foreach (var item in query)
                {
                    vp.Add(new VaultProcessed
                    {
                        vaultId = item.vaultId,
                        confId = item.confId,
                        action = item.action,
                        status = item.status,
                        orderDate = item.orderDate,
                        dueDate = item.dueDate,
                        timeStamp = item.timeStamp,
                        currencyAmmount = item.currencyAmmount,
                        fundingSource = item.fundingSource,
                        realDate = item.timeStamp.Hour < 21 ? item.timeStamp.Date : item.timeStamp.AddDays(1).Date,
                        validation = (item.timeStamp.Hour < 21 ? item.timeStamp : item.timeStamp.AddDays(1)).Date <= item.dueDate.Date ? "VALIDATED" : "NOT VALIDATED"
                    });
                }
                Console.WriteLine(vp.Count);
                dataGridView1.DataSource = vp;

            }
        }

        public void ProsesSetoranCPC()
        {
            var query = (from x in vp
                         where x.status == "Confirmed"
                         select x
                         ).ToList();

            sc = new List<SetoranCPC>();

            foreach (var item in query)
            {
                sc.Add(new SetoranCPC
                {
                    vaultId = item.vaultId,
                    confId = item.confId,
                    action = item.action,
                    status = item.status,
                    orderDate = item.orderDate,
                    dueDate = item.dueDate,
                    timeStamp = item.timeStamp,
                    currencyAmmount = item.currencyAmmount,
                    fundingSource = item.fundingSource,
                    realDate = item.timeStamp.Hour < 21 ? item.timeStamp.Date : item.timeStamp.AddDays(1).Date,
                    validation = (item.timeStamp.Hour < 21 ? item.timeStamp : item.timeStamp.AddDays(1)).Date <= item.dueDate.Date ? "VALIDATED" : "NOT VALIDATED"
                });
            }
            Console.WriteLine(sc.Count);
            dataGridView1.DataSource = sc;
        }
        
        public void ProsesOrderBlogHistory()
        {
            obhp = new List<OBHProcessed>();

            var cabangs = en.Cabangs.Select(x => x).ToList();



            var query = (from x in obh
                         join y in cabangs on x.cashpointId.TrimStart('B').TrimStart('0') equals y.kodeCabang
                         select new { x.cashpointId, x.confId, x.orderDate, x.dueDate, x.blogTime, x.action, x.status, x.currencyAmmount, y.kodePkt }
                         ).ToList();

            foreach (var item in query)
            {
                obhp.Add(new OBHProcessed
                {
                    cashpointId = item.cashpointId,
                    vendor = item.kodePkt,
                    confId = item.confId,
                    orderDate = item.orderDate,
                    dueDate = item.dueDate,
                    blogTime = item.blogTime,
                    action = item.action,
                    status = item.status,
                    currencyAmmount = item.currencyAmmount,
                    realDate = item.blogTime.Hour < 21 ? item.blogTime.Date : item.blogTime.AddDays(1).Date,
                    validation = (item.blogTime.Hour < 21 ? item.blogTime : item.blogTime.AddDays(1)).Date <= item.dueDate.Date ? "VALIDATED" : "NOT VALIDATED"
                });
            }
            dataGridView1.DataSource = obhp;
        }

        //BELUM SELSAI MASIH DI MINUS 1 DARI YG ATAS

    }
}
