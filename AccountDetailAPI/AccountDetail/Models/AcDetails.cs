using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountDetail.Models
{
    public class AcDetails
    {
        public string AccountNumber { get; set; }
    }


    public class JsonClass
    {
        public JsonClassDetail[] JsonDetail { get; set; }
    }

    public class JsonClassDetail
    {
        public string TranDate { get; set; }
        public string Particulars { get; set; }
        public string ValueDate { get; set; }
        public string Debit { get; set; }
        public string Credit { get; set; }
        public string Balance { get; set; }
        public DateTime TRAN_DATE { get; set; }
    }

}