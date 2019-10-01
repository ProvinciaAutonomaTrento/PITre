using System;
using System.Collections.Generic;
using System.Text;

namespace DocsPaDocumentale_DOCUMENTUM.DocsPaObjectTypes
{
    /// <summary>
    /// 
    /// </summary>
    public class TypeNodoTitolario
    {
        protected TypeNodoTitolario()
        { }

        public const string CODICE_REGISTRO = ObjectTypes.APPLICATION_PREFIX + "cod_registro";
        public const string LIVELLO = ObjectTypes.APPLICATION_PREFIX + "livello";
        public const string MESI_CONSERVAZIONE = ObjectTypes.APPLICATION_PREFIX + "tempo_conservazione";
        public const string ID_DOCSPA = ObjectTypes.APPLICATION_PREFIX + "id_docspa";
        public const string ID_CLASSIFICA = ObjectTypes.APPLICATION_PREFIX + "id_class";
    }
}