using DocsPaUtils.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.Mobile
{
    public class VerifyUpdate
    {
        public static string getUrlUpdate(string version, string model , string brand)
        {
            //    DocsPaDB.Query_DocsPAWS.Mobile.DocumentiMobile doc = new DocsPaDB.Query_DocsPAWS.Mobile.DocumentiMobile();
            //string s = Utils.InitConfigurationKeys.GetValue("0", Utils.DBKeys.FE_URL_SITOACCESSIBILE.ToString());
           // InitConfigurationKeys.GetValue("0", "FE_URL_SITOCOMPLETO");
            string lastVersion = InitConfigurationKeys.GetValue("0", "BE_VER_BUILD_"+brand);
            string url = "";

            try
            {
                Version mobileVersion = Version.Parse(version);
                Version dbVersion = Version.Parse(lastVersion);
                if (mobileVersion < dbVersion)
                {
                    url = InitConfigurationKeys.GetValue("0", "BE_URL_BUILD_" + brand); 
                }
            }
            catch (Exception e)
            {

            }

    
            return url;
        }
    }
}
