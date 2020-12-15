using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Conservazione.PARER
{
    public class Mailbox
    {
        private String _server;
        private String _username;
        private String _password;
        private String _port;
        private String _mail_from;
        private bool _use_ssl;
        private String _recipients;
        private String _policy_recipients;

        public String Server
        {
            get { return _server; }
            set { _server = value; }
        }

        public String Username
        {
            get { return _username; }
            set { _username = value; }
        }

        public String Password
        {
            get { return _password; }
            set { _password = value; }
        }

        public String Port
        {
            get { return _port; }
            set { _port = value; }
        }

        public String From
        {
            get { return _mail_from; }
            set { _mail_from = value; }
        }

        public bool UseSSL
        {
            get { return _use_ssl; }
            set { _use_ssl = value; }
        }

        public String MailStruttura
        {
            get { return _recipients; }
            set {_recipients = value;}
        }

        public String MailPolicy
        {
            get { return _policy_recipients; }
            set { _policy_recipients = value; }
        }
    }
}
