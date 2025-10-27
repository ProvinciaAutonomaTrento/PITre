// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Runtime.Serialization;

namespace DocsPaVO.Conservazione.PARER
{
    [DataContract]
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

        [DataMember]
        public String Server
        {
            get { return _server; }
            set { _server = value; }
        }

        [DataMember]
        public String Username
        {
            get { return _username; }
            set { _username = value; }
        }

        [DataMember]
        public String Password
        {
            get { return _password; }
            set { _password = value; }
        }

        [DataMember]
        public String Port
        {
            get { return _port; }
            set { _port = value; }
        }

        [DataMember]
        public String From
        {
            get { return _mail_from; }
            set { _mail_from = value; }
        }

        [DataMember]
        public bool UseSSL
        {
            get { return _use_ssl; }
            set { _use_ssl = value; }
        }

        [DataMember]
        public String MailStruttura
        {
            get { return _recipients; }
            set {_recipients = value;}
        }

        [DataMember]
        public String MailPolicy
        {
            get { return _policy_recipients; }
            set { _policy_recipients = value; }
        }
    }
}
