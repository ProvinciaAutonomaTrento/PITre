using System;
using System.Collections.Generic;
using System.Text;

namespace DocsPaDocumentale_OCS.DocsPaObjectType
{
   public sealed class ObjectTypes
    {
        /// <summary>
        /// 
        /// </summary>
        private ObjectTypes()
        { }

       //categorie
        public const string CATEGOTY_PROTOCOLLO = "DPA_PROTOCOLLO";
        public const string CATEGOTY_STAMPA_REGISTRO = "DPA_STAMPA_REGISTRO";
        public const string CATEGOTY_ALLEGATO = "DPA_ALLEGATO";
        public const string CATEGOTY_VERSIONE = "DPA_VERSIONE";

        // gestione diritti di accesso
        public const string ROLE_READER = "Reader";
        public const string ROLE_ADMINISTRATOR = "Administrator";
        public const string ROLE_AUTHOR = "Author";
    }
}

