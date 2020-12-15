using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.LibroFirma
{
    public class ProcessoFirma
    {
        #region private fields

        private string _idProcesso;
        private string _nome;
        private string _idRuoloAutore;
        private string _idPeopleAutore;
        private List<PassoFirma> _passi;
        private bool _isInvalidated;
        private bool _isProcessModel;

        #endregion

        #region public property

        /// <summary>
        /// System id del processo di firma
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
        /// Nome del processo di firma
        /// </summary>
        public string nome
        {
            get
            {
                return _nome;
            }

            set
            {
                _nome = value;
            }
        }

        /// <summary>
        /// Id del ruolo autore del processo di firma
        /// </summary>
        public string idRuoloAutore
        {
            get
            {
                return _idRuoloAutore;
            }

            set
            {
                _idRuoloAutore = value;
            }
        }

        /// <summary>
        /// Id dell'uUtente autore del processo di firma
        /// </summary>
        public string idPeopleAutore
        {
            get
            {
                return _idPeopleAutore;
            }

            set
            {
                _idPeopleAutore = value;
            }
        }

        /// <summary>
        /// Passi che compongono il processo di firma
        /// </summary>
        public List<PassoFirma> passi
        {
            get
            {
                return _passi;
            }
            set
            {
                _passi = value;
            }
        }

        /// <summary>
        /// Se true il processo è invalidato
        /// </summary>
        public bool isInvalidated
        {
            get
            {
                return _isInvalidated;
            }
            set
            {
                _isInvalidated = value;
            }
        }

        /// <summary>
        /// Se true è un modello di processo
        /// </summary>
        public bool IsProcessModel
        {
            get
            {
                return _isProcessModel;
            }
            set
            {
                _isProcessModel = value;
            }
        }

        #endregion
    }

    public enum ResultProcessoFirma
    {
        /// <summary>
        /// successo
        /// </summary>
        OK,
        DOCUMENTO_GIA_IN_LIBRO_FIRMA,
        DOCUMENTO_CONSOLIDATO,
        DOCUMENTO_BLOCCATO,
        EXISTING_PROCESS_NAME,
        FILE_NON_AMMESSO_ALLA_FIRMA,
        FILE_NON_ACQUISITO,
        KO,
        DOCUMENTO_PROTOCOLLATO,
        DOCUMENTO_REPERTORIATO,
        PASSO_PADES_SU_FILE_CADES
    }
}
