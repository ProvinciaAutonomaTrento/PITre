using System;
using DocsPaVO.utente;
using DocsPaVO.documento;

namespace DocsPaVO.CheckInOut
{
    /// <summary>
    /// Gestione delle informazioni relative ad un documento con stato CheckOut.
    /// </summary>
    /// <remarks>
    /// Le informazioni di stato su un documento CheckOut possono essere reperite 
    /// solamente da un utente amministratore e dall'utente stesso che ha 
    /// bloccato il documento.
    /// </remarks>
    [Serializable()]
    public class CheckOutStatus
    {
        private string _id = string.Empty;
        private DateTime _checkOutDate = DateTime.Now;
        private string _documentLocation = string.Empty;
        private string _machineName = string.Empty;

        private string _idUser = string.Empty;
        private string _idRole = string.Empty;
        private string _userName = string.Empty;
        private string _role = string.Empty;

        private string _idDocument = string.Empty;
        private string _documentNumber = string.Empty;
        private string _segnature = string.Empty;

        private bool _inConversionePdf = false;

        private bool _isAllegato = false;

        /// <summary>
        /// 
        /// </summary>
        public string ID
        {
            get
            {
                return this._id;
            }
            set
            {
                this._id = value;
            }
        }

        /// <summary>
        /// Data in cui è stato fatto il checkout del documento
        /// </summary>
        public DateTime CheckOutDate
        {
            get
            {
                return this._checkOutDate;
            }
            set
            {
                this._checkOutDate = value;
            }
        }

        /// <summary>
        /// Percorso del file per cui è stato fatto il checkout
        /// </summary>
        public string DocumentLocation
        {
            get
            {
                return this._documentLocation;
            }
            set
            {
                this._documentLocation = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string MachineName
        {
            get
            {
                return this._machineName;
            }
            set
            {
                this._machineName = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string IDUser
        {
            get
            {
                return this._idUser;
            }
            set
            {
                this._idUser = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string UserName
        {
            get
            {
                return this._userName;
            }
            set
            {
                this._userName = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string IDRole
        {
            get
            {
                return this._idRole;
            }
            set
            {
                this._idRole = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string RoleName
        {
            get
            {
                return this._role;
            }
            set
            {
                this._role = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string IDDocument
        {
            get
            {
                return this._idDocument;
            }
            set
            {
                this._idDocument = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string DocumentNumber
        {
            get
            {
                return this._documentNumber;
            }
            set
            {
                this._documentNumber = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Segnature
        {
            get
            {
                return this._segnature;
            }
            set
            {
                this._segnature = value;
            }
        }

        /// <summary>
        /// Se true, indica che il documento è in CheckOut per una conversione PDF
        /// </summary>
        public bool InConversionePdf
        {
            get
            {
                return this._inConversionePdf;
            }
            set
            {
                this._inConversionePdf = value;
            }
        }

        /// <summary>
        /// Se true, indica che il documento è un allegato
        /// </summary>
        public bool IsAllegato
        {
            get
            {
                return this._isAllegato;
            }
            set
            {
                this._isAllegato = value;
            }
        }

        public CheckOutStatus()
        {
        }

        public CheckOutStatus(string id)
        {
            this._id = id;
        }

        public CheckOutStatus(string id, DateTime checkOutDate)
            : this(id)
        {
            this._checkOutDate = checkOutDate;
        }

        public CheckOutStatus(string id, DateTime checkOutDate, string documentLocation)
            : this(id, checkOutDate)
        {
            this._documentLocation = documentLocation;
        }

        public CheckOutStatus(string id, DateTime checkOutDate, string documentLocation, string machineName)
            : this(id, checkOutDate, documentLocation)
        {
            this._machineName = machineName;
        }
    }
}