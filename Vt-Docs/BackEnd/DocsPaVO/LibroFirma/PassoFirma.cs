using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.LibroFirma
{
    public class PassoFirma
    {

        #region private fields

        private string _idPasso;
        private string _idProcesso;
        private int _numeroSequenza;
        private Evento _evento;
        private DocsPaVO.utente.TipoRuolo _tipoRuoloCoinvolto;
        private DocsPaVO.utente.Ruolo _ruoloCoinvolto;
        private DocsPaVO.utente.Utente _utenteCoinvolto;
        private string _note;
        private List<string> _idEventiDaNotificare;
        private string _dataScadenza;
        private char _invalidated;
        private bool _isModello;
        private bool _daAggiornare;
        private bool _isAutomatico;
        private string _idAOO;
        private string _idRF;
        private string _idMailRegistro;
        private string _idTipologia;
        private string _idStatoDiagramma;
        private bool _facoltativo;
        private IncludiInIstanzaPasso _includiPassoInIstanza = IncludiInIstanzaPasso.NON_SPECIFICATO;

        #endregion

        #region public property

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
        /// System id del processo di firma di appartenza
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
        /// Numero di sequenza del passo
        /// </summary>
        public int numeroSequenza
        {
            get
            {
                return _numeroSequenza;
            }

            set
            {
                _numeroSequenza = value;
            }
        }

        /// <summary>
        /// Id del tipo ruolo coinvolto
        /// </summary>
        public DocsPaVO.utente.TipoRuolo TpoRuoloCoinvolto
        {
            get
            {
                return _tipoRuoloCoinvolto;
            }

            set
            {
                _tipoRuoloCoinvolto = value;
            }
        }

        /// <summary>
        /// Id del ruolo coinvolto nel passo
        /// </summary>
        public DocsPaVO.utente.Ruolo ruoloCoinvolto
        {
            get
            {
                return _ruoloCoinvolto;
            }

            set
            {
                _ruoloCoinvolto = value;
            }
        }

        /// <summary>
        /// Id dell'utente coinvolto nel passo
        /// </summary>
        public DocsPaVO.utente.Utente utenteCoinvolto
        {
            get
            {
                return _utenteCoinvolto;
            }
            set
            {
                _utenteCoinvolto = value;
            }
        }

        /// <summary>
        /// Note del passo
        /// </summary>
        public string note
        {
            get
            {
                return _note;
            }
            set
            {
                _note = value;
            }
        }

        /// <summary>
        /// Opzioni di notifica del passo
        /// </summary>
        public List<string> idEventiDaNotificare
        {
            get
            {
                return _idEventiDaNotificare;
            }
            set
            {
                _idEventiDaNotificare = value;
            }
        }

        /// <summary>
        /// Data scadenza del passo
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

        /// <summary>
        /// Indica se il passo è invalidato:
        /// R=ruolo modificato/disabilitato
        /// U=utente modificato/disabilitato
        /// </summary>
        public char Invalidated
        {
            get
            {
                return _invalidated;
            }
            set
            {
                _invalidated = value;
            }
        }

        /// <summary>
        /// Indica se è un modello di passo
        /// </summary>
        public bool IsModello
        {
            get
            {
                return _isModello;
            }
            set
            {
                _isModello = value;
            }
        }

        public bool DaAggiornare
        {
            get
            {
                return _daAggiornare;
            }
            set
            {
                _daAggiornare = value;
            }
        }

        /// <summary>
        /// Id del registro di AOO utilizzato per i passi automatici
        /// </summary>
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
        /// Indica se il passo è facoltativo
        /// </summary>
        public bool IsFacoltativo
        {
            get
            {
                return _facoltativo;
            }
            set
            {
                _facoltativo = value;
            }
        }

        /// <summary>
        /// Indica se inserire il passo nell'istanza passo di firma (scelta dell'utente, determina l'inserimento della riga)
        /// </summary>
        public IncludiInIstanzaPasso IncludiInIstanzaPasso
        {
            get
            {
                return _includiPassoInIstanza;
            }
            set
            {
                _includiPassoInIstanza = value;
            }
        }

        #endregion
    }

    public enum IncludiInIstanzaPasso
    {
        SI, 
        NO,
        NON_SPECIFICATO
    }
}
