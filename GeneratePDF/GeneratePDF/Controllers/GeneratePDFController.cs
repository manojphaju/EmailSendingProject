using GeneratePDF.Models;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GeneratePDF.Controllers
{
    public class GeneratePDFController : Controller
    {
        #region
        public ActionResult Index()
        {
            return View();
        }
        #endregion

        #region Details
        public async Task<ActionResult> Details(MasterModel master)
        {
            Session["accountNumber"] = master.AccountNumber;


            List<AccountStatementDetails> accountStatementDetails = await BankStatement(master.AccountNumber);
            ViewBag.accountStatementDetails = accountStatementDetails;

            List<AccountDetails> accountDetails = await AccountDetails(master.AccountNumber);
            ViewBag.accountDetails = accountDetails;


            List<AccountBalance> accountBalance = await AccountBalance(master.AccountNumber);
            ViewBag.accountBalance = accountBalance;


            return View();
        }
        #endregion





        #region Rotativa
        /// <summary>
        /// Print Employees details
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> PrintPDF()
        {
            var AccountNumber = Session["accountNumber"].ToString();
            var report = new Rotativa.ActionAsPdf("Details", new { AccountNumber });
            return report;
        }
        #endregion

        #region BankStatement
        public async Task<List<AccountStatementDetails>> BankStatement(string accountNumber)
        {
            List<AccountStatementDetails> list = new List<AccountStatementDetails>();

            var APIBaseURL = ConfigurationManager.AppSettings["APIBaseURL"];
            var AuthUserName = ConfigurationManager.AppSettings["AuthUserName"];
            var AuthPassword = ConfigurationManager.AppSettings["AuthPassword"];

            HttpResponseMessage _res = new HttpResponseMessage();
            var accountObject = new AccountDetailModel
            {
                AccountNumber = accountNumber
            };
            // Serialize our concrete class into a JSON String
            var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(accountObject));
            // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
            var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");



            using (HttpClient httpClient = new HttpClient())
            {
                var byteArray = Encoding.ASCII.GetBytes(AuthUserName + ":" + AuthPassword);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                _res = await httpClient.PostAsync(APIBaseURL + "Account/StatementDetails", httpContent);
                string responseContent = await _res.Content.ReadAsStringAsync();

                AccountStatementDetails[] result = JsonConvert.DeserializeObject<AccountStatementDetails[]>(responseContent);
                foreach (var item in result)
                {
                    AccountStatementDetails jsonDetail = new AccountStatementDetails();
                    jsonDetail.Balance = item.Balance;
                    jsonDetail.Credit = item.Credit;
                    jsonDetail.Debit = item.Debit;
                    jsonDetail.Particulars = item.Particulars;
                    jsonDetail.TranDate = item.TranDate;
                    jsonDetail.TRAN_DATE = item.TRAN_DATE;
                    jsonDetail.ValueDate = item.ValueDate;
                    list.Add(jsonDetail);
                }
                return list;
            }
        }
        #endregion

        #region AccountDetails
        public async Task<List<AccountDetails>> AccountDetails(string accountNumber)
        {
            List<AccountDetails> list = new List<AccountDetails>();

            var APIBaseURL = ConfigurationManager.AppSettings["APIBaseURL"];
            var AuthUserName = ConfigurationManager.AppSettings["AuthUserName"];
            var AuthPassword = ConfigurationManager.AppSettings["AuthPassword"];

            HttpResponseMessage _res = new HttpResponseMessage();
            var accountObject = new AccountDetailModel
            {
                AccountNumber = accountNumber
            };
            // Serialize our concrete class into a JSON String
            var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(accountObject));
            // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
            var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
            using (HttpClient httpClient = new HttpClient())
            {
                var byteArray = Encoding.ASCII.GetBytes(AuthUserName + ":" + AuthPassword);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                _res = await httpClient.PostAsync(APIBaseURL + "Account/AccountDetails", httpContent);
                string responseContent = await _res.Content.ReadAsStringAsync();

                AccountDetails[] result = JsonConvert.DeserializeObject<AccountDetails[]>(responseContent);
                foreach (var item in result)
                {
                    AccountDetails jsonDetail = new AccountDetails();
                    jsonDetail.FORACID = item.FORACID;
                    jsonDetail.ACCT_NAME = item.ACCT_NAME;
                    jsonDetail.Curr = item.Curr;
                    jsonDetail.Scheme = item.Scheme;
                    jsonDetail.Address = item.Address;
                    jsonDetail.Branch = item.Branch;
                    jsonDetail.PASSCODE = item.PASSCODE;
                    jsonDetail.EMAIL_ID = item.EMAIL_ID;
                    jsonDetail.TEL_NO = item.TEL_NO;
                    jsonDetail.CS_EMAIL = item.CS_EMAIL;
                    list.Add(jsonDetail);
                }
                return list;
            }
        }
        #endregion

        #region AccountBalance
        public async Task<List<AccountBalance>> AccountBalance(string accountNumber)
        {
            List<AccountBalance> list = new List<AccountBalance>();

            var APIBaseURL = ConfigurationManager.AppSettings["APIBaseURL"];
            var AuthUserName = ConfigurationManager.AppSettings["AuthUserName"];
            var AuthPassword = ConfigurationManager.AppSettings["AuthPassword"];

            HttpResponseMessage _res = new HttpResponseMessage();
            var accountObject = new AccountDetailModel
            {
                AccountNumber = accountNumber
            };
            // Serialize our concrete class into a JSON String
            var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(accountObject));
            // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
            var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
            using (HttpClient httpClient = new HttpClient())
            {
                var byteArray = Encoding.ASCII.GetBytes(AuthUserName + ":" + AuthPassword);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                _res = await httpClient.PostAsync(APIBaseURL + "Account/AccountBalance", httpContent);
                string responseContent = await _res.Content.ReadAsStringAsync();

                AccountBalance[] result = JsonConvert.DeserializeObject<AccountBalance[]>(responseContent);
                foreach (var item in result)
                {
                    AccountBalance jsonDetail = new AccountBalance();
                    jsonDetail.OpeningBalance = item.OpeningBalance;
                    jsonDetail.TotalDebit = item.TotalDebit;
                    jsonDetail.DebitEntries = item.DebitEntries;
                    jsonDetail.TotalCredit = item.TotalCredit;
                    jsonDetail.CreditEntries = item.CreditEntries;
                    jsonDetail.ClosingBalance = item.ClosingBalance;
                    jsonDetail.From = item.From;
                    jsonDetail.To = item.To;

                    list.Add(jsonDetail);
                }
                return list;
            }
        }
        #endregion
    }
}
