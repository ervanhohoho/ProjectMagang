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
    
    public partial class Opti
    {
        public int idTransaksi { get; set; }
        public string idCashpoint { get; set; }
        public Nullable<System.DateTime> tanggal { get; set; }
        public Nullable<long> prediksi { get; set; }
    
        public virtual Cashpoint Cashpoint { get; set; }
    }
}
