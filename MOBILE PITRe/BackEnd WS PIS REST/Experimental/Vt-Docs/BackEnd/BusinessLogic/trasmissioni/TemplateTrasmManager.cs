using System;
using System.Collections;
using System.Data;
using log4net;

namespace BusinessLogic.Trasmissioni
{
	/// <summary>
	/// </summary>
	public class TemplateTrasmManager
	{
        private static ILog logger = LogManager.GetLogger(typeof(TemplateTrasmManager));
		//creazione template
		/// <summary>
		/// 
		/// </summary>
		/// <param name="template"></param>
		/// <returns></returns>
		public static DocsPaVO.trasmissione.TemplateTrasmissione addTemplate(DocsPaVO.trasmissione.TemplateTrasmissione template)
		{
			#region Codice Commentato
			/*logger.Debug("inserimentoTemplate");
			DocsPa_V15_Utils.Database db = DocsPaWS.Utils.dbControl.getDatabase();
			db.openConnection();	
			
			string numPar = checkTemplate(template, db);
			if (!numPar.Equals("0"))
				return null;
			try 
			{
				string insertString = 
					"INSERT INTO DPA_TEMPL_TRASM " +
					"(" + DocsPaWS.Utils.dbControl.getSystemIdColName() + " ID_TRASMISSIONE, VAR_TEMPLATE ) " +
					" VALUES (" + DocsPaWS.Utils.dbControl.getSystemIdNextVal("DPA_TEMPL_TRASM") +
					template.idTrasmissione + ", '" + template.descrizione.Replace("'", "''") + "')";
				logger.Debug(insertString);
				template.systemId = db.insertLocked(insertString, "DPA_TEMPL_TRASM");
							
				db.closeConnection();

			} 
			catch (Exception e) 
			{
				logger.Debug (e.Message);				
				db.closeConnection();
				throw new Exception("F_System");
			}
			return template;*/
			#endregion

			DocsPaDB.Query_DocsPAWS.Trasmissione obj = new DocsPaDB.Query_DocsPAWS.Trasmissione();
			return obj.addTemplate(template);
		}
		
		#region Metodo Commentato
		/*protected static string checkTemplate(DocsPaVO.trasmissione.TemplateTrasmissione template, DocsPa_V15_Utils.Database db) 
		{
			//si verifica se la parola chiave è già presente
			string selectString =
				"SELECT COUNT(*) FROM DPA_TEMPL_TRASM WHERE upper(VAR_TEMPLATE)='"+ template.descrizione.ToUpper() +"'" + 
				" AND ID_TRASMISSIONE = " + template.idTrasmissione;
					
			logger.Debug(selectString);
			string numPar= db.executeScalar(selectString).ToString();
			return numPar;
		}*/
		#endregion
		
		// ricerca template delle trasmissioni
		/// <summary>
		/// 
		/// </summary>
		/// <param name="idPeople"></param>
		/// <param name="idRuoloInUO"></param>
		/// <param name="tipo"></param>
		/// <returns></returns>
		public static ArrayList getListaTemplate(string idPeople, string idRuoloInUO, string tipo)
		{
			ArrayList listaTemplate = new ArrayList();

			try 
			{ 
				string queryString = getQueryTemplate(idPeople, idRuoloInUO, tipo, null );
				logger.Debug(queryString);

				DocsPaDB.Query_DocsPAWS.Trasmissione strDB = new DocsPaDB.Query_DocsPAWS.Trasmissione();
				strDB.getListTempl(queryString,listaTemplate);
			} 
			catch (Exception e) 
			{
				logger.Debug (e.Message);

				logger.Debug("Errore nella gestione delle trasmissioni (getListaTemplate)",e);
				throw new Exception(e.Message);
			}
			return listaTemplate;
		}


