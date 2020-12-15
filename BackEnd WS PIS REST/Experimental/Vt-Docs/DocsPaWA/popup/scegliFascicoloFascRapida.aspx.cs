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

namespace DocsPAWA.popup
{
	/// <summary>
	/// Summary description for scegliFascicoloFascRapida.
	/// </summary>
    public class scegliFascicoloFascRapida : DocsPAWA.CssPage
	{
		protected System.Web.UI.WebControls.Button btn_ok;
		protected System.Web.UI.WebControls.Panel pnlButtonOk;
		protected System.Web.UI.WebControls.Button btn_chiudi;
		protected System.Web.UI.WebControls.DataGrid DgListaFasc;
		protected static int currentPage;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_focus;
		protected DocsPAWA.DocsPaWR.Fascicolo[] listaFasc;
		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			//prendo la lista di fascicoli dalla sessione e faccio il Bind del datagrid
			if(!IsPostBack)
			{	
				ListaFascicoliSessionMng.SetAsLoaded(this);
				BindGrid();
			}
		}

//		private void Page_PreRender(object sender, EventArgs e)
//		{
//			SetFocus(this.hd_focus);
//		}
//		
		private void SetFocus(System.Web.UI.Control ctrl)
		{
			string s = "<SCRIPT language='javascript'>if(document.getElementById('" + ctrl.ID + "')!=null){document.getElementById('" + ctrl.ID + "').focus();} </SCRIPT>";
			RegisterStartupScript("focus", s);
		}

