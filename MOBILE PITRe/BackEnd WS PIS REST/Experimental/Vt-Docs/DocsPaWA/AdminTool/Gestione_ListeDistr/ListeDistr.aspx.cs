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
using DocsPAWA;

namespace Amministrazione.Gestione_ListeDistr
{
	/// <summary>
	/// Summary description for ListeDistr.
	/// </summary>
	public class ListeDistr : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Label lbl_avviso;
		protected System.Web.UI.WebControls.Label lbl_titolo;
		protected static DataSet dsListe = new DataSet();
		protected static DataSet dsCorrispondenti = new DataSet();
		protected static DataTable dtCorrLista = new DataTable();
		protected static DataSet dsCorrLista = new DataSet();
		protected System.Web.UI.WebControls.Panel Panel2;
		protected System.Web.UI.WebControls.Panel Panel3;
		protected System.Web.UI.WebControls.Label Label2;
		protected System.Web.UI.WebControls.TextBox txt_codiceCorr;
		protected System.Web.UI.WebControls.TextBox txt_descrizione;
		protected System.Web.UI.WebControls.ImageButton imgBtn_addCorr;
		protected System.Web.UI.WebControls.ImageButton imgBtn_descrizione;
		protected System.Web.UI.WebControls.DataGrid dg_2;
		protected System.Web.UI.HtmlControls.HtmlGenericControl DivDGElencoListe;
		protected System.Web.UI.HtmlControls.HtmlGenericControl DivDGCorrispondenti;
		protected System.Web.UI.WebControls.Label lbl_position;
	
		private DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
		private string idAmministrazione;
		protected System.Web.UI.WebControls.Label Label4;
		protected System.Web.UI.WebControls.TextBox txt_codiceLista;
		protected System.Web.UI.WebControls.Button btn_salva;
		protected System.Web.UI.WebControls.Button btn_nuova;
		protected System.Web.UI.WebControls.DataGrid dg_1;
		protected System.Web.UI.WebControls.Panel Panel1;
		protected System.Web.UI.WebControls.Label Label3;
		protected System.Web.UI.WebControls.TextBox txt_nomeLista;
		protected System.Web.UI.HtmlControls.HtmlInputHidden txt_confirmMod;
		protected System.Web.UI.HtmlControls.HtmlInputHidden txt_confirmDel;
		
