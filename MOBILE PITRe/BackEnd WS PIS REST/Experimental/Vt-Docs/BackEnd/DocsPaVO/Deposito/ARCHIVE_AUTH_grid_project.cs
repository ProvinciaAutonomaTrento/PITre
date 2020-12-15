using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Deposito
{
    public partial class ARCHIVE_AUTH_grid_project
    {
        #region Fields

        private Int32 _System_ID;
        private String _Registro;
        private String _Codice;
        private String _Descrizione;
        private DateTime _DataApertura;
        private DateTime _DataChiusura;


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

        public virtual String Codice
        {

            get
            {
                return _Codice;
            }

            set
            {
                _Codice = value;
            }
        }

        public virtual String Descrizione
        {

            get
            {
                return _Descrizione;
            }

            set
            {
                _Descrizione = value;
            }
        }

        public virtual DateTime DataApertura
        {

            get
            {
                return _DataApertura;
            }

            set
            {
                _DataApertura = value;
            }
        }

        public virtual DateTime DataChiusura
        {

            get
            {
                return _DataChiusura;
            }

            set
            {
                _DataChiusura = value;
            }
        }

        #endregion

        #region Default Constructor

        public ARCHIVE_AUTH_grid_project()
        {
        }

        #endregion

        #region Constructors

        public ARCHIVE_AUTH_grid_project(Int32 system_ID, String registro, String codice, String descrizione, DateTime dataApertura, DateTime dataChiusura)
        {
            System_ID = system_ID;
            Registro = registro;
            Codice = codice;
            Descrizione = descrizione;
            DataApertura = dataApertura;
            DataChiusura = dataChiusura;
        }
        #endregion
    }
}
