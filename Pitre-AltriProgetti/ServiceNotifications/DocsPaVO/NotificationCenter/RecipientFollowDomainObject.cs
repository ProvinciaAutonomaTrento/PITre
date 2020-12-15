using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.NotificationCenter
{
    /// <summary>
    /// Data structure for the recipients of the notification in the case of follow object.
    /// </summary>
    public class RecipientFollowDomainObject
    {
        #region private fields

        private int _idRole;
        private int _idUser;
        private string _application;
        #endregion

        #region const
        public const string INTERNAL = "INTERNAL";
        public const string EXTERNAL = "EXTERNAL";
        #endregion

        #region public property

        /// <summary>
        ///
        /// </summary>
        public int IDUSER
        {
            get
            {
                return _idUser;
            }
            set
            {
                _idUser = value;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public int IDROLE
        {
            get
            {
                return _idRole;
            }
            set
            {
                _idRole = value;
            }
        }

        /// <summary>
        /// Application that required the user to follow the object.
        /// It can assume the INTERNAL indicating that the request follow domain object came from Pitre, 
        /// EXTERNAL to indicate that the request came From an external application à Pitre
        /// </summary>
        public string APPLICATION
        {
            get
            {
                return _application;
            }
            set
            {
                _application = value;
            }
        }

        #endregion

    }
}