using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Domain
{
    public class Project
    {
        /// <summary>
        /// System id del fascicolo
        /// </summary>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Codice del fascicolo
        /// </summary>
        public string Code
        {
            get;
            set;
        }


        /// <summary>
        /// Descrizione del fascicolo
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se il fascicolo è privato
        /// </summary>
        public bool Private
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se il fascicolo è cartaceo
        /// </summary>
        public bool Paper
        {
            get;
            set;
        }

        /// <summary>
        /// Indica la data di collocazione
        /// </summary>
        public string CollocationDate
        {
            get;
            set;
        }

        /// <summary>
        /// Indica la collocazione fisica del fascicolo
        /// </summary>
        public string PhysicsCollocation
        {
            get;
            set;
        }

        /// <summary>
        /// Indica la data di creazione del fascicolo
        /// </summary>
        public string CreationDate
        {
            get;
            set;
        }

        /// <summary>
        /// Indica la data di apertura del fascicolo
        /// </summary>
        public string OpeningDate
        {
            get;
            set;
        }

        /// <summary>
        /// Indica la data di chiusura
        /// </summary>
        public string ClosureDate
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se il fascicolo è aperto
        /// </summary>
        public bool Open
        {
            get;
            set;
        }

        /// <summary>
        /// Id del fascicolo padre
        /// </summary>
        public string IdParent
        {
            get;
            set;
        }

        /// <summary>
        /// Titolario in cui è classificato il fascicolo
        /// </summary>
        public ClassificationScheme ClassificationScheme
        {
            get;
            set;
        }

        /// <summary>
        /// Template del fascicolo
        /// </summary>
        public Template Template
        {
            get;
            set;
        }

        /// <summary>
        /// Note del fascicolo
        /// </summary>
        public Note[] Note
        {
            get;
            set;
        }

        /// <summary>
        /// Tipo del documento G Generale P Procedimentale
        /// </summary>
        public string Type
        {
            get;
            set;
        }

        /// <summary>
        /// Numero del fascicolo
        /// </summary>
        public string Number
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se il fascicolo è controllato
        /// </summary>
        public bool Controlled
        {
            get;
            set;
        }

        /// <summary>
        /// Codice del nodo generale in cui è presente il fascicolo
        /// </summary>
        public string CodeNodeClassification
        {
            get;
            set;
        }

        /// <summary>
        /// Registro
        /// </summary>
        public Domain.Register Register
        {
            get;
            set;
        }
    }
}