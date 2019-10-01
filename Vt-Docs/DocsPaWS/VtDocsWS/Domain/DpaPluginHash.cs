using DocsPaVO.utente;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace VtDocsWS.Domain
{
    public class DpaPluginHash
    {
        [DataMember]
        public DateTime DataElaborazione
        {
            get;
            set;
        }

        [DataMember]
        public string HashFile
        {
            get;
            set;
        }

        [DataMember]
        public Utente Utente
        {
            get;
            set;
        }

        [DataMember]
        public string IdProfile
        {
            get;
            set;
        }

        [DataMember]
        public string SystemId
        {
            get;
            set;
        }

        [DataMember]
        public string AccessRight
        {
            get;
            set;
        }
    }
}