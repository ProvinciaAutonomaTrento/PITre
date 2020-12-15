using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.Utils;
using NttDataWA.UIManager;
using NttDatalLibrary;

namespace NttDataWA.Popup
{
    public partial class AnswerShowAnswers : System.Web.UI.Page
    {

        protected int numTotPage;
        protected int nRec;
        protected string tipoProto = "";
        protected string tempTipo = "";
        protected string sys = "";
        protected string reg = "";
        protected string segn = "";
        protected DocsPaWR.InfoDocumento[] infoDoc;
        protected DocsPaWR.FiltroRicerca fV1;
        protected DocsPaWR.FiltroRicerca[] fVList;
        protected ArrayList Dg_elem;
        protected DocsPaWR.EtichettaInfo[] etichette;
        protected string arrivo;
        protected string partenza;
        protected string interno;
        protected string grigio;


        #region Properties

        private int PageSize
        {
            get
            {
                int toReturn = 10;
                if (HttpContext.Current.Session["PageSize"] != null) Int32.TryParse(HttpContext.Current.Session["PageSize"].ToString(), out toReturn);
                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["PageSize"] = value;
            }
        }

        private int SelectedPage
        {
            get
            {
                int toReturn = 1;
                if (HttpContext.Current.Session["SelectedPage"] != null) Int32.TryParse(HttpContext.Current.Session["SelectedPage"].ToString(), out toReturn);
                if (toReturn < 1) toReturn = 1;

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["SelectedPage"] = value;
            }

        }

        private int SelectedRowIndex
        {
            get
            {
                int toReturn = -1;
                if (HttpContext.Current.Session["SelectedRowIndex"] != null) Int32.TryParse(HttpContext.Current.Session["SelectedRowIndex"].ToString(), out toReturn);

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["SelectedRowIndex"] = value;
            }

        }

        private int PagesCount
        {
            get
            {
                int toReturn = 0;
                if (HttpContext.Current.Session["PagesCount"] != null) Int32.TryParse(HttpContext.Current.Session["PagesCount"].ToString(), out toReturn);
                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["PagesCount"] = value;
            }
        }

        private DocsPaWR.FiltroRicerca[][] ListaFiltri
        {
            get
            {
                DocsPaWR.FiltroRicerca[][] toReturn = null;
                if (HttpContext.Current.Session["ListaFiltri"] != null) toReturn = (DocsPaWR.FiltroRicerca[][])HttpContext.Current.Session["ListaFiltri"];
                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["ListaFiltri"] = value;
            }
        }

        #endregion



        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                this.getParametersFromSession();

