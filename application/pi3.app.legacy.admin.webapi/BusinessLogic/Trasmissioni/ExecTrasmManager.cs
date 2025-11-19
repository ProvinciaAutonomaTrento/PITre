// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using BusinessLogic.Interoperabilita;
using BusinessLogic.RubricaComune;
using DocsPaVO.documento;
using DocsPaVO.ricerche;
using Serilog;
using System.Collections;
using System.Data;

namespace BusinessLogic.Trasmissioni;
public class ExecTrasmManager
{
    private static ILogger logger = Log.ForContext(typeof(ExecTrasmManager));

    public delegate void InviaEmailUtentiTrasmDelegate(string path, DocsPaVO.trasmissione.Trasmissione objTrasm);

	private static void CallBack(IAsyncResult result)
	{

		var del = result.AsyncState as InviaEmailUtentiTrasmDelegate;

		if (del != null)
			del.EndInvoke(result);
	}


	public static DocsPaVO.trasmissione.Trasmissione saveExecuteTrasmMethod(string path, DocsPaVO.trasmissione.Trasmissione objTrasm)
	{
		logger.Information("BEGIN");
		logger.Debug("INIZIO : saveExecuteTrasmMethod");

		ArrayList executeListaQueries = new ArrayList();
		try
		{
			using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
			{
				//Salvo la trasmissione
				logger.Debug("saveTrasmissione");
				objTrasm = Trasmissioni.TrasmManager.saveTrasmMethod(objTrasm);
				//Eseguo la trasmissione
				logger.Debug("executeTrasmissione");
				objTrasm = Trasmissioni.ExecTrasmManager.executeTrasmMethod(path, objTrasm);
				transactionContext.Complete();
			}
		}
		catch (Exception e)
		{
			logger.Debug("Errore nella gestione delle trasmissioni (saveExecuteTrasmMethod)", e);
			throw new Exception(e.Message);
		}

		// invio della email agli utenti
		//objTrasm = InviaEmailUtentiTrasm(path, objTrasm);

		logger.Debug("FINE : executeTrasmMethod");
		logger.Information("END");
		return objTrasm;
	}

	/// <summary>
	/// Esegue la trasmissione
	/// </summary>
	/// <param name="path">Path per il link al documeto da inserire nel corpo delle email</param>
	/// <param name="objTrasm">oggetto trasmissione</param>
	/// <returns>l'oggetto trasmissione stesso</returns>
	public static DocsPaVO.trasmissione.Trasmissione executeTrasmMethod(string path, DocsPaVO.trasmissione.Trasmissione objTrasm)
	{
		logger.Information("BEGIN");
		logger.Debug("INIZIO : executeTrasmMethod");

		ArrayList executeListaQueries = new ArrayList();

		try
		{
			using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
			{
				// imposta la data di invio alla trasmissione
				objTrasm.dataInvio = DocsPaUtils.Functions.Functions.GetDate(false);
				if (string.IsNullOrEmpty(objTrasm.NO_NOTIFY) || objTrasm.NO_NOTIFY.Equals("0"))
					if (objTrasm.systemId != null)
					{
						DocsPaDB.Query_DocsPAWS.Trasmissione objQueryTrasm = new DocsPaDB.Query_DocsPAWS.Trasmissione();
						string idDelegato = (!string.IsNullOrEmpty(objTrasm.delegato)) ? objTrasm.delegato : string.Empty;
						objQueryTrasm.UpdateDataInvioTrasmissione(objTrasm.systemId, idDelegato);
						objQueryTrasm.Dispose();
					}

				// gestione cessione diritti
				if (verificaCessione(objTrasm))
					gestioneCessioneDiritti(objTrasm);

				// imposta la visibilità dell'oggetto trasmesso
				ImpostaVisibilitaInSecurity(objTrasm, false, executeListaQueries);

				// notifica dell'avvenuta trasmissione al documentale
				NotifyTrasmissioneDocumentale(objTrasm, (DocsPaVO.trasmissione.infoSecurity[])executeListaQueries.ToArray(typeof(DocsPaVO.trasmissione.infoSecurity)));

				//Richiamo il metodo per il calcolo della atipicità del documento / fascicolo
				DocsPaDB.Query_DocsPAWS.Documentale documentale = new DocsPaDB.Query_DocsPAWS.Documentale();
				DocsPaVO.utente.InfoUtente infoUtente = new DocsPaVO.utente.InfoUtente();
				infoUtente.idAmministrazione = objTrasm.utente.idAmministrazione;
				if (objTrasm.tipoOggetto == DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO)
					documentale.CalcolaAtipicita(infoUtente, objTrasm.infoDocumento.docNumber, DocsPaVO.Security.InfoAtipicita.TipoOggettoAtipico.DOCUMENTO);
				//if (objTrasm.tipoOggetto == DocsPaVO.trasmissione.TipoOggetto.FASCICOLO)
				//    documentale.CalcolaAtipicita(infoUtente, objTrasm.infoFascicolo.idFascicolo, DocsPaVO.Security.InfoAtipicita.TipoOggettoAtipico.FASCICOLO);
				if (objTrasm.tipoOggetto == DocsPaVO.trasmissione.TipoOggetto.FASCICOLO)
				{
					documentale.CalcolaAtipicita(infoUtente, objTrasm.infoFascicolo.idFascicolo, DocsPaVO.Security.InfoAtipicita.TipoOggettoAtipico.FASCICOLO);
					List<SearchResultInfo> docs = BusinessLogic.Documenti.areaConservazioneManager.getListaDocumentiByIdProject(objTrasm.infoFascicolo.idFascicolo);
					if (docs != null)
					{
						docs.ForEach(
								f => documentale.CalcolaAtipicita(
									infoUtente,
									f.Id,
									DocsPaVO.Security.InfoAtipicita.TipoOggettoAtipico.DOCUMENTO));

					}

				}

				transactionContext.Complete();
			}
		}
		catch (Exception e)
		{
			logger.Debug("Errore nella gestione delle trasmissioni (executeTrasmMethod)", e);
			throw new Exception(e.Message);
		}

		// invio della email agli utenti
		logger.Debug("Invio della mail agli utenti");
		logger.Debug("objTrasm.NO_NOTIFY = " + objTrasm.NO_NOTIFY);
		if (string.IsNullOrEmpty(objTrasm.NO_NOTIFY) || objTrasm.NO_NOTIFY == "0")
		{
			// objTrasm = InviaEmailUtentiTrasm(path, objTrasm);
			//15/12/2015 : Faccio l'invio email in maniera asincrona
			logger.Information("Chiamata asincrona al metodo InviaEmailUtentiTrasm");
			AsyncCallback callback = new AsyncCallback(CallBack);
			InviaEmailUtentiTrasmDelegate inviaEmailUtentiTrasm = new InviaEmailUtentiTrasmDelegate(InviaEmailUtentiTrasm);
			inviaEmailUtentiTrasm.BeginInvoke(path, objTrasm, callback, inviaEmailUtentiTrasm);
		}

		logger.Debug("FINE : executeTrasmMethod");
		logger.Information("END");
		return objTrasm;
	}

	private static bool verificaCessione(DocsPaVO.trasmissione.Trasmissione objTrasm)
	{
		return (objTrasm.cessione != null && objTrasm.cessione.docCeduto);
	}

	private static void gestioneCessioneDiritti(DocsPaVO.trasmissione.Trasmissione objTrasm)
	{
		DocsPaDB.Query_DocsPAWS.Trasmissione trasmissione = new DocsPaDB.Query_DocsPAWS.Trasmissione();
		trasmissione.execSenderRigths(objTrasm);

	}


