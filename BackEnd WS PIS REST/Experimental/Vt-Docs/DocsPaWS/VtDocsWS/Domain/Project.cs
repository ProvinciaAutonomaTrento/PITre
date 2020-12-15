using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Domain
{
    /// <summary>
    /// Fascicolo
    /// </summary>
    [DataContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public class Project
    {
        /// <summary>
        /// System id del fascicolo
        /// </summary>
        [DataMember]
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Codice del fascicolo
        /// </summary>
        [DataMember]
        public string Code
        {
            get;
            set;
        }


        /// <summary>
        /// Descrizione del fascicolo
        /// </summary>
        [DataMember]
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se il fascicolo è privato
        /// </summary>
        [DataMember]
        public bool Private
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se il fascicolo è cartaceo
        /// </summary>
        [DataMember]
        public bool Paper
        {
            get;
            set;
        }

        /// <summary>
        /// Indica la data di collocazione
        /// </summary>
        [DataMember]
        public string CollocationDate
        {
            get;
            set;
        }

        /// <summary>
        /// Indica la collocazione fisica del fascicolo
        /// </summary>
        [DataMember]
        public string PhysicsCollocation
        {
            get;
            set;
        }

        /// <summary>
        /// Indica la data di creazione del fascicolo
        /// </summary>
        [DataMember]
        public string CreationDate
        {
            get;
            set;
        }

        /// <summary>
        /// Indica la data di apertura del fascicolo
        /// </summary>
        //[DataMember]
        //public string OpeningDate
        //{
        //    get;
        //    set;
        //}

        /// <summary>
        /// Indica la data di chiusura
        /// </summary>
        [DataMember]
        public string ClosureDate
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se il fascicolo è aperto
        /// </summary>
        [DataMember]
        public bool Open
        {
            get;
            set;
        }

        /// <summary>
        /// Id del fascicolo padre
        /// </summary>
        [DataMember]
        public string IdParent
        {
            get;
            set;
        }

        /// <summary>
        /// Titolario in cui è classificato il fascicolo
        /// </summary>
        [DataMember]
        public ClassificationScheme ClassificationScheme
        {
            get;
            set;
        }

        /// <summary>
        /// Template del fascicolo
        /// </summary>
        [DataMember]
        public Template Template
        {
            get;
            set;
        }

        /// <summary>
        /// Note del fascicolo
        /// </summary>
        [DataMember]
        public Note[] Note
        {
            get;
            set;
        }

        /// <summary>
        /// Tipo del documento G Generale P Procedimentale
        /// </summary>
        [DataMember]
        public string Type
        {
            get;
            set;
        }

        /// <summary>
        /// Numero del fascicolo
        /// </summary>
        [DataMember]
        public string Number
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se il fascicolo è controllato
        /// </summary>
        [DataMember]
        public bool Controlled
        {
            get;
            set;
        }

        /// <summary>
        /// Codice del nodo generale in cui è presente il fascicolo
        /// </summary>
        [DataMember]
        public string CodeNodeClassification
        {
            get;
            set;
        }

        /// <summary>
        /// Registro
        /// </summary>
        [DataMember]
        public Domain.Register Register
        {
            get;
            set;
        }
    }
}