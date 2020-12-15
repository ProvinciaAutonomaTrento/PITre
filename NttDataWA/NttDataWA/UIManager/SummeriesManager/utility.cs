using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.Web.UI;
using NttDataWA.DocsPaWR;
using System.Web.UI.WebControls;

namespace NttDataWA.UIManager.SummeriesManager
{
    /// <summary>
    /// utility: contiene funzioni di utilità varia.
    /// </summary>
    public class utility
    {
        ///// <summary>
        ///// do_alert: mostra un finestra di alert
        ///// </summary>
        ///// <param name="page">Pagina da cui viene richiamato il metodo</param>
        ///// <param name="message">messagio da mostrare</param>
        //public static void do_alert(Page page, string message)
        //{
        //    message = StringUtils.do_formatString(message);
        //    page.Response.Write("<script>alert('" + message + "');</script>");
        //}

        ///// <summary>
        ///// do_windowOpen: apre una nuova finestra
        ///// </summary>
        ///// <param name="page">Pagina da cui viene richiamato il metodo</param>
        ///// <param name="url">url della pagina da aprire</param>
        //public static void do_windowOpen(Page page, string url)
        //{
        //    page.RegisterStartupScript("open", "<script>var _width = (screen.width-10);var _height = (screen.height-90);window.open('" + url + "','_blank','top=0,left=0,width='+_width+',height='+_height+',fullscreen=no,toolbar=no,directories=no,status=yes,menubar=no,resizable=yes, scrollbars=auto');</script>");
        //}

        ///// <summary>
        ///// do_openinRightFrame
        ///// </summary>
        ///// <param name="page">Pagina da cui viene richiamato il metodo</param>
        ///// <param name="url">url da aprire nel frame destro</param>
        //public static void do_openinRightFrame(Page page, string url)
        //{
        //    string funct_dx2 = "top.principale.frames[1].location='" + url + "'";
        //    page.Response.Write("<script> " + funct_dx2 + "</script>");
        //}

        ///// <summary>
        ///// setSelectedFileReport: imposta il file report
        ///// </summary>
        ///// <param name="page">Pagina da cui viene richiamato il metodo</param>
        ///// <param name="fileDoc">file da impostare</param>
        ///// <param name="initialPath">repository</param>
        //public static void setSelectedFileReport(Page page, DocsPaWR.FileDocumento fileDoc, string initialPath)
        //{
        //    page.Session["FileManager.selectedReport"] = fileDoc;
        //    string visReportPath = "";
        //    if (initialPath != "")
        //    {
        //        visReportPath = initialPath + "/visualizzaReport.aspx";
        //    }
        //    else
        //    {
        //        visReportPath = "ProspettiRiepilogativi_RF.aspx";
        //    }
        //    //carica la new pg	
        //    //string funct_dx2 = "top.principale.iFrame_dettagli.document.location='"+url+"'";
        //    //page.Response.Write("<script> " + funct_dx2 + "</script>");
        //    string funct_dx = "top.principale.iFrame_dettagli.document.location='" + visReportPath + "'";
        //    //string  funct_dx = " window.open('"+visReportPath+"','main')"; 
        //    page.Response.Write("<script> " + funct_dx + "</script>");
        //}

        //public static void SetSelectedFileReport(Page page, FileDocumento fileDoc, bool showAsAttachment)
        //{
        //    // Se showAsAttachment è true, viene richiamata la pagina ShowReportAsAttachment.aspx
        //    // Questa chiamata risolve il problema riscontrato in corte dei conti, per cui un report
        //    // visualizzato come attachment lascia la pagina dietro con la scritta "Stampa in corso..." e
        //    // a volte apre il file all'interno della stessa finestra nonostante il content disposition sia 
        //    // attachment
        //    page.Session["FileManager.selectedReport"] = fileDoc;

