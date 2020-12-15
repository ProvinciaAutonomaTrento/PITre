using System;
using System.Runtime.Serialization;

namespace P3SBCLib.Contracts
{
    /// <summary>
    /// Contratto per far transitare ai client le eccezioni verificatesi nel servizio
    /// </summary>
    [DataContract(Name = "P3OperationFault", Namespace = "http://valueteam.com/CustomServices/P3SBC")]
    [Serializable()]
    public partial class P3OperationFault
    {
        /// <summary>
        /// Testo dell'eccezione verificatasi
        /// </summary>
        [DataMember()]
        public string Message
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Criteri necessari per la paginazione dei dati nelle ricerche degli oggetti in PITRE 
    /// </summary>
    [DataContract(Name = "CriteriPaginazione", Namespace = "http://valueteam.com/CustomServices/P3SBC")]
    public partial class CriteriPaginazione
    {
        /// <summary>
        /// Indica il numero di pagina richiesto 
        /// </summary>
        [DataMember(IsRequired = true)]
        public int Pagina
        {
            get;
            set;
        }

        /// <summary>
        /// Indica il numero di oggetti da includere nella ricerca per ciascuna pagina 
        /// </summary>
        [DataMember]
        public int OggettiPerPagina
        {
            get;
            set;
        }

        /// <summary>
        /// Indica il numero totale delle pagine restituite dalla ricerca 
        /// </summary>
        /// <remarks>
        /// Il dato è disponibile solamente come risultato della ricerca effettuata 
        /// </remarks>
        [DataMember]
        public int TotalePagine
        {
            get;
            set;
        }

        /// <summary>
        /// Indica il numero totale di oggetti (non paginati) restituiti dalla ricerca 
        /// </summary>
        /// <remarks>
        /// Il dato è disponibile solamente come risultato della ricerca effettuata 
        /// </remarks>
        [DataMember]
        public int TotaleOggetti
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Dati del file associato o da associare ad un Documento in SBC
    /// </summary>
    [DataContract(Name = "Documento", Namespace = "http://valueteam.com/CustomServices/P3SBC")]
    public partial class Documento
    {
        /// <summary>
        /// Nome del file con estensione nel documentale PITRE
        /// </summary>
        /// <remarks>
        /// Il file si riferisce al documento acquisito per la versione corrente del documento
        /// </remarks>
        [DataMember]
        public string Nomefile
        {
            get;
            set;
        }

        /// <summary>
        /// Formato MIME del file 
        /// </summary>
        [DataMember]
        public string MimeType
        {
            get;
            set;
        }

        /// <summary>
        /// File associato in formato di byte array 
        /// </summary>
        [DataMember]
        public byte[] Content
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Dati di un documento SBC
    /// </summary>
    [DataContract(Name = "InfoDocumento", Namespace = "http://valueteam.com/CustomServices/P3SBC")]
    public class InfoDocumento
    {
        /// <summary>
        /// Numero del documento
        /// </summary>
        [DataMember]
        public string Numero
        {
            get;
            set;
        }

        /// <summary>
        /// Se il documento è protocollato, riporta il numero del protocollo
        /// </summary>
        [DataMember]
        public string Protocollo
        {
            get;
            set;
        }

        /// <summary>
        /// Se il documento è protocollato, riporta la segnatura del protocollo 
        /// </summary>
        [DataMember]
        public string Segnatura
        {
            get;
            set;
        }

        /// <summary>
        /// Se il documento è protocollato rappresenta la data del protocollo,
        /// altrimenti riporta la data di creazione del documento
        /// </summary>
        [DataMember]
        public DateTime Data
        {
            get;
            set;
        }

        /// <summary>
        /// Tipologia del provvedimento in SBC 
        /// </summary>
        /// <remarks>
        /// Corrisponde alla tipologia del documento in PITRE, rappresenta due tipi di documenti:
        /// - Documento di Apertura
        /// - Provvedimento
        /// </remarks>
        [DataMember]
        public string Tipo
        {
            get;
            set;
        }

