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

namespace DocsPAWA.documento
{
	/// <summary>
	/// Summary description for gestioneDocSemplice.
	/// </summary>
	public class gestioneDocSemplice : System.Web.UI.Page
	{
		protected DocsPaWebCtrlLibrary.IFrameWebControl iFrame_sx;
		protected System.Web.UI.WebControls.Label lbl_ruolo;
		protected System.Web.UI.WebControls.DropDownList ddl_registri;
		protected System.Web.UI.WebControls.Label lbl_registri;
		protected System.Web.UI.WebControls.Image icoReg;
		protected System.Web.UI.WebControls.Image img_statoReg;
		protected DocsPaWebCtrlLibrary.IFrameWebControl iFrame_dx;

		private DocsPAWA.DocsPaWR.Registro[] userRegistri;
		private DocsPAWA.DocsPaWR.SchedaDocumento schedaDocumento;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			//controlla se session Id è not null
			Utils.startUp(this);
			try
			{
				Response.Expires = 0;
				this.iFrame_dx.NavigateTo = "tabDoc.aspx";
				
				loadSchedaDocumento();
			
				string rigaDescrizione="<tr><td align=\"center\" height=\"15\" class=\"titolo_bianco\" bgcolor=\"#810d06\">Registro</td></tr>";
				if(!IsPostBack) 
				{
					CaricaComboRegistri(ddl_registri);
					Page.RegisterClientScriptBlock("CallDescReg","<!--Desc Reg inzio--><DIV onmouseover=\"closeIt()\"><LAYER onmouseover=\"closeIt()\"></LAYER></DIV><DIV id=\"descreg\" style=\"visibility:hidden;LEFT: 210px; POSITION: absolute; TOP: 45px\"><div align=\"left\"><table cellSpacing=\"0\" border='1' bordercolordark='#ffffff' cellPadding=\"0\" bgColor=\"#d9d9d9\"  width='210px' height='60px'>"+rigaDescrizione+"<tr><td  bgColor=\"#d9d9d9\" class=\"testo_grigio_sp\">"+userRegistri[this.ddl_registri.SelectedIndex].descrizione+"</td></tr></table></div></DIV><!--Fine desc reg-->");	
			
					//carica il ruolo scelto
					this.lbl_ruolo.Text = UserManager.getRuolo(this).descrizione; 
				}
				else
					Page.RegisterClientScriptBlock("CallDescReg","<!--Desc Reg inzio--><DIV onmouseover=\"closeIt()\"><LAYER onmouseover=\"closeIt()\"></LAYER></DIV><DIV id=\"descreg\" style=\"visibility:hidden;LEFT: 210px; POSITION: absolute; TOP: 45px\"><div align=\"left\"><table cellSpacing=\"0\" border='1' bordercolordark='#ffffff' cellPadding=\"0\" bgColor=\"#d9d9d9\"  width='210px' height='60px'>"+rigaDescrizione+"<tr><td  bgColor=\"#d9d9d9\" class=\"testo_grigio_sp\">"+UserManager.getRuolo(this).registri[ddl_registri.SelectedIndex].descrizione+"</td></tr></table></div></DIV><!--Fine desc reg-->");	
			} 
			catch (Exception ex) 
			{
				ErrorManager.redirect(this, ex);
			}
			
		}

		private void nuovaSchedaDocumento() 
		{
	
			schedaDocumento = DocumentManager.getDocumentoSelezionato(this);
			if (schedaDocumento == null) 
			{
				//crea nuovo documento
				DocsPaWR.Utente utente = UserManager.getUtente(this);
				DocsPaWR.Ruolo ruolo = UserManager.getRuolo(this);

				schedaDocumento = new DocsPAWA.DocsPaWR.SchedaDocumento();				
				schedaDocumento.systemId = null;
				schedaDocumento.oggetto = new DocsPAWA.DocsPaWR.Oggetto();

				// campi obbligatori per DocsFusion
				schedaDocumento.idPeople = utente.idPeople;
				schedaDocumento.userId = utente.userId;
				//schedaDocumento.typeId = "LETTERA";
				schedaDocumento.typeId = DocumentManager.getTypeId();
				schedaDocumento.appId = "ACROBAT";
				schedaDocumento.privato = "0";
				

			}
		}
		
		
		private void loadSchedaDocumento()
		{	
			try 
			{

				Response.Expires = 0;			
				nuovaSchedaDocumento();				
			} 
			catch (Exception ex) 
			{
				ErrorManager.redirect(this, ex);
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
			this.ddl_registri.Load += new System.EventHandler(this.Page_Load);
			this.ddl_registri.SelectedIndexChanged += new System.EventHandler(this.ddl_registri_SelectedIndexChanged);
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new System.EventHandler(this.GestioneDocSemplice_PreRender);

		}
		#endregion

		private void CaricaComboRegistri(DropDownList ddl)
		{
			userRegistri = UserManager.getListaRegistri(this);
			ddl.Items.Clear();
			string stato;
			for(int i=0;i<userRegistri.Length;i++)
			{
				stato = UserManager.getStatoRegistro(userRegistri[i]);
				//if (!stato.Equals("R"))
			{
				//ddl.Items.Add(userRegistri[i].descrizione);
				ddl.Items.Add(userRegistri[i].codRegistro);
				ddl.Items[i].Value=userRegistri[i].systemId;	
			}		
			}
			
			//setto lo stato del registro
			if (userRegistri.Length > 0)
				setStatoReg(userRegistri[0]);
		}

		private void ddl_registri_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			//mette in sessione il registro selezionato

			if (ddl_registri.SelectedIndex!=-1)
			{
				if (userRegistri == null)
					userRegistri = UserManager.getListaRegistri(this);
				setStatoReg(userRegistri[ddl_registri.SelectedIndex]);
				setRegistro(userRegistri[ddl_registri.SelectedIndex]);
		    			
			}
		}

