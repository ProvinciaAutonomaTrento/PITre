using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using DocsPaVO.utente;
using DocsPaVO.Note;
using DocsPaVO.documento;
using DocsPaVO.PrjDocImport;

namespace BusinessLogic.Import.ImportDocuments.DocumentType
{
    class GrayDocument : Document
    {
        /// <summary>
        /// Funzine per l'estrazione dei dati da una riga di foglio excel
        /// </summary>
        /// <param name="row">L'oggetto contenente i dati letti dal foglio excel</param>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <param name="role">Il ruolo con cui è stata lanciata la procedura</param>
        /// <returns>L'oggetto con i dati estratti dalla riga del foglio excel</returns>
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

            // Prelevamento del numero da assegnare al protocollo e della data di protocollo
            if (isEnabledPregressi)
            {
                /*
                   if (String.IsNullOrEmpty(row["Data protocollo"].ToString()))
                       toReturn.ProtocolDate = DateTime.Now;
                   else
                       toReturn.ProtocolDate = DateTime.Parse(row["Data protocollo"].ToString());
                
                   */
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
                if (colonne.Contains("Codice Utente Creatore") ||
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

            // Prelevamento del pathname nel caso di stampa unione questa colonna non è presente e
            // il valore di pathname viene settato nel blocco cath a stringa vuota
            try
            {
                toReturn.Pathname = row["Pathname"].ToString().Trim();
            }
            catch (Exception e)
            {
                toReturn.Pathname = string.Empty;
            }
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
        /// Funzione per la validazione dei dati
        /// </summary>
        /// <param name="rowData">L'oggetto con i dati da validare</param>
        /// <param name="isProfilationRequired">True se è richiesta la profilazione obbligatoria</param>
        /// <param name="isRapidClassificationRequired">True se è richiesta la classificazione rapida obbligatoria</param>
        /// <param name="validationProblems">Lista dei problemi verificatisi in fase di validazione dei dati</param>
        /// <returns>True se la validazione passa</returns>
        protected override bool CheckDataValidity(DocumentRowData rowData, bool isProfilationRequired, bool isRapidClassificationRequired, out List<string> validationProblems)
        {
            #region Dichiarazione variabili

            // Il risultato della validazione
            bool validationResult = true;

            #endregion

            // Creazione della lista dei problemi
            validationProblems = new List<string>();

            // L'ordinale è obbligatorio
            if (String.IsNullOrEmpty(rowData.OrdinalNumber))
            {
                validationResult = false;
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
            
            // Se entrambi i campi 'Oggetto' e 'Codice Oggetto' non
            // sono valorizzati, la validazione non passa
            if (String.IsNullOrEmpty(rowData.ObjCode) &&
                String.IsNullOrEmpty(rowData.Obj))
            {
                validationResult = false;
                validationProblems.Add("Non risultano valorizzati nè il campo 'Codice Oggetto' nè il campo 'Oggetto'.  Valorizzare uno dei due.");
            }

            // Se entrambi i campi 'Oggetto' e 'Codice oggetto' sono valorizzati, errore
            if (!String.IsNullOrEmpty(rowData.ObjCode) &&
                !String.IsNullOrEmpty(rowData.Obj))
            {
                validationResult = false;
                validationProblems.Add("Risultano valorizzati entrambi i campi 'Codice Oggetto' e 'Oggetto'. Valorizzare solamente uno.");
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
            if (isProfilationRequired && String.IsNullOrEmpty(rowData.DocumentTipology))
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
                validationProblems.Add("La classificazione è obbligatoria ma non risultano valorizzati nè il campo 'Codice Fascicolo' nè i campi dedicati alla classificazione.");
            }

            // Retituzione del risultato di validazione
            return validationResult;
        }

        /// <summary>
        /// Funzione per la creazione dell'oggetto con le informazioni sul protocollo. In questo caso
        /// questa funzione restituisce un risultato nullo in quanto il documento grigio non è un 
        /// protocollo
        /// </summary>
        /// <param name="rowData">L'oggetto con le informazioni sul documento da creare</param>
        /// <param name="registrySyd">L'id del registro</param>
        /// <param name="rfSyd">L'id dell'RF</param>
        /// <param name="administrationSyd">L'id dell'amministrazione</param>
        /// <param name="isSmistamentoEnabled">True se è abilitato lo smistamento</param>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <param name="role">Il ruolo ocn cui è stata lanciata la procedura</param>
        /// <returns>L'oggetto protocollo (null per i documenti grigi)</returns>
        protected override Protocollo CreateProtocolObject(DocumentRowData rowData, string registrySyd, string rfSyd, string administrationSyd, bool isSmistamentoEnabled, InfoUtente userInfo, Ruolo role)
        {
            return null;
        }
    }
}
