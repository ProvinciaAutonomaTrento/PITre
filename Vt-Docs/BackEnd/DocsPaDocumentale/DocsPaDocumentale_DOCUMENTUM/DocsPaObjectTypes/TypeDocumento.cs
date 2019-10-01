using System;
using System.Collections.Generic;
using System.Text;

namespace DocsPaDocumentale_DOCUMENTUM.DocsPaObjectTypes
{
    /// <summary>
    /// 
    /// </summary>
    public class TypeDocumento
    {
        protected TypeDocumento()
        { }

        public const string DOC_NUMBER = ObjectTypes.APPLICATION_PREFIX + "doc_number";
        public const string NUMERO_VERSIONE = ObjectTypes.APPLICATION_PREFIX + "num_versione";
        public const string OGGETTO = ObjectTypes.APPLICATION_PREFIX + "oggetto";
        public const string TIPO_ATTO = ObjectTypes.APPLICATION_PREFIX + "tipo_atto";
        public const string DA_PROTOCOLLARE = ObjectTypes.APPLICATION_PREFIX + "da_protocollare";
        public const string TIPO_PROTOCOLLO = ObjectTypes.APPLICATION_PREFIX + "tipo_proto";
        public const string RUOLO_CREATORE = ObjectTypes.APPLICATION_PREFIX + "ruolo_creatore";
        public const string DATA_PROTOCOLLO = ObjectTypes.APPLICATION_PREFIX + "data_proto";
        public const string ANNO_PROTOCOLLO = ObjectTypes.APPLICATION_PREFIX + "anno";
        public const string UTENTE_PROTOCOLLATORE = ObjectTypes.APPLICATION_PREFIX + "protocollatore_user";
        public const string DESCRIZIONE_UTENTE_PROTOCOLLATORE = ObjectTypes.APPLICATION_PREFIX + "protocollatore_desc";
        public const string RUOLO_PROTOCOLLATORE = ObjectTypes.APPLICATION_PREFIX + "ruolo_proto";
        public const string SEGNATURA = ObjectTypes.APPLICATION_PREFIX + "segnatura";
        public const string CODICE_REGISTRO = ObjectTypes.APPLICATION_PREFIX + "codice_reg";
        public const string MITTENTE = ObjectTypes.APPLICATION_PREFIX + "mittente";
        public const string DESTINATARI = ObjectTypes.APPLICATION_PREFIX + "destinatari";
        public const string DESTINATARI_CC = ObjectTypes.APPLICATION_PREFIX + "destinatari_cc";
        public const string NOTE = ObjectTypes.APPLICATION_PREFIX + "note";
        public const string DATA_ANNULLAMENTO_PROTOCOLLO = ObjectTypes.APPLICATION_PREFIX + "data_annullamento";
        public const string MEZZO_SPEDIZIONE = ObjectTypes.APPLICATION_PREFIX + "mezzo_spedizione";
        public const string DYNAMIC_TYPE = ObjectTypes.APPLICATION_PREFIX + "dyn_type";
        public const string DYNAMIC_FIELD_NAME = ObjectTypes.APPLICATION_PREFIX + "dyn_field_name";
        public const string DYNAMIC_FIELD_VALUE = ObjectTypes.APPLICATION_PREFIX + "dyn_field_value";
        public const string DYNAMIC_FIELD_INDEX = ObjectTypes.APPLICATION_PREFIX + "dyn_value_index";
        public const string PREDISPONI_PROTOCOLLAZIONE = ObjectTypes.APPLICATION_PREFIX + "predisp_protocollazione";
        public const string NOTE_ANNULLAMENTO_PROTOCOLLO = ObjectTypes.APPLICATION_PREFIX + "note_annullamento";
        public const string PRIVATO = ObjectTypes.APPLICATION_PREFIX + "privato";
        public const string PERSONALE = ObjectTypes.APPLICATION_PREFIX + "personale";
        public const string NUMERO_PROTOCOLLO = ObjectTypes.APPLICATION_PREFIX + "num_proto";
        public const string PROTOCOLLO_PRECEDENTE = ObjectTypes.APPLICATION_PREFIX + "proto_precedente";
        public const string CHECKOUT_LOCAL_FILE_PATH = ObjectTypes.APPLICATION_PREFIX + "locked_filepath";
        public const string CHECKOUT_MACHINE_NAME = ObjectTypes.APPLICATION_PREFIX + "locked_file_machinename";
        public const string ID_DOCUMENTO_PRINCIPALE = ObjectTypes.APPLICATION_PREFIX + "id_documento_principale";
    }
}
