using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace DocsPAWA.AdminTool.Gestione_Qualifiche
{
    public partial class GestQual : System.Web.UI.Page
    {

        protected DocsPAWA.DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
        
        #region Dichiarazione costanti
        // Costanti che identificano i nomi delle colonne del datagrid
        private const string TABLE_COL_ID = "ID";
        private const string TABLE_COL_CODICE = "Codice";
        private const string TABLE_COL_DESCRIZIONE = "Descrizione";
        private const string TABLE_COL_ID_AMM = "ID_AMM";

        private const int GRID_COL_ID = 0; 
        private const int GRID_COL_CODICE = 1;
        private const int GRID_COL_DESCRIZIONE = 2;
        private const int GRID_COL_DETAIL = 3;
        private const int GRID_COL_DELETE = 4;
        private const int GRID_COL_ID_AMM = 5;
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.btn_nuova.Click += new System.EventHandler(this.btn_nuova_Click);
                // Caricamento lista QUALIFICHE
                this.FillListGQ();
                this.lbl_position.Text = "&nbsp;&bull;&nbsp;Amministrazione: " + AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "1");
            }
        }

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
            this.btn_nuova.Click += new System.EventHandler(this.btn_nuova_Click);
            this.btn_aggiungi.Click += new System.EventHandler(this.btn_aggiungi_Click);
            this.dg_GQ.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DeleteItemCommand);
            this.dg_GQ.ItemCreated += new System.Web.UI.WebControls.DataGridItemEventHandler(this.dg_GQ_ItemCreated);
            this.dg_GQ.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_GQ_ItemCommand);
        }        

        #region Gestione accesso ai dati
        /// Gestione caricamento lista registri
        private void FillListGQ()
        {
            Amministrazione.Manager.UtentiManager utManager = new Amministrazione.Manager.UtentiManager();
            int idAmministrazione = Convert.ToInt32(AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3"));
            List<DocsPaWR.Qualifica> qualifiche = utManager.GetQualifiche(idAmministrazione);
            DataSet ds = this.ConvertToDataSet(qualifiche);
            this.dg_GQ.DataSource = ds;
            this.dg_GQ.DataBind();
            qualifiche = null;
        }

        
        /// Conversione array
        private DataSet ConvertToDataSet(List<DocsPaWR.Qualifica> qualifiche)
        {
            DataSet ds = this.CreateGridDataSet();
            DataTable dt = ds.Tables["Qualifiche"];

            foreach (DocsPAWA.DocsPaWR.Qualifica qualifica in qualifiche)
            {
                DataRow row = dt.NewRow();

                row[TABLE_COL_ID] = qualifica.SYSTEM_ID;
                row[TABLE_COL_CODICE] = qualifica.CODICE;
                row[TABLE_COL_DESCRIZIONE] = qualifica.DESCRIZIONE;
                row[TABLE_COL_ID_AMM] = qualifica.ID_AMM;
                dt.Rows.Add(row);
            }

            return ds;
        }

      
        private DataSet CreateGridDataSet()
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable("Qualifiche");

            dt.Columns.Add(new DataColumn(TABLE_COL_ID));
            dt.Columns.Add(new DataColumn(TABLE_COL_CODICE));
            dt.Columns.Add(new DataColumn(TABLE_COL_DESCRIZIONE));
            dt.Columns.Add(new DataColumn(TABLE_COL_ID_AMM));
            ds.Tables.Add(dt);

            return ds;
        }


        private void Save()
        {
            DocsPAWA.DocsPaWR.Qualifica qual = new DocsPAWA.DocsPaWR.Qualifica();
            DocsPAWA.DocsPaWR.ValidationResultInfo result = null;
            bool insertMode = this.OnInsertMode();
            this.RefreshQualFromUI(ref qual, insertMode);

            if (insertMode) //inserimento
                result = this.InsertQual(qual);
            else //aggiornamento
                result = this.UpdateQual(this.dg_GQ.SelectedItem.Cells[GRID_COL_ID].Text, this.txt_descrizione.Text);

            if (!result.Value)
            {
                this.ShowValidationMessage(result); 
            }

            if (!insertMode)
            {
                // Aggiornamento
                pnl_info.Visible = false;

                this.ClearData();

                if (result.Value)
                {
                    // Aggiornamento elemento griglia corrente
                    this.RefreshGridItem(qual);
                }

                dg_GQ.SelectedIndex = -1;
            }
            else
            {
                // Inserimento

                // Refresh lista registri
                this.FillListGQ();

                // Predisposizione per un nuovo inserimento
                this.SetInsertMode();
            }
        }

        
        //popolo oggetto qualifica
        private void RefreshQualFromUI(ref DocsPAWA.DocsPaWR.Qualifica qualifica, bool insertMode)
        {

            qualifica.CODICE= this.txt_codice.Text.Trim();
            qualifica.DESCRIZIONE = this.txt_descrizione.Text.Trim();

            int idAmministrazione = Convert.ToInt32(AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3"));
            qualifica.ID_AMM = Convert.ToInt32(idAmministrazione);

        }


        
        /// Visualizzazione messaggi di validazione
        private void ShowValidationMessage(DocsPAWA.DocsPaWR.ValidationResultInfo validationResult)
        {
            // Visualizzazione delle regole di business non valide
            bool warningMessage;

            string validationMessage = this.GetValidationMessage(validationResult, out warningMessage);

            this.RegisterClientScript("ShowValidationMessage", "ShowValidationMessage('" + validationMessage + "'," + warningMessage.ToString().ToLower() + ");");
        }

        /// Inserimento di una nuova qualifica     
        private DocsPAWA.DocsPaWR.ValidationResultInfo InsertQual(DocsPAWA.DocsPaWR.Qualifica qual)
        {
            Amministrazione.Manager.UtentiManager utManager = new Amministrazione.Manager.UtentiManager();
            DocsPAWA.DocsPaWR.ValidationResultInfo retValue = utManager.InsertQual(qual);
            return retValue;
        }

        /// Aggiornamento di una qualifica     
        private DocsPAWA.DocsPaWR.ValidationResultInfo UpdateQual(String idQualifica, String descrizione)
        {
            Amministrazione.Manager.UtentiManager utManager = new Amministrazione.Manager.UtentiManager();
            DocsPAWA.DocsPaWR.ValidationResultInfo retValue = utManager.UpdateQual(idQualifica, descrizione);
            return retValue;
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

        //cancellazione qualifica
        private DocsPAWA.DocsPaWR.ValidationResultInfo DeleteQual(String idQualifica, int idAmministrazione)
        {
            Amministrazione.Manager.UtentiManager utManager = new Amministrazione.Manager.UtentiManager();
            DocsPAWA.DocsPaWR.ValidationResultInfo retValue = utManager.DeleteQual(idQualifica, idAmministrazione);
            return retValue;
        }

        private void dg_GQ_ItemCreated(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
        {
            e.Item.Cells[GRID_COL_DELETE].Attributes.Add("onclick", "if (!window.confirm('Sei sicuro di voler eliminare questa qualifica?')) {return false};");
        }

        #endregion

        #region Tasti pannello info

        private void btn_nuova_Click(object sender, System.EventArgs e)
        {      
            //visibilità informazioni
            pnl_info.Visible = true;
            btn_aggiungi.Text = "Aggiungi";       
            txt_codice.Visible = true;
            lbl_cod.Visible = false;

            // Rimozione dati controlli UI
            this.ClearData();

            SetFocus(txt_codice);
            dg_GQ.SelectedIndex = -1;
        }


        private void ClearData()
        {
            txt_codice.Text = string.Empty;
            lbl_cod.Text = string.Empty;
            txt_descrizione.Text = string.Empty;
        }


        private void SetInsertMode()
        {
            //visibilità informazioni
            pnl_info.Visible = true;
            btn_aggiungi.Text = "Aggiungi";
            txt_codice.Visible = true;
            lbl_cod.Visible = false;

            // Rimozione dati controlli UI
            this.ClearData();
            SetFocus(txt_codice);
            dg_GQ.SelectedIndex = -1;
        }


        private void RefreshGridItem(DocsPAWA.DocsPaWR.Qualifica qualifica)
        {
            DataGridItem item = this.dg_GQ.SelectedItem;

            if (item != null)
            {
                item.Cells[GRID_COL_DESCRIZIONE].Text = qualifica.DESCRIZIONE;
            }
        }


        private void btn_aggiungi_Click(object sender, System.EventArgs e)
        {
            bool controlloCodice = true;

            if (OnInsertMode())
            {
                if (!this.txt_codice.Text.Contains("'"))
                    controlloCodice = true;
                else
                {
                    controlloCodice = false;
                    this.AlertJS("Attenzione, non inserire caratteri speciali nel codice");
                }
            }

            if (controlloCodice)
            {
                this.Save();
            }
        }


        private bool OnInsertMode()
        {
            return (btn_aggiungi.Text.Equals("Aggiungi"));
        }


        /// Cancellazione qualifica
        private void Delete(String idQualifica, int idAmministrazione)
        {
            DocsPAWA.DocsPaWR.ValidationResultInfo result = null;
            bool canDelete = true;

            if (canDelete)
            {

                result = this.DeleteQual(idQualifica, idAmministrazione);

                if (!result.Value)
                {
                    this.ShowValidationMessage(result);
                }
                else
                {
                    this.FillListGQ();
                    pnl_info.Visible = false;
                    this.ClearData();
                    dg_GQ.SelectedIndex = -1;
                }
            }
            else
            {
                this.ShowValidationMessage(result);
            }
        }

        
        /// Handler cancellazione elemento griglia
        private void DeleteItemCommand(object sender, DataGridCommandEventArgs e)
        {
            this.dg_GQ.SelectedIndex = e.Item.ItemIndex;
            string idQualifica = this.dg_GQ.SelectedItem.Cells[GRID_COL_ID].Text;
            int idAmministrazione = Convert.ToInt32(AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3"));
            this.Delete(idQualifica,idAmministrazione);
        }

        private void dg_GQ_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            string commandName = e.CommandName;
            pnl_info.Visible = true;
            if (commandName.Equals("Select"))
            {
                //visibilià informazioni
                pnl_info.Visible = true;
                btn_aggiungi.Text = "Modifica";
                txt_codice.Visible = false;
                lbl_cod.Visible = true;
                lbl_cod.Text = e.Item.Cells[GRID_COL_CODICE].Text;
                txt_descrizione.Text = e.Item.Cells[GRID_COL_DESCRIZIONE].Text;               
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

        private void AlertJS(string msg)
        {
            if (!this.Page.IsStartupScriptRegistered("alertJavaScript"))
            {
                string scriptString = "<SCRIPT>alert('" + msg.Replace("'", "\\'") + "');</SCRIPT>";
                this.Page.RegisterStartupScript("alertJavaScript", scriptString);
            }
        }



        #endregion

    }
}