                if (!IsPostBack)
                {
                    this.ClearSessionData();
                    this.SelectedPage = 1;

                    if (tipoProto.ToString().Equals("G")) tempTipo = "G";
                    tipoProto = "T";

                    if (tipoProto.ToString().Equals("G"))
                    {
                        this.litTitle.Text = "Documenti in risposta al documento<br />" + sys;
                        if (this.GetFiltroDocGrigi(sys))
                        {
                            this.LoadData(true);
                            if (infoDoc == null || infoDoc.Length <= 0)
                            {
                                this.RenderMessage("Documenti non trovati");
                            }
                        }
                    }
                    else
                    {
                        string language = UIManager.UserManager.GetUserLanguage();

                        //MODIFICA INSERITA PER CONCATENAMENTO TRASVERSALE
                        if (tempTipo.Equals("G"))
                        {
                            this.litTitle.Text = Utils.Languages.GetLabelFromCode("ChainsAnswerDocuments", language) + "<br />" + sys;
                            if (this.GetFiltroDocGrigi(sys))
                            {
                                this.getLettereProtocolli();
                                this.LoadData(true);
                                if (infoDoc == null || infoDoc.Length <= 0)
                                {
                                    this.RenderMessage(Utils.Languages.GetMessageFromCode("ChainsAnswerNoneDocuments", language));
                                }
                            }
                        }
                        else
                        {
                            this.litTitle.Text = Utils.Languages.GetLabelFromCode("ChainsAnswerDocuments", language) + "<br />" + segn;
                            if (!string.IsNullOrEmpty(sys) && !string.IsNullOrEmpty(reg) && !string.IsNullOrEmpty(tipoProto))
                            {
                                if (this.GetFiltroDocInRisposta(sys, reg, tipoProto))
                                {
                                    this.getLettereProtocolli();
                                    this.LoadData(true);
                                    if (infoDoc == null || infoDoc.Length <= 0)
                                    {
                                        this.RenderMessage(Utils.Languages.GetMessageFromCode("ChainsAnswerNoneDocuments", language));
                                    }
                                }
                            }
                        }
                    }
                }
                if (((ScriptManager)Master.FindControl("ScriptManager1")).IsInAsyncPostBack)
                {
                    // detect action from async postback
                    switch (((ScriptManager)Master.FindControl("ScriptManager1")).AsyncPostBackSourceElementID)
                    {
                        case "upPnlGridList":
                            this.LoadData(true);
                            break;
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void getParametersFromSession()
        {
            SchedaDocumento doc = HttpContext.Current.Session["document"] as SchedaDocumento;
            if (doc!=null) {
                if (!string.IsNullOrEmpty(doc.systemId)) sys = doc.systemId;
                if (doc.registro!=null && !string.IsNullOrEmpty(doc.registro.systemId)) reg = doc.systemId;
                if (doc.protocollo!=null && !string.IsNullOrEmpty(doc.protocollo.segnatura)) segn = doc.protocollo.segnatura;
                if (!string.IsNullOrEmpty(doc.tipoProto)) tipoProto = doc.tipoProto;
            }
        }

        public bool GetFiltroDocGrigi(string docSys)
        {
            try
            {

                this.ListaFiltri = new DocsPaWR.FiltroRicerca[1][];
                this.ListaFiltri[0] = new DocsPaWR.FiltroRicerca[1];
                fVList = new DocsPaWR.FiltroRicerca[0];

                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.TIPO.ToString();
                fV1.valore = "T";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);


                ////Filtro per REGISTRI VISIBILI ALL'UTENTE
                //if (!UserManager.isFiltroAooEnabled())
                //{
                //    fV1 = new DocsPaWR.FiltroRicerca();
                //    fV1.argomento = DocsPaWR.FiltriDocumento.REGISTRI_UTENTE_IN_CONDITION_CON_NULL.ToString();
                //    fV1.valore = (String)Session["inRegCondition"];
                //    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                //}

                //Filtro per ID_PARENT
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.ID_PARENT.ToString();
                fV1.valore = docSys;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.PREDISPOSTO.ToString();
                fV1.valore = "true";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

                this.ListaFiltri[0] = fVList;
                return true;

            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        protected virtual void RenderMessage(string message)
        {
            rowMessage.InnerHtml = message;
            rowMessage.Visible = true;
        }

        protected void CloseMask(string returnValue)
        {
            this.ClearSessionData();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "if (parent.fra_main) {parent.fra_main.closeAjaxModal('AnswerShowAnswers', '" + returnValue + "');} else {parent.closeAjaxModal('AnswerShowAnswers', '" + returnValue + "');};", true);
        }

        private void LoadData(bool updateGrid)
        {
            if (this.SelectedPage.ToString() != this.grid_pageindex.Value)
            {
                this.SelectedPage = 1;
                if (!string.IsNullOrEmpty(this.grid_pageindex.Value)) this.SelectedPage = int.Parse(this.grid_pageindex.Value);
            }


            bool grigi = false;
            if (tipoProto.Equals("G"))
            {
                grigi = true;
            }
            DocsPaWR.InfoUtente infoUt = new DocsPaWR.InfoUtente();
            infoUt = UserManager.GetInfoUser();
            SearchResultInfo[] idProfiles;
            if (!UserManager.isFiltroAooEnabled())
                infoDoc = DocumentManager.getQueryInfoDocumentoPaging(infoUt.idGruppo, infoUt.idPeople, this, this.ListaFiltri, this.SelectedPage, out numTotPage, out nRec, true, grigi, true, false, out idProfiles);
            else
                infoDoc = DocumentManager.getQueryInfoDocumentoPaging(infoUt.idGruppo, infoUt.idPeople, this, this.ListaFiltri, this.SelectedPage, out numTotPage, out nRec, true, grigi, false, false, out idProfiles);

            this.PagesCount = numTotPage;


            //appoggio il risultato in sessione
            Session["listaDocInRisp.infoDoc"] = infoDoc;

            this.BindGrid(infoDoc);
        }

        public void BindGrid(DocsPaWR.InfoDocumento[] infos) 
        {
            DocsPaWR.InfoDocumento currentDoc;
            if (infos != null && infos.Length > 0)
            {
                this.SetMittDestColumnHeader();

                //Costruisco il datagrid
                Dg_elem = new ArrayList();
                string descrDoc = string.Empty;
                int numProt = new Int32();

                for (int i = 0; i < infos.Length; i++)
                {
                    currentDoc = ((DocsPaWR.InfoDocumento)infos[i]);
                    string dataApertura = "";

                    if (currentDoc.dataApertura != null && currentDoc.dataApertura.Length > 0)
                        dataApertura = currentDoc.dataApertura.Substring(0, 10);
                    if (currentDoc.numProt != null)
                        numProt = Int32.Parse(currentDoc.numProt);
                    else
                        numProt = Int32.Parse(currentDoc.docNumber);

                    descrDoc = numProt.ToString();
                    descrDoc = descrDoc + "\n" + dataApertura;

                    string MittDest = "";
                    if (currentDoc.mittDest != null && currentDoc.mittDest.Length > 0)
                    {
                        for (int g = 0; g < currentDoc.mittDest.Length; g++)
                            MittDest += currentDoc.mittDest[g] + " - ";
                        if (currentDoc.mittDest.Length > 0)
                            MittDest = MittDest.Substring(0, MittDest.Length - 3);
                    }

                    string newEtichetta = getEtichetta(currentDoc.tipoProto);

                    Dg_elem.Add(new Cols(descrDoc, currentDoc.oggetto, MittDest, newEtichetta, currentDoc.codRegistro, i, currentDoc.segnatura));
                }


                this.grdList.DataSource = Dg_elem;
                this.grdList.DataBind();

                string message = "";
                int retValue;
                for (int i = 0; i < infos.Length; i++)
                {
                    currentDoc = ((DocsPaWR.InfoDocumento)infos[i]);
                    retValue = DocumentManager.verifyDocumentACL("D", currentDoc.docNumber, UserManager.GetInfoUser(), out message);
                    if (retValue == 0)
                        ((ImageButton)this.grdList.Rows[i].Cells[7].Controls[1]).Visible = false;
                }
            }
            else
            {
                this.grdList.Visible = false;
                this.RenderMessage(Utils.Languages.GetMessageFromCode("ChainsAnswerNoneDocuments", UIManager.UserManager.GetUserLanguage()));
            }

            // rebuild navigator
            this.buildGridNavigator();
        }

        private const int COL_MITT_DEST_INDEX = 3;

        private void SetMittDestColumnHeader()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            string headerText = Utils.Languages.GetLabelFromCode("ChainsAnswerRecipient", language);
            if (tipoProto == "P")
                headerText = Utils.Languages.GetLabelFromCode("ChainsAnswerSender", language); ;
            this.grdList.Columns[COL_MITT_DEST_INDEX].HeaderText = headerText;
        }

        /// <summary>
        /// Prende i dati esistenti per le etichette dei protocolli (Inserita da Fabio)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void getLettereProtocolli()
        {
            DocsPaWR.DocsPaWebService wws = new DocsPaWR.DocsPaWebService();
            DocsPaWR.InfoUtente infoUtente = new DocsPaWR.InfoUtente();
            infoUtente = UserManager.GetInfoUser();
            String idAmm = UserManager.GetInfoUser().idAmministrazione;

            this.etichette = wws.getEtichetteDocumenti(infoUtente, idAmm);
            this.arrivo = etichette[0].Descrizione; //Valore A
            this.partenza = etichette[1].Descrizione; //Valore P
            this.interno = etichette[2].Descrizione; //Valore I
            this.grigio = etichette[3].Descrizione; //Valore G
        }

        private string getEtichetta(string value)
        {
            if ((value == "G") || (value == "R"))
            {
                return this.grigio;
            }
            else
            {
                if (value == "A")
                {
                    return this.arrivo;
                }
                else
                {
                    if (value == "I")
                    {
                        return this.interno;
                    }
                    else
                    {
                        return this.partenza;
                    }
                }
            }
        }

        public bool GetFiltroDocInRisposta(string docSys, string idRegistro, string tipoProto)
        {
            try
            {
                this.ListaFiltri = new DocsPaWR.FiltroRicerca[1][];
                this.ListaFiltri[0] = new DocsPaWR.FiltroRicerca[1];
                fVList = new DocsPaWR.FiltroRicerca[0];

                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DOCUMENTI_IN_RISPOSTA.ToString();
                fV1.valore = "true";
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

                //Filtro per protocolli in PARTENZA
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.TIPO.ToString();
                fV1.valore = "T";

                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                //if (!UserManager.isFiltroAooEnabled())
                //{
                //    if (tipoProto.Equals("A"))
                //    {
                //        //Filtro per REGISTRI VISIBILI ALL'UTENTE
                //        fV1 = new DocsPaWR.FiltroRicerca();
                //        fV1.argomento = DocsPaWR.FiltriDocumento.REGISTRI_UTENTE_IN_CONDITION.ToString();
                //        fV1.valore = (String)Session["inRegCondition"];
                //        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                //    }
                //    else
                //    {
                //        //Filtro per REGISTRO DEL DOCUMENTO PROTOCOLLATO
                //        fV1 = new DocsPaWR.FiltroRicerca();
                //        fV1.argomento = DocsPaWR.FiltriDocumento.REGISTRO.ToString();
                //        fV1.valore = reg;
                //        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                //    }
                //}

                //Filtro per ID_PARENT
                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.ID_PARENT.ToString();
                fV1.valore = docSys;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

                if (tipoProto != "G" && tipoProto != "T")
                {
                    //Filtro per SOLI DOCUMENTI PROTOCOLLATI
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DA_PROTOCOLLARE.ToString();
                    fV1.valore = "0";  //corrisponde a 'false'
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }

                //FILTRO PER PREDISPOSTI
                if (tipoProto == "T")
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.PREDISPOSTO.ToString();
                    fV1.valore = "true";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }

                this.ListaFiltri[0] = fVList;
                return true;

            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        protected void BtnClose_Click(object sender, EventArgs e)
        {
            try {
                this.CloseMask(string.Empty);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        public static System.Drawing.Color GetColor(object segnature)
        {
            if (segnature==null || string.IsNullOrEmpty(segnature.ToString()))
                return System.Drawing.Color.Black;
            else
                return System.Drawing.Color.Red;
        }

        protected void ImgGoDocument_Click(object sender, ImageClickEventArgs e)
        {
            try {
                DocsPaWR.InfoDocumento newinfoDoc;

                ImageButton ibtn1 = sender as ImageButton;
                int rowIndex = Convert.ToInt32(ibtn1.Attributes["RowIndex"]);

                if (rowIndex >= 0)
                {
                    string str_indexSel = this.grdList.Rows[rowIndex].Cells[6].Text;
                    int indexSel = Int32.Parse(str_indexSel);
                    this.infoDoc = (DocsPaWR.InfoDocumento[])Session["listaDocInRisp.infoDoc"];

                    if (indexSel > -1)
                        newinfoDoc = (DocsPaWR.InfoDocumento)this.infoDoc[indexSel];
                    else
                    {
                        newinfoDoc = null;
                        return;
                    }

                    if (newinfoDoc != null)
                    {
                        string message = string.Empty;
                        int retValue = DocumentManager.verifyDocumentACL("D", newinfoDoc.docNumber, UserManager.GetInfoUser(), out message);
                        if (retValue == 0)
                        {
                            string language = UIManager.UserManager.GetUserLanguage();
                            string msg = "ChainsAnswerNoRights";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        }
                        else
                        {
                            Session.Remove("listaDocInRisp.infoDoc");
                            this.CloseMask(newinfoDoc.docNumber);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ImgDelAnswer_Click(object sender, ImageClickEventArgs e)
        {
            try {
                this.getParametersFromSession();
                DocsPaWR.InfoDocumento infoDocSel;

                ImageButton ibtn1 = sender as ImageButton;
                int rowIndex = Convert.ToInt32(ibtn1.Attributes["RowIndex"]);

                if (rowIndex >= 0)
                {
                    string str_indexSel = this.grdList.Rows[rowIndex].Cells[6].Text;
                    int indexSel = Int32.Parse(str_indexSel);
                    this.infoDoc = (DocsPaWR.InfoDocumento[])Session["listaDocInRisp.infoDoc"];

                    if (indexSel > -1)
                    {
                        infoDocSel = (DocsPaWR.InfoDocumento)this.infoDoc[indexSel];
                    }
                    else
                    {
                        infoDocSel = null;
                        return;
                    }

                    //Scollego i documenti
                    bool retValue = DocumentManager.scollegaDocumento(this, infoDocSel.idProfile);

                    if (retValue)
                    {
                        if (this.SelectedPage > 1 && this.grdList.Rows.Count == 1)
                        {
                            this.SelectedPage = this.SelectedPage - 1;
                        }

                        DocsPaWR.InfoDocumento[] newListInfoDocumenti = new DocsPaWR.InfoDocumento[this.infoDoc.Length - 1];

                        if (newListInfoDocumenti.Length == 0)
                        {
                            this.LoadData(true);
                        }
                        else
                        {
                            int index = 0;

                            foreach (DocsPaWR.InfoDocumento infoDocumento in this.infoDoc)
                            {
                                if (infoDocumento.idProfile != infoDocSel.idProfile)
                                {
                                    newListInfoDocumenti[index] = infoDocumento;
                                    index++;
                                }
                            }

                            this.infoDoc = newListInfoDocumenti;
                            Session["listaDocInRisp.infoDoc"] = this.infoDoc;

                            this.BindGrid(this.infoDoc);
                        }

                        this.grdList.SelectedIndex = -1;
                    }
                }
                else
                {
                    string language = UIManager.UserManager.GetUserLanguage();
                    string msg = "ChainsAnswerNoDocumentSelected";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void buildGridNavigator()
        {
            this.plcNavigator.Controls.Clear();

            if (this.PagesCount > 1)
            {
                Panel panel = new Panel();
                panel.EnableViewState = true;
                panel.CssClass = "recordNavigator";

                int startFrom = 1;
                if (this.SelectedPage > 6) startFrom = this.SelectedPage - 5;

                int endTo = 10;
                if (this.SelectedPage > 6) endTo = this.SelectedPage + 5;
                if (endTo > this.PagesCount) endTo = this.PagesCount;

                if (startFrom > 1)
                {
                    LinkButton btn = new LinkButton();
                    btn.EnableViewState = true;
                    btn.Text = "...";
                    btn.Attributes["onclick"] = "$('#grid_pageindex').val(" + (startFrom - 1) + "); __doPostBack('upPnlGridList', ''); return false;";
                    panel.Controls.Add(btn);
                }

                for (int i = startFrom; i <= endTo; i++)
                {
                    if (i == this.SelectedPage)
                    {
                        Literal lit = new Literal();
                        lit.Text = "<span>" + i.ToString() + "</span>";
                        panel.Controls.Add(lit);
                    }
                    else
                    {
                        LinkButton btn = new LinkButton();
                        btn.EnableViewState = true;
                        btn.Text = i.ToString();
                        btn.Attributes["onclick"] = "$('#grid_pageindex').val($(this).text()); __doPostBack('upPnlGridList', ''); return false;";
                        panel.Controls.Add(btn);
                    }
                }

                if (endTo < this.PagesCount)
                {
                    LinkButton btn = new LinkButton();
                    btn.EnableViewState = true;
                    btn.Text = "...";
                    btn.Attributes["onclick"] = "$('#grid_pageindex').val(" + endTo + "); __doPostBack('upPnlGridList', ''); return false;";
                    panel.Controls.Add(btn);
                }

                this.plcNavigator.Controls.Add(panel);
            }

        }

        private void ClearSessionData()
        {
            HttpContext.Current.Session["PageSize"] = null;
            HttpContext.Current.Session["SelectedPage"] = null;
            HttpContext.Current.Session["SelectedRowIndex"] = null;
            HttpContext.Current.Session["PagesCount"] = null;
        }

        public class Cols
        {
            private string data;

            private string oggetto;
            private string mittDest;
            private string tipo;
            private string registro;
            private int chiave;
            private string segnatura;

            public Cols(string data, string oggetto, string mittDest, string tipo, string registro, int chiave, string segnatura)
            {
                this.data = data;
                this.oggetto = oggetto;
                this.tipo = tipo;
                this.registro = registro;
                this.mittDest = mittDest;
                this.chiave = chiave;
                this.segnatura = segnatura;
            }

            public string Data { get { return data; } }
            public string Oggetto { get { return oggetto; } }
            public string Tipo { get { return tipo; } }
            public string Registro { get { return registro; } }
            public string MittDest { get { return mittDest; } }
            public int Chiave { get { return chiave; } }
            public string Segnatura { get { return segnatura; } }


        }

    }
}