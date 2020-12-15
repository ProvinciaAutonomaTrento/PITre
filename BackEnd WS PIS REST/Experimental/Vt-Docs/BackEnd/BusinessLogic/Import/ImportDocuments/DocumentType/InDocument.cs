using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.OleDb;
using DocsPaVO.utente;
using DocsPaVO.Note;
using BusinessLogic.Documenti;
using DocsPaVO.documento;
using BusinessLogic.Utenti;
using System.Collections;
using DocsPaVO.rubrica;
using BusinessLogic.Fascicoli;
using DocsPaVO.fascicolazione;
using DocsPaVO.filtri;
using BusinessLogic.ProfilazioneDinamica;
using DocsPaVO.ProfilazioneDinamica;
using System.Text;
using DocsPaVO.PrjDocImport;
using DocsPaVO.addressbook;
using System.Globalization;

namespace BusinessLogic.Import.ImportDocuments.DocumentType
{
    /// <summary>
    /// Questa classe si occupa di importare i documenti in ingresso
    /// </summary>
    class InDocument : Document
    {
        /// <summary>
        /// Funzione per la lettura dei dati da una riga del foglio excel
        /// </summary>
        /// <param name="row">La riga del foglio excel da cui estrarre i dati</param>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <param name="role">Il ruolo dell'utente che ha lanciato la procedura</param>
        /// <param name="isEnabledPregressi">Il ruolo dell'utente che ha lanciato la procedura</param>
        /// <returns>Un oggetto con le informazioni estratte dal foglio excel</returns>
        protected override DocumentRowData ReadDataFromCurrentRow(OleDbDataReader row, InfoUtente userInfo, Ruolo role, bool isEnabledPregressi)
        {
            // L'oggetto da restituire
            DocumentRowData toReturn = new DocumentRowData();
            List<string> colonne = new List<string>();
            for (int i = 0; i < row.FieldCount; i++)
                colonne.Add(row.GetName(i));

            // Prelevamento dell'ordinale
            toReturn.OrdinalNumber = row["Ordinale"].ToString().Trim();

            // Prelevamento del codice dell'amministrazione
            toReturn.AdminCode = row["Codice Amministrazione"].ToString().Trim();

            // Prelevamento del numero da assegnare al protocollo e della data di protocollazione
            if (isEnabledPregressi)
            {
                if (colonne.Contains("Numero di protocollo"))
                {
                    toReturn.ProtocolNumber = row["Numero di protocollo"].ToString().Trim();
                }

                if (colonne.Contains("Data protocollo"))
                {
                    if (String.IsNullOrEmpty(row["Data protocollo"].ToString()))
                        toReturn.ProtocolDate = DateTime.Now;
                    else
                        toReturn.ProtocolDate = DateTime.Parse(row["Data protocollo"].ToString());

                }
                if (colonne.Contains("Codice Utente Creatore"))
                {
                    if (!String.IsNullOrEmpty(row["Codice Utente Creatore"].ToString()))
                    {
                        toReturn.CodiceUtenteCreatore = row["Codice Utente Creatore"].ToString().Trim();
                    }
                }

                if (colonne.Contains("Codice Ruolo Creatore"))
                {
                    if (!String.IsNullOrEmpty(row["Codice Ruolo Creatore"].ToString()))
                    {
                        toReturn.CodiceRuoloCreatore = row["Codice Ruolo Creatore"].ToString().Trim();
                    }
                }
            }
            else
            {

                if (colonne.Contains("Numero di protocollo") || 
                    colonne.Contains("Data protocollo")      ||
                     colonne.Contains("Codice Utente Creatore") ||
                     colonne.Contains("Codice Ruolo Creatore")     
                    
                    )
                {
                    throw new Exception("Ruolo non abilitato per questa importazione"); 
                }
            }
            

            // Prelevamento del codice registro
            toReturn.RegCode = row["Codice Registro"].ToString().Trim();

            // Prelevamento del codice RF
            toReturn.RFCode = row["Codice RF"].ToString().Trim();

            // Prelevamento del codice oggetto
            toReturn.ObjCode = row["Codice oggetto"].ToString().Trim();

            // Prelevamento dell'oggetto
            toReturn.Obj = row["Oggetto"].ToString().Trim();

            // Prelevamento del codice corrispondente, se valorizzato
            if (!String.IsNullOrEmpty(row["Codice corrispondente"].ToString()))
                toReturn.CorrCode = new List<string>()
                    {
                        row["Codice corrispondente"].ToString().Trim()
                    };

            // Prelevamento della descrizione del corrispondente, se valorizzato
            if (!String.IsNullOrEmpty(row["Corrispondente"].ToString()))
                toReturn.CorrDesc = new List<string>()
                    {
                        row["Corrispondente"].ToString().Trim()
                    };

            // Prelevamento del pathname
            toReturn.Pathname = row["Pathname"].ToString().Trim();

            // Prelevamento del valore che indica se il documento deve
            // essere salvato nell'area di lavoro
            toReturn.InWorkingArea = !String.IsNullOrEmpty(row["ADL"].ToString()) &&
                row["ADL"].ToString().Trim().ToUpper() == "SI";

            // Prelevamento delle note
            if (!String.IsNullOrEmpty(row["Note"].ToString()))
            {
                // Creazione della nota
                toReturn.Note = new InfoNota(row["Note"].ToString().Trim());

                //Impostazione della visibilità Tutti
                toReturn.Note.TipoVisibilita = TipiVisibilitaNotaEnum.Tutti;

                // Creazione dell'oggetto utilizzato per memorizzare le informazioni 
                // sull'utente creatore
                toReturn.Note.UtenteCreatore = new InfoUtenteCreatoreNota();

                // Impostazione della descrizione del ruolo
                toReturn.Note.UtenteCreatore.DescrizioneRuolo = role.descrizione;

                // Impostazione della descrizione dell'utente
                toReturn.Note.UtenteCreatore.DescrizioneUtente = userInfo.userId;

                // Impostazione dell'id del ruolo
                toReturn.Note.UtenteCreatore.IdRuolo = role.systemId;

                // Impostazione dell'id dell'utente
                toReturn.Note.UtenteCreatore.IdUtente = userInfo.userId;

                // La nota deve essere inserita
                toReturn.Note.DaInserire = true;

                // Impostazione della data di creazione
                toReturn.Note.DataCreazione = DateTime.Now;

            }

            // Prelevamento del codice del modello di trasmissione
            toReturn.TransmissionModelCode = row["Codice modello trasmissione"].ToString().
                Trim().Split(';');

            // Prelevamento della descrizione della tipologia di documento
            toReturn.DocumentTipology = row["Tipologia documento"].ToString().Trim().ToUpper();

            // Prelevamento del codice fascicolo
            if (!String.IsNullOrEmpty(row["Codice fascicolo"].ToString()))
                toReturn.ProjectCodes = row["Codice fascicolo"].ToString().Trim().Split(';');

            // Prelevamento della descrizione del fascicolo
            toReturn.ProjectDescription = row["Descrizione fascicolo"].ToString().Trim();

            // Prelevamento della descrizione del sottofascicolo
            toReturn.FolderDescrition = row["Descrizione sottofascicolo"].ToString().Trim();

            // Prelevamento della descrizione del titolario
            toReturn.Titolario = row["Titolario"].ToString().Trim();

            // Prelevamento del codice del nodo
            toReturn.NodeCode = row["Codice Nodo"].ToString().Trim();

            // Prelevamento della tipologia di fascicolo
            toReturn.ProjectTipology = row["Tipologia fascicolo"].ToString().Trim().ToUpper();

            // Restituzione dell'oggetto con i dati prelevati dalla riga
            return toReturn;

        }

