using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Deposito
{
  public partial  class ARCHIVE_DisposalState
    {
        
        #region Fields

        private Int32 _System_ID;
        private Int32 _Disposal_ID;
        private Int32 _DisposalStateType_ID;
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

        public virtual Int32 Disposal_ID
        {

            get
            {
                return _Disposal_ID;
            }

            set
            {
                _Disposal_ID = value;
            }
        }

        public virtual Int32 DisposalStateType_ID
        {

            get
            {
                return _DisposalStateType_ID;
            }

            set
            {
                _DisposalStateType_ID = value;
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

        public  ARCHIVE_DisposalState()
        {
        }

        #endregion

        #region Constructors

        public ARCHIVE_DisposalState(Int32 system_ID, Int32 transfer_ID, Int32 transferStateType_ID, DateTime dateTime)
        {
            System_ID = system_ID;
            Disposal_ID = transfer_ID;
            DisposalStateType_ID = transferStateType_ID;
            DateTime = dateTime;
        }
        #endregion
    }
}
