using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Amministrazione.Gestione_Titolario
{
	/// <summary>
	/// Summary description for RuoliTitolario.
	/// </summary>
	public class RuoliTitolario : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Button btnOK;
		protected System.Web.UI.WebControls.Button btnCancel;
		protected System.Web.UI.WebControls.DataGrid grdRuoliTitolario;
	
		private const string TABLE_RUOLI_TITOLARIO="RUOLI_TITOLARIO";
		private const string TABLECOL_ID_RUOLO="ID_RUOLO";
		private const string TABLECOL_CODICE_RUOLO="CODICE_RUOLO";
		private const string TABLECOL_DESCRIZIONE_RUOLO="DESCRIZIONE_RUOLO";
		private const string TABLECOL_GRUPPOASSOCIATO="GRUPPOASSOCIATO";

		private const int GRIDCOL_ID_RUOLO=0;
		private const int GRIDCOL_CODICE_RUOLO=1;
		private const int GRIDCOL_DESCRIZIONE_RUOLO=2;
		protected System.Web.UI.WebControls.CheckBox chkSelectDeselectAll;
        protected System.Web.UI.WebControls.RadioButtonList rbAttivaDisattivaAll;
		protected System.Web.UI.HtmlControls.HtmlInputHidden txtUnloadMode;
		protected System.Web.UI.WebControls.Label lblItemsCounter;
        protected System.Web.UI.WebControls.DropDownList ddl_ricTipo;
        protected System.Web.UI.WebControls.TextBox txt_ricerca;
        protected System.Web.UI.WebControls.Label lbl_ricercaRuoli;
		private const int GRIDCOL_GRUPPOASSOCIATO=3;

        private bool changeIndex = false;
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
			this.chkSelectDeselectAll.CheckedChanged += new System.EventHandler(this.chkSelectDeselectAll_CheckedChanged);
			this.grdRuoliTitolario.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.grdRuoliTitolario_ItemCommand);
			this.grdRuoliTitolario.PreRender += new System.EventHandler(this.grdRuoliTitolario_PreRender);
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			this.Load += new System.EventHandler(this.Page_Load);
            this.rbAttivaDisattivaAll.SelectedIndexChanged += new EventHandler(rbAttivaDisattivaAll_SelectedIndexChanged);
		}

        void rbAttivaDisattivaAll_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataGridItemCollection gridItems = this.grdRuoliTitolario.Items;
            if (this.rbAttivaDisattivaAll.Items[0].Selected)
            {
                for (int j = 0; j < this.grdRuoliTitolario.Items.Count; j++)
                {
                    ((CheckBox)this.grdRuoliTitolario.Items[j].Cells[5].Controls[1]).Checked = true;
                    ((CheckBox)this.grdRuoliTitolario.Items[j].Cells[6].Controls[1]).Checked = false;
                }
            }
            else
            {
                if (this.rbAttivaDisattivaAll.Items[1].Selected)
                {
                    for (int j = 0; j < this.grdRuoliTitolario.Items.Count; j++)
                    {
                        ((CheckBox)this.grdRuoliTitolario.Items[j].Cells[5].Controls[1]).Checked = false;
                        ((CheckBox)this.grdRuoliTitolario.Items[j].Cells[6].Controls[1]).Checked = true;
                    }
                }
                else
                {
                    for (int j = 0; j < this.grdRuoliTitolario.Items.Count; j++)
                    {
                        ((CheckBox)this.grdRuoliTitolario.Items[j].Cells[5].Controls[1]).Checked = false;
                        ((CheckBox)this.grdRuoliTitolario.Items[j].Cells[6].Controls[1]).Checked = false;
                    }
                }
            }
        }
		#endregion
		
		#region Gestione eventi

		#region Gestione eventi pagina

		private void Page_Load(object sender, System.EventArgs e)
		{
			Response.Expires=-1;
            this.Page.MaintainScrollPositionOnPostBack = true;

            if (!IsPostBack)
            {
                this.RegisterClientEvents();
                this.FillDatiRuoliTitolario();
                this.RefreshItemsCounter();
            }
            else
            {
                verificaSelezione();
            }
        }

		#endregion

        private void verificaSelezione()
        {
			DataGridItemCollection gridItems=this.grdRuoliTitolario.Items;

            for (int j = 0; j < this.grdRuoliTitolario.Items.Count; j++)
            {
                if (((CheckBox)this.grdRuoliTitolario.Items[j].Cells[5].Controls[1]).Checked)
                {
                    if (((CheckBox)this.grdRuoliTitolario.Items[j].Cells[6].Controls[1]).Checked)
                    {
                        this.RegisterClientScript("alertMsg", "alert('Non è possibile Attivare e Disattivare contemporanemente la visibilità del titolario per un dato ruolo.');");
                        ((CheckBox)this.grdRuoliTitolario.Items[j].Cells[6].Controls[1]).Checked = false;
                    }
                }
            }
        }

        private void chkSelectDeselectAll_CheckedChanged(object sender, System.EventArgs e)
        {
            this.SelectAllCheck(((CheckBox)sender).Checked);
        }

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			// Aggiornamento dati ruoli del titolario
			SAAdminTool.DocsPaWR.EsitoOperazione[] retValue=this.ApplyChanges();

            if (retValue != null)
            {
                if (retValue.Length > 0)
                {
                    // Aggiornamento non andato a buon fine, 
                    // visualizzazione messaggio di errore
                    SAAdminTool.AdminTool.EsitoOperazioneViewerSessionManager.Items = retValue;

                    this.RegisterClientScript("ShowValidationResultDialog", "ShowValidationResultDialog();");
                }

                this.DisposeDialog();

                this.RegisterClientEvents();
                this.FillDatiRuoliTitolario();
                this.RefreshItemsCounter();

                //if (retValue.Length == 0)
                //{
                //    // Aggiornamento andato a buon fine,
                //    // chiusura maschera e deallocazione risorse
                //    //this.RegisterClientScript("messageOk", "alert('Visibilità modificata con successo');");
                //}
                //else
                //{

                //}
            }
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			// Chiusura maschera e deallocazione risorse
			this.DisposeDialog();
            this.RegisterClientScript("CloseWindow","CloseWindow();");
        }

		#endregion

		#region Gestione lettura dati

		private void FillDatiRuoliTitolario()
		{
			SAAdminTool.DocsPaWR.OrgRuoloTitolario[] ruoliTitolario=this.GetRuoliTitolario();
			
			this.FillGridDataTableRuoliTitolario(ruoliTitolario);

			ruoliTitolario=null;
		}

		private SAAdminTool.DocsPaWR.OrgRuoloTitolario[] GetRuoliTitolario()
		{
			SAAdminTool.DocsPaWR.OrgRuoloTitolario[] retValue=null;

			if (RuoliTitolarioSessionManager.GetRuoliTitolario()==null || changeIndex == true)
			{
				string idNodoTitolario=Request.QueryString["ID_NODO_TITOLARIO"];
				string idRegistro=Request.QueryString["ID_REGISTRO"];
                if(idNodoTitolario.Equals(""))
                    idNodoTitolario = Request.QueryString["ID_TITOLARIO"];

				Amministrazione.Manager.TitolarioManager titolarioManager=new Amministrazione.Manager.TitolarioManager();
                if (this.ddl_ricTipo.SelectedItem.Value.Equals("T"))
                {
                    //Session["reloadHT"] = true;
                    retValue = titolarioManager.GetRuoliInTitolario(idNodoTitolario, idRegistro, null, null);
                }
                else
                    retValue = titolarioManager.GetRuoliInTitolario(idNodoTitolario, idRegistro, txt_ricerca.Text, ddl_ricTipo.SelectedItem.Value);
				
                titolarioManager=null;
                changeIndex = false;
				RuoliTitolarioSessionManager.SetRuoliTitolario(retValue);
			}
			else
			{
				retValue=RuoliTitolarioSessionManager.GetRuoliTitolario();
			}

            if (retValue.Length == 0)
            {
                grdRuoliTitolario.Visible = false;
                lbl_ricercaRuoli.Text = "Nessun ruolo per questa ricerca!";
            }
            else
            {
                grdRuoliTitolario.Visible = true;
                lbl_ricercaRuoli.Text = "";
            }
			return retValue;
		}



		private DataTable CreateGridDataTableRuoliTitolario()
		{
			DataTable dt=new DataTable(TABLE_RUOLI_TITOLARIO);

			dt.Columns.Add(TABLECOL_ID_RUOLO,typeof(string));
			dt.Columns.Add(TABLECOL_CODICE_RUOLO,typeof(string));
			dt.Columns.Add(TABLECOL_DESCRIZIONE_RUOLO,typeof(string));
			dt.Columns.Add(TABLECOL_GRUPPOASSOCIATO,typeof(bool));

			return dt;
		}

		private void FillGridDataTableRuoliTitolario(SAAdminTool.DocsPaWR.OrgRuoloTitolario[] ruoliTitolario)
		{
			DataTable dt=this.CreateGridDataTableRuoliTitolario();

            if (ruoliTitolario != null)
            {
                foreach (SAAdminTool.DocsPaWR.OrgRuoloTitolario ruoloTitolario in ruoliTitolario)
                {
                    DataRow row = dt.NewRow();

                    row[TABLECOL_ID_RUOLO] = ruoloTitolario.ID;
                    row[TABLECOL_CODICE_RUOLO] = ruoloTitolario.Codice;
                    row[TABLECOL_DESCRIZIONE_RUOLO] = ruoloTitolario.Descrizione;
                    row[TABLECOL_GRUPPOASSOCIATO] = ruoloTitolario.Associato;

                    dt.Rows.Add(row);
                    row = null;
                }
            }

			this.grdRuoliTitolario.DataSource=dt;
			this.grdRuoliTitolario.DataBind();

			dt=null;
		}

		#endregion

		#region Gestione salvataggio dati

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private SAAdminTool.DocsPaWR.EsitoOperazione[] ApplyChanges()
		{
			// Reperimento dei ruoli modificati rispetto ai valori iniziali
            SAAdminTool.DocsPaWR.OrgRuoloTitolario[] changedRuoliTitolario = null;
            SAAdminTool.DocsPaWR.OrgRuoloTitolario[] changedRuoliTitolarioDisattivati = null;
            bool confermaCancellazione = false;
            string idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
            string idRegistro = Request.QueryString["ID_REGISTRO"];

			// Aggiornamento dei dati su database
			AmmUtils.WebServiceLink ws=new AmmUtils.WebServiceLink();
            bool AllTitolario = false;
            string idDaValutare = "";
            if (Request.QueryString["ID_NODO_TITOLARIO"] == "")
            {
                AllTitolario = true;
                idDaValutare = Request.QueryString["ID_TITOLARIO"];
                ArrayList ruoliAttivati = new ArrayList();
                ArrayList ruoliDisattivati = new ArrayList();
                SAAdminTool.DocsPaWR.OrgRuoloTitolario ruoloTitolario = null;

                Hashtable originalRuoliTitolario = this.GetOriginalRuoliTitolario(); 
                for (int j = 0; j < this.grdRuoliTitolario.Items.Count; j++)
                {
 					string idRuoloTitolario = this.grdRuoliTitolario.Items[j].Cells[GRIDCOL_ID_RUOLO].Text;

					// Viene verificato se l'associazione del ruolo è stata modificata
                    if (originalRuoliTitolario.ContainsKey(idRuoloTitolario))
                    {
                        ruoloTitolario = (SAAdminTool.DocsPaWR.OrgRuoloTitolario)originalRuoliTitolario[idRuoloTitolario];
                        if (((CheckBox)this.grdRuoliTitolario.Items[j].Cells[5].Controls[1]).Checked)
                            ruoliAttivati.Add(ruoloTitolario);
                        else
                            if (((CheckBox)this.grdRuoliTitolario.Items[j].Cells[6].Controls[1]).Checked)
                                ruoliDisattivati.Add(ruoloTitolario);
						
                        ruoloTitolario=null;
                    }
                }

                if (ruoliAttivati.Count > 0)
                {
                    changedRuoliTitolario = new SAAdminTool.DocsPaWR.OrgRuoloTitolario[ruoliAttivati.Count];
                    ruoliAttivati.CopyTo(changedRuoliTitolario);
                    ruoliAttivati.Clear();
                    ruoliAttivati = null;
                }

                if (ruoliDisattivati.Count > 0)
                {
                    changedRuoliTitolarioDisattivati = new SAAdminTool.DocsPaWR.OrgRuoloTitolario[ruoliDisattivati.Count];
                    ruoliDisattivati.CopyTo(changedRuoliTitolarioDisattivati);
                    ruoliDisattivati.Clear();
                    ruoliDisattivati = null;
                }
            }
            else
            {
                idDaValutare = Request.QueryString["ID_NODO_TITOLARIO"];
                changedRuoliTitolario = this.GetChangedRuoliTitolario();
            }

            SAAdminTool.DocsPaWR.EsitoOperazione[] retValue = null;
            retValue = ws.AmmUpdateRuoliTitolario(idDaValutare, idAmm, AllTitolario, changedRuoliTitolario, changedRuoliTitolarioDisattivati, idRegistro);
            if (!AllTitolario)
                this.UpdateRuoliTitolarioOnSession(changedRuoliTitolario, retValue);

            return retValue;
		}

		private void UpdateRuoliTitolarioOnSession(SAAdminTool.DocsPaWR.OrgRuoloTitolario[] changedRuoliTitolario,
												   SAAdminTool.DocsPaWR.EsitoOperazione[] ruoliNonAggiornati)
		{
			Hashtable htChangedRuoliTitolario=new Hashtable();
			
			foreach (SAAdminTool.DocsPaWR.OrgRuoloTitolario ruoloTitolario in changedRuoliTitolario)
				htChangedRuoliTitolario.Add(Convert.ToInt32(ruoloTitolario.ID),ruoloTitolario);

			foreach (SAAdminTool.DocsPaWR.EsitoOperazione item in ruoliNonAggiornati)
			{
				if (htChangedRuoliTitolario.ContainsKey(item.Codice))
				{
                    SAAdminTool.DocsPaWR.OrgRuoloTitolario ruoloTitolario =
						(SAAdminTool.DocsPaWR.OrgRuoloTitolario) htChangedRuoliTitolario[item.Codice];

					ruoloTitolario.Associato=!ruoloTitolario.Associato;
				}
			}

			htChangedRuoliTitolario.Clear();
			htChangedRuoliTitolario=null;
		}

		/// <summary>
		/// Creazione di un oggetto HashTable contenente tutti 
		/// gli oggetti "OrgRuoloTitolario" con i valori iniziali
		/// </summary>
		/// <returns></returns>
		private Hashtable GetOriginalRuoliTitolario()
		{
			Hashtable retValue=new Hashtable();

            SAAdminTool.DocsPaWR.OrgRuoloTitolario[] ruoliTitolario = this.GetRuoliTitolario();
            
			if (ruoliTitolario!=null)
			{
				foreach (SAAdminTool.DocsPaWR.OrgRuoloTitolario ruoloTitolario in ruoliTitolario)
				{
					retValue.Add(ruoloTitolario.ID,ruoloTitolario);
				}
			}

			ruoliTitolario=null;

			return retValue;
		}

		/// <summary>
		/// Restituzione di un array di oggetti "OrgRuoloTitolario"
		/// contenente i soli ruoli modificati (inseriti o cancellati)
		/// </summary>
		/// <returns></returns>
		private SAAdminTool.DocsPaWR.OrgRuoloTitolario[] GetChangedRuoliTitolario()
		{
			Hashtable originalRuoliTitolario=this.GetOriginalRuoliTitolario();

			ArrayList changedRuoli=new ArrayList();

			DataGridItemCollection gridItems=this.grdRuoliTitolario.Items;

            SAAdminTool.DocsPaWR.OrgRuoloTitolario ruoloTitolario = null;
			string idRuoloTitolario=string.Empty;
			bool isChecked=false;

			foreach (DataGridItem gridItem in gridItems)
			{
				CheckBox checkBox=
					gridItem.Cells[GRIDCOL_GRUPPOASSOCIATO].Controls[0].FindControl("chkRuoloAttivo") as CheckBox;
				
				if (checkBox!=null)
				{
					idRuoloTitolario=gridItem.Cells[GRIDCOL_ID_RUOLO].Text;
					isChecked=checkBox.Checked;					

					// Viene verificato se l'associazione del ruolo è stata modificata
					if (originalRuoliTitolario.ContainsKey(idRuoloTitolario))
					{
						ruoloTitolario=(SAAdminTool.DocsPaWR.OrgRuoloTitolario) originalRuoliTitolario[idRuoloTitolario];
						
						// Se il ruolo risulta modificato, viene inserito nell'hashtable
						if (!ruoloTitolario.Associato.Equals(isChecked))
						{
							ruoloTitolario.Associato=isChecked;
							changedRuoli.Add(ruoloTitolario);
						}
				
						ruoloTitolario=null;
					}
				}
			}

			gridItems=null;

            SAAdminTool.DocsPaWR.OrgRuoloTitolario[] retValue = new SAAdminTool.DocsPaWR.OrgRuoloTitolario[changedRuoli.Count];
			changedRuoli.CopyTo(retValue);

			changedRuoli.Clear();
			changedRuoli=null;

			return retValue;
		}

		private SAAdminTool.DocsPaWR.OrgRuoloTitolario CloneObjRuoloTitolario(SAAdminTool.DocsPaWR.OrgRuoloTitolario originalRuoloTitolario)
		{
            SAAdminTool.DocsPaWR.OrgRuoloTitolario retValue = new SAAdminTool.DocsPaWR.OrgRuoloTitolario();
			
			retValue.ID=originalRuoloTitolario.ID;
			retValue.Codice=originalRuoloTitolario.Codice;
			retValue.Descrizione=originalRuoloTitolario.Descrizione;
			retValue.Associato=originalRuoloTitolario.Associato;

			return retValue;            
		}

		/// <summary>
		/// Chiusura maschera e deallocazione risorse
		/// </summary>
		private void DisposeDialog()
		{
			// Rimozione chiavi di sessione utilizzate dalla maschera
			this.ClearDialogSessionKeys();

			//this.RegisterClientScript("CloseWindow","CloseWindow();");
		}

		/// <summary>
		/// Rimozioni delle chiavi di sessione utilizzate remporaneamente dalla maschera
		/// </summary>
		private void ClearDialogSessionKeys()
		{
			RuoliTitolarioSessionManager.RemoveRuoliTitolario();
//
//			if (Session[SESSION_KEY_RUOLI_TITOLARIO]!=null)
//				Session.Remove(SESSION_KEY_RUOLI_TITOLARIO);
		}

		#endregion

		#region Gestione JavaScript

		/// <summary>
		/// Registrazione script client
		/// </summary>
		/// <param name="scriptKey"></param>
		/// <param name="scriptValue"></param>
		private void RegisterClientScript(string scriptKey,string scriptValue)
		{
			if(!this.Page.IsStartupScriptRegistered(scriptKey))
			{
				string scriptString = "<SCRIPT>" + scriptValue + "</SCRIPT>";
				this.Page.RegisterStartupScript(scriptKey, scriptString);
			}
		}

		/// <summary>
		/// Associazione funzioni agli handler javascript dei controlli
		/// </summary>
		private void RegisterClientEvents()
		{
			this.btnOK.Attributes.Add("onClick","VisualizzaAttendi();");
		}

		#endregion
		
		private void RefreshItemsCounter()
		{
            SAAdminTool.DocsPaWR.OrgRuoloTitolario[] ruoliTitolario = this.GetRuoliTitolario();
				
			int ruoliAttivi=0;

			foreach (SAAdminTool.DocsPaWR.OrgRuoloTitolario ruoloTitolario in ruoliTitolario)
				if (ruoloTitolario.Associato)
					ruoliAttivi++;

            string itemsCounterDescription = "Ruoli disponibili: " + ruoliTitolario.Length.ToString();
            if (Request.QueryString["ID_NODO_TITOLARIO"] != "")
				itemsCounterDescription += " - Ruoli attivi: " + ruoliAttivi.ToString();

			ruoliTitolario=null;

			this.lblItemsCounter.Text=itemsCounterDescription;
		}

        /// <summary>
        /// Gestione selezione / deselezione di tutti i checkbox colonna associa
        /// </summary>
        /// <param name="value"></param>
        private void SelectAllCheck(bool value)
        {
            DataGridItemCollection gridItems = this.grdRuoliTitolario.Items;

            foreach (DataGridItem gridItem in gridItems)
            {
                CheckBox checkBox =
                    gridItem.Cells[GRIDCOL_GRUPPOASSOCIATO].Controls[0].FindControl("chkRuoloAttivo") as CheckBox;

                if (checkBox != null)
                    checkBox.Checked = value;
            }
        }

        /// <summary>
        /// Selezione icona "Estendi visibilità a tutti i nodi figli"
        /// sul datagrid
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void grdRuoliTitolario_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            if (e.CommandName.Equals("ExtDown"))
            {
                CheckBox checkBox =
                    e.Item.Cells[GRIDCOL_GRUPPOASSOCIATO].Controls[0].FindControl("chkRuoloAttivo") as CheckBox;

                if (checkBox != null && !checkBox.Checked)
                {
                    checkBox.Checked = true;

                    // Aggiornamento dati ruoli del titolario
                    SAAdminTool.DocsPaWR.EsitoOperazione[] retValue = this.ApplyChanges();

                    if (retValue != null)
                    {
                        if (retValue.Length == 1 && retValue[0].Codice == 0)
                        {
                            // Aggiornamento andato a buon fine,
                            // deallocazione risorse
                            this.ExtendToChildNodes(e.Item.Cells[0].Text, true);
                            this.DisposeDialog();
                        }
                        else
                        {
                            // Aggiornamento non andato a buon fine, 
                            // visualizzazione messaggio di errore
                            string validationMessage = string.Empty;

                            foreach (SAAdminTool.DocsPaWR.EsitoOperazione esito in retValue)
                            {
                                if (validationMessage != string.Empty)
                                    validationMessage += @"\n";

                                validationMessage += " - " + esito.Descrizione;
                            }

                            if (validationMessage != string.Empty)
                                validationMessage = "Non è stato possibile aggiornare i seguenti ruoli: " +
                                    @"\n" + @"\n" + validationMessage +
                                    @"\n" + @"\nLa visibilità non è stata estesa ai nodi figli";

                            checkBox.Checked = false;

                            this.RegisterClientScript("ValidationMessage", "alert('" + validationMessage + "');");
                        }
                    }
                }
                else
                    this.ExtendToChildNodes(e.Item.Cells[0].Text, true);
            }


            if (e.CommandName.Equals("ExtDownNo"))
            {
                CheckBox checkBox =
                    e.Item.Cells[GRIDCOL_GRUPPOASSOCIATO].Controls[0].FindControl("chkRuoloAttivo") as CheckBox;

                if (checkBox != null && checkBox.Checked)
                    checkBox.Checked = false;
                this.ExtendToChildNodes(e.Item.Cells[0].Text, false);
                this.DisposeDialog();
            }
        }

		/// <summary>
		/// Estende o elimina la visibilità ad un dato ruolo per tutti i nodi figli di un dato nodo di titolario
		/// </summary>
		/// <param name="idRuolo"></param>
		private void ExtendToChildNodes(string idRuolo, bool check)
		{
            Hashtable table = this.GetOriginalRuoliTitolario();
            
            if (table.ContainsKey(idRuolo))
            {
                SAAdminTool.DocsPaWR.OrgRuoloTitolario ruoloTitolario = (SAAdminTool.DocsPaWR.OrgRuoloTitolario)table[idRuolo];
   
                AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();

                string idNodoTitolario = Request.QueryString["ID_NODO_TITOLARIO"];
                if (idNodoTitolario.Equals(""))
                    idNodoTitolario = Request.QueryString["ID_TITOLARIO"];
                string idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
                string idRegistro = Request.QueryString["ID_REGISTRO"];

                SAAdminTool.DocsPaWR.EsitoOperazione esito = ws.AmmExtendToChildNodes(idNodoTitolario, ruoloTitolario, idAmm, idRegistro, check);

                if (esito.Codice > 0)
                    this.RegisterClientScript("alertExtend", "alert('" + esito.Descrizione.Replace("'", "\\'") + "');");
                else
                    this.RegisterClientScript("alertExtend", "alert('Visibilità modificata con successo');");
            }
		}

		/// <summary>
		/// Pre render grdRuoliTitolario
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void grdRuoliTitolario_PreRender(object sender, System.EventArgs e)
		{
			for(int i=0;i<this.grdRuoliTitolario.Items.Count;i++)
			{
				this.grdRuoliTitolario.Items[i].Cells[4].Attributes.Add ("onclick","VisualizzaAttendi();");
                this.grdRuoliTitolario.Items[i].Cells[5].Attributes.Add("onclick", "VisualizzaAttendi();");
                if (Request.QueryString["ID_NODO_TITOLARIO"] == "")
                {
                    //this.grdRuoliTitolario.Items[i].Cells[4].Visible = false;
                    this.grdRuoliTitolario.Columns[4].Visible = false;
                    this.grdRuoliTitolario.Columns[3].Visible = false;
                    this.grdRuoliTitolario.Columns[5].Visible = true;
                    this.grdRuoliTitolario.Columns[6].Visible = true;
                    this.grdRuoliTitolario.Columns[7].Visible = false;
                    this.chkSelectDeselectAll.Visible = false;
                }
                else
                {
                    this.rbAttivaDisattivaAll.Visible = false;
                    this.grdRuoliTitolario.Columns[5].Visible = false;
                    this.grdRuoliTitolario.Columns[6].Visible = false;
                    this.grdRuoliTitolario.Columns[7].Visible = true;
                }
			}
		}

        protected void btn_find_Click(object sender, System.EventArgs e)
        {
            changeIndex = true;
            this.FillDatiRuoliTitolario();
            this.RefreshItemsCounter();
            
        }

        protected void ddl_ricTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.ddl_ricTipo.SelectedItem.Value.Equals("T"))
            {
                txt_ricerca.Text = string.Empty;
                txt_ricerca.BackColor = System.Drawing.Color.Gainsboro;
                txt_ricerca.ReadOnly = true;
            }
            else
            {
                txt_ricerca.BackColor = System.Drawing.Color.White;
                txt_ricerca.ReadOnly = false;
            }
        }


		/// <summary>
		/// Classe per la gestione dei dati in sessione
		/// relativamente alla maschera "RuoliTitolario.aspx"
		/// </summary>
		public sealed class RuoliTitolarioSessionManager
		{
			private const string SESSION_KEY_RUOLI_TITOLARIO="GestioneTitolario.RuoliTitolario";
			
			public static void SetRuoliTitolario(SAAdminTool.DocsPaWR.OrgRuoloTitolario[] ruoliTitolario)
			{
				HttpContext.Current.Session[SESSION_KEY_RUOLI_TITOLARIO]=ruoliTitolario;
			}

			public static SAAdminTool.DocsPaWR.OrgRuoloTitolario[] GetRuoliTitolario()
			{
				return HttpContext.Current.Session[SESSION_KEY_RUOLI_TITOLARIO] as SAAdminTool.DocsPaWR.OrgRuoloTitolario[];
			}

			public static void RemoveRuoliTitolario()
			{
				HttpContext.Current.Session.Remove(SESSION_KEY_RUOLI_TITOLARIO);
			}
		}

	}
}
