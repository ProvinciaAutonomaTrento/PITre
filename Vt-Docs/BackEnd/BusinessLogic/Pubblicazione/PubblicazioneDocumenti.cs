using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.Pubblicazione
{

    /// <summary>
    /// 
    /// </summary>
    public sealed class PubblicazioneDocumenti
    {
        /// <summary>
        /// 
        /// </summary>
        private PubblicazioneDocumenti()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginInfo"></param>
        /// <param name="filtro"></param>
        /// <returns></returns>
        public static string MaxDataPubblicazioneDocumento( DocsPaVO.Pubblicazione.FiltroPubblicazioneDocumenti filtro)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.PubblicazioneDocumenti pubblicazioneDocDb = new DocsPaDB.Query_DocsPAWS.PubblicazioneDocumenti();

                return pubblicazioneDocDb.MaxDataPubblicazioneDocumento(filtro);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginInfo"></param>
        /// <param name="idProfile"></param>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        public static DocsPaVO.documento.SchedaDocumento GetDocumento(DocsPaVO.Pubblicazione.DocumentoDaPubblicare documento)
        {
            try
            {
                DocsPaVO.utente.InfoUtente author = Utils.ImpersonateAuthor(documento);

                return BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(author, documento.DocNumber);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public static DocsPaVO.ProfilazioneDinamica.Templates getTemplatePerRicerca(DocsPaVO.Pubblicazione.LoginInfo loginInfo, string tipoAtto)
        //{
        //    DocsPaVO.utente.InfoUtente infoUtente = Utils.Login(loginInfo);

        //    try
        //    {
        //        return BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplatePerRicerca(infoUtente.idAmministrazione, tipoAtto);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        Utils.Logoff(infoUtente);
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        public static DocsPaVO.documento.FileDocumento GetFileDocumento(DocsPaVO.Pubblicazione.DocumentoDaPubblicare documento, DocsPaVO.documento.FileRequest fileRequest)
        {
            DocsPaVO.utente.InfoUtente author = Utils.ImpersonateAuthor(documento);
            
            return BusinessLogic.Documenti.FileManager.getFile(fileRequest, author);
        }


        //public static string getCodiceCorrispondente(DocsPaVO.Pubblicazione.LoginInfo loginInfo,string docnumber, string idOggettoCustom,string idTemplate)
        //{
        //    DocsPaVO.utente.InfoUtente infoUtente = Utils.Login(loginInfo);

        //    try
        //    {
        //        return BusinessLogic.Utenti.UserManager.getCodiceCorrispondente(docnumber, idOggettoCustom, idTemplate);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        Utils.Logoff(infoUtente);
        //    }
        //}

        //public static DocsPaVO.utente.Corrispondente getCorrispondenteBySystemID(DocsPaVO.Pubblicazione.LoginInfo loginInfo, string system_id)
        //{
        //    DocsPaVO.utente.InfoUtente infoUtente = Utils.Login(loginInfo);

        //    try
        //    {
        //        return BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(system_id);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        Utils.Logoff(infoUtente);
        //    }
        //}

        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        public static DocsPaVO.Pubblicazione.DocumentoDaPubblicare[] RicercaDocumentiDaPubblicare(DocsPaVO.Pubblicazione.FiltroDocumentiDaPubblicare filtro)
        {
            DocsPaDB.Query_DocsPAWS.PubblicazioneDocumenti pubblicazioneDocDb = new DocsPaDB.Query_DocsPAWS.PubblicazioneDocumenti();

            return pubblicazioneDocDb.RicercaDocumentiDaPubblicare(filtro);
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="loginInfo"></param>
        ///// <param name="filters"></param>
        ///// <returns></returns>
        //public static DocsPaVO.documento.InfoDocumento[] RicercaDocumenti(DocsPaVO.Pubblicazione.LoginInfo loginInfo, DocsPaVO.filtri.FiltroRicerca[][] filters)
        //{
        //    DocsPaVO.utente.InfoUtente infoUtente = Utils.Login(loginInfo);

        //    try
        //    {
        //        System.Collections.ArrayList list = BusinessLogic.Documenti.InfoDocManager.getQuery(infoUtente.idGruppo, infoUtente.idPeople, filters);

        //        if (list != null)
        //            return (DocsPaVO.documento.InfoDocumento[])list.ToArray(typeof(DocsPaVO.documento.InfoDocumento));
        //        else
        //            return new DocsPaVO.documento.InfoDocumento[0];
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        Utils.Logoff(infoUtente);
        //    }
        //}



        /// <summary>
        /// 
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        public static DocsPaVO.Pubblicazione.PubblicazioneDocumenti[] RicercaPubblicazioneDocumenti(DocsPaVO.Pubblicazione.FiltroPubblicazioneDocumenti filtro)
        {
            //DocsPaVO.utente.InfoUtente infoUtente = Utils.Login(loginInfo);

            try
            {
                DocsPaDB.Query_DocsPAWS.PubblicazioneDocumenti pubblicazioneDocDb = new DocsPaDB.Query_DocsPAWS.PubblicazioneDocumenti();

                return pubblicazioneDocDb.RicercaPubblicazioneDocumenti(filtro);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //Utils.Logoff(infoUtente);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        public static DocsPaVO.Pubblicazione.PubblicazioneDocumenti[] RicercaPubblicazioneDocumentiBySystemId(DocsPaVO.Pubblicazione.PubblicazioneDocumenti pubblicazione)
        {
            //DocsPaVO.utente.InfoUtente infoUtente = Utils.Login(loginInfo);

            try
            {
                DocsPaDB.Query_DocsPAWS.PubblicazioneDocumenti pubblicazioneDocDb = new DocsPaDB.Query_DocsPAWS.PubblicazioneDocumenti();

                return pubblicazioneDocDb.RicercaPubblicazioneDocumentiByIdProfile(pubblicazione);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //Utils.Logoff(infoUtente);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginInfo"></param>
        /// <param name="pubblicazione"></param>
        /// <returns></returns>
        public static bool InserimentoPubblicazioneDocumento(DocsPaVO.Pubblicazione.PubblicazioneDocumenti pubblicazione)
        {
            bool retValue = false;

            //DocsPaVO.utente.InfoUtente infoUtente = Utils.Login(loginInfo);

            try
            {
                using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                {
                    DocsPaDB.Query_DocsPAWS.PubblicazioneDocumenti pubblicazioneDocDb = new DocsPaDB.Query_DocsPAWS.PubblicazioneDocumenti();

                    retValue = pubblicazioneDocDb.InserimentoPubblicazioneDocumento(pubblicazione);

                    if (retValue)
                        transactionContext.Complete();
                }

                return retValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //Utils.Logoff(infoUtente);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pubblicazione"></param>
        /// <returns></returns>
        public static bool UpdatePubblicazioneDocumento(DocsPaVO.Pubblicazione.PubblicazioneDocumenti pubblicazione)
        {
            bool retValue = false;

            //DocsPaVO.utente.InfoUtente infoUtente = Utils.Login(loginInfo);

            try
            {
                using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                {
                    DocsPaDB.Query_DocsPAWS.PubblicazioneDocumenti pubblicazioneDocDb = new DocsPaDB.Query_DocsPAWS.PubblicazioneDocumenti();

                    retValue = pubblicazioneDocDb.UpdatePubblicazioneDocumento(pubblicazione);

                    if (retValue)
                        transactionContext.Complete();
                }

                return retValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //Utils.Logoff(infoUtente);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pubblicazione"></param>
        /// <returns></returns>
        public static bool UpdatePubblicazioneDocumentoGenerale(DocsPaVO.Pubblicazione.PubblicazioneDocumenti pubblicazione)
        {
            bool retValue = false;

            //DocsPaVO.utente.InfoUtente infoUtente = Utils.Login(loginInfo);

            try
            {
                using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                {
                    DocsPaDB.Query_DocsPAWS.PubblicazioneDocumenti pubblicazioneDocDb = new DocsPaDB.Query_DocsPAWS.PubblicazioneDocumenti();

                    retValue = pubblicazioneDocDb.UpdatePubblicazioneDocumentoGenerale(pubblicazione);

                    if (retValue)
                        transactionContext.Complete();
                }

                return retValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //Utils.Logoff(infoUtente);
            }
        }


        public static bool isexsistdocumentpubblicazione(string id_tipo_documento)
        {
            if (!string.IsNullOrEmpty(id_tipo_documento))
            {
                DocsPaDB.Query_DocsPAWS.PubblicazioneDocumenti pubblicazioneDocDb = new DocsPaDB.Query_DocsPAWS.PubblicazioneDocumenti();

                return pubblicazioneDocDb.exsistDocument(id_tipo_documento);
            }

            return false;
                
        }

        public static DocsPaVO.Pubblicazione.PubblicazioneDocumenti getPubblicazioneDocumentiByIdProfile(string id_profile)
        {
            DocsPaVO.Pubblicazione.PubblicazioneDocumenti pubblicazione = null;
            if(!string.IsNullOrEmpty(id_profile))
            {
               DocsPaDB.Query_DocsPAWS.PubblicazioneDocumenti pubblicazioneDocDb = new DocsPaDB.Query_DocsPAWS.PubblicazioneDocumenti();
               pubblicazione = pubblicazioneDocDb.getPubblicazioneDocumentoByIdProfile(id_profile);
            }
            return pubblicazione;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codice"></param>
        /// <returns></returns>
        public static string ricercaCodice(string codice)
        {
            DocsPaDB.Query_DocsPAWS.PubblicazioneDocumenti pubblicazioneDocDb = new DocsPaDB.Query_DocsPAWS.PubblicazioneDocumenti();
            return pubblicazioneDocDb.getCodiceCorretto(codice);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codice"></param>
        /// <returns></returns>
        public static bool codicePerlaPubblicazione(string codice)
        {
            DocsPaDB.Query_DocsPAWS.PubblicazioneDocumenti pubblicazioneDocDb = new DocsPaDB.Query_DocsPAWS.PubblicazioneDocumenti();
            return pubblicazioneDocDb.codicePerlaPubblicazione(codice);
        }
    }
}
