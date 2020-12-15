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
using Microsoft.Web.UI.WebControls;
using System.Globalization;
using System.Configuration;

namespace DocsPAWA.fascicoli
{
	/// <summary>
	/// Summary description for browsingFasc.
	/// </summary>
	public class browsingFasc_old : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Label lbl_ruolo;
		protected Microsoft.Web.UI.WebControls.TreeView Titolario;
		
		
		protected System.Web.UI.WebControls.DropDownList ddl_registri;
		protected System.Web.UI.WebControls.Image img_statoReg;

		#region variabili codice
		private bool allClass;
		private DocsPAWA.DocsPaWR.FiltroRicerca[][] qV;
		private DocsPAWA.DocsPaWR.FiltroRicerca fV1;
		private DocsPAWA.DocsPaWR.FiltroRicerca[] fVList;
		private int indexH;
		private System.Collections.Hashtable TheHash;
		private const string navigatePageSearch="tabRisultatiRicFasc.aspx";
		private DocsPAWA.DocsPaWR.Ruolo userRuolo;
		private DocsPAWA.DocsPaWR.Registro userReg;
		private int indexNodoSel;
		#endregion

		
		protected System.Web.UI.WebControls.Button Button1;
		protected System.Web.UI.WebControls.TextBox TextBox1;
		protected System.Web.UI.WebControls.DropDownList DDLFiltro1;
		protected System.Web.UI.WebControls.TextBox TextFiltro1;
		protected System.Web.UI.WebControls.DropDownList DDLFiltro2;
		protected System.Web.UI.WebControls.TextBox TextFiltro2;
		protected System.Web.UI.WebControls.DropDownList DDLFiltro3;
		protected System.Web.UI.WebControls.TextBox TextFiltro3;
		protected System.Web.UI.WebControls.Label Label1;
		protected System.Web.UI.WebControls.Label lbl_mostraTuttiFascicoli;
		protected System.Web.UI.WebControls.RadioButtonList rbl_MostraTutti;
		protected System.Web.UI.WebControls.DropDownList DDLFiltro4;
		protected System.Web.UI.WebControls.TextBox TextFiltro4;
		protected System.Web.UI.WebControls.DropDownList DDLFiltro5;
		protected System.Web.UI.WebControls.TextBox TextFiltro5;
		private Microsoft.Web.UI.WebControls.TreeNode node;

		private DocsPAWA.DocsPaWR.Registro[] userRegistri;

#region Sezione gestione script JavaScript lato client	
		//è necessario che alla fine delal sezione HTML della pagina
		//sia presente il seguente codice:
		//
		//				<script language="javascript">
		//					esecuzioneScriptUtente();
		//				</script>
		//
		//nella page_Load:		
		//				if (!IsPostBack)
		//				{
		//					generaFunctionChiamataScript();
		//				}
		//chiamare addScript() per aggiungere gli script 
		//da eseguire alla fine della routine che ne richiede
		//l'esecuzione. Infine, eseguire generaFunctionChiamataScript()
		
		private ArrayList storedScriptCall=new ArrayList();
		private string nameScript="script";
		protected DocsPaWebCtrlLibrary.ImageButton btn_new;
		protected DocsPaWebCtrlLibrary.ImageButton btn_del;
		protected DocsPaWebCtrlLibrary.ImageButton btn_ricFascicoli;
		protected System.Web.UI.WebControls.Image icoReg;
		protected System.Web.UI.WebControls.TextBox txt_RicTit;
		protected DocsPaWebCtrlLibrary.ImageButton btn_ricTit;
		protected System.Web.UI.WebControls.Label lbl_result;
		private int indexScript=0;
		
		private void addScript(string scriptBody)
		{
			indexScript++;
			string newScriptName=nameScript+indexScript.ToString();
			creaScript(newScriptName,scriptBody);
		}

		private void creaScript(string nameScript,string scriptBody)
		{
			try
			{
				//crea funxione script
				string script="<script language=\"javascript\">"+
					"function "+nameScript+"(){"+scriptBody+"}</script>";
				Response.Write(script);
				
				//crea chiamata alla funzione
				storedScriptCall.Add(nameScript);			
			}
			catch(System.Exception es) 
			{
				ErrorManager.redirect(this, es);
			} 
		}

