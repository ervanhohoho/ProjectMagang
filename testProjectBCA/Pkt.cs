//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace testProjectBCA
{
    using System;
    using System.Collections.Generic;
    
    public partial class Pkt
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Pkt()
        {
            this.laporanBons = new HashSet<laporanBon>();
            this.TransaksiAtms = new HashSet<TransaksiAtm>();
        }
    
        public string kodePkt { get; set; }
        public string namaPkt { get; set; }
        public string e2e { get; set; }
        public string koordinator { get; set; }
        public string kanwil { get; set; }
        public string sentralisasi { get; set; }
        public string vendor { get; set; }
        public string kodePktCabang { get; set; }
        public string kodePktATM { get; set; }
        public Nullable<long> kapasitasCPC { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<laporanBon> laporanBons { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TransaksiAtm> TransaksiAtms { get; set; }
    }
}
