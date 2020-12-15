using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using System.Collections.Specialized;
using NttDatalLibrary;
using NttDataWA.Utils;

namespace NttDataWA.Project
{
    public partial class Visibility : System.Web.UI.Page
    {
        public struct DocumentsVisibility
        {
            public string Ruolo { get; set; }
            public string Diritti { get; set; }
            public string CodiceRubrica { get; set; }
            public string Tipo { get; set; }
            public string DataFine { get; set; }
            public bool Rimosso { get; set; }
            public string Note { get; set; }
            public string IdCorr { get; set; }
            public string DataInsSecurity { get; set; }
            public string NoteSecurity { get; set; }
            public bool ShowHistory { get; set; }
            public string IdCorrGlobbRole { get; set; }
            public string TipoDiritto { get; set; }

        }

        private enum GrdVisibility
        {
            checkbox = 0,
            tipo = 1,
            ruolo = 2,
            motivo = 3,
            diritti = 4,
            datadiritto = 5,
            datafine = 6,
            noteacquisizione = 7,
            rimuovi = 8,
            storia = 9,
            rimosso = 10,
            note = 11,
            idcorr = 12
        }

        /// <summary>
        /// Initializes application labels
        /// </summary>
        private void InitializesLabel()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.SignatureA4.Title = Utils.Languages.GetLabelFromCode("PopupSignatureA4", language);
            this.litDescriptionText.Text = Utils.Languages.GetLabelFromCode("VisibilityLitDescription", language);
            this.litUser.Text = Utils.Languages.GetLabelFromCode("VisibilityUser", language);
            this.litRole.Text = Utils.Languages.GetLabelFromCode("VisibilityRole", language);
            this.litMotive.Text = Utils.Languages.GetLabelFromCode("VisibilityMotive", language);
            this.optAll.Text = Utils.Languages.GetLabelFromCode("VisibilityOptAll", language);
            this.optA.Text = Utils.Languages.GetLabelFromCode("VisibilityOptA", language);
            this.optAC.Text = Utils.Languages.GetLabelFromCode("VisibilityOptAC", language);
            this.optF.Text = Utils.Languages.GetLabelFromCode("VisibilityOptF", language);
            this.optUP.Text = Utils.Languages.GetLabelFromCode("VisibilityOptUP", language);
            this.optRP.Text = Utils.Languages.GetLabelFromCode("VisibilityOptRP", language);
            this.optT.Text = Utils.Languages.GetLabelFromCode("VisibilityOptT", language);
            this.litTypeRight.Text = Utils.Languages.GetLabelFromCode("VisibilityTypeRight", language);
            this.optAll2.Text = Utils.Languages.GetLabelFromCode("VisibilityOptAll", language);
            this.optR.Text = Utils.Languages.GetLabelFromCode("VisibilityOptR", language);
            this.optW.Text = Utils.Languages.GetLabelFromCode("VisibilityOptW", language);
            this.opt0.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt0", language);
            this.opt1.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt1", language);
            this.opt2.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt2", language);
            this.opt3.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt3", language);
            this.opt4.Text = Utils.Languages.GetLabelFromCode("VisibilityOpt4", language);
            this.litDateRight.Text = Utils.Languages.GetLabelFromCode("VisibilityDateRight", language);
            this.VisibilityLblDelVis.Text = Utils.Languages.GetLabelFromCode("VisibilityLblDelVis", language);
            this.VisibilityBtnFilter.Text = Utils.Languages.GetLabelFromCode("VisibilityBtnFilter", language);
            this.VisibilityBtnClear.Text = Utils.Languages.GetLabelFromCode("VisibilityBtnClear", language);
            this.VisibilityFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
            this.VisibilityTo.Text = Utils.Languages.GetLabelFromCode("VisibilityTo", language);
            this.GridDocuments.Columns[1].HeaderText = Utils.Languages.GetLabelFromCode("VisibilityType", language);
            this.GridDocuments.Columns[2].HeaderText = Utils.Languages.GetLabelFromCode("VisibilityRoleUser", language);
            this.GridDocuments.Columns[4].HeaderText = Utils.Languages.GetLabelFromCode("VisibilityMotive", language);
            this.GridDocuments.Columns[5].HeaderText = Utils.Languages.GetLabelFromCode("VisibilityRights", language);
            this.GridDocuments.Columns[6].HeaderText = Utils.Languages.GetLabelFromCode("VisibilityDateStarting", language);
            this.GridDocuments.Columns[7].HeaderText = Utils.Languages.GetLabelFromCode("VisibilityDateEnding", language);
            this.GridDocuments.Columns[8].HeaderText = Utils.Languages.GetLabelFromCode("VisibilityNotes", language);
            this.GridDocuments.Columns[10].HeaderText = Utils.Languages.GetLabelFromCode("VisibilityHistory", language);
            this.GridDocuments.Columns[11].HeaderText = Utils.Languages.GetLabelFromCode("VisibilityRemoved", language);
            this.GridDocuments.Columns[12].HeaderText = Utils.Languages.GetLabelFromCode("VisibilityNotes", language);
            this.GridDocuments.Columns[14].HeaderText = Utils.Languages.GetLabelFromCode("VisibilityDetails", language);
            this.VisibilityHistory.Title = Utils.Languages.GetLabelFromCode("VisibilityTitle", language);
            this.VisibilityRemove.Title = Utils.Languages.GetLabelFromCode("VisibilityRemove", language);
            this.VisibilityImgAddressBookUser.AlternateText = Utils.Languages.GetLabelFromCode("VisibilityImgAddressBookUser", language);
            this.VisibilityImgAddressBookUser.ToolTip = Utils.Languages.GetLabelFromCode("VisibilityImgAddressBookUser", language);
            this.VisibilityImgAddressBookRole.AlternateText = Utils.Languages.GetLabelFromCode("VisibilityImgAddressBookRole", language);
            this.VisibilityImgAddressBookRole.ToolTip = Utils.Languages.GetLabelFromCode("VisibilityImgAddressBookRole", language);
            this.AddressBook.Title = Utils.Languages.GetLabelFromCode("AddressBookTitle", language);
            this.MassiveVisibilityRemove.Title = Utils.Languages.GetLabelFromCode("VisibilityRemove", language);
            this.MassiveVisibilityRestore.Title = Utils.Languages.GetLabelFromCode("VisibilityRestoreTooltip", language);
            this.DdlMassiveOperation.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("VisibilitytDdlAzioniMassive", language));
            this.ImgInfoVisibility.ToolTip = Utils.Languages.GetLabelFromCode("VisibilityImgInfoVisibilityProjectTooltip", language);
        }

        protected string GetTitle()
        {
            return this.VisibilityHistory.Title;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                //Se ACL rimossa, allora visualizzo un messaggio di warning all'utente per poi reindirizzarlo alla HOME.
                Fascicolo Prj = UIManager.ProjectManager.getProjectInSession();

                if ((Prj.systemID != null && !string.IsNullOrEmpty(Prj.systemID)) && ProjectManager.CheckRevocationAcl())
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "function RedirectHome(){$(location).attr('href','" + this.ResolveUrl("~/Index.aspx") + "');} if (parent.fra_main) {parent.fra_main.ajaxDialogModal('RevocationAcl', 'warning', '','',null,null,'RedirectHome()')} else {parent.parent.ajaxDialogModal('RevocationAcl', 'warning', '','',null,null,'RedirectHome()');}", true);
                    return;
                }

                if (!IsPostBack)
                {
                    this.InitializesLabel();
                    this.DocumentAtipic();
                    this.LoadGridVisibility(null);
                    this.InitializePage();
                    this.InitializeAjaxAddressBook();
                    this.ControlAbortDocument();
                }
                else
                {
                    this.ReadRetValueFromPopup();
                }
                this.RefreshScript();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ReadRetValueFromPopup()
        {
            if (!string.IsNullOrEmpty(this.VisibilityRemove.ReturnValue))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('VisibilityRemove','');", true);
                this.LoadGridVisibility(null);
                this.UpContainer.Update();
                this.ProjectTabs.RefreshProjectabs();
                this.UpContainerProjectTab.Update();
                this.UpContainer.Update();
            }

            if (!string.IsNullOrEmpty(this.MassiveVisibilityRemove.ReturnValue))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('MassiveVisibilityRemove','');", true);
                this.LoadGridVisibility(null);
                this.UpContainer.Update();
            }

            if (!string.IsNullOrEmpty(this.MassiveVisibilityRestore.ReturnValue))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('MassiveVisibilityRestore','');", true);
                this.LoadGridVisibility(null);
                this.UpContainer.Update();
            }
        }

        private void ControlAbortDocument()
        {
            this.AbortDocument = false;
            SchedaDocumento doc = DocumentManager.getSelectedRecord();
            if (doc != null && !string.IsNullOrEmpty(doc.systemId) && (doc.tipoProto.ToUpper().Equals("A") || doc.tipoProto.ToUpper().Equals("P") || doc.tipoProto.ToUpper().Equals("I")))
            {
                if (doc != null && doc.tipoProto != null && doc.protocollo.protocolloAnnullato != null)
                {
                    this.AbortDocument = true;
                }

            }
        }

        protected void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
        }

        protected void InitializeAjaxAddressBook()
        {
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.AUTOCOMPLETE_MINIMUMPREFIXLENGTH.ToString()]))
            {
                this.AjaxAddressBookMinPrefixLenght = int.Parse(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.AUTOCOMPLETE_MINIMUMPREFIXLENGTH.ToString()]);
            }

           // this.RapidUser.MinimumPrefixLength = this.AjaxAddressBookMinPrefixLenght;

            string dataUser = UIManager.RoleManager.GetRoleInSession().systemId;
            dataUser = dataUser + "-" + UIManager.RegistryManager.GetRegistryInSession().systemId;

            string callType = string.Empty;

            callType = "CALLTYPE_IN_ONLY_USER";
            RapidUser.ContextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + callType;

            callType = "CALLTYPE_IN_ONLY_ROLE";
            RapidRole.ContextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + callType;

        }

        protected void InitializePage()
        {            
            Fascicolo fascicolo = UIManager.ProjectManager.getProjectInSession();
            this.litDescription.Text = Server.HtmlEncode(fascicolo.descrizione);

            this.ListCheck = new List<string>();
            this.LoadMassiveOperation();
        }

        protected void LoadGridVisibility(FilterVisibility[] ListFiltered)
        {
            try
            {
                bool cercaRimossi;

                Fascicolo fascicolo = UIManager.ProjectManager.getProjectInSession();
                this.TypeObject = "F";

                if (fascicolo == null)
                {
                    this.lblResult.Text = "Errore nel reperimento dei dati del fascicolo";
                    this.lblResult.Visible = true;
                    return;
                }
                cercaRimossi = true;

                //this.VisibilityList = DocumentManager.GetSimpliedListVisibilityWithFilters(this, InfoDoc.idProfile, cercaRimossi, ListFiltered);
                this.VisibilityList = DocumentManager.GetSimpliedListVisibilityWithFilters(this, fascicolo.systemID, cercaRimossi, ListFiltered);

                // Create and initialize a generic list
                List<DocumentsVisibility> DocDir = new List<DocumentsVisibility>();
                DocumentsVisibility DocVis = new DocumentsVisibility();
                string idRuoloPubblico = string.Empty;
                if(fascicolo.pubblico)
                    idRuoloPubblico = Utils.InitConfigurationKeys.GetValue(UserManager.GetUserInSession().idAmministrazione, DBKeys.ENABLE_FASCICOLO_PUBBLICO.ToString());
                if (string.IsNullOrEmpty(idRuoloPubblico))
                    idRuoloPubblico = "0";
                if (VisibilityList != null && VisibilityList.Length > 0)
                {
                    for (int i = 0; i < this.VisibilityList.Length; i++)
                    {
                        string descrSoggetto = UserManager.GetCorrespondingDescription(this, this.VisibilityList[i].soggetto);
                        string Corr = GetTipoCorr(this.VisibilityList[i]);
                        bool Removed = this.VisibilityList[i].deleted;
                        string Diritti = this.GetRightDescription(this.VisibilityList[i].accessRights);

                        DocVis.Ruolo = descrSoggetto;
                        DocVis.Diritti = this.setTipoDiritto(this.VisibilityList[i]);
                        DocVis.CodiceRubrica = this.VisibilityList[i].soggetto.codiceRubrica;
                        DocVis.Tipo = Corr;
                        DocVis.DataFine = this.VisibilityList[i].soggetto.dta_fine;
                        DocVis.TipoDiritto = Diritti;
                        DocVis.Rimosso = Removed;
                        DocVis.Note = this.VisibilityList[i].note;
                        DocVis.IdCorr = this.VisibilityList[i].personorgroup;
                        DocVis.DataInsSecurity = this.VisibilityList[i].dtaInsSecurity;
                        DocVis.NoteSecurity = this.VisibilityList[i].noteSecurity;
                        DocVis.ShowHistory = this.VisibilityList[i].soggetto.tipoCorrispondente == "R" ?
                            !string.IsNullOrEmpty((this.VisibilityList[i].soggetto as Ruolo).ShowHistory) && (this.VisibilityList[i].soggetto as Ruolo).ShowHistory != "0" : false;
                        if (this.VisibilityList[i].personorgroup.Equals(idRuoloPubblico))
                        {
                            DocVis.Ruolo = Utils.Languages.GetLabelFromCode("VisibilityPublicRole", UserManager.GetUserLanguage());
                            DocVis.Diritti = string.Empty;
                            this.PnlInfoVisibility.Visible = true;
                            //Se la visibilità al ruolo pubblico è stata rimossa, nascondo il tooltip con l'info
                            if (Removed)
                                this.PnlInfoVisibility.Visible = false;
                        }
                        DocDir.Add(DocVis);
                    }

                }
                else
                {
                    //
                }
                this.DocumentsVisibilityList = DocDir;


                if (ViewState["SortExp"] != null)
                {
                    List<DocumentsVisibility> listOrdered = this.DocumentsVisibilityList;
                    switch (ViewState["SortExp"].ToString())
                    {
                        case "Ruolo":
                            if (ViewState["SortDirection"].ToString() == "asc")
                                listOrdered = listOrdered.OrderBy(x => x.Ruolo).ToList();
                            else
                                listOrdered = listOrdered.OrderByDescending(x => x.Ruolo).ToList();
                            break;
                        case "diritti":
                            if (ViewState["SortDirection"].ToString() == "asc")
                                listOrdered = listOrdered.OrderBy(x => x.Diritti).ToList();
                            else
                                listOrdered = listOrdered.OrderByDescending(x => x.Diritti).ToList();
                            break;
                        case "TipoDiritto":
                            if (ViewState["SortDirection"].ToString() == "asc")
                                listOrdered = listOrdered.OrderBy(x => x.TipoDiritto).ToList();
                            else
                                listOrdered = listOrdered.OrderByDescending(x => x.TipoDiritto).ToList();
                            break;
                        case "DataInsSecurity":
                            if (ViewState["SortDirection"].ToString() == "asc")
                                listOrdered = listOrdered.OrderBy(x => x.DataInsSecurity).ToList();
                            else
                                listOrdered = listOrdered.OrderByDescending(x => x.DataInsSecurity).ToList();
                            break;
                        case "DataFine":
                            if (ViewState["SortDirection"].ToString() == "asc")
                                listOrdered = listOrdered.OrderBy(x => x.DataFine).ToList();
                            else
                                listOrdered = listOrdered.OrderByDescending(x => x.DataFine).ToList();
                            break;
                    }
                    this.DocumentsVisibilityList = listOrdered;
                }


                this.GridDocuments.DataSource = this.DocumentsVisibilityList;
                this.GridDocuments.DataBind();
                this.UpdPanelVisibility.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        protected void GridDocuments_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try {
                int selRow = 0;

                switch (e.CommandName)
                {
                    case "Select":
                        this.RowSelected = (GridViewRow)(((CustomImageButton)e.CommandSource).NamingContainer);
                        selRow = RowSelected.DataItemIndex;
                        this.GridDocuments.SelectedRowStyle.BackColor = System.Drawing.Color.Yellow;
                        break;
                    case "Erase":
                        this.RowSelected = (GridViewRow)(((CustomImageButton)e.CommandSource).NamingContainer);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "VisibilityRemove", "ajaxModalPopupVisibilityRemove();", true);
                        break;
                    case "Ripristina":
                        this.RowSelected = (GridViewRow)(((CustomImageButton)e.CommandSource).NamingContainer);
                        this.RestoreACL();
                        this.UpdPanelVisibility.Update();
                        break;
                    case "Cancel":
                        this.GridDocuments.EditIndex = -1;
                        this.UpdPanelVisibility.Update();
                        break;
                    case "ShowHistoryRole":
                        this.RowSelected = (GridViewRow)(((CustomImageButton)e.CommandSource).NamingContainer);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "VisibilityHistoryRole", "ajaxModalPopupVisibilityHistoryRole();", true);
                        break;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void GetDetailsCorresponding(string CodiceRubrica)
        {

            //Build object queryCorrispondente
            DocsPaWR.AddressbookQueryCorrispondente qco = new DocsPaWR.AddressbookQueryCorrispondente();

            qco.codiceRubrica = CodiceRubrica;
            qco.getChildren = true;
            qco.idAmministrazione = UserManager.GetInfoUser().idAmministrazione;
            qco.fineValidita = true;

            //Internal corresponding
            qco.tipoUtente = DocsPaWR.AddressbookTipoUtente.INTERNO;
            DocsPaWR.Corrispondente[] listaCorr = UserManager.getListCorrispondent(qco);

            //Visualize information of users
            string st_listaCorr = "";
            DocsPaWR.Corrispondente cor;

            for (int i = 0; i < listaCorr.Length; i++)
            {
                cor = (DocsPaWR.Corrispondente)listaCorr[i];
                if (cor.dta_fine != string.Empty)
                    st_listaCorr += "<li><span class=\"corr_grey\">" + ((DocsPaWR.Corrispondente)listaCorr[i]).descrizione + "</span></li>\n";
                else
                    st_listaCorr += "<li>" + ((DocsPaWR.Corrispondente)listaCorr[i]).descrizione + "</li>\n";
            }
            if (!string.IsNullOrEmpty(st_listaCorr))
                st_listaCorr = "<ul>\n"
                            + st_listaCorr
                            + "</ul>\n";

            Literal LblDetails = new Literal();
            Literal LblDetailsInfo = new Literal();
            Literal lblDetailsUser = new Literal();

            LblDetails = (Literal)GridDocuments.Rows[RowSelected.RowIndex].Cells[(int)GrdVisibility.ruolo].FindControl("LblDetails");
            LblDetailsInfo = (Literal)GridDocuments.Rows[RowSelected.RowIndex].Cells[(int)GrdVisibility.ruolo].FindControl("LblDetailsInfo");
            lblDetailsUser = (Literal)GridDocuments.Rows[RowSelected.RowIndex].Cells[(int)GrdVisibility.ruolo].FindControl("lblDetailsUser");

            if (st_listaCorr.Length > 0)
            {
                lblDetailsUser.Visible = true;
                LblDetails.Visible = true;
                LblDetailsInfo.Visible = true;
                lblDetailsUser.Text = Utils.Languages.GetLabelFromCode("VisibilityRoleDetails", UIManager.UserManager.GetUserLanguage());
                LblDetails.Text = st_listaCorr;
                GridDocuments.Rows[RowSelected.RowIndex].Cells[(int)GrdVisibility.ruolo].FindControl("divDetails").Visible = true;
            }
            else
            {
                LblDetails.Visible = false;
                LblDetailsInfo.Visible = false;
                lblDetailsUser.Visible = false;
                LblDetails.Text = "";
                GridDocuments.Rows[RowSelected.RowIndex].Cells[(int)GrdVisibility.ruolo].FindControl("divDetails").Visible = false;
            }

            this.UpContainer.Update();
            this.UpdPanelVisibility.Update();
        }

        protected string PersonOrGroup()
        {
            //Verify if user or role
            string personOrGroup;

            HiddenField hdnTipo = new HiddenField();
            hdnTipo = (HiddenField)GridDocuments.Rows[RowSelected.RowIndex].Cells[(int)GrdVisibility.tipo].FindControl("hdnTipo");

            if (hdnTipo.Value.ToUpper().Equals("UTENTE"))
                personOrGroup = "U";
            else
                personOrGroup = "R";

            return personOrGroup;
        }

        private void RestoreACL()
        {
            try
            {
                string personOrGroup = PersonOrGroup();

                DocumentoDiritto[] ListDocDir = VisibilityList;
                DocsPaWR.DocumentoDiritto docDiritti = ListDocDir[RowSelected.DataItemIndex];
                bool result = DocumentManager.RestoreACL(docDiritti, personOrGroup, UserManager.GetInfoUser(),"F");
                if (result)
                {
                    this.LoadGridVisibility(null);
                    this.GridDocuments.SelectedIndex = -1;
                }
                //else
                //{
                //    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Attenzione operazione non riuscita');", true);
                //}
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        protected string GetTipoCorr(DocumentoDiritto docDirit)
        {
            try
            {
                string rtn = "";
                if (docDirit.soggetto.tipoCorrispondente.Equals("P"))
                    rtn = "UTENTE";
                else if (docDirit.soggetto.tipoCorrispondente.Equals("R"))
                    rtn = "RUOLO";
                else if (docDirit.soggetto.tipoCorrispondente.Equals("U"))
                    rtn = "U.O.";
                return rtn;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }

        private string setTipoDiritto(DocumentoDiritto docDir)
        {
            string star = "";
            string RetVal = "";

            string language = UIManager.UserManager.GetUserLanguage();

            if (docDir.hideDocVersions)
                star = Environment.NewLine + "*";

            switch (docDir.tipoDiritto)
            {
                case DocumentoTipoDiritto.TIPO_ACQUISITO:
                    if (docDir.accessRights == (int)NttDataWA.Utils.HMdiritti.HDdiritti_Waiting)
                        RetVal = Utils.Languages.GetLabelFromCode("VisibilityLabelPending", language) + star;
                    else
                        RetVal = (string.IsNullOrEmpty(docDir.CopiaVisibilita) || docDir.CopiaVisibilita.Equals("0")) ? Utils.Languages.GetLabelFromCode("VisibilityLabelAcquired", language) + star : Utils.Languages.GetLabelFromCode("VisibilityLabelAcquiredCopy", language) + star;
                    break;
                case DocumentoTipoDiritto.TIPO_PROPRIETARIO:
                case DocumentoTipoDiritto.TIPO_DELEGATO:
                    RetVal = Utils.Languages.GetLabelFromCode("VisibilityLabelOwner", language) + star;
                    break;
                case DocumentoTipoDiritto.TIPO_TRASMISSIONE:
                    RetVal = Utils.Languages.GetLabelFromCode("VisibilityLabelTransmission", language) + star;
                    break;
                case DocumentoTipoDiritto.TIPO_TRASMISSIONE_IN_FASCICOLO:
                    RetVal = Utils.Languages.GetLabelFromCode("VisibilityLabelFolder", language) + star;
                    break;
                case DocumentoTipoDiritto.TIPO_SOSPESO:
                    RetVal = "SOSPESO" + star;
                    break;
                case DocumentoTipoDiritto.TIPO_CONSERVAZIONE:
                    RetVal = Utils.Languages.GetLabelFromCode("VisibilityLabelPreservation", language) + star;
                    break;
            }

            return RetVal;

        }

        /// <summary>
        /// Metodo per la generazione di una descrizione estesa del tipo diritto
        /// </summary>
        /// <param name="accessRight">Diritto di accesso</param>
        /// <returns>Descrizione del tipo di diritto</returns>
        protected String GetRightDescription(int accessRight)
        {
            String retVal = String.Empty;
            string language = UIManager.UserManager.GetUserLanguage();

            switch (accessRight)
            {
                case 0:
                case 255:
                case 63:
                    retVal = Utils.Languages.GetLabelFromCode("VisibilityLabelRW", language);
                    break;
                case 45:
                case 20:
                    retVal = Utils.Languages.GetLabelFromCode("VisibilityLabelReadOnly", language);
                    break;
                default:
                    break;

            }

            return retVal;

        }

        protected void DocumentAtipic()
        {
            //Verify document atipic
            DocsPaWR.InfoAtipicita InfoAtipic = null;
            SchedaDocumento doc = UIManager.DocumentManager.getSelectedRecord();
            if (doc != null && !string.IsNullOrEmpty(doc.docNumber))
                InfoAtipic = DocumentManager.GetInfoAtipicita(DocsPaWR.TipoOggettoAtipico.DOCUMENTO, doc.docNumber);
        }

        protected void GridDocuments_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {
            try {
                Literal LblDetails = new Literal();
                Literal LblDetailsInfo = new Literal();
                LblDetails = (Literal)GridDocuments.Rows[RowSelected.DataItemIndex].Cells[(int)GrdVisibility.tipo].FindControl("LblDetails");
                LblDetailsInfo = (Literal)GridDocuments.Rows[RowSelected.DataItemIndex].Cells[(int)GrdVisibility.tipo].FindControl("LblDetailsInfo");
                LblDetailsInfo.Text = "";
                LblDetails.Text = "";
                this.UpContainer.Update();
                this.UpdPanelVisibility.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void GridDocuments_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try {
                this.GridDocuments.PageIndex = e.NewPageIndex;
                this.GridDocuments.DataSource = this.DocumentsVisibilityList;
                this.GridDocuments.DataBind();
                this.UpdPanelVisibility.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void GridDocuments_PreRender(object sender, EventArgs e)
        {
            try {
                //Verifica se l'utente è abilitato alla funzione di editing ACL
                bool IsACLauthorized = UserManager.IsAuthorizedFunctions("ACL_RIMUOVI");
                string idRuoloPubblico = Utils.InitConfigurationKeys.GetValue(UserManager.GetUserInSession().idAmministrazione, DBKeys.ENABLE_FASCICOLO_PUBBLICO.ToString());
                if (string.IsNullOrEmpty(idRuoloPubblico))
                    idRuoloPubblico = "0";
                Fascicolo prj = ProjectManager.getProjectInSession();
                if (prj != null && !string.IsNullOrEmpty(prj.accessRights) && Int32.Parse(prj.accessRights) <= 45)
                {
                    IsACLauthorized = false;
                }

                bool ripristina = false;

                string language = UIManager.UserManager.GetUserLanguage();
                string removeTooltip = Utils.Languages.GetLabelFromCode("VisibilityRemoveTooltip", language);
                string restoreTooltip = Utils.Languages.GetLabelFromCode("VisibilityRestoreTooltip", language);

                CustomImageButton ImgType = new CustomImageButton();
                HiddenField hdnCodRubrica = new HiddenField();
                Literal LblDetails = new Literal();
                HiddenField hdnTipo = new HiddenField();
                CustomImageButton ImgDelete = new CustomImageButton();
                Image ImgTipo = new Image();
                Label LblEndDate = new Label();
                Label LblRemoved = new Label();
                Label LblDiritto = new Label();
                CheckBox chkbox = new CheckBox();

                DocsPaWR.InfoUtente infoUtente = UserManager.GetInfoUser();

                for (int i = 0; i < GridDocuments.Rows.Count; i++)
                {
                    if (GridDocuments.Rows[i].DataItemIndex >= 0)
                    {
                        hdnTipo = (HiddenField)GridDocuments.Rows[i].Cells[(int)GrdVisibility.tipo].FindControl("hdnTipo");
                        ImgTipo = (Image)GridDocuments.Rows[i].Cells[(int)GrdVisibility.tipo].FindControl("imgTipo");
                        LblEndDate = (Label)GridDocuments.Rows[i].Cells[(int)GrdVisibility.datafine].FindControl("LblEndDate");
                        LblRemoved = (Label)GridDocuments.Rows[i].Cells[(int)GrdVisibility.rimosso].FindControl("LblRemoved");
                        LblDiritto = (Label)GridDocuments.Rows[i].Cells[(int)GrdVisibility.diritti].FindControl("LblDiritto");
                        ImgDelete = (CustomImageButton)GridDocuments.Rows[i].Cells[(int)GrdVisibility.rimuovi].FindControl("ImgDelete");
                        chkbox = (CheckBox)GridDocuments.Rows[i].Cells[(int)GrdVisibility.checkbox].FindControl("chkbox");

                        if (!string.IsNullOrEmpty(LblEndDate.Text))
                        {
                            GridDocuments.Rows[i].ForeColor = System.Drawing.Color.Gray;
                            GridDocuments.Rows[i].Font.Bold = false;
                            GridDocuments.Rows[i].Font.Strikeout = false;
                        }

                        //Se Acl è revocata allora la riga è di colore rosso e barrata
                        int Removed;
                        if (LblRemoved.Text.ToUpper() == "TRUE") Removed = 1; else Removed = 0;

                        if (!string.IsNullOrEmpty(LblRemoved.Text) && Removed == 1)
                        {
                            GridDocuments.Rows[i].ForeColor = System.Drawing.Color.Red;
                            GridDocuments.Rows[i].Font.Bold = true;
                            GridDocuments.Rows[i].Font.Strikeout = true;
                            ImgDelete.ImageUrl = "../Images/Icons/ico_risposta.gif";
                            ImgDelete.CommandName = "Ripristina";
                            ImgDelete.ToolTip = restoreTooltip;
                            ImgDelete.OnMouseOutImage = "../Images/Icons/ico_risposta.gif";
                            ImgDelete.OnMouseOverImage = "../Images/Icons/ico_risposta.gif";

                            ripristina = true;
                        }
                        else
                        {
                            ImgDelete.ToolTip = removeTooltip;
                        }

                        //Se l'utente è proprietario del documento, non è MAI possibile rimuovere i diritti.
                        //Se l'utente ha la funzione di "editing ACL" può rimuovere i diritti anche ad altri ruoli/utenti.
                        string Diritto = LblDiritto.Text;
                        int IdCorr = Convert.ToInt32(((System.Web.UI.WebControls.GridView)(sender)).DataKeys[i].Value);

                        ImgDelete.Visible = IsACLauthorized && !this.AbortDocument;
                        chkbox.Visible = IsACLauthorized && !this.AbortDocument;

                        if (hdnTipo.Value.Equals("UTENTE"))
                        {
                            string cssClass = "nopointer";
                            if (i % 2 == 1) cssClass += " AltRow";
                            GridDocuments.Rows[i].CssClass = cssClass;

                            if (infoUtente.idPeople != IdCorr.ToString() && !ripristina && IsACLauthorized && !this.AbortDocument)
                            {
                                ImgDelete.Visible = true;
                                chkbox.Visible = true;
                            }

                            if (Diritto.Equals(Utils.Languages.GetLabelFromCode("VisibilityLabelOwner", language)) || IdCorr.ToString().Equals(idRuoloPubblico))
                            {
                                ImgDelete.Visible = false;
                                chkbox.Visible = false;
                            }

                            ImgTipo.ImageUrl = "~/Images/Icons/user_icon.png";
                        }
                        else
                        {
                            string jsOnClick = "$('#rowIndex').val('" + GridDocuments.Rows[i].RowIndex.ToString() + "'); $('#btnDetails').click();";
                            //GridDocuments.Rows[i].Cells[0].Attributes["onclick"] = jsOnClick;
                            GridDocuments.Rows[i].Cells[1].Attributes["onclick"] = jsOnClick;
                            GridDocuments.Rows[i].Cells[2].Attributes["onclick"] = jsOnClick;
                            GridDocuments.Rows[i].Cells[3].Attributes["onclick"] = jsOnClick;
                            GridDocuments.Rows[i].Cells[4].Attributes["onclick"] = jsOnClick;
                            GridDocuments.Rows[i].Cells[5].Attributes["onclick"] = jsOnClick;
                            GridDocuments.Rows[i].Cells[6].Attributes["onclick"] = jsOnClick;
                            GridDocuments.Rows[i].Cells[8].Attributes["onclick"] = jsOnClick;
                            GridDocuments.Rows[i].Cells[9].Attributes["onclick"] = jsOnClick;
                            GridDocuments.Rows[i].Cells[10].Attributes["onclick"] = jsOnClick;
                            GridDocuments.Rows[i].Cells[11].Attributes["onclick"] = jsOnClick;
                            GridDocuments.Rows[i].Cells[12].Attributes["onclick"] = jsOnClick;

                            if (infoUtente.idGruppo != IdCorr.ToString() && !ripristina && IsACLauthorized && !this.AbortDocument)
                            {
                                ImgDelete.Visible = true;
                                chkbox.Visible = true;
                            }

                            if (Diritto.Equals(Utils.Languages.GetLabelFromCode("VisibilityLabelOwner", language)) || IdCorr.ToString().Equals(idRuoloPubblico))
                            {
                                ImgDelete.Visible = false;
                                chkbox.Visible = false;
                            }

                            ImgTipo.ImageUrl = "~/Images/Icons/role2_icon.png";
                        }
                    }

                    chkbox.Attributes["rel"] = i.ToString();
                    chkbox.Attributes["onclick"] = "SetItemCheck()";

                    if (ripristina)
                    {
                        ImgDelete.Visible = true;
                        chkbox.Visible = true;
                    }

                }

                //i client senza acl non devono vedere la colonna
                //GridDocuments.Columns[4].Visible = IsACLauthorized;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void TxtUser_OnTextChanged(object sender, EventArgs e)
        {
            try {
                CustomTextArea caller = sender as CustomTextArea;
                string codeAddressBook = string.Empty;

                if (!string.IsNullOrEmpty(this.TxtUser.Text))
                {
                    this.SearchCorrespondent(this.TxtUser.Text, caller.ID, true, false, true);
                }
                else
                {
                    this.TxtUser.Text = string.Empty;
                    this.TxtDescriptionUser.Text = string.Empty;
                    this.IdSender.Value = string.Empty;
                }
                this.UpPnlUser.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void SearchCorrespondent(string addressCode, string idControl, bool endOfValidity, bool role, bool user)
        {
            try
            {
                DocsPaWR.Corrispondente corr = null;
                RubricaCallType calltype = DocsPaWR.RubricaCallType.CALLTYPE_PROTO_INT_MITT;
                ElementoRubrica[] listaCorr = null;

                listaCorr = UIManager.AddressBookManager.getElementiRubricaMultipliSoloInterni(addressCode, calltype, true, role, user);
                if (listaCorr != null && listaCorr.Length > 0)
                {
                    if (listaCorr.Length == 1)
                    {
                        corr = UIManager.AddressBookManager.GetCorrespondentBySystemId(listaCorr[0].systemId);
                    }
                    else
                    {
                        corr = null;
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_found", "alert('APRI FINESTRA SCEGLI CORRISPONDENTI');", true);
                    }
                }



                if (corr == null)
                {
                    if (idControl == "TxtUser")
                    {
                        this.TxtUser.Text = string.Empty;
                        this.TxtDescriptionUser.Text = string.Empty;
                        this.IdSender.Value = string.Empty;
                    }
                    else
                    {
                        if (idControl == "TxtRole")
                        {
                            this.TxtRole.Text = string.Empty;
                            this.TxtDescriptionRole.Text = string.Empty;
                            this.IdRecipient.Value = string.Empty;
                        }
                    }


                    //string msg = "Corrispondente non trovato.";
                    string msgDesc = "WarningDocumentCorrNotFound";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                }

                else
                {
                    if (idControl == "TxtUser")
                    {
                        this.TxtUser.Text = corr.codiceRubrica;
                        this.TxtDescriptionUser.Text = corr.descrizione;
                        this.IdSender.Value = corr.systemId;
                    }
                    else
                    {
                        if (idControl == "TxtRole")
                        {
                            this.TxtRole.Text = corr.codiceRubrica;
                            this.TxtDescriptionRole.Text = corr.descrizione;
                            this.IdRecipient.Value = corr.systemId;
                        }
                    }
                }

                if (idControl == "TxtUser")
                {
                    this.UpPnlUser.Update();
                }
                else
                {
                    if (idControl == "TxtRole")
                    {
                        this.UpPnlRole.Update();
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        protected void DdlDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            try {
                string language = UIManager.UserManager.GetUserLanguage();
                switch (this.DdlDate.SelectedIndex)
                {
                    case 0: //Valore singolo
                        this.TxtFrom.ReadOnly = false;
                        this.TxtTo.Visible = false;
                        this.VisibilityTo.Visible = false;
                        this.VisibilityFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        break;
                    case 1: //Intervallo
                        this.TxtFrom.ReadOnly = false;
                        this.TxtTo.ReadOnly = false;
                        this.VisibilityTo.Visible = true;
                        this.VisibilityFrom.Visible = true;
                        this.TxtTo.Visible = true;
                        this.VisibilityFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityFrom", language);
                        break;
                    case 2: //Oggi
                        this.VisibilityTo.Visible = false;
                        this.TxtTo.Visible = false;
                        this.TxtFrom.ReadOnly = true;
                        this.TxtFrom.Text = NttDataWA.Utils.dateformat.toDay();
                        break;
                    case 3: //Settimana corrente
                        this.VisibilityTo.Visible = true;
                        this.TxtTo.Visible = true;
                        this.TxtFrom.Text = NttDataWA.Utils.dateformat.getFirstDayOfWeek();
                        this.TxtTo.Text = NttDataWA.Utils.dateformat.getLastDayOfWeek();
                        this.TxtTo.ReadOnly = true;
                        this.TxtFrom.ReadOnly = true;
                        break;
                    case 4: //Mese corrente
                        this.VisibilityTo.Visible = true;
                        this.TxtTo.Visible = true;
                        this.TxtFrom.Text = NttDataWA.Utils.dateformat.getFirstDayOfMonth();
                        this.TxtTo.Text = NttDataWA.Utils.dateformat.getLastDayOfMonth();
                        this.TxtTo.ReadOnly = true;
                        this.TxtFrom.ReadOnly = true;
                        break;
                }

                this.UpContainer.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void VisibilityBtnFilter_Click(object sender, EventArgs e)
        {
            try {
                List<FilterVisibility> filterArrayVis = new List<FilterVisibility>();
                FilterVisibility vis = null;

                this.GridDocuments.PageIndex = 0;
                this.GridDocuments.DataSource = this.DocumentsVisibilityList;
                this.GridDocuments.DataBind();
                this.UpdPanelVisibility.Update();

                //Filtro Utente
                if (!string.IsNullOrEmpty(this.TxtUser.Text))
                {
                    vis = new FilterVisibility();
                    vis.Type = FilterVisibility2.USER;
                    vis.Value = this.IdSender.Value;
                }
                //Add value to array
                if (!string.IsNullOrEmpty(this.TxtUser.Text.Trim()))
                    filterArrayVis.Add(vis);

                //Filtro Ruolo
                if (!string.IsNullOrEmpty(this.TxtRole.Text))
                {
                    vis = new FilterVisibility();
                    vis.Type = FilterVisibility2.ROLE;
                    vis.Value = this.IdRecipient.Value;
                }
                //Add value to array
                if (!string.IsNullOrEmpty(this.TxtRole.Text.Trim()))
                    filterArrayVis.Add(vis);

                //Filtro motivo
                if (!string.IsNullOrEmpty(this.DdlCause.SelectedValue))
                {
                    vis = new FilterVisibility();
                    vis.Type = FilterVisibility2.CAUSE;
                    vis.Value = this.DdlCause.SelectedValue;
                }

                //Add value to array
                if (!string.IsNullOrEmpty(this.DdlCause.SelectedValue))
                    filterArrayVis.Add(vis);

                //Filtro sulla data
                switch (DdlDate.SelectedItem.Value)
                {
                    case "0":
                    case "2":
                        if (!string.IsNullOrEmpty(this.TxtFrom.Text))
                        {
                            vis = new FilterVisibility();
                            vis.Type = FilterVisibility2.DATE;
                            vis.Value = this.TxtFrom.Text;
                        }
                        break;
                    case "1":
                        if (!string.IsNullOrEmpty(this.TxtFrom.Text) && !string.IsNullOrEmpty(this.TxtTo.Text))
                        {
                            vis = new FilterVisibility();
                            vis.Type = FilterVisibility2.DATE;
                            vis.Value = this.TxtFrom.Text;
                            filterArrayVis.Add(vis);
                            vis.Type = FilterVisibility2.DATE;
                            vis.Value = this.TxtTo.Text;
                            filterArrayVis.Add(vis);
                            this.TxtFrom.Text = string.Empty;
                            this.TxtTo.Text = string.Empty;
                        }
                        break;
                    case "3":
                        if (!string.IsNullOrEmpty(TxtFrom.Text) && !string.IsNullOrEmpty(this.TxtTo.Text))
                        {
                            vis = new FilterVisibility();
                            vis.Type = FilterVisibility2.DATE_WEEK;
                        }
                        break;
                    case "4":
                        if (!string.IsNullOrEmpty(this.TxtFrom.Text) && !string.IsNullOrEmpty(this.TxtTo.Text))
                        {
                            vis = new FilterVisibility();
                            vis.Type = FilterVisibility2.DATE_MONTH;
                        }
                        break;

                }
                //Add value to array
                if (!string.IsNullOrEmpty(this.TxtFrom.Text) || !string.IsNullOrEmpty(this.TxtTo.Text))
                    filterArrayVis.Add(vis);

                switch (this.DdlType.SelectedValue)
                {
                    case "R":
                        // LETTURA
                        vis = new FilterVisibility();
                        vis.Type = FilterVisibility2.TYPE;
                        vis.Value = this.DdlType.SelectedValue;
                        break;
                    case "W":
                        // SCRITTURA
                        vis = new FilterVisibility();
                        vis.Type = FilterVisibility2.TYPE;
                        vis.Value = this.DdlType.SelectedValue;
                        break;
                }
                //Add value to array
                if (!string.IsNullOrEmpty(this.DdlType.SelectedValue))
                    filterArrayVis.Add(vis);

                if (filterArrayVis.Count != 0)
                    this.LoadGridVisibility(filterArrayVis.ToArray());
                else
                    this.LoadGridVisibility(null);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void TxtRole_TextChanged(object sender, EventArgs e)
        {
            try {
                CustomTextArea caller = sender as CustomTextArea;

                if (!string.IsNullOrEmpty(this.TxtRole.Text))
                {
                    this.SearchCorrespondent(this.TxtRole.Text, caller.ID, true, true, false);
                }
                else
                {
                    this.TxtRole.Text = string.Empty;
                    this.TxtDescriptionRole.Text = string.Empty;
                    this.IdRecipient.Value = string.Empty;
                }
                this.UpPnlRole.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void GridDocuments_Details(object sender, EventArgs e)
        {
            try {
                if (!string.IsNullOrEmpty(this.rowIndex.Value))
                {
                    int selRow = int.Parse(this.rowIndex.Value);
                    if (selRow == this.GridDocuments.SelectedIndex) selRow = -1;

                    this.GridDocuments.SelectedIndex = selRow;
                    this.RowSelected = this.GridDocuments.SelectedRow;
                    if (this.DocumentsVisibilityFilters == null || this.DocumentsVisibilityFilters.Count == 0)
                        this.GridDocuments.DataSource = this.DocumentsVisibilityList;
                    else
                        this.GridDocuments.DataSource = this.DocumentsVisibilityFilters;
                    this.GridDocuments.DataBind();

                    if (selRow >= 0)
                    {
                        HiddenField hdnCodRubrica = new HiddenField();
                        Literal LblDetails = new Literal();
                        LblDetails = (Literal)GridDocuments.Rows[selRow].Cells[(int)GrdVisibility.tipo].FindControl("VisibilityLblDetails");
                        hdnCodRubrica = (HiddenField)GridDocuments.Rows[selRow].Cells[(int)GrdVisibility.tipo].FindControl("hdnCodRubrica");
                        this.GetDetailsCorresponding(hdnCodRubrica.Value);
                    }

                    this.UpdPanelVisibility.Update();
                    this.UpContainer.Update();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void VisibilityBtnClear_Click(object sender, EventArgs e)
        {
            try {
                string language = UIManager.UserManager.GetUserLanguage();
                this.IdSender.Value = string.Empty;
                this.TxtUser.Text = string.Empty;
                this.TxtDescriptionUser.Text = string.Empty;
                this.IdRecipient.Value = string.Empty;
                this.TxtRole.Text = string.Empty;
                this.TxtDescriptionRole.Text = string.Empty;
                this.HiddenField2.Value = string.Empty;
                this.DdlCause.SelectedIndex = -1;
                this.DdlType.SelectedIndex = -1;
                this.DdlDate.SelectedIndex = -1;
                this.TxtFrom.Text = string.Empty;
                this.TxtTo.Text = string.Empty;
                this.TxtFrom.ReadOnly = false;
                this.TxtTo.Visible = false;
                this.VisibilityTo.Visible = false;
                this.LoadGridVisibility(null);
                this.VisibilityFrom.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                this.UpContainer.Update();
                this.UpdPanelVisibility.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        public RubricaCallType CallType
        {
            get
            {
                if (HttpContext.Current.Session["callType"] != null)
                    return (RubricaCallType)HttpContext.Current.Session["callType"];
                else return RubricaCallType.CALLTYPE_PROTO_INT_DEST;
            }
            set
            {
                HttpContext.Current.Session["callType"] = value;
            }
        }

        protected void VisibilityImgAddressBookUser_Click(object sender, EventArgs e)
        {
            try {
                this.CallType = RubricaCallType.CALLTYPE_VIS_UTENTE;
                HttpContext.Current.Session["AddressBook.from"] = "V_U_R_S";
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "UpPnlSender", "ajaxModalPopupAddressBook();", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void VisibilityImgAddressBookRole_Click(object sender, EventArgs e)
        {
            try {
                this.CallType = RubricaCallType.CALLTYPE_VIS_RUOLO;
                HttpContext.Current.Session["AddressBook.from"] = "V_R_R_S";
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "UpPnlSender", "ajaxModalPopupAddressBook();", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void btnAddressBookPostback_Click(object sender, EventArgs e)
        {
            try {
                List<NttDataWA.Popup.AddressBook.CorrespondentDetail> atList = (List<NttDataWA.Popup.AddressBook.CorrespondentDetail>)HttpContext.Current.Session["AddressBook.At"];
                string addressBookCallFrom = HttpContext.Current.Session["AddressBook.from"].ToString();

                if (atList != null && atList.Count > 0)
                {
                    Corrispondente corr = UIManager.AddressBookManager.GetCorrespondentBySystemId(atList[0].SystemID);
                    switch (addressBookCallFrom)
                    {
                        case "V_U_R_S":
                            this.TxtUser.Text = corr.codiceRubrica;
                            this.TxtDescriptionUser.Text = corr.descrizione;
                            this.IdSender.Value = corr.systemId;
                            this.UpPnlUser.Update();
                            break;

                        case "V_R_R_S":
                            this.TxtRole.Text = corr.codiceRubrica;
                            this.TxtDescriptionRole.Text = corr.descrizione;
                            this.IdRecipient.Value = corr.systemId;
                            this.UpPnlRole.Update();
                            break;
                    }
                }

                HttpContext.Current.Session["AddressBook.At"] = null;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void LoadMassiveOperation()
        {
            this.DdlMassiveOperation.Items.Add(new ListItem("", ""));
            string title = string.Empty;
            string language = UIManager.UserManager.GetUserLanguage();

            title = Utils.Languages.GetLabelFromCode("VisibilityRemoveTooltip", language);
            this.DdlMassiveOperation.Items.Add(new ListItem(title, "MASSIVE_REMOVE"));

            title = Utils.Languages.GetLabelFromCode("VisibilityRestoreTooltip", language);
            this.DdlMassiveOperation.Items.Add(new ListItem(title, "MASSIVE_RESTORE"));
        }

        protected void DdlMassiveOperation_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.LoadListCheck();

            if (this.ListCheck != null && this.ListCheck.Count > 0 && !string.IsNullOrEmpty(this.ListCheck[0]))
            {
                if (this.DdlMassiveOperation.SelectedValue == "MASSIVE_REMOVE")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "MassiveRemove", "ajaxModalPopupMassiveVisibilityRemove();", true);
                }
                if (this.DdlMassiveOperation.SelectedValue == "MASSIVE_RESTORE")
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "MassiveRestore", "ajaxModalPopupMassiveVisibilityRestore();", true);
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorMassiveOperationNoItemSelected', 'warning', '');", true);
            }

            this.DdlMassiveOperation.SelectedIndex = -1;
            this.UpnlAzioniMassive.Update();
        }

        private void LoadListCheck()
        {
            this.ListCheck = new List<string>();
            if (this.hdnIds.Value.Contains(","))
            {
                foreach (string id in this.hdnIds.Value.Split(','))
                    this.ListCheck.Add(id);
            }
            else
            {
                this.ListCheck.Add(this.hdnIds.Value);
            }
        }

        protected void GridDocuments_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (ViewState["SortDirection"] == null)
                ViewState["SortDirection"] = (e.SortDirection == SortDirection.Ascending) ? "asc" : "desc";
            else
                ViewState["SortDirection"] = (ViewState["SortDirection"].ToString() == "desc") ? "asc" : "desc";

            ViewState["SortExp"] = e.SortExpression;

            this.VisibilityBtnFilter_Click(null, null);

            this.UpdPanelVisibility.Update();
        }

        protected void GridDocuments_RowCreated(Object sender, GridViewRowEventArgs e)
        {
            try
            {
                //Posizione della freccetta nell'header
                if (e.Row.RowType == DataControlRowType.Header)
                {
                    if (ViewState["SortExp"] != null)
                    {
                        System.Web.UI.WebControls.Image arrow = new System.Web.UI.WebControls.Image();
                        arrow.BorderStyle = BorderStyle.None;

                        if (ViewState["SortDirection"].ToString() == "asc")
                        {
                            arrow.ImageUrl = "../Images/Icons/arrow_up.gif";
                        }
                        else
                        {
                            arrow.ImageUrl = "../Images/Icons/arrow_down.gif";
                        }

                        int cellIndex = -1;
                        switch (ViewState["SortExp"].ToString())
                        {
                            case "Ruolo":
                                cellIndex = (int)GrdVisibility.ruolo;
                                break;
                            case "diritti":
                                cellIndex = (int)GrdVisibility.motivo;
                                break;
                            case "TipoDiritto":
                                cellIndex = (int)GrdVisibility.diritti;
                                break;
                            case "DataInsSecurity":
                                cellIndex = (int)GrdVisibility.datadiritto;
                                break;
                            case "DataFine":
                                cellIndex = (int)GrdVisibility.datafine;
                                break;
                        }
                        e.Row.Cells[cellIndex].Controls.Add(arrow);
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected bool ShowHistoryRole(DocumentsVisibility docVis)
        {
            bool showHistory = false;
            if (docVis.Tipo.Equals("RUOLO"))
                showHistory = docVis.ShowHistory;
            return showHistory;
        }

        #region Sessions

        private int AjaxAddressBookMinPrefixLenght
        {
            get
            {
                int result = 3;
                if (HttpContext.Current.Session["ajaxAddressBookMinPrefixLenght"] != null)
                {
                    result = int.Parse(HttpContext.Current.Session["ajaxAddressBookMinPrefixLenght"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["ajaxAddressBookMinPrefixLenght"] = value;
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

        protected List<DocumentsVisibility> DocumentsVisibilityList
        {
            get
            {
                List<DocumentsVisibility> result = null;
                if (HttpContext.Current.Session["documentsVisibility"] != null)
                {
                    result = HttpContext.Current.Session["documentsVisibility"] as List<DocumentsVisibility>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["documentsVisibility"] = value;
            }
        }
        protected List<DocumentsVisibility> DocumentsVisibilityFilters
        {
            get
            {
                List<DocumentsVisibility> result = null;
                if (HttpContext.Current.Session["documentsVisibilityFilter"] != null)
                {
                    result = HttpContext.Current.Session["documentsVisibilityFilter"] as List<DocumentsVisibility>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["documentsVisibilityFilter"] = value;
            }
        }

        protected GridViewRow RowSelected
        {
            get
            {
                return HttpContext.Current.Session["RowSelected"] as GridViewRow;
            }
            set
            {
                HttpContext.Current.Session["RowSelected"] = value;
            }
        }

        private bool AbortDocument
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["abortDocument"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["abortDocument"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["abortDocument"] = value;
            }
        }

        private string isPersonOrGroup
        {
            get
            {
                return HttpContext.Current.Session["isPersonOrGroup"] as string;
            }
            set
            {
                HttpContext.Current.Session["isPersonOrGroup"] = value;
            }
        }

        public string TypeObject
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["typeObject"] != null)
                {
                    result = HttpContext.Current.Session["typeObject"].ToString();
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["typeObject"] = value;
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

        #endregion

    }
}