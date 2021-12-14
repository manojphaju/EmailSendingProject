using System.Configuration;

namespace MNepalAPI.BasicAuthentication
{
    public class ApiSecurity
    {
        public string AuthenticateAndLoad(string uname, string password)
        {
            string strUName = ConfigurationManager.AppSettings["AuthUserName"];
            string strPassword = ConfigurationManager.AppSettings["AuthUserName"];

            if (uname == strUName && password == strPassword)
            {
                return "authenticate";
            }
            else
            {
                return null;
            }
        }
    }
}
