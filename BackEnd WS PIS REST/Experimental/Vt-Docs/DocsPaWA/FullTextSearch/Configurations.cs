using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace DocsPAWA.FullTextSearch
{
    public class Configurations
    {
        /// <summary>
        /// 
        /// </summary>
        public static bool FullTextSearchEnabled
        {
            get
            {   
                int value;
                if (Int32.TryParse(System.Configuration.ConfigurationManager.AppSettings[ConfigSettings.KeysENUM.FULL_TEXT_SEARCH.ToString()], out value))
                    return (value > 0);
                else
                    return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static bool FullTextAlertMessageEnabled
        {
            get
            {
                bool retValue;
                if (bool.TryParse(System.Configuration.ConfigurationManager.AppSettings[ConfigSettings.KeysENUM.FULL_TEXT_ALERT_MESSAGE_ENABLED.ToString()], out retValue))
                    return retValue;
                else
                    return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static int FullTextMinTextLenght
        {
            get
            {
                int retValue;
                Int32.TryParse(System.Configuration.ConfigurationManager.AppSettings[ConfigSettings.KeysENUM.FULL_TEXT_MIN_TEXT_LENGTH.ToString()], out retValue);
                return retValue;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool CheckTextMinLenght(string text)
        {
            bool retValue = false;

            int minLenght = FullTextMinTextLenght;

            if (minLenght == 0)
            {
                retValue = true;
            }
            else
            {
                if (!string.IsNullOrEmpty(text) && text.Trim() != string.Empty)
                    retValue = (text.Length >= minLenght);
            }
            return retValue;
        }
    }
}
