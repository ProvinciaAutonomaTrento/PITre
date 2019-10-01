using System;
using System.Configuration;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace SAAdminTool.popup
{
	/// <summary>
	/// Summary description for rubrica.
	/// </summary>
    public class rubrica : SAAdminTool.CssPage
	{
		protected System.Web.UI.WebControls.RadioButtonList rbl_tipoCorr;
		protected System.Web.UI.WebControls.Label lblUO;
		protected System.Web.UI.WebControls.DropDownList DropDownList1;
		protected System.Web.UI.WebControls.TextBox TextUO;
		protected System.Web.UI.WebControls.Label lblRuolo;
		protected System.Web.UI.WebControls.DropDownList DropDownList2;
		protected System.Web.UI.WebControls.TextBox TextRuolo;
		protected System.Web.UI.WebControls.Label lblUtente;
		protected System.Web.UI.WebControls.DropDownList DropDownList3;
		protected System.Web.UI.WebControls.TextBox TextUtente;
		protected System.Web.UI.WebControls.Table tbl_Corr;
		protected System.Web.UI.WebControls.TextBox TextBox1;
		protected System.Web.UI.WebControls.Button btn_Ok;
		protected System.Web.UI.WebControls.Button btn_Chiudi;
		protected System.Web.UI.WebControls.Label Label2;
		protected System.Web.UI.HtmlControls.HtmlInputHidden h_corrispondenti;
	
		//my var
		
		protected System.Web.UI.WebControls.RadioButtonList rbl_corrispondente;
		protected TableRow tr;
		protected TableCell tc;
        private SAAdminTool.DocsPaWR.Corrispondente[] listaCorr;
		protected string wnd; 
		string msg_noresult;
		protected string target;
		protected RadioButtonList rb_Corr;
		protected System.Web.UI.WebControls.Label lbl_tipoCorr;
		protected System.Web.UI.WebControls.ImageButton btn_ricerca;
		protected System.Web.UI.WebControls.Label lbl_message;
		protected System.Web.UI.WebControls.Label Label1;
		protected System.Web.UI.WebControls.HyperLink HyperLink1;
		protected CheckBoxList cbl_Corr;
	
		private void Page_Load(object sender, System.EventArgs e) {
			try {
				// chi mi chiama
				wnd = Request.QueryString["wnd"];
				// se wnd=proto: corrispondente (mitt, mittInt o dest)
				target = Request.QueryString["target"];
				msg_noresult = "Corrispondenti non trovati";
				if (wnd!=null) {
					if (wnd == "proto" || wnd == "ric_E" || wnd == "ric_C" || wnd == "ric_CT") {
						this.rbl_tipoCorr.Visible = true; 
						this.lbl_tipoCorr.Visible = true;		
					}else 
					if (wnd == "trasm")
						msg_noresult = "Non esistono destinatari compatibili con la ragione selezionata";		
					
				}

				queryExec();
                listaCorr = (SAAdminTool.DocsPaWR.Corrispondente[])Session["rubrica.listaCorr"];

				if(listaCorr != null)
					this.DrawTable();
			} catch (Exception ex) {
				ErrorManager.redirect(this, ex);
			}
			
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e) {
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
		private void InitializeComponent() {    
			this.btn_ricerca.Click += new System.Web.UI.ImageClickEventHandler(this.btn_ricerca_Click);
			this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
			this.btn_Chiudi.Click += new System.EventHandler(this.btn_Chiudi_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void queryExec() {
			string codice = Request.QueryString["codice"];
			string typeQuery = Request.QueryString["typeQ"];
			string wnd = Request.QueryString["wnd"];

			if (codice!=null && typeQuery!=null) {
				if (typeQuery.Equals("C"))  //C= search Child
					queryC_Exec(codice, wnd);
			}
		}
		
		protected string checkFields() {
			string msg = "";

			if (this.TextUO.Text.Equals("") && this.TextRuolo.Text.Equals("") && this.TextUtente.Text.Equals("")) {
				msg = "Inserire almeno un criterio di ricerca";
			}
			return msg;
		}
		
		
		protected string queryD_Exec(string systemID)
		{
			DocsPaVO.addressbook.DettagliCorrispondente dettagliCorr = UserManager.getDettagliCorrispondente(this, systemID);
			
			string tab_Dettagli = "";
			if (dettagliCorr != null)
			{
				
				tab_Dettagli = " <table border='1' cellpadding='5' bordercolor='#800000' bordercolordark='#FFFFFF' bordercolorlight='#C0C0C0'>";
				//nome indirizzo
				tab_Dettagli += "<tr>";
					tab_Dettagli += "<td class='menu_1_grigio' background='../images/bg_pattern.gif'>INDIRIZZO</td>";
					tab_Dettagli += "<td class='testo_grigio' background='../images/bg_pattern.gif'>"+ dettagliCorr.Corrispondente[0].indirizzo+"</td>";
				tab_Dettagli += "</tr>";
				//nome cap
				tab_Dettagli += "<tr>";
				tab_Dettagli += "<td class='menu_1_grigio' background='../images/bg_pattern.gif'>CAP</td>";
				tab_Dettagli += "<td class='testo_grigio' background='../images/bg_pattern.gif'>"+ dettagliCorr.Corrispondente[0].cap+"</td>";
				tab_Dettagli +="</tr>";
				//nome città
				tab_Dettagli += "<tr>";
				tab_Dettagli += "<td class='menu_1_grigio' background='../images/bg_pattern.gif'>CITTA'</td>";
				tab_Dettagli += "<td class='testo_grigio' background='../images/bg_pattern.gif'>"+ dettagliCorr.Corrispondente[0].citta+"</td>";
				tab_Dettagli += "</tr>";
				//nome provincia
				tab_Dettagli += "<tr>";
				tab_Dettagli += "<td class='menu_1_grigio' background='../images/bg_pattern.gif'>PROVINCIA</td>";
				tab_Dettagli += "<td class='testo_grigio' background='../images/bg_pattern.gif'>"+ dettagliCorr.Corrispondente[0].provincia+"</td>";
				tab_Dettagli += "</tr>";
				//nome nazione
				tab_Dettagli += "<tr>";
				tab_Dettagli += "<td class='menu_1_grigio' background='../images/bg_pattern.gif'>NAZIONE</td>";
				tab_Dettagli += "<td class='testo_grigio' background='../images/bg_pattern.gif'>"+ dettagliCorr.Corrispondente[0].nazione+"</td>";
				tab_Dettagli += "</tr>";
				//nome telefono
				tab_Dettagli = "<tr>";
					tab_Dettagli += "<td class='menu_1_grigio' background='../images/bg_pattern.gif'>TELEFONO</td>";
					tab_Dettagli += "<td class='testo_grigio' background='../images/bg_pattern.gif'>"+ dettagliCorr.Corrispondente[0].telefono+"</td>";
				tab_Dettagli += "</tr>";
				//nome telefono 2
				tab_Dettagli += "<tr>";
					tab_Dettagli += "<td class='menu_1_grigio' background='../images/bg_pattern.gif'>TELEFONO 2</td>";
					tab_Dettagli += "<td class='testo_grigio' background='../images/bg_pattern.gif'>"+ dettagliCorr.Corrispondente[0].telefono2+"</td>";
				tab_Dettagli += "</tr>";
				//nome fax
				tab_Dettagli += "<tr>";
					tab_Dettagli += "<td class='menu_1_grigio' background='../images/bg_pattern.gif'>FAX</td>";
					tab_Dettagli += "<td class='testo_grigio' background='../images/bg_pattern.gif'>"+ dettagliCorr.Corrispondente[0].fax+"</td>";
				tab_Dettagli += "</tr>";
				tab_Dettagli += "</table>";
			}
	
		return tab_Dettagli;
		
		}


		private void queryC_Exec(string codiceRubrica, string wnd) {

			//costruzione oggetto queryCorrispondente
			DocsPaWR.AddressbookQueryCorrispondente qco = new SAAdminTool.DocsPaWR.AddressbookQueryCorrispondente();
		
			if (codiceRubrica.Equals("")) {
				if(!this.TextUO.Text.Equals("")) {
					if(this.DropDownList1.SelectedItem.Value.Equals("C")) 
						qco.codiceUO=this.TextUO.Text;
					else 
						qco.descrizioneUO=this.TextUO.Text;
				}
				if(!this.TextRuolo.Text.Equals("")) {
					qco.descrizioneRuolo=this.TextRuolo.Text;
				}
				if(!this.TextUtente.Text.Equals("")) {
					if(this.DropDownList3.SelectedItem.Value.Equals("N"))
						qco.nomeUtente=this.TextUtente.Text;
					else 
						qco.cognomeUtente=this.TextUtente.Text;
				}
			} 
			else {
				qco.codiceRubrica = codiceRubrica;
				qco.getChildren = true;
			}

			qco.idAmministrazione= ConfigSettings.getKey(ConfigSettings.KeysENUM.ID_AMMINISTRAZIONE);
			
			qco.idRegistri = UserManager.getListaIdRegistri(this); //rappresenta il registro selezionato dall'utente e non quello del documento
        
			if (wnd!=null){
				if (wnd == "proto" || wnd == "ric_E" || wnd == "ric_C" || wnd == "ric_CT") 
				{
					if (this.rbl_tipoCorr.SelectedItem.Value.Equals("I"))
						qco.tipoUtente=DocsPaWR.AddressbookTipoUtente.INTERNO;
					else
						if (this.rbl_tipoCorr.SelectedItem.Value.Equals("E"))
						qco.tipoUtente=DocsPaWR.AddressbookTipoUtente.ESTERNO;
					else
						qco.tipoUtente=DocsPaWR.AddressbookTipoUtente.GLOBALE;				
				}
			}
			else {  
			//corrispondenti interni
				qco.tipoUtente=DocsPaWR.AddressbookTipoUtente.INTERNO;			
			}
		
			listaCorr = searchCorr(qco, wnd);

			Session["rubrica.listaCorr"] = listaCorr;

		}

		private SAAdminTool.DocsPaWR.Corrispondente[] searchCorr(SAAdminTool.DocsPaWR.AddressbookQueryCorrispondente qco, string wnd) 
		{		
			if ((wnd != null && (wnd.Equals("proto") || wnd == "ric_E" || wnd == "ric_C" || wnd == "ric_CT")) || !(qco.codiceRubrica == null || qco.codiceRubrica.Equals("")))
				return UserManager.getListaCorrispondenti(this.Page,qco);	
			else {
				// c'è bisogno di ragione della trasmissione e dell'id del documento !!!!!
				
				DocsPaWR.AddressbookQueryCorrispondenteAutorizzato qcAut = new SAAdminTool.DocsPaWR.AddressbookQueryCorrispondenteAutorizzato();
				//qcAut.idRegistro = ""; non ho l'id del registro in InfoDoc quindi per ora non lo inserisco
				qcAut.tipoOggetto = SAAdminTool.DocsPaWR.TrasmissioneTipoOggetto.DOCUMENTO;
				
				//cerco la ragione in base all'id che ho nella querystring
				string index = Request.QueryString["index"];
				
				qcAut.ragione = TrasmManager.getRagioneSel(this);
				qcAut.ruolo = (SAAdminTool.DocsPaWR.Ruolo) Session["userRuolo"];
				qcAut.queryCorrispondente = qco;
				qcAut.idRegistro = qco.idRegistri[0];
				return UserManager.getListaCorrispondentiAutorizzati(this, qcAut); 									
			}
		}

		
		private void DrawTable() {
			cbl_Corr=new CheckBoxList();
			tbl_Corr.Controls.Clear();
			rb_Corr = new RadioButtonList();
			rb_Corr.ID = "rb_Corr";
			rb_Corr.CssClass="menu_1_grigio";

			for(int i=0;i<listaCorr.Length;i++) {
				DocsPaWR.Corrispondente myCorr=(SAAdminTool.DocsPaWR.Corrispondente)listaCorr[i];
				//Cell CheckBox
				tr=new TableRow();
				tc =new TableCell();
				tc.CssClass="testo_grigio";
				if (target != null && target != "" && target != "mitt" && target != "mittInt") {
					CheckBox  cb_Corr=new CheckBox();
					cb_Corr.CssClass="menu_1_grigio";
					cb_Corr.Text = getdecrizioneCorr(myCorr);
					//cb_Corr=myCorr.codiceRubrica;
					tc.Controls.Add(cb_Corr);
				} else {
					rb_Corr.Items.Add(getdecrizioneCorr(myCorr));
					tc.Controls.Add(rb_Corr);
				}

				tr.Cells.Add(tc);
				
				tbl_Corr.Rows.Add(tr);
				//Cell 
			}
			
		}

		private string getdecrizioneCorr(SAAdminTool.DocsPaWR.Corrispondente myCorr) {
			string link_ut = "";
			if (myCorr.GetType() == typeof(SAAdminTool.DocsPaWR.Ruolo)) {
				DocsPaWR.Ruolo corrRuolo = (SAAdminTool.DocsPaWR.Ruolo) myCorr;
				//inserisco il link per visualizzare gli utenti del ruolo
				link_ut = "<a href='rubrica.aspx?codice="+corrRuolo.codiceRubrica + "&TypeQ=C&wnd="+wnd+"&target="+target+"'><img src='../images/info.gif' border='0'/></a>";
				
			} 
			return link_ut + UserManager.getDecrizioneCorrispondente(this,myCorr);
		}
		
		private void btn_Ok_Click(object sender, System.EventArgs e) {					
			try {
					if (wnd!=null && wnd.Equals("proto")) 		
						setCorrispondentiProtocollo();
					else 
						if (wnd!=null && (wnd.Equals("ric_E") ||wnd.Equals("ric_C") || wnd.Equals("ric_CT") ))
						setCorrispondentiRicerca();
					else
						if (wnd!=null && wnd.Equals("trasm"))
						setCorrispondentiTrasmissione();

					
				} 
				catch (Exception ex) 
				{
					ErrorManager.redirect(this, ex);
				}

		}

		private void setCorrispondentiProtocollo() {
			DocsPaWR.Corrispondente corr;
			if (target == null)
				return;
			//gestione documento
			//DocsPaWR.SchedaDocumento schedaDoc = (SAAdminTool.DocsPaWR.SchedaDocumento)Session["docProtocollo.schedaDocumento"];
			DocsPaWR.SchedaDocumento schedaDoc = DocumentManager.getDocumentoInLavorazione(this);
			
			//controlla i corrispondenti selezionati, li mette in una hashTable e poi carica la pagina 
			if(target.Equals("dest")) {
				// Protocollo in uscita
				for(int i=0;i<this.tbl_Corr.Rows.Count;i++) { 
					CheckBox AppoChk =(CheckBox)this.tbl_Corr.Rows[i].Cells[0].Controls[0];
					if(AppoChk.Checked) {
						corr = (SAAdminTool.DocsPaWR.Corrispondente)listaCorr[i];
						schedaDoc = addDestinatari(schedaDoc, corr);
						
						((SAAdminTool.DocsPaWR.ProtocolloUscita)schedaDoc.protocollo).daAggiornareDestinatari = true;
					}
				}
			} else if (target.Equals("mitt") || target.Equals("mittInt")) {
				// Protocollo in entrata
				int selIndex = rb_Corr.SelectedIndex;
				if (selIndex > -1) 
					corr = listaCorr[selIndex];
				else 
					corr = null;
				if(target.Equals("mitt")) {
					((SAAdminTool.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo).mittente = corr;
					((SAAdminTool.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo).daAggiornareMittente = true;
				}
				else {
					((SAAdminTool.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo).mittenteIntermedio = corr;
					
					((SAAdminTool.DocsPaWR.ProtocolloEntrata)schedaDoc.protocollo).daAggiornareMittenteIntermedio = true;
				}
			}

			//Session["docProtocollo.schedaDocumento"] = schedaDoc;
			DocumentManager.setDocumentoInLavorazione(this,schedaDoc);
			Session.Remove("rubrica.listaCorr");
			Response.Write("<script>var k=window.open('../documento/docProtocollo.aspx','IframeTabs'); window.close();</script>");

		}

		private void setCorrispondentiTrasmissione() {
			
			//gestione trasmissione
			DocsPaWR.Trasmissione trasmissione = TrasmManager.getGestioneTrasmissione(this);
						
			//controlla i corrispondenti selezionati, li mette nella trasmisione e poi carica la pagina con i dettagli

			for(int i=0;i<this.tbl_Corr.Rows.Count;i++) { 
				CheckBox AppoChk=(CheckBox)this.tbl_Corr.Rows[i].Cells[0].Controls[0];
				if(AppoChk.Checked) {
					DocsPaWR.Corrispondente corr = (SAAdminTool.DocsPaWR.Corrispondente)listaCorr[i];
					trasmissione = addTrasmissioneSingola(trasmissione, corr);
				}
			}
			
			TrasmManager.setGestioneTrasmissione(this, trasmissione);
			Session.Remove("rubrica.listaCorr");
			
				Response.Write("<script>window.opener.top.principale.frames[1].document.trasmDatiTrasm_dx.submit();window.close();</script>");
			
			//	Response.Write("<script>window.open('../trasmissione/trasmDatiTrasm_dx.aspx','iFrame_dx'); window.close();</script>");

		}
		

			private void setCorrispondentiRicerca(){
				UserManager.removeCorrispondentiSelezionati(this);
				// Corrispondente per Ricerca
				DocsPaWR.Corrispondente corr;
				int selIndex = rb_Corr.SelectedIndex;
				if (selIndex > -1) 
					corr = listaCorr[selIndex];
				else 
					corr = null;

				if (wnd.Equals("ric_C") && (target != null && target.Equals("mittInt")))
					UserManager.setCorrispondenteIntSelezionato(this, corr);
				else
					UserManager.setCorrispondenteSelezionato(this, corr);
			
				Session.Remove("rubrica.listaCorr");

				Response.Write("<script>var k=window.opener.document.forms[0].submit(); window.close();</script>");

			}

		//DOCUMENTI
		private SAAdminTool.DocsPaWR.SchedaDocumento addDestinatari(SAAdminTool.DocsPaWR.SchedaDocumento schedaDoc, SAAdminTool.DocsPaWR.Corrispondente corr) {
			//controlo se esiste già il corrispondente selezionato
			DocsPaWR.Corrispondente[] listaDest;

			listaDest = ((SAAdminTool.DocsPaWR.ProtocolloUscita)schedaDoc.protocollo).destinatari;
			
			if (UserManager.esisteCorrispondente(listaDest,corr))
				return schedaDoc;
			if (UserManager.esisteCorrispondente(((SAAdminTool.DocsPaWR.ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza,corr))
				return schedaDoc;

			//aggiungo il corrispondente
			//di default lo aggiungo tra i destinatari principali
			if (schedaDoc.protocollo != null)
				((SAAdminTool.DocsPaWR.ProtocolloUscita)schedaDoc.protocollo).destinatari = UserManager.addCorrispondente(listaDest, corr);

			return schedaDoc;
		}

		//TASMISSIONI
		private SAAdminTool.DocsPaWR.Trasmissione addTrasmissioneSingola(SAAdminTool.DocsPaWR.Trasmissione trasmissione, SAAdminTool.DocsPaWR.Corrispondente corr) {
			
			if (trasmissione.trasmissioniSingole != null)
			{
				// controllo se esiste la trasmissione singola associata a corrispondente selezionato
				for(int i = 0; i < trasmissione.trasmissioniSingole.Length; i++) 
				{
					DocsPaWR.TrasmissioneSingola ts = (SAAdminTool.DocsPaWR.TrasmissioneSingola)trasmissione.trasmissioniSingole[i];
					if (ts.corrispondenteInterno.systemId.Equals(corr.systemId)) 
					{
						if(ts.daEliminare) 
						{
							((SAAdminTool.DocsPaWR.TrasmissioneSingola)trasmissione.trasmissioniSingole[i]).daEliminare = false;
							return trasmissione;
						}
						else
							return trasmissione;
					}
				}			
			}
			// Aggiungo la trasmissione singola
			DocsPaWR.TrasmissioneSingola trasmissioneSingola = new SAAdminTool.DocsPaWR.TrasmissioneSingola();
			trasmissioneSingola.tipoTrasm = "S";
			trasmissioneSingola.corrispondenteInterno = corr;
			trasmissioneSingola.ragione = TrasmManager.getRagioneSel(this);
			
			// Aggiungo la lista di trasmissioniUtente
			if( corr.GetType() == typeof(SAAdminTool.DocsPaWR.Ruolo)) {
				trasmissioneSingola.tipoDest = SAAdminTool.DocsPaWR.TrasmissioneTipoDestinatario.RUOLO;
				DocsPaWR.Corrispondente[] listaUtenti = queryUtenti(corr);
				//ciclo per utenti se dest è gruppo o ruolo
				for(int i= 0; i < listaUtenti.Length; i++) {
					DocsPaWR.TrasmissioneUtente trasmissioneUtente = new SAAdminTool.DocsPaWR.TrasmissioneUtente();
					trasmissioneUtente.utente = (SAAdminTool.DocsPaWR.Utente) listaUtenti[i];
					trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
				}
			}
			else {
				trasmissioneSingola.tipoDest = SAAdminTool.DocsPaWR.TrasmissioneTipoDestinatario.UTENTE;
				DocsPaWR.TrasmissioneUtente trasmissioneUtente = new SAAdminTool.DocsPaWR.TrasmissioneUtente();
				trasmissioneUtente.utente = (SAAdminTool.DocsPaWR.Utente) corr;
				trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
			}
			trasmissione.trasmissioniSingole = TrasmManager.addTrasmissioneSingola(trasmissione.trasmissioniSingole, trasmissioneSingola);

			return trasmissione;

		}

		private SAAdminTool.DocsPaWR.Corrispondente[] queryUtenti(SAAdminTool.DocsPaWR.Corrispondente corr) {
			
			//costruzione oggetto queryCorrispondente
			DocsPaWR.AddressbookQueryCorrispondente qco = new SAAdminTool.DocsPaWR.AddressbookQueryCorrispondente();

			qco.codiceRubrica = corr.codiceRubrica;
			qco.getChildren = true;
		
			qco.idAmministrazione= ConfigSettings.getKey(ConfigSettings.KeysENUM.ID_AMMINISTRAZIONE);
			
			//corrispondenti interni
			qco.tipoUtente=DocsPaWR.AddressbookTipoUtente.INTERNO;			
			
			return UserManager.getListaCorrispondenti(this.Page,qco);
		}
		
		private void btn_Chiudi_Click(object sender, System.EventArgs e) {
			try {			
				Session.Remove("rubrica.listaCorr");
				Response.Write("<script>window.close();</script>");
			} catch (Exception ex) {
				ErrorManager.redirect(this, ex);
			}

		}

		private void btn_ricerca_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
				try{
				string msg = checkFields();
				if (msg.Equals("")) {
					Session.Remove("rubrica.listaCorr");
					queryC_Exec("",wnd);
					if(listaCorr != null && listaCorr.Length > 0)
					{
						this.DrawTable();
						this.lbl_message.Visible = false;
						this.btn_Ok.Visible = true;
					}
					else
					{
						this.lbl_message.Text = msg_noresult;
						this.lbl_message.Visible = true;
						this.btn_Ok.Visible = false;
					}
				} 
				else {
					Response.Write("<script>alert('" + msg + "');</script>");	

				}
			} catch (Exception ex) {
				ErrorManager.redirect(this, ex);
			}

		}


	}
}
