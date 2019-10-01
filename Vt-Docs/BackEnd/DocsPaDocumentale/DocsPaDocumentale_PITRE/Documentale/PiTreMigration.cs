using System;
using System.Collections.Generic;
using DocsPaVO.documento;
using DocsPaVO.utente;
using DocumentManagerETDOCS = DocsPaDocumentale_ETDOCS.Documentale.DocumentManager;
using DocumentManagerDOCUMENTUM = DocsPaDocumentale_DOCUMENTUM.Documentale.DocumentManager;
using log4net;
using System.Security.Cryptography;



namespace DocsPaDocumentale_PITRE.Documentale
{
    public class PiTreMigration
    {
        private static ILog logger = LogManager.GetLogger(typeof(PiTreMigration));

        


        public static string SincronizzaDocNumber(List<string> docnumbers, bool dryrun)
        {
            string report = string.Empty;
            string repLine = string.Empty;
            try
            {
                foreach (string system_id in docnumbers)
                {
                    repLine = String.Format("Elaboro documento :{0}", system_id);
                    logger.DebugFormat(repLine);
                    report += repLine + "\r\n";

                    using (DocsPaDB.DBProvider dbp = new DocsPaDB.DBProvider())
                    {
                        //todo Tocare
                        string cmd = String.Format("select 	p.id_ruolo_creatore, p.author , d.id_gruppo, getidamm(p.author) id_amm from profile p , dpa_corr_globali d where  d.system_id=p.id_ruolo_creatore and p.system_id={0}", system_id);
                        using (System.Data.IDataReader reader = dbp.ExecuteReader(cmd))
                        {
                            while (reader.Read())
                            {
                                InfoUtente infout = new InfoUtente();
                                infout.idAmministrazione = reader.GetValue(reader.GetOrdinal("id_amm")).ToString();
                                infout.idCorrGlobali = reader.GetValue(reader.GetOrdinal("id_ruolo_creatore")).ToString();
                                infout.idPeople = reader.GetValue(reader.GetOrdinal("author")).ToString();
                                infout.idGruppo = reader.GetValue(reader.GetOrdinal("id_gruppo")).ToString();
                            
                                //GETdettaglio no sec
                                DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
                                DocsPaVO.documento.SchedaDocumento schedaDoc = new DocsPaVO.documento.SchedaDocumento();
                                schedaDoc = doc.GetDettaglioNoSecurity(infout, system_id, system_id);

                                DocsPaDocumentale_ETDOCS.Documentale.DocumentManager etdocs = new DocumentManagerETDOCS(infout);
                                DocsPaDocumentale_DOCUMENTUM.Documentale.DocumentManager DCTM = new DocumentManagerDOCUMENTUM(infout);
                                
                                //elaborazione DOC principale
                                for (int i = schedaDoc.documenti.Count - 1; i >= 0; i--)
                                {
                                    Documento documento = (DocsPaVO.documento.Documento)schedaDoc.documenti[i];
                                    byte[] dtcmContent = DCTM.GetFile(system_id, documento.version, null, null);
                                    // se null il file non esiste
                                    if (dtcmContent == null)
                                    {
                                        repLine = String.Format("Aggiungo Versione su Documentum {0}", documento.versionId);
                                        logger.DebugFormat(repLine);
                                        report += repLine + "\r\n";

                                        if (!dryrun)
                                        {
                                            if (DCTM.AddVersion(documento, false))
                                            {
                                                repLine = String.Format("Aggiungo File su Documentum");
                                                logger.Debug(repLine);
                                                report += repLine + "\r\n";
                                                if (!dryrun)
                                                    AddFileDCMT(system_id, documento, infout);
                                            }
                                            else
                                            {
                                                repLine = String.Format("ERRORE Non riesco a creare la versione {0} su Documentum", documento.version);
                                                logger.ErrorFormat(repLine);
                                                report += repLine + "\r\n";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        byte[] etdContent = etdocs.GetFile(system_id, documento.versionId, null, null);
                                        if (getHashSha256(dtcmContent) != getHashSha256(etdContent))
                                        {
                                            repLine = String.Format("ERRORE Attenzione!! i file tra  Documentum e FS sono diversi");
                                            logger.ErrorFormat(repLine);
                                            report += repLine + "\r\n";
                                        }
                                    }
                                } //for versioni documento principale

                                //ELABORAZIONE ALLEGATI
                                foreach (Allegato allegato in schedaDoc.allegati)
                                {
                                    DocsPaVO.documento.SchedaDocumento schedaAll = doc.GetDettaglioNoSecurity(infout, allegato.docNumber, allegato.docNumber);
                                    if (!DCTM.ContainsDocumento(schedaAll.docNumber))
                                    {
                                        if (!dryrun)
                                        {
                                            if (DCTM.AddAttachment(allegato, null))
                                            {
                                                repLine = String.Format("ERRORE Aggiungo Allegato su Documentum {0} ha dato Errore", allegato.docNumber);
                                                logger.ErrorFormat(repLine);
                                                report += repLine + "\r\n";
                                            }
                                        }
                                    }
                                    for (int i = schedaAll.documenti.Count - 1; i >= 0; i--)
                                    {
                                        Documento docAllegato = (DocsPaVO.documento.Documento)schedaDoc.documenti[i];
                                        byte[] dtcmContent = DCTM.GetFile(docAllegato.docNumber, docAllegato.version, null, null);
                                        // se null il file non esiste
                                        if (dtcmContent == null)
                                        {
                                            repLine = String.Format("Aggiungo Versione su Documentum {0}", docAllegato.versionId);
                                            logger.DebugFormat(repLine);
                                            report += repLine + "\r\n";

                                            if (!dryrun)
                                            {
                                                if (DCTM.AddVersion(docAllegato, false))
                                                {
                                                    repLine = String.Format("Aggiungo File su Documentum");
                                                    logger.Debug(repLine);
                                                    report += repLine + "\r\n";
                                                    if (!dryrun)
                                                        AddFileDCMT(schedaAll.systemId, docAllegato, infout);
                                                }
                                                else
                                                {
                                                    repLine = String.Format("ERRORE Non riesco a creare la versione {0} su Documentum", docAllegato.version);
                                                    logger.ErrorFormat(repLine);
                                                    report += repLine + "\r\n";
                                                }
                                            }
                                        }
                                        else
                                        {
                                            byte[] etdContent = etdocs.GetFile(system_id, docAllegato.versionId, null, null);
                                            if (getHashSha256(dtcmContent) != getHashSha256(etdContent))
                                            {
                                                repLine = String.Format("Attenzione!! i file tra  Documentum e FS sono diversi");
                                                logger.ErrorFormat(repLine);
                                                report += repLine + "\r\n";
                                            }
                                        }//else esiste content
                                    }//for versioni in allegato
                                }// forAllegati
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                repLine = String.Format("Errore in AddVersionInDocPrincipale msg {0}  stk {1}", ex.Message, ex.StackTrace);
                logger.ErrorFormat(repLine);
                report += repLine + "\r\n";
            }
            return repLine;
        }



        public static string AddAllegatiEVersioniInDocPrincipale(List<string> docnumbers, bool dryrun)
        {
            string report = string.Empty;
            string repLine = string.Empty;
            try
            {
                foreach (string system_id in docnumbers)
                {
                    repLine = String.Format("Elaboro documento :{0}", system_id);
                    logger.DebugFormat(repLine);
                    report += repLine + "\r\n";

                    using (DocsPaDB.DBProvider dbp = new DocsPaDB.DBProvider())
                    {
                        //todo Tocare
                        string cmd = String.Format("select 	p.id_ruolo_creatore, p.author , d.id_gruppo, getidamm(p.author) id_amm from profile p , dpa_corr_globali d where  d.system_id=p.id_ruolo_creatore and p.system_id={0}", system_id);
                        using (System.Data.IDataReader reader = dbp.ExecuteReader(cmd))
                        {
                            while (reader.Read())
                            {
                                InfoUtente infout = new InfoUtente();
                                infout.idAmministrazione = reader.GetValue(reader.GetOrdinal("id_amm")).ToString();
                                infout.idCorrGlobali = reader.GetValue(reader.GetOrdinal("id_ruolo_creatore")).ToString();
                                infout.idPeople = reader.GetValue(reader.GetOrdinal("author")).ToString();
                                infout.idGruppo = reader.GetValue(reader.GetOrdinal("id_gruppo")).ToString();
                              
                                //GETdettaglio no sec
                                DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
                                DocsPaVO.documento.SchedaDocumento schedaDoc = new DocsPaVO.documento.SchedaDocumento();
                                schedaDoc = doc.GetDettaglioNoSecurity(infout, system_id, system_id);

                                DocsPaDocumentale_ETDOCS.Documentale.DocumentManager etdocs = new DocumentManagerETDOCS(infout);
                                DocsPaDocumentale_DOCUMENTUM.Documentale.DocumentManager DCTM = new DocumentManagerDOCUMENTUM(infout);
                                //ELABORAZIONE ALLEGATI
                                foreach (Allegato allegato in schedaDoc.allegati)
                                {
                                    DocsPaVO.documento.SchedaDocumento schedaAll = doc.GetDettaglioNoSecurity(infout, allegato.docNumber, allegato.docNumber);
                                    if (!DCTM.ContainsDocumento(schedaAll.docNumber))
                                    {
                                        if (!dryrun)
                                        {
                                            if (DCTM.AddAttachment(allegato, null))
                                            {
                                                repLine = String.Format("ERRORE Aggiungo Allegato su Documentum {0} ha dato Errore", allegato.docNumber);
                                                logger.ErrorFormat(repLine);
                                                report += repLine + "\r\n";
                                            }
                                        }
                                    }
                                    for (int i = schedaAll.documenti.Count - 1; i >= 0; i--)
                                    {
                                        Documento docAllegato = (DocsPaVO.documento.Documento)schedaDoc.documenti[i];
                                        byte[] dtcmContent = DCTM.GetFile(docAllegato.docNumber, docAllegato.version, null, null);
                                        // se null il file non esiste
                                        if (dtcmContent == null)
                                        {
                                            repLine = String.Format("Aggiungo Versione su Documentum {0}", docAllegato.versionId);
                                            logger.DebugFormat(repLine);
                                            report += repLine + "\r\n";

                                            if (!dryrun)
                                            {
                                                if (DCTM.AddVersion(docAllegato, false))
                                                {
                                                    repLine = String.Format("Aggiungo File su Documentum");
                                                    logger.Debug(repLine);
                                                    report += repLine + "\r\n";
                                                    if (!dryrun)
                                                        AddFileDCMT(schedaAll.systemId, docAllegato, infout);
                                                }
                                                else
                                                {
                                                    repLine = String.Format("ERRORE Non riesco a creare la versione {0} su Documentum", docAllegato.version);
                                                    logger.ErrorFormat(repLine);
                                                    report += repLine + "\r\n";
                                                }
                                            }
                                        }
                                        else
                                        {
                                            byte[] etdContent = etdocs.GetFile(system_id, docAllegato.versionId, null, null);
                                            if (getHashSha256(dtcmContent) != getHashSha256(etdContent))
                                            {
                                                repLine = String.Format("Attenzione!! i file tra  Documentum e FS sono diversi");
                                                logger.ErrorFormat(repLine);
                                                report += repLine + "\r\n";
                                            }
                                        }//else esiste content
                                    }//for versioni in allegato
                                }// forAllegati
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                repLine = String.Format("Errore in AddVersionInDocPrincipale msg {0}  stk {1}", ex.Message, ex.StackTrace);
                logger.ErrorFormat(repLine);
                report += repLine + "\r\n";
            }
            return repLine;
        }



        public static string AddVersionInDocPrincipale(List<string> docnumbers, bool dryrun)
        {
            string report=string.Empty;
            string repLine = string.Empty;
            try
            {
                foreach (string system_id in docnumbers)
                {
                    repLine = String.Format("Elaboro documento :{0}",system_id);
                    logger.DebugFormat (repLine);
                    report += repLine+"\r\n";

                    using (DocsPaDB.DBProvider dbp = new DocsPaDB.DBProvider())
                    {
                        //todo Tocare
                        string cmd = String.Format("select 	p.id_ruolo_creatore, p.author , d.id_gruppo, getidamm(p.author) id_amm from profile p , dpa_corr_globali d where  d.system_id=p.id_ruolo_creatore and p.system_id={0}", system_id);
                        using (System.Data.IDataReader reader = dbp.ExecuteReader(cmd))
                        {
                            while (reader.Read())
                            {
                                InfoUtente infout = new InfoUtente();
                                infout.idAmministrazione = reader.GetValue(reader.GetOrdinal("id_amm")).ToString();
                                infout.idCorrGlobali = reader.GetValue(reader.GetOrdinal("id_ruolo_creatore")).ToString();
                                infout.idPeople = reader.GetValue(reader.GetOrdinal("author")).ToString();
                                infout.idGruppo = reader.GetValue(reader.GetOrdinal("id_gruppo")).ToString();
                               
                                //GETdettaglio no sec
                                DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
                                DocsPaVO.documento.SchedaDocumento schedaDoc = new DocsPaVO.documento.SchedaDocumento();
                                schedaDoc = doc.GetDettaglioNoSecurity(infout, system_id, system_id);

                                DocsPaDocumentale_ETDOCS.Documentale.DocumentManager etdocs = new DocumentManagerETDOCS(infout);
                                DocsPaDocumentale_DOCUMENTUM.Documentale.DocumentManager DCTM = new DocumentManagerDOCUMENTUM(infout);

                                //foreach (Documento documento in schedaDoc.documenti)
                                for (int i = schedaDoc.documenti.Count-1; i>=0;i--)
                                {
                                    Documento documento = (DocsPaVO.documento.Documento)schedaDoc.documenti[i];
                                    byte[] dtcmContent = DCTM.GetFile(system_id, documento.version, null, null);
                                    // se null il file non esiste
                                    if (dtcmContent == null)
                                    {
                                        repLine = String.Format("Aggiungo Versione su Documentum {0}", documento.versionId);
                                        logger.DebugFormat(repLine);
                                        report += repLine + "\r\n";

                                        if (!dryrun)
                                        {
                                            if (DCTM.AddVersion(documento, false))
                                            {
                                                repLine = String.Format("Aggiungo File su Documentum");
                                                logger.Debug(repLine);
                                                report += repLine + "\r\n";
                                                if (!dryrun)
                                                    AddFileDCMT(system_id, documento, infout);
                                            }
                                            else
                                            {
                                                repLine = String.Format("ERRORE Non riesco a creare la versione {0} su Documentum", documento.version);
                                                logger.ErrorFormat(repLine);
                                                report += repLine + "\r\n";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        byte[] etdContent = etdocs.GetFile(system_id, documento.versionId, null, null);
                                        if (getHashSha256(dtcmContent) != getHashSha256(etdContent))
                                        {
                                            repLine = String.Format("ERRORE Attenzione!! i file tra  Documentum e FS sono diversi");
                                            logger.ErrorFormat(repLine);
                                            report += repLine + "\r\n";
                                        }
                                    }
                                }//for versioni documenti
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                repLine = String.Format("Errore in AddVersionInDocPrincipale msg {0}  stk {1}", ex.Message, ex.StackTrace);
                logger.ErrorFormat(repLine);
                report += repLine + "\r\n";
            }
            return repLine;
        }


        public static string getHashSha256(byte[] bytes)
        {
            SHA256Managed hashstring = new SHA256Managed();
            byte[] hash = hashstring.ComputeHash(bytes);
            string hashString = BitConverter.ToString(hash).Replace(":", string.Empty).ToUpper();
            return hashString;
        }

        public static string P3Migration()
        {
            try
            {

                            
                //creazione fascicoli
//                using (DocsPaDB.DBProvider dbp = new DocsPaDB.DBProvider())
//                {
//                    string cmd = "SELECT p.system_id system_id,        author,        id_ruolo_creatore,        p.id_amm id_amm,        C.ID_GRUPPO id_gruppo   FROM project p, dpa_corr_globali c  WHERE     c.system_id = P.ID_RUOLO_CREATORE        AND cha_tipo_Proj = 'F'        AND cha_tipo_fascicolo = 'P'        AND p.system_id IN (50712739)";
 

//                    using (System.Data.IDataReader reader = dbp.ExecuteReader(cmd))
//                    {
//                        while (reader.Read())
//                        {
//                            string id_project = reader.GetValue(reader.GetOrdinal("system_id")).ToString();
//                            logger.Debug("MIGRAZIONE- fasc: " + id_project);
//                            string author_proj = reader.GetValue(reader.GetOrdinal("author")).ToString();
//                            string id_ruolo_fasc = reader.GetValue(reader.GetOrdinal("id_ruolo_creatore")).ToString();
//                            if (id_ruolo_fasc.Equals("105905231"))
//                                id_ruolo_fasc = "14768151";
//                            //infotente
//                            InfoUtente infoutf = new InfoUtente();
//                            DocsPaDocumentale_DOCUMENTUM.Documentale.UserManager usr = new DocsPaDocumentale_DOCUMENTUM.Documentale.UserManager();
//                            infoutf.dst = usr.GetSuperUserAuthenticationToken();
//                            infoutf.idAmministrazione = reader.GetValue(reader.GetOrdinal("id_amm")).ToString();
                          
//                            infoutf.idCorrGlobali = id_ruolo_fasc;
//                            infoutf.idPeople = author_proj;
//                            infoutf.idGruppo = reader.GetValue(reader.GetOrdinal("id_gruppo")).ToString();

//                            if (infoutf.idGruppo.Equals(""))
//                                infoutf.idGruppo = "14768150";
                                    
                           


//                           DocsPaDocumentale_ETDOCS.Documentale.ProjectManager projET= new DocsPaDocumentale_ETDOCS.Documentale.ProjectManager(infoutf);
                            
//                           DocsPaDB.Query_DocsPAWS.Utenti ut=new DocsPaDB.Query_DocsPAWS.Utenti();
//                          DocsPaVO.utente.Ruolo ruolo=  ut.GetRuolo(id_ruolo_fasc);
//                           // 
//DocsPaDB.Query_DocsPAWS.Fascicoli fasc=new DocsPaDB.Query_DocsPAWS.Fascicoli();
//DocsPaVO.fascicolazione.Fascicolo fascicolo=fasc.GetFascicoloById(id_project,infoutf);
//logger.Debug("MIGRAZIONE- fasc: " + fascicolo.codice);
                          
//                            DocsPaDocumentale_DOCUMENTUM.Documentale.ProjectManager projMAN = new DocsPaDocumentale_DOCUMENTUM.Documentale.ProjectManager(infoutf);
//                            DocsPaVO.fascicolazione.Classificazione classif = new DocsPaVO.fascicolazione.Classificazione();
//                           DocsPaVO.fascicolazione.ResultCreazioneFascicolo rtnFasc= DocsPaVO.fascicolazione.ResultCreazioneFascicolo.GENERIC_ERROR; 
//                            projMAN.CreateProject(classif,fascicolo, ruolo, false,out rtnFasc );
                                
//                            if(rtnFasc==DocsPaVO.fascicolazione.ResultCreazioneFascicolo.OK )
//                            {
//                                logger.Debug("MIGRAZIONE- fasc creato con successo");
//                                // allineamento Security con ACL Dctm FASCICOLO
//                                DocsPaDocumentale_DOCUMENTUM.DctmServices.Custom.AclDefinition aclData =
//                                    DocsPaDocumentale_DOCUMENTUM.DctmServices.Dfs4DocsPa.getAclDefinition(id_project, DocsPaDocumentale_DOCUMENTUM.DocsPaObjectTypes.ObjectTypes.FASCICOLO, infoutf);

//                                //Aggiornamento ACL in DCTM, con le credenziali di superuser
//                                DocsPaDocumentale_DOCUMENTUM.DctmServices.Custom.IAclService aclService =
//                                    DocsPaDocumentale_DOCUMENTUM.DctmServices.DctmServiceFactory.GetCustomServiceInstance<DocsPaDocumentale_DOCUMENTUM.DctmServices.Custom.IAclService>(infoutf.dst);

//                                aclService.ClearAndGrant(aclData, id_project);
//                                logger.Debug("MIGRAZIONE- fasc create ACL con successo");
//                                //crea folder ..
//                                System.Collections.ArrayList folders = fasc.GetFolderByIdFasc(infoutf, id_project, null, false, true);

//                                if (folders != null && folders.Count > 0)
//                                {
//                                    logger.Debug("MIGRAZIONE- fasc numero Folder trovare "+folders.Count);
//                                    DocsPaVO.fascicolazione.ResultCreazioneFolder rtnfolder = DocsPaVO.fascicolazione.ResultCreazioneFolder.GENERIC_ERROR; 
                                           
//                                    foreach (DocsPaVO.fascicolazione.Folder folder in folders)
//                                    {
//                                        rtnfolder=DocsPaVO.fascicolazione.ResultCreazioneFolder.GENERIC_ERROR;
//                                        if(folder.idParent!=folder.idFascicolo) //Is sotto fasc
//                                        {
//                                            logger.Debug("MIGRAZIONE- creazione folder "+folder.systemID+" "+folder.descrizione+" del fascicolo "+fascicolo.codice);
           
         
//            string[] formati = { "dd/MM/yyyy HH.mm.ss", "dd/MM/yyyy H.mm.ss", "dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy H:mm:ss", "dd/MM/yyyy" };
//          DateTime date=new DateTime(2014,07,21);
//                                            DateTime dateOut=new DateTime();
//                                           bool ok = DateTime.TryParse(folder.dtaApertura, out dateOut);
//                                           if (dateOut >= date)
//                                           {
//                                               bool rtn = projMAN.CreateFolder(folder, ruolo, out rtnfolder);

//                                               if (rtnfolder == DocsPaVO.fascicolazione.ResultCreazioneFolder.OK)
//                                               {
//                                                   logger.Debug("MIGRAZIONE- creazione folder " + folder.systemID + " ok");

//                                                   // allineamento Security con ACL Dctm FASCICOLO
//                                                   //  aclData =
//                                                   //   DocsPaDocumentale_DOCUMENTUM.DctmServices.Dfs4DocsPa.getAclDefinition(folder.systemID, DocsPaDocumentale_DOCUMENTUM.DocsPaObjectTypes.ObjectTypes.SOTTOFASCICOLO,infoutf);

//                                                   //Aggiornamento ACL in DCTM, con le credenziali di superuser
//                                                   // aclService =
//                                                   //      DocsPaDocumentale_DOCUMENTUM.DctmServices.DctmServiceFactory.GetCustomServiceInstance<DocsPaDocumentale_DOCUMENTUM.DctmServices.Custom.IAclService>(infoutf.dst);


//                                                   //  aclService.ClearAndGrant(aclData, folder.systemID);

//                                                   //    logger.Debug("MIGRAZIONE- creazione ACL folder " + folder.systemID + " ok");

//                                               }

//                                           }


//                                        }


//                                    }
//                                }
//                            }


                          

//                        }
//                    }
//                }

                using (DocsPaDB.DBProvider dbp = new DocsPaDB.DBProvider())
                {

                    string cmd = " select distinct(v.version_id) version_id, v.docnumber system_id, p.id_ruolo_creatore, p.author ,d.id_gruppo, getidamm(p.author) id_amm from versions v join profile p on v.DOCNUMBER = p.system_id , dpa_corr_globali d where  d.system_id=p.id_ruolo_creatore and  v.dta_creazione BETWEEN TO_DATE ('21/07/2014 00:00:00', 'dd/mm/yyyy HH24:mi:ss') AND TO_DATE ('21/07/2014 23:59:59','dd/mm/yyyy HH24:mi:ss') and p.id_documento_principale is null and p.creation_date not BETWEEN TO_DATE ('21/07/2014 00:00:00','dd/mm/yyyy HH24:mi:ss') and tO_DATE ('21/07/2014 23:59:59','dd/mm/yyyy HH24:mi:ss')";

                    using (System.Data.IDataReader reader = dbp.ExecuteReader(cmd))
                    {
                        while (reader.Read())
                        {
                           // string tipo = reader.GetValue(reader.GetOrdinal("cha_tipo_proto")).ToString();
                            //creo infout
                            InfoUtente infout = new InfoUtente();
                            DocsPaDocumentale_DOCUMENTUM.Documentale.UserManager usr = new DocsPaDocumentale_DOCUMENTUM.Documentale.UserManager();
                            infout.dst = usr.GetSuperUserAuthenticationToken();
                            infout.idAmministrazione = reader.GetValue(reader.GetOrdinal("id_amm")).ToString();
                            infout.idCorrGlobali = reader.GetValue(reader.GetOrdinal("id_ruolo_creatore")).ToString();
                            infout.idPeople = reader.GetValue(reader.GetOrdinal("author")).ToString();
                            infout.idGruppo = reader.GetValue(reader.GetOrdinal("id_gruppo")).ToString();
                            string system_id = reader.GetValue(reader.GetOrdinal("system_id")).ToString();
                            string version_id = reader.GetValue(reader.GetOrdinal("version_id")).ToString();

                            //GETdettaglio no sec
                            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
                            DocsPaVO.documento.SchedaDocumento schedaDoc = new DocsPaVO.documento.SchedaDocumento();
                            schedaDoc = doc.GetDettaglioNoSecurity(infout, system_id, system_id);

                            DocsPaDocumentale_DOCUMENTUM.Documentale.DocumentManager docs = new DocumentManagerDOCUMENTUM(infout);

                         
                            DocsPaDB.Query_DocsPAWS.Utenti ut = new DocsPaDB.Query_DocsPAWS.Utenti();
                            DocsPaVO.utente.Ruolo ruo = ut.GetRuolo(infout.idCorrGlobali);

                            ruo.idGruppo = infout.idGruppo;
                            ruo.systemId = infout.idCorrGlobali;

                            for (int i = 0; i < schedaDoc.documenti.Count; i++)
                            {
                                DocsPaVO.documento.Documento ver = schedaDoc.documenti[i] as DocsPaVO.documento.Documento;
                                if(ver.versionId.Equals(version_id))
                                {

                                    if (docs.AddVersion(ver, false))
                                    {
                                        AddFileDCMT(system_id, ver, infout);
                                    }
                                    else
                                    {
                                        logger.Debug("PiTreMigration Errore AddVersion del documento principale - DOCNUMBER: " + system_id + "VERSION_ID: " + ver.versionId);
                                        throw new Exception();
                                    }


                                }
                            }

                            


                            //
                            // allineamento Security con ACL Dctm
                            DocsPaDocumentale_DOCUMENTUM.DctmServices.Custom.AclDefinition aclData =
                                DocsPaDocumentale_DOCUMENTUM.DctmServices.Dfs4DocsPa.getAclDefinition(system_id, DocsPaDocumentale_DOCUMENTUM.DocsPaObjectTypes.ObjectTypes.DOCUMENTO, infout);

                            //Aggiornamento ACL in DCTM, con le credenziali di superuser
                            DocsPaDocumentale_DOCUMENTUM.DctmServices.Custom.IAclService aclService =
                                DocsPaDocumentale_DOCUMENTUM.DctmServices.DctmServiceFactory.GetCustomServiceInstance<DocsPaDocumentale_DOCUMENTUM.DctmServices.Custom.IAclService>(infout.dst);

                            aclService.ClearAndGrant(aclData, system_id);



                            //Acquisizione Immagini

                            ////DOC PRINC
                            //DocsPaDocumentale_ETDOCS.Documentale.DocumentManager etdocs = new DocumentManagerETDOCS(infout);
                            //DocsPaVO.documento.FileDocumento filedoc = new DocsPaVO.documento.FileDocumento();
                            //DocsPaVO.documento.FileRequest fileRequest = new FileRequest();

                            ////TODO: fare versioni scorrendo schedadocumenti dal max allo zero..

                            //fileRequest.versionId = ((DocsPaVO.documento.FileRequest)schedaDoc.documenti[0]).versionId;
                            //fileRequest.version = ((DocsPaVO.documento.FileRequest)schedaDoc.documenti[0]).version;
                            //fileRequest.versionLabel = ((DocsPaVO.documento.FileRequest)schedaDoc.documenti[0]).versionLabel;


                            //// filedoc.content=  etdocs.GetFile(system_id, fileRequest.version,fileRequest.versionId,  fileRequest.versionLabel);

                            //bool rtnfile = etdocs.GetFile(ref filedoc, ref fileRequest);

                            //string ext = etdocs.GetFileExtension(system_id, fileRequest.versionId);
                            //DocsPaDocumentale_DOCUMENTUM.Documentale.DocumentManager DCTM = new DocumentManagerDOCUMENTUM(infout);

                            //rtnfile = DCTM.PutFile(fileRequest, filedoc, ext);


                            //for (int i = 0; i < schedaDoc.allegati.Count; i++)
                            //{
                            //    //TODO: fare versioni scorrendo schedaallegati dal max allo zero..


                            //    fileRequest.versionId = ((DocsPaVO.documento.FileRequest)schedaDoc.allegati[i]).versionId;
                            //    fileRequest.version = ((DocsPaVO.documento.FileRequest)schedaDoc.allegati[i]).version;
                            //    fileRequest.versionLabel = ((DocsPaVO.documento.FileRequest)schedaDoc.allegati[i]).versionLabel;


                            //    // filedoc.content=  etdocs.GetFile(system_id, fileRequest.version,fileRequest.versionId,  fileRequest.versionLabel);

                            //    rtnfile = etdocs.GetFile(ref filedoc, ref fileRequest);

                            //    ext = etdocs.GetFileExtension(system_id, fileRequest.versionId);
                            //    DCTM = new DocumentManagerDOCUMENTUM(infout);
                            //    rtnfile = DCTM.PutFile(fileRequest, filedoc, ext);




                            //}




                            //fascicolazione , se non è stato creato fasc va prima creato il fasc..
                            //using (DocsPaDB.DBProvider dbpf = new DocsPaDB.DBProvider())
                            //{
                            //    string cmdf = "select  project_id id_folder, p2.system_id id_fascicolo from  project_components pc, project p,project p2 where link=168390371 and pc.project_id=p.system_id and p2.system_id=p.id_fascicolo";

                            //    using (System.Data.IDataReader readerf = dbpf.ExecuteReader(cmd))
                            //    {
                            //        while (reader.Read())
                            //        {

                            //            string id_fascicolo = reader.GetValue(reader.GetOrdinal("id_fascicolo")).ToString();
                            //            string id_folder = reader.GetValue(reader.GetOrdinal("id_folder")).ToString();
                            //            //si presume che i fascicoli siano tutti già stati creati e migrati su DCTM.
                            //            DocsPaDocumentale_DOCUMENTUM.Documentale.ProjectManager pjman = new DocsPaDocumentale_DOCUMENTUM.Documentale.ProjectManager(infout);
                            //            bool rtnf = pjman.AddDocumentInFolder(system_id, id_folder);

                            //        }

                            //    }

                            //}


                        }

                        return "";
                    }
                }
                //Aggiunta di allegati creati il 21 a documenti non creati il 21
                using (DocsPaDB.DBProvider dbp = new DocsPaDB.DBProvider())
                {

                    string cmd = "select p.docnumber docnumber, p.id_documento_principale system_id, p.id_ruolo_creatore, p.author ,d.id_gruppo, getidamm(p.author) id_amm from profile p, dpa_corr_globali d where d.system_id=p.id_ruolo_creatore and p.ID_DOCUMENTO_PRINCIPALE is not null and  p.creation_date BETWEEN TO_DATE ('21/07/2014 00:00:00','dd/mm/yyyy HH24:mi:ss') AND TO_DATE ('21/07/2014 23:59:59', 'dd/mm/yyyy HH24:mi:ss') and (select creation_date from profile where system_id = p.id_documento_principale) not BETWEEN TO_DATE ('21/07/2014 00:00:00', 'dd/mm/yyyy HH24:mi:ss') AND TO_DATE ('21/07/2014 23:59:59', 'dd/mm/yyyy HH24:mi:ss')";
                    using (System.Data.IDataReader reader = dbp.ExecuteReader(cmd))
                    {
                        int countOK = 0;
                        while (reader.Read())
                        {

                            // string tipo = reader.GetValue(reader.GetOrdinal("cha_tipo_proto")).ToString();
                            //creo infout
                            InfoUtente infout = new InfoUtente();
                            DocsPaDocumentale_DOCUMENTUM.Documentale.UserManager usr = new DocsPaDocumentale_DOCUMENTUM.Documentale.UserManager();
                            infout.dst = usr.GetSuperUserAuthenticationToken();
                            infout.idAmministrazione = reader.GetValue(reader.GetOrdinal("id_amm")).ToString();

                            infout.idCorrGlobali = reader.GetValue(reader.GetOrdinal("id_ruolo_creatore")).ToString();

                            infout.idPeople = reader.GetValue(reader.GetOrdinal("author")).ToString();

                            infout.idGruppo = reader.GetValue(reader.GetOrdinal("id_gruppo")).ToString();
                            string system_id = reader.GetValue(reader.GetOrdinal("system_id")).ToString();
                            string docnumber = reader.GetValue(reader.GetOrdinal("docnumber")).ToString();

                            //GETdettaglio no sec
                            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
                            DocsPaVO.documento.SchedaDocumento schedaDoc = new DocsPaVO.documento.SchedaDocumento();
                            schedaDoc = doc.GetDettaglioNoSecurity(infout, system_id, system_id);

                            DocsPaDocumentale_DOCUMENTUM.Documentale.DocumentManager docs = new DocumentManagerDOCUMENTUM(infout);


                            DocsPaDB.Query_DocsPAWS.Utenti ut = new DocsPaDB.Query_DocsPAWS.Utenti();

                            for (int i = 0; i < schedaDoc.allegati.Count; i++)
                            {
                                if ((schedaDoc.allegati[i] as DocsPaVO.documento.Allegato).docNumber.Equals(docnumber))
                                {
                                    //Dobbiamo creare l'allegato
                                    DocsPaVO.documento.Allegato all = schedaDoc.allegati[i] as DocsPaVO.documento.Allegato;
                                    string putFile = "Y";
                                    if (docs.AddAttachment(all, putFile))
                                    {
                                        SchedaDocumento schedaDocAll = doc.GetDettaglioNoSecurity(infout, all.docNumber, all.docNumber);
                                        if (schedaDocAll != null)
                                        {
                                            DocsPaVO.documento.Documento ver = new Documento();
                                            if (schedaDocAll.documenti.Count > 1)
                                            {
                                                int lenght = schedaDocAll.documenti.Count;
                                                ver = schedaDocAll.documenti[lenght - 1] as DocsPaVO.documento.Documento;
                                                AddFileDCMT(schedaDocAll.docNumber, ver, infout);

                                                for (int j = schedaDocAll.documenti.Count - 2; j >= 0; j--)
                                                {
                                                    ver = schedaDocAll.documenti[j] as DocsPaVO.documento.Documento;
                                                    if (docs.AddVersion(ver, false))
                                                    {
                                                        AddFileDCMT(schedaDocAll.docNumber, ver, infout);
                                                    }
                                                    else
                                                    {
                                                        logger.Debug("PiTreMigration Errore AddVersion dell'allegato - DOCNUMBER: " + system_id + "VERSION_ID: " + ver.versionId + "COUNT: " + countOK++);
                                                        throw new Exception();
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                ver = schedaDocAll.documenti[0] as DocsPaVO.documento.Documento;
                                                AddFileDCMT(schedaDocAll.docNumber, ver, infout);
                                            }
                                        }
                                        else
                                        {
                                            logger.Debug("PiTreMigration Errore GetDettaglioNoSecurity Allegato - DOCNUMBER: " + all.docNumber + "COUNT: " + countOK++);
                                            throw new Exception();
                                        }
                                    }
                                }
                                countOK++;
                            }
                        }
                        logger.Debug("PiTreMigration TOTALE allegati AGGIUNTI correttamente: " + countOK++);
                        return "";
                    }
                }

                //Aggiunta delle versioni create il 21 ad allegati non creati il 21
                using (DocsPaDB.DBProvider dbp = new DocsPaDB.DBProvider())
                {

                    string cmd = "select distinct(v.version_id) version_id, v.docnumber system_id, p.id_ruolo_creatore, p.author ,d.id_gruppo, getidamm(p.author) id_amm from versions v join profile p on v.DOCNUMBER = p.system_id, dpa_corr_globali d where  d.system_id=p.id_ruolo_creatore and p.ID_DOCUMENTO_PRINCIPALE is not null and v.dta_creazione BETWEEN TO_DATE ('21/07/2014 00:00:00','dd/mm/yyyy HH24:mi:ss') AND TO_DATE ('21/07/2014 23:59:59','dd/mm/yyyy HH24:mi:ss')and p.creation_date not BETWEEN TO_DATE ('21/07/2014 00:00:00', 'dd/mm/yyyy HH24:mi:ss') AND TO_DATE ('21/07/2014 23:59:59', 'dd/mm/yyyy HH24:mi:ss')  order by v.version_id asc";

                    using (System.Data.IDataReader reader = dbp.ExecuteReader(cmd))
                    {
                        int countOK = 0;
                        while (reader.Read())
                        {

                            // string tipo = reader.GetValue(reader.GetOrdinal("cha_tipo_proto")).ToString();
                            //creo infout
                            InfoUtente infout = new InfoUtente();
                            DocsPaDocumentale_DOCUMENTUM.Documentale.UserManager usr = new DocsPaDocumentale_DOCUMENTUM.Documentale.UserManager();
                            infout.dst = usr.GetSuperUserAuthenticationToken();
                            infout.idAmministrazione = reader.GetValue(reader.GetOrdinal("id_amm")).ToString();

                            infout.idCorrGlobali = reader.GetValue(reader.GetOrdinal("id_ruolo_creatore")).ToString();

                            infout.idPeople = reader.GetValue(reader.GetOrdinal("author")).ToString();

                            infout.idGruppo = reader.GetValue(reader.GetOrdinal("id_gruppo")).ToString();
                            string system_id = reader.GetValue(reader.GetOrdinal("system_id")).ToString();
                            string version_id = reader.GetValue(reader.GetOrdinal("version_id")).ToString();

                            //GETdettaglio no sec
                            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
                            DocsPaVO.documento.SchedaDocumento schedaDoc = new DocsPaVO.documento.SchedaDocumento();
                            schedaDoc = doc.GetDettaglioNoSecurity(infout, system_id, system_id);

                            DocsPaDocumentale_DOCUMENTUM.Documentale.DocumentManager docs = new DocumentManagerDOCUMENTUM(infout);


                            DocsPaDB.Query_DocsPAWS.Utenti ut = new DocsPaDB.Query_DocsPAWS.Utenti();

                            for (int i = 0; i < schedaDoc.documenti.Count; i++)
                            {
                                DocsPaVO.documento.Documento ver = schedaDoc.documenti[i] as DocsPaVO.documento.Documento;
                                if (ver.versionId.Equals(version_id))
                                {

                                    if (docs.AddVersion(ver, false))
                                    {
                                        AddFileDCMT(system_id, ver, infout);
                                    }
                                    else
                                    {
                                        logger.Debug("PiTreMigration Errore AddVersion del documento principale - DOCNUMBER: " + system_id + "VERSION_ID: " + ver.versionId + "COUNT: "+ countOK++);
                                        throw new Exception();
                                    }


                                }
                            }
                            countOK++;
                        }
                        logger.Debug("PiTreMigration TOTALE allegati AGGIUNTI correttamente: " + countOK++);
                        return "";
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.StackTrace + " " + ex.Message);
                return "";
            }

            finally
            { }
          


            
        }



        private static void AddFileDCMT(string docnumber, DocsPaVO.documento.Documento ver, InfoUtente infout)
        {
            DocsPaDocumentale_ETDOCS.Documentale.DocumentManager etdocs = new DocumentManagerETDOCS(infout);
            DocsPaDocumentale_DOCUMENTUM.Documentale.DocumentManager DCTM = new DocumentManagerDOCUMENTUM(infout);
            DocsPaVO.documento.FileDocumento filedoc = new DocsPaVO.documento.FileDocumento();
            DocsPaVO.documento.FileRequest fileRequest = new FileRequest();
            string ext;
            try
            {
                fileRequest = ver;

                if (!etdocs.GetFile(ref filedoc, ref fileRequest))
                {
                    logger.Debug("PiTreMigration.AddFileDCMT.GetFile - DOCNUMBER: " + docnumber + "VERSION_ID: " + ver.versionId);
                    throw new Exception();
                }
                if (filedoc != null)
                {
                
                    ext =  System.IO.Path.GetExtension((fileRequest.fileName).ToLowerInvariant());

                    if (!DCTM.PutFile(fileRequest, filedoc, ext))
                    {
                        logger.Debug("PiTreMigration.AddFileDCMT.PutFile - DOCNUMBER: " + docnumber + "VERSION_ID: " + ver.versionId);
                        throw new Exception();
                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug("PiTreMigration.AddFileDCMT - DOCNUMBER: " + docnumber);
                throw new Exception();
            }
        }
    }

}
