using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using DocsPaWR = DocsPAWA.DocsPaWR;

namespace DocsPAWA.AdminTool.Gestione_ChiaviConfig
{
    public partial class ChiaviConfig : System.Web.UI.Page
    {
        /// Costanti che identificano i nomi delle colonne del dataset 
        /// utilizzato per caricare i dati nel datagrid
        /// </summary>
        private const string TABLE_COL_ID = "ID";
        private const string TABLE_COL_ID_AMMINISTRAZIONE = "ID_AMMINISTRAZIONE";
        private const string TABLE_COL_CODICE = "Codice";
        private const string TABLE_COL_DESCRIZIONE = "Descrizione";
        private const string TABLE_COL_VALORE = "Valore";
        private const string TABLE_COL_TIPO = "Tipo";

        /// <summary>
        /// Costanti che identificano i nomi delle colonne del datagrid
        /// </summary>
        private const int GRID_COL_ID = 0;
        private const int GRID_COL_AMMINISTRAZIONE = 1;
        private const int GRID_COL_CODICE = 2;
        private const int GRID_COL_DESCRIZIONE = 3;
        private const int GRID_COL_VALORE = 4;
        private const int GRID_COL_TIPO = 5;
        private const int GRID_COL_DETAIL = 6;
        private const int GRID_COL_DELETE = 7; //non usata perchè le chiavi di configurazione non dovrebbero essere eliminate!!!


        #region Form_Load
        private void Page_Load(object sender, System.EventArgs e)
        {
            Session["AdminBookmark"] = "GestioneChiavi";

            //----- CONTROLLO DELL'UTENTE AMMINISTRATORE CONNESSO -----------
            if (Session.IsNewSession)
            {
                Response.Redirect("../Exit.aspx?FROM=EXPIRED");
            }

            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            if (!ws.CheckSession(Session.SessionID))
            {
                Response.Redirect("../Exit.aspx?FROM=ABORT");
            }
            // ---------------------------------------------------------------

            // Inizializzazione hashtable businessrules
            this.InitializeBusinessRuleControls();

            if (!IsPostBack)
            {
                lbl_position.Text = "&nbsp;&bull;&nbsp;Amministrazione: " + AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "1");

                // Caricamento lista chiavi
                this.FillListChiaviConfig();
            }
        }
        #endregion

        #region Datagrid

        private DataView OrdinaGrid(DataView dv, string sortColumn)
        {
            string sortMode = " ASC";
            dv.Sort = sortColumn + sortMode;
            return dv;
        }

        private void dg_chiavi_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            pnl_info.Visible = false;

            ViewState["riga"] = e.Item.DataSetIndex;

