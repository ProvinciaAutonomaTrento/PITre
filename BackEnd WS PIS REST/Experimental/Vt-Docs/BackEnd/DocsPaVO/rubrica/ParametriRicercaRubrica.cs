using System;
using System.Xml.Serialization;

namespace DocsPaVO.rubrica
{
    /// <summary>
    /// I parametri con cui effettuare la ricerca
    /// </summary>
    /// <remarks>
    /// Questa classe viene istanziata e riempita dall'applicazione client 
    /// (il frontend di una qualsiasi versione di DocsPA interfacciata
    /// con la rubrica) con i parametri relativi alla ricerca da
    /// effettuare. L'istanza di 
    /// <see cref="DocsPaVO.rubrica.ParametriRicercaRubrica">ParametriRicercaRubrica</see> viene
    /// poi passata ad uno dei metodi definiti in una classe derivata
    /// da <see cref="DocsPaWS.utenti.RubricaSearchAgent">RubricaSearchAgent</see>.
    /// </remarks>
    [Serializable]
    [XmlType("ParametriRicercaRubrica")]
    public class ParametriRicercaRubrica : MarshalByRefObject
    {
        /// <summary>
        /// Il tipo di chiamata (ricerca) che la rubrica è chiamata ad effettuare
        /// </summary>
        /// <remarks>
        /// Questo enumeratore permette di definire il tipo di chiamata (ricerca)
        /// che la rubrica è chiamata ad effettuare. In base al tipo di chiamata
        /// selezionato la rubrica imposta alcuni parametri di visualizzazione e 
        /// di ricerca.
        /// </remarks>
        [Serializable]
        [XmlType("RubricaCallType")]  /*IMPORTANTE: INSERIRE I NUOVI CALLTYPE ALLA FINE */
        public enum CallType
        {
            /// <summary>
            /// Protocollo in entrata
            /// </summary>
            CALLTYPE_PROTO_IN,

            /// <summary>
            /// Protocollo in entrata -  mittente originario
            /// </summary>
            CALLTYPE_PROTO_IN_INT,

            /// <summary>
            /// Protocollo in uscita
            /// </summary>
            CALLTYPE_PROTO_OUT,

            /// <summary>
            /// Trasmissione ai gerarchicamente inferiori
            /// </summary>
            CALLTYPE_TRASM_INF,

            /// <summary>
            /// Trasmissione ai gerarchicamente superiori
            /// </summary>
            CALLTYPE_TRASM_SUP,

            /// <summary>
            /// Trasmissione a tutti
            /// </summary>
            CALLTYPE_TRASM_ALL,

            /// <summary>
            /// Gestione della rubrica. Nessun risultato da restituire
            /// </summary>
            CALLTYPE_MANAGE,

            /// <summary>
            /// Protocollo in uscita - mittente (in DocsPA3 è selezionabile da rubrica)
            /// </summary>
            CALLTYPE_PROTO_OUT_MITT,

            /// <summary>
            /// Protocollo interno - mittente (in DocsPA3 è selezionabile da rubrica)
            /// </summary>
            CALLTYPE_PROTO_INT_MITT,

            /// <summary>
            /// Protocollo in ingresso
            /// </summary>
            CALLTYPE_PROTO_INGRESSO,

            /// <summary>
            /// Protocollo in entrata - Ufficio Referente
            /// </summary>
            CALLTYPE_UFFREF_PROTO,

            /// <summary>
            /// Gestione fascicoli - Locazione fisica
            /// </summary>
            CALLTYPE_GESTFASC_LOCFISICA,

            /// <summary>
            /// Gestione fascicoli - Ufficio Referente
            /// </summary>
            CALLTYPE_GESTFASC_UFFREF,

            CALLTYPE_EDITFASC_LOCFISICA,

            CALLTYPE_EDITFASC_UFFREF,

            CALLTYPE_NEWFASC_LOCFISICA,

            CALLTYPE_NEWFASC_UFFREF,

            CALLTYPE_RICERCA_MITTDEST,
            CALLTYPE_RICERCA_UFFREF,
            CALLTYPE_RICERCA_MITTINTERMEDIO,

            CALLTYPE_RICERCA_ESTESA,

            CALLTYPE_RICERCA_COMPLETAMENTO,

            CALLTYPE_RICERCA_TRASM,

            CALLTYPE_PROTO_INT_DEST,

            CALLTYPE_FILTRIRICFASC_UFFREF,

            CALLTYPE_FILTRIRICFASC_LOCFIS,

            CALLTYPE_RICERCA_DOCUMENTI,

            CALLTYPE_RICERCA_DOCUMENTI_CORR_INT,

            CALLTYPE_LISTE_DISTRIBUZIONE,

            CALLTYPE_TRASM_PARILIVELLO,

            CALLTYPE_MITT_MODELLO_TRASM,

            CALLTYPE_MODELLI_TRASM_ALL,

