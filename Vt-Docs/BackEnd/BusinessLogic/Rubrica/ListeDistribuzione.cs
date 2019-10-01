using System;
using System.Collections;
using System.Data;
using log4net;

namespace BusinessLogic.Rubrica
{
    public class ListeDistribuzione
	{
        private static ILog logger = LogManager.GetLogger(typeof(ListeDistribuzione));

		public ListeDistribuzione(){}

		public static bool isUniqueCodLista (string codLista,string idAmm) 
		{
			try 
			{
				DocsPaDB.Query_DocsPAWS.ListeDistr listeDistr = new DocsPaDB.Query_DocsPAWS.ListeDistr();
				return listeDistr.isUniqueCodLista(codLista,idAmm);
			}
			catch(Exception e) 
			{
				logger.Debug("Errore in Rubrica-ListeDistribuzione  - metodo: isUniqueCodLista",e);
				return false;
			}						
		}

        public static bool isUniqueCod(string codLista, string idAmm)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.ListeDistr listeDistr = new DocsPaDB.Query_DocsPAWS.ListeDistr();
                return listeDistr.isUniqueCod(codLista, idAmm);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in Rubrica-ListeDistribuzione  - metodo: isUniqueCodLista", e);
                return false;
            }
        }

		public static bool isUniqueNomeLista (string nomeLista, string idAmm) 
		{
			try 
			{
				DocsPaDB.Query_DocsPAWS.ListeDistr listeDistr = new DocsPaDB.Query_DocsPAWS.ListeDistr();
				return listeDistr.isUniqueNomeLista(nomeLista, idAmm);
			}
			catch(Exception e) 
			{
				logger.Debug("Errore in Rubrica-ListeDistribuzione  - metodo: isUniqueNomeLista",e);
				return false;
			}						
		}

		public static string getCodiceLista (string idLista) 
		{
			try 
			{
				DocsPaDB.Query_DocsPAWS.ListeDistr listeDistr = new DocsPaDB.Query_DocsPAWS.ListeDistr();
				return listeDistr.getCodiceLista(idLista);
			}
			catch(Exception e) 
			{
				logger.Debug("Errore in Rubrica-ListeDistribuzione  - metodo: getCorrispondentiByCodLista",e);
				return null;
			}						
		}

        public static string getRuoloOrUserLista(string idLista)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.ListeDistr listeDistr = new DocsPaDB.Query_DocsPAWS.ListeDistr();
                return listeDistr.getRuoloOrUserLista(idLista);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in Rubrica-ListeDistribuzione  - metodo: getRuoloOrUserLista", e);
                return null;
            }
        }

        public static ArrayList getCorrispondentiByCodLista(string codiceLista, string idAmm, DocsPaVO.utente.InfoUtente infoUtente) 
		{
			ArrayList corr = new ArrayList();
            ArrayList retVal = new ArrayList();
			try 
			{
				DocsPaDB.Query_DocsPAWS.ListeDistr listeDistr = new DocsPaDB.Query_DocsPAWS.ListeDistr();
				corr = listeDistr.getCorrispondentiByCodLista(codiceLista,idAmm);

                
                foreach (DocsPaVO.utente.Corrispondente c in corr)
                    if (c.inRubricaComune)
                        retVal.Add(BusinessLogic.RubricaComune.RubricaServices.UpdateCorrispondente(infoUtente, c));
                    else
                        retVal.Add(c);

			}
			catch(Exception e) 
			{
				logger.Debug("Errore in Rubrica-ListeDistribuzione  - metodo: getCorrispondentiByCodLista",e);
				return null;
			}
            return retVal;			
		}

        public static ArrayList getCorrispondentiByCodListaByUtente(string codiceLista, string idAmm, DocsPaVO.utente.InfoUtente infoUtente)
        {
            ArrayList corr = new ArrayList();
            ArrayList retVal = new ArrayList();
            try
            {
                DocsPaDB.Query_DocsPAWS.ListeDistr listeDistr = new DocsPaDB.Query_DocsPAWS.ListeDistr();
                corr = listeDistr.getCorrispondentiByCodListaByUtente(codiceLista, idAmm, infoUtente);


                foreach (DocsPaVO.utente.Corrispondente c in corr)
                    if (c.inRubricaComune)
                        retVal.Add(BusinessLogic.RubricaComune.RubricaServices.UpdateCorrispondente(infoUtente, c));
                    else
                        retVal.Add(c);

            }
            catch (Exception e)
            {
                logger.Debug("Errore in Rubrica-ListeDistribuzione  - metodo: getCorrispondentiByCodListaByUtente", e);
                return null;
            }
            return retVal;
        }

        public static ArrayList getCorrispondentiByDescLista(string descLista)
        {
            ArrayList corr = new ArrayList();
            try
            {
                DocsPaDB.Query_DocsPAWS.ListeDistr listeDistr = new DocsPaDB.Query_DocsPAWS.ListeDistr();
                corr = listeDistr.getCorrispondentiByDescLista(descLista);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in Rubrica-ListeDistribuzione  - metodo: getCorrispondentiByDescLista", e);
                return null;
            }
            return corr;
        }

		public static string getNomeLista(string codiceLista, string idAmm)
		{
			try 
			{
				DocsPaDB.Query_DocsPAWS.ListeDistr listeDistr = new DocsPaDB.Query_DocsPAWS.ListeDistr();
				return listeDistr.getNomeLista(codiceLista,idAmm);

			}
			catch(Exception e) 
			{
				logger.Debug("Errore in Rubrica-ListeDistribuzione  - metodo: getNomeLista",e);
				return null;
			}			
		}

        public static string getSystemIdLista(string codiceLista)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.ListeDistr listeDistr = new DocsPaDB.Query_DocsPAWS.ListeDistr();
                return listeDistr.getSystemIdLista(codiceLista);

            }
            catch (Exception e)
            {
                logger.Debug("Errore in Rubrica-ListeDistribuzione  - metodo: getSystemIdLista", e);
                return null;
            }
        }

		public static DataSet getListePerModificaUt(string idUtente, string idRuolo)
		{
			try 
			{
				DocsPaDB.Query_DocsPAWS.ListeDistr listeDistr = new DocsPaDB.Query_DocsPAWS.ListeDistr();
				return listeDistr.getListePerModificaUt(idUtente);

			}
			catch(Exception e) 
			{
				logger.Debug("Errore in Rubrica-ListeDistribuzione  - metodo: getListePerModificaUt",e);
				return null;
			}					
		}

        public static DataSet getListePerRuoloUt(string idUtente, string idRuolo)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.ListeDistr listeDistr = new DocsPaDB.Query_DocsPAWS.ListeDistr();
                return listeDistr.getListePerRuoloUt(idUtente, idRuolo);

            }
            catch (Exception e)
            {
                logger.Debug("Errore in Rubrica-ListeDistribuzione  - metodo: getListePerRuoloUt", e);
                return null;
            }
        }

		public static DataSet getListe(string idUtente, string idAmm)
		{
			try 
			{
				DocsPaDB.Query_DocsPAWS.ListeDistr listeDistr = new DocsPaDB.Query_DocsPAWS.ListeDistr();
				return listeDistr.getListe(idUtente,idAmm);

			}
			catch(Exception e) 
			{
				logger.Debug("Errore in Rubrica-ListeDistribuzione  - metodo: getListe",e);
				return null;
			}					
		}

        public static DataSet isCorrInListaDistr(string idCorr)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.ListeDistr listeDistr = new DocsPaDB.Query_DocsPAWS.ListeDistr();
                return listeDistr.isCorrInListaDistr(idCorr);

            }
            catch (Exception e)
            {
                logger.Debug("Errore in Rubrica-ListeDistribuzione  - metodo: isCorrInListaDistr", e);
                return null;
            }
        }

        public static DataSet getListe(string idUtente, string idAmm, string descrizione)
		{
			try 
			{
				DocsPaDB.Query_DocsPAWS.ListeDistr listeDistr = new DocsPaDB.Query_DocsPAWS.ListeDistr();
				return listeDistr.getListe(idUtente,idAmm,descrizione);

			}
			catch(Exception e) 
			{
				logger.Debug("Errore in Rubrica-ListeDistribuzione  - metodo: getListe",e);
				return null;
			}					
		}

		public static DataSet getListe(string idAmm)
		{
			try 
			{
				DocsPaDB.Query_DocsPAWS.ListeDistr listeDistr = new DocsPaDB.Query_DocsPAWS.ListeDistr();
				return listeDistr.getListe(idAmm);

			}
			catch(Exception e) 
			{
				logger.Debug("Errore in Rubrica-ListeDistribuzione  - metodo: getListe",e);
				return null;
			}					
		}

		public static void deleteLista(string codiceLista)
		{
			try 
			{
				DocsPaDB.Query_DocsPAWS.ListeDistr listeDistr = new DocsPaDB.Query_DocsPAWS.ListeDistr();
				listeDistr.deleteLista(codiceLista);

			}
			catch(Exception e) 
			{
				logger.Debug("Errore in Rubrica-ListeDistribuzione  - metodo: deleteLista",e);				
			}					
		}

		public static DataSet getCorrispondentiLista(string codiceLista)
		{
			try 
			{
				DocsPaDB.Query_DocsPAWS.ListeDistr listeDistr = new DocsPaDB.Query_DocsPAWS.ListeDistr();
				return listeDistr.getCorrispondentiLista(codiceLista);

			}
			catch(Exception e) 
			{
				logger.Debug("Errore in Rubrica-ListeDistribuzione  - metodo: getCorrispondentiLista",e);
				return null;
			}					
		}

		public static void deleteCorrLista(string codiceCorr)
		{
			try 
			{
				DocsPaDB.Query_DocsPAWS.ListeDistr listeDistr = new DocsPaDB.Query_DocsPAWS.ListeDistr();
				listeDistr.deleteCorrLista(codiceCorr);

			}
			catch(Exception e) 
			{
				logger.Debug("Errore in Rubrica-ListeDistribuzione  - metodo: deleteCorrLista",e);				
			}							
		}

		public static DataSet ricercaCorrLista(string p_ricercaDescrizione)
		{
			try 
			{
				DocsPaDB.Query_DocsPAWS.ListeDistr listeDistr = new DocsPaDB.Query_DocsPAWS.ListeDistr();
				return listeDistr.ricercaCorrLista(p_ricercaDescrizione);

			}
			catch(Exception e) 
			{
				logger.Debug("Errore in Rubrica-ListeDistribuzione  - metodo: ricercaCorrLista",e);
				return null;
			}					
		}

		public static void salvaLista(DataSet dsCorrLista, string nomeLista, string codiceLista, string idUtente, string idAmm)
		{
			try 
			{
				DocsPaDB.Query_DocsPAWS.ListeDistr listeDistr = new DocsPaDB.Query_DocsPAWS.ListeDistr();
				listeDistr.salvaLista(dsCorrLista,nomeLista,codiceLista,idUtente,idAmm);

			}
			catch(Exception e) 
			{
				logger.Debug("Errore in Rubrica-ListeDistribuzione  - metodo: salvaLista",e);				
			}						
		}

        public static int salvaListaGruppo(DataSet dsCorrLista, string nomeLista, string codiceLista, string idUtente, string idAmm, string gruppo)
        {
            int result = 0;
            try
            {
                DocsPaDB.Query_DocsPAWS.ListeDistr listeDistr = new DocsPaDB.Query_DocsPAWS.ListeDistr();
                result = listeDistr.salvaListaGruppo(dsCorrLista, nomeLista, codiceLista, idUtente, idAmm, gruppo);

            }
            catch (Exception e)
            {
                logger.Debug("Errore in Rubrica-ListeDistribuzione  - metodo: salvaLista", e);
            }
            return result;
        }

		public static void modificaLista(DataSet dsCorrLista, string idLista, string nomeLista, string codiceLista)
		{
			try 
			{
				DocsPaDB.Query_DocsPAWS.ListeDistr listeDistr = new DocsPaDB.Query_DocsPAWS.ListeDistr();
				listeDistr.modificaLista(dsCorrLista,idLista,nomeLista,codiceLista);

			}
			catch(Exception e) 
			{
				logger.Debug("Errore in Rubrica-ListeDistribuzione  - metodo: modificaLista",e);				
			}					
		}

        public static void modificaListaUser(DataSet dsCorrLista, string idLista, string nomeLista, string codiceLista, string idUtente)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.ListeDistr listeDistr = new DocsPaDB.Query_DocsPAWS.ListeDistr();
                listeDistr.modificaListaUser(dsCorrLista, idLista, nomeLista, codiceLista, idUtente);

            }
            catch (Exception e)
            {
                logger.Debug("Errore in Rubrica-ListeDistribuzione  - metodo: modificaListaUser", e);
            }
        }

        public static void modificaListaGruppo(DataSet dsCorrLista, string idLista, string nomeLista, string codiceLista, string idGruppo)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.ListeDistr listeDistr = new DocsPaDB.Query_DocsPAWS.ListeDistr();
                listeDistr.modificaListaGruppo(dsCorrLista, idLista, nomeLista, codiceLista, idGruppo);

            }
            catch (Exception e)
            {
                logger.Debug("Errore in Rubrica-ListeDistribuzione  - metodo: modificaListaGruppo", e);
            }
        }

		public static void modificaListaCorr(DataSet dsCorrLista, string idLista)
		{
			try 
			{
				DocsPaDB.Query_DocsPAWS.ListeDistr listeDistr = new DocsPaDB.Query_DocsPAWS.ListeDistr();
				listeDistr.modificaListaCorr(dsCorrLista,idLista);

			}
			catch(Exception e) 
			{
				logger.Debug("Errore in Rubrica-ListeDistribuzione  - metodo: modificaLista",e);				
			}					
		}

	}
}
