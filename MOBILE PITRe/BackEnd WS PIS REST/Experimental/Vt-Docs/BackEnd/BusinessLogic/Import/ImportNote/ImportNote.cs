using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data.OleDb;
using System.Text;
using DocsPaVO.PrjDocImport;
using DocsPaVO.Note;

namespace BusinessLogic.Import.ImportNote
{
    public class ImportNote
    {

        public static List<ImportResult> ImportElencoNote(byte[] content, string fileName, string modelPath, DocsPaVO.utente.InfoUtente userInfo)
        {
            // Il path completo in cui è posizionato il file excel contente le note da importare
            string completePath = String.Empty;

            // Il risultato dell'elaborazione
            List<ImportResult> result = new List<ImportResult>();
            try
            {
                // 1. Creazione del file temporaneo 
                fileName = fileName.Substring(fileName.LastIndexOf("\\") + 1);
                completePath = ImportUtils.CreateTemporaryFile(content, modelPath, fileName);

                // 2. Creazione delle note
                ArrayList listaRF = BusinessLogic.Utenti.RegistriManager.getListaRegistriRfRuolo(userInfo.idCorrGlobali, "1", "");
                result = CreateNote(completePath, listaRF);

                // 3. Cancellazione file temporaneo
                ImportUtils.DeleteTemporaryFile(completePath);
            }
            catch (Exception e)
            {
                // Se il file è stato creato, cancellazione
                if (!String.IsNullOrEmpty(completePath))
                    ImportUtils.DeleteTemporaryFile(completePath);

                // Creazione di un nuovo risultato con i dettagli dell'eccezione
                result.Add(new ImportResult()
                {
                    Outcome = ImportResult.OutcomeEnumeration.KO,
                    Message = e.Message
                });
            }

            // 4. Restituzione del risultato
            return result;

        }

     

        /// <summary>
        /// Funzione per la creazione delle note a partire da un foglio excel il cui path
        /// è passato per parametro
        /// </summary>
        private static List<ImportResult> CreateNote(string completePath, ArrayList listaRf)
        {
            string connectionString = String.Empty;
            OleDbConnection oleConnection = null;
            OleDbCommand oleCommand = null;
            OleDbDataReader dataReader = null;
            List<ImportResult> result = null;

            // Creazione della stringa di connessione
            connectionString = String.Format("Provider=" + System.Configuration.ConfigurationManager.AppSettings["DS_PROVIDER"] + "Data Source={0};Extended Properties=\"" + System.Configuration.ConfigurationManager.AppSettings["DS_EXTENDED_PROPERTIES"] + "\"", completePath);
            //connectionString = String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Macro;HDR=YES;\"", completePath);

            try
            {
                oleConnection = new OleDbConnection(connectionString);
                oleConnection.Open();
            }
            catch (Exception e)
            {
                throw new Exception("Errore durante la connessione al file excel. Dettagli: " + e.Message);
            }

            //Selezione dei dati sulle note da importare
            try
            {
                // Creazione della query per la selezione dei dati sulle note da importare
                oleCommand = new OleDbCommand("SELECT * FROM [Note$]", oleConnection);
                dataReader = oleCommand.ExecuteReader();
            }
            catch (Exception e)
            {
                // Chiusura del datareader
                if (dataReader != null && !dataReader.IsClosed)
                    dataReader.Close();

                // Chiusura della connessione
                oleConnection.Close();
                throw new Exception("Errore durante il recupero dei dati sulle note da importare. Dettagli: " + e.Message);
            }

            //Importazione note
            result = ExecuteImportNote(dataReader, listaRf);

            //Chiusura del data reader e della connessione
            try
            {
                if (dataReader != null && !dataReader.IsClosed)
                    dataReader.Close();
                oleConnection.Close();
            }
            catch (Exception e)
            {
                result.Add(new ImportResult()
                {
                    Outcome = ImportResult.OutcomeEnumeration.KO,
                    Message = "Errore durante la chiusura della connessione al file excel. Dettagli: " + e.Message
                });
            }
            return result;
        }

