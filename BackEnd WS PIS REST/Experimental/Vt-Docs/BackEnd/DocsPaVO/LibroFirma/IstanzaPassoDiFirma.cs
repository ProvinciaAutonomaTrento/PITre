using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.LibroFirma
{
    public enum TipoStatoPasso
    {
        NEW,
        LOOK,
        CLOSE,
        STUCK,
        CUT
    }

    [Serializable()]
    [XmlInclude(typeof(DocsPaVO.LibroFirma.TipoStatoPasso))]
    public class IstanzaPassoDiFirma
    {

        #region private fields

        private string _idIstanzaPasso;
        private string _idIstanzaProcesso;
        private string _idPasso;
        private TipoStatoPasso _statoPasso;
        private string _descrizioneStatoPasso;
        private string _tipoFirma;
        private string _idTipoEvento;
        private Evento _evento;
        private string _codiceTipoEvento;
        private string _dataEsecuzione;
        private string _dataScadenza;
        private string _motivoRespingimento;
        private DocsPaVO.utente.Ruolo _ruoloCoinvolto;
        private DocsPaVO.utente.Utente _utenteCoinvolto;
        private string _idNotificaEffettuata;
        private int _numeroSequenza;
        private string _note;
        private string _utenteLocker;
        private string _descrizioneUtenteLocker;

        #endregion

        #region public property


        /// <summary>
        /// System id dell'istanza di passo
        /// </summary>
        public string idIstanzaPasso
        {
            get
            {
                return _idIstanzaPasso;
            }

            set
            {
                _idIstanzaPasso = value;
            }
        }
        /// <summary>
        /// System id dell'istanza di processo di firma
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
        /// System id del passo
        /// </summary>
        public string idPasso
        {
            get
            {
                return _idPasso;
            }

            set
            {
                _idPasso = value;
            }
        }

        /// <summary>
        /// System id dello stato passo
        /// </summary>
        public TipoStatoPasso statoPasso
        {
            get
            {
                return _statoPasso;
            }

            set
            {
                _statoPasso = value;
            }
        }

        /// <summary>
        /// Descrizione dello stato passo
        /// </summary>
        public string descrizioneStatoPasso
        {
            get
            {
                return _descrizioneStatoPasso;
            }

            set
            {
                _descrizioneStatoPasso = value;
            }
        }

        /// <summary>
        /// Tipo firma (Elettronica/Digitale/Pades)
        /// </summary>
        public string TipoFirma
        {
            get { return _tipoFirma; }
            set { _tipoFirma = value; }
        }

        /// <summary>
        /// system id del tipo evento associato al passo
        /// </summary>
        public string IdTipoEvento
        {
            get { return _idTipoEvento; }
            set { _idTipoEvento = value; }
        }

        /// <summary>
        /// Codice tipo evento presente anche in DPA_LOG
        /// </summary>
        public string CodiceTipoEvento
        {
            get { return _codiceTipoEvento; }
            set { _codiceTipoEvento = value; }
        }

        /// <summary>
        /// Data in cui è stato eseguito il passo
        /// </summary>
        public string dataEsecuzione
        {
            get
            {
                return _dataEsecuzione;
            }

            set
            {
                _dataEsecuzione = value;
            }
        }

        /// <summary>
        /// Data entro cui poter eseguito il passo
        /// </summary>
        public string dataScadenza
        {
            get
            {
                return _dataScadenza;
            }

            set
            {
                _dataScadenza = value;
            }
        }
        /// <summary>
        /// Descriznoe motivo del respingimento
        /// </summary>
        public string motivoRespingimento
        {
            get
            {
                return _motivoRespingimento;
            }

            set
            {
                _motivoRespingimento = value;
            }
        }

        /// <summary>
        /// Ruolo titolare del passo
        /// </summary>
        public DocsPaVO.utente.Ruolo RuoloCoinvolto
        {
            get { return _ruoloCoinvolto; }
            set { _ruoloCoinvolto = value; }
        }

        /// <summary>
        /// Utente titolare del passo
        /// </summary>
        public DocsPaVO.utente.Utente UtenteCoinvolto
        {
            get { return _utenteCoinvolto; }
            set { _utenteCoinvolto = value; }
        }

        /// <summary>
        /// System id della notifica relativa all'avanzamento di passo
        /// </summary>
        public string idNotificaEffettuata
        {
            get
            {
                return _idNotificaEffettuata;
            }

            set
            {
                _idNotificaEffettuata = value;
            }
        }

        /// <summary>
        /// Numero relativo all'ordine del passo
        /// </summary>
        public int numeroSequenza
        {
            get { return _numeroSequenza; }
            set { _numeroSequenza = value; }
        }

        /// <summary>
        /// Note relative al passo
        /// </summary>
        public string Note
        {
            get { return _note; }
            set { _note = value; }
        }

        public Evento Evento
        {
            get
            {
                return _evento;
            }
            set
            {
                _evento = value;
            }
        }

        public string UtenteLocker
        {
            get
            {
                return _utenteLocker;
            }
            set
            {
                _utenteLocker = value;
            }
        }

        public string DescrizioneUtenteLocker
        {
            get
            {
                return _descrizioneUtenteLocker;
            }
            set
            {
                _descrizioneUtenteLocker = value;
            }
        }
        #endregion
    }
}