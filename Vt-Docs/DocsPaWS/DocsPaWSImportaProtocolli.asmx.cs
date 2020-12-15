using System;
using System.Data;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Configuration;

namespace DocsPaWS
{	
	/// <summary>
	/// maggio 2007 - gadamo
	/// 
	/// Web Service per la gestione dell'import in DocsPA di 
	/// protocolli inseriti su un foglio Excel pre-formattato
	/// 
	/// Il WS viene invocato da una Windows Application: ImportaProtocolli.exe
	/// 
	/// Il WS è stato testato solo per la versione di DocsPA con documentale ETDOC.
	/// 
	/// </summary>
	[WebService(Namespace = "http://www.etnoteam.it/DocsPaWSImportaProtocolli")]	
	public class DocsPaWSImportaProtocolli : System.Web.Services.WebService
	{		
		#region variabili
		private DocsPaVO.utente.Utente _utente;
		private DocsPaVO.utente.Ruolo _ruolo;
		private string _idAddress;
		private DocsPaWS.DocsPaWebService _ws;
		private string _sessionIdWS = "IPE";		
				
		#endregion
		
		#region TEST
		[WebMethod]
		public string TestMe()
		{
			return "LSDJDOI878SDFUYFDSFY";
		}
		#endregion
		
		#region PROTOCOLLA
		[WebMethod]		
		public string Protocolla(ArrayList listaCampi,
								 ArrayList listaDati,
								 out int numProto, 
								 out string segnatura)
		{
			string err = string.Empty;
			numProto = 0;
			segnatura = string.Empty;

			// CAMPI
			string userID = listaCampi[0].ToString();
			string pwd = listaCampi[1].ToString();
			string idCorrRuolo = listaCampi[2].ToString();
			string idGruppo = listaCampi[3].ToString();
			string livelloRuolo = listaCampi[4].ToString();
			string idUO = listaCampi[5].ToString();
			string codUO = listaCampi[6].ToString();
			string descUO = listaCampi[7].ToString();
			string idAmm = listaCampi[8].ToString();
			string idRegistro = listaCampi[9].ToString();
			string nomeUtenteProt = listaCampi[10].ToString();
			string cognomeUtenteProt = listaCampi[11].ToString();
                    
            // DATI
			string dataProtoEme = listaDati[0].ToString();
			string numProtoEme = listaDati[1].ToString();
			string protoEme = listaDati[2].ToString();
			string tipoProtocollo = listaDati[3].ToString();
			string oggetto = listaDati[4].ToString();
			string corrispondenti = listaDati[5].ToString();
            string corrispondentiCC = listaDati[6].ToString();
			string dataProtoMitt = listaDati[7].ToString();
			string numProtoMitt = listaDati[8].ToString();
			string dataArrivo = listaDati[9].ToString();
			string codClassifica = listaDati[10].ToString();						
			string note = listaDati[11].ToString();
            string systemIdRF = listaDati[12].ToString();					
			
			try
			{								
				this.canaleWSAperto();
				
				// login
				err = Login(userID,pwd,idAmm);
				if(err!=null && err!=string.Empty)
					return "ERRORE: " + err;
	
				// ruolo
				this._ruolo = new DocsPaVO.utente.Ruolo();
				this._ruolo.systemId = idCorrRuolo;
				this._ruolo.idGruppo=idGruppo;
					DocsPaVO.utente.UnitaOrganizzativa uo = new DocsPaVO.utente.UnitaOrganizzativa();
					uo.codice = codUO;
					uo.descrizione = descUO;
					uo.idAmministrazione = idAmm;
					uo.systemId = idUO;
				this._ruolo.uo = uo;
				this._ruolo.livello = livelloRuolo;
                this._ruolo.registri = new ArrayList();
                this._ruolo.registri.Add(this._ws.GetRegistroBySistemId(idRegistro));					
				
				// utente
				DocsPaVO.utente.InfoUtente infoUtente = this.getInfoUtente();

				DocsPaVO.utente.Corrispondente corr = null;								
				DocsPaVO.documento.SchedaDocumento schedaDoc = new DocsPaVO.documento.SchedaDocumento(); ;

				// Tipologia Protocollo				 
				if (tipoProtocollo == "P")
				{
					DocsPaVO.documento.ProtocolloUscita protoOUT = new DocsPaVO.documento.ProtocolloUscita();					

				    // un solo mittente = struttura del protocollista				   
			        corr = new DocsPaVO.utente.Corrispondente();
			        corr.systemId = idUO;
			        corr.codiceCorrispondente = codUO;
			        corr.descrizione = descUO;							
			        corr.tipoIE = "I"; 
			        corr.idAmministrazione = idAmm;
			        corr.dettagli = false;					
			        protoOUT.mittente = corr;

					// uno o più destinatari
					if (protoOUT.destinatari == null)
						protoOUT.destinatari = new ArrayList();

                    string[] lstCorrEst = corrispondenti.Split(';');
                    foreach (string corrEst in lstCorrEst)
                    {
                        protoOUT.destinatari.Add(corrDestOcc(corrEst.Trim(), idAmm));
                    }

                    if (!string.IsNullOrEmpty(corrispondentiCC.Trim()))
                    {
                        // uno o più destinatari in conoscenza
                        if (protoOUT.destinatariConoscenza == null)
                            protoOUT.destinatariConoscenza = new ArrayList();

                        string[] lstCorrEstCC = corrispondentiCC.Split(';');
                        foreach (string corrEstCC in lstCorrEstCC)
                        {
                            protoOUT.destinatariConoscenza.Add(corrDestOcc(corrEstCC.Trim(), idAmm));
                        }
                    }
                    
                    schedaDoc.protocollo = protoOUT;
					schedaDoc.tipoProto = "P";
				}

				// protocollo in arrivo
				if (tipoProtocollo == "A")
				{
					DocsPaVO.documento.ProtocolloEntrata protoIN = new DocsPaVO.documento.ProtocolloEntrata();
				
					// un solo mittente = struttura del protocollista				   
                    //corr = new DocsPaVO.utente.Corrispondente();
                    //corr.systemId = idUO;
                    //corr.codiceCorrispondente = codUO;
                    //corr.descrizione = descUO;
                    //corr.tipoIE = "I";
                    //corr.idAmministrazione = idAmm;
                    //corr.dettagli = false;
                    //protoIN.mittente = corr;
                    string corrispondente = string.Empty;
                    if (!string.IsNullOrEmpty(corrispondenti.Trim()))
                    {
                        if (corrispondenti.Contains(";"))
                            corrispondente = corrispondenti.Substring(0, corrispondenti.IndexOf(";") + 1);
                        else
                            corrispondente = corrispondenti;
                    }

                    DocsPaVO.utente.Corrispondente corr2 = new DocsPaVO.utente.Corrispondente();
                    corr2.descrizione = corrispondente;
                    corr2.tipoCorrispondente = "O";
                    corr2.tipoIE = "E";
                    corr2.idAmministrazione = idAmm;
                    corr2.dettagli = false;

                    protoIN.mittente = corr2;
										
					// protocollo mittente				
					if(dataProtoMitt!=null && dataProtoMitt!=string.Empty) 
						protoIN.dataProtocolloMittente = dataProtoMitt;
					if (numProtoMitt != null && numProtoMitt != string.Empty) 
						protoIN.descrizioneProtocolloMittente = numProtoMitt;
					
					// data arrivo
					if (dataArrivo != null && dataArrivo != string.Empty) 
					{						
						DocsPaVO.documento.Documento doc = new DocsPaVO.documento.Documento();
						doc.dataArrivo = dataArrivo;						
						
						schedaDoc.documenti = new ArrayList();
						schedaDoc.documenti.Add(doc);						
					}
						
					schedaDoc.protocollo = protoIN;
					schedaDoc.tipoProto = "A";
				}

				schedaDoc = this.creaSchedaDocumento(idRegistro,oggetto, note, schedaDoc);
				
				// esegue il tutto solo se il registro non è chiuso
				if(!schedaDoc.registro.stato.Equals("C"))
				{														
					// dati di emergenza
					schedaDoc.datiEmergenza = new DocsPaVO.documento.DatiEmergenza();
					schedaDoc.datiEmergenza.dataProtocollazioneEmergenza = dataProtoEme;
					schedaDoc.datiEmergenza.protocolloEmergenza = protoEme;											
					schedaDoc.datiEmergenza.cognomeProtocollatoreEmergenza = cognomeUtenteProt;
					schedaDoc.datiEmergenza.nomeProtocollatoreEmergenza = nomeUtenteProt;
									
					// protocollatore
					schedaDoc.protocollatore = new DocsPaVO.documento.Protocollatore(infoUtente.idPeople,idCorrRuolo,idUO,codUO);
							
					// creatore del documento
					schedaDoc.creatoreDocumento = new DocsPaVO.documento.CreatoreDocumento(infoUtente.idPeople,idCorrRuolo,idUO);

                    // controllo su univocità RF nel caso di RF presente nella segnatura
                    // questo codice prevede la presenza della chiave del web config nel WS
                    //if (ConfigurationManager.AppSettings["ENABLE_CODBIS_SEGNATURA"] != null && ConfigurationManager.AppSettings["ENABLE_CODBIS_SEGNATURA"] == "1")
                    //{
                        //DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                        //DocsPaVO.amministrazione.InfoAmministrazione infoAmm = amm.AmmGetInfoAmmCorrente(infoUtente.idAmministrazione);
                        //if (infoAmm.Segnatura.Contains("COD_RF_PROT"))
                        //{
                        //    // se ci sono uno o più RF associati al registro
                        //    DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
                        //    ArrayList listaRF = new ArrayList();
                        //    listaRF = utenti.GetListaRegistriRfRuolo(idCorrRuolo, "1", idRegistro); 
                        //    if (listaRF != null && listaRF.Count == 1) //se un solo RF non apro popup, ma selec direttamente.
                        //    {
                        //        DocsPaVO.utente.Registro reg = (DocsPaVO.utente.Registro)listaRF[0];
                        //        schedaDoc.id_rf_prot = reg.systemId;
                        //        schedaDoc.cod_rf_prot = reg.codRegistro;
                        //    }
                        //}
                    //}

                    //verifico se è stato inserito il codice RF da foglio excel
                    if (!string.IsNullOrEmpty(systemIdRF))
                    {
                        DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
                        DocsPaVO.utente.Registro reg = new DocsPaVO.utente.Registro();
                        utenti.GetRegistro(systemIdRF, ref reg);
                        if (reg != null && !string.IsNullOrEmpty(reg.systemId) && !string.IsNullOrEmpty(reg.codRegistro))
                        {
                            schedaDoc.id_rf_prot = reg.systemId;
                            schedaDoc.cod_rf_prot = reg.codRegistro;
                        }
                    }

					DocsPaVO.documento.ResultProtocollazione risultatoProtocollazione;
					DocsPaVO.documento.SchedaDocumento schedaResult = this._ws.DocumentoProtocolla(schedaDoc, infoUtente, this._ruolo, out risultatoProtocollazione);

					if (schedaResult != null)
					{
						this.putinfoCorr(schedaResult);
						numProto = Int32.Parse(schedaResult.protocollo.numero);					
						segnatura = schedaResult.protocollo.segnatura;		
						string idProfile = schedaResult.docNumber;
						err = "Ok - ";	
						
						// classificazione
						if(codClassifica!=null && codClassifica!=string.Empty)
						{
							DocsPaVO.fascicolazione.Fascicolo fascicolo = this._ws.FascicolazioneGetFascicoloDaCodice(infoUtente,codClassifica,schedaDoc.registro,false, false);
							if(fascicolo!=null)
							{
								// recupera il folder
								DocsPaVO.fascicolazione.Folder folder = this._ws.FascicolazioneGetFolder(infoUtente.idPeople,idGruppo,fascicolo);
								
								if(folder!=null)
								{
                                    bool outValue = false;
                                    string msg = string.Empty;
                                    // classifica
                                    outValue = this._ws.FascicolazioneAddDocFolder(infoUtente, idProfile, folder, fascicolo.descrizione, out msg);
                                    if (outValue)
										err += " - Classificato su: " + codClassifica;
									else
										err += " - Classificazione fallita!";
								}							
								else
								{
									err += " - Classificazione fallita!";
								}
							}
							else
							{
								err += " - Classificazione fallita!";
							}
						}
                        err += " Nuova segnatura: " + segnatura;
					}	
					else
					{
						err = "Errore: protocollazione fallita!"; 
					}
				}
				else
				{
					err = "registro chiuso";
				}							
			}
			catch(Exception ex)
			{
				err = "Errore: protocollazione fallita! - " + ex.ToString();				
			}
			finally
			{
				this.Logoff();
				this.chiudiCanaleWS();
			}
			
			return err;
		}			

