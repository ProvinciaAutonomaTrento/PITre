using System;
using System.Collections;
using System.Threading;
using log4net;

namespace BusinessLogic.AS400
{
    /// <summary>
    /// Summary description for AS400.
    /// </summary>
    public class AS400
    {
        private static ILog logger = LogManager.GetLogger(typeof(AS400));
        public AS400()
        {
            //TODO: ProfilazioneDinamica.Oggetti.Templates t=new ProfilazioneDinamica.Oggetti.Templates();

        }
        public static Mutex sem = new Mutex();
        /// <summary>
        /// AS400 CONSRER ogni protocollo fascicolato o modificato in oggetto deve finire anhce in 
        /// delle tabelle di collegamento presenti solo sulla versione etdocs sql di consrer che servono da tramite per 
        /// aggiornare il sistema as400 presente dal cliente. in controlli se il protocollo è fascicolato oppure ha oggetto modificato sono fatti all'interno della progetto DocspaAS400
        /// </summary>
        /// <param name="schedaDoc"></param>
        public static void setAs400InFolder(string idProfile, DocsPaVO.utente.InfoUtente infoUtente, string idFolder, string constAs400)
        {

            DocsPaVO.documento.SchedaDocumento schedaDoc = null;


            //AS400 
            string AS400 = System.Configuration.ConfigurationSettings.AppSettings["AS400"];
            DocsPaDB.DBProvider db = null;


            sem.WaitOne();
            try
            {
                if (AS400 != null && AS400.Equals("1"))
                {
                    logger.Debug("In AS400");
                    //					string[] checkValues= {"","",""} ; //inizializzazione valori, per essere lanciato l'as400 almeno uno dei valori deve esistere e deve esseere diverso da vuoto.
                    //ricerca scheda.
                    schedaDoc = BusinessLogic.Documenti.DocManager.getDettaglioPerNotificaAllegati(infoUtente, idProfile, "");
                    if (schedaDoc != null && schedaDoc.tipoProto != null
                        && schedaDoc.tipoProto != "G"
                        && schedaDoc.tipoProto != "R"
                        && schedaDoc.tipologiaAtto != null
                        && (schedaDoc.tipologiaAtto.descrizione != null
                        && schedaDoc.tipologiaAtto.descrizione != "")
                                                                     ) //SOLO PROTOCOLLI
                    {



                        DocsPaDB.Query_DocsPAWS.Model model = new DocsPaDB.Query_DocsPAWS.Model();
                        DocsPaVO.ProfilazioneDinamica.Templates schTmpl = model.getTemplateDettagli(schedaDoc.docNumber);

                        if (schTmpl != null)
                        {
                            for (int i = 0; i < schTmpl.ELENCO_OGGETTI.Count; i++)
                            {
                                DocsPaVO.ProfilazioneDinamica.OggettoCustom objc = (DocsPaVO.ProfilazioneDinamica.OggettoCustom)schTmpl.ELENCO_OGGETTI[i];
                                if (objc.DESCRIZIONE.ToLower().Trim().Equals("tipo atto"))
                                {
                                    //sostituisco il valore perchè il codice dell'AS400 in docspa30 in realtà la tipologia 
                                    //è nella profilazione dinamica, mentre il tipo atto in docspa è solo una categoria di tipi_atto_profilati_dinamicamente
                                    schedaDoc.tipologiaAtto.descrizione = objc.VALORE_DATABASE.Trim();
                                    //									checkValues[0]=objc.VALORE_DATABASE.Trim();
                                }
                                if (objc.DESCRIZIONE.ToLower().Trim().Equals("commissione referente"))
                                {
                                    //sostituisco il valore perchè il codice dell'AS400 in docspa30
                                    schedaDoc.commissioneRef = objc.VALORE_DATABASE.Trim();
                                    //									checkValues[1]=objc.VALORE_DATABASE.Trim();
                                }
                                if (objc.DESCRIZIONE.ToLower().Trim().Equals("numero"))
                                {
                                    //sostituisco il valore perchè il codice dell'AS400 in docspa30
                                    schedaDoc.numOggetto = objc.VALORE_DATABASE.Trim();
                                    //									checkValues[2]=objc.VALORE_DATABASE.Trim();
                                }
                            }
                        }
                    }


                    DocsPaAS400.InsertAgent ia = new DocsPaAS400.InsertAgent();
                    //per essere lanciato l'as400 almeno 
                    //uno dei valori deve esistere e deve esseere diverso da vuoto.
                    //					if(checkValues[0]!="" ||
                    //						checkValues[1]!=""||
                    //						checkValues[2]!="")
                    ia.addDocFolder(schedaDoc, schedaDoc.systemId, idFolder);

                }
                //FINE AS400
            }
            catch (Exception e)
            {
                logger.Debug(e);
            }
            finally
            {
                if (db != null)
                    db.Dispose();
                sem.ReleaseMutex();
            }
        }
        // <summary>
        /// AS400 CONSRER ogni protocollo fascicolato o modificato in oggetto deve finire anhce in 
        /// delle tabelle di collegamento presenti solo sulla versione etdocs sql di consrer che servono da tramite per 
        /// aggiornare il sistema as400 presente dal cliente. in controlli se il protocollo è fascicolato oppure ha oggetto modificato sono fatti all'interno della progetto DocspaAS400
        /// </summary>
        /// <param name="schedaDoc"></param>
        public static void setAs400(DocsPaVO.utente.InfoUtente infoUtente, string docnumber, string idProfile, string constAs400)
        {

            DocsPaVO.documento.SchedaDocumento schedaDoc = null;


            //AS400 
            string AS400 = System.Configuration.ConfigurationSettings.AppSettings["AS400"];
            DocsPaDB.DBProvider db = null;
            sem.WaitOne();
            try
            {
                if (AS400 != null && AS400.Equals("1"))
                {
                    logger.Debug("In AS400");
                    //					string[] checkValues= {"","",""} ; //inizializzazione valori, per essere lanciato l'as400 almeno uno dei valori deve esistere e deve esseere diverso da vuoto.

                    //ricerca scheda.
                    DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
                    schedaDoc = BusinessLogic.Documenti.DocManager.getDettaglioPerNotificaAllegati(infoUtente, idProfile, docnumber);
                    if (schedaDoc != null && schedaDoc.tipoProto != null
                        && schedaDoc.tipoProto != "G"
                        && schedaDoc.tipoProto != "R"
                        && schedaDoc.tipologiaAtto != null
                        && (schedaDoc.tipologiaAtto.descrizione != null
                        && schedaDoc.tipologiaAtto.descrizione != "")) //SOLO PROTOCOLLI
                    {

                        DocsPaDB.Query_DocsPAWS.Model model = new DocsPaDB.Query_DocsPAWS.Model();
                        DocsPaVO.ProfilazioneDinamica.Templates schTmpl = model.getTemplateDettagli(schedaDoc.docNumber);

                        if (schTmpl != null)
                        {
                            for (int i = 0; i < schTmpl.ELENCO_OGGETTI.Count; i++)
                            {
                                DocsPaVO.ProfilazioneDinamica.OggettoCustom objc = (DocsPaVO.ProfilazioneDinamica.OggettoCustom)schTmpl.ELENCO_OGGETTI[i];
                                if (objc.DESCRIZIONE.ToLower().Trim().Equals("tipo atto"))
                                {
                                    //sostituisco il valore perchè il codice dell'AS400 in docspa30 in realtà la tipologia 
                                    //è nella profilazione dinamica, mentre il tipo atto in docspa è solo una categoria di tipi_atto_profilati_dinamicamente
                                    schedaDoc.tipologiaAtto.descrizione = objc.VALORE_DATABASE.Trim();
                                    //									checkValues[0]=objc.VALORE_DATABASE.Trim();
                                }
                                if (objc.DESCRIZIONE.ToLower().Trim().Equals("commissione referente"))
                                {
                                    //sostituisco il valore perchè il codice dell'AS400 in docspa30 in realtà la tipologia 
                                    //è nella profilazione dinamica, mentre il tipo atto in docspa è solo una categoria di tipi_atto_profilati_dinamicamente
                                    schedaDoc.commissioneRef = objc.VALORE_DATABASE.Trim();
                                    //									checkValues[1]=objc.VALORE_DATABASE.Trim();
                                }
                                if (objc.DESCRIZIONE.ToLower().Trim().Equals("numero"))
                                {
                                    //sostituisco il valore perchè il codice dell'AS400 in docspa30 in realtà la tipologia 
                                    //è nella profilazione dinamica, mentre il tipo atto in docspa è solo una categoria di tipi_atto_profilati_dinamicamente
                                    schedaDoc.numOggetto = objc.VALORE_DATABASE.Trim();
                                    //									checkValues[2]=objc.VALORE_DATABASE.Trim();
                                }
                            }
                        }
                    }


                    DocsPaAS400.InsertAgent ia = new DocsPaAS400.InsertAgent();
                    //per essere lanciato l'as400 almeno 
                    //uno dei valori deve esistere e deve esseere diverso da vuoto.
                    //					if(checkValues[0]!="" ||
                    //						checkValues[1]!=""||
                    //						checkValues[2]!="")
                    //					{
                    db = new DocsPaDB.DBProvider();
                    db.BeginTransaction();
                    ArrayList queries = ia.getInsertQueries(constAs400, schedaDoc, db);
                    for (int i = 0; i < queries.Count; i++)
                    {

                        if (!db.ExecuteLockedNonQuery((string)queries[i]))
                            throw new Exception("Errore nell'inserimento dei dati in AS400");

                    }
                    db.CommitTransaction();
                }

                //				}
                //FINE AS400
            }
            catch (Exception e)
            {
                db.RollbackTransaction();
                logger.Debug(e);
            }
            finally
            {
                if (db != null)
                {
                    db.Dispose();

                }
                sem.ReleaseMutex();
            }
        }
        /// <summary>
        /// AS400 CONSRER ogni protocollo fascicolato o modificato in oggetto deve finire anhce in 
        /// delle tabelle di collegamento presenti solo sulla versione etdocs sql di consrer che servono da tramite per 
        /// aggiornare il sistema as400 presente dal cliente. in controlli se il protocollo è fascicolato oppure ha oggetto modificato sono fatti all'interno della progetto DocspaAS400
        /// </summary>
        /// <param name="schedaDoc"></param>
        public static void setAs400(DocsPaVO.documento.SchedaDocumento schedaDoc, string constAs400)
        {
            //AS400 
            string AS400 = System.Configuration.ConfigurationSettings.AppSettings["AS400"];
            DocsPaDB.DBProvider db = null;
            sem.WaitOne();
            try
            {
                if (AS400 != null && AS400.Equals("1"))
                {
                    //					string[] checkValues= {"","",""} ; //inizializzazione valori, per essere lanciato l'as400 almeno uno dei valori deve esistere e deve esseere diverso da vuoto.

                    logger.Debug("In AS400");
                    DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
                    //schedaDoc=BusinessLogic.Documenti.DocManager.getDettaglioPerNotificaAllegati(idGruppo,idPeople,idProfile,docnumber);
                    if (schedaDoc != null && schedaDoc.tipoProto != null
                        && schedaDoc.tipoProto != "G"
                        && schedaDoc.tipoProto != "R"
                        && schedaDoc.tipologiaAtto != null
                        && (schedaDoc.tipologiaAtto.descrizione != null
                        && schedaDoc.tipologiaAtto.descrizione != "")) //SOLO PROTOCOLLI
                    {
                        DocsPaDB.Query_DocsPAWS.Model model = new DocsPaDB.Query_DocsPAWS.Model();
                        DocsPaVO.ProfilazioneDinamica.Templates schTmpl = model.getTemplateDettagli(schedaDoc.docNumber);

                        if (schTmpl != null)
                        {
                            for (int i = 0; i < schTmpl.ELENCO_OGGETTI.Count; i++)
                            {
                                DocsPaVO.ProfilazioneDinamica.OggettoCustom objc = (DocsPaVO.ProfilazioneDinamica.OggettoCustom)schTmpl.ELENCO_OGGETTI[i];
                                if (objc.DESCRIZIONE.ToLower().Trim().Equals("tipo atto"))
                                {
                                    //sostituisco il valore perchè il codice dell'AS400 in docspa30 in realtà la tipologia 
                                    //è nella profilazione dinamica, mentre il tipo atto in docspa è solo una categoria di tipi_atto_profilati_dinamicamente
                                    schedaDoc.tipologiaAtto.descrizione = objc.VALORE_DATABASE.Trim();
                                    //									checkValues[0]=objc.VALORE_DATABASE.Trim();
                                }
                                if (objc.DESCRIZIONE.ToLower().Trim().Equals("commissione referente"))
                                {
                                    //sostituisco il valore perchè il codice dell'AS400 in docspa30 in realtà la tipologia 
                                    //è nella profilazione dinamica, mentre il tipo atto in docspa è solo una categoria di tipi_atto_profilati_dinamicamente
                                    schedaDoc.commissioneRef = objc.VALORE_DATABASE.Trim();
                                    //									checkValues[1]=objc.VALORE_DATABASE.Trim();
                                }
                                if (objc.DESCRIZIONE.ToLower().Trim().Equals("numero"))
                                {
                                    //sostituisco il valore perchè il codice dell'AS400 in docspa30 in realtà la tipologia 
                                    //è nella profilazione dinamica, mentre il tipo atto in docspa è solo una categoria di tipi_atto_profilati_dinamicamente
                                    schedaDoc.numOggetto = objc.VALORE_DATABASE.Trim();
                                    //checkValues[2]=objc.VALORE_DATABASE.Trim();
                                }
                            }
                        }
                    }




                    DocsPaAS400.InsertAgent ia = new DocsPaAS400.InsertAgent();
                    //					if(checkValues[0]!="" ||
                    //						checkValues[1]!=""||
                    //						checkValues[2]!="")
                    //					{
                    db = new DocsPaDB.DBProvider();
                    db.BeginTransaction();
                    ArrayList queries = ia.getInsertQueries(constAs400, schedaDoc, db);
                    for (int i = 0; i < queries.Count; i++)
                    {

                        if (!db.ExecuteLockedNonQuery((string)queries[i]))
                            throw new Exception("Errore nell'inserimento dei dati in AS400");

                    }
                    db.CommitTransaction();
                }
                //				}
                //FINE AS400
            }
            catch (Exception e)
            {
                db.RollbackTransaction();
                logger.Debug(e);
            }
            finally
            {
                if (db != null)
                {
                    db.Dispose();
                }
                sem.ReleaseMutex();
            }
        }

    }
}

