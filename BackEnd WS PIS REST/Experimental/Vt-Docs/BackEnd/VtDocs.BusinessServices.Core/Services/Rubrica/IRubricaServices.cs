using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VtDocs.BusinessServices.Entities.Rubrica;

namespace VtDocs.BusinessServices.Services.Rubrica
{
    /// <summary>
    /// Interfaccia per la gestione dei servizi di rubrica corrispondenti
    /// </summary>
    public interface IRubricaServices : IBusinessService
    {
        /// <summary>
        /// Servizio per il reperimento degli elementi in rubrica a partire dai dati di filtro impostati
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        GetElementiResponse GetElementi(GetElementiRequest request);

        /// <summary>
        /// Servizio per il reperimento dei dettagli di un corrispondente in rubrica
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        GetDettaglioElementoResponse GetDettaglioElemento(GetDettaglioElementoRequest request);

        /// <summary>
        /// Servizio per il salvataggio dei dettagli di un corrispondente in rubrica
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        SaveElementiResponse SaveElementi(SaveElementiRequest request);

        /// <summary>
        /// Servizio per la cancellazione di un elemento da rubrica
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        DeleteElementoResponse DeleteElemento(DeleteElementoRequest request);
    }
}
