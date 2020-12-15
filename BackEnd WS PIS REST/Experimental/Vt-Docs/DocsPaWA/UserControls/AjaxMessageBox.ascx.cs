using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocsPAWA.UserControls
{
    public partial class AjaxMessageBox : System.Web.UI.UserControl
    {

        /// <summary>
        /// Funzione per mostrare un messaggio
        /// </summary>
        /// <param name="message">Il messaggio da mostrare</param>
        public void ShowMessage(String message)
        {
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "myFunc", "alert('"+ message +"');", true);
        }


    }

}