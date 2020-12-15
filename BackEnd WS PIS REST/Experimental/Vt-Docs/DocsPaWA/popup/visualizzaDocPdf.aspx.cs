using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace DocsPAWA.popup
{
    public partial class visualizzaDocPdf : DocsPAWA.CssPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
           
            
            Response.Expires = -1;
            DocsPaWR.FileDocumento theDoc = null;
            string PosLabelPdf = null;
            DocsPaWR.labelPdf label = new DocsPAWA.DocsPaWR.labelPdf();
            bool tipo = false;
            string rotazione = null;
            string orientamento = null;
            string carattere = string.Empty;
            string colore = string.Empty;
            
            DocsPaWR.SchedaDocumento schedaCorrente = DocumentManager.getDocumentoSelezionato(Page);

            DocsPaVO.documento.FileRequest fr = new DocsPaVO.documento.FileRequest();

            if (this.Session["personalize"] != null)
            {
                PosLabelPdf = this.Session["personalize"].ToString();
            }
            if (Session["tipoLabel"] != null)
            {
                tipo = System.Convert.ToBoolean(Session["tipoLabel"].ToString());
            }
            if (Session["rotazione"] != null)
            {
                rotazione = Session["rotazione"].ToString();
            }
            if (Session["carattere"] != null)
            {
                carattere = Session["carattere"].ToString();
            }
            if (Session["colore"] != null)
            {
                colore = Session["colore"].ToString();
            }
            if (Session["orientamento"] != null)
            {
                orientamento = Session["orientamento"].ToString();
            }
            //carico i dati dentro l'oggetto Label
            label.position = PosLabelPdf;
            label.tipoLabel = tipo;
            label.label_rotation = rotazione;
            label.sel_font = carattere;
            label.sel_color = colore;
            label.orientamento = orientamento;
            theDoc = FileManager.getInstance(Session.SessionID).getVoidFileConSegnatura(fr, schedaCorrente, label, this);

                //aggiungo in session le info relative alle label
                Session.Add("labelProperties", theDoc.LabelPdf);
                if (theDoc != null)
                {
                    Response.ContentType = theDoc.contentType;
                    Response.AddHeader("content-disposition", "inline;filename=" + theDoc.name);
                    Response.AddHeader("content-length", theDoc.content.Length.ToString());
                    Response.BinaryWrite(theDoc.content);
                }
            }
        }
    }
