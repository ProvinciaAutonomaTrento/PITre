using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Deposito
{
    public partial class ARCHIVE_TransferPolicy_ProfileType
    {

        #region Fields

        private Int32 _TransferPolicy_ID;
        private Int32 _ProfileType_ID;
        #endregion

        #region Properties

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

        public virtual Int32 ProfileType_ID
        {

            get
            {
                return _ProfileType_ID;
            }

            set
            {
                _ProfileType_ID = value;
            }
        }

        #endregion

        #region Default Constructor

        public ARCHIVE_TransferPolicy_ProfileType()
        {
        }

        #endregion

        #region Constructors

        public ARCHIVE_TransferPolicy_ProfileType(Int32 transferPolicy_ID, Int32 profileType_ID)
        {
            TransferPolicy_ID = transferPolicy_ID;
            ProfileType_ID = profileType_ID;
        }

        #endregion

    }
}
