using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Document
{
    [Serializable()]
    public class GetStoricoDiagrammaProfilatiStatoRequest : Request
    {

        /// <summary>
        /// Numero identificativo del documento
        /// </summary>
        public int DocNumber
        {
            get;
            set;
        }
        
        /// <summary>
        /// TRUE se il servizio deve restituire la lista con tutti i cambiamenti di stato dei documenti 
        /// </summary>
        public bool DiagrammaStato
        {
            get;
            set;
        }

        /// <summary>
        /// TRUE se il servizio deve restituire la lista con tutti i cambiamenti di stato dei campi profilati
        /// </summary>
        public bool ProfilatiStato
        {
            get;
            set;
        }
    }
}