		public void BindGrid()
		{
			
			currentPage = 1 ;
			listaFasc = (DocsPAWA.DocsPaWR.Fascicolo[])Session["listaFascFascRapida"];

			if(listaFasc!=null)
			{
				//ricercaFascicoli.RicercaFascicoliPerFascicolazioneRapida
				DocsPaWR.Fascicolo currentFasc;

				if (listaFasc != null  && listaFasc.Length > 0)
				{
					//Costruisco il datagrid
					ArrayList Dg_elem = new ArrayList();		

					for(int i= 0; i< listaFasc.Length ; i++)
					{					
			
						currentFasc = ((DocsPAWA.DocsPaWR.Fascicolo) listaFasc[i]);

						string dtaApertura = "";
						string dtaChiusura = "";
				
						if (currentFasc.apertura != null && currentFasc.apertura.Length > 0)
							dtaApertura = currentFasc.apertura.Substring(0,10);

						if (currentFasc.chiusura != null && currentFasc.chiusura.Length > 0)
							dtaChiusura = currentFasc.chiusura.Substring(0,10);
				
						//dati registro associato al nodo di titolario
						string idRegistro = currentFasc.idRegistroNodoTit; // systemId del registro
						string codiceRegistro = currentFasc.codiceRegistroNodoTit; //codice del Registro
						//
						if(idRegistro!=null && idRegistro == String.Empty)//se il fascicolo è associato a un TITOLARIO con idREGISTRO = NULL
							codiceRegistro = "<B>TUTTI</B>";


						string tipoFasc = currentFasc.tipo;
						string descFasc = currentFasc.descrizione;
						string  stato = currentFasc.stato;
						string chiave = currentFasc.systemID;
						string codFasc = currentFasc.codice;
				
						Dg_elem.Add(new ricercaFascicoli.RicercaFascicoliPerFascicolazioneRapida(tipoFasc,codFasc,descFasc,dtaApertura,dtaChiusura,chiave,stato,codiceRegistro));		
				
					
					}
				
					this.DgListaFasc.SelectedIndex=-1;
					this.DgListaFasc.DataSource = Dg_elem;
					this.DgListaFasc.DataBind();
					
				
				}
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
			this.DgListaFasc.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.DgListaFasc_PageIndexChanged);
			this.DgListaFasc.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.DgListaFasc_ItemDataBound_1);
			this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
			this.btn_chiudi.Click += new System.EventHandler(this.btn_chiudi_Click);
			this.Load += new System.EventHandler(this.Page_Load);
			//this.PreRender += new System.EventHandler(this.Page_PreRender);

		}
		#endregion

		private void btn_chiudi_Click(object sender, System.EventArgs e)
		{
			
			Response.Write("<script>window.returnValue = 'N'; window.close();</script>");
		}

		private void DgListaFasc_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			DgListaFasc.CurrentPageIndex = e.NewPageIndex;
			currentPage = e.NewPageIndex + 1;
			// Caricamento del DataGrid
			BindGrid();
		}

		private void DgListaFasc_ItemDataBound_1(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem) 
			{
				RadioButton rb = e.Item.Cells[0].Controls[0].FindControl("OptFasc") as RadioButton;
	
				//il record relativo ai fascicoli chiusi li sottolineo
				//non si puo fascicolare in fascicoli chiusi
				string dtaChiusuraFasc = ((Label) e.Item.Cells[7].Controls[1]).Text;
				

				if(dtaChiusuraFasc!=null)
				{
					if(dtaChiusuraFasc==String.Empty)//se il fascicolo è aperto
					{
						rb.Enabled= true;
					}
					else //se il fascicolo è chiuso
					{
						rb.Enabled= false;
						e.Item.ToolTip = "Il fascicolo è chiuso";
					}
		
				}
			}
		}

		private void btn_ok_Click(object sender, System.EventArgs e)
		{
			//key: chiave dell'item, ovvero dell'elemento selezionato
			int  key;
			bool avanza = verificaSelezione(out key);
			
			if(avanza)
			{
				listaFasc =  (DocsPAWA.DocsPaWR.Fascicolo[])Session["listaFascFascRapida"];
				
				if(listaFasc!=null)
				{
					DocsPaWR.Fascicolo fascSel = (DocsPAWA.DocsPaWR.Fascicolo)listaFasc[key];

					FascicoliManager.setFascicoloSelezionatoFascRapida(this,fascSel);
					Response.Write("<script>window.returnValue = 'Y'; window.close();</script>");
				}
			}
			else
			{
				Response.Write("<script>alert('Attenzione: selezionare un fascicolo');</script>");
			}
		}
	
		/// <summary>
		/// Verifica se è stata selezionata almeno una opzione, ovvero un fascicoli
		/// nel datagrid
		/// </summary>
		/// <param name="itemIndex"></param>
		/// <returns></returns>
		private bool verificaSelezione(out int key)
		{
			bool verificaSelezione = false;
			key = - 1;
			foreach (DataGridItem dgItem in this.DgListaFasc.Items)
			{		
				RadioButton optFasc=dgItem.Cells[3].FindControl("optFasc") as RadioButton;
				if ((optFasc!=null) && optFasc.Checked == true)
				{
					key = dgItem.DataSetIndex;
					verificaSelezione = true;
					break;
				}
			}
			return verificaSelezione;
		}

		#region classe per la gestione della sessione
		/// <summary>
		/// Classe per la gestione dei dati in sessione relativamente
		/// alla dialog "RicercaFascicoli.aspx"
		/// </summary>
		public sealed class ListaFascicoliSessionMng
		{
			private ListaFascicoliSessionMng()
			{
			}
			/// <summary>
			/// Gestione rimozione dati in sessione
			/// </summary>
			/// <param name="page"></param>
			public static void ClearSessionData(Page page)
			{
				page.Session.Remove("listaFascFascRapida");
			}	
	
			/// <summary>
			/// Impostazione flag booleano, se true, la dialog è stata caricata almeno una volta
			/// </summary>
			/// <param name="page"></param>
			public static void SetAsLoaded(Page page)
			{
				page.Session["ListaFascicoliFascRapida.isLoaded"]=true;
			}

			/// <summary>
			/// Impostazione flag relativo al caricamento della dialog
			/// </summary>
			/// <param name="page"></param>
			public static void SetAsNotLoaded(Page page)
			{
				page.Session.Remove("ListaFascicoliFascRapida.isLoaded");
			}

			/// <summary>
			/// Verifica se la dialog è stata caricata almeno una volta
			/// </summary>
			/// <param name="page"></param>
			/// <returns></returns>
			public static bool IsLoaded(Page page)
			{
				return (page.Session["ListaFascicoliFascRapida.isLoaded"]!=null);
			}
			#endregion
		
		}
	}
}
