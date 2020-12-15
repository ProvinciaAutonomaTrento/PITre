using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.InstanceAccess
{
    public class InstanceAccessDocument
    {
        #region private fields

        private string _idInstanceAccessDocument;
        private string _idInstanceAccess;
        private string _docnumber;
        private string _typeRequest;
        private InfoDocument _infoDocument;
        private InfoProject _infoProject;
        private List<InstanceAccessAttachments> _attachments;
        private bool _enable;

        #endregion

        #region public property

        /// <summary>
        /// Id del documento all'interno dell'istanza
        /// </summary>
        public string ID_INSTANCE_ACCESS_DOCUMENT
        {
            get
            {
                return _idInstanceAccessDocument;
            }

            set
            {
                _idInstanceAccessDocument = value;
            }
        }

        /// <summary>
        /// Id dell'istanza di accesso a cui il documento appartiene
        /// </summary>
        public string ID_INSTANCE_ACCESS
        {
            get
            {
                return _idInstanceAccess;
            }

            set
            {
                _idInstanceAccess = value;
            }
        }

        /// <summary>
        /// Docnumber del documento
        /// </summary>
        public string DOCNUMBER
        {
            get
            {
                return _docnumber;
            }

            set
            {
                _docnumber = value;
            }
        }

        /// <summary>
        /// Tipo di richiesta
        /// </summary>
        public string TYPE_REQUEST
        {
            get
            {
                return _typeRequest;
            }

            set
            {
                _typeRequest = value;
            }
        }

        public InfoDocument INFO_DOCUMENT
        {
            get
            {
                return _infoDocument;
            }

            set
            {
                _infoDocument = value;
            }
        }

        /// <summary>
        /// Lista degli allegati, appartenenti al documento, selezionati per l'istanza di accesso
        /// </summary>
        public List<InstanceAccessAttachments> ATTACHMENTS
        {
            get
            {
                return _attachments;
            }

            set
            {
                _attachments = value;
            }
        }

        public InfoProject INFO_PROJECT
        {
            get
            {
                return _infoProject;
            }

            set
            {
                _infoProject = value;
            }
        }

        /// <summary>
        /// Documento selezionato oppure no
        /// </summary>
        public bool ENABLE
        {
            get
            {
                return _enable;
            }

            set
            {
                _enable = value;
            }
        }

        #endregion
    }
}
