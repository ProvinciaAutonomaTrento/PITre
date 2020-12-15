using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.UIManager;
using NttDataWA.DocsPaWR;
using NttDatalLibrary;
using System.Collections;

namespace NttDataWA.Deposito
{
    public partial class Versamento : System.Web.UI.Page
    {
        #region Fields

        protected int CountProjects
        {
            get
            {
                int result = 0;

                if (HttpContext.Current.Session["CountProjects"] != null)
                {
                    result = int.Parse(HttpContext.Current.Session["CountProjects"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["CountProjects"] = value;
            }
        }

        protected int CountDocuments
        {
            get
            {
                int result = 0;

                if (HttpContext.Current.Session["CountProjects"] != null)
                {
                    result = int.Parse(HttpContext.Current.Session["CountProjects"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["CountProjects"] = value;
            }
        }

        protected int Versamento_system_id
        {
            get
            {
                int result = 0;

                if (HttpContext.Current.Session["Versamento_system_id"] != null)
                {
                    result = int.Parse(HttpContext.Current.Session["Versamento_system_id"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["Versamento_system_id"] = value;
            }
        }

        protected string ListaVersamentoIDANDPolicyID
        {
            get
            {
                string result = string.Empty;
                if (Session["ListaVersamentoIDANDPolicyID"] != null)
                {
                    result = Session["ListaVersamentoIDANDPolicyID"].ToString();
                }

                return result;
            }
            set
            {
                Session["ListaVersamentoIDANDPolicyID"] = value;
            }
        }

        #region Oggetto TransferPolicyView/TransferPolicyView Original in Context
        protected List<ARCHIVE_View_Policy> TransferPolicyViewOriginalInContext
        {
            get
            {
                List<ARCHIVE_View_Policy> result = null;
                if (HttpContext.Current.Session["TransferPolicyViewOriginalInContext"] != null)
                {
                    result = HttpContext.Current.Session["TransferPolicyViewOriginalInContext"] as List<ARCHIVE_View_Policy>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["TransferPolicyViewOriginalInContext"] = value;
            }
        }

        protected List<ARCHIVE_View_Policy> TransferPolicyViewInContext
        {
            get
            {
                List<ARCHIVE_View_Policy> result = null;
                if (HttpContext.Current.Session["TransferPolicyViewInContext"] != null)
                {
                    result = HttpContext.Current.Session["TransferPolicyViewInContext"] as List<ARCHIVE_View_Policy>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["TransferPolicyViewInContext"] = value;
            }
        }
        #endregion

        #region Oggetto Transfer/Transfer Original in Context

        protected List<ARCHIVE_Transfer> TransferOriginalInContext
        {
            get
            {
                List<ARCHIVE_Transfer> result = null;
                if (HttpContext.Current.Session["TransferOriginalInContext"] != null)
                {
                    result = HttpContext.Current.Session["TransferOriginalInContext"] as List<ARCHIVE_Transfer>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["TransferOriginalInContext"] = value;
            }
        }

        protected List<ARCHIVE_Transfer> TransferInContext
        {
            get
            {
                List<ARCHIVE_Transfer> result = null;
                if (HttpContext.Current.Session["TransferInContext"] != null)
                {
                    result = HttpContext.Current.Session["TransferInContext"] as List<ARCHIVE_Transfer>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["TransferInContext"] = value;
            }
        }
        #endregion

        #region Oggetto TransferPolicy/TransferPolicy Original in Context
        protected List<ARCHIVE_TransferPolicy> TransferPolicyOriginalInContext
        {
            get
            {
                List<ARCHIVE_TransferPolicy> result = null;
                if (HttpContext.Current.Session["TransferPolicyOriginalInContext"] != null)
                {
                    result = HttpContext.Current.Session["TransferPolicyOriginalInContext"] as List<ARCHIVE_TransferPolicy>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["TransferPolicyOriginalInContext"] = value;
            }
        }

        protected List<ARCHIVE_TransferPolicy> TransferPolicyInContext
        {
            get
            {
                List<ARCHIVE_TransferPolicy> result = null;
                if (HttpContext.Current.Session["TransferPolicyInContext"] != null)
                {
                    result = HttpContext.Current.Session["TransferPolicyInContext"] as List<ARCHIVE_TransferPolicy>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["TransferPolicyInContext"] = value;
            }
        }
        #endregion

        #region Oggetto TransferState/TransferState Original in Context

        protected List<ARCHIVE_TransferState> TransferStateOriginalInContext
        {
            get
            {
                List<ARCHIVE_TransferState> result = null;
                if (HttpContext.Current.Session["TransferStateOriginalInContext"] != null)
                {
                    result = HttpContext.Current.Session["TransferStateOriginalInContext"] as List<ARCHIVE_TransferState>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["TransferStateOriginalInContext"] = value;
            }
        }

        protected List<ARCHIVE_TransferState> TransferStateInContext
        {
            get
            {
                List<ARCHIVE_TransferState> result = null;
                if (HttpContext.Current.Session["TransferStateInContext"] != null)
                {
                    result = HttpContext.Current.Session["TransferStateInContext"] as List<ARCHIVE_TransferState>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["TransferStateInContext"] = value;
            }
        }
        #endregion

        #region Oggetto TransferStateType/TransferStateType Original in Context

        protected List<ARCHIVE_TransferStateType> TransferStateOriginalTypeInContext
        {
            get
            {
                List<ARCHIVE_TransferStateType> result = null;
                if (HttpContext.Current.Session["TransferStateOriginalTypeInContext"] != null)
                {
                    result = HttpContext.Current.Session["TransferStateOriginalTypeInContext"] as List<ARCHIVE_TransferStateType>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["TransferStateOriginalTypeInContext"] = value;
            }
        }

        protected List<ARCHIVE_TransferStateType> TransferStateTypeInContext
        {
            get
            {
                List<ARCHIVE_TransferStateType> result = null;
                if (HttpContext.Current.Session["TransferStateTypeInContext"] != null)
                {
                    result = HttpContext.Current.Session["TransferStateTypeInContext"] as List<ARCHIVE_TransferStateType>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["TransferStateTypeInContext"] = value;
            }
        }
        #endregion

        #region Oggetto TransferViewDocumentsPolicy

        protected List<ARCHIVE_View_Documents_Policy> TransferPolicyViewDocumentsPolicyInContext
        {
            get
            {
                List<ARCHIVE_View_Documents_Policy> result = null;
                if (HttpContext.Current.Session["TransferPolicyViewDocumentsPolicyInContext"] != null)
                {
                    result = HttpContext.Current.Session["TransferPolicyViewDocumentsPolicyInContext"] as List<ARCHIVE_View_Documents_Policy>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["TransferPolicyViewDocumentsPolicyInContext"] = value;
            }
        }

        #endregion

        #region Oggetto TransferViewDocumentsPolicy

        protected List<ARCHIVE_View_Projects_Policy> TransferPolicyViewProjectsPolicyInContext
        {
            get
            {
                List<ARCHIVE_View_Projects_Policy> result = null;
                if (HttpContext.Current.Session["TransferPolicyViewProjectsPolicyInContext"] != null)
                {
                    result = HttpContext.Current.Session["TransferPolicyViewProjectsPolicyInContext"] as List<ARCHIVE_View_Projects_Policy>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["TransferPolicyViewProjectsPolicyInContext"] = value;
            }
        }

        #endregion

        public struct PolicyVisibility
        {
            public string IDPolicy { get; set; }
            public string Descrizione { get; set; }
            public string NumeroDocumenti { get; set; }
            public string NumeroFascicoli { get; set; }
        }

        private enum GrdVisibility
        {
            IDPolicy = 0,
            Descrizione = 1,
            NumeroDocumenti = 2,
            NumeroFascicoli = 3
        }

        public enum PageState
        {
            NEW,
            MOD,
            SEA
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    //TEST 
                    DocsPaWR.DocsPaWebService _ws = new DocsPaWebService();
                    //_ws.IsIDdocInArchive(983953);
                    //



                    this.InitializePage();
                    //RefreshScript();
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

        #region Reset

        private void ResetControlPage()
        {
            //TextBox
            TxtIdVersamento.Text = string.Empty;
            TxtDescrizioneVersamento.Text = string.Empty;
            TxtNoteVersamento.Text = string.Empty;
            TxtDATAStatoInDefinizione.Text = string.Empty;
            TxtDATAStatoProposto.Text = string.Empty;
            TxtDATAStatoApprovato.Text = string.Empty;
            TxtDATAStatoInEsecuzione.Text = string.Empty;
            TxtDATAStatoInEsecuzione.Text = string.Empty;
            TxtDATAStatoCompletato.Text = string.Empty;
            TxtDATAStatoErrore.Text = string.Empty;
            //Grid
            LoadDataInGrid();
        }

        private void ResetOnlyDataAreaTextBox()
        {
            TxtDATAStatoInDefinizione.Text = string.Empty;
            TxtDATAStatoProposto.Text = string.Empty;
            TxtDATAStatoApprovato.Text = string.Empty;
            TxtDATAStatoInEsecuzione.Text = string.Empty;
            TxtDATAStatoInEsecuzione.Text = string.Empty;
            TxtDATAStatoCompletato.Text = string.Empty;
            TxtDATAStatoErrore.Text = string.Empty;
        }

        private void ResetMySession()
        {
            this.Session["ID_VERSAMENTO"] = null;
            Versamento_system_id = 0;
            this.Session["PAGESTATE"] = null;
        }

        private void ResetInContext()
        {
            this.TransferInContext = null;
            this.TransferOriginalInContext = null;
            this.TransferPolicyInContext = null;
            this.TransferPolicyOriginalInContext = null;
            this.TransferPolicyViewDocumentsPolicyInContext = null;
            this.TransferPolicyViewInContext = null;
            this.TransferPolicyViewOriginalInContext = null;
            this.TransferPolicyViewProjectsPolicyInContext = null;
            this.TransferStateInContext = null;
            this.TransferStateOriginalInContext = null;
            this.TransferStateOriginalTypeInContext = null;
            this.TransferStateTypeInContext = null;
            this.Versamento_system_id = 0;
        }

        #endregion

        private void RefreshScript()
        {
            //  ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Policy", "ajaxModalPopupPolicy();", true);
        }

        #region Gestione Griglia Policy

        protected void LoadDataInGrid()
        {
            try
            {
                //DA CAMBIARE!!!!!!!!!!!!!!!!!!!!!!!!!!! 
                //****************************************
                //****************************************

                //Devo controllare non la TransferPolicyInContext ma i due datasource delle viste 
                //Documenti e fascicoli.
                //Perchè posso avere policy che non hanno nbe doc ne fasc trovati nella ricerca.
                // VA CAMBIATA!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //Label di segnalazione:
                string language = UIManager.UserManager.GetUserLanguage();
                //GRID POLICY:
                if (TransferPolicyInContext != null && TransferPolicyInContext.Count != 0)
                {
                    this.LitNoPolicyInTransfer.Text = Utils.Languages.GetLabelFromCode("LitPolicyInTransfer", language);

                    this.gvPolicy.DataSource = this.TransferPolicyViewInContext.Where(p => p.Totale_documenti > 0 || p.Totale_fascicoli > 0).ToList();

                    //Controllo la grid policy vuota:
                    this.gvPolicy.DataBind();
                    if (gvPolicy.Rows.Count == 0)
                    {
                        this.gvPolicy.DataSource = UIManager.ArchiveManager.GetDataSourceVuotoPerGrigliaPolicy();
                        this.gvPolicy.DataBind();
                        this.gvPolicy.Rows[0].Visible = false;
                    }
                    //Lista Documenti collegati alla policy:
                    //E' possibile che in fase di bind, la policy creata e completa
                    //non abbia nessun documento e Fascicolo associato.
                    if (this.TransferPolicyViewDocumentsPolicyInContext != null && this.TransferPolicyViewDocumentsPolicyInContext.Count > 0)
                    {
                        this.gvDocumentInPolicyTransfer.DataSource = this.TransferPolicyViewDocumentsPolicyInContext;
                        this.gvDocumentInPolicyTransfer.DataBind();
                        this.LitNoDocumentInPolicyTransfer.Text = Utils.Languages.GetLabelFromCode("LitDocumentTOTInPolicyTransfer", language).Replace("@", "[" + this.CountDocuments.ToString() + "]");
                    }
                    else
                    {
                        this.gvDocumentInPolicyTransfer.DataSource = UIManager.ArchiveManager.GetDataSourceVuotoPerGrigliaDocumentiPolicy();
                        this.gvDocumentInPolicyTransfer.DataBind();
                        this.gvDocumentInPolicyTransfer.Rows[0].Visible = false;
                        this.LitNoDocumentInPolicyTransfer.Text = Utils.Languages.GetLabelFromCode("LitNoDocumentInPolicyTransfer", language);
                    }


                    //Lista Projects collegati alla policy:
                    if (TransferPolicyViewProjectsPolicyInContext != null && TransferPolicyViewProjectsPolicyInContext.Count > 0)
                    {
                        this.gvProjectInPolicyTransfer.DataSource = this.TransferPolicyViewProjectsPolicyInContext;
                        this.gvProjectInPolicyTransfer.DataBind();
                        this.LitNoProjectInPolicyTransfer.Text = Utils.Languages.GetLabelFromCode("LitProjectTOTInPolicyTransfer", language).Replace("@", "[" + this.CountProjects.ToString() + "]");
                    }
                    else
                    {
                        this.gvProjectInPolicyTransfer.DataSource = UIManager.ArchiveManager.GetDataSourceVuotoPerGrigliaFascicoliPolicy();
                        this.gvProjectInPolicyTransfer.DataBind();
                        this.gvProjectInPolicyTransfer.Rows[0].Visible = false;
                        this.LitNoProjectInPolicyTransfer.Text = Utils.Languages.GetLabelFromCode("LitNoProjectInPolicyTransfer", language);
                    }
                }
                else
                {
                    //Label di segnalazione:
                    this.LitNoDocumentInPolicyTransfer.Text = Utils.Languages.GetLabelFromCode("LitNoDocumentInPolicyTransfer", language);
                    this.LitNoProjectInPolicyTransfer.Text = Utils.Languages.GetLabelFromCode("LitNoProjectInPolicyTransfer", language);
                    this.LitNoPolicyInTransfer.Text = Utils.Languages.GetLabelFromCode("LitNoPolicyInTransfer", language);

                    //Assegno un source vuoto per fare cmq il bind: policy
                    this.gvPolicy.DataSource = UIManager.ArchiveManager.GetDataSourceVuotoPerGrigliaPolicy();
                    this.gvPolicy.DataBind();
                    this.gvPolicy.Rows[0].Visible = false;
                    //Assegno un source vuoto per fare cmq il bind: documents in Policy
                    this.gvDocumentInPolicyTransfer.DataSource = UIManager.ArchiveManager.GetDataSourceVuotoPerGrigliaDocumentiPolicy();
                    this.gvDocumentInPolicyTransfer.DataBind();
                    this.gvDocumentInPolicyTransfer.Rows[0].Visible = false;
                    //Assegno un source vuoto per fare cmq il bind: project in policy
                    this.gvProjectInPolicyTransfer.DataSource = UIManager.ArchiveManager.GetDataSourceVuotoPerGrigliaFascicoliPolicy();
                    this.gvProjectInPolicyTransfer.DataBind();
                    this.gvProjectInPolicyTransfer.Rows[0].Visible = false;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        private void LoadViewDataInGrid()
        {
            try
            {
                string language = UIManager.UserManager.GetUserLanguage();
                //GRID POLICY:
                if (TransferPolicyInContext != null && TransferPolicyInContext.Count != 0)
                {
                    //Lista Documenti collegati alla policy:
                    //E' possibile che in fase di bind, la policy creata e completa
                    //non abbia nessun documento e Fascicolo associato.
                    if (this.TransferPolicyViewDocumentsPolicyInContext.Count > 0)
                    {
                        this.gvDocumentInPolicyTransfer.DataSource = this.TransferPolicyViewDocumentsPolicyInContext;
                        this.gvDocumentInPolicyTransfer.DataBind();
                        this.LitNoDocumentInPolicyTransfer.Text = Utils.Languages.GetLabelFromCode("LitDocumentTOTInPolicyTransfer", language).Replace("@", "[" + this.CountDocuments.ToString() + "]");
                    }
                    else
                    {
                        this.gvDocumentInPolicyTransfer.DataSource = UIManager.ArchiveManager.GetDataSourceVuotoPerGrigliaDocumentiPolicy();
                        this.gvDocumentInPolicyTransfer.DataBind();
                        this.gvDocumentInPolicyTransfer.Rows[0].Visible = false;
                        this.LitNoDocumentInPolicyTransfer.Text = Utils.Languages.GetLabelFromCode("LitNoDocumentInPolicyTransfer", language);
                    }


                    //Lista Projects collegati alla policy:
                    if (TransferPolicyViewProjectsPolicyInContext.Count > 0)
                    {
                        this.gvProjectInPolicyTransfer.DataSource = this.TransferPolicyViewProjectsPolicyInContext;
                        this.gvProjectInPolicyTransfer.DataBind();
                        this.LitNoProjectInPolicyTransfer.Text = Utils.Languages.GetLabelFromCode("LitProjectTOTInPolicyTransfer", language).Replace("@", "[" + this.CountProjects.ToString() + "]"); ;
                    }
                    else
                    {
                        this.gvProjectInPolicyTransfer.DataSource = UIManager.ArchiveManager.GetDataSourceVuotoPerGrigliaFascicoliPolicy();
                        this.gvProjectInPolicyTransfer.DataBind();
                        this.gvProjectInPolicyTransfer.Rows[0].Visible = false;
                        this.LitNoProjectInPolicyTransfer.Text = Utils.Languages.GetLabelFromCode("LitNoProjectInPolicyTransfer", language);
                    }
                }
                else
                {
                    //Label di segnalazione:
                    this.LitNoDocumentInPolicyTransfer.Text = Utils.Languages.GetLabelFromCode("LitNoDocumentInPolicyTransfer", language);
                    this.LitNoProjectInPolicyTransfer.Text = Utils.Languages.GetLabelFromCode("LitNoProjectInPolicyTransfer", language);
                    this.LitNoPolicyInTransfer.Text = Utils.Languages.GetLabelFromCode("LitNoPolicyInTransfer", language);

                    //Assegno un source vuoto per fare cmq il bind: policy
                    this.gvPolicy.DataSource = UIManager.ArchiveManager.GetDataSourceVuotoPerGrigliaPolicy();
                    this.gvPolicy.DataBind();
                    this.gvPolicy.Rows[0].Visible = false;
                    //Assegno un source vuoto per fare cmq il bind: documents in Policy
                    this.gvDocumentInPolicyTransfer.DataSource = UIManager.ArchiveManager.GetDataSourceVuotoPerGrigliaDocumentiPolicy();
                    this.gvDocumentInPolicyTransfer.DataBind();
                    this.gvDocumentInPolicyTransfer.Rows[0].Visible = false;
                    //Assegno un source vuoto per fare cmq il bind: project in policy
                    this.gvProjectInPolicyTransfer.DataSource = UIManager.ArchiveManager.GetDataSourceVuotoPerGrigliaFascicoliPolicy();
                    this.gvProjectInPolicyTransfer.DataBind();
                    this.gvProjectInPolicyTransfer.Rows[0].Visible = false;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        protected void gvPolicy_PreRender(object sender, EventArgs e)
        {
            try
            {
                string language = UIManager.UserManager.GetUserLanguage();
                //VEDERE COME SI METTE
                string TooltipChangePolicyInTransfer = Utils.Languages.GetLabelFromCode("TooltipChangePolicyInTransfer", language);

                for (int i = 0; i < gvPolicy.Rows.Count; i++)
                {
                    if (gvPolicy.Rows[i].DataItemIndex >= 0)
                    {
                        int IdPolicy = Convert.ToInt32(((System.Web.UI.WebControls.GridView)(sender)).DataKeys[i].Value);
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void gvPolicy_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {
            try
            {
                //PANNELLI DOCUMENTS E FASCICOLI.
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        #endregion

        /// <summary>
        /// Gestore dei popup di conferma!! Devo cambiare il nome, è poco chiaro così.
        /// 
        /// </summary>
        private void ReadRetValueFromPopup()
        {
            //Cambio stato ANALIZZA
            if (!string.IsNullOrEmpty(this.HiddenConfirmAnalisiVersamento.Value))
            {
                this.HiddenConfirmAnalisiVersamento.Value = string.Empty;
                //CAMBIO STATO ANALIZZA
                bool okChangeInAnalizzaVersamento = false;

                ///////////////////////////////////////////////////////////////////////////////////////////////
                //  Limiti Stefano
                //  06/08/2013
                //  aggiunta dell'aggiornamento delle policy nel db prima di avviare l'analisi del versamento
                //  altrimenti si analizza sempre il versamento senza nessuna policy abilitata
                ///////////////////////////////////////////////////////////////////////////////////////////////
               
                bool okUpdatepolicy = false;
                
                okUpdatepolicy = UIManager.ArchiveManager.UpdateARCHIVE_TransferPolicyMassive(this.TransferPolicyInContext);
                
                if (okUpdatepolicy)
                {
                    okChangeInAnalizzaVersamento = UIManager.ArchiveManager.StartAnalysisForTransfer(this.Versamento_system_id);

                    if (okChangeInAnalizzaVersamento)
                    {
                        this.GetDataByPageState();
                        ManagerBtnByTransferState();
                        this.upTranferPolicy.Update();
                        this.UpTranfetButtons.Update();
                        this.upHiddenFieldStatoApprovato.Update();
                        this.upTextBoxTransferState.Update();
                    }
                }

                ///////////////////////////////////////////////////////////////////////////////////////////////
                //  Fine modifica
                ///////////////////////////////////////////////////////////////////////////////////////////////
            }
            //Ritorno dallo stato Proposto a In Definizione per l a
            //Modifica delle policy incluse.
            if (!string.IsNullOrEmpty(this.HiddenConfirmBackToDefinition.Value))
            {
                this.HiddenConfirmBackToDefinition.Value = string.Empty;
                //CAMBIO STATO PROPONI
                bool okDeleteState = false;
                List<ARCHIVE_TransferState> _mytrsf = new List<ARCHIVE_TransferState>();

                _mytrsf = this.TransferStateInContext.Where(p => p.TransferStateType_ID == 2).Cast<DocsPaWR.ARCHIVE_TransferState>().ToList();

                okDeleteState = UIManager.ArchiveManager.DeleteARCHIVE_TransferState(_mytrsf[0].System_ID);
                if (okDeleteState)
                {
                    bool okupdateTransferPolicy = UIManager.ArchiveManager.UpdateARCHIVE_TransferPolicyMassive(this.TransferPolicyInContext);

                    this.GetDataByPageState();
                    ManagerBtnByTransferState();
                    this.upTranferPolicy.Update();
                    this.UpTranfetButtons.Update();
                    this.upHiddenCameBackToDefinition.Update();
                    this.upTextBoxTransferState.Update();
                }
            }

            //CAMBIO STATO ESEGUI
            if (!string.IsNullOrEmpty(this.HiddeConfirmEseguiTransfer.Value))
            {
                this.HiddeConfirmEseguiTransfer.Value = string.Empty;
                //CAMBIO STATO PROPONI
                int okinserito = 0;
                UIManager.ArchiveManager.InsertARCHIVE_TransferState(this.Versamento_system_id,
                                                                                 UIManager.ArchiveManager._dictionaryTransferState.Where(x => x.Value == "IN ESECUZIONE").Select(x => x.Key).FirstOrDefault(),
                                                                                ref okinserito);
                if (okinserito > 0)
                {
                    UIManager.ArchiveManager.InsertARCHIVE_JOB_Transfer(this.Versamento_system_id, 3, ref okinserito);
                }

                if (okinserito > 0)
                {
                    this.ResetOnlyDataAreaTextBox();
                    this.GetDataByPageState();
                    ManagerBtnByTransferState();
                    this.upTranferPolicy.Update();
                    this.UpTranfetButtons.Update();
                    this.upHiddenFieldStatoInEsecuzione.Update();
                    this.upTextBoxTransferState.Update();
                }
            }

            //CAMBIO STATO Approvato
            if (!string.IsNullOrEmpty(this.HiddeConfirmApprovaTransfer.Value))
            {
                this.HiddeConfirmApprovaTransfer.Value = string.Empty;
                //CAMBIO STATO PROPONI
                int okinserito = 0;
                UIManager.ArchiveManager.InsertARCHIVE_TransferState(this.Versamento_system_id,
                                                                                 UIManager.ArchiveManager._dictionaryTransferState.Where(x => x.Value == "APPROVATO").Select(x => x.Key).FirstOrDefault(),
                                                                                ref okinserito);
                if (okinserito > 0)
                {
                    ResetOnlyDataAreaTextBox();
                    this.GetDataByPageState();
                    ManagerBtnByTransferState();
                    this.upTranferPolicy.Update();
                    this.UpTranfetButtons.Update();
                    this.upHiddenFieldStatoApprovato.Update();
                    this.upTextBoxTransferState.Update();
                }
            }


            //CAMBIO STATO PRPPOSTO
            if (!string.IsNullOrEmpty(this.HiddeConfirmProponiTransfer.Value))
            {
                this.HiddeConfirmProponiTransfer.Value = string.Empty;
                //CAMBIO STATO PROPONI
                int okinserito = 0;
                UIManager.ArchiveManager.InsertARCHIVE_TransferState(this.Versamento_system_id,
                                                                                 UIManager.ArchiveManager._dictionaryTransferState.Where(x => x.Value == "PROPOSTO").Select(x => x.Key).FirstOrDefault(),
                                                                                 ref okinserito);
                if (okinserito > 0)
                {
                    ResetOnlyDataAreaTextBox();
                    this.GetDataByPageState();
                    ManagerBtnByTransferState();
                    this.upTranferPolicy.Update();
                    this.UpTranfetButtons.Update();
                    this.upHiddenFieldStatoProposto.Update();
                    this.upTextBoxTransferState.Update();
                }
            }

            //DELETE TRANFER
            if (!string.IsNullOrEmpty(this.HiddeConfirmDeleteTransfer.Value))
            {
                this.HiddeConfirmDeleteTransfer.Value = string.Empty;
                //DELETE
                bool okdelete = false;
                okdelete = UIManager.ArchiveManager.DeleteARCHIVE_Transfer(this.Versamento_system_id);
                if (okdelete)
                {
                    Session["PAGESTATE"] = "NEW";
                    this.Versamento_system_id = 0;
                    this.GetDataByPageState();
                    this.ManagerBtnByTransferState();
                    this.CallVisibilityByPageState();
                    this.ResetControlPage();
                    //Update:
                    this.UpHiddenFieldERASE.Update();
                    this.UpTranfetButtons.Update();
                    this.upTextBoxTransferState.Update();
                    this.upTextBoxTransferDetails.Update();
                    this.upTranferPolicy.Update();
                    this.upTransferDocument.Update();
                    this.upTransferProject.Update();

                    // this.upTransferDocumentProject.Update();

                }
            }

            //UPDATE TRANSFER
            if (!string.IsNullOrEmpty(this.HiddeConfirmUpdateTransfer.Value))
            {
                this.HiddeConfirmUpdateTransfer.Value = string.Empty;
                //UPDATE
                bool okupdateTransfer = false;
                bool okupdateTransferPolicy = false;
                okupdateTransfer = UIManager.ArchiveManager.UpdateARCHIVE_Transfer(TxtDescrizioneVersamento.Text.Trim(), TxtNoteVersamento.Text.Trim(), this.Versamento_system_id, TransferInContext[0].ID_Amministrazione);
                //DEVO FARE OVVIAMENTE ANCHE L'UPDATE DELLE ENABLED DELEL POLICY!!!!
                if (okupdateTransfer && this.TransferPolicyInContext != null)
                {
                    okupdateTransferPolicy = UIManager.ArchiveManager.UpdateARCHIVE_TransferPolicyMassive(this.TransferPolicyInContext);
                }
                Session["PAGESTATE"] = "MOD";
                this.Versamento_system_id = Convert.ToInt32(TxtIdVersamento.Text.Trim());
                this.ResetControlPage();
                this.GetDataByPageState();
                ManagerBtnByTransferState();
                this.UpHiddenFieldUPDATE.Update();
                this.UpTranfetButtons.Update();
                this.upTextBoxTransferState.Update();
                this.upTextBoxTransferDetails.Update();
                this.upTranferPolicy.Update();
                this.upTransferDocument.Update();
                this.upTransferProject.Update();
            }
        }

        /// <summary>
        /// Inizializzo la lingua ecc....
        /// </summary>
        private void InitializePage()
        {
            this.InitializeLanguage();
            //DA CAMBIARE!!!!!
            if (Request.QueryString["PAGESTATE"] != null && Request.QueryString["PAGESTATE"].ToUpper() == "NEW")
            {
                ResetMySession();
                ResetInContext();
                Session["PAGESTATE"] = "NEW";
            }
            if (Session["PAGESTATE"] != null && Session["PAGESTATE"].ToString().ToUpper() != "SEA")
                this.VerifyPAGESTATE();
            if (Session["ID_VERSAMENTO"] != null && Session["ID_VERSAMENTO"] != "")
                this.Versamento_system_id = Convert.ToInt32(Session["ID_VERSAMENTO"]);
            this.GetDataByPageState();
            this.CallVisibilityByPageState();
        }

        private bool VerifyPAGESTATE()
        {
            try
            {
                //Get di tutti i record nella tabella TransferState in stato diverso da completato
                List<DocsPaWR.ARCHIVE_Transfer> _lsttrandf = UIManager.ArchiveManager.GetAllARCHIVE_Transfer();
                List<DocsPaWR.ARCHIVE_TransferState> _lstxcheck = UIManager.ArchiveManager.GetAllTransferState();
                var query = (from o in _lsttrandf
                             join p in _lstxcheck
                                  on o.System_ID equals p.Transfer_ID
                             group p by p.Transfer_ID into G
                             select new
                             {
                                 System_id = G.Key,
                                 TransferStateType_ID = G.Max(s => s.TransferStateType_ID)
                             });


                ////prendo tutti quelli con stato diverso da completato e in errore
                var DiversiDaCompletato = from p in query.ToList()
                                          where p.TransferStateType_ID != UIManager.ArchiveManager._dictionaryTransferState.Where
                                                                          (x => x.Value == "EFFETTUATO").Select(x => x.Key).FirstOrDefault()
                                          select p;

                DiversiDaCompletato = DiversiDaCompletato.Where(x => x.TransferStateType_ID != UIManager.ArchiveManager._dictionaryTransferState.Where
                                                                          (y => y.Value == "IN ERRORE").Select(y => y.Key).FirstOrDefault());
                //Se ci sono non si possonbo creare nuovi versamenti.
                if (DiversiDaCompletato.Count() > 0 && DiversiDaCompletato != null)
                {
                    Session["PAGESTATE"] = "MOD";
                    Session["ID_VERSAMENTO"] = DiversiDaCompletato.FirstOrDefault().System_id;

                }
                return true;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        /// <summary>
        /// Caller Hub per il change della visibilità degli oggetti.
        /// </summary>
        /// <param name="p"></param>
        private void CallVisibilityByPageState()
        {

            if (Session["PAGESTATE"] == null)
            {
                VisibilityByPageState((PageState)Enum.Parse(typeof(PageState), "NEW"));
            }

            else
                VisibilityByPageState((PageState)Enum.Parse(typeof(PageState), Session["PAGESTATE"].ToString().ToUpper()));
        }

        /// <summary>
        /// Init oggetti in Lingua
        /// </summary>
        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.LitVersamentoProject.Text = Utils.Languages.GetLabelFromCode("VersamentoTitle", language);
            this.LitVersamentoId.Text = Utils.Languages.GetLabelFromCode("LitVersamentoId", language);
            this.LitVersamentoDescrizione.Text = Utils.Languages.GetLabelFromCode("LitVersamentoDescrizione", language);
            this.litExpandStatoVersamento.Text = Utils.Languages.GetLabelFromCode("litExpandStatoVersamento", language);
            this.litExpandPolicy.Text = Utils.Languages.GetLabelFromCode("litExpandPolicy", language);
            this.TxtStatoInDefinizione.Text = Utils.Languages.GetLabelFromCode("TxtStatoInDefinizione", language);
            this.TxtStatoProposto.Text = Utils.Languages.GetLabelFromCode("TxtStatoProposto", language);
            this.TxtStatoApprovato.Text = Utils.Languages.GetLabelFromCode("TxtStatoApprovato", language);
            this.TxtStatoCompletato.Text = Utils.Languages.GetLabelFromCode("TxtStatoCompletato", language);
            this.TxtStatoErrore.Text = Utils.Languages.GetLabelFromCode("TxtStatoErrore", language);
            this.TxtStatoInEsecuzione.Text = Utils.Languages.GetLabelFromCode("TxtStatoInEsecuzione", language);
            this.LitVersamentoDescrizioneStatoVersamento.Text = Utils.Languages.GetLabelFromCode("LitVersamentoDescrizioneStatoVersamento", language);
            this.LitVersamentoDescrizioneDataStatoVersamento.Text = Utils.Languages.GetLabelFromCode("LitVersamentoDescrizioneDataStatoVersamento", language);
            this.btnVersamentoNuovo.Text = Utils.Languages.GetLabelFromCode("VersamentoNuovo", language);
            this.btnVersamentoAnalizza.Text = Utils.Languages.GetLabelFromCode("VersamentoAnalizza", language);
            this.btnVersamentoProponi.Text = Utils.Languages.GetLabelFromCode("VersamentoProponi", language);
            this.btnVersamentoApprova.Text = Utils.Languages.GetLabelFromCode("VersamentoApprova", language);
            this.btnVersamentoEsegui.Text = Utils.Languages.GetLabelFromCode("VersamentoEsegui", language);
            this.btnVersamentoModifica.Text = Utils.Languages.GetLabelFromCode("VersamentoModifica", language);
            this.btnVersamentoElimina.Text = Utils.Languages.GetLabelFromCode("VersamentoElimina", language);
            this.btnCreaPolicy.Text = Utils.Languages.GetLabelFromCode("CreaPolicy", language);
            this.LitVersamentoNote.Text = Utils.Languages.GetLabelFromCode("LitVersamentoNote", language);
            this.TxtStatoAnalisiCompletata.Text = Utils.Languages.GetLabelFromCode("TxtStatoAnalisiCompletata", language);
            this.cBtnTransferReport.Text = Utils.Languages.GetLabelFromCode("btnVersamentoReport", language);
        }

        /// <summary>
        /// Set the objects visibility by page state
        /// Page state:
        /// enum QPageState
        /// N= NEW
        /// M= MOD
        /// </summary>
        /// <param name="querystrig"></param>
        private void VisibilityByPageState(PageState state)
        {
            //Comune.
            TxtIdVersamento.Enabled = false;
            switch (state)
            {
                //Page state new
                case (PageState.NEW):
                    //Resetto una possibile variabile sporca:
                    Session["ID_VERSAMENTO"] = null;
                    Versamento_system_id = 0;
                    //Pulsanti
                    btnVersamentoAnalizza.Enabled = false;
                    btnVersamentoProponi.Enabled = false;
                    btnVersamentoApprova.Enabled = false;
                    btnVersamentoEsegui.Enabled = false;
                    btnVersamentoModifica.Enabled = false;
                    btnVersamentoElimina.Enabled = false;
                    btnCreaPolicy.Enabled = false;
                    btnVersamentoNuovo.Enabled = true;
                    //TextBox
                    TxtDescrizioneVersamento.Enabled = true;
                    TxtNoteVersamento.Enabled = true;
                    break;

                //Page state Modifiy
                case (PageState.MOD):
                    //Pulsanti
                    ManagerBtnByTransferState();
                    //btnCreaPolicy.Enabled = true;
                    btnVersamentoNuovo.Enabled = false;
                    //Textbox
                    TxtDescrizioneVersamento.Enabled = true;
                    TxtNoteVersamento.Enabled = true;
                    break;
                case (PageState.SEA):
                    //Resetto una possibile variabile sporca:
                    //Pulsanti
                    btnVersamentoAnalizza.Enabled = false;
                    btnVersamentoProponi.Enabled = false;
                    btnVersamentoApprova.Enabled = false;
                    btnVersamentoEsegui.Enabled = false;
                    btnVersamentoModifica.Enabled = false;
                    btnVersamentoElimina.Enabled = false;
                    btnCreaPolicy.Enabled = false;
                    btnVersamentoNuovo.Enabled = false;
                    //TextBox
                    TxtDescrizioneVersamento.Enabled = false;
                    TxtNoteVersamento.Enabled = false;
                    break;

            }
        }

        /// <summary>
        /// Gestore dei pulsanti in base allo stato del Versamento.
        /// </summary>
        private void ManagerBtnByTransferState()
        {
            if (this.TransferStateInContext.Count > 0)
            {
                switch (this.TransferStateInContext.Max(p => p.TransferStateType_ID))
                {
                    //DEVO SOSTUIRE IL CODICE CON LA GET DAL DICTIONARY!!!!
                    //PER ORA METTO gli stati fissi:

                    //1	IN DEFINIZIONE
                    case 1:
                        btnVersamentoNuovo.Enabled = false;
                        if (this.gvPolicy != null && !this.gvPolicy.Rows[0].Visible)
                            btnVersamentoAnalizza.Enabled = false;
                        else
                            btnVersamentoAnalizza.Enabled = true;
                        btnVersamentoProponi.Enabled = false;
                        btnVersamentoApprova.Enabled = false;
                        btnVersamentoEsegui.Enabled = false;
                        btnVersamentoModifica.Enabled = true;
                        btnVersamentoElimina.Enabled = true;
                        btnCreaPolicy.Enabled = true;
                        break;
                    //7 ANALISI COMPLETATA
                    case 2:
                        btnVersamentoNuovo.Enabled = false;
                        btnVersamentoAnalizza.Enabled = false;
                        btnVersamentoProponi.Enabled = true;
                        btnVersamentoApprova.Enabled = false;
                        btnVersamentoEsegui.Enabled = false;
                        btnVersamentoModifica.Enabled = true;
                        btnVersamentoElimina.Enabled = true;
                        btnCreaPolicy.Enabled = false;
                        break;
                    //3	PROPOSTO
                    case 3:
                        btnVersamentoNuovo.Enabled = false;
                        btnVersamentoAnalizza.Enabled = false;
                        btnVersamentoProponi.Enabled = false;
                        btnVersamentoApprova.Enabled = true;
                        btnVersamentoEsegui.Enabled = false;
                        btnVersamentoModifica.Enabled = false;
                        btnVersamentoElimina.Enabled = true;
                        btnCreaPolicy.Enabled = false;
                        break;
                    //4	APPROVATO
                    case 4:
                        btnVersamentoNuovo.Enabled = false;
                        btnVersamentoAnalizza.Enabled = false;
                        btnVersamentoProponi.Enabled = false;
                        btnVersamentoApprova.Enabled = false;
                        btnVersamentoEsegui.Enabled = true;
                        btnVersamentoModifica.Enabled = false;
                        btnVersamentoElimina.Enabled = true;
                        btnCreaPolicy.Enabled = false;
                        break;
                    //5	IN ESECUZIONE
                    case 5:
                        btnVersamentoNuovo.Enabled = false;
                        btnVersamentoAnalizza.Enabled = false;
                        btnVersamentoProponi.Enabled = false;
                        btnVersamentoApprova.Enabled = false;
                        btnVersamentoEsegui.Enabled = false;
                        btnVersamentoModifica.Enabled = false;
                        btnVersamentoElimina.Enabled = false;
                        btnCreaPolicy.Enabled = false;
                        break;
                    //6	EFFETTUATO
                    case 6:
                        btnVersamentoNuovo.Enabled = false;
                        btnVersamentoAnalizza.Enabled = false;
                        btnVersamentoProponi.Enabled = false;
                        btnVersamentoApprova.Enabled = false;
                        btnVersamentoEsegui.Enabled = false;
                        btnVersamentoModifica.Enabled = false;
                        btnVersamentoElimina.Enabled = false;
                        btnCreaPolicy.Enabled = false;
                        break;
                    //7	IN ERRORE
                    case 7:
                        btnVersamentoNuovo.Enabled = false;
                        btnVersamentoAnalizza.Enabled = false;
                        btnVersamentoProponi.Enabled = false;
                        btnVersamentoApprova.Enabled = false;
                        btnVersamentoEsegui.Enabled = false;
                        btnVersamentoModifica.Enabled = false;
                        btnVersamentoElimina.Enabled = false;
                        btnCreaPolicy.Enabled = false;
                        break;

                }
            }
            //Filtro comune a tutti:
            //Se non ci sono policy associate al versamento devo disabilitare alcuni btn.
            //Btn degli stati
            if (this.TransferPolicyInContext == null)
            {
                btnVersamentoAnalizza.Enabled = false;
                btnVersamentoProponi.Enabled = false;
            }


            //Manager dei bottini accesi o spenti a seconda dello stato.

        }

        /// <summary>
        /// Get all date by Transfer systemId 
        /// </summary>
        private void GetDataByPageState()
        {
            if (this.Versamento_system_id != 0)
            {
                //  this.ResetInContext();
                //Devo dividere gli oggetti utili per l'update:
                //ORIGINAL:
                this.TransferOriginalInContext = UIManager.ArchiveManager.GetARCHIVE_TransferBySystem_ID(this.Versamento_system_id);
                this.TransferStateOriginalInContext = UIManager.ArchiveManager.GetARCHIVE_TransferStateByTransfer_ID(this.Versamento_system_id);
                this.TransferStateOriginalTypeInContext = UIManager.ArchiveManager.GetAllARCHIVE_TransferStateType();
                this.TransferPolicyOriginalInContext = UIManager.ArchiveManager.GetARCHIVE_TransferPolicyByTransfer_ID(this.Versamento_system_id);
                //FINE!
                //Devo dividere gli oggetti utili per l'update:
                //CORRENTE:
                this.TransferInContext = UIManager.ArchiveManager.GetARCHIVE_TransferBySystem_ID(this.Versamento_system_id);
                this.TransferStateInContext = UIManager.ArchiveManager.GetARCHIVE_TransferStateByTransfer_ID(this.Versamento_system_id);
                this.TransferStateTypeInContext = UIManager.ArchiveManager.GetAllARCHIVE_TransferStateType();
                this.TransferPolicyInContext = UIManager.ArchiveManager.GetARCHIVE_TransferPolicyByTransfer_ID(this.Versamento_system_id);

                //Valorizzo la vista dei documenti associati alla Policy:
                if (TransferPolicyInContext != null)
                {
                    //Valorizzo anche la vista delle policy associate a quel TranferID FILTRATE per STATO EFFETTUATO!!!!! POI DEVO FARE UN ENUM!!!
                    //Da pulire assolutamnete dopo il TEST!!!!
                    this.TransferPolicyViewInContext = UIManager.ArchiveManager.GetAllARCHIVE_View_Policy(this.Versamento_system_id).Where(x => x.Id_stato == 3 || x.Id_stato == 4 || x.Id_stato == 5).ToList();
                    //Valorizzo la vista dei documenti associati alla Policy:
                    this.TransferPolicyViewDocumentsPolicyInContext = UIManager.ArchiveManager.GetAllARCHIVE_View_Documents_Policy(GetSQLinStringFromPolicy(this.TransferPolicyInContext));
                    //Valorizzo la vista dei fascicoli associati alla Policy:
                    this.TransferPolicyViewProjectsPolicyInContext = UIManager.ArchiveManager.GetAllARCHIVE_View_Projects_Policy(GetSQLinStringFromPolicy(this.TransferPolicyInContext));
                }
                //FINE!
                //TextBox
                TxtIdVersamento.Text = TransferInContext[0].System_ID.ToString();
                TxtDescrizioneVersamento.Text = TransferInContext[0].Description;
                TxtNoteVersamento.Text = TransferInContext[0].Note;

                TxtDATAStatoInDefinizione.Text = String.Join("", from p in TransferStateInContext
                                                                 where p.TransferStateType_ID == UIManager.ArchiveManager._dictionaryTransferState.Where
                                                                                                 (x => x.Value == "IN DEFINIZIONE").Select(x => x.Key).FirstOrDefault()
                                                                 select p.DateTime.ToShortDateString());
                TxtDATAStatoAnalisiCompletata.Text = String.Join("", from p in TransferStateInContext
                                                                     where p.TransferStateType_ID == UIManager.ArchiveManager._dictionaryTransferState.Where
                                                                                                     (x => x.Value == "ANALISI COMPLETATA").Select(x => x.Key).FirstOrDefault()
                                                                     select p.DateTime.ToShortDateString());
                TxtDATAStatoProposto.Text = String.Join("", from p in TransferStateInContext
                                                            where p.TransferStateType_ID == UIManager.ArchiveManager._dictionaryTransferState.Where
                                                                                            (x => x.Value == "PROPOSTO").Select(x => x.Key).FirstOrDefault()
                                                            select p.DateTime.ToShortDateString());
                TxtDATAStatoApprovato.Text = String.Join("", from p in TransferStateInContext
                                                             where p.TransferStateType_ID == UIManager.ArchiveManager._dictionaryTransferState.Where
                                                                                             (x => x.Value == "APPROVATO").Select(x => x.Key).FirstOrDefault()
                                                             select p.DateTime.ToShortDateString());
                TxtDATAStatoInEsecuzione.Text = String.Join("", from p in TransferStateInContext
                                                                where p.TransferStateType_ID == UIManager.ArchiveManager._dictionaryTransferState.Where
                                                                                                (x => x.Value == "IN ESECUZIONE").Select(x => x.Key).FirstOrDefault()
                                                                select p.DateTime.ToShortDateString());
                TxtDATAStatoCompletato.Text = String.Join("", from p in TransferStateInContext
                                                              where p.TransferStateType_ID == UIManager.ArchiveManager._dictionaryTransferState.Where
                                                                                              (x => x.Value == "EFFETTUATO").Select(x => x.Key).FirstOrDefault()
                                                              select p.DateTime.ToShortDateString());
                TxtDATAStatoErrore.Text = String.Join("", from p in TransferStateInContext
                                                          where p.TransferStateType_ID == UIManager.ArchiveManager._dictionaryTransferState.Where
                                                                                          (x => x.Value == "IN ERRORE").Select(x => x.Key).FirstOrDefault()
                                                          select p.DateTime.ToShortDateString());

                cImgBtnReportErrore.Enabled = GetEnabledBaseTextBox(TxtDATAStatoErrore.Text);
                this.cBtnTransferReport.Enabled = this.GetEnabledBaseTextBox(this.TxtDATAStatoCompletato.Text);

                this.VersamentiTabs.enableImpactTabs(this.TransferStateInContext.Max(x => x.TransferStateType_ID) > UIManager.ArchiveManager._dictionaryTransferState.Where
                                                                                          (x => x.Value == "ANALISI COMPLETATA").Select(x => x.Key).FirstOrDefault());

            }
            else
            {
                //Resetto tutte le TextBox.
                ResetControlPage();
                this.VersamentiTabs.enableImpactTabs(false);
            }

            //Grids in form.
            LoadDataInGrid();
        }

        /// <summary>
        /// Utilità code cs per la traduzione in aspx di un booblean.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool GetBool(object obj)
        {
            return obj.ToString() == "1" ? true : false;
        }

        public bool GetEnabledByTransferState()
        {
            if (TransferStateInContext != null)
            {
                switch (this.TransferStateInContext.Max(p => p.TransferStateType_ID))
                {
                    //IN DEFINIZIONE
                    case 1:
                        return true;
                    //ANALISI COMPLETATA
                    case 2:
                        return true;
                }
            }
            return false;
        }

        public bool GetEnabledBaseTextBox(string text)
        {
            if (!string.IsNullOrEmpty(text))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Semplice concatenatore per una SQL con clausola IN dinamica.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private string GetSQLinStringFromPolicy(List<ARCHIVE_TransferPolicy> list)
        {
            int brk = 0;
            string sqlIn = string.Empty;
            //DEVO FILTRARE E PRENDERE SOLO LE ENABLED!
            var listEpured = from lst in list
                             where lst.Enabled == 1
                             select lst;

            foreach (ARCHIVE_TransferPolicy _pol in listEpured)
            {
                if (brk == 0)
                {
                    sqlIn = "" + _pol.System_ID;
                    brk++;
                }
                else
                    sqlIn += "," + _pol.System_ID;
            }
            sqlIn += "";

            return sqlIn;
        }

        /// <summary>
        /// Semplice concatenatore per una SQL con clausola IN dinamica.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private string GetSQLinStringFromPolicy(List<ARCHIVE_TransferPolicy> list, int id_versamento)
        {
            int brk = 0;
            string sqlIn = string.Empty;
            //DEVO FILTRARE E PRENDERE SOLO LE ENABLED!
            var listEpured = from lst in list
                             where lst.Enabled == 1
                             select lst;

            foreach (ARCHIVE_TransferPolicy _pol in listEpured)
            {
                if (brk == 0)
                {
                    sqlIn = "" + id_versamento + "," + _pol.System_ID;
                    brk++;
                }
                else
                    sqlIn += "," + _pol.System_ID;
            }
            sqlIn += "";

            return sqlIn;
        }

        /// <summary>
        /// Versamento Report
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnVersamentoReport_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Versamento Nuovo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnVersamentoNuovo_Click(object sender, EventArgs e)
        {
            //Controllo Campi
            if (string.IsNullOrEmpty(TxtDescrizioneVersamento.Text))
            {
                string msgErrNewArchiveTransfer = "msgErrNewArchiveTransfer";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgErrNewArchiveTransfer.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgErrNewArchiveTransfer.Replace("'", @"\'") + "', 'error', '');}", true);
                return;
            }

            //Devo prima controllare che nessun altro versamento sia in qualche stato fuorchè quello di Completato.
            DocsPaWR.ARCHIVE_TransferState _clsapp = new ARCHIVE_TransferState();
            Dictionary<int, string> _mydict = UIManager.ArchiveManager._dictionaryTransferState;


            //Get di tutti i record nella tabella TransferState in stato diverso da completato
            List<DocsPaWR.ARCHIVE_Transfer> _lsttrandf = UIManager.ArchiveManager.GetAllARCHIVE_Transfer();
            List<DocsPaWR.ARCHIVE_TransferState> _lstxcheck = UIManager.ArchiveManager.GetAllTransferState();
            var query = (from o in _lsttrandf
                         join p in _lstxcheck
                              on o.System_ID equals p.Transfer_ID
                         group p by p.Transfer_ID into G
                         select new
                         {
                             System_id = G.Key,
                             TransferStateType_ID = G.Max(s => s.TransferStateType_ID)
                         });


            ////prendo tutti quelli con stato diverso da completato
            var DiversiDaCompletato = from p in query.ToList()
                                      where p.TransferStateType_ID != UIManager.ArchiveManager._dictionaryTransferState.Where
                                                                      (x => x.Value == "EFFETTUATO").Select(x => x.Key).FirstOrDefault()
                                      select p;

            DiversiDaCompletato = DiversiDaCompletato.Where(x => x.TransferStateType_ID != UIManager.ArchiveManager._dictionaryTransferState.Where
                                                                       (y => y.Value == "IN ERRORE").Select(y => y.Key).FirstOrDefault());

            //Se ci sono non si possonbo creare nuovi versamenti.
            if (DiversiDaCompletato.Count() > 0 && DiversiDaCompletato != null)
            {
                string msgErrNewArchiveTransfer = "msgErrNewArchiveTransferStati";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgErrNewArchiveTransfer.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgErrNewArchiveTransfer.Replace("'", @"\'") + "', 'error', '');}", true);
                TxtDescrizioneVersamento.Text = string.Empty;
                TxtNoteVersamento.Text = string.Empty;
            }
            else
            {
                //Chiamo la ws per la get dell'id appena creato
                int system_idNewTransfer = 0;
                UIManager.ArchiveManager.InsertARCHIVE_Transfer(TxtDescrizioneVersamento.Text.Trim(),
                                                                TxtNoteVersamento.Text.Trim(),
                                                                ref system_idNewTransfer,
                                                                Convert.ToInt32(UserManager.GetInfoUser().idAmministrazione),
                                                                UIManager.ArchiveManager._dictionaryTransferState.Where
                                                                (x => x.Value == "IN DEFINIZIONE").Select(x => x.Key).FirstOrDefault()
                                                                );
                if (system_idNewTransfer > 0)
                {
                    Session["PAGESTATE"] = "MOD";
                    this.Versamento_system_id = system_idNewTransfer;
                    this.GetDataByPageState();
                    ManagerBtnByTransferState();
                    this.upTextBoxTransferDetails.Update();
                    this.upTextBoxTransferState.Update();
                    this.UpTranfetButtons.Update();
                    this.upTranferPolicy.Update();
                }
            }

        }

        /// <summary>
        /// Versamento Analizza (lancio analisi impatto)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnVersamentoAnalizza_Click(object sender, EventArgs e)
        {
            if (TransferStateInContext[0].TransferStateType_ID != 0)
            {
                if (TransferPolicyInContext.Count > 0)
                {
                    string msgConfirm = "btnVersamentoAnalizza_Click";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "if (parent.fra_main) {parent.fra_main.ajaxConfirmModal('" + msgConfirm.Replace("'", @"\'") + "', 'HiddenConfirmAnalisiVersamento', '');} else {parent.ajaxConfirmModal('" + msgConfirm.Replace("'", @"\'") + "', 'HiddenConfirmAnalisiVersamento', '');}", true);
                    this.upHiddenFieldStatoApprovato.Update();
                }
                else
                {
                    //nessuna policy? non si va avanti.
                    string msgErrChangeStateAnalisiCompletata = "msgErrChangeStateAnalisiCompletata";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgErrChangeStateAnalisiCompletata.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgErrChangeStateAnalisiCompletata.Replace("'", @"\'") + "', 'error', '');}", true);
                    return;
                }
            }
        }

        /// <summary>
        /// Passa il versamento in stato proponi
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnVersamentoProponi_Click(object sender, EventArgs e)
        {
            if (TransferStateInContext[0].TransferStateType_ID != 0)
            {
                string msgConfirm = "btnVersamentoProponi_Click";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "if (parent.fra_main) {parent.fra_main.ajaxConfirmModal('" + msgConfirm.Replace("'", @"\'") + "', 'HiddeConfirmProponiTransfer', '');} else {parent.ajaxConfirmModal('" + msgConfirm.Replace("'", @"\'") + "', 'HiddeConfirmProponiTransfer', '');}", true);
                this.upHiddenFieldStatoProposto.Update();
            }
        }

        /// <summary>
        /// Passa il versamento in stato approva
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnVersamentoApprova_Click(object sender, EventArgs e)
        {
            if (TransferStateInContext[0].TransferStateType_ID != 0)
            {
                string msgConfirm = "btnVersamentoApprova_Click";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "if (parent.fra_main) {parent.fra_main.ajaxConfirmModal('" + msgConfirm.Replace("'", @"\'") + "', 'HiddeConfirmApprovaTransfer', '');} else {parent.ajaxConfirmModal('" + msgConfirm.Replace("'", @"\'") + "', 'HiddeConfirmApprovaTransfer', '');}", true);
                this.upHiddenFieldStatoApprovato.Update();
            }
        }

        /// <summary>
        /// Passa il versamento in stato esegui
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnVersamentoEsegui_Click(object sender, EventArgs e)
        {
            if (TransferStateInContext[0].TransferStateType_ID != 0)
            {
                string msgConfirm = "btnVersamentoEsegui_Click";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "if (parent.fra_main) {parent.fra_main.ajaxConfirmModal('" + msgConfirm.Replace("'", @"\'") + "', 'HiddeConfirmEseguiTransfer', '');} else {parent.ajaxConfirmModal('" + msgConfirm.Replace("'", @"\'") + "', 'HiddeConfirmEseguiTransfer', '');}", true);
                this.upHiddenFieldStatoInEsecuzione.Update();
            }
        }

        /// <summary>
        /// modifica il versamento
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnVersamentoModifica_Click(object sender, EventArgs e)
        {
            if (TransferStateInContext[0].TransferStateType_ID != 0)
            {
                string msgConfirm = "btnVersamentoModifica_Click";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "if (parent.fra_main) {parent.fra_main.ajaxConfirmModal('" + msgConfirm.Replace("'", @"\'") + "', 'HiddeConfirmUpdateTransfer', '');} else {parent.ajaxConfirmModal('" + msgConfirm.Replace("'", @"\'") + "', 'HiddeConfirmUpdateTransfer', '');}", true);
                this.UpHiddenFieldUPDATE.Update();
            }

        }

        /// <summary>
        /// Pulsante per il postback del popup Policy.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnPolicyPostback_Click(object sender, EventArgs e)
        {
            this.Versamento_system_id = Convert.ToInt32(Session["Versamento_system_id"]);
            this.GetDataByPageState();
            ManagerBtnByTransferState();
            this.upTransferDocument.Update();
            this.upTranferPolicy.Update();
            this.UpTranfetButtons.Update();
            this.upTextBoxTransferState.Update();
        }

        /// <summary>
        /// Pulsante per il postback del popup del Log.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnTransferErrorLogPostback_click(object sender, EventArgs e)
        {
            //è inutile fare update
            this.upTextBoxTransferState.Update();
        }


        /// <summary>
        /// elimina il versamento
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnVersamentoElimina_Click(object sender, EventArgs e)
        {
            //Va in cascade!
            //Daje!
            if (TransferStateInContext[0].TransferStateType_ID != 0)
            {
                string msgConfirm = "msgVersamentoInfo_Click";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "if (parent.fra_main) {parent.fra_main.ajaxConfirmModal('" + msgConfirm.Replace("'", @"\'") + "', 'HiddeConfirmDeleteTransfer', '');} else {parent.ajaxConfirmModal('" + msgConfirm.Replace("'", @"\'") + "', 'HiddeConfirmDeleteTransfer', '');}", true);
                this.UpHiddenFieldERASE.Update();
            }
        }

        /// <summary>
        /// Creo e gestisco le policy.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCreaPolicy_Click(object sender, EventArgs e)
        {
            try
            {
                //Non so .... Test!
                //ResetMySession();
                Session["Versamento_system_id"] = this.Versamento_system_id;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Policy", "ajaxModalPopupPolicy();", true);
                this.upTranferPolicy.Update();
                this.upTransferDocument.Update();
                this.upTransferProject.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void cImgBtnTransferReport_Click(object sender, EventArgs e)
        {
            try
            {
                HttpContext.Current.Session["CurrentTransfer_ID"] = this.Versamento_system_id;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "TransferReport", "ajaxModalPopupTransferReport();", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }

        }

        /// <summary>
        /// Pulsante per il postback del popup per i report del versamento.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnTransferReportPostback_click(object sender, EventArgs e)
        {
            HttpContext.Current.Session["CurrentTransfer_ID"] = null;

        }

        protected void ckbIncludiEscludi_CheckedChanged(object sender, EventArgs e)
        {
            Boolean _checked = false;
            Int32 _idPoli = 0;
            Int32 _decodeBool = 0;

            if (this.TransferStateInContext.Max(x => x.TransferStateType_ID) == 2)
            {
                string msgConfirm = "msgReturnToDefinition";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "if (parent.fra_main) {parent.fra_main.ajaxConfirmModal('" + msgConfirm.Replace("'", @"\'") + "', 'HiddenConfirmBackToDefinition', '');} else {parent.ajaxConfirmModal('" + msgConfirm.Replace("'", @"\'") + "', 'HiddenConfirmBackToDefinition', '');}", true);
                this.upHiddenCameBackToDefinition.Update();
            }

            //Qui cè tutta la gestione delle policy Abilitate o meno in un Trasferimento.
            foreach (GridViewRow row in gvPolicy.Rows)
            {
                _checked = ((CheckBox)row.FindControl("ckbIncludiEscludi")).Checked;
                _idPoli = Convert.ToInt32(gvPolicy.DataKeys[row.RowIndex].Value);
                _decodeBool = (_checked) ? 1 : 0;
                //Update in context
                (from polInCont in this.TransferPolicyViewInContext
                 where polInCont.Id_policy == _idPoli
                 select polInCont).ToList().ForEach(polInCont => polInCont.Enabled = _decodeBool);

                (from polViewInCont in this.TransferPolicyInContext
                 where polViewInCont.System_ID == _idPoli
                 select polViewInCont).ToList().ForEach(polViewInCont => polViewInCont.Enabled = _decodeBool);
            }

            //Valorizzo la vista dei documenti associati alla Policy:
            this.TransferPolicyViewDocumentsPolicyInContext = UIManager.ArchiveManager.GetAllARCHIVE_View_Documents_Policy(GetSQLinStringFromPolicy(this.TransferPolicyInContext));
            //Valorizzo la vista dei fascicoli associati alla Policy:
            this.TransferPolicyViewProjectsPolicyInContext = UIManager.ArchiveManager.GetAllARCHIVE_View_Projects_Policy(GetSQLinStringFromPolicy(this.TransferPolicyInContext));

            if (TransferPolicyViewDocumentsPolicyInContext == null)
                TransferPolicyViewDocumentsPolicyInContext = UIManager.ArchiveManager.GetDataSourceVuotoPerGrigliaDocumentiPolicy().Where(x => x.Totale != 0).ToList();

            if (TransferPolicyViewProjectsPolicyInContext == null)
                TransferPolicyViewProjectsPolicyInContext = UIManager.ArchiveManager.GetDataSourceVuotoPerGrigliaFascicoliPolicy().Where(x => x.Totale != 0).ToList();

            // this.LoadDataInGrid();
            //this.upTranferPolicy.Update();
            //this.upTransferDocumentProject.Update();
            this.LoadViewDataInGrid();
            this.upTransferDocument.Update();
            this.upTransferProject.Update();
            this.upTranferPolicy.Update();
            this.UpTranfetButtons.Update();

        }

        protected void cImgBtnReportErrore_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                this.ListaVersamentoIDANDPolicyID = this.GetSQLinStringFromPolicy(this.TransferPolicyInContext, this.Versamento_system_id);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "TransferErrorLog", "ajaxModalPopupTransferErrorLog();", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        public void gvDocumentInPolicyTransferPageIndexChanging_click(object sender, GridViewPageEventArgs e)
        {
            this.gvDocumentInPolicyTransfer.DataSource = this.TransferPolicyViewDocumentsPolicyInContext.ToList();
            this.gvDocumentInPolicyTransfer.DataBind();
            this.gvDocumentInPolicyTransfer.PageIndex = e.NewPageIndex;
            this.gvDocumentInPolicyTransfer.DataBind();
            this.upTransferDocument.Update();
        }

        protected void gvProjectInPolicyTransferPageIndexChanging_click(object sender, GridViewPageEventArgs e)
        {
            this.gvProjectInPolicyTransfer.DataSource = this.TransferPolicyViewProjectsPolicyInContext.ToList();
            this.gvProjectInPolicyTransfer.DataBind();
            this.gvProjectInPolicyTransfer.PageIndex = e.NewPageIndex;
            this.gvProjectInPolicyTransfer.DataBind();
            this.upTransferProject.Update();
        }

        protected void gvPolicyPageIndexChanging_click(object sender, GridViewPageEventArgs e)
        {
            this.gvPolicy.DataSource = this.TransferPolicyViewInContext.Where(x => x.Totale_documenti != 0 && x.Totale_fascicoli != 0).ToList();
            //this.gvPolicy.DataBind();
            this.gvPolicy.PageIndex = e.NewPageIndex;
            this.gvPolicy.DataBind();
            //this.upTranferPolicy.Update();
        }

        public string SetCountDocuments(object tot)
        {
            this.CountDocuments = Convert.ToInt32(tot);
            return CountDocuments.ToString();
        }

        public string SetCountProjects(object tot)
        {
            this.CountProjects = Convert.ToInt32(tot);
            return CountProjects.ToString();
        }
    }
}
