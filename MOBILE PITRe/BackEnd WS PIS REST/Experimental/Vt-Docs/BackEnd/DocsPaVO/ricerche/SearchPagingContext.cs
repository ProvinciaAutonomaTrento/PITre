using System;
using System.Collections.Generic;
using System.Text;

namespace DocsPaVO.ricerche
{
    /// <summary>
    /// Classe per la gestione del contesto di paginazione per le ricerche
    /// </summary>
    [Serializable()]
    public class SearchPagingContext
    {
        /// <summary>
        /// 
        /// </summary>
        public SearchPagingContext()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        public SearchPagingContext(int page, int pageSize)
        {
            this.Page = page;
            this.PageSize = pageSize;
        }

        /// <summary>
        /// Impostazione numero totale record estratti
        /// </summary>
        public void SetRecordCount(int recordCount)
        {
            this.RecordCount = recordCount;

            // Determina il num di pagine totali
            this.PageCount = (recordCount / this.PageSize);

            if ((recordCount % this.PageSize) > 0)
                this.PageCount++;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Page = 0;

        /// <summary>
        /// 
        /// </summary>
        public int PageCount = 0;

        /// <summary>
        /// 
        /// </summary>
        public int PageSize = 0;

        /// <summary>
        /// 
        /// </summary>
        public int RecordCount = 0;

        /// <summary>
        /// Numero riga iniziale per la pagina richiesta
        /// </summary>
        public int StartRow
        {
            get { return ((this.Page * this.PageSize) - this.PageSize) + 1; }
        }

        /// <summary>
        /// Numero riga finale per la pagina richiesta
        /// </summary>
        public int EndRow
        {
            get { return (this.StartRow - 1) + this.PageSize; }
        }

        /// <summary>
        ///  Lista degli id profile di documenti restituiti dalla query di ricerca
        /// </summary>
        public List<String> IdProfilesList { get; set; }

        /// <summary>
        /// True se si desidera ricevere la lista dei system id dei documenti
        /// </summary>
        public bool GetIdProfilesList { get; set; }
    }
}