            CALLTYPE_MODELLI_TRASM_INF,

            CALLTYPE_MODELLI_TRASM_SUP,

            CALLTYPE_MODELLI_TRASM_PARILIVELLO,

            CALLTYPE_DEST_MODELLO_TRASM,

            CALLTYPE_ORGANIGRAMMA,

            CALLTYPE_ORGANIGRAMMA_TOTALE_PROTOCOLLO,

            CALLTYPE_STAMPA_REG_UO,

            CALLTYPE_ORGANIGRAMMA_TOTALE,

            CALLTYPE_ORGANIGRAMMA_INTERNO,

            CALLTYPE_RICERCA_TRASM_TODOLIST,

            CALLTYPE_RUOLO_REG_NOMAIL,

            CALLTYPE_UTENTE_REG_NOMAIL,

            CALLTYPE_PROTO_USCITA_SEMPLIFICATO,

            CALLTYPE_PROTO_OUT_MITT_SEMPLIFICATO,

            CALLTYPE_CORR_INT,

            CALLTYPE_CORR_EST,

            CALLTYPE_CORR_INT_EST,

            CALLTYPE_CORR_NO_FILTRI,

            CALLTYPE_RICERCA_CREATOR,

            CALLTYPE_ESTERNI_AMM,

            CALLTYPE_PROTO_OUT_ESTERNI,

            CALLTYPE_PROTO_IN_ESTERNI,

            CALLTYPE_RUOLO_RESP_REG,

            /// <summary>
            /// Ricerca trasmissioni, solo ruoli sottoposti
            /// </summary>
            CALLTYPE_RICERCA_TRASM_SOTTOPOSTO,

            CALLTYPE_MITT_MULTIPLI,

            CALLTYPE_MITT_MULTIPLI_SEMPLIFICATO,

            CALLTYPE_RICERCA_UO_RUOLI_SOTTOPOSTI,

            CALLTYPE_TUTTI_RUOLI,

            CALLTYPE_TUTTE_UO,

            CALLTYPE_CORR_INT_NO_UO,

            /// <summary>
            /// Call type per la ricerca di ruoli non inibiti
            /// </summary>
            CALLTYPE_REPLACE_ROLE,

            /// <summary>
            /// Call type per la selezione di corrispondenti da ricercare
            /// come destinatari di un modello
            /// </summary>
            CALLTYPE_DEST_FOR_SEARCH_MODELLI,

            /// <summary>
            /// Call type per la selezione di corrispondenti da ricercare
            /// come origine della funzione trova e sostituisci
            /// </summary>
            CALLTYPE_FIND_ROLE,

            CALLTYPE_OWNER_AUTHOR,

            /// <summary>
            /// Call type responsabile repertori
            /// </summary>
            CALLTYPE_RUOLO_RESP_REPERTORI,

            CALLTYPE_RICERCA_CORRISPONDENTE,

            /// <summary>
            /// Visibilità Ricerca Utente
            /// </summary>
            CALLTYPE_VIS_UTENTE,

            /// <summary>
            /// Visibilità Ricerca Ruolo
            /// </summary>
            CALLTYPE_VIS_RUOLO,

            CALLTYPE_RICERCA_CORR_NON_STORICIZZATO,

            /// <summary>
            ///Giordano Iacozzilli 20/06/2013
            ///Deposito: Creo un nuov Call_Type per il deposito.
            /// </summary>

            CALLTYPE_DEP_OSITO,

            /// <summary>
            /// Calltype per la selezione di corrispondenti esterni ed
            /// interni senza escusione di quelli disabilitati
            /// </summary>
            CALLTYPE_CORR_INT_EST_CON_DISABILITATI,

            /// <summary>
            /// Calltype per la selezione di corrispondenti
            /// interni senza escusione di quelli disabilitati
            /// </summary>
            CALLTYPE_CORR_INT_CON_DISABILITATI,

            /// <summary>
            /// Calltype per la selezione di corrispondenti
            /// esterni senza escusione di quelli disabilitati
            /// </summary>
            CALLTYPE_CORR_EST_CON_DISABILITATI
        }


        /// <summary>
        /// L'identità dell'utente che effettua la ricerca
        /// </summary>
        /// <remarks>Questi campi possono venire utilizzati dai 
        /// metodi di ricerca nel caso servissero informazioni sul chiamante. 
        /// Questo avviene ad esempio nel caso delle trasmissioni, per poter ricercare
        /// correttamente i corrispondenti in base al tipo di trasmissione e
        /// alla relativa gerarchia di interesse.</remarks>
        [Serializable]
        [XmlType("RubricaCallerIdentity")]
        public class CallerIdentity
        {
            /// <summary>
            /// L'Id dell'utente che effettua la ricerca
            /// </summary>
            public string IdUtente;

            /// <summary>
            /// L'Id del ruolo selezionato per l'utente che effettua la ricerca
            /// </summary>
            public string IdRuolo;