        /// <summary>
        /// Tipologia del provvedimento in SBC 
        /// </summary>
        /// <remarks>
        /// Corrisponde alla tipologia reale in SBC del documento di apertura pratica o del provvedimento
        /// </remarks>
        [DataMember]
        public string SottoTipologia
        {
            get;
            set;
        }

        /// <summary>
        /// Se il documento è un protocollo in ingresso, rappresenta il soggetto mittente
        /// </summary>
        [DataMember]
        public string Soggetto
        {
            get;
            set;
        }

        /// <summary>
        /// Oggetto del documento
        /// </summary>
        [DataMember]
        public string Oggetto
        {
            get;
            set;
        }

        /// <summary>
        /// Utente creatore del documento
        /// </summary>
        [DataMember]
        public string OperatoreInserimento
        {
            get;
            set;
        }

        /// <summary>
        /// Ultima nota visibile dall'utente corrente associata al documento
        /// </summary>
        [DataMember]
        public string Note
        {
            get;
            set;
        }

        /// <summary>
        /// Stato del documento
        /// </summary>
        [DataMember]
        public string Stato
        {
            get;
            set;
        }

        /// <summary>
        /// Indica che è stata impostata un'istanza di un oggetto documento
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public bool DocumentoSpecified
        {
            get;
            set;
        }

        /// <summary>
        /// Nome del file con estensione nel documentale PITRE
        /// </summary>
        /// <remarks>
        /// Il file si riferisce al documento acquisito per la versione corrente del documento
        /// </remarks>
        [DataMember]
        public string Nomefile
        {
            get;
            set;
        }

        /// <summary>
        /// Dati del file associato alla versione corrente del documento
        /// </summary>
        [DataMember]
        public Documento Documento
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Esito della ricerca documenti effettuata in SBC
    /// </summary>
    [DataContract(Name = "ElencoInfoDocumenti", Namespace = "http://valueteam.com/CustomServices/P3SBC")]
    public class ElencoInfoDocumenti
    {
        /// <summary>
        /// Documenti trovati con il criterio di filtro impostato
        /// </summary>
        [DataMember]
        public InfoDocumento[] Documenti
        {
            get;
            set;
        }

        /// <summary>
        /// Criteri di paginazione utilizzati per la ricerca
        /// </summary>
        [DataMember]
        public CriteriPaginazione Paginazione
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Definizione dei filtri di ricerca dei documenti in PITRE 
    /// </summary>
    [DataContract(Name = "FiltroRicercaDocumenti", Namespace = "http://valueteam.com/CustomServices/P3SBC")]
    public class FiltroRicercaDocumenti
    {
        /// <summary>
        /// Filtro per numero documento in SBC
        /// </summary>
        /// <remarks>
        /// Corrisponde all'id del documento in PITRE
        /// </remarks>
        [DataMember]
        public string Numero
        {
            get;
            set;
        }

        /// <summary>
        /// Filtro per numero di protocollo di un documento in SBC
        /// </summary>
        [DataMember]
        public string Protocollo
        {
            get;
            set;
        }

        /// <summary>
        /// Filtro per segnatura di protocollo di un documento in SBC
        /// </summary>
        [DataMember]
        public string Segnatura
        {
            get;
            set;
        }

        /// <summary>
        /// Filtro per tipologia del documento
        /// </summary>
        [DataMember]
        public string TipoDocumento
        {
            get;
            set;
        }

        /// <summary>
        /// Filtro per soggetto
        /// </summary>
        /// <remarks>
        /// In PITRE corrisponde ai soggetti mittente / destinatari di un protocollo
        /// </remarks>
        [DataMember]
        public string Soggetto
        {
            get;
            set;
        }

        /// <summary>
        /// Filtro per oggetto del documento
        /// </summary>
        [DataMember]
        public string Oggetto
        {
            get;
            set;
        }

        /// <summary>
        /// Filtro per data documento
        /// </summary>
        /// <remarks>
        /// In PITRE corrisponde alla data di protocollo
        /// </remarks>
        [DataMember]
        public Nullable<DateTime> DataDocumento
        {
            get;
            set;
        }

