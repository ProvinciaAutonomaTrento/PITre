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




namespace DocsPAWA
{
	/// <summary>
	/// versione 1.1
	/// </summary>
	public class sceltaRuolo : System.Web.UI.Page
	{
		//my objects
		DocsPaWR.Utente userHome;
		DocsPaWR.Ruolo userRuolo;
		protected System.Web.UI.WebControls.Image Image1;
		protected System.Web.UI.WebControls.Label lbl_ruoli;
		protected System.Web.UI.WebControls.DropDownList chklst_ruoli;
		protected System.Web.UI.WebControls.Label lbl_cod;
		protected System.Web.UI.WebControls.TextBox Cod_ruolo;
		protected System.Web.UI.WebControls.Label lbl_descr;
		protected System.Web.UI.WebControls.TextBox descr_ruolo;
		protected System.Web.UI.WebControls.Table TableUO;
		protected DocsPaWebCtrlLibrary.IFrameWebControl iFrame_sx;
		protected System.Web.UI.WebControls.DropDownList DDLOggettoTab1;
		protected DocsPaWebCtrlLibrary.IFrameWebControl iFrame_portale;
		protected int indiceUO;

		
		//end my obj
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			Utils.startUp(this);
			Response.Expires = 0;
			userHome = UserManager.getUtente(this);

			// cancello tutte le variabili di sessione risettando solo la userHome
			Session.RemoveAll();
			UserManager.setUtente(this,userHome);
					
			if(!IsPostBack && userHome != null)
			{	
				setInfoRuolo();	
				chklst_ruoli.SelectedIndex = 0;
				aggiornaDatiRuolo(0);
	
						
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
			this.chklst_ruoli.SelectedIndexChanged += new System.EventHandler(this.chklst_ruoli_SelectedIndexChanged);
			this.DDLOggettoTab1.SelectedIndexChanged += new System.EventHandler(this.DDLOggettoTab1_SelectedIndexChanged);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void setInfoRuolo()
		{
			this.descr_ruolo.Text="Nessun Ruolo";
			if(userHome != null)
			{
				if(userHome.ruoli != null)
				{
					for(int i=0;i<userHome.ruoli.Length;i++)	
						chklst_ruoli.Items.Add(((DocsPAWA.DocsPaWR.Ruolo)userHome.ruoli[i]).descrizione);
			
					if (userHome.ruoli.Length<=0) 
						this.descr_ruolo.Text="Nessun Ruolo";
					else 
					{	
						userRuolo=((DocsPAWA.DocsPaWR.Ruolo)userHome.ruoli[0]);
						Cod_ruolo.Text=userRuolo.codice;
						//livello_ruolo.Text=userRuolo.livello;
						descr_ruolo.Text=userRuolo.descrizione;
						//drawTable(createListHierarchy(indiceUO));				

					}
				}
				
			}
		}

		private void chklst_ruoli_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			aggiornaDatiRuolo(chklst_ruoli.SelectedIndex);
			DDLOggettoTab1.SelectedIndex=-1;

//			if (chklst_ruoli.SelectedIndex!=-1)
//			{		
//				Cod_ruolo.Text=((DocsPAWA.DocsPaWR.Ruolo)userHome.ruoli[chklst_ruoli.SelectedIndex]).codice;
//				livello_ruolo.Text=((DocsPAWA.DocsPaWR.Ruolo)userHome.ruoli[chklst_ruoli.SelectedIndex]).livello;
//				this.descr_ruolo.Text=((DocsPAWA.DocsPaWR.Ruolo)userHome.ruoli[chklst_ruoli.SelectedIndex]).descrizione;
//				
//				//costruisco tabella parte Unità Ogranizzativa	
//				drawTable(createListHierarchy(chklst_ruoli.SelectedIndex));
//				Session["userRuolo"]=((DocsPAWA.DocsPaWR.Ruolo)userHome.ruoli[chklst_ruoli.SelectedIndex]);
//				//userHome.unitaOrg.descrizione;				
//			}
//
//			//Ricarica il frame dei menù
//			Response.Write("<script>top.superiore.location.href = 'testata.aspx';</script>");

		}


		private void aggiornaDatiRuolo(int index)
		{
			if (index!=-1)
			{		
				Cod_ruolo.Text=((DocsPAWA.DocsPaWR.Ruolo)userHome.ruoli[index]).codice;
				//livello_ruolo.Text=((DocsPAWA.DocsPaWR.Ruolo)userHome.ruoli[index]).livello;
				this.descr_ruolo.Text=((DocsPAWA.DocsPaWR.Ruolo)userHome.ruoli[index]).descrizione;
				
				//costruisco tabella parte Unità Ogranizzativa	
				drawTable(createListHierarchy(index));
				Session["userRuolo"]=((DocsPAWA.DocsPaWR.Ruolo)userHome.ruoli[index]);
			}

			//Ricarica il frame dei menù
			Response.Write("<script>top.superiore.location.href = 'testata.aspx';</script>");
		}

