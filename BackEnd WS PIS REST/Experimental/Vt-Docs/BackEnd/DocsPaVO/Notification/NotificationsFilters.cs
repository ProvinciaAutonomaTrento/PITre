using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Notification
{
    public class NotificationsFilters
    {
        #region private fields

        private string _typeDocument;
        private bool _documentAcquired;
        private bool _documentSigned;
        private bool _documentUnsigned;
        private bool _pending;
        private string _dateEventFrom;
        private string _dateEventTo;
        private string _dateExpireFrom;
        private string _dateExpireTo;
        private string _authorDescription;
        private string _authorSystemId;
        private string _authorType;
        private string _object;
        private string _typeFileAquired;
        private string _notes;

        #endregion

        #region public property

        /// <summary>
        /// Type Document
        /// </summary>
        public string TYPE_DOCUMENT
        {
            get
            {
                return _typeDocument;
            }
            set
            {
                _typeDocument = value;
            }
        }

        /// <summary>
        /// Document acquired
        /// </summary>
        public bool DOCUMENT_ACQUIRED
        {
            get
            {
                return _documentAcquired;
            }
            set
            {
                _documentAcquired = value;
            }
        }

        /// <summary>
        /// Document signed
        /// </summary>
        public bool DOCUMENT_SIGNED
        {
            get
            {
                return _documentSigned;
            }
            set
            {
                _documentSigned = value;
            }
        }

        /// <summary>
        /// Document unsigned
        /// </summary>
        public bool DOCUMENT_UNSIGNED
        {
            get
            {
                return _documentUnsigned;
            }
            set
            {
               _documentUnsigned = value;
            }
        }

        /// <summary>
        /// Trasmission pending
        /// </summary>
        public bool PENDING
        {
            get
            {
                return _pending;
            }
            set
            {
                _pending = value;
            }
        }

        /// <summary>
        /// Date event from
        /// </summary>
        public string DATE_EVENT_FROM 
        {
            get
            {
                return _dateEventFrom;
            }
            set
            {
                _dateEventFrom = value;
            }
        }

        /// <summary>
        /// Date event to
        /// </summary>
        public string DATE_EVENT_TO 
        {
            get
            {
                return _dateEventTo;
            }
            set
            {
                _dateEventTo = value;
            }
        }

        /// <summary>
        /// Date expire from
        /// </summary>
        public string DATE_EXPIRE_FROM
        {
            get
            {
                return _dateExpireFrom;
            }
            set
            {
                _dateExpireFrom = value;
            }
        }

        /// <summary>
        /// Date expire to
        /// </summary>
        public string DATE_EXPIRE_TO 
        {
            get
            {
                return _dateExpireTo;
            }
            set
            {
                _dateExpireTo = value;
            }
        }

        /// <summary>
        /// Description author
        /// </summary>
        public string AUTHOR_DESCRIPTION
        {
            get
            {
                return _authorDescription;
            }
            set
            {
                _authorDescription = value;
            }
        }

        /// <summary>
        /// System ID author
        /// </summary>
        public string AUTHOR_SYSTEM_ID
        {
            get
            {
                return _authorSystemId;
            }
            set
            {
                _authorSystemId = value;
            }
        }

        /// <summary>
        /// Type author
        /// </summary>
        public string AUTHOR_TYPE 
        {
            get
            {
                return _authorType;
            }
            set
            {
                _authorType = value;
            }
        }

        /// <summary>
        /// Description object
        /// </summary>
        public string OBJECT 
        {
            get
            {
                return _object;
            }
            set
            {
                _object = value;
            }
        }

        /// <summary>
        /// Type file acquired
        /// </summary>
        public string TYPE_FILE_ACQUIRED 
        {
            get
            {
                return _typeFileAquired;
            }
            set
            {
                _typeFileAquired = value;
            }
        }

        /// <summary>
        /// Notes
        /// </summary>
        public string NOTES
        {
            get
            {
                return _notes;
            }
            set
            {
                _notes = value;
            }
        }

        #endregion
    }
}
