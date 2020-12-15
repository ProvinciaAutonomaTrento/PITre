using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Deposito
{
    public partial class ARCHIVE_LOG_TransferAndPolicy
    {
        #region Fields

        private Int32 _System_ID;
        private DateTime _Timestamp;
        private String _Action;
        private String _ActionType;
        private Int32 _ObjectType;
        private Int32 _ObjectID;

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

        public virtual DateTime Timestamp
        {

            get
            {
                return _Timestamp;
            }

            set
            {
                _Timestamp = value;
            }
        }

        public virtual String Action
        {

            get
            {
                return _Action;
            }

            set
            {
                _Action = value;
            }
        }

        public virtual String ActionType
        {

            get
            {
                return _ActionType;
            }

            set
            {
                _ActionType = value;
            }
        }

        public virtual Int32 ObjectType
        {

            get
            {
                return _ObjectType;
            }

            set
            {
                _ObjectType = value;
            }
        }

        public virtual Int32 ObjectID
        {

            get
            {
                return _ObjectID;
            }

            set
            {
                _ObjectID = value;
            }
        }

        #endregion

        #region Constructors

        public ARCHIVE_LOG_TransferAndPolicy()
        {
        }

        public ARCHIVE_LOG_TransferAndPolicy(Int32 system_ID, DateTime timestamp, String action,
                                             String actionType, Int32 objectType, Int32 objectID)
        {
            System_ID = system_ID;
            Timestamp = timestamp;
            Action = action;
            ActionType = actionType;
            ObjectType = objectType;
            ObjectID = objectID;

        }
        #endregion
    }
}