            /// <summary>
            /// L'Id del registro selezionato per l'utente che effettua la ricerca
            /// </summary>
            public string IdRegistro;

            /// <summary>
            /// Contiene la stringa dei registri (uno o più registri concatenati) necessari
            /// per la ricerca in rubrica
            /// </summary>
            public string filtroRegistroPerRicerca;

            /// <summary>
            /// L'idPeople selezionato per l'utente che effettua la ricerca
            /// </summary>
            public string IdPeople;
        }

        /// <summary>
        /// Il codice del "padre" dei corrispondenti di cui effettuare la ricerca
        /// </summary>
        /// <remarks>
        /// Quando viene riempito questo campo, i campi <see cref="codice"/>, 
        /// <see cref="descrizione"/> e <see cref="citta"/> vengono ignorati.
        /// Le classi che implementano la ricerca resituiscono i "figli" di  
        /// primo livello dell'elemento selezionato per la ricerca.
        /// </remarks>
        public string parent;

        /// <summary>
        /// Il codice del corrispodennte o dei corrispondenti da ricercare
        /// </summary>
        /// <remarks>La ricerca sul codice viene eseguita anche su una parte
        /// del codice stesso (Es. "TBN" per trovare tutti gli elementi il 
        /// cui codice inizi o contenga questa stringa.</remarks>
        public string codice;



        /// <summary>
        /// se effettuando una ricerca per codice rubrica, ponendo questo parametro a true,
        /// la query è effettuata in modo esatto senza like.
        /// </summary>
        public bool queryCodiceEsatta;
        /// <summary>
        /// systemId del corrispondente (usato per ricercare un corrisp dopo una modifica/cancellazione)
        /// </summary>
        public string systemId;

        /// <summary>
        /// La descrizione del corrispodennte o dei corrispondenti da ricercare
        /// </summary>
        /// <remarks>La ricerca sulla descrizione viene eseguita anche su una parte
        /// del codice stesso (Es. "SCUOLA" per trovare tutti gli elementi il 
        /// cui codice inizi o contenga questa stringa.</remarks>
        public string descrizione;

        /// <summary>
        /// La descrizione del corrispodennte o dei corrispondenti da ricercare
        /// </summary>
        /// <remarks>Come prima, solo che utilizza una colonna indicizzata</remarks>
        public string descrizioneIndicizzata;

        /// <summary>
        /// La città in cui si trova il corrispodennte o i corrispondenti da ricercare
        /// </summary>
        /// <remarks>La ricerca sulla città viene eseguita anche su una parte
        /// del codice stesso (Es. "MILA" per trovare tutti gli elementi il 
        /// cui codice inizi o contenga questa stringa.</remarks>
        public string citta;


        /// <summary>
        /// email associata al corrispondente
        /// </summary>
        public string email;

        /// <summary>
        /// Il codice fiscale del corrispodennte o dei corrispondenti da ricercare
        /// </summary>
        /// <remarks>La ricerca sul codice fiscale viene eseguita anche su una parte
        /// del codice fiscale stesso.</remarks>
        public string codiceFiscale;


        /// <summary>
        /// La partita iva del corrispodennte o dei corrispondenti da ricercare
        /// </summary>
        /// <remarks>La ricerca sulla partita iva viene eseguita anche su una parte
        /// della partita iva stessa.</remarks>
        public string partitaIva;

        /// <summary>
        /// Effettua la ricerca tra le UO
        /// </summary>
        public bool doUo;

        /// <summary>
        /// Effettua la ricerca tra le liste di distribuzione
        /// </summary>
        public bool doListe;

        /// <summary>
        /// Effettua la ricerca tra i ruoli 
        /// </summary>
        public bool doRuoli;

        /// <summary>
        /// Effettua la ricerca tra gli utenti
        /// </summary>
        public bool doUtenti;

        /// <summary>
        /// Se true, effettua la ricerca degli elementi in rubrica comune
        /// </summary>
        public bool doRubricaComune;

        /// <summary>
        /// Ricerca tra gli interni, Esterni o entrambi
        /// </summary>
        public DocsPaVO.addressbook.TipoUtente tipoIE;

        /// <summary>
        /// Il tipo di chiamata effettuata alla rubrica
        /// </summary>
        public CallType calltype;

        /// <summary>
        /// L'identità del chiamante
        /// </summary>
        public CallerIdentity caller;


        public string ObjectType;

        /// <summary>
        /// Effettua la ricerca tra gli RF
        /// </summary>
        public bool doRF;

        /// <summary>
        /// Ricerca per Località
        /// </summary>
        public string localita;

        /// <summary>
        /// Ricerca per codice fiscale/p.iva
        /// </summary>
        public string cf_piva;

        /// <summary>
        /// Autenticazione Sistemi Esterni : stealth
        /// Ricerca anche tra i sistemi esterni
        /// </summary>
        public bool extSystems;
    }

}