        /// <summary>
        /// Funzione per la vadazione dei dati contenuti nell'oggetto RowData
        /// </summary>
        /// <param name="rowData">L'oggetto con i dati da validare</param>
        /// <param name="isProfilationRequired">True se, per configurazione, è richiesta la profilazione obbligatoria</param>
        /// <param name="isRapidClassificationRequired">True se, per configurazione, è richiesta la classificazione rapida</param>
        /// <param name="validationProblems">Questo parametro verrà impostato con la lista dei problemi riscontrati durante la procedura di validazione</param>
        /// <returns>True se la procedura và a buon fine</returns>
        protected override bool CheckDataValidity(DocumentRowData rowData, bool isProfilationRequired, bool isRapidClassificationRequired, out List<string> validationProblems)
        {
            #region Dichiarazione variabili

            // Il risultato della validazione
            bool validationResult = true;

            // L'oggetto a cui richiedere le informaizoni sull'amministrazione
            DocsPaDB.Query_DocsPAWS.Amministrazione administrationManager = null;

            // Valore booleano che indica se è obbligatorio specificare l'RF
            bool rfRequired = false;

            // L'id dell'amministrazione
            string administrationSyd = String.Empty;

            #endregion

            // Creazione della lista dei problemi
            validationProblems = new List<string>();

            // L'ordinale è obbligatorio
            if (String.IsNullOrEmpty(rowData.OrdinalNumber))
            {
                validationResult = false;

                if (rowData is RDEDocumentRowData)
                    validationProblems.Add("Campo 'Numero protocollo emergenza' obbligatorios.");
                else
                    validationProblems.Add("Campo 'Ordinale' obbligatorio.");
            }

            // Il codice amministrazione è obbligatorio
            if (String.IsNullOrEmpty(rowData.AdminCode))
            {
                validationResult = false;
                validationProblems.Add("Campo 'Codice amministrazione' obbligatorio.");
            }

            // Il codice registro è obbligatorio
            if (String.IsNullOrEmpty(rowData.RegCode))
            {
                validationResult = false;
                validationProblems.Add("Campo 'Codice Registro' obbligatorio.");
            }

            // Il codice RF deve essere obbligatorio se richiesto dalla segnatura
            // Creazione dell'oggetto a cui richiedere le informazioni sull'amministrazione
            administrationManager = new DocsPaDB.Query_DocsPAWS.Amministrazione();

            try
            {
                administrationSyd = ImportUtils.GetAdministrationId(rowData.AdminCode);
            }
            catch (Exception e)
            {
                validationResult = false;
                validationProblems.Add(String.Format(
                    "Impossibile recuperare le informazioni sull'amministrazione {0}",
                    rowData.AdminCode));
            }

            // Se non è stato possibile ricavare l'id dell'amministrazione, problema
            if (!String.IsNullOrEmpty(administrationSyd))
            {
                // Si procede con la verifica 
                // Verifica dell'obbligatorietà della specifica dell'RF
                rfRequired = administrationManager.AmmGetListAmministrazioni().Where(
                    e => e.IDAmm == administrationSyd).FirstOrDefault().
                        Segnatura.Contains("COD_RF_PROT");

                // Se l'RF è richiesto ma non è specificato, la validazione non passa
                if (rfRequired && String.IsNullOrEmpty(rowData.RFCode))
                {
                    validationResult = false;
                    validationProblems.Add("Campo 'Codice RF' obbligatorio in quanto richiesto dalla segnatura.");
                }

            }

            // Se entrambi i campi 'Oggetto' e 'Codice Oggetto' non
            // sono valorizzati, la validazione non passa
            if (String.IsNullOrEmpty(rowData.ObjCode) &&
                String.IsNullOrEmpty(rowData.Obj))
            {
                validationResult = false;

                if (rowData is RDEDocumentRowData)
                    validationProblems.Add("Il campo 'Oggetto' è obbligatorio.");
                else
                    validationProblems.Add("Non risultano valorizzati nè il campo 'Codice Oggetto' nè il campo 'Oggetto'.  Valorizzare uno dei due.");
            }

            // Se entrambi i campi 'Oggetto' e 'Codice oggetto' sono valorizzati, errore
            if (!String.IsNullOrEmpty(rowData.ObjCode) &&
                !String.IsNullOrEmpty(rowData.Obj))
            {
                validationResult = false;
                validationProblems.Add("Risultano valorizzati entrambi i campi 'Codice Oggetto' e 'Oggetto'. Valorizzare solamente uno.");
            }

            // Se entrambi i campi 'Codice Corrispondente' e 'Corrispondente' non
            // sono valorizzati, la validazione non passa
            if (rowData.CorrCode == null &&
                rowData.CorrDesc == null)
            {
                validationResult = false;

                if (rowData is RDEDocumentRowData)
                    validationProblems.Add("Non sono state trovate informazioni sui corrispondenti. Controllare i relativi campi.");
                else
                    validationProblems.Add("Non risultano valorizzati nè il campo 'Codice Corrispondente' nè il campo 'Corrispondente'. Valorizzarne uno.");
            }

            // Se il campo 'Codice Fascicolo' del foglio excel è valorizzato, ed
            // è valorizzata anche la profilazione del fascicolo, errore in quanto non è possibile
            // valorizzarli entrambi
            if (rowData.ProjectCodes != null && rowData.ProjectCodes.Length > 0 &&
                (!String.IsNullOrEmpty(rowData.ProjectDescription) ||
                 !String.IsNullOrEmpty(rowData.FolderDescrition) ||
                 !String.IsNullOrEmpty(rowData.NodeCode) ||
                 !String.IsNullOrEmpty(rowData.ProjectTipology)))
            {
                validationResult = false;

                validationProblems.Add("Risultano valorizzati sia i campi 'Codice Fascicolo' che i campi riguardanti la specifica del fascicolo. Non è possibile specificarli entambi.");
            }

            // Se è richiesta profilazione del documento obbligatoria
            // ma non è valorizzato il campo 'Tipologia Documento', la validazione
            // non deve passare
            if (isProfilationRequired && String.IsNullOrEmpty(rowData.DocumentTipology) && !(rowData is RDEDocumentRowData))
            {
                validationResult = false;
                validationProblems.Add("La profilazione del documento è obbligatoria ma non risulta valorizzato il campo 'Tipologia Documento'");
            }

            // Se è richiesta fascicolazione rapida obbligatoria ma non sono specificati 
            // dati in 'Codice Fascicolo' o nei campi dedicati al fascicolo
            if (isRapidClassificationRequired &&
                rowData.ProjectCodes == null &&
                String.IsNullOrEmpty(rowData.ProjectDescription) &&
                String.IsNullOrEmpty(rowData.FolderDescrition) &&
                String.IsNullOrEmpty(rowData.NodeCode) &&
                String.IsNullOrEmpty(rowData.ProjectTipology))
            {
                validationResult = false;

                if (rowData is RDEDocumentRowData)
                    validationProblems.Add("Campo 'Codice Classifica' obbligatorio.");
                else
                    validationProblems.Add("La classificazione è obbligatoria ma non risultano valorizzati nè il campo 'Codice Fascicolo' nè i campi dedicati alla classificazione.");
            }

            // Se la riga è una riga di RDE, vengono effettuati ulteriori controlli
            if (rowData is RDEDocumentRowData)
            {
                RDEDocumentRowData converted = rowData as RDEDocumentRowData;

                // Campo Stringa protocollo emergenza obbligatorio
                if (String.IsNullOrEmpty(converted.EmergencyProtocolSignature))
                {
                    validationResult = false;
                    validationProblems.Add("Campo 'Stringa protocollo emergenza' obbligatorio.");
                }

                // Se il campo Data protocollo emergenza non è valorizzato, non si può procedere
                if (String.IsNullOrEmpty(converted.EmergencyProtocolDate))
                {
                    validationResult = false;
                    validationProblems.Add("Campo 'Data protocollo emergenza' obbligatorio.");
                }

                // Se il campo Data protocollo emergenza non è valorizzato, non si può procedere
                if (String.IsNullOrEmpty(converted.EmergencyProtocolTime))
                {
                    validationResult = false;
                    validationProblems.Add("Campo 'Ora protocollo emergenza' obbligatorio.");

                }


                // Il campo Data protocollo emergenza deve essere minore della data di protocollazione, che 
                // si suppone uguale alla data odierna. visto che il protocolla in giallo è stata dismessa 
                // nel senso che non la usa più nessuno.
                if (!String.IsNullOrEmpty(converted.EmergencyProtocolDate) &&
                    !String.IsNullOrEmpty(converted.EmergencyProtocolTime))
                {
                    //l'excel è formattato un modo tale che la data e l'ora possono solo arrivare nel formato valido.
                    string dataEmerg = converted.EmergencyProtocolDate + " " + converted.EmergencyProtocolTime;
                    DateTime dataEmergOut;

                    if (ImportUtils.IsDate(dataEmerg))
                    {
                        dataEmergOut = ImportUtils.ReadDate(dataEmerg);
                        if (dataEmergOut > System.DateTime.Now)
                        {
                            validationResult = false;
                            validationProblems.Add("Campo 'Data protocollo emergenza' deve essere minore della data di protocollazione.");
                        }
                    }
                    else
                    {
                        validationResult = false;
                        validationProblems.Add("Il campo data o il campo ora emergenza non sono espressi in un formato valido.");

                    }

                }

                // Se almeno uno fra i due campi "Data arrivo" e "Ora Arrivo", è valorizzato,
                // viene controllato che antrambi lo siano.
                if ((!String.IsNullOrEmpty(converted.ArrivalTime) && String.IsNullOrEmpty(converted.ArrivalDate)) ||
                    (!String.IsNullOrEmpty(converted.ArrivalDate) && String.IsNullOrEmpty(converted.ArrivalTime)))
                {
                    validationResult = false;
                    validationProblems.Add("Solo uno dei due campi \"Data arrivo\" e \"Ora Arrivo\" risulta valorizzato. Valorizzarli entrambi o nessuno dei due.");
                }

                // La data di protocollo mittente deve essere minore della
                // data attuale e minore della data di arrivo
                if (!String.IsNullOrEmpty(converted.SenderProtocolDate) &&
                    DateTime.Parse(converted.SenderProtocolDate) > DateTime.Now)
                {
                    validationProblems.Add("La data di protocollo mittente non può essere maggiore della data attuale.");
                    validationResult = false;
                }

                //controllo sulla validità del formato della data arrivo
                // Se non è una data, errore
                if (!string.IsNullOrEmpty(converted.ArrivalDate))
                {
                    if (!ImportUtils.IsDate(converted.ArrivalDate))
                    {
                        validationProblems.Add("Il campo data o il campo ora arrivo non sono espressi in un formato valido.");
                        validationResult = false;
                    }
                    else
                    {
                        DateTime arrivalDate = ImportUtils.ReadDate(converted.ArrivalDate);

                        // La data di arrivo deve essere minore della
                        // data attuale 
                        if (arrivalDate > DateTime.Now)
                        {
                            validationProblems.Add("La data di arrivo non può essere maggiore della data di protocollo.");
                            validationResult = false;
                        }

                        if (!String.IsNullOrEmpty(converted.SenderProtocolDate) &&
                            !String.IsNullOrEmpty(converted.ArrivalDate) &&
                            DateTime.Parse(converted.SenderProtocolDate) > arrivalDate)
                        {
                            validationResult = false;
                            validationProblems.Add("La data di protocollo mittente deve essere minore della data di arrivo.");
                        }
                    }
                }
                       
            }

            // Retituzione del risultato di validazione
            return validationResult;

        }

