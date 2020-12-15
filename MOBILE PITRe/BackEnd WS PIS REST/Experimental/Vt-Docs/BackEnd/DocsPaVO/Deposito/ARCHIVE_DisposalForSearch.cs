using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Deposito
{
    public partial class ARCHIVE_DisposalForSearch
    {
        #region fields

        private Int32 _System_id;
        private Int32 _ID_Amministrazione;
        private String _Description;
        private String _Stato;
        private DateTime _DateTime;
        private Int32 _numero_doc_scartati;
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

        public virtual Int32 Numero_doc_scartati
        {

            get
            {
                return _numero_doc_scartati;
            }

            set
            {
                _numero_doc_scartati = value;
            }
        }



        #endregion

        #region Methods

        public ARCHIVE_DisposalForSearch()
        {

        }

        public ARCHIVE_DisposalForSearch(Int32 system_id, Int32 iD_Amministrazione, String description, String stato, DateTime dateTime,
                                    Int32 numero_doc_scartati)
        {
            System_id = system_id;
            ID_Amministrazione = iD_Amministrazione;
            Description = description;
            Stato = stato;
            DateTime = dateTime;
            Numero_doc_scartati = numero_doc_scartati;
        }
        #endregion

    }
}
