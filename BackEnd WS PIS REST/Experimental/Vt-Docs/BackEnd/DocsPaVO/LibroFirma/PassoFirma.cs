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
        
        #endregion
    }
}
