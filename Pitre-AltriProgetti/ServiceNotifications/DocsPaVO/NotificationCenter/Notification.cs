using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.NotificationCenter
{
    /// <summary>
    /// 
    /// </summary>
    public class Notification
    {
        #region private fields

        private Recipient _recipient;
        private char _typeNotify;
        private DateTime _dtaNotify;
        private Items _items;

        #endregion

        #region public property

        /// <summary>
        /// Recipient
        /// </summary>
        public Recipient RECIPIENT
        {
            get
            {
                return _recipient;
            }

            set
            {
                _recipient = value;
            }
        }

        /// <summary>
        /// Type notify
        /// </summary>
        public char TYPE_NOTIFY
        {
            get
            {
                return _typeNotify;
            }

            set
            {
                _typeNotify = value;
            }
        }

        /// <summary>
        /// Date notify
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
        /// items notify
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

        #endregion
    }
}
