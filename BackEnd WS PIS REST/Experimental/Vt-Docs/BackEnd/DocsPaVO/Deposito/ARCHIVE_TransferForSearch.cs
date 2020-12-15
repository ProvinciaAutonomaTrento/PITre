using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Deposito
{
    public partial class ARCHIVE_TransferForSearch
    {
        #region fields

        private Int32 _System_id;
        private Int32 _ID_Amministrazione;
        private String _Description;
        private String _Stato;
        private DateTime _DateTime;
        private Int32 _numero_doc_effettivi;
        private Int32 _numero_doc_copie;
        #endregion

        #region Properties

        public virtual Int32 System_id
        {

            get
            {
                return _System_id;
            }

            set
            {
                _System_id = value;
            }
        }

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

        public virtual String Stato
        {

            get
            {
                return _Stato;
            }

            set
            {
                _Stato = value;
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

        public virtual Int32 Numero_doc_effettivi
        {

            get
            {
                return _numero_doc_effettivi;
            }

            set
            {
                _numero_doc_effettivi = value;
            }
        }

        public virtual Int32 Numero_doc_copie
        {

            get
            {
                return _numero_doc_copie;
            }

            set
            {
                _numero_doc_copie = value;
            }
        }

        #endregion

        #region Methods

        public ARCHIVE_TransferForSearch()
        {

        }

        public ARCHIVE_TransferForSearch(Int32 system_id, Int32 iD_Amministrazione, String description, String stato, DateTime dateTime,
                                    Int32 numero_doc_effettivi, Int32 numero_doc_copie)
        {
            System_id = system_id;
            ID_Amministrazione = iD_Amministrazione;
            Description = description;
            Stato = stato;
            DateTime = dateTime;
            Numero_doc_effettivi = numero_doc_effettivi;
            Numero_doc_copie = numero_doc_copie;
        }
        #endregion
    }
}
