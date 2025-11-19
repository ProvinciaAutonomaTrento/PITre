// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class PeopleEntity
    {
        public long SYSTEM_ID { get; set; }
        public string? USER_ID { get; set; }
        public string? FULL_NAME { get; set; }
        public string? DISABLED { get; set; }
        public string? USER_PASSWORD { get; set; }
        public string? USER_LOCATION { get; set; }
        public string? PHONE { get; set; }
        public string? EXTENSION { get; set; }
        public DateTime? LAST_LOGIN_DATE { get; set; }
        public DateTime? LAST_LOGIN_TIME { get; set; }
        public string? ALLOW_LOGIN { get; set; }
        public string? FAX { get; set; }
        public string? DID { get; set; }
        public string? TARGET_DOCSRVR { get; set; }
        public string? PRIMARY_GROUP { get; set; }
        public string? PRIMARY_LIB { get; set; }
        public long? PROFILE_DEFAULTS { get; set; }
        public string? CONNECT_BRIDGED { get; set; }
        public string? BUTTON_BAR { get; set; }
        public string? NETWORK_ID { get; set; }
        public string? ACL_DEFAULTS { get; set; }
        public string? SHOW_RESTORED { get; set; }
        public DateTime? PASS_EXP_DATE { get; set; }
        public string? LOGINS_REMAINING { get; set; }
        public string? PSWORD_VALID_FOR { get; set; }
        public string? NO_EXP_DATE { get; set; }
        public string? DR_USER { get; set; }
        public string? SEARCH_FORM_ID { get; set; }
        public string? EMAIL_ADDRESS { get; set; }
        public string? CHA_AMMINISTRATORE { get; set; }
        public string? CHA_NOTIFICA { get; set; }
        public string? VAR_TELEFONO { get; set; }
        public long? ID_AMM { get; set; }
        public string? CHA_RESP_ASS { get; set; }
        public string? CHA_ASSEGNATARIO { get; set; }
        public string? VAR_COGNOME { get; set; }
        public string? VAR_NOME { get; set; }
        public string? CHA_NOTIFICA_CON_ALLEGATO { get; set; }
        public string? VAR_SEDE { get; set; }
        public string? ENCRYPTED_PASSWORD { get; set; }
        public DateTime? PASSWORD_CREATION_DATE { get; set; }
        public string? PASSWORD_NEVER_EXPIRE { get; set; }
        public string? FROM_EMAIL_ADDRESS { get; set; }
        public string? LDAP_NEVER_SYNC { get; set; }
        public string? LDAP_ID_SYNC { get; set; }
        public long? ID_CLIENT_MODEL_PROCESSOR { get; set; }
        public string? CHA_SUPERADMIN { get; set; }
        public string? FACTORY_USER { get; set; }
        public string? IS_ENABLED_SMART_CLIENT { get; set; }
        public string? SMART_CLIENT_PDF_CONV_ON_SCAN { get; set; }
        public string? LDAP_AUTHENTICATED { get; set; }
        public string? ACCETTAZIONE_DISSERV { get; set; }
        public long? ID_DISPOSITIVO_STAMPA { get; set; }
        public string? ABILITATO_CENTRO_SERVIZI { get; set; }
        public string? MATRICOLA { get; set; }
        public long? ABILITATO_CHIAVI_CONFIG { get; set; }
        public string CHA_TIPO_COMPONENTI { get; set; }
        public string? CHA_SYSTEM_USER { get; set; }
        public string? VAR_MEMENTO { get; set; }
        public string? ABILITATO_ESIBIZIONE { get; set; }
        public string? CHA_TIPO_FIRMA { get; set; }
        public string? CHA_AUTOMATICO { get; set; }
        public string? CHA_DI_SISTEMA { get; set; }
        public string? VAR_CODICE_FISCALE { get; set; }
    }
}
