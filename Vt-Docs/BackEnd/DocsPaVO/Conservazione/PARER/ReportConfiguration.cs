using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.Conservazione.PARER
{
    public class ReportConfiguration
    {
        private String _idAmm;
        private String _subject;
        private String _body;
        private String[] _recipients;
        private String[] _fixed_recipients;
        private String[] _policy_recipients;
        private String _policy_subject;
        private String _policy_body;
        private Mailbox _mbox;


        public String idAmm 
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

        public String Subject
        {
            get
            {
                return _subject;
            }
            set
            {
                _subject = value;
            }
        }

        public String Body
        {
            get
            {
                return _body;
            }
            set
            {
                _body = value;
            }
        }

        public String[] Recipients
        {
            get
            {
                return _recipients;
            }
            set
            {
                _recipients = value;
            }
        }

        public String[] FixedRecipients
        {
            get
            {
                return _fixed_recipients;
            }
            set
            {
                _fixed_recipients = value;
            }
        }

        public String[] PolicyRecipients
        {
            get
            {
                return _policy_recipients;
            }
            set
            {
                _policy_recipients = value;
            }
        }

        public String PolicySubject
        {
            get
            {
                return _policy_subject;
            }
            set
            {
                _policy_subject = value;
            }
        }

        public String PolicyBody
        {
            get
            {
                return _policy_body;
            }
            set
            {
                _policy_body = value;
            }
        }

        public Mailbox MailBoxConfiguration
        {
            get
            {
                return _mbox;
            }
            set
            {
                _mbox = value;
            }
        }
    }
}
