using System;
using System.Collections.Generic;
using System.Text;

namespace DocsPaDocumentale_DOCUMENTUM.DocsPaObjectTypes
{
    /// <summary>
    /// 
    /// </summary>
    public class TypeSottofascicolo
    {
        /// <summary>
        /// 
        /// </summary>
        protected TypeSottofascicolo()
        { }

        /// <summary>
        /// Id del sottofascicolo
        /// </summary>
        public const string ID_DOCSPA = ObjectTypes.APPLICATION_PREFIX + "id_docspa";

        /// <summary>
        /// Id del fascicolo di appartenenza
        /// </summary>
        public const string ID_FASCICOLO = ObjectTypes.APPLICATION_PREFIX + "id_fascicolo";
    }
}