		private DocsPaVO.utente.Corrispondente corrDestOcc(string corrispondente, string idAmm)
		{
			DocsPaVO.utente.Corrispondente corr = new DocsPaVO.utente.Corrispondente();
			corr.descrizione = corrispondente;
			corr.tipoCorrispondente = "O";
			corr.tipoIE = "E";
			corr.idAmministrazione = idAmm;
			corr.dettagli = false;

			return corr;
		}

		private DocsPaVO.documento.SchedaDocumento creaSchedaDocumento(string idRegistro, string Oggetto, string note, DocsPaVO.documento.SchedaDocumento schedaDoc)
		{
			#region campi scheda costanti
			schedaDoc.systemId = null;
			schedaDoc.privato = "0";  //doc non privato
			schedaDoc.idPeople = this._utente.idPeople;
			schedaDoc.userId = this._utente.userId;		
			schedaDoc.typeId = "LETTERA";
			schedaDoc.appId = "ACROBAT";
										
			#endregion campi scheda costanti

			#region carica registro
			DocsPaVO.utente.Registro registro = new DocsPaVO.utente.Registro();			
			registro = this._ws.GetRegistroBySistemId(idRegistro);			
			schedaDoc.registro = registro;			
			#endregion carica registro

			#region crea oggetto scheda
			schedaDoc.oggetto = new DocsPaVO.documento.Oggetto();
			schedaDoc.oggetto.descrizione = Oggetto;
			#endregion crea oggetto scheda

			#region crea note scheda

            if (!string.IsNullOrEmpty(note))
            {
                DocsPaVO.utente.InfoUtente infoUtente = getInfoUtente();
                string idPeopleDelegato = "";
                if (infoUtente.delegato != null)
                    idPeopleDelegato = infoUtente.delegato.idPeople;
                DocsPaVO.Note.InfoNota nota = new DocsPaVO.Note.InfoNota(note.Replace("'", "''"), this._utente.systemId, this._ruolo.systemId, idPeopleDelegato);
                schedaDoc.noteDocumento = new System.Collections.Generic.List<DocsPaVO.Note.InfoNota>() { nota };
            }
            
			#endregion crea note scheda		

			return schedaDoc;

		}

