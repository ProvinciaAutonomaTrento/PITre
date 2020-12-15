using System;
using System.Collections.Generic;
using System.Text;

namespace DocsPaDocumentale_DOCUMENTUM.DocsPaObjectTypes
{
    /// <summary>
    /// 
    /// </summary>
    public class TypeFascicoloProcedimentale : TypeFascicolo
    {
        protected TypeFascicoloProcedimentale()
        { }

        public const string STATO = ObjectTypes.APPLICATION_PREFIX + "stato";
        public const string PRIVATO = ObjectTypes.APPLICATION_PREFIX + "privato";
        public const string DATA_APERTURA = ObjectTypes.APPLICATION_PREFIX + "data_apertura";
        public const string DATA_CHIUSURA = ObjectTypes.APPLICATION_PREFIX + "data_chiusura";
        public const string DYNAMIC_TYPE = ObjectTypes.APPLICATION_PREFIX + "dyn_type";
        public const string DYNAMIC_FIELD_NAME = ObjectTypes.APPLICATION_PREFIX + "dyn_field_name";
        public const string DYNAMIC_FIELD_VALUE = ObjectTypes.APPLICATION_PREFIX + "dyn_field_value";
        public const string DYNAMIC_FIELD_INDEX = ObjectTypes.APPLICATION_PREFIX + "dyn_value_index";
    }
}