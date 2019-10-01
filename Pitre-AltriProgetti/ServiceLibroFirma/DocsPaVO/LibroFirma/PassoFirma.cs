using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.LibroFirma
{
    public class PassoFirma
    {

        #region structs

        public enum TipoFirma
        {
            DIGITALE,
            ELETRONICA
        }

        public enum TipoFirmaDigitale
        {
            PADES,
            CADES,
            ELETTRONICA
        }
        #endregion

        #region private fields

        private string _idPasso;
        private string _idProcesso;
        private string _idDocumeto;
        private string _versionId;
        private int _numeroSequenza;
        private string _tipoFirma;
        private string _tipoEvento;
        private string _ruoloCoinvolto;
        private string _utenteCoinvolto;
        private string _note;
        private List<string> _idEventiDaNotificare;
        private string _dataScadenza;
        private string _docAll;
        private bool _isAutomatico;

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


        public string idDocumeto
        {
            get { return _idDocumeto; }
            set { _idDocumeto = value; }
        }


        public string VersionId
        {
            get { return _versionId; }
            set { _versionId = value; }
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
        /// Tipo di firma del passo
        /// </summary>
        public String tipoFirma
        {
            get
            {
                return _tipoFirma;
            }

            set
            {
                _tipoFirma = value;
            }
        }

        /// <summary>
        /// Tipo di firma digitale del passo
        /// </summary>
        public string tipoEvento
        {
            get 
            {
                return _tipoEvento;
            }
            set
            {
                _tipoEvento = value;
            }
        }

        /// <summary>
        /// Id del ruolo coinvolto nel passo
        /// </summary>
        public string idRuoloCoinvolto
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
        public string idUtenteCoinvolto
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

        /// <summary>
        /// Data scadenza del passo
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
        
        public bool isAutomatico
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
        #endregion
    }
}