		private void putinfoCorr(DocsPaVO.documento.SchedaDocumento sch)
		{
			if (sch != null && sch.protocollo != null)
			{
				if (sch.tipoProto.Equals("A"))
				{
					DocsPaVO.documento.ProtocolloEntrata pe = (DocsPaVO.documento.ProtocolloEntrata)sch.protocollo;
					if (pe.mittente != null	&& pe.mittente.info != null)
						BusinessLogic.Utenti.UserManager.addDettagliCorrOcc(pe.mittente);					
				}
				
				if (sch.tipoProto.Equals("P"))
				{
					DocsPaVO.documento.ProtocolloUscita pu = (DocsPaVO.documento.ProtocolloUscita)sch.protocollo;
					if (pu.mittente != null	&& pu.mittente.info != null)
						BusinessLogic.Utenti.UserManager.addDettagliCorrOcc(pu.mittente);
						
					if (pu.destinatari != null && pu.destinatari.Count > 0)
					{
						for (int i = 0; i < pu.destinatari.Count; i++)
						{
							DocsPaVO.utente.Corrispondente corr = (DocsPaVO.utente.Corrispondente)pu.destinatari[i];
							if (corr != null && corr.info != null)
								BusinessLogic.Utenti.UserManager.addDettagliCorrOcc(corr);
						}
					}
				}				
			}
		}
		#endregion
		