        /// <summary>
        /// Funzione per l'esecuzione dell'importazione delle note
        /// </summary>
        private static List<ImportResult> ExecuteImportNote(OleDbDataReader dataReader, ArrayList listaRf)
        {
            
            // Il risultato del controllo di validità dati per la riga in analisi
            bool validParameters = false;
            NotaElenco nota = null;
            List<ImportResult> importResult = new List<ImportResult>();

            // Il numero delle note importate
            int importedNote = 0;
            // Il numero delle note non importate
            int notImportedNote = 0;
            // Il risultato dell'ultima operazione di creazione
            ImportResult temp = null;
            // La lista degli errori avvenuti in una delle fasi di creazione delle note
            List<string> creationProblems = null;

            // Se non ci sono note da importare, il report conterrà
            // una sola riga con un messaggio che informi l'utente
            if (!dataReader.HasRows)
                importResult.Add(new ImportResult()
                {
                    Outcome = ImportResult.OutcomeEnumeration.OK,
                    Message = "Non sono state rilevate note da importare."
                });

            // Finchè ci sono dati da leggere
            while (dataReader.Read())
            {
                 // Reset variabili
                validParameters = false;
                nota = new NotaElenco();
                creationProblems = new List<string>();
                temp = new ImportResult();

                //Prelevamento dei dati dalla riga xls attuale
                try
                {
                    if (dataReader.GetName(0).ToUpper() == "Codice RF".ToUpper() && dataReader.GetName(1).ToUpper() == "Descrizione nota".ToUpper())
                    {
                        DocsPaDB.Query_DocsPAWS.RF rf = new DocsPaDB.Query_DocsPAWS.RF();
                        //Recupero l'id del rf 
                        nota.idRegRf = rf.getSystemIdRFDaDPA_EL_REGISTRI(dataReader["Codice RF"].ToString().Trim());
                        nota.codRegRf = dataReader["Codice RF"].ToString().Trim();
                        nota.descNota = dataReader["Descrizione nota"].ToString().Trim();
                    }
                    else
                    {
                        importResult.Add(new ImportResult()
                        {
                            Outcome = ImportResult.OutcomeEnumeration.KO,
                            Message = "Nome colonne foglio excel errate"
                        });
                        return importResult;
                    }
                }
                catch (Exception e)
                {
                    importResult.Add(new ImportResult()
                    {
                        Outcome = ImportResult.OutcomeEnumeration.KO,
                        Message = e.Message
                    });
                    notImportedNote++;
                }

                //Messaggio nota: descrizione e rf
                temp.Message = nota.descNota + " --- " + nota.codRegRf;
                
                //Controllo validità campi obbligatori
                validParameters = CheckDataValidity(nota, out creationProblems, listaRf);
               
                // Se i parametri di creazione della nota sono tutti validi e popolati
                if (validParameters)
                {
                    try
                    {
                        // Inserimento della nota
                        string message = "";
                        DocsPaDB.Query_DocsPAWS.Note dbNota = new DocsPaDB.Query_DocsPAWS.Note();
                        dbNota.InsertNotaInElenco(nota, out message);

                        if (!string.IsNullOrEmpty(message))
                        {
                            // Prova inserimento nota in db fallita
                            temp.Outcome = ImportResult.OutcomeEnumeration.KO;
                            creationProblems.Add(message);
                            temp.OtherInformation.AddRange(creationProblems);
                            notImportedNote++;
                        }
                        else
                        {
                            // Altrimenti il risultato è positivo
                            temp.Outcome = ImportResult.OutcomeEnumeration.OK;
                            // Il messaggio è un messaggio di successo
                            creationProblems.Add("Nota inserita in elenco");
                            temp.OtherInformation.AddRange(creationProblems);
                            importedNote++;
                        }
                    
                        importResult.Add(temp);
                    }
                    catch (Exception e)
                    {
                        // Viene creato un risultato negativo
                        importResult.Add(new ImportResult()
                        {
                            Outcome = ImportResult.OutcomeEnumeration.KO,
                            Message = e.Message
                        });
                        notImportedNote++;
                    }

                }
                else
                {
                    // I parametri non sono validi o correttamente popolati:  il risultato è negativo
                    temp.Outcome = ImportResult.OutcomeEnumeration.KO;
                    temp.OtherInformation.AddRange(creationProblems);
                    notImportedNote++;
                    importResult.Add(temp);
                }
            }

            // Aggiunta di un'ultima riga contenente il totale delle note importate e non importate
            importResult.Add(new ImportResult()
            {
                Outcome = ImportResult.OutcomeEnumeration.OK,
                Message = String.Format("Note importate: {0}; Note non importate: {1}",
                    importedNote, notImportedNote)
            });

            // Restituzione del report sui risultati
            return importResult;

        }

        private static bool CheckDataValidity(NotaElenco nota, out List<String> notValidData, ArrayList listaRF)
        {
            // La lista con gli errori da restituire
            List<String> problems = new List<string>();
            // Il risultato del controllo di validità
            bool validationResult = true;
            problems.Add("Nota non inserita in elenco");

            // Il codice rf è obbligatorio
            if (String.IsNullOrEmpty(nota.codRegRf))
            {
                validationResult = false;
                problems.Add("Campo 'Codice RF' obbligatorio.");
            }

            // La descrizione è obbligatoria
            if (String.IsNullOrEmpty(nota.descNota))
            {
                validationResult = false;
                problems.Add("Campo 'Descrizione nota' obbligatorio.");
            }

            // verifica se il ruolo dell'utente ha visibilità sull'rf a cui si vuole assocliare la nota
            bool RfVisibile = false;
            for (int i = 0; i < listaRF.Count; i++)
            {
                if (nota.idRegRf == ((DocsPaVO.utente.Registro)listaRF[i]).systemId)
                {
                    RfVisibile = true;
                    break;
                }
            }

            // RF visibile al ruolo dell'utente
            if (!RfVisibile)
            {
                validationResult = false;
                problems.Add("Rf non visibile al ruolo.");
            }

            if (validationResult)
                problems.Remove("Nota non inserita in elenco");

            // Impostazione della lista di problemi di vlaidazione
            notValidData = problems;

            // Restituzione del risultato della validazione
            return validationResult;
        }

    }
}
