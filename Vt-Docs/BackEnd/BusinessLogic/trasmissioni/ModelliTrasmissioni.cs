using System;
using System.Data;
using log4net;
using System.Collections.Generic;

namespace BusinessLogic.Trasmissioni
{
	
	public class ModelliTrasmissioni
	{
        private static ILog logger = LogManager.GetLogger(typeof(ModelliTrasmissioni));

		public ModelliTrasmissioni()
		{
			
		}

		public static System.Collections.ArrayList getModelliPerTrasm(string idAmm,DocsPaVO.utente.Registro[] registri,string idPeople,string idCorrGlobali,string idTipoDoc,string idDiagramma,string idStato,string cha_tipo_oggetto, bool AllReg)
		{
			try 
			{
				DocsPaDB.Query_DocsPAWS.ModTrasmissioni modTrasm = new DocsPaDB.Query_DocsPAWS.ModTrasmissioni();
                return modTrasm.getModelliPerTrasm(idAmm, registri, idPeople, idCorrGlobali, idTipoDoc, idDiagramma, idStato, cha_tipo_oggetto, AllReg);
			}
			catch(Exception e) 
			{
				logger.Debug("Errore in Trasmissioni - ModelliTrasmissioni - metodo: getModelliPerTrasm",e);				
				return null;
			}	
		}


