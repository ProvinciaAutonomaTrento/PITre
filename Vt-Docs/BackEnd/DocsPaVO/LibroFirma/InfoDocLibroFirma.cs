using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.LibroFirma
{
    public class InfoDocLibroFirma
    {
        #region private fields

        private string _docnumber;
        private string _oggetto;
        private string _dataCreazione;
        private string _dataProtocollo;
        private string _num_proto;
        private string _tipoProto;
        private string _tipologiaDocumento;
        private string _id_documento_principale;
        private string _oggetto_documento_principale;
        private string _versionId;
        private string _destinatario;
        private int _numAllegato;
        private int _numVersione;
        private string _idRegistro;

        #endregion

        #region public property

        public string Docnumber
        {
            get
            {
                return _docnumber;
            }
            set
            {
                _docnumber = value;
            }
        }

        public string Oggetto
        {
            get
            {
                return _oggetto;
            }
            set
            {
                _oggetto = value;
            }
        }

        public string DataCreazione
        {
            get
            {
                return _dataCreazione;
            }
            set
            {
                _dataCreazione = value;
            }
        }

        public string DataProtocollo
        {
            get
            {
                return _dataProtocollo;
            }
            set
            {
                _dataProtocollo = value;
            }
        }

        public string NumProto
        {
            get
            {
                return _num_proto;
            }
            set
            {
                _num_proto = value;
            }
        }

        public string TipoProto
        {
            get
            {
                return _tipoProto;
            }
            set
            {
                _tipoProto = value;
            }
        }

        public string TipologiaDocumento
        {
            get
            {
                return _tipologiaDocumento;
            }
            set
            {
                _tipologiaDocumento = value;
            }
        }

        public string IdDocumentoPrincipale
        {
            get
            {
                return _id_documento_principale;
            }
            set
            {
                _id_documento_principale = value;
            }
        }

        public string OggettoDocumentoPrincipale
        {
            get
            {
                return _oggetto_documento_principale;
            }
            set
            {
                _oggetto_documento_principale = value;
            }
        }

        public string VersionId
        {
            get
            {
                return _versionId;
            }
            set
            {
                _versionId = value;
            }
        }

        public string Destinatario
        {
            get
            {
                return _destinatario;
            }
            set
            {
                _destinatario = value;
            }
        }

        public int NumAllegato
        {
            get
            {
                return _numAllegato;
            }
            set
            {
                _numAllegato = value;
            }
        }

        public int NumVersione
        {
            get
            {
                return _numVersione;
            }
            set
            {
                _numVersione = value;
            }
        }

        public string IdRegistro
        {
            get
            {
                return _idRegistro;
            }

            set
            {
                _idRegistro = value;
            }
        }
        #endregion
    }
}
