using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Interoperabilita
{
    public class InfoCheckMailbox
    {
        private string _idCheckMailbox;
        private string _userId;
        private string _roleId;
        private string _regId;
        private string _mail;
        private int _elaborate;
        private int _total;
        private string _concluded;

        public string IdCheckMailbox
        {
            get
            {
                return _idCheckMailbox;
            }
            set
            {
                _idCheckMailbox = value;
            }
        }
        public string UserID
        {
            get
            {
                return _userId;
            }
            set
            {
                _userId = value;
            }
        }

        public string RoleID
        {
            get
            {
                return _roleId;
            }
            set
            {
                _roleId = value;
            }
        }

        public string RegisterID
        {
            get
            {
                return _regId;
            }
            set
            {
                _regId = value;
            }
        }

        public string Mail
        {
            get
            {
                return _mail;
            }
            set
            {
                _mail = value;
            }
        }

        public int Elaborate
        {
            get
            {
                return _elaborate;
            }
            set
            {
                _elaborate = value;
            }
        }

        public int Total
        {
            get
            {
                return _total;
            }
            set
            {
                _total = value;
            }
        }

        public string Concluded
        {
            get
            {
                return _concluded;
            }
            set
            {
                _concluded = value;
            }
        }
    }
}
