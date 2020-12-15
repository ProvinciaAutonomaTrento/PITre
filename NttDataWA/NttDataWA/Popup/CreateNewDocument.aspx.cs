using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.UIManager;
using NttDataWA.DocsPaWR;
using NttDataWA.Utils;

namespace NttDataWA.Popup
{
    public partial class CreateNewDocument : System.Web.UI.Page
    {
        #region properties

        protected int MaxLenghtObject
        {
            get
            {
                int result = 2000;
                if (HttpContext.Current.Session["maxLenghtObject"] != null)
                {
                    result = int.Parse(HttpContext.Current.Session["maxLenghtObject"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["maxLenghtObject"] = value;
            }
        }

        private string ReturnValue
        {
            get
            {
                if ((HttpContext.Current.Session["ReturnValuePopup"]) != null)
                    return HttpContext.Current.Session["ReturnValuePopup"].ToString();
                else
                    return string.Empty;
            }
        }

        #endregion

        #region Const

        private const string CLOSE_POPUP_OBJECT = "closePopupObject";

        #endregion

        #region standard method
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack)
            {
                this.InitLanguage();
                this.LoadKey();
                this.InitPage();
            }
            this.RefreshScript();
        }

        private void InitLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.CreateNewDocumentBtnSave.Text = Utils.Languages.GetLabelFromCode("CreateNewDocumentBtnSave", language);
            this.CreateNewDocumentBtnCancel.Text = Utils.Languages.GetLabelFromCode("CreateNewDocumentBtnCancel", language);
            this.DocumentLitObject.Text = Utils.Languages.GetLabelFromCode("DocumentLitObject", language);
            this.DocumentImgObjectary.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgObjectary", language);
            this.DocumentLitObjectChAv.Text = Utils.Languages.GetLabelFromCode("DocumentLitObjectChAv", language);
            this.CreateNewDocumentProject.Text = Utils.Languages.GetLabelFromCode("CreateNewDocumentProject", language);
            this.CreateNewDocumentCodeProject.Text = Utils.Languages.GetLabelFromCode("CreateNewDocumentCodeProject", language);
            this.CreateNewDocumentDescriptionProject.Text = Utils.Languages.GetLabelFromCode("CreateNewDocumentDescriptionProject", language);
        }

        private void ReadRetValueFromPopup()
        {
        }

        private void InitPage()
        {
            UIManager.RegistryManager.SetRegistryInSession(RoleManager.GetRoleInSession().registri[0]);
            this.TxtObject.Focus();
            this.PopulatesFieldsProject();
            
        }

        private void PopulatesFieldsProject()
        {
            string codice = string.Empty;
            string descrizione = string.Empty;
            bool isRootFolder = false;
            Fascicolo project = UIManager.ProjectManager.getProjectInSession();
            Folder folder = project.folderSelezionato;

            if (project.codice == folder.descrizione)
            {
                this.TxtCodeProject.Text = project.codice;
                this.TxtDescriptionProject.Text = project.descrizione;
            }
            else
            {
                this.CalcolaFascicolazioneRapida(folder, ref codice, ref descrizione, ref isRootFolder, project.codice);
                this.TxtCodeProject.Text = project.codice + "//" + codice.Substring(0, codice.Length - 2);

                DocsPaWR.Folder[] listaFolder = null;
                DocsPaWR.Fascicolo fasc = null;
                string separatore = "//";
                int posSep = this.TxtCodeProject.Text.IndexOf("//");
                if (this.TxtCodeProject.Text != string.Empty && posSep > -1)
                {

                    string codiceFascicolo = TxtCodeProject.Text.Substring(0, posSep);
                    string descrFolder = TxtCodeProject.Text.Substring(posSep + separatore.Length);

                    listaFolder = ProjectManager.getListaFolderDaCodiceFascicolo(this, codiceFascicolo, descrFolder, null);
                    if (listaFolder != null && listaFolder.Length > 0)
                    {
                        //calcolo fascicolazionerapida
                        InfoUtente infoUtente = UserManager.GetInfoUser();
                        fasc = ProjectManager.getFascicoloById(listaFolder[0].idFascicolo, infoUtente);

                        if (fasc != null)
                        {
                            //folder selezionato è l'ultimo
                            fasc.folderSelezionato = listaFolder[listaFolder.Length - 1];
                        }
                        codice = fasc.codice + separatore;
                        descrizione = fasc.descrizione + separatore;
                        for (int i = 0; i < listaFolder.Length; i++)
                        {
                            codice += listaFolder[i].descrizione + "/";
                            descrizione += listaFolder[i].descrizione + "/";
                        }
                        codice = codice.Substring(0, codice.Length - 1);
                        descrizione = descrizione.Substring(0, descrizione.Length - 1);

                    }
                }

                this.TxtCodeProject.Text = codice;
                this.TxtDescriptionProject.Text = descrizione;
            }
        }

