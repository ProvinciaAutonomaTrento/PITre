using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Document
{
    /// <summary>
    /// Classe request per il metodo "Ricerca"
    /// </summary>
    [Serializable()]
    public class RicercaRequest : Request
    {
        /// <summary>
        /// 
        /// </summary>
        [Serializable()]
        public class CampoRicerca
        {
            /// <summary>
            /// Nome campo da considerare nella ricerca
            /// </summary>
            public string NomeCampo
            {
                get;
                set;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Serializable()]
        public class CriterioOrdinamento
        {
            /// <summary>
            /// 
            /// </summary>
            public CriterioOrdinamento()
            { }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="campo"></param>
            /// <param name="direzione"></param>
            public CriterioOrdinamento(CampoRicerca campo, DirezioneOrdinamentoEnum direzione)
            {
                this.Campo = campo;
                this.Direzione = direzione;
            }

            /// <summary>
            /// 
            /// </summary>
            public enum DirezioneOrdinamentoEnum
            {
                ASC,
                DESC
            }

            /// <summary>
            /// 
            /// </summary>
            public CampoRicerca Campo
            {
                get;
                set;
            }

            /// <summary>
            /// 
            /// </summary>
            public DirezioneOrdinamentoEnum Direzione
            {
                get;
                set;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public RicercaRequest()
        {
            this.SecurityCheck = true;

            // Predisposizione di filtri di ricerca predefiniti
            this.FiltriRicerca = new FiltriRicercaDocumenti
            {
                GetNonProtocollati = true,
                GetProtocolliArrivo = true,
                GetProtocolliInterni = true,
                GetProtocolliPartenza = true,
                AnnoProtocollazione = DateTime.Today.Year
            };
        }

        /// <summary>
        /// 
        /// </summary>
        [Serializable()]
        public class FiltriRicercaDocumenti
        {
            /// <summary>
            /// Identificazione del codice dell'applicazione proprietaria per cui reperire i documenti
            /// </summary>
            public string CodiceApplicazione
            {
                get;
                set;
            }

            /// <summary>
            /// Filtro per oggetto documento
            /// </summary>
            public string Oggetto
            {
                get;
                set;
            }

            /// <summary>
            /// Identificativo della tipologia documento da filtrare
            /// </summary>
            public string IdTipoDocumento
            {
                get;
                set;
            }

            /// <summary>
            /// Identificativo dello stato del documento per cui filtrare
            /// </summary>
            public string IdStatoDocumento
            {
                get;
                set;
            }

            /// <summary>
            /// Indica al servizio se restituire i protocolli in arrivo
            /// </summary>
            public bool GetProtocolliArrivo
            {
                get;
                set;
            }

            /// <summary>
            /// Indica al servizio se restituire i protocolli in uscita
            /// </summary>
            public bool GetProtocolliPartenza
            {
                get;
                set;
            }

            /// <summary>
            /// Indica al servizio se restituire i protocolli internni
            /// </summary>
            public bool GetProtocolliInterni
            {
                get;
                set;
            }

            /// <summary>
            /// Indica al servizio se restituire i documenti non protocollati
            /// </summary>
            public bool GetNonProtocollati
            {
                get;
                set;
            }

            /// <summary>
            /// Filtro per anno di protocollazione
            /// </summary>
            public int AnnoProtocollazione
            {
                get;
                set;
            }

            /// <summary>
            /// 
            /// </summary>
            public List<FiltroRicercaCampoComune> CampiComuni
            {
                get;
                set;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Serializable()]
        public class FiltroRicercaCampoComune
        {
            /// <summary>
            /// 
            /// </summary>
            public string Nome
            {
                get;
                set;
            }

            /// <summary>
            /// 
            /// </summary>
            public string Valore
            {
                get;
                set;
            }
        }

        /// <summary>
        /// Contesto di paginazione corrente
        /// </summary>
        public PagingContext PagingContext
        {
            get;
            set;
        }

        /// <summary>
        /// Specifica se il servizio debba o meno applicare i vincoli di visibilità
        /// </summary>
        public bool SecurityCheck
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se restituire o meno per ogni documento la segnatura di repertorio
        /// </summary>
        public bool GetSegnaturaRepertorio
        {
            get;
            set;
        }

        /// <summary>
        /// Lista dei campi aggiuntivi richiesti in output
        /// </summary>
        public CampoRicerca[] CampiRichiestiInOutput
        {
            get;
            set;
        }

        /// <summary>
        /// Criterio di ordinamento dei dati
        /// </summary>
        public CriterioOrdinamento Ordinamento
        {
            get;
            set;
        }

        /// <summary>
        /// Filtri di ricerca applicati
        /// </summary>
        public FiltriRicercaDocumenti FiltriRicerca
        {
            get;
            set;
        }

        /// <summary>
        /// Filtri di ricerca applicati
        /// </summary>
        public DocsPaVO.filtri.FiltroRicerca[] FiltriRicercaLegacy
        {
            get;
            set;
        }
    }
}
