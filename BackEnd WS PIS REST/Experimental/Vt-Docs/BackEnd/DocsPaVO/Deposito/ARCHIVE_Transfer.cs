using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Deposito
{
    public partial class ARCHIVE_Transfer
    {
        #region Fields

        private Int32 _System_ID;
        private Int32 _ID_Amministrazione;
        private String _Description;
        private String _Note;
        #endregion

        #region Properties

        /// <summary>
        /// The database automatically generates this value.
        /// </summary>
        public virtual Int32 ID_Amministrazione
        {

            get
            {
                return _ID_Amministrazione;
            }

            set
            {
                _ID_Amministrazione = value;
            }
        }

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

        public virtual String Description
        {

            get
            {
                return _Description;
            }

            set
            {
                _Description = value;
            }
        }

        /// <summary>
        /// This is not a required field.
        /// </summary>
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

        public ARCHIVE_Transfer()
        {
        }

        #endregion

        #region Constructors

        public ARCHIVE_Transfer(Int32 system_ID,Int32 id_Amministrazione, String description, String note)
        {
            System_ID = system_ID;
            Description = description;
            Note = note;
            ID_Amministrazione = id_Amministrazione;
        }

        #endregion
    }
}