		private void generaFunctionChiamataScript()
		{
			try
			{
				Response.Write("<script language=\"javascript\">");
				Response.Write("function esecuzioneScriptUtente()");
				Response.Write("{");
				for (int i=0;i<storedScriptCall.Count;i++)
				{
					string call=(string)storedScriptCall[i];
					Response.Write(call+"();");
				}
				Response.Write("}");
				Response.Write("</script>");
			}
			catch(System.Exception es) 
			{
				ErrorManager.redirect(this, es);
			} 
		}
#endregion


		private void getParameterUser()
		{
			userRuolo=UserManager.getRuolo(this);
			userReg= UserManager.getRegistroSelezionato(this);
		}

		
		private void Page_PreRender(object sender, System.EventArgs e)
		{
			this.btn_new.Attributes.Add("onclick","ApriFinestraNewFasc();");
		}
		
		private void verificaCreazioneNewFascicolo()
		{
			string script="";
			DocsPaWR.ResultCreazioneFascicolo resultCreazione;

			try
			{
				DocsPaWR.FascicolazioneClassificazione classificazioneSel=FascicoliManager.getClassificazioneSelezionata(this);
				
				if (classificazioneSel!=null) 
				{
					DocsPaWR.Fascicolo fascicoloToCreate=FascicoliManager.getNewFascicolo(this);
					if (fascicoloToCreate!=null)
					{
						DocsPaWR.Fascicolo newFascicolo=FascicoliManager.newFascicolo(this,classificazioneSel,fascicoloToCreate,out resultCreazione);
						if (newFascicolo!=null)
						{
							script="alert('Il fascicolo è stato creato correttamente');document.location.reload();";
						}
						else
						{
							script="alert('Il fascicolo non è stato creato correttamente');document.location.reload();";
						}
						addScript(script);
						script="document.location.reload();";
						addScript(script);
						
					}
				}
			}
			catch(System.Exception ex)
			{
				
				ErrorManager.redirect(this,ex);
			}
			finally
			{
				FascicoliManager.removeNewFascicolo(this);
			}
		}
		
		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			Utils.startUp(this);
			getParameterUser();
			
			try
			{
				string rigaDescrizione="<tr><td align=\"center\" height=\"15\" class=\"titolo_bianco\" bgcolor=\"#810d06\">Registro</td></tr>";
				if(!Page.IsPostBack)
				{
					//ricaricamento della pagina
					impostazioniIniziali();
					verificaCreazioneNewFascicolo();
					Page.RegisterClientScriptBlock("CallDescReg","<!--Desc Reg inzio--><DIV onmouseover=\"closeIt()\"><LAYER onmouseover=\"closeIt()\"></LAYER></DIV><DIV id=\"descreg\" style=\"visibility:hidden;LEFT: 140px; POSITION: absolute; TOP: 45px\"><div align=\"left\"><table cellSpacing=\"0\" border='1' bordercolordark='#ffffff' cellPadding=\"0\" bgColor=\"#d9d9d9\"  width='210px' height='60px'>"+rigaDescrizione+"<tr><td  bgColor=\"#d9d9d9\" class=\"testo_grigio_sp\">"+userRegistri[this.ddl_registri.SelectedIndex].descrizione+"</td></tr></table></div></DIV><!--Fine desc reg-->");	

				}
				else
				{
					indexNodoSel=getIndexTitolarioSelezionato();
					Page.RegisterClientScriptBlock("CallDescReg","<!--Desc Reg inzio--><DIV onmouseover=\"closeIt()\"><LAYER onmouseover=\"closeIt()\"></LAYER></DIV><DIV id=\"descreg\" style=\"visibility:hidden;LEFT: 140px; POSITION: absolute; TOP: 45px\"><div align=\"left\"><table cellSpacing=\"0\" border='1' bordercolordark='#ffffff' cellPadding=\"0\" bgColor=\"#d9d9d9\"  width='210px' height='60px'>"+rigaDescrizione+"<tr><td  bgColor=\"#d9d9d9\" class=\"testo_grigio_sp\">"+UserManager.getRuolo(this).registri[ddl_registri.SelectedIndex].descrizione+"</td></tr></table></div></DIV><!--Fine desc reg-->");	

				}
				generaFunctionChiamataScript();
			}

