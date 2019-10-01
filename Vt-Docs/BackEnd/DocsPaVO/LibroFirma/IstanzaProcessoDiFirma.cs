using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.LibroFirma
{
    public enum TipoStatoProcesso
    {
        IN_EXEC,
        STOPPED,
        CLOSED,
        CLOSED_WITH_CUT,
        IN_ERROR,
        REPLAY
    }

    public class IstanzaProcessoDiFirma
    {
        #region private fields

        private string _idIstanzaProcesso; //ID_ISTANZA
        private string _idProcesso; //ID_PROCESSO
        private TipoStatoProcesso _statoProcesso; //STATO
       
        private string _docAll; //DOC_ALL
        private string _docNumber; //ID_DOCUMENTO
        private string _dataCreazione; //DATA CREAZIONE
        private string _numeroProtocollo; //NUMERO DI PROTOCOLLO
        private string _dataProtocollo; //DATA PROTOCOLLO
        private string _segnaturaRepertorio; //NUMERO DI REPERTORIO
        private string _oggetto;
       
        private string _versionId; //VERSION_ID
        private string _dataAttivazione; //ATTIVATO_IL
        private string _dataChiusura; //CONCLUSO_IL
        private string _numeroAllegato; //NUM_ALL
        private DocsPaVO.utente.Ruolo _ruoloProponente; //ID_RUOLO_PROPONENTE
        private DocsPaVO.utente.Utente _utenteProponente; //ID_UTENTE_PROPONENTE
        private string _descUtenteDelegato; //DESCRIZIONE DELL'UTENTE DELEGATO
        private int _numeroVersione; //NUM_VERSIONE
        private OpzioniNotifica _notifiche;
        private string _descrizione; //DESCRIZIONE
        private List<IstanzaPassoDiFirma> _istanzePassoDiFirma;
        private string _noteDiAvvio; //NOTE_AVVIO
        private string _motivoResingimento;
        private char _chaInterrotoDa; //CHA_INTERROTTO_DA
        private string _descUtenteInterruzione; //DESCRIZIONE UTENTE CHE HA INTERROTTO IL PROCESSO
        private string _descUtenteDelegatoInterruzione; //DESCRIZIONE UTENTE DELEGATO CHE HA INTERROTTO IL PROCESSO
        private bool _isTroncato;
        private bool _attivatoPerPassaggioStato = false; //ATTIVATO PER PASSAGGIO DI STATO
        #endregion

        #region public property



        /// <summary>
        /// System id dell'istanza processo di firma
        /// </summary>
        public string idIstanzaProcesso
        {
            get
            {
                return _idIstanzaProcesso;
            }

            set
            {
                _idIstanzaProcesso = value;
            }
        }
        /// <summary>
        /// System id del processo di firma da cui deriva
        /// </summary>
        public string idProcesso
        {
            get
            {
                return _idProcesso;
            }

            set
            {
                _idProcesso = value;
            }
        }

        /// <summary>
        /// System id dello stato del processo
        /// </summary>
        public TipoStatoProcesso statoProcesso
        {
            get
            {
                return _statoProcesso;
            }

            set
            {
                _statoProcesso = value;
            }
        }

        /// <summary>
        /// Contraddistingue il file come Documento o Allegato (DOC - ALL)
        /// </summary>
        public string docAll
        {
            get
            {
                return _docAll;
            }

            set
            {
                _docAll = value;
            }
        }

        /// <summary>
        /// Id del documento
        /// </summary>
        public string docNumber
        {
            get
            {
                return _docNumber;
            }

            set
            {
                _docNumber = value;
            }
        }

        /// <summary>
        /// Data creazione del docuemnto
        /// </summary>
        public string DataCreazione
        {
            get
            {
                return _dataCreazione;
            }
            set
            {
                _dataCreazione = value;
            }
        }


        /// <summary>
        /// Numero del protocollo
        /// </summary>
        public string NumeroProtocollo
        {
            get
            {
                return _numeroProtocollo;
            }
            set
            {
                _numeroProtocollo = value;
            }
        }

        /// <summary>
        /// Data del protocollo
        /// </summary>
        public string DataProtocollazione
        {
            get
            {
                return _dataProtocollo;
            }
            set
            {
                _dataProtocollo = value;
            }
        }

        /// <summary>
        /// Segnatura di repertorio
        /// </summary>
        public string SegnaturaRepertorio
        {
            get
            {
                return _segnaturaRepertorio;
            }
            set
            {
                _segnaturaRepertorio = value;
            }
        }

        /// <summary>
        /// Oggetto del documento
        /// </summary>
        public string oggetto
        {
            get
            {
                return _oggetto;
            }

            set
            {
                _oggetto = value;
            }
        }

        /// <summary>
        /// Id della version
        /// </summary>
        public string versionId
        {
            get { return _versionId; }
            set { _versionId = value; }
        }

        /// <summary>
        /// Ordine dell'allegato
        /// </summary>
        public string numeroAllegato
        {
            get { return _numeroAllegato; }
            set { _numeroAllegato = value; }
        }

        /// <summary>
        /// Numero della versione
        /// </summary>
        public int numeroVersione
        {
            get { return _numeroVersione; }
            set { _numeroVersione = value; }
        }

        /// <summary>
        /// Data in cui è stato attivato il passo
        /// </summary>
        public string dataAttivazione
        {
            get
            {
                return _dataAttivazione;
            }

            set
            {
                _dataAttivazione = value;
            }
        }

        /// <summary>
        /// Data in cui è terminato il processo
        /// </summary>
        public string dataChiusura
        {
            get
            {
                return _dataChiusura;
            }

            set
            {
                _dataChiusura = value;
            }
        }
        
        /// <summary>
        /// Ruolo del proponente
        /// </summary>
        public DocsPaVO.utente.Ruolo RuoloProponente
        {
            get { return _ruoloProponente; }
            set { _ruoloProponente = value; }
        }

        /// <summary>
        /// Utente proponente
        /// </summary>
        public DocsPaVO.utente.Utente UtenteProponente
        {
            get { return _utenteProponente; }
            set { _utenteProponente = value; }
        }

        /// <summary>
        /// Descrizione dell'utente delegante
        /// </summary>
        public string DescUtenteDelegato
        {
            get { return _descUtenteDelegato; }
            set { _descUtenteDelegato = value; }
        }


        public List<IstanzaPassoDiFirma> istanzePassoDiFirma
        {
            get
            {
                return _istanzePassoDiFirma;
            }
            set
            {
                _istanzePassoDiFirma = value;
            }
        }

        /// <summary>
        /// Nome del processo al momento della creazione dell'istanza
        /// </summary>
        public string Descrizione
        {
            get { return _descrizione; }
            set { _descrizione = value; }
        }

        /// <summary>
        /// Note di avvio del processo
        /// </summary>
        public string NoteDiAvvio
        {
            get { return _noteDiAvvio; }
            set { _noteDiAvvio = value; }
        }


        public string MotivoRespingimento
        {
            get { return _motivoResingimento; }
            set { _motivoResingimento = value; }
        }

        /// <summary>
        /// Tipo utente che ha interrotto il processo
        /// </summary>
        public char ChaInterroDa
        {
            get { return _chaInterrotoDa; }
            set { _chaInterrotoDa = value; }
        }

        /// <summary>
        /// Utente che ha interrotto l'istanza di processo
        /// </summary>
        public string DescUtenteInterruzione
        {
            get { return _descUtenteInterruzione; }
            set { _descUtenteInterruzione = value; }
        }

        /// <summary>
        /// Utente delegato che ha interrotto l'istanza di processo
        /// </summary>
        public string DescUtenteDelegatoInterruzione
        {
            get { return _descUtenteDelegatoInterruzione; }
            set { _descUtenteDelegatoInterruzione = value; }
        }

        /// </summary>
        public bool IsTroncato
        {
            get { return _isTroncato; }
            set { _isTroncato = value; }
        }

        public bool AttivatoPerPassaggioStato
        {
            get
            {
                return _attivatoPerPassaggioStato;
            }

            set
            {
                _attivatoPerPassaggioStato = value;
            }
        }

        public OpzioniNotifica Notifiche
        {
            get { return _notifiche; }
            set { _notifiche = value; }
        }
        #endregion
    }
}
