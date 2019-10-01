using System;

namespace NttDataWA.Utils
{
    /// <summary>
    /// Questa classe riporta le informazioni relative ad un documento M/Text
    /// </summary>
    [Serializable()]
    public class MTextDocumentInfo : ICloneable
    {
        /// <summary>
        /// Id della versione del documento VT-Docs cui si riferisce l'FQN
        /// </summary>
        public String DocumentVersionId { get; set; }

        /// <summary>
        /// Numero del documento VT-Docs cui si riferisci l'FQN
        /// </summary>
        public String DocumentDocNumber { get; set; }

        /// <summary>
        /// Full Qualified Name del documento M/Text
        /// </summary>
        public String FullQualifiedName { get; set; }

        /// <summary>
        /// Funzione per la clonazione di un oggetto MTextDocInfo
        /// </summary>
        /// <returns>Clone di questo oggetto</returns>
        public object Clone()
        {
            MTextDocumentInfo cloned = new MTextDocumentInfo();
            cloned.DocumentDocNumber = this.DocumentDocNumber;
            cloned.DocumentVersionId = this.DocumentVersionId;
            cloned.FullQualifiedName = this.FullQualifiedName;

            return cloned;
        }
    }
}