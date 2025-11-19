// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using BusinessLogic.Fascicoli;
using BusinessLogic.Import;
using BusinessLogic.ProfilazioneDinamica;
using BusinessLogic.Utenti;
using DocsPaVO;
using DocsPaVO.amministrazione;
using DocsPaVO.fascicolazione;
using DocsPaVO.ProfilazioneDinamica;
using DocsPaVO.utente;
using DocumentFormat.OpenXml.Spreadsheet;
using Pi3.Core.Services.File.Spreadsheet;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.PianoConservazione
{
    public class PianoConservazioneManager
    {
        private static ILogger logger = Serilog.Log.ForContext(typeof(PianoConservazioneManager));

        public static bool IsPianoConservazioneAcquisito(string idTitolario, string idRegistro, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool retValue = true;
            DocsPaDB.Query_DocsPAWS.PianoConservazione piano = new DocsPaDB.Query_DocsPAWS.PianoConservazione();

            try
            {
                retValue = piano.IsPianoConservazioneAcquisito(idTitolario, idRegistro);
            }
            catch (Exception e)
            {
                logger.Error("Errore in GetPianoConservazione: " + e.Message);
            }

            return retValue;
        }

        public static List<DocsPaVO.PianoConservazione> GetPianoConservazioneByIdClassificazione(string idClassificazione, DocsPaVO.utente.InfoUtente infoUtente)
        {
            List<DocsPaVO.PianoConservazione> pianoConservazione = new List<DocsPaVO.PianoConservazione>();
            DocsPaDB.Query_DocsPAWS.PianoConservazione piano = new DocsPaDB.Query_DocsPAWS.PianoConservazione();

            try
            {
                pianoConservazione = piano.GetPianoConservazioneByIdClassificazione(idClassificazione);
            }
            catch (Exception e)
            {
                logger.Error("Errore in GetPianoConservazioneByIdClassificazione: " + e.Message);
            }

            return pianoConservazione;
        }

        public static List<PianoCons_IntegrPIS> SearchPianoCons_IntegrPIS(string id, string codClassifica, string idPeopleInt, string idGruppoInt, string codeApplication, string idTipoDoc, string idTipoFasc, string pagenum, string pagesize, string orderColumn, string orderDirection, string id_amm)
        {
            Dictionary<string, string> filters = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(id)) filters.Add("ID", id);
            if (!string.IsNullOrWhiteSpace(codClassifica)) filters.Add("CODICE_CLASSIFICA", codClassifica);
            if (!string.IsNullOrWhiteSpace(idPeopleInt)) filters.Add("ID_PEOPLE", idPeopleInt);
            if (!string.IsNullOrWhiteSpace(idGruppoInt)) filters.Add("ID_GRUPPO", idGruppoInt);
            if (!string.IsNullOrWhiteSpace(codeApplication)) filters.Add("CODE_APPLICATION", codeApplication);
            if (!string.IsNullOrWhiteSpace(idTipoDoc)) filters.Add("ID_TIPO_DOC", idTipoDoc);
            if (!string.IsNullOrWhiteSpace(idTipoFasc)) filters.Add("ID_TIPO_FASC", idTipoFasc);

            DocsPaDB.Query_DocsPAWS.PianoConservazione piano = new DocsPaDB.Query_DocsPAWS.PianoConservazione();
            return piano.SearchPianoCons_IntegrPIS(filters, pagenum, pagesize, orderColumn, orderDirection, id_amm);
        }

        public static DocsPaVO.PianoConservazione GetPianoConservazioneByIdTipoAtto(string idTipoAtto)
        {
            DocsPaVO.PianoConservazione result = null;

            try
            {
                DocsPaDB.Query_DocsPAWS.PianoConservazione piano = new DocsPaDB.Query_DocsPAWS.PianoConservazione();
                result = piano.GetPianoConservazioneByIdTipoAtto(idTipoAtto);
            }
            catch (Exception e)
            {
                logger.Error("Errore in GetPianoConservazioneByIdTipoAtto: " + e.Message);
            }

            return result;
        }

        public static List<DocsPaVO.PianoConservazione> GetPianoConservazioneByCodiceClassificazione(string codiceClassificazione, Registro registro, string idAmministrazione, DocsPaVO.utente.InfoUtente infoUtente)
        {
            List<DocsPaVO.PianoConservazione> pianoConservazione = new List<DocsPaVO.PianoConservazione>();
            DocsPaDB.Query_DocsPAWS.PianoConservazione piano = new DocsPaDB.Query_DocsPAWS.PianoConservazione();
            string idRegistro = registro != null ? registro.systemId : string.Empty;
            try
            {
                //Estraggo l'id Clssificazione a partire dal codice
                string idClassificazione = string.Empty;
                //ArrayList listaFascicoli = BusinessLogic.Fascicoli.FascicoloManager.getListaFascicoliDaCodice(infoUtente, codiceClassificazione, registro, false, false, "I");
                string idTitolario = BusinessLogic.Fascicoli.TitolarioManager.GetIdTitolarioAttivo(idAmministrazione);
                DocsPaVO.fascicolazione.Fascicolo[] listaFascicoli = BusinessLogic.Fascicoli.FascicoloManager.GetFascicoloDaCodiceNoSecurityConservazione(codiceClassificazione, idAmministrazione, idTitolario, false, true, idRegistro);
                if (listaFascicoli != null && listaFascicoli.Length > 0)
                {
                    idClassificazione = listaFascicoli[0].systemID;
                    pianoConservazione = piano.GetPianoConservazioneByIdClassificazione(idClassificazione);
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in GetPianoConservazioneByIdClassificazione: " + e.Message);
            }

            return pianoConservazione;
        }

        public static DocsPaVO.PianoConservazione GetPianoConservazioneByIdTipoFasc(string idTipoFasc)
        {
            DocsPaVO.PianoConservazione result = null;

            try
            {
                DocsPaDB.Query_DocsPAWS.PianoConservazione piano = new DocsPaDB.Query_DocsPAWS.PianoConservazione();
                result = piano.GetPianoConservazioneByIdTipoFasc(idTipoFasc);
            }
            catch (Exception e)
            {
                logger.Error("Errore in GetPianoConservazioneByIdTipoFasc: " + e.Message);
            }

            return result;
        }

        public static bool InsertPianoConservazioneTipoAtto(string idPianoConservazione, string idTipoAtto, string idAmministrazione)
        {
            bool result = false;

            try
            {
                DocsPaDB.Query_DocsPAWS.PianoConservazione piano = new DocsPaDB.Query_DocsPAWS.PianoConservazione();
                result = piano.InsertPianoConservazioneTipoAtto(idPianoConservazione, idTipoAtto, idAmministrazione);
            }
            catch (Exception e)
            {
                logger.Error("Errore in InsertPianoConservazioneTipoAtto: " + e.Message);
            }

            return result;
        }

        public static bool UpdatePianoConservazioneTipoAtto(string idTipoAtto, string idPianoConservazione)
        {
            bool result = false;

            try
            {
                DocsPaDB.Query_DocsPAWS.PianoConservazione piano = new DocsPaDB.Query_DocsPAWS.PianoConservazione();
                result = piano.UpdatePianoConservazioneTipoAtto(idTipoAtto, idPianoConservazione);
            }
            catch (Exception e)
            {
                logger.Error("Errore in UpdatePianoConservazioneTipoAtto: " + e.Message);
            }

            return result;
        }

        public static bool ImportPianoCons_IntegrPIS(byte[] dati, DocsPaVO.utente.InfoUtente infoUtente,
            ISpreadsheetService spreadsheetService, string serverPath)
        {
            bool error = false;

            try
            {
                SpreadsheetModel model = null;

                if (!Directory.Exists(serverPath + "\\Modelli\\Import\\"))
                    Directory.CreateDirectory(serverPath + "\\Modelli\\Import\\");

                DocsPaDB.Utils.SimpleLog sl = new DocsPaDB.Utils.SimpleLog(serverPath + "\\Modelli\\Import\\logImportazioneTabellaConfine");

                sl.Log("**** Inizio importazione tabella di confine - " + System.DateTime.Now.ToString());

                //OleDbCommand xlsCmd = new OleDbCommand("select * from [Tabella di confine$]", xlsConn);
                //xlsReader = xlsCmd.ExecuteReader();
                try
                {
                    var stream = new MemoryStream(dati);
                    model = spreadsheetService.Read(stream).Result;
                }
                catch (Exception e)
                {
                    logger.Error("Errore durante la lettura del file. Dettaglio eccezione: " + e.Message);
                    return false;
                }


                string operazione = string.Empty;

                string ordinale = string.Empty;
                string azione = string.Empty;
                string codiceAmministrazione = string.Empty;
                string idAmministrazione = string.Empty;
                Registro registro = null;
                string idTitolarioAttivo = string.Empty;
                Fascicolo project = null;
                string idPeople = string.Empty;
                string idGruppo = string.Empty;
                string idPianoConservazione = string.Empty;
                string idTipoAtto = string.Empty;
                string idTipoFasc = string.Empty;
                DocsPaDB.Query_DocsPAWS.PianoConservazione piano = new DocsPaDB.Query_DocsPAWS.PianoConservazione();

                // La lista dei template
                Templates[] templates = null;

                Templates[] templatesFasc = null;

                DocsPaVO.PianoCons_IntegrPIS pianoCons_IntegrPIS = null;

                foreach (var sheetModel in model.Sheets)
                {
                    int row = 1;
                    var cellToCheck = new List<int>() { 0, 1, 2, 3 };


                    while (true)
                    {
                        var rowCells = sheetModel.Cells.Where(itm => itm.Row == row).ToList();

                        if (rowCells != null && rowCells.Any())
                        {
                            // check campi indispensabili
                            var toCheck = rowCells.Where(row => cellToCheck.Contains(row.Column) && string.IsNullOrWhiteSpace(row.ValueAsString)).ToList();
                            if (toCheck == null || !toCheck.Any())
                            {

                                try
                                {
                                    var xlsAzione = rowCells.Where(cell => cell.Column == 1)?.FirstOrDefault()?.ValueAsString;

                                    if (xlsAzione != null && !string.IsNullOrEmpty(xlsAzione.ToString()))
                                    {
                                        azione = xlsAzione.ToUpper();
                                        switch (azione)
                                        {
                                            case "I":
                                                operazione = "inserimento";
                                                break;
                                            case "M":
                                                operazione = "modifica";
                                                break;
                                            case "C":
                                                operazione = "cancellazione";
                                                break;
                                            default:
                                                throw new Exception("Campo AZIONE non valido");
                                                break;
                                        }
                                    }

                                    #region Controllo campi obbligatori

                                    //ORDINALE 
                                    if (rowCells.Where(cell => cell.Column == 0)?.FirstOrDefault()?.ValueAsString == null)
                                        throw new Exception("Campo obbligatorio ORDINALE non inserito nel modello");

                                    //AZIONE
                                    if (rowCells.Where(cell => cell.Column == 1)?.FirstOrDefault()?.ValueAsString == null)
                                        throw new Exception("Campo obbligatorio AZIONE non inserito nel modello");

                                    //REGISTRO
                                    if (rowCells.Where(cell => cell.Column == 2)?.FirstOrDefault()?.ValueAsString == null)
                                        throw new Exception("Campo obbligatorio REGISTRO non inserito nel modello");

                                    //ENTE
                                    if (rowCells.Where(cell => cell.Column == 3)?.FirstOrDefault()?.ValueAsString == null)
                                        throw new Exception("Campo obbligatorio ENTE non inserito nel modello");

                                    //DESCRIZIONE INTEGRAZIONE
                                    if (rowCells.Where(cell => cell.Column == 4)?.FirstOrDefault()?.ValueAsString == null)
                                        throw new Exception("Campo obbligatorio DESCRIZIONE INTEGRAZIONE non inserito nel modello");

                                    //CODE APPLICATION
                                    if (rowCells.Where(cell => cell.Column == 5)?.FirstOrDefault()?.ValueAsString == null)
                                        throw new Exception("Campo obbligatorio CODE APPLICATION non inserito nel modello");

                                    //CLASSIFICAZIONE
                                    if (rowCells.Where(cell => cell.Column == 6)?.FirstOrDefault()?.ValueAsString == null)
                                        throw new Exception("Campo obbligatorio CLASSIFICAZIONE non inserito nel modello");

                                    //TIPOLOGIA FASCICOLO
                                    if (rowCells.Where(cell => cell.Column == 7)?.FirstOrDefault()?.ValueAsString == null)
                                        throw new Exception("Campo obbligatorio TIPOLOGIA FASCICOLO non inserito nel modello");

                                    #endregion

                                    ordinale = rowCells.Where(cell => cell.Column == 0)?.FirstOrDefault()?.ValueAsString;

                                    Log.Information(string.Format("{0} tabella di confine per ordinale {1}", new object[] { operazione, ordinale }));

                                    sl.Log("");
                                    sl.Log(string.Format("{0} tabella di confine per ordinale {1}", new object[] { operazione, ordinale }));

                                    if (!codiceAmministrazione.Equals(rowCells.Where(cell => cell.Column == 2)?.FirstOrDefault()?.ValueAsString))
                                    {
                                        codiceAmministrazione = rowCells.Where(cell => cell.Column == 2)?.FirstOrDefault()?.ValueAsString;
                                        idAmministrazione = new DocsPaDB.Query_DocsPAWS.Amministrazione().GetIDAmm(codiceAmministrazione);

                                        if (string.IsNullOrEmpty(idAmministrazione))
                                            throw new Exception("Amministrazione " + codiceAmministrazione + " non trovata");

                                        // Prelevamento della lista dei template creati per l'amministrazione
                                        templates = (Templates[])ProfilazioneDocumenti.getTemplates(idAmministrazione).ToArray(typeof(Templates));

                                        templatesFasc = (Templates[])ProfilazioneFascicoli.getTemplatesFasc(idAmministrazione).ToArray(typeof(Templates));

                                        //Estrazione del titolario attivo per l'amministrazione indicata
                                        idTitolarioAttivo = BusinessLogic.Fascicoli.TitolarioManager.GetIdTitolarioAttivo(idAmministrazione);
                                    }

                                    //REGISTRO
                                    if (!string.IsNullOrWhiteSpace(rowCells.Where(cell => cell.Column == 3)?.FirstOrDefault()?.ValueAsString))
                                    {
                                        //Estraggo l'id del registro indicato
                                        registro = RegistriManager.getRegistroByCode(rowCells.Where(cell => cell.Column == 3)?.FirstOrDefault()?.ValueAsString);

                                        if (registro == null || string.IsNullOrEmpty(registro.systemId))
                                            throw new Exception("Registro " + rowCells.Where(cell => cell.Column == 3)?.First().ValueAsString + " non trovato");
                                    }

                                    //Classificazione
                                    if (!string.IsNullOrWhiteSpace(rowCells.Where(cell => cell.Column == 6)?.FirstOrDefault()?.ValueAsString))
                                    {
                                        //Estraggo l'id del codice classifica indicato
                                        project = FascicoloManager.GetFascicoloDaCodiceNoSecurity(rowCells.Where(cell => cell.Column == 6)?.First().ValueAsString, idAmministrazione, idTitolarioAttivo, true)[0];

                                        if (project == null || string.IsNullOrEmpty(project.systemID))
                                            throw new Exception("Classificazione " + rowCells.Where(cell => cell.Column == 6)?.First().ValueAsString + " non trovata");

                                    }

                                    //ID_PIANO_CONSERVAZIONE
                                    if (!string.IsNullOrWhiteSpace(rowCells.Where(cell => cell.Column == 7)?.FirstOrDefault()?.ValueAsString))
                                    {
                                        List<DocsPaVO.PianoConservazione> pianoConservazione = BusinessLogic.PianoConservazione.PianoConservazioneManager.GetPianoConservazioneByIdClassificazione(project.systemID, infoUtente);
                                        try
                                        {
                                            idPianoConservazione = pianoConservazione.Where(e => e.TipologiaFascicolo.ToUpper() == rowCells.Where(cell => cell.Column == 7)?.First().ValueAsString.ToUpper()).FirstOrDefault().SystemId.ToString();
                                        }
                                        catch (Exception e)
                                        {
                                            throw new Exception(String.Format(
                                                "Non è stato possibile recuperare le informazioni per la tipologia fascicolo {0}",
                                                rowCells.Where(cell => cell.Column == 7)?.First().ValueAsString));
                                        }
                                    }

                                    //ID_PEOPLE
                                    if (!string.IsNullOrWhiteSpace(rowCells.Where(cell => cell.Column == 9)?.FirstOrDefault()?.ValueAsString))
                                    {
                                        DocsPaVO.utente.Utente utente = BusinessLogic.Utenti.UserManager.getUtenteByCodice(rowCells.Where(cell => cell.Column == 9)?.First().ValueAsString, codiceAmministrazione);
                                        if (utente == null)
                                            throw new Exception("Utente " + rowCells.Where(cell => cell.Column == 9)?.First().ValueAsString + " non trovato");
                                        idPeople = utente.idPeople;
                                    }

                                    //ID_GRUPPO
                                    if (!string.IsNullOrWhiteSpace(rowCells.Where(cell => cell.Column == 10)?.FirstOrDefault()?.ValueAsString))
                                    {
                                        DocsPaVO.utente.Ruolo ruolo = BusinessLogic.Utenti.UserManager.getRuoloByCodice(rowCells.Where(cell => cell.Column == 10)?.First().ValueAsString);
                                        if (ruolo == null)
                                            throw new Exception("Ruolo " + rowCells.Where(cell => cell.Column == 10)?.First().ValueAsString + " non trovato");
                                        idGruppo = ruolo.idGruppo;
                                    }

                                    //TIPO DOCUMENTO
                                    if (!string.IsNullOrWhiteSpace(rowCells.Where(cell => cell.Column == 11)?.FirstOrDefault()?.ValueAsString))
                                    {
                                        // Ricerca del template con nome uguale a quello richiesto
                                        try
                                        {
                                            idTipoAtto = templates.Where(e => e.DESCRIZIONE.ToUpper() == rowCells.Where(cell => cell.Column == 11)?.First().ValueAsString.ToUpper()).FirstOrDefault().SYSTEM_ID.ToString();
                                        }
                                        catch (Exception e)
                                        {
                                            throw new Exception(String.Format(
                                                "Non è stato possibile recuperare le informazioni sul template {0}",
                                                rowCells.Where(cell => cell.Column == 11)?.First().ValueAsString));
                                        }
                                    }

                                    //CAMPO PROFILATO FASCICOLO
                                    if (!string.IsNullOrWhiteSpace(rowCells.Where(cell => cell.Column == 12)?.FirstOrDefault()?.ValueAsString))
                                    {
                                        // Ricerca del template con nome uguale a quello richiesto
                                        try
                                        {
                                            idTipoFasc = templatesFasc.Where(e => e.DESCRIZIONE.ToUpper() == rowCells.Where(cell => cell.Column == 12)?.First().ValueAsString.ToUpper()).FirstOrDefault().SYSTEM_ID.ToString();
                                        }
                                        catch (Exception e)
                                        {
                                            throw new Exception(String.Format(
                                                "Non è stato possibile recuperare le informazioni sul template {0}",
                                                rowCells.Where(cell => cell.Column == 12)?.First().ValueAsString));
                                        }
                                    }

                                    pianoCons_IntegrPIS = new PianoCons_IntegrPIS()
                                    {
                                        IdRegistro = registro.systemId,
                                        IdAmministrazione = idAmministrazione,
                                        DescIntegrazione = rowCells.Where(cell => cell.Column == 4)?.First().ValueAsString,
                                        CodeApplication = rowCells.Where(cell => cell.Column == 5)?.First().ValueAsString.ToString(),
                                        CodiceClassifica = rowCells.Where(cell => cell.Column == 6)?.First().ValueAsString.ToString(),
                                        IdPianoConservazione = idPianoConservazione,
                                        IdPeopleIntegrazione = idPeople,
                                        IdGruppoIntegrazione = idGruppo,
                                        IdTipoDoc = idTipoAtto,
                                        IdTipoFasc = idTipoFasc
                                    };

                                    switch (azione)
                                    {
                                        case "I":
                                            if (!InsertPianoCons_IntegrPIS(pianoCons_IntegrPIS))
                                            {
                                                throw new Exception("Errore inserimento");
                                            }

                                            break;
                                        case "M":
                                            var pianoCons_IntegrPIS_existing = SearchPianoCons_IntegrPIS(pianoCons_IntegrPIS.SystemId, pianoCons_IntegrPIS.CodiceClassifica, pianoCons_IntegrPIS.IdPeopleIntegrazione, pianoCons_IntegrPIS.IdGruppoIntegrazione,
                                                 pianoCons_IntegrPIS.CodeApplication, pianoCons_IntegrPIS.IdTipoDoc, pianoCons_IntegrPIS.IdTipoFasc, "", "", "", "", idAmministrazione)[0];
                                            if (pianoCons_IntegrPIS_existing == null || string.IsNullOrEmpty(pianoCons_IntegrPIS_existing.SystemId))
                                                throw new Exception("Nessun risultato trovato");

                                            pianoCons_IntegrPIS.SystemId = pianoCons_IntegrPIS_existing.SystemId;
                                            if (!UpdatePianoCons_IntegrPIS(pianoCons_IntegrPIS))
                                            {
                                                throw new Exception("Errore modifica");
                                            }

                                            break;
                                        case "C":
                                            pianoCons_IntegrPIS = SearchPianoCons_IntegrPIS(pianoCons_IntegrPIS.SystemId, pianoCons_IntegrPIS.CodiceClassifica, pianoCons_IntegrPIS.IdPeopleIntegrazione, pianoCons_IntegrPIS.IdGruppoIntegrazione,
                                                pianoCons_IntegrPIS.CodeApplication, pianoCons_IntegrPIS.IdTipoDoc, pianoCons_IntegrPIS.IdTipoFasc, "", "", "", "", idAmministrazione)[0];
                                            if (pianoCons_IntegrPIS == null || string.IsNullOrEmpty(pianoCons_IntegrPIS.SystemId))
                                                throw new Exception("Nessun risultato trovato");

                                            if (!DeletePianoCons_IntegrPIS(pianoCons_IntegrPIS.SystemId))
                                            {
                                                throw new Exception("Errore cancellazione");
                                            }

                                            break;
                                    }
                                }
                                catch (Exception e)
                                {
                                    sl.Log("");
                                    sl.Log(string.Format("ERRORE {0} Ordinale {1}: " + e.Message, new string[] { operazione, ordinale }));
                                    error = true;
                                }
                                row++;
                            }
                            else
                            {
                                row++;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                sl.Log("");
                logger.Information("**** Fine importazione Tabella di confine - " + System.DateTime.Now.ToString());
            }
            catch (Exception e)
            {
                logger.Error("Errore in ImportPianoCons_IntegrPIS: " + e.Message);
                error = true;
            }
            finally
            {
                //if (xlsReader != null)
                //    xlsReader.Close();

                //if (xlsConn != null)
                //    xlsConn.Close();
            }

            return error;
        }

        public static bool InsertPianoCons_IntegrPIS(PianoCons_IntegrPIS input)
        {
            DocsPaDB.Query_DocsPAWS.PianoConservazione piano = new DocsPaDB.Query_DocsPAWS.PianoConservazione();
            return piano.InsertPianoCons_IntegrPIS(input);
        }

        public static bool UpdatePianoCons_IntegrPIS(PianoCons_IntegrPIS input)
        {
            DocsPaDB.Query_DocsPAWS.PianoConservazione piano = new DocsPaDB.Query_DocsPAWS.PianoConservazione();
            return piano.UpdatePianoCons_IntegrPIS(input);
        }

        public static bool DeletePianoCons_IntegrPIS(string id)
        {
            DocsPaDB.Query_DocsPAWS.PianoConservazione piano = new DocsPaDB.Query_DocsPAWS.PianoConservazione();
            return piano.DeletePianoCons_IntegrPIS(id);
        }

        public static bool InsertPianoConservazioneTipoFasc(string idPianoConservazione, string idTipoFasc, string idAmministrazione)
        {
            bool result = false;

            try
            {
                DocsPaDB.Query_DocsPAWS.PianoConservazione piano = new DocsPaDB.Query_DocsPAWS.PianoConservazione();
                result = piano.InsertPianoConservazioneTipoFasc(idPianoConservazione, idTipoFasc, idAmministrazione);
            }
            catch (Exception e)
            {
                logger.Error("Errore in InsertPianoConservazioneTipoFasc: " + e.Message);
            }

            return result;
        }

        public static DocsPaVO.PianoConservazione GetPianoConservazioneById(string idPianoConservazione, DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaVO.PianoConservazione pianoConservazione = new DocsPaVO.PianoConservazione();
            DocsPaDB.Query_DocsPAWS.PianoConservazione piano = new DocsPaDB.Query_DocsPAWS.PianoConservazione();

            try
            {
                pianoConservazione = piano.GetPianoConservazioneById(idPianoConservazione);
            }
            catch (Exception e)
            {
                logger.Error("Errore in GetPianoConservazioneById: " + e.Message);
            }

            return pianoConservazione;
        }

        public static bool ImportPianoConservazione(byte[] dati, string idTitolario, string idAmm, string idRegistro, DocsPaVO.utente.InfoUtente infoUtente, ISpreadsheetService spreadsheetService, string serverPath)
        {
            bool error = false;
            string tempPath = serverPath; SpreadsheetModel spreadsheetModel = null;

            try
            {
                var stream = new MemoryStream(dati);
                spreadsheetModel = spreadsheetService.Read(stream).Result;
            }
            catch (Exception e)
            {
                logger.Error("Errore durante la lettura del file. Dettaglio eccezione: " + e.Message);
                return false;
            }

            var sheetModel = spreadsheetModel.Sheets.FirstOrDefault(s => s.Name.Equals("Piano conservazione", StringComparison.InvariantCultureIgnoreCase));

            try
            {
                if (!Directory.Exists(tempPath + "\\Modelli\\Import\\"))
                    Directory.CreateDirectory(tempPath + "\\Modelli\\Import\\");

                DocsPaDB.Utils.SimpleLog sl = new DocsPaDB.Utils.SimpleLog(tempPath + "\\Modelli\\Import\\logImportazionePianoConservazione");

                bool insert = false;
                int row = 1;

                //while (xlsReader.Read())
                while (sheetModel.Cells.Where(itm => itm.Row == row && !string.IsNullOrEmpty(itm.ValueAsString)).Any())
                {
                    var rowCells = sheetModel.Cells.Where(itm => itm.Row == row).ToList();
                    if (rowCells != null && rowCells.Any())
                    {
                        try
                        {
                            #region Controllo campi obbligatori
                            //CLASSIFICAZIONE
                            if (!rowCells.Where(itm => itm.Name.StartsWith("A")).Any() || string.IsNullOrEmpty(rowCells.Where(itm => itm.Name.StartsWith("A")).FirstOrDefault()?.ValueAsString))
                                throw new Exception("ERRORE: Campo obbligatorio CLASSIFICAZIONE non inserito nel modello");

                            //TIPOLOGIA FASCICOLO
                            if (!rowCells.Where(itm => itm.Name.StartsWith("D")).Any() || string.IsNullOrEmpty(rowCells.Where(itm => itm.Name.StartsWith("D")).FirstOrDefault()?.ValueAsString))
                                throw new Exception("ERRORE: Campo obbligatorio TIPOLOGIA FASCICOLO non inserito nel modello");

                            //TEMPO DI CONSERVAZIONE
                            if (!rowCells.Where(itm => itm.Name.StartsWith("E")).Any() || string.IsNullOrEmpty(rowCells.Where(itm => itm.Name.StartsWith("E")).FirstOrDefault()?.ValueAsString))
                                throw new Exception("ERRORE: Campo obbligatorio TEMPO DI CONSERVAZIONE non inserito nel modello");

                            #endregion

                            sl.Log("");
                            sl.Log("Inserimento Piano conservazione nodo: " + rowCells.Where(itm => itm.Name.StartsWith("A")).FirstOrDefault().ValueAsString + " e Tipologia fascicolo: " + rowCells.Where(itm => itm.Name.StartsWith("D")).FirstOrDefault().ValueAsString);

                            //Controllo l'esistenza del nodo di titolario e recupero il system_id
                            OrgNodoTitolario nodo = GetNodoTitolario(rowCells.Where(itm => itm.Name.StartsWith("A")).FirstOrDefault().ValueAsString.Trim(), idAmm, idRegistro, idTitolario);
                            if (nodo == null || string.IsNullOrEmpty(nodo.ID))
                                throw new Exception("ERRORE: codice di classificazione " + rowCells.Where(itm => itm.Name.StartsWith("A")).FirstOrDefault().ValueAsString + " non trovato.");

                            DocsPaVO.PianoConservazione pianoConservazione = new DocsPaVO.PianoConservazione();
                            pianoConservazione.IdClassificazione = nodo.ID;
                            pianoConservazione.CodiceClassificazione = nodo.Codice;
                            pianoConservazione.IdTitolario = idTitolario;
                            pianoConservazione.IdAmm = idAmm;
                            pianoConservazione.IdRegistro = idRegistro;
                            pianoConservazione.VoceProcedimento = rowCells.Any(itm => itm.Name.StartsWith("B")) ? rowCells.Where(itm => itm.Name.StartsWith("B")).FirstOrDefault().ValueAsString : string.Empty;
                            pianoConservazione.NumeroProcedimento = rowCells.Any(itm => itm.Name.StartsWith("C")) ? rowCells[2].ValueAsString : string.Empty;
                            pianoConservazione.TipologiaFascicolo = rowCells.Where(itm => itm.Name.StartsWith("D")).FirstOrDefault().ValueAsString;
                            pianoConservazione.TempoConservazione = rowCells.Where(itm => itm.Name.StartsWith("E")).FirstOrDefault().ValueAsString;
                            pianoConservazione.NoteChiusuraFascicolo = rowCells.Any(itm => itm.Name.StartsWith("F")) ? rowCells.Where(itm => itm.Name.StartsWith("F")).FirstOrDefault().ValueAsString : string.Empty;
                            pianoConservazione.NoteScartabilitaDocumenti = rowCells.Any(itm => itm.Name.StartsWith("G")) ? rowCells.Where(itm => itm.Name.StartsWith("G")).FirstOrDefault().ValueAsString : string.Empty;
                            pianoConservazione.NoteDocumenti = rowCells.Any(itm => itm.Name.StartsWith("H")) ? rowCells.Where(itm => itm.Name.StartsWith("H")).FirstOrDefault().ValueAsString : string.Empty;

                            insert = InsertPianoConservazione(pianoConservazione);
                            if (insert)
                            {
                                sl.Log("");
                                sl.Log("Piano conservazione nodo: " + rowCells.Where(itm => itm.Name.StartsWith("A")).FirstOrDefault().ValueAsString + " e Tipologia fascicolo: " + rowCells.Where(itm => itm.Name.StartsWith("D")).FirstOrDefault().ValueAsString + " inserito con successo.");
                                row++;
                            }
                            else
                            {
                                throw new Exception("Errore inserimento piano conservazione nodo: " + rowCells.Where(itm => itm.Name.StartsWith("A")).FirstOrDefault().ValueAsString + " e Tipologia fascicolo: " + rowCells.Where(itm => itm.Name.StartsWith("D")).FirstOrDefault().ValueAsString);
                            }
                        }
                        catch (Exception e)
                        {
                            sl.Log("");
                            sl.Log(e.Message);
                            error = true;
                            row++;
                        }
                    }
                    else
                    {
                        break;
                    }

                }


                sl.Log("");
                sl.Log("**** Fine importazione Piano di conservazione - " + System.DateTime.Now.ToString());
            }
            catch (Exception e)
            {
                logger.Error("Errore in ImportPianoConservazione: " + e.Message);
                error = true;
            }
            //finally
            //{
            //    if (xlsReader != null)
            //        xlsReader.Close();

            //    if (xlsConn != null)
            //        xlsConn.Close();
            //}

            return error;
        }

        private static OrgNodoTitolario GetNodoTitolario(string codice, string idAmm, string idRegistro, string idTitolario)
        {
            string queryRegistro = " AND ( ID_REGISTRO IS NULL OR ID_REGISTRO = " + idRegistro + " )";
            if(string.IsNullOrEmpty(idRegistro))
                queryRegistro = " AND ID_REGISTRO IS NULL ";

            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return amm.getNodoTitolario(codice, idAmm, queryRegistro, idTitolario);
        }

        public static bool InsertPianoConservazione(DocsPaVO.PianoConservazione pianoConservazione)
        {
            bool result = false;

            try
            {
                DocsPaDB.Query_DocsPAWS.PianoConservazione piano = new DocsPaDB.Query_DocsPAWS.PianoConservazione();
                result = piano.InsertPianoConservazione(pianoConservazione);
            }
            catch (Exception e)
            {
                logger.Error("Errore in InsertPianoConservazione: " + e.Message);
            }

            return result;
        }

        public static List<DocsPaVO.PianoConservazione> GetPianoConservazione(string idTitolario)
        {
            List<DocsPaVO.PianoConservazione> pianoConservazione = new List<DocsPaVO.PianoConservazione>();
            DocsPaDB.Query_DocsPAWS.PianoConservazione piano = new DocsPaDB.Query_DocsPAWS.PianoConservazione();

            try
            {
                pianoConservazione = piano.GetPianoConservazione(idTitolario);
            }
            catch (Exception e)
            {
                logger.Error("Errore in GetPianoConservazione: " + e.Message);
            }

            return pianoConservazione;
        }

        public static System.Collections.ArrayList GetLogImportPianoConservazione(string tempPath)
        {
            System.Collections.ArrayList fileLog = new System.Collections.ArrayList();
            string sLine = string.Empty;
            //string tempPath = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_TEMP_PATH");
            try
            {
                StreamReader objReader = new StreamReader(tempPath + "\\Modelli\\Import\\logImportazionePianoConservazione.log");
                while (sLine != null)
                {
                    sLine = objReader.ReadLine();
                    if (sLine != null)
                        fileLog.Add(sLine);
                }
                objReader.Close();

                return fileLog;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in GetLogImportPianoConservazione : " + e.Message);
                return fileLog;
            }
        }

        public static System.Collections.ArrayList GetLogImportTabellaConfine(string serverPath)
        {
            System.Collections.ArrayList fileLog = new System.Collections.ArrayList();
            string sLine = string.Empty;
            try
            {
                StreamReader objReader = new StreamReader(serverPath + "\\Modelli\\Import\\logImportazioneTabellaConfine.log");
                while (sLine != null)
                {
                    sLine = objReader.ReadLine();
                    if (sLine != null)
                        fileLog.Add(sLine);
                }
                objReader.Close();

                return fileLog;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in GetLogImportTabellaConfine : " + e.Message);
                return fileLog;
            }
        }

        public static bool StoricizzaPianoConservazione(string idTitolario, string idRegistro, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool retValue = true;
            DocsPaDB.Query_DocsPAWS.PianoConservazione piano = new DocsPaDB.Query_DocsPAWS.PianoConservazione();

            try
            {
                retValue = piano.StoricizzaPianoConservazione(idTitolario, idRegistro);
            }
            catch (Exception e)
            {
                logger.Error("Errore in StoricizzaPianoConservazione: " + e.Message);
            }

            return retValue;
        }
    }
}
