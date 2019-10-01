using System;
using System.Collections.Generic;

namespace DocsPaVO.Import.AddressBook
{
    /// <summary>
    /// Questa classe rappresenta il risultato dell'operazione di importazione
    /// relativa ad un corrispondente
    /// </summary>
    [Serializable()]
    public class AddressBookImportResult
    {
        /// <summary>
        /// Enumerazione dei possibili risultati
        /// </summary>
        public enum ResultEnum
        {
            OK,
            KO,
            Warning
        }

        // Lista dei problemi
        public List<String> Problems { get; set; }

        /// <summary>
        /// Metodo per la creazione e l'inizializzazione di una nuova istanza
        /// di questo oggetto
        /// </summary>
        public AddressBookImportResult()
        {
            // Creazione della lista dei problemi
            this.Problems = new List<string>();
            
            // Per default il risultato è positivo
            this.Result = ResultEnum.OK;
        
        }

        /// <summary>
        /// Esito dell'operazione
        /// </summary>
        public ResultEnum Result { get; set; }

        /// <summary>
        /// Funzione per l'aggiunta di un problema alla lista dei problemi
        /// </summary>
        /// <param name="problem">Il problema da aggiungere</param>
        public void AddProblem(string problem)
        {
            this.Problems.Add(problem);
        }

        /// <summary>
        /// Funzione per l'aggiunta di una lista di problemi ai problemi
        /// </summary>
        /// <param name="problems">La lista dei problemi da aggiungere a questa voce</param>
        public void AddProblem(List<string> problems)
        {
            this.Problems.AddRange(problems);
        }

        /// <summary>
        /// Funzione per la restituzione dei problemi
        /// </summary>
        /// <returns>La lista dei problemi</returns>
        public List<String> GetProblems()
        {
            return this.Problems;
        }

        /// <summary>
        /// Descrizione del risultato
        /// </summary>
        public string Message { get; set; }

    }
}
