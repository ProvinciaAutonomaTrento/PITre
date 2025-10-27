// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DocsPaVO.utente
{
    /// <summary>
	/// </summary>
    [Serializable]
    [DataContract]
	public class MailCorrispondente
    {
        [DataMember]
        public string systemId { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string Note { get; set; }
        [DataMember]
        public string Principale { get; set; }

        public override bool Equals(object obj)
        {
            return obj is MailCorrispondente && ((MailCorrispondente)obj).Email == this.Email;
        }
        //aggiunto GetHashCode perchè se no equals si arrabbia
        public override int GetHashCode()
        {
            return this.Email.GetHashCode();
        }
    }
}
