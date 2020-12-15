
namespace DocsPaVO.Import.Project
{
    /// <summary>
    /// Questa classe rappresenta un'eccezione generata dal processo di importazione fascicoli
    /// </summary>
    public class ImportProjectException : ImportException
    {
        /// <summary>
        /// Funzione per l'inizializzazione di un'eccezione rilevata durante l'importazione 
        /// di fascicoli
        /// </summary>
        /// <param name="message">Il messaggio da attribuire all'eccezione</param>
        public ImportProjectException(string message) : base(message) { }

    }
}