        /// <summary>
        /// Funzione per la creazione dell'oggetto protocollo in entrata
        /// </summary>
        /// <param name="rowData">L'oggetto con le informazioni sul documento da creare</param>
        /// <param name="registrySyd">Il system id del registo in cui protocollare</param>
        /// <param name="rfSyd">Il system id dell'rf in cui protocollare</param>
        /// <param name="administrationSyd">Il system id dell'amministrazione incui creare il documento</param>
        /// <param name="isSmistamentoEnabled">True se, da configurazione, è abilitato lo smistamento</param>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <param name="role">Il ruolo dell'utente che ha lanciato la procedura</param>
        /// <returns>L'oggetto Protocollo con le informazioni sul protocollo in ingresso da creare</returns>
        protected override Protocollo CreateProtocolObject(DocumentRowData rowData, string registrySyd, string rfSyd, string administrationSyd, bool isSmistamentoEnabled, InfoUtente userInfo, Ruolo role)
        {
            // Creazione dell'oggetto ProtocolloEntrata
            ProtocolloEntrata inProto = new ProtocolloEntrata();

            // Calcolo del mittente del protocollo
            // Se è valorizzata la proprietà CorrDesc della rowData, significa che
            // il corrispondente è di tipo occasionale
            if (rowData.CorrDesc != null && rowData.CorrDesc.Count > 0)
            {
                // Creazione del corrispondente
                inProto.mittente = new Corrispondente();

                // Impostazione della descrizione del corrispondente
                inProto.mittente.descrizione = rowData.CorrDesc[0];

                // Impostazione dell'id amministrazione
                inProto.mittente.idAmministrazione = administrationSyd;

                // Impostazione del tipo corrispondente ad O
                inProto.mittente.tipoCorrispondente = "O";

            }

            if(rowData.CorrCode != null && rowData.CorrCode.Count > 0)
                // Altrimenti si procede con il caricamento delle informazioni sul
                // corrispondente
                inProto.mittente = ImportUtils.GetCorrispondenteByCode(
                    ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_IN,
                    rowData.CorrCode[0].Trim(),
                    role,
                    userInfo,
                    registrySyd,
                    rfSyd,
                    isSmistamentoEnabled,
                    TipoUtente.GLOBALE);

            // Se non è stato ptrovato il corrispondente, eccezione
            if (inProto.mittente == null)
                throw new Exception("Impossibile recuperare le informazioni sul mittente del protocollo.");

            // Restituzione dell'oggetto con le informazioni sul protocollo
            return inProto;

        }

    }

}