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
using System.Xml;
using System.Xml.Serialization;
using System.Web.Services.Protocols;
using Microsoft.Web.UI.WebControls;
using System.Globalization;
using System.Configuration;

namespace DocsPAWA.fascicolo
{
	/// <summary>
	/// Summary description for fascDettagliFasc.
	/// </summary>
	public class fascDettagliFasc : DocsPAWA.CssPage
	{
		protected System.Web.UI.WebControls.Label Label1;
        protected System.Web.UI.WebControls.Panel pnl_profilazione;
        protected System.Web.UI.WebControls.TextBox txt_tipoFasc;
        protected System.Web.UI.WebControls.ImageButton img_dettagliProfilazioneFasc;
		protected System.Web.UI.WebControls.TextBox txt_ClassFasc;
		protected System.Web.UI.WebControls.Label Label2;
		protected System.Web.UI.WebControls.TextBox txt_fascdesc;
		protected System.Web.UI.WebControls.Label lbl_dataAp;
		protected System.Web.UI.WebControls.TextBox txt_fascApertura;
		protected System.Web.UI.WebControls.Label lbl_dataC;
		protected System.Web.UI.WebControls.TextBox txt_FascChiusura;
		protected System.Web.UI.WebControls.Label Label5;
		protected System.Web.UI.WebControls.TextBox txt_Fasctipo;
		protected System.Web.UI.WebControls.Label Label6;
		protected System.Web.UI.WebControls.TextBox txt_fascStato;
		protected System.Web.UI.WebControls.Label Label3;
		protected System.Web.UI.WebControls.TextBox txt_descrizione;
		protected System.Web.UI.WebControls.Label Label8;
		protected System.Web.UI.WebControls.TextBox txt_fascnote;
		protected System.Web.UI.WebControls.Label Label7;
		protected Microsoft.Web.UI.WebControls.TreeView Folders;

        protected System.Web.UI.WebControls.Label lblFascicoloCartaceo;
        protected System.Web.UI.WebControls.Label lblFascicoloPrivato;
        protected System.Web.UI.WebControls.CheckBox chkFascicoloCartaceo;
        protected System.Web.UI.WebControls.CheckBox chkFascicoloPrivato;