		#region LOGIN
		[WebMethod]
		public string Login(string userid, string pwd, string idAmm)
		{
			string esito = string.Empty;		
										
			try
			{
				this.canaleWSAperto();
				
				DocsPaVO.utente.UserLogin objLogin = new DocsPaVO.utente.UserLogin();
				objLogin.UserName = userid;
				objLogin.Password = pwd;
				objLogin.IdAmministrazione = idAmm;
				objLogin.Update = false;

				//autenticazione
				DocsPaVO.utente.UserLogin.LoginResult Lr = this.ExecuteLogin(objLogin, false);				

				if (Lr.Equals(DocsPaVO.utente.UserLogin.LoginResult.USER_ALREADY_LOGGED_IN))
					Lr = this.ExecuteLogin(objLogin, true);			
				
				switch (Lr)
				{
					case DocsPaVO.utente.UserLogin.LoginResult.OK:
					{							
						break;
					}
					case DocsPaVO.utente.UserLogin.LoginResult.APPLICATION_ERROR:
					{
						esito = "Login : errore generico";							
						break;
					}
					case DocsPaVO.utente.UserLogin.LoginResult.DISABLED_USER:
					{
						esito = "Login: utente non abilitato";							
						break;
					}
					case DocsPaVO.utente.UserLogin.LoginResult.UNKNOWN_USER:
					{
						esito = "Login: utente non riconosciuto";							
						break;
					}									
				}																											
			}
			catch (Exception ex)
			{
				esito = ex.ToString();	
			}			

			return esito;						
		}
		
