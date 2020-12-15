using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Deposito
{
    public partial class ARCHIVE_JOB_TransferPolicy
    {

        #region Fields

        private Int32 _System_ID; 
        private Int32 _TransferPolicy_ID;
        private Int32 _JobType_ID;
        private DateTime _InsertJobTimestamp;
        private DateTime? _StartJobTimestamp;
        private DateTime? _EndJobTimestamp;
        private Int32 _Executed;

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

        public virtual Int32 TransferPolicy_ID
        {

            get
            {
                return _TransferPolicy_ID;
            }

            set
            {
                _TransferPolicy_ID = value;
            }
        }

        public virtual Int32 JobType_ID
        {

            get
            {
                return _JobType_ID;
            }

            set
            {
                _JobType_ID = value;
            }
        }
                
        public virtual DateTime InsertJobTimestamp
        {

            get
            {
                return _InsertJobTimestamp;
            }

            set
            {
                _InsertJobTimestamp = value;
            }
        }
        
        /// <summary>
        /// This is not a required field.
        /// </summary>
        public virtual DateTime? StartJobTimestamp
        {

            get
            {
                return _StartJobTimestamp;
            }

            set
            {
                _StartJobTimestamp = value;
            }
        }

        /// <summary>
        /// This is not a required field.
        /// </summary>
        public virtual DateTime? EndJobTimestamp
        {

            get
            {
                return _EndJobTimestamp;
            }

            set
            {
                _EndJobTimestamp = value;
            }
        }

        public virtual Int32 Executed
        {

            get
            {
                return _Executed;
            }

            set
            {
                _Executed = value;
            }
        }

        #endregion

        #region Default Constructor

        public ARCHIVE_JOB_TransferPolicy()
        {
        }

        #endregion

        #region Constructors

        public ARCHIVE_JOB_TransferPolicy(Int32 system_ID, Int32 transfer_ID, Int32 jobType_ID, DateTime insertJobTimestamp, DateTime? startJobTimestamp, DateTime? endJobTimestamp, Int32 executed)
        {
            System_ID = system_ID;
            TransferPolicy_ID = transfer_ID;
            JobType_ID=jobType_ID;
            InsertJobTimestamp = insertJobTimestamp;
            StartJobTimestamp=startJobTimestamp;
            EndJobTimestamp=endJobTimestamp;
            Executed=executed;

            }
        #endregion
    }
}
