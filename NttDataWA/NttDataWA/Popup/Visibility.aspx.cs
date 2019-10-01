using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDatalLibrary;
using NttDataWA.Utils;

namespace NttDataWA.Popup
{
    public partial class Visibility : System.Web.UI.Page
    {

        #region Properties

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

        private string idProfile
        {
            get
            {
                return HttpContext.Current.Session["Visibility_idProfile"] as string;
            }
            set
            {
                HttpContext.Current.Session["Visibility_idProfile"] = value;
            }
        }

        protected SchedaDocumento documentWIP
        {
            get
            {
                return HttpContext.Current.Session["Visibility_document"] as SchedaDocumento;
            }
            set
            {
                HttpContext.Current.Session["Visibility_document"] = value;
            }
        }

        #endregion


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
            tipo = 0,
            ruolo = 1,
            motivo = 2,
            diritti = 3,
            datadiritto = 4,
            datafine = 5,
            noteacquisizione = 6,
            rimuovi = 7,
            storia = 8,
            rimosso = 9,
            note = 10,
            idcorr = 11
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (!IsPostBack)
                {
                    this.InitPage();
                    this.DocumentAtipic();
                    this.LoadGridVisibility(null);
                    this.ControlAbortDocument();
                }
                else
                {
                    this.ReadRetValueFromPopup();
                }
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
            }
        }

        private void InitPage()
        {
            this.LoadDocument();
            this.InitLanguage();
        }

        private void LoadDocument()
        {
            this.documentWIP = DocumentManager.getDocumentDetailsNoSecurity(this, this.idProfile, this.idProfile);
        }

        private void InitLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.BtnClose.Text = Utils.Languages.GetLabelFromCode("GenericBtnClose", language);
            this.GridDocuments.Columns[0].HeaderText = Utils.Languages.GetLabelFromCode("VisibilityType", language);
            this.GridDocuments.Columns[1].HeaderText = Utils.Languages.GetLabelFromCode("VisibilityRoleUser", language);
            this.GridDocuments.Columns[2].HeaderText = Utils.Languages.GetLabelFromCode("VisibilityMotive", language);
            this.GridDocuments.Columns[3].HeaderText = Utils.Languages.GetLabelFromCode("VisibilityRights", language);
            this.GridDocuments.Columns[4].HeaderText = Utils.Languages.GetLabelFromCode("VisibilityDateStarting", language);
            this.GridDocuments.Columns[5].HeaderText = Utils.Languages.GetLabelFromCode("VisibilityDateEnding", language);
            this.GridDocuments.Columns[6].HeaderText = Utils.Languages.GetLabelFromCode("VisibilityNotes", language);
            this.GridDocuments.Columns[8].HeaderText = Utils.Languages.GetLabelFromCode("VisibilityHistory", language);
            this.GridDocuments.Columns[9].HeaderText = Utils.Languages.GetLabelFromCode("VisibilityRemoved", language);
            this.GridDocuments.Columns[10].HeaderText = Utils.Languages.GetLabelFromCode("VisibilityNotes", language);
            this.GridDocuments.Columns[12].HeaderText = Utils.Languages.GetLabelFromCode("VisibilityDetails", language);
        }

        protected void BtnClose_Click(object sender, EventArgs e)
        {
            try {
                this.CloseMask(false);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void CloseMask(bool withReturnValue)
        {         
            string retValue = string.Empty;
            if (withReturnValue) retValue = "true";

            ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "if (parent.fra_main) {parent.fra_main.closeAjaxModal('Visibility', '" + retValue + "');} else {parent.closeAjaxModal('Visibility', '" + retValue + "');};", true);
        }

        protected void DocumentAtipic()
        {
            //Verify document atipic
            DocsPaWR.InfoAtipicita InfoAtipic = null;
            SchedaDocumento doc = this.documentWIP;
            if (doc != null && !string.IsNullOrEmpty(doc.docNumber))
                InfoAtipic = DocumentManager.GetInfoAtipicita(DocsPaWR.TipoOggettoAtipico.DOCUMENTO, doc.docNumber);
        }

        protected void LoadGridVisibility(FilterVisibility[] ListFiltered)
        {
            bool cercaRimossi;
            SchedaDocumento doc = this.documentWIP;

            if (doc == null)
            {
                this.lblResult.Text = "Errore nel reperimento dei dati del documento";
                this.lblResult.Visible = true;
                return;
            }

            InfoDocumento InfoDoc;
            InfoDoc = DocumentManager.getInfoDocumento(doc);
            if (InfoDoc == null)
            {
                this.lblResult.Text = "Errore nel reperimento dei dati del documento";
                this.lblResult.Visible = true;
                return;
            }

            cercaRimossi = true;

            this.VisibilityList = DocumentManager.GetSimpliedListVisibilityWithFilters(this, InfoDoc.idProfile, cercaRimossi, ListFiltered);

            // Create and initialize a generic list
            List<DocumentsVisibility> DocDir = new List<DocumentsVisibility>();
            DocumentsVisibility DocVis = new DocumentsVisibility();


            for (int i = 0; i < this.VisibilityList.Length; i++)
            {
                string descrSoggetto = UserManager.GetCorrespondingDescription(this, this.VisibilityList[i].soggetto);
                string Corr = this.GetTipoCorr(this.VisibilityList[i]);
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

                DocDir.Add(DocVis);
            }
            this.DocumentsVisibilityList = DocDir;

            this.GridDocuments.DataSource = this.DocumentsVisibilityList;
            this.GridDocuments.DataBind();
            this.UpdPanelVisibility.Update();

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

        /// <summary>
        /// Metodo per la generazione di una descrizione estesa del tipo diritto
        /// </summary>
        /// <param name="accessRight">Diritto di accesso</param>
        /// <returns>Descrizione del tipo di diritto</returns>
        protected String GetRightDescription(int accessRight)
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

        private string setTipoDiritto(DocumentoDiritto docDir)
        {
            string star = "";
            string RetVal = "";

            if (docDir.hideDocVersions)
                star = Environment.NewLine + "*";

            switch (docDir.tipoDiritto)
            {
                case DocumentoTipoDiritto.TIPO_ACQUISITO:
                    if (docDir.accessRights == (int)NttDataWA.Utils.HMdiritti.HDdiritti_Waiting)
                        RetVal = "IN ATTESA DI ACCETTAZIONE" + star;
                    else
                        RetVal = "ACQUISITO" + star;
                    break;
                case DocumentoTipoDiritto.TIPO_PROPRIETARIO:
                case DocumentoTipoDiritto.TIPO_DELEGATO:
                    RetVal = "PROPRIETARIO" + star;
                    break;
                case DocumentoTipoDiritto.TIPO_TRASMISSIONE:
                    RetVal = "TRASMISSIONE" + star;
                    break;
                case DocumentoTipoDiritto.TIPO_TRASMISSIONE_IN_FASCICOLO:
                    RetVal = "INSERIMENTO IN FASC." + star;
                    break;
                case DocumentoTipoDiritto.TIPO_SOSPESO:
                    RetVal = "SOSPESO" + star;
                    break;
                case DocumentoTipoDiritto.TIPO_CONSERVAZIONE:
                    RetVal = "CONSERVAZIONE" + star;
                    break;
            }

            return RetVal;

        }

        private void ControlAbortDocument()
        {
            this.AbortDocument = false;
            SchedaDocumento doc = this.documentWIP;
            if (doc != null && !string.IsNullOrEmpty(doc.systemId) && (doc.tipoProto.ToUpper().Equals("A") || doc.tipoProto.ToUpper().Equals("P") || doc.tipoProto.ToUpper().Equals("I")))
            {
                if (doc != null && doc.tipoProto != null && doc.protocollo.protocolloAnnullato != null)
                {
                    this.AbortDocument = true;
                }

            }
        }

        protected void GridDocuments_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try {
                int selRow = 0;

                switch (e.CommandName)
                {
                    case "Select":
                        this.RowSelected = (GridViewRow)(((ImageButton)e.CommandSource).NamingContainer);
                        selRow = RowSelected.DataItemIndex;
                        this.GridDocuments.SelectedRowStyle.BackColor = System.Drawing.Color.Yellow;
                        break;
                    case "Erase":
                        this.RowSelected = (GridViewRow)(((ImageButton)e.CommandSource).NamingContainer);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "VisibilityRemove", "ajaxModalPopupVisibilityRemove();", true);
                        break;
                    case "Ripristina":
                        this.RowSelected = (GridViewRow)(((ImageButton)e.CommandSource).NamingContainer);
                        this.RestoreACL();
                        break;
                    case "Cancel":
                        this.GridDocuments.EditIndex = -1;
                        break;
                }

                this.UpdPanelVisibility.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void RestoreACL()
        {
            string personOrGroup = this.PersonOrGroup();

            DocumentoDiritto[] ListDocDir = VisibilityList;
            DocsPaWR.DocumentoDiritto docDiritti = ListDocDir[RowSelected.DataItemIndex];
            bool result = DocumentManager.RestoreACL(docDiritti, personOrGroup, UserManager.GetInfoUser(),"D");
            if (result)
            {
                this.LoadGridVisibility(null);
                this.GridDocuments.SelectedIndex = -1;
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Attenzione operazione non riuscita');", true);
            }
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

        protected void GridDocuments_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {
            try {
                ((Literal)GridDocuments.Rows[RowSelected.DataItemIndex].Cells[(int)GrdVisibility.tipo].FindControl("LblDetails")).Text = string.Empty;
                ((Literal)GridDocuments.Rows[RowSelected.DataItemIndex].Cells[(int)GrdVisibility.tipo].FindControl("LblDetailsInfo")).Text = string.Empty;
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
                bool ripristina = false;

                ImageButton ImgType = new ImageButton();
                HiddenField hdnCodRubrica = new HiddenField();
                Literal LblDetails = new Literal();
                HiddenField hdnTipo = new HiddenField();
                ImageButton ImgDelete = new ImageButton();
                Image ImgTipo = new Image();
                Label LblEndDate = new Label();
                Label LblRemoved = new Label();
                Label LblDiritto = new Label();

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
                        ImgDelete = (ImageButton)GridDocuments.Rows[i].Cells[(int)GrdVisibility.rimuovi].FindControl("ImgDelete");

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
                            ImgDelete.ImageUrl = "~/Images/Icons/ico_risposta.gif";
                            ImgDelete.CommandName = "Ripristina";
                            ripristina = true;
                        }

                        //Se l'utente è proprietario del documento, non è MAI possibile rimuovere i diritti.
                        //Se l'utente ha la funzione di "editing ACL" può rimuovere i diritti anche ad altri ruoli/utenti.
                        string Diritto = LblDiritto.Text;
                        int IdCorr = Convert.ToInt32(((System.Web.UI.WebControls.GridView)(sender)).DataKeys[i].Value);

                        ImgDelete.Visible = IsACLauthorized && !this.AbortDocument;

                        if (hdnTipo.Value.Equals("UTENTE"))
                        {
                            if (i % 2 == 0) 
                                GridDocuments.Rows[i].CssClass = "NormalRow nopointer";
                            else
                                GridDocuments.Rows[i].CssClass = "AltRow nopointer";

                            if (infoUtente.idPeople != IdCorr.ToString() && !ripristina && IsACLauthorized && !this.AbortDocument)
                                ImgDelete.Visible = true;

                            if (Diritto.Equals("PROPRIETARIO"))
                                ImgDelete.Visible = false;

                            ImgTipo.ImageUrl = "~/Images/Icons/user_icon.png";
                        }
                        else
                        {
                            string jsOnClick = "$('#rowIndex').val('" + GridDocuments.Rows[i].RowIndex.ToString() + "'); $('#btnDetails').click();";
                            GridDocuments.Rows[i].Cells[0].Attributes["onclick"] = jsOnClick;
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
                                ImgDelete.Visible = true;

                            if (Diritto.Equals("PROPRIETARIO"))
                                ImgDelete.Visible = false;

                            ImgTipo.ImageUrl = "~/Images/Icons/role2_icon.png";
                        }
                    }

                    if (ripristina) ImgDelete.Visible = true;

                    if (int.Parse(this.documentWIP.accessRights) < (int)HMdiritti.HMdiritti_Write) ImgDelete.Visible = false;
                }

                //i client senza acl non devono vedere la colonna
                GridDocuments.Columns[4].Visible = IsACLauthorized;
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

            this.UpdPanelVisibility.Update();
        }

    }
}