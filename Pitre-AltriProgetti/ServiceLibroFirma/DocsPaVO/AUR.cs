using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO
{
    /// <summary>
    /// Questa classe rappresenta le asserzioni positive definite per le coppie (ruolo,utente) - tipo evento o utente - tipo evento 
    /// </summary>
    class AUR
    {
        private int _idAdm;
        private int _idRole;
        private int _idUser;
        private int _idRoleType;
        private int _idRF;
        private int _idUO;
        private int _idAOO;
        private char _typeNotify;

        public int IDAMM
        {
            set
            {
                _idAdm = value;
            }
            get
            {
                return _idAdm;
            }
        }

        public int IDROLE
        {
            set
            {
                _idRole = value;
            }
            get
            {
                return _idRole;
            }
        }

        public int IDUSER
        {
            set
            {
                _idUser= value;
            }
            get
            {
                return _idUser;
            }
        }

        public int IDROLETYPE
        {
            set
            {
                _idRoleType = value;
            }
            get
            {
                return _idRoleType;
            }
        }

        public int IDRF
        {
            set
            {
                _idRF = value;
            }
            get
            {
                return _idRF;
            }
        }

        public int IDUO
        {
            set
            {
                _idUO = value;
            }
            get
            {
                return _idUO;
            }
        }

        public int IDAOO
        {
            set
            {
                _idAOO = value;
            }
            get
            {
                return _idAOO;
            }
        }

        public char TYPENOTIFY
        {
            set
            {
                _typeNotify = value;
            }
            get
            {
                return _typeNotify;
            }
        }
    }
}