	#region GestioneRegistro

		private void setStatoReg(DocsPAWA.DocsPaWR.Registro  reg)
		{
			// inserisco il registro selezionato in sessione			
			UserManager.setRegistroSelezionato(this, reg);
			string nomeImg;

			if(UserManager.getStatoRegistro(reg).Equals("G"))
				nomeImg = "stato_giallo2.gif";
			else if (UserManager.getStatoRegistro(reg).Equals("V"))
				nomeImg = "stato_verde2.gif";
			else
				nomeImg = "stato_rosso2.gif";

			this.img_statoReg.ImageUrl = "../images/" + nomeImg;
		}


		private void setRegistro(DocsPAWA.DocsPaWR.Registro  reg)
		{
			schedaDocumento = DocumentManager.getDocumentoInLavorazione(this);
			schedaDocumento.registro = reg;
			//aggiunto per risettare la data
			if (schedaDocumento.protocollo != null)
			{
				schedaDocumento.protocollo.dataProtocollazione = null;
				if(schedaDocumento.protocollo.GetType() == typeof(DocsPAWA.DocsPaWR.ProtocolloEntrata)) 
				{
					if (! corrInRegistro(((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittente, reg))
						((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittente = null;
					if(! corrInRegistro(((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittenteIntermedio, reg))
						((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittenteIntermedio = null;

				}
				else
					if(schedaDocumento.protocollo.GetType() == typeof(DocsPAWA.DocsPaWR.ProtocolloUscita)) 
				{
					// destinatari
					DocsPaWR.Corrispondente[] listaCorr = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari;
					if (listaCorr != null) 
					{
						for (int i=listaCorr.Length-1; i > -1; i--) 
						{
							if(!corrInRegistro(listaCorr[i], reg)) 
								listaCorr = UserManager.removeCorrispondente(listaCorr,i);
						}
						((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari = listaCorr;
					}
					// destinatari per conoscenza
					listaCorr = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza;
					if (listaCorr != null) 
					{
						for (int i=listaCorr.Length-1; i > -1; i--) 
						{
							if(!corrInRegistro(listaCorr[i], reg)) 
								listaCorr = UserManager.removeCorrispondente(listaCorr,i);
						}
						((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza = listaCorr;
					}
				}
			}
			
		}

		private bool corrInRegistro(DocsPAWA.DocsPaWR.Corrispondente corr, DocsPAWA.DocsPaWR.Registro reg)
		{
			if (corr == null)
				return true;
			
			if (corr.idRegistro == null || corr.idRegistro == reg.systemId ||  corr.idRegistro == "")
				return true;
			else 
				return false;
		}

		#endregion


		private void GestioneDocSemplice_PreRender(object sender, System.EventArgs e)
		{

			if (schedaDocumento.registro != null) 
			{
				setStatoReg(schedaDocumento.registro);
				//lbl_registri.Text = schedaDocumento.registro.descrizione;
				lbl_registri.Text = schedaDocumento.registro.codRegistro;
				//setto anche il valore della combobox nel caso di documenti provenienti da "Riproponi dati"
				bool trovato = false;
				for (int i=0; i<this.ddl_registri.Items.Count || !trovato; i++)
				{
					if (this.ddl_registri.Items[i].Value == schedaDocumento.registro.systemId)
					{
						this.ddl_registri.SelectedIndex = i;
						trovato = true;
					}
				}
			}

			if(schedaDocumento.protocollo == null ||  schedaDocumento.protocollo.segnatura == null || schedaDocumento.protocollo.segnatura.Equals("")) 
			{
					this.ddl_registri.Visible = true;
					this.img_statoReg.Visible = true;
					this.lbl_registri.Visible = false;
					this.icoReg.Visible = true;
				
			} 
			else 
			{
				this.ddl_registri.Visible = false;
				this.lbl_registri.Visible = true;
				this.img_statoReg.Visible = true;
				this.icoReg.Visible = true;
			}
			
			DocumentManager.setDocumentoInLavorazione(this, schedaDocumento);
			this.iFrame_sx.NavigateTo = "docProtocolloSemplice.aspx";


		}


	}
}