        public static System.Collections.ArrayList getModelliPerTrasm(string idAmm, DocsPaVO.utente.Registro[] registri, string idPeople, string idCorrGlobali, string idTipoDoc, string idDiagramma, string idStato, string cha_tipo_oggetto, string system_id, string idRuoloUtente, bool AllReg, string accessrights)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.ModTrasmissioni modTrasm = new DocsPaDB.Query_DocsPAWS.ModTrasmissioni();
                return modTrasm.getModelliPerTrasm(idAmm, registri, idPeople, idCorrGlobali, idTipoDoc, idDiagramma, idStato, cha_tipo_oggetto, system_id, idRuoloUtente, AllReg, accessrights);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in Trasmissioni - ModelliTrasmissioni - metodo: getModelliPerTrasm", e);
                return null;
            }
        }

		public static string salvaModello(DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modelloTrasmissione)
		{
            string result = string.Empty;
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();
                    DocsPaDB.Query_DocsPAWS.ModTrasmissioni modTrasm = new DocsPaDB.Query_DocsPAWS.ModTrasmissioni();
                    result = modTrasm.salvaModello(modelloTrasmissione);
                    transactionContext.Complete();
                    //if (dbProvider.DBType.ToUpper().Equals("SQL"))
                    //{
                    //    // metodo per il reperimento del system id del modello appena inserito in sql
                    //    int sysId = modTrasm.findModelSystemId();
                    //    if (sysId > 0)
                    //    {
                    //        modelloTrasmissione.SYSTEM_ID = sysId;
                    //        modelloTrasmissione.CODICE = "MT_" + sysId.ToString();
                    //        modTrasm.insertSalvaModello(modelloTrasmissione);
                    //    }
                    //    else
                    //        modTrasm.deleteModello(modelloTrasmissione);
                    //}
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in Trasmissioni - ModelliTrasmissioni - metodo: salvaModello", e);				
                }
                return result;
            }						
		}

        public static bool isUniqueCodModelloTrasm(DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modello)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.ModTrasmissioni modelloTrasm = new DocsPaDB.Query_DocsPAWS.ModTrasmissioni();
                return modelloTrasm.isUniqueCodModelloTrasm(modello);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in ModelliTrasmissioni.cs  - metodo: isUniqueCodModelloTrasm", e);
                return false;
            }
        }

		public static System.Collections.ArrayList getModelliByAmm(string idAmm)
		{
			try
			{
				DocsPaDB.Query_DocsPAWS.ModTrasmissioni listModTrasm = new DocsPaDB.Query_DocsPAWS.ModTrasmissioni();
				return listModTrasm.getModelliByAmm(idAmm);

			}
			catch(Exception ex)
			{
				logger.Debug("Errore in Trasmissioni - ModelliTrasmissioni - metodo: getModelliByAmm",ex);				
				return null;
			}
		}

        public static System.Collections.ArrayList getModelliByAmmLite(string idAmm)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.ModTrasmissioni listModTrasm = new DocsPaDB.Query_DocsPAWS.ModTrasmissioni();
                return listModTrasm.getModelliByAmmLite(idAmm);

            }
            catch (Exception ex)
            {
                logger.Debug("Errore in Trasmissioni - ModelliTrasmissioni - metodo: getModelliByAmm", ex);
                return null;
            }
        }

        public static System.Collections.ArrayList getModelliByAmmPaging(string idAmm, int nPagina, string ricerca, string codice, out int numTotPag)
		{
			numTotPag = 0;
			try
			{
				DocsPaDB.Query_DocsPAWS.ModTrasmissioni listModTrasm = new DocsPaDB.Query_DocsPAWS.ModTrasmissioni();
                return listModTrasm.getModelliByAmmPaging(idAmm, nPagina, ricerca, codice, out numTotPag);
			}
			catch(Exception ex)
			{
				logger.Debug("Errore in Trasmissioni - ModelliTrasmissioni - metodo: getModelliByAmm",ex);				
				return null;
			}
		}

        public static System.Collections.ArrayList getModelliByDdlAmmPaging(string idAmm, int nPagina, DocsPaVO.filtri.FiltroRicerca[] filtriRicerca, out int numTotPag)
        {
            numTotPag = 0;
            try
            {
                DocsPaDB.Query_DocsPAWS.ModTrasmissioni listModTrasm = new DocsPaDB.Query_DocsPAWS.ModTrasmissioni();
                return listModTrasm.getModelliByDdlAmmPaging(idAmm, nPagina, filtriRicerca, out numTotPag);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in Trasmissioni - ModelliTrasmissioni - metodo: getModelliByDdlAmm", ex);
                return null;
            }
        }

        public static string getModelloSystemId()
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.ModTrasmissioni listModTrasm = new DocsPaDB.Query_DocsPAWS.ModTrasmissioni();
                return listModTrasm.getModelloSystemId();

            }
            catch (Exception ex)
            {
                logger.Debug("Errore in Trasmissioni - ModelliTrasmissioni - metodo: getModelloSystemId", ex);
                return null;
            }
        }

		public static System.Collections.ArrayList getModelliUtente(DocsPaVO.utente.Utente utente, DocsPaVO.utente.InfoUtente infoUt, DocsPaVO.filtri.FiltroRicerca[] filtriRicerca)
		{
			try
			{
				DocsPaDB.Query_DocsPAWS.ModTrasmissioni listModTrasm = new DocsPaDB.Query_DocsPAWS.ModTrasmissioni();
		        return listModTrasm.getModelliUtente(utente, infoUt, filtriRicerca);

			}
			catch(Exception ex)
			{
				logger.Debug("Errore in Trasmissioni - ModelliTrasmissioni - metodo: getModelliUtente",ex);				
				return null;
			}
		}

		public static DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione getModelloByID(string idAmm,string idModello)
		{
			try
			{
				DocsPaDB.Query_DocsPAWS.ModTrasmissioni listModTrasm = new DocsPaDB.Query_DocsPAWS.ModTrasmissioni();
				return listModTrasm.getModelloByID(idAmm,idModello);

			}
			catch(Exception ex)
			{
				logger.Debug("Errore in Trasmissioni - ModelliTrasmissioni - metodo: getModelloByID",ex);				
				return null;
			}
		}

        public static System.Collections.ArrayList getModelliByName(string idAmm, string nome, string tipoOggetto, string idRegistro, string idUtenteMittente, string idRuoloMittente)
        {
            DocsPaDB.Query_DocsPAWS.ModTrasmissioni objQuery = new DocsPaDB.Query_DocsPAWS.ModTrasmissioni();
            return objQuery.getModelliByName(idAmm,nome,tipoOggetto,idRegistro,idUtenteMittente,idRuoloMittente);
        }

		public static void CancellaModello(string idAmm, string idModello)
		{
			try 
			{
				DocsPaDB.Query_DocsPAWS.ModTrasmissioni delModTrasm = new DocsPaDB.Query_DocsPAWS.ModTrasmissioni();
				delModTrasm.CancellaModello(idAmm,idModello);
			}
			catch(Exception ex)
			{
				logger.Debug("Errore in Trasmissioni - ModelliTrasmissioni - metodo: CancellaModello",ex);	
			}
		
		}

		public static void CancellaDestModello(string idRagione,string varCodRubrica,string idModello)
		{
			try
			{
				DocsPaDB.Query_DocsPAWS.ModTrasmissioni delModTrasm = new DocsPaDB.Query_DocsPAWS.ModTrasmissioni();
				delModTrasm.CancellaDestModello(idRagione,varCodRubrica,idModello);
			}
			catch(Exception ex)
			{
				logger.Debug("Errore in Trasmissioni - ModelliTrasmissioni - metodo: CancellaDestModello",ex);	
			}
		
		}

        /// <summary>
        /// GIUGNO 2008 - Adamo
        /// MODELLI DI TRASMISSIONE:
        /// Gestione della notifica trasmissione degli utenti inserito nei modelli di trasmissione
        /// </summary>
        /// <param name="objTrasm">Oggetto Modelli_Trasmissioni.ModelloTrasmissione</param>
        /// <param name="operazione">Tipo di operazione da effettuare sul db:   'GET' = reperimento dati,   'SET' = modifica dei dati </param>
        /// <returns>L'oggetto stesso passato come parametro. Oggetto NULL se ci sono state eccezioni nel metodo</returns>
        public static DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione UtentiConNotificaTrasm(DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione objModTrasm, System.Collections.ArrayList utentiDaInserire, System.Collections.ArrayList utentiDaCancellare, string operazione)
        {
            DocsPaDB.Query_DocsPAWS.ModTrasmissioni objQuery = new DocsPaDB.Query_DocsPAWS.ModTrasmissioni();
            return objQuery.UtentiConNotificaTrasm(objModTrasm, utentiDaInserire, utentiDaCancellare, operazione);
        }

        /// <summary>
        /// GIUGNO 2008 - Adamo
        /// MODELLI DI TRASMISSIONE: 
        /// Salva i dati di cessione dei diritti su modello di trasmissione
        /// </summary>
        /// <param name="objTrasm">Oggetto Modelli_Trasmissioni.ModelloTrasmissione</param>        
        /// <returns>True se esito positivo</returns>
        public static bool SalvaCessioneDirittiSuModelliTrasm(DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione objTrasm)
        {
            DocsPaDB.Query_DocsPAWS.ModTrasmissioni objQuery = new DocsPaDB.Query_DocsPAWS.ModTrasmissioni();
            return objQuery.SalvaCessioneDirittiSuModelliTrasm(objTrasm);
        }

        public static System.Collections.ArrayList getModelliByAmmConRicerca(string idAmm, string codiceRicerca, string tipoRicerca)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.ModTrasmissioni listModTrasm = new DocsPaDB.Query_DocsPAWS.ModTrasmissioni();
                return listModTrasm.getModelliByAmmConRicerca(idAmm, codiceRicerca, tipoRicerca);

            }
            catch (Exception ex)
            {
                logger.Debug("Errore in Trasmissioni - ModelliTrasmissioni - metodo: getModelliByAmmConRicerca", ex);
                return null;
            }
        }

        public static System.Collections.ArrayList getModelliAssDiagrammi(string idTipo, string idDiagramma, string stato, string idAmm, bool selezionati, string tipo)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.ModTrasmissioni listModTrasm = new DocsPaDB.Query_DocsPAWS.ModTrasmissioni();
                return listModTrasm.getModelliAssDiagramma(idTipo, idDiagramma, stato, idAmm, selezionati, tipo);

            }
            catch (Exception ex)
            {
                logger.Debug("Errore in Trasmissioni - ModelliTrasmissioni - metodo: getModelliAssDiagrammi", ex);
                return null;
            }
        }

        public static System.Collections.ArrayList getModelliPerTrasmLite(string idAmm, DocsPaVO.utente.Registro[] registri, string idPeople, string idCorrGlobali, string idTipoDoc, string idDiagramma, string idStato, string cha_tipo_oggetto, string system_id, string idRuoloUtente, bool AllReg, string accessrights)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.ModTrasmissioni modTrasm = new DocsPaDB.Query_DocsPAWS.ModTrasmissioni();
                return modTrasm.getModelliPerTrasmLite(idAmm, registri, idPeople, idCorrGlobali, idTipoDoc, string.Empty, idDiagramma, idStato, cha_tipo_oggetto, system_id, idRuoloUtente, AllReg, accessrights);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in Trasmissioni - ModelliTrasmissioni - metodo: getModelliPerTrasmLite", e);
                return null;
            }
        }

        public static System.Collections.ArrayList getModelliPerTrasmLiteFasc(string idAmm, DocsPaVO.utente.Registro[] registri, string idPeople, string idCorrGlobali, string idTipoFasc, string idDiagramma, string idStato, string cha_tipo_oggetto, string system_id, string idRuoloUtente, bool AllReg, string accessrights)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.ModTrasmissioni modTrasm = new DocsPaDB.Query_DocsPAWS.ModTrasmissioni();
                return modTrasm.getModelliPerTrasmLite(idAmm, registri, idPeople, idCorrGlobali, string.Empty, idTipoFasc, idDiagramma, idStato, cha_tipo_oggetto, system_id, idRuoloUtente, AllReg, accessrights);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in Trasmissioni - ModelliTrasmissioni - metodo: getModelliPerTrasmLiteFasc", e);
                return null;
            }
        }

        /// <summary>
        /// Funzione per la ricerca e la sostituzione dei ruoli nei modelli di trasmissione
        /// </summary>
        /// <param name="request">Informazioni sull'azione da compiere</param>
        /// <returns>Risultato dell'operazione</returns>
        public static DocsPaVO.Modelli_Trasmissioni.FindAndReplaceResponse FindAndReplaceRoleInModelliTrasmissione(DocsPaVO.Modelli_Trasmissioni.FindAndReplaceRequest request)
        {
            DocsPaVO.Modelli_Trasmissioni.FindAndReplaceResponse retVal = new DocsPaVO.Modelli_Trasmissioni.FindAndReplaceResponse();
            try
            {
                using (DocsPaDB.Query_DocsPAWS.ModTrasmissioni modTrasm = new DocsPaDB.Query_DocsPAWS.ModTrasmissioni())
                {
                    retVal = modTrasm.FindAndReplaceRuoliInModelliTrasmissione(request);
                }

                return retVal;

            }
            catch (Exception e)
            {
                logger.Debug("Errore durante l'esecuzione dell'operazione di ricerca e sostituzione ruoli nei modelli di trasmissione.");
                throw new ApplicationException("Errore durante l'esecuzione dell'operazione di ricerca e sostituzione ruoli nei modelli di trasmissione.");
            }
 
        }

        public static DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione getModelloByIDSoloConNotifica(string idAmm, string idModello)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.ModTrasmissioni listModTrasm = new DocsPaDB.Query_DocsPAWS.ModTrasmissioni();
                return listModTrasm.getModelloByIDSoloConNotifica(idAmm, idModello);

            }
            catch (Exception ex)
            {
                logger.Debug("Errore in Trasmissioni - ModelliTrasmissioni - metodo: getModelloByID", ex);
                return null;
            }
        }


    }
}
