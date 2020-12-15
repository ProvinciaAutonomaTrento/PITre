using System;
using System.Collections.Generic;
using System.Text;

namespace DocsPaDocumentale_DOCUMENTUM.DocsPaObjectTypes
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ObjectTypes
    {
        /// <summary>
        /// 
        /// </summary>
        private ObjectTypes()
        { }

        public const string APPLICATION_PREFIX = "p3_";

        public const string DOCUMENTO = APPLICATION_PREFIX + "document";
        public const string DOCUMENTO_STAMPA_REGISTRO = APPLICATION_PREFIX + "stampa_registro";
        public const string TITOLARIO = APPLICATION_PREFIX + "titolario";
        public const string NODO_TITOLARIO = APPLICATION_PREFIX + "ndc";
        public const string FASCICOLO = APPLICATION_PREFIX + "fascicolo";
        public const string FASCICOLO_GENERALE = APPLICATION_PREFIX + "fascicolo_gen";
        public const string FASCICOLO_PROCEDIMENTALE = APPLICATION_PREFIX + "fascicolo_proc";
        public const string SOTTOFASCICOLO = APPLICATION_PREFIX + "sottofascicolo";
        public const string UTENTE = "dm_user";
        public const string GRUPPO = "dm_group";
    }
}
