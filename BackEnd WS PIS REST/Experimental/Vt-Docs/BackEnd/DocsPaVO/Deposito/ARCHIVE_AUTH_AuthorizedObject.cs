using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Deposito
{
    public partial class ARCHIVE_AUTH_AuthorizedObject
    {

        #region Fields

        private Int32 _System_ID;
        private Int32 _Authorization_ID;
        private Int32? _Project_ID;
        private Int32? _Profile_ID;

        #endregion

        #region Properties

        /// <summary>
        /// The database automatically generates this value.
        /// </summary>
        public virtual Int32 System_ID
        {

            get
            {
                return _System_ID;
            }

            set
            {
                _System_ID = value;
            }
        }

        public virtual Int32 Authorization_ID
        {

            get
            {
                return _Authorization_ID;
            }

            set
            {
                _Authorization_ID = value;
            }
        }

        public virtual Int32? Project_ID
        {

            get
            {
                return _Project_ID;
            }

            set
            {
                _Project_ID = value;
            }
        }

        public virtual Int32? Profile_ID
        {

            get
            {
                return _Profile_ID;
            }

            set
            {
                _Profile_ID = value;
            }
        }

        #endregion

        #region Default Constructor

        public ARCHIVE_AUTH_AuthorizedObject()
        {
        }

        #endregion

        #region Constructors

        public ARCHIVE_AUTH_AuthorizedObject(Int32 system_ID, Int32 authorization_ID, Int32? project_ID,
                        Int32? profile_ID)
        {
            System_ID = system_ID;
            Authorization_ID = authorization_ID;
            Project_ID = (Int32)project_ID;
            Profile_ID = (Int32)profile_ID;
        #endregion
        }
    }
}