		/// <summary>
		/// esecuzione della login a docspa
		/// </summary>
		/// <param name="objLogin">oggetto DocsPaVO.utente.UserLogin</param>
		/// <param name="forzaLogin">true o false</param>
		/// <returns>DocsPaVO.utente.UserLogin.LoginResult</returns>
		private DocsPaVO.utente.UserLogin.LoginResult ExecuteLogin(DocsPaVO.utente.UserLogin objLogin, bool forzaLogin)
		{ 
			this._utente = null;
			this._idAddress = null;
			return this._ws.Login(objLogin, out this._utente, forzaLogin, this._sessionIdWS, out this._idAddress);
		}

		private void Logoff()
		{					
			try
			{
				this._ws.Logoff(this._utente.userId, this._utente.idAmministrazione, this._sessionIdWS, this._utente.dst);				
			}
			catch
			{	
						
			}
		}
		#endregion
		
		#region gestione utente		

		/// <summary>
		/// Imposta l'oggetto utente
		/// </summary>
		/// <returns></returns>
		private DocsPaVO.utente.InfoUtente getInfoUtente()
		{
			DocsPaVO.utente.InfoUtente infoUtente = new DocsPaVO.utente.InfoUtente();

			infoUtente.idCorrGlobali = this._ruolo.systemId;
			infoUtente.idPeople = this._utente.idPeople;
			infoUtente.idGruppo = this._ruolo.idGruppo;
			infoUtente.dst = this._utente.dst;
			infoUtente.idAmministrazione = this._utente.idAmministrazione;
			infoUtente.userId = this._utente.userId;

			return infoUtente;
		}
		#endregion

		#region opzioni WS
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private bool canaleWSAperto()
		{
			if (this._ws == null)
				apriCanaleWS();

			return (this._ws != null);
		}
		
		/// <summary>
		/// 
		/// </summary>
		private void apriCanaleWS()
		{
			this._ws = new DocsPaWS.DocsPaWebService();			
		}

		/// <summary>
		/// 
		/// </summary>
		private void chiudiCanaleWS()
		{
			this._ws.Dispose();
		}
		#endregion
	}
}