		protected DocsPAWA.DocsPaWR.Fascicolo fascicolo;
		protected Hashtable HashFolder;
		private int indexH;
		protected DocsPaWebCtrlLibrary.ImageButton btn_aggiungi;
//		protected DocsPaWebCtrlLibrary.ImageButton btn_insDoc;
		protected DocsPaWebCtrlLibrary.ImageButton btn_rimuovi;
		//private int idFolderSelezionato;
		protected DocsPAWA.DocsPaWR.InfoUtente infoUt;
		protected DocsPAWA.DocsPaWR.Ruolo userRuolo;
		protected System.Web.UI.WebControls.Label Label4;
		protected System.Web.UI.WebControls.TextBox txt_cod_uff_ref;
		protected System.Web.UI.WebControls.TextBox txt_desc_uff_ref;
		protected System.Web.UI.WebControls.Panel pnl_uffRef;
		protected DocsPAWA.DocsPaWR.Utente userHome;
		protected DocsPAWA.DocsPaWR.Utente utente;
		protected DocsPAWA.DocsPaWR.Ruolo ruolo;
		protected DocsPAWA.DocsPaWR.Trasmissione trasmissione;
		protected System.Web.UI.WebControls.Label lbl_lf;
		protected System.Web.UI.WebControls.TextBox txt_cod_lf;
		protected System.Web.UI.WebControls.TextBox txt_desc_lf;
		protected System.Web.UI.WebControls.Label Label9;
		protected System.Web.UI.WebControls.TextBox txt_dta_lf;
        protected DocsPAWA.DocsPaWR.DocsPaWebService wws;
		private bool enableUfficioRef =(ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF)!= null 
			&& ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF).Equals("1"));

      protected System.Web.UI.HtmlControls.HtmlInputHidden hd_returnValueModal;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			fascicolo = FascicoliManager.getFascicoloSelezionato(this);
			if (fascicolo == null)
			{
				Response.Write("<SCRIPT>top.principale.iFrame_dx.iFrame_cn.document.location='../blank_page.htm';</SCRIPT>");
				return;
			}

            //Profilazione dinamica fascicoli
            if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1" && fascicolo.tipo.Equals("P"))
            {
                pnl_profilazione.Visible = true;
                if (fascicolo.template != null)
                {
                    txt_tipoFasc.Text = fascicolo.template.DESCRIZIONE; ;
                    fascicolo.template = ProfilazioneFascManager.getTemplateFascDettagli(fascicolo.systemID,this);
                    FascicoliManager.setFascicoloSelezionato(fascicolo);
                }
            }
            //Fine Profilazione dinamica fascicoli
		
			userRuolo= UserManager.getRuolo(this);
			userHome =UserManager.getUtente(this);
			infoUt=UserManager.getInfoUtente(this);
			
			if(!Page.IsPostBack)
			{
				HashFolder=new Hashtable();
				if (this.Page.Session["fascDettagliFasc.nodoSelezionato"]==null)
				{
					this.Page.Session["fascDettagliFasc.nodoSelezionato"]=getSelectedNodeFolder();
				}
				setInfoFascicolo();
				
			}
			else
			{
				this.Page.Session.Remove("fascDettagliFasc.nodoSelezionato");
				HashFolder=FascicoliManager.getHashFolder(this);
			}

			//selezionaUltimoNodoSelezionato();
			//controllo se devo creare una nuova cartella
			if (Session["descNewFolder"]!=null)
			{
                DocsPAWA.DocsPaWR.ResultCreazioneFolder result;
                if (!this.CreateNewFolder(out result))
                {
                    // Visualizzazione messaggio di errore
                    string errorMessage = string.Empty;
                    if (result == DocsPAWA.DocsPaWR.ResultCreazioneFolder.FOLDER_EXIST)
                        errorMessage = "Il sottofascicolo richiesto è già presente e non può essere duplicato";
                    else
                        errorMessage = "Errore nella creazione del sottofascicolo";

                    Response.Write(string.Format("<script>alert('{0}');</script>", errorMessage));
                }

				Session.Remove("descNewFolder");
			}

         //Seleziono il folder selezionato da ricerca sottofascicolo

         if (hd_returnValueModal.Value == "Y") //ritorno dalla modale di ricerca dei sottofascicoli
            SelezionaSottofascicolo();
         else
            selezionaUltimoNodoSelezionato();
			

			Folders.SelectExpands = true;

			if(enableUfficioRef)
			{
				this.pnl_uffRef.Visible = true;
				if(fascicolo.ufficioReferente != null)
				{
					DocsPaWR.Corrispondente corrRef = UserManager.getCorrispondenteBySystemID(this,fascicolo.ufficioReferente.systemId);
					this.txt_cod_uff_ref.Text = corrRef.codiceRubrica;
					this.txt_desc_uff_ref.Text = corrRef.descrizione;
					fascicolo.ufficioReferente =corrRef;
					FascicoliManager.setFascicoloSelezionato(this,fascicolo);
					// trasmetti a UO referente solo se vengo da nuovo fascicolo
					//se vengo dal dettaglio del fascicolo la trasmissione non deve partire
                    if (Request.QueryString["newFasc"] != null && this.Request.QueryString["newFasc"].ToString().Equals("1"))
                    {
						//effettuo la trasmissione dopo la creazione di un nuovo fascicolo(provendo da doc classifica)
						//Invia la trasmissione ai ruoli di riferimento dell'Ufficio Referente
						if(!getRagTrasmissioneUfficioReferente())
						{
							string theAlert = "<script>alert('Attenzione! Ragione di trasmissione assente per l\\'Ufficio referente.";
							theAlert = theAlert + "\\nLa trasmissione non è stata effettuata.');</script>";
							Response.Write(theAlert);
						}
						else
						{
							string esito = setCorrispondentiTrasmissione();
							if (!esito.Equals(""))
							{
								esito = esito.Replace("'", "''");
								Page.RegisterStartupScript("chiudi","<script>alert('" + esito + "')</script>");
								esito = "";
							}
							else
							{
								//richiamo il metodo che salva la trasmissione
                                DocsPAWA.DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente();
                                if (infoUtente.delegato != null)
                                    trasmissione.delegato = ((DocsPAWA.DocsPaWR.InfoUtente)(infoUtente.delegato)).idPeople;

                                //Nuovo metodo saveExecuteTrasm
                                trasmissione.daAggiornare = false;
                                DocsPaWR.Trasmissione trasm_res = TrasmManager.saveExecuteTrasm(this, trasmissione, infoUtente);
                                //trasmissione = TrasmManager.saveTrasm(this, trasmissione);
                                //trasmissione.daAggiornare = false;
                                //DocsPaWR.Trasmissione trasm_res = TrasmManager.executeTrasm(this,trasmissione);

								if(trasm_res!=null && trasm_res.ErrorSendingEmails)
									Response.Write("<script>window.alert('Non è stato possibile inoltrare una o più e-mail. \\nContattare l\\'amministratore per risolvere il problema.');</script>");
					
							}
						}
						//rimozione variabili di sessione
						TrasmManager.removeGestioneTrasmissione(this);
						TrasmManager.removeRagioneSel(this);
						FascicoliManager.removeUoReferenteSelezionato(this);
//						FascicoliManager.removeFascicoloSelezionato(this);
						//Rimuovo la variabile di sessione dopo la trasmissione
						
					}
				}
			}
			else
			{
				this.pnl_uffRef.Visible = false;
			}
            //Session.Remove("newFasc");
		}

		#region TRASMISSIONE FASCICOLO A UFFICIO REFERENTE

		private bool getRagTrasmissioneUfficioReferente()
		{
			bool retValue = true;
			bool verificaRagioni;
			trasmissione = TrasmManager.getGestioneTrasmissione(this);
			utente = UserManager.getUtente(this);
			ruolo = UserManager.getRuolo(this);

			//se è null la trasmissione è nuova altrimenti è in modifica
			if (trasmissione == null)
			{
				trasmissione = new DocsPAWA.DocsPaWR.Trasmissione();
				trasmissione.systemId = null;
				trasmissione.ruolo = ruolo;
				trasmissione.utente = utente;
				trasmissione.tipoOggetto = DocsPAWA.DocsPaWR.TrasmissioneTipoOggetto.FASCICOLO;
				trasmissione.infoDocumento = null;
				trasmissione.infoFascicolo = FascicoliManager.getInfoFascicoloDaFascicolo(FascicoliManager.getFascicoloSelezionato(this),this);
				TrasmManager.setGestioneTrasmissione(this, trasmissione);
			}
	
			DocsPaWR.RagioneTrasmissione ragTrasm = null;
	
			ragTrasm = FascicoliManager.TrasmettiFascicoloToUoReferente(ruolo, out verificaRagioni); 
			
			if(ragTrasm == null && !verificaRagioni)
			{
				retValue = false;	
			}	
			else
			{
				TrasmManager.setRagioneSel(this,ragTrasm);
			}
			return retValue;
		}
		private string setCorrispondentiTrasmissione() 
		{
			string esito="";
			try
			{
				
				DocsPaWR.Trasmissione trasmissione = TrasmManager.getGestioneTrasmissione(this);
				//creo l'oggetto qca in caso di trasmissioni a UO
				DocsPaWR.AddressbookQueryCorrispondente qco = new DocsPAWA.DocsPaWR.AddressbookQueryCorrispondente();
				qco.fineValidita = true;
				DocsPaWR.AddressbookQueryCorrispondenteAutorizzato qca = setQCA(qco);
				DocsPaWR.Corrispondente corrRef = FascicoliManager.getUoReferenteSelezionato(this);	
				if (corrRef != null)
				{
					// se siamo di fronte ad una UO, ne individuiamo i ruoli e trasmettiamo ai relativi utenti
					DocsPaWR.Ruolo[] listaRuoli = UserManager.getRuoliRiferimentoAutorizzati(this,qca,(DocsPAWA.DocsPaWR.UnitaOrganizzativa)corrRef);
					if (listaRuoli!=null && listaRuoli.Length>0)
					{
						for (int index=0; index < listaRuoli.Length; index++)
							trasmissione = addTrasmissioneSingola(trasmissione, (DocsPAWA.DocsPaWR.Ruolo) listaRuoli[index]);
					}
					else
					{
						if (esito.Equals("")) 
							esito += "Trasmissione non effettuata - ruoli di riferimento non autorizzati nella UO:\\n";
						esito += "\\nUO: " + corrRef.descrizione;
					}
				}
		
				TrasmManager.setGestioneTrasmissione(this, trasmissione);
	
			}
			catch(Exception ex)
			{
				ErrorManager.redirectToErrorPage(this,ex);
			}	
			return esito;
		}
	
		private DocsPAWA.DocsPaWR.AddressbookQueryCorrispondenteAutorizzato setQCA(DocsPAWA.DocsPaWR.AddressbookQueryCorrispondente qco)
		{
			DocsPaWR.AddressbookQueryCorrispondenteAutorizzato qcAut = new DocsPAWA.DocsPaWR.AddressbookQueryCorrispondenteAutorizzato();
			
			qcAut.tipoOggetto = DocsPAWA.DocsPaWR.TrasmissioneTipoOggetto.FASCICOLO;			
				qcAut.idNodoTitolario = FascicoliManager.getFascicoloSelezionato(this).idClassificazione;
				qcAut.idRegistro = FascicoliManager.getFascicoloSelezionato(this).idRegistroNodoTit;
				if (qcAut.idRegistro != null && qcAut.idRegistro.Equals(""))
					qcAut.idRegistro = null;
			
			//cerco la ragione in base all'id che ho nella querystring
			qcAut.ragione = TrasmManager.getRagioneSel(this);
			if (TrasmManager.getGestioneTrasmissione(this) != null)
			{
				qcAut.ruolo = TrasmManager.getGestioneTrasmissione(this).ruolo;
			}
			qcAut.queryCorrispondente = qco;	
			return qcAut;
		}

		private DocsPAWA.DocsPaWR.Trasmissione addTrasmissioneSingola(DocsPAWA.DocsPaWR.Trasmissione trasmissione, DocsPAWA.DocsPaWR.Corrispondente corr) 
		{
			
			if (trasmissione.trasmissioniSingole != null)
			{
				// controllo se esiste la trasmissione singola associata a corrispondente selezionato
				for(int i = 0; i < trasmissione.trasmissioniSingole.Length; i++) 
				{
					DocsPaWR.TrasmissioneSingola ts = (DocsPAWA.DocsPaWR.TrasmissioneSingola)trasmissione.trasmissioniSingole[i];
					if (ts.corrispondenteInterno.systemId.Equals(corr.systemId)) 
					{
						if(ts.daEliminare) 
						{
							((DocsPAWA.DocsPaWR.TrasmissioneSingola)trasmissione.trasmissioniSingole[i]).daEliminare = false;
							return trasmissione;
						}
						else
							return trasmissione;
					}
				}			
			}
			// Aggiungo la trasmissione singola
			DocsPaWR.TrasmissioneSingola trasmissioneSingola = new DocsPAWA.DocsPaWR.TrasmissioneSingola();
			trasmissioneSingola.tipoTrasm = "S";
			trasmissioneSingola.corrispondenteInterno = corr;
			trasmissioneSingola.ragione = TrasmManager.getRagioneSel(this);
			
			// Aggiungo la lista di trasmissioniUtente
			if( corr.GetType() == typeof(DocsPAWA.DocsPaWR.Ruolo)) 
			{
				trasmissioneSingola.tipoDest = DocsPAWA.DocsPaWR.TrasmissioneTipoDestinatario.RUOLO;
				DocsPaWR.Corrispondente[] listaUtenti = queryUtenti(corr.codiceRubrica);
				if (listaUtenti == null || listaUtenti.Length == 0)
					return trasmissione;
				//ciclo per utenti se dest è gruppo o ruolo
				for(int i= 0; i < listaUtenti.Length; i++) 
				{
					DocsPaWR.TrasmissioneUtente trasmissioneUtente = new DocsPAWA.DocsPaWR.TrasmissioneUtente();
					trasmissioneUtente.utente = (DocsPAWA.DocsPaWR.Utente) listaUtenti[i];
					if(TrasmManager.getRagioneSel(this).descrizione.Equals("RISPOSTA"))
						trasmissioneUtente.idTrasmRispSing=trasmissioneSingola.systemId;
					trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
				}
			}
			else 
			{
				trasmissioneSingola.tipoDest = DocsPAWA.DocsPaWR.TrasmissioneTipoDestinatario.UTENTE;
				DocsPaWR.TrasmissioneUtente trasmissioneUtente = new DocsPAWA.DocsPaWR.TrasmissioneUtente();
				trasmissioneUtente.utente = (DocsPAWA.DocsPaWR.Utente) corr;
				trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
			}
			trasmissione.trasmissioniSingole = TrasmManager.addTrasmissioneSingola(trasmissione.trasmissioniSingole, trasmissioneSingola);

			return trasmissione;

		}

		private DocsPAWA.DocsPaWR.Corrispondente[] queryUtenti(string codiceRubrica) 
		{
			
			//costruzione oggetto queryCorrispondente
			DocsPaWR.AddressbookQueryCorrispondente qco = new DocsPAWA.DocsPaWR.AddressbookQueryCorrispondente();

			qco.codiceRubrica = codiceRubrica;
			qco.getChildren = true;
			qco.fineValidita = true;
			
			qco.idAmministrazione=UserManager.getInfoUtente(this).idAmministrazione;// ConfigurationManager.AppSettings["ID_AMMINISTRAZIONE"];
			
			//corrispondenti interni
			qco.tipoUtente=DocsPaWR.AddressbookTipoUtente.INTERNO;			
			
			DocsPaWR.Corrispondente[] l_corrispondenti=UserManager.getListaCorrispondenti(this.Page,qco);
			
			return pf_getCorrispondentiFiltrati(l_corrispondenti);
	
		}

		private DocsPAWA.DocsPaWR.Corrispondente[] pf_getCorrispondentiFiltrati(DocsPAWA.DocsPaWR.Corrispondente[] corrispondenti)
		{
			string l_oldSystemId="";								
			System.Object[] l_objects=new System.Object[0];
			System.Object[] l_objects_ruoli = new System.Object[0];
			DocsPaWR.Ruolo[] lruolo = new DocsPAWA.DocsPaWR.Ruolo[0];
			int i = 0;
			foreach(DocsPAWA.DocsPaWR.Corrispondente t_corrispondente in corrispondenti)
			{
				string t_systemId=t_corrispondente.systemId;						
				if (t_systemId!=l_oldSystemId)
				{
					l_objects=Utils.addToArray(l_objects,t_corrispondente); 	
					l_oldSystemId=t_systemId;
					i = i + 1 ;
					continue;
				}
				else
				{
					/* il corrispondente non viene aggiunto, in quanto sarebbe un duplicato 
					 * ma viene aggiunto solamente il ruolo */
					
					if(t_corrispondente.GetType().Equals(typeof(DocsPAWA.DocsPaWR.Utente)))
					{
						if ((l_objects[i - 1]).GetType().Equals(typeof(DocsPAWA.DocsPaWR.Utente)))
						{
							l_objects_ruoli = ((Utils.addToArray(((DocsPAWA.DocsPaWR.Utente)(l_objects[i -1])).ruoli, ((DocsPAWA.DocsPaWR.Utente)t_corrispondente).ruoli[0])));			
							DocsPaWR.Ruolo[] l_ruolo=new DocsPAWA.DocsPaWR.Ruolo[l_objects_ruoli.Length];
							((DocsPAWA.DocsPaWR.Utente)(l_objects[i -1])).ruoli = l_ruolo;
							l_objects_ruoli.CopyTo(((DocsPAWA.DocsPaWR.Utente)(l_objects[i -1])).ruoli, 0);
						}
						
					}
				}
				
			}
			
			DocsPaWR.Corrispondente[] l_corrSearch=new DocsPAWA.DocsPaWR.Corrispondente[l_objects.Length];	
			l_objects.CopyTo(l_corrSearch,0);

			return l_corrSearch;
		}

		#endregion

		private void setInfoFascicolo()
		{
			fascicolo = FascicoliManager.getFascicoloSelezionato(this);
			
			//visualizzo dati fascicolo
			caricaValoriFascicoloSelezionato(fascicolo);
			
			//get folder dal fascicolo Fasc:					
			DocsPaWR.Folder folder=FascicoliManager.getFolder(this,fascicolo);
			
			
			caricaFoldersFascicolo(folder);

            //if (Request.QueryString["newFasc"] != null && this.Request.QueryString["newFasc"].ToString().Equals("1"))
            //{
            //    //Session["newFasc"] = "1";
            //    settaCodiceFascicoloInDocClassifica(fascicolo);

            //}


		}
        /// <summary>
        /// Client script che setta il codice del fascicolo procedimentale creato in DocClassifica
        /// </summary>
        /// <param name="fascicolo"></param>
        private void settaCodiceFascicoloInDocClassifica(DocsPAWA.DocsPaWR.Fascicolo fascicolo)
        {

            string script = "<script>if(top.principale.iFrame_sx.IframeTabs.docClassifica!=null){top.principale.iFrame_sx.IframeTabs.docClassifica.submit();};</script>";
            Page.RegisterStartupScript("codFasc", script);
        }


		private void clientScriptAggiornaFascicolo(DocsPAWA.DocsPaWR.Fascicolo fascicolo)
		{
			
			string script="<script>if(top.principale.iFrame_sx.IframeTabs.docClassifica!=null){if(top.principale.iFrame_sx.IframeTabs.docClassifica.txt_codFasc!=null){top.principale.iFrame_sx.IframeTabs.docClassifica.txt_codFasc.value='"+fascicolo.codice.Replace(@"\", @"\\") +"'}};</script>";
			
			Page.RegisterStartupScript("codFasc",script);

			string script1="<script>if(top.principale.iFrame_sx.IframeTabs.docClassifica!=null){if(top.principale.iFrame_sx.IframeTabs.docClassifica.h_codFasc!=null){top.principale.iFrame_sx.IframeTabs.docClassifica.h_codFasc.value='"+fascicolo.codice.Replace(@"\", @"\\") +"'}};</script>";
			
			Page.RegisterStartupScript("h_codFasc",script1);

		}

		private void caricaValoriFascicoloSelezionato(DocsPAWA.DocsPaWR.Fascicolo fasc)
		{
			this.txt_fascApertura.Text=fasc.apertura;
			this.txt_FascChiusura.Text=fasc.chiusura;
			this.txt_descrizione.Text=fasc.descrizione;

            Note.INoteManager noteManager = Note.NoteManagerFactory.CreateInstance(DocsPAWA.DocsPaWR.OggettiAssociazioniNotaEnum.Fascicolo);
            this.txt_fascnote.Text = noteManager.GetUltimaNotaAsString();

			this.txt_fascStato.Text=FascicoliManager.decodeStatoFasc(this,fasc.stato);
			this.txt_Fasctipo.Text=FascicoliManager.decodeTipoFasc(this,fasc.tipo);
			this.txt_ClassFasc.Text=getCodiceGerarchia(fasc);
			this.txt_fascdesc.Text=fasc.codice;
            this.chkFascicoloCartaceo.Checked = fasc.cartaceo;
            if ((fasc.privato == null) || (fasc.privato == "0"))
                this.chkFascicoloPrivato.Checked = false;
            else
                this.chkFascicoloPrivato.Checked = true;


			//bool statoAperturaFascicolo = false;

			if(fasc.stato.Equals("C"))
			{
				ViewState["Chiuso"]=true;				
			}
			else
			{
				ViewState["Chiuso"]=false;
				//statoAperturaFascicolo=true;
			}
//			this.btn_insDoc.Enabled=statoAperturaFascicolo;
			//per la visualizzazione della collocazione fisica
            if (!string.IsNullOrEmpty(fasc.idUoLF))
            {
                DocsPaWR.Corrispondente corrRef = UserManager.getCorrispondenteBySystemID(this, fasc.idUoLF);

                if (corrRef != null)
                {
                    this.txt_cod_lf.Text = corrRef.codiceRubrica;
                    this.txt_desc_lf.Text = corrRef.descrizione;
                    this.txt_dta_lf.Text = fasc.dtaLF;
                }
            }
		}

		private string getCodiceGerarchia(DocsPAWA.DocsPaWR.Fascicolo fascicolo)
		{
			string retValue="";
			DocsPaWR.FascicolazioneClassifica[] classifica=FascicoliManager.getGerarchia(this,fascicolo.idClassificazione,UserManager.getUtente(this).idAmministrazione);
			if (classifica!=null)
			{
				//Elisa 11/08/2005 gestione nodo titolario ReadOnly
				//Session.Add("cha_ReadOnly",classifica[classifica.Length-1].cha_ReadOnly);
                Session.Add("classificaSelezionata", classifica[classifica.Length - 1]);
				//
				retValue=((DocsPAWA.DocsPaWR.FascicolazioneClassifica)classifica[classifica.Length-1]).codice;
			}

			return retValue;
		}

		private void caricaFoldersFascicolo(DocsPAWA.DocsPaWR.Folder folder)
		{
			FascicoliManager.removeHashFolder(this);
			Folders.Nodes.Clear();
			HashFolder.Clear();
			
			indexH=0;
			Microsoft.Web.UI.WebControls.TreeNode rootFolder=new Microsoft.Web.UI.WebControls.TreeNode();
			if(folder!=null)
			{
				//Creo la root folder dell'albero
				//Modifica per sostituire la dicitura "Root Folder" con il codice del fascicolo
				rootFolder.Text=fascicolo.codice;   // folder.descrizione;
				//rootFolder.NavigateUrl="fascDettagliFasc.aspx?idFolder="+indexH.ToString();
				//rootFolder.Target="iFrame_cn";
				rootFolder.ID=indexH.ToString();

				//aggiungo la root folder alla collezione dei nodi dell'albero
				Folders.Nodes.Add(rootFolder);
				
				//aggiungo la root folder alla tabella di hash associata
				HashFolder.Add(indexH,folder);
				
				indexH=indexH+1;
			}
					
			//Costruzione Albero Folder del fascicolo.	
			if (folder.childs.Length >0)
			{
				for(int k=0;k<folder.childs.Length;k++)
				{
					this.CreateTree(rootFolder,folder.childs[k]);
				}
			}
			
			FascicoliManager.setHashFolder(this,this.HashFolder);
			
		}

		private Microsoft.Web.UI.WebControls.TreeNode addFolderNode(Microsoft.Web.UI.WebControls.TreeNode parentNode,DocsPaWR.Folder folder)
		{
            Microsoft.Web.UI.WebControls.TreeNode node = new Microsoft.Web.UI.WebControls.TreeNode();

			node.Text=folder.descrizione;
			node.ID=indexH.ToString();
			//node.NavigateUrl="fascDettagliFasc.aspx?idFolder="+indexH.ToString();
			//node.Target="iFrame_cn";

			//aggiunge il nodo creato al nodo genitore
			parentNode.Nodes.Add(node);
			
			//aggiunge nella hashtable, la folder corrispondente al nodo creato 
			HashFolder.Add(indexH,folder);	
			
			indexH=indexH+1;
			
			return node;
		}


		public void CreateTree(Microsoft.Web.UI.WebControls.TreeNode parentNode,DocsPaWR.Folder obj)
		{
            Microsoft.Web.UI.WebControls.TreeNode newAddedNode = addFolderNode(parentNode, obj);
			newAddedNode.Expanded = true;

			int g=obj.childs.Length;
			for(int j=0;j<g;j++)
			{
				DocsPaWR.Folder newFolder=obj.childs[j];
				
				//richiama la funzione ricorsivamente
				CreateTree(newAddedNode,newFolder);
			}
		}

		private Microsoft.Web.UI.WebControls.TreeNode getSelectedNodeFolder()
		{
			Microsoft.Web.UI.WebControls.TreeNode nodeToSelect;
			if (this.Page.Session["fascDettagliFasc.nodoSelezionato"]!=null)
			{
                nodeToSelect = (Microsoft.Web.UI.WebControls.TreeNode)this.Page.Session["fascDettagliFasc.nodoSelezionato"];
			}
			else
			{
				if (Folders.Nodes.Count>0)
				{
					nodeToSelect=Folders.GetNodeFromIndex(Folders.SelectedNodeIndex);
				}
				else
				{
					nodeToSelect=null;
				}
				this.Page.Session["fascDettagliFasc.nodoSelezionato"]=nodeToSelect;
			}
			
			return nodeToSelect;
		}
		private void expandNode(Microsoft.Web.UI.WebControls.TreeNode nodeFolderToSelect)
		{
			Hashtable nodi=new Hashtable();
			Microsoft.Web.UI.WebControls.TreeNode selectedNode=nodeFolderToSelect;			
			int index=0;
			while (selectedNode!=null)
			{
				nodi.Add(index,selectedNode);
				if (selectedNode.Parent.GetType().ToString()!=Folders.GetType().ToString())
				{
                    selectedNode = (Microsoft.Web.UI.WebControls.TreeNode)selectedNode.Parent;
				}
				else
				{
					selectedNode=null;
				}

				index++;
			}
			for (int i=nodi.Count-1;i>=0;i--)
			{
                selectedNode = (Microsoft.Web.UI.WebControls.TreeNode)nodi[i];
				selectedNode.Expanded=true;
			}
			nodi.Clear();
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
			this.Folders.SelectedIndexChange += new Microsoft.Web.UI.WebControls.SelectEventHandler(this.Folders_SelectedIndexChange);
			this.btn_rimuovi.Click += new System.Web.UI.ImageClickEventHandler(this.btn_rimuovi_Click);
			this.img_dettagliProfilazioneFasc.Click += new ImageClickEventHandler(img_dettagliProfilazioneFasc_Click);
            this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new System.EventHandler(this.fascDettagliFasc_PreRender);

		}
		#endregion

        private void img_dettagliProfilazioneFasc_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (txt_tipoFasc.Text != "" && fascicolo.template != null)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ApriDettagliProfilazione", "ApriDettagliProfilazione();", true);
            }
        }

		private void btn_rimuovi_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			try
			{
				bool rootFolder = false;
				string nFasc = "";
				DocsPaWR.Folder selectedFolder=getSelectedFolder(out rootFolder);
				if (rootFolder)
				{
					if(fascicolo.tipo.Equals("P"))
					{
						Response.Write("<script>alert('Non è possibile rimuovere il fascicolo procedimentale: " + fascicolo.codice + "') ;</script>");
					}
					if(fascicolo.tipo.Equals("G"))
					{
						Response.Write("<script>alert('Non è possibile rimuovere il fascicolo generale: " + fascicolo.codice + "') ;</script>");
					}
					return;
				}
				if (selectedFolder!=null)
				{
					/* Se il folder selezionato ha figli (doc o sottocartelle) su cui HO visibilità 
					 * non deve essere rimosso. Dopo l'avviso all'utente, la procedura termina */
					if(selectedFolder.childs.Length > 0)
					{
						Response.Write("<script>alert('Non è possibile rimuovere il sottofascicolo selezionato:\\n\\ncontiene DOCUMENTI o SOTTOFASCICOLI');</script>");
					}
					else
					{
						/* Se il folder selezionato ha figli (doc o sottocartelle) su cui NON HO 
						 * la visibilità non deve essere rimosso */
						//CanRemoveFascicolo ritornerà un bool: true = posso rimuovere il folder, false altrimenti
						if (!FascicoliManager.CanRemoveFascicolo(this,selectedFolder.systemID, out nFasc))
						{
							if(nFasc.Equals("0") || nFasc.Equals(""))
							{
								Response.Write("<script>alert('Non è possibile rimuovere il sottofascicolo selezionato:\\n\\ncontiene DOCUMENTI');</script>");							
							}
							else
							{
								Response.Write("<script>alert('Non è possibile rimuovere il sottofascicolo selezionato:\\n\\ncontiene DOCUMENTI o SOTTOFASCICOLI');</script>");						
							}
						}
						else
						{
                            Microsoft.Web.UI.WebControls.TreeNode parentNode = (Microsoft.Web.UI.WebControls.TreeNode)getSelectedNodeFolder().Parent;
							FascicoliManager.delFolder(this,selectedFolder);
							DocsPaWR.Folder folder=FascicoliManager.getFolder(this,fascicolo);
							caricaFoldersFascicolo(folder);
							DocsPaWR.Folder folderToSelect=(DocsPAWA.DocsPaWR.Folder)HashFolder[parentNode.ID];
							selectNodeFolder(parentNode);
						}
					}
				}
			}
			catch(System.Web.Services.Protocols.SoapException es) 
			{
				ErrorManager.redirect(this, es);
			} 
		}

		private DocsPAWA.DocsPaWR.Folder getSelectedFolder(out bool rootFolder)
		{
			DocsPaWR.Folder folderSelected = null;
			rootFolder = false;
            Microsoft.Web.UI.WebControls.TreeNode node = this.Folders.GetNodeFromIndex(Folders.SelectedNodeIndex);
			int id=Int32.Parse(node.ID);
			//inserito questo controllo per verificare se la cartella
			//sia una root folder (a indice 0),pertanto ,non cancellabile.
			//suggerimento : individuare il metodo che dia l'indice del nodo 
			//selezionato in modo da disabilitare al click sul nodo ,il  bottone
			//rimuovi
			if(id >= 0)
			{
				folderSelected=(DocsPAWA.DocsPaWR.Folder)this.HashFolder[id];
				FascicoliManager.setFolderSelezionato(this,folderSelected);
				if (id == 0)
					rootFolder = true;
			}
			return folderSelected;
		}

		private void selezionaUltimoNodoSelezionato()
		{
			Microsoft.Web.UI.WebControls.TreeNode nodeToSelect=getSelectedNodeFolder();
			
			selectNodeFolder(nodeToSelect);
		}

      protected void img_cercaSottoFasc_Click(object sender, ImageClickEventArgs e)
      {
         string idFascicolo = fascicolo.systemID;
         RegisterStartupScript("openModale", "<script>ApriRicercaSottoFascicoli('" + idFascicolo + "','" + "" + "')</script>");
      }

      private void SelezionaSottofascicolo()
      {

         hd_returnValueModal.Value = "";
         if (Session["NodeIndexRicercaSottoFascicoli"] != null)
         {
            string indx = Session["NodeIndexRicercaSottoFascicoli"].ToString();
            Session.Remove("NodeIndexRicercaSottoFascicoli");
            if (!string.IsNullOrEmpty(indx) && indx.Contains("."))
            {
                string[] ar = indx.Split('.');
                string nodoDaEspandere = string.Empty;
                  for(int i = 0; i < ar.Length -1; i++)
                  {
                      nodoDaEspandere += ar[i];
                      Folders.GetNodeFromIndex(nodoDaEspandere).Expanded = true;
                      if (i < ar.Length - 2)
                          nodoDaEspandere += ".";
                  }
            }
           // Folders.GetNodeFromIndex("0").Expanded = true;
            Microsoft.Web.UI.WebControls.TreeNode trnParent;

            trnParent = (Microsoft.Web.UI.WebControls.TreeNode)Folders.GetNodeFromIndex(indx).Parent;

            trnParent.Expanded = true;
            Folders.SelectedNodeIndex = indx;


         }
      }

		private void selectNodeFolder(Microsoft.Web.UI.WebControls.TreeNode nodeFolderToSelect)
		{
			//TreeNode nodo=HashFolder[folderToSelect];
			//Folders.Nodes[folderToSelect]
			string idFolder="";
			if (nodeFolderToSelect!=null)
			{
				expandNode(nodeFolderToSelect);
				DocsPaWR.Folder folder=(DocsPAWA.DocsPaWR.Folder)HashFolder[Int32.Parse(nodeFolderToSelect.ID)];
				FascicoliManager.setFolderSelezionato(this,folder);
				if (folder!=null)
				{
					idFolder=nodeFolderToSelect.ID;
				}
				else
				{
					idFolder="";
				}
			}
			else
			{
				idFolder="";
			}

		}

	
        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
		private bool CreateNewFolder(out DocsPaWR.ResultCreazioneFolder result)
		{
            bool retValue = false;
            result = DocsPAWA.DocsPaWR.ResultCreazioneFolder.GENERIC_ERROR;

			try
			{
				DocsPaWR.Folder folderSelected=FascicoliManager.getFolderSelezionato(this);
				
				Microsoft.Web.UI.WebControls.TreeNode nodeSelected=getSelectedNodeFolder();

				if (folderSelected!=null)
				{
					DocsPaWR.Folder newFolder=new DocsPAWA.DocsPaWR.Folder();

                    newFolder.idFascicolo = fascicolo.systemID;
                    newFolder.idParent = folderSelected.systemID;

                    newFolder.descrizione = Session["descNewFolder"].ToString();

                    if (FascicoliManager.newFolder(this, ref newFolder, infoUt, userRuolo, out result))
                    {
                        DocsPaWR.Folder folder = FascicoliManager.getFolder(this, fascicolo);
                        caricaFoldersFascicolo(folder);

                        selectNodeFolder(nodeSelected);
                    }

                    retValue = (result == DocsPAWA.DocsPaWR.ResultCreazioneFolder.OK);
				}
			}
			catch(System.Web.Services.Protocols.SoapException es) 
			{
				ErrorManager.redirect(this, es);
			}

            return retValue;
		}

		private void btn_insDoc_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			bool rootFolder;
			bool outValue = false;
            DocsPaWR.Folder selectedFolder=getSelectedFolder(out rootFolder);
			if (selectedFolder !=null)
			{
				DocsPaWR.SchedaDocumento schedaDoc = DocumentManager.getDocumentoSelezionato(this);
                String message = String.Empty;
                DocumentManager.addDocumentoInFolder(this, schedaDoc.systemId, selectedFolder.systemID, false, out outValue, out message);
				Response.Write("<script>var k=window.open('../documento/docClassifica.aspx','IframeTabs');</script>");
			
			}
		}

		private void fascDettagliFasc_PreRender(object sender, System.EventArgs e)
		{
			this.btn_aggiungi.Attributes.Add("onclick","ApriFinestraNewFolder('dettagliFasc');");
			//abilitazione delle funzioni in base al ruolo
			UserManager.disabilitaFunzNonAutorizzate(this);

			//Elisa 11/08/2005 gestione nodo titolario ReadOnly
			//if(Session["cha_ReadOnly"]!=null)
            if (Session["classificaSelezionata"] != null)
			{
                DocsPaWR.FascicolazioneClassifica classifica = (DocsPaWR.FascicolazioneClassifica)Session["classificaSelezionata"];

				//if(!UserManager.ruoloIsAutorized(this,this.btn_aggiungi.Tipologia.ToString()) || (bool) Session["cha_ReadOnly"]==true)
                if (!UserManager.ruoloIsAutorized(this, this.btn_aggiungi.Tipologia.ToString()) || classifica.cha_ReadOnly)
				{
					this.btn_aggiungi.Enabled=false;
				}
				else
				{
					this.btn_aggiungi.Enabled=true;
				}
			}

			verificaHMdiritti();
			verificaChiusuraFascicolo();

            //Controllo se il ruolo utente è autorizzato a creare documenti privati
            if (!UserManager.ruoloIsAutorized(this, "DO_FASC_PRIVATO"))
            {
                this.lblFascicoloPrivato.Visible = false;
                this.chkFascicoloPrivato.Visible = false;
            }
            else
            {
                this.lblFascicoloPrivato.Visible = true;
                this.chkFascicoloPrivato.Visible = true;
            }
            //se valorizzato lo vedo comunque, altrimenti perdo l'informazione.
            if ((this.fascicolo.privato == null) || (this.fascicolo.privato == "0"))
                this.chkFascicoloPrivato.Visible = false;
            else
                this.chkFascicoloPrivato.Visible = true;


		}
		
		private void verificaChiusuraFascicolo()
		{
			if(fascicolo!=null && fascicolo.stato=="C")
			{
				this.btn_aggiungi.Enabled = false;
//				this.btn_insDoc.Enabled = false;
				this.btn_rimuovi.Enabled = false;
			}
		
		}
		private void verificaHMdiritti()
		{
			//disabilitazione dei bottoni in base all'autorizzazione di HM 
			//sul documento
			
			if (fascicolo !=null && ( (fascicolo.accessRights!=null && fascicolo.accessRights!="") || fascicolo.inArchivio=="1") )  
			{
				if(UserManager.disabilitaButtHMDiritti(fascicolo.accessRights))
				{
					//bottoni che devono essere disabilitati in caso
					//di diritti di sola lettura
					this.btn_aggiungi.Enabled = false;
//					this.btn_insDoc.Enabled = false;
					this.btn_rimuovi.Enabled = false;
				}
			}
		}

		private void Folders_SelectedIndexChange(object sender, Microsoft.Web.UI.WebControls.TreeViewSelectEventArgs e)
		{
			
			bool rootFolder;
			DocsPaWR.Folder selectedFolder=getSelectedFolder(out rootFolder);
		}

	}
}
