using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NttDataWA.Utils
{
    public class SSOAuthTokenHelper
    {
           /// <summary>
        /// Authentication token prefix
        /// </summary>
        private const string TOKEN_PREFIX = "SSO=";

        /// <summary>
        /// Format value encrypted
        /// </summary>
        /// <remarks>
        /// {0} = UserId
        /// {1} = SessionId
        /// {2} = Data erogazione token
        /// </remarks>
        private const string ENCRYPTED_VALUE_FORMAT = "UID={0};SESSIONID={1};DATETIME={2}";
        private const string UID = "UID";
        private const string SESSIONID = "SESSIONID";
        private const string DATETIME = "DATETIME";

        /// <summary>
        /// 
        /// </summary>
        private SSOAuthTokenHelper()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static string GetSessionId()
        {
            try
            {
                if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Session != null)
                    return System.Web.HttpContext.Current.Session.SessionID;
                else
                    return Guid.NewGuid().ToString();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Generate Token
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string Generate(string userId, string language)
        {
            try
            {
                CryptoString crypto = new CryptoString(userId);
                string encodedValue = crypto.Encrypt(string.Format(ENCRYPTED_VALUE_FORMAT, userId, GetSessionId(), DateTime.Now));

                return string.Format("{0}{1}", TOKEN_PREFIX, encodedValue);
            }
            //catch (Exception ex)
            //{
            //    throw new ApplicationException(Utils.Languages.GetMessageFromCode("ErrorTokenGenerator", language), ex);
            //}
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

    }
}