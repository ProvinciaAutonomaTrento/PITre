using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.documento
{
    public class DocumentoVisualizzato
    {
        private string _systemId;
        private string _idPeople;
        private string _idGruppo;
        private string _idAmm;

        private string _idProfile;
        private string _segnatura;
        private string _oggetto;

        public string System_id
        {
            get
            {
                return _systemId;
            }
            set
            {
                _systemId = value;
            }
        }

        public string IdPeople
        {
            get
            {
                return _idPeople;
            }
            set
            {
                _idPeople = value;
            }
        }

        public string IdGruppo
        {
            get
            {
                return _idGruppo;
            }
            set
            {
                _idGruppo = value;
            }
        }

        public string IdAmm
        {
            get
            {
                return _idAmm;
            }
            set
            {
                _idAmm = value;
            }
        }

        public string IdProfile
        {
            get
            {
                return _idProfile;
            }
            set
            {
                _idProfile = value;
            }
        }

        public string Segnatura
        {
            get
            {
                return _segnatura;
            }
            set
            {
                _segnatura = value;
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
    }
}