		private string getCodRuolo(string systemID)
		{
			int cont;
			
			string Codice = "";	
			cont = 0;
			
			while ((cont < userHome.ruoli.Length) && !((DocsPAWA.DocsPaWR.Ruolo)userHome.ruoli[cont]).systemId.Equals(systemID))
			{
				cont = cont +1 ;
			}
			if (cont < userHome.ruoli.Length)
			{
				Codice = ((DocsPAWA.DocsPaWR.Ruolo)userHome.ruoli[cont]).codice;
			}
			return 	Codice;
		}



		private int getIndexRuolo(string  systemID)
		{
			int cont;
			int indice = -1;	
			cont = 0;
			
			while ((cont < userHome.ruoli.Length) && !((DocsPAWA.DocsPaWR.Ruolo)userHome.ruoli[cont]).systemId.Equals(systemID))
			{
				cont = cont +1 ;
			}
			if (cont < userHome.ruoli.Length)
			{
				indice = cont;
			}
			return 	indice;
		}



		private ArrayList createListHierarchy (int indexUO)
		{
			ArrayList Hlist = new ArrayList();
			
			Hlist.Add(((DocsPAWA.DocsPaWR.Ruolo) userHome.ruoli[indexUO]).uo.descrizione);
			DocsPaWR.UnitaOrganizzativa CurrUO=((DocsPAWA.DocsPaWR.Ruolo) userHome.ruoli[indexUO]).uo.parent;
			

			while (CurrUO!=null)
			{
				Hlist.Add(CurrUO.descrizione);
				CurrUO = CurrUO.parent;					
			}

			return Hlist;
		}
        
		private void drawTable (ArrayList HList)
		{
			for(int i= 0; i< HList.Count ; i++)
			{
				TableRow row = new TableRow();
				TableCell Tcell = new TableCell();
				Tcell.CssClass="testoNero";
				TableUO.Rows.Add(row);
				TableUO.Rows[i].Cells.Add(Tcell);
				Tcell.Text=(HList[HList.Count-i-1].ToString());
			}
		}

		private void Button1_Click(object sender, System.EventArgs e)
		{
			Response.Write("<script>alert('ppp');</script>");
			Response.Write("<script>window.open('testata.aspx','superiore','');</script>");
			Response.Redirect("testata.aspx");
		}



		private void aggiornaToDoList()
		{
			try 
			{
				//array contenitore degli array filtro di ricerca
				DocsPaWR.FiltroRicerca[][] qV;
				DocsPaWR.FiltroRicerca fV1;
				DocsPaWR.FiltroRicerca[] fVList;

				qV = new DocsPAWA.DocsPaWR.FiltroRicerca[1][];
				qV[0]=new DocsPAWA.DocsPaWR.FiltroRicerca[1];
				fVList=new DocsPAWA.DocsPaWR.FiltroRicerca[0];		

				#region filtro "oggetto trasmesso"
				if (this.DDLOggettoTab1.SelectedIndex >= 0)
				{
					fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
					fV1.argomento=DocsPaWR.FiltriTrasmissioneNascosti.TIPO_OGGETTO.ToString();
					fV1.valore=this.DDLOggettoTab1.SelectedItem.Value.ToString();
					fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
				}
				#endregion
				#region filtro "TO DO LIST" 
				
				
					fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
					fV1.argomento=DocsPaWR.FiltriTrasmissioneNascosti.TODO_LIST.ToString();
					
					fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
				
				#endregion

				qV[0]=fVList;

				DocumentManager.setFiltroRicTrasm(this,qV[0]);

				this.iFrame_sx.NavigateTo="ricercaTrasm/tabRisultatiRicTrasm.aspx?tiporic=R";	
				

			}
			catch(System.Exception es) 
			{
				//System.Diagnostics.Debug.WriteLine("error Login"+es.Message.ToString());
				ErrorManager.redirect(this,es);
			}
			
		}

		private void DDLOggettoTab1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			Session["Tipo_obj"]=DDLOggettoTab1.SelectedItem.Value;
			aggiornaDatiRuolo(chklst_ruoli.SelectedIndex);
			aggiornaToDoList();
		}

		

		
	}
}
