using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Notification
{
    public class Notification
    {
        #region private fields

        private string _idNotify;
        private string _idEvent;
        private string _producer;
        private DateTime _dtaEvent;
        private DateTime _dtaNotify;
        private string _idPeople;
        private string _idGroup;
        private char _typeNotification;
        private string _multiplicity;
        private string _typeEvent;
        private string _domainObject;
        private string _idObject;
        private string _idSpecializedObject;
        private char _readNotification;
        private Items _items;
        private string _itemSpecialized;
        private string _color;
        private string _textForSorting;
        private string _extension;
        private string _notes;
        private char _signed;

        #endregion

        #region public property

        /// <summary>
        /// Notification ID
        /// </summary>
        public string ID_NOTIFY
        {
            get
            {
                return _idNotify;
            }

            set
            {
                _idNotify = value;
            }
        }

        /// <summary>
        /// Event ID
        /// </summary>
        public string ID_EVENT
        {
            get
            {
                return _idEvent;
            }

            set
            {
                _idEvent = value;
            }
        }

        /// <summary>
        /// Producer of the event that generated the notification
        /// </summary>
        public string PRODUCER
        {
            get
            {
                return _producer;
            }

            set
            {
                _producer = value;
            }
        }

        /// <summary>
        /// Date of the event generation
        /// </summary>
        public DateTime DTA_EVENT
        {
            get
            {
                return _dtaEvent;
            }

            set
            {
                _dtaEvent = value;
            }
        }

        /// <summary>
        /// Date of generation of the notification
        /// </summary>
        public DateTime DTA_NOTIFY
        {
            get
            {
                return _dtaNotify;
            }
            set
            {
                _dtaNotify = value;
            }
        }

        /// <summary>
        /// User Identifier notification recipient
        /// </summary>
        public string ID_PEOPLE
        {
            get
            {
                return _idPeople;
            }

            set
            {
                _idPeople = value;
            }
        }

        /// <summary>
        /// Identifier of the notification recipient role
        /// </summary>
        public string ID_GROUP
        {
            get
            {
                return _idGroup;
            }

            set
            {
                _idGroup = value;
            }
        }

        /// <summary>
        /// Type notification
        /// </summary>
        public char TYPE_NOTIFICATION
        {
            get
            {
                return _typeNotification;
            }

            set
            {
                _typeNotification = value;
            }
        }

        /// <summary>
        /// Type event
        /// </summary>
        public string TYPE_EVENT
        {
            get
            {
                return _typeEvent;
            }

            set
            {
                _typeEvent = value;
            }
        }

        /// <summary>
        /// Multiplicity (one, all)
        /// </summary>
        public string MULTIPLICITY
        {
            get
            {
                return _multiplicity;
            }

            set
            {
                _multiplicity = value;
            }
        }

        /// <summary>
        /// Domain object (document, folder, job)
        /// </summary>
        public string DOMAINOBJECT
        {
            get
            {
                return _domainObject;
            }

            set
            {
                _domainObject = value;
            }
        }

        /// <summary>
        /// Object ID
        /// </summary>
        public string ID_OBJECT
        {
            get
            {
                return _idObject;
            }

            set
            {
                _idObject = value;
            }
        }

        /// <summary>
        /// 
        /// Id specialized object
        /// </summary>
        public string ID_SPECIALIZED_OBJECT
        {
            get
            {
                return _idSpecializedObject;
            }

            set
            {
                _idSpecializedObject = value;
            }
        }

        /// <summary>
        /// This flag is 1 if the notification has already been read by the recipient
        /// </summary>
        public char READ_NOTIFICATION
        {
            get
            {
                return _readNotification;
            }

            set
            {
                _readNotification = value;
            }
        }

        /// <summary>
        /// object contains information about the notification to be displayed side front end
        /// </summary>
        public Items ITEMS
        {
            get
            {
                return _items;
            }

            set
            {
                _items = value;
            }
        }

        /// <summary>
        /// Contains information about the notification to be displayed side front end when you expand the notification
        /// </summary>
        public string ITEM_SPECIALIZED
        {
            get
            {
                return _itemSpecialized;
            }

            set
            {
                _itemSpecialized = value;
            }
        }

        /// <summary>
        /// Notification field populated and used frontend for sorting the list notifications.
        /// </summary>
        public string TEXT_SORTING
        {
            get
            {
                return _textForSorting;
            }

            set
            {
                _textForSorting = value;
            }
        }

        /// <summary>
        /// Color notification
        /// </summary>
        public string COLOR
        {
            get
            {
                return _color;
            }

            set
            {
                _color = value;
            }
        }

        /// <summary>
        /// Estensione del file acquisito (No colonna DB)
        /// </summary>
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
        /// Nota della notifica
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

        /// <summary>
        /// This flag is 1 if the document is signed (No colonna DB)
        /// </summary>
        public char SIGNED
        {
            get
            {
                return _signed;
            }

            set
            {
                _signed = value;
            }
        }
        #endregion

    }
}
