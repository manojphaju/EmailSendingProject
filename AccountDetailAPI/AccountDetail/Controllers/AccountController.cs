using AccountDetail.Models;
using MNepalAPI.BasicAuthentication;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace AccountDetail.Controllers
{
    [MyBasicAuthenticationFilter]
    public class AccountController : ApiController
    {
        #region StatementDetails
        [Route("api/Account/StatementDetails")]
        [HttpPost]
        public async Task<HttpResponseMessage> StatementDetails(AcDetails accountNumber)
        {
            try
            {
                string strAccountStatementDetails = @" SELECT  TO_CHAR(T.TRAN_DATE,'DD-MM-YYYY') ""TranDate"", trim( T.TRAN_PARTICULAR || ' '||T.TRAN_RMKS|| ' ' ||T.REF_NUM) ""Particulars"",
        TO_CHAR(T.VALUE_DATE,'DD-MM-YYYY') ""ValueDate"",
        trim( TO_CHAR( nvl(CASE WHEN T.PART_TRAN_TYPE ='D' THEN T.TRAN_AMT ELSE NULL END,0) ,'999,999,999,990.99')) ""Debit"",
        trim( TO_CHAR( nvl(CASE WHEN T.PART_TRAN_TYPE ='C' THEN T.TRAN_AMT ELSE NULL END,0),'999,999,999,990.99')) ""Credit"", '0.00' AS ""Balance"",TRAN_DATE FROM HTD T ,GAM G 
        WHERE T.ACID=G.ACID  
        and g.foracid = '" + accountNumber.AccountNumber + @"'
        AND  T.DEL_FLG ='N' and t.pstd_flg = 'Y'  
AND T.TRAN_DATE BETWEEN '12-NOV-2021' AND '25-NOV-2021'
        UNION ALL
        SELECT TO_CHAR(E.EOD_DATE,'DD-MM-YYYY'),'~Date summary',NULL,NULL,NULL, TO_CHAR(nvl(E.TRAN_DATE_BAL,0) +10000000,'999,999,999,999.99'),E.EOD_DATE FROM EAB E, GAM G 
        WHERE E.ACID = G.ACID 
        AND G.FORACID = '" + accountNumber.AccountNumber + @"'
        AND E.EOD_DATE = (select TBAADM.GETNCHECKWORKINGDAY(trunc(sysdate-5)) from dual)
ORDER BY 7,2  ";





                OracleDataReader oraODR1 = null;
                OracleConnection oraConn = new OracleConnection(ConfigurationManager.ConnectionStrings["NiblConnectionString"].ConnectionString);
                OracleCommand oraCmd = new OracleCommand(strAccountStatementDetails, oraConn);

                DataTable dt = new DataTable("Data");
                oraConn.Open();
                oraODR1 = oraCmd.ExecuteReader();
                dt.Load(oraODR1);

                string json = JsonConvert.SerializeObject(dt, Formatting.Indented);
                var jsonData = JsonConvert.DeserializeObject(json);
                oraConn.Close();
                return Request.CreateResponse(HttpStatusCode.OK, jsonData);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        #region AccountDetails
        [Route("api/Account/AccountDetails")]
        [HttpPost]
        public async Task<HttpResponseMessage> AccountDetails(AcDetails accountNumber)
        {
            try
            {
                string strAccountDetailsQuery = @" SELECT FORACID ,ACCT_NAME ,ACCT_CRNCY_CODE ""Curr"",
(select schm_desc from gsp where schm_code =g.schm_code and del_flg ='N') ""Scheme"" ,
CUST_COMU_ADDR1 ""Address"" , 
INITCAP((select sol_desc from sol where sol_id  =g.sol_id and del_flg ='N')) ""Branch"",
					 TO_CHAR(DATE_OF_BIRTH,'MMYYYY') PASSCODE,EMAIL_ID,
(SELECT TEL_NO FROM TBAADM.C_SOL_ADDRESS WHERE SOL_ID =C.PRIMARY_SOL_ID)TEL_NO,
(SELECT CS_EMAIL FROM TBAADM.C_SOL_ADDRESS WHERE SOL_ID =C.PRIMARY_SOL_ID)CS_EMAIL
    FROM GAM G, CMG C WHERE G.CUST_ID = C.CUST_ID AND C.DEL_FLG ='N' 
	AND  FORACID ='" + accountNumber.AccountNumber + @"' ";

                OracleDataReader oraODR1 = null;
                OracleConnection oraConn = new OracleConnection(ConfigurationManager.ConnectionStrings["NiblConnectionString"].ConnectionString);
                OracleCommand oraCmd = new OracleCommand(strAccountDetailsQuery, oraConn);

                DataTable dt = new DataTable("Data");
                oraConn.Open();
                oraODR1 = oraCmd.ExecuteReader();
                dt.Load(oraODR1);

                string json = JsonConvert.SerializeObject(dt, Formatting.Indented);
                var jsonData = JsonConvert.DeserializeObject(json);
                oraConn.Close();
                return Request.CreateResponse(HttpStatusCode.OK, jsonData);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        #endregion

        #region
        [Route("api/Account/AccountBalance")]
        [HttpPost]
        public async Task<HttpResponseMessage> AccountBalance(AcDetails accountNumber)
        {
            try
            {
                string strAccountBalanceQuery = @"  
SELECT 
TRIM(TO_CHAR(TBAADM.COMMONPACKAGE.EABBAL_INACCCRNCY(G.ACID,(TBAADM.GETNCHECKWORKINGDAY((( TBAADM.GETNCHECKWORKINGDAY( TRUNC(SYSDATE) -1 ))  -1 )))),'999,999,999,990.99')) ""OpeningBalance"",
trim(to_char((nvl((select sum(CASE WHEN PART_TRAN_TYPE ='D' THEN TRAN_AMT ELSE 0 END) from htd 
where acid = g.acid and del_flg ='N' and pstd_flg ='Y' and tran_date =(select TBAADM.GETNCHECKWORKINGDAY(trunc(sysdate-1)) from dual)
) ,0)),'999,999,999,990.99')) ""TotalDebit"",
trim(to_char(nvl((select sum(CASE WHEN PART_TRAN_TYPE ='D' THEN 1 ELSE 0 END) from htd 
where acid = g.acid and del_flg ='N' and pstd_flg ='Y' and tran_date =(select TBAADM.GETNCHECKWORKINGDAY(trunc(sysdate-1)) from dual)
) ,0),'999,999,999,990.99')) ""DebitEntries"", 
trim(to_char( nvl((select sum(CASE WHEN PART_TRAN_TYPE ='C' THEN TRAN_AMT ELSE 0 END) from htd 
where acid = g.acid and  del_flg ='N' and pstd_flg ='Y' and tran_date =(select TBAADM.GETNCHECKWORKINGDAY(trunc(sysdate-1)) from dual)
) ,0),'999,999,999,990.99')) ""TotalCredit"",
trim(to_char(nvl((select sum(CASE WHEN PART_TRAN_TYPE ='C' THEN 1 ELSE 0 END) from htd 
where acid = g.acid and  del_flg ='N' and pstd_flg ='Y' and tran_date =(select TBAADM.GETNCHECKWORKINGDAY(trunc(sysdate-1)) from dual)
) ,0),'999,999,999,990.99')) ""CreditEntries"", 
TRIM(TO_CHAR(TBAADM.COMMONPACKAGE.EABBAL_INACCCRNCY(G.ACID,TBAADM.GETNCHECKWORKINGDAY(trunc(sysdate-1)) ),'999,999,999,990.99') )  ""ClosingBalance"",          
TO_CHAR(TBAADM.GETNCHECKWORKINGDAY(trunc(sysdate-5)) ,'DD-MM-YYYY') ""From"", 
            TO_CHAR(TBAADM.GETNCHECKWORKINGDAY(trunc(sysdate-1))  ,'DD-MM-YYYY') ""To""
            FROM GAM G 
            WHERE g.foracid = '" + accountNumber.AccountNumber + @"'
            AND   acct_Cls_flg ='N'";

                OracleDataReader oraODR1 = null;
                OracleConnection oraConn = new OracleConnection(ConfigurationManager.ConnectionStrings["NiblConnectionString"].ConnectionString);
                OracleCommand oraCmd = new OracleCommand(strAccountBalanceQuery, oraConn);

                DataTable dt = new DataTable("Data");
                oraConn.Open();
                oraODR1 = oraCmd.ExecuteReader();
                dt.Load(oraODR1);

                string json = JsonConvert.SerializeObject(dt, Formatting.Indented);
                var jsonData = JsonConvert.DeserializeObject(json);
                oraConn.Close();
                return Request.CreateResponse(HttpStatusCode.OK, jsonData);
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
    }
}