        /// <summary>
        /// Filtro per data documento finale
        /// </summary>
        /// <remarks>
        /// In PITRE corrisponde alla data di protocollo.
        /// Valido solo se viene specificata la DataDocumento ai fini della ricerca dei protocolli per intervallo di date
        /// </remarks>
        [DataMember]
        public Nullable<DateTime> DataDocumentoFinale
        {
            get;
            set;
        }

        /// <summary>
        /// Filtro per data di creazione del documento in SBC
        /// </summary>
        [DataMember]
        public Nullable<DateTime> DataCreazioneDocumento
        {
            get;
            set;
        }

        /// <summary>
        /// Filtro per data di creazione del documento in SBC
        /// </summary>
        /// <remarks>
        /// Valido solo se viene specificata la DataCreazioneDocumento ai fini della ricerca dei documenti per intervallo di date
        /// </remarks>
        [DataMember]
        public Nullable<DateTime> DataCreazioneDocumentoFinale
        {
            get;
            set;
        }

        /// <summary>
        /// Filtro che si riferisce alla pratica in cui cercare i documenti
        /// </summary>
        [DataMember]
        public string NumeroPratica
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Dati di un bene oggetto della pratica in SBC 
    /// </summary>
    /// <remarks>
    /// Corrisponde ad una serie di attributi personalizzati di una tipologia fascicolo in PITRE
    /// </remarks>
    [DataContract(Name = "Bene", Namespace = "http://valueteam.com/CustomServices/P3SBC")]
    public partial class Bene
    {
        /// <summary>
        /// Codice del bene in SBC 
        /// </summary>
        /// <remarks>
        /// In PITRE corrisponde ad un campo personalizzato del fascicolo di una particolare tipologia denominato "Codice"
        /// </remarks>
        [DataMember]
        public string Codice
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione del bene in SBC 
        /// </summary>
        /// <remarks>
        /// In PITRE corrisponde ad un campo personalizzato del fascicolo di una particolare tipologia denominato "Descrizione"
        /// </remarks>
        [DataMember]
        public string Descrizione
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Dati di una pratica in SBC 
    /// </summary>
    /// <remarks>
    /// In PITRE corrisponde ad un fascicolo di una particolare tipoligia 
    /// </remarks>
    [DataContract(Name = "Pratica", Namespace = "http://valueteam.com/CustomServices/P3SBC")]
    public partial class Pratica
    {
        /// <summary>
        /// Codice classificazione della pratica in SBC
        /// </summary>
        /// <remarks>
        /// Corrisponde al codice del nodo titolario di appartenenza del fascicolo in PITRE
        /// </remarks>
        [DataMember]
        public string ClassificazionePratica
        {
            get;
            set;
        }

        /// <summary>
        /// Tipologia della pratica in SBC 
        /// </summary>
        /// <remarks>
        /// Corrisponde alla tipologia del fascicolo in PITRE 
        /// </remarks>
        [DataMember]
        public string Tipo
        {
            get;
            set;
        }

        /// <summary>
        /// Numero della pratica in SBC 
        /// </summary>
        /// <remarks>
        /// Corrisponde al codice del fascicolo in PITRE 
        /// </remarks>
        [DataMember]
        public string Numero
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione della pratica in SBC 
        /// </summary>
        /// <remarks>
        /// Corrisponde alla descrizione del fascicolo in PITRE 
        /// </remarks>
        [DataMember]
        public string Descrizione
        {
            get;
            set;
        }

        /// <summary>
        /// Data di apertura della pratica in SBC 
        /// </summary>
        /// <remarks>
        /// Corrisponde alla data di apertura del fascicolo in PITRE 
        /// </remarks>
        [DataMember]
        public DateTime DataApertura
        {
            get;
            set;
        }

        /// <summary>
        /// Indica che è stata impostata l'istanza di un bene
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public bool BeneSpecified
        {
            get;
            set;
        }

        /// <summary>
        /// Dati del bene oggetto della pratica in SBC 
        /// </summary>
        /// <remarks>
        /// Corrisponde ad una serie di attributi personalizzati di una tipologia fascicolo in PITRE 
        /// </remarks>
        [DataMember]
        public Bene Bene
        {
            get;
            set;
        }

