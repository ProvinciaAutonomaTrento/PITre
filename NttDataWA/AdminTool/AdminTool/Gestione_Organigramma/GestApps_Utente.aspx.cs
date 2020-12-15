using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace SAAdminTool.AdminTool.Gestione_Organigramma
{
    public partial class GestApps_Utente : System.Web.UI.Page
    {

        public String idUtente, idApplicazione = "";
        int index;

        #region Dichiarazione costanti
        // Costanti che identificano i nomi delle colonne del datagrid Applicazioni
        private const string TABLE_COL_ID = "idApp";
        private const string TABLE_COL_CODICE = "Codice";
        private const string TABLE_COL_DESCRIZIONE = "Descrizione";

        private const int GRID_COL_ID = 0;  //id qualifica (non visibile)
        private const int GRID_COL_CODICE = 1;  //codice qualifica
        private const int GRID_COL_DESCRIZIONE = 2;  //descrizione qualifica
        private const int GRID_COL_INSERT = 3; //tasto per inserimento

        // Costanti che identificano i nomi delle colonne del datagrid utente
        private const string TABLE_COL_SYSTEM_ID = "system_id";
        private const string TABLE_COL_DESCRIZIONE_UTENTE = "Descrizione_utente";

        private const int GRID_COL_SYSTEM_ID = 0;  //id qualifica (non visibile)
        private const int GRID_COL_DESCRIZIONE_UTENTE = 1;  //descrizione qualifica 
        private const int GRID_COL_DELETE = 2; //tasto per cancellazione

        #endregion

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

       
        private void InitializeComponent()
        {
            this.dg_applicazioni.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_applicazioni_utente_ItemCommand);
            this.dg_applicazioni.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dg_applicazioni_PageIndexChanged);
            this.dg_applicazioni.PreRender += new System.EventHandler(this.dg_applicazioni_PreRender);
            this.dg_appute.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_applicazioni_utente_ItemCommand);
            this.dg_appute.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dg_appute_PageIndexChanged);
            this.dg_appute.PreRender += new System.EventHandler(this.dg_appute_PreRender);
        }
        #endregion

        #region Page Load
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Expires = -1;

            idApplicazione = Request.QueryString["idApplicazione"];  //ruolo
            idUtente = Request.QueryString["idPeople"];  //utente

            if (!IsPostBack)
            {
                lbl_percorso.Text = "Gestione Applicazioni Utente";

                this.FillListApplicazioni();
                this.FillListUtente();
            }
        }
        #endregion

        #region Gestione accesso ai dati
        /// Gestione caricamento lista registri
        private void FillListApplicazioni()
        {
            DocsPaWR.ExtApplication[] applicatons = SAAdminTool.ApplicationManager.getListaApplicazioni(this);
            DataSet apps = this.ConvertToDataSet_dg_applicazioni(applicatons);

            // Impostazione dataset in sessione
            this.SetSessionDataSetApplicazioni(apps);
            DataView dvApplicazioni = apps.Tables["Applicazioni"].DefaultView;
            dvApplicazioni.Sort = "Descrizione ASC";

            this.dg_applicazioni.DataSource = apps;
            this.dg_applicazioni.DataBind();

            apps = null;
        }

        private void FillListUtente()
        {
            DocsPaWR.Utente utente = SAAdminTool.UserManager.GetUtenteByIdPeople(idUtente);
            Array appute = utente.extApplications;

            if (appute.Length != 0)
            {
                DataSet dsUtente = this.ConvertToDataSet_dg_appute(appute);

                // Impostazione dataset in sessione
                this.SetSessionDataSetUtente(dsUtente);
                DataView dvUtente = dsUtente.Tables["AppUte"].DefaultView;
                dvUtente.Sort = "Descrizione_utente ASC";

                this.dg_appute.DataSource = dsUtente;
                this.dg_appute.DataBind();

                appute = null;
                this.lbl_risultatoUtentiApps.Visible = false;
            }
            else
            {
                this.lbl_risultatoUtentiApps.Visible = true;
                this.dg_appute.DataSource = null;
                this.dg_appute.DataBind();
            }
        }

        /// Conversione array per il datagrid Applicazioni
        private DataSet ConvertToDataSet_dg_applicazioni(Array applicazioni)
        {
            DataSet ds = this.CreateGridDataSet_dg_applicazioni();
            DataTable dt = ds.Tables["Applicazioni"];

            foreach (DocsPaWR.ExtApplication applicazione in applicazioni)
            {
                DataRow row = dt.NewRow();

                row[TABLE_COL_ID] = applicazione.systemId;
                row[TABLE_COL_CODICE] = applicazione.codice;
                row[TABLE_COL_DESCRIZIONE] = applicazione.descrizione;
                dt.Rows.Add(row);
            }
            return ds;
        }

      
        private DataSet CreateGridDataSet_dg_applicazioni()
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable("Applicazioni");

            dt.Columns.Add(new DataColumn(TABLE_COL_ID));
            dt.Columns.Add(new DataColumn(TABLE_COL_CODICE));
            dt.Columns.Add(new DataColumn(TABLE_COL_DESCRIZIONE));
            ds.Tables.Add(dt);
            return ds;
        }


        /// Conversione array per il datagrid utente
        private DataSet ConvertToDataSet_dg_appute(Array pgqs)
        {
            DataSet ds = this.CreateGridDataSet_dg_appute();
            DataTable dt = ds.Tables["AppUte"];

            foreach (DocsPaWR.ExtApplication pgq in pgqs)
            {
                DataRow row = dt.NewRow();
                row[TABLE_COL_SYSTEM_ID] = pgq.systemId;
                row[TABLE_COL_DESCRIZIONE_UTENTE] = pgq.codice + " - " + pgq.descrizione;
                dt.Rows.Add(row);
            }
            return ds;
        }


        private DataSet CreateGridDataSet_dg_appute()
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable("AppUte");

            dt.Columns.Add(new DataColumn(TABLE_COL_SYSTEM_ID));
            dt.Columns.Add(new DataColumn(TABLE_COL_DESCRIZIONE_UTENTE));
            ds.Tables.Add(dt);
            return ds;
        }

        private void Save(String idApplicazione)
        {
            SAAdminTool.DocsPaWR.ValidationResultInfo result = null;
            result = this.AddApplicazioneUtente(idApplicazione);
            
            if (!result.Value)
            {
                this.ShowValidationMessage(result);
            }

        }

        /// Cancellazione peopleGroups
        private void Delete(String idApplicazione)
        {
            SAAdminTool.DocsPaWR.ValidationResultInfo result = null;
            result = this.DeleteApplicazioneUtente(idApplicazione);

            if (!result.Value)
            {
                this.ShowValidationMessage(result);
            }  
        }

        //cancellazione peopleGroups
        private SAAdminTool.DocsPaWR.ValidationResultInfo DeleteApplicazioneUtente(String idApplicazione)
        {
            SAAdminTool.DocsPaWR.ValidationResultInfo retValue = SAAdminTool.ApplicationManager.ExtAppDeleteUte(idApplicazione, idUtente);
            return retValue;
        }

        private SAAdminTool.DocsPaWR.ValidationResultInfo AddApplicazioneUtente(String idApplicazione)
        {
            SAAdminTool.DocsPaWR.ValidationResultInfo retValue = SAAdminTool.ApplicationManager.ExtAppAddUte(idApplicazione, idUtente); ;
            return retValue;
        }

        /// Visualizzazione messaggi di validazione
        private void ShowValidationMessage(SAAdminTool.DocsPaWR.ValidationResultInfo validationResult)
        {
            // Visualizzazione delle regole di business non valide
            bool warningMessage;
            string validationMessage = this.GetValidationMessage(validationResult, out warningMessage);
            this.RegisterClientScript("ShowValidationMessage", "alert('" + validationMessage + "');");
        }

        private string GetValidationMessage(SAAdminTool.DocsPaWR.ValidationResultInfo validationResult,
                                            out bool warningMessage)
        {
            string retValue = string.Empty;
            bool errorMessage = false;

            foreach (SAAdminTool.DocsPaWR.BrokenRule rule in validationResult.BrokenRules)
            {
                if (!errorMessage && rule.Level == SAAdminTool.DocsPaWR.BrokenRuleLevelEnum.Error)
                    errorMessage = true;

                if (retValue != string.Empty)
                    retValue += "\\n";

                retValue += " - " + rule.Description;
            }

            if (errorMessage)
                retValue = "Sono state riscontrate le seguenti anomalie:\\n\\n" + retValue;
            else
                retValue = "Attenzione:\\n\\n" + retValue;

            warningMessage = !errorMessage;
            return retValue.Replace("'", "\\'");
        }

        /// Inserimento di un nuovo PeopleGroupsApplicazioni    
        private SAAdminTool.DocsPaWR.ValidationResultInfo InsertPeopleGroupsQual(SAAdminTool.DocsPaWR.PeopleGroupsQualifiche pgq)
        {
            Amministrazione.Manager.UtentiManager utManager = new Amministrazione.Manager.UtentiManager();
            SAAdminTool.DocsPaWR.ValidationResultInfo retValue = utManager.InsertPeopleGroupsQual(pgq);
            return retValue;
        }


        //popolo oggetto applicazione
        private void RefreshPGFromUI(ref SAAdminTool.DocsPaWR.PeopleGroupsQualifiche pgq, String idAmm, String idUo, String idGruppo, String idUtente, String idApp)
        {
            pgq.ID_AMM = Convert.ToInt32(idAmm);
            pgq.ID_UO = Convert.ToInt32(idUo);
            pgq.ID_GRUPPO = Convert.ToInt32(idGruppo);
            pgq.ID_PEOPLE = Convert.ToInt32(idUtente);
            pgq.ID_QUALIFICA = Convert.ToInt32(idApp);
        }

        #endregion

        #region Gestione tasti pannello
        private void dg_applicazioni_utente_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            string commandName = e.CommandName;
      
            if (commandName.Equals("Inserimento"))
            {
                idApplicazione = e.Item.Cells[GRID_COL_ID].Text;
                this.Save(idApplicazione);
                this.FillListApplicazioni();
                this.FillListUtente();
                
            }
            if (commandName.Equals("Eliminazione"))
            {
                idApplicazione = e.Item.Cells[GRID_COL_SYSTEM_ID].Text;
                this.Delete(idApplicazione);
                this.FillListApplicazioni();
                this.FillListUtente();
       
            }
        }

        private void dg_appute_PreRender(object sender, System.EventArgs e)
        {
            for (int i = 0; i < this.dg_appute.Items.Count; i++)
            {

                if (this.dg_appute.Items[i].ItemIndex >= 0)
                {
                    switch (this.dg_appute.Items[i].ItemType.ToString().Trim())
                    {
                        case "Item":
                            this.dg_appute.Items[i].Attributes.Add("onmouseover", "this.className='bg_grigioS'");
                            this.dg_appute.Items[i].Attributes.Add("onmouseout", "this.className='bg_grigioN'");
                            break;

                        case "AlternatingItem":
                            this.dg_appute.Items[i].Attributes.Add("onmouseover", "this.className='bg_grigioS'");
                            this.dg_appute.Items[i].Attributes.Add("onmouseout", "this.className='bg_grigioA'");
                            break;
                    }
                }
            }
        }

        private void dg_applicazioni_PreRender(object sender, System.EventArgs e)
        {
            for (int i = 0; i < this.dg_applicazioni.Items.Count; i++)
            {

                if (this.dg_applicazioni.Items[i].ItemIndex >= 0)
                {
                    switch (this.dg_applicazioni.Items[i].ItemType.ToString().Trim())
                    {
                        case "Item":
                            this.dg_applicazioni.Items[i].Attributes.Add("onmouseover", "this.className='bg_grigioS'");
                            this.dg_applicazioni.Items[i].Attributes.Add("onmouseout", "this.className='bg_grigioN'");
                            break;

                        case "AlternatingItem":
                            this.dg_applicazioni.Items[i].Attributes.Add("onmouseover", "this.className='bg_grigioS'");
                            this.dg_applicazioni.Items[i].Attributes.Add("onmouseout", "this.className='bg_grigioA'");
                            break;
                    }
                }
            }
        }

        private void dg_applicazioni_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {
            try
            {
                this.dg_applicazioni.CurrentPageIndex = e.NewPageIndex;

                DataSet dsApplicazioni = this.GetSessionDataSetApplicazioni();
                DataView dvApplicazioni = dsApplicazioni.Tables["Applicazioni"].DefaultView;
                dvApplicazioni.Sort = "Descrizione ASC";
                this.dg_applicazioni.DataSource = dvApplicazioni;
                this.dg_applicazioni.DataBind();
            }
            catch
            {
                //this.gestErrori();
            }
        }

        private void dg_appute_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {
            try
            {
                this.dg_appute.CurrentPageIndex = e.NewPageIndex;

                DataSet dsUtente = this.GetSessionDataSetUtente();
                DataView dvUtente = dsUtente.Tables["Utente"].DefaultView;
                dvUtente.Sort = "Descrizione_utente ASC";
                this.dg_appute.DataSource = dvUtente;
                this.dg_appute.DataBind();

            }
            catch
            {
                //this.gestErrori();
            }
        }
        #endregion

        #region Gestione javascript


        /// Registrazione script client        
        private void RegisterClientScript(string scriptKey, string scriptValue)
        {
            if (!this.Page.IsStartupScriptRegistered(scriptKey))
            {
                string scriptString = "<SCRIPT>" + scriptValue + "</SCRIPT>";
                this.Page.RegisterStartupScript(scriptKey, scriptString);
            }
        }
        #endregion

        #region Gestione sessione
        private void SetSessionDataSetApplicazioni(DataSet dsApplicazioni)
        {
            Session["APPLICAZIONIDATASET"] = dsApplicazioni;
        }

        private DataSet GetSessionDataSetApplicazioni()
        {
            return (DataSet)Session["APPLICAZIONIDATASET"];
        }

        private void RemoveSessionDataSetApplicazioni()
        {
            this.GetSessionDataSetApplicazioni().Dispose();
            Session.Remove("APPLICAZIONIDATASET");
        }

        private void SetSessionDataSetUtente(DataSet dsUtente)
        {
            Session["UTENTEDATASET"] = dsUtente;
        }

        private DataSet GetSessionDataSetUtente()
        {
            return (DataSet)Session["UTENTEDATASET"];
        }

        private void RemoveSessionDataSetUtente()
        {
            this.GetSessionDataSetUtente().Dispose();
            Session.Remove("UTENTEDATASET");
        }
        #endregion

    }
}