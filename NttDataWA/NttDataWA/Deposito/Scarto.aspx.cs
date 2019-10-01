using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;

namespace NttDataWA.Deposito
{
    public partial class Scarto : System.Web.UI.Page
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

        protected int Scarto_system_id
        {
            get
            {
                int result = 0;
                if (HttpContext.Current.Session["Scarto_system_id"] != null)
                {
                    result = int.Parse(HttpContext.Current.Session["Scarto_system_id"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["Scarto_system_id"] = value;
            }
        }

        protected string ListaScartoIDANDPolicyID
        {
            get
            {
                string result = string.Empty;
                if (Session["ListaScartoIDANDPolicyID"] != null)
                {
                    result = Session["ListaScartoIDANDPolicyID"].ToString();
                }

                return result;
            }
            set
            {
                Session["ListaScartoIDANDPolicyID"] = value;
            }
        }

        #region Oggetto Disposal/Disposal Original in Context

        protected List<ARCHIVE_Disposal> DisposalOriginalInContext
        {
            get
            {
                List<ARCHIVE_Disposal> result = null;
                if (HttpContext.Current.Session["DisposalOriginalInContext"] != null)
                {
                    result = HttpContext.Current.Session["DisposalOriginalInContext"] as List<ARCHIVE_Disposal>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["DisposalOriginalInContext"] = value;
            }
        }

        protected List<ARCHIVE_Disposal> DisposalInContext
        {
            get
            {
                List<ARCHIVE_Disposal> result = null;
                if (HttpContext.Current.Session["DisposalInContext"] != null)
                {
                    result = HttpContext.Current.Session["DisposalInContext"] as List<ARCHIVE_Disposal>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["DisposalInContext"] = value;
            }
        }
        #endregion

        #region Oggetto DisposalState/DisposalState Original in Context

        protected List<ARCHIVE_DisposalState> DisposalStateOriginalInContext
        {
            get
            {
                List<ARCHIVE_DisposalState> result = null;
                if (HttpContext.Current.Session["DisposalStateOriginalInContext"] != null)
                {
                    result = HttpContext.Current.Session["DisposalStateOriginalInContext"] as List<ARCHIVE_DisposalState>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["DisposalStateOriginalInContext"] = value;
            }
        }

        protected List<ARCHIVE_DisposalState> DisposalStateInContext
        {
            get
            {
                List<ARCHIVE_DisposalState> result = null;
                if (HttpContext.Current.Session["DisposalStateInContext"] != null)
                {
                    result = HttpContext.Current.Session["DisposalStateInContext"] as List<ARCHIVE_DisposalState>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["DisposalStateInContext"] = value;
            }
        }
        #endregion

        #region Oggetto DisposalStateType/DisposalStateType Original in Context

        protected List<ARCHIVE_DisposalStateType> DisposalStateOriginalTypeInContext
        {
            get
            {
                List<ARCHIVE_DisposalStateType> result = null;
                if (HttpContext.Current.Session["DisposalStateOriginalTypeInContext"] != null)
                {
                    result = HttpContext.Current.Session["DisposalStateOriginalTypeInContext"] as List<ARCHIVE_DisposalStateType>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["DisposalStateOriginalTypeInContext"] = value;
            }
        }

        protected List<ARCHIVE_DisposalStateType> DisposalStateTypeInContext
        {
            get
            {
                List<ARCHIVE_DisposalStateType> result = null;
                if (HttpContext.Current.Session["DisposalStateTypeInContext"] != null)
                {
                    result = HttpContext.Current.Session["DisposalStateTypeInContext"] as List<ARCHIVE_DisposalStateType>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["DisposalStateTypeInContext"] = value;
            }
        }
        #endregion

        #region Oggetto DisposalViewDocumentsPolicy

        protected List<ARCHIVE_View_Documents_Policy> DisposalPolicyViewDocumentsPolicyInContext
        {
            get
            {
                List<ARCHIVE_View_Documents_Policy> result = null;
                if (HttpContext.Current.Session["DisposalPolicyViewDocumentsPolicyInContext"] != null)
                {
                    result = HttpContext.Current.Session["DisposalPolicyViewDocumentsPolicyInContext"] as List<ARCHIVE_View_Documents_Policy>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["DisposalPolicyViewDocumentsPolicyInContext"] = value;
            }
        }

        #endregion

        #region Oggetto DisposalViewDocumentsPolicy

        protected List<ARCHIVE_View_Projects_Policy> DisposalPolicyViewProjectsPolicyInContext
        {
            get
            {
                List<ARCHIVE_View_Projects_Policy> result = null;
                if (HttpContext.Current.Session["DisposalPolicyViewProjectsPolicyInContext"] != null)
                {
                    result = HttpContext.Current.Session["DisposalPolicyViewProjectsPolicyInContext"] as List<ARCHIVE_View_Projects_Policy>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["DisposalPolicyViewProjectsPolicyInContext"] = value;
            }
        }

        #endregion


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
            TxtIdScarto.Text = string.Empty;
            TxtDescrizioneScarto.Text = string.Empty;
            TxtNoteScarto.Text = string.Empty;
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
            this.Session["ID_SCARTO"] = null;
            Scarto_system_id = 0;
            this.Session["PAGESTATE"] = null;
        }

        private void ResetInContext()
        {
            this.DisposalInContext = null;
            this.DisposalOriginalInContext = null;
            this.DisposalPolicyViewDocumentsPolicyInContext = null;
            this.DisposalPolicyViewProjectsPolicyInContext = null;
            this.DisposalStateInContext = null;
            this.DisposalStateOriginalInContext = null;
            this.DisposalStateOriginalTypeInContext = null;
            this.DisposalStateTypeInContext = null;
            this.Scarto_system_id = 0;
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
                //Label di segnalazione:
                string language = UIManager.UserManager.GetUserLanguage();
                //GRID POLICY:
                if (DisposalInContext != null && DisposalInContext.Count != 0)
                {
                    //Lista Documenti collegati alla sospensione:
                    //E' possibile che in fase di bind, la sospensione creata e completa
                    //non abbia nessun documento e Fascicolo associato.
                    if (this.DisposalPolicyViewDocumentsPolicyInContext != null && this.DisposalPolicyViewDocumentsPolicyInContext.Count > 0)
                    {
                        this.gvDocumentInPolicyDisposal.DataSource = this.DisposalPolicyViewDocumentsPolicyInContext;
                        this.gvDocumentInPolicyDisposal.DataBind();
                        this.LitNoDocumentInPolicyDisposal.Text = Utils.Languages.GetLabelFromCode("LitDocumentTOTInDisposal", language).Replace("@", "[" + this.CountDocuments.ToString() + "]");
                    }
                    else
                    {
                        this.gvDocumentInPolicyDisposal.DataSource = UIManager.ArchiveManager.GetDataSourceVuotoPerGrigliaDocumentiPolicy();
                        this.gvDocumentInPolicyDisposal.DataBind();
                        this.gvDocumentInPolicyDisposal.Rows[0].Visible = false;
                        this.LitNoDocumentInPolicyDisposal.Text = Utils.Languages.GetLabelFromCode("LitNoDocumentInDisposal", language);
                    }


                    //Lista Projects collegati alla sospensione:
                    if (DisposalPolicyViewProjectsPolicyInContext != null && DisposalPolicyViewProjectsPolicyInContext.Count > 0)
                    {
                        this.gvProjectInPolicyDisposal.DataSource = this.DisposalPolicyViewProjectsPolicyInContext;
                        this.gvProjectInPolicyDisposal.DataBind();
                        this.LitNoProjectInPolicyDisposal.Text = Utils.Languages.GetLabelFromCode("LitProjectTOTInDisposal", language).Replace("@", "[" + this.CountProjects.ToString() + "]");
                    }
                    else
                    {
                        this.gvProjectInPolicyDisposal.DataSource = UIManager.ArchiveManager.GetDataSourceVuotoPerGrigliaFascicoliPolicy();
                        this.gvProjectInPolicyDisposal.DataBind();
                        this.gvProjectInPolicyDisposal.Rows[0].Visible = false;
                        this.LitNoProjectInPolicyDisposal.Text = Utils.Languages.GetLabelFromCode("LitNoProjectInDisposal", language);
                    }
                }
                else
                {
                    //Label di segnalazione:
                    this.LitNoDocumentInPolicyDisposal.Text = Utils.Languages.GetLabelFromCode("LitNoDocumentInDisposal", language);
                    this.LitNoProjectInPolicyDisposal.Text = Utils.Languages.GetLabelFromCode("LitNoProjectInDisposal", language);
                    //Assegno un source vuoto per fare cmq il bind: documents in Policy
                    this.gvDocumentInPolicyDisposal.DataSource = UIManager.ArchiveManager.GetDataSourceVuotoPerGrigliaDocumentiPolicy();
                    this.gvDocumentInPolicyDisposal.DataBind();
                    this.gvDocumentInPolicyDisposal.Rows[0].Visible = false;
                    //Assegno un source vuoto per fare cmq il bind: project in sospensione
                    this.gvProjectInPolicyDisposal.DataSource = UIManager.ArchiveManager.GetDataSourceVuotoPerGrigliaFascicoliPolicy();
                    this.gvProjectInPolicyDisposal.DataBind();
                    this.gvProjectInPolicyDisposal.Rows[0].Visible = false;
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
                if (DisposalInContext != null && DisposalInContext.Count != 0)
                {
                    //Lista Documenti collegati alla sospensione:
                    //E' possibile che in fase di bind, la sospensione creata e completa
                    //non abbia nessun documento e Fascicolo associato.
                    if (this.DisposalPolicyViewDocumentsPolicyInContext.Count > 0)
                    {
                        this.gvDocumentInPolicyDisposal.DataSource = this.DisposalPolicyViewDocumentsPolicyInContext;
                        this.gvDocumentInPolicyDisposal.DataBind();
                        this.LitNoDocumentInPolicyDisposal.Text = Utils.Languages.GetLabelFromCode("LitDocumentTOTInDisposal", language).Replace("@", "[" + this.CountDocuments.ToString() + "]");
                    }
                    else
                    {
                        this.gvDocumentInPolicyDisposal.DataSource = UIManager.ArchiveManager.GetDataSourceVuotoPerGrigliaDocumentiPolicy();
                        this.gvDocumentInPolicyDisposal.DataBind();
                        this.gvDocumentInPolicyDisposal.Rows[0].Visible = false;
                        this.LitNoDocumentInPolicyDisposal.Text = Utils.Languages.GetLabelFromCode("LitNoDocumentInDisposal", language);
                    }


                    //Lista Projects collegati alla sospensione:
                    if (DisposalPolicyViewProjectsPolicyInContext.Count > 0)
                    {
                        this.gvProjectInPolicyDisposal.DataSource = this.DisposalPolicyViewProjectsPolicyInContext;
                        this.gvProjectInPolicyDisposal.DataBind();
                        this.LitNoProjectInPolicyDisposal.Text = Utils.Languages.GetLabelFromCode("LitProjectTOTInDisposal", language).Replace("@", "[" + this.CountProjects.ToString() + "]"); ;
                    }
                    else
                    {
                        this.gvProjectInPolicyDisposal.DataSource = UIManager.ArchiveManager.GetDataSourceVuotoPerGrigliaFascicoliPolicy();
                        this.gvProjectInPolicyDisposal.DataBind();
                        this.gvProjectInPolicyDisposal.Rows[0].Visible = false;
                        this.LitNoProjectInPolicyDisposal.Text = Utils.Languages.GetLabelFromCode("LitNoProjectInDisposal", language);
                    }
                }
                else
                {
                    //Label di segnalazione:
                    this.LitNoDocumentInPolicyDisposal.Text = Utils.Languages.GetLabelFromCode("LitNoDocumentInDisposal", language);
                    this.LitNoProjectInPolicyDisposal.Text = Utils.Languages.GetLabelFromCode("LitNoProjectInDisposal", language);
                    //Assegno un source vuoto per fare cmq il bind: documents in Policy
                    this.gvDocumentInPolicyDisposal.DataSource = UIManager.ArchiveManager.GetDataSourceVuotoPerGrigliaDocumentiPolicy();
                    this.gvDocumentInPolicyDisposal.DataBind();
                    this.gvDocumentInPolicyDisposal.Rows[0].Visible = false;
                    //Assegno un source vuoto per fare cmq il bind: project in sospensione
                    this.gvProjectInPolicyDisposal.DataSource = UIManager.ArchiveManager.GetDataSourceVuotoPerGrigliaFascicoliPolicy();
                    this.gvProjectInPolicyDisposal.DataBind();
                    this.gvProjectInPolicyDisposal.Rows[0].Visible = false;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
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
            if (!string.IsNullOrEmpty(this.HiddenConfirmAnalisiScarto.Value))
            {
                this.HiddenConfirmAnalisiScarto.Value = string.Empty;
                //CAMBIO STATO ANALIZZA
                int okChangeInAnalizzaScarto = 0;
                UIManager.ArchiveManager.InsertARCHIVE_JOB_Disposal(this.Scarto_system_id, 1, ref okChangeInAnalizzaScarto);
                //QUA ROBBA START RICERCA
                if (okChangeInAnalizzaScarto > 0)
                {
                    this.GetDataByPageState();
                    ManagerBtnByDisposalState();
                    this.UpTranfetButtons.Update();
                    this.upHiddenFieldStatoApprovato.Update();
                    this.upTextBoxDisposalState.Update();
                }

            }
            //Ritorno dallo stato Proposto a In Definizione per l a
            if (!string.IsNullOrEmpty(this.HiddenConfirmBackToDefinition.Value))
            {
                this.HiddenConfirmBackToDefinition.Value = string.Empty;
                //CAMBIO STATO PROPONI
                bool okDeleteState = false;
                List<ARCHIVE_DisposalState> _mytrsf = new List<ARCHIVE_DisposalState>();

                _mytrsf = this.DisposalStateInContext.Where(p => p.DisposalStateType_ID == 2).Cast<DocsPaWR.ARCHIVE_DisposalState>().ToList();

                okDeleteState = UIManager.ArchiveManager.DeleteARCHIVE_DisposalState(_mytrsf[0].System_ID);
                if (okDeleteState)
                {
                    //QUAROBBA!!!!
                    //IL RITORNO AL VECCHIO STATO.

                    this.GetDataByPageState();
                    ManagerBtnByDisposalState();
                    this.UpTranfetButtons.Update();
                    this.upHiddenCameBackToDefinition.Update();
                    this.upTextBoxDisposalState.Update();
                }
            }

            //CAMBIO STATO ESEGUI
            if (!string.IsNullOrEmpty(this.HiddeConfirmEseguiDisposal.Value))
            {
                this.HiddeConfirmEseguiDisposal.Value = string.Empty;
                //CAMBIO STATO PROPONI
                int okinserito = 0;
                UIManager.ArchiveManager.InsertARCHIVE_DisposalState(this.Scarto_system_id,
                                                                                 UIManager.ArchiveManager._dictionaryDisposalState.Where(x => x.Value == "IN ESECUZIONE").Select(x => x.Key).FirstOrDefault(),
                                                                                ref okinserito);

                if (okinserito > 0)
                {
                    this.ResetOnlyDataAreaTextBox();
                    this.GetDataByPageState();
                    ManagerBtnByDisposalState();
                    this.UpTranfetButtons.Update();
                    this.upHiddenFieldStatoInEsecuzione.Update();
                    this.upTextBoxDisposalState.Update();
                }
            }

            //CAMBIO STATO Approvato
            if (!string.IsNullOrEmpty(this.HiddeConfirmApprovaDisposal.Value))
            {
                this.HiddeConfirmApprovaDisposal.Value = string.Empty;
                //CAMBIO STATO PROPONI
                int okinserito = 0;
                UIManager.ArchiveManager.InsertARCHIVE_DisposalState(this.Scarto_system_id,
                                                                                 UIManager.ArchiveManager._dictionaryDisposalState.Where(x => x.Value == "APPROVATO").Select(x => x.Key).FirstOrDefault(),
                                                                                ref okinserito);
                if (okinserito > 0)
                {
                    ResetOnlyDataAreaTextBox();
                    this.GetDataByPageState();
                    ManagerBtnByDisposalState();
                    this.UpTranfetButtons.Update();
                    this.upHiddenFieldStatoApprovato.Update();
                    this.upTextBoxDisposalState.Update();
                }
            }


            //CAMBIO STATO PRPPOSTO
            if (!string.IsNullOrEmpty(this.HiddeConfirmProponiDisposal.Value))
            {
                this.HiddeConfirmProponiDisposal.Value = string.Empty;
                //CAMBIO STATO PROPONI
                int okinserito = 0;
                UIManager.ArchiveManager.InsertARCHIVE_DisposalState(this.Scarto_system_id,
                                                                                 UIManager.ArchiveManager._dictionaryDisposalState.Where(x => x.Value == "PROPOSTO").Select(x => x.Key).FirstOrDefault(),
                                                                                 ref okinserito);
                if (okinserito > 0)
                {
                    ResetOnlyDataAreaTextBox();
                    this.GetDataByPageState();
                    ManagerBtnByDisposalState();
                    this.UpTranfetButtons.Update();
                    this.upHiddenFieldStatoProposto.Update();
                    this.upTextBoxDisposalState.Update();
                }
            }

            //DELETE DISPOSAL
            if (!string.IsNullOrEmpty(this.HiddeConfirmDeleteDisposal.Value))
            {
                this.HiddeConfirmDeleteDisposal.Value = string.Empty;
                //DELETE
                bool okdelete = false;
                okdelete = UIManager.ArchiveManager.DeleteARCHIVE_Disposal(this.Scarto_system_id);
                if (okdelete)
                {
                    Session["PAGESTATE"] = "NEW";
                    this.Scarto_system_id = 0;
                    this.GetDataByPageState();
                    this.ManagerBtnByDisposalState();
                    this.CallVisibilityByPageState();
                    //Update:
                    this.UpHiddenFieldERASE.Update();
                    this.UpTranfetButtons.Update();
                    this.upTextBoxDisposalDetails.Update();
                    this.upTextBoxDisposalState.Update();
                    this.upDisposalDocument.Update();
                    this.upDisposalProject.Update();
                }
            }

            //UPDATE DISPOSAL
            if (!string.IsNullOrEmpty(this.HiddeConfirmUpdateDisposal.Value))
            {
                this.HiddeConfirmUpdateDisposal.Value = string.Empty;
                //UPDATE
                bool okupdateDisposal = false;
                okupdateDisposal = UIManager.ArchiveManager.UpdateARCHIVE_Disposal(TxtDescrizioneScarto.Text.Trim(), TxtNoteScarto.Text.Trim(), this.Scarto_system_id, DisposalInContext[0].ID_Amministrazione);
                Session["PAGESTATE"] = "MOD";
                this.Scarto_system_id = Convert.ToInt32(TxtIdScarto.Text.Trim());
                this.ResetControlPage();
                this.GetDataByPageState();
                ManagerBtnByDisposalState();
                this.UpHiddenFieldUPDATE.Update();
                this.UpTranfetButtons.Update();
                this.upTextBoxDisposalState.Update();
                this.upTextBoxDisposalDetails.Update();
                this.upDisposalDocument.Update();
                this.upDisposalProject.Update();
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
            if (Session["ID_SCARTO"] != null && Session["ID_SCARTO"] != "")
                this.Scarto_system_id = Convert.ToInt32(Session["ID_SCARTO"]);
            this.GetDataByPageState();
            this.CallVisibilityByPageState();
        }

        private bool VerifyPAGESTATE()
        {
            try
            {
                //Get di tutti i record nella tabella DisposalState in stato diverso da completato
                List<DocsPaWR.ARCHIVE_Disposal> _lsttrandf = UIManager.ArchiveManager.GetAllARCHIVE_Disposal();
                List<DocsPaWR.ARCHIVE_DisposalState> _lstxcheck = UIManager.ArchiveManager.GetAllDisposalState();
                var query = (from o in _lsttrandf
                             join p in _lstxcheck
                                  on o.System_ID equals p.Disposal_ID
                             group p by p.Disposal_ID into G
                             select new
                             {
                                 System_id = G.Key,
                                 DisposalStateType_ID = G.Max(s => s.DisposalStateType_ID)
                             });


                ////prendo tutti quelli con stato diverso da completato e in errore
                var DiversiDaCompletato = from p in query.ToList()
                                          where p.DisposalStateType_ID != UIManager.ArchiveManager._dictionaryDisposalState.Where
                                                                          (x => x.Value == "EFFETTUATO").Select(x => x.Key).FirstOrDefault()
                                          select p;

                DiversiDaCompletato = DiversiDaCompletato.Where(x => x.DisposalStateType_ID != UIManager.ArchiveManager._dictionaryDisposalState.Where
                                                                          (y => y.Value == "IN ERRORE").Select(y => y.Key).FirstOrDefault());
                //Se ci sono non si possonbo creare nuovi versamenti.
                if (DiversiDaCompletato.Count() > 0 && DiversiDaCompletato != null)
                {
                    Session["PAGESTATE"] = "MOD";
                    Session["ID_SCARTO"] = DiversiDaCompletato.FirstOrDefault().System_id;

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
            this.LitScartoProject.Text = Utils.Languages.GetLabelFromCode("ScartoTitle", language);
            this.LitScartoId.Text = Utils.Languages.GetLabelFromCode("LitScartoId", language);
            this.LitScartoDescrizione.Text = Utils.Languages.GetLabelFromCode("LitScartoDescrizione", language);
            this.litExpandStatoScarto.Text = Utils.Languages.GetLabelFromCode("litExpandStatoScarto", language);
            this.TxtStatoInDefinizione.Text = Utils.Languages.GetLabelFromCode("TxtStatoInDefinizione", language);
            this.TxtStatoProposto.Text = Utils.Languages.GetLabelFromCode("TxtStatoProposto", language);
            this.TxtStatoApprovato.Text = Utils.Languages.GetLabelFromCode("TxtStatoApprovato", language);
            this.TxtStatoCompletato.Text = Utils.Languages.GetLabelFromCode("TxtStatoCompletato", language);
            this.TxtStatoErrore.Text = Utils.Languages.GetLabelFromCode("TxtStatoErrore", language);
            this.TxtStatoInEsecuzione.Text = Utils.Languages.GetLabelFromCode("TxtStatoInEsecuzione", language);
            this.LitScartoDescrizioneStatoScarto.Text = Utils.Languages.GetLabelFromCode("LitScartoDescrizioneStatoScarto", language);
            this.LitScartoDescrizioneDataStatoScarto.Text = Utils.Languages.GetLabelFromCode("LitScartoDescrizioneDataStatoScarto", language);
            this.btnScartoNuovo.Text = Utils.Languages.GetLabelFromCode("ScartoNuovo", language);
            this.btnScartoAnalizza.Text = Utils.Languages.GetLabelFromCode("ScartoAnalizza", language);
            this.btnScartoProponi.Text = Utils.Languages.GetLabelFromCode("ScartoProponi", language);
            this.btnScartoApprova.Text = Utils.Languages.GetLabelFromCode("ScartoApprova", language);
            this.btnScartoEsegui.Text = Utils.Languages.GetLabelFromCode("ScartoEsegui", language);
            this.btnScartoModifica.Text = Utils.Languages.GetLabelFromCode("ScartoModifica", language);
            this.btnScartoElimina.Text = Utils.Languages.GetLabelFromCode("ScartoElimina", language);
            this.LitScartoNote.Text = Utils.Languages.GetLabelFromCode("LitScartoNote", language);
            this.TxtStatoAnalisiCompletata.Text = Utils.Languages.GetLabelFromCode("TxtStatoAnalisiCompletataScarto", language);
            this.BtnDisposalReport.Text = Utils.Languages.GetLabelFromCode("btnScartoReport", language);
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
            TxtIdScarto.Enabled = false;
            switch (state)
            {
                //Page state new
                case (PageState.NEW):
                    //Resetto una possibile variabile sporca:
                    Session["ID_SCARTO"] = null;
                    Scarto_system_id = 0;
                    //Pulsanti
                    btnScartoAnalizza.Enabled = false;
                    btnScartoProponi.Enabled = false;
                    btnScartoApprova.Enabled = false;
                    btnScartoEsegui.Enabled = false;
                    btnScartoModifica.Enabled = false;
                    btnScartoElimina.Enabled = false;
                    btnScartoNuovo.Enabled = true;
                    //TextBox
                    TxtDescrizioneScarto.Enabled = true;
                    TxtNoteScarto.Enabled = true;
                    break;

                //Page state Modifiy
                case (PageState.MOD):
                    //Pulsanti
                    ManagerBtnByDisposalState();
                    //btnCreaPolicy.Enabled = true;
                    btnScartoNuovo.Enabled = false;
                    //Textbox
                    TxtDescrizioneScarto.Enabled = true;
                    TxtNoteScarto.Enabled = true;
                    break;
                case (PageState.SEA):
                    //Resetto una possibile variabile sporca:
                    //Pulsanti
                    btnScartoAnalizza.Enabled = false;
                    btnScartoProponi.Enabled = false;
                    btnScartoApprova.Enabled = false;
                    btnScartoEsegui.Enabled = false;
                    btnScartoModifica.Enabled = false;
                    btnScartoElimina.Enabled = false;
                    btnScartoNuovo.Enabled = false;
                    //TextBox
                    TxtDescrizioneScarto.Enabled = false;
                    TxtNoteScarto.Enabled = false;
                    break;

            }
        }

        /// <summary>
        /// Gestore dei pulsanti in base allo stato del Scarto.
        /// </summary>
        private void ManagerBtnByDisposalState()
        {
            if (this.DisposalStateInContext.Count > 0)
            {
                switch (this.DisposalStateInContext.Max(p => p.DisposalStateType_ID))
                {
                    //DEVO SOSTUIRE IL CODICE CON LA GET DAL DICTIONARY!!!!
                    //PER ORA METTO gli stati fissi:

                    //1	IN DEFINIZIONE
                    case 1:
                        btnScartoNuovo.Enabled = false;
                        btnScartoAnalizza.Enabled = true;
                        btnScartoProponi.Enabled = false;
                        btnScartoApprova.Enabled = false;
                        btnScartoEsegui.Enabled = false;
                        btnScartoModifica.Enabled = true;
                        btnScartoElimina.Enabled = true;
                        break;
                    //7 ANALISI COMPLETATA
                    case 2:
                        btnScartoNuovo.Enabled = false;
                        btnScartoAnalizza.Enabled = false;
                        btnScartoProponi.Enabled = true;
                        btnScartoApprova.Enabled = false;
                        btnScartoEsegui.Enabled = false;
                        btnScartoModifica.Enabled = true;
                        btnScartoElimina.Enabled = true;
                        break;
                    //3	PROPOSTO
                    case 3:
                        btnScartoNuovo.Enabled = false;
                        btnScartoAnalizza.Enabled = false;
                        btnScartoProponi.Enabled = false;
                        btnScartoApprova.Enabled = true;
                        btnScartoEsegui.Enabled = false;
                        btnScartoModifica.Enabled = false;
                        btnScartoElimina.Enabled = true;
                        break;
                    //4	APPROVATO
                    case 4:
                        btnScartoNuovo.Enabled = false;
                        btnScartoAnalizza.Enabled = false;
                        btnScartoProponi.Enabled = false;
                        btnScartoApprova.Enabled = false;
                        btnScartoEsegui.Enabled = true;
                        btnScartoModifica.Enabled = false;
                        btnScartoElimina.Enabled = true;
                        break;
                    //5	IN ESECUZIONE
                    case 5:
                        btnScartoNuovo.Enabled = false;
                        btnScartoAnalizza.Enabled = false;
                        btnScartoProponi.Enabled = false;
                        btnScartoApprova.Enabled = false;
                        btnScartoEsegui.Enabled = false;
                        btnScartoModifica.Enabled = false;
                        btnScartoElimina.Enabled = false;
                        break;
                    //6	EFFETTUATO
                    case 6:
                        btnScartoNuovo.Enabled = false;
                        btnScartoAnalizza.Enabled = false;
                        btnScartoProponi.Enabled = false;
                        btnScartoApprova.Enabled = false;
                        btnScartoEsegui.Enabled = false;
                        btnScartoModifica.Enabled = false;
                        btnScartoElimina.Enabled = false;
                        break;
                    //7	IN ERRORE
                    case 7:
                        btnScartoNuovo.Enabled = false;
                        btnScartoAnalizza.Enabled = false;
                        btnScartoProponi.Enabled = false;
                        btnScartoApprova.Enabled = false;
                        btnScartoEsegui.Enabled = false;
                        btnScartoModifica.Enabled = false;
                        btnScartoElimina.Enabled = false;
                        break;

                }
            }
        }

        /// <summary>
        /// Get all date by Disposal systemId 
        /// </summary>
        private void GetDataByPageState()
        {
            
            if (this.Scarto_system_id != 0)
            {
                //  this.ResetInContext();
                //Devo dividere gli oggetti utili per l'update:
                //ORIGINAL:
                this.DisposalOriginalInContext = UIManager.ArchiveManager.GetARCHIVE_DisposalBySystem_ID(this.Scarto_system_id);
                this.DisposalStateOriginalInContext = UIManager.ArchiveManager.GetARCHIVE_DisposalStateByDisposal_ID(this.Scarto_system_id);
                this.DisposalStateOriginalTypeInContext = UIManager.ArchiveManager.GetAllARCHIVE_DisposalStateType();
                //FINE!
                //Devo dividere gli oggetti utili per l'update:
                //CORRENTE:
                this.DisposalInContext = UIManager.ArchiveManager.GetARCHIVE_DisposalBySystem_ID(this.Scarto_system_id);
                this.DisposalStateInContext = UIManager.ArchiveManager.GetARCHIVE_DisposalStateByDisposal_ID(this.Scarto_system_id);
                this.DisposalStateTypeInContext = UIManager.ArchiveManager.GetAllARCHIVE_DisposalStateType();

                //QUI LE VISTE CON I DATI DELLO SCARTO
                this.DisposalPolicyViewDocumentsPolicyInContext = UIManager.ArchiveManager.GetAllARCHIVE_View_Documents_Disposal(this.Scarto_system_id);
                //Valorizzo la vista dei fascicoli associati alla Policy:
                this.DisposalPolicyViewProjectsPolicyInContext = UIManager.ArchiveManager.GetAllARCHIVE_View_Projects_Disposal(this.Scarto_system_id);

                //FINE!
                //TextBox
                TxtIdScarto.Text = DisposalInContext[0].System_ID.ToString();
                TxtDescrizioneScarto.Text = DisposalInContext[0].Description;
                TxtNoteScarto.Text = DisposalInContext[0].Note;

                TxtDATAStatoInDefinizione.Text = String.Join("", from p in DisposalStateInContext
                                                                 where p.DisposalStateType_ID == UIManager.ArchiveManager._dictionaryDisposalState.Where
                                                                                                 (x => x.Value == "IN DEFINIZIONE").Select(x => x.Key).FirstOrDefault()
                                                                 select p.DateTime.ToShortDateString());
                TxtDATAStatoAnalisiCompletata.Text = String.Join("", from p in DisposalStateInContext
                                                                     where p.DisposalStateType_ID == UIManager.ArchiveManager._dictionaryDisposalState.Where
                                                                                                     (x => x.Value == "RICERCA COMPLETATA").Select(x => x.Key).FirstOrDefault()
                                                                     select p.DateTime.ToShortDateString());
                TxtDATAStatoProposto.Text = String.Join("", from p in DisposalStateInContext
                                                            where p.DisposalStateType_ID == UIManager.ArchiveManager._dictionaryDisposalState.Where
                                                                                            (x => x.Value == "PROPOSTO").Select(x => x.Key).FirstOrDefault()
                                                            select p.DateTime.ToShortDateString());
                TxtDATAStatoApprovato.Text = String.Join("", from p in DisposalStateInContext
                                                             where p.DisposalStateType_ID == UIManager.ArchiveManager._dictionaryDisposalState.Where
                                                                                             (x => x.Value == "APPROVATO").Select(x => x.Key).FirstOrDefault()
                                                             select p.DateTime.ToShortDateString());
                TxtDATAStatoInEsecuzione.Text = String.Join("", from p in DisposalStateInContext
                                                                where p.DisposalStateType_ID == UIManager.ArchiveManager._dictionaryDisposalState.Where
                                                                                                (x => x.Value == "IN ESECUZIONE").Select(x => x.Key).FirstOrDefault()
                                                                select p.DateTime.ToShortDateString());
                TxtDATAStatoCompletato.Text = String.Join("", from p in DisposalStateInContext
                                                              where p.DisposalStateType_ID == UIManager.ArchiveManager._dictionaryDisposalState.Where
                                                                                              (x => x.Value == "EFFETTUATO").Select(x => x.Key).FirstOrDefault()
                                                              select p.DateTime.ToShortDateString());
                TxtDATAStatoErrore.Text = String.Join("", from p in DisposalStateInContext
                                                          where p.DisposalStateType_ID == UIManager.ArchiveManager._dictionaryDisposalState.Where
                                                                                          (x => x.Value == "IN ERRORE").Select(x => x.Key).FirstOrDefault()
                                                          select p.DateTime.ToShortDateString());

                
            }
            else
            {
                //Resetto tutte le TextBox.
                ResetControlPage();
            }
            //Pulsante errore e Report devono essere disabilitati anche se non c'è nessuno scarto in gestione
            cImgBtnReportErrore.Enabled = GetEnabledBaseTextBox(TxtDATAStatoErrore.Text);
            BtnDisposalReport.Enabled = this.GetEnabledByDisposalState();

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

        /// <summary>
        /// Utility creata per abilitare il report dello scarto in funzione del suo stato
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool GetEnabledByDisposalState()
        {
            if (DisposalStateInContext != null)
            {
                switch (this.DisposalStateInContext.Max(p => p.DisposalStateType_ID))
                {
                    //IN DEFINIZIONE
                    case 1:
                        return false;
                    //RICERCA COMPLETATA
                    case 2:
                        return true;
                    //PROPOSTO
                    case 3:
                        return true;
                    //APPROVATO
                    case 4:
                        return true;
                    //IN ESECUZIONE
                    case 5:
                        return true;
                    //EFFETTUATO
                    case 6:
                        return false;
                    //ERRORE
                    case 7:
                        return false;
                }
            }
            return false;
        }

        public bool GetEnabledBaseTextBox(string text)
        {
            if (this.TxtDATAStatoErrore.Text != string.Empty)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Scarto Report
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnScartoReport_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Scarto Nuovo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnScartoNuovo_Click(object sender, EventArgs e)
        {
            //Controllo Campi
            if (string.IsNullOrEmpty(TxtDescrizioneScarto.Text))
            {
                string msgErrNewArchiveDisposal = "msgErrNewArchiveDisposal";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgErrNewArchiveDisposal.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgErrNewArchiveDisposal.Replace("'", @"\'") + "', 'error', '');}", true);
                return;
            }

            //Devo prima controllare che nessun altro Scarto sia in qualche stato fuorchè quello di Completato.
            DocsPaWR.ARCHIVE_DisposalState _clsapp = new ARCHIVE_DisposalState();
            Dictionary<int, string> _mydict = UIManager.ArchiveManager._dictionaryDisposalState;


            //Get di tutti i record nella tabella DisposalState in stato diverso da completato
            List<DocsPaWR.ARCHIVE_Disposal> _lsttrandf = UIManager.ArchiveManager.GetAllARCHIVE_Disposal();
            List<DocsPaWR.ARCHIVE_DisposalState> _lstxcheck = UIManager.ArchiveManager.GetAllDisposalState();
            var query = (from o in _lsttrandf
                         join p in _lstxcheck
                              on o.System_ID equals p.Disposal_ID
                         group p by p.Disposal_ID into G
                         select new
                         {
                             System_id = G.Key,
                             DisposalStateType_ID = G.Max(s => s.DisposalStateType_ID)
                         });


            ////prendo tutti quelli con stato diverso da completato
            var DiversiDaCompletato = from p in query.ToList()
                                      where p.DisposalStateType_ID != UIManager.ArchiveManager._dictionaryDisposalState.Where
                                                                      (x => x.Value == "EFFETTUATO").Select(x => x.Key).FirstOrDefault()
                                      select p;

            DiversiDaCompletato = DiversiDaCompletato.Where(x => x.DisposalStateType_ID != UIManager.ArchiveManager._dictionaryDisposalState.Where
                                                                       (y => y.Value == "IN ERRORE").Select(y => y.Key).FirstOrDefault());

            //Se ci sono non si possonbo creare nuovi versamenti.
            if (DiversiDaCompletato.Count() > 0 && DiversiDaCompletato != null)
            {
                string msgErrNewArchiveDisposal = "msgErrNewArchiveDisposalStati";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgErrNewArchiveDisposal.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgErrNewArchiveDisposal.Replace("'", @"\'") + "', 'error', '');}", true);
                TxtDescrizioneScarto.Text = string.Empty;
                TxtNoteScarto.Text = string.Empty;
            }
            else
            {
                //Chiamo la ws per la get dell'id appena creato
                int system_idNewDisposalState = 0;
                UIManager.ArchiveManager.InsertARCHIVE_Disposal(TxtDescrizioneScarto.Text.Trim(),
                                                                TxtNoteScarto.Text.Trim(),
                                                                ref system_idNewDisposalState,
                                                                Convert.ToInt32(UserManager.GetInfoUser().idAmministrazione),
                                                                UIManager.ArchiveManager._dictionaryDisposalState.Where
                                                                (x => x.Value == "IN DEFINIZIONE").Select(x => x.Key).FirstOrDefault()
                                                                );
                if (system_idNewDisposalState > 0)
                {
                    Session["PAGESTATE"] = "MOD";
                    this.Scarto_system_id = system_idNewDisposalState;
                    this.GetDataByPageState();
                    ManagerBtnByDisposalState();
                    this.upTextBoxDisposalDetails.Update();
                    this.upTextBoxDisposalState.Update();
                    this.UpTranfetButtons.Update();
                }
            }

        }

        /// <summary>
        /// Scarto Analizza (lancio analisi impatto)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnScartoAnalizza_Click(object sender, EventArgs e)
        {
            if (DisposalStateInContext[0].DisposalStateType_ID != 0)
            {
                List<ARCHIVE_JOB_Disposal> _lstbj = UIManager.ArchiveManager.GetARCHIVE_JOB_DisposalByDisposal_ID(this.Scarto_system_id).Where(x => x.Executed != 1).ToList();
                if (_lstbj.Count > 0)
                {
                    string msgResultRICERCA = "msgResultRICERCA";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + msgResultRICERCA.Replace("'", @"\'") + "', 'warning', '');", true);
                    this.UpTranfetButtons.Update();
                    this.upTextBoxDisposalState.Update();
                    this.upTextBoxDisposalDetails.Update();
                    this.upDisposalDocument.Update();
                    this.upDisposalProject.Update();
                }
                else
                {
                    string msgConfirm = "btnScartoAnalizza_Click";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "if (parent.fra_main) {parent.fra_main.ajaxConfirmModal('" + msgConfirm.Replace("'", @"\'") + "', 'HiddenConfirmAnalisiScarto', '');} else {parent.ajaxConfirmModal('" + msgConfirm.Replace("'", @"\'") + "', 'HiddenConfirmAnalisiScarto', '');}", true);
                    this.upHiddenFieldStatoApprovato.Update();
                }
            }
        }

        /// <summary>
        /// Passa il Scarto in stato proponi
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnScartoProponi_Click(object sender, EventArgs e)
        {
            if (DisposalStateInContext[0].DisposalStateType_ID != 0)
            {
                string msgConfirm = "btnScartoProponi_Click";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "if (parent.fra_main) {parent.fra_main.ajaxConfirmModal('" + msgConfirm.Replace("'", @"\'") + "', 'HiddeConfirmProponiDisposal', '');} else {parent.ajaxConfirmModal('" + msgConfirm.Replace("'", @"\'") + "', 'HiddeConfirmProponiDisposal', '');}", true);
                this.upHiddenFieldStatoProposto.Update();
            }
        }

        /// <summary>
        /// Passa il Scarto in stato approva
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnScartoApprova_Click(object sender, EventArgs e)
        {
            if (DisposalStateInContext[0].DisposalStateType_ID != 0)
            {
                string msgConfirm = "btnScartoApprova_Click";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "if (parent.fra_main) {parent.fra_main.ajaxConfirmModal('" + msgConfirm.Replace("'", @"\'") + "', 'HiddeConfirmApprovaDisposal', '');} else {parent.ajaxConfirmModal('" + msgConfirm.Replace("'", @"\'") + "', 'HiddeConfirmApprovaDisposal', '');}", true);
                this.upHiddenFieldStatoApprovato.Update();
            }
        }

        /// <summary>
        /// Passa il Scarto in stato esegui
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnScartoEsegui_Click(object sender, EventArgs e)
        {
            if (DisposalStateInContext[0].DisposalStateType_ID != 0)
            {
                string msgConfirm = "btnScartoEsegui_Click";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "if (parent.fra_main) {parent.fra_main.ajaxConfirmModal('" + msgConfirm.Replace("'", @"\'") + "', 'HiddeConfirmEseguiDisposal', '');} else {parent.ajaxConfirmModal('" + msgConfirm.Replace("'", @"\'") + "', 'HiddeConfirmEseguiDisposal', '');}", true);
                this.upHiddenFieldStatoInEsecuzione.Update();
            }
        }

        /// <summary>
        /// modifica il Scarto
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnScartoModifica_Click(object sender, EventArgs e)
        {
            if (DisposalStateInContext[0].DisposalStateType_ID != 0)
            {
                string msgConfirm = "btnScartoModifica_Click";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "if (parent.fra_main) {parent.fra_main.ajaxConfirmModal('" + msgConfirm.Replace("'", @"\'") + "', 'HiddeConfirmUpdateDisposal', '');} else {parent.ajaxConfirmModal('" + msgConfirm.Replace("'", @"\'") + "', 'HiddeConfirmUpdateDisposal', '');}", true);
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
            this.Scarto_system_id = Convert.ToInt32(Session["Scarto_system_id"]);
            this.GetDataByPageState();
            ManagerBtnByDisposalState();
            this.upDisposalDocument.Update();
            this.UpTranfetButtons.Update();
            this.upTextBoxDisposalState.Update();
        }

        protected void cImgBtnDisposalReport_Click(object sender, EventArgs e)
        {
            try
            {
                HttpContext.Current.Session["CurrentDisposal_ID"] = this.Scarto_system_id;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "DisposalReport", "ajaxModalPopupDisposalReport();", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }

        }

        /// <summary>
        /// Pulsante per il postback del popup per i report dello scarto.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDisposalReportPostback_click(object sender, EventArgs e)
        {
            HttpContext.Current.Session["CurrentDisposal_ID"] = null;

        }

        /// <summary>
        /// Pulsante per il postback del popup del Log.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDisposalErrorLogPostback_click(object sender, EventArgs e)
        {
            //è inutile fare update
            this.upTextBoxDisposalState.Update();
        }

        /// <summary>
        /// elimina il Scarto
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnScartoElimina_Click(object sender, EventArgs e)
        {
            //Va in cascade!
            //Daje!
            if (DisposalStateInContext[0].DisposalStateType_ID != 0)
            {
                string msgConfirm = "msgScartoInfo_Click";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "if (parent.fra_main) {parent.fra_main.ajaxConfirmModal('" + msgConfirm.Replace("'", @"\'") + "', 'HiddeConfirmDeleteDisposal', '');} else {parent.ajaxConfirmModal('" + msgConfirm.Replace("'", @"\'") + "', 'HiddeConfirmDeleteDisposal', '');}", true);
                this.UpHiddenFieldERASE.Update();
            }
        }

        protected void cImgBtnReportErrore_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "DisposalErrorLog", "ajaxModalPopupDisposalErrorLog();", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        public void gvDocumentInPolicyDisposalPageIndexChanging_click(object sender, GridViewPageEventArgs e)
        {
            this.gvDocumentInPolicyDisposal.DataSource = this.DisposalPolicyViewDocumentsPolicyInContext.ToList();
            this.gvDocumentInPolicyDisposal.DataBind();
            this.gvDocumentInPolicyDisposal.PageIndex = e.NewPageIndex;
            this.gvDocumentInPolicyDisposal.DataBind();
            this.upDisposalDocument.Update();
        }

        protected void gvProjectInPolicyDisposalPageIndexChanging_click(object sender, GridViewPageEventArgs e)
        {
            this.gvProjectInPolicyDisposal.DataSource = this.DisposalPolicyViewProjectsPolicyInContext.ToList();
            this.gvProjectInPolicyDisposal.DataBind();
            this.gvProjectInPolicyDisposal.PageIndex = e.NewPageIndex;
            this.gvProjectInPolicyDisposal.DataBind();
            this.upDisposalProject.Update();
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
