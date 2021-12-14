using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeneratePDF.Models
{
    public class EmployeeModel
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Designation { get; set; }

        public decimal Salary { get; set; }
    }

    public class AccountDetailModel
    {
        public string? AccountNumber { get; set; }
    }

    public class MasterModel {
       public List<AccountStatementDetails> accountStatementDetails { get; set; }
        public List<AccountDetails> accountDetails { get; set; } 
        public List<AccountBalance> accountBalances { get; set; }
        public string AccountNumber { get; set; }
    }

    public class JsonClass
    {
        public AccountStatementDetails[] Json { get; set; }

    }

    public class AccountStatementDetails
    {
        public string TranDate { get; set; }
        public string Particulars { get; set; }
        public string ValueDate { get; set; }
        public string Debit { get; set; }
        public string Credit { get; set; }
        public string Balance { get; set; }
        public DateTime TRAN_DATE { get; set; }

    }


    public class Rootobject
    {
        public AccountDetails[] Acc { get; set; }
    }

    public class AccountDetails
    {
        public string FORACID { get; set; }
        public string ACCT_NAME { get; set; }
        public string Curr { get; set; }
        public string Scheme { get; set; }
        public string Address { get; set; }
        public string Branch { get; set; }
        public string PASSCODE { get; set; }
        public string EMAIL_ID { get; set; }
        public string TEL_NO { get; set; }
        public string CS_EMAIL { get; set; }
    }



    public class Rootobject1
    {
        public AccountBalance[] Property1 { get; set; }
    }

    public class AccountBalance
    {
        public string OpeningBalance { get; set; }
        public string TotalDebit { get; set; }
        public string DebitEntries { get; set; }
        public string TotalCredit { get; set; }
        public string CreditEntries { get; set; }
        public string ClosingBalance { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }

    
}