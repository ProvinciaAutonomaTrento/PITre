using System;
using System.Collections.Generic;
using System.Text;

namespace DocsPaDocumentale_DOCUMENTUM.DocsPaObjectTypes
{
    /// <summary>
    /// 
    /// </summary>
    public class TypeDocumentoStampaRegistro
    {
        protected TypeDocumentoStampaRegistro()
        { }

        public const string DOC_NUMBER = ObjectTypes.APPLICATION_PREFIX + "doc_number";
        public const string NUMERO_VERSIONE = ObjectTypes.APPLICATION_PREFIX + "num_versione";
        public const string CHECKOUT_LOCAL_FILE_PATH = ObjectTypes.APPLICATION_PREFIX + "locked_filepath";
        public const string CHECKOUT_MACHINE_NAME = ObjectTypes.APPLICATION_PREFIX + "locked_file_machinename";
    }
}
