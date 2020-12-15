using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Data.OleDb;
using BusinessLogic.Import;
using DocsPaVO.documento;
using DocsPaVO.utente;

namespace BusinessLogic.Formazione
{
    public class UnitaOrganizzativaManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(UnitaOrganizzativaManager));

        public static bool PulisciUnitaOrganizzativa(string idUo, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Debug("INIZIO PulisciUnitaOrganizzativa, Utente: " + infoUtente.userId);
            bool result = true;
            try
            {
                DocsPaDB.Query_DocsPAWS.Formazione.UnitaOrganizzativa uo = new DocsPaDB.Query_DocsPAWS.Formazione.UnitaOrganizzativa();
                result = uo.PulisciUnitaOrganizzativa(idUo, infoUtente);
            }
            catch(Exception e)
            {
                result = false;
                logger.Error("Errore in PulisciUnitaOrganizzativa: " + e.Message);
            }
            return result;
            logger.Debug("FINE PulisciUnitaOrganizzativa, Utente: " + infoUtente.userId);
        }

        public static bool PopolaUnitaOrganizzativa(string idUO, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Debug("INIZIO PopolaUnitaOrganizzativa, Utente: " + infoUtente.userId);

            bool result = true;
            string connectionString = String.Empty;         
            OleDbConnection oleConnection = null;        
            OleDbCommand oleCommand = null;
            OleDbDataReader dataReader = null;
            Dictionary<string, string> docPrinc = new Dictionary<string, string>();
            try
            {
                #region Caricamento file di import
                String _rootPath = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_TEMP_PATH");
                //test
                // _rootPath = @"C:\_ROOT\PiTre\temp";
                // fine test

                string codiceUO = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(idUO).codiceRubrica;
                string codiceAmm = BusinessLogic.Amministrazione.AmministraManager.GetCodAmmById(infoUtente.idAmministrazione); 
                string pathFormazione = _rootPath + @"\Formazione\" + codiceAmm + @"\" + codiceUO +  @"\importDocumentiUO.xls";
                string provider = System.Configuration.ConfigurationManager.AppSettings["DS_PROVIDER"];
                string extendedProperty = System.Configuration.ConfigurationManager.AppSettings["DS_EXTENDED_PROPERTIES"];

                try
                {
                    oleConnection = ImportUtils.ConnectToFile(provider, extendedProperty, pathFormazione);
                }
                catch(Exception e)
                {
                    throw new Exception("Errore di connessione al file xls: " + e.Message);
                }

                #endregion

                #region Creazione documenti da file

                #region NON PROTOCOLLATI
                try
                { 
                    oleCommand = new OleDbCommand("SELECT * FROM [NON PROTOCOLLATI$]", oleConnection);
                    dataReader = oleCommand.ExecuteReader();
                }
                catch (Exception e)
                {
                    dataReader.Close();
                    throw new Exception("Errore durante il recupero dei dati sui documenti non protocollato: " + e.Message);
                }

                while (dataReader.Read())
                {
                    string ordinale = dataReader["Ordinale"].ToString();
                    if(String.IsNullOrEmpty(ordinale)) { break;  } // aggiunto controllo perchè iterava anche su righe vuote
                    string idDoc = CreaDocumento(dataReader, codiceUO, codiceAmm);
                    

                    docPrinc.Add(ordinale, idDoc);
                }

                #endregion
                #region ALLEGATI
                try
                {
                    oleCommand = new OleDbCommand("SELECT * FROM [ALLEGATI$]", oleConnection);
                    dataReader = oleCommand.ExecuteReader();
                }
                catch (Exception e)
                {
                    dataReader.Close();
                    throw new Exception("Errore durante il recupero dei dati su allegati: " + e.Message);
                }
                string idDocumentoPrincipale;
                while (dataReader.Read())
                {
                    if (docPrinc.ContainsKey(dataReader["Ordinale Principale"].ToString()))
                    {
                        idDocumentoPrincipale = docPrinc[dataReader["Ordinale Principale"].ToString()];
                        CreaAllegato(dataReader, idDocumentoPrincipale, codiceUO, codiceAmm);
                    }
                }
                #endregion

                #endregion

                try
                {
                    oleConnection.Close();
                }
                catch (Exception e)
                {
                    throw new Exception("Errore chiusura connesione xls: " + e.Message);                   
                }
            }
            catch (Exception e)
            {
                result = false;
                logger.Error("Errore in PopolaUnitaOrganizzativa: " + e.Message);
            }
            return result;
            logger.Debug("FINE PopolaUnitaOrganizzativa, Utente: " + infoUtente.userId);
        }

        private static string CreaDocumento(OleDbDataReader row, string codiceUo, string codiceAmministrazione)
        {
            SchedaDocumento schedaDoc;
            InfoUtente infoUtenteCreatore;
            InfoUtente infoUtenteAreaLavoro;
            Utente utenteCreatore;
            Utente utenteAreaLavore;
            Ruolo ruoloCreatore;
            Registro registro;
            string msg = string.Empty;
            string docnumber = string.Empty;
            try
            {
                string codiceUtenteCreatore = row["Codice utente creatore"].ToString();
                string codiceRuoloCreatore = row["Codice ruolo creatore"].ToString();
                string codiceAmm = row["Codice Amministrazione"].ToString();
                string tipoDocumento = row["Tipo"].ToString();
                string codiceFascicolo = row["Codice fascicolo"].ToString();
                string codiceRegistro = row["Codice Registro"].ToString();
                string nomeFile = row["Pathname"].ToString();

                // correzione virgole
                codiceFascicolo = codiceFascicolo.Replace(",", ".");

                utenteCreatore = BusinessLogic.Utenti.UserManager.getUtenteByCodice(codiceUtenteCreatore, codiceAmm);
                ruoloCreatore = BusinessLogic.Utenti.UserManager.getRuoloByCodice(codiceRuoloCreatore);
                ruoloCreatore = BusinessLogic.Utenti.UserManager.getRuoloById(ruoloCreatore.systemId);
                infoUtenteCreatore = BusinessLogic.Utenti.UserManager.GetInfoUtente(utenteCreatore, ruoloCreatore);
                registro = BusinessLogic.Utenti.RegistriManager.getRegistroByCode(codiceRegistro);
                
                schedaDoc = Documenti.DocManager.NewSchedaDocumento(infoUtenteCreatore);
                schedaDoc.repositoryContext = null;
                switch(tipoDocumento)
                {
                    case "NP":
                        schedaDoc.tipoProto = "G";
                        schedaDoc.oggetto = new Oggetto();
                        schedaDoc.oggetto.descrizione = row["Oggetto"].ToString();
                        schedaDoc = BusinessLogic.Documenti.DocSave.addDocGrigia(schedaDoc, infoUtenteCreatore, ruoloCreatore);
                        BusinessLogic.UserLog.UserLog.WriteLog(infoUtenteCreatore, "DOCUMENTOADDDOCGRIGIA", schedaDoc.systemId, string.Format("{0} {1}", "N.ro Doc.: ", schedaDoc.docNumber), DocsPaVO.Logger.CodAzione.Esito.OK);
                        break;
                }
                if (schedaDoc != null && !string.IsNullOrEmpty(schedaDoc.systemId))
                {
                    docnumber = schedaDoc.systemId;

                    string codiceUtenteAreaLavoro = row["Codice utente ADL"].ToString();
                    if (!string.IsNullOrEmpty(codiceUtenteAreaLavoro))
                    {
                        utenteAreaLavore = BusinessLogic.Utenti.UserManager.getUtenteByCodice(codiceUtenteAreaLavoro, codiceAmm);
                        infoUtenteAreaLavoro = BusinessLogic.Utenti.UserManager.GetInfoUtente(utenteAreaLavore, ruoloCreatore);
                        BusinessLogic.Documenti.areaLavoroManager.execAddLavoroMethod(docnumber, schedaDoc.tipoProto, registro.systemId, infoUtenteAreaLavoro, null);
                    }

                    if(!string.IsNullOrEmpty(nomeFile))
                        AcquisisciDocumento((FileRequest)schedaDoc.documenti[0], nomeFile, codiceUo, codiceAmministrazione, infoUtenteCreatore);
                    
                    if (!string.IsNullOrEmpty(codiceFascicolo))
                    {
                        string idAmministrazione = infoUtenteCreatore.idAmministrazione;
                        string idTitolarioAttivo = Fascicoli.TitolarioManager.GetIdTitolarioAttivo(idAmministrazione);
                        DocsPaVO.fascicolazione.Fascicolo fascicolo = Fascicoli.FascicoloManager.getFascicoloDaCodice2(
                            idAmministrazione,
                            ruoloCreatore.idGruppo,
                            infoUtenteCreatore.idPeople,
                            codiceFascicolo,
                            registro,
                            true,
                            true,
                            idTitolarioAttivo);
                        
                        BusinessLogic.Fascicoli.FascicoloManager.addDocFascicolo(
                            infoUtenteCreatore,
                            schedaDoc.systemId,
                            fascicolo.systemID,
                            true,
                            out msg);
                    }
                }
                
            }
            catch(Exception e)
            {
                logger.Error(e.Message, e);
                schedaDoc = null;
            }
            return docnumber;
        }

        private static void CreaAllegato(OleDbDataReader row, string idDocumentoPrincipale, string codiceUo, string codiceAmministrazione)
        {
            Allegato allegato = new Allegato();
            InfoUtente infoUtenteCreatore;
            Utente utenteCreatore;
            Ruolo ruoloCreatore;
            try
            {
                string codiceUtenteCreatore = row["Codice utente creatore"].ToString();
                string codiceRuoloCreatore = row["Codice ruolo creatore"].ToString();
                string codiceAmm = row["Codice Amministrazione"].ToString();
                string nomeFile = row["Pathname"].ToString();

                utenteCreatore = BusinessLogic.Utenti.UserManager.getUtenteByCodice(codiceUtenteCreatore, codiceAmm);
                ruoloCreatore = BusinessLogic.Utenti.UserManager.getRuoloByCodice(codiceRuoloCreatore);
                infoUtenteCreatore = BusinessLogic.Utenti.UserManager.GetInfoUtente(utenteCreatore, ruoloCreatore);

                allegato.descrizione = row["Descrizione"].ToString();
                allegato.docNumber = idDocumentoPrincipale;
                allegato = BusinessLogic.Documenti.AllegatiManager.aggiungiAllegato(infoUtenteCreatore, allegato);
                if(allegato != null && !string.IsNullOrEmpty(allegato.docNumber))
                {
                    AcquisisciDocumento((FileRequest)allegato, nomeFile, codiceUo, codiceAmministrazione, infoUtenteCreatore, true);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Errore in creazione allegato: " + e.Message);
            }
        }

        private static bool AcquisisciDocumento(FileRequest fileRequest, string nomeFile, string codiceUO, string codiceAmm, InfoUtente infoUtente, bool isAllegato = false)
        {
            bool result = true;
            byte[] fileContent;
            String _rootPath = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_TEMP_PATH");
            //test
            // _rootPath = @"C:\_ROOT\PiTre\temp";
            // fine test

            string pathFormazione = _rootPath + @"\Formazione\" + codiceAmm + @"\" + codiceUO + @"\File\";
            if (isAllegato)
            {
                pathFormazione = System.IO.Path.Combine(pathFormazione, "allegati");
            }

            string filePath = System.IO.Path.Combine(pathFormazione, nomeFile);
            try
            {
                fileContent = System.IO.File.ReadAllBytes(filePath);
            }
            catch(Exception e )
            {
                result = false;
                throw new Exception("Errore nel recupero del file: " + e.Message);
            }

            FileDocumento fileDocumento = new FileDocumento();
            fileDocumento.name = nomeFile;
            fileDocumento.length = fileContent.Length;
            fileDocumento.content = fileContent;
            try
            {
               BusinessLogic.Documenti.FileManager.putFile(fileRequest, fileDocumento, infoUtente);
            }
            catch (Exception e)
            {
                result = false;
                throw new Exception("Errore durante l'acquisizione del file.");
            }

            return result;
        }
    }
}