        /// <summary>
        /// Indica che è stata impostata l'istanza di un protocollo
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public bool ProtocolloSpecified
        {
            get;
            set;
        }

        /// <summary>
        /// Dati del documento di richiesta iniziale della pratica 
        /// </summary>
        /// <remarks>
        /// In PITRE corrisponde ad un protocollo in ingresso 
        /// </remarks>
        [DataMember]
        public InfoDocumento Protocollo
        {
            get;
            set;
        }

        /// <summary>
        /// Indica che è stata impostata l'istanza di un provvedimento
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public bool ProvvedimentoSpecified
        {
            get;
            set;
        }

        /// <summary>
        /// Dati del provvedimento finale associato alla pratica in SBC
        /// </summary>
        /// <remarks>
        /// In PITRE corrisponde ad un documento di una particolare tipologia inserito nel fascicolo (che rappresenta la pratica in SBC)
        /// </remarks>
        [DataMember]
        public InfoDocumento Provvedimento
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Esito della ricerca documenti effettuata in SBC
    /// </summary>
    [DataContract(Name = "ElencoPratiche", Namespace = "http://valueteam.com/CustomServices/P3SBC")]
    public class ElencoPratiche
    {
        /// <summary>
        /// Criteri di paginazione utilizzati per la ricerca
        /// </summary>
        [DataMember]
        public CriteriPaginazione Paginazione
        {
            get;
            set;
        }

        /// <summary>
        /// Pratiche trovate con il criterio di filtro impostato
        /// </summary>
        [DataMember]
        public Pratica[] Pratiche
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Definizione dei filtri di ricerca per le pratiche in PITRE 
    /// </summary>
    [DataContract(Name = "FiltriRicercaPratiche", Namespace = "http://valueteam.com/CustomServices/P3SBC")]
    public partial class FiltriRicercaPratiche
    {
        /// <summary>
        /// Filtro per la classificazione della pratica SBC
        /// </summary>
        /// <remarks>
        /// Una volta stabilito il titolario in SBC, sarà assegnato un nodo di classificazione cui le amministrazioni dei beni culturali lavoreranno 
        /// </remarks>
        [DataMember(IsRequired = true)]
        public string ClassificazionePratica
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se ricercare le pratiche SBC anche nei nodi figli della classificazione richiesta
        /// </summary>
        [DataMember()]
        public bool CercaInClassificazioniFiglie
        {
            get;
            set;
        }

        /// <summary>
        /// Intervallo iniziale all'interno del quale dovrebbe trovarsi la pratica 
        /// </summary>
        [DataMember()]
        public Nullable<DateTime> DataAperturaPratica
        {
            get;
            set;
        }

        /// <summary>
        /// Intervallo finale all'interno del quale dovrebbe trovarsi la pratica 
        /// </summary>
        /// <remarks>
        /// Valido solo se viene specificata la DataAperturaPratica ai fini della ricerca dei protocolli per intervallo di date
        /// </remarks>
        [DataMember()]
        public Nullable<DateTime> DataAperturaPraticaFinale
        {
            get;
            set;
        }

        /// <summary>
        /// Filtro per i beni contenuti nelle pratiche SBC
        /// </summary>
        [DataMember]
        public string[] CodiciBene
        {
            get;
            set;
        }

        /// <summary>
        /// Filtro per numero della pratica in SBC 
        /// </summary>
        /// <remarks>
        /// Corrisponde al codice del fascicolo in PITRE 
        /// </remarks>
        [DataMember]
        public string NumeroPratica
        {
            get;
            set;
        }

        /// <summary>
        /// Filtro per tipologia della pratica in SBC 
        /// </summary>
        /// <remarks>
        /// Corrisponde alla tipologia del fascicolo in PITRE 
        /// </remarks>
        [DataMember]
        public string TipoPratica
        {
            get;
            set;
        }

