using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Task
{
    public enum TipoTask
    {
        RICEVUTI,
        ASSEGNATI
    }

    public class Task
    {
        #region private fields

        private string _idTask;
        private DocsPaVO.utente.Ruolo _ruoloMittente;
        private DocsPaVO.utente.Utente _utenteMittente;
        private DocsPaVO.utente.Ruolo _ruoloDestinatario;
        private DocsPaVO.utente.Utente _utenteDestinatario;
        private string _idProject;
        private string _codProject;
        private string _idProfile;
        private string _descrizioneObject;
        private string _idProfileReview;
        private string _idTrasmissione;
        private string _idTrasmSingola;
        private string _idRagioneTrasm;
        private string _descRagione;
        private string _idTipoAtto;
        private string _contributoObbligatorio;
        private StatoTask _statoTask;

        #endregion

        #region public fields

        /// <summary>
        /// System_id del TASK
        /// </summary>
        public string ID_TASK
        {
            get
            {
                return _idTask;
            }

            set
            {
                _idTask = value;
            }
        }

        /// <summary>
        /// Ruolo mittente del TASK
        /// </summary>
        public DocsPaVO.utente.Ruolo RUOLO_MITTENTE
        {
            get
            {
                return _ruoloMittente;
            }

            set
            {
                _ruoloMittente = value;
            }
        }

        /// <summary>
        /// Utente mittente del TASK
        /// </summary>
        public DocsPaVO.utente.Utente UTENTE_MITTENTE
        {
            get
            {
                return _utenteMittente;
            }

            set
            {
                _utenteMittente = value;
            }
        }

        /// <summary>
        /// Ruolo destinatario del TASK
        /// </summary>
        public DocsPaVO.utente.Ruolo RUOLO_DESTINATARIO
        {
            get
            {
                return _ruoloDestinatario;
            }

            set
            {
                _ruoloDestinatario = value;
            }
        }

        /// <summary>
        /// Utente destinatario del TASK
        /// </summary>
        public DocsPaVO.utente.Utente UTENTE_DESTINATARIO
        {
            get
            {
                return _utenteDestinatario;
            }

            set
            {
                _utenteDestinatario = value;
            }
        }

        /// <summary>
        /// Id del fascicolo
        /// </summary>
        public string ID_PROJECT
        {
            get
            {
                return _idProject;
            }

            set
            {
                _idProject = value;
            }
        }

        /// <summary>
        /// Codice del fascicolo
        /// </summary>
        public string COD_PROJECT
        {
            get
            {
                return _codProject;
            }

            set
            {
                _codProject = value;
            }
        }

        /// <summary>
        /// Id del documento
        /// </summary>
        public string ID_PROFILE
        {
            get
            {
                return _idProfile;
            }

            set
            {
                _idProfile = value;
            }
        }

        /// <summary>
        /// Descizione del documento/codice del fascicolo
        /// </summary>
        public string DESCRIPTION_OBJECT
        {
            get
            {
                return _descrizioneObject;
            }

            set
            {
                _descrizioneObject = value;
            }
        }

        /// <summary>
        /// Id del documento in risposta
        /// </summary>
        public string ID_PROFILE_REVIEW
        {
            get
            {
                return _idProfileReview;
            }

            set
            {
                _idProfileReview = value;
            }
        }

        /// <summary>
        /// System_id della Tipologia documentale da utilizzare per la creazione del documento in risposta
        /// </summary>
        public string ID_TIPO_ATTO
        {
            get
            {
                return _idTipoAtto;
            }

            set
            {
                _idTipoAtto = value;
            }
        }

        /// <summary>
        /// Booleano che identifica se la creazione del contributo è obbligatoria
        /// </summary>
        public string CONTRIBUTO_OBBLIGATORIO
        {
            get { return _contributoObbligatorio; }
            set { _contributoObbligatorio = value; }
        }

        /// <summary>
        /// System_id della trasmissione
        /// </summary>
        public string ID_TRASMISSIONE
        {
            get
            {
                return _idTrasmissione;
            }

            set
            {
                _idTrasmissione = value;
            }
        }

        /// <summary>
        /// System_id della trasmissione singola
        /// </summary>
        public string ID_TRASM_SINGOLA
        {
            get
            {
                return _idTrasmSingola;
            }

            set
            {
                _idTrasmSingola = value;
            }
        }

        /// <summary>
        /// System_id della ragione di trasmissione
        /// </summary>
        public string ID_RAGIONE_TRASM
        {
            get
            {
                return _idRagioneTrasm;
            }

            set
            {
                _idRagioneTrasm = value;
            }
        }

        /// <summary>
        /// Descrizione della raione di trasmissione
        /// </summary>
        public string DESC_RAGIONE_TRASM
        {
            get
            {
                return _descRagione;
            }

            set
            {
                _descRagione = value;
            }
        }

        public StatoTask STATO_TASK
        {
            get { return _statoTask; }
            set { _statoTask = value; }
        }

        #endregion
    }

    public class StatoTask
    {
        #region private fields
        private string _idStatoTask;
        private string _dataApertura;
        private string _dataScadenza;
        private string _dataAnnullamento;
        private string _dataLavorazione;
        private string _dataChiusura;
        private string _noteLavorazione;
        private string _noteRiapertura;
        private StatoAvanzamento _stato;
        
        #endregion

        #region public fields

        public string ID_STATO_TASK
        {
            get { return _idStatoTask; }
            set { _idStatoTask = value; }
        }

        //Data apertura del task
        public string DATA_APERTURA 
        {
            get { return _dataApertura; }
            set { _dataApertura = value; }
        }

        /// <summary>
        /// Data scadenza del TASK
        /// </summary>
        public string DATA_SCADENZA
        {
            get { return _dataScadenza; }
            set { _dataScadenza = value;}
        }

        /// <summary>
        /// Data di annullamento del task
        /// </summary>
        public string DATA_ANNULLAMENTO
        {
            get { return _dataAnnullamento; }
            set { _dataAnnullamento = value; }
        }

        /// <summary>
        /// Data di lavorazione del task
        /// </summary>
        public string DATA_LAVORAZIONE
        {
            get { return _dataLavorazione; }
            set { _dataLavorazione = value; }
        }

        /// <summary>
        /// Data di chiusura del task
        /// </summary>
        public string DATA_CHIUSURA
        {
            get { return _dataChiusura; }
            set { _dataChiusura = value; }
        }

        /// <summary>
        /// Note di Lavorazione del Task
        /// </summary>
        public string NOTE_LAVORAZIONE
        {
            get { return _noteLavorazione; }
            set { _noteLavorazione = value; }
        }

        /// <summary>
        /// Note di riapertura del task
        /// </summary>
        public string NOTE_RIAPERTURA
        {
            get { return _noteRiapertura; }
            set { _noteRiapertura = value; }
        }

        public StatoAvanzamento STATO
        {
            get { return _stato;}
            set { _stato = value; }
        }

        #endregion
    }

    public enum StatoAvanzamento
    {
        Aperto,
        Lavorato, 
        Chiuso ,
        Riaperto 
    }
}
