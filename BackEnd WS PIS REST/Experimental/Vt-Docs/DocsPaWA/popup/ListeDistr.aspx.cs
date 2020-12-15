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

namespace DocsPAWA.popup.RubricaDocsPA
{
	/// <summary>
	/// Summary description for ListeDistr.
	/// </summary>
	public class ListeDistr : DocsPAWA.CssPage
	{
        
		protected System.Web.UI.WebControls.Panel Panel1;
		protected System.Web.UI.WebControls.Panel Panel2;
        protected System.Web.UI.WebControls.Panel Panel3;
        protected System.Web.UI.WebControls.Label Label1;
		protected System.Web.UI.WebControls.DataGrid dg_1;
		protected System.Web.UI.WebControls.Label Label2;
		protected System.Web.UI.WebControls.TextBox txt_descrizione;
		protected System.Web.UI.WebControls.DataGrid dg_2;
		protected System.Web.UI.WebControls.ImageButton imgBtn_descrizione;
		protected DataSet dsListe;
		protected DataSet dsCorrispondenti;
		protected System.Web.UI.WebControls.Label Label3;
		protected System.Web.UI.WebControls.TextBox txt_nomeLista;
		protected string idUtente;
		protected System.Web.UI.HtmlControls.HtmlGenericControl DivDGElencoListe;
		protected System.Web.UI.WebControls.ImageButton btn_nuova;
		protected System.Web.UI.WebControls.ImageButton btn_annulla;
		protected System.Web.UI.WebControls.ImageButton btn_salva;
		protected System.Web.UI.HtmlControls.HtmlGenericControl DivDGCorrispondenti;
		protected System.Web.UI.HtmlControls.HtmlGenericControl DivDGCorrLista;
		protected System.Web.UI.WebControls.ImageButton imgBtn_addCorr;
		protected System.Web.UI.WebControls.Label Label4;
        protected System.Web.UI.WebControls.Label Label5;
		protected System.Web.UI.WebControls.TextBox txt_codiceLista;
		protected System.Web.UI.WebControls.TextBox txt_codiceCorr;
		protected System.Web.UI.HtmlControls.HtmlInputHidden txt_confirmDel;
		protected System.Web.UI.HtmlControls.HtmlInputHidden txt_confirmMod;
        protected System.Web.UI.WebControls.RadioButtonList rbl_share;


		private ScrollKeeper scrollDg_1 = new ScrollKeeper();
				
