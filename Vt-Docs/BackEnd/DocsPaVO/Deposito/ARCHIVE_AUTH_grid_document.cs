using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Deposito
{
    public partial class ARCHIVE_AUTH_grid_document
    {
        #region Fields

        private Int32 _System_ID;
        private String _Registro;
        private String _Tipo;
        private String _ID_Protocollo;
        private DateTime _Data;
        private String _Oggetto;
        private String _Mittente_Destinatario;

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

        public virtual String Registro
        {

            get
            {
                return _Registro;
            }

            set
            {
                _Registro = value;
            }
        }

        public virtual String Tipo
        {

            get
            {
                return _Tipo;
            }

            set
            {
                _Tipo = value;
            }
        }

        public virtual String ID_Protocollo
        {

            get
            {
                return _ID_Protocollo;
            }

            set
            {
                _ID_Protocollo = value;
            }
        }

        public virtual DateTime Data
        {

            get
            {
                return _Data;
            }

            set
            {
                _Data = value;
            }
        }

        public virtual String Oggetto
        {

            get
            {
                return _Oggetto;
            }

            set
            {
                _Oggetto = value;
            }
        }

        public virtual String Mittente_Destinatario
        {

            get
            {
                return _Mittente_Destinatario;
            }

            set
            {
                _Mittente_Destinatario = value;
            }
        }

        #endregion

        #region Default Constructor

        public ARCHIVE_AUTH_grid_document()
        {
        }

        #endregion

        #region Constructors

        public ARCHIVE_AUTH_grid_document(Int32 system_ID, String registro, String tipo, String iD_Protocollo, 
                                            DateTime data, String oggetto, String mittente_Destinatario)
        {
            System_ID = system_ID;
            Registro = registro;
            Tipo = tipo;
            ID_Protocollo = iD_Protocollo;
            Data = data;
            Oggetto = oggetto;
            Mittente_Destinatario = mittente_Destinatario;

        }
        #endregion

    }
}
