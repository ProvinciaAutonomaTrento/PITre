using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Deposito
{
    public partial class ARCHIVE_TransferPolicyType
    {
        #region Fields

        private Int32 _System_ID;
        private String _Name;
        #endregion

        #region Properties

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

        public virtual String Name
        {

            get
            {
                return _Name;
            }

            set
            {
                _Name = value;
            }
        }

        #endregion

        #region Default Constructor

        public ARCHIVE_TransferPolicyType()
        {
        }

        #endregion

        #region Constructors

        public ARCHIVE_TransferPolicyType(Int32 system_ID, String name)
        {
            System_ID = system_ID;
            Name = name;
        }

        #endregion 
    }
}