        /// <summary>
        /// Filtro per numero del provvedimento della pratica in SBC 
        /// </summary>
        /// <remarks>
        /// Corrisponde all'id del documento contenuto nella pratica in PITRE
        /// </remarks>
        [DataMember]
        public string NumeroProvvedimento
        {
            get;
            set;
        }

        /// <summary>
        /// Consente di filtrare la ricerca alle sole pratiche del titolario opportuno che non risultano già associate a un Bene
        /// </summary>
        [DataMember]
        public bool SoloPraticheNonAssegnate
        {
            get;
            set;
        }

        /// <summary>
        /// Filtro di ricerca inerente i documenti contenuti nella pratica
        /// </summary>
        /// <remarks>
        /// Se specificato, la ricerca riporterà solamente le pratiche 
        /// contenenti i documenti estratti in base alla definizione del filtro
        /// </remarks>
        [DataMember]
        public FiltroRicercaDocumenti FiltroDocumenti
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Rappresenta un account utente in PITRE 
    /// </summary>
    /// <remarks>
    /// PITRE al momento non utilizza LDAP ma un proprio sistema di autenticazione basato su ruoli i cui metadati risiedono sul proprio database. Il portale esterno SBC, tramite gli attributi definiti dalla classe, può "associare" i propri utenti di portale (presenti in apposito ramo LDAP, i quali si considerano già esistenti in PITRE) con i corrispondenti utenti PITRE. 
    /// </remarks>
    [DataContract(Name = "Utente", Namespace = "http://valueteam.com/CustomServices/P3SBC")]
    public partial class Utente
    {
        /// <summary>
        /// UserId dell'utente
        /// </summary>
        [DataMember()]
        public string UserId
        {
            get;
            set;
        }

        /// <summary>
        /// Nome dell'utente
        /// </summary>
        [DataMember()]
        public string Nome
        {
            get;
            set;
        }

        /// <summary>
        /// Cognome dell'utente
        /// </summary>
        [DataMember()]
        public string Cognome
        {
            get;
            set;
        }

        /// <summary>
        /// Email dell'utente
        /// </summary>
        [DataMember()]
        public string Email
        {
            get;
            set;
        }

        /// <summary>
        /// Sede di appartenenza dell'utente
        /// </summary>
        [DataMember()]
        public string Sede
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Esito della ricerca documenti effettuata in SBC
    /// </summary>
    [DataContract(Name = "ElencoUtenti", Namespace = "http://valueteam.com/CustomServices/P3SBC")]
    public partial class ElencoUtenti
    {
        /// <summary>
        /// Criteri di paginazione utilizzati per la ricerca
        /// </summary>
        [DataMember]
        public CriteriPaginazione Paginazione
        {
            get;
            set;
        }

        /// <summary>
        /// Utenti trovati con il criterio di filtro impostato
        /// </summary>
        [DataMember]
        public Utente[] Utenti
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Definizione dei filtri di ricerca per gli utenti in PITRE
    /// </summary>
    [DataContract(Name = "FiltriRicercaUtenti", Namespace = "http://valueteam.com/CustomServices/P3SBC")]
    public partial class FiltriRicercaUtenti
    {
        /// <summary>
        /// Classificazioni cui gli utenti da estrarre devono avere la visibilità
        /// </summary>
        [DataMember(IsRequired = true)]
        public string[] Classificazioni
        {
            get;
            set;
        }

        /// <summary>
        /// Filtro per UserId che inizia con
        /// </summary>
        [DataMember]
        public string UserId
        {
            get;
            set;
        }

        /// <summary>
        /// Filtro per Nome che inzia con
        /// </summary>
        [DataMember]
        public string Nome
        {
            get;
            set;
        }

        /// <summary>
        /// Filtro per Cognome che inizia con
        /// </summary>
        [DataMember]
        public string Cognome
        {
            get;
            set;
        }

        /// <summary>
        /// Filtro per Email che inizia con
        /// </summary>
        [DataMember()]
        public string Email
        {
            get;
            set;
        }

        /// <summary>
        /// Filtro per Sede che inizia con
        /// </summary>
        [DataMember()]
        public string Sede
        {
            get;
            set;
        }
    }
}
