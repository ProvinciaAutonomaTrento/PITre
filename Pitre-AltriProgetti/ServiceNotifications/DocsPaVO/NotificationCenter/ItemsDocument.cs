using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.NotificationCenter
{
    public class ItemsDocument : Items
    {
        #region private field

        private string _segnatura;
        private string _documentType;
        private string _listNumber;

        #endregion

        #region public property

        /// <summary>
        /// protocol number
        /// </summary>
        public string SEGNATURA
        {
            get 
            {
                return _segnatura;
            }

            set
            {
                _segnatura = value;
            }
        }

        /// <summary>
        /// List number
        /// </summary>
        public string LIST_NUMBER
        {
            get
            {
                return _listNumber;
            }

            set
            {
                _listNumber = value;
            }
        }
        #endregion
    }
}