		private void Page_Load(object sender, System.EventArgs e)
		{
			idUtente = UserManager.getInfoUtente(this).idPeople;

			//Imposto lo ScrollKeeper per il DataGrid dell'elencoListe
			/*
			HtmlForm frm = (HtmlForm) this.FindControl("Form1");
			scrollDg_1.WebControl = "DivDGElencoListe";
			frm.Controls.Add(scrollDg_1);
			*/

			//Viene controllata la presenza in sessione di una eventuale selezione di corrispondenti fatta da rubrica
			//ATTENZIONE : Questa la variabile in sessione ("selCorrDaRubrica") deve essere rimossa in ogni metodo aggiunto 
			//in questa pagina, la prima cosa che un nuovo metodo deve fare è appunto (Session.Remove("selCorrDaRubrica")
			//Questo per risolvere il classico problema del doppio passaggio dal PageLoad
			if(Session["selCorrDaRubrica"] != null)
				addCorrSelDaRubrica((DocsPAWA.DocsPaWR.ElementoRubrica[]) Session["selCorrDaRubrica"]);

			//Gestione del tasto invio
			DocsPAWA.Utils.DefaultButton (this, ref txt_codiceCorr, ref imgBtn_addCorr);
			DocsPAWA.Utils.DefaultButton (this, ref txt_nomeLista, ref btn_salva);
			DocsPAWA.Utils.DefaultButton (this, ref txt_descrizione, ref imgBtn_addCorr);
			
			if(!IsPostBack)
			{
				Panel1.Visible = true;
				Panel2.Visible = false;
                Panel3.Visible = false;
				
				//dsListe = UserManager.getListePerModificaUt(this);
                dsListe = UserManager.getListePerRuoloUt(this);
				dg_1.DataSource = dsListe;
				dg_1.DataBind();

				DataTable dt = new DataTable();
				dt.Columns.Add("ID_DPA_CORR");
				dt.Columns.Add("VAR_DESC_CORR");
                dt.Columns.Add("VAR_COD_RUBRICA");
                dt.Columns.Add("CHA_TIPO_IE");
                dt.Columns.Add("CHA_DISABLED_TRASM");
				dsCorrispondenti = new DataSet();
				dsCorrispondenti.Tables.Add(dt);
				ViewState.Add("dsCorr",dsCorrispondenti);
                
				btn_salva.Visible = false;
                Panel3.Visible = false;
				this.imgBtn_descrizione.Attributes["onClick"] = "_ApriRubrica();";
				this.btn_nuova.Attributes["onClick"] = "confirmMod();";
			}
            if (!IsPostBack)
            {
                rbl_share.Items[0].Text = rbl_share.Items[0].Text.Replace("@usr@", UserManager.getUtente(this).descrizione);
                rbl_share.Items[1].Text = rbl_share.Items[1].Text.Replace("@grp@", UserManager.getRuolo(this).descrizione);
            }		
		}

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
			this.dg_1.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dg_1_PageIndexChanged);
			this.dg_1.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_1_EditCommand);
			this.dg_1.SelectedIndexChanged += new System.EventHandler(this.dg_1_SelectedIndexChanged);
			this.txt_codiceCorr.TextChanged += new System.EventHandler(this.txt_codiceCorr_TextChanged);
			this.imgBtn_addCorr.Click += new System.Web.UI.ImageClickEventHandler(this.imgBtn_addCorr_Click);
			this.dg_2.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dg_2_PageIndexChanged);
			this.dg_2.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_2_EditCommand);
			this.btn_nuova.Click += new System.Web.UI.ImageClickEventHandler(this.btn_nuova_Click);
			this.btn_salva.Click += new System.Web.UI.ImageClickEventHandler(this.btn_salva_Click);
			this.btn_annulla.Click += new System.Web.UI.ImageClickEventHandler(this.btn_annulla_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void dg_1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			Session.Remove("selCorrDaRubrica");
			dg_2.CurrentPageIndex = 0;

			btn_salva.Visible = true;
            Panel3.Visible = true;
			
			Panel2.Visible = true;
			Label2.Visible = true;
			Label3.Visible = true;
			txt_descrizione.Visible = true;
			txt_descrizione.Text = "";
			txt_codiceCorr.Visible = true;
			txt_codiceCorr.Text = "";
			txt_nomeLista.Visible = true;
			txt_nomeLista.Text = dg_1.SelectedItem.Cells[1].Text;
			txt_codiceLista.Text = UserManager.getCodiceLista(this, dg_1.SelectedItem.Cells[0].Text);
			txt_codiceLista.Enabled = false;

			imgBtn_descrizione.Visible = true;
			imgBtn_addCorr.Visible = true;

			ViewState.Add("dsCorr",UserManager.getCorrispondentiLista(this,dg_1.SelectedItem.Cells[0].Text));
			dg_2.DataSource = ((DataSet) ViewState["dsCorr"]);
			dg_2.DataBind();
			dg_2.Visible = true;

            if (UserManager.getRuoloOrUserLista(this, dg_1.SelectedItem.Cells[0].Text) != null)
            {
                this.rbl_share.SelectedValue = "grp";
            }
            else
            {
                this.rbl_share.SelectedValue = "usr";
            }
			
			//SetFocus(txt_codiceLista);
		}

		private void dg_1_EditCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			if( ((System.Web.UI.HtmlControls.HtmlInputHidden) this.FindControl("txt_confirmDel")).Value == "si")
			{
				Session.Remove("selCorrDaRubrica");
				
				int rigaSelezionata = e.Item.ItemIndex;
				dg_1.CurrentPageIndex = 0;
				UserManager.deleteListaDistribuzione(this,dg_1.Items[rigaSelezionata].Cells[0].Text);
                // dsListe = UserManager.getListePerModificaUt(this);
                dsListe = UserManager.getListePerRuoloUt(this);
				dg_1.DataSource = dsListe;
				dg_1.DataBind();
				Panel2.Visible = false;
				dg_1.SelectedIndex = -1;
				((System.Web.UI.HtmlControls.HtmlInputHidden) this.FindControl("txt_confirmDel")).Value = "";
				btn_salva.Visible = false;
                Panel3.Visible = false;
			}
		}
		
		private void btn_salva_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			Session.Remove("selCorrDaRubrica");
			
			if(dg_1.SelectedIndex != -1)
			{
				if(txt_nomeLista.Text == "" || txt_nomeLista.Text.Trim() == ""|| ((DataSet) ViewState["dsCorr"]).Tables[0].Rows.Count == 0 || txt_codiceLista.Text == "" || txt_codiceLista.Text.Trim() == "")
				{
					RegisterStartupScript("Errore inserimento","<script>alert('Inserire il nome della lista, il codice e aggiungere i corrispondenti !')</script>");
					//SetFocus(txt_codiceLista);
					return;
				}

				//Modifica di una lista esistente
				if(dg_1.SelectedIndex == -1)
				{
					RegisterStartupScript("Modifica effettuata","<script>alert('Selezionare una lista !');</script>");
					return;
				}
				string idLista = dg_1.SelectedItem.Cells[0].Text;
				string nomeLista = txt_nomeLista.Text.Replace("'","''");
				string codiceLista = txt_codiceLista.Text.Replace("'","''");

                if (nomeLista != UserManager.getNomeLista(this, UserManager.getCodiceLista(this, idLista), UserManager.getInfoUtente().idAmministrazione))
				{
					if(!UserManager.isUniqueNomeLista(this,nomeLista, UserManager.getInfoUtente().idAmministrazione))
					{
						RegisterStartupScript("Modifica non effettuata","<script>alert('Nome lista non univoco !');</script>");
						SetFocus(txt_nomeLista);
						return;	
					}										
				}

                if (this.rbl_share.SelectedValue == "usr")
                {
                    UserManager.modificaListaUser(this, ((DataSet)ViewState["dsCorr"]), idLista, nomeLista, codiceLista, idUtente);
                }
                else
                {
                    string idGruppo = UserManager.getRuolo().idGruppo;
                    UserManager.modificaListaGruppo(this, ((DataSet)ViewState["dsCorr"]), idLista, nomeLista, codiceLista, idGruppo);
                }
				RegisterStartupScript("Modifica effettuata","<script>alert('Operazione effettuata con successo!');</script>");
				dg_1.SelectedIndex = -1;
				Panel2.Visible = false;
				// dsListe = UserManager.getListePerModificaUt(this);
                dsListe = UserManager.getListePerRuoloUt(this);
				dg_1.DataSource = dsListe;
				dg_1.DataBind();
				txt_confirmMod.Value = "";
			}
			else
			{
				//Inserimento di una nuova lista
				if(txt_nomeLista.Text == "" || txt_nomeLista.Text.Trim() == "" || ((DataSet) ViewState["dsCorr"]).Tables[0].Rows.Count == 0 || txt_codiceLista.Text == "" || txt_codiceLista.Text.Trim() == "")
				{
					RegisterStartupScript("Errore inserimento","<script>alert('Inserire il nome della lista, il codice e aggiungere i corrispondenti !')</script>");
					SetFocus(txt_codiceLista);
					return;
				}
				else
				{
					string nomeLista = txt_nomeLista.Text.Replace("'","''");
	                string codiceLista = txt_codiceLista.Text.Replace("'","''");

					if(!UserManager.isUniqueCodLista(this,codiceLista,UserManager.getInfoUtente().idAmministrazione))
					{
						RegisterStartupScript("Modifica non effettuata","<script>alert('Codice lista non univoco !');</script>");
						SetFocus(txt_codiceLista);
						return;	
					}
					if(!UserManager.isUniqueNomeLista(this,nomeLista, UserManager.getInfoUtente().idAmministrazione))
					{
						RegisterStartupScript("Modifica non effettuata","<script>alert('Nome lista non univoco !');</script>");
						SetFocus(txt_nomeLista);
						return;	
					}

                    string idGruppo = UserManager.getRuolo().idGruppo;
                    if (this.rbl_share.Items[0].Selected)
                    {
                        UserManager.salvaLista(this, ((DataSet)ViewState["dsCorr"]), nomeLista, codiceLista, idUtente, UserManager.getInfoUtente(this).idAmministrazione, "no");
                    }
                    else
                    {
                        UserManager.salvaLista(this, ((DataSet)ViewState["dsCorr"]), nomeLista, codiceLista, idGruppo, UserManager.getInfoUtente(this).idAmministrazione, "yes");
                    }
					RegisterStartupScript("Inserimento effettuato","<script>alert('Operazione effettuata con successo!');</script>");
					dg_1.SelectedIndex = -1;
					Panel2.Visible = false;	
					// dsListe = UserManager.getListePerModificaUt(this);
                    dsListe = UserManager.getListePerRuoloUt(this);
					dg_1.DataSource = dsListe;
					dg_1.DataBind();
					txt_confirmMod.Value = "";
				}
			}
			btn_salva.Visible = false;
            Panel3.Visible = false;
		}

		private void btn_annulla_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			Session.Remove("selCorrDaRubrica");
			
			string script = "<script>window.top.close();</script>";
			RegisterStartupScript("chiudiFinestra",script);
		}

		private void dg_2_EditCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			if( ((System.Web.UI.HtmlControls.HtmlInputHidden) this.FindControl("txt_confirmDel")).Value == "si")
			{
				Session.Remove("selCorrDaRubrica");
				int rigaSelezionata = (dg_2.PageSize * dg_2.CurrentPageIndex) +  e.Item.ItemIndex;
                if (e.Item.ItemIndex == 0 && dg_2.Items.Count == 1 && dg_2.CurrentPageIndex != 0)
				    dg_2.CurrentPageIndex = dg_2.CurrentPageIndex - 1;
				if(dg_1.SelectedIndex != -1)
				{
					((DataSet) ViewState["dsCorr"]).Tables[0].Rows[rigaSelezionata].Delete();
					((DataSet) ViewState["dsCorr"]).Tables[0].AcceptChanges();
					string idLista = dg_1.SelectedItem.Cells[0].Text;
					UserManager.modificaLista(this,((DataSet) ViewState["dsCorr"]),idLista);
					dg_2.DataSource = ((DataSet) ViewState["dsCorr"]);
					dg_2.DataBind();
					dg_2.Visible  = true;
					if(dg_2.Items.Count == 0)
					{
						UserManager.deleteListaDistribuzione(this,dg_1.SelectedItem.Cells[0].Text);
						// dsListe = UserManager.getListePerModificaUt(this);
                        dsListe = UserManager.getListePerRuoloUt(this);
						dg_1.DataSource = dsListe;
						dg_1.DataBind();
						Panel2.Visible = false;
						btn_salva.Visible = false;
                        Panel3.Visible = false;
						dg_1.SelectedIndex = -1;				
					}
				}
				else
				{
					((DataSet) ViewState["dsCorr"]).Tables[0].Rows[rigaSelezionata].Delete();
					dg_2.DataSource = ((DataSet) ViewState["dsCorr"]);
					dg_2.DataBind();
					dg_2.Visible  = true;				
				}
				((System.Web.UI.HtmlControls.HtmlInputHidden) this.FindControl("txt_confirmDel")).Value = "";
			}
		}

		private void SetFocus(System.Web.UI.Control ctrl)
		{
				string s = "<SCRIPT language='javascript'>document.getElementById('" + ctrl.ID + "').focus();</SCRIPT>";
				RegisterStartupScript("focus", s);
		}
		
		private void btn_nuova_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			if( ((System.Web.UI.HtmlControls.HtmlInputHidden) this.FindControl("txt_confirmMod")).Value == "")
			{
				Session.Remove("selCorrDaRubrica");
				btn_salva.Visible = true;
                Panel3.Visible = true;
				dg_1.SelectedIndex = -1;
				dg_2.SelectedIndex = -1;
				dg_1.CurrentPageIndex = 0;
				dg_2.CurrentPageIndex = 0;
				txt_descrizione.Text = "";
				txt_descrizione.Visible = true;
				txt_nomeLista.Text = "";
				txt_nomeLista.Visible = true;
				txt_nomeLista.ReadOnly = false;
				txt_nomeLista.BackColor = System.Drawing.Color.White;
				txt_codiceCorr.Text = "";
				txt_codiceCorr.Visible = true;
				txt_codiceLista.Text = "";
				txt_codiceLista.Enabled = true;
				Label2.Visible = true;
				Label3.Visible = true;
				imgBtn_descrizione.Visible = true;
				imgBtn_addCorr.Visible = true;
	            
				Panel1.Visible = true;
				Panel2.Visible = true;
				
				((DataSet) ViewState["dsCorr"]).Tables[0].Rows.Clear();
				dg_2.DataSource = ((DataSet) ViewState["dsCorr"]);
				dg_2.DataBind();
				dg_2.Visible = true;			
				
                SetFocus(txt_codiceLista);		
			}
		}

		private void imgBtn_addCorr_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			Session.Remove("selCorrDaRubrica");
			
			DocsPaWR.Corrispondente corr = null;
			//corr = UserManager.getCorrispondente(this,txt_codiceCorr.Text,true);
            corr = UserManager.getCorrispondenteRubrica(this, txt_codiceCorr.Text, DocsPAWA.DocsPaWR.RubricaCallType.CALLTYPE_LISTE_DISTRIBUZIONE);
			
			if(txt_codiceCorr.Visible)
				SetFocus(txt_codiceCorr);
			
			if(corr != null)
			{
				if(!verificaDuplicazioneCorr(corr))
				{
					DataRow dr = ((DataSet) ViewState["dsCorr"]).Tables[0].NewRow();
					dr[0] = corr.systemId;
					dr[1] = corr.descrizione;
                    if (corr.disabledTrasm)
                        dr[4] = "1";
                    else
                        dr[4] = "0";
					((DataSet) ViewState["dsCorr"]).Tables[0].Rows.Add(dr);
					dg_2.DataSource = ((DataSet) ViewState["dsCorr"]);
					dg_2.DataBind();
					txt_codiceCorr.Text = "";
					txt_descrizione.Text = "";
					((System.Web.UI.HtmlControls.HtmlInputHidden) this.FindControl("txt_confirmMod")).Value = "si";
				}
				else
				{
					txt_codiceCorr.Text = "";
					txt_descrizione.Text = "";					
				}
			}		
		}

		private void txt_codiceCorr_TextChanged(object sender, System.EventArgs e)
		{
			//Session.Remove("selCorrDaRubrica");
			
			DocsPaWR.Corrispondente corr = null;
            if (txt_codiceCorr.Text != "")
            {
                //corr = UserManager.getCorrispondente(this,txt_codiceCorr.Text,true);
                corr = UserManager.getCorrispondenteRubrica(this, txt_codiceCorr.Text, DocsPAWA.DocsPaWR.RubricaCallType.CALLTYPE_LISTE_DISTRIBUZIONE);
            }
			if(corr ==  null)
			{
				if(txt_codiceCorr.Text != "")
					RegisterStartupScript("Errore Corrispondente","<script>alert('Codice corrispondente errato !')</script>");

				txt_descrizione.Text = "";

				if(txt_codiceCorr.Visible)
					SetFocus(txt_codiceCorr);
			}
			else
			{
				txt_descrizione.Text = corr.descrizione;
				if(txt_codiceCorr.Visible)
					SetFocus(txt_codiceCorr);
			}
		}

		private bool verificaDuplicazioneCorr(DocsPAWA.DocsPaWR.Corrispondente corr)
		{
            if (ViewState["dsCorr"] != null && ((DataSet)ViewState["dsCorr"]).Tables[0] != null && ((DataSet)ViewState["dsCorr"]).Tables[0].Rows != null && ((DataSet)ViewState["dsCorr"]).Tables[0].Rows.Count != 0)
            {
                for (int i = 0; i < ((DataSet)ViewState["dsCorr"]).Tables[0].Rows.Count; i++)
                {
                    if (corr != null && corr.systemId == ((DataSet)ViewState["dsCorr"]).Tables[0].Rows[i][0].ToString())
                        return true;
                }
            }
			return false;			
		}

		private void addCorrSelDaRubrica(DocsPAWA.DocsPaWR.ElementoRubrica[] selCorrDaRubrica)
		{

            for(int i=0; i<selCorrDaRubrica.Length; i++)
			{
				DocsPaWR.ElementoRubrica el		= (DocsPAWA.DocsPaWR.ElementoRubrica) selCorrDaRubrica[i];
                //DocsPaWR.Corrispondente corr	= UserManager.getCorrispondenteByCodRubricaIE(this,el.codice,el.interno ? DocsPAWA.DocsPaWR.AddressbookTipoUtente.INTERNO : DocsPAWA.DocsPaWR.AddressbookTipoUtente.ESTERNO);
                //DocsPaWR.Corrispondente corr = UserManager.getCorrispondenteRubrica(this, el.codice, DocsPAWA.DocsPaWR.RubricaCallType.CALLTYPE_LISTE_DISTRIBUZIONE);
                DocsPaWR.Corrispondente corr = UserManager.getCorrispondenteBySystemIDDisabled(this.Page, el.systemId);
				
				if(corr != null && !verificaDuplicazioneCorr(corr))
				{
                    if (ViewState["dsCorr"] == null)
                    {
                        DataTable dt = new DataTable();
                        dt.Columns.Add("ID_DPA_CORR");
                        dt.Columns.Add("VAR_DESC_CORR");
                        dt.Columns.Add("VAR_COD_RUBRICA");
                        dt.Columns.Add("CHA_TIPO_IE");
                        dt.Columns.Add("CHA_DISABLED_TRASM");
                        dsCorrispondenti = new DataSet();
                        dsCorrispondenti.Tables.Add(dt);
                        ViewState.Add("dsCorr", dsCorrispondenti);
                    }

					DataRow dr = ((DataSet) ViewState["dsCorr"]).Tables[0].NewRow();
					dr[0] = corr.systemId;
					dr[1] = corr.descrizione;
                    dr[2] = corr.codiceRubrica;
                    dr[3] = corr.tipoIE;
                    if (corr.disabledTrasm)
                        dr[4] = "1";
                    else
                        dr[4] = "0";

                    ((DataSet) ViewState["dsCorr"]).Tables[0].Rows.Add(dr);
					((System.Web.UI.HtmlControls.HtmlInputHidden) this.FindControl("txt_confirmMod")).Value = "si";
				}
			}
			dg_2.DataSource = ((DataSet) ViewState["dsCorr"]);
			dg_2.DataBind();
			
			
			//txt_codiceCorr.Text = "";
			txt_descrizione.Text = "";			
		}

		private void dg_2_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			Session.Remove("selCorrDaRubrica");
			dg_2.CurrentPageIndex = e.NewPageIndex;
			dg_2.DataSource = ((DataSet) ViewState["dsCorr"]);
			dg_2.DataBind();
		}

		private void dg_1_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			Session.Remove("selCorrDaRubrica");
			dg_1.SelectedIndex = -1;
			dg_1.CurrentPageIndex = e.NewPageIndex;
			// dsListe = UserManager.getListePerModificaUt(this);
            dsListe = UserManager.getListePerRuoloUt(this);
			dg_1.DataSource = dsListe;
			dg_1.DataBind();
			Panel2.Visible = false;
            Panel3.Visible = false;
		}

        protected string GetText(System.Data.DataRowView dr)
        {
            return dr["VAR_DESC_CORR"].ToString();
        }

        protected Color GetForeColor(System.Data.DataRowView dr)
        {
            if (dr["CHA_DISABLED_TRASM"].ToString().ToUpper().Equals("1"))
            {
                return Color.Red;
            }
            else
            {
                return Color.Black;
            }
        }
	}
}
