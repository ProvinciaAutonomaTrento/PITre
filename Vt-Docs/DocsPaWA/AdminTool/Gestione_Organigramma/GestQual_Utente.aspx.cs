using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace DocsPAWA.AdminTool.Gestione_Organigramma
{
    public partial class GestQual_Utente : System.Web.UI.Page
    {

        public String idAmm, idUo, idGruppo, idPeople = "";
        int index;

        #region Dichiarazione costanti
        // Costanti che identificano i nomi delle colonne del datagrid qualifiche
        private const string TABLE_COL_ID = "idQual";
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
            this.dg_qualifiche.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_qualifiche_utente_ItemCommand);
            this.dg_qualifiche.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dg_qualifiche_PageIndexChanged);
            this.dg_qualifiche.PreRender += new System.EventHandler(this.dg_qualifiche_PreRender);
            this.dg_utente.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_qualifiche_utente_ItemCommand);
            this.dg_utente.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dg_utente_PageIndexChanged);
            this.dg_utente.PreRender += new System.EventHandler(this.dg_utente_PreRender);
        }
        #endregion

        #region Page Load
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Expires = -1;

            idAmm = Request.QueryString["idAmm"]; //amministrazione
            idUo = Request.QueryString["idUo"];  //ufficio
            idGruppo = Request.QueryString["idGruppo"];  //ruolo
            idPeople = Request.QueryString["idPeople"];  //utente

            if (!IsPostBack)
            {
                DocsPaWR.Corrispondente uo = DocsPAWA.UserManager.getCorrispondenteBySystemID(this, idUo);
                DocsPaWR.Ruolo ruolo = DocsPAWA.UserManager.getRuoloByIdGruppo(idGruppo, this);
                DocsPaWR.Utente utente = DocsPAWA.UserManager.GetUtenteByIdPeople(idPeople);

                lbl_percorso.Text = uo.descrizione + " > " + ruolo.descrizione + " > " + utente.descrizione;

                this.FillListQualifiche();
                this.FillListUtente();
            }
        }
        #endregion

        #region Gestione accesso ai dati
        /// Gestione caricamento lista registri
        private void FillListQualifiche()
        {
            Amministrazione.Manager.UtentiManager utManager = new Amministrazione.Manager.UtentiManager();
            int idAmministrazione = Convert.ToInt32(AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3"));
            List<DocsPaWR.Qualifica> qualifiche = utManager.GetQualifiche(idAmministrazione);
            DataSet dsQualifiche = this.ConvertToDataSet_dg_qualifiche(qualifiche);

            // Impostazione dataset in sessione
            this.SetSessionDataSetQualifiche(dsQualifiche);
            DataView dvQualifiche = dsQualifiche.Tables["Qualifiche"].DefaultView;
            dvQualifiche.Sort = "Descrizione ASC";
            
            this.dg_qualifiche.DataSource = dsQualifiche;
            this.dg_qualifiche.DataBind();

            qualifiche = null;
        }

        private void FillListUtente()
        {
            Amministrazione.Manager.UtentiManager utManager = new Amministrazione.Manager.UtentiManager();
            List<DocsPaWR.PeopleGroupsQualifiche> pgqs = utManager.GetPeopleGroupsQualifiche(idAmm,idUo,idGruppo,idPeople);
            if (pgqs.Count != 0)
            {
                DataSet dsUtente = this.ConvertToDataSet_dg_utente(pgqs);

                // Impostazione dataset in sessione
                this.SetSessionDataSetUtente(dsUtente);
                DataView dvUtente = dsUtente.Tables["Utente"].DefaultView;
                dvUtente.Sort = "Descrizione_utente ASC";

                this.dg_utente.DataSource = dsUtente;
                this.dg_utente.DataBind();

                pgqs = null;
                this.lbl_risultatoUtentiQualifiche.Visible = false;
            }
            else
            {
                this.lbl_risultatoUtentiQualifiche.Visible = true;
                this.dg_utente.DataSource = null;
                this.dg_utente.DataBind();
            }
        }

        /// Conversione array per il datagrid qualifiche
        private DataSet ConvertToDataSet_dg_qualifiche(List<DocsPaWR.Qualifica> qualifiche)
        {
            DataSet ds = this.CreateGridDataSet_dg_qualifiche();
            DataTable dt = ds.Tables["Qualifiche"];

            foreach (DocsPAWA.DocsPaWR.Qualifica qualifica in qualifiche)
            {
                DataRow row = dt.NewRow();

                row[TABLE_COL_ID] = qualifica.SYSTEM_ID;
                row[TABLE_COL_CODICE] = qualifica.CODICE;
                row[TABLE_COL_DESCRIZIONE] = qualifica.DESCRIZIONE;
                dt.Rows.Add(row);
            }
            return ds;
        }

      
        private DataSet CreateGridDataSet_dg_qualifiche()
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable("Qualifiche");

            dt.Columns.Add(new DataColumn(TABLE_COL_ID));
            dt.Columns.Add(new DataColumn(TABLE_COL_CODICE));
            dt.Columns.Add(new DataColumn(TABLE_COL_DESCRIZIONE));
            ds.Tables.Add(dt);
            return ds;
        }


        /// Conversione array per il datagrid utente
        private DataSet ConvertToDataSet_dg_utente(List<DocsPaWR.PeopleGroupsQualifiche> pgqs)
        {
            DataSet ds = this.CreateGridDataSet_dg_utente();
            DataTable dt = ds.Tables["Utente"];

            foreach (DocsPAWA.DocsPaWR.PeopleGroupsQualifiche pgq in pgqs)
            {
                DataRow row = dt.NewRow();
                row[TABLE_COL_SYSTEM_ID] = pgq.SYSTEM_ID;
                row[TABLE_COL_DESCRIZIONE_UTENTE] = pgq.CODICE + " - " + pgq.DESCRIZIONE;
                dt.Rows.Add(row);
            }
            return ds;
        }


        private DataSet CreateGridDataSet_dg_utente()
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable("Utente");

            dt.Columns.Add(new DataColumn(TABLE_COL_SYSTEM_ID));
            dt.Columns.Add(new DataColumn(TABLE_COL_DESCRIZIONE_UTENTE));
            ds.Tables.Add(dt);
            return ds;
        }

        private void Save(String idAmm, String idUo, String idGruppo, String idPeople, String idQual)
        {
            DocsPAWA.DocsPaWR.PeopleGroupsQualifiche pgq = new DocsPAWA.DocsPaWR.PeopleGroupsQualifiche();
            DocsPAWA.DocsPaWR.ValidationResultInfo result = null;
            this.RefreshPGFromUI(ref pgq, idAmm, idUo, idGruppo, idPeople, idQual);

            result = this.InsertPeopleGroupsQual(pgq);
            
            if (!result.Value)
            {
                this.ShowValidationMessage(result);
            }

        }

        /// Cancellazione peopleGroups
        private void Delete(String idPeopleGroups)
        {
            DocsPAWA.DocsPaWR.ValidationResultInfo result = null;           
            result = this.DeletePeopleGroups(idPeopleGroups);

            if (!result.Value)
            {
                this.ShowValidationMessage(result);
            }  
        }

        //cancellazione peopleGroups
        private DocsPAWA.DocsPaWR.ValidationResultInfo DeletePeopleGroups(String idPeopleGroups)
        {
            Amministrazione.Manager.UtentiManager utManager = new Amministrazione.Manager.UtentiManager();
            DocsPAWA.DocsPaWR.ValidationResultInfo retValue = utManager.DeletePeopleGroups(idPeopleGroups);
            return retValue;
        }

        /// Visualizzazione messaggi di validazione
        private void ShowValidationMessage(DocsPAWA.DocsPaWR.ValidationResultInfo validationResult)
        {
            // Visualizzazione delle regole di business non valide
            bool warningMessage;
            string validationMessage = this.GetValidationMessage(validationResult, out warningMessage);
            this.RegisterClientScript("ShowValidationMessage", "alert('" + validationMessage + "');");
        }

        private string GetValidationMessage(DocsPAWA.DocsPaWR.ValidationResultInfo validationResult,
                                            out bool warningMessage)
        {
            string retValue = string.Empty;
            bool errorMessage = false;

            foreach (DocsPAWA.DocsPaWR.BrokenRule rule in validationResult.BrokenRules)
            {
                if (!errorMessage && rule.Level == DocsPAWA.DocsPaWR.BrokenRuleLevelEnum.Error)
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

        /// Inserimento di un nuovo PeopleGroupsQualifiche    
        private DocsPAWA.DocsPaWR.ValidationResultInfo InsertPeopleGroupsQual(DocsPAWA.DocsPaWR.PeopleGroupsQualifiche pgq)
        {
            Amministrazione.Manager.UtentiManager utManager = new Amministrazione.Manager.UtentiManager();
            DocsPAWA.DocsPaWR.ValidationResultInfo retValue = utManager.InsertPeopleGroupsQual(pgq);
            return retValue;
        }


        //popolo oggetto qualifica
        private void RefreshPGFromUI(ref DocsPAWA.DocsPaWR.PeopleGroupsQualifiche pgq, String idAmm, String idUo, String idGruppo, String idPeople, String idQual)
        {
            pgq.ID_AMM = Convert.ToInt32(idAmm);
            pgq.ID_UO = Convert.ToInt32(idUo);
            pgq.ID_GRUPPO = Convert.ToInt32(idGruppo);
            pgq.ID_PEOPLE = Convert.ToInt32(idPeople);
            pgq.ID_QUALIFICA = Convert.ToInt32(idQual);
        }

        #endregion

        #region Gestione tasti pannello
        private void dg_qualifiche_utente_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            string commandName = e.CommandName;
      
            if (commandName.Equals("Inserimento"))
            {
                String idQual=e.Item.Cells[GRID_COL_ID].Text;
                this.Save(idAmm, idUo, idGruppo, idPeople, idQual);
                this.FillListQualifiche();
                this.FillListUtente();
                
            }
            if (commandName.Equals("Eliminazione"))
            {
                String idPeopleGroups = e.Item.Cells[GRID_COL_SYSTEM_ID].Text;
                this.Delete(idPeopleGroups);
                this.FillListQualifiche();
                this.FillListUtente();
       
            }
        }

        private void dg_utente_PreRender(object sender, System.EventArgs e)
        {
            for (int i = 0; i < this.dg_utente.Items.Count; i++)
            {

                if (this.dg_utente.Items[i].ItemIndex >= 0)
                {
                    switch (this.dg_utente.Items[i].ItemType.ToString().Trim())
                    {
                        case "Item":
                            this.dg_utente.Items[i].Attributes.Add("onmouseover", "this.className='bg_grigioS'");
                            this.dg_utente.Items[i].Attributes.Add("onmouseout", "this.className='bg_grigioN'");
                            break;

                        case "AlternatingItem":
                            this.dg_utente.Items[i].Attributes.Add("onmouseover", "this.className='bg_grigioS'");
                            this.dg_utente.Items[i].Attributes.Add("onmouseout", "this.className='bg_grigioA'");
                            break;
                    }
                }
            }
        }

        private void dg_qualifiche_PreRender(object sender, System.EventArgs e)
        {
            for (int i = 0; i < this.dg_qualifiche.Items.Count; i++)
            {

                if (this.dg_qualifiche.Items[i].ItemIndex >= 0)
                {
                    switch (this.dg_qualifiche.Items[i].ItemType.ToString().Trim())
                    {
                        case "Item":
                            this.dg_qualifiche.Items[i].Attributes.Add("onmouseover", "this.className='bg_grigioS'");
                            this.dg_qualifiche.Items[i].Attributes.Add("onmouseout", "this.className='bg_grigioN'");
                            break;

                        case "AlternatingItem":
                            this.dg_qualifiche.Items[i].Attributes.Add("onmouseover", "this.className='bg_grigioS'");
                            this.dg_qualifiche.Items[i].Attributes.Add("onmouseout", "this.className='bg_grigioA'");
                            break;
                    }
                }
            }
        }

        private void dg_qualifiche_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {
            try
            {
                this.dg_qualifiche.CurrentPageIndex = e.NewPageIndex;

                DataSet dsQualifiche = this.GetSessionDataSetQualifiche();
                DataView dvQualifiche = dsQualifiche.Tables["Qualifiche"].DefaultView;
                dvQualifiche.Sort = "Descrizione ASC";
                this.dg_qualifiche.DataSource = dvQualifiche;
                this.dg_qualifiche.DataBind();
            }
            catch
            {
                //this.gestErrori();
            }
        }

        private void dg_utente_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {
            try
            {
                this.dg_utente.CurrentPageIndex = e.NewPageIndex;

                DataSet dsUtente = this.GetSessionDataSetUtente();
                DataView dvUtente = dsUtente.Tables["Utente"].DefaultView;
                dvUtente.Sort = "Descrizione_utente ASC";
                this.dg_utente.DataSource = dvUtente;
                this.dg_utente.DataBind();

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
        private void SetSessionDataSetQualifiche(DataSet dsQualifiche)
        {
            Session["QUALIFICHEDATASET"] = dsQualifiche;
        }

        private DataSet GetSessionDataSetQualifiche()
        {
            return (DataSet)Session["QUALIFICHEDATASET"];
        }

        private void RemoveSessionDataSetQualifiche()
        {
            this.GetSessionDataSetQualifiche().Dispose();
            Session.Remove("QUALIFICHEDATASET");
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