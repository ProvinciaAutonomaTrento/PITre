using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Deposito
{
    public partial class ARCHIVE_TransferState
    {

        #region Fields

        private Int32 _System_ID;
        private Int32 _Transfer_ID;
        private Int32 _TransferStateType_ID;
        private DateTime _DateTime;
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

        public virtual Int32 Transfer_ID
        {

            get
            {
                return _Transfer_ID;
            }

            set
            {
                _Transfer_ID = value;
            }
        }

        public virtual Int32 TransferStateType_ID
        {

            get
            {
                return _TransferStateType_ID;
            }

            set
            {
                _TransferStateType_ID = value;
            }
        }

        public virtual DateTime DateTime
        {

            get
            {
                return _DateTime;
            }

            set
            {
                _DateTime = value;
            }
        }

        #endregion

        #region Default Constructor

        public  ARCHIVE_TransferState()
        {
        }

        #endregion

        #region Constructors

        public  ARCHIVE_TransferState(Int32 system_ID, Int32 transfer_ID, Int32 transferStateType_ID, DateTime dateTime)
        {
            System_ID = system_ID;
            Transfer_ID = transfer_ID;
            TransferStateType_ID = transferStateType_ID;
            DateTime = dateTime;
        }
        #endregion

    }

}