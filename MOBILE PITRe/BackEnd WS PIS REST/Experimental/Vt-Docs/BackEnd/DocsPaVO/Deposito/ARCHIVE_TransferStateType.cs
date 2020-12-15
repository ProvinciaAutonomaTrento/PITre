using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace DocsPaVO.Deposito
{
    public partial class ARCHIVE_TransferStateType
    {

        #region Fields

        private Int32 _System_ID;
        private String _Name;
        private List<ARCHIVE_TransferState> _ARCHIVE_TransferStateList;

      


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

        public virtual List<ARCHIVE_TransferState> ARCHIVE_TransferStateList
        {

            get
            {
                return _ARCHIVE_TransferStateList;
            }

        }

        #endregion

        #region Default Constructor

        public ARCHIVE_TransferStateType()
        {
        }

        #endregion

        #region Constructors

        public ARCHIVE_TransferStateType(Int32 system_ID, String name)
        {
            System_ID = system_ID;
            Name = name;
        }

        #endregion
    }
}
