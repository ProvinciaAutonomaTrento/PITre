using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDataWA.Utils;
using NttDatalLibrary;

namespace NttDataWA.Popup
{
    public partial class MassiveVisibilityRemove : System.Web.UI.Page
    {

        #region Properties

        private bool IsFasc
        {
            get
            {
                return !string.IsNullOrEmpty(Request.QueryString["objType"]) && Request.QueryString["objType"].Equals("P") ? true : false;
            }
        }

        protected List<String> ListCheck
        {
            get
            {
                List<String> result = null;
                if (HttpContext.Current.Session["visibility.listCheck"] != null)
                {
                    result = HttpContext.Current.Session["visibility.listCheck"] as List<String>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["visibility.listCheck"] = value;
            }
        }

        protected DocumentoDiritto[] VisibilityList
        {
            get
            {
                DocumentoDiritto[] result = null;
                if (HttpContext.Current.Session["visibilityList"] != null)
                {
                    result = HttpContext.Current.Session["visibilityList"] as DocumentoDiritto[];
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["visibilityList"] = value;
            }
        }

        private int MaxLenghtNote
        {
            get
            {
                return 128;
            }
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.InitializePage();
            }

            this.RefreshScript();
        }

        protected void BtnClose_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
            this.CloseMask(true);
        }

        protected void BtnConfirm_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);

            // Lista elementi Non rimossi
            List<string> listNotRemoved = new List<string>();

            // Lista elementi rimossi
            List<string> listRemoved = new List<string>();

            foreach (string index in this.ListCheck)
            {
                if (!this.VisibilityList[int.Parse(index)].deleted)
                {
                    //Verifica se si vuole rimuovere acl a un ruolo o un utente
                    string personOrGroup = this.VisibilityList[int.Parse(index)].soggetto.tipoCorrispondente;

                    //Identifico un elemento in dt che ritrovo nella listaDocDir
                    string idCorr = this.VisibilityList[int.Parse(index)].personorgroup;
                    string diritti = this.setTipoDiritto(this.VisibilityList[int.Parse(index)]);
                    string tipodiritto = this.VisibilityList[int.Parse(index)].tipoDiritto.ToString();

                    int indice = 0;
                    if (!this.VisibilityList[int.Parse(index)].deleted)
                        indice = int.Parse(index);

                    DocumentoDiritto docDiritti = this.VisibilityList[indice];
                    docDiritti.personorgroup = personOrGroup;
                    docDiritti.note = this.txtNote.Text;

                    bool result;
                    if (this.IsFasc)
                        result = ProjectManager.editingFascACL(this.GetProjectFromDocument(docDiritti), personOrGroup, UserManager.GetInfoUser());
                    else
                        result = DocumentManager.editingACL(docDiritti, personOrGroup, UserManager.GetInfoUser());

                    if (result)
                    {
                        // OK
                        // Attualmente non utilizzata
                        // Utilizzabile nell'eventualità di un reoprt
                        listRemoved.Add(this.VisibilityList[int.Parse(index)].soggetto.descrizione);
                    }
                    else
                    {
                        //Popolo una lista di stringhe con gli elementi che non sono stati rimossi dalla ACL
                        listNotRemoved.Add(this.VisibilityList[int.Parse(index)].soggetto.descrizione);
                    }

                }
            }

            this.litMessage.Text = "";
            // Resoconto degli elementi andati a buon fine
            if (listRemoved != null && listRemoved.Count > 0)
            {
                string resultConcat = string.Empty;
                foreach (string tempMess in listRemoved)
                {
                    resultConcat += utils.FormatJs(tempMess) + "<br />\n";
                }
                // Report degli elementi che non sono stati rimossi
                this.litMessage.Text += "<p>"+this.GetLabel("MassiveVisibilityReport") + "<br />\n" + resultConcat+"</p>\n";
            }
            // Resoconto degli elementi non andati a buon fine
            if (listNotRemoved != null && listNotRemoved.Count > 0)
            {
                string resultConcat = string.Empty;
                foreach (string tempMess in listNotRemoved)
                {
                    resultConcat += utils.FormatJs(tempMess) + "<br />\n";
                }
                // Report degli elementi che non sono stati rimossi
                this.litMessage.Text += "<p>"+this.GetLabel("MassiveVisibilityErrReport") + "<br />\n" + resultConcat+"</p>\n";
            }
            this.UpPnlMessage.Update();

            this.plcNote.Visible = false;
            this.UpPnlNote.Update();

