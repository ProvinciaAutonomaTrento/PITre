using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.InstanceAccess
{
    public class InstanceAccessAttachments
    {

        #region private fields

        private string _systemId;
        private string _idInstanceAccessDocument;
        private string _idAttach;
        private string _fileName;
        private string _extension;
        private bool _enable;

        #endregion

        #region public property

        public string SYSTEM_ID
        {
            get
            {
                return _systemId;
            }

            set
            {
                _systemId = value;
            }
        }

        /// <summary>
        /// Id del documento, all'interno dell'istanza, a cui l'allegato appartiene
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
        /// Id dell'allegato
        /// </summary>
        public string ID_ATTACH
        {
            get
            {
                return _idAttach;
            }

            set
            {
                _idAttach = value;
            }
        }

        public string FILE_NAME
        {
            get
            {
                return _fileName;
            }

            set
            {
                _fileName = value;
            }
        }

        public string EXTENSION
        {
            get
            {
                return _extension;
            }

            set
            {
                _extension = value;
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
