using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Document
{
    [Serializable()]
    public class GetStoricoDiagrammaProfilatiStatoResponse : Response
    {
        /// <summary>
        /// Lista di tutti i cambi di stato dei documenti
        /// </summary>
        public List<StoricoDiagrammaStato> ListaStoricoDiagrammaStato
        {
            get;
            set;
        }

        /// <summary>
        /// Lista di tutti i cambi di stato dei campi profilati
        /// </summary>
        public List<StoricoProfilatiStato> ListaStoricoProfilatiStato
        {
            get;
            set;
        }
    }
}
