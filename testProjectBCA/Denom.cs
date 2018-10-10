using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testProjectBCA
{
    public class Denom
    {
        public Denom()
        {
            d100 = 0;
            d50 = 0;
            d20 = 0;
        }

        public DateTime tgl { set; get; }
        public Int64 d100 { set; get; }
        public Int64 d50 { set; get; }
        public Int64 d20 { set; get; }
    }
}