		private void Page_Load(object sender, System.EventArgs e)
		{
            Session["AdminBookmark"] = "ListeDistribuzione";
            
            //----- CONTROLLO DELL'UTENTE AMMINISTRATORE CONNESSO -----------
			if(Session.IsNewSession)
			{
				Response.Redirect("../Exit.aspx?FROM=EXPIRED");
			}
			
			AmmUtils.WebServiceLink  ws=new AmmUtils.WebServiceLink();
			if(!ws.CheckSession(Session.SessionID))
			{
				Response.Redirect("../Exit.aspx?FROM=ABORT");
			}

			if(Session["AMMDATASET"] == null)
			{
				RegisterStartupScript("NoProfilazione","<script>alert('Attenzione selezionare un\\'amministrazione !'); document.location = '../Gestione_Homepage/Home.aspx';</script>");
				return;
			}
			// ---------------------------------------------------------------
			
			//Decommentare con integrazione in FrontEnd
			if(Session["selCorrDaRubrica"] != null)
				addCorrSelDaRubrica((DocsPAWA.DocsPaWR.ElementoRubrica[]) Session["selCorrDaRubrica"]);

			//Gestione del tasto invio
			DocsPAWA.Utils.DefaultButton (this, ref txt_codiceCorr, ref imgBtn_addCorr);
			DocsPAWA.Utils.DefaultButton (this, ref txt_nomeLista, ref btn_salva);
			DocsPAWA.Utils.DefaultButton (this, ref txt_descrizione, ref imgBtn_addCorr);
			
			string[] amministrazione = ((string) Session["AMMDATASET"]).Split('@');
			string codiceAmministrazione  = amministrazione[0];
			idAmministrazione = wws.getIdAmmByCod(codiceAmministrazione);

			if(!IsPostBack)
			{
				lbl_position.Text = "&nbsp;&bull;&nbsp;Amministrazione: " + AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"],"1");

				DocsPAWA.DocsPaWR.Utente ut = new DocsPAWA.DocsPaWR.Utente();
				ut.codiceAmm = codiceAmministrazione;
				ut.idAmministrazione = idAmministrazione;
				ut.tipoIE = "I";
				Session.Add("userData",ut);

				DocsPAWA.DocsPaWR.Ruolo rl = new DocsPAWA.DocsPaWR.Ruolo();
				rl.codiceAmm = codiceAmministrazione;
				rl.idAmministrazione = idAmministrazione;
				rl.tipoIE = "I";
				Session.Add("userRuolo",rl);


				Panel1.Visible = true;
				Panel2.Visible = false;
				
				dsListe = wws.getListeDistribuzioneAmm(idAmministrazione);
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
				this.imgBtn_descrizione.Attributes["onClick"] = "_ApriRubrica();";			
				this.btn_nuova.Attributes["onClick"] = "confirmMod();";
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
			this.btn_salva.Click += new System.EventHandler(this.btn_salva_Click);
			this.btn_nuova.Click += new System.EventHandler(this.btn_nuova_Click);
			this.dg_1.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dg_1_PageIndexChanged);
			this.dg_1.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_1_EditCommand);
			this.dg_1.SelectedIndexChanged += new System.EventHandler(this.dg_1_SelectedIndexChanged);
			this.txt_codiceCorr.TextChanged += new System.EventHandler(this.txt_codiceCorr_TextChanged);
			this.imgBtn_addCorr.Click += new System.Web.UI.ImageClickEventHandler(this.imgBtn_addCorr_Click);
			this.dg_2.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dg_2_PageIndexChanged);
			this.dg_2.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_2_EditCommand);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void dg_1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			Session.Remove("selCorrDaRubrica");
			dg_2.CurrentPageIndex = 0;
			btn_salva.Visible = true;
			
			Panel2.Visible = true;
			Label2.Visible = true;
			Label3.Visible = true;
			txt_descrizione.Visible = true;
			txt_descrizione.Text = "";
			txt_codiceCorr.Visible = true;
			txt_codiceCorr.Text = "";
			txt_nomeLista.Visible = true;
			txt_nomeLista.Text = dg_1.SelectedItem.Cells[1].Text;
			txt_codiceLista.Text = wws.getCodiceLista(dg_1.SelectedItem.Cells[0].Text);
			txt_codiceLista.Enabled = false;
			
			imgBtn_descrizione.Visible = true;
			imgBtn_addCorr.Visible = true;

			ViewState.Add("dsCorr",wws.getCorrispondentiLista(dg_1.SelectedItem.Cells[0].Text));
			dg_2.DataSource = ((DataSet) ViewState["dsCorr"]);
			dg_2.DataBind();
			dg_2.Visible = true;
			
			//SetFocus(txt_codiceLista);
		}

		private void SetFocus(System.Web.UI.Control ctrl)
		{
			string s = "<SCRIPT language='javascript'>document.getElementById('" + ctrl.ID + "').focus() </SCRIPT>";
			RegisterStartupScript("focus", s);
		}

		private void dg_1_EditCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			if( ((System.Web.UI.HtmlControls.HtmlInputHidden) this.FindControl("txt_confirmDel")).Value == "si")
			{
				Session.Remove("selCorrDaRubrica");
				
				int rigaSelezionata = e.Item.ItemIndex;
				dg_1.CurrentPageIndex = 0;
				wws.deleteListaDistribuzione(dg_1.Items[rigaSelezionata].Cells[0].Text);
				dsListe = wws.getListeDistribuzioneAmm(idAmministrazione);
				dg_1.DataSource = dsListe;
				dg_1.DataBind();
				Panel2.Visible = false;
				dg_1.SelectedIndex = -1;
				((System.Web.UI.HtmlControls.HtmlInputHidden) this.FindControl("txt_confirmDel")).Value = "";
				btn_salva.Visible = false;
			}
		}

		private void btn_salva_Click(object sender, System.EventArgs e)
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

                if (nomeLista != wws.getNomeLista(wws.getCodiceLista(idLista), UserManager.getInfoUtente().idAmministrazione))
				{
					if(!wws.isUniqueNomeLista(nomeLista, UserManager.getInfoUtente().idAmministrazione))
					{
						RegisterStartupScript("Modifica non effettuata","<script>alert('Nome lista non univoco !');</script>");
						SetFocus(txt_nomeLista);
						return;	
					}										
				}

				wws.modificaLista(((DataSet) ViewState["dsCorr"]),idLista,nomeLista,codiceLista);
				RegisterStartupScript("Modifica effettuata","<script>alert('Operazione effettuata con successo!');</script>");
				dg_1.SelectedIndex = -1;
				Panel2.Visible = false;
				dsListe = wws.getListeDistribuzioneAmm(idAmministrazione);
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

					if(!wws.isUniqueCod(codiceLista,idAmministrazione))
					{
						RegisterStartupScript("Modifica non effettuata","<script>alert('Codice lista non univoco !');</script>");
						SetFocus(txt_codiceLista);
						return;	
					}
					if(!wws.isUniqueNomeLista(nomeLista, idAmministrazione))
					{
						RegisterStartupScript("Modifica non effettuata","<script>alert('Nome lista non univoco !');</script>");
						SetFocus(txt_nomeLista);
						return;	
					}		

					wws.salvaLista(((DataSet) ViewState["dsCorr"]),nomeLista,codiceLista,null,idAmministrazione);
					RegisterStartupScript("Inserimento effettuato","<script>alert('Operazione effettuata con successo!');</script>");
					dg_1.SelectedIndex = -1;
					Panel2.Visible = false;	
					dsListe = wws.getListeDistribuzioneAmm(idAmministrazione);
					dg_1.DataSource = dsListe;
					dg_1.DataBind();	
					txt_confirmMod.Value = "";
				}
			}
			btn_salva.Visible = false;
		}

		private void dg_2_EditCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			if( ((System.Web.UI.HtmlControls.HtmlInputHidden) this.FindControl("txt_confirmDel")).Value == "si")
			{
				Session.Remove("selCorrDaRubrica");
				int rigaSelezionata = (dg_2.PageSize * dg_2.CurrentPageIndex) +  e.Item.ItemIndex;
                if (e.Item.ItemIndex == 0 && dg_2.Items.Count == 1 && dg_2.CurrentPageIndex != 0)
                    dg_2.CurrentPageIndex = dg_2.CurrentPageIndex - 1;
                if (dg_1.SelectedIndex != -1)
				{
					((DataSet) ViewState["dsCorr"]).Tables[0].Rows[rigaSelezionata].Delete();
					((DataSet) ViewState["dsCorr"]).Tables[0].AcceptChanges();
					string idLista = dg_1.SelectedItem.Cells[0].Text;
					string nomeLista = txt_nomeLista.Text.Replace("'","''");
					string codiceLista = txt_codiceLista.Text.Replace("'","''");
					wws.modificaLista((DataSet) ViewState["dsCorr"],idLista,nomeLista,codiceLista);
					dg_2.DataSource = ((DataSet) ViewState["dsCorr"]);
					dg_2.DataBind();
					dg_2.Visible  = true;
					if(dg_2.Items.Count == 0)
					{
						wws.deleteListaDistribuzione(dg_1.SelectedItem.Cells[0].Text);
						dsListe = wws.getListeDistribuzioneAmm(idAmministrazione);
						dg_1.DataSource = dsListe;
						dg_1.DataBind();
						Panel2.Visible = false;
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

		private void btn_nuova_Click(object sender, System.EventArgs e)
		{
			if( ((System.Web.UI.HtmlControls.HtmlInputHidden) this.FindControl("txt_confirmMod")).Value == "")
			{
				Session.Remove("selCorrDaRubrica");
				btn_salva.Visible = true;

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
			
			DocsPAWA.DocsPaWR.Corrispondente corr = null;
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
			
			DocsPAWA.DocsPaWR.Corrispondente corr = null;
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
			dsListe = wws.getListeDistribuzioneAmm(idAmministrazione);
			dg_1.DataSource = dsListe;
			dg_1.DataBind();
			Panel2.Visible = false;
		}

		private bool verificaDuplicazioneCorr(DocsPAWA.DocsPaWR.Corrispondente corr)
		{
			for(int i=0; i<((DataSet) ViewState["dsCorr"]).Tables[0].Rows.Count; i++)
			{
				if(corr.systemId == ((DataSet) ViewState["dsCorr"]).Tables[0].Rows[i][0].ToString())
					return true;
			}
			return false;			
		}

		private void addCorrSelDaRubrica(DocsPAWA.DocsPaWR.ElementoRubrica[] selCorrDaRubrica)
		{
			for(int i=0; i<selCorrDaRubrica.Length; i++)
			{
				DocsPAWA.DocsPaWR.ElementoRubrica el		= (DocsPAWA.DocsPaWR.ElementoRubrica) selCorrDaRubrica[i];
				DocsPAWA.DocsPaWR.Corrispondente corr	= UserManager.getCorrispondenteByCodRubricaIE(this,el.codice,el.interno ? DocsPAWA.DocsPaWR.AddressbookTipoUtente.INTERNO : DocsPAWA.DocsPaWR.AddressbookTipoUtente.ESTERNO);
				
				if(!verificaDuplicazioneCorr(corr))
				{
					DataRow dr = ((DataSet) ViewState["dsCorr"]).Tables[0].NewRow();
					dr[0] = corr.systemId;
					dr[1] = corr.descrizione;
                    dr[2] = corr.codiceRubrica;
                    dr[3] = corr.tipoIE;
                    if (corr.disabledTrasm)
                        dr[4] = "1";
                    else
                        dr[4] = "0";

                    ((DataSet)ViewState["dsCorr"]).Tables[0].Rows.Add(dr);
					((System.Web.UI.HtmlControls.HtmlInputHidden) this.FindControl("txt_confirmMod")).Value = "si";
				}
			}
			dg_2.DataSource = ((DataSet) ViewState["dsCorr"]);
			dg_2.DataBind();			
			
			//txt_codiceCorr.Text = "";
			txt_descrizione.Text = "";			
		}

        protected void dg_1_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Pager)
            {

                if (e.Item.Cells.Count > 0)
                {

                    e.Item.Cells[0].Attributes.Add("colspan", e.Item.Cells[0].ColumnSpan.ToString());

                }

            }
        }

        protected void dg_2_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Pager)
            {

                if (e.Item.Cells.Count > 0)
                {

                    e.Item.Cells[0].Attributes.Add("colspan", e.Item.Cells[0].ColumnSpan.ToString());

                }

            }
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
