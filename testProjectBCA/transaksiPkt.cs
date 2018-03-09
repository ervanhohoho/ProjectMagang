using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testProjectBCA
{
    public class transaksiPkt
    {
        public String kodePkt { set; get; }
        public DateTime tanggalPengajuan { set; get; }
        public List<Int64> saldoAwal { set; get; }
        public List<Int64> setorUang { set; get; }
        public List<Int64> penerimaanBon { set; get; }
        public List<Int64> penerimaanBonAdhoc { set; get; }
        public List<Int64> pengisianAtm { set; get; }
        public List<Int64> pengisianCrm { set; get; }
        public List<Int64> bongkaranAtm { set; get; }
        public List<Int64> bongkaranCrm { set; get; }
        public List<Int64> bongkaranCdm { set; get; }
        public List<List<Int64>> bonAtmYangDisetujui { set; get; }
        public List<Int64> saldoAkhir { set; get; }
        public List<List<Int64>> permintaanBon { set; get; }
        public List<Int64> permintaanAdhoc { set; get; }
        public List<Int64> saldoAwalHitungan { set; get; }
        public List<Int64> saldoAkhirHitungan { set; get; }
        public transaksiPkt()
        {
            saldoAwal = new List<Int64>();
            setorUang = new List<Int64>();
            penerimaanBon = new List<Int64>();
            penerimaanBonAdhoc = new List<Int64>();
            pengisianAtm = new List<Int64>();
            pengisianCrm = new List<Int64>();
            bongkaranAtm = new List<Int64>();
            bongkaranCrm = new List<Int64>();
            bongkaranCdm = new List<Int64>();
            //Penerimaan bon CIT untuk hari H
            bonAtmYangDisetujui = new List<List<Int64>>();
            saldoAkhir = new List<Int64>();
            permintaanBon = new List<List<Int64>>();
            bongkaranAtm = new List<Int64>();
            saldoAwalHitungan = new List<Int64>();
            saldoAkhirHitungan = new List<Int64>();
        }
        public void hitungSaldoAkhir()
        {
            for(int i = 0;i<saldoAwalHitungan.Count;i++)
            {
                Int64 temp = saldoAwalHitungan[i];
                temp = temp - pengisianAtm[i] - pengisianCrm[i] + bongkaranAtm[i] + bongkaranCdm[i] + bongkaranCrm[i] + penerimaanBon[i] + penerimaanBonAdhoc[i] - setorUang[i];
                saldoAkhirHitungan.Add(temp);
            }
        }
    }

}
