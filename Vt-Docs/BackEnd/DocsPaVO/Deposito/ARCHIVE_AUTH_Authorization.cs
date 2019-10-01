using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Deposito
{
    public partial class ARCHIVE_AUTH_Authorization
    {
        #region Fields

        private Int32? _System_ID;
        private Int32? _People_ID;
        private DateTime? _StartDate;
        private DateTime? _EndDate;
        private String _Note;

        #endregion

        #region Properties

        /// <summary>
        /// The database automatically generates this value.
        /// </summary>
        public virtual Int32? System_ID
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

        public virtual Int32? People_ID
        {

            get
            {
                return _People_ID;
            }

            set
            {
                _People_ID = value;
            }
        }

        public virtual DateTime? StartDate
        {

            get
            {
                return _StartDate;
            }

            set
            {
                _StartDate = value;
            }
        }
        /// <summary>
        /// This is not a required field.
        /// </summary>
        public virtual DateTime? EndDate
        {

            get
            {
                return _EndDate;
            }

            set
            {
                _EndDate = value;
            }
        }

        public virtual String Note
        {

            get
            {
                return _Note;
            }

            set
            {
                _Note = value;
            }
        }

        #endregion

        #region Default Constructor

        public ARCHIVE_AUTH_Authorization()
        {
        }

        #endregion

        #region Constructors

        public ARCHIVE_AUTH_Authorization(Int32 system_ID, Int32 people_ID, DateTime? startDate,
                        DateTime? endDate, String note)
        {
            System_ID = system_ID;
            People_ID = people_ID;
            StartDate = startDate;
            EndDate = endDate;
            Note = note;
        }
        #endregion

    }
}
