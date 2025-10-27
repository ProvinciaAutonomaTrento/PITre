// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
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
    [DataContract]
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
        private string _idAOO;
        private string _idRF;
        private string _idMailRegistro;
        private string _idTipologia;
        private string _idStatoDiagramma;
        private bool _isAutomatico;
        private string _errore;
        private string _applicaSegnaturaPermanente = string.Empty;
        private string _posizioneSegnaturaPermanente = string.Empty;
        #endregion

        #region public property


        /// <summary>
        /// System id dell'istanza di passo
        /// </summary>
        [DataMember]
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
        [DataMember]
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
        [DataMember]
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
        [DataMember]
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
        [DataMember]
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
        [DataMember]
        public string TipoFirma
        {
            get { return _tipoFirma; }
            set { _tipoFirma = value; }
        }

        /// <summary>
        /// system id del tipo evento associato al passo
        /// </summary>
        [DataMember]
        public string IdTipoEvento
        {
            get { return _idTipoEvento; }
            set { _idTipoEvento = value; }
        }

        /// <summary>
        /// Codice tipo evento presente anche in DPA_LOG
        /// </summary>
        [DataMember]
        public string CodiceTipoEvento
        {
            get { return _codiceTipoEvento; }
            set { _codiceTipoEvento = value; }
        }

        /// <summary>
        /// Data in cui è stato eseguito il passo
        /// </summary>
        [DataMember]
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
        [DataMember]
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
        [DataMember]
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
        [DataMember]
        public DocsPaVO.utente.Ruolo RuoloCoinvolto
        {
            get { return _ruoloCoinvolto; }
            set { _ruoloCoinvolto = value; }
        }

        /// <summary>
        /// Utente titolare del passo
        /// </summary>
        [DataMember]
        public DocsPaVO.utente.Utente UtenteCoinvolto
        {
            get { return _utenteCoinvolto; }
            set { _utenteCoinvolto = value; }
        }

        /// <summary>
        /// System id della notifica relativa all'avanzamento di passo
        /// </summary>
        [DataMember]
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
        [DataMember]
        public int numeroSequenza
        {
            get { return _numeroSequenza; }
            set { _numeroSequenza = value; }
        }

        /// <summary>
        /// Note relative al passo
        /// </summary>
        [DataMember]
        public string Note
        {
            get { return _note; }
            set { _note = value; }
        }

        [DataMember]
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

        [DataMember]
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

        [DataMember]
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

        /// <summary>
        /// Id del registro di AOO utilizzato per i passi automatici
        /// </summary>
        [DataMember]
        public string IdAOO
        {
            get
            {
                return _idAOO;
            }
            set
            {
                _idAOO = value;
            }
        }

        /// <summary>
        /// Id del registro di RF utilizzato per i passi automatici
        /// </summary>
        [DataMember]
        public string IdRF
        {
            get
            {
                return _idRF;
            }
            set
            {
                _idRF = value;
            }
        }

        /// <summary>
        /// Id Mail di registro
        /// </summary>
        [DataMember]
        public string IdMailRegistro
        {
            get
            {
                return _idMailRegistro;
            }
            set
            {
                _idMailRegistro = value;
            }
        }

        /// <summary>
        /// Id della tipologia
        /// </summary>
        [DataMember]
        public string IdTipologia
        {
            get
            {
                return _idTipologia;
            }
            set
            {
                _idTipologia = value;
            }
        }

        /// <summary>
        /// Id stato del diagramma
        /// </summary>
        [DataMember]
        public string IdStatoDiagramma
        {
            get
            {
                return _idStatoDiagramma;
            }
            set
            {
                _idStatoDiagramma = value;
            }
        }


        /// <summary>
        /// Indica se il passo è un passo automatico
        /// </summary>
        [DataMember]
        public bool IsAutomatico
        {
            get
            {
                return _isAutomatico;
            }
            set
            {
                _isAutomatico = value;
            }

        }

        /// <summary>
        /// Errore esecuzione del passo
        /// </summary>
        [DataMember]
        public string Errore
        {
            get
            {
                return _errore;
            }
            set
            {
                _errore = value;
            }
        }

        /// <summary>
        /// Se ad 1, indica se applicare la segnatura permanente
        /// </summary>
        [DataMember]
        public string ApplicaSegnaturaPermanente
        {
            get
            {
                return _applicaSegnaturaPermanente;
            }

            set
            {
                _applicaSegnaturaPermanente = value;
            }
        }

        /// <summary>
        /// Indica la dove posizionare la segnatura in caso di protocollo in partenza o repertorio
        /// </summary>
        [DataMember]
        public string PosizioneSegnaturaPermanente
        {
            get
            {
                return _posizioneSegnaturaPermanente;
            }

            set
            {
                _posizioneSegnaturaPermanente = value;
            }
        }
        #endregion
    }
}