        //    if (showAsAttachment)
        //    {
        //        string funct_dx = "top.principale.iFrame_dettagli.document.location='ShowReportAsAttachment.aspx'";
        //        page.Response.Write("<script> " + funct_dx + "</script>");
        //    }
        //    else
        //        setSelectedFileReport(page, fileDoc, String.Empty);

        //}


        /// <summary>
        /// getSelectedFileReport: ritorna il file report
        /// </summary>
        /// <param name="page">Pagina da cui viene richiamato il metodo</param>
        /// <returns>file report</returns>
        public static object getSelectedFileReport(Page page)
        {
            return page.Session["FileManager.selectedReport"];
        }

        public static void RemoveFileReport(Page page)
        {
            page.Session.Remove("FileManager.selectedReport");
        }

        /// <summary>
        /// DO_OpenPage: apre una nuova pagina
        /// </summary>
        /// <param name="page">Pagina da cui viene richiamato il metodo</param>
        /// <param name="url">url della pagina da aprire</param>
        public static void DO_OpenPage(Page page, string url)
        {
            page.Response.Redirect(url);
        }

        /// <summary>
        /// DO_UpdateParameters: Aggiorna i parametri da stampare
        /// </summary>
        /// <param name="parametri">ArrayList contenente i vari parametri</param>
        /// <param name="current">parametro da stampare</param>
        public static ArrayList DO_UpdateParameters(ArrayList parametri, Parametro current)
        {
            bool trovato = false;
            foreach (Parametro p in parametri)
            {
                if (p.Nome == current.Nome)
                {
                    p.Descrizione = current.Descrizione;
                    p.Valore = current.Valore;
                    trovato = true;
                }
            }

            if (!trovato)
            {
                parametri.Add(current);
            }
            return parametri;
        }

        /// <summary>
        /// DO_UpdateParameters
        /// </summary>
        /// <param name="parametri">ArrayList contenente i vari parametri</param>
        /// <param name="current">parametro da stampare</param>
        /// <param name="update">true = Aggiorna il parametro; false = non aggiornare il parametro</param>
        /// <returns></returns>
        public static ArrayList DO_UpdateParameters(ArrayList parametri, Parametro current, bool update)
        {
            bool trovato = false;
            foreach (Parametro p in parametri)
            {
                if (p.Nome == current.Nome)
                {
                    if (update)
                    {
                        p.Descrizione = current.Descrizione;
                        p.Valore = current.Valore;
                    }
                    trovato = true;
                }
            }
            if (!trovato)
            {
                parametri.Add(current);
            }
            return parametri;
        }

        /// <summary>
        /// Funzione per settare il focus ad un controllo
        /// </summary>
        /// <param name="page">pagina contenente il controllo</param>
        /// <param name="control">controllo sul quale impostare il focus</param>
        public static void SetFocus(Page page, object control)
        {
            // verifica che il controllo a cui si vuole dare il focus non sia nullo
            if (control == null)
                return;

            // rileva l'ID univoco del controllo
            string ctrlID = ((WebControl)control).UniqueID;

            // implementa scripting client-side per focus
            string script;
            script = "<script language=\"javascript\">";
            script += "var control = document.getElementById(\"" + ctrlID + "\");";
            script += "if(control!=null){control.focus();";
            // se è un TextBox, oltre a dare il focus, seleziona il testo
            if (control.GetType().ToString() == "System.Web.UI.WebControls.TextBox")
            {
                script += "control.select();";
            }
            script += "}<";
            script += "/script>";

            // "inietta" il codice client-side nell'output HTML della pagina
            page.RegisterStartupScript("Focus", script);
        }



        private static string DO_GetFileName(string path)
        {
            string[] tmp = path.Split('\\');
            return tmp[tmp.Length - 1];
        }
    }

    /// <summary>
    /// StringUtils: contiene funzioni di utilità 
    /// per le stringhe.
    /// </summary>
    public class StringUtils
    {
        public static string do_formatString(string s)
        {
            s = s.Replace("'", "\\'");
            s = s.Trim();
            return s;
        }
    }

}