            this.BtnConfirm.Enabled = false;
            this.UpPnlButtons.Update();
        }

        #endregion

        #region Methods

        protected void InitializePage()
        {
            this.InitializeLanguage();
            this.InitializeMessage();
        }

        private void InitializeMessage()
        {
            bool hasItemsToDelete = false;
            if (this.IsFasc)
                this.litMessage.Text = "<p>" + this.GetLabel("MassiveVisibilityRemoveMsgProject") + "<br />\n";
            else
                this.litMessage.Text = "<p>"+this.GetLabel("MassiveVisibilityRemoveMsg")+"<br />\n";

            string mess = string.Empty;
            foreach (string index in this.ListCheck)
            {
                if (!this.VisibilityList[int.Parse(index)].deleted)
                {
                    hasItemsToDelete = true;
                    string elementoDaRimuovere = string.Empty;
                    elementoDaRimuovere = "- " + this.VisibilityList[int.Parse(index)].soggetto.descrizione;
                    this.litMessage.Text += elementoDaRimuovere + "<br />\n";
                }
                else
                {
                    //elementi che non è possibile rimuovere
                    string elementoNonRemovibile = string.Empty;
                    elementoNonRemovibile = "- " + this.VisibilityList[int.Parse(index)].soggetto.descrizione;
                    mess = mess + elementoNonRemovibile + "<br />\n";
                }
            }

            // composizione messaggio per elementi non removibili
            if (!string.IsNullOrEmpty(mess))
            {
                if (hasItemsToDelete)
                    this.litMessage.Text += "</p>\n"
                                        + "<p>" + this.GetLabel("MassiveVisibilityNotRemovableMsg") + "<br />\n"
                                        + mess + "</p>\n";
                else
                {
                    this.BtnConfirm.Enabled = false;
                    this.UpPnlButtons.Update();

                    this.litMessage.Text = "<p>" + this.GetLabel("MassiveVisibilityNotRemovableMsg") + "<br />\n"
                                        + mess + "</p>\n";
                }
            }
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.BtnConfirm.Text = Utils.Languages.GetLabelFromCode("GenericBtnOk", language);
            this.BtnClose.Text = Utils.Languages.GetLabelFromCode("GenericBtnClose", language);
            this.litNote.Text = Utils.Languages.GetLabelFromCode("MassiveVisibilityNote", language);
            this.litNotesChars.Text = Utils.Languages.GetLabelFromCode("MassiveVisibilityNoteChars", language);
        }

        private string GetLabel(string id)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            return Utils.Languages.GetLabelFromCode(id, language);
        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "charsLeft", "charsLeft('txtNote', " + this.MaxLenghtNote + ", '" + utils.FormatJs(this.litNote.Text) + "');", true);
        }

        private void CloseMask(bool withReturnValue)
        {
            string retValue = withReturnValue ? "true" : "false";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "closeAjaxModal", "parent.closeAjaxModal('MassiveVisibilityRemove', '" + retValue + "');", true);
        }

        private string setTipoDiritto(DocumentoDiritto docDir)
        {
            string star = (docDir.hideDocVersions ? Environment.NewLine + "*" : "");

            if (docDir.tipoDiritto.Equals(DocumentoTipoDiritto.TIPO_ACQUISITO))
            {
                if (docDir.accessRights == (int)HMdiritti.HDdiritti_Waiting)
                    return "IN ATTESA DI ACCETTAZIONE" + star;
                else
                    return "ACQUISITO" + star;
            }
            else
                if (docDir.tipoDiritto.Equals(DocumentoTipoDiritto.TIPO_PROPRIETARIO))
                    return "PROPRIETARIO" + star;
                else
                    if (docDir.tipoDiritto.Equals(DocumentoTipoDiritto.TIPO_TRASMISSIONE))
                        return "TRASMISSIONE" + star;
                    else
                        if (docDir.tipoDiritto.Equals(DocumentoTipoDiritto.TIPO_TRASMISSIONE_IN_FASCICOLO))
                            return "INSERIMENTO IN FASC." + star;
                        else
                            if (docDir.tipoDiritto.Equals(DocumentoTipoDiritto.TIPO_SOSPESO))
                                return "SOSPESO" + star;
                            else
                                if (docDir.tipoDiritto.Equals(DocumentoTipoDiritto.TIPO_DELEGATO))
                                    return "PROPRIETARIO" + star;
            return "";
        }

        /// <summary>
        /// Metodo per la generazione di una descrizione estesa del tipo diritto
        /// </summary>
        /// <param name="accessRight">Diritto di accesso</param>
        /// <returns>Descrizione del tipo di diritto</returns>
        private String GetRightDescription(int accessRight)
        {
            String retVal = String.Empty;

            switch (accessRight)
            {
                case 0:
                case 255:
                case 63:
                    retVal = "Lettura / Scrittura";
                    break;
                case 45:
                case 20:
                    retVal = "Lettura";
                    break;
                default:
                    break;

            }

            return retVal;

        }

        private FascicoloDiritto GetProjectFromDocument(DocumentoDiritto docs)
        {
            FascicoloDiritto projects = new FascicoloDiritto();

            projects.accessRights = docs.accessRights;
            projects.Checked = docs.Checked;
            projects.deleted = docs.deleted;
            projects.dtaInsSecurity = docs.dtaInsSecurity;
            projects.idObj = docs.idObj;
            projects.note = docs.note;
            projects.noteSecurity = docs.noteSecurity;
            projects.personorgroup = docs.personorgroup;
            projects.removed = docs.removed;
            projects.soggetto = docs.soggetto;
            switch (docs.tipoDiritto)
            {
                case DocumentoTipoDiritto.TIPO_ACQUISITO:
                    projects.tipoDiritto = FascicoloTipoDiritto.TIPO_ACQUISITO;
                    break;
                case DocumentoTipoDiritto.TIPO_DELEGATO:
                    projects.tipoDiritto = FascicoloTipoDiritto.TIPO_DELEGATO;
                    break;
                case DocumentoTipoDiritto.TIPO_PROPRIETARIO:
                    projects.tipoDiritto = FascicoloTipoDiritto.TIPO_PROPRIETARIO;
                    break;
                case DocumentoTipoDiritto.TIPO_SOSPESO:
                    projects.tipoDiritto = FascicoloTipoDiritto.TIPO_SOSPESO;
                    break;
                case DocumentoTipoDiritto.TIPO_TRASMISSIONE:
                    projects.tipoDiritto = FascicoloTipoDiritto.TIPO_TRASMISSIONE;
                    break;
            }
            projects.rootFolder = UIManager.ProjectManager.getProjectInSession().folderSelezionato.systemID;

            return projects;
        }

        #endregion

    }
}