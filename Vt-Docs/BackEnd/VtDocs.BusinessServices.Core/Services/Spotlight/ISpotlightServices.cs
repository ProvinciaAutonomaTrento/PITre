using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Spotlight
{
    /// <summary>
    /// Servizi di ricerca spotlight
    /// </summary>
    public interface ISpotlightService : VtDocs.BusinessServices.IBusinessService
    {
        /// <summary>
        /// Ricerca spotlight per i documenti
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        VtDocs.BusinessServices.Entities.Spotlight.SpotlightResponse SpotlightDocumenti(VtDocs.BusinessServices.Entities.Spotlight.SpotlightRequest request);

        /// <summary>
        /// Ricerca spotlight per i fascicoli
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        VtDocs.BusinessServices.Entities.Spotlight.SpotlightResponse SpotlightFascicoli(VtDocs.BusinessServices.Entities.Spotlight.SpotlightRequest request);
    }
}