            switch (e.CommandName)
            {
                case "Select":
                    pnl_info.Visible = true;
                    lbl_cod.Visible = true;
                    txt_codice.Visible = false;
                    btn_salva.Text = "Modifica";

                    SetFocus(txt_descrizione);
                    //recupera le informazioni sulla chiave selezionata per farne vedere il dettaglio
                    DocsPaWR.ChiaveConfigurazione chiave = this.GetChiaveConfigurazione(e.Item.Cells[GRID_COL_CODICE].Text, e.Item.Cells[GRID_COL_AMMINISTRAZIONE].Text);
                    this.BindUI(chiave);

                    break;
            }
        }

        private void dg_chiavi_ItemCreated(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
        {
            //la cancellazione è al momento disabilitata
            e.Item.Cells[GRID_COL_DELETE].Attributes.Add("onclick", "if (!window.confirm('Sei sicuro di voler eliminare questa chiave?')) {return false};");
        }

        protected void dg_chiavi_SelectedIndexChanged(object sender, EventArgs e)
        {
        }


        #endregion

        private void SetFocus(System.Web.UI.Control ctrl)
        {
            string s = "<SCRIPT language='javascript'>document.getElementById('" + ctrl.ID + "').focus() </SCRIPT>";
            RegisterStartupScript("focus", s);
        }

        #region Gestione accesso ai dati

        /// <summary>
        /// Reperimento idChiave  corrente
        /// </summary>
        private int CurrentIDChiaveConfig
        {
            get
            {
                if (this.ViewState["CurrentIDChiaveConfig"] != null)
                    return Convert.ToInt32(this.ViewState["CurrentIDChiaveConfig"]);
                else
                    return 0;
            }
            set
            {
                this.ViewState["CurrentIDChiaveConfig"] = value;
            }
        }

        /// <summary>
        /// Reperimento delle chiavi nell'amministrazione corrente
        /// </summary>
        /// <returns></returns>
        private DocsPaWR.ChiaveConfigurazione[] GetChiaviConfigurazione()
        {

            DocsPaWR.ChiaveConfigurazione[] l_chiaviConfig_globali;
            DocsPaWR.ChiaveConfigurazione[] l_chiaviConfig_ammin;
            //si prendono le chiavi globali idAmm=0;
            l_chiaviConfig_globali = this.GetChiaviConfigurazione("0");

            //si aggiungono le chiavi legate all'amministrazione, se ci sono
            string idAmministrazione = this.GetIdAmministrazione();
            //si potrebbe verificare quando non esistono ancora amministrazioni
            if (idAmministrazione == null)
                return l_chiaviConfig_globali;
            l_chiaviConfig_ammin = this.GetChiaviConfigurazione(idAmministrazione);

            //se non ci sono le chiavi globali
            if (l_chiaviConfig_globali == null || l_chiaviConfig_globali.Length < 1)
                return l_chiaviConfig_ammin;
            //se non ci sono le chiavi di amministrazione
            if (l_chiaviConfig_ammin.Length == null || l_chiaviConfig_ammin.Length < 1)
                return l_chiaviConfig_globali;

            //sommo i due risultati
            if (l_chiaviConfig_ammin != null && l_chiaviConfig_ammin.Length > 0)
            {
                DocsPaWR.ChiaveConfigurazione[] listaChiavi = new DocsPAWA.DocsPaWR.ChiaveConfigurazione[l_chiaviConfig_globali.Length + l_chiaviConfig_ammin.Length];
                for (int i = 0; i < l_chiaviConfig_globali.Length; i++)
                    listaChiavi[i] = l_chiaviConfig_globali[i];
                for (int j = 0; j < l_chiaviConfig_ammin.Length; j++)
                    listaChiavi[j + l_chiaviConfig_globali.Length] = l_chiaviConfig_ammin[j];
                return listaChiavi;
            }

            //negli altri casi fccio ritornare almeno le chiavi globali
            return l_chiaviConfig_globali;
        }

        /// <summary>
        /// Reperimento delle chiavi di configurazione
        /// </summary>
        /// <param name="codiceAmministrazione"></param>
        /// <returns></returns>
        private DocsPaWR.ChiaveConfigurazione[] GetChiaviConfigurazione(string idAmministrazione)
        {
            //prende le chiavi relative all'amministrazione indicata e quelle globali
            DocsPAWA.utils.ConfigRepository icC = utils.InitConfigurationKeys.getInstance(idAmministrazione);
            return icC.ListaChiavi;
        }

        /// <summary>
        /// Reperimento di una chiave
        /// </summary>
        /// <param name="idTipoRuolo"></param>
        /// <returns></returns>
        private DocsPAWA.DocsPaWR.ChiaveConfigurazione GetChiaveConfigurazione(string idChiaveConfig, string idAmministrazione)
        {
            utils.ConfigRepository cR = utils.InitConfigurationKeys.getInstance(idAmministrazione);
            return (DocsPAWA.DocsPaWR.ChiaveConfigurazione)cR[idChiaveConfig];
        }



        /// <summary>
        /// Aggiornamento di una chiave
        /// </summary>
        /// <param name="tipoRuolo"></param>
        /// <returns></returns>
        private DocsPAWA.DocsPaWR.ValidationResultInfo UpdateChiaveConfig(ref DocsPAWA.DocsPaWR.ChiaveConfigurazione chiaveConfig)
        {
            //Undo Modifiche Lorusso 22-10-2012
            ////DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            AmmUtils.WebServiceLink wsl = new AmmUtils.WebServiceLink();
            return wsl.updateChiaveConfig(chiaveConfig);
            //End Undo
            //DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            //return ws.updateChiaveConfig(chiaveConfig);
        }

        private void Clear(string idAmm)
        {
            DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            ws.clearHashTableChiaviConfig(idAmm);
            utils.InitConfigurationKeys.remove(idAmm);
        }

        /// <summary>
        /// Associazione dati della chiave ai campi della UI
        /// </summary>
        /// <param name="chiaveConfig"></param>
        private void BindUI(DocsPAWA.DocsPaWR.ChiaveConfigurazione chiaveConfig)
        {
            this.ClearData();

            this.CurrentIDChiaveConfig = Convert.ToInt32(chiaveConfig.IDChiave);
            this.txt_codice.Text = chiaveConfig.Codice;
            this.lbl_cod.Text = this.txt_codice.Text;
            this.txt_descrizione.Text = chiaveConfig.Descrizione;
            this.txt_valore.Text = chiaveConfig.Valore;
            this.lbl_idAmm.Text = chiaveConfig.IDAmministrazione;
            if (chiaveConfig.IsGlobale.Equals("1"))
                this.lbl_tipo_chiave.Text = "GLOBALE";
            else
                this.lbl_tipo_chiave.Text = "AMM: " + this.GetCodiceAmministrazione();
            if (chiaveConfig.Modificabile.Equals("1"))
            {
                this.txt_valore.ReadOnly = false;
                this.btn_salva.Enabled = true;
            }
            else
            {
                this.txt_valore.ReadOnly = true;
                this.btn_salva.Enabled = false;
            }
        }

        /// <summary>
        /// Aggiornamento della chiave dai dati dei campi della UI
        /// </summary>
        private void RefreshChiaveConfigFromUI(DocsPAWA.DocsPaWR.ChiaveConfigurazione chiaveConfig)
        {
            chiaveConfig.IDChiave = this.CurrentIDChiaveConfig.ToString();
            chiaveConfig.Codice = this.txt_codice.Text.Trim();
            chiaveConfig.Descrizione = this.txt_descrizione.Text.Trim();
            chiaveConfig.Valore = this.txt_valore.Text.Trim();
            chiaveConfig.IDAmministrazione = this.lbl_idAmm.Text;
        }

        /// <summary>
        /// Rimozione dati UI
        /// </summary>
        private void ClearData()
        {
            this.CurrentIDChiaveConfig = 0;
            this.txt_codice.Text = string.Empty;
            this.lbl_cod.Text = string.Empty;
            this.txt_descrizione.Text = string.Empty;
            this.txt_valore.Text = string.Empty;
            this.lbl_tipo_chiave.Text = string.Empty;
        }

        /// <summary>
        /// Reperimento codice amministrazione corrente
        /// </summary>
        /// <returns></returns>
        private string GetCodiceAmministrazione()
        {
            return AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "0");
        }

        private string GetIdAmministrazione()
        {
            return AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
        }

        /// <summary>
        /// Impostazione dell'id dell'amministrazione (a cosa serve?)
        /// </summary>
        /// <param name="tipoRuolo"></param>
        private void SetIdAmministrazione(DocsPAWA.DocsPaWR.ChiaveConfigurazione chiaveConfig)
        {
            Amministrazione.Manager.OrganigrammaManager orgManager = new Amministrazione.Manager.OrganigrammaManager();
            orgManager.CurrentIDAmm(this.GetCodiceAmministrazione());
        }

        /// <summary>
        /// Salvataggio dati della chiave
        /// </summary>
        private void Save()
        {
            DocsPaWR.ChiaveConfigurazione chiaveConfig = new DocsPAWA.DocsPaWR.ChiaveConfigurazione();
            this.RefreshChiaveConfigFromUI(chiaveConfig);

            DocsPaWR.ValidationResultInfo result = null;

            result = this.UpdateChiaveConfig(ref chiaveConfig);

            if (!result.Value)
            {
                this.ShowValidationMessage(result);
            }
            else
            {
                // Inserimento
                this.lbl_tit.Visible = true;

                // Pulizia della cache
                //pulizia della cache sul backend e sul frontend
                this.Clear(chiaveConfig.IDAmministrazione);
                //ricalcolo delle chiavi
                this.FillListChiaviConfig();
            }
        }

        private string GetValidationMessage(DocsPAWA.DocsPaWR.ValidationResultInfo validationResult,
                                            out Control firstInvalidControl,
                                            out bool warningMessage)
        {
            string retValue = string.Empty;
            bool errorMessage = false;
            firstInvalidControl = null;

            foreach (DocsPAWA.DocsPaWR.BrokenRule rule in validationResult.BrokenRules)
            {
                if (!errorMessage && rule.Level == DocsPaWR.BrokenRuleLevelEnum.Error)
                    errorMessage = true;

                if (retValue != string.Empty)
                    retValue += "\\n";

                retValue += " - " + rule.Description;

                if (firstInvalidControl == null)
                    firstInvalidControl = this.GetBusinessRuleControl(rule.ID);
            }

            if (errorMessage)
                retValue = "Sono state riscontrate le seguenti anomalie:\\n\\n" + retValue;
            else
                retValue = "Attenzione:\\n\\n" + retValue;

            warningMessage = !errorMessage;

            return retValue.Replace("'", "\\'");
        }

        /// <summary>
        /// Hashtable businessrules
        /// </summary>
        private Hashtable _businessRuleControls = null;

        /// <summary>
        /// Inizializzazione hashtable per le businessrule:
        /// - Key:		ID della regola di business
        /// - Value:	Controllo della UI contenente il 
        ///				dato in conflitto con la regola di business
        /// </summary>
        private void InitializeBusinessRuleControls()
        {
            this._businessRuleControls = new Hashtable();

            this._businessRuleControls.Add("CODICE_CHIAVE", this.txt_codice);
            this._businessRuleControls.Add("DESCRIZIONE_CHIAVE", this.txt_descrizione);
            this._businessRuleControls.Add("VALORE_CHIAVE", this.txt_valore);
        }

        private Control GetBusinessRuleControl(string idBusinessRule)
        {
            return this._businessRuleControls[idBusinessRule] as Control;
        }

        /// <summary>
        /// Visualizzazione messaggi di validazione
        /// </summary>
        /// <param name="validationResult"></param>
        private void ShowValidationMessage(DocsPAWA.DocsPaWR.ValidationResultInfo validationResult)
        {
            // Visualizzazione delle regole di business non valide
            bool warningMessage;
            Control firstInvalidControl;

            string validationMessage = this.GetValidationMessage(validationResult, out firstInvalidControl, out warningMessage);

            this.RegisterClientScript("ShowValidationMessage", "ShowValidationMessage('" + validationMessage + "'," + warningMessage.ToString().ToLower() + ");");

            if (firstInvalidControl != null)
                this.SetFocus(firstInvalidControl);
        }

        /// <summary>
        /// Verifica se si è in fase di inserimento
        /// </summary>
        /// <returns></returns>
        /// NB: per ora non è utilizzato perchè si è deciso che non si possono aggiungere chiavi con il tool di amministrazione
        private bool OnInsertMode()
        {
            return (btn_salva.Text.Equals("Aggiungi"));
        }

        /// <summary>
        /// Gestione caricamento lista chiavi
        /// </summary>
        private void FillListChiaviConfig()
        {
            DocsPaWR.ChiaveConfigurazione[] chiaviConfig = this.GetChiaviConfigurazione();
            DataSet ds = this.ConvertToDataSet(chiaviConfig);

            DataView dv = ds.Tables["dsChiaviConfig"].DefaultView;
            dv.Sort = TABLE_COL_CODICE + " ASC";

            this.dg_chiavi.DataSource = dv;
            this.dg_chiavi.DataBind();

            ds.Dispose();
            ds = null;

            chiaviConfig = null;
        }

        /// <summary>
        /// Aggiornamento elemento griglia corrente
        /// </summary>
        /// <param name="registro"></param>
        private void RefreshGridItem(DocsPAWA.DocsPaWR.ChiaveConfigurazione chiaveConfig)
        {
            DataGridItem item = this.dg_chiavi.SelectedItem;
            if (item != null)
            {
                item.Cells[GRID_COL_DESCRIZIONE].Text = chiaveConfig.Descrizione;
                item.Cells[GRID_COL_VALORE].Text = chiaveConfig.Valore;
            }
        }

        /// <summary>
        /// Conversione array
        /// </summary>
        /// <param name="registri"></param>
        /// <returns></returns>
        private DataSet ConvertToDataSet(DocsPaWR.ChiaveConfigurazione[] chiaviConfig)
        {
            DataSet ds = this.CreateGridDataSet();
            DataTable dt = ds.Tables["dsChiaviConfig"];

            foreach (DocsPAWA.DocsPaWR.ChiaveConfigurazione chiaveConfig in chiaviConfig)
            {
                if (chiaveConfig.Visibile.Equals("1"))
                {
                    //
                    // Mev CS 1.3
                    // La chiave globale PGU_FE_DISABLE_AMM_GEST_CONS non deve essere gestibile tra le chiavi di configurazione
                    if (!chiaveConfig.Codice.Equals("PGU_FE_DISABLE_AMM_GEST_CONS"))
                    {

                        DataRow row = dt.NewRow();

                        row[TABLE_COL_ID] = chiaveConfig.IDChiave;
                        row[TABLE_COL_ID_AMMINISTRAZIONE] = chiaveConfig.IDAmministrazione;
                        row[TABLE_COL_CODICE] = chiaveConfig.Codice;
                        row[TABLE_COL_DESCRIZIONE] = chiaveConfig.Descrizione;
                        row[TABLE_COL_VALORE] = chiaveConfig.Valore;

                        string tipo = "";
                        if (chiaveConfig.IsGlobale.Equals("1"))
                            tipo = "GLOBALE";
                        else
                            tipo = "AMM: " + this.GetCodiceAmministrazione();
                        row[TABLE_COL_TIPO] = tipo;

                        if (chiaveConfig.IsConservazione.Equals("1"))
                        {
                            if (!this.IsConservazionePARER())
                                dt.Rows.Add(row);
                        }
                        else
                        {
                            dt.Rows.Add(row);
                        }
                    }
                    
                    //
                    // Old Code
                    //DataRow row = dt.NewRow();

                    //row[TABLE_COL_ID] = chiaveConfig.IDChiave;
                    //row[TABLE_COL_ID_AMMINISTRAZIONE] = chiaveConfig.IDAmministrazione;
                    //row[TABLE_COL_CODICE] = chiaveConfig.Codice;
                    //row[TABLE_COL_DESCRIZIONE] = chiaveConfig.Descrizione;
                    //row[TABLE_COL_VALORE] = chiaveConfig.Valore;

                    //string tipo = "";
                    //if (chiaveConfig.IsGlobale.Equals("1"))
                    //    tipo = "GLOBALE";
                    //else
                    //    tipo = "AMM: " + this.GetCodiceAmministrazione();
                    //row[TABLE_COL_TIPO] = tipo;

                    //dt.Rows.Add(row);
                    
                    //
                    // End Mev CS 1.3
                }
            }

            return ds;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private DataSet CreateGridDataSet()
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable("dsChiaviConfig");

            dt.Columns.Add(new DataColumn(TABLE_COL_ID));
            dt.Columns.Add(new DataColumn(TABLE_COL_ID_AMMINISTRAZIONE));
            dt.Columns.Add(new DataColumn(TABLE_COL_CODICE));
            dt.Columns.Add(new DataColumn(TABLE_COL_DESCRIZIONE));
            dt.Columns.Add(new DataColumn(TABLE_COL_VALORE));
            dt.Columns.Add(new DataColumn(TABLE_COL_TIPO));
            ds.Tables.Add(dt);
            return ds;
        }

        /// <summary>
        /// Predisposizione per l'inserimento di un nuovo registro
        /// </summary>

        #endregion

        #region Gestione javascript


        /// <summary>
        /// Registrazione script client
        /// </summary>
        /// <param name="scriptKey"></param>
        /// <param name="scriptValue"></param>
        private void RegisterClientScript(string scriptKey, string scriptValue)
        {
            if (!this.Page.IsStartupScriptRegistered(scriptKey))
            {
                string scriptString = "<SCRIPT>" + scriptValue + "</SCRIPT>";
                this.Page.RegisterStartupScript(scriptKey, scriptString);
            }
        }

        #endregion

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dg_chiavi.ItemCreated += new System.Web.UI.WebControls.DataGridItemEventHandler(this.dg_chiavi_ItemCreated);
            this.dg_chiavi.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_chiavi_ItemCommand);
            this.btn_chiudiPnlInfo.Click += new System.Web.UI.ImageClickEventHandler(this.btn_chiudiPnlInfo_Click);
            this.btn_salva.Click += new System.EventHandler(this.btn_salva_Click);
            this.Load += new System.EventHandler(this.Page_Load);

        }
        #endregion

        private void btn_chiudiPnlInfo_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            //visibilità informazioni
            pnl_info.Visible = false;
            this.dg_chiavi.SelectedIndex = -1;
        }

        protected void btn_salva_Click(object sender, EventArgs e)
        {
            this.Save();
        }

        /// <summary>
        /// INTEGRAZIONE PITRE-PARER
        /// Determina se è attiva la conservazione PARER
        /// </summary>
        /// <returns></returns>
        protected bool IsConservazionePARER()
        {
            bool result = false;

            string IS_CONSERVAZIONE_PARER = string.Empty;
            IS_CONSERVAZIONE_PARER = DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_WA_CONSERVAZIONE");
            result = ((string.IsNullOrEmpty(IS_CONSERVAZIONE_PARER) || IS_CONSERVAZIONE_PARER.Equals("0")) ? false : true);

            return result;
        }
    }
}