	/// <summary>
	/// Imposta la visibilità dell'oggetto trasmesso
	/// </summary>
	/// <param name="objTrasm">oggetto trasmissione</param>
	/// <param name="estendeVisibilitaGerarchica">True = estende anche ai superiori gerarchici; False = solo al destinatario</param>
	private static void ImpostaVisibilitaInSecurity(DocsPaVO.trasmissione.Trasmissione objTrasm, bool estendeVisibilitaGerarchica, ArrayList executeListaQueries)
	{
		logger.Information("BEGIN");
		ArrayList executeQueries = new ArrayList();


		DocsPaDB.Query_DocsPAWS.DiagrammiStato diagr = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
		DocsPaVO.DiagrammaStato.Stato stat = new DocsPaVO.DiagrammaStato.Stato();
		stat.STATO_FINALE = false;
		InfoDocumento infoD = objTrasm.infoDocumento;
		//DocsPaVO.fascicolazione.InfoFascicolo infoF = new DocsPaVO.fascicolazione.InfoFascicolo();
		if (infoD != null && !string.IsNullOrEmpty(infoD.docNumber))
		{
			stat = diagr.getStatoDoc(infoD.docNumber);
		}
		else
		{
			DocsPaVO.fascicolazione.InfoFascicolo infoF = objTrasm.infoFascicolo;
			if (infoF != null && !string.IsNullOrEmpty(infoF.idFascicolo))
			{
				stat = diagr.getStatoFasc(infoF.idFascicolo);
			}
		}
		if (stat != null && stat.STATO_FINALE)
		{
			GeneraQueryVisibilita(objTrasm, estendeVisibilitaGerarchica, executeQueries, executeListaQueries, 1);
		}
		else
		{
			GeneraQueryVisibilita(objTrasm, estendeVisibilitaGerarchica, executeQueries, executeListaQueries);
		}
		DocsPaDB.Query_DocsPAWS.Trasmissione obj = new DocsPaDB.Query_DocsPAWS.Trasmissione();
		obj.execSenderRigths(executeQueries, objTrasm);

		//Solo se abilitata la funzione di atipicità per i destinatari della trasmissione che già hanno un tipo diritto "A" viene fatto un update a "T"
		foreach (DocsPaVO.trasmissione.TrasmissioneSingola ts in objTrasm.trasmissioniSingole)
		{
			if (ts.corrispondenteInterno is DocsPaVO.utente.Ruolo)
			{
				DocsPaVO.utente.Ruolo ruolo = ((DocsPaVO.utente.Ruolo)ts.corrispondenteInterno);
				string idGruppo = ruolo.idGruppo;
				string valoreChiaveAtipicita = "0";
				if (!String.IsNullOrEmpty(objTrasm.utente.idAmministrazione))
					valoreChiaveAtipicita = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(objTrasm.utente.idAmministrazione, "ATIPICITA_DOC_FASC");
				if (string.IsNullOrEmpty(valoreChiaveAtipicita))
				{
					valoreChiaveAtipicita = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "ATIPICITA_DOC_FASC");
				}
				if (!string.IsNullOrEmpty(valoreChiaveAtipicita) && valoreChiaveAtipicita.Equals("1"))
				{
					if (objTrasm.infoDocumento != null && !String.IsNullOrEmpty(objTrasm.infoDocumento.docNumber))
					{
						obj.UpdateTipoDirittoTrasm(idGruppo, objTrasm.infoDocumento.docNumber);
					}
					if (objTrasm.infoFascicolo != null && !String.IsNullOrEmpty(objTrasm.infoFascicolo.idFascicolo))
					{
						obj.UpdateTipoDirittoTrasm(idGruppo, objTrasm.infoFascicolo.idFascicolo);
						obj.UpdateTipoDirittoTrasmFolder(idGruppo, objTrasm.infoFascicolo.idFascicolo);
					}
				}
			}
			if (ts.corrispondenteInterno is DocsPaVO.utente.Utente)
			{
				DocsPaVO.utente.Utente utente = ((DocsPaVO.utente.Utente)ts.corrispondenteInterno);
				string idPeople = utente.idPeople;
				string valoreChiaveAtipicita = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(utente.idAmministrazione, "ATIPICITA_DOC_FASC");
				if (string.IsNullOrEmpty(valoreChiaveAtipicita))
				{
					valoreChiaveAtipicita = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "ATIPICITA_DOC_FASC");
				}
				if (!string.IsNullOrEmpty(valoreChiaveAtipicita) && valoreChiaveAtipicita.Equals("1"))
				{
					if (objTrasm.infoDocumento != null && !String.IsNullOrEmpty(objTrasm.infoDocumento.docNumber))
					{
						obj.UpdateTipoDirittoTrasm(idPeople, objTrasm.infoDocumento.docNumber);
					}
					if (objTrasm.infoFascicolo != null && !String.IsNullOrEmpty(objTrasm.infoFascicolo.idFascicolo))
					{
						obj.UpdateTipoDirittoTrasm(idPeople, objTrasm.infoFascicolo.idFascicolo);
						obj.UpdateTipoDirittoTrasmFolder(idPeople, objTrasm.infoFascicolo.idFascicolo);
					}
				}
			}
		}
		//}
		logger.Information("END");
	}

	/// <summary>
	/// Notifica dell'avvenuta trasmissione al documentale
	/// </summary>
	/// <param name="trasmissione"></param>
	/// <param name="infoSecurityList"></param>
	private static void NotifyTrasmissioneDocumentale(DocsPaVO.trasmissione.Trasmissione trasmissione, DocsPaVO.trasmissione.infoSecurity[] infoSecurityList)
	{
		logger.Information("BEGIN");
		// if (infoSecurityList.Length > 0)
		// {
		DocsPaVO.utente.InfoUtente infoUtente = new DocsPaVO.utente.InfoUtente(trasmissione.utente, trasmissione.ruolo);

		DocsPaDocumentale.Interfaces.IAclEventListener eventsNotification = new DocsPaDocumentale.Documentale.AclEventListener(infoUtente);
		eventsNotification.TrasmissioneCompletataEventHandler(trasmissione, infoSecurityList);
		// }
		logger.Information("END");
	}

	/// <summary>
	/// Manda l'email agli utenti della trasmissione
	/// </summary>
	/// <param name="objTrasm"></param>
	public static void InviaEmailUtentiTrasm(string path, DocsPaVO.trasmissione.Trasmissione objTrasm)
	{
		try
		{
			//si costruisce il corpo della mail
			logger.Debug("Costruzione mail");
			string bodyMail = "";
			string subject = "";
			string priorita = null;
			for (int i = 0; i < objTrasm.trasmissioniSingole.Count; i++)
			{
				DocsPaVO.trasmissione.TrasmissioneSingola currTrasmSing = (DocsPaVO.trasmissione.TrasmissioneSingola)objTrasm.trasmissioniSingole[i];
				if (currTrasmSing.ragione.notifica == null || (currTrasmSing.ragione.notifica != null && !currTrasmSing.ragione.notifica.Equals("NN")))
				{
					string oggetto;
					if (((string)DocsPaVO.trasmissione.Trasmissione.oggettoStringa[objTrasm.tipoOggetto]).Equals("D"))
					{
						oggetto = "documento";
					}
					else
					{
						oggetto = "fascicolo";
					}
					//bodyMail = "<font face='Arial'>Le è stato trasmesso con ragione " + currTrasmSing.ragione.descrizione;
					//logger.Debug(bodyMail);
					//if (currTrasmSing.tipoDest == DocsPaVO.trasmissione.TipoDestinatario.RUOLO)
					//{
					//    bodyMail = bodyMail + " al ruolo " + ((DocsPaVO.utente.Ruolo)currTrasmSing.corrispondenteInterno).descrizione;
					//    logger.Debug(bodyMail);
					//}

					//if (currTrasmSing.dataScadenza != null && !currTrasmSing.dataScadenza.Equals(""))
					//{
					//    bodyMail = bodyMail + " (con scadenza " + currTrasmSing.dataScadenza + ")";
					//}
					//bodyMail = bodyMail + " il seguente " + oggetto + "<br>";
					//if (objTrasm.noteGenerali != null && !objTrasm.noteGenerali.Equals(""))
					//{
					//    bodyMail = bodyMail + "NOTE GENERALI: " + objTrasm.noteGenerali + "<br>";
					//}
					//if (currTrasmSing.noteSingole != null && !currTrasmSing.noteSingole.Equals(""))
					//{
					//    bodyMail = bodyMail + "NOTE SINGOLE: " + currTrasmSing.noteSingole + "<br>";
					//}
					//if (objTrasm.infoDocumento != null)
					//{
					//    bodyMail = bodyMail + "OGGETTO: " + objTrasm.infoDocumento.oggetto + "<br>";
					//    if (objTrasm.infoDocumento.segnatura != null)
					//    {
					//        bodyMail = bodyMail + "SEGNATURA: " + objTrasm.infoDocumento.segnatura + "<br>";
					//    }
					// priorita = objTrasm.infoDocumento.evidenza;
					//}
					//else
					//{
					//    bodyMail = bodyMail + "FASCICOLO: " + objTrasm.infoFascicolo.codice + " (" + objTrasm.infoFascicolo.descrizione + ")<br>";
					//}
					if ((objTrasm.infoDocumento != null))
					{
						priorita = objTrasm.infoDocumento.evidenza;
					}
					bodyMail = creaBodyMessageNotifica(objTrasm, currTrasmSing);

					string defaultSubject = string.Empty;
					if (DocsPaVO.Settings.AppSettings.Instance.APPLICATION_NAME != null &&
                                DocsPaVO.Settings.AppSettings.Instance.APPLICATION_NAME.ToString().Trim() != "")
					{
						defaultSubject = DocsPaVO.Settings.AppSettings.Instance.APPLICATION_NAME.ToString();
					}

					//new : defaultSubject andrà letto da una chiave di web.config

					if (defaultSubject.Trim() != "")
					{
						subject = defaultSubject.Trim() + " - Trasmissione";
					}
					else
					{
						subject = "Trasmissione";
					}
					subject += " " + oggetto;
					subject += " : ";
					if (objTrasm.infoDocumento != null)
					{

						if (objTrasm.infoDocumento.oggetto.Length > 172)
						{
							subject += objTrasm.infoDocumento.oggetto.Substring(0, 171) + " ...";
						}
						else
						{
							subject += objTrasm.infoDocumento.oggetto;
						}
					}
					else
					{
						if (objTrasm.infoFascicolo.descrizione.Length > 172)
						{
							subject += objTrasm.infoFascicolo.descrizione.Substring(0, 171) + " ...";
						}
						else
						{
							subject += objTrasm.infoFascicolo.descrizione;
						}
					}
					// old - subject = "Trasmissione " + oggetto;

					string indirizzoMitt = null;
					DocsPaDB.Query_DocsPAWS.Utenti utente = new DocsPaDB.Query_DocsPAWS.Utenti();
					string fromEmail = utente.GetFromEmailUtente(objTrasm.utente.idPeople);

					if (fromEmail != null && !fromEmail.Equals(""))
					{
						indirizzoMitt = fromEmail;
					}
					else
					{
						//Ricerca se esiste l'email from notifica dell'amministrazione
						DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
						string idAmministrazionePerMail = string.Empty;
						if (objTrasm.utente != null && string.IsNullOrEmpty(objTrasm.utente.idAmministrazione) && objTrasm.ruolo != null && !string.IsNullOrEmpty(objTrasm.ruolo.idAmministrazione))
						{
							idAmministrazionePerMail = objTrasm.ruolo.idAmministrazione;
						}
						else
						{
							idAmministrazionePerMail = objTrasm.utente.idAmministrazione;
						}
						string fromEmailAmministra = amm.GetEmailAddress(idAmministrazionePerMail);

						if (fromEmailAmministra != null && !fromEmailAmministra.Equals(""))
						{
							indirizzoMitt = fromEmailAmministra;
						}
						else
						{
							if (objTrasm.utente.email != null && !objTrasm.utente.email.Equals(""))
							{
								indirizzoMitt = objTrasm.utente.email;
							}
							else
							{   // PALUMBO: per ovviare al caso di trasmissione salvata e succ. trasmessa (non arriva l'objTrasm.utente.email)
								if (objTrasm.utente.idPeople != null && !objTrasm.utente.idPeople.Equals(""))
								{
									indirizzoMitt = utente.GetEmailUtente(objTrasm.utente.idPeople);
								}
								// se l'utente mittente della trasmisisone non ha l'email viene presa dal web.config del WS
								else if (DocsPaVO.Settings.AppSettings.Instance.mittenteNotificaTrasmissione != null)
								{
									indirizzoMitt = DocsPaVO.Settings.AppSettings.Instance.mittenteNotificaTrasmissione;
								}
							}
						}
					}

					string realBodyMail = bodyMail;
					// ----------------------------------------------------------------
					//   invio EMAIL a tutti gli utenti
					// ----------------------------------------------------------------
					CMAttachment[] l_attachments = null;
					if (oggetto != "fascicolo")
					{
						DocsPaVO.utente.InfoUtente l_infoUtente = new DocsPaVO.utente.InfoUtente(objTrasm.utente, objTrasm.ruolo);
						DocsPaVO.documento.SchedaDocumento l_schedaDocumento = BusinessLogic.Documenti.DocManager.getDettaglioPerNotificaAllegatiNoSecurity(l_infoUtente, objTrasm.infoDocumento.idProfile, objTrasm.infoDocumento.docNumber);
                            // creazione allegati della mail
						l_attachments = GetAttachments(l_schedaDocumento, l_infoUtente);
                        }
                        for (int j = 0; j < currTrasmSing.trasmissioneUtente.Count; j++)
					{
						DocsPaVO.trasmissione.TrasmissioneUtente tu = (DocsPaVO.trasmissione.TrasmissioneUtente)currTrasmSing.trasmissioneUtente[j];
						logger.Debug("invio email a utente nr." + j + " - indirizzo: " + tu.utente.email);
						string mail = tu.utente.email;
						if (mail != null && !mail.Equals(""))
						{
							string tipoNotifica = "";

							//la ragione non ha notifica, vedo se ce l'ha l'utente
							if (((currTrasmSing.ragione.notifica == null)
								|| (currTrasmSing.ragione.notifica != null && (currTrasmSing.ragione.notifica == ""))))
							{
								if (tu.utente.notifica != null)
								{
									switch (tu.utente.notifica)
									{
										case "E":
											{
												tipoNotifica = "E"; // solo link
												if (pf_notificaConAllegato(tu.utente))
												{
													tipoNotifica = "ED"; // link e allegati
												}
											}
											break;
										case "":
											{
												tipoNotifica = ""; // nessuna notifica
												if (pf_notificaConAllegato(tu.utente))
												{
													tipoNotifica = "EA"; // solo allegati
												}
											}
											break;
									}
								}
							}
							else
							{
								tipoNotifica = currTrasmSing.ragione.notifica.ToString();
							}

							//Cerco gli allegati solo per i documenti, nel caso in cui:
							// - la notifica della ragione è del tipo EA o ED
							// - la notifica dell'utente è del tipo EA o ED
							/*
							if ((tipoNotifica.Equals("EA") || tipoNotifica.Equals("ED"))
								 && oggetto != "fascicolo")
							{


								DocsPaVO.utente.InfoUtente l_infoUtente = new DocsPaVO.utente.InfoUtente(objTrasm.utente, objTrasm.ruolo);
								DocsPaVO.documento.SchedaDocumento l_schedaDocumento = BusinessLogic.Documenti.DocManager.getDettaglioPerNotificaAllegati(l_infoUtente, objTrasm.infoDocumento.idProfile, objTrasm.infoDocumento.docNumber);


								// creazione allegati della mail
								l_attachments = GetAttachments(l_schedaDocumento, l_infoUtente);
							}
							*/
							switch (tipoNotifica)
							{
								case "E": // TRAMITE EMAIL (SOLO LINK)
								case "ED": // TRAMITE EMAIL (LINK E ALLEGATI)
									{
										bodyMail = realBodyMail;

										bodyMail += "<br />";

										if (objTrasm.infoDocumento != null)
										{
											// Creazione oggetto responsabile del reperimento delle
											// informazioni sul documento
											DocsPaDB.Query_DocsPAWS.Documenti documenti = new DocsPaDB.Query_DocsPAWS.Documenti();

											// Reperimento delle informazioni sul documento
											DocsPaVO.documento.InfoDocumento infoDoc = documenti.GetInfoDocumento(
												objTrasm.ruolo.idGruppo,
												objTrasm.utente.idPeople,
												objTrasm.infoDocumento.docNumber,
												false);


											//if (currTrasmSing.corrispondenteInterno != null && currTrasmSing.corrispondenteInterno.tipoCorrispondente.Equals("R"))
											if (currTrasmSing.corrispondenteInterno != null && currTrasmSing.corrispondenteInterno.GetType().Equals(typeof(DocsPaVO.utente.Ruolo)))
											{
												DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
												DocsPaVO.utente.Ruolo role = utenti.getRuoloById(currTrasmSing.corrispondenteInterno.systemId);
												// Link all'immagine del documento
												if (role != null)
												{
													if (infoDoc != null && infoDoc.acquisitaImmagine != "0")
														bodyMail += String.Format(
															"<br />Link all'<a href='{0}/visualizzaLink.aspx?docNumber={1}&idProfile={2}&groupId={3}&numVersion='>immagine del documento</a><br />",
															path, objTrasm.infoDocumento.docNumber, objTrasm.infoDocumento.idProfile, role.idGruppo);

													// Link alla scheda documento
													bodyMail += String.Format("<br />Link alla <a href='{0}/VisualizzaOggetto.htm?idAmministrazione={1}&tipoOggetto=D&idObj={2}&tipoProto={3}&groupId={4}'>scheda di dettaglio del documento</a>",
														path, objTrasm.utente.idAmministrazione, objTrasm.infoDocumento.docNumber, objTrasm.infoDocumento.tipoProto, role.idGruppo);
												}
												else
												{
													if (infoDoc != null && infoDoc.acquisitaImmagine != "0")
														bodyMail += String.Format(
														   "<br />Link all'<a href='{0}/visualizzaLink.aspx?docNumber={1}&idProfile={2}&numVersion='>immagine del documento</a><br />",
														   path, objTrasm.infoDocumento.docNumber, objTrasm.infoDocumento.idProfile);
													// Link alla scheda documento
													bodyMail += String.Format("<br />Link alla <a href='{0}/VisualizzaOggetto.htm?idAmministrazione={1}&tipoOggetto=D&idObj={2}&tipoProto={3}'>scheda di dettaglio del documento</a>",
														path, objTrasm.utente.idAmministrazione, objTrasm.infoDocumento.docNumber, objTrasm.infoDocumento.tipoProto);
												}
											}
											else
											{
												if (infoDoc != null && infoDoc.acquisitaImmagine != "0")
													bodyMail += String.Format(
													   "<br />Link all'<a href='{0}/visualizzaLink.aspx?docNumber={1}&idProfile={2}&numVersion='>immagine del documento</a><br />",
													   path, objTrasm.infoDocumento.docNumber, objTrasm.infoDocumento.idProfile);
												// Link alla scheda documento
												bodyMail += String.Format("<br />Link alla <a href='{0}/VisualizzaOggetto.htm?idAmministrazione={1}&tipoOggetto=D&idObj={2}&tipoProto={3}'>scheda di dettaglio del documento</a>",
													path, objTrasm.utente.idAmministrazione, objTrasm.infoDocumento.docNumber, objTrasm.infoDocumento.tipoProto);
											}
										}
										else
										{
											if (currTrasmSing.corrispondenteInterno != null && currTrasmSing.corrispondenteInterno.GetType().Equals(typeof(DocsPaVO.utente.Ruolo)))
											{
												DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
												DocsPaVO.utente.Ruolo role = utenti.getRuoloById(currTrasmSing.corrispondenteInterno.systemId);

												bodyMail += "<br />Link alla ";
												bodyMail = bodyMail + "<a href='" + path + "/VisualizzaOggetto.htm?idAmministrazione=" + objTrasm.utente.idAmministrazione;
												bodyMail = bodyMail + "&tipoOggetto=F&idObj=" + objTrasm.infoFascicolo.codice + "&groupId=" + role.idGruppo + "'>scheda di dettaglio del fascicolo</a>";
											}
											else
											{
												bodyMail += "<br />Link alla ";
												bodyMail = bodyMail + "<a href='" + path + "/VisualizzaOggetto.htm?idAmministrazione=" + objTrasm.utente.idAmministrazione;
												bodyMail = bodyMail + "&tipoOggetto=F&idObj=" + objTrasm.infoFascicolo.codice + "'>scheda di dettaglio del fascicolo</a>"; ;
											}
										}
										bodyMail += "</font>";
									}
									break;
								default:
									break;
							}


							bool res;
							if (tipoNotifica != "")
							{
								if (indirizzoMitt != null && !indirizzoMitt.Equals(""))//se indirizzo email mittente è presente
								{
									if (tipoNotifica.Equals("EA") || tipoNotifica.Equals("ED"))
										res = BusinessLogic.Interoperabilita.Notifica.notificaByMail(mail, indirizzoMitt, subject, bodyMail, priorita, tu.utente.idAmministrazione, l_attachments);
									else
										res = BusinessLogic.Interoperabilita.Notifica.notificaByMail(mail, indirizzoMitt, subject, bodyMail, priorita, tu.utente.idAmministrazione, null);
									res = true; // messo per compilare
								}
								else
								{
									if (indirizzoMitt == null)
									{
										logger.Debug("Errore nella gestione delle trasmissioni: indirizzo email del mittente assente");
										logger.Debug("Aggiungere la chiave mittenteNotificaTrasmissione sul web.config del WS e valorizzarla.");
									}
									if (indirizzoMitt != null && indirizzoMitt.Equals(""))
									{
										logger.Debug("Errore nella gestione delle trasmissioni: indirizzo email del mittente assente");
										logger.Debug("Valorizzare la chiave mittenteNotificaTrasmissione sul web.config del WS.");
									}
									res = false;
								}
								if (res == false)
								{
									logger.Debug("Errore nella gestione delle trasmissioni (executeTrasmMethod)");
									throw new Exception("Errore in : executeTrasmMethod");
								}
							}
						}
					}
				}
			}
		}
		catch (Exception e)
		{
			logger.Debug("Errore durante la spedizione delle email.", e);
			objTrasm.ErrorSendingEmails = true;
		}
	}

	/// <summary>
	/// Generazione delle query che serviranno alla propagazione della visibilità
	/// </summary>
	/// <param name="objTrasm">oggetto trasmissione</param>
	/// <param name="estendeVisibilitaGerarchica">True = estende anche ai superiori gerarchici; False = solo al destinatario</param>
	private static void GeneraQueryVisibilita(DocsPaVO.trasmissione.Trasmissione objTrasm, bool estendeVisibilitaGerarchica, ArrayList executeQueries, ArrayList executeListaQueries, int docInFinalState = 0)
	{
		//si ricava l'identificativo dell'oggetto
		string idObj = "";
		string systemIdFolderControllato = null;
		if (objTrasm.tipoOggetto == DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO)
		{
			if (objTrasm.infoDocumento != null)
			{
				idObj = objTrasm.infoDocumento.idProfile;

				ForceHideDocPreviousVersionsSetting(objTrasm);

				CheckIfDocumentIsConsolidated(objTrasm);
			}
		}
		if (objTrasm.tipoOggetto == DocsPaVO.trasmissione.TipoOggetto.FASCICOLO)
		{
			if (objTrasm.infoFascicolo != null)
				idObj = objTrasm.infoFascicolo.idFascicolo;
		}

		ArrayList ruoliSuperiori = new ArrayList();

		//si estraggono le singole trasmissioni
		for (int i = 0; i < objTrasm.trasmissioniSingole.Count; i++)
		{
			DocsPaVO.trasmissione.TrasmissioneSingola currentTrasmissione = (DocsPaVO.trasmissione.TrasmissioneSingola)objTrasm.trasmissioniSingole[i];
			logger.Debug("Query relative all'idObj " + idObj);

			ArrayList queryObj = getQueryTrasm(idObj, objTrasm, currentTrasmissione, false, estendeVisibilitaGerarchica, ref ruoliSuperiori);

			for (int j = 0; j < queryObj.Count; j++)
			{
				if (!executeListaQueries.Contains(queryObj[j]))
				{
					//devo capire se fare Update del record o nulla
					int res = verificaDuplicazioneSecurity(executeListaQueries, (DocsPaVO.trasmissione.infoSecurity)queryObj[j]);
					string query = "";
					if (res > 0)
					{
						if (res == 1)
						{
							//se il documento ha associato un diagramma che si trova in stato finale allora i
							//superiori devono ereditare con diritti a 45
							if (docInFinalState == 1)
							{
								((DocsPaVO.trasmissione.infoSecurity)queryObj[j]).accessRights = "45";
							}
							string idGruppoTrasm = ((DocsPaVO.trasmissione.infoSecurity)queryObj[j]).idGruppoTrasm;
							query = "INSERT INTO SECURITY " +
								"(THING, PERSONORGROUP, ACCESSRIGHTS, ID_GRUPPO_TRASM, CHA_TIPO_DIRITTO, HIDE_DOC_VERSIONS) " +
								"VALUES ("
								+ ((DocsPaVO.trasmissione.infoSecurity)queryObj[j]).thing + ","
								+ ((DocsPaVO.trasmissione.infoSecurity)queryObj[j]).personOrGroup + ","
								+ ((DocsPaVO.trasmissione.infoSecurity)queryObj[j]).accessRights + ","
								+ (!string.IsNullOrEmpty(idGruppoTrasm) ? idGruppoTrasm : "NULL") + ",'"
								+ ((DocsPaVO.trasmissione.infoSecurity)queryObj[j]).chaTipoDiritto + "', "
								+ (((DocsPaVO.trasmissione.infoSecurity)queryObj[j]).hideDocPreviousVersions ? "'1'" : "NULL") + ")";
						}
						if (res == 2)
						{
							//se il documento ha associato un diagramma che si trova in stato finale allora i
							//superiori devono ereditare con diritti a 45
							if (docInFinalState == 1)
							{
								((DocsPaVO.trasmissione.infoSecurity)queryObj[j]).accessRights = "45";
							}
							string idGruppoTrasm = ((DocsPaVO.trasmissione.infoSecurity)queryObj[j]).idGruppoTrasm;
							query = "UPDATE SECURITY SET ACCESSRIGHTS=" + ((DocsPaVO.trasmissione.infoSecurity)queryObj[j]).accessRights
								+ ",ID_GRUPPO_TRASM=" + (!string.IsNullOrEmpty(idGruppoTrasm) ? idGruppoTrasm : null)
								+ ", CHA_TIPO_DIRITTO='" + ((DocsPaVO.trasmissione.infoSecurity)queryObj[j]).chaTipoDiritto
								+ "', HIDE_DOC_VERSIONS=" + (((DocsPaVO.trasmissione.infoSecurity)queryObj[j]).hideDocPreviousVersions ? "'1'" : "NULL")
								+ " WHERE THING=" + ((DocsPaVO.trasmissione.infoSecurity)queryObj[j]).thing
								+ " AND PERSONORGROUP=" + ((DocsPaVO.trasmissione.infoSecurity)queryObj[j]).personOrGroup;
						}
						executeListaQueries.Add(queryObj[j]);
						executeQueries.Add(query);
					}
				}
				else
					logger.Debug("QUERY RIPETUTA QUINDI NON ESEGUITA: " + queryObj[j].ToString());
			}

			// FASCICOLI
			if (objTrasm.tipoOggetto == DocsPaVO.trasmissione.TipoOggetto.FASCICOLO)
			{
				ArrayList regs = null;

				//Verifica se il fascicolo è controllato o meno
				DocsPaDB.Query_DocsPAWS.Fascicoli fasc = new DocsPaDB.Query_DocsPAWS.Fascicoli();
				string controllato = fasc.isControllatoModified(objTrasm.infoFascicolo.idFascicolo);

				if (!string.IsNullOrEmpty(controllato) && controllato.Equals("1"))
				{

					DocsPaVO.utente.InfoUtente infoUtente = new DocsPaVO.utente.InfoUtente();
					infoUtente.idPeople = objTrasm.utente.idPeople;
					infoUtente.idGruppo = objTrasm.ruolo.idGruppo;
					infoUtente.idAmministrazione = objTrasm.utente.idAmministrazione;
					DocsPaDB.Query_DocsPAWS.Utenti usersDb = new DocsPaDB.Query_DocsPAWS.Utenti();
					infoUtente.idCorrGlobali = (usersDb.getUtenteById(infoUtente.idPeople)).systemId;

					DocsPaVO.fascicolazione.Fascicolo tempFasc = fasc.GetFascicoloById(idObj, infoUtente);

					DocsPaVO.fascicolazione.Folder tempFold = BusinessLogic.Fascicoli.FolderManager.getFolder(objTrasm.utente.idPeople, objTrasm.ruolo.idGruppo, tempFasc);

					systemIdFolderControllato = tempFold.systemID;

				}

				// RUOLO
				if (currentTrasmissione.corrispondenteInterno.GetType().Equals(typeof(DocsPaVO.utente.Ruolo)))
					regs = BusinessLogic.Utenti.RegistriManager.getRegistriRuolo(currentTrasmissione.corrispondenteInterno.systemId);

				// UTENTE
				if (currentTrasmissione.corrispondenteInterno.GetType().Equals(typeof(DocsPaVO.utente.Utente)))
				{
					DocsPaVO.utente.Utente ut = (DocsPaVO.utente.Utente)currentTrasmissione.corrispondenteInterno;
					ArrayList ruoli = BusinessLogic.Utenti.UserManager.getRuoliUtente(ut.idPeople);
					ArrayList rgs = new ArrayList();
					if (ruoli != null && ruoli.Count > 0)
					{
						for (int j = 0; j < ruoli.Count; j++)
						{
							DocsPaVO.utente.Ruolo ruo = (DocsPaVO.utente.Ruolo)ruoli[j];
							// prende i registri
							regs = BusinessLogic.Utenti.RegistriManager.getRegistriRuolo(ruo.systemId);
							for (int k = 0; k < regs.Count; k++)
								if (!rgs.Contains(regs[k]))
									rgs.Add(regs[k]);
						}
					}
					regs = rgs;
				}

				ArrayList listaIdObj = BusinessLogic.Fascicoli.FolderManager.getIdFolderDoc(idObj, regs);
				for (int k = 0; k < listaIdObj.Count; k++)
				{
					logger.Debug("Query relative all'idObj " + listaIdObj[k].ToString());
					ArrayList queryObj2 = getQueryTrasm(listaIdObj[k].ToString(), objTrasm, currentTrasmissione, true, estendeVisibilitaGerarchica, ref ruoliSuperiori);

					for (int j = 0; j < queryObj2.Count; j++)
					{
						if (!executeListaQueries.Contains(queryObj2[j]))
						{
							//devo capire se fare Updare del record o nulla
							int res = verificaDuplicazioneSecurity(executeListaQueries, (DocsPaVO.trasmissione.infoSecurity)queryObj2[j]);
							string query = "";
							if (res > 0)
							{
								//se il documento ha associato un diagramma che si trova in stato finale allora i
								//superiori devono ereditare con diritti a 45
								if (docInFinalState == 1)
								{
									((DocsPaVO.trasmissione.infoSecurity)queryObj[j]).accessRights = "45";
								}

								if (res == 1)
									query = "INSERT INTO SECURITY " +
										"(THING, PERSONORGROUP, ACCESSRIGHTS, ID_GRUPPO_TRASM, CHA_TIPO_DIRITTO, HIDE_DOC_VERSIONS) VALUES (" +
										((DocsPaVO.trasmissione.infoSecurity)queryObj2[j]).thing + ","
										+ ((DocsPaVO.trasmissione.infoSecurity)queryObj2[j]).personOrGroup + ","
										+ ((DocsPaVO.trasmissione.infoSecurity)queryObj2[j]).accessRights + ","
										+ ((DocsPaVO.trasmissione.infoSecurity)queryObj2[j]).idGruppoTrasm + ",'"
										+ ((DocsPaVO.trasmissione.infoSecurity)queryObj2[j]).chaTipoDiritto + "',"
										+ (((DocsPaVO.trasmissione.infoSecurity)queryObj2[j]).hideDocPreviousVersions ? "'1'" : "NULL") + ")";

								if (res == 2)
									query = "UPDATE SECURITY SET ACCESSRIGHTS=" + ((DocsPaVO.trasmissione.infoSecurity)queryObj2[j]).accessRights
										+ ",ID_GRUPPO_TRASM=" + ((DocsPaVO.trasmissione.infoSecurity)queryObj2[j]).idGruppoTrasm
										+ ", CHA_TIPO_DIRITTO='" + ((DocsPaVO.trasmissione.infoSecurity)queryObj2[j]).chaTipoDiritto
										+ "', HIDE_DOC_VERSIONS=" + (((DocsPaVO.trasmissione.infoSecurity)queryObj2[j]).hideDocPreviousVersions ? "'1'" : "NULL")
										+ " WHERE THING='" + ((DocsPaVO.trasmissione.infoSecurity)queryObj2[j]).thing
											+ "' AND PERSONORGROUP=" + ((DocsPaVO.trasmissione.infoSecurity)queryObj2[j]).personOrGroup;

								executeListaQueries.Add(queryObj2[j]);
								executeQueries.Add(query);
							}
						}
						else
							logger.Debug("QUERY RIPETUTA QUINDI NON ESEGUITA: " + queryObj2[j].ToString());
					}
				}
			}
		}
	}

	/// <summary>
	/// Ritorna il body dell'email che verrà inviata in caso di notifica
	/// </summary>
	/// <param name="trasm"></param>
	/// <param name="trasmSingola"></param>
	/// <returns></returns>
	public static string creaBodyMessageNotifica(DocsPaVO.trasmissione.Trasmissione trasm, DocsPaVO.trasmissione.TrasmissioneSingola trasmSingola)
	{
		string bodyMail = "<font face='Arial'>";

		if (trasm.infoDocumento != null)
		{
			//testo per notifiche doc

			string testDoc = trasmSingola.ragione.testoMsgNotificaDoc;
			bodyMail = bodyMail + testDoc.Replace("\n", "<BR>");

			bodyMail = bodyMail.Replace("RAG_TRASM", "<B>" + trasmSingola.ragione.descrizione + "</B>");
			if (trasmSingola.tipoDest == DocsPaVO.trasmissione.TipoDestinatario.RUOLO)
			{
				bodyMail = bodyMail.Replace("DEST_TRASM", "<B>" + ((DocsPaVO.utente.Ruolo)trasmSingola.corrispondenteInterno).descrizione + "</B>");
			}
			if (trasmSingola.tipoDest == DocsPaVO.trasmissione.TipoDestinatario.UTENTE)
			{
				{
					bodyMail = bodyMail.Replace("DEST_TRASM", "<B>" + ((DocsPaVO.utente.Utente)trasmSingola.corrispondenteInterno).descrizione + "</B>");
				}
			}

			if (trasm.noteGenerali != null && trasm.noteGenerali != string.Empty)
			{
				bodyMail = bodyMail.Replace("NOTE_GEN", "<B>" + trasm.noteGenerali + "</B>");
			}
			else
			{
				bodyMail = bodyMail.Replace("NOTE_GEN", "---------");
			}

			if (trasmSingola.noteSingole != null && trasmSingola.noteSingole != string.Empty)
			{
				bodyMail = bodyMail.Replace("NOTE_IND", "<B>" + trasmSingola.noteSingole + "</B>");
			}
			else
			{
				bodyMail = bodyMail.Replace("NOTE_IND", "---------");
			}

			if (!string.IsNullOrEmpty(trasm.infoDocumento.segnatura)) // solo per i protocolli metto la segnatura
			{
				bodyMail = bodyMail.Replace("SEGN_ID_DOC", "<B>" + trasm.infoDocumento.segnatura + "</B>");
			}
			else
			{
				bodyMail = bodyMail.Replace("SEGN_ID_DOC", "<B>" + trasm.infoDocumento.idProfile + "</B>");
			}
			#region old
			//if (trasm.infoDocumento.mittDest[0] != null && trasm.infoDocumento.mittDest.Count > 0 && !string.IsNullOrEmpty(trasm.infoDocumento.mittDest[0].ToString()))
			//    bodyMail = bodyMail.Replace("MITT_PROTO", "<B>" + trasm.infoDocumento.mittDest[0].ToString() + "</B>");
			//else
			//    bodyMail = bodyMail.Replace("MITT_PROTO", "---------");
			#endregion

			string mittDest = string.Empty;

			try
			{
				if (trasm.infoDocumento.tipoProto == "A")
				{
					if (trasm.infoDocumento.mittDest != null && trasm.infoDocumento.mittDest.Count > 0)
						mittDest = trasm.infoDocumento.mittDest[0] as string;

					if (string.IsNullOrEmpty(mittDest))
						bodyMail = bodyMail.Replace("MITT_PROTO", "---------");
					else
						bodyMail = bodyMail.Replace("MITT_PROTO", string.Format("<B>{0}</B>", mittDest));
				}
				else
					bodyMail = bodyMail.Replace("MITT_PROTO", "---------");
			}
			catch (Exception e) { logger.Debug(e.Message); }

			bodyMail = bodyMail.Replace("OGG_DOC", "<B>" + trasm.infoDocumento.oggetto + "</B>");
		}
		else
		{
			//testo per notifiche fasc
			string testoFasc = trasmSingola.ragione.testoMsgNotificaFasc;
			if (trasm.infoFascicolo != null)
			{
				bodyMail = bodyMail + testoFasc.Replace("\n", "<BR>");

				bodyMail = bodyMail.Replace("RAG_TRASM", "<B>" + trasmSingola.ragione.descrizione + "</B>");
				if (trasmSingola.tipoDest == DocsPaVO.trasmissione.TipoDestinatario.RUOLO)
				{
					bodyMail = bodyMail.Replace("DEST_TRASM", "<B>" + ((DocsPaVO.utente.Ruolo)trasmSingola.corrispondenteInterno).descrizione + "</B>");
				}
				if (trasmSingola.tipoDest == DocsPaVO.trasmissione.TipoDestinatario.UTENTE)
				{
					bodyMail = bodyMail.Replace("DEST_TRASM", "<B>" + ((DocsPaVO.utente.Utente)trasmSingola.corrispondenteInterno).descrizione + "</B>");
				}

				if (trasm.noteGenerali != null && trasm.noteGenerali != string.Empty)
				{
					bodyMail = bodyMail.Replace("NOTE_GEN", "<B>" + trasm.noteGenerali + "</B>");
				}
				else
				{
					bodyMail = bodyMail.Replace("NOTE_GEN", "---------");
				}

				if (trasmSingola.noteSingole != null && trasmSingola.noteSingole != string.Empty)
				{
					bodyMail = bodyMail.Replace("NOTE_IND", "<B>" + trasmSingola.noteSingole + "</B>");
				}
				else
				{
					bodyMail = bodyMail.Replace("NOTE_IND", "---------");
				}
				bodyMail = bodyMail.Replace("COD_FASC", "<B>" + trasm.infoFascicolo.codice + "</B>");
				bodyMail = bodyMail.Replace("DESC_FASC", "<B>" + trasm.infoFascicolo.descrizione + "</B>");
			}
		}


		return bodyMail;
	}

	/// <summary>
	/// Creazione di un array di oggetti "CMAttachment":
	/// rappresentano il documento principale e gli allegati
	/// che vengono inviati per mail
	/// </summary>
	/// <param name="schedaDocumento"></param>
	/// <param name="infoUtente"></param>
	/// <returns></returns>
	private static CMAttachment[] GetAttachments(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.InfoUtente infoUtente)
	{
		System.Int64 fileSize = System.Int64.Parse(((DocsPaVO.documento.Documento)schedaDocumento.documenti[0]).fileSize);
		string pathFiles = string.Empty;
		Dictionary<string, string> CoppiaNomeFileENomeOriginale = null;
		if (fileSize > 0 || (schedaDocumento.allegati != null && schedaDocumento.allegati.Count > 0))
		{
			string codiceRegistro = "";
			if (schedaDocumento.registro != null)
			{
				DocsPaDB.Query_DocsPAWS.Trasmissione trasmissione =
					new DocsPaDB.Query_DocsPAWS.Trasmissione();
				codiceRegistro = trasmissione.GetRegistryCode(schedaDocumento.registro.systemId);
				trasmissione.Dispose();
			}

			//creazione del logger
			//NON VIENE UTILIZZATO
			/*
			string basePathLogger = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_TEMP_PATH");
			basePathLogger = basePathLogger.Replace("%DATA", DateTime.Now.ToString("yyyyMMdd"));
			basePathLogger = basePathLogger + "\\Fax";
			DocsPaUtils.Functions.Functions.CheckEsistenzaDirectory(basePathLogger);
			string pathLogger = basePathLogger + codiceRegistro + "\\invio";
			*/

			// inserimento dei file in una cartella temporanei
			string basePathFiles = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_TEMP_PATH");
			//basePathFiles = basePathFiles.Replace("%DATA", DateTime.Now.ToString("yyyyMMdd"));
			//basePathFiles = basePathFiles + "\\Invio_files";
			//pathFiles = basePathFiles + codiceRegistro;
			if (!string.IsNullOrEmpty(codiceRegistro))
				pathFiles = Path.Combine(basePathFiles, @"Invio_files\" + codiceRegistro + @"\" + Guid.NewGuid().ToString());
			else
				pathFiles = Path.Combine(basePathFiles, @"Invio_files\" + Guid.NewGuid().ToString());
			DocsPaUtils.Functions.Functions.CheckEsistenzaDirectory(pathFiles);
			logger.Debug("Estrazione documento principale da inviare");

			// estrazione del documento principale e degli allegati nella cartella "pathFiles"
			estrazioneDocPrincipale(infoUtente, schedaDocumento, pathFiles, out CoppiaNomeFileENomeOriginale);
		}

		ArrayList list = new ArrayList();

		if (!pathFiles.Equals(string.Empty))
		{
			string[] files = System.IO.Directory.GetFiles(pathFiles);
			if (CoppiaNomeFileENomeOriginale != null)
			{
				foreach (string fileAttPath in CoppiaNomeFileENomeOriginale.Keys)
				{
					var att = new CMAttachment(Path.GetFileName(fileAttPath), Interoperabilita.MimeMapper.GetMimeType(Path.GetExtension(fileAttPath)), fileAttPath);
					if (CoppiaNomeFileENomeOriginale.ContainsKey(fileAttPath.ToLowerInvariant()))
						att.name = CoppiaNomeFileENomeOriginale[fileAttPath.ToLowerInvariant()];

					list.Add(att);
				}
			}
			else
			{   //fallback
				foreach (string fileAttPath in files)
				{
					var att = new CMAttachment(Path.GetFileName(fileAttPath), Interoperabilita.MimeMapper.GetMimeType(Path.GetExtension(fileAttPath)), fileAttPath);
					list.Add(att);
				}
			}
			if (System.IO.Directory.Exists(pathFiles))
				Directory.Delete(pathFiles, true);
		}

		CMAttachment[] retValue = new CMAttachment[list.Count];
		list.CopyTo(retValue);
		list = null;
		return retValue;
	}

	/// <summary>
	/// </summary>
	/// <param name="utente"></param>
	/// <returns></returns>
	private static bool pf_notificaConAllegato(DocsPaVO.utente.Utente utente)
	{
		bool retValue = utente.notificaConAllegato;
		return retValue;
	}

	/// <summary>
	/// Forza l'impostazione "HideDocumentPreviousVersions = true" su tutte le trasmissioni singole
	/// della trasmissione fornita qualora il mittente non abbia i diritti di visibilità completi su tutte le versioni
	/// del documento
	/// </summary>
	/// <param name="objTrasm"></param>
	private static void ForceHideDocPreviousVersionsSetting(DocsPaVO.trasmissione.Trasmissione objTrasm)
	{
		using (DocsPaDB.Query_DocsPAWS.Documenti documentiDb = new DocsPaDB.Query_DocsPAWS.Documenti())
		{
			if (!documentiDb.HasDocumentVersionsFullVisibility(objTrasm.infoDocumento.idProfile, objTrasm.utente.idPeople, objTrasm.ruolo.systemId))
			{
				foreach (DocsPaVO.trasmissione.TrasmissioneSingola ts in objTrasm.trasmissioniSingole)
					ts.hideDocumentPreviousVersions = true;
			}
		}
	}

	/// <summary>
	/// Verifica se il documento da trasmettere è consolidato o meno,
	/// in funzione dell'impostazione nascondi versioni precedenti
	/// </summary>
	/// <param name="objTrasm"></param>
	private static void CheckIfDocumentIsConsolidated(DocsPaVO.trasmissione.Trasmissione objTrasm)
	{
		if (objTrasm.tipoOggetto == DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO && objTrasm.infoDocumento != null)
		{
			DocsPaVO.documento.DocumentConsolidationStateInfo consolidationState = BusinessLogic.Documenti.DocumentConsolidation.GetState(
							new DocsPaVO.utente.InfoUtente(objTrasm.utente, objTrasm.ruolo),
							objTrasm.infoDocumento.idProfile);

			foreach (DocsPaVO.trasmissione.TrasmissioneSingola ts in objTrasm.trasmissioniSingole)
			{
				if (ts.hideDocumentPreviousVersions)
				{
					if (consolidationState.State == DocsPaVO.documento.DocumentConsolidationStateEnum.None)
					{
						// La trasmissione con opzioni "Nascondi versioni precedenti" può essere
						// applicata solo ad un documento in stato consolidato
						ts.hideDocumentPreviousVersions = false;
					}
				}
			}
		}
	}

	/// <summary>
	/// Ricava le query utili alla visibilità
	/// </summary>
	/// <param name="idObj"></param>
	/// <param name="objTrasm"></param>
	/// <param name="currentTrasmissione"></param>
	/// <param name="trasmessoConFasc"></param>
	/// <returns></returns>
	private static ArrayList getQueryTrasm(string idObj,
			DocsPaVO.trasmissione.Trasmissione objTrasm,
			DocsPaVO.trasmissione.TrasmissioneSingola currentTrasmissione,
			bool trasmessoConFasc,
			 bool estendeVisibilitaGerarchica,
			ref ArrayList ruoliSuperiori)
	{
		//LogTrasmissioneSingola("currentTrasmissione", currentTrasmissione);

		//if (objTrasm != null)
		//{
		//    int index = 0;
		//    foreach (DocsPaVO.trasmissione.TrasmissioneSingola item in objTrasm.trasmissioniSingole)
		//    {
		//        LogTrasmissioneSingola(string.Format("objTrasm.trasmissioniSingole[{0}]", index), item);
		//        index++;
		//    }
		//}
		//else
		//    Debugger.Write("objTrasm NULL");

		ArrayList returnList = new ArrayList();
		string codiceACL = "";
		string idRegistro = null;
		string idNodoTitolario = null;
		string tipoDiritto = null;
		string idPeopleGroup = null;

		try
		{
			#region codice commentato
			//// imposta l'ID (idPeopleGroup) del destinatario (RUOLO o UTENTE)
			////  if (currentTrasmissione.corrispondenteInterno.GetType() == typeof(DocsPaVO.utente.Ruolo))
			//string q = "SELECT CHA_TIPO_URP FROM DPA_CORR_GLOBALI WHERE SYSTEM_ID=" + currentTrasmissione.corrispondenteInterno.systemId;
			//    using (DocsPaDB.DBProvider db = new DocsPaDB.DBProvider())
			//    {
			//        using (IDataReader dr = db.ExecuteReader(q))
			//        {
			//            if (dr.Read())
			//            {
			//                tipoUrp = dr.GetValue(0).ToString();
			//            }
			//        }
			//    }
			//if (currentTrasmissione.corrispondenteInterno.tipoCorrispondente.ToUpper() == "R")
			//{
			//    try
			//    {

			//        idPeopleGroup = ((DocsPaVO.utente.Ruolo)currentTrasmissione.corrispondenteInterno).idGruppo;
			//        logger.Debug("Corrispondente interno: ruolo con id_gruppo = " + idPeopleGroup);

			//        //alcune volte arriva da FE livello o UO null ecco perchè questa porcheria...
			//        if (string.IsNullOrEmpty(((DocsPaVO.utente.Ruolo)currentTrasmissione.corrispondenteInterno).livello))
			//            throw new Exception("il livello del ruolo è NULL.");

			//        if (string.IsNullOrEmpty(((DocsPaVO.utente.Ruolo)currentTrasmissione.corrispondenteInterno).uo.ToString()))
			//            throw new Exception("la UO del ruolo è NULL.");

			//    }
			//    catch (Exception e)
			//    {

			//        logger.Debug(e.Message + " CASO CON ERRORE APSS: ricerca del ruolo corrispondenteInterno della trasmissione");
			//        currentTrasmissione.corrispondenteInterno = BusinessLogic.Utenti.UserManager.getRuolo(currentTrasmissione.corrispondenteInterno.systemId);
			//        idPeopleGroup = ((DocsPaVO.utente.Ruolo)currentTrasmissione.corrispondenteInterno).idGruppo;
			//        logger.Debug("Corrispondente interno: ruolo con id_gruppo = " + idPeopleGroup);

			//    }
			//}
			//if (currentTrasmissione.corrispondenteInterno.GetType() == typeof(DocsPaVO.utente.Utente))
			//{
			//    idPeopleGroup = ((DocsPaVO.utente.Utente)currentTrasmissione.corrispondenteInterno).idPeople;
			//    logger.Debug("Corrispondente interno: utente - idPeople:" + idPeopleGroup);
			//}


			//if (tipoUrp.ToUpper().Equals("R"))
			//{
			//    logger.Debug("Corrispondente interno: ruolo con idGruppo = " + ((DocsPaVO.utente.Ruolo)currentTrasmissione.corrispondenteInterno).idGruppo);
			//    idPeopleGroup = ((DocsPaVO.utente.Ruolo)currentTrasmissione.corrispondenteInterno).idGruppo;
			//}
			////if (currentTrasmissione.corrispondenteInterno.GetType() == typeof(DocsPaVO.utente.Utente))
			//else if (tipoUrp.ToUpper().Equals("P"))
			//{
			//    idPeopleGroup = ((DocsPaVO.utente.Utente)currentTrasmissione.corrispondenteInterno).idPeople;
			//    logger.Debug("Corrispondente interno: utente - idPeople:" + idPeopleGroup);
			//}
			//else throw new Exception("ID_GRUPPO o ID_PEOPLE NULL");
			#endregion

			DocsPaDB.Query_DocsPAWS.Trasmissione trasmissione = new DocsPaDB.Query_DocsPAWS.Trasmissione();
			if (!string.IsNullOrEmpty(currentTrasmissione.corrispondenteInterno.systemId))
				idPeopleGroup = trasmissione.estrazioneTipoIdPeople(currentTrasmissione.corrispondenteInterno.systemId);
			else
				idPeopleGroup = ((DocsPaVO.utente.Utente)currentTrasmissione.corrispondenteInterno).idPeople;

			if (string.IsNullOrEmpty(idPeopleGroup))
				throw new Exception("ID_GRUPPO o ID_PEOPLE NULL");

			// imposta il codice ACL
			DocsPaVO.HMDiritti.HMdiritti HMD = new DocsPaVO.HMDiritti.HMdiritti();

			// nel caso la ragione di trasmissione abbia diritti NONE non viene fatto nulla
			if (currentTrasmissione.ragione.tipoDiritti == DocsPaVO.trasmissione.TipoDiritto.NONE)
				return returnList;

			if (currentTrasmissione.ragione.tipoDiritti == DocsPaVO.trasmissione.TipoDiritto.READ)
			{
				codiceACL = HMD.HMdiritti_Read.ToString();
			}
			else
			{
				codiceACL = HMD.HMdiritti_Eredita.ToString();
			}

			// imposta i dati necessari
			//MEV 2020:Se il documento è privato o ad utente non estendo la visibilita ai superiori
			bool isPrivato = false;
			if (objTrasm.tipoOggetto == DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO)
			{
				idRegistro = objTrasm.infoDocumento.idRegistro;
				isPrivato = objTrasm.infoDocumento.personale == "1" || objTrasm.infoDocumento.privato == "1";
			}
			else
			{
				idRegistro = objTrasm.infoFascicolo.idRegistro;
				idNodoTitolario = objTrasm.infoFascicolo.idClassificazione;
				isPrivato = !string.IsNullOrEmpty(objTrasm.infoFascicolo.privato) && objTrasm.infoFascicolo.privato == "1";
			}

			DocsPaDB.Query_DocsPAWS.Trasmissione obj = new DocsPaDB.Query_DocsPAWS.Trasmissione();

			if (IsEnabledVisibPosticipataInTrasmConWF())
			{
				#region GESTIONE IN DUE FASI: POSTICIPO DELLA VISIBILITà PER LE TRASM.NI CON WORKFLOW (NUOVO)

				// imposta il tipo diritto in base al parametro booleano "trasmessoConFasc"
				if (trasmessoConFasc)
				{
					tipoDiritto = "F";
				}
				else
				{
					if (currentTrasmissione.ragione.tipo.Equals("W"))
						tipoDiritto = "A";
					else
						tipoDiritto = "T";
				}
				// tramite il parametro booleano "estendeVisibilitaGerarchica" il sistema gestisce le 2 fasi del processo di una trasmissione:
				//
				//      Fase 1 - invio della trasm.ne: estende la visibilità solo in "A" (oggetto trasmesso) o "F" (oggetto trasmesso in fascicolo).
				//               Tuttavia in questa fase sarà estesa la visibilità gerarchica se la ragione di trasm.ne prevede ereditarietà (e non Workflow).
				//
				//      Fase 2 - la trasmissione prevedeva Workflow ed è stata accettata in un secondo momento: ora si estendere SOLO la visibilità gerarchica
				//

				//Caso anomalia IS per cui estraeva i ruoli storicizzati, in questo caso la uo era nulla e quindi estendeva la visibilità a tutti i ruoli. Quindi se la UO è null non estendo
				/*
				if (currentTrasmissione != null && currentTrasmissione.corrispondenteInterno != null && currentTrasmissione.corrispondenteInterno.GetType() == typeof(DocsPaVO.utente.Ruolo))
				{
					if (((DocsPaVO.utente.Ruolo)currentTrasmissione.corrispondenteInterno).uo == null)
						estendeVisibilitaGerarchica = false;
				}
				*/
				if (estendeVisibilitaGerarchica)
				{
					// FASE 2
					tipoDiritto = "A";
					if (currentTrasmissione.ragione.eredita.Equals("1") && !isPrivato)
						returnList = obj.GetQueryTrasmEstVisibSup(currentTrasmissione, objTrasm, idRegistro, idNodoTitolario, idObj, idPeopleGroup, codiceACL, tipoDiritto, ref ruoliSuperiori);
				}
				else
				{
					// FASE 1 (diritto: A o F)
					string codiceACL_ToSet = codiceACL;
					if (currentTrasmissione.ragione.tipo.Equals("W"))
						codiceACL_ToSet = HMD.HDdiritti_Waiting.ToString();

					returnList = obj.GetQueryTrasm(currentTrasmissione, objTrasm, idRegistro, idNodoTitolario, idObj, idPeopleGroup, codiceACL_ToSet, tipoDiritto);

					// se la ragione di trasm.ne
					//          - NON ha Workflow
					//          - ma prevede ereditarietà gerarchica
					// allora procede subito anche ad estendere la visibilità gerarchica
					if (!currentTrasmissione.ragione.tipo.Equals("W") && currentTrasmissione.ragione.eredita.Equals("1") && !isPrivato)
					{
						if (BusinessLogic.Interoperabilita.Semplificata.InteroperabilitaSemplificataManager.IsDocumentReceivedWithIS(idObj))
							logger.Debug("IS - Estensione dei diritti di visibilità");
						tipoDiritto = "A";
						ArrayList listaSupGer = new ArrayList();
						listaSupGer = obj.GetQueryTrasmEstVisibSup(currentTrasmissione, objTrasm, idRegistro, idNodoTitolario, idObj, idPeopleGroup, codiceACL_ToSet, tipoDiritto, ref ruoliSuperiori);
						if (listaSupGer != null && listaSupGer.Count > 0)
							foreach (DocsPaVO.trasmissione.infoSecurity info in listaSupGer)
								returnList.Add(info);
					}
				}
				#endregion
			}
			else
			{
				#region GESTIONE IN UNICA FASE: ESTENDE SUBITO LA VISIBILITà GERARCHICA (VECCHIA GESTIONE)

				// imposta il tipo diritto in base al parametro booleano "trasmessoConFasc"
				if (trasmessoConFasc)
					tipoDiritto = "F";
				else
					tipoDiritto = "T";

				returnList = obj.GetQueryTrasm(currentTrasmissione, objTrasm, idRegistro, idNodoTitolario, idObj, idPeopleGroup, codiceACL, tipoDiritto);

				if (currentTrasmissione.ragione.eredita.Equals("1") && !isPrivato)
				{
					ArrayList listaSupGer = new ArrayList();
					listaSupGer = obj.GetQueryTrasmEstVisibSup(currentTrasmissione, objTrasm, idRegistro, idNodoTitolario, idObj, idPeopleGroup, codiceACL, tipoDiritto, ref ruoliSuperiori);
					if (listaSupGer != null && listaSupGer.Count > 0)
						foreach (DocsPaVO.trasmissione.infoSecurity info in listaSupGer)
							returnList.Add(info);
				}
				#endregion
			}

			return returnList;

		}
		catch (Exception e)
		{
			#region Codice Commentato
			/*
			if(dbOpen)
			{
				db.closeConnection();
			}
			logger.Debug(e.Message);
			*/
			#endregion

			logger.Debug("Errore nella gestione delle trasmissioni (getQueryTrasm)", e);
			throw e;
		}
	}

	/// <summary>
	/// metodo che verifica se la query da eseguire in security è replicata o meno
	/// 0: indica che il record NON DEVE essere inserito
	/// 1: indica che il record DEVE essere inserito
	/// 2: indica che il record DEVE essere aggiornato
	/// </summary>
	/// <param name="listaExecuteQuery"></param>
	/// <param name="infoSec"></param>
	/// <returns></returns>
	private static int verificaDuplicazioneSecurity(ArrayList listaExecuteQuery, DocsPaVO.trasmissione.infoSecurity infoSec)
	{
		int intValue = 1;

		if (infoSec.tipoQuery.ToUpper().Equals("I"))
		{
			foreach (DocsPaVO.trasmissione.infoSecurity el in listaExecuteQuery)
			{
				// se coincidono thing e personorgroup devo verificare che coincida anche accessrigth e cha_tipo_diritto
				//
				if ((el.thing == infoSec.thing)
					&& (el.personOrGroup == infoSec.personOrGroup))
				{
					//se anche accessrigth e cha_tipo_diritto coincidono allora non devo aggiornare il record
					if ((el.accessRights == infoSec.accessRights)
						&& (el.chaTipoDiritto == infoSec.chaTipoDiritto))
					{
						//VUOL DIRE CHE IL RECORD CORRENTE GIa' è presente nell'arrayList, QUINDI la query non deve essere inserita nella lista
						intValue = 0;
						break;

					}
					else
					{
						//se accessrigth e cha_tipo_diritto non coincidono allora devo aggiornare il record qualora
						//l'accessrights corrente sia maggiore di quello già presente nella lista di query da eseguire
						// in tal caso all'uscita da questo metodo nella lista di query si dovrà inserire una update
						if (Int32.Parse(el.accessRights) < Int32.Parse(infoSec.accessRights) && Int32.Parse(el.accessRights) != 0)
						{
							intValue = 2;
							break;
						}
						//se accessright sono uguali ma cambia il tipo diritto non devo inserire alcun record
						if (Int32.Parse(el.accessRights) == Int32.Parse(infoSec.accessRights) && Int32.Parse(el.accessRights) != 0)
						{
							intValue = 0;
							break;
						}
					}
				}
			}
		}

		if (infoSec.tipoQuery.ToUpper().Equals("U"))
		{
			intValue = 2;
		}

		return intValue;
	}

	/// <summary>
	/// Verifica se il sistema è impostato ad avere la gestione della visibilità posticipata ai superiori gerarchici
	/// nelle trasmisissioni con workflow
	/// </summary>
	/// <returns></returns>
	public static bool IsEnabledVisibPosticipataInTrasmConWF()
	{
		return (!string.IsNullOrEmpty(DocsPaVO.Settings.AppSettings.Instance.VISIB_POST_TRASM_WF) && DocsPaVO.Settings.AppSettings.Instance.VISIB_POST_TRASM_WF.Equals("1"));
	}

	/// <summary>
	/// controlla se la trasm è stata eseguita.
	/// </summary>
	/// <param name="id"></param>
	/// <param name="tipoTrasm"></param>
	/// <returns></returns>
	public static bool checkExecTrasm(string id, string tipoTrasm)
	{
		bool rtn = false;
		DocsPaDB.Query_DocsPAWS.Trasmissione trasmissione = new DocsPaDB.Query_DocsPAWS.Trasmissione();

		string sql = "Select count(t.system_id) as cnt from dpa_trasmissione t,dpa_trasm_singola s,dpa_ragione_trasm r where  t.system_id=s.id_trasmissione and s.id_ragione=r.system_id   and  t.id_profile=" + id + " and upper(r.var_desc_ragione)='" + tipoTrasm.ToUpper() + "'";
		string cnt = string.Empty;

		using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
		{
			using (System.Data.IDataReader reader = dbProvider.ExecuteReader(sql))
			{
				while (reader.Read())
				{
					cnt = reader.GetValue(reader.GetOrdinal("cnt")).ToString();
				}
			}
		}
		if (!cnt.Equals(string.Empty) && cnt != "0")
		{
			rtn = true;
		}
		return rtn;
	}

	/// <summary>
	/// ACCETTAZIONE-RIFIUTO TRASMISSIONE
	/// </summary>
	/// <param name="objTrasmUtente"></param>
	/// <param name="idTrasmissione"></param>
	/// <returns></returns>
	public static bool executeAccRifMethod(DocsPaVO.trasmissione.TrasmissioneUtente objTrasmUtente, string idTrasmissione, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.InfoUtente infoUtente, IBaseRubricaComuneService rubricaComuneService, out string errore, out string mode, out string idObj, DocsPaVO.fascicolazione.Fascicolo fascicolo = null)
	{
		logger.Information("BEGIN");
		bool retValue = true;
		errore = string.Empty;
		mode = string.Empty;
		idObj = string.Empty;
		try
		{
			using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
			{
				string updateTrasmUtente = string.Empty;
				string modo = string.Empty;

				DocsPaVO.trasmissione.TrasmissioneSingola trasmsingola = getTrasmSingola(objTrasmUtente);
				if (!TrasmManager.checkTrasm_UNO_TUTTI_AccettataRifiutata(trasmsingola))
				{
					errore = "La Trasmissione risulta già ACCETTATA/RIFIUTATA";
					return false;
				}

				// verifica prima se la trasmissione è di documento e se questo non è stato messo nel frattempo in "Cestino"
				if (isTrasmDocInCestino(idTrasmissione))
				{
					errore = "Non è possibile accettare/rifiutare il Documento in quanto rimosso dal sistema.";
					return false;
				}

				DocsPaDB.Query_DocsPAWS.Trasmissione trasmissione = new DocsPaDB.Query_DocsPAWS.Trasmissione();

				// costruisce la query di update
				if (objTrasmUtente.tipoRisposta == DocsPaVO.trasmissione.TipoRisposta.ACCETTAZIONE)
				{
					if (trasmissione.UpdateTrasmUtenteAccettata(objTrasmUtente, idTrasmissione))
						modo = "ACCETTATA";
				}
				else
				{
					if (trasmissione.UpdateTrasmissioneUtenteRifiutata(objTrasmUtente, idTrasmissione))
						modo = "RIFIUTATA";
				}

				//updateTrasmUtente è la query che verrà eseguita per aggiornare il db correttamente
				//a seconda che siamo in accettazione o rifiuto
				//string delegato = "";
				//if (infoUtente.delegato != null)
				//    delegato = infoUtente.delegato.idPeople;
				updateTrasmUtente = trasmissione.GetUpdateTrasmissioneUtente(objTrasmUtente, modo, idTrasmissione, infoUtente);

				logger.Debug("executeAccRifMethod: " + updateTrasmUtente);
				DocsPaVO.trasmissione.Trasmissione trasm = new DocsPaVO.trasmissione.Trasmissione();
				if (updateTrasmUtente != null && updateTrasmUtente != string.Empty)
				{
					DocsPaDB.Query_DocsPAWS.Trasmissione obj = new DocsPaDB.Query_DocsPAWS.Trasmissione();
					trasm = BusinessLogic.Trasmissioni.TrasmManager.CreateObjTrasmissioneByIDLite(idTrasmissione, objTrasmUtente.systemId);
					if (!string.IsNullOrEmpty(modo))
						mode = modo + "_" + trasm.tipoOggetto;
					idObj = ((trasm.infoDocumento != null && !string.IsNullOrEmpty(trasm.infoDocumento.idProfile)) ? trasm.infoDocumento.idProfile : trasm.infoFascicolo.idFascicolo);
					obj.execACCRifMeth(updateTrasmUtente);
					obj.Dispose();

					// gestione della visibilità
					if (IsEnabledVisibPosticipataInTrasmConWF())
					{
						GestioneVisibilitaOggettoTrasmesso(modo, objTrasmUtente, idTrasmissione, ruolo, infoUtente);

						// Reperimento oggetto trasmissione
						//DocsPaVO.trasmissione.Trasmissione trasm = BusinessLogic.Trasmissioni.TrasmManager.CreateObjTrasmissioneByID(idTrasmissione);

						// Notifica evento di trasmissione accettata / rifiutata al sistema documentale sottostante
						DocsPaDocumentale.Interfaces.IAclEventListener eventListener = new DocsPaDocumentale.Documentale.AclEventListener(infoUtente);
						eventListener.TrasmissioneAccettataRifiutataEventHandler(trasm, ruolo, objTrasmUtente.tipoRisposta);

						//Richiamo il metodo per il calcolo della atipicità del documento / fascicolo
						DocsPaDB.Query_DocsPAWS.Documentale documentale = new DocsPaDB.Query_DocsPAWS.Documentale();
						infoUtente.idAmministrazione = ruolo.idAmministrazione;
						if (trasm.tipoOggetto == DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO)
							documentale.CalcolaAtipicita(infoUtente, trasm.infoDocumento.docNumber, DocsPaVO.Security.InfoAtipicita.TipoOggettoAtipico.DOCUMENTO);
						if (trasm.tipoOggetto == DocsPaVO.trasmissione.TipoOggetto.FASCICOLO)
						{
							documentale.CalcolaAtipicita(infoUtente, trasm.infoFascicolo.idFascicolo, DocsPaVO.Security.InfoAtipicita.TipoOggettoAtipico.FASCICOLO);
							List<SearchResultInfo> docs = BusinessLogic.Documenti.areaConservazioneManager.getListaDocumentiByIdProject(trasm.infoFascicolo.idFascicolo);
							if (docs != null)
							{
								docs.ForEach(
										f => documentale.CalcolaAtipicita(
											infoUtente,
											f.Id,
											DocsPaVO.Security.InfoAtipicita.TipoOggettoAtipico.DOCUMENTO));

							}

						}
					}

					//MEV FASCICOLAZIONE OBBLIGATORIA 23/01/2017
					if (trasm.tipoOggetto == DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO && objTrasmUtente.tipoRisposta == DocsPaVO.trasmissione.TipoRisposta.ACCETTAZIONE && fascicolo != null)
					{
						bool result = false;
						string msg = string.Empty;
						if (fascicolo != null && fascicolo.folderSelezionato != null)
						{
							result = BusinessLogic.Fascicoli.FolderManager.addDocFolder(infoUtente, trasm.infoDocumento.idProfile, fascicolo.folderSelezionato.systemID, false, out msg);
							if (result)
							{
								string domainObject = (fascicolo.folderSelezionato.idFascicolo.Equals(fascicolo.folderSelezionato.idParent) ? "fascicolo" : "sottofascicolo");
								string descr = string.Empty;
								if (fascicolo.folderSelezionato.descrizione.ToUpper().Equals("ROOT FOLDER") && !string.IsNullOrEmpty(fascicolo.descrizione))
									descr = fascicolo.descrizione;
								else
									descr = fascicolo.folderSelezionato.descrizione;
								BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "FOLDERADDDOC", fascicolo.folderSelezionato.idFascicolo, "Inserimento doc " + trasm.infoDocumento.idProfile + " in " + domainObject + ": " + descr, DocsPaVO.Logger.CodAzione.Esito.OK);
								BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCADDINFOLDER", trasm.infoDocumento.idProfile, "Inserimento doc " + trasm.infoDocumento.idProfile + " in " + domainObject + ": " + descr, DocsPaVO.Logger.CodAzione.Esito.OK);
							}
						}
						else
						{
							result = BusinessLogic.Fascicoli.FascicoloManager.addDocFascicolo(infoUtente, trasm.infoDocumento.idProfile, fascicolo.systemID, true, out msg);
							if (result)
							{
								if (fascicolo.tipo.Equals("P"))
								{
									BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "FASCICOLOADDDOC", fascicolo.systemID, "Inserimento doc " + trasm.infoDocumento.idProfile + " in fascicolo: " + fascicolo.codice, DocsPaVO.Logger.CodAzione.Esito.OK);
									BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCADDINFASC", trasm.infoDocumento.idProfile, "Inserimento doc " + trasm.infoDocumento.idProfile + " in fascicolo: " + fascicolo.codice, DocsPaVO.Logger.CodAzione.Esito.OK);
								}
								else
								{
									BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCADDINCLASS", trasm.infoDocumento.idProfile, "Inserimento doc " + trasm.infoDocumento.idProfile + " in classifica: " + fascicolo.codice, DocsPaVO.Logger.CodAzione.Esito.OK);

								}
							}
						}
					}

					//MEV LIBRO FIRMA 25/05/2015
					if (trasm.tipoOggetto == DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO)
					{
						DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
						if (objTrasmUtente.tipoRisposta == DocsPaVO.trasmissione.TipoRisposta.ACCETTAZIONE)
						{
							//Aggiorno la dta_accettazione in libro firma(qualora fosse presente)
							libroFirma.UpdateAcceptDateInLibroFirma(trasmsingola.systemId, infoUtente);
						}
						else if (objTrasmUtente.tipoRisposta == DocsPaVO.trasmissione.TipoRisposta.RIFIUTO)
						{
							//Se esistono elementi in libro firma legati alla trasmissione che si stà rifiutando, scrivo nel log l'evento di interruzione, cloncludo l'istanza del processo e rimuovo li elementi
							List<DocsPaVO.LibroFirma.ElementoInLibroFirma> elementi = libroFirma.GetElementiInLibroFirmaByIdTrasmSingola(trasmsingola.systemId, infoUtente);
							if (elementi != null && elementi.Count > 0)
							{
								string msg = string.Empty;
								string varMetodo = string.Empty;
								foreach (DocsPaVO.LibroFirma.ElementoInLibroFirma e in elementi)
								{
									BusinessLogic.LibroFirma.LibroFirmaManager.AggiornaDataEsecuzioneElemento(e.InfoDocumento.Docnumber, DocsPaVO.LibroFirma.TipoStatoElemento.NO_COMPETENZA.ToString());
									if (libroFirma.EliminaElementoInLibroFirma(e.IdIstanzaPasso))
									{
										string dateInterruption = DateTime.Now.ToString();
										string interrottoDa = "T"; //Titolare
										libroFirma.InterruptionSignatureProcess(e.IdIstanzaProcesso, DocsPaVO.LibroFirma.TipoStatoProcesso.STOPPED, e.InfoDocumento.Docnumber, objTrasmUtente.noteRifiuto, dateInterruption, interrottoDa, infoUtente);
										libroFirma.UpdateStatoIstanzaPasso(e.IdIstanzaPasso, e.InfoDocumento.VersionId, DocsPaVO.LibroFirma.TipoStatoPasso.STUCK.ToString(), infoUtente, dateInterruption);

										BusinessLogic.LibroFirma.LibroFirmaManager.SalvaStoricoIstanzaProcessoFirma(e.IdIstanzaProcesso, e.InfoDocumento.Docnumber, "Interruzione del processo di firma", infoUtente);
										DocsPaVO.LibroFirma.IstanzaProcessoDiFirma istanza = libroFirma.GetIstanzaProcessoDiFirmaByIdIstanzaProcessoLite(e.IdIstanzaProcesso, infoUtente);
										BusinessLogic.LibroFirma.LibroFirmaManager.ModificaStatoDocumentoPerInterruzione(istanza, infoUtente, rubricaComuneService);
										msg = "Interruzione del processo di firma per il file " + e.InfoDocumento.Docnumber;
										varMetodo = string.IsNullOrEmpty(e.InfoDocumento.IdDocumentoPrincipale) ? "INTERROTTO_PROCESSO_DOCUMENTO_DAL_TITOLARE" : "INTERROTTO_PROCESSO_ALLEGATO_DAL_TITOLARE";
										BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, ruolo.idGruppo, infoUtente.idAmministrazione, varMetodo, e.InfoDocumento.Docnumber, msg, DocsPaVO.Logger.CodAzione.Esito.OK, (infoUtente != null && infoUtente.delegato != null ? infoUtente.delegato : null), "1", null, dateInterruption);
									}


								}
							}
						}
					}

					//MEV TASK: se abilitata la funzionalità task e il tipo ragione prevede l'attivazione del task, all'accettazione creo un TASK
					if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "ENABLE_TASK")) &&
						  !DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "ENABLE_TASK").ToString().Equals("0"))
					{
						DocsPaDB.Query_DocsPAWS.Task t = new DocsPaDB.Query_DocsPAWS.Task();
						if (objTrasmUtente.tipoRisposta == DocsPaVO.trasmissione.TipoRisposta.ACCETTAZIONE && trasm != null
							&& t.IsRagioneDiTipoTask(trasmsingola.ragione.systemId))
						{

							DocsPaVO.Task.Task task = new DocsPaVO.Task.Task();
							task.RUOLO_DESTINATARIO = new DocsPaVO.utente.Ruolo() { idGruppo = ruolo.idGruppo };
							task.UTENTE_DESTINATARIO = new DocsPaVO.utente.Utente() { idPeople = infoUtente.idPeople };
							task.RUOLO_MITTENTE = new DocsPaVO.utente.Ruolo() { idGruppo = trasm.ruolo.idGruppo };
							task.UTENTE_MITTENTE = new DocsPaVO.utente.Utente() { idPeople = trasm.utente.idPeople };
							task.ID_TRASMISSIONE = idTrasmissione;
							if (trasm.infoDocumento != null && !string.IsNullOrEmpty(trasm.infoDocumento.idProfile))
								task.ID_PROFILE = trasm.infoDocumento.idProfile;
							else
								task.ID_PROJECT = trasm.infoFascicolo.idFascicolo;
							task.ID_TRASM_SINGOLA = trasmsingola.systemId;
							task.ID_RAGIONE_TRASM = trasmsingola.ragione.systemId;
							task.STATO_TASK = new DocsPaVO.Task.StatoTask() { DATA_SCADENZA = trasmsingola.dataScadenza, NOTE_RIAPERTURA = trasmsingola.noteSingole };

							BusinessLogic.Task.TaskManager.CreateTask(task, infoUtente);
						}
					}

					// MEV PORTALE PROCEDIMENTI MIBACT
					// Individuo eventuali ragioni che prevedano il cambio stato del fascicolo trasmesso
					if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_ENABLE_PORTALE_PROCEDIMENTI")) && !DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_ENABLE_PORTALE_PROCEDIMENTI").Equals("0"))
					{
						if (trasm.tipoOggetto == DocsPaVO.trasmissione.TipoOggetto.FASCICOLO && objTrasmUtente.tipoRisposta == DocsPaVO.trasmissione.TipoRisposta.ACCETTAZIONE)
						{
							BusinessLogic.Procedimenti.ProcedimentiManager.CambioStatoProcedimento(trasm.infoFascicolo.idFascicolo, "ACCETTAZIONE", trasmsingola.ragione.systemId, infoUtente);
						}
					}

				}

				trasmissione.Dispose();

				transactionContext.Complete();
			}

		}

		catch (Exception ex)
		{
			logger.Debug("Errore in executeAccRifMethod: ", ex);
			errore = "Errore generico durante l\\'operazione di accettazione/rifiuto";
			retValue = false;
		}
		logger.Information("END");
		return retValue;
	}

	public static DocsPaVO.trasmissione.TrasmissioneSingola getTrasmSingola(DocsPaVO.trasmissione.TrasmissioneUtente objTrasmUtente)
	{
		DocsPaVO.trasmissione.TrasmissioneSingola retValue = new DocsPaVO.trasmissione.TrasmissioneSingola();
		string cmdText = string.Empty;

		using (DocsPaDB.DBProvider db = new DocsPaDB.DBProvider())
		{
			cmdText = "select system_id,cha_tipo_dest,cha_tipo_trasm, id_ragione, dta_scadenza, var_note_sing from dpa_trasm_singola where system_id in ( " +
						"select id_trasm_singola from dpa_trasm_utente where system_id=" + objTrasmUtente.systemId + ")";
			DataSet ds = new DataSet();
			db.ExecuteQuery(ds, cmdText);

			if (ds.Tables[0].Rows.Count != 0)
			{
				for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
				{
					retValue.systemId = ds.Tables[0].Rows[i]["SYSTEM_ID"].ToString();
					retValue.ragione = new DocsPaVO.trasmissione.RagioneTrasmissione() { systemId = ds.Tables[0].Rows[i]["ID_RAGIONE"].ToString() };
					retValue.dataScadenza = !string.IsNullOrEmpty(ds.Tables[0].Rows[i]["DTA_SCADENZA"].ToString()) ? ds.Tables[0].Rows[i]["DTA_SCADENZA"].ToString() : string.Empty;
					retValue.noteSingole = ds.Tables[0].Rows[i]["VAR_NOTE_SING"].ToString();
					switch (ds.Tables[0].Rows[i]["CHA_TIPO_DEST"].ToString())
					{
						case "G":
							retValue.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.GRUPPO;
							break;

						case "R":
							retValue.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.RUOLO;
							break;

						case "U":
							retValue.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.UTENTE;
							break;
					}
					retValue.tipoTrasm = ds.Tables[0].Rows[i]["CHA_TIPO_TRASM"].ToString();
				}
			}
		}
		return retValue;
	}

	/// <summary>
	/// Verifica se è una trasmissione di un documento in cestino
	/// </summary>
	/// <param name="idTrasmissione">ID della trasmissione</param>
	/// <returns>True o False</returns>
	public static bool isTrasmDocInCestino(string idTrasmissione)
	{
		bool retValue = false;
		string cmdText = string.Empty;
		string idProfile = string.Empty;

		using (DocsPaDB.DBProvider db = new DocsPaDB.DBProvider())
		{
			cmdText = "select id_profile from dpa_trasmissione where system_id = " + idTrasmissione;
			db.ExecuteScalar(out idProfile, cmdText);
			if ((idProfile == null || idProfile.Equals("") || idProfile.Equals(string.Empty)) == false)
				retValue = BusinessLogic.Documenti.DocManager.checkdocInCestino(idProfile).Equals("1");
		}
		return retValue;
	}

	/// <summary>
	/// Gestione della visibilita dell'oggetto trasmesso a seguito di accettazione/rifiuto
	/// </summary>
	/// <param name="modo">possibili valori: ACCETTATA, RIFIUTATA</param>
	/// <param name="objTrasmUtente">oggetto trasmissione utente</param>
	/// <param name="idTrasmissione">system_id della trasmissione principale</param>
	/// <param name="ruolo"></param>
	/// <param name="infoUtente"></param>
	private static void GestioneVisibilitaOggettoTrasmesso(string modo, DocsPaVO.trasmissione.TrasmissioneUtente objTrasmUtente, string idTrasmissione, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.InfoUtente infoUtente)
	{
		/*
		 *                  READ ME!
		 *                  I casi da considerare per estendere (o non estendere) la visibilità gerarchica 
		 *                  sul documento/fascicolo oggetto della trasmissione corrente sono i seguenti:
		 *                  
		 *  
		 *                  destinatari che accettano/rifiutano = UNO: (dpa_trasm_singola.cha_tipo_trasm = 'S')
		 *                  ------------------------------------------------------------------------------------
		 *                  
		 *                          se accettata:       genera la visibilità gerarchica, 
		 *                                              si mantiene la visibilità sull'oggetto trasmesso al ruolo dell'utente che sta accettando
		 *                                              
		 *                          se rifiutata:       NON genera la visibilità gerarchica,
		 *                                              elimina la visibilità sull'oggetto trasmesso al ruolo dell'utente che sta rifiutando
		 *                                              
		 *                                              
		 *                  destinatari che accettano/rifiutano = TUTTI: (dpa_trasm_singola.cha_tipo_trasm = 'T')
		 *                  ------------------------------------------------------------------------------------
		 * 
		 *                          se accettata:       il primo utente che accetta fa 
		 *                                              generare la visibilità gerarchica e 
		 *                                              mantiene la visibilità sull'oggetto trasmesso al ruolo dell'utente che sta accettando
		 *                                              
		 *                                              se accettano TUTTI: è già accaduto il caso di cui sopra
		 * 
		 *
		 *                          se rifiuta:         se un utente rifiuta ma esistono altri utenti nel ruolo che non hanno ancora 
		 *                                              accettato/rifiutato, non succede nulla.
		 *                                              
		 *                                              se TUTTI gli utenti rifiutano allora
		 *                                              NON genera la visibilità gerarchica,
		 *                                              elimina la visibilità sull'oggetto trasmesso al ruolo dell'utente che sta rifiutando
		 */

		string quantiDestinatari = string.Empty;    // possibili valori: 'S' = a UNO, 'T' a TUTTI            
		string idPeopleOrGroup = string.Empty;      // utente o ruolo destinatario della trasmissione
		string accessRightsToSet = string.Empty;    // ACL da acquisire rispetto alla ragione di trasmissione
		DocsPaVO.trasmissione.TrasmissioneSingola trasmSingolaCorrente = new DocsPaVO.trasmissione.TrasmissioneSingola();
		DocsPaVO.HMDiritti.HMdiritti HMD = new DocsPaVO.HMDiritti.HMdiritti(); // codici ACL
		bool isFascicolo = false;

		try
		{
			// crea l'oggetto trasmissione tramite la system_id nota
			DocsPaVO.trasmissione.Trasmissione objTrasmissione;
			//objTrasmissione = TrasmManager.CreateObjTrasmissioneByID(idTrasmissione);
			objTrasmissione = TrasmManager.CreateObjTrasmissioneByIDLite(idTrasmissione, objTrasmUtente.systemId);

			// reperisce l'ID dell'Oggetto trasmesso
			string idObj = string.Empty; ;
			if (objTrasmissione.tipoOggetto == DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO)
			{
				if (objTrasmissione.infoDocumento != null)
					idObj = objTrasmissione.infoDocumento.idProfile;
			}
			if (objTrasmissione.tipoOggetto == DocsPaVO.trasmissione.TipoOggetto.FASCICOLO)
			{
				if (objTrasmissione.infoFascicolo != null)
				{
					idObj = objTrasmissione.infoFascicolo.idFascicolo;
					isFascicolo = true;
				}
			}
			// reperisce i seguenti dati:
			//      - quanti destinatari devono accettare/rifiutare la trasmissione (uno o tutti)            
			//      - l'ID del ruolo o utente
			foreach (DocsPaVO.trasmissione.TrasmissioneSingola ts in objTrasmissione.trasmissioniSingole)
			{
                if (ts?.trasmissioneUtente != null && ts.trasmissioneUtente.Count > 0)
                {
                    DocsPaVO.trasmissione.TrasmissioneUtente tu = ts.trasmissioneUtente[0] as DocsPaVO.trasmissione.TrasmissioneUtente;

                    trasmSingolaCorrente = ts;
                    quantiDestinatari = ts.tipoTrasm;
                    // reperimento dell'accessRights
                    switch (ts.ragione.tipoDiritti)
                    {
                        case DocsPaVO.trasmissione.TipoDiritto.READ:
                            accessRightsToSet = HMD.HMdiritti_Read.ToString();
                            break;
                        case DocsPaVO.trasmissione.TipoDiritto.WRITE:
                            accessRightsToSet = HMD.HMdiritti_Write.ToString();
                            break;
                        case DocsPaVO.trasmissione.TipoDiritto.WAITING:
                            accessRightsToSet = HMD.HDdiritti_Waiting.ToString();
                            break;
                    }

                }
			}

			switch (quantiDestinatari)
			{
				case "S":   // UNO

					switch (modo)
					{
						case "ACCETTATA":
							ImpostaVisibilitaInSecurity(objTrasmissione, true, new ArrayList());
							//ImpostaTipoDirittoInSecurity(idObj, ruolo.idGruppo, infoUtente.idPeople, "A", "T");
							//AggiornaAccessRights_Waiting(idObj, ruolo.idGruppo, infoUtente.idPeople, HMD.HDdiritti_Waiting.ToString(), HMD.HMdiritti_Read.ToString(), accessRightsToSet);
							ImpostaDirittoEAggiornaAccessrights(idObj, ruolo.idGruppo, infoUtente.idPeople, "A", "T", HMD.HDdiritti_Waiting.ToString(), HMD.HMdiritti_Read.ToString(), accessRightsToSet);
							if (isFascicolo)
								AggiornaAccessRights_DocsInFascicolo(idObj, ruolo.idGruppo, infoUtente.idPeople, HMD.HDdiritti_Waiting.ToString(), HMD.HMdiritti_Read.ToString(), accessRightsToSet);
							break;

						case "RIFIUTATA":
							EliminaVisibilitaRuolo_Utente(objTrasmissione, ruolo, infoUtente);
							break;
					}
					break;

				case "T":   // TUTTI

					switch (modo)
					{
						case "ACCETTATA":
							if (VerificaAccettazioneTrasm_aTutti(trasmSingolaCorrente))
							{
								ImpostaVisibilitaInSecurity(objTrasmissione, true, new ArrayList());
								//ImpostaTipoDirittoInSecurity(idObj, ruolo.idGruppo, infoUtente.idPeople, "A", "T");
								//AggiornaAccessRights_Waiting(idObj, ruolo.idGruppo, infoUtente.idPeople, HMD.HDdiritti_Waiting.ToString(), HMD.HMdiritti_Read.ToString(), accessRightsToSet);
								ImpostaDirittoEAggiornaAccessrights(idObj, ruolo.idGruppo, infoUtente.idPeople, "A", "T", HMD.HDdiritti_Waiting.ToString(), HMD.HMdiritti_Read.ToString(), accessRightsToSet);
								if (isFascicolo)
									AggiornaAccessRights_DocsInFascicolo(idObj, ruolo.idGruppo, infoUtente.idPeople, HMD.HDdiritti_Waiting.ToString(), HMD.HMdiritti_Read.ToString(), accessRightsToSet);
							}
							break;

						case "RIFIUTATA":
							if (VerificaRifiutoTrasm_aTutti(trasmSingolaCorrente))
								EliminaVisibilitaRuolo_Utente(objTrasmissione, ruolo, infoUtente);
							break;
					}
					break;
			}
		}
		catch (Exception ex)
		{
			throw new Exception(ex.Message);
		}
	}

	/// <summary>
	/// Update di TIPO DIRITTO e ACCESSRIGHTS nella tabella SECURITY
	/// </summary>
	/// <param name="thing">ID Oggetto da ricercare nella query</param>
	/// <param name="idGroup">ID Gruppo da ricercare nella query</param>
	/// <param name="idPeople">ID People da ricercare nella query</param>
	/// <param name="tipoDirittoAttuale">Diritto attuale da ricercare nella query</param>
	/// <param name="tipoDirittoToSet">Diritto da impostare</param>
	/// <param name="accessRightsAttuale">Accessrights da ricercare nella query</param>
	/// <param name="accessRightsLettura">Accessrights da ricercare nella query</param>
	/// <param name="accessRightsToSet">Accessrights da impostare</param>
	/// <returns></returns>
	public static bool ImpostaDirittoEAggiornaAccessrights(string thing, string idGroup, string idPeople, string tipoDirittoAttuale, string tipoDirittoToSet, string accessRightsAttuale, string accessRightsLettura, string accessRightsToSet)
	{
		bool retValue = false;
		DocsPaDB.Query_DocsPAWS.Trasmissione obj = new DocsPaDB.Query_DocsPAWS.Trasmissione();
		string condizione = "THING = " + thing + " AND PERSONORGROUP IN (" + idGroup + "," + idPeople + ") AND ACCESSRIGHTS IN (" + accessRightsAttuale + "," + accessRightsLettura + ")";
		// Prelevamento dello stato del documento
		DocsPaVO.DiagrammaStato.Stato stato = DiagrammiStato.DiagrammiStato.getStatoDoc(thing);

		// Se lo stato è stato reperito correttamente ed è in stato finale, i diritti vanno impostati a 45
		// altrimenti si considera come documento non nello stato finale
		if (stato != null &&
			stato.STATO_FINALE)
		{
			// un documento in stato finale non può essere riportato a accessrights = 63
			retValue = obj.impostaDirittoEAggiornaAccessrights(tipoDirittoToSet, condizione, "45");
		}
		else
			retValue = obj.impostaDirittoEAggiornaAccessrights(tipoDirittoToSet, condizione, accessRightsToSet);

		return retValue;
	}

	/// <summary>
	/// Update di ACCESSRIGHTS nella tabella SECURITY quando l'oggetto è di tipo FASCICOLO
	/// </summary>
	/// <param name="thing">ID Fascicolo (Tipo "F")</param>
	/// <param name="idGroup">ID Gruppo da ricercare nella query</param>
	/// <param name="idPeople">ID People da ricercare nella query</param>
	/// <param name="accessRightsAttuale">ACCESSRIGHTS attuale da ricercare nella query</param>
	/// <param name="accessRightsToSet">ACCESSRIGHTS da impostare</param>
	/// <returns></returns>
	public static bool AggiornaAccessRights_DocsInFascicolo(string thing, string idGroup, string idPeople, string accessRightsAttuale, string accessRightsLettura, string accessRightsToSet)
	{
		DocsPaDB.Query_DocsPAWS.Trasmissione obj = new DocsPaDB.Query_DocsPAWS.Trasmissione();
		return obj.aggiornaAccessRights_DocsInFascicolo(thing, idGroup, idPeople, accessRightsAttuale, accessRightsLettura, accessRightsToSet);
	}

	/// <summary>
	/// Elimina la visibilità dell'oggetto al ruolo
	/// </summary>
	/// <param name="objTrasmissione"></param>
	/// <param name="ruolo"></param>
	/// <param name="infoUtente"></param>
	/// <returns></returns>
	private static bool EliminaVisibilitaRuolo_Utente(DocsPaVO.trasmissione.Trasmissione objTrasmissione, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.InfoUtente infoUtente)
	{
		logger.Debug("Trasmissione RIFIUTATA: inizio procedura per eliminazione visibilità a ruolo/utente");

		bool retValue = true;
		string idOggettoTrasmesso = string.Empty;
		string tipoOggetto = string.Empty;
		bool eliminataVisibilita = false;
		bool isFascicolo = false;

		// reperisce l'ID dell'oggetto trasmesso
		if (objTrasmissione.tipoOggetto.Equals(DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO))
		{
			idOggettoTrasmesso = objTrasmissione.infoDocumento.idProfile;
			tipoOggetto = "D";
		}
		if (objTrasmissione.tipoOggetto.Equals(DocsPaVO.trasmissione.TipoOggetto.FASCICOLO))
		{
			idOggettoTrasmesso = objTrasmissione.infoFascicolo.idFascicolo;
			tipoOggetto = "F";
			isFascicolo = true;
		}

		DocsPaDB.Query_DocsPAWS.Trasmissione obj = new DocsPaDB.Query_DocsPAWS.Trasmissione();

		// verifica se esiste più di una trasmissione in TDL che devono essere accettate dall'utente
		int contaTrasmInTDL_UT = obj.ContaTrasm_UT_inTDL(tipoOggetto, idOggettoTrasmesso, infoUtente.idPeople);

		logger.Debug("Step 1 - Trasmissioni in TDL con oggetto ID [" + idOggettoTrasmesso + "] e utente [" + infoUtente.idPeople + "] : " + contaTrasmInTDL_UT.ToString());

		if (contaTrasmInTDL_UT.Equals(0))
		{
			// elimina in security il record di "In attesa accettazione" per l'utente, se esiste
			logger.Debug("Step 2 - Elimina in security il record di 'In attesa accettazione' per utente");
			obj.EliminaAttesaAccettazione(idOggettoTrasmesso, infoUtente.idPeople, isFascicolo);

			// verifica se esiste più di una trasmissione in TDL che devono essere accettate dal ruolo
			int contaTrasmInTDL_RUOLO = obj.ContaTrasm_RUOLO_inTDL(tipoOggetto, idOggettoTrasmesso, ruolo.idGruppo);

			logger.Debug("Step 3 - Trasmissioni in TDL con oggetto ID [" + idOggettoTrasmesso + "] e ruolo [" + ruolo.idGruppo + "] : " + contaTrasmInTDL_RUOLO.ToString());

			if (contaTrasmInTDL_RUOLO.Equals(0))
			{
				// verifica se esistono trasmissioni già viste o accettate su questo oggetto per questo utente/ruolo
				int contaTrasm = obj.ContaTrasm_Da_ACC_Per_OggettoRuoloUtente(tipoOggetto, idOggettoTrasmesso, ruolo.idGruppo, infoUtente.idPeople);

				logger.Debug("Step 4 - Trasmissioni da ACCETTARE su oggetto ID [" + idOggettoTrasmesso + "] con ruolo/utente [" + ruolo.idGruppo + "/" + infoUtente.idPeople + "] : " + contaTrasm.ToString());

				if (contaTrasm.Equals(0))
				{
					// se non esistono trasm.ni a ruolo in TDL, verifica in Security se l'oggetto non sia già visibile all'utente/ruolo perchè acquisito (accessrights = 'A' o 'F' o 'T')
					logger.Debug("Step 5 - Verifica attuale visibilità su oggetto con ID [" + idOggettoTrasmesso + "] del ruolo/utente [" + ruolo.idGruppo + "/" + infoUtente.idPeople + "]");
					if (obj.verificaSecurityPerEliminaVisibRuolo_Utente(idOggettoTrasmesso, ruolo.idGruppo, infoUtente.idPeople))
					{
						// elimina in security il record di "In attesa accettazione" per il ruolo, se esiste
						logger.Debug("Step 6 - Elimina in security il record di 'In attesa accettazione' per il ruolo");
						obj.EliminaAttesaAccettazione(idOggettoTrasmesso, ruolo.idGruppo, isFascicolo);

						// elimina in security la visibilità ruolo
						logger.Debug("Step 7 - Elimina la visibilità ruolo");
						eliminataVisibilita = obj.EliminaVisibilitaRuolo_Utente(ruolo.idGruppo, idOggettoTrasmesso);
						if (eliminataVisibilita)
						{
							// pulisce l'ADL
							logger.Debug("Step 8 - Pulisce ADL");
							DocsPaDB.Query_DocsPAWS.Amministrazione objAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
							DocsPaDB.Query_DocsPAWS.Utenti objUtenti = new DocsPaDB.Query_DocsPAWS.Utenti();
							objAmm.AmmEliminaDocADLUtente(infoUtente.idPeople, objUtenti.GetFromCorrGlobGeneric("SYSTEM_ID", "ID_GRUPPO = " + ruolo.idGruppo), idOggettoTrasmesso, isFascicolo);
						}
					}
				}
				else
				{
					// imposta tipo diritto della security da "A" a "T"
					logger.Debug("Step 5 - Altre trasmissioni da ACCETTARE: imposta nuovo tipo diritto da A a T");
					ImpostaTipoDirittoInSecurity(idOggettoTrasmesso, ruolo.idGruppo, infoUtente.idPeople, "A", "T");
				}
			}
		}

		logger.Debug("Trasmissione RIFIUTATA: fine procedura: visibilita eliminata: " + (eliminataVisibilita ? "SI" : "NO"));

		return retValue;
	}

	/// <summary>
	/// Verifica se è il primo utente che accetta la trasmissione:
	/// in questo caso il metodo restituisce un valore True e si procederà ad impostare la visibilità gerarchica sull'oggetto,
	/// altrimenti non succedera nulla.
	/// </summary>
	/// <param name="trasmSingolaCorrente">Trasm singola corrente</param>
	/// <returns>true: è il primo utente che accetta; false: altrimenti</returns>
	private static bool VerificaAccettazioneTrasm_aTutti(DocsPaVO.trasmissione.TrasmissioneSingola trasmSingolaCorrente)
	{
		bool retValue = false;
		int conta = 0;

		// solo il primo che accetta permette al sistema di impostare la visibilità gerarchica
		foreach (DocsPaVO.trasmissione.TrasmissioneUtente tu in trasmSingolaCorrente.trasmissioneUtente)
			if (!tu.dataAccettata.Equals(""))
				conta++;

		if (conta == 1)
			retValue = true;

		return retValue;
	}

	/// <summary>
	/// Verifica se le trasmissioni utenti sono state tutte rifiutate:
	/// in questo caso il metodo restituisce un valore True e si procederà all'eliminazione della visibilità sull'oggetto,
	/// altrimenti non succedera nulla.
	/// </summary>
	/// <param name="trasmSingolaCorrente">Trasm singola corrente</param>
	/// <returns>true: hanno rifiutato tutti; false: esiste ancora qualche utente che deve rifiutare/accettare la trasm.ne</returns>
	private static bool VerificaRifiutoTrasm_aTutti(DocsPaVO.trasmissione.TrasmissioneSingola trasmSingolaCorrente)
	{
		bool retValue = true;

		// se esiste anche solo una trasmissione utente non ancora rifiutata, non deve succedere nulla (ritorna False)          
		foreach (DocsPaVO.trasmissione.TrasmissioneUtente tu in trasmSingolaCorrente.trasmissioneUtente)
		{
			if (tu.dataRifiutata.Equals(""))
			{
				retValue = false;
				break;
			}
		}

		return retValue;
	}

	/// <summary>
	/// Update di TIPO DIRITTO nella tabella SECURITY
	/// </summary>
	/// <param name="thing">ID Oggetto da ricercare nella query</param>
	/// <param name="idGroup">ID Gruppo da ricercare nella query</param>
	/// <param name="idPeople">ID People da ricercare nella query</param>
	/// <param name="tipoDirittoAttuale">Diritto attuale da ricercare nella query</param>
	/// <param name="tipoDirittoToSet">Diritto da impostare</param>
	/// <returns></returns>
	public static bool ImpostaTipoDirittoInSecurity(string thing, string idGroup, string idPeople, string tipoDirittoAttuale, string tipoDirittoToSet)
	{
		DocsPaDB.Query_DocsPAWS.Trasmissione obj = new DocsPaDB.Query_DocsPAWS.Trasmissione();
		string condizione = "THING = " + thing + " AND PERSONORGROUP IN (" + idGroup + "," + idPeople + ") AND CHA_TIPO_DIRITTO = '" + tipoDirittoAttuale + "'";
		return obj.impostaTipoDirittoInSecurity(tipoDirittoToSet, condizione);
	}

        /// <summary>
        /// Estrae il Documento Principale
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="schedaDoc"></param>
        /// <param name="path"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        private static bool estrazioneDocPrincipale(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento schedaDoc, string path, out Dictionary<string, string> CoppiaNomeFileENomeOriginale)
        {
            System.IO.FileStream fs = null;
            System.IO.FileStream fsAll = null;
            CoppiaNomeFileENomeOriginale = new Dictionary<string, string>();
            byte[] content = null;
            DocsPaDocumentale.Documentale.DocumentManager documentManager = new DocsPaDocumentale.Documentale.DocumentManager(infoUtente);
            string NomeOriginale = string.Empty;
            try
            {
                System.Int64 fileSize = System.Int64.Parse(((DocsPaVO.documento.Documento)schedaDoc.documenti[0]).fileSize);
                if (fileSize > 0)
                {
                    //estrazione documento principale
                    DocsPaVO.documento.Documento doc = (DocsPaVO.documento.Documento)schedaDoc.documenti[0];

                    //modifica bug estensione file
                    char[] dot = { '.' };
                    string[] parts = doc.fileName.Split(dot);
                    string suffix = parts[parts.Length - 1];
                    string docPrincipaleName = "Documento_principale." + suffix;
                    //fine modifica

                    //string docPrincipaleName="Documento_principale."+doc.fileName.Substring(doc.fileName.IndexOf(".")+1);
                    fs = new System.IO.FileStream(path + "\\" + docPrincipaleName, System.IO.FileMode.Create);
                    string library = DocsPaDB.Utils.Personalization.getInstance(infoUtente.idAmministrazione).getLibrary();

                    //byte[] content=DocsPaWS.Interoperabilita.InteroperabilitaInvioSegnatura.getDocument(infoUtente,doc.docNumber,doc.version,doc.versionId,doc.versionLabel,logger);
                    content = documentManager.GetFile(doc.docNumber, doc.version, doc.versionId, doc.versionLabel);

                    if (content == null)
                    {
                        logger.Debug("Errore nella gestione delle trasmissioni (estrazioneDocPrincipale| principale)");
                        throw new Exception();
                    }

                    NomeOriginale = BusinessLogic.Documenti.FileManager.getOriginalFileName(infoUtente, doc);

                    if (String.IsNullOrEmpty(NomeOriginale))
                        NomeOriginale = docPrincipaleName;

                    CoppiaNomeFileENomeOriginale.Add(String.Format(path + "\\" + docPrincipaleName).ToLowerInvariant(), NomeOriginale);

                    fs.Write(content, 0, content.Length);
                    fs.Close();
                    fs = null;
                }
                DocsPaDB.Query_DocsPAWS.Documenti docWs = new DocsPaDB.Query_DocsPAWS.Documenti();

                // estrazione degli allegati
                foreach (DocsPaVO.documento.Allegato allegato in schedaDoc.allegati)
                {
                    fileSize = System.Int64.Parse(allegato.fileSize);
                    if (fileSize > 0 && !(allegato.descrizione != null && allegato.descrizione.Equals("E-Mail Ricevuta") && allegato.fileName != null && allegato.fileName.ToUpper().EndsWith(".EML")))
                    {
                        // come da segnalazione del Consiglio Provinciale 198405, escludo dalla lista degli allegati quelli IS e PEC
                        // Utilizzo CHA_ALLEGATI_ESTERNO in VERSIONS
                        string tipoAllegato = docWs.GetTipologiaAllegato(allegato.versionId);
                        //if (!docWs.GeIsAllegatoIS(allegato.versionId).Equals("1") && !docWs.GeIsAllegatoPEC(allegato.versionId).Equals("1"))
                        if (tipoAllegato != "P"
                            && tipoAllegato != "I"
                            && tipoAllegato != "D"
                            )
                        {
                            fs = new System.IO.FileStream(path + @"\" + Path.GetFileName(allegato.fileName), System.IO.FileMode.Create);

                            content = documentManager.GetFile(allegato.docNumber, allegato.version, allegato.versionId, allegato.versionLabel);

                            if (content == null)
                            {
                                logger.Debug("Errore nella gestione delle trasmissioni (estrazioneDocPrincipale| allegati)");
                                throw new Exception();
                            }

                            NomeOriginale = BusinessLogic.Documenti.FileManager.getOriginalFileName(infoUtente, allegato);

                            if (String.IsNullOrEmpty(NomeOriginale))
                                NomeOriginale = Path.GetFileName(allegato.fileName);

                            CoppiaNomeFileENomeOriginale.Add(String.Format(path + @"\" + Path.GetFileName(allegato.fileName)).ToLowerInvariant(), NomeOriginale);

                            fs.Write(content, 0, content.Length);
                            fs.Close();
                            fs = null;
                        }
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                //logger.addMessage("Estrazione del file non eseguita.Eccezione: "+e.ToString());
                logger.Debug("Estrazione del file non eseguita.Eccezione: " + e.ToString());
                if (fs != null) fs.Close();
                if (fsAll != null) fsAll.Close();
                return false;
            }
        }
    }