		public static ArrayList getTemplateDaNome(string idPeople, string idRuoloInUO, string tipo, string nomeTemplate)
		{
			ArrayList listaTemplate = new ArrayList();
			try 
			{ 
				string queryString = getQueryTemplate(idPeople, idRuoloInUO, tipo,nomeTemplate );
				logger.Debug(queryString);

				DocsPaDB.Query_DocsPAWS.Trasmissione strDB = new DocsPaDB.Query_DocsPAWS.Trasmissione();
				strDB.getListTempl(queryString,listaTemplate);
			} 
			catch (Exception e) 
			{
				logger.Debug (e.Message);
				//db.closeConnection();			
				logger.Debug("Errore nella gestione delle trasmissioni (getListaTemplate)",e);
				throw new Exception(e.Message);
			}
			
			return listaTemplate;
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="idPeople"></param>
		/// <param name="idRuoloInUO"></param>
		/// <param name="tipo"></param>
		/// <returns></returns>
		private static string getQueryTemplate(string idPeople, string idRuoloInUO, string tipo, string nomeTemplate) 
		{
			#region Codice Commentato
			/*string queryString =
				"SELECT A.SYSTEM_ID, A.ID_TRASMISSIONE, A.VAR_TEMPLATE FROM DPA_TEMPL_TRASM A, DPA_TRASMISSIONE B " +
				"WHERE A.ID_TRASMISSIONE = B.SYSTEM_ID " +
				"AND B.ID_PEOPLE = " + idPeople + " AND B.ID_RUOLO_IN_UO = " + idRuoloInUO ; 
			//per ora non gestiamo doc e fascicoli !!
			if (tipo != null && !tipo.Equals(""))
				queryString += " AND B.CHA_TIPO_DP ='" + tipo +"'";
			queryString += 	" ORDER BY A.VAR_TEMPLATE";
			return queryString;*/
			#endregion 

			DocsPaDB.Query_DocsPAWS.Trasmissione obj = new DocsPaDB.Query_DocsPAWS.Trasmissione();
			return obj.getQueryTemplate(idPeople, idRuoloInUO, tipo, nomeTemplate);
		}

		#region Metodo Commentato
		/*
		private static DocsPaVO.trasmissione.TemplateTrasmissione getDatiTemplate(IDataReader dr) 
		{
			DocsPaVO.trasmissione.TemplateTrasmissione template = new DocsPaVO.trasmissione.TemplateTrasmissione();		
			template.systemId = dr.GetValue(0).ToString();
			template.idTrasmissione = dr.GetValue(1).ToString();
			template.descrizione = dr.GetValue(2).ToString();
			return template;
		} 
		*/
		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="template"></param>
		public static void deleteTemplate(DocsPaVO.trasmissione.TemplateTrasmissione template)
		{
			#region Codice Commentato
			/*logger.Debug("cancellaTemplate");
			DocsPa_V15_Utils.Database db=DocsPaWS.Utils.dbControl.getDatabase();
			bool dbOpen=false;
			try
			{
				db.openConnection();
				dbOpen=true;
				//costruzione della query
				string deleteString="DELETE FROM DPA_TEMPL_TRASM WHERE SYSTEM_ID="+template.systemId;
				db.executeNonQuery(deleteString);
				db.closeConnection();
			}
			catch(Exception e)
			{
				if(dbOpen)
				{
					db.closeConnection();
				}
				throw e;
			}*/
			#endregion

			DocsPaDB.Query_DocsPAWS.Trasmissione obj = new DocsPaDB.Query_DocsPAWS.Trasmissione();
			if (!obj.DeleteTemplate(template))
			{
				logger.Debug("Errore nella gestione delle trasmissioni (deleteTemplate)");
				throw new Exception();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="template"></param>
		public static void updateTemplate(DocsPaVO.trasmissione.TemplateTrasmissione template)
		{
			#region Codice Commentato
			/*logger.Debug("update Template");
			DocsPa_V15_Utils.Database db=DocsPaWS.Utils.dbControl.getDatabase();
			DataSet dataSet= new DataSet();
			try
			{
				db.openConnection();
				string updateString="UPDATE DPA_TEMPL_TRASM SET " +
					" VAR_TEMPLATE ='" + template.descrizione + "' " +
					" WHERE System_ID = " + template.systemId;
				logger.Debug(updateString);
				db.executeNonQuery(updateString);

				db.closeConnection();
			}
			catch(Exception e)
			{
				logger.Debug(e.Message);
				db.rollbackTransaction();
				db.closeConnection();	
				throw e;
			}*/
			#endregion

			DocsPaDB.Query_DocsPAWS.Trasmissione obj = new DocsPaDB.Query_DocsPAWS.Trasmissione();
			if (!obj.UpdateTemplate(template))
			{
				logger.Debug("Errore nella gestione delle trasmissioni (updateTemplate)");
				throw new Exception();
			}
			//return ;
		}

	
		#region   creazione trasmissione da template 13/09/04

		public static DocsPaVO.trasmissione.Trasmissione creaTrasmDaTemplate(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.trasmissione.TemplateTrasmissione template, DocsPaVO.utente.InfoUtente infoUtente,DocsPaVO.utente.Ruolo ruolo)
		{
			DocsPaVO.documento.InfoDocumento infoDocumento = Documenti.DocManager.getInfoDocumento(schedaDocumento);
			DocsPaVO.utente.Utente utente = Utenti.UserManager.getUtente(infoUtente.idPeople);
			return addTrasmDaTemplate(infoDocumento, template, utente, ruolo);
		}
	
		public static DocsPaVO.trasmissione.Trasmissione addTrasmDaTemplate(DocsPaVO.documento.InfoDocumento infoDocumento, DocsPaVO.trasmissione.TemplateTrasmissione template, DocsPaVO.utente.Utente utente, DocsPaVO.utente.Ruolo ruolo)
		{
			//DocsPaVO.utente.Utente utente = utenti.getUtente(infoUtente.idPeople);
			//DocsPaVO.utente.Ruolo ruolo = Utenti.UserManager.getRuolo(infoUtente.idCorrGlobali);
//			DocsPaVO.utente.InfoUtente infoUtente = null;
			if (template == null)
				return null;
			
			DocsPaVO.trasmissione.Trasmissione trasmTemplate = cercaTrasmissioneTemplate(template, infoDocumento, utente, ruolo);
			if (trasmTemplate == null)
				return null;
			DocsPaVO.trasmissione.Trasmissione trasmissione = new DocsPaVO.trasmissione.Trasmissione(); 
			//Ricostruisci la nuova trasmissione, con le trasmissioni utenti aggiornate
			trasmissione.utente = utente;
			trasmissione.ruolo = ruolo;
			trasmissione.tipoOggetto = trasmTemplate.tipoOggetto;
			trasmissione.noteGenerali = template.descrizione; // CONTROLLARE DOVE PRENDERE L'INFORMAZIONE
			trasmissione.infoDocumento = infoDocumento;

			if (trasmTemplate.trasmissioniSingole != null && trasmTemplate.trasmissioniSingole.Count > 0)
				for (int i = 0; i < trasmTemplate.trasmissioniSingole.Count; i++)
				{
					trasmissione = addTrasmissioneSingola(trasmissione, ((DocsPaVO.trasmissione.TrasmissioneSingola)trasmTemplate.trasmissioniSingole[i]),utente.idAmministrazione);
				}
            if (trasmissione != null && trasmissione.trasmissioniSingole != null && trasmissione.trasmissioniSingole.Count > 0)
            {
               
                trasmissione = TrasmManager.saveTrasmMethod(trasmissione);
            }
            else
            {
                logger.Debug("Errore nella creazione dell'oggetto trasmissione da template o trasmissioni singole non presenti");
                return null;
            }
			return trasmissione;

		}


		public static DocsPaVO.trasmissione.Trasmissione addTrasmFascicoloDaTemplate(DocsPaVO.fascicolazione.InfoFascicolo infoFascicolo, DocsPaVO.trasmissione.TemplateTrasmissione template, DocsPaVO.utente.Utente utente, DocsPaVO.utente.Ruolo ruolo)
		{
			//DocsPaVO.utente.Utente utente = utenti.getUtente(infoUtente.idPeople);
			//DocsPaVO.utente.Ruolo ruolo = Utenti.UserManager.getRuolo(infoUtente.idCorrGlobali);
			//			DocsPaVO.utente.InfoUtente infoUtente = null;
			if (template == null)
				return null;
			
			DocsPaVO.trasmissione.Trasmissione trasmTemplate = cercaTrasmissioneFascicoloTemplate(template, infoFascicolo, utente, ruolo);
			if (trasmTemplate == null)
				return null;
			DocsPaVO.trasmissione.Trasmissione trasmissione = new DocsPaVO.trasmissione.Trasmissione(); 
			//Ricostruisci la nuova trasmissione, con le trasmissioni utenti aggiornate
			trasmissione.utente = utente;
			trasmissione.ruolo = ruolo;
			trasmissione.tipoOggetto = trasmTemplate.tipoOggetto;
			trasmissione.noteGenerali = template.descrizione; // CONTROLLARE DOVE PRENDERE L'INFORMAZIONE
			trasmissione.infoFascicolo=infoFascicolo;

			if (trasmTemplate.trasmissioniSingole != null && trasmTemplate.trasmissioniSingole.Count > 0)
				for (int i = 0; i < trasmTemplate.trasmissioniSingole.Count; i++)
				{
					trasmissione = addTrasmissioneSingola(trasmissione, ((DocsPaVO.trasmissione.TrasmissioneSingola)trasmTemplate.trasmissioniSingole[i]),utente.idAmministrazione);
				}
            if (trasmissione != null && trasmissione.trasmissioniSingole != null && trasmissione.trasmissioniSingole.Count > 0)
            {
                
                trasmissione = TrasmManager.saveTrasmMethod(trasmissione);
            }
            else
            {
                logger.Debug("Errore nella creazione dell'oggetto trasmissione da template o trasmissioni singole non presenti");
                return null;
            }
			return trasmissione;

		}


		private static DocsPaVO.trasmissione.Trasmissione cercaTrasmissioneTemplate(DocsPaVO.trasmissione.TemplateTrasmissione template, DocsPaVO.documento.InfoDocumento infoDocumento, DocsPaVO.utente.Utente utente, DocsPaVO.utente.Ruolo ruolo)
		{
			DocsPaVO.trasmissione.OggettoTrasm oggTrasm = new DocsPaVO.trasmissione.OggettoTrasm();
			oggTrasm.infoDocumento  = infoDocumento;
			return Trasmissioni.QueryTrasmManager.getTrasmissione(template.idTrasmissione, "E", false, null, utente, ruolo);
		}


		private static DocsPaVO.trasmissione.Trasmissione cercaTrasmissioneFascicoloTemplate(DocsPaVO.trasmissione.TemplateTrasmissione template, DocsPaVO.fascicolazione.InfoFascicolo infoFascicolo, DocsPaVO.utente.Utente utente, DocsPaVO.utente.Ruolo ruolo)
		{
			DocsPaVO.trasmissione.OggettoTrasm oggTrasm = new DocsPaVO.trasmissione.OggettoTrasm();
			oggTrasm.infoFascicolo  = infoFascicolo;

			return Trasmissioni.QueryTrasmManager.getTrasmissione(template.idTrasmissione, "E", false, null, utente, ruolo);
		}

		private static DocsPaVO.trasmissione.Trasmissione addTrasmissioneSingola(DocsPaVO.trasmissione.Trasmissione trasmissione, DocsPaVO.trasmissione.TrasmissioneSingola trasmSingolaTemplate, string idAmministrazione) 
		{
			DocsPaVO.utente.Corrispondente corr = trasmSingolaTemplate.corrispondenteInterno;
			if(!Utenti.addressBookManager.isCorrispondenteValido(corr.systemId))
			{
				logger.Debug("Corrispondente: id:" + corr.systemId + " - desc:" + corr.descrizione + " non valido");
				return trasmissione;
			}
			DocsPaVO.trasmissione.RagioneTrasmissione ragione = trasmSingolaTemplate.ragione;
			// Aggiungo la trasmissione singola
			DocsPaVO.trasmissione.TrasmissioneSingola trasmissioneSingola = new DocsPaVO.trasmissione.TrasmissioneSingola();
			trasmissioneSingola.tipoTrasm = "S";
			trasmissioneSingola.corrispondenteInterno = corr;
			trasmissioneSingola.ragione = ragione;
			
			// Aggiungo la lista di trasmissioniUtente
			if( corr.GetType() == typeof(DocsPaVO.utente.Ruolo)) 
			{
				trasmissioneSingola.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.RUOLO;
				ArrayList listaUtenti = queryUtenti(corr.codiceRubrica, idAmministrazione);
				//ciclo per utenti se dest è gruppo o ruolo
				if (listaUtenti == null || listaUtenti.Count==0)
					return trasmissione;
				for(int i= 0; i < listaUtenti.Count; i++) 
				{
					DocsPaVO.trasmissione.TrasmissioneUtente trasmissioneUtente = new DocsPaVO.trasmissione.TrasmissioneUtente();
					trasmissioneUtente.utente = (DocsPaVO.utente.Utente) listaUtenti[i];
					if(ragione.descrizione.Equals("RISPOSTA"))
						trasmissioneUtente.idTrasmRispSing=trasmissioneSingola.systemId;
					trasmissioneSingola.trasmissioneUtente.Add(trasmissioneUtente);
				}
			}
			else 
			{
				trasmissioneSingola.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.UTENTE;
				DocsPaVO.trasmissione.TrasmissioneUtente trasmissioneUtente = new DocsPaVO.trasmissione.TrasmissioneUtente();
				trasmissioneUtente.utente = (DocsPaVO.utente.Utente) corr;
				trasmissioneSingola.trasmissioneUtente.Add(trasmissioneUtente);
			}
			trasmissione.trasmissioniSingole.Add(trasmissioneSingola);
			return trasmissione;

		}

		private static ArrayList queryUtenti(string codiceRubrica, string idAmministrazione) 
		{
			
			//costruzione oggetto queryCorrispondente
			DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();

			qco.codiceRubrica = codiceRubrica;
			qco.getChildren = true;
			qco.fineValidita = true;
			
			qco.idAmministrazione= idAmministrazione;
			
			//corrispondenti interni
			qco.tipoUtente=DocsPaVO.addressbook.TipoUtente.INTERNO;			
			
			return Utenti.addressBookManager.getListaCorrispondenti(qco);
		}



		#endregion
	
	
	}
}
