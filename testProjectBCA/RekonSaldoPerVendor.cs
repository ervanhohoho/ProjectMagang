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
    
    public partial class RekonSaldoPerVendor
    {
        public int id { get; set; }
        public string cashPointtId { get; set; }
        public string confId { get; set; }
        public Nullable<System.DateTime> orderDate { get; set; }
        public string vendor { get; set; }
        public string actionRekon { get; set; }
        public string statusRekon { get; set; }
        public Nullable<System.DateTime> dueDate { get; set; }
        public Nullable<System.DateTime> blogTime { get; set; }
        public Nullable<long> currencyAmmount { get; set; }
        public Nullable<System.DateTime> realDate { get; set; }
        public string validation { get; set; }
    }
}