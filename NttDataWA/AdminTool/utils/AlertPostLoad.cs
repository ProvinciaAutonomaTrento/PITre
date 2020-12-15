using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/* ABBATANGELI GIANLUIGI
 * Visualizza un alert javascript dopo il caricamento della pagina 
 * Possiede il metodo da aggiungere alla pagina per KeepSessionAlive */
namespace SAAdminTool.utils
{
    /// <summary>
    /// Show an javascript alert after page load.
    /// </summary>
    public class AlertPostLoad
    {
        public static void OutOfMaxRowSearchable(System.Web.UI.Page page, int totalRow)
        {
            string valoreChiaveDB = SAAdminTool.utils.InitConfigurationKeys.GetValue(UserManager.getInfoUtente().idAmministrazione, "MAX_ROW_SEARCHABLE");
            if (valoreChiaveDB.Length==0)
                valoreChiaveDB = SAAdminTool.utils.InitConfigurationKeys.GetValue("0", "MAX_ROW_SEARCHABLE");
            //string message = "Il filtro scelto è debole e restituisce troppi risultati. (Trovati: " + totalRow.ToString() + " / Limite: " + valoreChiaveDB + ")";
            string message = "Con i criteri di ricerca inseriti il sistema ha restituito " + totalRow.ToString() + " elementi ma il numero massimo ammesso è pari a " + valoreChiaveDB + ". E’ necessario affinare la ricerca per procedere.";
            page.ClientScript.RegisterStartupScript(page.GetType(), "alert", "<script>alert(\"" + message + "\");</script>");
        }
        
        public static void GenericMessage(System.Web.UI.Page page, string message)
        {
            page.ClientScript.RegisterStartupScript(page.GetType(), "alert", "<script>alert(\"" + message + "\");</script>");
        }

        public static void KeepSessionAlive(System.Web.UI.Page page)
        {
            int milliseconds = 18000;
            string imgTag = "<img id=\"imgKA\" width=\"1\" height=\"1\" src=\"../Images/sessionrefresch.gif?\" alt=\".\" />";
            string javascriptFunction = "<script language=\"JavaScript\"> function keepAliveSession(imgName) { myImg = document.getElementById(imgName); if (myImg) myImg.src = myImg.src.replace(/\\?.*$/, '?' + Math.random()); } </script>";
            javascriptFunction += "<script language=\"JavaScript\"> window.setInterval(\"keepAliveSession('imgKA')\", " + milliseconds + "); </script>";
            page.ClientScript.RegisterClientScriptBlock(page.GetType(), "imgKA", imgTag);
            page.ClientScript.RegisterStartupScript(page.GetType(), "keepAlive", javascriptFunction);
        }
    }
}