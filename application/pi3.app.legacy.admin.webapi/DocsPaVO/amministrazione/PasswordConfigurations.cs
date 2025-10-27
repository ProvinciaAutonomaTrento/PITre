// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Runtime.Serialization;

namespace DocsPaVO.amministrazione
{
    /// <summary>
    /// Gestione dati relativi alle configurazioni delle password per l'amministrazione
    /// </summary>
    [Serializable()]
    [DataContract]
    public class PasswordConfigurations
    {
        /// <summary>
        /// 
        /// </summary>
        public PasswordConfigurations()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        public PasswordConfigurations(IDataReader reader)
        {
            if (!reader.IsDBNull(reader.GetOrdinal("SYSTEM_ID")))
                this.IdAmministrazione = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("SYSTEM_ID")));

            if (!reader.IsDBNull(reader.GetOrdinal("ENABLE_PASSWORD_EXPIRATION")))
                this.ExpirationEnabled = (Convert.ToInt16(reader.GetValue(reader.GetOrdinal("ENABLE_PASSWORD_EXPIRATION"))) > 0);

            if (!reader.IsDBNull(reader.GetOrdinal("PASSWORD_EXPIRATION_DAYS")))
                this.ValidityDays = reader.GetInt32(reader.GetOrdinal("PASSWORD_EXPIRATION_DAYS"));

            if (!reader.IsDBNull(reader.GetOrdinal("PASSWORD_MIN_LENGTH")))
                this.MinLength = reader.GetInt32(reader.GetOrdinal("PASSWORD_MIN_LENGTH"));

            if (!reader.IsDBNull(reader.GetOrdinal("PASSWORD_SPECIAL_CHAR_LIST")))
                this.SpecialCharacters = reader.GetString(reader.GetOrdinal("PASSWORD_SPECIAL_CHAR_LIST")).ToCharArray();
        }

        [DataMember]
        public int IdAmministrazione { get; set; } = 0;
        [DataMember]
        public bool ExpirationEnabled { get; set; } = false;
        [DataMember]
        public int ValidityDays { get; set; } = 0;
        [DataMember]
        public int MinLength { get; set; } = 0;
        [DataMember]
        public char[] SpecialCharacters { get; set; } = new char[0];
    }
}
