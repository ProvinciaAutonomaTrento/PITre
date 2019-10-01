using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.UIManager;
using NttDataWA.DocsPaWR;
using NttDataWA.CheckInOut;

namespace NttDataWA.Popup
{
    public partial class AbortCounter : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (!IsPostBack)
                {
                    this.InitializeLanguage();
                    // Verifica se il documento è in stato checkout
                    bool isCheckedOut = CheckInOutServices.IsCheckedOutDocument();
                    //Verifica se il documento si trova nello stato finale
                    DocsPaWR.Stato statoDoc = UIManager.DiagrammiManager.GetStateDocument(UIManager.DocumentManager.getSelectedRecord().docNumber);
                    bool isFinalState = statoDoc != null ? statoDoc.STATO_FINALE : false;
                    //verifica se il documento deve essere ancora accettato a seguito di una trasmissione
                    bool isToAccept = UserManager.disabilitaButtHMDirittiTrasmInAccettazione(DocumentManager.getSelectedRecord().accessRights);
                    //verifico se il repertorio può essere eliminato
                    bool isNotCancel = isCheckedOut || isFinalState || isToAccept;

                    string language = UIManager.UserManager.GetUserLanguage();
                    // preparo l'eventuale motivo del mancato annullamento
                    if (isCheckedOut)
                    {
                        this.AbortCounterLliDesc.Text = Utils.Languages.GetLabelFromCode("ErroDocumentCheckOut", language) + "<br/>";
                    }
                    if (isFinalState)
                    {
                        this.AbortCounterLliDesc.Text = Utils.Languages.GetLabelFromCode("ErroDocumentFinaleState", language) + "<br/>";
                    }
                    if (isToAccept)
                    {
                        this.AbortCounterLliDesc.Text = Utils.Languages.GetLabelFromCode("ErroDocumentNotAccept", language)  + "<br/>";
                    }

                    if (isNotCancel)
                    {
                        this.AbortCounterBtnOk.Enabled = false;
                    }
                    else
                    {
                    //    this.lbl_messageCheckOut.Style.Add("display", "none");
                    //    this.lbl_messageCheckOut_descrizione.Style.Add("display", "none");
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.AbortCounterBtnOk.Text = Utils.Languages.GetLabelFromCode("AbortCounterBtnOk", language);
            this.AbortCounterBtnClose.Text = Utils.Languages.GetLabelFromCode("AbortCounterBtnClose", language);
            this.AbortCounterLliDesc.Text = Utils.Languages.GetLabelFromCode("AbortCounterLliDesc", language);
        }

        protected void AbortCounterBtnClose_Click(object sender, EventArgs e)
        {
            Response.Write("<html><body><script type=\"text/javascript\">parent.closeAjaxModal('AbortCounter', '');</script></body></html>");
            Response.End();
        }

        protected void AbortCounterBtnOk_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
            if (!String.IsNullOrEmpty(this.TxtTextAborCounter.Text) && !String.IsNullOrEmpty(this.IdObjectCustom))
            {
                SchedaDocumento doc = DocumentManager.getSelectedRecord();
                string idOggetto = this.IdObjectCustom.Replace("btn_a_", string.Empty);
                //Annullamento
                ProfilerDocManager.AnnullaContatoreDiRepertorio(idOggetto, doc.docNumber);

                Templates tempTemp = ProfilerDocManager.getTemplateDettagli(doc.docNumber);

                //Storicizzazione
                DocsPaWR.OggettoCustom oggettoCustom = tempTemp.ELENCO_OGGETTI.Where(oggetto => oggetto.SYSTEM_ID.ToString().Equals(idOggetto)).FirstOrDefault();
                DocsPaWR.Storicizzazione storico = new DocsPaWR.Storicizzazione();
                storico.ID_TEMPLATE = doc.template.SYSTEM_ID.ToString();
                storico.DATA_MODIFICA = oggettoCustom.DATA_ANNULLAMENTO;
                storico.ID_PROFILE = doc.docNumber;
                storico.ID_OGG_CUSTOM = oggettoCustom.SYSTEM_ID.ToString();
                storico.ID_PEOPLE = UserManager.GetInfoUser().idPeople;
                storico.ID_RUOLO_IN_UO = UserManager.GetInfoUser().idCorrGlobali;
                storico.DESC_MODIFICA = this.TxtTextAborCounter.Text.Replace("'", "''");

                ProfilerDocManager.Storicizza(storico);

                for (int i = 0; i < doc.template.ELENCO_OGGETTI.Length; i++)
                {
                    if (doc.template.ELENCO_OGGETTI[i].SYSTEM_ID.ToString().Equals(idOggetto))
                    {
                        doc.template.ELENCO_OGGETTI[i] = oggettoCustom;
                        break;
                    }
                    
                }


                DocumentManager.setSelectedRecord(doc);

                Response.Write("<html><body><script type=\"text/javascript\">parent.closeAjaxModal('AbortCounter', 'up');</script></body></html>");
                Response.End();
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('La motivazione è obbligatoria.');", true);
            }
        }

        public string IdObjectCustom
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["idObjectCustom"] != null)
                {
                    result = HttpContext.Current.Session["idObjectCustom"].ToString();
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["idObjectCustom"] = value;
            }
        }

    }
}