			catch(System.Exception ex)
			{
				ErrorManager.redirect(this,ex);
			}
		}

		private void setTitolarioInAlbero()
		{			
			DocsPaWR.FascicolazioneClassificazione classificazioneSelezionata=FascicoliManager.getClassificazioneSelezionata(this);
			
			TheHash=FascicoliManager.getTheHash(this);	
			
			//verifico se nella hashtable esistono gli oggetti con i quali
			//è stato caricato l'albero dei titolari
			if (TheHash!=null)
			{
				//se classificazioneSelezionata non è null, dal momento che non
				//siamo in un postback, significa che è stata richiesta
				//la creazione di un nuovo fascicolo per il titolario
				//precedentemente selezionato, e che era stato messo in session.
				if (classificazioneSelezionata==null)
				{
					int idClass=getIndexTitolarioSelezionato();
					indexNodoSel=idClass;
					classificazioneSelezionata=(DocsPAWA.DocsPaWR.FascicolazioneClassificazione)TheHash[idClass];
				}
				else
				{
					//la pagina è stata ricaricata per effettura la creazione di un
					//fascicolo per il titolario selezionato
					for (int i=0;i<TheHash.Count;i++)
					{
						//recupero l'index del titolario
						DocsPaWR.FascicolazioneClassificazione classificazione=(DocsPAWA.DocsPaWR.FascicolazioneClassificazione)TheHash[i];
						if (classificazione==classificazioneSelezionata)
						{
							indexNodoSel=i;
							break;
						}
					}
				}

				FascicoliManager.setClassificazioneSelezionata(this,classificazioneSelezionata);
				FascicoliManager.setDescrizioneClassificazione(this,classificazioneSelezionata.descrizione);
			}
			else
			{
				indexNodoSel=0;
			}
		}

		private int getIndexTitolarioSelezionato()
		{
			int idClass=0;
			Microsoft.Web.UI.WebControls.TreeNode nodoSel=Titolario.GetNodeFromIndex(Titolario.SelectedNodeIndex);

			//verifico l'esistenza di un nodo dell'albero dei titolari selezionato
			if (nodoSel!=null)
			{
				idClass=Int32.Parse(nodoSel.ID);
			}
			else
			{
				idClass=0;
			}
			
			return idClass;
		}

		private void impostazioniIniziali()
		{
			FascicoliManager.removeAllClassValue(this);
			caricaAttributiDisponibili();
			caricaRegistriDisponibili();
			settaRegistroSelezionato();

			/* modifiche sab 30-01-03
			if (this.ddl_registri.SelectedItem!=null)
			{
				caricaTitolariRegistro();
				setTitolarioInAlbero();
			}

			if (Titolario.SelectedNodeIndex!=null)
			{
				ricercaFascicoli();
			}
			*/
		}

		#region setStatoReg

		private void setStatoReg(DocsPAWA.DocsPaWR.Registro  reg)
		{
			// inserisco il registro selezionato in sessione			
			UserManager.setRegistroSelezionato(this, reg);
			string nomeImg;

			if(UserManager.getStatoRegistro(reg).Equals("G"))
				nomeImg = "stato_giallo.gif";
			else if (UserManager.getStatoRegistro(reg).Equals("V"))
				nomeImg = "stato_verde.gif";
			else
				nomeImg = "stato_rosso.gif";

			this.img_statoReg.ImageUrl = "../images/" + nomeImg;
		}

		#endregion

		#region setStato_OLD
		private void setStatoReg_old(DocsPAWA.DocsPaWR.Registro  reg)
		{
			// inserisco il registro selezionato in sessione			
			
			UserManager.setRegistroSelezionato(this, reg);

			string dataApertura=reg.dataApertura;
			string nomeImg;
			DateTime dt_cor = DateTime.Now;
			
			CultureInfo ci = new CultureInfo("it-IT");

			
			string[] formati={"dd/MM/yyyy","dd/MM/yyyy HH.mm.ss","dd/MM/yyyy H.mm.ss",};

			DateTime d_ap = DateTime.ParseExact(dataApertura,formati,ci.DateTimeFormat,DateTimeStyles.AllowWhiteSpaces);
			//aggiungo un giorno per fare il confronto con now (che comprende anche minuti e secondi)
			d_ap = d_ap.AddDays(1);
		
			string mydate = dt_cor.ToString(ci);

			//DateTime dt = DateTime.ParseExact(mydate,formati,ci.DateTimeFormat,DateTimeStyles.AllowWhiteSpaces);

			string StatoAperto=ConfigSettings.getKey("STATO.REG.APERTO");
			if (reg.stato.Equals(StatoAperto)) 
			{
				if (dt_cor.CompareTo(d_ap)>0)
				{
					//data odierna maggiore della data di apertura del registro
					nomeImg = "stato_giallo.gif";
				} 
				else
					nomeImg = "stato_verde.gif";
			}
			else
				nomeImg = "stato_rosso.gif";	

			this.img_statoReg.ImageUrl = "../images/" + nomeImg;
		}

		#endregion

		private void caricaAttributiDisponibili()
		{
			for (int i=1;i<=5;i++)
			{
				//				DropDownList ddlControl;
				//				ddlControl=(DropDownList)this.FindControl("DDLFiltro"+i.ToString());	
				//				ddlControl.DataSource = Enum.GetNames(typeof(DocsPAWA.DocsPaWR.FiltriFascicolazione));
				//				ddlControl.DataMember="";
				//				ddlControl.DataBind();

				DropDownList ddlControl=(DropDownList)this.FindControl("DDLFiltro"+i.ToString());
				string[] arrayFiltri=Enum.GetNames(typeof(DocsPAWA.DocsPaWR.FiltriFascicolazione));
				if(ConfigSettings.getKey(ConfigSettings.KeysENUM.VISUALIZZA_ID_LEG)!=null 
					&& ConfigSettings.getKey(ConfigSettings.KeysENUM.VISUALIZZA_ID_LEG).Equals("1"))
				{
					Utils.populateDdlWithEnumValuesANdKeys(ddlControl,arrayFiltri);
				}
				else
				{
					System.Collections.ArrayList arrayFiltriCorr=new System.Collections.ArrayList();
					for(int j=0;j<arrayFiltri.Length;j++)
					{
						if(!arrayFiltri[j].Equals("CODICE_LEGISLATURA"))
						{
							arrayFiltriCorr.Add(arrayFiltri[j]);
						}
					}
					string[] arrayFiltri1=(string[]) arrayFiltriCorr.ToArray(typeof(string));
					Utils.populateDdlWithEnumValuesANdKeys(ddlControl,arrayFiltri1);
				}
			}
		}

		private void caricaRegistriDisponibili()
		{
			this.lbl_ruolo.Text=userRuolo.descrizione;
			userRegistri = UserManager.getListaRegistri(this);
			for(int i=0;i<userRegistri.Length;i++)
			{
				//this.ddl_registri.Items.Add((userRuolo.registri[i]).descrizione);
				this.ddl_registri.Items.Add((userRegistri[i]).codRegistro);
				this.ddl_registri.Items[i].Value=(userRegistri[i]).systemId;
			}
		}


		private string getJSToOpenResultPage(int idFascicolo)
		{
			string newUrl;
			newUrl=navigatePageSearch+"?idClass="+idFascicolo;
			return newUrl;
		}


		private void caricaTitolariRegistro()
		{
			//distruggo l'albero precedente.
			Titolario.Nodes.Clear();
			string codClassifica;

			codClassifica = this.txt_RicTit.Text;
			if (codClassifica.Equals(""))
				codClassifica = null;

			//cosi' nuovo hash nella stessa locazione di mem.
			if (TheHash!=null)
				TheHash.Clear();
			else
				TheHash=new Hashtable();

			indexH=0;
			
			//Recupero elenco titolari associati al registro selezionato

			DocsPaWR.FascicolazioneClassificazione[] FascClass=FascicoliManager.fascicolazioneGetTitolario(this, codClassifica,true);

			for(int k=0;k<FascClass.Length;k++)
			{
				//costruisco nodo root dell'albero	
				Microsoft.Web.UI.WebControls.TreeNode RootAppo=new Microsoft.Web.UI.WebControls.TreeNode();
				RootAppo.Target="centrale";
				RootAppo.Text=FascClass[k].codice+"-"+FascClass[k].descrizione;
				indexH=indexH+1;
				TheHash.Add(indexH,FascClass[k]);
				
				RootAppo.ID=indexH.ToString();
				RootAppo.NavigateUrl="tabrisultatiRicfasc.aspx?idClass="+indexH.ToString();
				RootAppo.Target="iFrame_dx";

				//lo aggiungo all'albero
				Titolario.Nodes.Add(RootAppo);

				this.CreateTree(RootAppo,FascClass[k]);
				FascicoliManager.setTheHash(this,TheHash);
			}
		}

		private void settaRegistroSelezionato()
		{
			if (ddl_registri.SelectedIndex!=-1)
			{
				if (userRegistri == null)
					userRegistri = UserManager.getListaRegistri(this);
				UserManager.setRegistroSelezionato(this, userRegistri[this.ddl_registri.SelectedIndex]);
				setStatoReg(userRegistri[this.ddl_registri.SelectedIndex]);
				//attenzione! ripulire i campi relativi al mittente e all'oggetto che dipendono dal registro			    			
			}
		}

		private void ddl_registri_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			settaRegistroSelezionato();
			caricaTitolariRegistro();
		}

		

		public void CreateTree(Microsoft.Web.UI.WebControls.TreeNode TreeN,DocsPaWR.FascicolazioneClassificazione FClass)
		{
			int g=FClass.childs.Length;
			DocsPaWR.FascicolazioneClassificazione FClassChild;

			for(int j=0;j<g;j++)
			{
				node=new Microsoft.Web.UI.WebControls.TreeNode();// TreeNode(((DocsPaVO.fascicolazione.classificazione)FascClass.childs[j]).descrizione,"Fascicoli_cn.aspx");
				FClassChild=(DocsPAWA.DocsPaWR.FascicolazioneClassificazione)FClass.childs[j];
				node.Text=FClassChild.codice+"-"+FClassChild.descrizione;
				indexH=indexH+1;
				TheHash.Add(indexH,FClass.childs[j]);
				
				node.ID=indexH.ToString();
				node.NavigateUrl=getJSToOpenResultPage(indexH);
				node.Target="iFrame_dx";
				TreeN.Nodes.Add(node);
				//node.Expanded = true;
				CreateTree(node,FClassChild);
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
			this.ddl_registri.SelectedIndexChanged += new System.EventHandler(this.ddl_registri_SelectedIndexChanged);
			this.btn_ricTit.Click += new System.Web.UI.ImageClickEventHandler(this.btn_ricTit_Click);
			this.DDLFiltro1.SelectedIndexChanged += new System.EventHandler(this.DDLFiltro1_SelectedIndexChanged);
			this.rbl_MostraTutti.SelectedIndexChanged += new System.EventHandler(this.rbl_MostraTutti_SelectedIndexChanged);
			this.btn_ricFascicoli.Click += new System.Web.UI.ImageClickEventHandler(this.btn_ricFascicoli_Click);
			this.btn_new.Click += new System.Web.UI.ImageClickEventHandler(this.btn_new_Click);
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new System.EventHandler(this.Page_PreRender);

		}
		#endregion

		private void ricercaFascicoli()
		{
			try
			{
				//array contenitore degli array filtro di ricerca
				qV = new DocsPAWA.DocsPaWR.FiltroRicerca[1][];
				qV[0]=new DocsPAWA.DocsPaWR.FiltroRicerca[1];
				
				fVList=new DocsPAWA.DocsPaWR.FiltroRicerca[0];			
				
				#region recupera filtri
				for (int i=1;i<=5;i++)
				{
					DropDownList ddlControl;
					TextBox textBoxControl;
					ddlControl=(DropDownList)this.FindControl("DDLFiltro"+i.ToString());
					textBoxControl=(TextBox)this.FindControl("TextFiltro"+i.ToString());
					if (ddlControl.SelectedIndex >= 0)
					{
						if (textBoxControl.Text!="")
						{
							fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
							fV1.argomento=ddlControl.SelectedItem.Value.ToString();
							fV1.valore=textBoxControl.Text.ToString();
							fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
						}
					}
				}
				#endregion

				qV[0]=fVList;

				FascicoliManager.setFiltroRicFasc(this,qV);
			}
			catch(System.Exception es)
			{
				ErrorManager.redirect(this,es);
			}
		}

		private void eseguiRicerca()
		{
			
			ricercaFascicoli();
			
			string newUrl=navigatePageSearch+"?idClass="+indexNodoSel.ToString();
				
			//	Response.Write("<script>parent.iFrame_dx.document.location='"+newUrl+"';</script>");	
			Response.Write("<script language='javascript'>top.principale.iFrame_dx.document.location='"+newUrl+"';</script>");	
		}

		private bool getMostraTuttiFascicoliValue()
		{
			bool retFunction;

			if (this.rbl_MostraTutti.SelectedItem.Value.Equals("S"))
			{
				retFunction=true;
			}
			else
			{
				retFunction=false;
			}
			
			return retFunction;
		}
		


		private void Titolario_SelectedIndexChange(object sender, Microsoft.Web.UI.WebControls.TreeViewSelectEventArgs e)
		{
			try
			{
				eseguiRicerca();
			}
			catch(System.Exception es)
			{
				ErrorManager.redirect(this,es);
			}
		}

		private void rbl_MostraTutti_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			allClass=getMostraTuttiFascicoliValue();
			FascicoliManager.setAllClassValue(this,allClass);
			eseguiRicerca();
		}

		private void btn_ModifTrasm_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
		
		}

		private void gestioneEliminazioneFascicolo()
		{
		}

		private void btn_del_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
		
		}

		private void btn_rimuoviVersione_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
		}

		private void btn_ricFascicoli_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			try
			{
				eseguiRicerca();
			}
			catch(System.Exception es)
			{
				ErrorManager.redirect(this,es);
			}

		}

		
		private void enableFascFields()
		{
			this.DDLFiltro1.Enabled = true;
			this.DDLFiltro2.Enabled = true;
			this.DDLFiltro3.Enabled = true;
			this.DDLFiltro4.Enabled = true;
			this.DDLFiltro5.Enabled = true;

			this.TextFiltro1.Enabled = true;
			this.TextFiltro2.Enabled = true;
			this.TextFiltro3.Enabled = true;
			this.TextFiltro4.Enabled = true;
			this.TextFiltro5.Enabled = true;

			this.lbl_mostraTuttiFascicoli.Enabled = true;
			this.rbl_MostraTutti.Enabled = true;

			//this.btn_ricFascicoli.Enabled = true;
			this.btn_del.Enabled = true;
			this.btn_new.Enabled = true;


		}
		private void btn_ricTit_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			if (this.txt_RicTit.Text.Equals(""))
			{
				Response.Write("<script>alert('Attenzione: Si sta per visualizzare tutto il titolario');</script>");
			}
			
			if (this.ddl_registri.SelectedItem!=null)
			{
				caricaTitolariRegistro();
				setTitolarioInAlbero();
			}
			
			if (TheHash == null || TheHash.Count < 1)
				return;

			enableFascFields();

			if (Titolario.SelectedNodeIndex!=null)
			{
				ricercaFascicoli();
			}
		}

		private void btn_new_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
		
		}

		private void DDLFiltro1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
		
		}
	}
}