        private void CalcolaFascicolazioneRapida(Folder folder, ref string codice, ref string descrizione, ref bool isRootFolder, string codFascicolo)
        {
            Folder parent = null;

            if (folder.descrizione.Equals(codFascicolo))
                isRootFolder = true;
            else
            {
                codice = folder.descrizione + "//" + codice;
                descrizione = folder.descrizione + "//" + descrizione;
                parent = ProjectManager.getFolder(this, folder.idParent);
                parent = ProjectManager.getFolder(this, parent);
            }
            if (!isRootFolder)
                CalcolaFascicolazioneRapida(parent, ref codice, ref descrizione, ref isRootFolder, codFascicolo);
        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "InitializeLengthCharacters", "charsLeft('TxtObject','" + this.MaxLenghtObject + "','Descrizione oggetto');", true);
        }

        private void LoadKey()
        {
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_MAX_LENGTH_OGGETTO.ToString())))
            {
                this.MaxLenghtObject = int.Parse(Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_MAX_LENGTH_OGGETTO.ToString()));
            }
        }
        #endregion

        #region event button

        protected void CreateNewDocumentBtnSave_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
            try
            {
                if (string.IsNullOrEmpty(this.TxtObject.Text))
                {
                    string msgDesc = "WarningDocumentRequestObject";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                    return;
                }

                //CREO IL DOCUMENTO
                SchedaDocumento newDoc = UIManager.DocumentManager.NewSchedaDocumento();
                newDoc.oggetto = new Oggetto();
                newDoc.oggetto.descrizione = this.TxtObject.Text;
                newDoc = DocumentManager.creaDocumentoGrigio(this, newDoc);

                if (newDoc != null && !string.IsNullOrEmpty(newDoc.systemId))
                {
                    Fascicolo fascicolo = UIManager.ProjectManager.getProjectInSession();
                    // se la creazione del doc è andata a buon fine ..
                    //eseguo la fascicolazione RAPIDA
                    if (fascicolo != null && fascicolo.systemID != null)
                    {

                        string msg = string.Empty;
                        if (!DocumentManager.fascicolaRapida(this, newDoc.systemId, newDoc.docNumber, string.Empty, fascicolo, out msg))
                        {
                            if (string.IsNullOrEmpty(msg))
                            {
                                string language = UIManager.UserManager.GetUserLanguage();
                                msg = Utils.Languages.GetMessageFromCode("WarningDocumentNoClassificated", language);
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '', '" + utils.FormatJs(msg) + "');} else {parent.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning', '', '" + utils.FormatJs(msg) + "');};", true);
                            }
                        }
                    }
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "if (parent.fra_main) {parent.fra_main.closeAjaxModal('CreateNewDocument', 'up');} else {parent.closeAjaxModal('CreateNewDocument', 'up');};", true);
                }
                else
                {
                    string msg = "ErrorReadElementsSignBook";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                    return;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void CreateNewDocumentBtnCancel_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
            try
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "if (parent.fra_main) {parent.fra_main.closeAjaxModal('CreateNewDocument', '');} else {parent.closeAjaxModal('CreateNewDocument', '');};", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void TxtCodeObject_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
            try
            {
                List<DocsPaWR.Registro> registries = new List<Registro>();
                registries = UIManager.RegistryManager.GetListRegistriesAndRF(UIManager.RoleManager.GetRoleInSession().systemId, "1", string.Empty).ToList<DocsPaWR.Registro>();
                registries.Add(UIManager.RegistryManager.GetRegistryInSession());

                List<string> aL = new List<string>();
                if (registries != null)
                {
                    for (int i = 0; i < registries.Count; i++)
                    {
                        aL.Add(registries[i].systemId);
                    }
                }

                DocsPaWR.Oggetto[] listaObj = null;

                // E' inutile finire nel backend se la casella di testo è vuota (a parte il fatto che 
                // la funzione, in questo caso, restituisce tutto l'oggettario)
                if (!string.IsNullOrEmpty(this.TxtCodeObject.Text.Trim()))
                {
                    //In questo momento tralascio la descrizione oggetto che metto come stringa vuota
                    listaObj = DocumentManager.getListaOggettiByCod(aL.ToArray<string>(), string.Empty, this.TxtCodeObject.Text);
                }
                else
                {
                    listaObj = new DocsPaWR.Oggetto[] { 
                            new DocsPaWR.Oggetto()
                            {
                                descrizione = String.Empty,
                                codOggetto = String.Empty
                            }};
                }

                if (listaObj != null && listaObj.Length > 0)
                {
                    this.TxtObject.Text = listaObj[0].descrizione;
                    this.TxtCodeObject.Text = listaObj[0].codOggetto;
                }
                else
                {
                    this.TxtObject.Text = string.Empty;
                    this.TxtCodeObject.Text = string.Empty;
                }

                this.UpdPnlObject.Update();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "focus", "$('#" + this.CreateNewDocumentBtnSave.ClientID + "').focus();", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void btnObjectPostback_Click(object sender, EventArgs e)
        {
            this.TxtObject.Text = this.ReturnValue.Split('#').First();
            if (this.ReturnValue.Split('#').Length > 1)
                this.TxtCodeObject.Text = this.ReturnValue.Split('#').Last();
            this.UpdPnlObject.Update();
        }

        #endregion
    }
}