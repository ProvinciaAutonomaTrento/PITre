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
        CLOSED
    }

    public class IstanzaProcessoDiFirma
    {
        #region private fields

        private string _idIstanzaProcesso; //ID_ISTANZA
        private string _idProcesso; //ID_PROCESSO
        private TipoStatoProcesso _statoProcesso; //STATO
        private string _docAll; //DOC_ALL
        private string _docNumber; //ID_DOCUMENTO
        private string _versionId; //VERSION_ID
        private string _dataAttivazione; //ATTIVATO_IL
        private string _dataChiusura; //CONCLUSO_IL
        private string _numeroAllegato; //NUM_ALL
        private DocsPaVO.utente.Ruolo _ruoloProponente; //ID_RUOLO_PROPONENTE
        private DocsPaVO.utente.Utente _utenteProponente; //ID_UTENTE_PROPONENTE
        private int _numeroVersione; //NUM_VERSIONE
        private string _notifica_interrotto; //NOTIFICA_INTERROTTO
        private string _notifica_concluso; //NOTIFICA_CONCLUSO
        private string _descrizione; //DESCRIZIONE
        private List<IstanzaPassoDiFirma> _istanzePassoDiFirma;

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
        /// Richiesta notifica per interruzione
        /// </summary>
        public bool Notifica_interrotto
        {
            get 
            {
                bool retVal = false;
                if (!string.IsNullOrEmpty(_notifica_interrotto))
                {
                    bool.TryParse(_notifica_interrotto, out retVal);
                }

                return retVal; 
            }
            set 
            {
                _notifica_interrotto = value.ToString(); 
            }
        }

        /// <summary>
        /// Richiesta notifica per conclusione
        /// </summary>
        public bool Notifica_concluso
        {
            get 
            {
                bool retVal = false;
                if (!string.IsNullOrEmpty(_notifica_concluso))
                {
                    bool.TryParse(_notifica_concluso, out retVal);
                }

                return retVal; 
            }
            set 
            {
                _notifica_concluso = value.ToString(); 
            }
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

        #endregion
    }
}
