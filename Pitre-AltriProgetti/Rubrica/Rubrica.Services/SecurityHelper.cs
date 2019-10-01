using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Rubrica.Library.Security;

namespace Rubrica
{
    /// <summary>
    /// Classe di utilità per la gestione della security
    /// </summary>
    internal sealed class SecurityHelper
    {
        /// <summary>
        /// Login utente
        /// </summary>
        /// <param name="credentials"></param>
        public static void Login(SecurityCreadentialsSoapHeader credentials)
        {
            if (!RubricaPrincipal.Login(new SecurityCredentials { UserName = credentials.UserName, Password = credentials.Password }))
                throw new System.Security.SecurityException(Properties.Resources.InvalidCredentialsException);
        }
    }
}
